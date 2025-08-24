using System.Collections.Generic;
using NKM.Contract2;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildAttendance : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_ATTENDANCE";

	private static NKCPopupGuildAttendance m_Instance;

	public Text m_lbLastAttendanceCount;

	public NKCPopupGuildAttendanceSlot m_pfbSlot;

	public Transform m_trSlotParent;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnOK;

	public GameObject m_objInvalid;

	private List<NKCPopupGuildAttendanceSlot> m_lstSlot = new List<NKCPopupGuildAttendanceSlot>();

	public static NKCPopupGuildAttendance Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildAttendance>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_POPUP_ATTENDANCE", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildAttendance>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			Object.Destroy(m_lstSlot[i].gameObject);
		}
		m_lstSlot.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(base.Close);
		NKCUtil.SetHotkey(m_btnOK, HotkeyEventType.Confirm);
	}

	public void Open(int lastAttendanceCount)
	{
		NKCUtil.SetGameobjectActive(m_objInvalid, NKCGuildManager.IsFirstDay());
		NKCUtil.SetLabelText(m_lbLastAttendanceCount, lastAttendanceCount.ToString());
		m_lstSlot.Clear();
		for (int i = 0; i < GuildAttendanceTemplet.Instance.BasicRewards.Count; i++)
		{
			RewardUnit reward = GuildAttendanceTemplet.Instance.BasicRewards[i];
			NKCPopupGuildAttendanceSlot nKCPopupGuildAttendanceSlot = Object.Instantiate(m_pfbSlot, m_trSlotParent);
			nKCPopupGuildAttendanceSlot.SetData(0, reward, bComplete: false);
			m_lstSlot.Add(nKCPopupGuildAttendanceSlot);
		}
		for (int j = 0; j < GuildAttendanceTemplet.Instance.AdditionalRewards.Count; j++)
		{
			GuildAttendanceTemplet.AdditionalReward additionalReward = GuildAttendanceTemplet.Instance.AdditionalRewards[j];
			NKCPopupGuildAttendanceSlot nKCPopupGuildAttendanceSlot2 = Object.Instantiate(m_pfbSlot, m_trSlotParent);
			nKCPopupGuildAttendanceSlot2.SetData(additionalReward.AttendanceCount, additionalReward.Item, additionalReward.AttendanceCount <= lastAttendanceCount);
			m_lstSlot.Add(nKCPopupGuildAttendanceSlot2);
		}
		UIOpened();
	}
}
