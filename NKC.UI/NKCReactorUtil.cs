using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCReactorUtil
{
	public static NKMUnitReactorTemplet GetReactorTemplet(int unitID)
	{
		return GetReactorTemplet(NKMUnitManager.GetUnitTempletBase(unitID));
	}

	public static NKMUnitReactorTemplet GetReactorTemplet(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase != null)
		{
			return NKMUnitReactorTemplet.Find(unitTempletBase.m_ReactorId);
		}
		return null;
	}

	public static NKMReactorSkillTemplet[] GetReactorSkillTemplet(NKMUnitTempletBase unitTempletBase)
	{
		return GetReactorTemplet(unitTempletBase)?.skillTemplets;
	}

	public static NKMReactorSkillTemplet[] GetReactorSkillTemplet(int unitID)
	{
		return GetReactorTemplet(unitID)?.skillTemplets;
	}

	public static Sprite GetReactorLevelSprite(int level)
	{
		if (level > NKMCommonConst.ReactorMaxLevel)
		{
			return null;
		}
		string text = "";
		switch (level)
		{
		case 0:
			text = "AB_UI_REACTOR_LEVEL_0";
			break;
		case 1:
			text = "AB_UI_REACTOR_LEVEL_1";
			break;
		case 2:
			text = "AB_UI_REACTOR_LEVEL_2";
			break;
		case 3:
			text = "AB_UI_REACTOR_LEVEL_3";
			break;
		case 4:
			text = "AB_UI_REACTOR_LEVEL_4";
			break;
		case 5:
			text = "AB_UI_REACTOR_LEVEL_5";
			break;
		default:
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_reactor_sprite", text);
	}

	public static bool IsReactorUnit(int unitID)
	{
		return IsReactorUnit(NKMUnitManager.GetUnitTempletBase(unitID));
	}

	public static bool IsReactorUnit(NKMUnitTempletBase unitTempletBase)
	{
		bool result = false;
		if (unitTempletBase != null && !unitTempletBase.IsRearmUnit)
		{
			result = NKMUnitReactorTemplet.Find(unitTempletBase.m_ReactorId)?.EnableByTag ?? false;
		}
		return result;
	}

	public static bool CheckCanTryLevelUp(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return false;
		}
		if (!IsReactorUnit(unitData.m_UnitID))
		{
			return false;
		}
		return true;
	}
}
