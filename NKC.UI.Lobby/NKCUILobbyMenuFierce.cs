using System;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuFierce : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_btnFierce;

	public GameObject m_objReddot;

	public GameObject m_objClosed;

	public Text m_lbLeftTime;

	private NKMFierceTemplet m_FierceTemplet;

	private bool m_bUseRemainTime;

	private DateTime m_EndDateUTC = DateTime.MinValue;

	private float m_fDeltaTime;

	public void Init(ContentsType contentsType = ContentsType.None)
	{
		NKCUtil.SetGameobjectActive(this, bValue: true);
		NKCUtil.SetButtonClickDelegate(m_btnFierce, OnBtn);
		m_FierceTemplet = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().FierceTemplet;
		m_ContentsType = contentsType;
		m_EndDateUTC = DateTime.MinValue;
		m_bUseRemainTime = false;
		m_btnFierce.m_bGetCallbackWhileLocked = true;
	}

	protected override void UpdateLock()
	{
		m_bLocked = !NKCContentManager.IsContentsUnlocked(m_ContentsType);
		NKCUtil.SetLabelText(m_lbLock, NKCContentManager.GetLockedMessage(m_ContentsType));
		NKCUtil.SetGameobjectActive(m_objLock, m_bLocked);
		if (m_bLocked)
		{
			m_bUseRemainTime = false;
			m_btnFierce.SetLock(value: true);
			NKCUtil.SetGameobjectActive(m_objClosed, bValue: false);
			NKCUtil.SetGameobjectActive(m_objReddot, bValue: false);
		}
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		m_btnFierce.SetLock(value: false);
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		NKCFierceBattleSupportDataMgr.FIERCE_STATUS status = nKCFierceBattleSupportDataMgr.GetStatus();
		if (nKCFierceBattleSupportDataMgr.FierceTemplet == null || !nKCFierceBattleSupportDataMgr.IsCanAccessFierce())
		{
			m_bUseRemainTime = false;
			m_btnFierce.SetLock(value: true);
			NKCUtil.SetGameobjectActive(m_objClosed, bValue: false);
			NKCUtil.SetGameobjectActive(m_objReddot, bValue: false);
			return;
		}
		switch (status)
		{
		case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_UNUSABLE:
		case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_LOCKED:
			m_btnFierce.SetLock(value: true);
			break;
		case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_WAIT:
			m_EndDateUTC = NKMTime.LocalToUTC(nKCFierceBattleSupportDataMgr.FierceTemplet.FierceGameStart);
			break;
		case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE:
			m_EndDateUTC = NKMTime.LocalToUTC(nKCFierceBattleSupportDataMgr.FierceTemplet.FierceGameEnd);
			break;
		case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_REWARD:
		case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_COMPLETE:
			m_EndDateUTC = NKMTime.LocalToUTC(nKCFierceBattleSupportDataMgr.FierceTemplet.FierceRewardPeriodEnd);
			break;
		}
		NKCUtil.SetGameobjectActive(m_objClosed, status == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_REWARD || status == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_COMPLETE);
		NKCUtil.SetGameobjectActive(m_objReddot, status == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_REWARD);
		m_bUseRemainTime = m_EndDateUTC != DateTime.MinValue;
		NKCUtil.SetGameobjectActive(m_lbLeftTime, m_bUseRemainTime);
		if (m_bUseRemainTime)
		{
			UpdateLeftTime();
		}
	}

	private void OnBtn()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.FIERCE))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NORMAL, NKCContentManager.GetLockedMessage(ContentsType.FIERCE));
		}
		else if (m_btnFierce.m_bLock)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NORMAL, NKCStringTable.GetString("SI_PF_POPUP_NO_EVENT"));
		}
		else
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_FIERCE, "");
		}
	}

	public void UpdateLeftTime()
	{
		NKCUtil.SetLabelText(m_lbLeftTime, NKCUtilString.GetRemainTimeString(m_EndDateUTC, 1));
		if (NKCSynchronizedTime.IsFinished(m_EndDateUTC))
		{
			m_bUseRemainTime = false;
		}
	}

	public void Update()
	{
		if (m_bUseRemainTime && m_lbLeftTime != null && m_lbLeftTime.gameObject.activeSelf)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				UpdateLeftTime();
			}
		}
	}
}
