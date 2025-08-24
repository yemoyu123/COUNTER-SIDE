using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Friend;

public class NKCPopupFriendMentoringInviteReward : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_friend";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FRIEND_MENTORING_INVITE_REWARD";

	private static NKCPopupFriendMentoringInviteReward m_Instance;

	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSE_BUTTON;

	public EventTrigger m_NKM_UI_POPUP_FRIEND_MENTORING_INVITE_REWARD_BG;

	public Text m_MENTORING_INVITE_COMPLETE_COUNT_TEXT_02;

	public NKCUIComStateButton m_BUTTON_ALL;

	public List<NKCUISlot> m_lstReward = new List<NKCUISlot>();

	public Slider m_MENTORING_MISSION_LIST_PROGRESS_SLIDER;

	public List<GameObject> m_lstCircleOff = new List<GameObject>();

	public List<GameObject> m_lstCircleOn = new List<GameObject>();

	public List<Text> m_lstRewardNum = new List<Text>();

	private const int FIXED_REWARD_COUNT = 5;

	private UnityAction m_callBack;

	public static NKCPopupFriendMentoringInviteReward Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFriendMentoringInviteReward>("ab_ui_nkm_ui_friend", "NKM_UI_POPUP_FRIEND_MENTORING_INVITE_REWARD", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupFriendMentoringInviteReward>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "NKCPopupFriendMentoringSearch";

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
		if (m_NKM_UI_POPUP_FRIEND_MENTORING_INVITE_REWARD_BG != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_NKM_UI_POPUP_FRIEND_MENTORING_INVITE_REWARD_BG.triggers.Add(entry);
		}
		if (m_lstReward.Count > 5)
		{
			Debug.LogError($"{5}개 사용하기로 정함");
		}
		if (m_BUTTON_ALL != null)
		{
			m_BUTTON_ALL.PointerClick.RemoveAllListeners();
			m_BUTTON_ALL.PointerClick.AddListener(OnAllReceive);
			NKCUtil.SetHotkey(m_BUTTON_ALL, HotkeyEventType.Confirm);
		}
		if (m_NKM_UI_POPUP_CLOSE_BUTTON != null)
		{
			m_NKM_UI_POPUP_CLOSE_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_POPUP_CLOSE_BUTTON.PointerClick.AddListener(CheckInstanceAndClose);
		}
		foreach (NKCUISlot item in m_lstReward)
		{
			item?.Init();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnAllReceive()
	{
		NKCPacketSender.Send_NKMPacket_MENTORING_COMPLETE_INVITE_REWARD_ALL_REQ();
	}

	public void Open(HashSet<int> rewardHistories, UnityAction callback = null)
	{
		int menteeMissionCompletCnt = NKCScenManager.CurrentUserData().GetMenteeMissionCompletCnt();
		NKCUtil.SetLabelText(m_MENTORING_INVITE_COMPLETE_COUNT_TEXT_02, menteeMissionCompletCnt.ToString());
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		ResetUI(rewardHistories.ToList());
		m_callBack = callback;
		UIOpened();
	}

	public void ResetUI(List<int> rewardHistories)
	{
		NKMMentoringTemplet currentTempet = NKCMentoringUtil.GetCurrentTempet();
		if (currentTempet == null)
		{
			return;
		}
		IOrderedEnumerable<NKMMentoringRewardTemplet> orderedEnumerable = from x in NKMMentoringRewardTemplet.GetRewardGroupList(currentTempet.RewardGroupId)
			orderby x.InviteSuccessRequireCnt
			select x;
		if (orderedEnumerable.Count() > 5)
		{
			Debug.LogError($"{5}개 사용하기로 정함");
		}
		int num = 0;
		bool flag = false;
		int menteeMissionCompletCnt = NKCScenManager.CurrentUserData().GetMenteeMissionCompletCnt();
		int num2 = 0;
		List<int> list = new List<int>();
		foreach (NKMMentoringRewardTemplet data in orderedEnumerable)
		{
			if (data == null)
			{
				continue;
			}
			if (data.InviteSuccessRequireCnt > num2)
			{
				num2 = data.InviteSuccessRequireCnt;
			}
			bool num3 = rewardHistories.Contains(data.InviteSuccessRequireCnt);
			bool flag2 = false;
			bool flag3 = data.InviteSuccessRequireCnt <= menteeMissionCompletCnt;
			if (!num3 && flag3)
			{
				flag2 = true;
				flag = true;
			}
			NKCUtil.SetGameobjectActive(m_lstCircleOff[num], !flag3);
			NKCUtil.SetGameobjectActive(m_lstCircleOn[num], flag3);
			NKCUISlot.SlotData data2 = NKCUISlot.SlotData.MakeRewardTypeData(data.RewardType, data.RewardId, data.RewardCount);
			NKCUISlot.OnClick onClick = null;
			if (!num3 && flag2)
			{
				onClick = delegate
				{
					NKCPacketSender.Send_NKMPacket_MENTORING_COMPLETE_INVITE_REWARD_REQ(data.InviteSuccessRequireCnt);
				};
			}
			m_lstReward[num].SetData(data2, bEnableLayoutElement: false, onClick);
			m_lstReward[num].SetRewardFx(flag3 && flag2);
			m_lstReward[num].SetDisable(flag3 && !flag2);
			m_lstReward[num].SetEventGet(flag3 && !flag2);
			NKCUtil.SetLabelText(m_lstRewardNum[num], data.InviteSuccessRequireCnt.ToString());
			list.Add(data.InviteSuccessRequireCnt);
			num++;
		}
		if (m_MENTORING_MISSION_LIST_PROGRESS_SLIDER != null)
		{
			float value = 0f;
			if (menteeMissionCompletCnt > 0)
			{
				float num4 = 0f;
				if (list.Count > 0)
				{
					num4 = m_MENTORING_MISSION_LIST_PROGRESS_SLIDER.maxValue / (float)list.Count;
				}
				for (int num5 = 0; num5 < list.Count; num5++)
				{
					if (list[num5] == menteeMissionCompletCnt)
					{
						value = (float)(num5 + 1) * num4;
						break;
					}
					if (list[num5] > menteeMissionCompletCnt)
					{
						value = (float)num5 * num4;
						if (value >= m_MENTORING_MISSION_LIST_PROGRESS_SLIDER.maxValue)
						{
							value = m_MENTORING_MISSION_LIST_PROGRESS_SLIDER.maxValue;
							break;
						}
						int num6 = list[num5] - list[num5 - 1];
						float num7 = num4 / (float)num6 * (float)(num6 - (list[num5] - menteeMissionCompletCnt));
						value += num7;
						break;
					}
				}
			}
			m_MENTORING_MISSION_LIST_PROGRESS_SLIDER.value = value;
		}
		if (!flag)
		{
			m_BUTTON_ALL.Lock();
			NKCScenManager.CurrentUserData().SetMentoringNotify(bSet: false);
			if (m_callBack != null)
			{
				m_callBack();
			}
		}
		else
		{
			m_BUTTON_ALL.UnLock();
		}
	}
}
