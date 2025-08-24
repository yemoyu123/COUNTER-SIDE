using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class CFilter
{
	private CFilter root;

	private Dictionary<char, CFilter> dic = new Dictionary<char, CFilter>();

	private bool isLeaf;

	public void Init()
	{
		StringValidSet.Init();
	}

	public string Filter(string original)
	{
		if (string.IsNullOrEmpty(original))
		{
			return "";
		}
		if (root == null || dic.Count == 0)
		{
			return original;
		}
		string text = original;
		string text2 = original.ToUpper();
		int num = 0;
		while (num < text2.Length)
		{
			int num2 = Match(text2.Substring(num, text2.Length - num));
			if (num2 > 0)
			{
				text = Replace(text.ToCharArray(), num, num2);
				num += num2;
			}
			else
			{
				num++;
			}
		}
		return text;
	}

	private int Match(string text)
	{
		CFilter value = root;
		bool flag = false;
		int num = 0;
		int b = 0;
		for (int i = 0; i < text.Length; i++)
		{
			if (StringValidSet.CheckIgnoreSet(text[i]) && value != root)
			{
				if (flag)
				{
					return Mathf.Max(num, b);
				}
				b = i + 1;
				continue;
			}
			if (!value.dic.TryGetValue(text[i], out value))
			{
				return num;
			}
			if (value.isLeaf)
			{
				num = i + 1;
				flag = true;
			}
		}
		return num;
	}

	private string Replace(char[] charArray, int start, int count)
	{
		for (int i = start; i < start + count; i++)
		{
			charArray[i] = '*';
		}
		return new string(charArray);
	}

	public bool CheckFilter(string data, bool ignoreWhiteSpace)
	{
		return Check(data, ignoreWhiteSpace);
	}

	public bool CheckNickNameFilter(char[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (!CheckForNickName(data, i))
			{
				return false;
			}
		}
		return true;
	}

	public void AddFilterString(string data)
	{
		root = this;
		Add(data.ToUpper());
	}

	private void Add(string data, int index = 0)
	{
		if (data.Length == index)
		{
			isLeaf = true;
			return;
		}
		if (!dic.TryGetValue(data[index], out var value))
		{
			value = new CFilter();
			value.root = this;
		}
		dic[data[index]] = value;
		value.Add(data, ++index);
	}

	private bool Check(string data, bool ignoreWhiteSpace, int index = 0)
	{
		string text = data.ToUpper();
		if (text.Length == index || isLeaf)
		{
			return !isLeaf;
		}
		if (!ignoreWhiteSpace && char.IsWhiteSpace(text[index]))
		{
			return false;
		}
		if (dic.TryGetValue(text[index], out var value))
		{
			return value.Check(text, ignoreWhiteSpace, ++index);
		}
		return true;
	}

	private bool CheckForNickName(char[] data, int index = 0)
	{
		if (data.Length == index || isLeaf)
		{
			return !isLeaf;
		}
		if (char.IsWhiteSpace(data[index]))
		{
			return false;
		}
		if (!StringValidSet.Valid(data[index]))
		{
			return false;
		}
		if (dic.TryGetValue(char.ToUpper(data[index]), out var value))
		{
			return value.CheckForNickName(data, ++index);
		}
		return true;
	}
}
