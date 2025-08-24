using System;
using System.Collections.Generic;
using NKC.StringSearch;
using NKM;

namespace NKC.Sort;

public class NKCMiscSortSystemFilterTokens
{
	public class FilterToken
	{
		private bool m_inverse;

		private string m_strValue;

		private bool m_bHasIntValue;

		private int m_intValue;

		private NKM_ITEM_GRADE m_miscGrade = NKM_ITEM_GRADE.NIG_COUNT;

		public bool Inverse => m_inverse;

		public FilterToken(string value)
		{
			m_strValue = value;
			ParseToken(value);
		}

		private void ParseToken(string value)
		{
			if (value[0] == '-')
			{
				m_inverse = true;
				value = value.Substring(1);
				m_strValue = value;
			}
			m_bHasIntValue = int.TryParse(value, out m_intValue);
			if (!m_bHasIntValue)
			{
				m_miscGrade = NKCFilterTokensUtil.ParseGradeItem(value);
			}
		}

		public bool CheckFilter(NKMItemMiscTemplet miscData, NKCMiscSortSystem.MiscListOptions sortOptions)
		{
			if (miscData == null)
			{
				return false;
			}
			if (NKCStringSearchTool.Contains(miscData.GetItemName(), m_strValue))
			{
				return true;
			}
			if (m_bHasIntValue)
			{
				return false;
			}
			if (m_miscGrade != NKM_ITEM_GRADE.NIG_COUNT && miscData.m_NKM_ITEM_GRADE == m_miscGrade)
			{
				return true;
			}
			return false;
		}
	}

	public string m_TokenString = string.Empty;

	public List<FilterToken> m_lstFilterToken = new List<FilterToken>();

	public string TokenString => m_TokenString;

	public void Clear()
	{
		m_TokenString = string.Empty;
		m_lstFilterToken.Clear();
	}

	public void SetStringToken(string text)
	{
		m_TokenString = text;
		Tokenize(text);
	}

	public bool CheckFilter(NKMItemMiscTemplet miscData, NKCMiscSortSystem.MiscListOptions sortOptions)
	{
		foreach (FilterToken item in m_lstFilterToken)
		{
			if (item.CheckFilter(miscData, sortOptions) == item.Inverse)
			{
				return false;
			}
		}
		return true;
	}

	public void Tokenize(string text)
	{
		m_lstFilterToken.Clear();
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = NKCStringSearchTool.NormalizeSearchInput(text).Split(NKCFilterTokensUtil.Seperator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string value in array)
			{
				m_lstFilterToken.Add(new FilterToken(value));
			}
		}
	}
}
