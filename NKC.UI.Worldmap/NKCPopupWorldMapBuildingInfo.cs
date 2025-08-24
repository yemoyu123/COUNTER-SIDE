using System.Text;
using ClientPacket.WorldMap;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCPopupWorldMapBuildingInfo : NKCUIBase
{
	public delegate void OnButton(int BuildingID);

	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_world_map_renewal";

	public const string UI_ASSET_NAME = "NKM_UI_WORLD_MAP_RENEWAL_BUILDING_INFO_POPUP";

	public NKCUIComStateButton m_csbtnClose;

	[Header("빌딩 정보")]
	public Image m_imgBuilding;

	public Text m_lbBuildingPoint;

	public Text m_lbLevel;

	public Text m_lbName;

	public Text m_lbMaxLevel;

	public Text m_lbDescription;

	public Text m_lbInformation;

	[Header("아래쪽")]
	public NKCUIComStateButton m_csbtnBuild;

	public Text m_lbBuild;

	public Text m_lbBuildLocked;

	public NKCUIComStateButton m_csbtnDestory;

	public GameObject m_objBuildCost;

	public NKCUIItemCostSlot m_tagBuildPoint;

	public NKCUIItemCostSlot m_tagCredit;

	public Text m_lbSlotCount;

	[Header("건설 불가 경고")]
	public GameObject m_objRootAlert;

	public Text m_lbAlert;

	private OnButton dOnBuild;

	private OnButton dOnDestory;

	private int m_BuildingID;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_WORLDMAP_BUILDING;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.ResourceOnly;

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Init()
	{
		if (m_csbtnClose != null)
		{
			m_csbtnClose.PointerClick.RemoveAllListeners();
			m_csbtnClose.PointerClick.AddListener(base.Close);
		}
		if (m_csbtnBuild != null)
		{
			m_csbtnBuild.PointerClick.RemoveAllListeners();
			m_csbtnBuild.PointerClick.AddListener(OnBtnBuild);
			NKCUtil.SetHotkey(m_csbtnBuild, HotkeyEventType.Confirm);
		}
		if (m_csbtnDestory != null)
		{
			m_csbtnDestory.PointerClick.RemoveAllListeners();
			m_csbtnDestory.PointerClick.AddListener(OnBtnRemove);
		}
	}

	public void OpenForBuild(int cityID, NKMWorldMapBuildingTemplet buildingTemplet, int currentBuildPoint, int cityBuildingCount, OnButton onBuild)
	{
		dOnBuild = onBuild;
		dOnDestory = null;
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = buildingTemplet.GetLevelTemplet(1);
		SetDataCommon(buildingTemplet, levelTemplet, currentBuildPoint, cityBuildingCount);
		NKCUtil.SetGameobjectActive(m_csbtnDestory, bValue: false);
		NKCUtil.SetLabelText(m_lbBuild, NKCUtilString.GET_STRING_WORLDMAP_BUILDING_BUILD);
		NKCUtil.SetLabelText(m_lbBuildLocked, NKCUtilString.GET_STRING_WORLDMAP_BUILDING_BUILD);
		SetCost(levelTemplet, currentBuildPoint);
		SetBuildButtonAndAlert(cityID, levelTemplet, bUpgrade: false);
		UIOpened();
	}

	public void OpenForManagement(int cityID, NKMWorldMapBuildingTemplet buildingTemplet, NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet, int currentBuildPoint, int cityBuildingCount, OnButton onLevelUp, OnButton onRemove)
	{
		dOnBuild = onLevelUp;
		dOnDestory = onRemove;
		SetDataCommon(buildingTemplet, levelTemplet, currentBuildPoint, cityBuildingCount);
		NKCUtil.SetGameobjectActive(m_csbtnDestory, buildingTemplet.Key != 1);
		NKCUtil.SetLabelText(m_lbBuild, NKCUtilString.GET_STRING_LEVEL_UP);
		NKCUtil.SetLabelText(m_lbBuildLocked, NKCUtilString.GET_STRING_LEVEL_UP);
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet2 = buildingTemplet.GetLevelTemplet(levelTemplet.level + 1);
		SetCost(levelTemplet2, currentBuildPoint);
		SetBuildButtonAndAlert(cityID, levelTemplet2, bUpgrade: true);
		UIOpened();
	}

	private void SetCost(NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet, int currentBuildPoint)
	{
		if (levelTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objBuildCost, bValue: true);
			bool flag = levelTemplet.reqBuildingPoint > 0;
			NKCUtil.SetGameobjectActive(m_tagBuildPoint, flag);
			if (flag)
			{
				m_tagBuildPoint.SetData(911, levelTemplet.reqBuildingPoint, currentBuildPoint);
			}
			{
				foreach (NKMWorldMapBuildingTemplet.LevelTemplet.CostItem buildCostItem in levelTemplet.BuildCostItems)
				{
					if (buildCostItem.ItemID == 1)
					{
						m_tagCredit.SetData(buildCostItem.ItemID, buildCostItem.Count, NKCScenManager.CurrentUserData().GetCredit());
					}
					else
					{
						Debug.LogError($"Unexpected Build Cost! building id {levelTemplet.id}, level {levelTemplet.level}");
					}
				}
				return;
			}
		}
		NKCUtil.SetGameobjectActive(m_objBuildCost, bValue: false);
		NKCUtil.SetGameobjectActive(m_tagBuildPoint, bValue: false);
	}

	private void SetDataCommon(NKMWorldMapBuildingTemplet buildingTemplet, NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet, int currentBuildPoint, int cityBuildingCount)
	{
		m_BuildingID = buildingTemplet.Key;
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_world_map_renewal_building_icon", levelTemplet.iconPath);
		NKCUtil.SetImageSprite(m_imgBuilding, orLoadAssetResource, bDisableIfSpriteNull: true);
		NKCUtil.SetLabelText(m_lbName, levelTemplet.GetName());
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, levelTemplet.level));
		NKCUtil.SetLabelText(m_lbMaxLevel, string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_MAX_LEVEL_ONE_PARAM, buildingTemplet.FindMaxLevel()));
		NKCUtil.SetLabelText(m_lbBuildingPoint, currentBuildPoint.ToString());
		NKCUtil.SetLabelText(m_lbDescription, levelTemplet.GetDesc());
		NKCUtil.SetLabelText(m_lbInformation, levelTemplet.GetInfo());
		NKCUtil.SetLabelText(m_lbSlotCount, string.Empty);
	}

	private void SetBuildButtonAndAlert(int cityID, NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet, bool bUpgrade)
	{
		if (levelTemplet != null)
		{
			if (((!bUpgrade) ? NKMWorldMapManager.CanBuild(NKCScenManager.CurrentUserData(), cityID, levelTemplet.id) : NKMWorldMapManager.CanLevelUpBuilding(NKCScenManager.CurrentUserData(), cityID, levelTemplet.id)) == NKM_ERROR_CODE.NEC_OK)
			{
				m_csbtnBuild.UnLock();
			}
			else
			{
				m_csbtnBuild.Lock();
			}
			StringBuilder stringBuilder = new StringBuilder();
			NKMWorldMapCityData cityData = NKCScenManager.CurrentUserData().m_WorldmapData.GetCityData(cityID);
			if (levelTemplet.reqCityLevel > 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				string text = "- " + string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_REQ_CITY_LEVEL_ONE_PARAM, levelTemplet.reqCityLevel);
				if (cityData.level < levelTemplet.reqCityLevel)
				{
					text = GetRedString(text);
				}
				stringBuilder.Append(text);
			}
			if (levelTemplet.reqBuildingID != 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				NKMWorldMapBuildingTemplet.LevelTemplet cityBuildingTemplet = NKMWorldMapManager.GetCityBuildingTemplet(levelTemplet.reqBuildingID, levelTemplet.reqBuildingLevel);
				string text2 = "- " + string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_REQ_BUILD_TWO_PARAM, cityBuildingTemplet.GetName(), cityBuildingTemplet.level);
				NKMWorldmapCityBuildingData buildingData = cityData.GetBuildingData(levelTemplet.reqBuildingID);
				if (buildingData == null || buildingData.level < levelTemplet.reqBuildingLevel)
				{
					text2 = GetRedString(text2);
				}
				stringBuilder.Append(text2);
			}
			if (levelTemplet.reqClearDiveId != 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(levelTemplet.reqClearDiveId);
				string text3 = "- " + string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_DIVE_CLEAR_ONE_PARAM, nKMDiveTemplet.IndexID);
				if (!NKCScenManager.CurrentUserData().CheckDiveHistory(levelTemplet.reqClearDiveId))
				{
					text3 = GetRedString(text3);
				}
				stringBuilder.Append(text3);
			}
			if (levelTemplet.notBuildingTogether != 0)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				NKMWorldMapBuildingTemplet.LevelTemplet cityBuildingTemplet2 = NKMWorldMapManager.GetCityBuildingTemplet(levelTemplet.notBuildingTogether, 1);
				string text4 = "- " + string.Format(NKCStringTable.GetString("SI_DP_WORLDMAP_BUILDING_NOT_BUILDING_ONE_PARAM"), cityBuildingTemplet2.GetName());
				if (cityData.GetBuildingData(levelTemplet.notBuildingTogether) != null)
				{
					text4 = GetRedString(text4);
				}
				stringBuilder.Append(text4);
			}
			NKCUtil.SetLabelText(m_lbAlert, stringBuilder.ToString());
			NKCUtil.SetGameobjectActive(m_objRootAlert, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRootAlert, bValue: false);
			m_csbtnBuild.Lock();
		}
	}

	private string GetRedString(string str)
	{
		return "<color=#FF0000>" + str + "</color>";
	}

	private void OnBtnBuild()
	{
		dOnBuild?.Invoke(m_BuildingID);
	}

	private void OnBtnRemove()
	{
		dOnDestory?.Invoke(m_BuildingID);
	}
}
