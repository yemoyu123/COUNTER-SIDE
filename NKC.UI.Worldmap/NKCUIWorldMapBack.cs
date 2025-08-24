using System.Collections.Generic;
using ClientPacket.Raid;
using ClientPacket.WorldMap;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Worldmap;

public class NKCUIWorldMapBack : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	public RectTransform m_rtBackground;

	public Vector2 m_vCameraMoveBound;

	private Vector3 MainCameraPosition;

	public float rubberScale = 1f;

	public float scrollSensitivity = 1f;

	private Vector2 currentCameraPos;

	public List<NKCUIWorldMapCity> m_lstCity;

	private Dictionary<int, NKCUIWorldMapCity> m_dicCity = new Dictionary<int, NKCUIWorldMapCity>();

	public float m_fCameraZPos = -676f;

	public Vector2 m_vCameraMoveRange;

	private bool m_bEnableDrag = true;

	public Animator m_amtorWorldmapBack;

	private bool m_bDragging;

	private Vector2 totalDrag;

	public void SetEnableDrag(bool bSet)
	{
		m_bEnableDrag = bSet;
	}

	public void Init(NKCUIWorldMapCity.OnClickCity onSelectCity, NKCUIWorldMapCityEventPin.OnClickEvent onSelectEvent)
	{
		GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		foreach (NKCUIWorldMapCity item in m_lstCity)
		{
			NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(item.m_CityID);
			if (cityTemplet == null)
			{
				Debug.LogError($"CityID {item.m_CityID} does not exist!");
				NKCUtil.SetGameobjectActive(item, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(item, bValue: true);
				item.Init(onSelectCity, onSelectEvent);
				m_dicCity.Add(cityTemplet.m_ID, item);
			}
		}
		base.gameObject.SetActive(value: false);
	}

	public void SetData(NKMWorldMapData worldMapData)
	{
		base.gameObject.SetActive(value: true);
		foreach (KeyValuePair<int, NKCUIWorldMapCity> item in m_dicCity)
		{
			item.Value.SetData(worldMapData.GetCityData(item.Key));
		}
		NKCCamera.StopTrackingCamera();
		NKCCamera.GetTrackingPos().SetNowValue(0f, 0f, m_fCameraZPos);
	}

	public void PlayPinSDAniByCityID(int cityID, NKCASUIUnitIllust.eAnimation eAnim, bool bLoop = false)
	{
		NKCUIWorldMapCity value = null;
		if (m_dicCity.TryGetValue(cityID, out value) && value != null)
		{
			value.PlaySDAnim(eAnim, bLoop);
		}
	}

	public void CleanUpEventPinSpineSD(int cityID)
	{
		NKCUIWorldMapCity value = null;
		if (m_dicCity.TryGetValue(cityID, out value) && value != null)
		{
			value.CleanUpEventPinSpineSD();
		}
	}

	public Vector3 GetPinSDPos(int cityID)
	{
		NKCUIWorldMapCity value = null;
		if (m_dicCity.TryGetValue(cityID, out value) && value != null)
		{
			return value.GetPinSDPos() + base.transform.localPosition;
		}
		return new Vector3(0f, 0f, 0f);
	}

	public void UpdateCity(int cityID, NKMWorldMapCityData cityData)
	{
		if (!m_dicCity.TryGetValue(cityID, out var value))
		{
			Debug.LogError("CityUI for city " + cityID + " Not Found, maybe city does not exist in CityTemplet!");
		}
		else if (!value.SetData(cityData))
		{
			Debug.LogError("City Icon SetData Failed!!!");
		}
	}

	public void CityEventSpawned(int cityID)
	{
		if (!m_dicCity.TryGetValue(cityID, out var _))
		{
			Debug.LogError("CityUI for city " + cityID + " Not Found, maybe city does not exist in CityTemplet!");
		}
	}

	public void UpdateCityRaidData(NKMRaidDetailData raidDetailData)
	{
		if (m_dicCity.TryGetValue(raidDetailData.cityID, out var value))
		{
			value.UpdateCityRaidData();
		}
	}

	public void OnBeginDrag(PointerEventData data)
	{
	}

	public void OnDrag(PointerEventData pointData)
	{
		if (m_bEnableDrag)
		{
			float value = NKCCamera.GetPosNowX() - pointData.delta.x * 10f;
			float value2 = NKCCamera.GetPosNowY() - pointData.delta.y * 10f;
			value = Mathf.Clamp(value, 0f - m_vCameraMoveRange.x, m_vCameraMoveRange.x);
			value2 = Mathf.Clamp(value2, 0f - m_vCameraMoveRange.y, m_vCameraMoveRange.y);
			NKCCamera.TrackingPos(1f, value, value2);
		}
	}

	private float Rubber(float currentValue, float Limit)
	{
		float num = Mathf.Abs(currentValue);
		return Limit * num / (Limit + num) * Mathf.Sign(currentValue);
	}

	public void OnEndDrag(PointerEventData data)
	{
	}

	private void Update()
	{
		if (!m_bDragging)
		{
			currentCameraPos.x = NKCCamera.GetPosNowX();
			currentCameraPos.y = NKCCamera.GetPosNowY();
		}
	}

	public RectTransform GetPinRect(int cityID)
	{
		if (m_dicCity.TryGetValue(cityID, out var value) && value != null)
		{
			return value.GetComponent<RectTransform>();
		}
		return null;
	}
}
