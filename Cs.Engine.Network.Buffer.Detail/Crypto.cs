using Cs.Core.Util;

namespace Cs.Engine.Network.Buffer.Detail;

public static class Crypto
{
	private static readonly ulong[] MaskList = new ulong[4] { 14170986657190717782uL, 15546886188969944187uL, 15913139373130964729uL, 3486779174683840252uL };

	public static void Encrypt(byte[] buffer, int size)
	{
		if (buffer != null)
		{
			int maskIndex = 0;
			Encrypt(buffer, size, ref maskIndex, MaskList);
		}
	}

	public static void Encrypt(byte[] buffer, int size, ulong[] maskList)
	{
		if (buffer != null)
		{
			int maskIndex = 0;
			Encrypt(buffer, size, ref maskIndex, maskList);
		}
	}

	public static void Encrypt(byte[] buffer, int size, ref int maskIndex)
	{
		if (buffer != null)
		{
			Encrypt(buffer, size, ref maskIndex, MaskList);
		}
	}

	public static void Encrypt(byte[] buffer, int size, ref int maskIndex, ulong[] maskList)
	{
		if (buffer == null)
		{
			return;
		}
		int num = 0;
		while (num < size)
		{
			ulong num2 = maskList[maskIndex];
			int num3 = size - num;
			if (num3 >= 8)
			{
				(buffer.DirectToUint64(num) ^ num2).DirectWriteTo(buffer, num);
				num += 8;
			}
			else
			{
				for (int i = num; i < size; i++)
				{
					int num4 = i - num;
					buffer[i] ^= (byte)((long)num2 & (255L << num4 >>> num4));
				}
				num += num3;
			}
			maskIndex = (maskIndex + 1) % maskList.Length;
		}
	}
}
