using ClientPacket.WorldMap;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCUIWorldMapCity : MonoBehaviour
{
	public enum LocationStatus
	{
		Deactive,
		Active,
		Progress,
		Complete
	}

	public delegate void OnClickCity(int CityID);

	[Header("기본")]
	public int m_CityID;

	public RectTransform m_rtRoot;

	public NKCUIComStateButton m_csbtnButton;

	public Animator m_Animator;

	public GameObject m_BRANCH_OPEN;

	public Text m_lbName;

	public Image m_imgLevelGuage;

	public Text m_lbLevel;

	[Header("진행중 표시")]
	public GameObject m_objProgressRoot;

	public Text m_lbProgressTimeLeft;

	[Header("SD 루트")]
	public RectTransform m_rtSDRoot;

	[Header("이벤트")]
	public NKCUIWorldMapCityEventPin m_UIEventPin;

	private LocationStatus m_CurrentStatus;

	private NKMWorldMapCityData m_CityData;

	private NKMUnitData m_CityLeaderUnitData;

	private NKCASUIUnitIllust m_spineSD;

	private OnClickCity dOnClickCity;

	private RectTransform _Rect;

	public RectTransform Rect
	{
		get
		{
			if (_Rect == null)
			{
				_Rect = GetComponent<RectTransform>();
			}
			return _Rect;
		}
	}

	public void Init(OnClickCity onClickCity, NKCUIWorldMapCityEventPin.OnClickEvent onClickEvent)
	{
		dOnClickCity = onClickCity;
		if (m_csbtnButton != null)
		{
			m_csbtnButton.PointerClick.RemoveAllListeners();
			m_csbtnButton.PointerClick.AddListener(OnSlotClicked);
		}
		if (m_UIEventPin != null)
		{
			m_UIEventPin.Init(onClickEvent);
		}
	}

	public void OnSlotClicked()
	{
		dOnClickCity?.Invoke(m_CityID);
	}

	public void CleanUp()
	{
		if (m_spineSD != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		}
		m_spineSD = null;
	}

	public void CleanUpEventPinSpineSD()
	{
		if (m_UIEventPin != null)
		{
			m_UIEventPin.CleanUpSpineSD();
		}
	}

	public void PlaySDAnim(NKCASUIUnitIllust.eAnimation eAnim, bool bLoop = false)
	{
		if (m_UIEventPin != null)
		{
			m_UIEventPin.PlaySDAnim(eAnim, bLoop);
		}
	}

	public Vector3 GetPinSDPos()
	{
		if (m_UIEventPin != null)
		{
			return m_UIEventPin.GetPinSDPos() + base.transform.localPosition;
		}
		return new Vector3(0f, 0f, 0f);
	}

	public bool SetData(NKMWorldMapCityData cityData)
	{
		NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(m_CityID);
		if (cityTemplet == null)
		{
			Debug.LogError($"CityTemplet Not Found. ID {m_CityID}");
			return false;
		}
		NKCUtil.SetLabelText(m_lbName, cityTemplet.GetName());
		bool bValue = false;
		if (cityData != null)
		{
			m_CityData = cityData;
			NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(m_CityData.leaderUnitUID);
			m_CityLeaderUnitData = unitFromUID;
			NKCUtil.SetLabelText(m_lbLevel, cityData.level.ToString());
			SetExpBar(cityData);
			if (cityData.HasMission())
			{
				if (NKCSynchronizedTime.IsFinished(cityData.worldMapMission.completeTime))
				{
					SetState(LocationStatus.Complete);
				}
				else
				{
					SetState(LocationStatus.Progress);
				}
				UpdateClock();
			}
			else
			{
				SetState(LocationStatus.Active);
			}
			SetEventData(cityData.worldMapEventGroup);
		}
		else
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				NKMWorldMapData worldmapData = nKMUserData.m_WorldmapData;
				if (worldmapData != null && worldmapData.GetUnlockedCityCount() < NKMWorldMapManager.GetPossibleCityCount(nKMUserData.UserLevel))
				{
					bValue = true;
				}
			}
			m_CityData = null;
			m_CityLeaderUnitData = null;
			NKCUtil.SetLabelText(m_lbLevel, "-");
			SetState(LocationStatus.Deactive);
			SetEventData(null);
		}
		NKCUtil.SetGameobjectActive(m_BRANCH_OPEN, bValue);
		return true;
	}

	public void UpdateCityRaidData()
	{
		m_UIEventPin.UpdateRaidData();
	}

	private void SetExpBar(NKMWorldMapCityData cityData)
	{
		if (m_imgLevelGuage != null)
		{
			m_imgLevelGuage.fillAmount = NKMWorldMapManager.GetCityExpPercent(cityData);
		}
	}

	private void SetEventData(NKMWorldMapEventGroup eventGroupData)
	{
		if (m_UIEventPin == null || eventGroupData == null)
		{
			NKCUtil.SetGameobjectActive(m_UIEventPin, bValue: false);
			return;
		}
		if (eventGroupData.worldmapEventID == 0)
		{
			NKCUtil.SetGameobjectActive(m_UIEventPin, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_UIEventPin, bValue: true);
		m_UIEventPin.SetData(m_CityID, eventGroupData);
	}

	private void SetState(LocationStatus status)
	{
		m_CurrentStatus = status;
		if (m_Animator != null)
		{
			m_Animator.SetBool("Locked", status == LocationStatus.Deactive);
			m_Animator.SetBool("Mission", status == LocationStatus.Progress);
			m_Animator.SetBool("Complete", status == LocationStatus.Complete);
		}
		switch (m_CurrentStatus)
		{
		case LocationStatus.Active:
			NKCUtil.SetGameobjectActive(m_objProgressRoot, bValue: false);
			break;
		case LocationStatus.Progress:
			NKCUtil.SetGameobjectActive(m_objProgressRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbProgressTimeLeft, bValue: true);
			break;
		case LocationStatus.Complete:
			NKCUtil.SetGameobjectActive(m_objProgressRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbProgressTimeLeft, bValue: false);
			break;
		case LocationStatus.Deactive:
			NKCUtil.SetGameobjectActive(m_objProgressRoot, bValue: false);
			break;
		}
		SetSD(status);
	}

	private void SetSD(LocationStatus status)
	{
		switch (status)
		{
		case LocationStatus.Active:
			OpenSDIllust(m_CityLeaderUnitData);
			if (m_spineSD != null)
			{
				float fStartTime2 = NKMRandom.Range(0f, m_spineSD.GetAnimationTime(NKCASUIUnitIllust.eAnimation.SD_IDLE));
				m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true, 0, bForceRestart: true, fStartTime2);
			}
			break;
		case LocationStatus.Progress:
		{
			NKMWorldMapMissionTemplet missionTemplet = NKMWorldMapManager.GetMissionTemplet(m_CityData.worldMapMission.currentMissionID);
			OpenSDIllust(m_CityLeaderUnitData);
			NKCASUIUnitIllust.eAnimation eAnim = ((missionTemplet == null) ? NKCASUIUnitIllust.eAnimation.SD_WORKING : NKCUIWorldMap.GetMissionProgressAnimationType(missionTemplet.m_eMissionType));
			if (m_spineSD != null)
			{
				float fStartTime3 = NKMRandom.Range(0f, m_spineSD.GetAnimationTime(eAnim));
				m_spineSD.SetAnimation(eAnim, loop: true, 0, bForceRestart: true, fStartTime3);
			}
			break;
		}
		case LocationStatus.Complete:
			NKMWorldMapManager.GetMissionTemplet(m_CityData.worldMapMission.currentMissionID);
			OpenSDIllust(m_CityLeaderUnitData);
			if (m_spineSD != null)
			{
				float fStartTime = NKMRandom.Range(0f, m_spineSD.GetAnimationTime(NKCASUIUnitIllust.eAnimation.SD_WIN));
				m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_WIN, loop: true, 0, bForceRestart: true, fStartTime);
			}
			break;
		case LocationStatus.Deactive:
			OpenSDIllust(null);
			break;
		}
	}

	private void OpenSDIllust(NKMUnitData unitData)
	{
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_spineSD);
		if (unitData == null)
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
			m_spineSD = null;
			return;
		}
		m_spineSD = NKCResourceUtility.OpenSpineSD(unitData);
		if (m_spineSD != null)
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: true);
			m_spineSD.SetParent(m_rtSDRoot, worldPositionStays: false);
			RectTransform rectTransform = m_spineSD.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector2.zero;
				rectTransform.localScale = Vector3.one;
				rectTransform.localRotation = Quaternion.identity;
			}
		}
		else
		{
			Debug.LogError("spine SD data not found from unitID : " + unitData.m_UnitID);
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
		}
	}

	private void UpdateClock()
	{
		if (m_CityData != null)
		{
			m_lbProgressTimeLeft.text = NKCSynchronizedTime.GetTimeLeftString(m_CityData.worldMapMission.completeTime);
			if (NKCSynchronizedTime.IsFinished(m_CityData.worldMapMission.completeTime))
			{
				SetState(LocationStatus.Complete);
			}
		}
	}

	private void Update()
	{
		if (m_CurrentStatus == LocationStatus.Progress)
		{
			UpdateClock();
		}
		m_rtRoot.rotation = Quaternion.identity;
	}
}
