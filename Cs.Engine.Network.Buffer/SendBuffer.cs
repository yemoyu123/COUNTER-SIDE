using System;
using System.Collections.Generic;
using System.IO;
using Cs.Engine.Network.Buffer.Detail;

namespace Cs.Engine.Network.Buffer;

public sealed class SendBuffer
{
	private readonly byte[] headBuffer = new byte[4096];

	private readonly LinkedList<TailBuffer> tailBuffers = new LinkedList<TailBuffer>();

	private int headOffset;

	public byte[] Data => headBuffer;

	public int HeadOffset => headOffset;

	public bool HasData
	{
		get
		{
			if (headOffset <= 0)
			{
				return tailBuffers.Count > 0;
			}
			return true;
		}
	}

	internal int TailCount => tailBuffers.Count;

	private int HeadReservedSize => headBuffer.Length - headOffset;

	public int CalcTotalSize()
	{
		int num = headOffset;
		foreach (TailBuffer tailBuffer in tailBuffers)
		{
			num += tailBuffer.Offset;
		}
		return num;
	}

	public BinaryWriter GetWriter()
	{
		return new BinaryWriter(new SendStream(this));
	}

	public void Consume(int size)
	{
		if (headOffset < size)
		{
			throw new ArgumentException($"this.offset:{headOffset} size:{size}");
		}
		headOffset -= size;
		if (headOffset > 0)
		{
			System.Buffer.BlockCopy(headBuffer, size, headBuffer, 0, headOffset);
		}
		TryConsumeTailBuffers();
	}

	public byte[] Flush()
	{
		byte[] array = new byte[CalcTotalSize()];
		int num = 0;
		System.Buffer.BlockCopy(headBuffer, 0, array, num, HeadOffset);
		num += HeadOffset;
		headOffset = 0;
		foreach (TailBuffer tailBuffer in tailBuffers)
		{
			System.Buffer.BlockCopy(tailBuffer.Data, 0, array, num, tailBuffer.Offset);
			num += tailBuffer.Offset;
			tailBuffer.ToRecycleBin();
		}
		tailBuffers.Clear();
		return array;
	}

	public void Absorb(ZeroCopyBuffer zeroCopyBuffer)
	{
		TailBuffer[] array = zeroCopyBuffer.Move();
		foreach (TailBuffer value in array)
		{
			tailBuffers.AddLast(value);
		}
		TryConsumeTailBuffers();
	}

	internal void Write(byte[] buffer, int offset, int count)
	{
		if (tailBuffers.Count == 0 && HeadReservedSize > 0)
		{
			int num = System.Math.Min(HeadReservedSize, count);
			System.Buffer.BlockCopy(buffer, offset, headBuffer, headOffset, num);
			headOffset += num;
			offset += num;
			count -= num;
		}
		if (count == 0)
		{
			return;
		}
		TailBuffer tailBuffer = null;
		if (tailBuffers.Count > 0)
		{
			tailBuffer = tailBuffers.Last.Value;
			if (!tailBuffer.IsFull)
			{
				int num2 = tailBuffer.AddData(buffer, offset, count);
				offset += num2;
				count -= num2;
			}
		}
		while (count > 0)
		{
			if (tailBuffer != null && !tailBuffer.IsFull)
			{
				throw new Exception($"memory offset error. lastBuffer.Offset:{tailBuffer.Offset}");
			}
			TailBuffer tailBuffer2 = TailBuffer.Create(buffer, offset, count);
			tailBuffers.AddLast(tailBuffer2);
			offset += tailBuffer2.Offset;
			count -= tailBuffer2.Offset;
		}
	}

	internal void Clear()
	{
		headOffset = 0;
		foreach (TailBuffer tailBuffer in tailBuffers)
		{
			tailBuffer.ToRecycleBin();
		}
		tailBuffers.Clear();
	}

	private void TryConsumeTailBuffers()
	{
		while (tailBuffers.Count > 0)
		{
			TailBuffer value = tailBuffers.First.Value;
			if (HeadReservedSize >= value.Offset)
			{
				System.Buffer.BlockCopy(value.Data, 0, headBuffer, headOffset, value.Offset);
				headOffset += value.Offset;
				value.ToRecycleBin();
				tailBuffers.RemoveFirst();
				continue;
			}
			break;
		}
	}
}
