using System;
using System.Collections.Generic;
using NKC.UI.Event;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIMissionAchieveSlot : MonoBehaviour
{
	public delegate void OnClickMASlot(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot);

	[Header("미션 아이콘")]
	public Image m_ImgMissionIcon;

	[Header("미션 반복 표시")]
	public GameObject m_NKM_UI_MISSION_LIST_SLOT_REPEAT_BADGE;

	public Text m_NKM_UI_MISSION_LIST_SLOT_REPEAT_BADGE_Text;

	public GameObject m_NKM_UI_MISSION_LIST_SLOT_REPEAT_CLEAR;

	public GameObject m_NKM_UI_MISSION_LIST_SLOT_CLEAR_IMG;

	public GameObject m_NKM_UI_MISSION_LIST_SLOT_BADGE_BG_DAILY;

	public GameObject m_NKM_UI_MISSION_LIST_SLOT_BADGE_BG_WEEKLY;

	public GameObject m_NKM_UI_MISSION_LIST_SLOT_BADGE_BG_MONTHLY;

	[Header("미션 완료 버튼")]
	public NKCUIComStateButton m_csbtnDisable;

	public NKCUIComStateButton m_csbtnComplete;

	public NKCUIComStateButton m_csbtnProgress;

	public NKCUIComStateButton m_csbtnRefresh;

	[Header("미션 완료 표시")]
	[Tooltip("완료도 하고 보상도 받았을때 나옴")]
	public GameObject m_NKM_UI_MISSION_LIST_SLOT_COMPLETE;

	[Tooltip("완료도 하고 보상도 받았을때 나옴")]
	public GameObject m_NKM_UI_MISSION_LIST_SLOT_COMPLETE_TEXT;

	[Tooltip("완료도 하고 보상도 받은 엠블럼 미션")]
	public GameObject m_NKM_UI_MISSION_LIST_SLOT_EMBLEM_COMPLETE_TEXT;

	[Tooltip("완료만 하고 보상 아직 안받음")]
	public GameObject m_NKM_UI_MISSION_LIST_SLOT_CLEAR;

	[Header("미션 제목")]
	public Text m_NKM_UI_MISSION_LIST_SLOT_MISSION_TITLE;

	[Header("미션 설명")]
	public Text m_NKM_UI_MISSION_LIST_SLOT_MISSION_EXPLAIN;

	[Header("미션 진행도")]
	[Tooltip("미션 진행도 슬라이더")]
	public Slider m_NKM_UI_MISSION_LIST_SLOT_SLIDER;

	[Tooltip("미션 진행도 텍스트 : 0/1 이런거")]
	public Text m_NKM_UI_MISSION_LIST_SLOT_GAUGE_TEXT;

	[Tooltip("미션 완료했을 때 나타나는 오브젝트")]
	public GameObject m_NKM_UI_MISSION_LIST_SLOT_GAUGE_COMPLETE;

	[Header("반복 미션 쿨타임")]
	[Tooltip("쿨타임 중인 미션")]
	public GameObject m_NKM_UI_MISSION_LIST_SLOT_COOL_TIME;

	[Tooltip("다시 시도 가능할때까지 남은 시간")]
	public Text m_NKM_UI_MISSION_LIST_SLOT_BUTTONS_COMPLETE_TEXT;

	[Header("보상 슬롯")]
	public List<NKCUISlot> m_lstRewardSlot;

	public bool m_bShowRewardName;

	[Header("진행 불가능 오브젝트")]
	public GameObject m_objLock;

	private NKMMissionTemplet m_MissionTemplet;

	private NKMMissionData m_MissionData;

	private NKMMissionManager.MissionStateData m_MissionUIData;

	private OnClickMASlot m_OnClickMASlotMove;

	private OnClickMASlot m_OnClickMASlotComplete;

	private OnClickMASlot m_OnClickMASlotRefresh;

	private OnClickMASlot m_OnClickMASlotLocked;

	private float m_fLastUIUpdateTime;

	private bool m_bNeedRefresh;

	private NKCAssetInstanceData m_InstanceData;

	private NKCUIComButton m_csbtnNKM_UI_MISSION_LIST_SLOT_BUTTONS_PROGRESS;

	private NKCUIComButton m_csbtnm_NKM_UI_MISSION_LIST_SLOT_BUTTONS_DISABLE;

	private bool m_bHasCooltime;

	private DateTime m_NextActivateTime;

	public NKMMissionTemplet GetNKMMissionTemplet()
	{
		return m_MissionTemplet;
	}

	public NKMMissionData GetNKMMissionData()
	{
		return m_MissionData;
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
	}

	public static NKCUIMissionAchieveSlot GetNewInstance(Transform parent, string bundleName, string assetName)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCUIMissionAchieveSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIMissionAchieveSlot>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIMissionAchieveSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		component.Init();
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Init()
	{
		for (int i = 0; i < m_lstRewardSlot.Count; i++)
		{
			if (m_lstRewardSlot[i] != null)
			{
				m_lstRewardSlot[i].Init();
			}
		}
	}

	private void Update()
	{
		if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy && m_fLastUIUpdateTime + 1f < Time.time)
		{
			m_fLastUIUpdateTime = Time.time;
			UpdateRemainCoolTimeTextUI();
		}
	}

	public void SetData(NKMMissionTemplet cNKMMissionTemplet, OnClickMASlot OnClickMASlotMove = null, OnClickMASlot OnClickMASlotComplete = null, OnClickMASlot OnClickMASlotRefresh = null, OnClickMASlot onClickMASlotLocked = null)
	{
		bool flag = false;
		if (m_MissionTemplet == cNKMMissionTemplet)
		{
			flag = true;
		}
		m_MissionTemplet = cNKMMissionTemplet;
		m_MissionData = NKMMissionManager.GetMissionData(cNKMMissionTemplet);
		m_MissionUIData = NKMMissionManager.GetMissionStateData(cNKMMissionTemplet);
		if (m_MissionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(m_MissionTemplet.m_MissionTabId);
		if (missionTabTemplet == null)
		{
			return;
		}
		m_OnClickMASlotMove = OnClickMASlotMove;
		m_OnClickMASlotComplete = OnClickMASlotComplete;
		m_OnClickMASlotRefresh = OnClickMASlotRefresh;
		m_OnClickMASlotLocked = onClickMASlotLocked;
		NKCUtil.SetButtonClickDelegate(m_csbtnProgress, OnClickMove);
		NKCUtil.SetButtonClickDelegate(m_csbtnComplete, OnClickComplete);
		NKCUtil.SetButtonClickDelegate(m_csbtnRefresh, OnClickRefresh);
		NKCUtil.SetButtonClickDelegate(m_csbtnDisable, OnClickLocked);
		NKMMissionManager.MissionState state = m_MissionUIData.state;
		NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_CLEAR, state == NKMMissionManager.MissionState.CAN_COMPLETE);
		NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_REPEAT_BADGE, m_MissionTemplet.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.NONE);
		NKCUtil.SetImageSprite(m_ImgMissionIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_MISSION_SPRITE", "AB_UI_" + m_MissionTemplet.m_MissionIcon));
		if (m_MissionTemplet.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.NONE)
		{
			if (!flag)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_BADGE_BG_DAILY, m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.DAILY);
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_BADGE_BG_WEEKLY, m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.WEEKLY);
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_BADGE_BG_MONTHLY, m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.MONTHLY);
				if (m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.DAILY)
				{
					NKCUtil.SetLabelText(m_NKM_UI_MISSION_LIST_SLOT_REPEAT_BADGE_Text, NKCUtilString.GET_STRING_MISSION_RESET_INTERVAL_DAILY);
				}
				else if (m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.WEEKLY)
				{
					NKCUtil.SetLabelText(m_NKM_UI_MISSION_LIST_SLOT_REPEAT_BADGE_Text, NKCUtilString.GET_STRING_MISSION_RESET_INTERVAL_WEEKLY);
				}
				else if (m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.MONTHLY)
				{
					NKCUtil.SetLabelText(m_NKM_UI_MISSION_LIST_SLOT_REPEAT_BADGE_Text, NKCUtilString.GET_STRING_MISSION_RESET_INTERVAL_MONTHLY);
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_BADGE_BG_DAILY, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_BADGE_BG_WEEKLY, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_BADGE_BG_MONTHLY, bValue: false);
		}
		_ = m_MissionTemplet.m_ResetInterval;
		bool flag2 = state == NKMMissionManager.MissionState.CAN_COMPLETE || state == NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE;
		bool flag3 = state == NKMMissionManager.MissionState.REPEAT_COMPLETED || state == NKMMissionManager.MissionState.COMPLETED;
		NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_REPEAT_CLEAR, state == NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE || state == NKMMissionManager.MissionState.REPEAT_COMPLETED);
		NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_CLEAR_IMG, state == NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE);
		NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_GAUGE_COMPLETE, state == NKMMissionManager.MissionState.REPEAT_COMPLETED || state == NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE || state == NKMMissionManager.MissionState.COMPLETED || state == NKMMissionManager.MissionState.CAN_COMPLETE);
		NKCUtil.SetGameobjectActive(m_csbtnComplete, flag2);
		if (state == NKMMissionManager.MissionState.ONGOING)
		{
			if (m_MissionTemplet.m_MissionCond.mission_cond == NKM_MISSION_COND.DONATE_MISSION_ITEM)
			{
				NKCUtil.SetGameobjectActive(m_csbtnProgress, bValue: true);
				NKCUtil.SetGameobjectActive(m_csbtnDisable, bValue: false);
				m_csbtnProgress?.SetTitleText(NKCStringTable.GetString("SI_PF_MISSION_DONATE"));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_csbtnProgress, m_MissionTemplet.m_ShortCutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE);
				NKCUtil.SetGameobjectActive(m_csbtnDisable, m_MissionTemplet.m_ShortCutType == NKM_SHORTCUT_TYPE.SHORTCUT_NONE);
				m_csbtnProgress?.SetTitleText(NKCStringTable.GetString("SI_PF_MISSION_MOVE"));
			}
			NKCUtil.SetGameobjectActive(m_csbtnRefresh, m_MissionTemplet.m_MissionPoolID > 0);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnProgress, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnDisable, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnRefresh, bValue: false);
		}
		NKCUtil.SetLabelText(m_NKM_UI_MISSION_LIST_SLOT_MISSION_TITLE, m_MissionTemplet.GetTitle());
		NKCUtil.SetLabelText(m_NKM_UI_MISSION_LIST_SLOT_MISSION_EXPLAIN, m_MissionTemplet.GetDesc());
		long progressCount = m_MissionUIData.progressCount;
		if (m_NKM_UI_MISSION_LIST_SLOT_SLIDER != null)
		{
			m_NKM_UI_MISSION_LIST_SLOT_SLIDER.value = (float)progressCount / (float)m_MissionTemplet.m_Times;
		}
		if (state == NKMMissionManager.MissionState.ONGOING || state == NKMMissionManager.MissionState.LOCKED)
		{
			NKCUtil.SetLabelText(m_NKM_UI_MISSION_LIST_SLOT_GAUGE_TEXT, $"{progressCount} / {m_MissionTemplet.m_Times}");
		}
		else
		{
			NKCUtil.SetLabelText(m_NKM_UI_MISSION_LIST_SLOT_GAUGE_TEXT, "");
		}
		NKCScenManager.GetScenManager().GetMyUserData();
		m_bHasCooltime = false;
		if (missionTabTemplet.HasDateLimit && !NKCSynchronizedTime.IsStarted(missionTabTemplet.m_startTimeUtc))
		{
			m_bHasCooltime = true;
			m_NextActivateTime = missionTabTemplet.m_startTimeUtc;
		}
		if (!NKMContentUnlockManager.IsStarted(missionTabTemplet.m_UnlockInfo))
		{
			m_bHasCooltime = true;
			m_NextActivateTime = NKMContentUnlockManager.GetConditionStartTime(missionTabTemplet.m_UnlockInfo);
		}
		else if (m_MissionTemplet.m_ResetInterval != NKM_MISSION_RESET_INTERVAL.NONE && m_MissionUIData.IsMissionCompleted)
		{
			m_bHasCooltime = true;
			m_NextActivateTime = GetNextResetTime();
		}
		if (!m_bHasCooltime)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COOL_TIME, bValue: false);
			if (m_MissionUIData.IsMissionCompleted)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COMPLETE, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COMPLETE_TEXT, missionTabTemplet.m_MissionType != NKM_MISSION_TYPE.EMBLEM);
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_EMBLEM_COMPLETE_TEXT, missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.EMBLEM);
				NKCUtil.SetGameobjectActive(m_csbtnComplete, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnProgress, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COMPLETE, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COOL_TIME, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnComplete, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnProgress, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COMPLETE, bValue: false);
			UpdateRemainCoolTimeTextUI();
		}
		if (!flag)
		{
			for (int i = 0; i < m_MissionTemplet.m_MissionReward.Count; i++)
			{
				if (m_lstRewardSlot.Count > i)
				{
					MissionReward missionReward = m_MissionTemplet.m_MissionReward[i];
					NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(missionReward.reward_type, missionReward.reward_id, missionReward.reward_value);
					if (m_bShowRewardName)
					{
						bool bShowNumber = false;
						switch (slotData.eType)
						{
						case NKCUISlot.eSlotMode.ItemMisc:
						case NKCUISlot.eSlotMode.UnitCount:
							bShowNumber = true;
							break;
						case NKCUISlot.eSlotMode.Mold:
						{
							NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(slotData.ID);
							if (itemMoldTempletByID != null)
							{
								bShowNumber = !itemMoldTempletByID.m_bPermanent;
							}
							break;
						}
						}
						m_lstRewardSlot[i].SetData(slotData, bShowName: true, bShowNumber, bEnableLayoutElement: true, null);
						m_lstRewardSlot[i].SetOpenItemBoxOnClick();
					}
					else
					{
						m_lstRewardSlot[i].SetData(slotData);
					}
					m_lstRewardSlot[i].SetActive(bSet: true);
				}
				else
				{
					Debug.LogError($"보상슬롯 갯수가 부족함 - 미션아이디 : {m_MissionTemplet.m_MissionID}, 슬롯 갯수 : {m_lstRewardSlot.Count}, 보상 갯수 : {m_MissionTemplet.m_MissionReward.Count}");
				}
			}
			for (int j = m_MissionTemplet.m_MissionReward.Count; j < m_lstRewardSlot.Count; j++)
			{
				m_lstRewardSlot[j].SetActive(bSet: false);
			}
		}
		if (missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.EVENT_PASS && (flag2 || flag3))
		{
			NKCUtil.SetGameobjectActive(m_csbtnRefresh, bValue: false);
		}
		m_bNeedRefresh = false;
		bool flag4 = NKMMissionManager.IsMissionOpened(m_MissionTemplet);
		NKCUtil.SetGameobjectActive(m_objLock, !flag4 && !m_bHasCooltime);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	private DateTime GetNextResetTime()
	{
		DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime(-1.0);
		NKMTime.TimePeriod timePeriod = NKMTime.TimePeriod.Day;
		if (m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.DAILY)
		{
			timePeriod = NKMTime.TimePeriod.Day;
		}
		else if (m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.WEEKLY)
		{
			timePeriod = NKMTime.TimePeriod.Week;
		}
		else if (m_MissionTemplet.m_ResetInterval == NKM_MISSION_RESET_INTERVAL.MONTHLY)
		{
			timePeriod = NKMTime.TimePeriod.Month;
		}
		return NKMTime.GetNextResetTime(serverUTCTime, timePeriod);
	}

	private void UpdateRemainCoolTimeTextUI()
	{
		if (!m_bHasCooltime)
		{
			return;
		}
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(m_NextActivateTime);
		if (timeLeft.TotalMilliseconds <= 0.0 && !m_bNeedRefresh)
		{
			m_bNeedRefresh = true;
			if (NKCUIMissionAchievement.IsInstanceOpen)
			{
				NKCUIMissionAchievement.Instance.RefreshMissionUI();
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
			}
			if (NKCUIEvent.IsInstanceOpen)
			{
				NKCUIEvent.Instance.RefreshUI();
			}
		}
		NKCUtil.SetLabelText(msg: (timeLeft.TotalDays >= 1.0) ? string.Format(NKCUtilString.GET_STRING_MISSION_REMAIN_TWO_PARAM, NKCUtilString.GET_STRING_TIME_PERIOD, string.Format(NKCUtilString.GET_STRING_TIME_DAY_ONE_PARAM, (int)timeLeft.TotalDays)) : ((timeLeft.TotalHours >= 1.0) ? string.Format(NKCUtilString.GET_STRING_MISSION_REMAIN_TWO_PARAM, NKCUtilString.GET_STRING_TIME, string.Format(NKCUtilString.GET_STRING_TIME_HOUR_ONE_PARAM, (int)timeLeft.TotalHours)) : ((timeLeft.TotalMinutes >= 1.0) ? string.Format(NKCUtilString.GET_STRING_MISSION_REMAIN_TWO_PARAM, NKCUtilString.GET_STRING_TIME, string.Format(NKCUtilString.GET_STRING_TIME_MINUTE_ONE_PARAM, (int)timeLeft.TotalMinutes)) : ((!(timeLeft.TotalSeconds >= 1.0)) ? string.Format(NKCUtilString.GET_STRING_MISSION_REMAIN_TWO_PARAM, NKCUtilString.GET_STRING_TIME, NKCUtilString.GET_STRING_TIME_A_SECOND_AGO) : string.Format(NKCUtilString.GET_STRING_MISSION_REMAIN_TWO_PARAM, NKCUtilString.GET_STRING_TIME, string.Format(NKCUtilString.GET_STRING_TIME_SECOND_ONE_PARAM, (int)timeLeft.TotalSeconds))))), label: m_NKM_UI_MISSION_LIST_SLOT_BUTTONS_COMPLETE_TEXT);
	}

	public void OnClickMove()
	{
		if (m_MissionTemplet != null && m_MissionTemplet.m_MissionCond.mission_cond == NKM_MISSION_COND.DONATE_MISSION_ITEM)
		{
			OnDonate();
		}
		else if (m_OnClickMASlotMove != null)
		{
			m_OnClickMASlotMove(this);
		}
	}

	public void OnClickComplete()
	{
		if (m_OnClickMASlotComplete != null)
		{
			m_OnClickMASlotComplete(this);
		}
	}

	public void OnClickRefresh()
	{
		if (m_OnClickMASlotRefresh != null)
		{
			m_OnClickMASlotRefresh(this);
		}
	}

	public void OnClickLocked()
	{
		if (m_OnClickMASlotLocked != null)
		{
			m_OnClickMASlotLocked(this);
		}
	}

	private void OnDonate()
	{
		if (m_MissionTemplet != null && m_MissionTemplet.m_MissionCond.value1.Count != 0)
		{
			int num = m_MissionTemplet.m_MissionCond.value1[0];
			long times = m_MissionTemplet.m_Times;
			long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(num);
			long progressCount = m_MissionUIData.progressCount;
			int num2 = (int)(times - progressCount);
			int num3 = (int)Math.Min(num2, countMiscItem);
			int minValue = ((num3 > 0) ? 1 : 0);
			NKCUISlot.SlotData itemSlotData = NKCUISlot.SlotData.MakeMiscItemData(num, 1L);
			string itemName = NKMItemManager.GetItemMiscTempletByID(num).GetItemName();
			string title = NKCStringTable.GetString("SI_DP_POPUP_DELIVERY_MISSION_TITLE");
			string desc = string.Format(NKCStringTable.GetString("SI_PF_POPUP_DELIVERY_MISSION_DESC"), itemName);
			NKCPopupItemSlider.Instance.Open(title, desc, itemSlotData, minValue, num3, num2, bShowHaveCount: true, OnDonateConfirm, num3);
		}
	}

	private void OnDonateConfirm(int count)
	{
		if (count > 0)
		{
			_ = m_MissionTemplet.m_MissionCond.value1[0];
			NKCPacketSender.Send_NKMPacket_MISSION_GIVE_ITEM_REQ(m_MissionTemplet.m_MissionID, count);
		}
	}

	public void SetActive(bool bSet)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bSet);
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void SetForceActivateEventPassRefreshButton()
	{
		NKMMissionManager.MissionState state = m_MissionUIData.state;
		bool num = state == NKMMissionManager.MissionState.CAN_COMPLETE || state == NKMMissionManager.MissionState.REPEAT_CAN_COMPLETE;
		bool flag = state == NKMMissionManager.MissionState.REPEAT_COMPLETED || state == NKMMissionManager.MissionState.COMPLETED;
		if (!(num || flag))
		{
			NKCUtil.SetGameobjectActive(m_csbtnRefresh, bValue: true);
		}
	}

	public void SetForceMissionDisabled()
	{
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(m_MissionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (!m_bHasCooltime)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COOL_TIME, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COMPLETE, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COMPLETE_TEXT, missionTabTemplet.m_MissionType != NKM_MISSION_TYPE.EMBLEM);
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_EMBLEM_COMPLETE_TEXT, missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.EMBLEM);
				NKCUtil.SetGameobjectActive(m_csbtnComplete, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnProgress, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COOL_TIME, bValue: true);
				NKCUtil.SetGameobjectActive(m_csbtnComplete, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnProgress, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_MISSION_LIST_SLOT_COMPLETE, bValue: false);
				UpdateRemainCoolTimeTextUI();
			}
		}
	}
}
