using ClientPacket.Common;
using ClientPacket.Game;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUITournamentRoundEntry : NKCUITournamentRoundBase
{
	public NKCUITournamentPlayerSlotTree m_PlayerSlotTreeA;

	public NKCUITournamentPlayerSlotTree m_PlayerSlotTreeB;

	public NKCUITournamentPlayerSlotTree m_PlayerSlotTreeC;

	public NKCUITournamentPlayerSlotTree m_PlayerSlotTreeD;

	public NKCUITournamentPlayerSlotTree m_PlayerSlotTreeAB;

	public NKCUITournamentPlayerSlotTree m_PlayerSlotTreeCD;

	public NKCUITournamentPlayerSlotTree m_PlayerSlotTreeFinal;

	public NKCUITournamentPlayerSlot m_PlayerSlotWinner;

	private const int Winner = 0;

	private const int FinalR = 2;

	private const int QuarterFinalL_A = 3;

	private const int EntryL_U_A = 7;

	private const int AllSlotCount = 15;

	public override void Init(OnClickPlayerSlot dOnClickPlayerSlot, OnClickShowResult dOnClickShowResult)
	{
		base.Init(dOnClickPlayerSlot, dOnClickShowResult);
		m_PlayerSlotTreeA.Init(OnClickSlot, isFinalGroup: false);
		m_PlayerSlotTreeB.Init(OnClickSlot, isFinalGroup: false);
		m_PlayerSlotTreeC.Init(OnClickSlot, isFinalGroup: false);
		m_PlayerSlotTreeD.Init(OnClickSlot, isFinalGroup: false);
		m_PlayerSlotTreeAB.Init(OnClickSlot, isFinalGroup: false);
		m_PlayerSlotTreeCD.Init(OnClickSlot, isFinalGroup: false);
		m_PlayerSlotTreeFinal.Init(OnClickSlot, isFinalGroup: false);
		m_PlayerSlotWinner?.Init(OnClickSlot);
		m_BTree.AddNode(m_PlayerSlotTreeFinal);
		m_BTree.AddNode(m_PlayerSlotTreeAB);
		m_BTree.AddNode(m_PlayerSlotTreeCD);
		m_BTree.AddNode(m_PlayerSlotTreeA);
		m_BTree.AddNode(m_PlayerSlotTreeB);
		m_BTree.AddNode(m_PlayerSlotTreeC);
		m_BTree.AddNode(m_PlayerSlotTreeD);
		m_BTree.CreateSlotTree(m_PlayerSlotWinner);
		m_BTree.SetSlotLink();
		NKCUtil.SetButtonClickDelegate(m_csbtnResultTop, OnClickResultTop);
		NKCUtil.SetButtonClickDelegate(m_csbtnResultMiddle, OnClickResultMiddle);
		NKCUtil.SetButtonClickDelegate(m_csbtnResultBottom, OnClickResultBottom);
		m_showResultState.Add(NKMTournamentGroups.GroupA, new ResultShowState());
		m_showResultState.Add(NKMTournamentGroups.GroupB, new ResultShowState());
		m_showResultState.Add(NKMTournamentGroups.GroupC, new ResultShowState());
		m_showResultState.Add(NKMTournamentGroups.GroupD, new ResultShowState());
		m_showResultState.Add(NKMTournamentGroups.GlobalGroupA, new ResultShowState());
		m_showResultState.Add(NKMTournamentGroups.GlobalGroupB, new ResultShowState());
		m_showResultState.Add(NKMTournamentGroups.GlobalGroupC, new ResultShowState());
		m_showResultState.Add(NKMTournamentGroups.GlobalGroupD, new ResultShowState());
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMTournamentGroups group, bool refreshScroll = false)
	{
		m_UIGroup = ChangeToUIGroup(group);
		NKMTournamentInfo tournamentInfo = NKCTournamentManager.GetTournamentInfo(group);
		NKMTournamentInfo tournamentInfoPredict = NKCTournamentManager.GetTournamentInfoPredict(group);
		if (tournamentInfo != null)
		{
			bool flag = true;
			foreach (long item in tournamentInfo.slotUserUid)
			{
				if (item > 0)
				{
					flag = false;
					if (!tournamentInfo.userInfo.ContainsKey(item))
					{
						Log.Error($"UserUId: {item} ProfileData not exist in NKMTournamentInfo in {group}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Tournament/NKCUITournamentRoundEntry.cs", 94);
					}
				}
			}
			if (flag)
			{
				Log.Error($"SlotUserUId is all 0 in NKMTournamentInfo in {group}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Tournament/NKCUITournamentRoundEntry.cs", 99);
			}
		}
		bool activeSelf = base.gameObject.activeSelf;
		base.gameObject.SetActive(value: true);
		if (GetProfildDataSet(tournamentInfo, tournamentInfoPredict, 0).profildData == null)
		{
			InitPlayerPrefResultShowState(group);
		}
		SetResultShowState(group);
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(ISetScrollPosition(activeSelf, refreshScroll));
		}
		NKMAssetName nKMAssetName = null;
		switch (group)
		{
		case NKMTournamentGroups.GroupA:
		case NKMTournamentGroups.GlobalGroupA:
			nKMAssetName = new NKMAssetName("ui_single_tournament_sprite", "UI_SINGLE_TOURNAMENT_GROUP_A");
			break;
		case NKMTournamentGroups.GroupB:
		case NKMTournamentGroups.GlobalGroupB:
			nKMAssetName = new NKMAssetName("ui_single_tournament_sprite", "UI_SINGLE_TOURNAMENT_GROUP_B");
			break;
		case NKMTournamentGroups.GroupC:
		case NKMTournamentGroups.GlobalGroupC:
			nKMAssetName = new NKMAssetName("ui_single_tournament_sprite", "UI_SINGLE_TOURNAMENT_GROUP_C");
			break;
		case NKMTournamentGroups.GroupD:
		case NKMTournamentGroups.GlobalGroupD:
			nKMAssetName = new NKMAssetName("ui_single_tournament_sprite", "UI_SINGLE_TOURNAMENT_GROUP_D");
			break;
		default:
			nKMAssetName = new NKMAssetName();
			break;
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(nKMAssetName);
		NKCUtil.SetImageSprite(m_GroupName, orLoadAssetResource);
		NKCUtil.SetGameobjectActive(m_objWinnerFx, bValue: false);
		_ = NKMTournamentTemplet.Find(NKCTournamentManager.TournamentId)?.IsGroupCheeringTime(group) ?? false;
		bool flag2 = false;
		int count = m_BTree.PlayerSlotTree.Count;
		for (int i = 0; i < count; i++)
		{
			NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet = GetProfildDataSet(tournamentInfo, tournamentInfoPredict, i);
			bool flag3 = false;
			bool leafPlayer = false;
			if (i == 0)
			{
				flag3 = m_showResultState[m_UIGroup].m_showEntryTopResult;
			}
			else if (i < 3)
			{
				flag3 = m_showResultState[m_UIGroup].m_showEntryTopResult || m_showResultState[m_UIGroup].m_showEntryMiddleResult;
			}
			else if (i > 2 && i < 7)
			{
				flag3 = m_showResultState[m_UIGroup].m_showEntryMiddleResult || m_showResultState[m_UIGroup].m_showEntryBottomResult;
			}
			else
			{
				flag3 = m_showResultState[m_UIGroup].m_showEntryBottomResult;
				leafPlayer = true;
			}
			if (base.IsCheerResultState)
			{
				flag3 = false;
			}
			m_BTree.PlayerSlotTree[i]?.SetData(profildDataSet.slotIndex, profildDataSet.profildData, profildDataSet.profildDataPredict, profildDataSet.group, flag3, leafPlayer);
			if (base.IsCheerResultState && profildDataSet.profildDataPredict == null)
			{
				m_BTree.PlayerSlotTree[i]?.SetAsBlankSlot();
			}
			if (!base.IsCheerMode && !base.IsCheerResultState)
			{
				if (i < 3)
				{
					if (!flag3)
					{
						m_BTree.PlayerSlotTree[i]?.SetAsBlankSlot();
					}
					else if (!m_showResultState[m_UIGroup].m_showEntryTopResult)
					{
						m_BTree.PlayerSlotTree[i].HideResultIcon();
					}
				}
				else if (i > 2 && i < 7)
				{
					if (!flag3)
					{
						m_BTree.PlayerSlotTree[i]?.SetAsBlankSlot();
					}
					else if (!m_showResultState[m_UIGroup].m_showEntryMiddleResult)
					{
						m_BTree.PlayerSlotTree[i]?.HideResultIcon();
					}
				}
			}
			if (i == 0)
			{
				flag2 = profildDataSet.profildData != null;
			}
		}
		m_BTree.SetDetailButtonActive();
		SetCheerTimeText(group);
		NKCUtil.SetGameobjectActive(m_csbtnResultTop, flag2 && m_showResultState[m_UIGroup].m_showEntryMiddleResult && !base.IsCheerResultState);
		NKCUtil.SetGameobjectActive(m_csbtnResultMiddle, flag2 && m_showResultState[m_UIGroup].m_showEntryBottomResult && !base.IsCheerResultState);
		NKCUtil.SetGameobjectActive(m_csbtnResultBottom, flag2 && !base.IsCheerResultState);
		m_csbtnResultTop?.SetLock(m_showResultState[m_UIGroup].m_showEntryTopResult);
		m_csbtnResultMiddle?.SetLock(m_showResultState[m_UIGroup].m_showEntryMiddleResult);
		m_csbtnResultBottom?.SetLock(m_showResultState[m_UIGroup].m_showEntryBottomResult);
	}

	public override void PlayWinnerFx()
	{
		NKCUtil.SetGameobjectActive(m_objWinnerFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objWinnerFx, m_PlayerSlotWinner.ProfileData != null && m_showResultState[m_UIGroup].m_showEntryTopResult);
	}

	public override void CancelCheering()
	{
		Open(m_UIGroup);
	}

	public override void Refresh()
	{
		Open(m_UIGroup);
	}

	public override bool HaveWinner()
	{
		return m_PlayerSlotWinner.ProfileData != null;
	}

	private void OnClickSlot(int slotIndex, long userUId)
	{
		m_BTree.GetSlotTree(slotIndex)?.SetCheeringByUserUId(userUId);
		SetCheerEnable(value: true);
		m_BTree.SetDetailButtonActive();
		if (m_dOnClickPlayerSlot != null)
		{
			m_dOnClickPlayerSlot();
		}
	}

	private void OnClickResultTop()
	{
		m_showResultState[m_UIGroup].m_showEntryTopResult = true;
		SetPlayerPrefTopResultShowState(m_UIGroup);
		Open(m_UIGroup);
		PlayWinnerFx();
		NKCSoundManager.PlaySound(winnerSound, 1f, 0f, 0f);
		if (m_dOnClickShowResult != null)
		{
			m_dOnClickShowResult();
		}
	}

	private void OnClickResultMiddle()
	{
		m_showResultState[m_UIGroup].m_showEntryMiddleResult = true;
		SetPlayerPrefMiddleResultShowState(m_UIGroup);
		Open(m_UIGroup);
		bool flag = false;
		for (int i = 3; i < 7; i++)
		{
			NKCUITournamentPlayerSlot playerSlot = m_BTree.GetPlayerSlot(i);
			if (playerSlot != null && playerSlot.IsCheerSuccess())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			NKCSoundManager.PlaySound(cheerSuccessSound, 1f, 0f, 0f);
		}
		else
		{
			NKCSoundManager.PlaySound(cheerFailSound, 1f, 0f, 0f);
		}
		if (m_dOnClickShowResult != null)
		{
			m_dOnClickShowResult();
		}
	}

	private void OnClickResultBottom()
	{
		m_showResultState[m_UIGroup].m_showEntryBottomResult = true;
		SetPlayerPrefBottomResultShowState(m_UIGroup);
		Open(m_UIGroup);
		bool flag = false;
		for (int i = 7; i < 15; i++)
		{
			NKCUITournamentPlayerSlot playerSlot = m_BTree.GetPlayerSlot(i);
			if (playerSlot != null && playerSlot.IsCheerSuccess())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			NKCSoundManager.PlaySound(cheerSuccessSound, 1f, 0f, 0f);
		}
		else
		{
			NKCSoundManager.PlaySound(cheerFailSound, 1f, 0f, 0f);
		}
		if (m_dOnClickShowResult != null)
		{
			m_dOnClickShowResult();
		}
	}

	private NKMTournamentGroups ChangeToUIGroup(NKMTournamentGroups group)
	{
		if (group <= NKMTournamentGroups.Finals)
		{
			return group;
		}
		return (NKMTournamentGroups)((int)group % 10);
	}
}
