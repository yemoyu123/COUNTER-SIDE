using System;
using NKM;
using UnityEngine;

namespace NKC;

public static class NKCServerStringFormatter
{
	public static readonly string[] Seperators = new string[1] { "@@" };

	public static string TranslateServerFormattedString(string targetString)
	{
		if (string.IsNullOrEmpty(targetString))
		{
			return "";
		}
		try
		{
			string[] array = targetString.Split(Seperators, StringSplitOptions.None);
			string[] array2 = new string[array.Length - 1];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = NKCStringTable.GetString(TryPraseItemName(array[i + 1]), bSkipErrorCheck: true);
			}
			string format = NKCStringTable.GetString(array[0], bSkipErrorCheck: true);
			object[] args = array2;
			return string.Format(format, args);
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.Message);
			return "";
		}
	}

	public static string TryPraseItemName(string param)
	{
		if (param.StartsWith("<MiscId>"))
		{
			string text = param.Substring("<MiscId>".Length);
			if (!int.TryParse(text, out var result))
			{
				throw new ArgumentException("MiscId TryPrase Fail string:" + text);
			}
			return (NKMItemManager.GetItemMiscTempletByID(result) ?? throw new ArgumentException($"MiscId Invalid Id:{result}")).GetItemName();
		}
		if (param.StartsWith("<EquipId>"))
		{
			string text = param.Substring("<EquipId>".Length);
			if (!int.TryParse(text, out var result2))
			{
				throw new ArgumentException("EquipId TryPrase Fail string:" + text);
			}
			return (NKMItemManager.GetEquipTemplet(result2) ?? throw new ArgumentException($"EquipId Invalid Id:{result2}")).GetItemName();
		}
		if (param.StartsWith("<MoldId>"))
		{
			string text = param.Substring("<MoldId>".Length);
			if (!int.TryParse(text, out var result3))
			{
				throw new ArgumentException("TryPrase Fail string:" + text);
			}
			return (NKMItemManager.GetItemMoldTempletByID(result3) ?? throw new ArgumentException($"MoldId Invalid Id:{result3}")).GetItemName();
		}
		return param;
	}
}
