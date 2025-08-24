using System.Collections.Generic;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopBossInfoDetailSlot : MonoBehaviour
{
	public Text m_lbStep;

	public Text m_lbHP;

	public Text m_lbBattleCondition;

	public Text m_lbKillPoint;

	public void SetData(GuildRaidTemplet raidTemplet, NKMDungeonTempletBase templetBase, bool bExtraBoss = false)
	{
		string msg = (bExtraBoss ? NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_RAID_EXTRA_BOSS : string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_RAID_UI_LEVEL_INFO, raidTemplet.GetStageIndex()));
		NKCUtil.SetLabelText(m_lbStep, msg);
		NKCUtil.SetLabelText(m_lbHP, NKMDungeonManager.GetBossHp(raidTemplet.GetStageId(), templetBase.m_DungeonLevel).ToString("N0"));
		List<NKMBattleConditionTemplet> battleConditions = templetBase.BattleConditions;
		if (battleConditions.Count > 0)
		{
			foreach (NKMBattleConditionTemplet item in battleConditions)
			{
				NKCUtil.SetLabelText(m_lbBattleCondition, (item != null && !item.m_bHide) ? item.BattleCondDesc_Translated : NKCUtilString.GET_STRING_NO_EXIST);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbBattleCondition, NKCUtilString.GET_STRING_NO_EXIST);
		}
		NKCUtil.SetLabelText(m_lbKillPoint, raidTemplet.GetRewardPoint().ToString("N0"));
	}
}
