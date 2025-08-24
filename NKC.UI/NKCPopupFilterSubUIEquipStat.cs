using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupFilterSubUIEquipStat : NKCUIBase
{
	public delegate void OnClickStatSlot(NKM_STAT_TYPE statType, int setOptionID, int selectedSlotIdx);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_SELECTION";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FILTER_EQUIP_STAT";

	private static NKCPopupFilterSubUIEquipStat m_Instance;

	public NKCPopupFilterSubUIEquipStatSlot m_pfbSlot;

	public Text m_lbTitle;

	public ScrollRect m_sr;

	public Transform m_trContents;

	public Transform m_trObjPool;

	public GameObject m_objArrow;

	public GameObject m_objBG;

	private List<NKCPopupFilterSubUIEquipStatSlot> m_lstVisible = new List<NKCPopupFilterSubUIEquipStatSlot>();

	private Stack<NKCPopupFilterSubUIEquipStatSlot> m_stkSlot = new Stack<NKCPopupFilterSubUIEquipStatSlot>();

	private int m_selectedIdx = -1;

	public static NKCPopupFilterSubUIEquipStat Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFilterSubUIEquipStat>("AB_UI_NKM_UI_POPUP_SELECTION", "NKM_UI_POPUP_FILTER_EQUIP_STAT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFilterSubUIEquipStat>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static bool IsInstanceOpen()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			return true;
		}
		return false;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		EventTrigger eventTrigger = m_objBG.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = m_objBG.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		eventTrigger.triggers.Clear();
		eventTrigger.triggers.Add(entry);
	}

	public void Open(bool bShowSetOption, List<NKM_STAT_TYPE> lstSelectedStatType, OnClickStatSlot onClickStatSlot, int selectedSlotIdx)
	{
		if (m_selectedIdx == selectedSlotIdx)
		{
			Close();
			return;
		}
		m_selectedIdx = selectedSlotIdx;
		ResetSlot();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objBG, bValue: false);
		NKCUtil.SetGameobjectActive(m_objArrow, bValue: true);
		if (bShowSetOption)
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_FILTER_EQUIP_TYPE_STAT_SET);
			for (int i = 0; i < NKMItemManager.m_lstItemEquipSetOptionTemplet.Count; i++)
			{
				if (NKMItemManager.m_lstItemEquipSetOptionTemplet[i].UseFilter && NKMOpenTagManager.IsOpened(NKMItemManager.m_lstItemEquipSetOptionTemplet[i].m_OpenTag))
				{
					NKCPopupFilterSubUIEquipStatSlot slot = GetSlot();
					slot.transform.SetParent(m_trContents);
					slot.GetButton().PointerClick.RemoveAllListeners();
					slot.GetButton().PointerClick.AddListener(delegate
					{
						onClickStatSlot(slot.GetStatType(), slot.GetSetOptionID(), selectedSlotIdx);
					});
					m_lstVisible.Add(slot);
					NKCUtil.SetGameobjectActive(slot, bValue: true);
					slot.SetData(NKMItemManager.m_lstItemEquipSetOptionTemplet[i]);
				}
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTitle, GetTitle(selectedSlotIdx));
			foreach (NKCStatInfoTemplet value in NKMTempletContainer<NKCStatInfoTemplet>.Values)
			{
				if (value.StatType != NKM_STAT_TYPE.NST_RANDOM && value.UseFilter)
				{
					NKCPopupFilterSubUIEquipStatSlot slot2 = GetSlot();
					slot2.transform.SetParent(m_trContents);
					slot2.GetButton().PointerClick.RemoveAllListeners();
					slot2.GetButton().PointerClick.AddListener(delegate
					{
						onClickStatSlot(slot2.GetStatType(), slot2.GetSetOptionID(), selectedSlotIdx);
					});
					m_lstVisible.Add(slot2);
					NKCUtil.SetGameobjectActive(slot2, bValue: true);
					bool flag = lstSelectedStatType.Contains(value.StatType);
					slot2.SetData(value.StatType, flag);
					if (flag)
					{
						slot2.GetButton().Lock();
					}
					else
					{
						slot2.GetButton().UnLock();
					}
				}
			}
		}
		UIOpened();
	}

	public void Open(List<NKM_STAT_TYPE> lstStatType, int selectedIdx)
	{
		ResetSlot();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objBG, bValue: true);
		NKCUtil.SetGameobjectActive(m_objArrow, bValue: false);
		NKCUtil.SetLabelText(m_lbTitle, GetTitle(selectedIdx));
		int i;
		for (i = 0; i < lstStatType.Count; i++)
		{
			NKCStatInfoTemplet nKCStatInfoTemplet = NKMTempletContainer<NKCStatInfoTemplet>.Find((NKCStatInfoTemplet x) => x.StatType == lstStatType[i]);
			if (nKCStatInfoTemplet != null && nKCStatInfoTemplet.StatType != NKM_STAT_TYPE.NST_RANDOM && nKCStatInfoTemplet.UseFilter)
			{
				NKCPopupFilterSubUIEquipStatSlot slot = GetSlot();
				slot.transform.SetParent(m_trContents);
				slot.GetButton().PointerClick.RemoveAllListeners();
				m_lstVisible.Add(slot);
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				slot.SetData(nKCStatInfoTemplet.StatType);
			}
		}
		UIOpened();
	}

	public void Open(List<int> lstSetOptions)
	{
		ResetSlot();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objBG, bValue: true);
		NKCUtil.SetGameobjectActive(m_objArrow, bValue: false);
		NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_FILTER_EQUIP_TYPE_STAT_SET);
		for (int i = 0; i < lstSetOptions.Count; i++)
		{
			NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(lstSetOptions[i]);
			if (equipSetOptionTemplet != null && equipSetOptionTemplet.UseFilter && NKMOpenTagManager.IsOpened(equipSetOptionTemplet.m_OpenTag))
			{
				NKCPopupFilterSubUIEquipStatSlot slot = GetSlot();
				slot.transform.SetParent(m_trContents);
				slot.GetButton().PointerClick.RemoveAllListeners();
				m_lstVisible.Add(slot);
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				slot.SetData(lstSetOptions[i]);
			}
		}
		UIOpened();
	}

	private string GetTitle(int selectedIdx)
	{
		return selectedIdx switch
		{
			0 => NKCUtilString.GET_STRING_FILTER_EQUIP_TYPE_STAT_MAIN, 
			1 => NKCUtilString.GET_STRING_FILTER_EQUIP_TYPE_STAT_SUB1, 
			2 => NKCUtilString.GET_STRING_EQUIP_FILTER_HIDDEN_OPTION, 
			_ => "", 
		};
	}

	private NKCPopupFilterSubUIEquipStatSlot GetSlot()
	{
		if (m_stkSlot.Count > 0)
		{
			return m_stkSlot.Pop();
		}
		return Object.Instantiate(m_pfbSlot, m_trContents);
	}

	public override void CloseInternal()
	{
		m_selectedIdx = -1;
		ResetSlot();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnDestroy()
	{
		while (m_stkSlot.Count > 0)
		{
			Object.Destroy(m_stkSlot.Pop());
		}
		for (int num = m_lstVisible.Count - 1; num >= 0; num--)
		{
			Object.Destroy(m_lstVisible[num]);
		}
	}

	private void ResetSlot()
	{
		for (int i = 0; i < m_lstVisible.Count; i++)
		{
			m_lstVisible[i].GetButton().UnLock();
			m_lstVisible[i].transform.SetParent(m_trObjPool);
			NKCUtil.SetGameobjectActive(m_lstVisible[i], bValue: false);
			m_stkSlot.Push(m_lstVisible[i]);
		}
		m_lstVisible.Clear();
	}

	public void Clean()
	{
		while (m_stkSlot.Count > 0)
		{
			Object.Destroy(m_stkSlot.Pop());
		}
	}
}
