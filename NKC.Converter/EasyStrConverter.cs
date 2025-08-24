using System;
using System.Text;

namespace NKC.Converter;

public class EasyStrConverter : IStrConverter
{
	private readonly StringBuilder _strBuilder = new StringBuilder();

	public string Encryption(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		_strBuilder.Clear();
		int length = str.Length;
		for (int i = 0; i < length; i++)
		{
			char ch = str[i];
			char c = ShiftChar(ch, length);
			_strBuilder.Append(Convert.ToString(c));
		}
		return _strBuilder.ToString();
	}

	public string Decryption(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		_strBuilder.Clear();
		int length = str.Length;
		for (int i = 0; i < length; i++)
		{
			char ch = str[i];
			char c = ShiftChar(ch, -length);
			_strBuilder.Append(Convert.ToString(c));
		}
		return _strBuilder.ToString();
	}

	public char ShiftChar(char ch, int range)
	{
		if (char.IsUpper(ch))
		{
			return ShiftChar(ch, range, 65, 90);
		}
		if (char.IsLower(ch))
		{
			return ShiftChar(ch, range, 97, 122);
		}
		if (char.IsNumber(ch))
		{
			return ShiftChar(ch, range, 48, 57);
		}
		return ch;
	}

	private char ShiftChar(char ch, int range, int min, int max)
	{
		int num = max - min;
		int num2 = range % num;
		if (num2 == 0)
		{
			if (range > 0)
			{
				num2 += 2;
			}
			if (range < 0)
			{
				num2 -= 2;
			}
		}
		int num3 = ch + num2;
		if (num3 > max)
		{
			int num4 = num3 % max;
			return Convert.ToChar(min + num4 - 1);
		}
		if (num3 < min)
		{
			int num5 = min % num3;
			return Convert.ToChar(max - num5 + 1);
		}
		return Convert.ToChar(num3);
	}
}
