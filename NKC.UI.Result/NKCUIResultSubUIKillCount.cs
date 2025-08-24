using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUIKillCount : NKCUIResultSubUIBase
{
	[Header("킬카운트")]
	public Text m_lbScoreGain;

	public Text m_lbBestScore;

	public Text m_lbTotalScore;

	public Text m_lbBattleTime;

	public GameObject m_objNewRecord;

	public Text m_lbNewRecordDesc;

	private bool m_bFinished;

	public void SetData(NKCUIResult.BattleResultData data, bool bIgnoreAutoClose = false)
	{
		if (data == null || data.m_KillCountGain == 0L)
		{
			base.ProcessRequired = false;
			return;
		}
		bool flag = data.m_KillCountGain > data.m_KillCountStageRecord;
		NKCUtil.SetGameobjectActive(m_objNewRecord, flag);
		NKCUtil.SetLabelText(m_lbNewRecordDesc, flag ? NKCUtilString.GET_FIERCE_BATTLE_END_TEXT_NEW_RECORD : "");
		NKCUtil.SetLabelText(m_lbScoreGain, data.m_KillCountGain.ToString("#,##0"));
		NKCUtil.SetLabelText(m_lbBestScore, data.m_KillCountStageRecord.ToString("#,##0"));
		NKCUtil.SetLabelText(m_lbTotalScore, data.m_KillCountTotal.ToString("#,##0"));
		string text = "-:--:--";
		if (data.m_battleData != null && data.m_battleData.playTime > 0f)
		{
			text = NKCUtilString.GetTimeSpanString(TimeSpan.FromSeconds(data.m_battleData.playTime));
		}
		NKCUtil.SetLabelText(m_lbBattleTime, text.ToString());
		base.ProcessRequired = true;
		m_bIgnoreAutoClose = bIgnoreAutoClose;
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
		float currentTime = 0f;
		if (bAutoSkip)
		{
			while (1f > currentTime && !m_bHadUserInput)
			{
				if (!m_bPause)
				{
					currentTime += Time.deltaTime;
				}
				yield return null;
			}
		}
		else
		{
			while (m_bPause)
			{
				yield return null;
			}
			yield return WaitAniOrInput(null);
		}
		FinishProcess();
	}

	public override void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
