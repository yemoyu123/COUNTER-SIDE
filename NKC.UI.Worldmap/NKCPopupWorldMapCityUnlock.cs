using ClientPacket.WorldMap;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCPopupWorldMapCityUnlock : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_world_map_renewal";

	public const string UI_ASSET_NAME = "NKM_UI_WORLD_MAP_RENEWAL_BRANCH_OPEN_POPUP";

	public NKCUIComStateButton m_csbtnClose;

	public Text m_lbName;

	public Text m_lbDescription;

	public Text m_lbCurrentCityCount;

	public NKCUIPriceTag m_tagCashCost;

	public NKCUIComStateButton m_csbtnBuildCash;

	public NKCUIPriceTag m_tagCreditCost;

	public NKCUIPriceTag m_tagCreditCostDisable;

	public Text m_lbCreditReqLevel;

	public NKCUIComStateButton m_csbtnBuildCredit;

	private int m_CityID;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_WORLDMAP_BUILDING;

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void InitUI()
	{
		if (m_csbtnBuildCash != null)
		{
			m_csbtnBuildCash.PointerClick.RemoveAllListeners();
			m_csbtnBuildCash.PointerClick.AddListener(OnBuildCash);
		}
		else
		{
			Debug.LogError("BuildCityCashBtn Not Connected!");
		}
		if (m_csbtnBuildCredit != null)
		{
			m_csbtnBuildCredit.PointerClick.RemoveAllListeners();
			m_csbtnBuildCredit.PointerClick.AddListener(OnBuildCredit);
		}
		else
		{
			Debug.LogError("BuildCityCreditBtn Not Connected!");
		}
		if (m_csbtnClose != null)
		{
			NKCUtil.SetGameobjectActive(m_csbtnClose, bValue: false);
		}
	}

	public void Open(int cityID)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMWorldMapData worldmapData = nKMUserData.m_WorldmapData;
		if (worldmapData.IsCityUnlocked(cityID))
		{
			Debug.LogWarning("Logic Warning : Trying to unlock already unlocked city");
			return;
		}
		NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(cityID);
		if (cityTemplet == null)
		{
			Debug.LogError($"City Templet not found! (id : {cityID})");
			return;
		}
		m_CityID = cityID;
		NKCUtil.SetLabelText(m_lbName, cityTemplet.GetName());
		NKCUtil.SetLabelText(m_lbDescription, cityTemplet.GetDesc());
		SetButtonData(nKMUserData);
		int unlockedCityCount = worldmapData.GetUnlockedCityCount();
		NKCUtil.SetLabelText(m_lbCurrentCityCount, string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_CITY_COUNT_ONE_PARAM, unlockedCityCount));
		UIOpened();
	}

	private void SetButtonData(NKMUserData userData)
	{
		NKMWorldMapData worldmapData = userData.m_WorldmapData;
		int unlockedCityCount = worldmapData.GetUnlockedCityCount();
		if (unlockedCityCount < NKMWorldMapManager.GetPossibleCityCount(userData.UserLevel))
		{
			m_csbtnBuildCredit.UnLock();
			m_tagCreditCost.SetData(1, NKMWorldMapManager.GetCityOpenCost(worldmapData, isCash: false), showMinus: false, changeColor: false);
		}
		else
		{
			m_csbtnBuildCredit.Lock();
			int nextAreaUnlockLevel = NKMWorldMapManager.GetNextAreaUnlockLevel(unlockedCityCount);
			NKCUtil.SetLabelText(m_lbCreditReqLevel, string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_CREDIT_REQ_LEVEL_ONE_PARAM, nextAreaUnlockLevel));
			m_tagCreditCostDisable.SetData(1, NKMWorldMapManager.GetCityOpenCost(worldmapData, isCash: false), showMinus: false, changeColor: false);
		}
		m_tagCashCost.SetData(101, NKMWorldMapManager.GetCityOpenCost(worldmapData, isCash: true), showMinus: false, changeColor: false);
	}

	private void OnBuildCash()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMWorldMapData worldmapData = nKMUserData.m_WorldmapData;
		if (worldmapData != null)
		{
			NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WORLDMAP_CITY_UNLOCK_DESC, 101, NKMWorldMapManager.GetCityOpenCost(worldmapData, isCash: true), delegate
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_SET_CITY_REQ(m_CityID, bCash: true);
			});
		}
	}

	private void OnBuildCredit()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMWorldMapData worldmapData = nKMUserData.m_WorldmapData;
		if (worldmapData == null)
		{
			return;
		}
		if (worldmapData.GetUnlockedCityCount() < NKMWorldMapManager.GetPossibleCityCount(nKMUserData.UserLevel))
		{
			NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WORLDMAP_CITY_UNLOCK_DESC, 1, NKMWorldMapManager.GetCityOpenCost(worldmapData, isCash: false), delegate
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_SET_CITY_REQ(m_CityID, bCash: false);
			});
		}
		else
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().Send_NKMPacket_WORLDMAP_SET_CITY_REQ(m_CityID, bCash: false);
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		int itemID = itemData.ItemID;
		if (itemID == 1 || itemID == 101)
		{
			SetButtonData(NKCScenManager.CurrentUserData());
		}
	}
}
