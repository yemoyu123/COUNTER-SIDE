using System;
using System.IO;
using Cs.Engine.Network.Buffer.Detail;

namespace Cs.Engine.Network.Buffer;

internal sealed class MemoryPipe : IDisposable
{
	private readonly ZeroCopyBuffer buffer = new ZeroCopyBuffer();

	private long offset;

	private bool disposed;

	public long Length => buffer.CalcTotalSize() - offset;

	public void Dispose()
	{
		if (!disposed)
		{
			disposed = true;
			buffer.Hold().Dispose();
		}
	}

	public Stream GetReadStream()
	{
		return new ZeroCopyInputStream(buffer, (int)offset);
	}

	public void Write(byte[] data, int offset, int count)
	{
		buffer.Write(data, offset, count);
	}

	public void Adavnce(long size)
	{
		long num = buffer.CalcTotalSize();
		if (size > num)
		{
			throw new ArgumentException($"[MemoryPipe] invalid advance size:{size} totalSize:{num}");
		}
		offset += size;
		while (buffer.SegmentCount > 1)
		{
			TailBuffer tailBuffer = buffer.Peek();
			if (offset < tailBuffer.Offset)
			{
				break;
			}
			offset -= tailBuffer.Offset;
			buffer.PopHeadSegment();
		}
	}
}
