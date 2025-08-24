using Cs.Core.Util;

namespace Cs.Memory;

public static class Crypto2
{
	private const ulong OddMask = 6148914691236517205uL;

	private const ulong EvenMask = 12297829382473034410uL;

	private static readonly ulong[] MaskList = new ulong[16]
	{
		14003937370121879411uL, 295159725236528685uL, 14656252856989855980uL, 3126201044280739051uL, 6176412274767465921uL, 8501111619623644353uL, 1001882303165547266uL, 889784367385610816uL, 8403001398375820177uL, 15646421979254498160uL,
		15540104736269140030uL, 4473111575030559303uL, 16641115610173278858uL, 7005653296469604124uL, 7641466651897675454uL, 18242667629599333687uL
	};

	public static void Encrypt(byte[] buffer, int size)
	{
		Encrypt(buffer, 0, size);
	}

	public static void Decrypt(byte[] buffer, int size)
	{
		Decrypt(buffer, 0, size);
	}

	public static void Encrypt(byte[] buffer, int offset, int size)
	{
		int num = 0;
		int num2 = 0;
		while (num2 < size)
		{
			ulong num3 = MaskList[num];
			int num4 = offset + num2;
			int num5 = size - num2;
			if (num5 >= 8)
			{
				ulong num6 = buffer.DirectToUint64(num4) ^ num3;
				ulong num7 = num6 & 0x5555555555555555L;
				ulong num8 = ((num6 & 0xAAAAAAAAAAAAAAAAuL) >> 1) | (num7 << 1);
				num8 = (num8 & 0xFFFFFFFF00000000uL) | ((num8 & 0xFF000000u) >> 8) | ((num8 & 0xFF0000) << 8) | ((num8 & 0xFF00) >> 8) | ((num8 & 0xFF) << 8);
				num8.DirectWriteTo(buffer, num4);
				num2 += 8;
			}
			else
			{
				for (int i = 0; i < num5; i++)
				{
					buffer[num4 + i] ^= (byte)((long)num3 & (255L << i >>> i));
				}
				num2 += num5;
			}
			num = (num + 1) % MaskList.Length;
		}
	}

	public static void Decrypt(byte[] buffer, int offset, int size)
	{
		int num = 0;
		int num2 = 0;
		while (num2 < size)
		{
			ulong num3 = MaskList[num];
			int num4 = offset + num2;
			int num5 = size - num2;
			if (num5 >= 8)
			{
				ulong num6 = buffer.DirectToUint64(num4);
				num6 = (num6 & 0xFFFFFFFF00000000uL) | ((num6 & 0xFF000000u) >> 8) | ((num6 & 0xFF0000) << 8) | ((num6 & 0xFF00) >> 8) | ((num6 & 0xFF) << 8);
				ulong num7 = num6 & 0x5555555555555555L;
				num6 = ((num6 & 0xAAAAAAAAAAAAAAAAuL) >> 1) | (num7 << 1);
				(num6 ^ num3).DirectWriteTo(buffer, num4);
				num2 += 8;
			}
			else
			{
				for (int i = 0; i < num5; i++)
				{
					buffer[num4 + i] ^= (byte)((long)num3 & (255L << i >>> i));
				}
				num2 += num5;
			}
			num = (num + 1) % MaskList.Length;
		}
	}
}
