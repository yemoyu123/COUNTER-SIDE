using System.Collections.Generic;
using System.Linq;
using ClientPacket.Guild;
using Cs.Core.Util;
using NKC.PacketHandler;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_DUNGEON_ATK_READY : NKC_SCEN_BASIC
{
	private NKMStageTempletV2 m_StageTemplet;

	private NKMDungeonTempletBase m_DungeonTempletBase;

	private int m_eventDeckIndex;

	private DeckContents m_deckContents;

	private string m_BGMName = "";

	private GameObject m_NUM_OPERATION;

	private GameObject m_objNUM_OPERATION_BG;

	private NKMTrackingFloat m_BloomIntensity = new NKMTrackingFloat();

	private NKMDeckIndex m_LastDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE, 0);

	private NKM_DECK_TYPE m_UsedDeckType = NKM_DECK_TYPE.NDT_DAILY;

	private NKMDeckIndex m_SelectedDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE, 0);

	private NKMEventDeckData m_SelectedEventDeck;

	private int m_LastMultiplyRewardCount = 1;

	private bool m_bOperationSkip;

	private NKM_SHORTCUT_TYPE m_ShortcutType;

	private string m_ShortcutParam = "";

	private long m_lSupportUserUID;

	public DeckContents SavedDeckContents => m_deckContents;

	public NKMDeckIndex GetLastDeckIndex()
	{
		return m_LastDeckIndex;
	}

	public NKMEventDeckData GetLastEventDeck()
	{
		return m_SelectedEventDeck;
	}

	public int GetLastMultiplyRewardCount()
	{
		return m_LastMultiplyRewardCount;
	}

	public long GetLastSupportUserUID()
	{
		return m_lSupportUserUID;
	}

	public NKC_SCEN_DUNGEON_ATK_READY()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_DUNGEON_ATK_READY;
	}

	public void DoAfterLogout()
	{
		m_LastDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE, 0);
		m_SelectedDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE, 0);
		m_SelectedEventDeck = null;
		m_lSupportUserUID = 0L;
		m_LastMultiplyRewardCount = 1;
		m_bOperationSkip = false;
		m_StageTemplet = null;
		m_eventDeckIndex = 0;
		m_deckContents = DeckContents.NORMAL;
		m_BGMName = "";
		m_ShortcutType = NKM_SHORTCUT_TYPE.SHORTCUT_NONE;
		m_ShortcutParam = "";
	}

	public int GetEpisodeID()
	{
		if (m_StageTemplet == null)
		{
			return 0;
		}
		return m_StageTemplet.EpisodeId;
	}

	public EPISODE_DIFFICULTY GetEpisodeDifficulty()
	{
		if (m_StageTemplet == null)
		{
			return EPISODE_DIFFICULTY.NORMAL;
		}
		return m_StageTemplet.m_Difficulty;
	}

	public int GetActID()
	{
		if (m_StageTemplet == null)
		{
			return 0;
		}
		return m_StageTemplet.ActId;
	}

	public int GetStageIndex()
	{
		if (m_StageTemplet == null)
		{
			return 0;
		}
		return m_StageTemplet.m_StageIndex;
	}

	public int GetStageUIIndex()
	{
		if (m_StageTemplet == null)
		{
			return 0;
		}
		return m_StageTemplet.m_StageUINum;
	}

	public NKMStageTempletV2 GetStageTemplet()
	{
		return m_StageTemplet;
	}

	public NKMDungeonTempletBase GetDungeonTempletBase()
	{
		return m_DungeonTempletBase;
	}

	public void SetBackButtonShortcut(NKM_SHORTCUT_TYPE shortcutType, string shortcutParam)
	{
		m_ShortcutType = shortcutType;
		m_ShortcutParam = shortcutParam;
	}

	public void SetDungeonInfo(NKMDungeonTempletBase dungeonTempletBase, DeckContents eventDeckContents = DeckContents.NORMAL)
	{
		m_DungeonTempletBase = dungeonTempletBase;
		m_StageTemplet = dungeonTempletBase.StageTemplet;
		m_deckContents = eventDeckContents;
		m_BGMName = "";
		m_UsedDeckType = NKM_DECK_TYPE.NDT_DAILY;
		m_eventDeckIndex = dungeonTempletBase.m_UseEventDeck;
	}

	public void SetDungeonInfo(NKMStageTempletV2 stageTemplet, DeckContents eventDeckContents = DeckContents.NORMAL)
	{
		m_StageTemplet = stageTemplet;
		m_deckContents = eventDeckContents;
		m_BGMName = "";
		m_DungeonTempletBase = stageTemplet.DungeonTempletBase;
		m_UsedDeckType = NKM_DECK_TYPE.NDT_DAILY;
		m_eventDeckIndex = stageTemplet.GetEventDeckID();
	}

	public void SetReservedBGM(string bgmName)
	{
		m_BGMName = bgmName;
	}

	private void OnClickStartCommomProcess(bool bEventDeck, NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (!NKMEpisodeMgr.HasEnoughResource(stageTemplet, m_LastMultiplyRewardCount))
		{
			return;
		}
		if (m_LastMultiplyRewardCount > 1 || m_bOperationSkip)
		{
			if (!m_bOperationSkip)
			{
				NKMRewardMultiplyTemplet.RewardMultiplyItem costItem = NKMRewardMultiplyTemplet.GetCostItem(NKMRewardMultiplyTemplet.ScopeType.General);
				if (!myUserData.CheckPrice(costItem.MiscItemCount * (m_LastMultiplyRewardCount - 1), costItem.MiscItemId))
				{
					NKCShopManager.OpenItemLackPopup(costItem.MiscItemId, costItem.MiscItemCount * (m_LastMultiplyRewardCount - 1));
					return;
				}
			}
			else if (!myUserData.CheckPrice(NKMCommonConst.SkipCostMiscItemCount * m_LastMultiplyRewardCount, NKMCommonConst.SkipCostMiscItemId))
			{
				NKCShopManager.OpenItemLackPopup(NKMCommonConst.SkipCostMiscItemId, NKMCommonConst.SkipCostMiscItemCount * m_LastMultiplyRewardCount);
				return;
			}
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCUtil.CheckCommonStartCond(myUserData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCUtil.OnExpandInventoryPopup(nKM_ERROR_CODE);
			return;
		}
		if (m_bOperationSkip && stageTemplet.DungeonTempletBase != null)
		{
			if (!myUserData.CheckDungeonClear(stageTemplet.DungeonTempletBase.m_DungeonID))
			{
				NKCPopupOKCancel.OpenOKBox(NKCStringTable.GetString("SI_DP_NOTICE"), NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_STAGE"));
				return;
			}
			List<long> unitList = new List<long>();
			if (NKCUIPrepareEventDeck.IsInstanceOpen)
			{
				unitList = m_SelectedEventDeck.m_dicUnit.Values.ToList();
			}
			else
			{
				myUserData.m_ArmyData.GetDeckList(m_SelectedDeckIndex.m_eDeckType, m_SelectedDeckIndex.m_iIndex, ref unitList);
			}
			NKCPacketSender.Send_NKMPacket_DUNGEON_SKIP_REQ(stageTemplet.DungeonTempletBase.m_DungeonID, unitList, m_LastMultiplyRewardCount);
			return;
		}
		NKCUICutScenPlayer.CutScenCallBack cutScenCallBack = null;
		int fierceBossID = 0;
		if (m_deckContents == DeckContents.FIERCE_BATTLE_SUPPORT)
		{
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr != null)
			{
				fierceBossID = nKCFierceBattleSupportDataMgr.CurBossID;
			}
		}
		STAGE_TYPE sTAGE_TYPE = stageTemplet.m_STAGE_TYPE;
		if (sTAGE_TYPE == STAGE_TYPE.ST_DUNGEON || sTAGE_TYPE != STAGE_TYPE.ST_PHASE)
		{
			cutScenCallBack = ((!bEventDeck) ? ((NKCUICutScenPlayer.CutScenCallBack)delegate
			{
				NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedDeckIndex.m_iIndex, stageTemplet.Key, 0, stageTemplet.DungeonTempletBase.m_DungeonStrID, 0, bLocal: false, m_LastMultiplyRewardCount, fierceBossID, m_lSupportUserUID);
			}) : ((NKCUICutScenPlayer.CutScenCallBack)delegate
			{
				NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedEventDeck, stageTemplet.Key, 0, stageTemplet.DungeonTempletBase.m_DungeonID, 0, bLocal: false, m_LastMultiplyRewardCount, fierceBossID, m_lSupportUserUID);
			}));
		}
		else
		{
			_ = stageTemplet.PhaseTemplet;
			cutScenCallBack = ((!bEventDeck) ? ((NKCUICutScenPlayer.CutScenCallBack)delegate
			{
				NKCPacketSender.Send_NKMPacket_PHASE_START_REQ(stageTemplet.Key, m_SelectedDeckIndex, m_lSupportUserUID);
			}) : ((NKCUICutScenPlayer.CutScenCallBack)delegate
			{
				NKCPacketSender.Send_NKMPacket_PHASE_START_REQ(stageTemplet.Key, m_SelectedEventDeck, m_lSupportUserUID);
			}));
		}
		bool flag = true;
		if (NKCScenManager.CurrentUserData() != null)
		{
			flag = NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene;
		}
		bool flag2 = false;
		bool isOnGoing = NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing();
		if (!myUserData.CheckStageCleared(stageTemplet) || (flag && !isOnGoing))
		{
			NKCCutScenTemplet stageBeforeCutscen = stageTemplet.GetStageBeforeCutscen();
			if (stageBeforeCutscen != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(stageBeforeCutscen.m_CutScenStrID, cutScenCallBack, stageTemplet.Key);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
				flag2 = true;
			}
		}
		if (!flag2)
		{
			cutScenCallBack();
		}
	}

	private void OnClickStartCommomProcess(bool bEventDeck, NKMDungeonTempletBase dungeonTempletBase)
	{
		if (dungeonTempletBase == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (m_LastMultiplyRewardCount > 1 || m_bOperationSkip)
		{
			if (!m_bOperationSkip)
			{
				NKMRewardMultiplyTemplet.RewardMultiplyItem costItem = NKMRewardMultiplyTemplet.GetCostItem(NKMRewardMultiplyTemplet.ScopeType.General);
				if (!myUserData.CheckPrice(costItem.MiscItemCount * (m_LastMultiplyRewardCount - 1), costItem.MiscItemId))
				{
					NKCShopManager.OpenItemLackPopup(costItem.MiscItemId, costItem.MiscItemCount * (m_LastMultiplyRewardCount - 1));
					return;
				}
			}
			else if (!myUserData.CheckPrice(NKMCommonConst.SkipCostMiscItemCount * m_LastMultiplyRewardCount, NKMCommonConst.SkipCostMiscItemId))
			{
				NKCShopManager.OpenItemLackPopup(NKMCommonConst.SkipCostMiscItemId, NKMCommonConst.SkipCostMiscItemCount * m_LastMultiplyRewardCount);
				return;
			}
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCUtil.CheckCommonStartCond(myUserData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCUtil.OnExpandInventoryPopup(nKM_ERROR_CODE);
			return;
		}
		if (m_bOperationSkip && dungeonTempletBase != null)
		{
			if (!myUserData.CheckDungeonClear(dungeonTempletBase.m_DungeonID))
			{
				NKCPopupOKCancel.OpenOKBox(NKCStringTable.GetString("SI_DP_NOTICE"), NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_STAGE"));
				return;
			}
			List<long> unitList = new List<long>();
			if (NKCUIPrepareEventDeck.IsInstanceOpen)
			{
				unitList = m_SelectedEventDeck.m_dicUnit.Values.ToList();
			}
			else
			{
				myUserData.m_ArmyData.GetDeckList(m_SelectedDeckIndex.m_eDeckType, m_SelectedDeckIndex.m_iIndex, ref unitList);
			}
			NKCPacketSender.Send_NKMPacket_DUNGEON_SKIP_REQ(dungeonTempletBase.m_DungeonID, unitList, m_LastMultiplyRewardCount);
			return;
		}
		NKCUICutScenPlayer.CutScenCallBack cutScenCallBack = null;
		int fierceBossID = 0;
		if (m_deckContents == DeckContents.FIERCE_BATTLE_SUPPORT)
		{
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr != null)
			{
				fierceBossID = nKCFierceBattleSupportDataMgr.CurBossID;
			}
		}
		cutScenCallBack = ((!bEventDeck) ? ((NKCUICutScenPlayer.CutScenCallBack)delegate
		{
			NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedDeckIndex.m_iIndex, 0, 0, dungeonTempletBase.m_DungeonStrID, 0, bLocal: false, m_LastMultiplyRewardCount, fierceBossID, 0L);
		}) : ((NKCUICutScenPlayer.CutScenCallBack)delegate
		{
			NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedEventDeck, 0, 0, dungeonTempletBase.m_DungeonID, 0, bLocal: false, m_LastMultiplyRewardCount, fierceBossID, m_lSupportUserUID);
		}));
		bool flag = true;
		if (NKCScenManager.CurrentUserData() != null)
		{
			flag = NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene;
		}
		bool flag2 = false;
		bool isOnGoing = NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing();
		if (!myUserData.CheckDungeonClear(dungeonTempletBase.m_DungeonID) || (flag && !isOnGoing))
		{
			NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(dungeonTempletBase.m_CutScenStrIDBefore);
			if (cutScenTemple != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(cutScenTemple.m_CutScenStrID, cutScenCallBack, dungeonTempletBase.m_DungeonStrID);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
				flag2 = true;
			}
		}
		if (!flag2)
		{
			cutScenCallBack();
		}
	}

	private void OnClickStartShadowPalace(bool bEventDeck, NKMDungeonTempletBase dungeonTempletBase)
	{
		if (dungeonTempletBase == null)
		{
			return;
		}
		NKMUserData cNKMUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCUtil.CheckCommonStartCond(cNKMUserData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCUtil.OnExpandInventoryPopup(nKM_ERROR_CODE);
			return;
		}
		NKCUICutScenPlayer.CutScenCallBack cutScenCallBack = null;
		cutScenCallBack = ((!bEventDeck) ? ((NKCUICutScenPlayer.CutScenCallBack)delegate
		{
			NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedDeckIndex.m_iIndex, 0, 0, dungeonTempletBase.m_DungeonStrID, cNKMUserData.m_ShadowPalace.currentPalaceId, bLocal: false, 1, 0, 0L);
		}) : ((NKCUICutScenPlayer.CutScenCallBack)delegate
		{
			NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedEventDeck, 0, 0, dungeonTempletBase.m_DungeonID, cNKMUserData.m_ShadowPalace.currentPalaceId, bLocal: false, m_LastMultiplyRewardCount, 0, m_lSupportUserUID);
		}));
		bool bPlayCutscene = cNKMUserData.m_UserOption.m_bPlayCutscene;
		bool flag = false;
		bool isOnGoing = NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing();
		if (dungeonTempletBase.m_DungeonID > 0 && (!cNKMUserData.CheckDungeonClear(dungeonTempletBase.m_DungeonID) || (bPlayCutscene && !isOnGoing)))
		{
			NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(dungeonTempletBase.m_CutScenStrIDBefore);
			if (cutScenTemple != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(cutScenTemple.m_CutScenStrID, cutScenCallBack, dungeonTempletBase.m_DungeonStrID);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
				flag = true;
			}
		}
		if (!flag)
		{
			cutScenCallBack();
		}
	}

	private void OnClickStartFierceBattleSupport(bool bEventDeck, NKMDungeonTempletBase dungeonTempletBase)
	{
		if (dungeonTempletBase == null)
		{
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCUtil.CheckCommonStartCond(NKCScenManager.GetScenManager().GetMyUserData());
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCUtil.OnExpandInventoryPopup(nKM_ERROR_CODE);
			return;
		}
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr != null)
		{
			int curBossID = nKCFierceBattleSupportDataMgr.CurBossID;
			if (bEventDeck)
			{
				NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedEventDeck, 0, 0, dungeonTempletBase.m_DungeonID, 0, bLocal: false, 0, curBossID, m_lSupportUserUID);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedDeckIndex.m_iIndex, 0, 0, dungeonTempletBase.m_DungeonStrID, 0, bLocal: false, 0, curBossID, 0L);
			}
		}
	}

	public void OnClickGuildCoop(bool bEventDeck, NKMDungeonTempletBase dungeonTempletBase)
	{
		if (dungeonTempletBase == null || !NKCPacketHandlers.Check_NKM_ERROR_CODE(NKCGuildCoopManager.CanStartArena(dungeonTempletBase)))
		{
			return;
		}
		NKMUserData cNKMUserData = NKCScenManager.GetScenManager().GetMyUserData();
		GuildDungeonMemberInfo guildDungeonMemberInfo = NKCGuildCoopManager.GetGuildDungeonMemberInfo().Find((GuildDungeonMemberInfo x) => x.profile.userUid == cNKMUserData.m_UserUID);
		if (guildDungeonMemberInfo == null || (NKCGuildCoopManager.m_ArenaPlayableCount <= 0 && NKCGuildCoopManager.m_ArenaTicketBuyCount >= NKMCommonConst.GuildDungeonConstTemplet.ArenaTicketBuyCount))
		{
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCUtil.CheckCommonStartCond(cNKMUserData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCUtil.OnExpandInventoryPopup(nKM_ERROR_CODE);
			return;
		}
		if (NKMCommonConst.GuildDungeonConstTemplet.ArenaTicketBuyCount - NKCGuildCoopManager.m_ArenaTicketBuyCount > 0 && NKCGuildCoopManager.m_ArenaPlayableCount <= 0 && guildDungeonMemberInfo.arenaList.Count >= NKMCommonConst.GuildDungeonConstTemplet.ArenaPlayCountBasic)
		{
			NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_PLAY_COUNT_BUY_TEXT, 101, NKMCommonConst.GuildDungeonConstTemplet.TicketCost, delegate
			{
				NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_TICKET_BUY_REQ();
			});
			return;
		}
		NKCUICutScenPlayer.CutScenCallBack cutScenCallBack = null;
		cutScenCallBack = ((!bEventDeck) ? ((NKCUICutScenPlayer.CutScenCallBack)delegate
		{
			NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedDeckIndex.m_iIndex, 0, 0, dungeonTempletBase.m_DungeonStrID, 0, bLocal: false, 1, 0, 0L);
		}) : ((NKCUICutScenPlayer.CutScenCallBack)delegate
		{
			NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(m_SelectedEventDeck, 0, 0, dungeonTempletBase.m_DungeonID, 0, bLocal: false, 0, 0, 0L);
		}));
		cutScenCallBack();
	}

	private void OnClickDefence(bool bEventDeck, NKMDungeonTempletBase dungeonTempletBase)
	{
		if (dungeonTempletBase == null)
		{
			return;
		}
		NKMDefenceTemplet defenceTemplet = NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now);
		if (defenceTemplet == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCUtil.CheckCommonStartCond(myUserData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCUtil.OnExpandInventoryPopup(nKM_ERROR_CODE);
			return;
		}
		NKCUICutScenPlayer.CutScenCallBack cutScenCallBack = null;
		cutScenCallBack = delegate
		{
			NKCPacketSender.Send_NKMPacket_DEFENCE_GAME_START_REQ(defenceTemplet.Key, m_SelectedEventDeck);
		};
		bool flag = true;
		if (NKCScenManager.CurrentUserData() != null)
		{
			flag = NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene;
		}
		bool flag2 = false;
		if (!myUserData.CheckDungeonClear(dungeonTempletBase.m_DungeonID) || flag)
		{
			NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(dungeonTempletBase.m_CutScenStrIDBefore);
			if (cutScenTemple != null)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(cutScenTemple.m_CutScenStrID, cutScenCallBack, dungeonTempletBase.m_DungeonStrID);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
				flag2 = true;
			}
		}
		if (!flag2)
		{
			cutScenCallBack();
		}
	}

	public void StartByRepeatOperation()
	{
		if (m_eventDeckIndex == 0)
		{
			UI_DUNGEON_START_CLICK(NKCUIDeckViewer.Instance.GetSelectDeckIndex(), 0L);
		}
		else if (NKCUIPrepareEventDeck.Instance.CheckStartPossible())
		{
			NKCUIPrepareEventDeck.Instance.StartGame();
		}
	}

	private bool CheckPlayCount()
	{
		if (m_StageTemplet == null)
		{
			return true;
		}
		if (m_StageTemplet.EnterLimit <= 0)
		{
			return true;
		}
		int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(m_StageTemplet.Key);
		if (m_StageTemplet.EnterLimit - statePlayCnt <= 0)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			int num = 0;
			if (nKMUserData != null)
			{
				num = nKMUserData.GetStageRestoreCnt(m_StageTemplet.Key);
			}
			if (!m_StageTemplet.Restorable)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ENTER_LIMIT_OVER);
			}
			else if (num >= m_StageTemplet.RestoreLimit)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WARFARE_GAEM_HUD_RESTORE_LIMIT_OVER_DESC);
			}
			else
			{
				NKCPopupResourceWithdraw.Instance.OpenForRestoreEnterLimit(m_StageTemplet, delegate
				{
					NKCPacketSender.Send_NKMPacket_RESET_STAGE_PLAY_COUNT_REQ(m_StageTemplet.Key);
				}, num);
			}
			return false;
		}
		return true;
	}

	public void UI_DUNGEON_START_CLICK(NKMDeckIndex selectedDeckIndex, long supportUserUID = 0L)
	{
		m_SelectedDeckIndex = selectedDeckIndex;
		m_lSupportUserUID = supportUserUID;
		if (!NKCUtil.ProcessDeckErrorMsg(NKMMain.IsValidDeck(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData, selectedDeckIndex)) || m_StageTemplet == null)
		{
			return;
		}
		m_LastMultiplyRewardCount = NKCUIDeckViewer.Instance.GetCurrMultiplyRewardCount();
		m_bOperationSkip = NKCUIDeckViewer.Instance.GetOperationSkipState();
		if (CheckPlayCount())
		{
			if (m_StageTemplet != null)
			{
				OnClickStartCommomProcess(bEventDeck: false, m_StageTemplet);
			}
			else if (m_DungeonTempletBase != null)
			{
				OnClickStartCommomProcess(bEventDeck: false, m_DungeonTempletBase);
			}
		}
	}

	public void OnEventDeckConfirm(NKMStageTempletV2 stageTemplet, NKMDungeonTempletBase dungeonTempletBase, NKMEventDeckData eventDeckData, long supportUserUID = 0L)
	{
		if (eventDeckData == null || (stageTemplet == null && dungeonTempletBase == null))
		{
			return;
		}
		if (m_SelectedEventDeck == null)
		{
			m_SelectedEventDeck = new NKMEventDeckData();
		}
		m_SelectedEventDeck.DeepCopy(eventDeckData);
		m_LastMultiplyRewardCount = NKCUIPrepareEventDeck.Instance.GetCurrMultiplyRewardCount();
		m_bOperationSkip = NKCUIPrepareEventDeck.Instance.GetOperationSkipState();
		m_lSupportUserUID = supportUserUID;
		switch (m_deckContents)
		{
		case DeckContents.SHADOW_PALACE:
			OnClickStartShadowPalace(bEventDeck: true, dungeonTempletBase);
			return;
		case DeckContents.FIERCE_BATTLE_SUPPORT:
			if (NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr() != null)
			{
				OnClickStartFierceBattleSupport(bEventDeck: true, dungeonTempletBase);
			}
			return;
		case DeckContents.GUILD_COOP:
			OnClickGuildCoop(bEventDeck: true, dungeonTempletBase);
			return;
		case DeckContents.DEFENCE:
			OnClickDefence(bEventDeck: true, dungeonTempletBase);
			return;
		}
		if (stageTemplet != null)
		{
			OnClickStartCommomProcess(bEventDeck: true, stageTemplet);
		}
		else if (dungeonTempletBase != null)
		{
			OnClickStartCommomProcess(bEventDeck: true, dungeonTempletBase);
		}
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		if (m_eventDeckIndex == 0)
		{
			NKCUIDeckViewer.Instance.LoadComplete();
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		if (m_LastDeckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_NONE)
		{
			m_LastDeckIndex.m_eDeckType = NKM_DECK_TYPE.NDT_DAILY;
		}
		if (m_SelectedEventDeck == null)
		{
			m_SelectedEventDeck = new NKMEventDeckData();
		}
		if (m_DungeonTempletBase != null && NKCTutorialManager.IsTutorialDungeon(m_DungeonTempletBase.m_DungeonID))
		{
			NKCUICutScenPlayer.CutScenCallBack cutScenCallBack = null;
			switch (m_DungeonTempletBase.m_DungeonID)
			{
			case 1004:
				cutScenCallBack = delegate
				{
					NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(new NKMEventDeckData(), 11211, 0, m_DungeonTempletBase.m_DungeonID, 0, bLocal: false, 1, 0, 0L);
				};
				break;
			case 1005:
				cutScenCallBack = delegate
				{
					NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(new NKMEventDeckData(), 11212, 0, m_DungeonTempletBase.m_DungeonID, 0, bLocal: false, 1, 0, 0L);
				};
				break;
			case 1006:
				cutScenCallBack = delegate
				{
					NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(new NKMEventDeckData(), 11213, 0, m_DungeonTempletBase.m_DungeonID, 0, bLocal: false, 1, 0, 0L);
				};
				break;
			case 1007:
				cutScenCallBack = delegate
				{
					NKCPacketSender.Send_NKMPacket_GAME_LOAD_REQ(new NKMEventDeckData(), 11214, 0, m_DungeonTempletBase.m_DungeonID, 0, bLocal: false, 1, 0, 0L);
				};
				break;
			}
			bool flag = true;
			if (NKCScenManager.CurrentUserData() != null)
			{
				flag = NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene;
			}
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			if (cutScenCallBack != null && (!myUserData.CheckStageCleared(m_StageTemplet) || flag))
			{
				NKCCutScenTemplet stageBeforeCutscen = m_StageTemplet.GetStageBeforeCutscen();
				if (stageBeforeCutscen != null)
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_CUTSCEN_DUNGEON().SetReservedOneCutscenType(stageBeforeCutscen.m_CutScenStrID, cutScenCallBack, m_StageTemplet.Key);
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON);
					return;
				}
			}
			if (cutScenCallBack != null)
			{
				cutScenCallBack();
				return;
			}
		}
		if (m_deckContents == DeckContents.PHASE && NKCPhaseManager.ShouldPlayNextPhase())
		{
			NKCPhaseManager.PlayNextPhase();
			return;
		}
		NKCCamera.EnableBloom(bEnable: false);
		NKCCamera.GetCamera().orthographic = false;
		NKCCamera.GetTrackingPos().SetNowValue(0f, 0f, -1000f);
		if (m_objNUM_OPERATION_BG == null)
		{
			m_objNUM_OPERATION_BG = NKCUIManager.OpenUI("NUM_OPERATION_BG");
		}
		if (m_objNUM_OPERATION_BG != null)
		{
			NKCCamera.RescaleRectToCameraFrustrum(m_objNUM_OPERATION_BG.GetComponent<RectTransform>(), NKCCamera.GetCamera(), new Vector2(200f, 200f), -1000f);
		}
		if (m_eventDeckIndex == 0)
		{
			NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
			{
				MenuName = NKCUtilString.GET_STRING_ATTACK_READY
			};
			if (m_StageTemplet == null)
			{
				return;
			}
			NKMEpisodeTempletV2 episodeTemplet = m_StageTemplet.EpisodeTemplet;
			if (episodeTemplet == null)
			{
				return;
			}
			if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
			{
				options.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.PrepareDungeonBattle_Daily;
			}
			else if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_COUNTERCASE)
			{
				options.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.PrepareDungeonBattleWithoutCost;
			}
			else
			{
				options.eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.PrepareDungeonBattle;
			}
			options.upsideMenuShowResourceList = new List<int> { 1, m_StageTemplet.m_StageReqItemID, 101 };
			options.dOnSideMenuButtonConfirm = UI_DUNGEON_START_CLICK;
			if (m_LastDeckIndex.m_eDeckType == m_UsedDeckType)
			{
				if (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(m_LastDeckIndex) == null)
				{
					m_LastDeckIndex = new NKMDeckIndex(m_UsedDeckType, 0);
				}
				options.DeckIndex = m_LastDeckIndex;
			}
			else
			{
				options.DeckIndex = new NKMDeckIndex(m_UsedDeckType, 0);
			}
			options.dOnBackButton = OnBackButton;
			options.SelectLeaderUnitOnOpen = true;
			options.bEnableDefaultBackground = true;
			options.bUpsideMenuHomeButton = false;
			if (m_StageTemplet != null)
			{
				if (m_StageTemplet.m_StageReqItemID == 0)
				{
					options.CostItemID = 2;
				}
				else
				{
					options.CostItemID = m_StageTemplet.m_StageReqItemID;
				}
				options.CostItemCount = m_StageTemplet.m_StageReqItemCount;
				if (m_StageTemplet.m_StageReqItemID == 2)
				{
					NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref options.CostItemCount);
				}
				options.StageBattleStrID = m_StageTemplet.m_StageBattleStrID;
				bool flag2 = false;
				bool flag3 = true;
				flag2 = NKCContentManager.CheckContentStatus(ContentsType.OPERATION_MULTIPLY, out var _) == NKCContentManager.eContentStatus.Open;
				options.bUsableOperationSkip = flag2 && flag3 && m_StageTemplet.m_bActiveBattleSkip;
			}
			NKCUIDeckViewer.Instance.Open(options);
		}
		else
		{
			bool flag4 = m_DungeonTempletBase != null && NKCTutorialManager.IsTutorialDungeon(m_DungeonTempletBase.m_DungeonID);
			NKMDungeonEventDeckTemplet eventDeckTemplet = NKMDungeonManager.GetEventDeckTemplet(m_eventDeckIndex);
			if (eventDeckTemplet != null && NKMDungeonManager.IsEventDeckSelectRequired(eventDeckTemplet, NKMContentsVersionManager.HasTag("OPERATOR")) && !flag4)
			{
				NKCUIPrepareEventDeck.Instance.Open(m_StageTemplet, m_DungeonTempletBase, null, OnEventDeckConfirm, OnBackButton, m_deckContents);
			}
			else
			{
				OnEventDeckConfirm(m_StageTemplet, m_DungeonTempletBase, new NKMEventDeckData(), 0L);
			}
		}
	}

	public override void PlayScenMusic()
	{
		switch (m_deckContents)
		{
		case DeckContents.SHADOW_PALACE:
			if (string.IsNullOrEmpty(m_BGMName))
			{
				NKCSoundManager.PlayScenMusic(m_NKM_SCEN_ID);
			}
			else
			{
				NKCSoundManager.PlayMusic(m_BGMName);
			}
			break;
		case DeckContents.PHASE:
			if (!NKCPhaseManager.IsPhaseOnGoing())
			{
				NKCSoundManager.PlayScenMusic(m_NKM_SCEN_ID);
			}
			break;
		default:
			NKCSoundManager.PlayScenMusic(m_NKM_SCEN_ID);
			break;
		}
	}

	private void OnBackButton()
	{
		switch (m_deckContents)
		{
		case DeckContents.SHADOW_PALACE:
			BackToShadowBattleScene();
			break;
		case DeckContents.FIERCE_BATTLE_SUPPORT:
			BackToFierceBattleSupportScene();
			break;
		case DeckContents.GUILD_COOP:
			BackToGuildCoopScene();
			break;
		default:
			BackToOperationScene();
			break;
		case DeckContents.DEFENCE:
			BackToShortcut();
			break;
		}
	}

	private void BackToOperationScene()
	{
		if (m_StageTemplet != null && m_StageTemplet.EpisodeTemplet != null && m_StageTemplet.EpisodeCategory == EPISODE_CATEGORY.EC_COUNTERCASE && m_StageTemplet.EpisodeId == 50)
		{
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetCounterCaseNormalActID(m_StageTemplet.ActId);
		}
		if (!NKCScenManager.GetScenManager().Get_SCEN_OPERATION().PlayByFavorite)
		{
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedStage(m_StageTemplet);
		}
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
	}

	private void BackToShadowBattleScene()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_SHADOW_BATTLE().SetShadowPalaceID(NKCScenManager.CurrentUserData().m_ShadowPalace.currentPalaceId);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_BATTLE);
	}

	private void BackToFierceBattleSupportScene()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT);
	}

	private void BackToGuildCoopScene()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_COOP);
	}

	private void BackToShortcut()
	{
		if (m_ShortcutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE)
		{
			NKCContentManager.MoveToShortCut(m_ShortcutType, m_ShortcutParam);
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		NKCUIPrepareEventDeck.CheckInstanceAndClose();
		if (NKCUIDeckViewer.CheckInstance())
		{
			m_LastDeckIndex = NKCUIDeckViewer.Instance.GetSelectDeckIndex();
		}
		NKCUIDeckViewer.CheckInstanceAndClose();
		m_ShortcutType = NKM_SHORTCUT_TYPE.SHORTCUT_NONE;
		m_ShortcutParam = "";
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
		if (!NKCCamera.IsTrackingCameraPos())
		{
			NKCCamera.TrackingPos(10f, NKMRandom.Range(-50f, 50f), NKMRandom.Range(-50f, 50f), NKMRandom.Range(-1000f, -900f));
		}
		m_BloomIntensity.Update(Time.deltaTime);
		if (!m_BloomIntensity.IsTracking())
		{
			m_BloomIntensity.SetTracking(NKMRandom.Range(1f, 2f), 4f, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
		NKCCamera.SetBloomIntensity(m_BloomIntensity.GetNowValue());
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}
}
