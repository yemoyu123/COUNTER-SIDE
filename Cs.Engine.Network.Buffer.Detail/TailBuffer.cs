using System;
using Cs.Engine.Core;

namespace Cs.Engine.Network.Buffer.Detail;

internal sealed class TailBuffer
{
	private const int BufferSize = 4096;

	private static SimpleObjectPool<TailBuffer> pool = new SimpleObjectPool<TailBuffer>(() => new TailBuffer());

	private readonly byte[] data = new byte[4096];

	private int size;

	public byte[] Data => data;

	public int Offset => size;

	public bool IsFull => data.Length == size;

	private TailBuffer()
	{
	}

	public static TailBuffer Create()
	{
		return pool.CreateInstance();
	}

	public static TailBuffer Create(byte[] data, int offset, int size)
	{
		TailBuffer tailBuffer = pool.CreateInstance();
		tailBuffer.FillData(data, offset, size);
		return tailBuffer;
	}

	public int AddData(byte[] data, int offset, int size)
	{
		int num = System.Math.Min(this.data.Length - this.size, size);
		System.Buffer.BlockCopy(data, offset, this.data, this.size, num);
		this.size += num;
		return num;
	}

	public void ToRecycleBin()
	{
		size = 0;
		pool.ToRecycleBin(this);
	}

	private void FillData(byte[] data, int offset, int size)
	{
		this.size = System.Math.Min(this.data.Length, size);
		System.Buffer.BlockCopy(data, offset, this.data, 0, this.size);
	}
}
