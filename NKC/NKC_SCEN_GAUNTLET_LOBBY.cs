using System;
using System.Collections.Generic;
using ClientPacket.Pvp;
using ClientPacket.User;
using NKC.UI;
using NKC.UI.Gauntlet;
using NKM;
using UnityEngine.Video;

namespace NKC;

public class NKC_SCEN_GAUNTLET_LOBBY : NKC_SCEN_BASIC
{
	private NKCAssetResourceData m_UILoadResourceData;

	private NKCUIGauntletLobby m_NKCUIGauntletLobby;

	private NKC_GAUNTLET_LOBBY_TAB m_NKC_GAUNTLET_LOBBY_TAB;

	private NKCPopupGauntletBattleRecord m_NKCPopupGauntletBattleRecord;

	private RANK_TYPE m_Latest_RANK_TYPE;

	private NKM_ERROR_CODE m_Reserved_NKM_ERROR_CODE;

	private NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE m_lastest_AsyncTab = NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE.MAX;

	public static float AsyncRefreshCooltime;

	public List<AsyncPvpTarget> AsyncTargetList = new List<AsyncPvpTarget>();

	private long asyncUserUID;

	private bool m_bWaitForEmoticon;

	private NKCPopupGauntletOutgameReward m_NKCPopupGauntletOutgameReward;

	private NKCPopupGauntletNewSeasonAlarm m_NKCPopupGauntletNewSeasonAlarm;

	private DateTime m_lastPVPChargePacketSendTime;

	private DateTime m_lastPVPPracticeChargePacketSendTime;

	private const long CHARGE_POINT_REFRESH_PACKET_INTERVAL_TICK = 600000000L;

	private int m_iNpcNewOpenTierSlot;

	public NKCUIGauntletLobby GauntletLobby => m_NKCUIGauntletLobby;

	public NKCUIGauntletLobbyCustom GauntletLobbyCustom => m_NKCUIGauntletLobby.m_NKCUIGauntletLobbyCustom;

	public NKCPopupGauntletOutgameReward NKCPopupGauntletOutgameReward
	{
		get
		{
			if (m_NKCPopupGauntletOutgameReward == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupGauntletOutgameReward>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_RANK_REWARD_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), delegate
				{
					m_NKCPopupGauntletOutgameReward = null;
				});
				m_NKCPopupGauntletOutgameReward = loadedUIData.GetInstance<NKCPopupGauntletOutgameReward>();
				m_NKCPopupGauntletOutgameReward?.InitUI();
			}
			return m_NKCPopupGauntletOutgameReward;
		}
	}

	public NKCPopupGauntletNewSeasonAlarm NKCPopupGauntletNewSeasonAlarm
	{
		get
		{
			if (m_NKCPopupGauntletNewSeasonAlarm == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupGauntletNewSeasonAlarm>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_RANK_NEWSEASON_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), delegate
				{
					m_NKCPopupGauntletNewSeasonAlarm = null;
				});
				m_NKCPopupGauntletNewSeasonAlarm = loadedUIData.GetInstance<NKCPopupGauntletNewSeasonAlarm>();
				m_NKCPopupGauntletNewSeasonAlarm?.InitUI();
			}
			return m_NKCPopupGauntletNewSeasonAlarm;
		}
	}

	public void SetReserved_NKM_ERROR_CODE(NKM_ERROR_CODE eNKM_ERROR_CODE)
	{
		m_Reserved_NKM_ERROR_CODE = eNKM_ERROR_CODE;
	}

	private void CheckNKCPopupGauntletOutgameRewardAndClose()
	{
		if (m_NKCPopupGauntletOutgameReward != null && m_NKCPopupGauntletOutgameReward.IsOpen)
		{
			m_NKCPopupGauntletOutgameReward.Close();
		}
	}

	private void CheckNKCPopupGauntletNewSeasonAlarmAndClose()
	{
		if (m_NKCPopupGauntletNewSeasonAlarm != null && m_NKCPopupGauntletNewSeasonAlarm.IsOpen)
		{
			m_NKCPopupGauntletNewSeasonAlarm.Close();
		}
	}

	public NKC_SCEN_GAUNTLET_LOBBY()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAUNTLET_LOBBY;
	}

	public void SetLatestRANK_TYPE(RANK_TYPE eRANK_TYPE)
	{
		m_Latest_RANK_TYPE = eRANK_TYPE;
	}

	public void SetReservedLobbyTab(NKC_GAUNTLET_LOBBY_TAB eNKC_GAUNTLET_LOBBY_TAB)
	{
		m_NKC_GAUNTLET_LOBBY_TAB = eNKC_GAUNTLET_LOBBY_TAB;
	}

	public void SetReservedAsyncTab(NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE eAsyncTab = NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE.MAX)
	{
		m_lastest_AsyncTab = eAsyncTab;
	}

	public RANK_TYPE GetCurrRankType()
	{
		if (m_NKCUIGauntletLobby == null)
		{
			return RANK_TYPE.COUNT;
		}
		return m_NKCUIGauntletLobby.GetCurrRankType();
	}

	public NKC_GAUNTLET_LOBBY_TAB GetCurrentLobbyTab()
	{
		if (m_NKCUIGauntletLobby == null)
		{
			return NKC_GAUNTLET_LOBBY_TAB.NGLT_RANK;
		}
		return m_NKCUIGauntletLobby.GetCurrentLobbyTab();
	}

	public void DoAfterLogout()
	{
		SetLatestRANK_TYPE(RANK_TYPE.MY_LEAGUE);
		m_Reserved_NKM_ERROR_CODE = NKM_ERROR_CODE.NEC_OK;
		m_bWaitForEmoticon = false;
	}

	public bool IsWaitForEmoticon()
	{
		return m_bWaitForEmoticon;
	}

	public void SetWaitForEmoticon(bool bValue)
	{
		m_bWaitForEmoticon = bValue;
	}

	public void OnLoginSuccess()
	{
		NKCUIGauntletLobbyRank.SetAlertDemotion(bSet: false);
		NKCUIGauntletLobbyLeague.SetAlertDemotion(bSet: false);
		if (asyncUserUID != NKCScenManager.CurrentUserData().m_UserUID)
		{
			AsyncRefreshCooltime = 0f;
			AsyncTargetList.Clear();
			asyncUserUID = NKCScenManager.CurrentUserData().m_UserUID;
		}
	}

	public void ClearCacheData()
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.CloseInstance();
			m_NKCUIGauntletLobby = null;
		}
		if (m_NKCPopupGauntletBattleRecord != null)
		{
			m_NKCPopupGauntletBattleRecord.CloseInstance();
			m_NKCPopupGauntletBattleRecord = null;
		}
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		NKCCollectionManager.Init();
		if (m_NKCUIGauntletLobby == null)
		{
			m_UILoadResourceData = NKCUIGauntletLobby.OpenInstanceAsync();
		}
		else
		{
			m_UILoadResourceData = null;
		}
	}

	public override void ScenLoadUpdate()
	{
		if (!NKCAssetResourceManager.IsLoadEnd())
		{
			return;
		}
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null && subUICameraVideoPlayer.IsPreparing())
		{
			return;
		}
		if (m_NKCUIGauntletLobby == null && m_UILoadResourceData != null)
		{
			if (!NKCUIGauntletLobby.CheckInstanceLoaded(m_UILoadResourceData, out m_NKCUIGauntletLobby))
			{
				return;
			}
			m_UILoadResourceData = null;
		}
		ScenLoadLastStart();
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.InitUI();
		}
		SetBG();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: false);
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.m_NKCUIGauntletLobbyAsyncV2?.SetReserveOpenNpcBotTier(m_iNpcNewOpenTierSlot);
			m_NKCUIGauntletLobby.Open(m_NKC_GAUNTLET_LOBBY_TAB, m_Latest_RANK_TYPE, m_lastest_AsyncTab);
			NKCContentManager.SetUnlockedContent();
			NKCContentManager.ShowContentUnlockPopup(null);
		}
		if (m_Reserved_NKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(m_Reserved_NKM_ERROR_CODE);
			m_Reserved_NKM_ERROR_CODE = NKM_ERROR_CODE.NEC_OK;
		}
	}

	private void SetBG()
	{
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
			subUICameraVideoPlayer.m_fMoviePlaySpeed = 1f;
			subUICameraVideoPlayer.SetAlpha(0.6f);
			subUICameraVideoPlayer.Play("Gauntlet_BG.mp4", bLoop: true);
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.Close();
		}
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
		if (m_NKCPopupGauntletBattleRecord != null)
		{
			m_NKCPopupGauntletBattleRecord.CloseInstance();
			m_NKCPopupGauntletBattleRecord = null;
		}
		m_lastest_AsyncTab = NKCUIGauntletLobbyAsyncV2.PVP_ASYNC_TYPE.MAX;
		m_iNpcNewOpenTierSlot = 0;
		CheckNKCPopupGauntletOutgameRewardAndClose();
	}

	public void OpenBattleRecord(NKM_GAME_TYPE pvpGameType)
	{
		if (m_NKCPopupGauntletBattleRecord == null)
		{
			m_NKCPopupGauntletBattleRecord = NKCPopupGauntletBattleRecord.OpenInstance();
		}
		m_NKCPopupGauntletBattleRecord?.Open(pvpGameType);
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void ProcessPVPPointCharge(int itemID = 6)
	{
		switch (itemID)
		{
		case 6:
			if (NKCSynchronizedTime.IsFinished(new DateTime(m_lastPVPChargePacketSendTime.Ticks + 600000000)))
			{
				long countMiscItem2 = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(6);
				int cHARGE_POINT_MAX_COUNT = NKMPvpCommonConst.Instance.CHARGE_POINT_MAX_COUNT;
				if (countMiscItem2 < cHARGE_POINT_MAX_COUNT && NKCSynchronizedTime.IsFinished(new DateTime(new DateTime(NKCPVPManager.GetLastUpdateChargePointTicks()).Ticks + NKMPvpCommonConst.Instance.CHARGE_POINT_REFRESH_INTERVAL_TICKS)) && !NKMPopUpBox.IsOpenedWaitBox())
				{
					m_lastPVPChargePacketSendTime = NKCSynchronizedTime.GetServerUTCTime();
					NKCPacketSender.Send_NKMPacket_PVP_CHARGE_POINT_REFRESH_REQ(itemID);
				}
			}
			break;
		case 9:
			if (NKCSynchronizedTime.IsFinished(m_lastPVPPracticeChargePacketSendTime.Ticks + 600000000))
			{
				long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(9);
				int cHARGE_POINT_MAX_COUNT_FOR_PRACTICE = NKMPvpCommonConst.Instance.CHARGE_POINT_MAX_COUNT_FOR_PRACTICE;
				if (countMiscItem < cHARGE_POINT_MAX_COUNT_FOR_PRACTICE && NKCSynchronizedTime.IsFinished(NKMTime.GetNextResetTime(new DateTime(NKCPVPManager.GetLastUpdateChargePointTicks()), NKMTime.TimePeriod.Day)) && !NKMPopUpBox.IsOpenedWaitBox())
				{
					m_lastPVPPracticeChargePacketSendTime = NKCSynchronizedTime.GetServerUTCTime();
					NKCPacketSender.Send_NKMPacket_PVP_CHARGE_POINT_REFRESH_REQ(itemID);
				}
			}
			break;
		}
	}

	public void OnRecv(NKMPacket_PVP_RANK_LIST_ACK cNKMPacket_PVP_RANK_LIST_ACK)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(cNKMPacket_PVP_RANK_LIST_ACK);
		}
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_RANK_LIST_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK);
		}
	}

	public void OnRecv(NKMPacket_PVP_RANK_WEEK_REWARD_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().NKCPopupGauntletOutgameReward.Open(bWeeklyReward: true, sPacket.rewardData, bRank: true, sPacket.isScoreChanged);
	}

	public void OnRecv(NKMPacket_PVP_RANK_SEASON_REWARD_ACK cNKMPacket_PVP_RANK_SEASON_REWARD_ACK)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK);
		}
		if (cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.rewardData != null)
		{
			NKCPopupGauntletOutgameReward?.Open(bWeeklyReward: false, cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.rewardData, bRank: true, cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.isScoreChanged);
			NKCPopupGauntletOutgameReward?.SetRankRewardData(cNKMPacket_PVP_RANK_SEASON_REWARD_ACK.rankRewardData);
		}
		else
		{
			NKCPopupGauntletNewSeasonAlarm?.Open(bRank: true);
		}
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_RANK_SEASON_REWARD_ACK packet)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(packet);
		}
		if (packet.rewardData != null)
		{
			NKCPopupGauntletOutgameReward?.Open(bWeeklyReward: false, packet.rewardData, bRank: false);
		}
		else
		{
			NKCPopupGauntletNewSeasonAlarm?.Open(bRank: false);
		}
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_WEEKLY_REWARD_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
		if (sPacket.rewardData != null)
		{
			NKCPopupGauntletOutgameReward?.OpenForLeague(sPacket.rewardData);
		}
	}

	public void OnRecv(NKMPacket_LEAGUE_PVP_SEASON_REWARD_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
		if (sPacket.rewardData != null)
		{
			NKCPopupGauntletOutgameReward?.OpenForLeague(sPacket.rewardData);
		}
		else
		{
			NKCPopupGauntletNewSeasonAlarm?.OpenForLeague();
		}
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_TARGET_LIST_ACK packet)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(packet);
		}
	}

	public void OnRecv(NKMPacket_ASYNC_PVP_RANK_LIST_ACK packet)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(packet);
		}
	}

	public void OnRecv(NKMPacket_REVENGE_PVP_TARGET_LIST_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_NPC_PVP_TARGET_LIST_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_UPDATE_DEFENCE_DECK_ACK packet)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(packet);
		}
	}

	public void OnRecvEventPvpSeasonInfo()
	{
		m_NKCUIGauntletLobby?.OnRecvEventPvpSeasonInfo();
	}

	public void OnRecvEventPvpReward()
	{
		m_NKCUIGauntletLobby?.OnRecvEventPvpReward();
	}

	public void SetAsyncTargetList(List<AsyncPvpTarget> newlist)
	{
		AsyncTargetList.Clear();
		AsyncTargetList.AddRange(newlist.FindAll((AsyncPvpTarget v) => InvalidTarget(v)));
	}

	public void SetTargetData(AsyncPvpTarget refreshedTargetData)
	{
		for (int i = 0; i < AsyncTargetList.Count; i++)
		{
			if (AsyncTargetList[i].userFriendCode == refreshedTargetData.userFriendCode)
			{
				AsyncTargetList[i] = refreshedTargetData;
				break;
			}
		}
	}

	private bool InvalidTarget(AsyncPvpTarget target)
	{
		if (target == null)
		{
			return false;
		}
		if (target.asyncDeck == null)
		{
			return false;
		}
		if (target.asyncDeck.ship == null)
		{
			return false;
		}
		if (target.asyncDeck.units == null)
		{
			return false;
		}
		if (target.asyncDeck.units.Count != 8)
		{
			return false;
		}
		return true;
	}

	public void SetReserveOpenNpcBotTier(int iNewOpenTier)
	{
		m_iNpcNewOpenTierSlot = iNewOpenTier;
	}

	public void OnRecv(NKMPacket_PVP_CASTING_VOTE_UNIT_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_PVP_CASTING_VOTE_SHIP_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_PVP_CASTING_VOTE_OPERATOR_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_JUKEBOX_CHANGE_BGM_ACK sPacket)
	{
		if (m_NKCUIGauntletLobby != null)
		{
			m_NKCUIGauntletLobby.OnRecv(sPacket);
		}
	}
}
