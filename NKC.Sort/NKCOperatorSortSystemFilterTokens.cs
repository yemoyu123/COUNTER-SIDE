using System;
using System.Collections.Generic;
using NKC.StringSearch;
using NKM;
using NKM.Templet;

namespace NKC.Sort;

public class NKCOperatorSortSystemFilterTokens
{
	public class FilterToken
	{
		private bool m_inverse;

		private string m_strValue;

		private bool m_bHasIntValue;

		private int m_intValue;

		private NKM_UNIT_GRADE m_unitGrade = NKM_UNIT_GRADE.NUG_COUNT;

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
				m_unitGrade = NKCFilterTokensUtil.ParseGrade(value);
			}
		}

		public bool CheckFilter(NKMOperator operatorData, NKCOperatorSortSystem.OperatorListOptions sortOptions)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
			if (m_bHasIntValue && CheckIntFilter(operatorData, unitTempletBase, sortOptions))
			{
				return true;
			}
			if (m_unitGrade != NKM_UNIT_GRADE.NUG_COUNT)
			{
				return unitTempletBase.m_NKM_UNIT_GRADE == m_unitGrade;
			}
			if (NKCStringSearchTool.Contains(unitTempletBase.GetUnitName(), m_strValue))
			{
				return true;
			}
			if (NKCStringSearchTool.ContainKeywordList(unitTempletBase.m_lstSearchKeyword, m_strValue))
			{
				return true;
			}
			if (m_bHasIntValue)
			{
				return false;
			}
			NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(operatorData.subSkill.id);
			if (skillTemplet != null && NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID), m_strValue))
			{
				return true;
			}
			if (NKCStringSearchTool.Contains(unitTempletBase.GetUnitTitle(), m_strValue))
			{
				return true;
			}
			if (NKCUtil.IsUsingAdminUserFunction() && NKCStringSearchTool.Contains(unitTempletBase.m_UnitStrID, m_strValue))
			{
				return true;
			}
			if (NKCStringSearchTool.Contains(NKCStringTable.GetString(NKCOperatorUtil.GetSkillTemplet(operatorData.mainSkill.id).m_OperSkillNameStrID), m_strValue))
			{
				return true;
			}
			return false;
		}

		public bool CheckIntFilter(NKMOperator operatorData, NKMUnitTempletBase unitTempletBase, NKCOperatorSortSystem.OperatorListOptions sortOptions)
		{
			if (unitTempletBase == null)
			{
				return false;
			}
			if (operatorData.level == m_intValue)
			{
				return true;
			}
			if (NKCUtil.IsUsingAdminUserFunction() && operatorData.id == m_intValue)
			{
				return true;
			}
			if (operatorData.mainSkill.level == m_intValue)
			{
				return true;
			}
			if (operatorData.subSkill.level == m_intValue)
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

	public bool CheckFilter(NKMOperator operatorData, NKCOperatorSortSystem.OperatorListOptions sortOptions)
	{
		foreach (FilterToken item in m_lstFilterToken)
		{
			if (item.CheckFilter(operatorData, sortOptions) == item.Inverse)
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
