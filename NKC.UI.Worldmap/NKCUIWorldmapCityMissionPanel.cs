using System.Collections.Generic;
using ClientPacket.WorldMap;
using NKC.Publisher;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCUIWorldmapCityMissionPanel : MonoBehaviour
{
	[Header("List")]
	public GameObject m_objMissionListRoot;

	public List<NKCUIWorldMapMissionSlot> m_lstSlot;

	[Header("Progress")]
	public GameObject m_objMissionProgressRoot;

	public NKCUIWorldMapMissionSlot m_SlotProgress;

	public Text m_lbMisionDeckIndex;

	public Text m_lbMissionTimeLeft;

	public NKCUIComStateButton m_csbtnCancelMission;

	public RectTransform m_rtMissionSDSoloPos;

	private NKCASUIUnitIllust m_spineSDAlly;

	[Header("Etc")]
	public GameObject m_objManagerRequired;

	public NKCUIComStateButton m_csbtnMissionRefresh;

	public NKCUIPriceTag m_tagRefreshPrice;

	public GameObject m_INFO_TWN;

	private NKMWorldMapCityData m_CityData;

	private bool m_bCompleteRequestSent;

	private readonly int[] m_aEnemyID = new int[3] { 511003, 511004, 511006 };

	private int m_SelectedMissionID;

	public void Init()
	{
		foreach (NKCUIWorldMapMissionSlot item in m_lstSlot)
		{
			item.Init(OnSelectMission);
		}
		m_SlotProgress.Init(null);
		if (m_csbtnMissionRefresh != null)
		{
			m_csbtnMissionRefresh.PointerClick.RemoveAllListeners();
			m_csbtnMissionRefresh.PointerClick.AddListener(OnMissionRefresh);
		}
		if (m_csbtnCancelMission != null)
		{
			m_csbtnCancelMission.PointerClick.RemoveAllListeners();
			m_csbtnCancelMission.PointerClick.AddListener(OnMissionCancel);
		}
		NKCUtil.SetGameobjectActive(m_INFO_TWN, NKMContentsVersionManager.HasCountryTag(CountryTagType.TWN));
	}

	public void SetData(NKMWorldMapCityData cityData)
	{
		if (cityData == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_WORLDMAP_CITY_DATA_IS_NULL);
			NKCUtil.SetGameobjectActive(m_objMissionListRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMissionProgressRoot, bValue: false);
			return;
		}
		m_bCompleteRequestSent = false;
		m_CityData = cityData;
		if (IsMissionRunning())
		{
			SetDataMissionProgress(cityData);
		}
		else
		{
			SetDataMissionList(cityData);
		}
	}

	private void SetDataMissionList(NKMWorldMapCityData cityData)
	{
		NKCUtil.SetGameobjectActive(m_objMissionListRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objMissionProgressRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnMissionRefresh, NKCScenManager.CurrentUserData().IsSuperUser());
		NKCUtil.SetGameobjectActive(m_objManagerRequired, !HasLeader());
		int leaderLevel = 0;
		if (cityData != null)
		{
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(cityData.leaderUnitUID);
			if (unitFromUID != null)
			{
				leaderLevel = unitFromUID.m_UnitLevel;
			}
			for (int i = 0; i < m_lstSlot.Count; i++)
			{
				if (i < cityData.worldMapMission.stMissionIDList.Count)
				{
					int id = cityData.worldMapMission.stMissionIDList[i];
					NKMWorldMapMissionTemplet missionTemplet = NKMWorldMapManager.GetMissionTemplet(id);
					if (missionTemplet != null)
					{
						m_lstSlot[i].SetData(missionTemplet, leaderLevel);
						NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: true);
					}
					else
					{
						Debug.LogError("Mission Templet Null! ID : " + id);
						NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: false);
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: false);
				}
			}
		}
		UpdateRefreshButton();
	}

	private void SetDataMissionProgress(NKMWorldMapCityData cityData)
	{
		NKCUtil.SetGameobjectActive(m_objMissionListRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMissionProgressRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_csbtnMissionRefresh, bValue: false);
		NKCUtil.SetGameobjectActive(m_objManagerRequired, !HasLeader());
		NKMWorldMapMissionTemplet missionTemplet = NKMWorldMapManager.GetMissionTemplet(cityData.worldMapMission.currentMissionID);
		if (missionTemplet == null)
		{
			Debug.LogError($"MissionTemplet Not Found! ID : {cityData.worldMapMission.currentMissionID}");
			return;
		}
		m_SlotProgress.SetData(missionTemplet);
		NKCUtil.SetLabelText(m_lbMisionDeckIndex, NKCUtilString.GET_STRING_WORLDMAP_CITY_LEADER);
		NKCUtil.SetLabelText(m_lbMissionTimeLeft, NKCSynchronizedTime.GetTimeLeftString(m_CityData.worldMapMission.completeTime));
		NKCPublisherModule.Push.UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.WORLD_MAP_MISSION_COMPLETE);
		SetMissionProgressSD(missionTemplet);
	}

	private void SetMissionProgressSD(NKMWorldMapMissionTemplet missionTemplet)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(m_CityData.leaderUnitUID);
			NKCASUIUnitIllust.eAnimation missionProgressAnimationType = NKCUIWorldMap.GetMissionProgressAnimationType(missionTemplet.m_eMissionType);
			bool bValue = OpenSDIllust(unitFromUID, ref m_spineSDAlly, m_rtMissionSDSoloPos);
			NKCUtil.SetGameobjectActive(m_rtMissionSDSoloPos, bValue);
			m_spineSDAlly.SetAnimation(missionProgressAnimationType, loop: true);
		}
	}

	private bool OpenSDIllust(NKMUnitData unitData, ref NKCASUIUnitIllust SpineIllust, RectTransform parent)
	{
		if (unitData == null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(SpineIllust);
			SpineIllust = null;
			return false;
		}
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(SpineIllust);
		SpineIllust = NKCResourceUtility.OpenSpineSD(unitData);
		if (SpineIllust != null)
		{
			SpineIllust.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = SpineIllust.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector2.zero;
				rectTransform.localScale = Vector2.one;
				rectTransform.localRotation = Quaternion.identity;
			}
			return true;
		}
		Debug.LogError("spine data not found from unitID : " + unitData.m_UnitID);
		return false;
	}

	private bool OpenSDIllust(int targetUnitID, ref NKCASUIUnitIllust SpineIllust, RectTransform parent)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(targetUnitID);
		if (unitTempletBase == null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(SpineIllust);
			SpineIllust = null;
			return false;
		}
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(SpineIllust);
		SpineIllust = NKCResourceUtility.OpenSpineSD(unitTempletBase);
		if (SpineIllust != null)
		{
			SpineIllust.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = SpineIllust.GetRectTransform();
			rectTransform.localPosition = Vector2.zero;
			rectTransform.localScale = Vector2.one;
			rectTransform.localRotation = Quaternion.identity;
			return true;
		}
		Debug.LogError("spine data not found from unitID : " + unitTempletBase.m_UnitID);
		return false;
	}

	private void UpdateRefreshButton()
	{
		m_tagRefreshPrice.SetData(3, 50);
	}

	private bool IsMissionRunning()
	{
		return NKMWorldMapManager.IsMissionRunning(m_CityData);
	}

	private void Update()
	{
		ProcessHotkey();
		if (m_objMissionProgressRoot.activeSelf && IsMissionRunning())
		{
			NKCUtil.SetLabelText(m_lbMissionTimeLeft, NKCSynchronizedTime.GetTimeLeftString(m_CityData.worldMapMission.completeTime));
			if (NKCSynchronizedTime.IsFinished(m_CityData.worldMapMission.completeTime))
			{
				SendCompleteReq();
			}
		}
	}

	private void SendCompleteReq()
	{
		if (m_CityData != null && !m_bCompleteRequestSent)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_MISSION_COMPLETE_REQ(m_CityData.cityID);
			m_bCompleteRequestSent = true;
		}
	}

	public void CleanUp()
	{
		m_CityData = null;
	}

	private bool HasLeader()
	{
		if (m_CityData != null)
		{
			return m_CityData.leaderUnitUID != 0;
		}
		return false;
	}

	private void OnSelectMission(int missionID)
	{
		m_SelectedMissionID = missionID;
		if (IsMissionRunning())
		{
			Debug.LogError("Logic Error : Accssing to Mission ");
			return;
		}
		if (m_CityData.leaderUnitUID == 0L)
		{
			Debug.LogWarning("지부장 없이 여기로 들어오면 안 됨");
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(m_CityData.leaderUnitUID);
		NKMWorldMapMissionTemplet missionTemplet = NKMWorldMapManager.GetMissionTemplet(missionID);
		if (unitFromUID.m_UnitLevel < missionTemplet.m_ReqManagerLevel)
		{
			NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_LEADER_LEVEL_LOW);
			return;
		}
		if (NKMWorldMapManager.IsMissionLeaderOnly(missionTemplet.m_eMissionType))
		{
			int successRate = NKMWorldMapManager.GetMissionSuccessRate(missionTemplet, nKMUserData.m_ArmyData, m_CityData);
			NKCCompanyBuff.IncreaseMissioRateInWorldMap(nKMUserData.m_companyBuffDataList, ref successRate);
			string content = string.Format(NKCUtilString.GET_STRING_WORLDMAP_CITY_MISSION_CONFIRM_TWO_PARAM, missionTemplet.GetMissionName(), successRate);
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, content, delegate
			{
				OnMissionDeckSelected(new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), 0L);
			});
			return;
		}
		NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
		{
			MenuName = NKCUtilString.GET_STRING_WORLDMAP_CITY_MISSION_SELECT_SQUAD,
			eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.WorldMapMissionDeckSelect,
			dOnSideMenuButtonConfirm = OnMissionDeckSelected,
			DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, 0),
			dOnBackButton = null,
			SelectLeaderUnitOnOpen = true,
			bEnableDefaultBackground = true,
			bUpsideMenuHomeButton = false,
			WorldMapMissionID = missionID,
			WorldMapMissionCityID = m_CityData.cityID,
			StageBattleStrID = string.Empty
		};
		NKCUIDeckViewer.Instance.Open(options);
	}

	private void OnMissionRefresh()
	{
		if (NKCScenManager.CurrentUserData().IsSuperUser())
		{
			NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WORLDMAP_CITY_MISSION_REFRESH, 3, 50, delegate
			{
				SendRefreshRequest(bCash: true);
			}, null, showResource: true);
		}
	}

	private void SendRefreshRequest(bool bCash)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_MISSION_REFRESH_REQ(m_CityData.cityID);
	}

	private void OnMissionCancel()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WORLDMAP_CITY_MISSION_CANCEL, SendDispatchCancel);
	}

	private void SendDispatchCancel()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ(m_CityData.cityID, m_CityData.worldMapMission.currentMissionID);
	}

	public void OnMissionDeckSelected(NKMDeckIndex deckIndex, long supportUserUID = 0L)
	{
		NKCUIDeckViewer.CheckInstanceAndClose();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_CITY_MISSION_REQ(m_CityData.cityID, m_SelectedMissionID, deckIndex);
	}

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (itemData.ItemID == 3 && !IsMissionRunning())
		{
			UpdateRefreshButton();
		}
	}

	private void ProcessHotkey()
	{
		if (IsMissionRunning() || !NKCUIManager.IsTopmostUI(NKCUIWorldMap.GetInstance()))
		{
			return;
		}
		if (NKCInputManager.CheckHotKeyEvent(HotkeyEventType.ShowHotkey))
		{
			for (int i = 0; i < m_lstSlot.Count; i++)
			{
				int num = i + 1;
				NKCUIComHotkeyDisplay.OpenInstance(m_lstSlot[i].m_cbtnMissionSelect.transform, $"{num}");
			}
		}
		if (Input.GetKeyUp(KeyCode.Alpha1))
		{
			if (m_lstSlot[0] != null)
			{
				OnSelectMission(m_lstSlot[0].MissionID);
			}
		}
		else if (Input.GetKeyUp(KeyCode.Alpha2))
		{
			if (m_lstSlot[1] != null)
			{
				OnSelectMission(m_lstSlot[1].MissionID);
			}
		}
		else if (Input.GetKeyUp(KeyCode.Alpha3) && m_lstSlot[2] != null)
		{
			OnSelectMission(m_lstSlot[2].MissionID);
		}
	}
}
