using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Community;
using ClientPacket.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Friend;

public class NKCUIFriendMentoring : MonoBehaviour
{
	[Header("멘토 등록")]
	public GameObject m_Slot_Area;

	public GameObject m_Center_OFF;

	public GameObject m_Center_ON;

	public GameObject m_NKM_UI_FRIEND_MENTORING_INFO;

	public GameObject m_NKM_UI_FRIEND_MENTORING_MISSION_LIST;

	public NKCUIFriendMentoringSlot m_MenteeSlot;

	public NKCUIFriendMentoringSlot m_MentorSlot;

	public NKCUISlotProfile m_MenteeSlotProfile;

	public NKCUISlotProfile m_MentorSlotProfile;

	public NKCUIComStateButton m_NKM_UI_FRIEND_MENTORING_INFO_ADD_BUTTON;

	[Header("멘티 미션")]
	public NKCUISlot m_MenteeMissionAllClearSlot;

	public Text m_NKM_UI_FRIEND_MENTORING_MISSION_LIST_TEXT_02;

	public Text m_NKM_UI_FRIEND_MENTORING_MISSION_LIST_COUNT_TEXT;

	public Text m_NKM_UI_FRIEND_MENTORING_MISSION_LIST_COMPLETE_TEXT;

	public Slider m_MENTORING_MISSION_LIST_PROGRESS_SLIDER;

	public LoopScrollRect m_MISSION_LIST_ScrollRect;

	public NKCUIComStateButton m_NKM_UI_FRIEND_MENTORING_SLOT_ADD_BUTTON;

	[Header("멘토 목록")]
	public GameObject m_NKM_UI_FRIEND_MENTORING_MISSION_COMPLETE;

	[Header("멘티 리스트")]
	public GameObject m_NKM_UI_FRIEND_MENTORING_INVITE;

	public RectTransform m_ILLUST_Root;

	public LoopScrollRect m_MENTORING_INVITE_LIST_ScrollRect;

	public InputField m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT;

	public NKCUIComButton m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON;

	public NKCUIComButton m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH;

	[Space]
	public NKCUIComButton m_NKM_UI_FRIEND_MENTORING_REFRESH_BUTTON;

	public NKCUIComButton m_NKM_UI_FRIEND_MENTORING_INVITE_REWARD_BUTTON;

	public LoopScrollRect m_NKM_UI_FRIEND_LIST_ScrollView;

	public NKCUICharacterView m_NKM_UI_FRIEND_MENTORING_INVITE_ILLUST_VIEW;

	private MentoringIdentity m_eMentorType;

	private NKMMentoringTemplet m_curMetoringTemplet;

	private UnityAction m_CallBack;

	private bool bCheckHasMentor;

	private int m_iMyMenteeCnt;

	private List<MenteeInfo> m_lstMyMentee = new List<MenteeInfo>();

	private List<FriendListData> m_lstSearchMentee = new List<FriendListData>();

	private List<NKMMissionTemplet> m_lstMenteeMission = new List<NKMMissionTemplet>();

	public Transform m_rtMentoringSlotParent;

	private List<NKCUIFriendSlot> m_slotList = new List<NKCUIFriendSlot>();

	private Stack<RectTransform> m_slotPool = new Stack<RectTransform>();

	public Transform m_trMentoringHideParent;

	private List<NKCUIMentoringSlot> m_lstMentoring = new List<NKCUIMentoringSlot>();

	private Stack<RectTransform> m_mentoringSlotPool = new Stack<RectTransform>();

	private List<long> lstAlreadyUID = new List<long>();

	public MentoringIdentity GetCurMentoringIdentity()
	{
		return m_eMentorType;
	}

	public NKMMentoringTemplet GetCurMentoringTemplet()
	{
		return m_curMetoringTemplet;
	}

	public void Init()
	{
		m_eMentorType = MentoringIdentity.Mentee;
		m_curMetoringTemplet = null;
		if (m_MenteeMissionAllClearSlot != null)
		{
			m_MenteeMissionAllClearSlot.Init();
		}
		if (m_NKM_UI_FRIEND_MENTORING_INFO_ADD_BUTTON != null)
		{
			m_NKM_UI_FRIEND_MENTORING_INFO_ADD_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_FRIEND_MENTORING_INFO_ADD_BUTTON.PointerClick.AddListener(delegate
			{
				NKCPacketSender.Send_kNKMPacket_MENTORING_RECEIVE_LIST_REQ(MentoringIdentity.Mentee);
			});
		}
		if (m_NKM_UI_FRIEND_MENTORING_SLOT_ADD_BUTTON != null)
		{
			m_NKM_UI_FRIEND_MENTORING_SLOT_ADD_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_FRIEND_MENTORING_SLOT_ADD_BUTTON.PointerClick.AddListener(delegate
			{
				NKCPacketSender.Send_kNKMPacket_MENTORING_RECEIVE_LIST_REQ(MentoringIdentity.Mentee);
			});
		}
		if (m_MISSION_LIST_ScrollRect != null)
		{
			m_MISSION_LIST_ScrollRect.dOnGetObject += GetMissionSlot;
			m_MISSION_LIST_ScrollRect.dOnReturnObject += ReturnMissionSlot;
			m_MISSION_LIST_ScrollRect.dOnProvideData += ProvideData;
			m_MISSION_LIST_ScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_MISSION_LIST_ScrollRect);
		}
		if (m_NKM_UI_FRIEND_MENTORING_INVITE_REWARD_BUTTON != null)
		{
			m_NKM_UI_FRIEND_MENTORING_INVITE_REWARD_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_FRIEND_MENTORING_INVITE_REWARD_BUTTON.PointerClick.AddListener(OnClickMentorReward);
		}
		if (m_NKM_UI_FRIEND_LIST_ScrollView != null)
		{
			m_NKM_UI_FRIEND_LIST_ScrollView.dOnGetObject += GetFriendSlot;
			m_NKM_UI_FRIEND_LIST_ScrollView.dOnReturnObject += ReturnFriendSlot;
			m_NKM_UI_FRIEND_LIST_ScrollView.dOnProvideData += ProvideFriendSlotToMenteeData;
			m_NKM_UI_FRIEND_LIST_ScrollView.PrepareCells();
			NKCUtil.SetScrollHotKey(m_NKM_UI_FRIEND_LIST_ScrollView);
		}
		if (m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON != null)
		{
			m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON.PointerClick.AddListener(OnClickMenteeSearch);
		}
		if (m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH != null)
		{
			m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH.PointerClick.RemoveAllListeners();
			m_NKM_UI_FRIEND_TOP_SUBMENU_REFRESH.PointerClick.AddListener(OnClickMenteeSearchRefresh);
		}
		if (m_MENTORING_INVITE_LIST_ScrollRect != null)
		{
			m_MENTORING_INVITE_LIST_ScrollRect.dOnGetObject += GetMentoringSlot;
			m_MENTORING_INVITE_LIST_ScrollRect.dOnReturnObject += ReturnMentoringSlot;
			m_MENTORING_INVITE_LIST_ScrollRect.dOnProvideData += ProvideMentoringData;
			m_MENTORING_INVITE_LIST_ScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_MENTORING_INVITE_LIST_ScrollRect);
		}
		if (m_NKM_UI_FRIEND_MENTORING_REFRESH_BUTTON != null)
		{
			m_NKM_UI_FRIEND_MENTORING_REFRESH_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_FRIEND_MENTORING_REFRESH_BUTTON.PointerClick.AddListener(OnClickMatchListRefresh);
		}
		if (m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT != null)
		{
			m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT.onEndEdit.RemoveAllListeners();
			m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT.onEndEdit.AddListener(OnEndEditSearch);
		}
	}

	public void UpdateData(UnityAction callBack = null)
	{
		m_curMetoringTemplet = NKCMentoringUtil.GetCurrentTempet();
		if (!bCheckHasMentor)
		{
			NKCPacketSender.Send_NKMPacket_MENTORING_DATA_REQ();
			m_CallBack = callBack;
		}
		else
		{
			callBack();
			m_CallBack = null;
		}
	}

	public void Open()
	{
		UpdateUI();
		lstAlreadyUID.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void Close()
	{
		bCheckHasMentor = false;
		m_curMetoringTemplet = null;
		m_NKM_UI_FRIEND_MENTORING_INVITE_ILLUST_VIEW.CloseImmediatelyIllust();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void UpdateMyMetor()
	{
		bCheckHasMentor = true;
		if (m_CallBack != null)
		{
			m_CallBack();
		}
	}

	public int GetMenteeCnt()
	{
		return m_iMyMenteeCnt;
	}

	public void UpdateMyMenteeList()
	{
		m_lstSearchMentee.Clear();
		m_lstMyMentee = NKCScenManager.CurrentUserData().MentoringData.lstMenteeMatch;
		m_iMyMenteeCnt = m_lstMyMentee.Count;
		if (m_CallBack != null)
		{
			m_CallBack();
		}
	}

	public void UpdateMyMentoringIdentity(bool bForceUpdate = false)
	{
		m_eMentorType = NKCMentoringUtil.GetMentoringIdentity(NKCScenManager.CurrentUserData());
		if (m_eMentorType == MentoringIdentity.Mentor)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			bool flag = false;
			if (nKMUserData != null && nKMUserData.MentoringData.lstMenteeMatch != null && nKMUserData.MentoringData.bMentoringNotify)
			{
				flag = true;
			}
			NKCPacketSender.Send_NKMPacket_MENTORING_MATCH_LIST_REQ(flag || bForceUpdate);
		}
		else
		{
			UpdateMyMetor();
		}
	}

	public void UpdateMenteeList()
	{
		m_lstSearchMentee.Clear();
		List<FriendListData> lstRecommend = NKCScenManager.CurrentUserData().MentoringData.lstRecommend;
		List<FriendListData> lstInvited = NKCScenManager.CurrentUserData().MentoringData.lstInvited;
		if (m_eMentorType == MentoringIdentity.Mentor)
		{
			NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
			if (userProfileData != null)
			{
				NKMUnitData nKMUnitData = new NKMUnitData();
				nKMUnitData.m_UnitID = userProfileData.commonProfile.mainUnitId;
				nKMUnitData.m_SkinID = userProfileData.commonProfile.mainUnitSkinId;
				m_NKM_UI_FRIEND_MENTORING_INVITE_ILLUST_VIEW.SetCharacterIllust(nKMUnitData);
			}
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_INVITE_ILLUST_VIEW, userProfileData != null);
		}
		if (lstRecommend != null && lstRecommend.Count > 0)
		{
			m_lstSearchMentee.AddRange(lstRecommend);
			if (lstInvited != null)
			{
				m_lstSearchMentee.AddRange(lstInvited);
			}
		}
		if (lstAlreadyUID.Count > 0)
		{
			int iCnt = 0;
			while (iCnt < m_lstSearchMentee.Count)
			{
				int num;
				if (lstAlreadyUID.Find((long e) => e == m_lstSearchMentee[iCnt].commonProfile.userUid) > 0)
				{
					m_lstSearchMentee.RemoveAt(iCnt);
					num = iCnt - 1;
					iCnt = num;
				}
				num = iCnt + 1;
				iCnt = num;
			}
		}
		m_MENTORING_INVITE_LIST_ScrollRect.TotalCount = m_lstSearchMentee.Count;
		m_MENTORING_INVITE_LIST_ScrollRect.SetIndexPosition(0);
		m_MENTORING_INVITE_LIST_ScrollRect.RefreshCells(bForce: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_INVITE, bValue: true);
	}

	private void UpdateUI()
	{
		switch (m_eMentorType)
		{
		case MentoringIdentity.Mentee:
			UpdateMenteeUI();
			break;
		case MentoringIdentity.Mentor:
			UpdateMentorUI();
			break;
		}
	}

	private void UpdateMenteeUI()
	{
		if (m_curMetoringTemplet == null)
		{
			return;
		}
		m_lstMenteeMission.Clear();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKMCommonProfile nKMCommonProfile = null;
		if (NKCScenManager.CurrentUserData().UserProfileData != null)
		{
			nKMCommonProfile = NKCScenManager.CurrentUserData().UserProfileData.commonProfile;
		}
		if (nKMCommonProfile != null)
		{
			nKMCommonProfile.nickname = myUserData.m_UserNickName;
			m_MenteeSlot.SetData(MentoringIdentity.Mentee, nKMCommonProfile, (!NKCGuildManager.HasGuild()) ? null : NKCGuildManager.MyGuildData);
			m_MenteeSlotProfile.SetProfiledata(nKMCommonProfile, null);
		}
		FriendListData myMentor = NKCScenManager.CurrentUserData().MentoringData.MyMentor;
		if (myMentor != null)
		{
			m_MentorSlot.SetData(MentoringIdentity.Mentor, myMentor.commonProfile, myMentor.guildData);
			m_MentorSlotProfile.SetProfiledata(myMentor.commonProfile, null);
		}
		else
		{
			m_MentorSlot.SetData(MentoringIdentity.Mentor, null, new NKMGuildData());
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_SLOT_ADD_BUTTON, myMentor == null);
		NKCUtil.SetGameobjectActive(m_Center_ON, myMentor != null);
		NKCUtil.SetGameobjectActive(m_Center_OFF, myMentor == null);
		NKCUtil.SetGameobjectActive(m_Slot_Area, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_INVITE, bValue: false);
		bool flag = false;
		bool bMenteeGraduate = NKCScenManager.CurrentUserData().MentoringData.bMenteeGraduate;
		int num = 0;
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(m_curMetoringTemplet.AllClearMissionId);
		if (missionTemplet != null && myMentor != null && !bMenteeGraduate)
		{
			flag = true;
			foreach (int item in missionTemplet.m_MissionCond.value1)
			{
				NKMMissionTemplet missionTemplet2 = NKMMissionManager.GetMissionTemplet(item);
				if (missionTemplet2 != null)
				{
					NKMMissionData missionData = myUserData.m_MissionData.GetMissionData(missionTemplet2);
					if (missionData != null && missionData.IsComplete)
					{
						num++;
					}
					m_lstMenteeMission.Add(missionTemplet2);
				}
			}
			m_lstMenteeMission.Sort(NKMMissionManager.Comparer);
			m_MISSION_LIST_ScrollRect.TotalCount = m_lstMenteeMission.Count;
			m_MISSION_LIST_ScrollRect.SetIndexPosition(0);
			m_MISSION_LIST_ScrollRect.RefreshCells(bForce: true);
			if (missionTemplet.m_MissionReward != null && missionTemplet.m_MissionReward.Count > 0)
			{
				bool num2 = m_lstMenteeMission.Count <= num;
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(missionTemplet.m_MissionReward[0].reward_type, missionTemplet.m_MissionReward[0].reward_id, missionTemplet.m_MissionReward[0].reward_value);
				m_MenteeMissionAllClearSlot.SetData(data, bEnableLayoutElement: false, OnClickRewardAllClear);
				if (num2)
				{
					NKMMissionData missionData2 = myUserData.m_MissionData.GetMissionData(missionTemplet);
					if (missionData2 != null && missionData2.IsComplete)
					{
						m_MenteeMissionAllClearSlot.SetDisable(disable: true);
						m_MenteeMissionAllClearSlot.SetEventGet(bActive: true);
						m_MenteeMissionAllClearSlot.SetRewardFx(bActive: false);
						NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_MISSION_LIST_COUNT_TEXT, bValue: false);
						NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_MISSION_LIST_COMPLETE_TEXT, bValue: true);
					}
					else
					{
						m_MenteeMissionAllClearSlot.SetDisable(disable: false);
						m_MenteeMissionAllClearSlot.SetEventGet(bActive: false);
						m_MenteeMissionAllClearSlot.SetRewardFx(bActive: true);
						NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_MISSION_LIST_COUNT_TEXT, bValue: true);
						NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_MISSION_LIST_COMPLETE_TEXT, bValue: false);
					}
				}
				NKCUtil.SetLabelText(m_NKM_UI_FRIEND_MENTORING_MISSION_LIST_COUNT_TEXT, $"{num}/{m_lstMenteeMission.Count}");
				if (m_MENTORING_MISSION_LIST_PROGRESS_SLIDER != null)
				{
					m_MENTORING_MISSION_LIST_PROGRESS_SLIDER.value = ((num == 0) ? 0f : ((float)num / (float)m_lstMenteeMission.Count));
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_MenteeMissionAllClearSlot.gameObject, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_MISSION_COMPLETE, bMenteeGraduate);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_MISSION_LIST, flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_INFO, !flag);
	}

	private void OnClickRewardAllClear(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (NKCScenManager.CurrentUserData().MentoringData.MyMentor == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_FRIEND_MENTORING_MENTEE_MISSION_NOT_COMPLETE);
			return;
		}
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(m_curMetoringTemplet.AllClearMissionId);
		if (missionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(m_curMetoringTemplet.MissionTabId);
		if (missionTabTemplet != null)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null && NKMMissionManager.IsMissionTabExpired(missionTabTemplet, nKMUserData))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(missionTemplet);
			}
		}
	}

	private void UpdateMentorUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_INFO, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_MISSION_LIST, bValue: false);
		NKCUtil.SetGameobjectActive(m_Slot_Area, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_INVITE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_MISSION_COMPLETE, bValue: false);
		int totalCount = 0;
		if (m_lstMyMentee != null && m_lstMyMentee.Count > 0)
		{
			totalCount = m_lstMyMentee.Count;
		}
		m_NKM_UI_FRIEND_LIST_ScrollView.PrepareCells();
		m_NKM_UI_FRIEND_LIST_ScrollView.TotalCount = totalCount;
		m_NKM_UI_FRIEND_LIST_ScrollView.SetIndexPosition(0);
		m_NKM_UI_FRIEND_LIST_ScrollView.RefreshCells(bForce: true);
	}

	public void ResetUI()
	{
		UpdateUI();
	}

	public RectTransform GetMissionSlot(int index)
	{
		NKCUIMissionAchieveSlot newInstance = NKCUIMissionAchieveSlot.GetNewInstance(null, "AB_UI_NKM_UI_FRIEND", "NKM_UI_FRIEND_MENTORING_MISSION_SLOT");
		if (newInstance != null)
		{
			return newInstance.GetComponent<RectTransform>();
		}
		return null;
	}

	public void ReturnMissionSlot(Transform tr)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		tr.SetParent(base.transform);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideData(Transform tr, int index)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		if (component != null)
		{
			NKMMissionTemplet cNKMMissionTemplet = m_lstMenteeMission[index];
			component.SetData(cNKMMissionTemplet, OnClickMove, OnClickComplete);
		}
	}

	public void OnClickMove(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (cNKCUIMissionAchieveSlot == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (nKMMissionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(nKMMissionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, nKMUserData))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED);
			}
			else
			{
				NKCContentManager.MoveToShortCut(nKMMissionTemplet.m_ShortCutType, nKMMissionTemplet.m_ShortCut);
			}
		}
	}

	public void OnClickComplete(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (cNKCUIMissionAchieveSlot == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		if (NKCScenManager.CurrentUserData().MentoringData.MyMentor == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_FRIEND_MENTORING_MENTEE_MISSION_NOT_COMPLETE);
			return;
		}
		NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (nKMMissionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(nKMMissionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, nKMUserData))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(nKMMissionTemplet);
			}
		}
	}

	private void OnClickMentorReward()
	{
		if (m_eMentorType == MentoringIdentity.Mentor)
		{
			NKCPacketSender.Send_NKMPacket_MENTORING_INVITE_REWARD_LIST_REQ();
		}
	}

	private RectTransform GetFriendSlot(int index)
	{
		if (m_slotPool.Count > 0)
		{
			RectTransform rectTransform = m_slotPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			rectTransform.SetParent(m_NKM_UI_FRIEND_LIST_ScrollView.content);
			return rectTransform;
		}
		NKCUIFriendSlot newInstance = NKCUIFriendSlot.GetNewInstance(m_NKM_UI_FRIEND_LIST_ScrollView.content);
		if (newInstance == null)
		{
			return null;
		}
		newInstance.transform.localScale = Vector3.one;
		m_slotList.Add(newInstance);
		return newInstance.GetComponent<RectTransform>();
	}

	public void ReturnFriendSlot(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(m_trMentoringHideParent);
		m_slotPool.Push(tr.GetComponent<RectTransform>());
	}

	public void ProvideFriendSlotToMenteeData(Transform tr, int index)
	{
		if (m_lstMyMentee != null && m_lstMyMentee.Count > 0 && m_lstMyMentee.Count > index)
		{
			_ = m_lstMyMentee[index];
			_ = tr.GetComponent<NKCUIFriendSlot>() != null;
		}
	}

	private RectTransform GetMentoringSlot(int index)
	{
		if (m_mentoringSlotPool.Count > 0)
		{
			RectTransform rectTransform = m_mentoringSlotPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		NKCUIMentoringSlot newInstance = NKCUIMentoringSlot.GetNewInstance(m_rtMentoringSlotParent);
		if (newInstance == null)
		{
			return null;
		}
		newInstance.transform.localScale = Vector3.one;
		m_lstMentoring.Add(newInstance);
		return newInstance.GetComponent<RectTransform>();
	}

	public void ReturnMentoringSlot(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		m_mentoringSlotPool.Push(tr.GetComponent<RectTransform>());
	}

	public void ProvideMentoringData(Transform tr, int index)
	{
		if (m_lstSearchMentee != null && m_lstSearchMentee.Count > 0 && m_lstSearchMentee.Count > index)
		{
			FriendListData friendListData = m_lstSearchMentee[index];
			NKCUIMentoringSlot component = tr.GetComponent<NKCUIMentoringSlot>();
			if (component != null)
			{
				component.SetDataForSearch(friendListData.commonProfile, friendListData.lastLoginDate.Ticks, AlreadySendMenteeUID);
			}
		}
	}

	private void AlreadySendMenteeUID(long sendReqUID)
	{
		lstAlreadyUID.Add(sendReqUID);
	}

	private void OnEndEditSearch(string input)
	{
		if (NKCInputManager.IsChatSubmitEnter())
		{
			if (!m_NKM_UI_FRIEND_TOP_SEARCH_BUTTON.m_bLock)
			{
				OnClickMenteeSearch();
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	private void OnClickMenteeSearch()
	{
		if (m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT != null)
		{
			NKCPacketSender.Send_NKMPacket_MENTORING_SEARCH_LIST_REQ(MentoringIdentity.Mentor, m_NKM_UI_FRIEND_TOP_SEARCH_INPUT_TEXT.text);
		}
	}

	private void OnClickMenteeSearchRefresh()
	{
		NKCPacketSender.Send_kNKMPacket_MENTORING_RECEIVE_LIST_REQ(MentoringIdentity.Mentor, bForce: true);
	}

	private void OnClickMatchListRefresh()
	{
		if (m_eMentorType == MentoringIdentity.Mentor)
		{
			NKCPacketSender.Send_NKMPacket_MENTORING_MATCH_LIST_REQ(bForce: true);
		}
	}
}
