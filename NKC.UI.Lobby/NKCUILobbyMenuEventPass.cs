using System;
using NKM;
using NKM.EventPass;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuEventPass : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objRoot;

	public GameObject m_objNotify;

	public Image m_imgCounterPass;

	public Image m_imgCounterGauge;

	public Text m_lbLeftTime;

	public Text m_lbCounterPassLv;

	public GameObject m_objEmpty;

	private NKMEventPassTemplet m_EventPassTemplet;

	private float updateTimer = -1f;

	private static DateTime initUTCTime;

	private static bool initDateTime;

	public void Init(ContentsType contentsType = ContentsType.None)
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
		}
		m_ContentsType = contentsType;
		InitTime();
	}

	private void InitTime()
	{
		if (!initDateTime)
		{
			initUTCTime = NKCSynchronizedTime.GetServerUTCTime();
			initDateTime = true;
		}
		NKCUIEventPass.m_dOnMissionUpdate = CheckResetMissionTime;
	}

	private void CheckResetMissionTime()
	{
		if (NKCSynchronizedTime.IsFinished(NKMTime.GetNextResetTime(initUTCTime, NKMTime.TimePeriod.Day)))
		{
			NKCUIEventPass.DailyMissionRedDot = false;
			initUTCTime = NKCSynchronizedTime.GetServerUTCTime();
		}
		if (NKCSynchronizedTime.IsFinished(NKMTime.GetNextResetTime(initUTCTime, NKMTime.TimePeriod.Week)))
		{
			NKCUIEventPass.WeeklyMissionRedDot = false;
			initUTCTime = NKCSynchronizedTime.GetServerUTCTime();
		}
	}

	public void Update()
	{
		UpdateEventPassEnabled();
		UpdateLeftTime();
	}

	public void UpdateEventPassEnabled()
	{
		updateTimer -= Time.deltaTime;
		if (updateTimer <= 0f)
		{
			CheckButtonEnable();
			CheckResetMissionTime();
			if (base.gameObject.activeSelf && m_EventPassTemplet == null)
			{
				CheckPassInfo();
			}
			updateTimer = 1f;
		}
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		UpdateContents(userData);
	}

	public void UpdateContents(NKMUserData userData)
	{
		CheckButtonEnable();
		CheckResetMissionTime();
		CheckPassInfo();
	}

	private void CheckPassInfo()
	{
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager != null)
		{
			NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Find(eventPassDataManager.EventPassId);
			if (nKMEventPassTemplet != null)
			{
				m_EventPassTemplet = nKMEventPassTemplet;
				UpdatePassExpAndLv();
				UpdatePassIcon();
				UpdateLeftTime();
			}
		}
	}

	protected override void UpdateLock()
	{
		m_bLocked = !NKCContentManager.IsContentsUnlocked(m_ContentsType);
		UpdateLock(m_bLocked);
	}

	public void UpdateLock(bool bLock)
	{
		NKCUtil.SetLabelText(m_lbLock, NKCContentManager.GetLockedMessage(m_ContentsType));
		NKCUtil.SetGameobjectActive(m_objLock, bLock);
		m_bLocked = bLock;
		if (!m_bLocked)
		{
			UpdateContents(NKCScenManager.CurrentUserData());
		}
	}

	private void CheckButtonEnable()
	{
		bool flag = NKCUIEventPass.IsEventTime(activateAlarm: false);
		NKCUtil.SetGameobjectActive(m_objRoot, flag);
		NKCUtil.SetGameobjectActive(m_objEmpty, !flag);
		if (flag)
		{
			bool bValue = NKCUIEventPass.RewardRedDot || NKCUIEventPass.DailyMissionRedDot || NKCUIEventPass.WeeklyMissionRedDot;
			NKCUtil.SetGameobjectActive(m_objNotify, bValue);
		}
	}

	private void OnButton()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(m_ContentsType);
		}
		else
		{
			if (!NKCUIEventPass.IsEventTime())
			{
				return;
			}
			NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
			if (eventPassDataManager != null)
			{
				if (!eventPassDataManager.EventPassDataReceived)
				{
					NKCUIEventPass.OpenUIStandby = true;
					NKCUIEventPass.EventPassDataManager = null;
					NKCPacketSender.Send_NKMPacket_EVENT_PASS_REQ(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
				}
				else
				{
					NKCUIEventPass.Instance.Open(eventPassDataManager);
				}
			}
		}
	}

	private void UpdatePassIcon()
	{
		if (m_EventPassTemplet == null)
		{
			return;
		}
		if (m_EventPassTemplet.EventPassMainRewardType == NKM_REWARD_TYPE.RT_SKIN)
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_EventPassTemplet.EventPassMainReward);
			if (skinTemplet != null)
			{
				NKCUtil.SetImageSprite(m_imgCounterPass, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, skinTemplet), bDisableIfSpriteNull: true);
				return;
			}
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_EventPassTemplet.EventPassMainReward);
		if (unitTempletBase != null)
		{
			NKCUtil.SetImageSprite(m_imgCounterPass, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase), bDisableIfSpriteNull: true);
		}
	}

	private void UpdatePassExpAndLv()
	{
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager != null && m_EventPassTemplet != null)
		{
			int totalExp = eventPassDataManager.TotalExp;
			int passLevelUpExp = m_EventPassTemplet.PassLevelUpExp;
			int passMaxLevel = m_EventPassTemplet.PassMaxLevel;
			int num = totalExp % passLevelUpExp;
			int num2 = Mathf.Min(passMaxLevel, totalExp / passLevelUpExp + 1);
			NKCUtil.SetLabelText(m_lbCounterPassLv, num2.ToString());
			if (num2 >= passMaxLevel)
			{
				num = passLevelUpExp;
			}
			NKCUtil.SetImageFillAmount(m_imgCounterGauge, (float)num / (float)passLevelUpExp);
		}
	}

	public void UpdateLeftTime()
	{
		if (m_EventPassTemplet == null)
		{
			return;
		}
		if (NKCSynchronizedTime.IsFinished(m_EventPassTemplet.EventPassEndDate))
		{
			m_EventPassTemplet = null;
			return;
		}
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(NKCSynchronizedTime.ToUtcTime(m_EventPassTemplet.EventPassEndDate));
		if (timeLeft.Ticks <= 0)
		{
			m_EventPassTemplet = null;
			return;
		}
		string remainTimeString = NKCUtilString.GetRemainTimeString(timeLeft, 1);
		NKCUtil.SetLabelText(m_lbLeftTime, remainTimeString);
	}
}
