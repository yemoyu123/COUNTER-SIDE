using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI.Result;

public class NKCUIResultSubUIShadowLife : NKCUIResultSubUIBase
{
	public List<Animator> m_lstLife;

	public float UI_END_DELAY_TIME = 2f;

	private bool m_bFinished;

	private int m_prevLife;

	private int m_updateLife;

	public void SetData(BATTLE_RESULT_TYPE resultType, int prevLife, int currLife, bool bIgnoreAutoClose = false)
	{
		if (resultType != BATTLE_RESULT_TYPE.BRT_LOSE || prevLife <= currLife)
		{
			base.ProcessRequired = false;
			return;
		}
		m_prevLife = prevLife;
		m_updateLife = currLife;
		for (int i = 0; i < m_lstLife.Count; i++)
		{
			if (i < m_prevLife)
			{
				m_lstLife[i].Play("NKM_UI_SHADOW_READY_LIFE");
			}
			else
			{
				m_lstLife[i].Play("NKM_UI_SHADOW_READY_LIFE_OFF");
			}
		}
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
		int lifeIdx = -1;
		if (m_prevLife != m_updateLife)
		{
			for (int i = 0; i < m_lstLife.Count; i++)
			{
				if (i < m_updateLife)
				{
					m_lstLife[i].Play("NKM_UI_SHADOW_READY_LIFE");
				}
				else if (i < m_prevLife)
				{
					m_lstLife[i].Play("NKM_UI_SHADOW_READY_LIFE_DOWN");
					lifeIdx = i;
				}
				else
				{
					m_lstLife[i].Play("NKM_UI_SHADOW_READY_LIFE_OFF");
				}
			}
			yield return null;
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
			yield return WaitAniOrInput(m_lstLife[lifeIdx]);
		}
		FinishProcess();
	}

	public override void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
