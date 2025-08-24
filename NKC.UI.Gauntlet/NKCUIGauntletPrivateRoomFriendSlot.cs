using System;
using ClientPacket.Common;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletPrivateRoomFriendSlot : MonoBehaviour
{
	public delegate void OnDragBegin();

	public NKCUISlotProfile m_NKCUISlot;

	public GameObject m_obj1Min_BG;

	public GameObject m_obj2Min_BG;

	public GameObject m_obj3Min_BG;

	public Text m_lbLevel;

	public Text m_lbName;

	public Text m_lbUID;

	public Text m_lbLastOnlineTime;

	public NKCUIComStateButton m_csbtnSimpleUserInfoSlot;

	public GameObject m_objMySlot;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_GuildBadgeUI;

	public Text m_lbGuildName;

	public NKCUIComTitlePanel m_TitlePanel;

	[Header("커스텀 매치 초대")]
	public NKCUIComStateButton m_csbtnInviteToCustomMatch;

	private long m_UserUID;

	private OnDragBegin m_dOnDragBegin;

	private NKCAssetInstanceData m_InstanceData;

	private FriendListData m_FriendListData;

	private NKMCommonProfile m_CommonProfile;

	private NKMGuildSimpleData m_GuildSimpleData;

	public static NKCUIGauntletPrivateRoomFriendSlot GetNewInstance(Transform parent, OnDragBegin onDragBegin)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_PRIVATE_ROOM_INVITE_SLOT");
		NKCUIGauntletPrivateRoomFriendSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGauntletPrivateRoomFriendSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIGauntletPrivateRoomFriendSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		component.m_dOnDragBegin = onDragBegin;
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDragBeginImpl()
	{
		if (m_dOnDragBegin != null)
		{
			m_dOnDragBegin();
		}
	}

	private void OnClick()
	{
		if (m_UserUID > 0)
		{
			NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(m_UserUID, NKM_DECK_TYPE.NDT_PVP);
		}
	}

	private void OnClickOffline()
	{
		_ = m_UserUID;
		_ = 0;
	}

	public void SetUI(NKMUserProfileData userProfileData)
	{
		if (userProfileData != null)
		{
			m_CommonProfile = userProfileData.commonProfile;
			m_GuildSimpleData = userProfileData.guildData;
			SetUIData();
		}
	}

	public void SetUI(FriendListData cFriendListData, bool showTimeAndButtons)
	{
		m_FriendListData = cFriendListData;
		if (m_FriendListData == null)
		{
			return;
		}
		m_CommonProfile = m_FriendListData.commonProfile;
		m_GuildSimpleData = m_FriendListData.guildData;
		SetUIData();
		if (showTimeAndButtons)
		{
			TimeSpan timeSpan = DateTime.Now - m_FriendListData.lastLoginDate;
			NKCUtil.SetGameobjectActive(m_obj1Min_BG, timeSpan.TotalMinutes <= 1.0);
			NKCUtil.SetGameobjectActive(m_obj2Min_BG, 1.0 < timeSpan.TotalMinutes && timeSpan.TotalMinutes <= 2.0);
			NKCUtil.SetGameobjectActive(m_obj3Min_BG, 2.0 < timeSpan.TotalMinutes && timeSpan.TotalMinutes <= 3.0);
			NKCUtil.SetLabelText(m_lbLastOnlineTime, NKCUtilString.GetLastTimeString(m_FriendListData.lastLoginDate));
			m_csbtnSimpleUserInfoSlot.PointerDown.RemoveAllListeners();
			m_csbtnSimpleUserInfoSlot.PointerDown.AddListener(delegate
			{
				OnDragBeginImpl();
			});
			m_csbtnInviteToCustomMatch.PointerClick.RemoveAllListeners();
			m_csbtnInviteToCustomMatch.PointerClick.AddListener(OnClickInviteReq);
			m_csbtnSimpleUserInfoSlot.PointerClick.RemoveAllListeners();
			m_csbtnSimpleUserInfoSlot.PointerClick.AddListener(OnClick);
		}
	}

	private void SetUIData()
	{
		m_UserUID = m_CommonProfile.userUid;
		m_NKCUISlot.SetProfiledata(m_CommonProfile, null);
		m_lbName.text = m_CommonProfile.nickname;
		m_lbUID.text = NKCUtilString.GetFriendCode(m_CommonProfile.friendCode, bOpponent: true);
		m_lbLevel.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_CommonProfile.level);
		bool flag = false;
		if (m_UserUID == NKCScenManager.CurrentUserData().m_UserUID)
		{
			flag = true;
		}
		NKCUtil.SetGameobjectActive(m_objMySlot, flag);
		if (flag)
		{
			m_lbName.color = NKCUtil.GetColor("#FFDF5D");
		}
		else
		{
			m_lbName.color = Color.white;
		}
		SetGuildData();
		m_TitlePanel?.SetData(m_CommonProfile);
	}

	private void SetGuildData()
	{
		if (m_objGuild != null)
		{
			GameObject objGuild = m_objGuild;
			NKMGuildSimpleData guildSimpleData = m_GuildSimpleData;
			NKCUtil.SetGameobjectActive(objGuild, guildSimpleData != null && guildSimpleData.guildUid > 0);
			if (m_objGuild.activeSelf)
			{
				m_GuildBadgeUI.SetData(m_GuildSimpleData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, m_GuildSimpleData.guildName);
			}
		}
	}

	private void OnClickInviteReq()
	{
		if (NKCPrivatePVPRoomMgr.GetPlayerList().Count + 1 >= NKMPvpCommonConst.Instance.PvpRoomMaxPlayerCount)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_PRIVATE_PVP_OBSERVE_COUNT_MAX, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else
		{
			NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_LOBBY_INVITE_REQ(m_FriendListData);
		}
	}
}
