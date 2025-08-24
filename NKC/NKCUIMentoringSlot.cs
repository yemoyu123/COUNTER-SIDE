using System;
using ClientPacket.Common;
using ClientPacket.Community;
using NKC.UI;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIMentoringSlot : MonoBehaviour
{
	public delegate void callBackFunc(long uid);

	public Text m_NKM_UI_FRIEND_LIST_SLOT_INFO_NAME;

	public Text m_NKM_UI_FRIEND_LIST_SLOT_INFO_LV;

	public Text m_NKM_UI_FRIEND_LIST_SLOT_INFO_UID;

	public Text m_NKM_UI_FRIEND_LIST_SLOT_INFO_TIME_TEXT_2;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public Text m_lbGuildName;

	public NKCUIComButton m_BGButton;

	public NKCUIComButton m_cbtnUserInfo;

	public NKCUIComButton m_cbtnDelete;

	public NKCUIComButton m_cbtnAdd;

	public NKCUIComButton m_cbtnConfirm;

	public NKCUIComButton m_cbtnCancel;

	public NKCUISlotProfile m_NKCUISlot;

	public GameObject m_RECOMMEND_BADGE;

	public GameObject m_INVITE_BADGE;

	private FriendListData m_friendListData;

	[Header("멘토링")]
	public GameObject m_SUB_BUTTON_layout_gruop;

	public GameObject m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_MENTORING_ADD;

	public NKCUIComButton m_cbtnMentoring_Add;

	public GameObject m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_MENTORING_REQUEST;

	public NKCUIComButton m_cbtnMentoring_Request;

	public GameObject m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_TEXT;

	public GameObject m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_DELETE;

	public NKCUIComButton m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_DELETE;

	public Text m_MENTORING_PROCEEDING;

	public Text m_MENTORING_COMPLETE;

	private NKCAssetInstanceData m_Instance;

	private long m_MentoringTargetUID;

	private callBackFunc m_callBack;

	private long m_lFriendCode;

	public static NKCUIMentoringSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_FRIEND", "NKM_UI_FRIEND_LIST_SLOT_MENTORING");
		NKCUIMentoringSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIMentoringSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIMentoringSlot Prefab null!");
			return null;
		}
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.m_Instance = nKCAssetInstanceData;
			component.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		}
		component.gameObject.SetActive(value: false);
		component.m_cbtnConfirm.PointerClick.RemoveAllListeners();
		component.m_cbtnConfirm.PointerClick.AddListener(component.OnClickConfirm);
		component.m_cbtnCancel.PointerClick.RemoveAllListeners();
		component.m_cbtnCancel.PointerClick.AddListener(component.OnClickCancel);
		component.m_cbtnMentoring_Add.PointerClick.RemoveAllListeners();
		component.m_cbtnMentoring_Add.PointerClick.AddListener(component.OnClickAddMentor);
		component.m_cbtnMentoring_Request.PointerClick.RemoveAllListeners();
		component.m_cbtnMentoring_Request.PointerClick.AddListener(component.OnClickRequestMentee);
		component.m_BGButton.PointerClick.RemoveAllListeners();
		component.m_BGButton.PointerClick.AddListener(component.OnClickBG);
		return component;
	}

	public void Clear()
	{
		if (m_Instance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_Instance);
		}
	}

	private void OnClickConfirm()
	{
		if (m_MentoringTargetUID != 0L)
		{
			string mentorName = NKCScenManager.CurrentUserData().GetMentorName(m_MentoringTargetUID);
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_FRIEND_MENTORING_REGISTER_MENTOR_ACCEPT_TITLE, string.Format(NKCUtilString.GET_FRIEND_MENTORING_REGISTER_MENTOR_ACCEPT_DESC_01, mentorName), delegate
			{
				NKCPacketSender.Send_NKMPacket_MENTORING_ACCEPT_MENTOR_REQ(m_MentoringTargetUID);
			});
		}
	}

	private void OnClickCancel()
	{
		if (m_MentoringTargetUID != 0L)
		{
			string mentorName = NKCScenManager.CurrentUserData().GetMentorName(m_MentoringTargetUID);
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_FRIEND_MENTORING_REGISTER_MENTOR_DISACCEPT_TITLE, string.Format(NKCUtilString.GET_FRIEND_MENTORING_REGISTER_MENTOR_DISACCEPT_DESC_01, mentorName), delegate
			{
				NKCPacketSender.Send_NKMPacket_MENTORING_DISACCEPT_MENTOR_REQ(m_MentoringTargetUID);
			});
		}
	}

	private void SetGuildData()
	{
		NKMGuildSimpleData guildData = m_friendListData.guildData;
		if (m_objGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, guildData != null && guildData.guildUid > 0);
			if (guildData != null && m_objGuild.activeSelf)
			{
				m_BadgeUI.SetData(guildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, guildData.guildName);
			}
		}
	}

	public void SetData(FriendListData _NKMUserProfileData)
	{
		m_friendListData = _NKMUserProfileData;
		SetGuildData();
		SetData(_NKMUserProfileData.commonProfile, _NKMUserProfileData.lastLoginDate.Ticks);
	}

	private void SetData(NKMCommonProfile profile, long lastLoginTime)
	{
		m_NKM_UI_FRIEND_LIST_SLOT_INFO_NAME.text = profile.nickname;
		m_NKM_UI_FRIEND_LIST_SLOT_INFO_LV.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, profile.level);
		m_NKM_UI_FRIEND_LIST_SLOT_INFO_UID.text = NKCUtilString.GetFriendCode(profile.friendCode);
		m_NKCUISlot.SetProfiledata(profile, null);
		DateTime lastTime = new DateTime(lastLoginTime);
		m_NKM_UI_FRIEND_LIST_SLOT_INFO_TIME_TEXT_2.text = NKCUtilString.GetLastTimeString(lastTime);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_TEXT, bValue: true);
		if (m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_DELETE != null)
		{
			m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_DELETE.PointerClick.RemoveAllListeners();
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_DELETE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_MENTORING_ADD, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_MENTORING_REQUEST, bValue: false);
		NKCUtil.SetGameobjectActive(m_cbtnUserInfo, bValue: false);
		NKCUtil.SetGameobjectActive(m_SUB_BUTTON_layout_gruop, bValue: true);
		m_MentoringTargetUID = 0L;
		base.gameObject.SetActive(value: true);
	}

	public void SetData(NKMCommonProfile profile, long lastLoginTime, bool Invited)
	{
		SetData(profile, lastLoginTime);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_DELETE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_TEXT, bValue: false);
		NKCUtil.SetGameobjectActive(m_RECOMMEND_BADGE, !Invited);
		NKCUtil.SetGameobjectActive(m_INVITE_BADGE, Invited);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_MENTORING_ADD, !Invited);
		NKCUtil.SetGameobjectActive(m_SUB_BUTTON_layout_gruop, Invited);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_DELETE, bValue: false);
		NKCUtil.SetGameobjectActive(m_MENTORING_PROCEEDING, bValue: false);
		NKCUtil.SetGameobjectActive(m_MENTORING_COMPLETE, bValue: false);
		m_MentoringTargetUID = profile.userUid;
		m_lFriendCode = profile.friendCode;
	}

	public void SetDataForSearch(NKMCommonProfile profile, long lastLoginTime, callBackFunc callBack = null)
	{
		SetData(profile, lastLoginTime);
		NKCUtil.SetGameobjectActive(m_RECOMMEND_BADGE, bValue: false);
		NKCUtil.SetGameobjectActive(m_INVITE_BADGE, bValue: false);
		NKCUtil.SetGameobjectActive(m_SUB_BUTTON_layout_gruop, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_DELETE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_MENTORING_TEXT, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_MENTORING_ADD, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_LIST_SLOT_BUTTON_MENTORING_REQUEST, bValue: true);
		m_MentoringTargetUID = profile.userUid;
		m_lFriendCode = profile.friendCode;
		m_callBack = callBack;
	}

	private void OnClickAddMentor()
	{
		if (m_MentoringTargetUID != 0L)
		{
			string mentorName = NKCScenManager.CurrentUserData().GetMentorName(m_MentoringTargetUID);
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_FRIEND_MENTORING_REGISTER_MENTOR_ACCEPT_TITLE, string.Format(NKCUtilString.GET_FRIEND_MENTORING_REGISTER_MENTOR_ACCEPT_DESC_01, mentorName), delegate
			{
				NKCPacketSender.Send_NKMPacket_MENTORING_ADD_REQ(MentoringIdentity.Mentor, m_MentoringTargetUID);
			});
		}
	}

	private void OnClickRequestMentee()
	{
		if (m_MentoringTargetUID != 0L)
		{
			NKMUserData.strMentoringData mentoringData = NKCScenManager.CurrentUserData().MentoringData;
			if (mentoringData.lstMenteeMatch != null && mentoringData.lstMenteeMatch.Count > 0 && mentoringData.lstMenteeMatch.Count >= NKMMentoringConst.MenteeLimitBelongCount)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_FRIEND_MENTORING_LIMIT_COUNT_DESC);
				return;
			}
			NKCPacketSender.Send_NKMPacket_MENTORING_ADD_REQ(MentoringIdentity.Mentee, m_MentoringTargetUID);
			m_callBack?.Invoke(m_MentoringTargetUID);
		}
	}

	public void OnClickBG()
	{
		if (m_lFriendCode != 0L)
		{
			NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ nKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ = new NKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ();
			nKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ.friendCode = m_lFriendCode;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_USER_PROFILE_BY_FRIEND_CODE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}
}
