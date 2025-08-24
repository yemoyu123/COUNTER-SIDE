using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public static class NKMSkinManager
{
	public static Dictionary<int, NKMSkinTemplet> m_dicSkinTemplet;

	private static Dictionary<int, List<NKMSkinTemplet>> m_dicSkinForCharacter;

	public static void LoadFromLua()
	{
		m_dicSkinTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", "LUA_SKIN_TEMPLET", "SkinTemplet", NKMSkinTemplet.LoadFromLUA);
	}

	public static NKMSkinTemplet GetSkinTemplet(int skinID)
	{
		if (skinID == 0)
		{
			return null;
		}
		if (m_dicSkinTemplet.TryGetValue(skinID, out var value))
		{
			return value;
		}
		return null;
	}

	public static NKMSkinTemplet GetSkinTemplet(int skinID, int unitID)
	{
		if (m_dicSkinTemplet.TryGetValue(skinID, out var value) && IsSkinForCharacter(unitID, value))
		{
			return value;
		}
		return null;
	}

	public static NKMSkinTemplet GetSkinTemplet(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return null;
		}
		if (unitData.m_SkinID != 0 && m_dicSkinTemplet.TryGetValue(unitData.m_SkinID, out var value) && IsSkinForCharacter(unitData.m_UnitID, value))
		{
			return value;
		}
		return null;
	}

	public static bool IsSkinForCharacter(int unitID, NKMSkinTemplet templet)
	{
		if (templet == null)
		{
			return false;
		}
		if (templet.m_SkinEquipUnitID == unitID)
		{
			return true;
		}
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
		if (nKMUnitTempletBase != null && nKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return nKMUnitTempletBase.IsSameBaseUnit(templet.m_SkinEquipUnitID);
		}
		return false;
	}

	public static bool IsSkinForCharacter(int unitID, int skinID)
	{
		NKMSkinTemplet skinTemplet = GetSkinTemplet(skinID);
		return IsSkinForCharacter(unitID, skinTemplet);
	}

	public static NKM_ERROR_CODE CanEquipSkin(NKMUserData userData, NKMUnitData unitData, int newSkinID)
	{
		if (unitData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (newSkinID != 0)
		{
			if (unitData.IsSeized)
			{
				return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED;
			}
			if (!userData.m_InventoryData.HasItemSkin(newSkinID))
			{
				return NKM_ERROR_CODE.NEC_FAIL_SKIN_NOT_OWNED;
			}
			if (!IsSkinForCharacter(unitData.m_UnitID, newSkinID))
			{
				return NKM_ERROR_CODE.NEC_FAIL_SKIN_UNIT_NOT_MATCH;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static List<NKMSkinTemplet> GetSkinlistForCharacter(int unitID, NKMInventoryData inventoryData)
	{
		if (m_dicSkinForCharacter == null)
		{
			MakeCharacterSkinList();
		}
		unitID = GetBaseUnitID(unitID);
		if (m_dicSkinForCharacter.TryGetValue(unitID, out var value))
		{
			if (inventoryData != null)
			{
				return value.FindAll((NKMSkinTemplet x) => x.EnableByTag && (!x.m_bExclude || inventoryData.HasItemSkin(x.m_SkinID)));
			}
			return value.FindAll((NKMSkinTemplet x) => x.EnableByTag);
		}
		return null;
	}

	private static void MakeCharacterSkinList()
	{
		m_dicSkinForCharacter = new Dictionary<int, List<NKMSkinTemplet>>();
		foreach (NKMSkinTemplet value in m_dicSkinTemplet.Values)
		{
			if (value.EnableByTag)
			{
				if (m_dicSkinForCharacter.ContainsKey(value.m_SkinEquipUnitID))
				{
					m_dicSkinForCharacter[value.m_SkinEquipUnitID].Add(value);
					continue;
				}
				List<NKMSkinTemplet> list = new List<NKMSkinTemplet>();
				list.Add(value);
				m_dicSkinForCharacter[value.m_SkinEquipUnitID] = list;
			}
		}
	}

	public static bool IsCharacterHasSkin(int unitID)
	{
		if (m_dicSkinForCharacter == null)
		{
			MakeCharacterSkinList();
		}
		unitID = GetBaseUnitID(unitID);
		return m_dicSkinForCharacter.ContainsKey(unitID);
	}

	public static string GetSkillCutin(NKMUnitData unitData, string origName)
	{
		if (unitData == null)
		{
			return origName;
		}
		NKMSkinTemplet skinTemplet = GetSkinTemplet(unitData);
		if (!string.IsNullOrEmpty(skinTemplet?.m_HyperSkillCutin))
		{
			return skinTemplet.m_HyperSkillCutin;
		}
		return origName;
	}

	private static int GetBaseUnitID(int unitID)
	{
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
		if (nKMUnitTempletBase.m_BaseUnitID != 0 && nKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return nKMUnitTempletBase.m_BaseUnitID;
		}
		return unitID;
	}

	public static List<NKMSkinTemplet> GetSkinTemplets(string skinStrID)
	{
		return m_dicSkinTemplet.Values.Where((NKMSkinTemplet templet) => templet.m_SkinStrID == skinStrID).ToList();
	}
}
