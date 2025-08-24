using System.Collections.Generic;
using System.Linq;
using NKM;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupHiddenOptionPopup : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_FACTORY";

	private const string UI_ASSET_NAME = "NKM_UI_FACTORY_POPUP_HIDDEN_OPTION";

	private static NKCPopupHiddenOptionPopup m_Instance;

	public ScrollRect m_scrollView1;

	public ScrollRect m_scrollView2;

	public ScrollRect m_scrollView3;

	public Transform m_contentSocket1;

	public Transform m_contentSocket2;

	public Transform m_contentSocket3;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnClose;

	public EventTrigger m_eventTriggerBg;

	public NKCPopupEquipOptionListSlot m_prfNKCPopupEquipOptionListSlot;

	private List<List<NKCPopupEquipOptionListSlot>> m_lstOptionSlot = new List<List<NKCPopupEquipOptionListSlot>>();

	public static NKCPopupHiddenOptionPopup Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupHiddenOptionPopup>("AB_UI_NKM_UI_FACTORY", "NKM_UI_FACTORY_POPUP_HIDDEN_OPTION", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupHiddenOptionPopup>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => "Hidden Option";

	public override eMenutype eUIType => eMenutype.Popup;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance?.Release();
		m_Instance = null;
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnOK, base.Close);
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, base.Close);
		if (m_eventTriggerBg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_eventTriggerBg.triggers.Add(entry);
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open(int potentialOpGroupID, int potentialGroupID2)
	{
		NKMPotentialOptionGroupTemplet nKMPotentialOptionGroupTemplet = NKMTempletContainer<NKMPotentialOptionGroupTemplet>.Find(potentialOpGroupID);
		if (nKMPotentialOptionGroupTemplet == null)
		{
			return;
		}
		NKMPotentialOptionGroupTemplet nKMPotentialOptionGroupTemplet2 = null;
		if (potentialGroupID2 > 0)
		{
			nKMPotentialOptionGroupTemplet2 = NKMTempletContainer<NKMPotentialOptionGroupTemplet>.Find(potentialGroupID2);
			if (nKMPotentialOptionGroupTemplet2 == null)
			{
				return;
			}
		}
		int num = nKMPotentialOptionGroupTemplet.OptionList.Count();
		if (potentialGroupID2 > 0)
		{
			num += nKMPotentialOptionGroupTemplet2.OptionList.Count();
		}
		m_lstOptionSlot.Clear();
		for (int i = 0; i < num; i++)
		{
			m_lstOptionSlot.Add(new List<NKCPopupEquipOptionListSlot>());
		}
		InstantiatOptionSlotPrefab(m_contentSocket1, num);
		InstantiatOptionSlotPrefab(m_contentSocket2, num);
		InstantiatOptionSlotPrefab(m_contentSocket3, num);
		int num2 = 0;
		foreach (NKMPotentialOptionTemplet option in nKMPotentialOptionGroupTemplet.OptionList)
		{
			if (option == null)
			{
				int count = m_lstOptionSlot[num2].Count;
				for (int j = 0; j < count; j++)
				{
					NKCUtil.SetLabelText(m_lstOptionSlot[num2][j].m_OPTION_NAME, "");
					NKCUtil.SetGameobjectActive(m_lstOptionSlot[num2][j], bValue: false);
				}
				continue;
			}
			int num3 = Mathf.Min(option.sockets.Length, m_lstOptionSlot[num2].Count);
			bool bPercent = NKMUnitStatManager.IsPercentStat(option.StatType);
			for (int k = 0; k < num3; k++)
			{
				NKMPotentialSocketTemplet nKMPotentialSocketTemplet = option.sockets[k];
				if (nKMPotentialSocketTemplet == null || (nKMPotentialSocketTemplet.MinStat == 0f && nKMPotentialSocketTemplet.MaxStat == 0f))
				{
					NKCUtil.SetLabelText(m_lstOptionSlot[num2][k].m_OPTION_NAME, "");
					NKCUtil.SetGameobjectActive(m_lstOptionSlot[num2][k], bValue: false);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstOptionSlot[num2][k], bValue: true);
					string detailStatShortNameForInvenEquip = NKCUtilString.GetDetailStatShortNameForInvenEquip(option.StatType, nKMPotentialSocketTemplet.MinStat, nKMPotentialSocketTemplet.MaxStat, bPercent);
					NKCUtil.SetLabelText(m_lstOptionSlot[num2][k].m_OPTION_NAME, detailStatShortNameForInvenEquip);
				}
			}
			num2++;
		}
		if (nKMPotentialOptionGroupTemplet2 != null)
		{
			foreach (NKMPotentialOptionTemplet option2 in nKMPotentialOptionGroupTemplet2.OptionList)
			{
				if (option2 == null)
				{
					int count2 = m_lstOptionSlot[num2].Count;
					for (int l = 0; l < count2; l++)
					{
						NKCUtil.SetLabelText(m_lstOptionSlot[num2][l].m_OPTION_NAME, "");
						NKCUtil.SetGameobjectActive(m_lstOptionSlot[num2][l], bValue: false);
					}
					continue;
				}
				int num4 = Mathf.Min(option2.sockets.Length, m_lstOptionSlot[num2].Count);
				bool bPercent2 = NKMUnitStatManager.IsPercentStat(option2.StatType);
				for (int m = 0; m < num4; m++)
				{
					NKMPotentialSocketTemplet nKMPotentialSocketTemplet2 = option2.sockets[m];
					if (nKMPotentialSocketTemplet2 == null || (nKMPotentialSocketTemplet2.MinStat == 0f && nKMPotentialSocketTemplet2.MaxStat == 0f))
					{
						NKCUtil.SetLabelText(m_lstOptionSlot[num2][m].m_OPTION_NAME, "");
						NKCUtil.SetGameobjectActive(m_lstOptionSlot[num2][m], bValue: false);
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_lstOptionSlot[num2][m], bValue: true);
						string detailStatShortNameForInvenEquip2 = NKCUtilString.GetDetailStatShortNameForInvenEquip(option2.StatType, nKMPotentialSocketTemplet2.MinStat, nKMPotentialSocketTemplet2.MaxStat, bPercent2);
						NKCUtil.SetLabelText(m_lstOptionSlot[num2][m].m_OPTION_NAME, detailStatShortNameForInvenEquip2);
					}
				}
				num2++;
			}
		}
		if (m_scrollView1 != null)
		{
			m_scrollView1.verticalNormalizedPosition = 1f;
		}
		if (m_scrollView2 != null)
		{
			m_scrollView2.verticalNormalizedPosition = 1f;
		}
		if (m_scrollView3 != null)
		{
			m_scrollView3.verticalNormalizedPosition = 1f;
		}
		UIOpened();
	}

	private void InstantiatOptionSlotPrefab(Transform scrollContent, int optionCount)
	{
		if (scrollContent == null)
		{
			return;
		}
		int childCount = scrollContent.childCount;
		if (childCount < optionCount)
		{
			for (int i = 0; i < optionCount - childCount; i++)
			{
				Object.Instantiate(m_prfNKCPopupEquipOptionListSlot, scrollContent);
			}
		}
		childCount = scrollContent.childCount;
		for (int j = 0; j < childCount; j++)
		{
			if (j < optionCount)
			{
				m_lstOptionSlot[j].Add(scrollContent.GetChild(j).GetComponent<NKCPopupEquipOptionListSlot>());
				scrollContent.GetChild(j).gameObject.SetActive(value: true);
			}
			else
			{
				scrollContent.GetChild(j).gameObject.SetActive(value: false);
			}
		}
	}

	private void Release()
	{
		m_lstOptionSlot?.Clear();
		m_lstOptionSlot = null;
	}
}
