using System;
using System.Collections.Generic;
using ClientPacket.Item;
using DG.Tweening;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeCraftSlot : MonoBehaviour
{
	public delegate void OnClickSelect(int index);

	public GameObject m_NKM_UI_FACTORY_CRAFT_SLOT_BG_LOCK;

	public GameObject m_NKM_UI_FACTORY_CRAFT_SLOT_BG_NO_LOCK;

	public Text m_NKM_UI_FACTORY_CRAFT_SLOT_NUMBER;

	public Text m_NKM_UI_FACTORY_CRAFT_SLOT_TEXT_NUM_FOR_LOCK;

	public NKCUISlot m_AB_ICON_SLOT;

	public Text m_NKM_UI_FACTORY_CRAFT_SLOT_NAME;

	public GameObject m_NKM_UI_FACTORY_CRAFT_SLOT_TIME;

	public Text m_NKM_UI_FACTORY_CRAFT_SLOT_TEXT;

	public Text m_NKM_UI_FACTORY_CRAFT_SLOT_TIME_Text;

	public GameObject m_BUTTON_GET;

	public GameObject m_BUTTON_INSTANT_CRAFT;

	public GameObject m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_ANIM;

	public GameObject m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PART_COMPLETE;

	public GameObject m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PART_00;

	public GameObject m_NKM_UI_FACTORY_CRAFT_SLOT_BG_COMPLETE;

	public NKCUIComButton m_NKM_UI_FACTORY_CRAFT_SLOT_BG;

	public NKCUIComButton m_BUTTON;

	public NKCUIComButton m_BUTTON_SELECT;

	public NKCUIComButton m_NKM_UI_FACTORY_CRAFT_SLOT_BUTTONS_INSTANT;

	public List<DOTweenAnimation> m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PARTS;

	private bool? m_bPlaying;

	private int m_Index = -1;

	private OnClickSelect m_dOnClickSelect;

	private OnClickSelect m_dOnClickGet;

	private OnClickSelect m_dOnClickInstanceGet;

	private NKMCraftSlotData m_NKMEquipCreationSlotData;

	public void SetIndex(int index)
	{
		m_Index = index;
	}

	public int GetIndex()
	{
		return m_Index;
	}

	private void OnClickBG()
	{
		if (m_NKM_UI_FACTORY_CRAFT_SLOT_BG_LOCK.activeSelf)
		{
			OnClickUnLock();
		}
		else if (!m_BUTTON_INSTANT_CRAFT.gameObject.activeSelf)
		{
			OnClickGetOrSelect();
		}
		else
		{
			OnClickInstantCraft();
		}
	}

	public void Init(int index, OnClickSelect dOnClickSelect = null, OnClickSelect dOnClickGet = null, OnClickSelect dOnClickInstanceGet = null)
	{
		SetIndex(index);
		m_dOnClickSelect = dOnClickSelect;
		m_dOnClickGet = dOnClickGet;
		m_dOnClickInstanceGet = dOnClickInstanceGet;
		if (m_AB_ICON_SLOT != null)
		{
			m_AB_ICON_SLOT.Init();
		}
		m_NKM_UI_FACTORY_CRAFT_SLOT_BG.PointerClick.RemoveAllListeners();
		m_NKM_UI_FACTORY_CRAFT_SLOT_BG.PointerClick.AddListener(OnClickBG);
		m_BUTTON.PointerClick.RemoveAllListeners();
		m_BUTTON.PointerClick.AddListener(OnClickGetOrSelect);
		m_BUTTON_SELECT.PointerClick.RemoveAllListeners();
		m_BUTTON_SELECT.PointerClick.AddListener(OnClickGetOrSelect);
		if (m_NKM_UI_FACTORY_CRAFT_SLOT_BUTTONS_INSTANT != null)
		{
			m_NKM_UI_FACTORY_CRAFT_SLOT_BUTTONS_INSTANT.PointerClick.RemoveAllListeners();
			m_NKM_UI_FACTORY_CRAFT_SLOT_BUTTONS_INSTANT.PointerClick.AddListener(OnClickInstantCraft);
		}
	}

	public void OnClickUnLock()
	{
		if (m_Index >= 1 && m_Index <= NKMCraftData.MAX_CRAFT_SLOT_DATA)
		{
			NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_CRAFT_SLOT_ADD, 101, 300, UnLockConfirm);
		}
	}

	public void UnLockConfirm()
	{
		NKMPacket_CRAFT_UNLOCK_SLOT_REQ packet = new NKMPacket_CRAFT_UNLOCK_SLOT_REQ();
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void SetLockUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_BG_LOCK, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_BG_NO_LOCK, bValue: false);
		m_NKM_UI_FACTORY_CRAFT_SLOT_TEXT_NUM_FOR_LOCK.text = m_Index.ToString();
	}

	private void PlayAni()
	{
		if (!m_bPlaying.HasValue || m_bPlaying != true)
		{
			m_bPlaying = true;
			for (int i = 0; i < m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PARTS.Count; i++)
			{
				m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PARTS[i].DOPlay();
			}
		}
	}

	private void StopAni()
	{
		if (!m_bPlaying.HasValue || m_bPlaying != false)
		{
			m_bPlaying = false;
			for (int i = 0; i < m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PARTS.Count; i++)
			{
				m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PARTS[i].DOPause();
			}
		}
	}

	public void ResetUI(bool bUpdateIconSlot = true)
	{
		if (m_Index < 1 || m_Index > NKMCraftData.MAX_CRAFT_SLOT_DATA)
		{
			SetLockUI();
			return;
		}
		NKMCraftData craftData = NKCScenManager.GetScenManager().GetMyUserData().m_CraftData;
		if (craftData == null)
		{
			SetLockUI();
			return;
		}
		NKMCraftSlotData nKMCraftSlotData = (m_NKMEquipCreationSlotData = craftData.GetSlotData((byte)m_Index));
		if (nKMCraftSlotData == null)
		{
			SetLockUI();
			return;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_BG_LOCK, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_BG_NO_LOCK, bValue: true);
		if (nKMCraftSlotData.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_EMPTY)
		{
			NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT.gameObject, bValue: false);
			m_NKM_UI_FACTORY_CRAFT_SLOT_NAME.text = NKCUtilString.GET_STRING_FORGE_CRAFT_WAIT_NAME;
			m_NKM_UI_FACTORY_CRAFT_SLOT_TEXT.text = NKCUtilString.GET_STRING_FORGE_CRAFT_WAIT_TEXT;
			NKCUtil.SetGameobjectActive(m_BUTTON_GET, bValue: false);
			NKCUtil.SetGameobjectActive(m_BUTTON_INSTANT_CRAFT, bValue: false);
			NKCUtil.SetGameobjectActive(m_BUTTON_SELECT, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_ANIM, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PART_COMPLETE, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_TIME, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_BG_COMPLETE, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PART_00, bValue: false);
			StopAni();
			m_NKM_UI_FACTORY_CRAFT_SLOT_NUMBER.text = m_Index.ToString();
		}
		else if (nKMCraftSlotData.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW)
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(nKMCraftSlotData.MoldID);
			if (itemMoldTempletByID != null)
			{
				NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT.gameObject, bValue: true);
				if (bUpdateIconSlot)
				{
					m_AB_ICON_SLOT.SetData(NKCUISlot.SlotData.MakeMoldItemData(itemMoldTempletByID.m_MoldID, nKMCraftSlotData.Count), bShowName: false, bShowNumber: true, bEnableLayoutElement: true, null);
				}
				m_NKM_UI_FACTORY_CRAFT_SLOT_NAME.text = itemMoldTempletByID.GetItemName();
				m_NKM_UI_FACTORY_CRAFT_SLOT_TEXT.text = NKCUtilString.GET_STRING_FORGE_CRAFT_ING_TEXT;
				NKCUtil.SetGameobjectActive(m_BUTTON_GET, bValue: false);
				NKCUtil.SetGameobjectActive(m_BUTTON_INSTANT_CRAFT, bValue: true);
				NKCUtil.SetGameobjectActive(m_BUTTON_SELECT, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_ANIM, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PART_COMPLETE, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_TIME, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_BG_COMPLETE, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PART_00, bValue: true);
				PlayAni();
				TimeSpan timeSpan = new TimeSpan(nKMCraftSlotData.CompleteDate - NKCSynchronizedTime.GetServerUTCTime().Ticks);
				m_NKM_UI_FACTORY_CRAFT_SLOT_TIME_Text.text = NKCUtilString.GetTimeSpanString(timeSpan);
				m_NKM_UI_FACTORY_CRAFT_SLOT_NUMBER.text = m_Index.ToString();
			}
			else
			{
				SetLockUI();
			}
		}
		else if (nKMCraftSlotData.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED)
		{
			NKMItemMoldTemplet itemMoldTempletByID2 = NKMItemManager.GetItemMoldTempletByID(nKMCraftSlotData.MoldID);
			if (itemMoldTempletByID2 != null)
			{
				NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT.gameObject, bValue: true);
				if (bUpdateIconSlot)
				{
					m_AB_ICON_SLOT.SetData(NKCUISlot.SlotData.MakeMoldItemData(itemMoldTempletByID2.m_MoldID, nKMCraftSlotData.Count));
				}
				m_NKM_UI_FACTORY_CRAFT_SLOT_NAME.text = itemMoldTempletByID2.GetItemName();
				m_NKM_UI_FACTORY_CRAFT_SLOT_TEXT.text = NKCUtilString.GET_STRING_FORGE_CRAFT_COMPLETED_TEXT;
				NKCUtil.SetGameobjectActive(m_BUTTON_GET, bValue: true);
				NKCUtil.SetGameobjectActive(m_BUTTON_INSTANT_CRAFT, bValue: false);
				NKCUtil.SetGameobjectActive(m_BUTTON_SELECT, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_ANIM, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PART_COMPLETE, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_TIME, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_BG_COMPLETE, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_CRAFT_SLOT_PROGRESS_PART_00, bValue: true);
				StopAni();
				m_NKM_UI_FACTORY_CRAFT_SLOT_NUMBER.text = m_Index.ToString();
			}
			else
			{
				SetLockUI();
			}
		}
		else
		{
			SetLockUI();
		}
	}

	public void OnClickGetOrSelect()
	{
		if (m_NKMEquipCreationSlotData == null)
		{
			return;
		}
		if (m_NKMEquipCreationSlotData.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED)
		{
			if (m_dOnClickGet != null)
			{
				m_dOnClickGet(m_Index);
			}
		}
		else if (m_NKMEquipCreationSlotData.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_EMPTY && m_dOnClickSelect != null)
		{
			m_dOnClickSelect(m_Index);
		}
	}

	public void OnClickInstantCraft()
	{
		if (m_NKMEquipCreationSlotData != null && m_NKMEquipCreationSlotData.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW && m_dOnClickInstanceGet != null)
		{
			m_dOnClickInstanceGet(m_Index);
		}
	}

	public RectTransform GetButtonRect()
	{
		if (m_NKMEquipCreationSlotData == null)
		{
			return null;
		}
		return m_NKMEquipCreationSlotData.GetState(NKCSynchronizedTime.GetServerUTCTime()) switch
		{
			NKM_CRAFT_SLOT_STATE.NECSS_EMPTY => m_BUTTON_SELECT.GetComponent<RectTransform>(), 
			NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW => m_BUTTON_INSTANT_CRAFT.GetComponent<RectTransform>(), 
			NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED => m_BUTTON_GET.GetComponent<RectTransform>(), 
			_ => null, 
		};
	}
}
