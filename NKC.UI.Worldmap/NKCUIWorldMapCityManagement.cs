using System;
using ClientPacket.WorldMap;
using NKM;
using UnityEngine;

namespace NKC.UI.Worldmap;

public class NKCUIWorldMapCityManagement : MonoBehaviour
{
	private enum State
	{
		CityMission,
		Building
	}

	[Header("Left")]
	public NKCUIWorldMapCityDetail m_UICityDetail;

	[Header("Right")]
	public NKCUIComToggle m_tglMission;

	public NKCUIComToggle m_tglBuilding;

	public GameObject[] m_objBuildingReddot;

	public NKCUIWorldmapCityMissionPanel m_UICityMission;

	public NKCUIWorldmapCityBuildPanel m_UICityBuilding;

	private State m_eState;

	private NKMWorldMapCityData m_CityData;

	private Action m_openCallback;

	private Action m_closeCallback;

	public bool IsOpen => base.gameObject.activeSelf;

	public void Init(NKCUIWorldMapCityDetail.OnSelectNextCity onSelectNextCity, NKCUIWorldMapCityDetail.OnExit onExit)
	{
		m_UICityDetail.Init(onSelectNextCity, onExit);
		m_UICityMission.Init();
		m_UICityBuilding.Init();
		if (m_tglMission != null)
		{
			m_tglMission.OnValueChanged.RemoveAllListeners();
			m_tglMission.OnValueChanged.AddListener(OnTglMission);
		}
		else
		{
			Debug.LogError("Mission Toggle Button Not Set");
		}
		if (m_tglBuilding != null)
		{
			m_tglBuilding.OnValueChanged.RemoveAllListeners();
			m_tglBuilding.OnValueChanged.AddListener(OnTglBuilding);
		}
		else
		{
			Debug.LogError("Building Toggle Button Not Set");
		}
	}

	public void CleanUp()
	{
		m_eState = State.CityMission;
		m_CityData = null;
		m_UICityDetail.CleanUp();
		m_UICityMission.CleanUp();
		m_UICityBuilding.CleanUp();
	}

	public void Open(NKMWorldMapCityData cityData, Action openCallback, Action closeCallback)
	{
		base.gameObject.SetActive(value: true);
		openCallback?.Invoke();
		m_openCallback = openCallback;
		m_closeCallback = closeCallback;
		SetState(m_eState, cityData);
	}

	private void SetState(State state, NKMWorldMapCityData cityData)
	{
		m_eState = state;
		NKCUtil.SetGameobjectActive(m_UICityMission, state == State.CityMission);
		NKCUtil.SetGameobjectActive(m_UICityBuilding, state == State.Building);
		switch (m_eState)
		{
		case State.Building:
			m_tglBuilding.Select(bSelect: true, bForce: true);
			break;
		case State.CityMission:
			m_tglMission.Select(bSelect: true, bForce: true);
			break;
		}
		SetData(cityData);
	}

	public void SetData(NKMWorldMapCityData cityData)
	{
		m_CityData = cityData;
		m_UICityDetail.SetData(cityData);
		switch (m_eState)
		{
		case State.Building:
			m_UICityBuilding.SetData(cityData);
			break;
		case State.CityMission:
			m_UICityMission.SetData(cityData);
			break;
		}
		bool bValue = NKMWorldMapManager.GetUsableBuildPoint(cityData) > 0;
		for (int i = 0; i < m_objBuildingReddot.Length; i++)
		{
			NKCUtil.SetGameobjectActive(m_objBuildingReddot[i], bValue);
		}
	}

	public void Close()
	{
		CleanUp();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_closeCallback?.Invoke();
	}

	public void Unhide()
	{
		m_openCallback?.Invoke();
	}

	private void OnTglMission(bool value)
	{
		if (value)
		{
			SetState(State.CityMission, m_CityData);
		}
	}

	private void OnTglBuilding(bool value)
	{
		if (value)
		{
			SetState(State.Building, m_CityData);
		}
	}

	public void CityDataUpdated(NKMWorldMapCityData cityData)
	{
		SetData(cityData);
	}

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (m_eState == State.CityMission)
		{
			m_UICityMission.OnInventoryChange(itemData);
		}
	}

	public void SetFXBuildingID(int buildingID)
	{
		if (m_eState == State.Building)
		{
			m_UICityBuilding.SetFXBuildingID(buildingID);
		}
	}
}
