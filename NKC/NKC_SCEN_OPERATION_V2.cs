using ClientPacket.Mode;
using NKC.UI;
using NKC.UI.Result;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;

namespace NKC;

public class NKC_SCEN_OPERATION_V2 : NKC_SCEN_BASIC
{
	private NKCUIEpisodeViewerCC m_NKCUIEpisodeViewerCC;

	private NKCUIManager.LoadedUIData m_loadUIDataCC;

	private NKCUICounterCaseNormal m_NKCUICounterCaseNormal;

	private NKCUIManager.LoadedUIData m_loadUIDataCCNormal;

	private NKCUIOperationV2 m_NKCUIOperationV2;

	private NKCUIManager.LoadedUIData m_loadUIDataOperation;

	private EPISODE_CATEGORY m_ReservedEpisodeCategory = EPISODE_CATEGORY.EC_COUNT;

	private NKMEpisodeTempletV2 m_ReservedEpisodeTemplet;

	private NKMStageTempletV2 m_ReservedStageTemplet;

	private int m_ReservedCutscenDungeonClearDGID;

	private int m_ReservedCounterCaseActID;

	private NKMRewardData m_RewardData;

	private int m_LastPlayedMainStreamID;

	private int m_LastPlayedSubStreamID;

	private int m_LastPlayedSeasonalID;

	private bool m_bPlayByFavorite;

	private const float FIVE_SECONDS = 5f;

	private float m_deltaTime;

	public bool PlayByFavorite
	{
		get
		{
			return m_bPlayByFavorite;
		}
		set
		{
			m_bPlayByFavorite = value;
		}
	}

	public void SetLastPlayedMainStream(int id)
	{
		m_LastPlayedMainStreamID = id;
	}

	public int GetLastPlayedMainStream()
	{
		return m_LastPlayedMainStreamID;
	}

	public void SetLastPlayedSubStream(int id)
	{
		m_LastPlayedSubStreamID = id;
	}

	public int GetLastPlayedSubStream()
	{
		return m_LastPlayedSubStreamID;
	}

	public void SetLastPlayedSeasonal(int id)
	{
		m_LastPlayedSeasonalID = id;
	}

	public int GetLastPlayedSeasonal()
	{
		return m_LastPlayedSeasonalID;
	}

	public NKC_SCEN_OPERATION_V2()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_OPERATION;
	}

	public void DoAfterLogOut()
	{
		SetReservedCutscenDungeonClearDGID(0);
		NKMEpisodeMgr.ClearFavoriteStage();
		m_ReservedEpisodeCategory = EPISODE_CATEGORY.EC_COUNT;
		m_ReservedStageTemplet = null;
		m_ReservedEpisodeTemplet = null;
		m_ReservedCutscenDungeonClearDGID = 0;
		m_LastPlayedMainStreamID = 0;
		m_LastPlayedSubStreamID = 0;
		m_LastPlayedSeasonalID = 0;
	}

	public EPISODE_CATEGORY GetReservedEpisodeCategory()
	{
		return m_ReservedEpisodeCategory;
	}

	public void SetReservedEpisodeCategory(int groupID)
	{
		NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(groupID);
		SetReservedEpisodeCategory(nKMEpisodeGroupTemplet.EpCategory);
	}

	public void SetReservedEpisodeCategory(EPISODE_CATEGORY epCate)
	{
		m_ReservedEpisodeCategory = epCate;
		m_ReservedEpisodeTemplet = null;
		m_ReservedStageTemplet = null;
	}

	public NKMEpisodeTempletV2 GetReservedEpisodeTemplet()
	{
		return m_ReservedEpisodeTemplet;
	}

	public void SetReservedEpisodeTemplet(NKMEpisodeTempletV2 epTemplet)
	{
		m_ReservedEpisodeTemplet = epTemplet;
		if (epTemplet != null)
		{
			m_ReservedEpisodeCategory = epTemplet.m_EPCategory;
			m_ReservedStageTemplet = null;
		}
		else
		{
			m_ReservedStageTemplet = null;
		}
	}

	public NKMStageTempletV2 GetReservedStageTemplet()
	{
		return m_ReservedStageTemplet;
	}

	public void SetReservedStage(NKMStageTempletV2 stageTemplet)
	{
		m_ReservedStageTemplet = stageTemplet;
		if (stageTemplet != null)
		{
			m_ReservedEpisodeCategory = stageTemplet.EpisodeCategory;
			m_ReservedEpisodeTemplet = stageTemplet.EpisodeTemplet;
		}
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(m_loadUIDataCC))
		{
			m_loadUIDataCC = NKCUIEpisodeViewerCC.OpenNewInstanceAsync();
		}
		if (!NKCUIManager.IsValid(m_loadUIDataCCNormal))
		{
			m_loadUIDataCCNormal = NKCUICounterCaseNormal.OpenNewInstanceAsync();
		}
		if (!NKCUIManager.IsValid(m_loadUIDataOperation))
		{
			m_loadUIDataOperation = NKCUIOperationV2.OpenNewInstanceAsync();
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_NKCUIOperationV2 == null)
		{
			if (m_loadUIDataOperation != null && m_loadUIDataOperation.CheckLoadAndGetInstance<NKCUIOperationV2>(out m_NKCUIOperationV2))
			{
				m_NKCUIOperationV2.InitUI();
				NKCUtil.SetGameobjectActive(m_NKCUIOperationV2.gameObject, bValue: false);
			}
			else
			{
				Debug.LogError("NKC_SCEN_OPERATION.ScenLoadUIComplete - ui load m_NKCUIOperationV2 failed");
			}
		}
		if (m_NKCUIEpisodeViewerCC == null)
		{
			if (m_loadUIDataCC != null && m_loadUIDataCC.CheckLoadAndGetInstance<NKCUIEpisodeViewerCC>(out m_NKCUIEpisodeViewerCC))
			{
				m_NKCUIEpisodeViewerCC.InitUI();
				NKCUtil.SetGameobjectActive(m_NKCUIEpisodeViewerCC.gameObject, bValue: false);
			}
			else
			{
				Debug.LogError("NKC_SCEN_OPERATION.ScenLoadUIComplete - ui load m_NKCUIEpisodeViewerCC failed");
			}
		}
		if (m_NKCUICounterCaseNormal == null)
		{
			if (m_loadUIDataCCNormal != null && m_loadUIDataCCNormal.CheckLoadAndGetInstance<NKCUICounterCaseNormal>(out m_NKCUICounterCaseNormal))
			{
				m_NKCUICounterCaseNormal.InitUI();
				NKCUtil.SetGameobjectActive(m_NKCUICounterCaseNormal.gameObject, bValue: false);
			}
			else
			{
				Debug.LogError("NKC_SCEN_OPERATION.ScenLoadUIComplete - ui load m_NKCUICounterCaseNormal failed");
			}
		}
		if (m_NKCUIOperationV2 != null)
		{
			m_NKCUIOperationV2.PreLoad();
		}
	}

	public override void ScenDataReq()
	{
		base.ScenDataReq();
		m_deltaTime = 0f;
		if (m_ReservedCutscenDungeonClearDGID > 0)
		{
			NKMPacket_CUTSCENE_DUNGEON_CLEAR_REQ nKMPacket_CUTSCENE_DUNGEON_CLEAR_REQ = new NKMPacket_CUTSCENE_DUNGEON_CLEAR_REQ();
			nKMPacket_CUTSCENE_DUNGEON_CLEAR_REQ.dungeonID = m_ReservedCutscenDungeonClearDGID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_CUTSCENE_DUNGEON_CLEAR_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	public override void ScenDataReqWaitUpdate()
	{
		m_deltaTime += Time.deltaTime;
		if (m_deltaTime > 5f)
		{
			m_deltaTime = 0f;
			Set_NKC_SCEN_STATE(NKC_SCEN_STATE.NSS_FAIL);
		}
		else if (m_ReservedCutscenDungeonClearDGID <= 0)
		{
			base.ScenDataReqWaitUpdate();
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_NKCUIOperationV2.Open(m_RewardData != null);
		if (m_RewardData != null)
		{
			NKCUIResult.Instance.OpenComplexResult(NKCScenManager.CurrentUserData().m_ArmyData, m_RewardData, OnCloseResultPopup, 0L);
			m_RewardData = null;
		}
	}

	public override void PlayScenMusic()
	{
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_NKCUICounterCaseNormal.Close();
		m_NKCUIEpisodeViewerCC.Close();
		m_NKCUIOperationV2.Close();
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_NKCUICounterCaseNormal = null;
		m_loadUIDataCCNormal?.CloseInstance();
		m_loadUIDataCCNormal = null;
		m_NKCUIEpisodeViewerCC = null;
		m_loadUIDataCC?.CloseInstance();
		m_loadUIDataCC = null;
		m_NKCUIOperationV2 = null;
		m_loadUIDataOperation?.CloseInstance();
		m_loadUIDataOperation = null;
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
		if (!NKCCamera.IsTrackingCameraPos())
		{
			NKCCamera.TrackingPos(10f, NKMRandom.Range(-50f, 50f), NKMRandom.Range(-50f, 50f), NKMRandom.Range(-1000f, -900f));
		}
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void ProcessRelogin()
	{
		if (!NKCGameEventManager.IsEventPlaying())
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKCScenManager.GetScenManager().GetNowScenID());
		}
	}

	public void SetCounterCaseNormalActID(int id)
	{
		m_ReservedCounterCaseActID = id;
	}

	public int GetCounterCaseNormalActID()
	{
		return m_ReservedCounterCaseActID;
	}

	public void OnRecv(NKMPacket_COUNTERCASE_UNLOCK_ACK cNKMPacket_COUNTERCASE_UNLOCK_ACK)
	{
		if (m_NKCUICounterCaseNormal != null && m_NKCUICounterCaseNormal.IsOpen)
		{
			m_NKCUICounterCaseNormal.UpdateLeftslot();
			m_NKCUICounterCaseNormal.UpdateRightSlots(bSlotAni: false, cNKMPacket_COUNTERCASE_UNLOCK_ACK.dungeonID);
		}
	}

	public void OnRecv(NKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK)
	{
		m_ReservedCutscenDungeonClearDGID = 0;
		if (m_NKCUICounterCaseNormal != null && m_NKCUICounterCaseNormal.IsOpen)
		{
			m_NKCUICounterCaseNormal.UpdateLeftslot();
			m_NKCUICounterCaseNormal.UpdateRightSlots();
		}
		if (m_NKCUIEpisodeViewerCC != null && m_NKCUIEpisodeViewerCC.IsOpen)
		{
			m_NKCUIEpisodeViewerCC.UpdateUI();
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK.dungeonClearData.dungeonId);
		if (dungeonTempletBase != null && dungeonTempletBase.StageTemplet != null)
		{
			if (dungeonTempletBase.StageTemplet.EpisodeCategory == EPISODE_CATEGORY.EC_COUNTERCASE)
			{
				SetCounterCaseNormalActID(dungeonTempletBase.StageTemplet.ActId);
				SetReservedEpisodeTemplet(dungeonTempletBase.StageTemplet.EpisodeTemplet);
			}
			else
			{
				NKMStageTempletV2 possibleNextOperation = dungeonTempletBase.StageTemplet.GetPossibleNextOperation();
				if (possibleNextOperation != null)
				{
					NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedStage(possibleNextOperation);
				}
				else
				{
					NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedStage(dungeonTempletBase.StageTemplet);
				}
			}
		}
		m_RewardData = cNKMPacket_CUTSCENE_DUNGEON_CLEAR_ACK.dungeonClearData.rewardData;
	}

	public void SetReservedCutscenDungeonClearDGID(int dungeonID)
	{
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
		if (dungeonTempletBase != null)
		{
			m_ReservedStageTemplet = NKMEpisodeMgr.FindStageTempletByBattleStrID(dungeonTempletBase.m_DungeonStrID);
			if (m_ReservedStageTemplet != null)
			{
				if (m_ReservedStageTemplet.EpisodeCategory == EPISODE_CATEGORY.EC_COUNTERCASE)
				{
					SetCounterCaseNormalActID(m_ReservedStageTemplet.ActId);
					SetReservedEpisodeTemplet(m_ReservedStageTemplet.EpisodeTemplet);
				}
				else
				{
					NKMStageTempletV2 possibleNextOperation = m_ReservedStageTemplet.GetPossibleNextOperation();
					if (possibleNextOperation != null && !NKCScenManager.CurrentUserData().CheckStageCleared(possibleNextOperation))
					{
						SetReservedStage(possibleNextOperation);
					}
					else
					{
						SetReservedStage(m_ReservedStageTemplet);
					}
				}
			}
			m_ReservedCutscenDungeonClearDGID = dungeonID;
		}
		else
		{
			m_ReservedCutscenDungeonClearDGID = 0;
		}
	}

	public void OpenCounterCaseViewer()
	{
		if (NKCUIOperationNodeViewer.isOpen())
		{
			NKCUIOperationNodeViewer.Instance.Close();
		}
		if (m_NKCUIEpisodeViewerCC != null)
		{
			m_NKCUIEpisodeViewerCC.Open();
		}
	}

	public void OpenCounterCaseNormalAct(int actID)
	{
		m_NKCUICounterCaseNormal.SetActID(actID);
		m_NKCUICounterCaseNormal.Open();
	}

	public void ReopenEpisodeView()
	{
		if (m_NKCUICounterCaseNormal != null && m_NKCUICounterCaseNormal.IsOpen)
		{
			m_NKCUICounterCaseNormal.Close();
		}
		if (m_NKCUIEpisodeViewerCC != null && m_NKCUIEpisodeViewerCC.IsOpen)
		{
			m_NKCUIEpisodeViewerCC.Close();
		}
		if (NKCUIOperationNodeViewer.isOpen())
		{
			NKCUIOperationNodeViewer.Instance.Close();
		}
		m_NKCUIOperationV2.Close();
		m_NKCUIOperationV2.Open();
	}

	public void OnRecv(NKMPacket_STAGE_UNLOCK_ACK sPacket)
	{
	}

	public void SetTutorialMainstreamGuide(NKCGameEventManager.NKCGameEventTemplet eventTemplet, UnityAction Complete)
	{
		if (m_NKCUIOperationV2 != null)
		{
			m_NKCUIOperationV2.SetTutorialMainstreamGuide(eventTemplet, Complete);
		}
		else
		{
			Complete?.Invoke();
		}
	}

	public RectTransform GetDailyRect()
	{
		if (m_NKCUIOperationV2 != null)
		{
			return m_NKCUIOperationV2.GetDailyRect();
		}
		return null;
	}

	public RectTransform GetStageSlot(int stageIndex)
	{
		if (m_NKCUIOperationV2 == null || !m_NKCUIOperationV2.IsOpen)
		{
			return null;
		}
		return m_NKCUIOperationV2.GetStageSlotRect(stageIndex);
	}

	public RectTransform GetActSlot(int actIndex)
	{
		if (m_NKCUIOperationV2 == null || !m_NKCUIOperationV2.IsOpen)
		{
			return null;
		}
		return m_NKCUIOperationV2.GetActSlotRect(actIndex);
	}

	public RectTransform GetCounterCaseSlot(int unitID)
	{
		if (m_NKCUIEpisodeViewerCC == null || !m_NKCUIEpisodeViewerCC.IsOpen)
		{
			return null;
		}
		return m_NKCUIEpisodeViewerCC.GetSlotByUnitID(unitID);
	}

	public NKCUICCNormalSlot GetCounterCaseListItem(int index)
	{
		if (m_NKCUIEpisodeViewerCC == null || !m_NKCUIEpisodeViewerCC.IsOpen)
		{
			return null;
		}
		if (m_NKCUICounterCaseNormal == null || !m_NKCUICounterCaseNormal.IsOpen)
		{
			return null;
		}
		return m_NKCUICounterCaseNormal.GetItemByStageIdx(index);
	}

	public void OnCloseResultPopup()
	{
		NKCContentManager.ShowContentUnlockPopup(TutorialCheck);
	}

	public void TutorialCheck()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OPERATION)
		{
			NKCTutorialManager.TutorialRequired(TutorialPoint.EpisodeResult);
		}
	}
}
