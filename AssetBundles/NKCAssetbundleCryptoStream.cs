using System;
using System.IO;
using Cs.Engine.Network.Buffer.Detail;

namespace AssetBundles;

public class NKCAssetbundleCryptoStream : FileStream
{
	private byte[] decryptedArray = new byte[212];

	private long decryptSize;

	public NKCAssetbundleCryptoStream(string path, FileMode mode, FileAccess access)
		: base(path, mode, access)
	{
		decryptSize = Math.Min(Length, 212L);
		base.Read(decryptedArray, 0, (int)decryptSize);
		Crypto.Encrypt(decryptedArray, (int)decryptSize, AssetBundleManager.GetMaskList(path));
		Seek(0L, SeekOrigin.Begin);
	}

	public override int Read(byte[] array, int offset, int count)
	{
		long position = Position;
		int result = base.Read(array, offset, count);
		if (position >= decryptSize)
		{
			return result;
		}
		if (position + count < decryptSize)
		{
			Array.Copy(decryptedArray, position, array, offset, count);
		}
		else
		{
			int num = (int)(decryptSize - position);
			Array.Copy(decryptedArray, position, array, offset, num);
		}
		return result;
	}
}
