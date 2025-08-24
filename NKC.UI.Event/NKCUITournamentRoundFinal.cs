using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Game;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUITournamentRoundFinal : NKCUITournamentRoundBase
{
	public enum TournamentInfoIndex
	{
		Champion,
		Final_A,
		Final_B,
		SemiFinalL_A,
		SemiFinalL_B,
		SemiFinalR_A,
		SemiFinalR_B,
		ThirdMatch_A,
		ThirdMatch_B
	}

	public NKCUITournamentPlayerSlotTree m_playerSlotTreeA;

	public NKCUITournamentPlayerSlotTree m_playerSlotTreeB;

	public NKCUITournamentPlayerSlotTree m_playerSlotTreeFinal;

	public NKCUITournamentPlayerSlotTree m_playerSlotTree3rdPlace;

	public NKCUITournamentPlayerSlot m_playerSlotWinner;

	public NKCUITournamentPlayerSlot m_playerSlotPref;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objLeaderboard;

	public NKCUITournamentRankSlot m_RankSlot;

	private NKCUITournamentPlayerSlot m_playerSlot3rdWinner;

	public override void Init(OnClickPlayerSlot dOnClickPlayerSlot, OnClickShowResult dOnClickShowResult)
	{
		base.Init(dOnClickPlayerSlot, dOnClickShowResult);
		m_playerSlotTreeA.Init(OnClickSlot, isFinalGroup: true);
		m_playerSlotTreeB.Init(OnClickSlot, isFinalGroup: true);
		m_playerSlotTreeFinal.Init(OnClickSlot, isFinalGroup: true);
		m_playerSlotTree3rdPlace.Init(OnClickSlot, isFinalGroup: true);
		m_playerSlotWinner?.Init(OnClickSlot);
		m_BTree.AddNode(m_playerSlotTreeFinal);
		m_BTree.AddNode(m_playerSlotTreeA);
		m_BTree.AddNode(m_playerSlotTreeB);
		m_playerSlotTree3rdPlace.TreeUnderA = m_playerSlotTreeA;
		m_playerSlotTree3rdPlace.TreeUnderB = m_playerSlotTreeB;
		m_BTree.InsertToTournamentTree(-1, m_playerSlotTree3rdPlace);
		m_BTree.CreateSlotTree(m_playerSlotWinner);
		m_BTree.SetSlotLink();
		m_playerSlot3rdWinner = Object.Instantiate(m_playerSlotPref, base.transform);
		if (m_playerSlot3rdWinner != null)
		{
			m_playerSlot3rdWinner.SetPlayerSlotUnder(m_playerSlotTree3rdPlace.m_playerA, m_playerSlotTree3rdPlace.m_playerB);
			m_playerSlot3rdWinner.m_titlePanel = null;
			m_playerSlot3rdWinner.gameObject.SetActive(value: false);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnResultTop, OnClickResultTop);
		NKCUtil.SetButtonClickDelegate(m_csbtnResultMiddle, OnClickResultMiddle);
		NKCUtil.SetButtonClickDelegate(m_csbtnResultBottom, OnClickResultBottom);
		m_showResultState.Add(NKMTournamentGroups.Finals, new ResultShowState());
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(new NKMAssetName("ui_single_tournament_sprite", "UI_SINGLE_TOURNAMENT_FINALS"));
		NKCUtil.SetImageSprite(m_GroupName, orLoadAssetResource);
		base.gameObject.SetActive(value: false);
	}

	public void Open(bool refreshScroll = false)
	{
		m_UIGroup = NKMTournamentGroups.Finals;
		NKMTournamentInfo tournamentInfo = NKCTournamentManager.GetTournamentInfo(m_UIGroup);
		NKMTournamentInfo tournamentInfoPredict = NKCTournamentManager.GetTournamentInfoPredict(m_UIGroup);
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
						Log.Error($"UserUId: {item} ProfileData not exist in NKMTournamentInfo in {m_UIGroup}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Tournament/NKCUITournamentRoundFinal.cs", 108);
					}
				}
			}
			if (flag)
			{
				Log.Error($"SlotUserUId is all 0 in NKMTournamentInfo in {m_UIGroup}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Tournament/NKCUITournamentRoundFinal.cs", 113);
			}
		}
		bool activeSelf = base.gameObject.activeSelf;
		base.gameObject.SetActive(value: true);
		NKCUtil.SetGameobjectActive(m_objWinnerFx, bValue: false);
		NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(NKCTournamentManager.TournamentId);
		_ = nKMTournamentTemplet?.IsGroupCheeringTime(m_UIGroup) ?? false;
		NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet = GetProfildDataSet(tournamentInfo, tournamentInfoPredict, 0);
		if (profildDataSet.profildData == null)
		{
			InitPlayerPrefResultShowState(m_UIGroup);
		}
		SetResultShowState(m_UIGroup);
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(ISetScrollPosition(activeSelf, refreshScroll));
		}
		bool flag2 = !base.IsCheerResultState && m_showResultState[m_UIGroup].m_showEntryTopResult;
		m_playerSlotWinner?.SetData(profildDataSet.slotIndex, profildDataSet.profildData, profildDataSet.profildDataPredict, profildDataSet.group, flag2, leafPlayer: false);
		if (base.IsCheerResultState && profildDataSet.profildDataPredict == null)
		{
			m_playerSlotWinner?.SetAsBlankSlot();
		}
		if (!base.IsCheerMode && !base.IsCheerResultState && !flag2)
		{
			m_playerSlotWinner?.SetAsBlankSlot();
		}
		NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet2 = GetProfildDataSet(tournamentInfo, tournamentInfoPredict, 1);
		NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet3 = GetProfildDataSet(tournamentInfo, tournamentInfoPredict, 2);
		flag2 = !base.IsCheerResultState && (m_showResultState[m_UIGroup].m_showEntryTopResult || m_showResultState[m_UIGroup].m_showEntryMiddleResult);
		m_playerSlotTreeFinal.SetProfileDataA(profildDataSet2, flag2, leafPlayer: false, base.IsCheerMode, base.IsCheerResultState, m_showResultState[m_UIGroup].m_showEntryTopResult);
		m_playerSlotTreeFinal.SetProfileDataB(profildDataSet3, flag2, leafPlayer: false, base.IsCheerMode, base.IsCheerResultState, m_showResultState[m_UIGroup].m_showEntryTopResult);
		NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet4 = GetProfildDataSet(tournamentInfo, tournamentInfoPredict, 3);
		NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet5 = GetProfildDataSet(tournamentInfo, tournamentInfoPredict, 4);
		flag2 = !base.IsCheerResultState && m_showResultState[m_UIGroup].m_showEntryMiddleResult;
		m_playerSlotTreeA.SetProfileDataA(profildDataSet4, flag2, leafPlayer: true, base.IsCheerResultState);
		m_playerSlotTreeA.SetProfileDataB(profildDataSet5, flag2, leafPlayer: true, base.IsCheerResultState);
		NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet6 = GetProfildDataSet(tournamentInfo, tournamentInfoPredict, 5);
		NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet7 = GetProfildDataSet(tournamentInfo, tournamentInfoPredict, 6);
		m_playerSlotTreeB.SetProfileDataA(profildDataSet6, flag2, leafPlayer: true, base.IsCheerResultState);
		m_playerSlotTreeB.SetProfileDataB(profildDataSet7, flag2, leafPlayer: true, base.IsCheerResultState);
		NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet8 = GetProfildDataSet(tournamentInfo, tournamentInfoPredict, 7);
		m_playerSlot3rdWinner?.SetDataEx(profildDataSet8.profildData, profildDataSet8.profildDataPredict);
		NKCUITournamentPlayerSlot playerSlot = m_BTree.GetPlayerSlot(7);
		NKCUITournamentPlayerSlotTree.ProfileDataSet profileDataSet = default(NKCUITournamentPlayerSlotTree.ProfileDataSet);
		profileDataSet.slotIndex = 7;
		profileDataSet.group = NKCTournamentManager.GetOriginalGroup(NKMTournamentGroups.Finals);
		profileDataSet.profildData = null;
		profileDataSet.profildDataPredict = null;
		if (profildDataSet2.profildData != null)
		{
			if (profildDataSet4.profildData != null && profildDataSet4.profildData.commonProfile.userUid == profildDataSet2.profildData.commonProfile.userUid)
			{
				profileDataSet.profildData = profildDataSet5.profildData;
			}
			else if (profildDataSet5.profildData != null && profildDataSet5.profildData.commonProfile.userUid == profildDataSet2.profildData.commonProfile.userUid)
			{
				profileDataSet.profildData = profildDataSet4.profildData;
			}
		}
		if (profildDataSet2.profildDataPredict != null)
		{
			if (profildDataSet4.profildData != null && profildDataSet4.profildData.commonProfile.userUid == profildDataSet2.profildDataPredict.commonProfile.userUid)
			{
				profileDataSet.profildDataPredict = profildDataSet5.profildData;
			}
			else if (profildDataSet5.profildData != null && profildDataSet5.profildData.commonProfile.userUid == profildDataSet2.profildDataPredict.commonProfile.userUid)
			{
				profileDataSet.profildDataPredict = profildDataSet4.profildData;
			}
		}
		flag2 = !base.IsCheerResultState && (m_showResultState[m_UIGroup].m_showEntryBottomResult || m_showResultState[m_UIGroup].m_showEntryMiddleResult);
		playerSlot?.SetData(profileDataSet.slotIndex, profileDataSet.profildData, profileDataSet.profildDataPredict, profileDataSet.group, flag2, leafPlayer: false);
		if (base.IsCheerResultState && profileDataSet.profildDataPredict == null)
		{
			playerSlot?.SetAsBlankSlot();
		}
		if (!base.IsCheerMode && !base.IsCheerResultState)
		{
			if (!flag2)
			{
				playerSlot?.SetAsBlankSlot();
			}
			else if (!m_showResultState[m_UIGroup].m_showEntryBottomResult)
			{
				playerSlot?.HideResultIcon();
			}
		}
		NKCUITournamentPlayerSlot playerSlot2 = m_BTree.GetPlayerSlot(8);
		NKCUITournamentPlayerSlotTree.ProfileDataSet profileDataSet2 = default(NKCUITournamentPlayerSlotTree.ProfileDataSet);
		profileDataSet2.slotIndex = 8;
		profileDataSet2.group = NKCTournamentManager.GetOriginalGroup(NKMTournamentGroups.Finals);
		profileDataSet2.profildData = null;
		profileDataSet2.profildDataPredict = null;
		if (profildDataSet3.profildData != null)
		{
			if (profildDataSet6.profildData != null && profildDataSet6.profildData.commonProfile.userUid == profildDataSet3.profildData.commonProfile.userUid)
			{
				profileDataSet2.profildData = profildDataSet7.profildData;
			}
			else if (profildDataSet7.profildData != null && profildDataSet7.profildData.commonProfile.userUid == profildDataSet3.profildData.commonProfile.userUid)
			{
				profileDataSet2.profildData = profildDataSet6.profildData;
			}
		}
		if (profildDataSet3.profildDataPredict != null)
		{
			if (profildDataSet6.profildData != null && profildDataSet6.profildData.commonProfile.userUid == profildDataSet3.profildDataPredict.commonProfile.userUid)
			{
				profileDataSet2.profildDataPredict = profildDataSet7.profildData;
			}
			else if (profildDataSet7.profildData != null && profildDataSet7.profildData.commonProfile.userUid == profildDataSet3.profildDataPredict.commonProfile.userUid)
			{
				profileDataSet2.profildDataPredict = profildDataSet6.profildData;
			}
		}
		playerSlot2?.SetData(profileDataSet2.slotIndex, profileDataSet2.profildData, profileDataSet2.profildDataPredict, profileDataSet2.group, flag2, leafPlayer: false);
		if (base.IsCheerResultState && profileDataSet2.profildDataPredict == null)
		{
			playerSlot2?.SetAsBlankSlot();
		}
		if (!base.IsCheerMode && !base.IsCheerResultState)
		{
			if (!flag2)
			{
				playerSlot2?.SetAsBlankSlot();
			}
			else if (!m_showResultState[m_UIGroup].m_showEntryBottomResult)
			{
				playerSlot2?.HideResultIcon();
			}
		}
		m_BTree.SetDetailButtonActive();
		SetCheerTimeText(m_UIGroup);
		bool flag3 = profildDataSet.profildData != null;
		NKCUtil.SetGameobjectActive(m_csbtnResultTop, flag3 && m_showResultState[m_UIGroup].m_showEntryMiddleResult && !base.IsCheerResultState);
		NKCUtil.SetGameobjectActive(m_csbtnResultMiddle, flag3 && !base.IsCheerResultState);
		NKCUtil.SetGameobjectActive(m_csbtnResultBottom, flag3 && m_showResultState[m_UIGroup].m_showEntryMiddleResult && !base.IsCheerResultState);
		m_csbtnResultTop?.SetLock(m_showResultState[m_UIGroup].m_showEntryTopResult);
		m_csbtnResultMiddle?.SetLock(m_showResultState[m_UIGroup].m_showEntryMiddleResult);
		m_csbtnResultBottom?.SetLock(m_showResultState[m_UIGroup].m_showEntryBottomResult);
		bool flag4 = flag3 && m_showResultState[m_UIGroup].m_showEntryTopResult;
		NKCUtil.SetGameobjectActive(m_objLeaderboard, flag4);
		if (flag4)
		{
			NKMTournamentProfileData nKMTournamentProfileData = profildDataSet.profildData;
			if (nKMTournamentProfileData == null)
			{
				nKMTournamentProfileData = new NKMTournamentProfileData();
			}
			LeaderBoardSlotData data = LeaderBoardSlotData.MakeSlotData(LeaderBoardType.BT_TOURNAMENT, nKMTournamentProfileData, 1, NKCTournamentManager.TournamentId);
			NKMTournamentProfileData nKMTournamentProfileData2 = null;
			if (profildDataSet2.profildData != null && profildDataSet2.profildData.commonProfile.userUid == profildDataSet.profildData.commonProfile.userUid)
			{
				nKMTournamentProfileData2 = profildDataSet3.profildData;
			}
			else if (profildDataSet3.profildData != null && profildDataSet3.profildData.commonProfile.userUid == profildDataSet.profildData.commonProfile.userUid)
			{
				nKMTournamentProfileData2 = profildDataSet2.profildData;
			}
			if (nKMTournamentProfileData2 == null)
			{
				nKMTournamentProfileData2 = new NKMTournamentProfileData();
			}
			LeaderBoardSlotData data2 = LeaderBoardSlotData.MakeSlotData(LeaderBoardType.BT_TOURNAMENT, nKMTournamentProfileData2, 2, NKCTournamentManager.TournamentId);
			NKMTournamentProfileData nKMTournamentProfileData3 = profildDataSet8.profildData;
			if (nKMTournamentProfileData3 == null)
			{
				nKMTournamentProfileData3 = new NKMTournamentProfileData();
			}
			LeaderBoardSlotData data3 = LeaderBoardSlotData.MakeSlotData(LeaderBoardType.BT_TOURNAMENT, nKMTournamentProfileData3, 3, NKCTournamentManager.TournamentId);
			string title = ((nKMTournamentTemplet != null) ? nKMTournamentTemplet.GetTournamentSeasonTitle() : "");
			m_RankSlot.SetData(title, data, data2, data3, 0, null);
		}
	}

	public override List<long> GetCheeringUIdList()
	{
		List<long> cheeringUIdList = m_BTree.GetCheeringUIdList();
		if (cheeringUIdList.Count > 7)
		{
			cheeringUIdList.RemoveRange(7, cheeringUIdList.Count - 7);
		}
		long item = 0L;
		bool flag = m_playerSlotTreeA.IsFullTree();
		bool flag2 = m_playerSlotTreeB.IsFullTree();
		if (!flag && !flag2)
		{
			item = -1L;
		}
		else if (flag && flag2)
		{
			if (m_playerSlotTree3rdPlace.m_playerA.ProfileDataPredict != null && m_playerSlotTree3rdPlace.m_playerA.IsCheering())
			{
				item = m_playerSlotTree3rdPlace.m_playerA.ProfileDataPredict.commonProfile.userUid;
			}
			else if (m_playerSlotTree3rdPlace.m_playerB.ProfileDataPredict != null && m_playerSlotTree3rdPlace.m_playerB.IsCheering())
			{
				item = m_playerSlotTree3rdPlace.m_playerB.ProfileDataPredict.commonProfile.userUid;
			}
		}
		else if (m_playerSlotTree3rdPlace.m_playerA.ProfileDataPredict != null)
		{
			item = m_playerSlotTree3rdPlace.m_playerA.ProfileDataPredict.commonProfile.userUid;
		}
		else if (m_playerSlotTree3rdPlace.m_playerB.ProfileDataPredict != null)
		{
			item = m_playerSlotTree3rdPlace.m_playerB.ProfileDataPredict.commonProfile.userUid;
		}
		cheeringUIdList.Add(item);
		return cheeringUIdList;
	}

	public override bool IsCheeringCompleted(out List<int> notCheeredSlotIndex, out int _cheerSlotCount, out int _notCheeringCount)
	{
		notCheeredSlotIndex = new List<int>();
		int num = 0;
		int num2 = 0;
		bool result = true;
		if (!m_playerSlotTreeFinal.IsBlankTree())
		{
			num++;
			if (!m_playerSlotTreeFinal.IsCheering())
			{
				result = false;
				notCheeredSlotIndex.Add(1);
				notCheeredSlotIndex.Add(2);
				num2++;
			}
		}
		if (!m_playerSlotTreeA.IsBlankTree())
		{
			num++;
			if (!m_playerSlotTreeA.IsCheering())
			{
				result = false;
				notCheeredSlotIndex.Add(3);
				notCheeredSlotIndex.Add(4);
				num2++;
			}
		}
		if (!m_playerSlotTreeB.IsBlankTree())
		{
			num++;
			if (!m_playerSlotTreeB.IsCheering())
			{
				result = false;
				notCheeredSlotIndex.Add(5);
				notCheeredSlotIndex.Add(6);
				num2++;
			}
		}
		if (m_playerSlotTreeA.IsFullTree() || m_playerSlotTreeB.IsFullTree())
		{
			num++;
			if (!m_playerSlotTree3rdPlace.IsCheering())
			{
				result = false;
				notCheeredSlotIndex.Add(7);
				notCheeredSlotIndex.Add(8);
				num2++;
			}
		}
		_cheerSlotCount = num;
		_notCheeringCount = num2;
		return result;
	}

	public override void PlayWinnerFx()
	{
		NKCUtil.SetGameobjectActive(m_objWinnerFx, bValue: false);
	}

	public override void CancelCheering()
	{
		Open();
	}

	public override void Refresh()
	{
		Open();
	}

	public override bool HaveWinner()
	{
		return m_playerSlotWinner.ProfileData != null;
	}

	private void OnClickSlot(int slotIndex, long userUId)
	{
		NKCUITournamentPlayerSlotTree slotTree = m_BTree.GetSlotTree(slotIndex);
		slotTree?.SetCheeringByUserUId(userUId);
		NKMTournamentProfileData predictData = slotTree?.GetOtherSlotProfileData(slotIndex);
		switch (slotIndex)
		{
		case 3:
		case 4:
			m_playerSlotTree3rdPlace.m_playerA.SetPredictData(predictData);
			break;
		case 5:
		case 6:
			m_playerSlotTree3rdPlace.m_playerB.SetPredictData(predictData);
			break;
		}
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
		Open();
		NKCUtil.SetGameobjectActive(m_objWinnerFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objWinnerFx, m_playerSlotWinner.ProfileData != null && m_showResultState[m_UIGroup].m_showEntryTopResult);
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
		Open();
		bool flag = false;
		int num = 6;
		for (int i = 3; i <= num; i++)
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
		Open();
		bool flag = false;
		int num = 8;
		for (int i = 7; i <= num; i++)
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
}
