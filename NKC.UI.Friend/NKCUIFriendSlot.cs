using System;
using ClientPacket.Common;
using ClientPacket.Community;
using NKC.UI.Component;
using NKC.UI.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Friend;

public class NKCUIFriendSlot : MonoBehaviour
{
	public enum FRIEND_SLOT_TYPE
	{
		FST_NONE,
		FST_FRIEND_LIST,
		FST_BLOCK_LIST,
		FST_FRIEND_SEARCH,
		FST_FRIEND_SEARCH_RECOMMEND,
		FST_RECEIVE_REQ,
		FST_SENT_REQ,
		FST_GAUNTLET_LIST,
		FST_GUILD_LIST,
		FST_GAUNTLET_CUSTOM_LIST,
		FST_OFFICE
	}

	public Text m_NKM_UI_FRIEND_LIST_SLOT_INFO_NAME;

	public Text m_NKM_UI_FRIEND_LIST_SLOT_INFO_LV;

	public Text m_NKM_UI_FRIEND_LIST_SLOT_INFO_UID;

	public GameObject m_NKM_UI_FRIEND_LIST_SLOT_INFO_LINE;

	public GameObject m_NKM_UI_FRIEND_LIST_SLOT_INFO_TIME;

	public Text m_NKM_UI_FRIEND_LIST_SLOT_INFO_TIME_TEXT_2;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public Text m_lbGuildName;

	public NKCUIComTitlePanel m_TitlePanel;

	public NKCUIComButton m_BGButton;

	public NKCUIComButton m_cbtnUserInfo;

	public NKCUIComButton m_cbtnDelete;

	public NKCUIComButton m_cbtnAdd;

	public NKCUIComButton m_cbtnConfirm;

	public NKCUIComButton m_cbtnCancel;

	public NKCUIComButton m_cbtnDomitory;

	public NKCUISlotProfile m_NKCUISlot;

	public GameObject m_RECOMMEND_BADGE;

	private FRIEND_SLOT_TYPE m_FRIEND_SLOT_TYPE = FRIEND_SLOT_TYPE.FST_FRIEND_LIST;

	private FriendListData m_friendListData;

	public GameObject m_SUB_BUTTON_layout_gruop;

	[Header("멘토링")]
	public GameObject m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_TEXT;

	public GameObject m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_DELETE;

	public NKCUIComButton m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_DELETE;

	public Text m_MENTORING_PROCEEDING;

	public Text m_MENTORING_COMPLETE;

	private NKCAssetInstanceData m_Instance;

	private long m_lFriendCoide;

	public static NKCUIFriendSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_FRIEND", "NKM_UI_FRIEND_LIST_SLOT");
		NKCUIFriendSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIFriendSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIFriendSlot Prefab null!");
			return null;
		}
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		}
		component.m_Instance = nKCAssetInstanceData;
		component.gameObject.SetActive(value: false);
		component.m_BGButton.PointerClick.RemoveAllListeners();
		component.m_BGButton.PointerClick.AddListener(component.OnClickSlotBGButton);
		component.m_cbtnUserInfo.PointerClick.RemoveAllListeners();
		component.m_cbtnUserInfo.PointerClick.AddListener(component.OnClickSlotBGButton);
		component.m_cbtnDelete.PointerClick.RemoveAllListeners();
		component.m_cbtnDelete.PointerClick.AddListener(component.OnClickDelete);
		component.m_cbtnAdd.PointerClick.RemoveAllListeners();
		component.m_cbtnAdd.PointerClick.AddListener(component.OnClickAdd);
		component.m_cbtnConfirm.PointerClick.RemoveAllListeners();
		component.m_cbtnConfirm.PointerClick.AddListener(component.OnClickConfirm);
		component.m_cbtnCancel.PointerClick.RemoveAllListeners();
		component.m_cbtnCancel.PointerClick.AddListener(component.OnClickCancel);
		component.m_cbtnDomitory.PointerClick.RemoveAllListeners();
		component.m_cbtnDomitory.PointerClick.AddListener(component.OnClickDomitory);
		return component;
	}

	private void OnClickDelete()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_FRIEND_DELETE_REQ, OnClickDelete_);
	}

	private void OnClickAdd()
	{
		OnClickFriendREQ();
	}

	private void OnClickConfirm()
	{
		OnClickAcceptFriend();
	}

	private void OnClickCancel()
	{
		if (m_FRIEND_SLOT_TYPE == FRIEND_SLOT_TYPE.FST_BLOCK_LIST)
		{
			OnClickCancelBlockFriend();
		}
		else if (m_FRIEND_SLOT_TYPE == FRIEND_SLOT_TYPE.FST_RECEIVE_REQ)
		{
			OnClickRejectFriend();
		}
		else if (m_FRIEND_SLOT_TYPE == FRIEND_SLOT_TYPE.FST_SENT_REQ)
		{
			OnClickCancelAddFriend();
		}
	}

	public FriendListData GetFriendListData()
	{
		return m_friendListData;
	}

	public void SetActive(bool bSet)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bSet);
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void Clear()
	{
		if (m_Instance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_Instance);
		}
	}

	private void SetGuildData()
	{
		NKMGuildSimpleData guildData = m_friendListData.guildData;
		if (m_objGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, guildData != null && guildData.guildUid > 0);
			if (m_objGuild.activeSelf && guildData != null)
			{
				m_BadgeUI.SetData(guildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, guildData.guildName);
			}
		}
	}

	public FRIEND_SLOT_TYPE Get_FRIEND_SLOT_TYPE()
	{
		return m_FRIEND_SLOT_TYPE;
	}

	public void SetData(FRIEND_SLOT_TYPE slotType, FriendListData _NKMUserProfileData)
	{
		m_friendListData = _NKMUserProfileData;
		SetGuildData();
		SetData(slotType, _NKMUserProfileData.commonProfile, _NKMUserProfileData.lastLoginDate.Ticks);
	}

	private void SetData(FRIEND_SLOT_TYPE slotType, NKMCommonProfile profile, long lastLoginTime)
	{
		m_FRIEND_SLOT_TYPE = slotType;
		m_NKM_UI_FRIEND_LIST_SLOT_INFO_NAME.text = profile.nickname;
		m_NKM_UI_FRIEND_LIST_SLOT_INFO_LV.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, profile.level);
		m_NKM_UI_FRIEND_LIST_SLOT_INFO_UID.text = NKCUtilString.GetFriendCode(profile.friendCode);
		m_NKCUISlot.SetProfiledata(profile, null);
		NKCUtil.SetGameobjectActive(m_RECOMMEND_BADGE, slotType == FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH_RECOMMEND);
		DateTime lastTime = new DateTime(lastLoginTime);
		bool bValue = true;
		if (slotType == FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH_RECOMMEND && NKCUtilString.GetLastTimeSpan(lastTime).TotalDays > 0.0)
		{
			bValue = false;
		}
		m_NKM_UI_FRIEND_LIST_SLOT_INFO_TIME_TEXT_2.text = NKCUtilString.GetLastTimeString(lastTime);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_INFO_LINE, bValue);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_INFO_TIME, bValue);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_INFO_TIME_TEXT_2, bValue);
		NKCUtil.SetGameobjectActive(m_cbtnDelete, slotType == FRIEND_SLOT_TYPE.FST_FRIEND_LIST);
		NKCUtil.SetGameobjectActive(m_cbtnCancel, slotType == FRIEND_SLOT_TYPE.FST_BLOCK_LIST || slotType == FRIEND_SLOT_TYPE.FST_RECEIVE_REQ || slotType == FRIEND_SLOT_TYPE.FST_SENT_REQ);
		NKCUtil.SetGameobjectActive(m_cbtnAdd, slotType == FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH || slotType == FRIEND_SLOT_TYPE.FST_FRIEND_SEARCH_RECOMMEND);
		NKCUtil.SetGameobjectActive(m_cbtnConfirm, slotType == FRIEND_SLOT_TYPE.FST_RECEIVE_REQ);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_TEXT, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_DELETE, bValue: false);
		NKCUtil.SetGameobjectActive(m_cbtnUserInfo, bValue: true);
		NKCUtil.SetGameobjectActive(m_SUB_BUTTON_layout_gruop, bValue: true);
		m_lFriendCoide = profile.friendCode;
		m_TitlePanel?.SetData(profile);
		base.gameObject.SetActive(value: true);
	}

	private void OnClickDelete_()
	{
		NKMPacket_FRIEND_DELETE_REQ nKMPacket_FRIEND_DELETE_REQ = new NKMPacket_FRIEND_DELETE_REQ();
		nKMPacket_FRIEND_DELETE_REQ.friendCode = GetFriendListData().commonProfile.friendCode;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_DELETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OnClickSlotBGButton()
	{
		if (m_lFriendCoide != 0L)
		{
			NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ nKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ = new NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ();
			nKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ.friendCode = GetFriendListData().commonProfile.friendCode;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	private void OnClickCancelBlockFriend()
	{
		NKMPacket_FRIEND_BLOCK_REQ nKMPacket_FRIEND_BLOCK_REQ = new NKMPacket_FRIEND_BLOCK_REQ();
		nKMPacket_FRIEND_BLOCK_REQ.friendCode = GetFriendListData().commonProfile.friendCode;
		nKMPacket_FRIEND_BLOCK_REQ.isCancel = true;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_BLOCK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OnClickAcceptFriend()
	{
		NKMPacket_FRIEND_ACCEPT_REQ nKMPacket_FRIEND_ACCEPT_REQ = new NKMPacket_FRIEND_ACCEPT_REQ();
		nKMPacket_FRIEND_ACCEPT_REQ.friendCode = GetFriendListData().commonProfile.friendCode;
		nKMPacket_FRIEND_ACCEPT_REQ.isAllow = true;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_ACCEPT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OnClickRejectFriend()
	{
		NKMPacket_FRIEND_ACCEPT_REQ nKMPacket_FRIEND_ACCEPT_REQ = new NKMPacket_FRIEND_ACCEPT_REQ();
		nKMPacket_FRIEND_ACCEPT_REQ.friendCode = GetFriendListData().commonProfile.friendCode;
		nKMPacket_FRIEND_ACCEPT_REQ.isAllow = false;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_ACCEPT_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OnClickCancelAddFriend()
	{
		NKMPacket_FRIEND_CANCEL_REQUEST_REQ nKMPacket_FRIEND_CANCEL_REQUEST_REQ = new NKMPacket_FRIEND_CANCEL_REQUEST_REQ();
		nKMPacket_FRIEND_CANCEL_REQUEST_REQ.friendCode = GetFriendListData().commonProfile.friendCode;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_CANCEL_REQUEST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OnClickFriendREQ()
	{
		NKMPacket_FRIEND_REQUEST_REQ nKMPacket_FRIEND_REQUEST_REQ = new NKMPacket_FRIEND_REQUEST_REQ();
		nKMPacket_FRIEND_REQUEST_REQ.friendCode = GetFriendListData().commonProfile.friendCode;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_REQUEST_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OnClickDomitory()
	{
		if (m_friendListData != null)
		{
			if (m_friendListData.hasOffice)
			{
				NKCPacketSender.Send_NKMPacket_OFFICE_STATE_REQ(m_friendListData.commonProfile.userUid);
			}
			else
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_OFFICE_FRIEND_CANNOT_VISIT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
		}
	}
}
