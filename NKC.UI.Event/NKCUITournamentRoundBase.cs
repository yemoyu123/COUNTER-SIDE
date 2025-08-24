using System;
using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Game;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public abstract class NKCUITournamentRoundBase : MonoBehaviour
{
	protected class ResultShowState
	{
		public bool m_showEntryTopResult;

		public bool m_showEntryMiddleResult;

		public bool m_showEntryBottomResult;
	}

	public delegate void OnClickPlayerSlot();

	public delegate void OnClickShowResult();

	public ScrollRect m_ScrollRect;

	public GameObject m_objWinnerFx;

	public Text m_cheerTime;

	public Text m_matchStartTime;

	public Image m_GroupName;

	[Header("\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffdư")]
	public NKCUIComStateButton m_csbtnResultTop;

	public NKCUIComStateButton m_csbtnResultMiddle;

	public NKCUIComStateButton m_csbtnResultBottom;

	[Header("\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffdư")]
	public string winnerSound;

	public string cheerSuccessSound;

	public string cheerFailSound;

	protected NKCUITournamentBinaryTree m_BTree = new NKCUITournamentBinaryTree();

	protected float m_initialScrollPosition = -1f;

	protected NKMTournamentGroups m_UIGroup;

	protected Dictionary<NKMTournamentGroups, ResultShowState> m_showResultState = new Dictionary<NKMTournamentGroups, ResultShowState>();

	private bool tweenForceEnd = true;

	protected OnClickPlayerSlot m_dOnClickPlayerSlot;

	protected OnClickShowResult m_dOnClickShowResult;

	public bool IsCheerResultState { get; set; }

	public bool IsCheerMode { get; set; }

	public virtual void Init(OnClickPlayerSlot dOnClickPlayerSlot, OnClickShowResult dOnClickShowResult)
	{
		m_dOnClickPlayerSlot = dOnClickPlayerSlot;
		m_dOnClickShowResult = dOnClickShowResult;
	}

	public virtual bool IsCheeringCompleted(out List<int> notCheeredSlotIndex, out int cheerSlotCount, out int notCheeringCount)
	{
		return m_BTree.IsCheeringCompleted(out notCheeredSlotIndex, out cheerSlotCount, out notCheeringCount);
	}

	public abstract void PlayWinnerFx();

	public void Close()
	{
		ReleaseData();
		base.gameObject.SetActive(value: false);
	}

	public void ReleaseData()
	{
		m_BTree.ReleaseData();
	}

	public void SetCheerEnable(bool value)
	{
		m_BTree.SetCheerEnable(value);
	}

	public virtual List<long> GetCheeringUIdList()
	{
		return m_BTree.GetCheeringUIdList();
	}

	public void SetTweenComplete()
	{
		if (!tweenForceEnd)
		{
			return;
		}
		DOTweenAnimation[] componentsInChildren = m_ScrollRect.content.GetComponentsInChildren<DOTweenAnimation>();
		tweenForceEnd = false;
		if (componentsInChildren == null)
		{
			return;
		}
		DOTweenAnimation[] array = componentsInChildren;
		foreach (DOTweenAnimation dOTweenAnimation in array)
		{
			if (dOTweenAnimation.animationType == DOTweenAnimation.AnimationType.Scale)
			{
				dOTweenAnimation.DOComplete();
			}
		}
	}

	public abstract void CancelCheering();

	public abstract void Refresh();

	public abstract bool HaveWinner();

	public virtual int GetCheerCorrectCount()
	{
		int num = 0;
		foreach (NKCUITournamentPlayerSlotTree item in m_BTree.TournamentTree)
		{
			if (item.IsBlankTree())
			{
				num++;
				continue;
			}
			if (item.m_playerA.IsCheerSuccess())
			{
				num++;
			}
			if (item.m_playerB.IsCheerSuccess())
			{
				num++;
			}
		}
		return num;
	}

	protected NKMTournamentProfileData GetTournemantProfileData(NKMTournamentInfo tournamentInfo, int index)
	{
		if (tournamentInfo == null || index < 0 || tournamentInfo.slotUserUid.Count <= index)
		{
			return null;
		}
		NKMTournamentProfileData value = null;
		tournamentInfo.userInfo.TryGetValue(tournamentInfo.slotUserUid[index], out value);
		return value;
	}

	protected NKCUITournamentPlayerSlotTree.ProfileDataSet GetProfildDataSet(NKMTournamentInfo tournamentInfo, NKMTournamentInfo tournamentInfoPredict, int slotIndex)
	{
		NKCUITournamentPlayerSlotTree.ProfileDataSet result = default(NKCUITournamentPlayerSlotTree.ProfileDataSet);
		result.profildData = GetTournemantProfileData(tournamentInfo, slotIndex);
		result.profildDataPredict = GetTournemantProfileData(tournamentInfoPredict, slotIndex);
		result.slotIndex = slotIndex;
		result.group = tournamentInfo?.groupIndex ?? NKMTournamentGroups.None;
		return result;
	}

	protected void SetCheerTimeText(NKMTournamentGroups group)
	{
		NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(NKCTournamentManager.TournamentId);
		string msg = "";
		string msg2 = "";
		if (nKMTournamentTemplet != null)
		{
			DateTime groupCheeringStartTime = nKMTournamentTemplet.GetGroupCheeringStartTime(group);
			DateTime groupCheeringEndTime = nKMTournamentTemplet.GetGroupCheeringEndTime(group);
			msg = string.Format("{0} ~ {1}", groupCheeringStartTime.ToString("yyyy-MM-dd HH:mm"), groupCheeringEndTime.ToString("yyyy-MM-dd HH:mm"));
			msg2 = groupCheeringEndTime.ToString("yyyy-MM-dd HH:mm");
		}
		NKCUtil.SetLabelText(m_cheerTime, msg);
		NKCUtil.SetLabelText(m_matchStartTime, msg2);
	}

	protected void InitPlayerPrefResultShowState(NKMTournamentGroups group)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			int tournamentId = NKCTournamentManager.TournamentId;
			string key = $"TOURNAMENT_TOP_RESULT_{tournamentId}_{nKMUserData.m_UserUID}_{group.ToString()}";
			string key2 = $"TOURNAMENT_MIDDLE_RESULT_{tournamentId}_{nKMUserData.m_UserUID}_{group.ToString()}";
			string key3 = $"TOURNAMENT_BOTTOM_RESULT_{tournamentId}_{nKMUserData.m_UserUID}_{group.ToString()}";
			if (PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.DeleteKey(key);
			}
			if (PlayerPrefs.HasKey(key2))
			{
				PlayerPrefs.DeleteKey(key2);
			}
			if (PlayerPrefs.HasKey(key3))
			{
				PlayerPrefs.DeleteKey(key3);
			}
		}
	}

	public bool IsAllResultShown(NKMTournamentGroups group)
	{
		if (m_showResultState[group].m_showEntryTopResult && m_showResultState[group].m_showEntryMiddleResult)
		{
			return m_showResultState[group].m_showEntryBottomResult;
		}
		return false;
	}

	protected void SetResultShowState(NKMTournamentGroups group)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			int tournamentId = NKCTournamentManager.TournamentId;
			string key = $"TOURNAMENT_TOP_RESULT_{tournamentId}_{nKMUserData.m_UserUID}_{group.ToString()}";
			string key2 = $"TOURNAMENT_MIDDLE_RESULT_{tournamentId}_{nKMUserData.m_UserUID}_{group.ToString()}";
			string key3 = $"TOURNAMENT_BOTTOM_RESULT_{tournamentId}_{nKMUserData.m_UserUID}_{group.ToString()}";
			m_showResultState[group].m_showEntryTopResult = PlayerPrefs.GetInt(key, 0) != 0;
			m_showResultState[group].m_showEntryMiddleResult = PlayerPrefs.GetInt(key2, 0) != 0;
			m_showResultState[group].m_showEntryBottomResult = PlayerPrefs.GetInt(key3, 0) != 0;
		}
	}

	protected void SetPlayerPrefTopResultShowState(NKMTournamentGroups group)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			int tournamentId = NKCTournamentManager.TournamentId;
			PlayerPrefs.SetInt($"TOURNAMENT_TOP_RESULT_{tournamentId}_{nKMUserData.m_UserUID}_{group.ToString()}", 1);
		}
	}

	protected void SetPlayerPrefMiddleResultShowState(NKMTournamentGroups group)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			int tournamentId = NKCTournamentManager.TournamentId;
			PlayerPrefs.SetInt($"TOURNAMENT_MIDDLE_RESULT_{tournamentId}_{nKMUserData.m_UserUID}_{group.ToString()}", 1);
		}
	}

	protected void SetPlayerPrefBottomResultShowState(NKMTournamentGroups group)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			int tournamentId = NKCTournamentManager.TournamentId;
			PlayerPrefs.SetInt($"TOURNAMENT_BOTTOM_RESULT_{tournamentId}_{nKMUserData.m_UserUID}_{group.ToString()}", 1);
		}
	}

	protected IEnumerator ISetScrollPosition(bool refresh, bool refreshScroll)
	{
		yield return null;
		if (m_initialScrollPosition < 0f)
		{
			m_initialScrollPosition = Mathf.Max(0f, m_ScrollRect.verticalNormalizedPosition);
		}
		if (!refresh || refreshScroll)
		{
			m_ScrollRect.verticalNormalizedPosition = (m_showResultState[m_UIGroup].m_showEntryTopResult ? 1f : m_initialScrollPosition);
		}
	}

	private void OnDestroy()
	{
		m_BTree?.ClearTree();
		m_BTree = null;
		m_showResultState?.Clear();
		m_showResultState = null;
		m_dOnClickPlayerSlot = null;
	}
}
