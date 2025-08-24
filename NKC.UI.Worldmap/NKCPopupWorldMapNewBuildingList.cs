using System.Collections.Generic;
using ClientPacket.WorldMap;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCPopupWorldMapNewBuildingList : NKCUIBase
{
	public delegate void OnBuildingSelected(int buildingID);

	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_world_map_renewal";

	public const string UI_ASSET_NAME = "NKM_UI_WORLD_MAP_RENEWAL_BUILDING_LIST_POPUP";

	public NKCUIWorldMapCityBuildingSlot m_pfbBuildingSlot;

	public NKCUIComStateButton m_csbtnClose;

	public Text m_lbBuildingPointLeft;

	public LoopScrollRect m_LoopScroll;

	public Text m_lbBuildingSlot;

	private NKMWorldMapCityData m_cityData;

	private int m_cityBuildingPointLeft;

	private OnBuildingSelected dOnBuildingSelected;

	private List<NKMWorldMapBuildingTemplet> m_lstBuildingTemplet = new List<NKMWorldMapBuildingTemplet>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_WORLDMAP_NEW_BUILDING;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.ResourceOnly;

	public override void CloseInternal()
	{
		m_cityData = null;
		base.gameObject.SetActive(value: false);
	}

	public void Init()
	{
		if (null != m_LoopScroll)
		{
			m_LoopScroll.dOnGetObject += GetSlot;
			m_LoopScroll.dOnReturnObject += ReturnSlot;
			m_LoopScroll.dOnProvideData += ProvideSlotData;
			m_LoopScroll.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScroll);
		}
		m_lstBuildingTemplet.Clear();
		foreach (NKMWorldMapBuildingTemplet value in NKMTempletContainer<NKMWorldMapBuildingTemplet>.Values)
		{
			m_lstBuildingTemplet.Add(value);
		}
		m_lstBuildingTemplet.Sort(SortBuildingData);
		if (m_csbtnClose != null)
		{
			m_csbtnClose.PointerClick.RemoveAllListeners();
			m_csbtnClose.PointerClick.AddListener(base.Close);
		}
	}

	private int SortBuildingData(NKMWorldMapBuildingTemplet a, NKMWorldMapBuildingTemplet b)
	{
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = a.GetLevelTemplet(1);
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet2 = b.GetLevelTemplet(1);
		if (levelTemplet == null && levelTemplet2 == null)
		{
			return 0;
		}
		if (levelTemplet == null && levelTemplet2 != null)
		{
			return 1;
		}
		if (levelTemplet != null && levelTemplet2 == null)
		{
			return -1;
		}
		if (levelTemplet.sortIndex == levelTemplet2.sortIndex)
		{
			return a.Key.CompareTo(b.Key);
		}
		return levelTemplet.sortIndex.CompareTo(levelTemplet2.sortIndex);
	}

	private RectTransform GetSlot(int index)
	{
		NKCUIWorldMapCityBuildingSlot nKCUIWorldMapCityBuildingSlot = Object.Instantiate(m_pfbBuildingSlot);
		nKCUIWorldMapCityBuildingSlot.Init();
		nKCUIWorldMapCityBuildingSlot.transform.localPosition = Vector3.zero;
		nKCUIWorldMapCityBuildingSlot.transform.localScale = Vector3.one;
		return nKCUIWorldMapCityBuildingSlot.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		go.SetParent(base.transform);
		Object.Destroy(go);
	}

	private void ProvideSlotData(Transform transform, int idx)
	{
		NKCUIWorldMapCityBuildingSlot component = transform.GetComponent<NKCUIWorldMapCityBuildingSlot>();
		if (component != null && idx < m_lstBuildingTemplet.Count)
		{
			NKMWorldMapBuildingTemplet buildingTemplet = m_lstBuildingTemplet[idx];
			component.SetDataForBuild(m_cityData, m_cityBuildingPointLeft, buildingTemplet, SlotSelected);
		}
	}

	public void Open(NKMWorldMapCityData cityData, int cityBuildingPointLeft, OnBuildingSelected onBuildingSelected)
	{
		m_cityData = cityData;
		m_cityBuildingPointLeft = cityBuildingPointLeft;
		dOnBuildingSelected = onBuildingSelected;
		base.gameObject.SetActive(value: true);
		m_LoopScroll.TotalCount = m_lstBuildingTemplet.Count;
		m_LoopScroll.RefreshCells();
		NKCUtil.SetLabelText(m_lbBuildingPointLeft, cityBuildingPointLeft.ToString());
		NKCUtil.SetLabelText(m_lbBuildingSlot, string.Empty);
		UIOpened();
	}

	private void SlotSelected(int buildingID)
	{
		dOnBuildingSelected?.Invoke(buildingID);
	}

	public RectTransform GetBuildingSlot(int buildingID)
	{
		int num = m_lstBuildingTemplet.FindIndex((NKMWorldMapBuildingTemplet v) => v.GetLevelTemplet(1).id == buildingID);
		if (num < 0)
		{
			return null;
		}
		m_LoopScroll.SetIndexPosition(num);
		NKCUIWorldMapCityBuildingSlot[] componentsInChildren = m_LoopScroll.content.GetComponentsInChildren<NKCUIWorldMapCityBuildingSlot>();
		for (int num2 = 0; num2 < componentsInChildren.Length; num2++)
		{
			if (componentsInChildren[num2] != null && componentsInChildren[num2].BuildingID == buildingID)
			{
				return componentsInChildren[num2].GetComponent<RectTransform>();
			}
		}
		return null;
	}
}
