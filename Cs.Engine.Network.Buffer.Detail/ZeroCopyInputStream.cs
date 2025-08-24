using System;
using System.IO;

namespace Cs.Engine.Network.Buffer.Detail;

internal sealed class ZeroCopyInputStream : Stream
{
	private readonly TailBuffer[] tailBuffers;

	private readonly int totalSize;

	private readonly int fixedOffset;

	private int index;

	private int offset;

	public override bool CanRead => IsReadable();

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public override long Length { get; }

	public override long Position
	{
		get
		{
			return CalcCurrentFullOffset() - fixedOffset;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public ZeroCopyInputStream(ZeroCopyBuffer buffer)
	{
		tailBuffers = buffer.GetView();
		totalSize = buffer.CalcTotalSize();
		Length = totalSize;
	}

	public ZeroCopyInputStream(ZeroCopyBuffer buffer, int fixedOffset)
	{
		tailBuffers = buffer.GetView();
		totalSize = buffer.CalcTotalSize();
		if (fixedOffset < 0 || fixedOffset > totalSize)
		{
			throw new Exception($"[ZeroCopyInputStream] invalid offset. fixed:{fixedOffset} length:{Length}");
		}
		this.fixedOffset = fixedOffset;
		Length = totalSize - this.fixedOffset;
		ResetOffset(this.fixedOffset);
	}

	public override void Flush()
	{
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = 0;
		while (CanRead)
		{
			TailBuffer tailBuffer = tailBuffers[index];
			if (this.offset > tailBuffer.Offset)
			{
				throw new Exception($"memory offset error. this.offset:{this.offset} buffer offset:{tailBuffer.Offset}");
			}
			if (this.offset == tailBuffer.Offset)
			{
				index++;
				this.offset = 0;
				continue;
			}
			int num2 = System.Math.Min(count - num, tailBuffer.Offset - this.offset);
			System.Buffer.BlockCopy(tailBuffer.Data, this.offset, buffer, offset + num, num2);
			this.offset += num2;
			num += num2;
			if (num > count)
			{
				throw new Exception($"memory offset error. transferred:{num} count:{count}");
			}
			if (num == count)
			{
				break;
			}
		}
		return num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		int num = 0;
		switch (origin)
		{
		case SeekOrigin.Begin:
			num = fixedOffset + (int)offset;
			break;
		case SeekOrigin.Current:
			num = CalcCurrentFullOffset() + (int)offset;
			break;
		case SeekOrigin.End:
			num = (int)(Length - offset);
			break;
		}
		ResetOffset(num);
		return num;
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	private void ResetOffset(int offset)
	{
		if (offset > totalSize)
		{
			throw new Exception($"[ZeroCopyInputStream] invalid offset:{offset} length:{totalSize}");
		}
		index = 0;
		this.offset = offset;
		for (int i = 0; i < tailBuffers.Length; i++)
		{
			TailBuffer tailBuffer = tailBuffers[i];
			if (this.offset < tailBuffer.Offset)
			{
				break;
			}
			index++;
			this.offset -= tailBuffer.Offset;
		}
		if (index >= tailBuffers.Length && this.offset > 0)
		{
			throw new Exception($"index:{index} buffers:{tailBuffers.Length} offset:{this.offset}");
		}
	}

	private bool IsReadable()
	{
		if (tailBuffers.Length == 0)
		{
			return false;
		}
		if (index >= tailBuffers.Length)
		{
			if (offset == 0)
			{
				return false;
			}
			throw new Exception($"invalid index:{index} #buffer:{tailBuffers.Length} offset:{offset}");
		}
		if (index >= tailBuffers.Length)
		{
			return offset < tailBuffers[index].Offset;
		}
		return true;
	}

	private int CalcCurrentFullOffset()
	{
		int num = offset;
		for (int i = 0; i < index; i++)
		{
			num += tailBuffers[i].Offset;
		}
		return num;
	}
}
