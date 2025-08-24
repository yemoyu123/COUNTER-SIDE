using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Mode;
using ClientPacket.Raid;
using ClientPacket.WorldMap;
using NKC.Trim;
using NKC.UI.Result;
using NKC.UI.Shop;
using NKC.UI.Trim;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCUIWorldMap : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_world_map_renewal";

	private const string UI_ASSET_NAME = "NKM_UI_WORLD_MAP_RENEWAL_FRONT";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	private readonly List<int> lstResources = new List<int> { 1065, 3, 2, 101 };

	public NKCUIWorldMapBack m_UIWorldmapBack;

	public NKCUIWorldMapCityManagement m_UICityManagement;

	public NKCUIComStateButton m_csbtnEventList;

	public NKCUIComStateButton m_csbtnRaidDeckSetup;

	public NKCUIComStateButton m_csbtnShopShortcut;

	public NKCUIComStateButton m_csbtnRaidCoopRequestAll;

	public NKCUIComStateButton m_csbtnDive;

	public GameObject m_objDiveOnProgress;

	public GameObject m_objDiveLock;

	public Text m_lbDiveDepth;

	private static NKC_WORLD_MAP_WARNING_TYPE m_Reserved_NKC_WORLD_MAP_WARNING_TYPE = NKC_WORLD_MAP_WARNING_TYPE.NWMWT_NONE;

	private static int m_ReservedWarningCityID = -1;

	private static int m_ReservedPinIntroCityID = -1;

	private bool m_bWaitingRaidTopPlayer;

	public GameObject m_objEventListReddot;

	private bool m_bRaidJoinResultRedDot;

	private bool m_bHelpListReddot;

	[Header("SUB UIs")]
	private NKCPopupWorldMapCityUnlock _UICityUnlock;

	private NKCPopupWorldMapEventList _UIEventList;

	private NKCPopupTopPlayer _UITopPlayer;

	private bool m_bShowIntro;

	private Coroutine m_crtSDStartCameraMove;

	private NKM_WORLDMAP_EVENT_TYPE m_eCurrentSDCameraEventType = NKM_WORLDMAP_EVENT_TYPE.WET_NONE;

	private int m_iMissionCompleteReqCnt;

	private Coroutine m_crtChangeSceneAfterFadeout;

	private List<NKCUIResult.CityMissionResultData> m_lstCityMissionResultData = new List<NKCUIResult.CityMissionResultData>();

	private bool m_bGotCityMissionNewEvent;

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

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override string MenuName => NKCUtilString.GET_STRING_WORLDMAP;

	public override eTransitionEffectType eTransitionEffect
	{
		get
		{
			if (!m_bShowIntro)
			{
				return eTransitionEffectType.None;
			}
			return eTransitionEffectType.FadeInOut;
		}
	}

	public override string GuideTempletID => "ARTICLE_WORLDMAP_INFO";

	public override List<int> UpsideMenuShowResourceList => lstResources;

	private NKCPopupWorldMapCityUnlock UICityUnlock
	{
		get
		{
			if (_UICityUnlock == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupWorldMapCityUnlock>("ab_ui_nkm_ui_world_map_renewal", "NKM_UI_WORLD_MAP_RENEWAL_BRANCH_OPEN_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), null);
				_UICityUnlock = loadedUIData.GetInstance<NKCPopupWorldMapCityUnlock>();
				_UICityUnlock.InitUI();
			}
			return _UICityUnlock;
		}
	}

	private NKCPopupWorldMapEventList UIEventList
	{
		get
		{
			if (_UIEventList == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupWorldMapEventList>("ab_ui_nkm_ui_world_map_renewal", "NKM_UI_WORLD_MAP_RENEWAL_EVENT_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				_UIEventList = loadedUIData.GetInstance<NKCPopupWorldMapEventList>();
				_UIEventList.Init();
				NKCUtil.SetGameobjectActive(_UIEventList, bValue: false);
			}
			return _UIEventList;
		}
	}

	private NKCPopupTopPlayer UITopPlayer
	{
		get
		{
			if (_UITopPlayer == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupTopPlayer>("AB_UI_NKM_UI_WORLD_MAP_RENEWAL", "NKM_UI_POPUP_WORLD_MAP_RENEWAL_RANKING", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				_UITopPlayer = loadedUIData.GetInstance<NKCPopupTopPlayer>();
				_UITopPlayer.Init();
				NKCUtil.SetGameobjectActive(_UITopPlayer, bValue: false);
			}
			return _UITopPlayer;
		}
	}

	private NKMWorldMapData WorldmapData => NKCScenManager.CurrentUserData().m_WorldmapData;

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIWorldMap>("ab_ui_nkm_ui_world_map_renewal", "NKM_UI_WORLD_MAP_RENEWAL_FRONT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIWorldMap GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIWorldMap>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public static void SetReservedWarning(NKC_WORLD_MAP_WARNING_TYPE type, int cityID)
	{
		m_Reserved_NKC_WORLD_MAP_WARNING_TYPE = type;
		m_ReservedWarningCityID = cityID;
	}

	public static void SetReservedPinIntroCityID(int cityID)
	{
		m_ReservedPinIntroCityID = cityID;
	}

	private bool IsOpenUITopPlayer()
	{
		if (_UITopPlayer == null)
		{
			return false;
		}
		return UITopPlayer.IsOpen;
	}

	public NKCPopupTopPlayer GetUITopPlayer()
	{
		return UITopPlayer;
	}

	private void ReddotProcess()
	{
		EventListReddotProcess();
	}

	private bool CheckHelpListReddot()
	{
		return m_bHelpListReddot;
	}

	private bool CheckJoinRaidResultReddot()
	{
		return m_bRaidJoinResultRedDot;
	}

	private bool CheckSeasonPointReddot()
	{
		return NKCAlarmManager.CheckRaidSeasonRewardNotify();
	}

	private void EventListReddotProcess()
	{
		NKCUtil.SetGameobjectActive(m_objEventListReddot, CheckEnableEventListReddot());
		bool CheckEnableEventListReddot()
		{
			return (byte)(0u | (CheckHelpListReddot() ? 1u : 0u) | (CheckJoinRaidResultReddot() ? 1u : 0u) | (CheckSeasonPointReddot() ? 1u : 0u)) != 0;
		}
	}

	public void CloseCityManagementUI()
	{
		if (m_UICityManagement != null)
		{
			m_UICityManagement.Close();
		}
	}

	public override void CloseInternal()
	{
		if (m_UICityManagement != null)
		{
			m_UICityManagement.Close();
		}
		base.gameObject.SetActive(value: false);
		NKCUtil.SetGameobjectActive(m_UIWorldmapBack, bValue: false);
		if (NKCPopupWarning.IsInstanceOpen)
		{
			NKCPopupWarning.Instance.Close();
		}
		if (m_crtSDStartCameraMove != null)
		{
			StopCoroutine(m_crtSDStartCameraMove);
			m_crtSDStartCameraMove = null;
		}
		ClearCoroutineChangeSceneAfterFadeout();
		NKCCamera.DisableFocusBlur();
		NKCCamera.TurnOffCrashUpDown();
		NKCUIManager.SetScreenInputBlock(bSet: false);
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
		Object.Destroy(m_UIWorldmapBack.gameObject);
	}

	public override void OnBackButton()
	{
		if (m_UICityManagement.IsOpen)
		{
			m_UICityManagement.Close();
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
		}
	}

	public override void Hide()
	{
		base.Hide();
		NKCUtil.SetGameobjectActive(m_UIWorldmapBack, bValue: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		NKCUtil.SetGameobjectActive(m_UIWorldmapBack, bValue: true);
		NKCUtil.SetGameobjectActive(m_objDiveLock, !NKCContentManager.IsContentsUnlocked(ContentsType.DIVE));
		m_UIWorldmapBack.SetData(NKCScenManager.CurrentUserData().m_WorldmapData);
		SetRaidBatchSupportButtonState();
		if (m_UICityManagement != null && m_UICityManagement.IsOpen)
		{
			m_UICityManagement.Unhide();
		}
	}

	public void Init()
	{
		if (m_UIWorldmapBack != null)
		{
			m_UIWorldmapBack.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas), worldPositionStays: false);
			m_UIWorldmapBack.transform.position = Vector3.zero;
			m_UIWorldmapBack.Init(SelectCity, OnSelectEvent);
		}
		else
		{
			Debug.LogError("Worldmap Back Not Found!!!");
		}
		if (m_UICityManagement != null)
		{
			m_UICityManagement.Init(SelectNextCity, UnselectCity);
		}
		if (m_csbtnEventList != null)
		{
			m_csbtnEventList.PointerClick.RemoveAllListeners();
			m_csbtnEventList.PointerClick.AddListener(OnClickEventList);
		}
		else
		{
			Debug.LogError("WorldMap : EventListBtn Not Connected");
		}
		if (m_csbtnRaidDeckSetup != null)
		{
			m_csbtnRaidDeckSetup.PointerClick.RemoveAllListeners();
			m_csbtnRaidDeckSetup.PointerClick.AddListener(OnClickRaidDeckSetup);
		}
		else
		{
			Debug.LogError("WorldMap : RaidDeckSetupBtn Not Connected");
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnShopShortcut, OnClickShopShortcut);
		if (m_csbtnDive != null)
		{
			m_csbtnDive.PointerClick.RemoveAllListeners();
			m_csbtnDive.PointerClick.AddListener(OnClickDive);
		}
		else
		{
			Debug.LogError("WorldMap : DiveBtn Not Connected");
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnRaidCoopRequestAll, OnClickRaidCoopRequestAll);
	}

	public void Open(bool bDiveReverseAni)
	{
		m_bWaitingRaidTopPlayer = false;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKCUtil.SetGameobjectActive(m_UIWorldmapBack, bValue: true);
		NKCUtil.SetGameobjectActive(m_objDiveLock, !NKCContentManager.IsContentsUnlocked(ContentsType.DIVE));
		m_UIWorldmapBack.SetData(nKMUserData.m_WorldmapData);
		m_UIWorldmapBack.SetEnableDrag(bSet: true);
		NKCUtil.SetGameobjectActive(m_objEventListReddot, bValue: false);
		bool flag = NKMTutorialManager.IsTutorialCompleted(TutorialStep.RaidEventCheck, NKCScenManager.CurrentUserData());
		NKCUtil.SetGameobjectActive(m_csbtnEventList, flag);
		NKCUtil.SetGameobjectActive(m_csbtnRaidDeckSetup, flag);
		if (flag)
		{
			NKCPacketSender.Send_NKMPacket_RAID_COOP_LIST_REQ();
			NKCPacketSender.Send_NKMPacket_RAID_RESULT_LIST_REQ();
		}
		SetDiveButtonProgress();
		SetRaidBatchSupportButtonState();
		UnselectCity();
		UIOpened();
		ShowReservedWarning();
		if (bDiveReverseAni)
		{
			m_UIWorldmapBack.m_amtorWorldmapBack.Play("NKM_UI_WORLD_MAP_RENEWAL_DIVEINTRO_REVERSE");
		}
		TutorialCheck();
	}

	private void CleanUpEventPinSpineSD(int cityID)
	{
		m_UIWorldmapBack.CleanUpEventPinSpineSD(cityID);
	}

	private void DoAfterCloseWarning(bool bCanceled)
	{
		Debug.Log("DoAfterCloseWarning " + bCanceled);
		m_UIWorldmapBack.PlayPinSDAniByCityID(m_ReservedPinIntroCityID, NKCASUIUnitIllust.eAnimation.SD_START);
		if (!bCanceled)
		{
			m_crtSDStartCameraMove = StartCoroutine(ProcessSDStartCamera(m_ReservedPinIntroCityID));
		}
		else
		{
			m_UIWorldmapBack.SetEnableDrag(bSet: true);
		}
		m_ReservedPinIntroCityID = -1;
	}

	private IEnumerator ProcessSDStartCamera(int cityID)
	{
		NKCUIManager.SetScreenInputBlock(bSet: true);
		NKMWorldMapCityData cityData = NKCScenManager.CurrentUserData().m_WorldmapData.GetCityData(cityID);
		if (cityData == null)
		{
			yield break;
		}
		NKMWorldMapEventTemplet cNKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(cityData.worldMapEventGroup.worldmapEventID);
		if (cNKMWorldMapEventTemplet == null)
		{
			yield break;
		}
		m_eCurrentSDCameraEventType = cNKMWorldMapEventTemplet.eventType;
		if (cNKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
		{
			if (cNKMWorldMapEventTemplet.raidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_SOLO_RAID)
			{
				NKCSoundManager.PlaySound("FX_UI_WORLDMAP_BOSS_RAID_HAPPEN", 1f, 0f, 0f);
				Vector3 cameraOrgPos = new Vector3(NKCCamera.GetPosNowX(), NKCCamera.GetPosNowY(), NKCCamera.GetPosNowZ());
				Vector3 pinSDPos = m_UIWorldmapBack.GetPinSDPos(cityID);
				NKCCamera.TrackingPos(0.6f, pinSDPos.x, pinSDPos.y, -300f);
				NKCCamera.SetFocusBlur(1.7f);
				yield return new WaitForSecondsWithCancel(1.6f, CanSkipSDCamera, null);
				NKCCamera.TrackingPos(0.6f, cameraOrgPos.x, cameraOrgPos.y, cameraOrgPos.z);
				yield return new WaitForSecondsWithCancel(0.6f, CanSkipSDCamera, null);
				m_UIWorldmapBack.SetEnableDrag(bSet: true);
				NKCUIManager.SetScreenInputBlock(bSet: false);
				CheckTutorialEvent(cNKMWorldMapEventTemplet.eventType);
			}
			else
			{
				NKCSoundManager.PlaySound("FX_UI_WORLDMAP_BOSS_RAID_HAPPEN", 1f, 0f, 0f);
				Vector3 cameraOrgPos = new Vector3(NKCCamera.GetPosNowX(), NKCCamera.GetPosNowY(), NKCCamera.GetPosNowZ());
				Vector3 pinSDPos2 = m_UIWorldmapBack.GetPinSDPos(cityID);
				NKCCamera.TrackingPos(0.6f, pinSDPos2.x, pinSDPos2.y, -300f);
				NKCCamera.SetFocusBlur(3.7f);
				yield return new WaitForSecondsWithCancel(2.2f, CanSkipSDCamera, null);
				NKCCamera.UpDownCrashCamera(5f, 1.5f, 0.025f);
				yield return new WaitForSecondsWithCancel(1.6f, CanSkipSDCamera, null);
				NKCCamera.TrackingPos(0.6f, cameraOrgPos.x, cameraOrgPos.y, cameraOrgPos.z);
				yield return new WaitForSecondsWithCancel(0.6f, CanSkipSDCamera, null);
				m_UIWorldmapBack.SetEnableDrag(bSet: true);
				NKCUIManager.SetScreenInputBlock(bSet: false);
				CheckTutorialEvent(cNKMWorldMapEventTemplet.eventType);
			}
		}
		else if (cNKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_DIVE)
		{
			NKCSoundManager.PlaySound("FX_UI_WORLDMAP_DIVE_HAPPEN", 1f, 0f, 0f);
			Vector3 cameraOrgPos = new Vector3(NKCCamera.GetPosNowX(), NKCCamera.GetPosNowY(), NKCCamera.GetPosNowZ());
			Vector3 pinSDPos3 = m_UIWorldmapBack.GetPinSDPos(cityID);
			NKCCamera.TrackingPos(0.6f, pinSDPos3.x, pinSDPos3.y, -400f);
			yield return new WaitForSecondsWithCancel(3.3f, CanSkipSDCamera, null);
			NKCCamera.TrackingPos(0.6f, cameraOrgPos.x, cameraOrgPos.y, cameraOrgPos.z);
			yield return new WaitForSecondsWithCancel(0.6f, CanSkipSDCamera, null);
			m_UIWorldmapBack.SetEnableDrag(bSet: true);
			NKCUIManager.SetScreenInputBlock(bSet: false);
			CheckTutorialEvent(cNKMWorldMapEventTemplet.eventType);
		}
	}

	private bool CanSkipSDCamera()
	{
		if (Input.anyKeyDown)
		{
			return IsTutorialForEventCompleted(m_eCurrentSDCameraEventType);
		}
		return false;
	}

	public void ShowReservedWarning()
	{
		if (m_Reserved_NKC_WORLD_MAP_WARNING_TYPE == NKC_WORLD_MAP_WARNING_TYPE.NWMWT_RAID)
		{
			NKCPopupWarning.Instance.Open(NKCUtilString.GET_STRING_WORLDMAP_EVENT_WARNING);
			CleanUpEventPinSpineSD(m_ReservedWarningCityID);
			m_UIWorldmapBack.PlayPinSDAniByCityID(m_ReservedPinIntroCityID, NKCASUIUnitIllust.eAnimation.SD_START);
		}
		else if (m_Reserved_NKC_WORLD_MAP_WARNING_TYPE == NKC_WORLD_MAP_WARNING_TYPE.NWMWT_DIVE)
		{
			NKCPopupWarning.Instance.Open(NKCUtilString.GET_STRING_WORLDMAP_EVENT_WARNING);
			CleanUpEventPinSpineSD(m_ReservedWarningCityID);
			m_UIWorldmapBack.PlayPinSDAniByCityID(m_ReservedPinIntroCityID, NKCASUIUnitIllust.eAnimation.SD_START);
		}
		m_Reserved_NKC_WORLD_MAP_WARNING_TYPE = NKC_WORLD_MAP_WARNING_TYPE.NWMWT_NONE;
		m_ReservedWarningCityID = -1;
	}

	private void SetDiveButtonProgress()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool bValue = false;
		if (nKMUserData.m_DiveGameData != null && !nKMUserData.m_DiveGameData.Floor.Templet.IsEventDive)
		{
			bValue = true;
		}
		NKCUtil.SetGameobjectActive(m_objDiveOnProgress, bValue);
		NKCUtil.SetGameobjectActive(m_objDiveLock, !NKCContentManager.IsContentsUnlocked(ContentsType.DIVE));
		int selectedIndex;
		NKMDiveTemplet currNormalDiveTemplet = NKCDiveManager.GetCurrNormalDiveTemplet(out selectedIndex);
		if (currNormalDiveTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbDiveDepth, currNormalDiveTemplet.Get_STAGE_NAME_SUB());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDiveDepth, string.Empty);
		}
	}

	public void SetRaidBatchSupportButtonState()
	{
		bool flag = NKCUtil.CanRaidCoop();
		m_csbtnRaidCoopRequestAll?.SetLock(!flag);
	}

	private void SelectCity(int cityID)
	{
		if (cityID == 0)
		{
			m_UICityManagement.Close();
		}
		else if (IsCityUnlocked(cityID))
		{
			_OnSelectCity(cityID);
		}
		else
		{
			UICityUnlock.Open(cityID);
		}
	}

	private void _OnSelectCity(int CityID)
	{
		m_bGotCityMissionNewEvent = false;
		m_iMissionCompleteReqCnt = 0;
		m_lstCityMissionResultData.Clear();
		NKMWorldMapCityData cityData = WorldmapData.GetCityData(CityID);
		if (cityData != null && cityData.worldMapMission.completeTime != 0L && NKCSynchronizedTime.IsFinished(cityData.worldMapMission.completeTime))
		{
			List<int> list = new List<int>();
			foreach (NKMWorldMapCityData value in WorldmapData.worldMapCityDataMap.Values)
			{
				if (value != null && value.worldMapMission.completeTime != 0L && NKCSynchronizedTime.IsFinished(value.worldMapMission.completeTime))
				{
					list.Add(value.cityID);
				}
			}
			if (list.Count >= 1)
			{
				foreach (int item in list)
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_MISSION_COMPLETE_REQ(item);
					m_iMissionCompleteReqCnt++;
				}
				return;
			}
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_MISSION_COMPLETE_REQ(CityID);
		}
		else
		{
			m_UICityManagement.Open(cityData, delegate
			{
				NKCUtil.SetGameobjectActive(m_UIWorldmapBack, bValue: false);
			}, delegate
			{
				NKCUtil.SetGameobjectActive(m_UIWorldmapBack, bValue: true);
				m_UIWorldmapBack.SetData(NKCScenManager.CurrentUserData().m_WorldmapData);
			});
		}
	}

	private void SelectNextCity(int currentCityID, bool bForward)
	{
		List<int> list = new List<int>(WorldmapData.worldMapCityDataMap.Keys);
		list.Sort();
		int num = list.IndexOf(currentCityID);
		if (num < 0)
		{
			Debug.LogError("Selected city not found");
		}
		else
		{
			if (list.Count <= 1)
			{
				return;
			}
			int num2 = (bForward ? 1 : (-1));
			for (int i = 1; i < list.Count; i++)
			{
				int index = num + num2 * i;
				index = NKCUtil.CalculateNormalizedIndex(index, list.Count);
				int cityID = list[index];
				if (IsCityUnlocked(cityID))
				{
					SelectCity(cityID);
					break;
				}
			}
		}
	}

	public void UnselectCity()
	{
		SelectCity(0);
	}

	private bool IsCityUnlocked(int cityID)
	{
		return NKCScenManager.CurrentUserData().m_WorldmapData.IsCityUnlocked(cityID);
	}

	private void OnSelectEvent(int cityID, int eventID, long eventUID)
	{
		NKMWorldMapCityData cityData = NKCScenManager.CurrentUserData().m_WorldmapData.GetCityData(cityID);
		if (cityData == null)
		{
			Debug.LogError($"City not found. ID : {cityID}");
			return;
		}
		NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(eventID);
		if (nKMWorldMapEventTemplet == null)
		{
			Debug.LogError($"EventTemplet Null! ID : {eventID}");
			return;
		}
		switch (nKMWorldMapEventTemplet.eventType)
		{
		case NKM_WORLDMAP_EVENT_TYPE.WET_DIVE:
		{
			bool flag = NKCScenManager.CurrentUserData().m_DiveGameData != null && NKCScenManager.CurrentUserData().m_DiveGameData.DiveUid == cityData.worldMapEventGroup.eventUid;
			if (NKCSynchronizedTime.IsFinished(cityData.worldMapEventGroup.eventGroupEndDate) || (cityData.worldMapEventGroup.eventUid > 0 && !flag))
			{
				NKCPacketSender.Send_NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_REQ(cityData.cityID);
				break;
			}
			UIEventList.Close();
			ShowDivingAndGotoDiveScen(cityID, nKMWorldMapEventTemplet.stageID);
			break;
		}
		case NKM_WORLDMAP_EVENT_TYPE.WET_RAID:
			if (NKCScenManager.GetScenManager().GetNKCRaidDataMgr().CheckCompletableRaid(cityData.worldMapEventGroup.eventUid))
			{
				NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(cityData.worldMapEventGroup.eventUid);
				if (nKMRaidDetailData != null && nKMRaidDetailData.curHP <= 0f)
				{
					if (!m_bWaitingRaidTopPlayer)
					{
						m_bWaitingRaidTopPlayer = true;
						NKCPacketSender.Send_NKMPacket_RAID_DETAIL_INFO_REQ(cityData.worldMapEventGroup.eventUid);
					}
				}
				else
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetRaidUID(eventUID);
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID);
				}
			}
			else
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetRaidUID(eventUID);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID);
			}
			break;
		}
	}

	private void ShowDivingAndGotoDiveScen(int cityID = 0, int eventDiveID = 0)
	{
		ChangeSceneAfterDiveEffect(ContentsType.DIVE, cityID, eventDiveID);
	}

	private void OnClickDive()
	{
		if (NKCContentManager.IsContentsUnlocked(ContentsType.DIVE))
		{
			ShowDivingAndGotoDiveScen();
		}
		else
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.DIVE);
		}
	}

	private void OnClickShadow()
	{
		if (NKCContentManager.IsContentsUnlocked(ContentsType.SHADOW_PALACE))
		{
			ChangeSceneAfterDiveEffect(ContentsType.SHADOW_PALACE);
		}
		else
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.SHADOW_PALACE);
		}
	}

	private void OnClickTrim()
	{
		if (!NKCUITrimUtility.OpenTagEnabled)
		{
			return;
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.DIMENSION_TRIM))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.DIMENSION_TRIM);
		}
		else if (NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime) == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRIM_NOT_INTERVAL_TIME);
		}
		else if (NKCTrimManager.TrimModeState != null)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRIM_EXIST_TRIM_COMBAT_DATA, delegate
			{
				NKCTrimManager.ProcessTrim();
			});
		}
		else
		{
			ChangeSceneAfterDiveEffect(ContentsType.DIMENSION_TRIM);
		}
	}

	private void OnClickRaidCoopRequestAll()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_WORLDMAP_RAID_COOP_REQUEST_ALL, NKCPacketSender.Send_NKMPacket_RAID_SET_COOP_ALL_REQ);
	}

	private void ChangeSceneAfterDiveEffect(ContentsType type, int cityID = 0, int eventDiveID = 0)
	{
		ClearCoroutineChangeSceneAfterFadeout();
		m_UIWorldmapBack.m_amtorWorldmapBack.Play("NKM_UI_WORLD_MAP_RENEWAL_DIVEINTRO");
		m_crtChangeSceneAfterFadeout = StartCoroutine(ChangeSceneAfterFadeout(type, cityID, eventDiveID));
	}

	private void ClearCoroutineChangeSceneAfterFadeout()
	{
		if (m_crtChangeSceneAfterFadeout != null)
		{
			StopCoroutine(m_crtChangeSceneAfterFadeout);
			m_crtChangeSceneAfterFadeout = null;
		}
	}

	private IEnumerator ChangeSceneAfterFadeout(ContentsType type, int cityID = 0, int eventDiveID = 0)
	{
		yield return new WaitForSeconds(0.25f);
		NKCUIFadeInOut.FadeOut(0.25f, delegate
		{
			NKCScenManager.GetScenManager().SetSkipScenChangeFadeOutEffect(bSet: true);
			switch (type)
			{
			case ContentsType.DIVE:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OpenDiveSearch(cityID, eventDiveID);
				break;
			case ContentsType.SHADOW_PALACE:
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().OpenShadowPalace();
				break;
			case ContentsType.FIERCE:
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT);
				break;
			case ContentsType.DIMENSION_TRIM:
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_TRIM);
				break;
			}
		});
		m_crtChangeSceneAfterFadeout = null;
	}

	private bool IsOpenUIEventList()
	{
		if (_UIEventList == null)
		{
			return false;
		}
		return UIEventList.IsOpen;
	}

	private void OnClickEventList()
	{
		OpenEventList(selectLastTab: false);
	}

	public void OpenEventList(bool selectLastTab)
	{
		UIEventList.Open(WorldmapData, OnSelectEvent, CheckHelpListReddot(), CheckJoinRaidResultReddot(), selectLastTab);
	}

	private void OnClickRaidDeckSetup()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().SetRaidUID(0L);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID_READY().SetGuildRaid(bGuildRaid: false);
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID_READY);
	}

	private void OnClickShopShortcut()
	{
		NKCUIShop.ShopShortcut("TAB_EXCHANGE_DIVE_POINT", 1);
	}

	public void OnRecv(NKMPacket_RAID_RESULT_ACCEPT_ACK cNKMPacket_RAID_RESULT_ACCEPT_ACK, int cityID)
	{
		if (IsOpenUIEventList())
		{
			UIEventList.OnRecv(cNKMPacket_RAID_RESULT_ACCEPT_ACK, cityID);
		}
	}

	public void OnRecv(NKMPacket_RAID_RESULT_ACCEPT_ALL_ACK sPacket, List<int> lstCity)
	{
		if (IsOpenUIEventList())
		{
			UIEventList.OnRecv(sPacket, lstCity);
		}
	}

	public void OnRecv(NKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK cNKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK)
	{
		if (base.IsOpen)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(cNKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK.cityID);
				if (cityData != null)
				{
					CityDataUpdated(cityData);
				}
			}
		}
		if (IsOpenUIEventList())
		{
			UIEventList.OnRecv(cNKMPacket_WORLDMAP_REMOVE_EVENT_DUNGEON_ACK);
		}
	}

	public void OnRecv(NKMPacket_RAID_POINT_EXTRA_REWARD_ACK sPacket)
	{
		if (IsOpenUIEventList())
		{
			UIEventList.OnRecv(sPacket);
		}
	}

	public void OnRecv(NKMPacket_WORLDMAP_EVENT_CANCEL_ACK cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK)
	{
		if (base.IsOpen)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				NKMWorldMapCityData cityData = nKMUserData.m_WorldmapData.GetCityData(cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK.cityID);
				if (cityData != null)
				{
					CityDataUpdated(cityData);
				}
			}
		}
		if (IsOpenUIEventList())
		{
			UIEventList.OnRecv(cNKMPacket_WORLDMAP_EVENT_CANCEL_ACK);
		}
	}

	public void OnRecv(NKMPacket_RAID_RESULT_LIST_ACK cNKMPacket_RAID_RESULT_LIST_ACK)
	{
		m_bRaidJoinResultRedDot = false;
		foreach (NKMRaidResultData raidResultData in cNKMPacket_RAID_RESULT_LIST_ACK.raidResultDataList)
		{
			if (!raidResultData.IsOnGoing())
			{
				m_bRaidJoinResultRedDot = true;
				break;
			}
		}
		if (IsOpenUIEventList())
		{
			UIEventList.OnRecv(cNKMPacket_RAID_RESULT_LIST_ACK);
		}
		ReddotProcess();
	}

	public void OnRecv(NKMPacket_RAID_COOP_LIST_ACK cNKMPacket_RAID_COOP_LIST_ACK)
	{
		m_bHelpListReddot = false;
		if (cNKMPacket_RAID_COOP_LIST_ACK != null && cNKMPacket_RAID_COOP_LIST_ACK.coopRaidDataList != null && cNKMPacket_RAID_COOP_LIST_ACK.coopRaidDataList.Count > 0)
		{
			m_bHelpListReddot = true;
		}
		if (IsOpenUIEventList())
		{
			UIEventList.OnRecv(cNKMPacket_RAID_COOP_LIST_ACK);
		}
		ReddotProcess();
	}

	public void OnRecv(NKMPacket_DIVE_EXPIRE_NOT cNKMPacket_DIVE_EXPIRE_NOT)
	{
		SetDiveButtonProgress();
		if (IsOpenUIEventList())
		{
			UIEventList.OnRecv(cNKMPacket_DIVE_EXPIRE_NOT);
		}
	}

	public void OnRecv(NKMRaidDetailData raidDetailData, bool bForce = false)
	{
		m_UIWorldmapBack.UpdateCityRaidData(raidDetailData);
		if (m_bWaitingRaidTopPlayer || bForce)
		{
			m_bWaitingRaidTopPlayer = false;
			NKMRaidTemplet raidTemplet = NKMRaidTemplet.Find(raidDetailData.stageID);
			OpenTopPlayerPopup(raidTemplet, raidDetailData.raidJoinDataList, raidDetailData.raidUID);
		}
	}

	public void OnRecv(NKMPacket_RAID_SEASON_NOT sPacket)
	{
		if (IsOpenUIEventList())
		{
			UIEventList.OnRecv(sPacket);
		}
		ReddotProcess();
	}

	public void OnRecv(NKMPacket_WORLDMAP_MISSION_COMPLETE_ACK sPacket, NKMWorldMapCityData cityData, NKCUIResult.CityMissionResultData cityUIData, bool bGotNewEvent)
	{
		m_iMissionCompleteReqCnt--;
		if (cityData == null)
		{
			Debug.LogError("FATAL : City/Area Does not exist, Cilent-Server Templet info sync off");
			return;
		}
		NKCScenManager.GetScenManager().GetMyUserData();
		m_lstCityMissionResultData.Add(cityUIData);
		if (bGotNewEvent && !m_bGotCityMissionNewEvent)
		{
			m_bGotCityMissionNewEvent = true;
		}
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(cityData.leaderUnitUID);
		if (unitFromUID != null)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_WORLDMAP_MISSION_COMPLETE, unitFromUID);
		}
		if (bGotNewEvent)
		{
			NKMWorldMapEventTemplet nKMWorldMapEventTemplet = NKMWorldMapEventTemplet.Find(sPacket.worldMapEventGroup.worldmapEventID);
			if (nKMWorldMapEventTemplet != null)
			{
				if (nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_RAID)
				{
					NKCPacketSender.Send_NKMPacket_MY_RAID_LIST_REQ();
					NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReservedWarning(NKC_WORLD_MAP_WARNING_TYPE.NWMWT_RAID, cityData.cityID);
				}
				else if (nKMWorldMapEventTemplet.eventType == NKM_WORLDMAP_EVENT_TYPE.WET_DIVE)
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReservedWarning(NKC_WORLD_MAP_WARNING_TYPE.NWMWT_DIVE, cityData.cityID);
				}
				else
				{
					NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReservedWarning(NKC_WORLD_MAP_WARNING_TYPE.NWMWT_NONE, -1);
				}
			}
		}
		if (m_iMissionCompleteReqCnt != 0)
		{
			return;
		}
		if (NKCPopupOKCancel.isOpen())
		{
			NKCPopupOKCancel.ClosePopupBox();
		}
		m_lstCityMissionResultData.OrderBy((NKCUIResult.CityMissionResultData x) => x.m_CityID).ToList();
		if (m_bGotCityMissionNewEvent)
		{
			NKCUIResult.Instance.OpenCityMissionResult(m_lstCityMissionResultData, delegate
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().CloseCityManageUI();
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().ShowReservedWarningUI();
			});
		}
		else
		{
			NKCUIResult.Instance.OpenCityMissionResult(m_lstCityMissionResultData, null);
		}
	}

	public void UpdateCityRaidData(NKMRaidDetailData raidDetailData)
	{
		m_UIWorldmapBack.UpdateCityRaidData(raidDetailData);
	}

	public void RefreshEventList()
	{
		if (IsOpenUIEventList())
		{
			UIEventList.RefreshEventDataList(WorldmapData);
			UIEventList.RefreshUI();
		}
	}

	public void OpenTopPlayerPopup(NKMRaidTemplet raidTemplet, List<NKMRaidJoinData> raidJoinDataList, long raidUID)
	{
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(raidUID);
		if (nKMRaidDetailData == null)
		{
			return;
		}
		if (NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate) && nKMRaidDetailData.curHP > 0f)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_RAID().SetRaidUID(raidUID);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_RAID);
			return;
		}
		string title_ = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, raidTemplet.RaidLevel);
		string dungeonName = raidTemplet.DungeonTempletBase.GetDungeonName();
		string subTitle = NKCStringTable.GetString("SI_PF_WORLD_MAP_RENEWAL_RANKING_TITLE");
		UITopPlayer.Open(title_, dungeonName, subTitle, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UNIT_FACE_CARD", raidTemplet.FaceCardName), LeaderBoardSlotData.MakeSlotDataList(raidJoinDataList, raidTemplet.RaidTryCount), new List<NKMEmblemData>(), raidUID, delegate(long uid)
		{
			NKCPacketSender.Send_NKMPacket_RAID_RESULT_ACCEPT_REQ(uid);
			NKCPacketSender.Send_NKMPacket_RAID_RESULT_LIST_REQ();
		});
	}

	public void OnWorldManCitySet(int cityID, NKMWorldMapCityData cityData)
	{
		if (_UICityUnlock != null && _UICityUnlock.IsOpen)
		{
			_UICityUnlock.Close();
		}
		m_UIWorldmapBack.UpdateCity(cityID, cityData);
		SelectCity(cityID);
		ShowAreaUnlockedEffect();
	}

	public void CityLeaderChanged(NKMWorldMapCityData cityData)
	{
		if (cityData != null)
		{
			m_UIWorldmapBack.UpdateCity(cityData.cityID, cityData);
			if (m_UICityManagement != null && m_UICityManagement.IsOpen)
			{
				m_UICityManagement.CityDataUpdated(cityData);
			}
		}
	}

	public void CityDataUpdated(NKMWorldMapCityData cityData)
	{
		if (cityData != null)
		{
			m_UIWorldmapBack.UpdateCity(cityData.cityID, cityData);
			if (m_UICityManagement != null && m_UICityManagement.IsOpen)
			{
				m_UICityManagement.CityDataUpdated(cityData);
			}
		}
	}

	public void CityEventSpawned(int cityID)
	{
		m_UIWorldmapBack.CityEventSpawned(cityID);
	}

	private void ShowAreaUnlockedEffect()
	{
		NKCUICompleteEffect.Instance.OpenCityOpened();
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (m_UICityManagement.IsOpen)
		{
			m_UICityManagement.OnInventoryChange(itemData);
		}
	}

	public void SetFXBuildingID(int buildingID)
	{
		if (m_UICityManagement != null && m_UICityManagement.IsOpen)
		{
			m_UICityManagement.SetFXBuildingID(buildingID);
		}
	}

	public RectTransform GetPinRect(int cityID)
	{
		return m_UIWorldmapBack.GetPinRect(cityID);
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.WorldMap);
	}

	private bool IsTutorialForEventCompleted(NKM_WORLDMAP_EVENT_TYPE type)
	{
		return type switch
		{
			NKM_WORLDMAP_EVENT_TYPE.WET_RAID => NKCTutorialManager.TutorialRequired(TutorialPoint.RaidWarning, play: false) == TutorialStep.None, 
			NKM_WORLDMAP_EVENT_TYPE.WET_DIVE => NKCTutorialManager.TutorialRequired(TutorialPoint.DiveWarning) == TutorialStep.None, 
			_ => true, 
		};
	}

	private void CheckTutorialEvent(NKM_WORLDMAP_EVENT_TYPE type)
	{
		switch (type)
		{
		case NKM_WORLDMAP_EVENT_TYPE.WET_RAID:
			NKCTutorialManager.TutorialRequired(TutorialPoint.RaidWarning);
			break;
		case NKM_WORLDMAP_EVENT_TYPE.WET_DIVE:
			NKCTutorialManager.TutorialRequired(TutorialPoint.DiveWarning);
			break;
		}
	}

	public static NKCASUIUnitIllust.eAnimation GetMissionProgressAnimationType(NKMWorldMapMissionTemplet.WorldMapMissionType missionType)
	{
		switch (missionType)
		{
		case NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_OFFICE:
			return NKCASUIUnitIllust.eAnimation.SD_WORKING;
		case NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_MINING:
			return NKCASUIUnitIllust.eAnimation.SD_MINING;
		case NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_DEFENCE:
			return NKCASUIUnitIllust.eAnimation.SD_ATTACK;
		case NKMWorldMapMissionTemplet.WorldMapMissionType.WMT_EXPLORE:
			return NKCASUIUnitIllust.eAnimation.SD_RUN;
		default:
			Debug.LogError("Mission templet missiontype undefined");
			return NKCASUIUnitIllust.eAnimation.SD_WALK;
		}
	}
}
