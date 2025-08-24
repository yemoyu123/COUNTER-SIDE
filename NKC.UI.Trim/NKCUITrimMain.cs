using System;
using System.Collections;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Trim;

public class NKCUITrimMain : NKCUIBase
{
	public enum TrimState
	{
		Entry,
		Selected
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_trim";

	private const string UI_ASSET_NAME = "AB_UI_TRIM";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public Animator m_aniEnter;

	public Animator m_aniInfo;

	public string m_aniEnterIdle;

	public string m_aniInfo_01_to_02;

	public string m_aniInfo_02_to_01;

	public string m_aniInfo_02;

	public NKCUITrimSlot[] m_trimSlot;

	public NKCUITrimMainInfo m_TrimMainInfo;

	public GameObject m_objRemainTime;

	public Text m_lbRemainData;

	public string m_bgmName;

	private TrimState m_eTrimState;

	private int m_selectedIndex;

	private List<int> m_trimIdList = new List<int>();

	private float m_dateUpdateTimerSec;

	private float m_dateUpdateTimerMin;

	private bool m_bShowIntervalTime;

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsUIOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "TRIM";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override string GuideTempletID => "ARTICLE_TRIM_INFO";

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUITrimMain>("ab_ui_trim", "AB_UI_TRIM", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUITrimMain GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUITrimMain>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public override void CloseInternal()
	{
		NKCSoundManager.PlayScenMusic();
		m_trimIdList.Clear();
		base.gameObject.SetActive(value: false);
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
	}

	public override void OnBackButton()
	{
		if (m_eTrimState == TrimState.Selected)
		{
			SetToEntryState();
			return;
		}
		base.OnBackButton();
		NKCScenManager.GetScenManager()?.Get_SCEN_OPERATION().SetReservedEpisodeCategory(EPISODE_CATEGORY.EC_TRIM);
		NKCScenManager.GetScenManager()?.ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION, bForce: false);
	}

	public void Init()
	{
		if (m_trimSlot != null)
		{
			int num = m_trimSlot.Length;
			for (int i = 0; i < num; i++)
			{
				m_trimSlot[i]?.Init(i, OnClickTrimSlot);
			}
		}
		m_TrimMainInfo?.Init();
		m_aniEnter.keepAnimatorControllerStateOnDisable = true;
		m_aniInfo.keepAnimatorControllerStateOnDisable = true;
	}

	public void Open(NKMTrimIntervalTemplet trimIntervalTemplet, int prevTrimId)
	{
		if (trimIntervalTemplet == null)
		{
			return;
		}
		m_trimIdList.Clear();
		base.gameObject.SetActive(value: true);
		int[] array = null;
		if (trimIntervalTemplet != null)
		{
			array = trimIntervalTemplet.TrimSlot;
		}
		if (array != null)
		{
			int num = m_trimSlot.Length;
			for (int i = 0; i < num; i++)
			{
				if (array.Length <= i || array[i] <= 0)
				{
					m_trimSlot[i].SetActive(value: false);
					continue;
				}
				m_trimSlot[i].SetActive(value: true);
				m_trimSlot[i].SetData(array[i]);
				m_trimSlot[i].SetSlotState(NKCUITrimSlot.SlotState.Default);
				m_trimIdList.Add(array[i]);
			}
		}
		m_eTrimState = TrimState.Entry;
		m_selectedIndex = -1;
		m_dateUpdateTimerSec = 0f;
		m_dateUpdateTimerMin = 0f;
		string remainTimeStringExWithoutEnd = NKCUtilString.GetRemainTimeStringExWithoutEnd(NKCUITrimUtility.GetRemainDateMsg());
		NKCUtil.SetLabelText(m_lbRemainData, remainTimeStringExWithoutEnd);
		NKCSoundManager.PlayMusic(m_bgmName, bLoop: true);
		UIOpened();
		NKCTutorialManager.TutorialRequired(TutorialPoint.TrimEntry);
		SetTrimSelectState(prevTrimId);
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		int count = m_trimIdList.Count;
		if (count == 0)
		{
			return false;
		}
		switch (hotkey)
		{
		case HotkeyEventType.NextTab:
			if (m_eTrimState == TrimState.Entry)
			{
				m_selectedIndex = 0;
				if (m_selectedIndex < m_trimIdList.Count)
				{
					OnClickTrimSlot(m_selectedIndex, m_trimIdList[m_selectedIndex]);
				}
				return true;
			}
			m_selectedIndex = (m_selectedIndex + 1) % count;
			if (m_selectedIndex < m_trimIdList.Count)
			{
				OnClickTrimSlot(m_selectedIndex, m_trimIdList[m_selectedIndex]);
			}
			return true;
		case HotkeyEventType.PrevTab:
			if (m_eTrimState == TrimState.Entry)
			{
				m_selectedIndex = 0;
				if (m_selectedIndex < m_trimIdList.Count)
				{
					OnClickTrimSlot(m_selectedIndex, m_trimIdList[m_selectedIndex]);
				}
				return true;
			}
			m_selectedIndex = (m_selectedIndex - 1 + count) % count;
			if (m_selectedIndex < m_trimIdList.Count)
			{
				OnClickTrimSlot(m_selectedIndex, m_trimIdList[m_selectedIndex]);
			}
			return true;
		default:
			return false;
		}
	}

	public void RefreshUI()
	{
		if (base.IsHidden)
		{
			return;
		}
		if (m_trimSlot != null)
		{
			int count = m_trimIdList.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_trimSlot.Length > i)
				{
					m_trimSlot[i].SetData(m_trimIdList[i]);
				}
			}
		}
		if (m_selectedIndex >= 0 && m_trimIdList.Count > m_selectedIndex)
		{
			m_TrimMainInfo?.SetData(m_trimIdList[m_selectedIndex]);
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		RefreshUI();
		if (m_bShowIntervalTime)
		{
			string remainTimeStringExWithoutEnd = NKCUtilString.GetRemainTimeStringExWithoutEnd(NKCUITrimUtility.GetRemainDateMsg());
			NKCUtil.SetLabelText(m_lbRemainData, remainTimeStringExWithoutEnd);
		}
	}

	private void Update()
	{
		if (!m_bShowIntervalTime)
		{
			return;
		}
		if (m_dateUpdateTimerSec > 1f)
		{
			DateTime remainDateMsg = NKCUITrimUtility.GetRemainDateMsg();
			if (NKCSynchronizedTime.GetTimeLeft(remainDateMsg).TotalMinutes >= 1.0 && m_dateUpdateTimerMin < 60f)
			{
				m_dateUpdateTimerSec = 0f;
				return;
			}
			string remainTimeStringExWithoutEnd = NKCUtilString.GetRemainTimeStringExWithoutEnd(remainDateMsg);
			NKCUtil.SetLabelText(m_lbRemainData, remainTimeStringExWithoutEnd);
			m_dateUpdateTimerSec = 0f;
			m_dateUpdateTimerMin = 0f;
		}
		m_dateUpdateTimerSec += Time.deltaTime;
		m_dateUpdateTimerMin += Time.deltaTime;
	}

	private void SetToEntryState()
	{
		if (m_eTrimState == TrimState.Entry)
		{
			return;
		}
		if (m_trimSlot != null)
		{
			int num = m_trimSlot.Length;
			for (int i = 0; i < num; i++)
			{
				m_trimSlot[i]?.SetSlotState(NKCUITrimSlot.SlotState.Default);
			}
		}
		m_aniInfo.Play(m_aniInfo_02_to_01);
		m_eTrimState = TrimState.Entry;
		m_selectedIndex = -1;
	}

	private void SetToSelectedState(int slotIndex, int trimId)
	{
		SetTrimInfo(slotIndex, trimId);
		m_aniInfo.Play(m_aniInfo_01_to_02);
		m_eTrimState = TrimState.Selected;
	}

	private void SetTrimInfo(int slotIndex, int trimId)
	{
		m_TrimMainInfo?.SetData(trimId);
		if (m_trimSlot != null)
		{
			int num = m_trimSlot.Length;
			for (int i = 0; i < num; i++)
			{
				bool flag = slotIndex == i;
				m_trimSlot[i]?.SetSlotState(flag ? NKCUITrimSlot.SlotState.Selected : NKCUITrimSlot.SlotState.Disable);
			}
		}
	}

	private void SetTrimSelectState(int prevTrimId)
	{
		if (prevTrimId <= 0 || m_trimSlot == null)
		{
			return;
		}
		int num = m_trimSlot.Length;
		for (int i = 0; i < num; i++)
		{
			if (m_trimSlot[i].IsActive())
			{
				m_trimSlot[i].LetChangeClickState(prevTrimId);
			}
		}
		m_aniEnter.Play(m_aniEnterIdle);
		m_aniInfo.Play(m_aniInfo_02);
		if (m_eTrimState == TrimState.Selected)
		{
			m_TrimMainInfo?.OnClickEnter();
		}
	}

	private IEnumerator WaitingTrimInfoOpenSequece()
	{
		AnimatorStateInfo anistateInfo = m_aniInfo.GetCurrentAnimatorStateInfo(0);
		while (anistateInfo.IsName(m_aniInfo_01_to_02))
		{
			yield return null;
		}
		m_TrimMainInfo.OnClickEnter();
	}

	private void OnClickTrimSlot(int slotIndex, int trimId)
	{
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(trimId);
		if (nKMTrimTemplet == null)
		{
			SetToEntryState();
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			SetToEntryState();
			return;
		}
		if (!NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in nKMTrimTemplet.m_UnlockInfo))
		{
			string message = NKCContentManager.MakeUnlockConditionString(in nKMTrimTemplet.m_UnlockInfo, bSimple: false);
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(message, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			SetToEntryState();
			return;
		}
		m_bShowIntervalTime = nKMTrimTemplet.ShowInterval;
		NKCUtil.SetGameobjectActive(m_objRemainTime, m_bShowIntervalTime);
		m_selectedIndex = slotIndex;
		if (m_eTrimState == TrimState.Entry)
		{
			SetToSelectedState(slotIndex, trimId);
		}
		else if (m_eTrimState == TrimState.Selected)
		{
			if (m_trimSlot[slotIndex] != null && m_trimSlot[slotIndex].TrimSlotState == NKCUITrimSlot.SlotState.Selected)
			{
				SetToEntryState();
			}
			else
			{
				SetTrimInfo(slotIndex, trimId);
			}
		}
	}
}
