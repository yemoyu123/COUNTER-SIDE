using System;
using System.Collections;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupUserLevelUp : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_levelup";

	private const string UI_ASSET_NAME = "NKM_UI_LEVELUP";

	private static NKCPopupUserLevelUp m_Instance;

	public Animator m_ani;

	public Text m_lbCurLevel;

	public Text m_lbNextLevel;

	public Text m_lbDesc;

	private Action dOnClose;

	private const int m_iTotalAniFrame = 303;

	private const float m_fTotalAniTime = 5.05f;

	private float m_fStartTime;

	private int m_nextLevel;

	public static NKCPopupUserLevelUp instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupUserLevelUp>("ab_ui_nkm_ui_levelup", "NKM_UI_LEVELUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupUserLevelUp>();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "LevelUp";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		Close();
		if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
		{
			dOnClose?.Invoke();
			dOnClose = null;
			return;
		}
		NKCContentManager.ShowContentUnlockPopup(delegate
		{
			dOnClose?.Invoke();
			dOnClose = null;
		}, STAGE_UNLOCK_REQ_TYPE.SURT_PLAYER_LEVEL);
	}

	public void Open(NKMUserData userData, Action onClose)
	{
		if (userData != null)
		{
			Open(userData.UserLevel - 1, userData.UserLevel, onClose);
		}
	}

	public void Open(int curLevel, int nextLevel, Action onClose)
	{
		m_fStartTime = 0f;
		m_lbCurLevel.text = curLevel.ToString();
		m_lbNextLevel.text = nextLevel.ToString();
		m_nextLevel = nextLevel;
		NKMUserExpTemplet nKMUserExpTemplet = NKMUserExpTemplet.Find(nextLevel);
		if (nKMUserExpTemplet != null)
		{
			m_lbDesc.text = NKCStringTable.GetString(nKMUserExpTemplet.m_strLevelUpDesc);
		}
		else
		{
			m_lbDesc.text = string.Empty;
		}
		dOnClose = onClose;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		StartCoroutine(WaitAnimator());
		UIOpened();
	}

	private IEnumerator WaitAnimator()
	{
		while (!(m_ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			yield return null;
		}
		Close();
	}

	private void Update()
	{
		m_fStartTime += Time.deltaTime;
		if (m_fStartTime > 5.05f)
		{
			StopAllCoroutines();
			Close();
		}
	}
}
