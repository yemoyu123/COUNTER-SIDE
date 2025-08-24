using System.Collections;
using System.Collections.Generic;
using NKC.Trim;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUITrim : NKCUIResultSubUIBase
{
	public List<NKCUIResultSubUITrimSlot> m_lstSlot;

	public Text m_lbTrimLevel;

	private bool m_bFinished;

	private float DelayBeforeClose = 3f;

	public void SetData(NKCUIResult.BattleResultData data)
	{
		base.ProcessRequired = data != null && data.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_TRIM && NKCTrimManager.TrimModeState != null;
		if (!base.ProcessRequired)
		{
			return;
		}
		if (m_lstSlot != null)
		{
			for (int i = 0; i < m_lstSlot.Count; i++)
			{
				if (!(m_lstSlot[i] == null))
				{
					m_lstSlot[i].SetData(i);
				}
			}
		}
		NKCUtil.SetLabelText(m_lbTrimLevel, NKCTrimManager.TrimModeState.trimLevel.ToString());
	}

	public override void FinishProcess()
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_bFinished = true;
			StopAllCoroutines();
		}
	}

	public override bool IsProcessFinished()
	{
		return m_bFinished;
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		m_bFinished = false;
		m_bHadUserInput = false;
		yield return null;
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		NKMGameRuntimeTeamData nKMGameRuntimeTeamData = gameClient.GetGameRuntimeData()?.GetMyRuntimeTeamData(gameClient.m_MyTeam);
		if (nKMGameRuntimeTeamData == null || !nKMGameRuntimeTeamData.m_bAutoRespawn)
		{
			yield break;
		}
		float currentTime = 0f;
		while (DelayBeforeClose > currentTime && !m_bHadUserInput)
		{
			if (!m_bPause)
			{
				currentTime += Time.deltaTime;
			}
			yield return null;
		}
		m_bFinished = true;
	}
}
