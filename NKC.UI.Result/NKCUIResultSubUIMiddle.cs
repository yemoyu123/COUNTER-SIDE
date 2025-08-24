using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI.Result;

public class NKCUIResultSubUIMiddle : NKCUIResultSubUIBase
{
	public Animator m_animator;

	public GameObject m_objWorldMapBranchExp;

	public List<NKCUIResultSubUIWorldmapBranchExp> m_lstWorldMapBranchExp;

	public NKCUIResultSubUIDungeon m_uiDungeonMission;

	public NKCUIResultSubUIRaid m_NKCUIResultSubUIRaid;

	private bool m_bFinished;

	private float UI_END_DELAY_TIME = 1f;

	private bool m_bAutoSkip;

	private bool bWaiting;

	public void Init()
	{
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		m_bFinished = false;
		bWaiting = false;
		m_bAutoSkip = bAutoSkip;
		bool bWorldMapBranchExpProcessWaiting = false;
		bool bDungeonMissionProcessWaiting = false;
		bool bRaidProcessWaiting = false;
		if (!m_lstWorldMapBranchExp[0].ProcessRequired && !m_uiDungeonMission.ProcessRequired && !m_NKCUIResultSubUIRaid.ProcessRequired)
		{
			yield break;
		}
		int processRequiredCount = 0;
		if (m_lstWorldMapBranchExp[0].ProcessRequired)
		{
			processRequiredCount++;
			NKCUtil.SetGameobjectActive(m_objWorldMapBranchExp, bValue: true);
			foreach (NKCUIResultSubUIWorldmapBranchExp item in m_lstWorldMapBranchExp)
			{
				NKCUtil.SetGameobjectActive(item.gameObject, item.ProcessRequired);
			}
			yield return m_lstWorldMapBranchExp[0].Process(m_bAutoSkip);
			bWorldMapBranchExpProcessWaiting = true;
		}
		if (m_uiDungeonMission.ProcessRequired)
		{
			processRequiredCount++;
			NKCUtil.SetGameobjectActive(m_uiDungeonMission, bValue: true);
			yield return m_uiDungeonMission.Process(m_bAutoSkip);
			bDungeonMissionProcessWaiting = true;
		}
		if (m_NKCUIResultSubUIRaid.ProcessRequired)
		{
			processRequiredCount++;
			NKCUtil.SetGameobjectActive(m_NKCUIResultSubUIRaid, bValue: true);
			yield return m_NKCUIResultSubUIRaid.Process(m_bAutoSkip);
			bRaidProcessWaiting = true;
		}
		yield return null;
		while ((processRequiredCount > 0 || m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) && !m_bHadUserInput)
		{
			if (bWorldMapBranchExpProcessWaiting && m_lstWorldMapBranchExp[0].IsProcessFinished())
			{
				bWorldMapBranchExpProcessWaiting = false;
				processRequiredCount--;
			}
			if (bDungeonMissionProcessWaiting && m_uiDungeonMission.IsProcessFinished())
			{
				bDungeonMissionProcessWaiting = false;
				processRequiredCount--;
			}
			if (bRaidProcessWaiting && m_NKCUIResultSubUIRaid.IsProcessFinished())
			{
				bRaidProcessWaiting = false;
				processRequiredCount--;
			}
			yield return null;
		}
		FinishProcess();
	}

	public override bool IsProcessFinished()
	{
		return m_bFinished;
	}

	public override void FinishProcess()
	{
		if (bWaiting)
		{
			return;
		}
		bWaiting = true;
		StopAllCoroutines();
		m_animator.Play("INTRO", 0, 1f);
		foreach (NKCUIResultSubUIWorldmapBranchExp item in m_lstWorldMapBranchExp)
		{
			item.FinishProcess();
		}
		m_uiDungeonMission.FinishProcess();
		m_NKCUIResultSubUIRaid.FinishProcess();
		StartCoroutine(WaitForCloseAnimation());
	}

	public IEnumerator WaitForCloseAnimation()
	{
		m_bHadUserInput = false;
		if (m_bAutoSkip)
		{
			float currentTime = 0f;
			while (UI_END_DELAY_TIME > currentTime && !m_bHadUserInput)
			{
				currentTime += Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			yield return WaitAniOrInput(null);
		}
		while (m_bPause)
		{
			yield return null;
		}
		yield return PlayCloseAnimation(m_animator);
		m_bFinished = true;
		bWaiting = false;
	}

	public override void Close()
	{
		foreach (NKCUIResultSubUIWorldmapBranchExp item in m_lstWorldMapBranchExp)
		{
			item.Close();
		}
		m_uiDungeonMission.Close();
		m_NKCUIResultSubUIRaid.Close();
		base.Close();
	}

	public void SetDataBattleResult(NKCUIResult.BattleResultData data, float endDelayTime, bool bIgnoreAutoClose = false)
	{
		UI_END_DELAY_TIME = endDelayTime;
		foreach (NKCUIResultSubUIWorldmapBranchExp item in m_lstWorldMapBranchExp)
		{
			item.SetData(-1, -1, -1, -1, -1, bIgnoreAutoClose);
		}
		m_uiDungeonMission.SetData(data.m_lstMissionData, data.m_bShowMedal, bIgnoreAutoClose, data.m_bShowClearPoint, data.m_fArenaClearPoint);
		m_NKCUIResultSubUIRaid.SetData(data, bIgnoreAutoClose);
		m_bIgnoreAutoClose = bIgnoreAutoClose;
		if (m_uiDungeonMission.ProcessRequired || m_NKCUIResultSubUIRaid.ProcessRequired)
		{
			base.ProcessRequired = true;
		}
		else
		{
			base.ProcessRequired = false;
		}
	}

	public void SetDataWorldMapMissionResult(List<NKCUIResult.CityMissionResultData> lstResultData, float endDelayTime, bool bIgnoreAutoClose = false)
	{
		UI_END_DELAY_TIME = endDelayTime;
		for (int i = 0; i < m_lstWorldMapBranchExp.Count; i++)
		{
			if (lstResultData.Count <= i)
			{
				m_lstWorldMapBranchExp[i].SetData(-1, -1, -1, -1, -1);
			}
			else
			{
				m_lstWorldMapBranchExp[i].SetData(lstResultData[i].m_CityID, lstResultData[i].m_CityLevelOld, lstResultData[i].m_CityLevelNew, lstResultData[i].m_CityExpOld, lstResultData[i].m_CityExpNew, lstResultData[i].m_bGreatSuccess, bIgnoreAutoClose);
			}
		}
		m_uiDungeonMission.SetData(null, bShowMedal: false);
		m_NKCUIResultSubUIRaid.SetData(null);
		m_bIgnoreAutoClose = bIgnoreAutoClose;
		if (m_lstWorldMapBranchExp[0].ProcessRequired)
		{
			base.ProcessRequired = true;
		}
		else
		{
			base.ProcessRequired = false;
		}
	}

	public void SetDataNull()
	{
		foreach (NKCUIResultSubUIWorldmapBranchExp item in m_lstWorldMapBranchExp)
		{
			item.SetData(-1, -1, -1, -1, -1);
		}
		m_uiDungeonMission.SetData(null);
		m_NKCUIResultSubUIRaid.SetData(null);
		m_bIgnoreAutoClose = false;
		base.ProcessRequired = false;
	}
}
