using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupShipCommandModuleSlot : MonoBehaviour
{
	public enum SHIP_MODULE_STATE
	{
		LOCKED,
		IDLE_01,
		SET_01_TO_02,
		IDLE_02,
		SET_02_TO_01
	}

	public delegate void OnSetting(int slotIndex);

	public Animator m_Ani;

	public List<NKCPopupShipCommandModuleSlotSocket> m_lstSocket = new List<NKCPopupShipCommandModuleSlotSocket>();

	public Image m_imgSlotNum;

	public NKCUIComStateButton m_btnInfo;

	public NKCUIComStateButton m_btnOpen;

	public NKCUIComStateButton m_btnSetting;

	public GameObject m_objLock;

	public GameObject m_objDisable;

	public GameObject m_objOpenFx;

	public GameObject m_objRerollFx;

	public GameObject m_objConfirmFx;

	private OnSetting m_dOnOpen;

	private OnSetting m_dOnClickSetting;

	private int m_moduleIndex = -1;

	public void Init(OnSetting dOnOpen, OnSetting dOnSetting)
	{
		for (int i = 0; i < m_lstSocket.Count; i++)
		{
			m_lstSocket[i].Init();
		}
		m_btnInfo.PointerClick.RemoveAllListeners();
		m_btnInfo.PointerClick.AddListener(OnClickInfo);
		m_btnOpen.PointerClick.RemoveAllListeners();
		m_btnOpen.PointerClick.AddListener(OnClickOpen);
		m_btnOpen.m_bGetCallbackWhileLocked = true;
		m_btnSetting.PointerClick.RemoveAllListeners();
		m_btnSetting.PointerClick.AddListener(OnClickSetting);
		m_dOnOpen = dOnOpen;
		m_dOnClickSetting = dOnSetting;
		NKCUtil.SetGameobjectActive(m_objOpenFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objConfirmFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRerollFx, bValue: false);
	}

	public void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(m_objOpenFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objConfirmFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRerollFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDisable, bValue: false);
		m_moduleIndex = -1;
		for (int i = 0; i < m_lstSocket.Count; i++)
		{
			m_lstSocket[i].CloseInternal();
		}
	}

	public void SetData(int moduleIndex, NKMShipCmdModule curModuleData, NKMShipCmdModule nextModuleData = null)
	{
		NKCUtil.SetGameobjectActive(m_objDisable, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLock, curModuleData == null);
		NKCUtil.SetGameobjectActive(m_btnOpen, curModuleData == null || IsEmptySlot(curModuleData));
		NKCUtil.SetGameobjectActive(m_btnSetting, curModuleData != null && !IsEmptySlot(curModuleData));
		NKCUtil.SetImageSprite(m_imgSlotNum, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_SHIP_INFO_SPRITE", $"NKM_UI_SHIP_MODULE_FONT_SLOT_0{moduleIndex + 1}"));
		if (curModuleData == null)
		{
			m_btnOpen.Lock();
		}
		else
		{
			m_btnOpen.UnLock();
		}
		m_moduleIndex = moduleIndex;
		for (int i = 0; i < m_lstSocket.Count; i++)
		{
			if (curModuleData != null)
			{
				NKCUtil.SetGameobjectActive(m_lstSocket[i], bValue: true);
				m_lstSocket[i].SetData(moduleIndex, i, curModuleData.slots[i], (nextModuleData == null) ? null : nextModuleData.slots[i]);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSocket[i], bValue: false);
				m_lstSocket[i].SetData(moduleIndex, i, null);
			}
		}
	}

	public void SetState(SHIP_MODULE_STATE state)
	{
		switch (state)
		{
		case SHIP_MODULE_STATE.LOCKED:
		case SHIP_MODULE_STATE.IDLE_01:
			m_Ani.SetTrigger("01_IDLE");
			break;
		case SHIP_MODULE_STATE.IDLE_02:
			m_Ani.SetTrigger("02_IDLE");
			break;
		case SHIP_MODULE_STATE.SET_01_TO_02:
			m_Ani.SetTrigger("01_TO_02");
			break;
		case SHIP_MODULE_STATE.SET_02_TO_01:
			m_Ani.SetTrigger("02_TO_01");
			break;
		}
	}

	public void SetDisable(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objDisable, bValue);
	}

	private bool IsEmptySlot(NKMShipCmdModule slotData)
	{
		if (slotData == null || slotData.slots == null)
		{
			return true;
		}
		for (int i = 0; i < slotData.slots.Length; i++)
		{
			if (slotData.slots[i] == null)
			{
				return true;
			}
		}
		return false;
	}

	private void OnClickInfo()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(NKCPopupShipCommandModule.Instance.GetShipData().m_UnitID);
		NKCPopupShipModuleOption.Instance.Open(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, unitTempletBase.m_NKM_UNIT_GRADE, m_moduleIndex);
	}

	private void OnClickOpen()
	{
		if (m_btnOpen.m_bLock)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_SHIP_COMMAND_MODULE_SLOT_NOT_OPEN);
		}
		else
		{
			m_dOnOpen(m_moduleIndex);
		}
	}

	private void OnClickSetting()
	{
		m_dOnClickSetting(m_moduleIndex);
	}

	public void ShowOpenFx()
	{
		NKCUtil.SetGameobjectActive(m_objOpenFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOpenFx, bValue: true);
	}

	public void ShowRerollFx()
	{
		NKCUtil.SetGameobjectActive(m_objRerollFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRerollFx, bValue: true);
		for (int i = 0; i < m_lstSocket.Count; i++)
		{
			m_lstSocket[i].ShowRerollFx();
		}
	}

	public void ShowConfirmFx()
	{
		NKCUtil.SetGameobjectActive(m_objConfirmFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objConfirmFx, bValue: true);
	}

	public void DisableAllFx()
	{
		NKCUtil.SetGameobjectActive(m_objOpenFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRerollFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objConfirmFx, bValue: false);
		for (int i = 0; i < m_lstSocket.Count; i++)
		{
			m_lstSocket[i].DisableAllFx();
		}
	}
}
