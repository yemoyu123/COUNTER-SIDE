using System.Collections;
using UnityEngine;

namespace NKC.UI.Result;

public class NKCUIResultSubUITip : NKCUIResultSubUIBase
{
	public float UI_END_DELAY_TIME = 2f;

	private bool m_bFinished;

	public override void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void SetData(BATTLE_RESULT_TYPE resultType, bool bIgnoreAutoClose = false)
	{
		switch (resultType)
		{
		case BATTLE_RESULT_TYPE.BRT_WIN:
			base.ProcessRequired = false;
			break;
		case BATTLE_RESULT_TYPE.BRT_LOSE:
		case BATTLE_RESULT_TYPE.BRT_DRAW:
			base.ProcessRequired = true;
			break;
		}
		m_bIgnoreAutoClose = bIgnoreAutoClose;
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		m_bFinished = false;
		yield return null;
		m_bHadUserInput = false;
		float currentTime = 0f;
		if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetAlarmRepeatOperationQuitByDefeat())
		{
			m_bPause = true;
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetAlarmRepeatOperationQuitByDefeat(bSet: false);
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetStopReason(NKCStringTable.GetString("SI_POPUP_REPEAT_FAIL_DEFEAT"));
			NKCPopupRepeatOperation.Instance.OpenForResult(delegate
			{
				m_bPause = false;
			});
		}
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
}
