using System.Collections;

namespace Cs.Engine.Util;

public static class BitArrayExt
{
	public static byte[] ToByteArray(this BitArray data)
	{
		byte[] array = new byte[data.Length / 8];
		data.CopyTo(array, 0);
		return array;
	}

	public static int GetByteCount(this BitArray data)
	{
		return data.Length / 8;
	}
}
