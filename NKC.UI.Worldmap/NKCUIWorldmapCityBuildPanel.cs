using System.Collections.Generic;
using ClientPacket.WorldMap;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCUIWorldmapCityBuildPanel : MonoBehaviour
{
	public NKCUIWorldMapCityBuildingSlot m_pfbBuildingSlot;

	public Text m_lbBuildPoint;

	public LoopScrollRect m_LoopScroll;

	public Text m_lbBuildSlot;

	private NKMWorldMapCityData m_cityData;

	private int m_cityBuildingPointCache;

	private List<NKMWorldmapCityBuildingData> m_lstBuildingData = new List<NKMWorldmapCityBuildingData>();

	private bool m_bSlotReady;

	private int m_fxBuildingID;

	internal void Init()
	{
		if (null != m_LoopScroll)
		{
			m_LoopScroll.dOnGetObject += GetSlot;
			m_LoopScroll.dOnReturnObject += ReturnSlot;
			m_LoopScroll.dOnProvideData += ProvideSlotData;
			NKCUtil.SetScrollHotKey(m_LoopScroll);
		}
	}

	private RectTransform GetSlot(int index)
	{
		NKCUIWorldMapCityBuildingSlot nKCUIWorldMapCityBuildingSlot = Object.Instantiate(m_pfbBuildingSlot);
		nKCUIWorldMapCityBuildingSlot.Init();
		nKCUIWorldMapCityBuildingSlot.transform.localPosition = Vector3.zero;
		nKCUIWorldMapCityBuildingSlot.transform.localScale = Vector3.one;
		return nKCUIWorldMapCityBuildingSlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform tr)
	{
		tr.SetParent(base.transform);
		Object.Destroy(tr.gameObject);
	}

	private void ProvideSlotData(Transform transform, int idx)
	{
		NKCUIWorldMapCityBuildingSlot component = transform.GetComponent<NKCUIWorldMapCityBuildingSlot>();
		if (!(component != null))
		{
			return;
		}
		if (idx < m_lstBuildingData.Count)
		{
			NKMWorldmapCityBuildingData nKMWorldmapCityBuildingData = m_lstBuildingData[idx];
			component.SetDataForManage(m_cityData, m_cityBuildingPointCache, nKMWorldmapCityBuildingData.id, nKMWorldmapCityBuildingData.level, OnSelectCityBuilding);
			bool flag = m_fxBuildingID > 0 && m_fxBuildingID == nKMWorldmapCityBuildingData.id;
			component.SetFx(flag);
			if (flag)
			{
				m_fxBuildingID = 0;
			}
		}
		else
		{
			component.SetEmpty(OnSelectBuildNewBuilding);
			component.SetFx(bShow: false);
		}
	}

	internal void SetData(NKMWorldMapCityData cityData)
	{
		m_cityData = cityData;
		m_cityBuildingPointCache = NKMWorldMapManager.GetUsableBuildPoint(m_cityData);
		if (!m_bSlotReady)
		{
			m_bSlotReady = true;
			m_LoopScroll.PrepareCells();
		}
		m_lstBuildingData.Clear();
		m_lstBuildingData.AddRange(cityData.worldMapCityBuildingDataMap.Values);
		m_lstBuildingData.Sort(SortBuildingData);
		m_LoopScroll.TotalCount = m_lstBuildingData.Count + 1;
		m_LoopScroll.RefreshCells();
		NKCUtil.SetLabelText(m_lbBuildPoint, m_cityBuildingPointCache.ToString());
		NKCUtil.SetLabelText(m_lbBuildSlot, string.Empty);
	}

	private int SortBuildingData(NKMWorldmapCityBuildingData a, NKMWorldmapCityBuildingData b)
	{
		NKMWorldMapBuildingTemplet.LevelTemplet cityBuildingTemplet = NKMWorldMapManager.GetCityBuildingTemplet(a.id, a.level);
		NKMWorldMapBuildingTemplet.LevelTemplet cityBuildingTemplet2 = NKMWorldMapManager.GetCityBuildingTemplet(b.id, b.level);
		if (cityBuildingTemplet == null && cityBuildingTemplet2 == null)
		{
			return 0;
		}
		if (cityBuildingTemplet == null && cityBuildingTemplet2 != null)
		{
			return 1;
		}
		if (cityBuildingTemplet != null && cityBuildingTemplet2 == null)
		{
			return -1;
		}
		if (cityBuildingTemplet.sortIndex == cityBuildingTemplet2.sortIndex)
		{
			return a.id.CompareTo(b.id);
		}
		return cityBuildingTemplet.sortIndex.CompareTo(cityBuildingTemplet2.sortIndex);
	}

	internal void CleanUp()
	{
		m_cityData = null;
		m_cityBuildingPointCache = 0;
		m_fxBuildingID = 0;
	}

	private bool GetBuildingTemplets(int buildingID, out NKMWorldMapBuildingTemplet buildingTemplet, out NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet)
	{
		if (m_cityData == null)
		{
			buildingTemplet = null;
			levelTemplet = null;
			return false;
		}
		NKMWorldmapCityBuildingData buildingData = m_cityData.GetBuildingData(buildingID);
		if (buildingData == null)
		{
			buildingTemplet = null;
			levelTemplet = null;
			return false;
		}
		buildingTemplet = NKMWorldMapBuildingTemplet.Find(buildingID);
		levelTemplet = buildingTemplet.GetLevelTemplet(buildingData.level);
		return true;
	}

	private void OnSelectCityBuilding(int buildingID)
	{
		if (!GetBuildingTemplets(buildingID, out var buildingTemplet, out var levelTemplet))
		{
			Debug.LogError("Target Building Not Found!");
			return;
		}
		NKCPopupWorldMapBuildingInfo popupWorldMapBuildingInfo = NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().PopupWorldMapBuildingInfo;
		if (popupWorldMapBuildingInfo != null)
		{
			popupWorldMapBuildingInfo.OpenForManagement(m_cityData.cityID, buildingTemplet, levelTemplet, m_cityBuildingPointCache, m_lstBuildingData.Count, TryLevelupBuilding, TryRemoveBuilding);
		}
	}

	private void OnSelectBuildNewBuilding(int buildingID)
	{
		NKCPopupWorldMapNewBuildingList popupWorldMapNewBuildingList = NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().PopupWorldMapNewBuildingList;
		if (popupWorldMapNewBuildingList != null)
		{
			popupWorldMapNewBuildingList.Open(m_cityData, m_cityBuildingPointCache, OnBuildTargetSelected);
		}
	}

	private void TryLevelupBuilding(int buildingID)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_BUILD_LEVELUP_REQ(m_cityData.cityID, buildingID);
	}

	private void TryRemoveBuilding(int buildingID)
	{
		if (GetBuildingTemplets(buildingID, out var _, out var levelTemplet))
		{
			NKCPopupResourceWithdraw.Instance.OpenForWorldmapBuildingRemove(levelTemplet, delegate
			{
				OnConfirmRemoveBuilding(buildingID);
			});
		}
	}

	private void OnConfirmRemoveBuilding(int buildingID)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_BUILD_EXPIRE_REQ(m_cityData.cityID, buildingID);
	}

	private void OnBuildTargetSelected(int buildingID)
	{
		NKMWorldMapBuildingTemplet buildingTemplet = NKMWorldMapBuildingTemplet.Find(buildingID);
		NKCPopupWorldMapBuildingInfo popupWorldMapBuildingInfo = NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().PopupWorldMapBuildingInfo;
		if (popupWorldMapBuildingInfo != null)
		{
			popupWorldMapBuildingInfo.OpenForBuild(m_cityData.cityID, buildingTemplet, m_cityBuildingPointCache, m_lstBuildingData.Count, TryBuildBuilding);
		}
	}

	private void TryBuildBuilding(int buildingID)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_BUILD_REQ(m_cityData.cityID, buildingID);
	}

	public void SetFXBuildingID(int buildingID)
	{
		m_fxBuildingID = buildingID;
	}

	public RectTransform GetEmptySlot()
	{
		m_LoopScroll.SetIndexPosition(m_lstBuildingData.Count);
		NKCUIWorldMapCityBuildingSlot[] componentsInChildren = m_LoopScroll.content.GetComponentsInChildren<NKCUIWorldMapCityBuildingSlot>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != null && componentsInChildren[i].m_objEmpty.activeSelf)
			{
				return componentsInChildren[i].GetComponent<RectTransform>();
			}
		}
		return null;
	}
}
