using System;
using System.Collections.Generic;
using NKC.UI.Guide;
using NKC.UI.Option;
using NKM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUpsideMenu : MonoBehaviour
{
	[Serializable]
	public struct ResourceUI
	{
		public GameObject objRoot;

		public Image imgIcon;

		public Text lbCount;
	}

	public enum eMode
	{
		Disable,
		Normal,
		BackButtonOnly,
		LeftsideOnly,
		LeftsideWithHamburger,
		ResourceOnly,
		Invalid
	}

	private bool m_bOpen;

	public GameObject m_objContentRoot;

	public GameObject objBackMenuRoot;

	public GameObject objResourceRoot;

	public GameObject objSubMenuRoot;

	[Header("BackMenu")]
	public NKCUIComButton btnBackButton;

	public Text lbBackButtonTitleText;

	[Header("Guide")]
	public GameObject goRootGuide;

	public NKCUIComStateButton csbtnGuide;

	[Header("Home")]
	public GameObject objHome;

	public NKCUIComButton cbtnHome;

	[Header("Canvas Groups")]
	public CanvasGroup cgBackButton;

	public CanvasGroup cgSubMenu;

	public CanvasGroup cgResources;

	[Header("Resources")]
	public NKCUIComItemCount m_UICountCredit;

	public NKCUIComItemCount m_UICountEternium;

	public Image m_imgEterniumCap;

	public NKCUIComItemCount m_UICountCash;

	public NKCUIComItemCount m_UICountInformation;

	public NKCUIComItemCount m_UICountDailyTicket;

	public NKCUIComItemCount m_UICountContract;

	public NKCUIComItemCount m_UICountContractSP;

	public NKCUIComItemCount m_UICountContractInstant;

	public NKCUIComItemCount m_UICountContractShip;

	public NKCUIComItemCount m_UICountContractInstantShip;

	public NKCUIComItemCount m_UICountPVPPoint;

	public NKCUIComItemCount m_UICountPVPTicket;

	public NKCUIRechargePvpAsyncTicket m_RechargeAsyncTicket;

	public List<NKCUIComItemCount> m_lstUICountEtc;

	[Header("Rightmost Icons")]
	public NKCUIComStateButton m_btnChat;

	public NKCUIComButton m_cbtnMail;

	public GameObject objNewMail;

	public NKCUIComButton m_cbtnOption;

	public NKCUIComStateButton m_cBtnHamburger;

	public GameObject m_objHamburgerNotify;

	[Header("Etc")]
	public NKCUIRectMove m_rmRectMove;

	private List<int> m_lstShowResourceList;

	private string m_GuideStrID;

	private bool m_bIsSetOtherParent;

	private NKCUIComItemCount GetUIItemCount(int itemID)
	{
		return itemID switch
		{
			1 => m_UICountCredit, 
			2 => m_UICountEternium, 
			101 => m_UICountCash, 
			3 => m_UICountInformation, 
			1001 => m_UICountContract, 
			401 => m_UICountContractSP, 
			1002 => m_UICountContractInstant, 
			5 => m_UICountPVPPoint, 
			13 => m_UICountPVPTicket, 
			1015 => m_UICountContractShip, 
			_ => m_lstUICountEtc.Find((NKCUIComItemCount x) => x.CurrentItemID == itemID), 
		};
	}

	public void InitUI()
	{
		base.gameObject.SetActive(value: false);
		SetMoveToShop(m_UICountCredit);
		SetMoveToShop(m_UICountEternium);
		SetMoveToShop(m_UICountCash);
		SetMoveToShop(m_UICountInformation);
		SetMoveToShop(m_UICountDailyTicket);
		SetMoveToShop(m_UICountContract);
		SetMoveToShop(m_UICountContractSP);
		SetMoveToShop(m_UICountContractInstant);
		SetMoveToShop(m_UICountContractShip);
		SetMoveToShop(m_UICountContractInstantShip);
		SetMoveToShop(m_UICountPVPPoint);
		SetMoveToShop(m_UICountPVPTicket);
		foreach (NKCUIComItemCount item in m_lstUICountEtc)
		{
			SetMoveToShop(item);
		}
		if (m_btnChat != null)
		{
			m_btnChat.PointerClick.RemoveAllListeners();
			m_btnChat.PointerClick.AddListener(OnChatButton);
		}
		if (m_cbtnMail != null)
		{
			m_cbtnMail.PointerClick.RemoveAllListeners();
			m_cbtnMail.PointerClick.AddListener(OnMailButton);
		}
		if (m_cbtnOption != null)
		{
			m_cbtnOption.PointerClick.RemoveAllListeners();
			m_cbtnOption.PointerClick.AddListener(OnOptionButton);
		}
		if (m_cBtnHamburger != null)
		{
			m_cBtnHamburger.PointerClick.RemoveAllListeners();
			m_cBtnHamburger.PointerClick.AddListener(OnHamburgerButton);
		}
		if (csbtnGuide != null)
		{
			csbtnGuide.PointerClick.RemoveAllListeners();
			csbtnGuide.PointerClick.AddListener(OnInformationButton);
		}
		if (cbtnHome != null)
		{
			cbtnHome.PointerClick.RemoveAllListeners();
			cbtnHome.PointerClick.AddListener(OnHomeButton);
		}
		NKCMailManager.dOnMailFlagChange = (NKCMailManager.OnMailFlagChange)Delegate.Combine(NKCMailManager.dOnMailFlagChange, new NKCMailManager.OnMailFlagChange(SetNewMail));
		if (m_UICountPVPTicket != null)
		{
			m_UICountPVPTicket.SetMaxCount(NKMPvpCommonConst.Instance.AsyncTicketMaxCount);
			m_UICountPVPTicket.SetTimeLabel(NKCUtilString.GET_STRING_UPSIDE_MENU_WAIT_ITEM);
		}
	}

	public void UpdateTimeContents()
	{
		RfreshDailyContents();
	}

	public void RfreshDailyContents()
	{
	}

	public void Update()
	{
		if (m_bOpen)
		{
			ProcessHotkey();
		}
	}

	private void ProcessHotkey()
	{
		eMode topmoseUIUpsidemenuMode = NKCUIManager.GetTopmoseUIUpsidemenuMode();
		if (topmoseUIUpsidemenuMode != eMode.Disable && (uint)(topmoseUIUpsidemenuMode - 5) > 1u)
		{
			if (!string.IsNullOrEmpty(m_GuideStrID) && csbtnGuide.gameObject.activeInHierarchy && NKCInputManager.CheckHotKeyEvent(HotkeyEventType.Help))
			{
				NKCInputManager.ConsumeHotKeyEvent(HotkeyEventType.Help);
				OnInformationButton();
			}
			if (m_cBtnHamburger.gameObject.activeInHierarchy && NKCInputManager.CheckHotKeyEvent(HotkeyEventType.HamburgerMenu))
			{
				OnHamburgerButton();
			}
			if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
			{
				NKCUIComHotkeyDisplay.OpenInstance(csbtnGuide?.transform, HotkeyEventType.Help);
				NKCUIComHotkeyDisplay.OpenInstance(btnBackButton?.transform, HotkeyEventType.Cancel);
				NKCUIComHotkeyDisplay.OpenInstance(m_cBtnHamburger?.transform, HotkeyEventType.HamburgerMenu);
			}
		}
	}

	private void SetMoveToShop(NKCUIComItemCount targetButton)
	{
		if (!(targetButton == null))
		{
			targetButton.SetOnClickPlusBtn(targetButton.OpenMoveToShopPopup);
		}
	}

	public void Move(bool bOutsideScreen, bool bAnimate)
	{
		if (m_rmRectMove != null)
		{
			string text = (bOutsideScreen ? "Out" : "In");
			m_rmRectMove.Move(text, bAnimate);
		}
	}

	private void OpenWithBackButton(string CurrentMenuName, UnityAction OnBackButton, bool bShowHome = false, bool bShowResource = true, bool bShowPost = true, bool bShowOption = true, bool bShowHamburger = true)
	{
		NKCUtil.SetGameobjectActive(objBackMenuRoot, bValue: true);
		OpenCommonAction(bShowHome, bShowResource, bShowPost, bShowOption, bShowHamburger);
		if (lbBackButtonTitleText != null && !string.IsNullOrEmpty(CurrentMenuName))
		{
			lbBackButtonTitleText.text = CurrentMenuName;
		}
		if (btnBackButton != null)
		{
			btnBackButton.PointerClick.RemoveAllListeners();
			if (OnBackButton != null)
			{
				btnBackButton.PointerClick.AddListener(OnBackButton);
			}
		}
	}

	public void OnHomeButton()
	{
		NKCUIManager.OnHomeButton();
	}

	public void OnInformationButton()
	{
		NKCUIPopUpGuide.Instance.Open(m_GuideStrID);
	}

	public void OnChatButton()
	{
	}

	public void OnMailButton()
	{
		if (NKCUIMail.IsInstanceOpen)
		{
			NKCUIMail.Instance.Close();
		}
		else
		{
			NKCUIMail.Instance.Open();
		}
	}

	public void OnOptionButton()
	{
		if (NKCUIGameOption.IsInstanceOpen)
		{
			NKCUIGameOption.Instance.Close();
		}
		else
		{
			NKCUIGameOption.Instance.Open(NKC_GAME_OPTION_MENU_TYPE.NORMAL);
		}
	}

	public void OnHamburgerButton()
	{
		NKCPopupHamburgerMenu.instance.OpenUI();
	}

	private void UpdateEterniumCap()
	{
		if (NKCScenManager.CurrentUserData() != null)
		{
			NKCUtil.SetImageFillAmount(m_imgEterniumCap, NKCScenManager.CurrentUserData().GetEterniumCapProgress());
		}
	}

	public void SetNewMail(bool bValue)
	{
		NKCUtil.SetGameobjectActive(objNewMail, bValue);
	}

	public void SetHamburgerNotify(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objHamburgerNotify, bValue);
	}

	private void OpenCommonAction(bool bShowHome, bool bShowResource, bool bShowPost, bool bShowOption, bool bShowHamburger)
	{
		NKCUtil.SetGameobjectActive(objHome, bShowHome);
		if (objResourceRoot != null && objResourceRoot.activeSelf == !bShowResource)
		{
			objResourceRoot.SetActive(bShowResource);
		}
		bool flag = bShowPost || bShowOption || bShowHamburger;
		if (objSubMenuRoot != null && objSubMenuRoot.activeSelf == !flag)
		{
			objSubMenuRoot.SetActive(flag);
		}
		if (flag)
		{
			NKCUtil.SetGameobjectActive(m_cbtnMail, bShowPost);
			NKCUtil.SetGameobjectActive(m_cbtnOption, bShowOption);
			NKCUtil.SetGameobjectActive(m_cBtnHamburger, bShowHamburger);
		}
	}

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (base.gameObject.activeInHierarchy && m_lstShowResourceList != null)
		{
			NKCUIComItemCount uIItemCount = GetUIItemCount(itemData.ItemID);
			if (uIItemCount != null)
			{
				uIItemCount.UpdateData(itemData);
			}
			if (itemData.ItemID == 13)
			{
				m_RechargeAsyncTicket?.UpdateData();
			}
			if (itemData.ItemID == 2)
			{
				UpdateEterniumCap();
			}
		}
	}

	public void RegisterUserdataCallback(NKMUserData newUserData)
	{
		newUserData.m_InventoryData.dOnMiscInventoryUpdate += OnInventoryChange;
	}

	public void Open(List<int> lstShowResource, eMode mode, string name, string guideTempletStrID, bool disableSubMenu)
	{
		m_bOpen = true;
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
		{
			mode = eMode.BackButtonOnly;
		}
		switch (mode)
		{
		case eMode.Normal:
			OpenWithBackButton(name, OnBackButton, bShowHome: true);
			UpdateResourceUI(NKCScenManager.CurrentUserData(), lstShowResource);
			SetHamburgerNotify(NKCAlarmManager.CheckAllNotify(NKCScenManager.CurrentUserData()));
			break;
		case eMode.LeftsideOnly:
			OpenWithBackButton(name, OnBackButton, bShowHome: true, bShowResource: false, bShowPost: false, bShowOption: false, bShowHamburger: false);
			break;
		case eMode.LeftsideWithHamburger:
			OpenWithBackButton(name, OnBackButton, bShowHome: true, bShowResource: false, bShowPost: false, bShowOption: false);
			SetHamburgerNotify(NKCAlarmManager.CheckAllNotify(NKCScenManager.CurrentUserData()));
			break;
		case eMode.BackButtonOnly:
			OpenWithBackButton(name, OnBackButton, bShowHome: false, bShowResource: false, bShowPost: false, bShowOption: false, bShowHamburger: false);
			break;
		}
		if (mode == eMode.ResourceOnly)
		{
			SetParentOfResourceUI(NKCUIManager.eUIBaseRect.UIFrontPopup);
			UpdateResourceUI(NKCScenManager.CurrentUserData(), lstShowResource);
		}
		else
		{
			ResetParentOfResourceUI();
		}
		m_GuideStrID = guideTempletStrID;
		NKCUtil.SetGameobjectActive(goRootGuide, !string.IsNullOrEmpty(guideTempletStrID));
		if (disableSubMenu)
		{
			cgBackButton.alpha = 0.5f;
			cgBackButton.blocksRaycasts = false;
			cgSubMenu.alpha = 0.5f;
			cgSubMenu.blocksRaycasts = false;
			cgResources.blocksRaycasts = false;
		}
		else
		{
			cgBackButton.alpha = 1f;
			cgBackButton.blocksRaycasts = true;
			cgSubMenu.alpha = 1f;
			cgSubMenu.blocksRaycasts = true;
			cgResources.blocksRaycasts = true;
		}
		Move(bOutsideScreen: false, bAnimate: true);
	}

	private void OnBackButton()
	{
		NKCUIManager.OnBackButton();
	}

	private void UpdateResourceUI(NKMUserData userData, List<int> lstShowResource)
	{
		m_lstShowResourceList = lstShowResource;
		foreach (NKCUIComItemCount item in m_lstUICountEtc)
		{
			item.CleanUp();
			NKCUtil.SetGameobjectActive(item, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_UICountCredit, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountEternium, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountCash, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountInformation, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountDailyTicket, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountContract, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountContractSP, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountContractInstant, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountContractShip, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountContractInstantShip, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountPVPPoint, bValue: false);
		NKCUtil.SetGameobjectActive(m_UICountPVPTicket, bValue: false);
		int num = 0;
		foreach (int item2 in lstShowResource)
		{
			NKCUIComItemCount nKCUIComItemCount = GetUIItemCount(item2);
			if (nKCUIComItemCount == null)
			{
				if (num < m_lstUICountEtc.Count)
				{
					nKCUIComItemCount = m_lstUICountEtc[num];
					num++;
				}
				else
				{
					Debug.LogError("not enough Upsidemenu Item show ui buffer");
				}
			}
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(item2);
			if (itemMiscTempletByID != null && itemMiscTempletByID.EnableByTag && nKCUIComItemCount != null)
			{
				NKCUtil.SetGameobjectActive(nKCUIComItemCount, bValue: true);
				nKCUIComItemCount.SetData(userData, item2);
				nKCUIComItemCount.transform.SetAsLastSibling();
			}
		}
		m_RechargeAsyncTicket?.UpdateData();
		UpdateEterniumCap();
	}

	public void Close()
	{
		if (m_bOpen)
		{
			ResetParentOfResourceUI();
			m_bOpen = false;
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetParentOfResourceUI(NKCUIManager.eUIBaseRect type)
	{
		objResourceRoot.transform.SetParent(NKCUIManager.GetUIBaseRect(type).transform, worldPositionStays: true);
		objResourceRoot.transform.SetAsLastSibling();
		m_bIsSetOtherParent = true;
	}

	public void ResetParentOfResourceUI()
	{
		if (m_bIsSetOtherParent)
		{
			objResourceRoot.transform.SetParent(m_objContentRoot.transform, worldPositionStays: true);
			m_bIsSetOtherParent = false;
		}
	}
}
