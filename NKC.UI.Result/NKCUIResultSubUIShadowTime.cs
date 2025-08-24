using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUIShadowTime : NKCUIResultSubUIBase
{
	public Text m_txtRecentTime;

	public Text m_txtBestTime;

	public GameObject m_objNewRecord;

	public float UI_END_DELAY_TIME = 2f;

	private bool m_bFinished;

	public void SetData(BATTLE_RESULT_TYPE resultType, int currClearTime, int bestClearTime, bool bIgnoreAutoClose = false)
	{
		if (resultType != BATTLE_RESULT_TYPE.BRT_WIN || currClearTime == 0)
		{
			base.ProcessRequired = false;
			return;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(currClearTime);
		NKCUtil.SetLabelText(m_txtRecentTime, NKCUtilString.GetTimeSpanString(timeSpan));
		string msg = "-:--:--";
		if (bestClearTime > 0)
		{
			timeSpan = TimeSpan.FromSeconds(bestClearTime);
			msg = NKCUtilString.GetTimeSpanString(timeSpan);
		}
		NKCUtil.SetLabelText(m_txtBestTime, msg);
		NKCUtil.SetGameobjectActive(m_objNewRecord, bestClearTime == 0 || currClearTime < bestClearTime);
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
			while (UI_END_DELAY_TIME > currentTime && !m_bHadUserInput)
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
