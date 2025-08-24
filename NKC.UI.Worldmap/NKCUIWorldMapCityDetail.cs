using System.Collections;
using System.Collections.Generic;
using ClientPacket.WorldMap;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCUIWorldMapCityDetail : MonoBehaviour
{
	private enum EmotionType
	{
		None,
		Heart
	}

	public delegate void OnSelectNextCity(int currentCityID, bool bForward);

	public delegate void OnExit();

	[Header("상단부")]
	public Text m_lbCityLevel;

	public Image m_imgCityExp;

	public Text m_lbCityName;

	public Text m_lbCityTitle;

	[Header("중단부")]
	public RectTransform m_rtSDRoot;

	public RectTransform m_rtSDMoveRange;

	public float m_fSDScale = 1.2f;

	public Image m_imgSDEmotion;

	public NKCUIComTextUnitLevel m_lbLeaderLevel;

	public Text m_lbLeaderName;

	private int m_currentSDUnitID = -1;

	private int m_currentSDSkinID = -1;

	private NKCASUIUnitIllust m_spineSD;

	public Image m_imgOffice;

	public GameObject m_objLeaderSeized;

	[Header("하단부")]
	public NKCUIComStateButton m_csbtnSetLeader;

	public Text m_lbSetLeader;

	public Image m_imgSetLeader;

	public Sprite m_spAddLeader;

	public Sprite m_spChangeLeader;

	[Header("이벤트")]
	public EventTrigger m_evtSwipe;

	public float m_swipeDelta = 10f;

	public GameObject m_objChangeCity;

	public NKCUIComStateButton m_csbtnNextCity;

	public NKCUIComStateButton m_csbtnPrevCity;

	private int m_CityID;

	private bool bSwipePossible = true;

	private NKMWorldMapCityData m_CityData;

	private OnSelectNextCity dOnSelectNextCity;

	private OnExit dOnExit;

	public float MoveSpeed = 50f;

	public void Init(OnSelectNextCity onSelectNextCity, OnExit onExit)
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Drag;
		entry.callback.AddListener(CitySwipe);
		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.BeginDrag;
		entry2.callback.AddListener(BeginDrag);
		m_evtSwipe.triggers.Clear();
		m_evtSwipe.triggers.Add(entry);
		m_evtSwipe.triggers.Add(entry2);
		dOnSelectNextCity = onSelectNextCity;
		dOnExit = onExit;
		m_csbtnSetLeader.PointerClick.AddListener(OnBtnLeaderSelect);
		NKCUtil.SetButtonClickDelegate(m_csbtnNextCity, SelectNextCity);
		NKCUtil.SetButtonClickDelegate(m_csbtnPrevCity, SelectPrevCity);
	}

	public void CleanUp()
	{
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		ResetSDPosition();
		m_currentSDUnitID = -1;
		m_currentSDSkinID = -1;
		m_spineSD = null;
		m_CityData = null;
	}

	public void SetData(NKMWorldMapCityData cityData)
	{
		if (cityData == null)
		{
			Debug.Log("CityData Null!!");
			return;
		}
		NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(cityData.cityID);
		if (cityTemplet == null)
		{
			Debug.LogError($"Fatal : CityTemplet not exist(id : {cityData.cityID}). server/client off sync");
			return;
		}
		m_CityID = cityTemplet.m_ID;
		m_CityData = cityData;
		NKCUtil.SetLabelText(m_lbCityName, cityTemplet.GetName());
		NKCUtil.SetLabelText(m_lbCityTitle, cityTemplet.GetTitle());
		NKCUtil.SetLabelText(m_lbCityLevel, cityData.level.ToString());
		SetExpBar(cityData);
		SetLeaderData(cityData);
		SetOffice(cityData);
		ShowEmotion(EmotionType.None, 0f);
		NKMWorldMapData nKMWorldMapData = NKCScenManager.CurrentUserData()?.m_WorldmapData;
		NKCUtil.SetGameobjectActive(m_objChangeCity, nKMWorldMapData != null && nKMWorldMapData.GetUnlockedCityCount() > 1);
	}

	private void SetExpBar(NKMWorldMapCityData cityData)
	{
		if (m_imgCityExp != null)
		{
			m_imgCityExp.fillAmount = NKMWorldMapManager.GetCityExpPercent(cityData);
		}
	}

	private void SetOffice(NKMWorldMapCityData cityData)
	{
		string bundleName = "AB_UI_NKM_UI_WORLD_MAP_RENEWAL_OFFICE";
		string assetName = string.Empty;
		Dictionary<int, NKMWorldmapCityBuildingData>.Enumerator enumerator = cityData.worldMapCityBuildingDataMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = NKMWorldMapBuildingTemplet.Find(enumerator.Current.Value.id).GetLevelTemplet(enumerator.Current.Value.level);
			if (levelTemplet == null)
			{
				Debug.Log($"buildingTemplet.GetLevelTemplet({enumerator.Current.Value.level}) is null");
			}
			else if (!string.IsNullOrEmpty(levelTemplet.ManagerRoomPath))
			{
				assetName = levelTemplet.ManagerRoomPath;
				break;
			}
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(bundleName, assetName);
		if (orLoadAssetResource != null)
		{
			m_imgOffice.sprite = orLoadAssetResource;
		}
	}

	public void SetLeaderData(NKMWorldMapCityData cityData)
	{
		if (cityData != null)
		{
			if (cityData.leaderUnitUID == 0L)
			{
				NKCUtil.SetLabelText(m_lbSetLeader, NKCUtilString.GET_STRING_WORLDMAP_CITY_SET_LEADER);
				NKCUtil.SetImageSprite(m_imgSetLeader, m_spAddLeader);
				NKCUtil.SetLabelText(m_lbLeaderLevel, "");
				NKCUtil.SetLabelText(m_lbLeaderName, "");
				OpenSDIllust(null);
				NKCUtil.SetGameobjectActive(m_objLeaderSeized, bValue: false);
			}
			else
			{
				NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
				if (nKMUserData == null)
				{
					return;
				}
				NKCUtil.SetLabelText(m_lbSetLeader, NKCUtilString.GET_STRING_WORLDMAP_CITY_CHANGE_LEADER);
				NKCUtil.SetImageSprite(m_imgSetLeader, m_spChangeLeader);
				NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(cityData.leaderUnitUID);
				if (unitFromUID != null)
				{
					OpenSDIllust(unitFromUID);
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID);
					if (unitTempletBase != null)
					{
						m_lbLeaderLevel?.SetLevel(unitFromUID, 0, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM);
						NKCUtil.SetLabelText(m_lbLeaderName, unitTempletBase.GetUnitName());
					}
					else
					{
						NKCUtil.SetLabelText(m_lbLeaderLevel, "");
						NKCUtil.SetLabelText(m_lbLeaderName, "");
					}
					NKCUtil.SetGameobjectActive(m_objLeaderSeized, unitFromUID.IsSeized);
				}
				else
				{
					Debug.LogError("leader unit not exist! uid : " + cityData.leaderUnitUID);
					OpenSDIllust(null);
					NKCUtil.SetLabelText(m_lbLeaderLevel, "");
					NKCUtil.SetLabelText(m_lbLeaderName, "");
					NKCUtil.SetGameobjectActive(m_objLeaderSeized, bValue: false);
				}
			}
			bool flag = cityData.worldMapMission != null && cityData.worldMapMission.currentMissionID != 0;
			NKCUtil.SetGameobjectActive(m_csbtnSetLeader, !flag);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnSetLeader, bValue: false);
			OpenSDIllust(null);
			NKCUtil.SetLabelText(m_lbLeaderLevel, "");
			NKCUtil.SetLabelText(m_lbLeaderName, "");
		}
	}

	public void BeginDrag(BaseEventData data)
	{
		bSwipePossible = true;
	}

	public void CitySwipe(BaseEventData cBaseEventData)
	{
		if (bSwipePossible)
		{
			PointerEventData pointerEventData = cBaseEventData as PointerEventData;
			if (pointerEventData.delta.x < 0f - m_swipeDelta)
			{
				SelectPrevCity();
				bSwipePossible = false;
			}
			else if (pointerEventData.delta.x > m_swipeDelta)
			{
				SelectNextCity();
				bSwipePossible = false;
			}
		}
	}

	private void SelectNextCity()
	{
		if (dOnSelectNextCity != null)
		{
			dOnSelectNextCity(m_CityID, bForward: true);
		}
	}

	private void SelectPrevCity()
	{
		if (dOnSelectNextCity != null)
		{
			dOnSelectNextCity(m_CityID, bForward: false);
		}
	}

	private void OnClick(BaseEventData data)
	{
		if (bSwipePossible && dOnExit != null)
		{
			dOnExit();
		}
	}

	private void OnBtnLeaderSelect()
	{
		NKMWorldMapCityData cityData = NKCScenManager.GetScenManager().GetMyUserData().m_WorldmapData.GetCityData(m_CityID);
		if (cityData != null && (cityData.worldMapMission == null || cityData.worldMapMission.currentMissionID == 0))
		{
			NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
			options.bDescending = true;
			options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			options.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
			options.bShowRemoveSlot = cityData.leaderUnitUID != 0;
			options.bShowHideDeckedUnitMenu = false;
			options.bHideDeckedUnit = false;
			options.strUpsideMenuName = NKCUtilString.GET_STRING_WORLDMAP_CITY_SELECT_LEADER;
			options.strEmptyMessage = NKCUtilString.GET_STRING_WORLDMAP_CITY_NO_EXIST_LEADER;
			options.bIncludeUndeckableUnit = false;
			options.setExcludeUnitUID = new HashSet<long> { cityData.leaderUnitUID };
			options.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
			options.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
			options.m_bUseFavorite = true;
			NKCUIUnitSelectList.Instance.Open(options, OnLeaderUnitSelected);
		}
	}

	private void OnLeaderUnitSelected(List<long> lstUnitUID)
	{
		if (lstUnitUID.Count != 1)
		{
			Debug.LogError("Fatal Error : UnitSelectList returned wrong list");
			return;
		}
		long leaderUID = lstUnitUID[0];
		NKCUIUnitSelectList.CheckInstanceAndClose();
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_SET_LEADER_REQ(m_CityID, leaderUID);
	}

	private void OpenSDIllust(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
			m_currentSDUnitID = 0;
			m_currentSDSkinID = 0;
			return;
		}
		if (unitData.m_UnitID == m_currentSDUnitID && unitData.m_SkinID == m_currentSDSkinID)
		{
			PlaySdAnimation();
			return;
		}
		m_currentSDUnitID = unitData.m_UnitID;
		m_currentSDSkinID = unitData.m_SkinID;
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		m_spineSD = NKCResourceUtility.OpenSpineSD(unitData);
		if (m_spineSD != null)
		{
			m_spineSD.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true);
			m_spineSD.SetParent(m_rtSDRoot, worldPositionStays: false);
			RectTransform rectTransform = m_spineSD.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector3.zero;
				rectTransform.localScale = Vector3.one * m_fSDScale;
			}
			ResetSDPosition();
			PlaySdAnimation();
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
		}
	}

	private void PlaySdAnimation()
	{
		if (m_CityData != null)
		{
			if (NKMWorldMapManager.IsMissionRunning(m_CityData))
			{
				SDWork();
			}
			else
			{
				SDIdle();
			}
		}
	}

	private void ResetSDPosition()
	{
		m_rtSDRoot.DOKill();
		m_rtSDRoot.localRotation = Quaternion.identity;
		m_rtSDRoot.anchoredPosition = new Vector2(m_rtSDMoveRange.GetWidth() * 0.5f, m_rtSDMoveRange.GetHeight());
	}

	private void SDRandomWalk()
	{
		m_rtSDRoot.DOKill();
		Vector2 endValue = new Vector2(Random.Range(0f, m_rtSDMoveRange.GetWidth()), Random.Range(0f, m_rtSDMoveRange.GetHeight()));
		bool flag = endValue.x - m_rtSDRoot.anchoredPosition.x < 0f;
		m_rtSDRoot.localRotation = (flag ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity);
		m_rtSDRoot.DOAnchorPos(endValue, MoveSpeed).SetSpeedBased(isSpeedBased: true).SetEase(Ease.Linear)
			.OnComplete(SDIdle);
		m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_WALK, loop: true);
	}

	private void SDIdle()
	{
		m_rtSDRoot.DOKill();
		float duration = Random.Range(1, 4);
		m_rtSDRoot.DOAnchorPos(m_rtSDRoot.anchoredPosition, duration).OnComplete(SDRandomWalk);
		m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true);
	}

	private void SDWork()
	{
		ResetSDPosition();
		m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_WORKING, loop: true);
	}

	private void ShowEmotion(EmotionType type, float time)
	{
		NKCUtil.SetGameobjectActive(m_imgSDEmotion, type != EmotionType.None);
		if (type != EmotionType.None && type == EmotionType.Heart)
		{
			StartCoroutine(WaitAndPlay(time, delegate
			{
				ShowEmotion(EmotionType.None, 0f);
			}));
		}
	}

	private IEnumerator WaitAndPlay(float time, UnityAction action)
	{
		yield return new WaitForSeconds(time);
		action?.Invoke();
	}

	private void SDWin()
	{
		m_rtSDRoot.DOKill();
		float animationTime = m_spineSD.GetAnimationTime(NKCASUIUnitIllust.eAnimation.SD_WIN);
		m_rtSDRoot.DOAnchorPos(m_rtSDRoot.anchoredPosition, animationTime).OnComplete(PlaySdAnimation);
		m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_WIN, loop: false);
	}

	public void OnDonate()
	{
		ShowEmotion(EmotionType.Heart, 2f);
	}

	public void OnMissionStart()
	{
		SDWork();
	}

	public void OnMissionCancel()
	{
		SDRandomWalk();
	}

	public void OnMissionComplete()
	{
		SDWin();
	}

	public void OnCityLevelup()
	{
		SDWin();
	}
}
