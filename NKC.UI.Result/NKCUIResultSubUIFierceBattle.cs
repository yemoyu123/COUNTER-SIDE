using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUIFierceBattle : NKCUIResultSubUIBase
{
	[Header("격전지원")]
	public GameObject m_Result_FierceBattle;

	public Text m_FIERCE_BATTLE_SCORE;

	public Text m_FIERCE_BATTLE_BEST_SCORE;

	public Text m_FIERCE_BATTLE_HP;

	public Text m_FIERCE_BATTLE_TIME;

	public GameObject m_NEW_RECORD_TAG_TEXT;

	public Text m_FIERCE_BATTLE_SCORE_INFO_TEXT;

	private bool m_bFinished;

	public void SetData(NKCUIResult.BattleResultData data, bool bIgnoreAutoClose = false)
	{
		if (data == null)
		{
			base.ProcessRequired = false;
			return;
		}
		bool flag = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().GetBossGroupPoint() < data.m_iFierceScore;
		NKCUtil.SetGameobjectActive(m_NEW_RECORD_TAG_TEXT, flag);
		NKCUtil.SetLabelText(m_FIERCE_BATTLE_SCORE_INFO_TEXT, flag ? NKCUtilString.GET_FIERCE_BATTLE_END_TEXT_NEW_RECORD : NKCUtilString.GET_FIERCE_BATTLE_END_TEXT);
		NKCUtil.SetLabelText(m_FIERCE_BATTLE_SCORE, data.m_iFierceScore.ToString());
		NKCUtil.SetLabelText(m_FIERCE_BATTLE_BEST_SCORE, data.m_iFierceBestScore.ToString());
		float num = Math.Max(data.m_fFierceLastBossHPPercent, 0f);
		NKCUtil.SetLabelText(m_FIERCE_BATTLE_HP, $"{num}%");
		string text = "-:--:--";
		if (data.m_fFierceRestTime > 0f)
		{
			text = NKCUtilString.GetTimeSpanString(TimeSpan.FromSeconds(data.m_fFierceRestTime));
		}
		NKCUtil.SetLabelText(m_FIERCE_BATTLE_TIME, text.ToString());
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
		yield return null;
	}

	public override void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
