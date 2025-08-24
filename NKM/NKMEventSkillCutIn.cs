using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Unit;
using UnityEngine;

namespace NKM;

public class NKMEventSkillCutIn : NKMUnitStateEventOneTime, INKCUnitstateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public string m_SkillStrID = string.Empty;

	public string m_CutinString = string.Empty;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Client;

	public override void ApplyEventClient(NKCGameClient cNKMGame, NKCUnitClient cNKMUnit)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && !gameOptionData.ViewSkillCutIn)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = cNKMUnit.GetUnitTempletBase();
		string skillName;
		if (!string.IsNullOrEmpty(m_CutinString))
		{
			skillName = NKCStringTable.GetString(m_CutinString);
		}
		else if (!string.IsNullOrEmpty(m_SkillStrID))
		{
			skillName = ((unitTempletBase == null || !unitTempletBase.IsShip()) ? NKMUnitSkillManager.GetUnitSkillTemplet(m_SkillStrID, cNKMUnit.GetUnitData()).GetSkillName() : NKMShipSkillManager.GetShipSkillTempletByStrID(m_SkillStrID).GetName());
		}
		else
		{
			if (string.IsNullOrEmpty(cNKMUnit.GetUnitStateNow().m_SkillCutInName))
			{
				Log.Error("[NKCEventSkillCutIn] skillName Not Found!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCUnitStateEvent.cs", 156);
				return;
			}
			skillName = NKCStringTable.GetString(cNKMUnit.GetUnitStateNow().m_SkillCutInName);
		}
		Sprite faceSprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, cNKMUnit.GetUnitData());
		if (!cNKMGame.IsEnemy(cNKMGame.m_MyTeam, cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE_ORG))
		{
			cNKMGame.PlaySkillCutIn(cNKMUnit, bHyper: false, bRight: true, faceSprite, unitTempletBase.GetUnitName(), skillName);
		}
		else
		{
			cNKMGame.PlaySkillCutIn(cNKMUnit, bHyper: false, bRight: false, faceSprite, unitTempletBase.GetUnitName(), skillName);
		}
	}

	public void DeepCopyFromSource(NKMEventSkillCutIn source)
	{
		DeepCopy(source);
		m_SkillStrID = source.m_SkillStrID;
		m_CutinString = source.m_CutinString;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_SkillStrID", ref m_SkillStrID);
		cNKMLua.GetData("m_CutinString", ref m_CutinString);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
	}
}
