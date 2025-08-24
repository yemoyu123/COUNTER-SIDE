using System.IO;
using Cs.Engine.Network.Buffer;
using Cs.Engine.Network.Buffer.Detail;
using LZ4;

namespace Cs.Engine.Util;

public static class Lz4Util
{
	public static byte[] Compress(byte[] source)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (LZ4Stream lZ4Stream = new LZ4Stream(memoryStream, LZ4StreamMode.Compress))
		{
			lZ4Stream.Write(source, 0, source.Length);
		}
		return memoryStream.ToArray();
	}

	public static ZeroCopyBuffer Decompress(byte[] source)
	{
		ZeroCopyBuffer zeroCopyBuffer = new ZeroCopyBuffer();
		using ZeroCopyOutputStream zeroCopyOutputStream = new ZeroCopyOutputStream(zeroCopyBuffer);
		using LZ4Stream lZ4Stream = new LZ4Stream(new MemoryStream(source), LZ4StreamMode.Decompress);
		byte[] array = new byte[4096];
		int num = 0;
		while ((num = lZ4Stream.Read(array, 0, array.Length)) != 0)
		{
			zeroCopyOutputStream.Write(array, 0, num);
		}
		return zeroCopyBuffer;
	}

	public static ZeroCopyBuffer Decompress(Stream source)
	{
		ZeroCopyBuffer zeroCopyBuffer = new ZeroCopyBuffer();
		using Stream stream = zeroCopyBuffer.GetOutputStream();
		using LZ4Stream lZ4Stream = new LZ4Stream(source, LZ4StreamMode.Decompress);
		byte[] array = new byte[4096];
		int num = 0;
		while ((num = lZ4Stream.Read(array, 0, array.Length)) != 0)
		{
			stream.Write(array, 0, num);
		}
		return zeroCopyBuffer;
	}
}
