using System;
using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Game;
using Cs.Logging;
using NKC.UI.Gauntlet;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUITournamentPlayoff : MonoBehaviour
{
	public GameObject m_objNone;

	public NKCUITournamentRoundEntry m_tournamentEntry;

	public NKCUITournamentRoundFinal m_tournamentFinal;

	public Animator m_animator;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdư")]
	public NKCUIComStateButton m_csbtnCheerMode;

	public NKCUIComStateButton m_csbtnCheerDisabled;

	public NKCUIComStateButton m_csbtnCheerResult;

	public NKCUIComStateButton m_csbtnCheerStatistic;

	public GameObject m_objCheerTimeRoot;

	public TMP_Text m_lbCheerTimeLeft;

	public TMP_Text m_lbCheerDisabledText;

	public GameObject m_objCheerRedDot;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd")]
	public GameObject m_objCheerModeRoot;

	public GameObject m_objCheerModeFx;

	public GameObject m_objCheerState;

	public GameObject m_objCheerResult;

	public Text m_lbCheerSlotCount;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd Ƚ\ufffd\ufffd")]
	public GameObject m_objCheerCorrectRoot;

	public Text m_lbCheerCorrectCount;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdư")]
	public NKCUIComToggle[] m_tglGroup;

	public GameObject[] m_objTglCheerFx;

	private static NKCUIGauntletAsyncReady GauntletAsyncReady;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private NKMTournamentGroups m_currentGroup;

	private int m_currentGroupTglIdx;

	private static float m_nextCheerTime;

	public void Init()
	{
		m_tournamentEntry.Init(SetCheeringCount, null);
		m_tournamentFinal.Init(SetCheeringCount, null);
		Array values = Enum.GetValues(typeof(NKMTournamentGroups));
		_ = values.Length;
		int num = 0;
		foreach (int group in values)
		{
			if (group == 0)
			{
				continue;
			}
			if (num < m_tglGroup.Length)
			{
				int toggleIndex = num;
				NKCUtil.SetToggleValueChangedDelegate(m_tglGroup[num], delegate(bool value)
				{
					OnToggleGroup(value, toggleIndex, (NKMTournamentGroups)group);
				});
			}
			num++;
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnCheerMode, OnClickCheerMode);
		NKCUtil.SetButtonClickDelegate(m_csbtnCheerStatistic, OnClickCheerStatictic);
		NKCUtil.SetButtonClickDelegate(m_csbtnCheerResult, OnClickCheerResult);
		m_currentGroup = NKMTournamentGroups.None;
		if (m_animator == null)
		{
			m_animator = GetComponent<Animator>();
		}
		if (m_animator != null)
		{
			m_animator.keepAnimatorControllerStateOnDisable = true;
		}
		base.gameObject.SetActive(value: false);
	}

	public void Open(bool keepCheerState)
	{
		bool flag = !base.gameObject.activeSelf;
		base.gameObject.SetActive(value: true);
		NKMTournamentInfo tournamentInfo = NKCTournamentManager.GetTournamentInfo(NKMTournamentGroups.Finals);
		if (tournamentInfo == null)
		{
			tournamentInfo = NKCTournamentManager.GetTournamentInfo(NKMTournamentGroups.GlobalFinals);
		}
		bool flag2 = tournamentInfo != null && tournamentInfo.userInfo.Count > 0;
		NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(NKCTournamentManager.TournamentId);
		int num = m_tglGroup.Length - 1;
		m_tglGroup[num].SetLock(nKMTournamentTemplet == null || nKMTournamentTemplet.GetGroupCheeringStartTime(NKMTournamentGroups.Finals) > NKCSynchronizedTime.ServiceTime || !flag2);
		if (NKCTournamentManager.m_replayTournamentGroup != NKMTournamentGroups.None)
		{
			int num2 = (int)(NKCTournamentManager.m_replayTournamentGroup - 1) % 10;
			if (num2 >= 0 && num2 <= num)
			{
				m_tglGroup[num2].Select(bSelect: false, bForce: true);
				m_tglGroup[num2].Select(bSelect: true);
			}
			NKCTournamentManager.SetReplayTournamentGroup(NKMTournamentGroups.None);
			return;
		}
		bool flag3 = false;
		if (nKMTournamentTemplet != null)
		{
			flag3 = nKMTournamentTemplet.IsGroupCheeringTime(m_currentGroup);
		}
		if (keepCheerState && m_objCheerModeRoot.activeSelf && flag3)
		{
			return;
		}
		if (flag)
		{
			m_currentGroup = NKMTournamentGroups.None;
			if (!m_tglGroup[num].m_bLock)
			{
				m_tglGroup[num].Select(bSelect: false, bForce: true);
				m_tglGroup[num].Select(bSelect: true);
			}
			else
			{
				m_tglGroup[0].Select(bSelect: false, bForce: true);
				m_tglGroup[0].Select(bSelect: true);
			}
		}
		else
		{
			m_tglGroup[m_currentGroupTglIdx].Select(bSelect: false, bForce: true);
			m_tglGroup[m_currentGroupTglIdx].Select(bSelect: true);
		}
	}

	public void StartCheerCoolTime()
	{
		m_nextCheerTime = Time.time + (float)NKMCommonConst.TournamentPredictionCoolTime;
	}

	public static NKCUIGauntletAsyncReady GetInstanceGauntletAsyncReady()
	{
		if (GauntletAsyncReady == null)
		{
			m_loadedUIData = NKCUIManager.OpenNewInstance<NKCUIGauntletAsyncReady>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_ASYNC_READY", NKCUIManager.eUIBaseRect.UIFrontCommon, null);
			GauntletAsyncReady = m_loadedUIData.GetInstance<NKCUIGauntletAsyncReady>();
			if (GauntletAsyncReady != null)
			{
				GauntletAsyncReady.Init();
			}
		}
		return GauntletAsyncReady;
	}

	public bool IsCheerMode()
	{
		if (base.gameObject.activeSelf)
		{
			return m_csbtnCheerMode.IsSelected;
		}
		return false;
	}

	public bool IsCheerResuleMode()
	{
		if (base.gameObject.activeSelf && m_objCheerModeRoot.activeSelf)
		{
			return m_objCheerResult.activeSelf;
		}
		return false;
	}

	public void ToggleCheerMode(bool resetCheering)
	{
		m_csbtnCheerMode.Select(!m_csbtnCheerMode.IsSelected, bForce: true);
		NKCUtil.SetGameobjectActive(m_objCheerModeRoot, m_csbtnCheerMode.IsSelected);
		NKCUtil.SetGameobjectActive(m_objCheerModeFx, bValue: true);
		NKCUtil.SetGameobjectActive(m_objCheerState, bValue: true);
		NKCUtil.SetGameobjectActive(m_objCheerResult, bValue: false);
		NKCUITournamentRoundBase currentRoundUI = GetCurrentRoundUI();
		if (currentRoundUI != null)
		{
			currentRoundUI.IsCheerMode = m_csbtnCheerMode.IsSelected;
			if (currentRoundUI.IsCheerMode)
			{
				currentRoundUI.Refresh();
			}
			currentRoundUI.SetCheerEnable(m_csbtnCheerMode.IsSelected);
			if (resetCheering && !m_csbtnCheerMode.IsSelected)
			{
				currentRoundUI.CancelCheering();
			}
		}
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
		m_currentGroup = NKMTournamentGroups.None;
	}

	public void ReleaseData()
	{
		m_currentGroup = NKMTournamentGroups.None;
		if (m_animator != null)
		{
			m_animator.Rebind();
		}
		m_tournamentEntry?.ReleaseData();
		m_tournamentFinal?.ReleaseData();
		if (GauntletAsyncReady != null)
		{
			GauntletAsyncReady.Close();
			GauntletAsyncReady = null;
		}
		m_loadedUIData?.CloseInstance();
		m_loadedUIData = null;
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (m_nextCheerTime > 0f)
		{
			NKMTournamentTemplet templet = NKMTournamentTemplet.Find(NKCTournamentManager.TournamentId);
			SetCheerState(templet, m_currentGroup);
			if (m_nextCheerTime < Time.time && m_nextCheerTime > 0f)
			{
				m_nextCheerTime = 0f;
			}
		}
	}

	private IEnumerator ICheckIntroAniState()
	{
		while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		GetCurrentRoundUI().PlayWinnerFx();
	}

	private NKCUITournamentRoundBase GetCurrentRoundUI()
	{
		if (m_tournamentEntry.gameObject.activeSelf)
		{
			return m_tournamentEntry;
		}
		if (m_tournamentFinal.gameObject.activeSelf)
		{
			return m_tournamentFinal;
		}
		return null;
	}

	private void OnToggleGroup(bool value, int toggleIdx, NKMTournamentGroups group)
	{
		if (!value || !base.gameObject.activeSelf)
		{
			return;
		}
		if (NKCTournamentManager.m_TournamentTemplet.IsUnify)
		{
			group += 10;
		}
		bool flag = m_currentGroup == NKMTournamentGroups.None;
		bool refreshScroll = m_currentGroup != group;
		m_currentGroup = group;
		m_currentGroupTglIdx = toggleIdx;
		NKMTournamentState tournamentState = NKCTournamentManager.GetTournamentState();
		NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(NKCTournamentManager.TournamentId);
		Log.Debug($"TournamentState: {tournamentState}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Tournament/NKCUITournamentPlayoff.cs", 278);
		bool flag2 = false;
		switch (tournamentState)
		{
		case NKMTournamentState.Ended:
		case NKMTournamentState.Tryout:
			flag2 = false;
			break;
		case NKMTournamentState.Progressing:
			flag2 = nKMTournamentTemplet != null && nKMTournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Final32) < NKCSynchronizedTime.ServiceTime;
			break;
		case NKMTournamentState.Final32:
		case NKMTournamentState.Final4:
		case NKMTournamentState.Closing:
			flag2 = true;
			break;
		}
		if (!flag2)
		{
			m_tournamentEntry.Close();
			m_tournamentFinal.Close();
			NKCUtil.SetGameobjectActive(m_objNone, bValue: true);
			NKCUtil.SetGameobjectActive(m_objCheerTimeRoot, bValue: false);
			m_csbtnCheerStatistic.SetLock(value: true);
			NKCUtil.SetGameobjectActive(m_csbtnCheerMode, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnCheerDisabled, !m_csbtnCheerMode.gameObject.activeSelf);
			NKCUtil.SetGameobjectActive(m_csbtnCheerResult, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
		m_csbtnCheerStatistic.SetLock(value: false);
		bool flag3 = false;
		if (nKMTournamentTemplet != null)
		{
			bool bValue = nKMTournamentTemplet.IsGroupCheeringTime(NKMTournamentGroups.GroupA);
			NKCUtil.SetGameobjectActive(m_objTglCheerFx[0], bValue);
			bool bValue2 = nKMTournamentTemplet.IsGroupCheeringTime(NKMTournamentGroups.GroupB);
			NKCUtil.SetGameobjectActive(m_objTglCheerFx[1], bValue2);
			bool bValue3 = nKMTournamentTemplet.IsGroupCheeringTime(NKMTournamentGroups.GroupC);
			NKCUtil.SetGameobjectActive(m_objTglCheerFx[2], bValue3);
			bool bValue4 = nKMTournamentTemplet.IsGroupCheeringTime(NKMTournamentGroups.GroupD);
			NKCUtil.SetGameobjectActive(m_objTglCheerFx[3], bValue4);
			bool bValue5 = nKMTournamentTemplet.IsGroupCheeringTime(NKMTournamentGroups.Finals);
			NKCUtil.SetGameobjectActive(m_objTglCheerFx[4], bValue5);
			flag3 = nKMTournamentTemplet.IsGroupCheeringTime(m_currentGroup);
		}
		SetCheerState(nKMTournamentTemplet, group);
		m_csbtnCheerMode.Select(bSelect: false);
		m_csbtnCheerResult.Select(bSelect: false);
		GetCurrentRoundUI()?.SetCheerEnable(value: false);
		NKCUtil.SetGameobjectActive(m_objCheerModeRoot, bValue: false);
		bool flag4 = NKCTournamentManager.GetTournamentInfoPredict(group) == null;
		NKCUtil.SetGameobjectActive(m_objCheerRedDot, flag3 && flag4);
		if (group != NKMTournamentGroups.Finals && group != NKMTournamentGroups.GlobalFinals)
		{
			m_tournamentEntry.IsCheerMode = false;
			m_tournamentEntry.IsCheerResultState = false;
			m_tournamentEntry.Open(group, refreshScroll);
			m_tournamentFinal.Close();
		}
		else
		{
			m_tournamentFinal.IsCheerMode = false;
			m_tournamentFinal.IsCheerResultState = false;
			m_tournamentEntry.Close();
			m_tournamentFinal.Open(refreshScroll);
		}
		NKCUITournamentRoundBase currentRoundUI = GetCurrentRoundUI();
		if (!flag)
		{
			currentRoundUI?.SetTweenComplete();
		}
		if (currentRoundUI != null && currentRoundUI.HaveWinner() && !flag3)
		{
			NKCUtil.SetGameobjectActive(m_csbtnCheerMode, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnCheerDisabled, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnCheerResult, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnCheerResult, bValue: false);
		}
	}

	private void SetCheerState(NKMTournamentTemplet templet, NKMTournamentGroups group)
	{
		string text = "";
		bool flag = false;
		if (templet != null)
		{
			flag = templet.IsGroupCheeringTime(group);
			if (flag)
			{
				text = templet.GetGroupCheeringRemainTimeString(group);
				NKCUtil.SetLabelText(m_lbCheerTimeLeft, string.Format(NKCUtilString.GET_STRING_REMAIN_TIME_LEFT_ONE_PARAM, text));
			}
		}
		NKCUtil.SetGameobjectActive(m_objCheerTimeRoot, flag);
		if (!flag || m_nextCheerTime < Time.time)
		{
			NKCUtil.SetLabelText(m_lbCheerDisabledText, NKCStringTable.GetString("SI_PF_TOURNAMENT_GROUP_BUTTON_PREDICT"));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbCheerDisabledText, $"00:{m_nextCheerTime - Time.time:00}");
		}
		NKCUtil.SetGameobjectActive(m_csbtnCheerMode, flag && m_nextCheerTime < Time.time);
		NKCUtil.SetGameobjectActive(m_csbtnCheerDisabled, !m_csbtnCheerMode.gameObject.activeSelf && !m_csbtnCheerResult.gameObject.activeSelf);
	}

	private void SetCheeringCount()
	{
		int cheerSlotCount = 0;
		int notCheeringCount = 0;
		NKCUITournamentRoundBase currentRoundUI = GetCurrentRoundUI();
		if (currentRoundUI != null)
		{
			currentRoundUI.IsCheeringCompleted(out var _, out cheerSlotCount, out notCheeringCount);
		}
		NKCUtil.SetLabelText(m_lbCheerSlotCount, $"{cheerSlotCount - notCheeringCount}/{cheerSlotCount}");
	}

	private void SetCheeringCorrectCount()
	{
		NKCUITournamentRoundBase currentRoundUI = GetCurrentRoundUI();
		NKCUtil.SetGameobjectActive(m_lbCheerCorrectCount, currentRoundUI != null);
		NKCUtil.SetGameobjectActive(m_objCheerCorrectRoot, currentRoundUI != null && currentRoundUI.IsAllResultShown(m_currentGroup));
		if (currentRoundUI != null)
		{
			NKCUtil.SetLabelText(m_lbCheerCorrectCount, string.Format(NKCUtilString.GET_STRING_TOURNAMENT_CHEERING_CORRECT_COUNT, currentRoundUI.GetCheerCorrectCount()));
		}
	}

	private void OnClickCheerMode()
	{
		bool flag = false;
		int cheerSlotCount = 0;
		int notCheeringCount = 0;
		List<int> notCheeredSlotIndex = null;
		NKCUITournamentRoundBase currentRoundUI = GetCurrentRoundUI();
		if (m_csbtnCheerMode.IsSelected)
		{
			if (currentRoundUI != null)
			{
				flag = currentRoundUI.IsCheeringCompleted(out notCheeredSlotIndex, out cheerSlotCount, out notCheeringCount);
			}
			if (!flag)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TOURNAMENT_CHEERING_NOT_COMPLETED);
				return;
			}
		}
		ToggleCheerMode(resetCheering: false);
		if (!m_csbtnCheerMode.IsSelected)
		{
			List<long> slotUserUId = currentRoundUI?.GetCheeringUIdList();
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_PREDICTION_REQ(NKCTournamentManager.TournamentId, NKCTournamentManager.GetOriginalGroup(m_currentGroup), slotUserUId);
			return;
		}
		if (currentRoundUI != null)
		{
			flag = currentRoundUI.IsCheeringCompleted(out notCheeredSlotIndex, out cheerSlotCount, out notCheeringCount);
		}
		NKCUtil.SetLabelText(m_lbCheerSlotCount, $"{cheerSlotCount - notCheeringCount}/{cheerSlotCount}");
	}

	private void OnClickCheerStatictic()
	{
		NKCPacketSender.Send_NKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ(NKCTournamentManager.TournamentId, NKCTournamentManager.GetOriginalGroup(m_currentGroup));
	}

	private void OnClickCheerResult()
	{
		NKCUITournamentRoundBase currentRoundUI = GetCurrentRoundUI();
		if (!(currentRoundUI == null))
		{
			m_csbtnCheerResult.Select(!m_csbtnCheerResult.IsSelected, bForce: true);
			currentRoundUI.IsCheerResultState = m_csbtnCheerResult.IsSelected;
			NKCUtil.SetGameobjectActive(m_objCheerModeRoot, m_csbtnCheerResult.IsSelected);
			NKCUtil.SetGameobjectActive(m_objCheerModeFx, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCheerState, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCheerResult, bValue: true);
			if (m_csbtnCheerResult.IsSelected)
			{
				SetCheeringCorrectCount();
			}
			currentRoundUI.Refresh();
		}
	}
}
