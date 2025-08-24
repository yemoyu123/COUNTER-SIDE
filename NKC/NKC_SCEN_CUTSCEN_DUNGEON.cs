using System.Collections.Generic;
using ClientPacket.Game;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_CUTSCEN_DUNGEON : NKC_SCEN_BASIC
{
	public enum CUTSCEN_DUNGEON_START_TYPE
	{
		CDST_DUNGEON,
		CDST_ONE_CUTSCEN
	}

	private GameObject m_NUF_CUTSCEN_DUNGEON;

	private int m_DungeonID;

	private string m_cutscenStrID = "";

	private int m_stageID;

	private NKCUICutScenPlayer.CutScenCallBack m_CutscenCallBack;

	private NKCUIManager.LoadedUIData m_NKCUICutscenDungeonUIData;

	private NKCUICutscenDungeon m_NKCUICutscenDungeon;

	private CUTSCEN_DUNGEON_START_TYPE m_CUTSCEN_DUNGEON_START_TYPE;

	public NKC_SCEN_CUTSCEN_DUNGEON()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_CUTSCENE_DUNGEON;
		m_NUF_CUTSCEN_DUNGEON = GameObject.Find("NUF_CUTSCEN_DUNGEON");
	}

	public void OnRecv(NKMPacket_GAME_LOAD_ACK cNKMPacket_GAME_LOAD_ACK, int multiply = 1)
	{
		NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.UpdateItemInfo(cNKMPacket_GAME_LOAD_ACK.costItemDataList);
		if (cNKMPacket_GAME_LOAD_ACK.gameData != null && cNKMPacket_GAME_LOAD_ACK.gameData.m_WarfareID == 0 && cNKMPacket_GAME_LOAD_ACK.gameData.m_DungeonID > 0)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(cNKMPacket_GAME_LOAD_ACK.gameData.m_DungeonID);
			if (dungeonTempletBase != null)
			{
				string key = $"{NKCScenManager.CurrentUserData().m_UserUID}_{dungeonTempletBase.m_DungeonStrID}";
				if (!PlayerPrefs.HasKey(key) && !NKCScenManager.CurrentUserData().CheckDungeonClear(dungeonTempletBase.m_DungeonStrID))
				{
					PlayerPrefs.SetInt(key, 0);
				}
			}
		}
		NKCScenManager.GetScenManager().GetGameClient().SetGameDataDummy(cNKMPacket_GAME_LOAD_ACK.gameData);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAME);
		Debug.Log("Cutscen_dungeon - NKMPacket_GAME_LOAD_ACK - update - start");
	}

	public void SetReservedDungeonType(int dungeonID)
	{
		m_CUTSCEN_DUNGEON_START_TYPE = CUTSCEN_DUNGEON_START_TYPE.CDST_DUNGEON;
		m_DungeonID = dungeonID;
		m_stageID = 0;
	}

	public void SetReservedCutscenStage(NKMStageTempletV2 stageTemplet)
	{
		m_CUTSCEN_DUNGEON_START_TYPE = CUTSCEN_DUNGEON_START_TYPE.CDST_DUNGEON;
		m_stageID = stageTemplet?.Key ?? 0;
		m_DungeonID = ((stageTemplet != null && stageTemplet.DungeonTempletBase != null) ? stageTemplet.DungeonTempletBase.m_DungeonID : 0);
	}

	public void SetReservedOneCutscenType(string cutscenStrID, NKCUICutScenPlayer.CutScenCallBack callBack, string dungeonStrID)
	{
		m_CUTSCEN_DUNGEON_START_TYPE = CUTSCEN_DUNGEON_START_TYPE.CDST_ONE_CUTSCEN;
		m_cutscenStrID = cutscenStrID;
		m_CutscenCallBack = callBack;
		m_stageID = NKMEpisodeMgr.FindStageTempletByBattleStrID(dungeonStrID)?.Key ?? 0;
	}

	public void SetReservedOneCutscenType(string cutscenStrID, NKCUICutScenPlayer.CutScenCallBack callBack, int stageID = 0)
	{
		m_CUTSCEN_DUNGEON_START_TYPE = CUTSCEN_DUNGEON_START_TYPE.CDST_ONE_CUTSCEN;
		m_cutscenStrID = cutscenStrID;
		m_CutscenCallBack = callBack;
		m_stageID = stageID;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (NKCUICutScenPlayer.HasInstance)
		{
			NKCUICutScenPlayer.Instance.UnLoad();
		}
		if (m_CUTSCEN_DUNGEON_START_TYPE == CUTSCEN_DUNGEON_START_TYPE.CDST_DUNGEON)
		{
			if (m_DungeonID > 0)
			{
				NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_DungeonID);
				if (dungeonTempletBase != null)
				{
					NKCUICutScenPlayer.Instance.Load(dungeonTempletBase.m_CutScenStrIDBefore);
					NKCUICutScenPlayer.Instance.Load(dungeonTempletBase.m_CutScenStrIDAfter);
				}
			}
		}
		else if (m_CUTSCEN_DUNGEON_START_TYPE == CUTSCEN_DUNGEON_START_TYPE.CDST_ONE_CUTSCEN && !string.IsNullOrEmpty(m_cutscenStrID))
		{
			NKCUICutScenPlayer.Instance.Load(m_cutscenStrID);
		}
		m_NUF_CUTSCEN_DUNGEON.SetActive(value: true);
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (!m_bLoadedUI)
		{
			m_NKCUICutscenDungeonUIData = NKCUICutscenDungeon.OpenNewInstance();
			if (m_NKCUICutscenDungeon == null && (m_NKCUICutscenDungeonUIData == null || !m_NKCUICutscenDungeonUIData.CheckLoadAndGetInstance<NKCUICutscenDungeon>(out m_NKCUICutscenDungeon)))
			{
				Debug.LogError("Error - NKC_SCEN_CUTSCEN_DUNGEON.ScenLoadComplete() : UI Load Failed!");
			}
		}
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_NKCUICutscenDungeon.Open();
		if (m_CUTSCEN_DUNGEON_START_TYPE == CUTSCEN_DUNGEON_START_TYPE.CDST_DUNGEON)
		{
			if (m_DungeonID <= 0)
			{
				return;
			}
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_DungeonID);
			if (dungeonTempletBase != null)
			{
				Queue<string> queue = new Queue<string>();
				if (dungeonTempletBase.m_CutScenStrIDBefore != "")
				{
					queue.Enqueue(dungeonTempletBase.m_CutScenStrIDBefore);
				}
				if (dungeonTempletBase.m_CutScenStrIDAfter != "")
				{
					queue.Enqueue(dungeonTempletBase.m_CutScenStrIDAfter);
				}
				NKCPacketSender.Send_NKMPacket_CUTSCENE_DUNGEON_START_REQ(m_DungeonID);
				int stageID = NKMEpisodeMgr.FindStageTempletByBattleStrID(dungeonTempletBase.m_DungeonStrID)?.Key ?? 0;
				NKCUICutScenPlayer.Instance.Play(queue, stageID, DoAfterCutscenForDungeonScenStartType);
			}
		}
		else if (m_CUTSCEN_DUNGEON_START_TYPE == CUTSCEN_DUNGEON_START_TYPE.CDST_ONE_CUTSCEN)
		{
			NKCUICutScenPlayer.Instance.Play(m_cutscenStrID, m_stageID, m_CutscenCallBack);
		}
	}

	private void DoAfterCutscenForDungeonScenStartType()
	{
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedCutscenDungeonClearDGID(m_DungeonID);
		NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(m_stageID);
		if (nKMStageTempletV != null)
		{
			NKMStageTempletV2 possibleNextOperation = nKMStageTempletV.GetPossibleNextOperation();
			if (possibleNextOperation != null)
			{
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedStage(possibleNextOperation);
			}
			else
			{
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedStage(nKMStageTempletV);
			}
		}
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_NKCUICutscenDungeon != null)
		{
			m_NKCUICutscenDungeon.Close();
		}
		if (NKCUICutScenPlayer.HasInstance)
		{
			NKCUICutScenPlayer.Instance.StopWithInvalidatingCallBack();
			NKCUICutScenPlayer.Instance.UnLoad();
		}
		if (m_NUF_CUTSCEN_DUNGEON != null)
		{
			m_NUF_CUTSCEN_DUNGEON.SetActive(value: false);
		}
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_NKCUICutscenDungeon = null;
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}
}
