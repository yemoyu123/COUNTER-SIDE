using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.UI;

public class NKCUIOperationSubChallenge : MonoBehaviour
{
	[Header("모의작전")]
	public List<NKCUIOperationSubChallengeSlot> m_lstDailySlot = new List<NKCUIOperationSubChallengeSlot>();

	[Header("디멘션 트리밍")]
	public NKCUIOperationSubChallengeSlot m_slotShadow;

	[Header("디멘션 트리밍")]
	public NKCUIOperationSubChallengeSlot m_slotDimension;

	[Header("타임어택")]
	public NKCUIOperationSubChallengeSlot m_slotTimeAttack;

	public void Open()
	{
		NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMTempletContainer<NKMEpisodeGroupTemplet>.Find((NKMEpisodeGroupTemplet x) => x.EpCategory == EPISODE_CATEGORY.EC_DAILY);
		if (nKMEpisodeGroupTemplet != null && nKMEpisodeGroupTemplet.lstEpisodeTemplet.Count == m_lstDailySlot.Count)
		{
			for (int num = 0; num < nKMEpisodeGroupTemplet.lstEpisodeTemplet.Count; num++)
			{
				m_lstDailySlot[num].SetData(nKMEpisodeGroupTemplet.lstEpisodeTemplet[num], OnClickDailySlot);
			}
		}
		NKMEpisodeGroupTemplet groupTemplet = NKMTempletContainer<NKMEpisodeGroupTemplet>.Find((NKMEpisodeGroupTemplet x) => x.EpCategory == EPISODE_CATEGORY.EC_SHADOW);
		m_slotShadow.SetData(groupTemplet, EPISODE_CATEGORY.EC_SHADOW, OnClickShadow);
		NKMEpisodeGroupTemplet groupTemplet2 = NKMTempletContainer<NKMEpisodeGroupTemplet>.Find((NKMEpisodeGroupTemplet x) => x.EpCategory == EPISODE_CATEGORY.EC_TRIM);
		m_slotDimension.SetData(groupTemplet2, EPISODE_CATEGORY.EC_TRIM, OnClickDimension);
		NKMEpisodeGroupTemplet groupTemplet3 = NKMTempletContainer<NKMEpisodeGroupTemplet>.Find((NKMEpisodeGroupTemplet x) => x.EpCategory == EPISODE_CATEGORY.EC_TIMEATTACK);
		m_slotTimeAttack.SetData(groupTemplet3, EPISODE_CATEGORY.EC_TIMEATTACK, OnClickTimeAttack);
		NKMEpisodeTempletV2 reservedEpisodeTemplet = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet();
		if (reservedEpisodeTemplet != null)
		{
			NKCUIOperationNodeViewer.Instance.Open(reservedEpisodeTemplet);
		}
		TutorialCheck();
	}

	private void OnClickDailySlot(int key)
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(key, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV != null)
		{
			NKCUIOperationNodeViewer.Instance.Open(nKMEpisodeTempletV);
		}
	}

	private void OnClickShadow(int key)
	{
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_SHADOW_PALACE, "");
	}

	private void OnClickDimension(int key)
	{
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_TRIM, "");
	}

	private void OnClickTimeAttack(int key)
	{
		NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(key);
		if (nKMEpisodeGroupTemplet != null && nKMEpisodeGroupTemplet.lstEpisodeTemplet.Count > 0)
		{
			NKCUIOperationNodeViewer.Instance.Open(nKMEpisodeGroupTemplet.lstEpisodeTemplet[0]);
		}
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Operation_Challenge);
	}
}
