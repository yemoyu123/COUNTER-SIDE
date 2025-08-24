using System;
using System.Collections.Generic;
using NKC.StringSearch;
using NKM;
using NKM.Templet;

namespace NKC.Sort;

public class NKCEquipSortSystemFilterTokens
{
	private enum StatPosition
	{
		All,
		Main,
		Sub,
		Set,
		Potential
	}

	private enum StatCompareType
	{
		None,
		More,
		Less
	}

	private class EnvironmentToken
	{
		public StatPosition statPosition;

		public bool TryParse(string str)
		{
			if (NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_EQUIP_FILTER_HIDDEN_OPTION"), str))
			{
				statPosition = StatPosition.Potential;
				return true;
			}
			if (NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_SORT_EQUIP_SET_OPTION"), str) || NKCStringSearchTool.Contains(NKCSearchKeywordTemplet.GetKeyword("SI_KW_PF_SORT_EQUIP_SET_OPTION"), str))
			{
				statPosition = StatPosition.Set;
				return true;
			}
			return false;
		}
	}

	private class FilterToken
	{
		private bool m_inverse;

		private string m_strValue;

		private bool m_bHasIntValue;

		private int m_intValue;

		private int m_Tier = -1;

		private int m_EnchantLevel = -1;

		private EnvironmentToken m_Env;

		private NKM_ITEM_GRADE m_eGrade = NKM_ITEM_GRADE.NIG_COUNT;

		private NKM_UNIT_STYLE_TYPE m_eUnitStyle;

		private ITEM_EQUIP_POSITION m_ePosition = ITEM_EQUIP_POSITION.IEP_NONE;

		private NKCEquipSortSystem.eFilterOption m_EquipType = NKCEquipSortSystem.eFilterOption.All;

		private StatCompareType m_eStatCompareType;

		private float m_fStatCompareValue;

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
			if (value[0] != '+')
			{
				m_bHasIntValue = int.TryParse(value, out m_intValue);
				if (m_bHasIntValue)
				{
					return;
				}
			}
			value = ExtractCompareValue(value);
			m_strValue = value;
			m_eGrade = NKCUtil.ConvertUnitGradeToItemGrade(NKCFilterTokensUtil.ParseGrade(value));
			m_eUnitStyle = NKCFilterTokensUtil.ParseStyle(value);
			m_ePosition = ParseEquipPosition(value);
			m_EquipType = ParseEquipType(value);
			if (NKCStringSearchTool.SplitPrefixNumber(value, out var number, out var subString))
			{
				if (NKCStringSearchTool.IsAbbreviation("Tier", subString, bFirstLetterMatch: true) || NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_FILTER_EQUIP_TIER"), subString, bFirstLetterMatch: true))
				{
					m_Tier = number;
				}
				if (NKCStringSearchTool.IsAbbreviation("SI_PF_TUTORIAL_TEXUTRE_EQUIP_ENCHANT_1_ENHANCE", subString, bFirstLetterMatch: true))
				{
					m_EnchantLevel = number;
				}
			}
			else
			{
				if ((value[0] == 't' || value[0] == 'T' || value[0] == 'l' || value[0] == 'L') && !int.TryParse(value.Substring(1), out m_Tier))
				{
					m_Tier = -1;
				}
				if (value[0] == '+' && !int.TryParse(value.Substring(1), out m_EnchantLevel))
				{
					m_EnchantLevel = -1;
				}
			}
		}

		public ITEM_EQUIP_POSITION ParseEquipPosition(string value)
		{
			if (NKCStringSearchTool.StartsWith(NKCUtilString.GetEquipPositionString(ITEM_EQUIP_POSITION.IEP_WEAPON), value))
			{
				return ITEM_EQUIP_POSITION.IEP_WEAPON;
			}
			if (NKCStringSearchTool.StartsWith(NKCUtilString.GetEquipPositionString(ITEM_EQUIP_POSITION.IEP_DEFENCE), value))
			{
				return ITEM_EQUIP_POSITION.IEP_DEFENCE;
			}
			if (NKCStringSearchTool.StartsWith(NKCUtilString.GetEquipPositionString(ITEM_EQUIP_POSITION.IEP_ACC), value) || NKCStringSearchTool.StartsWith("\ufffdǼ\ufffd\ufffd\ufffd\ufffd\ufffd", value))
			{
				return ITEM_EQUIP_POSITION.IEP_ACC;
			}
			return ITEM_EQUIP_POSITION.IEP_NONE;
		}

		public NKCEquipSortSystem.eFilterOption ParseEquipType(string value)
		{
			if (NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_FILTER_EQUIP_TYPE_USE_RELIC"), value))
			{
				return NKCEquipSortSystem.eFilterOption.Equip_Relic;
			}
			if (NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_FILTER_EQUIP_TYPE_USE_PRIVATE_NORMAL"), value))
			{
				return NKCEquipSortSystem.eFilterOption.Equip_Private;
			}
			if (NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_FILTER_EQUIP_TYPE_USE_PRIVATE_AWAKEN"), value))
			{
				return NKCEquipSortSystem.eFilterOption.Equip_Private_Awaken;
			}
			if (NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_FILTER_EQUIP_TYPE_USE_NORMAL"), value))
			{
				return NKCEquipSortSystem.eFilterOption.Equip_Non_Private;
			}
			return NKCEquipSortSystem.eFilterOption.All;
		}

		public string ExtractCompareValue(string value)
		{
			int num = value.IndexOf('>');
			if (num > 0)
			{
				string text = value.Substring(0, num);
				string text2 = value.Substring(num + 1);
				if (float.TryParse(text, out m_fStatCompareValue))
				{
					m_eStatCompareType = StatCompareType.Less;
					return text2;
				}
				if (float.TryParse(text2, out m_fStatCompareValue))
				{
					m_eStatCompareType = StatCompareType.More;
					return text;
				}
				m_eStatCompareType = StatCompareType.None;
				return value;
			}
			num = value.IndexOf('<');
			if (num > 0)
			{
				string text3 = value.Substring(0, num);
				string text4 = value.Substring(num + 1);
				if (float.TryParse(text3, out m_fStatCompareValue))
				{
					m_eStatCompareType = StatCompareType.More;
					return text4;
				}
				if (float.TryParse(text4, out m_fStatCompareValue))
				{
					m_eStatCompareType = StatCompareType.Less;
					return text3;
				}
			}
			m_eStatCompareType = StatCompareType.None;
			return value;
		}

		public void SetEnv(EnvironmentToken env)
		{
			m_Env = env;
		}

		public bool CheckFilter(NKMEquipItemData equipData, NKCEquipSortSystem.EquipListOptions sortOptions)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipData.m_ItemEquipID);
			if (m_bHasIntValue && CheckIntFilter(equipData, equipTemplet, sortOptions))
			{
				return true;
			}
			if (m_eGrade != NKM_ITEM_GRADE.NIG_COUNT)
			{
				return equipTemplet.m_NKM_ITEM_GRADE == m_eGrade;
			}
			if (m_eStatCompareType == StatCompareType.None)
			{
				if (NKCStringSearchTool.Contains(equipTemplet.GetItemName(), m_strValue))
				{
					return true;
				}
				if (NKCStringSearchTool.ContainKeywordList(equipTemplet.m_lstSearchKeyword, m_strValue))
				{
					return true;
				}
			}
			if (m_bHasIntValue)
			{
				return false;
			}
			if (NKCUtil.IsUsingAdminUserFunction() && NKCStringSearchTool.Contains(equipTemplet.m_ItemEquipStrID, m_strValue))
			{
				return true;
			}
			if (equipTemplet.IsValidEquipPosition(m_ePosition))
			{
				return true;
			}
			if (equipTemplet.m_EquipUnitStyleType == m_eUnitStyle)
			{
				return true;
			}
			if (m_Tier == equipTemplet.m_NKM_ITEM_TIER)
			{
				return true;
			}
			if (m_EnchantLevel == equipData.m_EnchantLevel)
			{
				return true;
			}
			switch (m_EquipType)
			{
			case NKCEquipSortSystem.eFilterOption.Equip_Relic:
				if (equipTemplet.IsRelic())
				{
					return true;
				}
				break;
			case NKCEquipSortSystem.eFilterOption.Equip_Private:
			case NKCEquipSortSystem.eFilterOption.Equip_Private_Awaken:
				if (equipTemplet.IsPrivateEquip())
				{
					return true;
				}
				break;
			case NKCEquipSortSystem.eFilterOption.Equip_Non_Private:
				if (!equipTemplet.IsPrivateEquip() && !equipTemplet.IsRelic())
				{
					return true;
				}
				break;
			}
			if (equipData.m_OwnerUnitUID != 0L)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(equipData.m_OwnerUnitUID));
				if (unitTempletBase != null)
				{
					if (NKCStringSearchTool.Contains(unitTempletBase.GetUnitName(), m_strValue))
					{
						return true;
					}
					if (unitTempletBase.m_bAwaken && NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_FILTER_UNIT_TYPE_AWAKEN") + unitTempletBase.GetUnitName(), m_strValue, bFirstLetterMatch: true))
					{
						return true;
					}
				}
			}
			if (equipTemplet.IsPrivateEquip())
			{
				foreach (int privateUnit in equipTemplet.PrivateUnitList)
				{
					NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(privateUnit);
					if (unitTempletBase2 != null)
					{
						if (NKCStringSearchTool.Contains(unitTempletBase2.GetUnitName(), m_strValue))
						{
							return true;
						}
						if (unitTempletBase2.m_bAwaken && NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString("SI_PF_FILTER_UNIT_TYPE_AWAKEN") + unitTempletBase2.GetUnitName(), m_strValue, bFirstLetterMatch: true))
						{
							return true;
						}
					}
				}
			}
			for (int i = 0; i < equipData.m_Stat.Count; i++)
			{
				StatPosition position = ((i == 0) ? StatPosition.Main : StatPosition.Sub);
				if (CheckStat(m_strValue, equipData, equipData.m_Stat[i], position))
				{
					return true;
				}
			}
			if (equipData.potentialOptions.Count > 0)
			{
				for (int j = 0; j < equipData.potentialOptions.Count; j++)
				{
					float totalStatValue = equipData.potentialOptions[j].GetTotalStatValue();
					if (CheckStat(m_strValue, equipData.potentialOptions[j].statType, totalStatValue, StatPosition.Potential))
					{
						return true;
					}
				}
			}
			if (m_eStatCompareType == StatCompareType.None)
			{
				NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(equipData.m_SetOptionId);
				if (equipSetOptionTemplet != null && CheckStatPosition(StatPosition.Set))
				{
					if (NKCStringSearchTool.IsAbbreviation(NKCStringTable.GetString(equipSetOptionTemplet.m_EquipSetName), m_strValue))
					{
						return true;
					}
					if (CheckStat(m_strValue, equipSetOptionTemplet.m_StatType_1, equipSetOptionTemplet.m_StatValue_1, StatPosition.Set))
					{
						return true;
					}
					if (CheckStat(m_strValue, equipSetOptionTemplet.m_StatType_2, equipSetOptionTemplet.m_StatValue_2, StatPosition.Set))
					{
						return true;
					}
				}
			}
			if (m_eStatCompareType != StatCompareType.None && CheckStatPosition(StatPosition.All) && CheckAllStat(equipData))
			{
				return true;
			}
			return false;
		}

		private bool CheckIntFilter(NKMEquipItemData equipData, NKMEquipTemplet equipTemplet, NKCEquipSortSystem.EquipListOptions sortOptions)
		{
			if (equipTemplet.m_NKM_ITEM_TIER == m_intValue)
			{
				return true;
			}
			if (equipData.m_EnchantLevel == m_intValue)
			{
				return true;
			}
			if (NKCUtil.IsUsingAdminUserFunction() && equipData.m_ItemEquipID == m_intValue)
			{
				return true;
			}
			return false;
		}

		private bool CheckStat(string token, NKMEquipItemData equipData, EQUIP_ITEM_STAT statData, StatPosition position)
		{
			float statValue = statData.stat_value + (float)equipData.m_EnchantLevel * statData.stat_level_value;
			return CheckStat(token, statData.type, statValue, position);
		}

		private bool CheckStat(string token, NKM_STAT_TYPE statType, float statValue, StatPosition position)
		{
			if (statType == NKM_STAT_TYPE.NST_RANDOM || statType == NKM_STAT_TYPE.NST_END)
			{
				return false;
			}
			if (!CheckStatPosition(position))
			{
				return false;
			}
			NKCStatInfoTemplet nKCStatInfoTemplet = NKCStatInfoTemplet.Find(statType);
			if (m_eStatCompareType != StatCompareType.None)
			{
				float num = statValue;
				if (nKCStatInfoTemplet != null && nKCStatInfoTemplet.HasNegativeName)
				{
					num = Math.Abs(statValue);
				}
				if (NKMUnitStatManager.IsPercentStat(statType))
				{
					num *= 100f;
				}
				if (m_eStatCompareType == StatCompareType.More)
				{
					if (m_fStatCompareValue > num)
					{
						return false;
					}
				}
				else if (m_eStatCompareType == StatCompareType.Less && m_fStatCompareValue < num)
				{
					return false;
				}
			}
			if (NKCStringSearchTool.IsAbbreviation(NKCUtilString.GetStatShortName(statType, statValue), token))
			{
				return true;
			}
			if (nKCStatInfoTemplet != null && NKCStringSearchTool.AbbreviationKeywordList(nKCStatInfoTemplet.m_lstSearchKeyword, token))
			{
				return true;
			}
			return false;
		}

		private bool CheckStatPosition(StatPosition position)
		{
			if (m_Env == null || m_Env.statPosition == StatPosition.All)
			{
				return true;
			}
			if (m_Env.statPosition == position)
			{
				return true;
			}
			return false;
		}

		private bool CheckAllStat(NKMEquipItemData equipData)
		{
			Dictionary<NKM_STAT_TYPE, float> dicStat = new Dictionary<NKM_STAT_TYPE, float>();
			for (int i = 0; i < equipData.m_Stat.Count; i++)
			{
				EQUIP_ITEM_STAT eQUIP_ITEM_STAT = equipData.m_Stat[i];
				float value = eQUIP_ITEM_STAT.stat_value + (float)equipData.m_EnchantLevel * eQUIP_ITEM_STAT.stat_level_value;
				AddStat(eQUIP_ITEM_STAT.type, value);
			}
			for (int j = 0; j < equipData.potentialOptions.Count; j++)
			{
				float totalStatValue = equipData.potentialOptions[j].GetTotalStatValue();
				AddStat(equipData.potentialOptions[j].statType, totalStatValue);
			}
			foreach (KeyValuePair<NKM_STAT_TYPE, float> item in dicStat)
			{
				if (CheckStat(m_strValue, item.Key, item.Value, StatPosition.All))
				{
					return true;
				}
			}
			return false;
			void AddStat(NKM_STAT_TYPE type, float num)
			{
				if (dicStat.TryGetValue(type, out var value2))
				{
					dicStat[type] = value2 + num;
				}
				else
				{
					dicStat[type] = num;
				}
			}
		}
	}

	public string m_TokenString = string.Empty;

	private List<FilterToken> m_lstFilterToken = new List<FilterToken>();

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

	public bool CheckFilter(NKMEquipItemData equipData, NKCEquipSortSystem.EquipListOptions sortOptions)
	{
		foreach (FilterToken item in m_lstFilterToken)
		{
			if (item.CheckFilter(equipData, sortOptions) == item.Inverse)
			{
				return false;
			}
		}
		return true;
	}

	private void Tokenize(string text)
	{
		m_lstFilterToken.Clear();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = NKCStringSearchTool.NormalizeSearchInput(text).Split(NKCFilterTokensUtil.Seperator, StringSplitOptions.RemoveEmptyEntries);
		EnvironmentToken environmentToken = new EnvironmentToken();
		bool flag = false;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (environmentToken.TryParse(text2))
			{
				flag = true;
				continue;
			}
			FilterToken filterToken = new FilterToken(text2);
			if (flag)
			{
				filterToken.SetEnv(environmentToken);
				environmentToken = new EnvironmentToken();
				flag = false;
			}
			m_lstFilterToken.Add(filterToken);
		}
	}
}
