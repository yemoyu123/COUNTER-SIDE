using System.Collections;
using NKC.UI.Guide;
using NKM.Event;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUIEventSubUIBar : NKCUIEventSubUIBase
{
	private enum Phase
	{
		ENTRY,
		CREATE
	}

	public Animator m_AniEventGremoryBar;

	public NKCUIEventBarPhaseEntry m_eventBarPhaseEntry;

	public NKCUIEventBarPhaseCreate m_eventBarPhaseCreate;

	public NKCUIEventBarResult m_eventBarResult;

	public NKCUIComStateButton m_csbtnCreatePhase;

	public NKCUIComStateButton m_csbtnInitialPhase;

	public NKCUIComStateButton m_csbtnHelp;

	public float m_introDuration;

	[Header("GUIDE ID(숫자)")]
	public int m_guideId;

	private Phase m_phase;

	private bool forceEntry;

	private bool m_bBeginnerOpen;

	private const string ALREADY_OPENED_KEY = "EVENT_BAR_ALREADY_OPENED";

	private float m_introTimer;

	public static int EventID;

	public static bool RefreshUI;

	public override void Init()
	{
		base.Init();
		m_eventBarPhaseEntry?.Init();
		m_eventBarPhaseCreate?.Init();
		m_eventBarResult?.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnCreatePhase, OnClickCreatePhase);
		NKCUtil.SetButtonClickDelegate(m_csbtnInitialPhase, OnClickInitialPhase);
		NKCUtil.SetButtonClickDelegate(m_csbtnHelp, OnClickHelp);
		NKCUtil.SetHotkey(m_csbtnCreatePhase, HotkeyEventType.NextTab);
		NKCUtil.SetHotkey(m_csbtnInitialPhase, HotkeyEventType.NextTab);
		NKCUtil.SetHotkey(m_csbtnHelp, HotkeyEventType.Help);
		m_AniEventGremoryBar.keepAnimatorControllerStateOnDisable = true;
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		if (tabTemplet != null)
		{
			m_tabTemplet = tabTemplet;
			Open(tabTemplet.m_EventID);
			SetDateLimit();
		}
	}

	public void Open(NKMEventCollectionIndexTemplet eventCollectionIndexTemplet)
	{
		if (eventCollectionIndexTemplet != null)
		{
			NKMEventTabTemplet tabTemplet = NKMEventTabTemplet.Find(NKCUtil.GetIntValue(eventCollectionIndexTemplet.m_Option, "EventTabID", 0));
			Open(tabTemplet);
		}
	}

	private void Open(int eventID)
	{
		NKCUIVoiceManager.StopVoice();
		EventID = eventID;
		m_AniEventGremoryBar.Rebind();
		m_AniEventGremoryBar?.SetTrigger("INTRO");
		m_eventBarPhaseEntry?.SetData(eventID);
		forceEntry = false;
		if (m_eventBarResult != null)
		{
			m_eventBarResult.Init();
			m_eventBarResult.Close();
		}
		m_phase = Phase.ENTRY;
		RefreshUI = false;
		m_introTimer = m_introDuration;
		StartCoroutine(IntroTimeElapse());
		if (PlayerPrefs.GetInt("EVENT_BAR_ALREADY_OPENED") == 0 && m_guideId > 0)
		{
			m_bBeginnerOpen = true;
			StartCoroutine(OpenHelp());
		}
		else
		{
			m_bBeginnerOpen = false;
		}
	}

	public override void Refresh()
	{
		if (forceEntry)
		{
			Open(EventID);
			return;
		}
		if (RefreshUI)
		{
			switch (m_phase)
			{
			case Phase.ENTRY:
				m_eventBarPhaseEntry?.Refresh();
				break;
			case Phase.CREATE:
				m_eventBarPhaseCreate?.Refresh();
				break;
			}
			RefreshUI = false;
		}
		if (NKCPopupEventBarMission.IsInstanceOpen)
		{
			NKCPopupEventBarMission.Instance.RefreshMission();
		}
	}

	public override void Close()
	{
		base.Close();
		m_eventBarPhaseEntry?.Close();
		m_eventBarPhaseCreate?.Close();
		NKCUIVoiceManager.StopVoice();
		forceEntry = true;
	}

	public void ActivateCreateFx()
	{
		if (m_phase == Phase.CREATE)
		{
			m_eventBarPhaseCreate.ActivateCreateFx();
		}
	}

	public override void Hide()
	{
		base.Hide();
		if (m_phase == Phase.ENTRY)
		{
			m_eventBarPhaseEntry.Hide();
		}
	}

	private IEnumerator OpenHelp()
	{
		float delayTime = 1f;
		while (delayTime > 0f)
		{
			delayTime -= Time.deltaTime;
			yield return null;
		}
		PlayerPrefs.SetInt("EVENT_BAR_ALREADY_OPENED", 1);
		m_bBeginnerOpen = false;
		OnClickHelp();
	}

	private IEnumerator IntroTimeElapse()
	{
		while (m_introTimer > 0f)
		{
			m_introTimer -= Time.deltaTime;
			yield return null;
		}
	}

	private void SetDateLimit(NKMEventCollectionIndexTemplet eventCollectionIndexTemplet)
	{
		if (eventCollectionIndexTemplet != null)
		{
			NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(eventCollectionIndexTemplet.DateStrId);
			if (nKMIntervalTemplet != null)
			{
				if (NKCSynchronizedTime.GetTimeLeft(NKMTime.LocalToUTC(nKMIntervalTemplet.EndDate)).TotalDays > (double)NKCSynchronizedTime.UNLIMITD_REMAIN_DAYS)
				{
					NKCUtil.SetLabelText(m_lbEventLimitDate, NKCUtilString.GET_STRING_EVENT_DATE_UNLIMITED_TEXT);
				}
				else
				{
					NKCUtil.SetLabelText(m_lbEventLimitDate, NKCUtilString.GetTimeIntervalString(nKMIntervalTemplet.StartDate, nKMIntervalTemplet.EndDate, NKMTime.INTERVAL_FROM_UTC));
				}
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbEventLimitDate, "");
		}
	}

	private void OnClickCreatePhase()
	{
		if (!m_bBeginnerOpen && !(m_introTimer > 0f))
		{
			NKCUIVoiceManager.StopVoice();
			m_eventBarPhaseCreate?.SetData(EventID);
			m_AniEventGremoryBar?.SetTrigger("TRANS_TO_PH2");
			m_phase = Phase.CREATE;
		}
	}

	private void OnClickInitialPhase()
	{
		if (!NKCUIEventBarResult.IsInstanceOpen && !m_bBeginnerOpen)
		{
			NKCUIVoiceManager.StopVoice();
			m_eventBarPhaseCreate?.OnLeavePhase();
			m_eventBarPhaseEntry?.SetData(EventID);
			m_AniEventGremoryBar?.SetTrigger("TRANS_TO_PH1");
			m_phase = Phase.ENTRY;
		}
	}

	private void OnClickHelp()
	{
		if (!m_bBeginnerOpen)
		{
			NKCUIPopupTutorialImagePanel.Instance.Open(m_guideId, null);
		}
	}

	public bool OnClickClose()
	{
		if (m_phase == Phase.CREATE)
		{
			OnClickInitialPhase();
			return true;
		}
		return false;
	}
}
