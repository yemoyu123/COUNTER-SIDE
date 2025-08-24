using System;
using System.Collections.Generic;
using NKC.StringSearch;
using NKC.Templet;
using NKM;
using NKM.Templet;

namespace NKC.Sort;

public class NKCUnitSortSystemFilterTokens
{
	public enum Movetype
	{
		All,
		Land,
		Air
	}

	public class FilterToken
	{
		private bool m_inverse;

		private string m_strValue;

		private bool m_bHasIntValue;

		private int m_intValue;

		private bool m_bAwaken;

		private bool m_bRearm;

		private bool m_bReactor;

		private bool m_bLifeTime;

		private NKM_UNIT_GRADE m_unitGrade = NKM_UNIT_GRADE.NUG_COUNT;

		private NKM_UNIT_STYLE_TYPE m_unitStyle;

		private NKM_UNIT_ROLE_TYPE m_unitRole;

		private Movetype m_eMoveType;

		private bool m_bTag;

		private NKCUnitTagInfoTemplet.SuffixType m_tagSuffix;

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
			if (value[0] == '#' || value[0] == '＃')
			{
				m_bTag = true;
				if (value[value.Length - 1] == '+' || value[value.Length - 1] == '＋')
				{
					m_tagSuffix = NKCUnitTagInfoTemplet.SuffixType.POSITIVE;
					value = value.Substring(1, value.Length - 2);
				}
				else if (value[value.Length - 1] == '-' || value[value.Length - 1] == '－')
				{
					m_tagSuffix = NKCUnitTagInfoTemplet.SuffixType.NEGATIVE;
					value = value.Substring(1, value.Length - 2);
				}
				else
				{
					m_tagSuffix = NKCUnitTagInfoTemplet.SuffixType.NONE;
					value = value.Substring(1);
				}
				m_strValue = value;
				return;
			}
			m_bHasIntValue = int.TryParse(value, out m_intValue);
			if (!m_bHasIntValue)
			{
				if (value.Length >= 2)
				{
					m_bAwaken = NKCStringSearchTool.StartsWith(NKCStringTable.GetString("SI_PF_FILTER_UNIT_TYPE_AWAKEN"), m_strValue);
					m_bRearm = NKCStringSearchTool.StartsWith(NKCStringTable.GetString("SI_PF_FILTER_UNIT_TYPE_REARMAMENT"), m_strValue);
					m_bReactor = NKCStringSearchTool.StartsWith(NKCStringTable.GetString("SI_PF_UNIT_REACTOR_TEXT"), m_strValue);
					m_bLifeTime = NKCStringSearchTool.StartsWith(NKCStringTable.GetString("SI_DP_LIFETIME"), m_strValue) || NKCStringSearchTool.StartsWith(NKCStringTable.GetString("SI_DP_LOYALTY_LIFETIME"), m_strValue);
					m_unitRole = NKCFilterTokensUtil.ParseRole(value);
				}
				m_unitGrade = NKCFilterTokensUtil.ParseGrade(value);
				m_unitStyle = NKCFilterTokensUtil.ParseStyle(value);
				m_eMoveType = ParseMoveType(value);
			}
		}

		private Movetype ParseMoveType(string str)
		{
			if (NKCStringSearchTool.StartsWith(NKCStringTable.GetString("SI_PF_FILTER_ATK_TYP_AIR"), str))
			{
				return Movetype.Air;
			}
			if (NKCStringSearchTool.StartsWith(NKCStringTable.GetString("SI_PF_FILTER_ATK_TYP"), str))
			{
				return Movetype.Land;
			}
			return Movetype.All;
		}

		public bool CheckFilter(NKMUnitData unitData, NKCUnitSortSystem.UnitListOptions sortOptions)
		{
			NKMUnitTempletBase unitTempletBase = unitData.GetUnitTempletBase();
			if (m_bTag)
			{
				return CheckUnitTag(unitTempletBase);
			}
			if (m_bHasIntValue && CheckIntFilter(unitData, unitTempletBase, sortOptions))
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
			if (NKCStringSearchTool.Contains(unitTempletBase.GetUnitTitle(), m_strValue))
			{
				return true;
			}
			if (NKCUtil.IsUsingAdminUserFunction() && NKCStringSearchTool.Contains(unitTempletBase.m_UnitStrID, m_strValue))
			{
				return true;
			}
			if (unitTempletBase.m_bAwaken)
			{
				if (m_bAwaken)
				{
					return true;
				}
				if (NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_FILTER_UNIT_TYPE_AWAKEN") + unitTempletBase.GetUnitName(), m_strValue, bFirstLetterMatch: true))
				{
					return true;
				}
			}
			if (unitTempletBase.IsRearmUnit)
			{
				if (m_bRearm)
				{
					return true;
				}
				if (NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_FILTER_UNIT_TYPE_REARMAMENT") + unitTempletBase.GetUnitName(), m_strValue, bFirstLetterMatch: true))
				{
					return true;
				}
			}
			if (m_bReactor && unitTempletBase.IsReactorUnit)
			{
				return true;
			}
			if (m_bLifeTime && unitData.IsPermanentContract)
			{
				return true;
			}
			if (unitTempletBase.HasUnitStyleType(m_unitStyle))
			{
				return true;
			}
			if (unitTempletBase.HasUnitRoleType(m_unitRole))
			{
				return true;
			}
			if (m_eMoveType != Movetype.All)
			{
				if (m_eMoveType == Movetype.Air && unitTempletBase.m_bAirUnit)
				{
					return true;
				}
				if (m_eMoveType == Movetype.Land && !unitTempletBase.m_bAirUnit)
				{
					return true;
				}
			}
			foreach (NKM_UNIT_TAG item in unitTempletBase.m_hsUnitTag)
			{
				if (NKCStringSearchTool.StartsWith(NKCUtilString.GetUnitTagString(item), m_strValue))
				{
					return true;
				}
			}
			if (unitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				if (NKCStringSearchTool.StartsWith(NKCUtilString.GetSourceTypeName(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE), m_strValue))
				{
					return true;
				}
				if (unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB != NKM_UNIT_SOURCE_TYPE.NUST_NONE && NKCStringSearchTool.StartsWith(NKCUtilString.GetSourceTypeName(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB), m_strValue))
				{
					return true;
				}
			}
			return false;
		}

		public bool CheckIntFilter(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKCUnitSortSystem.UnitListOptions sortOptions)
		{
			if (unitTempletBase == null)
			{
				return false;
			}
			if (CheckFinalUnitCost(unitData, m_intValue, sortOptions))
			{
				return true;
			}
			if (unitData.m_UnitLevel == m_intValue)
			{
				return true;
			}
			if (NKCUtil.IsUsingAdminUserFunction() && unitData.m_UnitID == m_intValue)
			{
				return true;
			}
			return false;
		}

		private bool CheckFinalUnitCost(NKMUnitData unitData, int cost, NKCUnitSortSystem.UnitListOptions sortOptions)
		{
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
			if (sortOptions.bUseBanData && NKCBanManager.IsBanUnit(unitStatTemplet.m_UnitID) && unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, NKCBanManager.GetBanData(), null) == cost)
			{
				return true;
			}
			if (sortOptions.bUseUpData && NKCBanManager.IsUpUnit(unitStatTemplet.m_UnitID) && unitStatTemplet.GetRespawnCost(bPVP: true, bLeader: false, null, NKCBanManager.m_dicNKMUpData) == cost)
			{
				return true;
			}
			if (unitStatTemplet.GetRespawnCost(bLeader: false, null, null) == cost)
			{
				return true;
			}
			return false;
		}

		private bool CheckUnitTag(NKMUnitTempletBase unitTempletBase)
		{
			if (unitTempletBase.m_lstUnitTag == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(m_strValue))
			{
				return false;
			}
			foreach (string item in unitTempletBase.m_lstUnitTag)
			{
				NKCUnitTagInfoTemplet nKCUnitTagInfoTemplet = NKCUnitTagInfoTemplet.Find(item);
				if (nKCUnitTagInfoTemplet != null && (m_tagSuffix == NKCUnitTagInfoTemplet.SuffixType.NONE || nKCUnitTagInfoTemplet.m_Suffix == m_tagSuffix) && NKCStringSearchTool.StartsWith(NKCStringTable.GetString(nKCUnitTagInfoTemplet.tagStringKey), m_strValue))
				{
					return true;
				}
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

	public bool CheckFilter(NKMUnitData unitData, NKCUnitSortSystem.UnitListOptions sortOptions)
	{
		foreach (FilterToken item in m_lstFilterToken)
		{
			if (item.CheckFilter(unitData, sortOptions) == item.Inverse)
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
