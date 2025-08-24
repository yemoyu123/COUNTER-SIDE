using System;
using System.IO;

namespace Cs.Engine.Network.Buffer.Detail;

internal sealed class SendStream : Stream
{
	private readonly SendBuffer buffer;

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

	public override long Length => buffer.CalcTotalSize();

	public override long Position
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public SendStream(SendBuffer sendBuffer)
	{
		buffer = sendBuffer;
	}

	public override void Flush()
	{
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		this.buffer.Write(buffer, offset, count);
	}
}
