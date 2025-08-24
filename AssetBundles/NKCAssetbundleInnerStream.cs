using System;
using System.IO;
using Cs.Engine.Network.Buffer.Detail;

namespace AssetBundles;

public class NKCAssetbundleInnerStream : Stream
{
	private Stream betterStream;

	private byte[] decryptedArray = new byte[212];

	private long decryptSize;

	public override bool CanRead => betterStream.CanRead;

	public override bool CanSeek => betterStream.CanSeek;

	public override bool CanWrite => betterStream.CanWrite;

	public override long Length => betterStream.Length;

	public override long Position
	{
		get
		{
			return betterStream.Position;
		}
		set
		{
			betterStream.Position = value;
		}
	}

	public static string GetJarRelativePath(string path)
	{
		return path.Substring(path.LastIndexOf("!/assets/", StringComparison.OrdinalIgnoreCase) + "!/assets/".Length);
	}

	public NKCAssetbundleInnerStream(string path)
	{
		if (path.Contains("jar:"))
		{
			path = GetJarRelativePath(path);
		}
		betterStream = BetterStreamingAssets.OpenRead(path);
		decryptSize = Math.Min(Length, 212L);
		betterStream.Read(decryptedArray, 0, (int)decryptSize);
		Crypto.Encrypt(decryptedArray, (int)decryptSize, AssetBundleManager.GetMaskList(path));
		Seek(0L, SeekOrigin.Begin);
	}

	public override void Flush()
	{
		betterStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		long position = Position;
		int result = betterStream.Read(buffer, offset, count);
		if (position < decryptSize)
		{
			if (position + count < decryptSize)
			{
				Array.Copy(decryptedArray, position, buffer, offset, count);
				return result;
			}
			Array.Copy(length: (int)(decryptSize - position), sourceArray: decryptedArray, sourceIndex: position, destinationArray: buffer, destinationIndex: offset);
		}
		return result;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return betterStream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		betterStream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		betterStream.Write(buffer, offset, count);
	}
}
