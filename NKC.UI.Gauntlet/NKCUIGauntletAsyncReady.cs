using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ClientPacket.Common;
using ClientPacket.Pvp;
using Cs.Engine.Network.Buffer;
using Cs.Engine.Util;
using Cs.GameServer.Replay;
using Cs.Logging;
using Cs.Protocol;
using NKC.UI.Collection;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletAsyncReady : NKCUIBase
{
	[Serializable]
	public class UserInfoUI
	{
		public NKCUILeagueTier Tier;

		public Text Rank;

		public Text Score;

		public Text Level;

		public Text Name;

		public Text FriendCode;

		public GameObject Guild;

		public NKCUIGuildBadge GuildBadge;

		public Text GuildName;

		public NKCUIComTitlePanel TitlePanel;
	}

	[Serializable]
	public class DeckInfoUI
	{
		public NKCUIOperatorDeckSlot OperatorInfo;

		public Image Ship;

		public NKCUIShipInfoSummary ShipInfo;

		public NKCDeckViewUnitSlot[] Unit = new NKCDeckViewUnitSlot[8];

		public Text Power;

		public Text AvgCost;
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_ASYNC_READY";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public UserInfoUI m_myUserInfo;

	public DeckInfoUI m_myDeckInfo;

	public UserInfoUI m_enemyUserInfo;

	public DeckInfoUI m_enemyDeckInfo;

	public NKCUIComStateButton m_btnDeckEdit;

	public NKCUIComStateButton m_btnBattle;

	public GameObject m_objBattleTicketIcon;

	public GameObject m_objBattleTicketCount;

	public Text m_txtGetScore;

	public GameObject m_objGetScoreDesc;

	public GameObject m_objRightShipPrivate;

	[Header("포인트")]
	public GameObject m_objPVPDoublePoint;

	public Text m_lbPVPDoublePoint;

	public GameObject m_objPVPPoint;

	public Text m_lbRemainPVPPoint;

	public GameObject m_objRemainPVPPointPlusTime;

	public Text m_lbPlusPVPPoint;

	public Text m_lbRemainPVPPointPlusTime;

	public GameObject m_objPVPPointRoot;

	[Header("Sprite")]
	public Sprite SpriteUnitPrivate;

	[Header("etc")]
	public GameObject m_objGAUNTLET_ASYNC_READY_INFO_DESC;

	public NKCUIComStateButton m_csbtnReplay;

	private NKMDeckIndex m_curDeckIndex;

	private AsyncPvpTarget m_target;

	private NpcPvpTarget m_targetNpc;

	private NKM_GAME_TYPE m_gameType;

	private NKMAsyncDeckData m_tournamentDeckA;

	private NKMAsyncDeckData m_tournamentDeckB;

	private int m_tournamentSlotIndex;

	private NKMTournamentGroups m_tournamentGroup;

	private float m_fPrevUpdateTime;

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

	public override string GuideTempletID => "ARTICLE_PVP_ASYNC";

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC)
			{
				return new List<int> { 5, 101 };
			}
			if (m_gameType == NKM_GAME_TYPE.NGT_PVE_SIMULATED)
			{
				return new List<int>();
			}
			return new List<int> { 13, 5, 101 };
		}
	}

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIGauntletAsyncReady>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_ASYNC_READY", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIGauntletAsyncReady GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIGauntletAsyncReady>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public void Init()
	{
		for (int i = 0; i < m_myDeckInfo.Unit.Length; i++)
		{
			int index = i;
			NKCDeckViewUnitSlot obj = m_myDeckInfo.Unit[i];
			obj.Init(i, bEnableDrag: false);
			obj.m_NKCUIComButton.PointerClick.RemoveAllListeners();
			obj.m_NKCUIComButton.PointerClick.AddListener(delegate
			{
				OnTouchDeckEdit(index, NKM_UNIT_TYPE.NUT_NORMAL);
			});
		}
		NKCUIComButton component = m_myDeckInfo.Ship.GetComponent<NKCUIComButton>();
		if (component != null)
		{
			component.PointerClick.RemoveAllListeners();
			component.PointerClick.AddListener(delegate
			{
				OnTouchDeckEdit(0, NKM_UNIT_TYPE.NUT_SHIP);
			});
		}
		m_myDeckInfo.OperatorInfo.Init(delegate
		{
			OnTouchDeckEdit(0, NKM_UNIT_TYPE.NUT_OPERATOR);
		});
		for (int num = 0; num < m_enemyDeckInfo.Unit.Length; num++)
		{
			int index2 = num;
			m_enemyDeckInfo.Unit[num].Init(num, bEnableDrag: false);
			m_enemyDeckInfo.Unit[num].m_NKCUIComButton.PointerClick.RemoveAllListeners();
			m_enemyDeckInfo.Unit[num].m_NKCUIComButton.PointerClick.AddListener(delegate
			{
				OnTouchEnemyDeckEdit(index2, NKM_UNIT_TYPE.NUT_NORMAL);
			});
		}
		NKCUtil.SetButtonClickDelegate(m_enemyDeckInfo.Ship.GetComponent<NKCUIComButton>(), delegate
		{
			OnTouchEnemyDeckEdit(0, NKM_UNIT_TYPE.NUT_SHIP);
		});
		m_enemyDeckInfo.OperatorInfo.Init(delegate
		{
			OnTouchEnemyDeckEdit(0, NKM_UNIT_TYPE.NUT_OPERATOR);
		});
		m_btnDeckEdit.PointerClick.RemoveAllListeners();
		m_btnDeckEdit.PointerClick.AddListener(delegate
		{
			OnTouchDeckEdit(0, NKM_UNIT_TYPE.NUT_INVALID);
		});
		m_btnBattle.PointerClick.RemoveAllListeners();
		m_btnBattle.PointerClick.AddListener(OnTouchBattle);
		NKCUtil.SetButtonClickDelegate(m_csbtnReplay, OnClickReplay);
	}

	public void Open(AsyncPvpTarget target, NKMDeckIndex lastDeckIndex, NKM_GAME_TYPE gameType)
	{
		if (target == null)
		{
			Debug.LogError("NKCUIGauntletAsyncReady target 없는데 왜 ?");
			return;
		}
		m_target = target;
		m_gameType = gameType;
		if (NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(lastDeckIndex) == null)
		{
			m_curDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP, 0);
		}
		else
		{
			m_curDeckIndex = lastDeckIndex;
		}
		SetMyUserInfo();
		SetMyDeckInfo(m_curDeckIndex);
		SetEnemyUserInfo(target);
		SetEnemyDeckInfo(target.asyncDeck);
		UpdateRankPVPPointUI();
		NKCUtil.SetGameobjectActive(m_csbtnReplay, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnDeckEdit, bValue: true);
		NKCUtil.SetGameobjectActive(m_btnBattle, bValue: true);
		UIOpened();
	}

	public void Open(NpcPvpTarget target, NKMDeckIndex lastDeckIndex, NKM_GAME_TYPE gameType)
	{
		if (target == null)
		{
			Debug.LogError("NKCUIGauntletAsyncReady.Open - NpcPvpTarget is null");
			return;
		}
		m_targetNpc = target;
		m_gameType = gameType;
		if (NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(lastDeckIndex) == null)
		{
			m_curDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP, 0);
		}
		else
		{
			m_curDeckIndex = lastDeckIndex;
		}
		SetMyUserInfo();
		SetMyDeckInfo(m_curDeckIndex);
		SetEnemyUserInfo(target);
		SetEnemyDeckInfo(target.asyncDeck);
		UpdateRankPVPPointUI();
		NKCUtil.SetGameobjectActive(m_csbtnReplay, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnDeckEdit, bValue: true);
		NKCUtil.SetGameobjectActive(m_btnBattle, bValue: true);
		UIOpened();
	}

	public void Open(int resultSlotIndex, NKMTournamentGroups group, NKMTournamentProfileData targetA, NKMTournamentProfileData targetB, NKM_GAME_TYPE gameType, bool replayActive)
	{
		m_gameType = gameType;
		m_tournamentSlotIndex = resultSlotIndex;
		m_tournamentGroup = group;
		if (targetA != null)
		{
			m_tournamentDeckA = targetA.deck;
			SetTournamentUserInfo(targetA.commonProfile, m_myUserInfo, targetA.guildData);
			SetTournamentDeckInfo(targetA.deck, m_myDeckInfo);
		}
		else
		{
			m_tournamentDeckA = null;
			SetTournamentUserInfoEmpty(m_myUserInfo);
			SetTournamentDeckInfo(new NKMAsyncDeckData(), m_myDeckInfo);
		}
		if (targetB != null)
		{
			m_tournamentDeckB = targetB.deck;
			SetTournamentUserInfo(targetB.commonProfile, m_enemyUserInfo, targetB.guildData);
			SetTournamentDeckInfo(targetB.deck, m_enemyDeckInfo);
		}
		else
		{
			m_tournamentDeckB = null;
			SetTournamentUserInfoEmpty(m_enemyUserInfo);
			SetTournamentDeckInfo(new NKMAsyncDeckData(), m_enemyDeckInfo);
		}
		NKCUtil.SetGameobjectActive(m_objPVPPointRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBattleTicketIcon, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBattleTicketCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnReplay, replayActive);
		NKCUtil.SetGameobjectActive(m_btnDeckEdit, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnBattle, bValue: false);
		UIOpened();
	}

	public NKMDeckIndex GetLastDeckIndex()
	{
		return m_curDeckIndex;
	}

	private void SetMyUserInfo()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_myUserInfo.Level, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKMUserData.UserLevel);
		NKCUtil.SetLabelText(m_myUserInfo.Name, NKCUtilString.GetUserNickname(nKMUserData.m_UserNickName, bOpponent: false));
		NKCUtil.SetGameobjectActive(m_enemyUserInfo.FriendCode, nKMUserData.m_FriendCode > 0);
		NKCUtil.SetLabelText(m_myUserInfo.FriendCode, NKCUtilString.GetFriendCode(nKMUserData.m_FriendCode, bOpponent: false));
		m_myUserInfo.TitlePanel?.SetData((nKMUserData.UserProfileData != null) ? nKMUserData.UserProfileData.commonProfile : null);
		NKCUtil.SetGameobjectActive(m_myUserInfo.Guild, NKCGuildManager.HasGuild());
		if (m_myUserInfo.Guild != null && m_myUserInfo.Guild.activeSelf)
		{
			m_myUserInfo.GuildBadge.SetData(NKCGuildManager.MyGuildData.badgeId, bOpponent: false);
			NKCUtil.SetLabelText(m_myUserInfo.GuildName, NKCUtilString.GetUserGuildName(NKCGuildManager.MyGuildData.name, bOpponent: false));
		}
		PvpState asyncData = nKMUserData.m_AsyncData;
		int num = NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime());
		NKCUtil.SetGameobjectActive(m_myUserInfo.Tier, bValue: true);
		NKCUtil.SetGameobjectActive(m_myUserInfo.Score, bValue: true);
		NKCUtil.SetGameobjectActive(m_myUserInfo.Rank, bValue: true);
		if (asyncData.SeasonID != num)
		{
			NKCUtil.SetLabelText(m_myUserInfo.Score, NKCPVPManager.GetResetScore(asyncData.SeasonID, asyncData.Score, NKM_GAME_TYPE.NGT_ASYNC_PVP).ToString());
			NKCUtil.SetLabelText(m_myUserInfo.Rank, "");
			NKMPvpRankTemplet asyncPvpRankTempletByScore = NKCPVPManager.GetAsyncPvpRankTempletByScore(num, NKCUtil.GetScoreBySeason(num, asyncData.SeasonID, asyncData.Score, NKM_GAME_TYPE.NGT_ASYNC_PVP));
			if (asyncPvpRankTempletByScore != null)
			{
				m_myUserInfo.Tier.SetUI(asyncPvpRankTempletByScore);
			}
		}
		else
		{
			NKMPvpRankTemplet asyncPvpRankTempletByTier = NKCPVPManager.GetAsyncPvpRankTempletByTier(num, asyncData.LeagueTierID);
			if (asyncPvpRankTempletByTier != null)
			{
				m_myUserInfo.Tier.SetUI(asyncPvpRankTempletByTier);
			}
			NKCUtil.SetLabelText(m_myUserInfo.Score, asyncData.Score.ToString());
			NKCUtil.SetLabelText(m_myUserInfo.Rank, string.Format($"{asyncData.Rank}{NKCUtilString.GetRankNumber(asyncData.Rank, bUpper: true)}"));
		}
	}

	private void SetEnemyUserInfo(AsyncPvpTarget target)
	{
		SetEnemyUserInfo(target.userLevel, target.userNickName, target.userFriendCode, target.guildData, target.tier, target.rank, target.score, target.titleId);
	}

	private void SetEnemyUserInfo(NpcPvpTarget target)
	{
		SetEnemyUserInfo(target.userLevel, target.userNickName, target.userFriendCode, null, target.tier, 0, target.score, 0);
	}

	private void SetEnemyUserInfo(int userLv, string nickName, long friendCode, NKMGuildSimpleData guildData, int tier, int rank, int score, int titleId)
	{
		NKCUtil.SetLabelText(m_enemyUserInfo.Level, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, userLv);
		NKCUtil.SetLabelText(m_enemyUserInfo.Name, NKCUtilString.GetUserNickname(nickName, bOpponent: true));
		NKCUtil.SetGameobjectActive(m_enemyUserInfo.FriendCode, friendCode > 0);
		NKCUtil.SetLabelText(m_enemyUserInfo.FriendCode, NKCUtilString.GetFriendCode(friendCode, bOpponent: true));
		NKCUtil.SetGameobjectActive(m_enemyUserInfo.Guild, guildData != null && guildData.guildUid > 0);
		if (m_enemyUserInfo.Guild != null && m_enemyUserInfo.Guild.activeSelf)
		{
			m_enemyUserInfo.GuildBadge.SetData(guildData.badgeId, bOpponent: true);
			NKCUtil.SetLabelText(m_enemyUserInfo.GuildName, NKCUtilString.GetUserGuildName(guildData.guildName, bOpponent: true));
		}
		int seasonID = NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime());
		NKMPvpRankTemplet asyncPvpRankTempletByTier = NKCPVPManager.GetAsyncPvpRankTempletByTier(seasonID, tier);
		if (asyncPvpRankTempletByTier != null)
		{
			m_enemyUserInfo.Tier.SetUI(asyncPvpRankTempletByTier);
		}
		if (rank == 0)
		{
			NKCUtil.SetLabelText(m_enemyUserInfo.Rank, "");
		}
		else
		{
			NKCUtil.SetLabelText(m_enemyUserInfo.Rank, string.Format($"{rank}{NKCUtilString.GetRankNumber(rank, bUpper: true)}"));
		}
		NKCUtil.SetLabelText(m_enemyUserInfo.Score, score.ToString());
		NKCUtil.SetGameobjectActive(m_objGetScoreDesc, bValue: true);
		if (m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC && m_targetNpc != null)
		{
			NKCUtil.SetLabelText(m_txtGetScore, $"+{score}");
		}
		else
		{
			PvpState asyncData = NKCScenManager.CurrentUserData().m_AsyncData;
			if (asyncData == null)
			{
				return;
			}
			NKMPvpRankTemplet asyncPvpRankTempletByScore = NKCPVPManager.GetAsyncPvpRankTempletByScore(seasonID, asyncData.Score);
			if (asyncPvpRankTempletByScore != null)
			{
				int num = NKCUtil.CalcAddScore(asyncPvpRankTempletByScore.LeagueType, asyncData.Score, score);
				NKCUtil.SetLabelText(m_txtGetScore, $"+{num}");
			}
			else
			{
				NKCUtil.SetLabelText(m_txtGetScore, "");
			}
		}
		NKCUtil.SetGameobjectActive(m_enemyUserInfo.Rank, m_gameType != NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC);
		NKCUtil.SetGameobjectActive(m_enemyUserInfo.Tier, m_gameType != NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC);
		NKCUtil.SetGameobjectActive(m_enemyUserInfo.Score, m_gameType != NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC);
		bool flag = m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC || m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE;
		NKCUtil.SetGameobjectActive(m_objGAUNTLET_ASYNC_READY_INFO_DESC, !flag);
		m_enemyUserInfo.TitlePanel?.SetData(titleId);
	}

	private void SetTournamentUserInfo(NKMCommonProfile profile, UserInfoUI userInfoUI, NKMGuildSimpleData guildData)
	{
		SetTournamentUserInfo(userInfoUI, profile.level, profile.nickname, profile.friendCode, guildData, profile.titleId);
	}

	private void SetTournamentUserInfo(UserInfoUI userInfoUI, int userLv, string nickName, long friendCode, NKMGuildSimpleData guildData, int titleId)
	{
		NKCUtil.SetLabelText(userInfoUI.Level, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, userLv);
		NKCUtil.SetLabelText(userInfoUI.Name, NKCUtilString.GetUserNickname(nickName, bOpponent: true));
		NKCUtil.SetGameobjectActive(userInfoUI.FriendCode, friendCode > 0);
		NKCUtil.SetLabelText(userInfoUI.FriendCode, NKCUtilString.GetFriendCode(friendCode, bOpponent: true));
		NKCUtil.SetGameobjectActive(userInfoUI.Guild, guildData != null && guildData.guildUid > 0);
		if (userInfoUI.Guild != null && userInfoUI.Guild.activeSelf)
		{
			userInfoUI.GuildBadge.SetData(guildData.badgeId, bOpponent: true);
			NKCUtil.SetLabelText(userInfoUI.GuildName, NKCUtilString.GetUserGuildName(guildData.guildName, bOpponent: true));
		}
		NKCUtil.SetGameobjectActive(m_objGetScoreDesc, bValue: false);
		NKCUtil.SetLabelText(m_txtGetScore, "");
		NKCUtil.SetGameobjectActive(userInfoUI.Rank, bValue: false);
		NKCUtil.SetGameobjectActive(userInfoUI.Tier, bValue: false);
		NKCUtil.SetGameobjectActive(userInfoUI.Score, bValue: false);
		NKCUtil.SetGameobjectActive(m_objGAUNTLET_ASYNC_READY_INFO_DESC, bValue: false);
		userInfoUI.TitlePanel?.SetData(titleId);
	}

	private void SetTournamentUserInfoEmpty(UserInfoUI userInfoUI)
	{
		NKCUtil.SetLabelText(userInfoUI.Level, "");
		NKCUtil.SetLabelText(userInfoUI.Name, "");
		NKCUtil.SetGameobjectActive(userInfoUI.FriendCode, bValue: false);
		NKCUtil.SetGameobjectActive(userInfoUI.Guild, bValue: false);
		NKCUtil.SetGameobjectActive(m_objGetScoreDesc, bValue: false);
		NKCUtil.SetLabelText(m_txtGetScore, "");
		NKCUtil.SetGameobjectActive(userInfoUI.Rank, bValue: false);
		NKCUtil.SetGameobjectActive(userInfoUI.Tier, bValue: false);
		NKCUtil.SetGameobjectActive(userInfoUI.Score, bValue: false);
		NKCUtil.SetGameobjectActive(m_objGAUNTLET_ASYNC_READY_INFO_DESC, bValue: false);
		userInfoUI.TitlePanel?.SetData(0);
	}

	private void SetMyDeckInfo(NKMDeckIndex myDeckIndex)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		NKMDeckData deckData = armyData.GetDeckData(myDeckIndex);
		if (deckData == null)
		{
			return;
		}
		NKMUnitData shipFromUID = armyData.GetShipFromUID(deckData.m_ShipUID);
		NKMUnitTempletBase shipTempletBase = null;
		NKCUtil.SetGameobjectActive(m_myDeckInfo.Ship, bValue: true);
		if (shipFromUID != null)
		{
			m_myDeckInfo.Ship.enabled = true;
			m_myDeckInfo.Ship.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, shipFromUID);
			shipTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
		}
		else
		{
			m_myDeckInfo.Ship.enabled = false;
			m_myDeckInfo.Ship.sprite = null;
		}
		m_myDeckInfo.ShipInfo.SetShipData(shipFromUID, shipTempletBase, myDeckIndex);
		for (int i = 0; i < m_myDeckInfo.Unit.Length; i++)
		{
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(deckData.m_listDeckUnitUID[i]);
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_myDeckInfo.Unit[i];
			if (unitFromUID != null)
			{
				nKCDeckViewUnitSlot.SetData(unitFromUID);
				nKCDeckViewUnitSlot.SetLeader(i == deckData.m_LeaderIndex, bEffect: false);
			}
			else
			{
				nKCDeckViewUnitSlot.SetData(null);
				nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
			}
		}
		if (!NKCOperatorUtil.IsHide())
		{
			NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(deckData.m_OperatorUID);
			if (operatorData != null)
			{
				m_myDeckInfo.OperatorInfo.SetData(operatorData);
			}
			else
			{
				m_myDeckInfo.OperatorInfo.SetEmpty();
			}
		}
		NKCUtil.SetGameobjectActive(m_myDeckInfo.OperatorInfo, !NKCOperatorUtil.IsHide());
		NKCUtil.SetLabelText(m_myDeckInfo.Power, armyData.GetArmyAvarageOperationPower(myDeckIndex).ToString("N0"));
		NKCUtil.SetLabelText(m_myDeckInfo.AvgCost, $"{armyData.CalculateDeckAvgSummonCost(myDeckIndex).ToString():0.00}");
	}

	private void SetEnemyDeckInfo(NKMAsyncDeckData asyncDeck)
	{
		if (NKCPVPManager.GetPvpAsyncSeasonTemplet(NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime())) == null)
		{
			Debug.LogError("Gauntlet Async Ready - NKMPvpRankSeasonTemplet is null");
		}
		else
		{
			if (asyncDeck == null)
			{
				return;
			}
			NKMAsyncUnitData ship = asyncDeck.ship;
			if (ship.unitId > 0)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(ship.unitId);
				NKCUtil.SetGameobjectActive(m_enemyDeckInfo.Ship, bValue: true);
				m_enemyDeckInfo.Ship.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
				NKMUnitData shipData = NKMDungeonManager.MakeUnitData(ship, -1L);
				m_enemyDeckInfo.ShipInfo.SetShipData(shipData, unitTempletBase);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_enemyDeckInfo.Ship, bValue: false);
				m_enemyDeckInfo.Ship.sprite = null;
				m_enemyDeckInfo.ShipInfo.SetShipData(null, null);
			}
			if (!NKCOperatorUtil.IsHide())
			{
				if (asyncDeck.operatorUnit != null && asyncDeck.operatorUnit.id > 0)
				{
					NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(asyncDeck.operatorUnit.id);
					m_enemyDeckInfo.OperatorInfo.SetData(unitTempletBase2, asyncDeck.operatorUnit.level);
				}
				else
				{
					m_enemyDeckInfo.OperatorInfo.SetEmpty();
				}
			}
			NKCUtil.SetGameobjectActive(m_enemyDeckInfo.OperatorInfo, !NKCOperatorUtil.IsHide());
			bool enableShowBan = m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY || m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE;
			bool enableShowUpUnit = m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY || m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE;
			bool flag = false;
			for (int i = 0; i < m_enemyDeckInfo.Unit.Length; i++)
			{
				NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_enemyDeckInfo.Unit[i];
				if (i >= asyncDeck.units.Count)
				{
					nKCDeckViewUnitSlot.SetData(null, bEnableButton: false);
					nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
					continue;
				}
				NKMAsyncUnitData nKMAsyncUnitData = asyncDeck.units[i];
				if (nKMAsyncUnitData.unitId > 0)
				{
					NKMUnitData cNKMUnitData = NKMDungeonManager.MakeUnitData(nKMAsyncUnitData, -1L);
					nKCDeckViewUnitSlot.SetEnableShowBan(enableShowBan);
					nKCDeckViewUnitSlot.SetEnableShowUpUnit(enableShowUpUnit);
					nKCDeckViewUnitSlot.SetData(cNKMUnitData, bEnableButton: false);
					nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
				}
				else
				{
					nKCDeckViewUnitSlot.SetPrivate();
					flag = true;
				}
			}
			if (asyncDeck.operationPower >= 0)
			{
				NKCUtil.SetLabelText(m_enemyDeckInfo.Power, asyncDeck.operationPower.ToString("N0"));
			}
			else
			{
				NKCUtil.SetLabelText(m_enemyDeckInfo.Power, "???");
			}
			if (flag)
			{
				NKCUtil.SetLabelText(m_enemyDeckInfo.AvgCost, "???");
			}
			else
			{
				NKCUtil.SetLabelText(m_enemyDeckInfo.AvgCost, $"{CalculateDeckAvgSummonCost(asyncDeck):0.00}");
			}
		}
	}

	private void SetTournamentDeckInfo(NKMAsyncDeckData asyncDeck, DeckInfoUI deckInfo)
	{
		if (asyncDeck == null)
		{
			return;
		}
		NKMAsyncUnitData ship = asyncDeck.ship;
		if (ship.unitId > 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(ship.unitId);
			NKCUtil.SetGameobjectActive(deckInfo.Ship, bValue: true);
			deckInfo.Ship.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
			NKMUnitData shipData = NKMDungeonManager.MakeUnitData(ship, -1L);
			deckInfo.ShipInfo.SetShipData(shipData, unitTempletBase);
		}
		else
		{
			NKCUtil.SetGameobjectActive(deckInfo.Ship, bValue: false);
			deckInfo.Ship.sprite = null;
			deckInfo.ShipInfo.SetShipData(null, null);
		}
		deckInfo.Ship.enabled = true;
		if (!NKCOperatorUtil.IsHide())
		{
			if (asyncDeck.operatorUnit != null && asyncDeck.operatorUnit.id > 0)
			{
				deckInfo.OperatorInfo.SetData(asyncDeck.operatorUnit);
			}
			else
			{
				deckInfo.OperatorInfo.SetEmpty();
			}
		}
		NKCUtil.SetGameobjectActive(deckInfo.OperatorInfo, !NKCOperatorUtil.IsHide());
		for (int i = 0; i < deckInfo.Unit.Length; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = deckInfo.Unit[i];
			if (i >= asyncDeck.units.Count)
			{
				nKCDeckViewUnitSlot.SetData(null, bEnableButton: false);
				nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
				continue;
			}
			NKMAsyncUnitData nKMAsyncUnitData = asyncDeck.units[i];
			if (nKMAsyncUnitData.unitId > 0)
			{
				NKMUnitData nKMUnitData = new NKMUnitData();
				nKMUnitData.FillDataFromAsyncUnitData(nKMAsyncUnitData);
				nKCDeckViewUnitSlot.SetData(nKMUnitData);
				nKCDeckViewUnitSlot.SetLeader(asyncDeck.leaderIndex == i, bEffect: false);
			}
			else
			{
				nKCDeckViewUnitSlot.SetData(null, bEnableButton: false);
				nKCDeckViewUnitSlot.SetLeader(bLeader: false, bEffect: false);
			}
		}
		NKCUtil.SetLabelText(deckInfo.Power, asyncDeck.operationPower.ToString("N0"));
		NKCUtil.SetLabelText(deckInfo.AvgCost, $"{CalculateDeckAvgSummonCost(asyncDeck):0.00}");
	}

	public void UpdateRankPVPPointUI()
	{
		if (m_gameType == NKM_GAME_TYPE.NGT_PVE_SIMULATED)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objPVPPointRoot, bValue: true);
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		long countMiscItem = inventoryData.GetCountMiscItem(301);
		int cHARGE_POINT_MAX_COUNT = NKMPvpCommonConst.Instance.CHARGE_POINT_MAX_COUNT;
		long countMiscItem2 = inventoryData.GetCountMiscItem(6);
		int rewardChargePoint = NKMPvpCommonConst.Instance.CHARGE_POINT_ONE_STEP;
		int bonusRatio = 0;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCCompanyBuff.IncreaseChargePointOfPvpWithBonusRatio(nKMUserData.m_companyBuffDataList, ref rewardChargePoint, out bonusRatio);
		}
		if (countMiscItem > 0)
		{
			NKCUtil.SetGameobjectActive(m_objPVPDoublePoint, bValue: true);
			NKCUtil.SetLabelText(m_lbPVPDoublePoint, countMiscItem.ToString());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objPVPDoublePoint, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objPVPPoint, bValue: true);
		NKCUtil.SetLabelText(m_lbRemainPVPPoint, $"{countMiscItem2}<color=#5d77a3>/{cHARGE_POINT_MAX_COUNT}</color>");
		if (NKCScenManager.CurrentUserData().m_AsyncData == null)
		{
			return;
		}
		if (countMiscItem2 < cHARGE_POINT_MAX_COUNT)
		{
			NKCUtil.SetGameobjectActive(m_objRemainPVPPointPlusTime, bValue: true);
			if (bonusRatio > 0)
			{
				NKCUtil.SetLabelText(m_lbPlusPVPPoint, $"<color=#00baff>+{rewardChargePoint}</color>");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbPlusPVPPoint, $"+{rewardChargePoint}");
			}
			DateTime dateTime = new DateTime(NKCPVPManager.GetLastUpdateChargePointTicks());
			DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
			TimeSpan timeSpan = new DateTime(dateTime.Ticks + NKMPvpCommonConst.Instance.CHARGE_POINT_REFRESH_INTERVAL_TICKS) - serverUTCTime;
			if (timeSpan.TotalHours >= 1.0)
			{
				NKCUtil.SetLabelText(m_lbRemainPVPPointPlusTime, $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbRemainPVPPointPlusTime, $"{timeSpan.Minutes}:{timeSpan.Seconds:00}");
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().ProcessPVPPointCharge();
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRemainPVPPointPlusTime, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objBattleTicketIcon, m_gameType != NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC);
		NKCUtil.SetGameobjectActive(m_objBattleTicketCount, m_gameType != NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC);
	}

	public override void CloseInternal()
	{
		m_tournamentDeckA = null;
		m_tournamentDeckB = null;
		base.gameObject.SetActive(value: false);
	}

	private void OnTouchDeckEdit(int slotIndex, NKM_UNIT_TYPE unitType)
	{
		if (m_gameType == NKM_GAME_TYPE.NGT_PVE_SIMULATED)
		{
			if (m_tournamentDeckA != null)
			{
				switch (unitType)
				{
				case NKM_UNIT_TYPE.NUT_NORMAL:
					OnSelectDeckViewUnit(slotIndex, m_myDeckInfo.Unit, m_tournamentDeckA.equips);
					break;
				case NKM_UNIT_TYPE.NUT_SHIP:
				{
					NKMUnitData nKMUnitData = new NKMUnitData();
					nKMUnitData.FillDataFromAsyncUnitData(m_tournamentDeckA.ship);
					OnSelectShip(nKMUnitData);
					break;
				}
				case NKM_UNIT_TYPE.NUT_OPERATOR:
					OnSelectOperator(m_tournamentDeckA.operatorUnit);
					break;
				}
			}
			return;
		}
		NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
		{
			MenuName = NKCUtilString.GET_STRING_GAUNTLET,
			eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.AsyncPvPBattleStart,
			dOnSideMenuButtonConfirm = OnTouchDeckSelect,
			dOnBackButton = OnTouchDeckSelect
		};
		if (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(m_curDeckIndex) == null)
		{
			m_curDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP, 0);
		}
		options.DeckIndex = m_curDeckIndex;
		options.SelectLeaderUnitOnOpen = true;
		options.bEnableDefaultBackground = false;
		options.bUpsideMenuHomeButton = false;
		options.upsideMenuShowResourceList = UpsideMenuShowResourceList;
		options.StageBattleStrID = string.Empty;
		NKCUIDeckViewer.Instance.Open(options);
	}

	private void OnTouchEnemyDeckEdit(int slotIndex, NKM_UNIT_TYPE unitType)
	{
		if (m_gameType == NKM_GAME_TYPE.NGT_PVE_SIMULATED && m_tournamentDeckB != null)
		{
			switch (unitType)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
				OnSelectDeckViewUnit(slotIndex, m_enemyDeckInfo.Unit, m_tournamentDeckB.equips);
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
			{
				NKMUnitData nKMUnitData = new NKMUnitData();
				nKMUnitData.FillDataFromAsyncUnitData(m_tournamentDeckB.ship);
				OnSelectShip(nKMUnitData);
				break;
			}
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				OnSelectOperator(m_tournamentDeckB.operatorUnit);
				break;
			}
		}
	}

	private void OnTouchBattle()
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = CheckSelectDeck();
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (m_gameType != NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC && !nKMUserData.CheckPrice(1L, 13))
		{
			NKCShopManager.OpenItemLackPopup(13, 1);
		}
		else if (IsAllUnitsNotEquipedAllGears(m_curDeckIndex))
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_GAUNTLET_DECK_UNIT_NOT_ALL_EQUIPED_GEAR_DESC, delegate
			{
				if (IsNotEnoughGauntletPoint())
				{
					NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_TICKET_USE_POPUP_TEXT, SendBattleStartReq);
				}
				else
				{
					SendBattleStartReq();
				}
			});
		}
		else if (IsNotEnoughGauntletPoint())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_TICKET_USE_POPUP_TEXT, SendBattleStartReq);
		}
		else
		{
			SendBattleStartReq();
		}
	}

	private bool IsAllUnitsNotEquipedAllGears(NKMDeckIndex curDeck)
	{
		if (m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY)
		{
			return !NKMArmyData.IsAllUnitsEquipedAllGears(curDeck);
		}
		return false;
	}

	private bool IsNotEnoughGauntletPoint()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (m_gameType != NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC)
		{
			return nKMUserData.m_InventoryData.GetCountMiscItem(6) < NKMPvpCommonConst.Instance.ASYNC_PVP_WIN_POINT;
		}
		return false;
	}

	private void SendBattleStartReq()
	{
		if (m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC)
		{
			NKCPacketSender.Send_NKMPacket_ASYNC_PVP_START_GAME_REQ(m_targetNpc.userFriendCode, m_curDeckIndex.m_iIndex, m_gameType);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_ASYNC_PVP_START_GAME_REQ(m_target.userFriendCode, m_curDeckIndex.m_iIndex, m_gameType);
		}
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_START_GAME_ACK packet)
	{
		if (packet.errorCode == NKM_ERROR_CODE.NEC_OK)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetReservedGameType(m_gameType);
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetDeckIndex(m_curDeckIndex.m_iIndex);
			if (m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetReservedAsyncTarget(m_targetNpc);
			}
			else
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetReservedAsyncTarget(m_target);
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH);
			return;
		}
		if (packet.errorCode == NKM_ERROR_CODE.NEC_FAIL_ASYNC_PVP_CANNOT_FOUND_TARGET)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY()?.SetReserved_NKM_ERROR_CODE(packet.errorCode);
			OnBackButton();
			return;
		}
		NKCPopupOKCancel.OpenOKBox(packet.errorCode, delegate
		{
			if (m_gameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC)
			{
				SetEnemyUserInfo(m_targetNpc);
				SetEnemyDeckInfo(m_targetNpc.asyncDeck);
			}
			else
			{
				AsyncPvpTarget asyncPvpTarget = NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().AsyncTargetList.Find((AsyncPvpTarget v) => v.userFriendCode == m_target.userFriendCode);
				if (asyncPvpTarget != null)
				{
					SetEnemyUserInfo(asyncPvpTarget);
					SetEnemyDeckInfo(asyncPvpTarget.asyncDeck);
				}
			}
		});
	}

	private NKM_ERROR_CODE CheckSelectDeck()
	{
		return NKMMain.IsValidDeck(NKCScenManager.CurrentUserData().m_ArmyData, m_curDeckIndex.m_eDeckType, m_curDeckIndex.m_iIndex, NKM_GAME_TYPE.NGT_ASYNC_PVP);
	}

	private void OnTouchDeckSelect(NKMDeckIndex deckIndex, long supportUserUID = 0L)
	{
		m_curDeckIndex = deckIndex;
		SetMyDeckInfo(deckIndex);
		NKCUIDeckViewer.Instance.Close();
	}

	private void OnTouchDeckSelect()
	{
		OnTouchDeckSelect(m_curDeckIndex, 0L);
	}

	private void OnSelectDeckViewUnit(int selectedIndex, NKCDeckViewUnitSlot[] listNKCDeckViewUnitSlot, List<NKMEquipItemData> listNKMEquipItemData)
	{
		List<NKMUnitData> list = new List<NKMUnitData>();
		for (int i = 0; i < listNKCDeckViewUnitSlot.Length; i++)
		{
			if (listNKCDeckViewUnitSlot[i].m_NKMUnitData != null)
			{
				list.Add(listNKCDeckViewUnitSlot[i].m_NKMUnitData);
			}
		}
		int num = 0;
		for (int j = 0; j < listNKCDeckViewUnitSlot.Length; j++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = listNKCDeckViewUnitSlot[j];
			if (j != selectedIndex)
			{
				nKCDeckViewUnitSlot.ButtonDeSelect();
			}
			else
			{
				if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_DETAIL_HISTORY))
				{
					nKCDeckViewUnitSlot.ButtonDeSelect();
					continue;
				}
				if (nKCDeckViewUnitSlot.m_NKMUnitData == null)
				{
					continue;
				}
				nKCDeckViewUnitSlot.ButtonSelect();
				NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(list, num);
				NKCUICollectionUnitInfo.CheckInstanceAndOpen(nKCDeckViewUnitSlot.m_NKMUnitData, openOption, listNKMEquipItemData, NKCUICollectionUnitInfo.eCollectionState.CS_STATUS, isGauntlet: true);
			}
			if (nKCDeckViewUnitSlot.m_NKMUnitData != null)
			{
				num++;
			}
		}
	}

	private void OnSelectShip(NKMUnitData shipUnitData)
	{
		if (shipUnitData != null && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_DETAIL_HISTORY))
		{
			NKCUICollectionShipInfo.CheckInstanceAndOpen(shipUnitData, NKMDeckIndex.None, null, null, isGauntlet: true);
		}
	}

	public void OnSelectOperator(NKMOperator operatorData)
	{
		if (operatorData != null && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_DETAIL_HISTORY))
		{
			NKCUICollectionOperatorInfoV2.CheckInstanceAndOpen(operatorData, null, NKCUICollectionOperatorInfoV2.eCollectionState.CS_STATUS);
		}
	}

	private void OnClickReplay()
	{
		NKCPacketSender.Send_kNKMPacket_TOURNAMENT_REPLAY_LINK_REQ(NKCTournamentManager.TournamentId, NKCTournamentManager.GetOriginalGroup(m_tournamentGroup), m_tournamentSlotIndex);
	}

	public void ReplayFileReq(string url, string checksum)
	{
		StartCoroutine(IRequestTournamentReplay(url, checksum));
	}

	private ReplayData GetReplayData(byte[] replayByte)
	{
		ZeroCopyBuffer zeroCopyBuffer = Lz4Util.Decompress(replayByte);
		using (zeroCopyBuffer.Hold())
		{
			using PacketReader packetReader = new PacketReader(zeroCopyBuffer.GetReader());
			string text = packetReader.GetString();
			if (text != "RV006")
			{
				Log.Warn("[ReplayData] version mismatched. current:RV006 saved:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletAsyncReady.cs", 1052);
				return null;
			}
			ReplayData replayData = new ReplayData();
			packetReader.GetWithoutNullBit(replayData);
			Log.Debug($"[ReplayData] syncCount:{replayData.syncList.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletAsyncReady.cs", 1058);
			return replayData;
		}
	}

	private IEnumerator IRequestTournamentReplay(string url, string checksum)
	{
		using (UnityWebRequest request = UnityWebRequest.Get(url))
		{
			request.SendWebRequest();
			while (request.result == UnityWebRequest.Result.InProgress)
			{
				yield return null;
			}
			if (request.result == UnityWebRequest.Result.Success)
			{
				byte[] data = request.downloadHandler.data;
				bool flag;
				if (string.IsNullOrEmpty(checksum))
				{
					flag = true;
				}
				else
				{
					byte[] array = MD5.Create().ComputeHash(data);
					StringBuilder stringBuilder = new StringBuilder();
					byte[] array2 = array;
					foreach (byte b in array2)
					{
						stringBuilder.Append(b.ToString("x2"));
					}
					string text = stringBuilder.ToString();
					if (text != checksum)
					{
						Log.Error("replay download file md5 check sum is not matched  url : " + url + ", checksum : " + checksum + ", download file check sum : " + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletAsyncReady.cs", 1099);
						flag = false;
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					ReplayData replayData = GetReplayData(data);
					if (replayData != null)
					{
						if (replayData.gameData != null)
						{
							replayData.replayName = NKCReplayMgr.MakeReplayDataFileName(replayData.gameData.m_GameUID);
						}
						NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
						if (myUserData != null)
						{
							string path = myUserData.m_UserUID.ToString();
							Path.Combine(ReplayRecorder.ReplaySavePath, path, replayData.replayName + ".replay");
							ReplayRecorder.WriteReplayDataToFile(myUserData.m_UserUID.ToString(), replayData);
						}
						NKCTournamentManager.SetReplayTournamentGroup(m_tournamentGroup);
						NKCScenManager.GetScenManager().GetNKCReplayMgr().StartPlaying(replayData);
					}
				}
			}
			else
			{
				Log.Error("ReplayData url request error: " + url, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletAsyncReady.cs", 1135);
			}
		}
		NKMPopUpBox.CloseWaitBox();
	}

	public override void OnBackButton()
	{
		if (m_gameType == NKM_GAME_TYPE.NGT_PVE_SIMULATED)
		{
			base.OnBackButton();
			return;
		}
		base.OnBackButton();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB.NGLT_ASYNC);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_LOBBY);
	}

	private float CalculateDeckAvgSummonCost(NKMAsyncDeckData asyncDeckData)
	{
		int num = 0;
		int num2 = 0;
		if (asyncDeckData == null)
		{
			return 0f;
		}
		for (int i = 0; i < 8; i++)
		{
			if (i < asyncDeckData.units.Count)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(asyncDeckData.units[i].unitId);
				if (unitStatTemplet != null)
				{
					num2++;
					num += unitStatTemplet.GetRespawnCost(bLeader: false, null, null);
				}
			}
		}
		if (num2 == 0)
		{
			return 0f;
		}
		return ((float)num - 1f) / (float)num2;
	}

	private void Update()
	{
		if (m_fPrevUpdateTime + 1f < Time.time)
		{
			m_fPrevUpdateTime = Time.time;
			UpdateRankPVPPointUI();
		}
	}
}
