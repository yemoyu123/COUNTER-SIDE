using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Office;
using NKC.UI.Guide;
using NKM;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIOfficeUpsideMenu : MonoBehaviour
{
	public enum MenuState
	{
		MinimapRoom,
		MinimapFacility,
		Room,
		Facility,
		Decoration
	}

	public Text m_lbTitleName;

	public NKCUIComStateButton m_csbtnBack;

	public NKCUIComStateButton m_csbtnHome;

	public NKCUIComStateButton m_csbtnHelp;

	[Header("햄버거 메뉴")]
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objMenuRedDot;

	[Header("미니맵 룸 언락 카운트")]
	public GameObject m_objRoomUnlocked;

	public Text m_lbRoomUnlockCount;

	[Header("방 정보")]
	public GameObject m_objRoomMove;

	public Transform m_RoomInfoBg;

	public NKCUIComStateButton m_csbtnLeftMove;

	public NKCUIComStateButton m_csbtnRightMove;

	public Text m_lbRoomName;

	public Text m_lbRoomCount;

	public string m_strRoomCountColor;

	[Header("친구 프로필 아이콘")]
	public GameObject m_objProfileRoot;

	public NKCUISlotProfile m_profileSlot;

	private int m_iDormMaxCount;

	private MenuState m_eMenuState;

	private List<int> m_dormIdList;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnBack, OnBtnBack);
		NKCUtil.SetButtonClickDelegate(m_csbtnHome, OnBtnHome);
		NKCUtil.SetButtonClickDelegate(m_csbtnHelp, OnBtnHelp);
		NKCUtil.SetButtonClickDelegate(m_csbtnMenu, OnBtnMenu);
		NKCUtil.SetHotkey(m_csbtnMenu, HotkeyEventType.HamburgerMenu);
		NKCUtil.SetButtonClickDelegate(m_csbtnRightMove, OnBtnRightMove);
		NKCUtil.SetButtonClickDelegate(m_csbtnLeftMove, OnBtnLeftMove);
		m_iDormMaxCount = 0;
		foreach (NKMOfficeRoomTemplet value in NKMTempletContainer<NKMOfficeRoomTemplet>.Values)
		{
			if (value.Type == NKMOfficeRoomTemplet.RoomType.Dorm)
			{
				m_iDormMaxCount++;
			}
		}
	}

	public void SetState(MenuState state, NKMOfficeRoomTemplet roomTemplet = null)
	{
		m_eMenuState = state;
		switch (state)
		{
		case MenuState.MinimapRoom:
			NKCUtil.SetGameobjectActive(m_objRoomUnlocked, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRoomMove, bValue: false);
			SetTitleText(NKCUtilString.GET_STRING_OFFICE_MINIMAP);
			UpdateMinimapRoomInfo();
			break;
		case MenuState.MinimapFacility:
			NKCUtil.SetGameobjectActive(m_objRoomUnlocked, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRoomMove, bValue: false);
			SetTitleText(NKCUtilString.GET_STRING_OFFICE_MINIMAP);
			break;
		case MenuState.Room:
			NKCUtil.SetGameobjectActive(m_objRoomUnlocked, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRoomMove, bValue: true);
			SetTitleText(NKCUtilString.GET_STRING_OFFICE_DORMITORY);
			UpdateRoomIndex(roomTemplet);
			break;
		case MenuState.Facility:
			NKCUtil.SetGameobjectActive(m_objRoomUnlocked, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRoomMove, bValue: false);
			NKCUtil.SetGameobjectActive(m_objProfileRoot, bValue: false);
			break;
		case MenuState.Decoration:
			NKCUtil.SetGameobjectActive(m_objRoomUnlocked, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRoomMove, bValue: true);
			NKCUtil.SetLabelText(m_lbTitleName, NKCUtilString.GET_STRING_OFFICE_DECORATE);
			break;
		}
		NKCUtil.SetGameobjectActive(m_csbtnMenu.gameObject, state != MenuState.Decoration);
		if (roomTemplet != null)
		{
			UpdateRoomInfo(roomTemplet);
		}
	}

	public void UpdateMinimapRoomInfo()
	{
		NKCUIOfficeMapFront instance = NKCUIOfficeMapFront.GetInstance();
		if (!(instance == null))
		{
			int openedDormsCount = instance.OfficeData.GetOpenedDormsCount();
			NKCUtil.SetLabelText(m_lbRoomUnlockCount, string.Format(NKCUtilString.GET_STRING_OFFICE_OPENED_DORMS_COUNT, openedDormsCount, m_iDormMaxCount));
		}
	}

	public void UpdateRoomInfo(NKMOfficeRoomTemplet roomTemplet)
	{
		if (roomTemplet == null)
		{
			NKCUtil.SetLabelText(m_lbTitleName, "");
			NKCUtil.SetLabelText(m_lbRoomName, "");
			return;
		}
		if (roomTemplet.IsFacility)
		{
			NKCUtil.SetLabelText(m_lbTitleName, NKCStringTable.GetString(roomTemplet.Name));
			return;
		}
		NKMOfficeRoom nKMOfficeRoom = NKCUIOfficeMapFront.GetInstance()?.OfficeData.GetOfficeRoom(roomTemplet.ID);
		if (nKMOfficeRoom != null && !string.IsNullOrEmpty(nKMOfficeRoom.name))
		{
			NKCUtil.SetLabelText(m_lbRoomName, nKMOfficeRoom.name);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRoomName, NKCStringTable.GetString(roomTemplet.Name));
		}
	}

	public void SetRedDotNotify()
	{
		bool bValue = NKCAlarmManager.CheckAllNotify(NKCScenManager.CurrentUserData());
		NKCUtil.SetGameobjectActive(m_objMenuRedDot, bValue);
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
		if (value)
		{
			SetRedDotNotify();
		}
	}

	public void SetTitleText(string title)
	{
		NKCUIOfficeMapFront instance = NKCUIOfficeMapFront.GetInstance();
		if (instance == null)
		{
			return;
		}
		if (instance.Visiting)
		{
			NKCUtil.SetGameobjectActive(m_csbtnHelp.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_objProfileRoot, bValue: true);
			NKMCommonProfile friendProfile = instance.OfficeData.GetFriendProfile();
			if (friendProfile != null)
			{
				m_profileSlot?.SetProfiledata(friendProfile, null);
				NKCUtil.SetLabelText(m_lbTitleName, string.Format(NKCUtilString.GET_STRING_OFFICE_FRIEND_NICKNAME, friendProfile.nickname));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objProfileRoot, bValue: false);
				NKCUtil.SetLabelText(m_lbTitleName, string.Format(NKCUtilString.GET_STRING_OFFICE_FRIEND_NICKNAME, ""));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnHelp.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_objProfileRoot, bValue: false);
			NKCUtil.SetLabelText(m_lbTitleName, title);
		}
	}

	private void UpdateRoomIndex(NKMOfficeRoomTemplet roomTemplet)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			m_dormIdList = nKMUserData.OfficeData.GetOpenedRoomIdList();
		}
		if (m_dormIdList != null && roomTemplet != null)
		{
			int num = m_dormIdList.IndexOf(roomTemplet.ID);
			NKCUtil.SetLabelText(m_lbRoomCount, $"<color={m_strRoomCountColor}>{num + 1}</color>/{m_dormIdList.Count}");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRoomCount, "<color=" + m_strRoomCountColor + ">0</color>/0");
		}
	}

	private void OnBtnHome()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
	}

	private void OnBtnBack()
	{
		NKCUIManager.OnBackButton();
	}

	private void OnBtnMenu()
	{
		NKCPopupHamburgerMenu.instance.OpenUI();
	}

	private void OnBtnHelp()
	{
		switch (m_eMenuState)
		{
		case MenuState.MinimapRoom:
		case MenuState.MinimapFacility:
			NKCUIPopUpGuide.Instance.Open("ARTICLE_OFFICE_INFO");
			break;
		case MenuState.Room:
		case MenuState.Decoration:
			NKCUIPopUpGuide.Instance.Open("ARTICLE_OFFICE_DORMI");
			break;
		default:
			NKCUIPopUpGuide.Instance.Open("ARTICLE_OFFICE_FACILITY");
			break;
		}
	}

	public void OnBtnLeftMove()
	{
		if (m_dormIdList != null)
		{
			int num = m_dormIdList.IndexOf(NKCUIOffice.GetInstance().RoomID);
			if (num >= 0 && num < m_dormIdList.Count)
			{
				num = ((num != 0) ? (num - 1) : (m_dormIdList.Count - 1));
				NKCUIOffice.GetInstance().MoveToRoom(m_dormIdList[num]);
			}
		}
	}

	public void OnBtnRightMove()
	{
		if (m_dormIdList != null)
		{
			int num = m_dormIdList.IndexOf(NKCUIOffice.GetInstance().RoomID);
			if (num >= 0 && num < m_dormIdList.Count)
			{
				num = ((num != m_dormIdList.Count - 1) ? (num + 1) : 0);
				NKCUIOffice.GetInstance().MoveToRoom(m_dormIdList[num]);
			}
		}
	}

	private void OnDestroy()
	{
		m_dormIdList?.Clear();
		m_dormIdList = null;
	}
}
