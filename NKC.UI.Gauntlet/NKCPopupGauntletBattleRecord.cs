using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Logging;
using NKC.UI.Collection;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletBattleRecord : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_POPUP_BATTLE_RECORD";

	private NKCUIManager.LoadedUIData m_loadedUIData;

	public NKCUIComStateButton m_csbtnClose;

	public Animator m_amtContents;

	public GameObject m_objContents;

	public Transform m_trBattleRecordSlotRoot;

	public NKCUIComToggleGroup m_ctglGroupBRSlot;

	public LoopScrollRect m_LoopScrollRect;

	public Image m_imgModeBG;

	public Text m_lbMode;

	[Header("내정보")]
	public NKCUIGauntletBattleRecordInfo m_myBattleRecordInfo;

	[Header("상대정보")]
	public NKCUIGauntletBattleRecordInfo m_enemyBattleRecordInfo;

	[Header("")]
	public GameObject m_objNoHistory;

	public PvpHistoryList m_arrangedPvpHistoryList;

	public int m_SelectedIndex;

	private PvpSingleHistory m_selectedPvpSingleHistory;

	private bool m_bFirstInitLoopScroll = true;

	public NKCUIComStateButton m_playReplayDataButton;

	public NKCUIComStateButton m_csbtnDeckCopy;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override List<int> UpsideMenuShowResourceList => new List<int> { 5, 101 };

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET_BATTLE_RECORD;

	public static NKCPopupGauntletBattleRecord OpenInstance()
	{
		NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupGauntletBattleRecord>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_POPUP_BATTLE_RECORD", NKCUIManager.eUIBaseRect.UIFrontCommon, null);
		NKCPopupGauntletBattleRecord instance = loadedUIData.GetInstance<NKCPopupGauntletBattleRecord>();
		if (instance != null)
		{
			instance.InitUI(loadedUIData);
		}
		return instance;
	}

	public override void OnCloseInstance()
	{
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.ClearCells();
		}
	}

	public void CloseInstance()
	{
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.ClearCells();
		}
		NKCUICollectionShipInfo.CheckInstanceAndClose();
		NKCUICollectionUnitInfo.CheckInstanceAndClose();
		NKCUICollectionOperatorInfo.CheckInstanceAndClose();
		int num = NKCAssetResourceManager.CloseResource("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_POPUP_BATTLE_RECORD");
		Log.Debug($"gauntlet battle record close resource retval is {num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCPopupGauntletBattleRecord.cs", 49);
		Close();
		if (m_loadedUIData != null)
		{
			m_loadedUIData.CloseInstance();
		}
	}

	public void InitUI(NKCUIManager.LoadedUIData loadedUIData)
	{
		m_loadedUIData = loadedUIData;
		m_csbtnClose.PointerClick.RemoveAllListeners();
		m_csbtnClose.PointerClick.AddListener(base.Close);
		m_LoopScrollRect.dOnGetObject += GetSlot;
		m_LoopScrollRect.dOnReturnObject += ReturnSlot;
		m_LoopScrollRect.dOnProvideData += ProvideData;
		m_LoopScrollRect.ContentConstraintCount = 1;
		NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		m_myBattleRecordInfo.Init();
		m_enemyBattleRecordInfo.Init();
		if (m_playReplayDataButton != null)
		{
			m_playReplayDataButton.enabled = true;
			m_playReplayDataButton.PointerClick.RemoveAllListeners();
			m_playReplayDataButton.PointerClick.AddListener(OnPlayReplayData);
		}
		NKCUtil.SetBindFunction(m_csbtnDeckCopy, OnClickDeckCopy);
		NKCUtil.SetHotkey(m_csbtnDeckCopy, HotkeyEventType.Copy);
		NKCUtil.SetGameobjectActive(m_playReplayDataButton, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKM_GAME_TYPE pvpGameType)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null && myUserData.m_PvpData != null)
		{
			m_arrangedPvpHistoryList = null;
			switch (pvpGameType)
			{
			case NKM_GAME_TYPE.NGT_ASYNC_PVP:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
				m_arrangedPvpHistoryList = myUserData.m_AsyncPvpHistory;
				break;
			case NKM_GAME_TYPE.NGT_PVP_RANK:
				m_arrangedPvpHistoryList = myUserData.m_SyncPvpHistory;
				break;
			case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
			case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
				m_arrangedPvpHistoryList = myUserData.m_LeaguePvpHistory;
				m_arrangedPvpHistoryList.FilterByGameType(pvpGameType);
				break;
			case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
				m_arrangedPvpHistoryList = myUserData.m_PrivatePvpHistory;
				break;
			case NKM_GAME_TYPE.NGT_PVP_EVENT:
				m_arrangedPvpHistoryList = myUserData.m_EventPvpHistory;
				break;
			default:
				m_arrangedPvpHistoryList = myUserData.m_SyncPvpHistory;
				break;
			}
			bool uI = true;
			if (m_arrangedPvpHistoryList == null)
			{
				uI = false;
			}
			else if (m_arrangedPvpHistoryList.GetCount() <= 0)
			{
				uI = false;
			}
			NKCUtil.SetGameobjectActive(m_csbtnDeckCopy, NKMOpenTagManager.IsOpened("COPY_SQUAD"));
			UIOpened();
			SetUI(uI);
		}
	}

	public void OnClickSlot(int index)
	{
		if (m_arrangedPvpHistoryList != null && m_arrangedPvpHistoryList.GetCount() > index)
		{
			m_SelectedIndex = index;
			SetDetailRecord(m_arrangedPvpHistoryList.GetData(index));
		}
	}

	public RectTransform GetSlot(int index)
	{
		NKCPopupGauntletBRSlot newInstance = NKCPopupGauntletBRSlot.GetNewInstance(m_trBattleRecordSlotRoot, OnClickSlot);
		if (newInstance == null)
		{
			return null;
		}
		if (newInstance.m_ctglBRSlot != null)
		{
			newInstance.m_ctglBRSlot.SetToggleGroup(m_ctglGroupBRSlot);
		}
		return newInstance.GetComponent<RectTransform>();
	}

	public void ReturnSlot(Transform tr)
	{
		NKCPopupGauntletBRSlot component = tr.GetComponent<NKCPopupGauntletBRSlot>();
		tr.SetParent(base.transform);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideData(Transform tr, int index)
	{
		NKCPopupGauntletBRSlot component = tr.GetComponent<NKCPopupGauntletBRSlot>();
		if (!(component != null) || m_arrangedPvpHistoryList == null || m_arrangedPvpHistoryList.GetCount() <= 0 || m_arrangedPvpHistoryList.GetCount() <= index)
		{
			return;
		}
		PvpSingleHistory data = m_arrangedPvpHistoryList.GetData(index);
		if (data != null)
		{
			component.SetUI(data, index);
		}
		if (component.m_ctglBRSlot != null)
		{
			if (m_SelectedIndex == index)
			{
				component.m_ctglBRSlot.Select(bSelect: true, bForce: true);
			}
			else
			{
				component.m_ctglBRSlot.Select(bSelect: false, bForce: true);
			}
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void SetDetailRecord(PvpSingleHistory cPvpSingleHistory)
	{
		m_selectedPvpSingleHistory = null;
		if (cPvpSingleHistory == null)
		{
			return;
		}
		m_selectedPvpSingleHistory = cPvpSingleHistory;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			m_amtContents.Play("NKM_UI_GAUNTLET_VSRECORD_INTRO");
			DateTime nowUTC = new DateTime(m_selectedPvpSingleHistory.RegdateTick);
			NKM_GAME_TYPE gameType = m_selectedPvpSingleHistory.GameType;
			int seasonId = NKCPVPManager.FindPvPSeasonID(gameType, nowUTC);
			m_myBattleRecordInfo.SetLeagueInfo(gameType, seasonId, m_selectedPvpSingleHistory.MyTier);
			m_enemyBattleRecordInfo.SetLeagueInfo(gameType, seasonId, m_selectedPvpSingleHistory.TargetTier);
			bool flag = false;
			switch (m_selectedPvpSingleHistory.GameType)
			{
			case NKM_GAME_TYPE.NGT_PVP_RANK:
				m_imgModeBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_RANK");
				NKCUtil.SetLabelText(m_lbMode, NKCUtilString.GET_STRING_GAUNTLET_RANK_GAME);
				NKCUtil.SetLabelTextColor(m_lbMode, NKCUtil.GetColor("#FFFFFFFF"));
				flag = true;
				break;
			case NKM_GAME_TYPE.NGT_ASYNC_PVP:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
			case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
				m_imgModeBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_ASYNCMAYCH");
				NKCUtil.SetLabelText(m_lbMode, NKCUtilString.GET_STRING_GAUNTLET_ASYNC_GAME);
				NKCUtil.SetLabelTextColor(m_lbMode, NKCUtil.GetColor("#FFFFFFFF"));
				flag = true;
				break;
			case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
				m_imgModeBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_LEAGUE");
				NKCUtil.SetLabelText(m_lbMode, NKCUtilString.GET_STRING_GAUNTLET_LEAGUE_TITLE);
				NKCUtil.SetLabelTextColor(m_lbMode, NKCUtil.GetColor("#FFFFFFFF"));
				flag = true;
				break;
			case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
				m_imgModeBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_LEAGUE");
				NKCUtil.SetLabelText(m_lbMode, NKCUtilString.GET_STRING_GAUNTLET_UNLIMITED_TITLE);
				NKCUtil.SetLabelTextColor(m_lbMode, NKCUtil.GetColor("#FFFFFFFF"));
				flag = true;
				break;
			case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
				m_imgModeBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_NORMAL");
				NKCUtil.SetLabelText(m_lbMode, NKCUtilString.GET_STRING_PRIVATE_PVP);
				NKCUtil.SetLabelTextColor(m_lbMode, NKCUtil.GetColor("#FFFFFFFF"));
				flag = false;
				break;
			case NKM_GAME_TYPE.NGT_PVP_EVENT:
				m_imgModeBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_GAUNTLET_SPRITE", "AB_UI_NKM_UI_GAUNTLET_ELLIPSE_NORMAL");
				NKCUtil.SetLabelText(m_lbMode, NKCUtilString.GET_STRING_GAUNTLET_EVENT_GAME);
				NKCUtil.SetLabelTextColor(m_lbMode, NKCUtil.GetColor("#FFFFFFFF"));
				flag = false;
				break;
			default:
				flag = false;
				break;
			}
			m_myBattleRecordInfo.SetUserInfo(m_selectedPvpSingleHistory.MyUserLevel, myUserData.m_UserNickName, myUserData.m_FriendCode, m_selectedPvpSingleHistory.MyScore, myUserData.UserProfileData.commonProfile.titleId, m_selectedPvpSingleHistory.SourceGuildUid, m_selectedPvpSingleHistory.SourceGuildBadgeId, m_selectedPvpSingleHistory.SourceGuildName, bOpponent: false, flag);
			m_enemyBattleRecordInfo.SetUserInfo(m_selectedPvpSingleHistory.TargetUserLevel, m_selectedPvpSingleHistory.TargetNickName, m_selectedPvpSingleHistory.TargetFriendCode, m_selectedPvpSingleHistory.TargetScore, m_selectedPvpSingleHistory.TargetTitleId, m_selectedPvpSingleHistory.TargetGuildUid, m_selectedPvpSingleHistory.TargetGuildBadgeId, m_selectedPvpSingleHistory.TargetGuildName, bOpponent: true, flag);
			PVP_RESULT pvpResult = PVP_RESULT.LOSE;
			switch (m_selectedPvpSingleHistory.Result)
			{
			case PVP_RESULT.WIN:
				pvpResult = PVP_RESULT.LOSE;
				break;
			case PVP_RESULT.LOSE:
				pvpResult = PVP_RESULT.WIN;
				break;
			case PVP_RESULT.DRAW:
				pvpResult = PVP_RESULT.DRAW;
				break;
			}
			m_myBattleRecordInfo.SetPvpResult(m_selectedPvpSingleHistory.Result);
			m_enemyBattleRecordInfo.SetPvpResult(pvpResult);
			bool bDraftBanMode = false;
			if (gameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
			{
				bDraftBanMode = true;
			}
			else if ((m_selectedPvpSingleHistory.myBanUnitIds.Count > 0 && m_selectedPvpSingleHistory.myBanUnitIds[0] != 0) || (m_selectedPvpSingleHistory.targetBanUnitIds.Count > 0 && m_selectedPvpSingleHistory.targetBanUnitIds[0] != 0))
			{
				bDraftBanMode = true;
			}
			m_myBattleRecordInfo.SetDeckInfo(m_selectedPvpSingleHistory.MyDeckData, m_selectedPvpSingleHistory.myBanUnitIds, m_selectedPvpSingleHistory.myBanShipIds, bDraftBanMode);
			m_enemyBattleRecordInfo.SetDeckInfo(m_selectedPvpSingleHistory.TargetDeckData, m_selectedPvpSingleHistory.targetBanUnitIds, m_selectedPvpSingleHistory.targetBanShipIds, bDraftBanMode);
			NKCUtil.SetGameobjectActive(m_playReplayDataButton, CheckForReplayData());
		}
	}

	private void SetUI(bool bExistHistory)
	{
		if (bExistHistory)
		{
			if (m_bFirstInitLoopScroll)
			{
				m_LoopScrollRect.PrepareCells();
			}
			m_bFirstInitLoopScroll = false;
			NKCUtil.SetGameobjectActive(m_objNoHistory, bValue: false);
			NKCUtil.SetGameobjectActive(m_objContents, bValue: true);
			if (m_arrangedPvpHistoryList != null && m_arrangedPvpHistoryList.GetCount() > 0)
			{
				m_SelectedIndex = 0;
				m_arrangedPvpHistoryList.Sort();
				m_LoopScrollRect.TotalCount = m_arrangedPvpHistoryList.GetCount();
				m_LoopScrollRect.velocity = new Vector2(0f, 0f);
				m_LoopScrollRect.SetIndexPosition(0);
				SetDetailRecord(m_arrangedPvpHistoryList.GetData(m_SelectedIndex));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objNoHistory, bValue: true);
			NKCUtil.SetGameobjectActive(m_objContents, bValue: false);
		}
	}

	public void OnPlayReplayData()
	{
		if (m_selectedPvpSingleHistory != null)
		{
			NKCReplayMgr.GetNKCReplaMgr().StartPlaying(m_selectedPvpSingleHistory.gameUid);
		}
	}

	public bool CheckForReplayData()
	{
		if (m_selectedPvpSingleHistory == null)
		{
			return false;
		}
		if (!NKCReplayMgr.IsReplayOpened())
		{
			return false;
		}
		if (m_selectedPvpSingleHistory.GameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC || m_selectedPvpSingleHistory.GameType == NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE)
		{
			return false;
		}
		if (NKCReplayMgr.GetNKCReplaMgr().IsInReplayDataFileList(m_selectedPvpSingleHistory.gameUid))
		{
			return true;
		}
		return false;
	}

	public void OnClickDeckCopy()
	{
		if (m_selectedPvpSingleHistory.TargetDeckData == null)
		{
			return;
		}
		int shipID = ((m_selectedPvpSingleHistory.TargetDeckData.ship != null) ? m_selectedPvpSingleHistory.TargetDeckData.ship.unitId : 0);
		int operID = ((m_selectedPvpSingleHistory.TargetDeckData.operatorUnit != null) ? m_selectedPvpSingleHistory.TargetDeckData.operatorUnit.id : 0);
		List<int> list = new List<int>();
		for (int i = 0; i < 8; i++)
		{
			NKMAsyncUnitData nKMAsyncUnitData = m_selectedPvpSingleHistory.TargetDeckData.units[i];
			if (nKMAsyncUnitData == null)
			{
				list.Add(0);
			}
			else
			{
				list.Add(nKMAsyncUnitData.unitId);
			}
		}
		int leaderIndex = m_selectedPvpSingleHistory.TargetDeckData.leaderIndex;
		NKCPopupDeckCopy.MakeDeckCopyCode(shipID, operID, list, leaderIndex);
	}
}
