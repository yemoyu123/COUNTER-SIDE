using System.Collections.Generic;
using ClientPacket.Game;
using ClientPacket.Mode;
using ClientPacket.Raid;
using ClientPacket.WorldMap;
using NKC.PacketHandler;
using NKC.UI;
using NKC.UI.Result;
using NKC.UI.Worldmap;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Video;

namespace NKC;

public class NKC_SCEN_WORLDMAP : NKC_SCEN_BASIC
{
	private NKCUIWorldMap m_NKCUIWorldMap;

	private NKCUIManager.LoadedUIData m_NKCUIWorldMapUIData;

	private bool m_bShowIntro;

	private bool m_bWaitingMovie;

	private bool m_bReserveOpenEventList;

	private bool m_bSelectLastEventListTab;

	private const float MOVIE_PLAY_SPEED = 1.5f;

	private bool m_bReservedDiveReverseAni;

	private NKCPopupWorldMapBuildingInfo m_PopupWorldMapBuildingInfo;

	private NKCPopupWorldMapNewBuildingList m_PopupWorldMapNewBuildingList;

	private NKCPopupWorldmapEventOKCancel m_NKCPopupWorldmapEventOKCancel;

	private int m_introSoundUID;

	private NKMRaidDetailData m_WaitRaidDetailData;

	public bool ReserveOpenEventList => m_bReserveOpenEventList;

	public NKCPopupWorldMapBuildingInfo PopupWorldMapBuildingInfo
	{
		get
		{
			if (m_PopupWorldMapBuildingInfo == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupWorldMapBuildingInfo>("ab_ui_nkm_ui_world_map_renewal", "NKM_UI_WORLD_MAP_RENEWAL_BUILDING_INFO_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), delegate
				{
					m_PopupWorldMapBuildingInfo = null;
				});
				m_PopupWorldMapBuildingInfo = loadedUIData.GetInstance<NKCPopupWorldMapBuildingInfo>();
				m_PopupWorldMapBuildingInfo?.Init();
			}
			return m_PopupWorldMapBuildingInfo;
		}
	}

	public bool IsOpenPopupWorldMapBuildingInfo
	{
		get
		{
			if (m_PopupWorldMapBuildingInfo == null)
			{
				return false;
			}
			return m_PopupWorldMapBuildingInfo.IsOpen;
		}
	}

	public NKCPopupWorldMapNewBuildingList PopupWorldMapNewBuildingList
	{
		get
		{
			if (m_PopupWorldMapNewBuildingList == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupWorldMapNewBuildingList>("ab_ui_nkm_ui_world_map_renewal", "NKM_UI_WORLD_MAP_RENEWAL_BUILDING_LIST_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), delegate
				{
					m_PopupWorldMapNewBuildingList = null;
				});
				m_PopupWorldMapNewBuildingList = loadedUIData.GetInstance<NKCPopupWorldMapNewBuildingList>();
				m_PopupWorldMapNewBuildingList?.Init();
			}
			return m_PopupWorldMapNewBuildingList;
		}
	}

	public bool IsOpenPopupWorldMapNewBuildingList
	{
		get
		{
			if (m_PopupWorldMapNewBuildingList == null)
			{
				return false;
			}
			return m_PopupWorldMapNewBuildingList.IsOpen;
		}
	}

	public NKCPopupWorldmapEventOKCancel NKCPopupWorldmapEventOKCancel
	{
		get
		{
			if (m_NKCPopupWorldmapEventOKCancel == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupWorldmapEventOKCancel>("AB_UI_NKM_UI_WORLD_MAP_RENEWAL", "NKM_UI_WORLD_MAP_POPUP_EventOKCancel", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), delegate
				{
					m_NKCPopupWorldmapEventOKCancel = null;
				});
				m_NKCPopupWorldmapEventOKCancel = loadedUIData.GetInstance<NKCPopupWorldmapEventOKCancel>();
				m_NKCPopupWorldmapEventOKCancel?.InitUI();
			}
			return m_NKCPopupWorldmapEventOKCancel;
		}
	}

	public void SetShowIntro()
	{
		m_bShowIntro = true;
	}

	public void SetReserveOpenEventList(bool selectLastEventListTab = false)
	{
		m_bReserveOpenEventList = true;
		m_bSelectLastEventListTab = selectLastEventListTab;
	}

	public void SetReservedDiveReverseAni(bool bSet)
	{
		m_bReservedDiveReverseAni = bSet;
	}

	public NKC_SCEN_WORLDMAP()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_WORLDMAP;
	}

	public void SetReservedWarning(NKC_WORLD_MAP_WARNING_TYPE type, int cityID)
	{
		NKCUIWorldMap.SetReservedWarning(type, cityID);
	}

	private void VideoPlayMessageCallback(NKCUIComVideoPlayer.eVideoMessage message)
	{
		Debug.Log("Worldmap Video Callback : " + message);
		switch (message)
		{
		case NKCUIComVideoPlayer.eVideoMessage.PlayFailed:
		case NKCUIComVideoPlayer.eVideoMessage.PlayComplete:
			m_bWaitingMovie = false;
			break;
		case NKCUIComVideoPlayer.eVideoMessage.PlayBegin:
			break;
		}
	}

	public void DoAfterLogout()
	{
		SetReservedWarning(NKC_WORLD_MAP_WARNING_TYPE.NWMWT_NONE, -1);
		NKCUIWorldMap.SetReservedPinIntroCityID(-1);
		SetReservedDiveReverseAni(bSet: false);
	}

	public override void ScenDataReq()
	{
		base.ScenDataReq();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.m_WorldmapData.CheckIfHaveSpecificEvent(NKM_WORLDMAP_EVENT_TYPE.WET_RAID))
		{
			NKCPacketSender.Send_NKMPacket_MY_RAID_LIST_REQ();
		}
		else
		{
			base.ScenDataReqWaitUpdate();
		}
	}

	public override void ScenDataReqWaitUpdate()
	{
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (m_bShowIntro)
		{
			NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
			if (subUICameraVideoPlayer != null)
			{
				subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
				subUICameraVideoPlayer.m_fMoviePlaySpeed = 1.5f;
				m_bWaitingMovie = true;
				subUICameraVideoPlayer.Play("Worldmap_Intro.mp4", bLoop: false, bPlaySound: false, VideoPlayMessageCallback);
				m_introSoundUID = NKCSoundManager.PlaySound("FX_UI_WORLDMAP_INTRO", 1f, 0f, 0f);
			}
			else
			{
				m_bWaitingMovie = false;
			}
		}
		if (!NKCUIManager.IsValid(m_NKCUIWorldMapUIData))
		{
			m_NKCUIWorldMapUIData = NKCUIWorldMap.OpenNewInstanceAsync();
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_NKCUIWorldMap == null)
		{
			if (m_NKCUIWorldMapUIData != null && m_NKCUIWorldMapUIData.CheckLoadAndGetInstance<NKCUIWorldMap>(out m_NKCUIWorldMap))
			{
				m_NKCUIWorldMap.Init();
			}
			else
			{
				Debug.LogError("Error - NKC_SCEN_WORLDMAP.ScenLoadComplete() : UI Load Failed!");
			}
		}
	}

	public override void ScenLoadUpdate()
	{
		if (!NKCAssetResourceManager.IsLoadEnd())
		{
			return;
		}
		if (m_bWaitingMovie)
		{
			if (Input.anyKeyDown)
			{
				Debug.Log("TrySkip");
				NKCSoundManager.StopSound(m_introSoundUID);
				if (PlayerPrefs.GetInt("WORLDMAP_LOADING_SKIP", 0) == 1)
				{
					m_bWaitingMovie = false;
				}
			}
		}
		else
		{
			if (PlayerPrefs.GetInt("WORLDMAP_LOADING_SKIP", 0) == 0)
			{
				PlayerPrefs.SetInt("WORLDMAP_LOADING_SKIP", 1);
			}
			ScenLoadLastStart();
		}
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		if (m_bShowIntro)
		{
			NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
			if (subUICameraVideoPlayer != null)
			{
				subUICameraVideoPlayer.Stop();
			}
		}
		m_bShowIntro = false;
	}

	public override void ScenStart()
	{
		base.ScenStart();
		OpenWorldMap();
		NKCCamera.GetCamera().orthographic = false;
		if (m_WaitRaidDetailData != null && null != m_NKCUIWorldMap)
		{
			m_NKCUIWorldMap.OnRecv(m_WaitRaidDetailData, bForce: true);
			m_WaitRaidDetailData = null;
		}
	}

	public void OnRecv(NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK cNKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK)
	{
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.OnRecv(cNKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK);
		}
	}

	public void OnRecv(NKMPacket_RAID_POINT_EXTRA_REWARD_ACK sPacket)
	{
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_WORLDMAP_EVENT_CANCEL_ACK cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK)
	{
		NKCPopupWorldmapEventOKCancel?.Close();
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.OnRecv(cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK);
		}
	}

	public void OnRecv(NKMPacket_RAID_RESULT_ACCEPT_ACK cNKMPacket_RAID_RESULT_ACCEPT_ACK, int cityID)
	{
		if (cityID >= 0)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData == null)
			{
				return;
			}
			NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(cityID);
			if (cityData != null)
			{
				CityDataUpdated(cityData);
			}
		}
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.OnRecv(cNKMPacket_RAID_RESULT_ACCEPT_ACK, cityID);
			if (cityID == -1)
			{
				m_NKCUIWorldMap.OpenEventList(selectLastTab: true);
			}
		}
	}

	public void OnRecv(NKMPacket_RAID_RESULT_ACCEPT_ALL_ACK sPacket, List<int> lstCity)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			foreach (int item in lstCity)
			{
				if (item >= 0)
				{
					NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(item);
					if (cityData != null)
					{
						CityDataUpdated(cityData);
					}
				}
			}
		}
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.OnRecv(sPacket, lstCity);
		}
	}

	public void OnRecv(NKMPacket_RAID_RESULT_LIST_ACK cNKMPacket_RAID_RESULT_LIST_ACK)
	{
		if (!(m_NKCUIWorldMap == null))
		{
			m_NKCUIWorldMap.OnRecv(cNKMPacket_RAID_RESULT_LIST_ACK);
		}
	}

	public void OnRecv(NKMPacket_RAID_COOP_LIST_ACK cNKMPacket_RAID_COOP_LIST_ACK)
	{
		if (!(m_NKCUIWorldMap == null))
		{
			m_NKCUIWorldMap.OnRecv(cNKMPacket_RAID_COOP_LIST_ACK);
		}
	}

	public void OnRecv(NKMPacket_MY_RAID_LIST_ACK cNKMPacket_MY_RAID_LIST_ACK)
	{
		if (m_NKC_SCEN_STATE == NKC_SCEN_STATE.NSS_DATA_REQ_WAIT)
		{
			base.ScenDataReqWaitUpdate();
		}
		else
		{
			if (m_NKCUIWorldMap == null)
			{
				return;
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData == null || nKMUserData.m_WorldmapData == null)
			{
				return;
			}
			if (m_NKCUIWorldMap.IsOpen && cNKMPacket_MY_RAID_LIST_ACK != null && cNKMPacket_MY_RAID_LIST_ACK.myRaidDataList != null)
			{
				for (int i = 0; i < cNKMPacket_MY_RAID_LIST_ACK.myRaidDataList.Count; i++)
				{
					NKMMyRaidData nKMMyRaidData = cNKMPacket_MY_RAID_LIST_ACK.myRaidDataList[i];
					if (nKMMyRaidData != null)
					{
						NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(nKMMyRaidData.cityID);
						if (cityData != null)
						{
							CityDataUpdated(cityData);
						}
					}
				}
			}
			else
			{
				OpenWorldMap();
			}
		}
	}

	public void OnRecv(NKMPacket_DIVE_EXPIRE_NOT cNKMPacket_DIVE_EXPIRE_NOT)
	{
		if (!(m_NKCUIWorldMap == null))
		{
			m_NKCUIWorldMap.OnRecv(cNKMPacket_DIVE_EXPIRE_NOT);
		}
	}

	public void OnRecv(NKMPacket_RAID_DETAIL_INFO_ACK cNKMPacket_RAID_DETAIL_INFO_ACK)
	{
		if (!(m_NKCUIWorldMap == null))
		{
			m_NKCUIWorldMap.OnRecv(cNKMPacket_RAID_DETAIL_INFO_ACK.raidDetailData);
		}
	}

	public void OnRecv(NKMPacket_RAID_SWEEP_ACK sPacket)
	{
		m_WaitRaidDetailData = sPacket.raidDetailData;
	}

	public void OnRecv(NKMPacket_RAID_SEASON_NOT sPacket)
	{
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_WORLDMAP_MISSION_COMPLETE_ACK sPacket, NKMWorldMapCityData cityData, NKCUIResult.CityMissionResultData cityUIData, bool bGotNewEvent)
	{
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.OnRecv(sPacket, cityData, cityUIData, bGotNewEvent);
		}
	}

	private void OpenWorldMap()
	{
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.Open(m_bReservedDiveReverseAni);
			if (m_bReserveOpenEventList)
			{
				m_NKCUIWorldMap.OpenEventList(m_bSelectLastEventListTab);
			}
		}
		m_bReserveOpenEventList = false;
		m_bReservedDiveReverseAni = false;
		m_bSelectLastEventListTab = false;
	}

	public void CloseCityManageUI()
	{
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.CloseCityManagementUI();
		}
	}

	public void ShowReservedWarningUI()
	{
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.ShowReservedWarning();
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		CloseDiveSearch();
		if (m_NKCUIWorldMap != null)
		{
			m_NKCUIWorldMap.Close();
		}
		m_NKCUIWorldMapUIData?.CloseInstance();
		m_NKCUIWorldMapUIData = null;
		m_NKCUIWorldMap = null;
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void OpenDiveSearch()
	{
		OpenDiveSearch(0, 0);
	}

	public void OpenDiveSearch(int cityID, int eventDiveID)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE_READY().SetTargetEventID(cityID, eventDiveID);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DIVE_READY);
	}

	private void CloseDiveSearch()
	{
	}

	public void OpenShadowPalace()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_SHADOW_PALACE);
	}

	public void ProcessReLogin()
	{
		if (!NKCGameEventManager.IsEventPlaying())
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKCScenManager.GetScenManager().GetNowScenID());
		}
	}

	public void RefreshEventList()
	{
		if (m_NKCUIWorldMap != null && m_NKCUIWorldMap.IsOpen)
		{
			m_NKCUIWorldMap.RefreshEventList();
		}
	}

	public void OpenTopPlayerPopup(NKMRaidTemplet raidTemplet, List<NKMRaidJoinData> raidJoinDataList, long raidUID)
	{
		if (m_NKCUIWorldMap.IsOpen)
		{
			m_NKCUIWorldMap.OpenTopPlayerPopup(raidTemplet, raidJoinDataList, raidUID);
		}
	}

	public void RefreshCityRaidData(NKMRaidDetailData raidDetailData)
	{
		if (m_NKCUIWorldMap.IsOpen)
		{
			m_NKCUIWorldMap.UpdateCityRaidData(raidDetailData);
		}
	}

	public void RefreshRaidCoopAllButtonState()
	{
		if (m_NKCUIWorldMap != null && m_NKCUIWorldMap.IsOpen)
		{
			m_NKCUIWorldMap.SetRaidBatchSupportButtonState();
		}
	}

	public void Send_NKMPacket_WORLDMAP_SET_CITY_REQ(int cityID, bool bCash)
	{
		NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(cityID);
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.CanOpenCity(cityTemplet, NKCScenManager.GetScenManager().GetMyUserData(), bCash);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
			return;
		}
		NKMPacket_WORLDMAP_SET_CITY_REQ nKMPacket_WORLDMAP_SET_CITY_REQ = new NKMPacket_WORLDMAP_SET_CITY_REQ();
		nKMPacket_WORLDMAP_SET_CITY_REQ.cityID = cityID;
		nKMPacket_WORLDMAP_SET_CITY_REQ.isCash = bCash;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_SET_CITY_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void OnWorldManCitySet(int cityID, NKMWorldMapCityData cityData)
	{
		if (m_NKCUIWorldMap.IsOpen)
		{
			m_NKCUIWorldMap.OnWorldManCitySet(cityID, cityData);
		}
	}

	public void Send_NKMPacket_WORLDMAP_SET_LEADER_REQ(int cityID, long leaderUID)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMWorldMapManager.CanSetLeader(NKCScenManager.GetScenManager().GetMyUserData(), leaderUID);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
			return;
		}
		NKMPacket_WORLDMAP_SET_LEADER_REQ nKMPacket_WORLDMAP_SET_LEADER_REQ = new NKMPacket_WORLDMAP_SET_LEADER_REQ();
		nKMPacket_WORLDMAP_SET_LEADER_REQ.cityID = cityID;
		nKMPacket_WORLDMAP_SET_LEADER_REQ.leaderUID = leaderUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_SET_LEADER_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void OnCityLeaderChanged(NKMWorldMapCityData cityData)
	{
		if (m_NKCUIWorldMap.IsOpen)
		{
			m_NKCUIWorldMap.CityLeaderChanged(cityData);
		}
	}

	public void Send_NKMPacket_WORLDMAP_CITY_MISSION_REQ(int cityID, int missionID, NKMDeckIndex deckIndex)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMWorldMapCityData cityData = myUserData.m_WorldmapData.GetCityData(cityID);
		if (cityData == null)
		{
			Debug.LogError($"city data is null - {cityID}");
			return;
		}
		NKMWorldMapManager.GetCityTemplet(cityID);
		NKM_ERROR_CODE nKM_ERROR_CODE = cityData.CanStartMission(myUserData, missionID, deckIndex);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
			return;
		}
		NKMPacket_WORLDMAP_CITY_MISSION_REQ nKMPacket_WORLDMAP_CITY_MISSION_REQ = new NKMPacket_WORLDMAP_CITY_MISSION_REQ();
		nKMPacket_WORLDMAP_CITY_MISSION_REQ.cityID = cityID;
		nKMPacket_WORLDMAP_CITY_MISSION_REQ.missionID = missionID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_CITY_MISSION_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void Send_NKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ(int cityID, int missionID)
	{
		NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(cityID);
		if (cityData == null)
		{
			Debug.LogError($"city data is null - {cityID}");
			return;
		}
		NKMWorldMapManager.GetCityTemplet(cityID);
		NKM_ERROR_CODE nKM_ERROR_CODE = cityData.CanCancelMission(missionID);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
			return;
		}
		NKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ nKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ = new NKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ();
		nKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ.cityID = cityID;
		nKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ.missionID = missionID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void CityDataUpdated(NKMWorldMapCityData cityData)
	{
		if (m_NKCUIWorldMap.IsOpen)
		{
			m_NKCUIWorldMap.CityDataUpdated(cityData);
		}
	}

	public void CityEventSpawned(int cityID)
	{
		if (m_NKCUIWorldMap.IsOpen)
		{
			m_NKCUIWorldMap.CityEventSpawned(cityID);
		}
	}

	public void CityBuildingChanged(NKMWorldMapCityData cityData, int changedBuildingID = 0)
	{
		CityBuildingFX(changedBuildingID);
		CityDataUpdated(cityData);
		if (m_PopupWorldMapBuildingInfo != null && m_PopupWorldMapBuildingInfo.IsOpen)
		{
			m_PopupWorldMapBuildingInfo.Close();
		}
		if (m_PopupWorldMapNewBuildingList != null && m_PopupWorldMapNewBuildingList.IsOpen)
		{
			m_PopupWorldMapNewBuildingList.Close();
		}
	}

	public void CityBuildingFX(int buildingID)
	{
		if (m_NKCUIWorldMap.IsOpen)
		{
			m_NKCUIWorldMap.SetFXBuildingID(buildingID);
		}
	}

	public void Send_NKMPacket_WORLDMAP_MISSION_REFRESH_REQ(int cityID)
	{
		if (!NKCScenManager.GetScenManager().GetMyUserData().CheckPrice(50L, 3))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CASH));
			return;
		}
		NKMPacket_WORLDMAP_MISSION_REFRESH_REQ nKMPacket_WORLDMAP_MISSION_REFRESH_REQ = new NKMPacket_WORLDMAP_MISSION_REFRESH_REQ();
		nKMPacket_WORLDMAP_MISSION_REFRESH_REQ.cityID = cityID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_MISSION_REFRESH_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void Send_NKMPacket_WORLDMAP_MISSION_COMPLETE_REQ(int cityID)
	{
		NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(cityID);
		if (cityData == null)
		{
			Debug.LogError($"city data is null - {cityID}");
			return;
		}
		if (cityData.worldMapMission.currentMissionID == 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_NOT_DOING));
			return;
		}
		if (!NKCSynchronizedTime.IsFinished(cityData.worldMapMission.completeTime))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DOING));
			return;
		}
		NKMPacket_WORLDMAP_MISSION_COMPLETE_REQ nKMPacket_WORLDMAP_MISSION_COMPLETE_REQ = new NKMPacket_WORLDMAP_MISSION_COMPLETE_REQ();
		nKMPacket_WORLDMAP_MISSION_COMPLETE_REQ.cityID = cityID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_MISSION_COMPLETE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void Send_NKMPacket_WORLDMAP_COLLECT_REQ(int cityID)
	{
	}

	public void Send_NKMPacket_WORLDMAP_BUILD_REQ(int cityID, int buildingID)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMWorldMapManager.CanBuild(NKCScenManager.CurrentUserData(), cityID, buildingID);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
			return;
		}
		NKMPacket_WORLDMAP_BUILD_REQ nKMPacket_WORLDMAP_BUILD_REQ = new NKMPacket_WORLDMAP_BUILD_REQ();
		nKMPacket_WORLDMAP_BUILD_REQ.cityID = cityID;
		nKMPacket_WORLDMAP_BUILD_REQ.buildID = buildingID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_BUILD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void Send_NKMPacket_WORLDMAP_BUILD_LEVELUP_REQ(int cityID, int buildingID)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMWorldMapManager.CanLevelUpBuilding(NKCScenManager.CurrentUserData(), cityID, buildingID);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
			return;
		}
		NKMPacket_WORLDMAP_BUILD_LEVELUP_REQ nKMPacket_WORLDMAP_BUILD_LEVELUP_REQ = new NKMPacket_WORLDMAP_BUILD_LEVELUP_REQ();
		nKMPacket_WORLDMAP_BUILD_LEVELUP_REQ.cityID = cityID;
		nKMPacket_WORLDMAP_BUILD_LEVELUP_REQ.buildID = buildingID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_BUILD_LEVELUP_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void Send_NKMPacket_WORLDMAP_BUILD_EXPIRE_REQ(int cityID, int buildingID)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMWorldMapManager.CanExpireBuilding(NKCScenManager.CurrentUserData(), cityID, buildingID);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
			return;
		}
		NKMPacket_WORLDMAP_BUILD_EXPIRE_REQ nKMPacket_WORLDMAP_BUILD_EXPIRE_REQ = new NKMPacket_WORLDMAP_BUILD_EXPIRE_REQ();
		nKMPacket_WORLDMAP_BUILD_EXPIRE_REQ.cityID = cityID;
		nKMPacket_WORLDMAP_BUILD_EXPIRE_REQ.buildID = buildingID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_WORLDMAP_BUILD_EXPIRE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public RectTransform GetCityRect(int cityID)
	{
		if (m_NKCUIWorldMap != null)
		{
			return m_NKCUIWorldMap.GetPinRect(cityID);
		}
		return null;
	}
}
