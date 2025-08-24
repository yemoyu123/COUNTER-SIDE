using System.Text;
using ClientPacket.WorldMap;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCUIWorldMapCityBuildingSlot : MonoBehaviour
{
	public delegate void OnSelectSlot(int m_BuildingID);

	public NKCUIComStateButton m_csbtnSlot;

	public Image m_imgIcon;

	public Text m_lbName;

	public Text m_lbLevel;

	public GameObject m_objMaxLevel;

	public Text m_lbInformation;

	[Header("관리시")]
	public GameObject m_objRootManage;

	public GameObject m_objEmpty;

	public GameObject m_objCanLevelUp;

	public GameObject m_objReqCityLevel;

	public Text m_lbReqCityLevel;

	public Transform m_trEffect;

	[Header("건설시")]
	public GameObject m_objRootBuild;

	public GameObject m_objRootPrice;

	public NKCUIPriceTag m_tagReqBuildPoint;

	public NKCUIPriceTag m_tagCreditPrice;

	public GameObject m_objLock;

	public Text m_lbLockReason;

	private int m_BuildingID;

	private OnSelectSlot dOnSelectSlot;

	private NKCAssetInstanceData m_buildFX;

	public int BuildingID => m_BuildingID;

	public void Init()
	{
		if (m_csbtnSlot != null)
		{
			m_csbtnSlot.PointerClick.RemoveAllListeners();
			m_csbtnSlot.PointerClick.AddListener(OnClick);
		}
	}

	public void SetEmpty(OnSelectSlot onSelectSlot)
	{
		NKCUtil.SetGameobjectActive(m_objRootManage, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootBuild, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: true);
		NKCUtil.SetGameobjectActive(m_objCanLevelUp, bValue: false);
		NKCUtil.SetGameobjectActive(m_objReqCityLevel, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootPrice, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
		dOnSelectSlot = onSelectSlot;
	}

	public void SetDataCommonOnly(int BuildingID, int Level, OnSelectSlot onSelectSlot)
	{
		NKMWorldMapBuildingTemplet nKMWorldMapBuildingTemplet = NKMWorldMapBuildingTemplet.Find(BuildingID);
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = nKMWorldMapBuildingTemplet.GetLevelTemplet(Level);
		SetDataCommonOnly(nKMWorldMapBuildingTemplet, levelTemplet, onSelectSlot);
	}

	public void SetDataCommonOnly(NKMWorldMapBuildingTemplet buildingTemplet, NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet, OnSelectSlot onSelectSlot)
	{
		SetDataCommon(buildingTemplet, levelTemplet, onSelectSlot);
		NKCUtil.SetGameobjectActive(m_objRootManage, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootBuild, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanLevelUp, bValue: false);
		NKCUtil.SetGameobjectActive(m_objReqCityLevel, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootPrice, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
	}

	public void SetDataForManage(NKMWorldMapCityData cityData, int cityBuildPointLeft, int BuildingID, int Level, OnSelectSlot onSelectSlot)
	{
		NKMWorldMapBuildingTemplet nKMWorldMapBuildingTemplet = NKMWorldMapBuildingTemplet.Find(BuildingID);
		if (nKMWorldMapBuildingTemplet == null)
		{
			Log.Error($"Building ID: {BuildingID} is not exist in WORLDMAP_CITY_BUILDING templet", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/WorldMap/NKCUIWorldMapCityBuildingSlot.cs", 95);
			return;
		}
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = nKMWorldMapBuildingTemplet.GetLevelTemplet(Level);
		SetDataForManage(cityData, cityBuildPointLeft, nKMWorldMapBuildingTemplet, levelTemplet, onSelectSlot);
	}

	public void SetDataForManage(NKMWorldMapCityData cityData, int cityBuildPointLeft, NKMWorldMapBuildingTemplet buildingTemplet, NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet, OnSelectSlot onSelectSlot)
	{
		SetDataCommon(buildingTemplet, levelTemplet, onSelectSlot);
		NKCUtil.SetGameobjectActive(m_objRootManage, bValue: true);
		NKCUtil.SetGameobjectActive(m_objRootBuild, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootPrice, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		bool bValue = NKMWorldMapManager.CanLevelUpBuilding(NKCScenManager.CurrentUserData(), cityData.cityID, buildingTemplet.Key) == NKM_ERROR_CODE.NEC_OK;
		NKCUtil.SetGameobjectActive(m_objCanLevelUp, bValue);
		NKCUtil.SetGameobjectActive(m_objReqCityLevel, bValue: false);
	}

	public void SetDataForBuild(NKMWorldMapCityData cityData, int cityBuildPointLeft, int BuildingID, OnSelectSlot onSelectSlot)
	{
		NKMWorldMapBuildingTemplet buildingTemplet = NKMWorldMapBuildingTemplet.Find(BuildingID);
		SetDataForBuild(cityData, cityBuildPointLeft, buildingTemplet, onSelectSlot);
	}

	public void SetDataForBuild(NKMWorldMapCityData cityData, int cityBuildPointLeft, NKMWorldMapBuildingTemplet buildingTemplet, OnSelectSlot onSelectSlot)
	{
		NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = buildingTemplet.GetLevelTemplet(1);
		if (levelTemplet == null)
		{
			return;
		}
		SetDataCommon(buildingTemplet, levelTemplet, onSelectSlot);
		NKCUtil.SetGameobjectActive(m_objRootManage, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootBuild, bValue: true);
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCanLevelUp, bValue: false);
		NKCUtil.SetGameobjectActive(m_objReqCityLevel, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRootPrice, bValue: true);
		m_tagReqBuildPoint.SetDataByHaveCount(levelTemplet.reqBuildingPoint, cityBuildPointLeft);
		foreach (NKMWorldMapBuildingTemplet.LevelTemplet.CostItem buildCostItem in levelTemplet.BuildCostItems)
		{
			if (buildCostItem.ItemID == 1)
			{
				m_tagCreditPrice.SetData(buildCostItem.ItemID, buildCostItem.Count);
			}
			else
			{
				Debug.LogError($"Unexpected Build Cost! building id {levelTemplet.id}, level {levelTemplet.level}");
			}
		}
		if (CanBuild(cityData, levelTemplet, out var reason))
		{
			NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objLock, bValue: true);
		NKCUtil.SetLabelText(m_lbLockReason, reason);
	}

	private bool CanBuild(NKMWorldMapCityData cityData, NKMWorldMapBuildingTemplet.LevelTemplet targetBuildingLevelTemplet, out string reason)
	{
		bool result = true;
		StringBuilder stringBuilder = new StringBuilder();
		if (cityData.GetBuildingData(targetBuildingLevelTemplet.id) != null)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_ALREADY_BUILD);
			result = false;
		}
		if (cityData.level < targetBuildingLevelTemplet.reqCityLevel)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_REQ_CITY_LEVEL_ONE_PARAM, targetBuildingLevelTemplet.reqCityLevel));
			result = false;
		}
		if (targetBuildingLevelTemplet.reqBuildingID != 0)
		{
			NKMWorldmapCityBuildingData buildingData = cityData.GetBuildingData(targetBuildingLevelTemplet.reqBuildingID);
			if (buildingData == null || buildingData.level < targetBuildingLevelTemplet.reqBuildingLevel)
			{
				NKMWorldMapBuildingTemplet nKMWorldMapBuildingTemplet = NKMWorldMapBuildingTemplet.Find(targetBuildingLevelTemplet.reqBuildingID);
				if (nKMWorldMapBuildingTemplet == null)
				{
					Debug.LogError("Required Building Not Found!!");
					result = false;
				}
				NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet = nKMWorldMapBuildingTemplet.GetLevelTemplet(targetBuildingLevelTemplet.reqBuildingLevel);
				if (levelTemplet == null)
				{
					Debug.LogError("Required Building Not Found!!");
					result = false;
				}
				if (targetBuildingLevelTemplet.reqBuildingLevel > 1)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.Append(string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_REQ_BUILD_TWO_PARAM, levelTemplet.GetName(), targetBuildingLevelTemplet.reqBuildingLevel));
				}
				else
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.Append(string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_REQ_BUILDING_ONE_PARAM, levelTemplet.GetName()));
				}
				result = false;
			}
		}
		if (targetBuildingLevelTemplet.reqClearDiveId != 0 && !NKCScenManager.CurrentUserData().CheckDiveHistory(targetBuildingLevelTemplet.reqClearDiveId))
		{
			NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(targetBuildingLevelTemplet.reqClearDiveId);
			if (nKMDiveTemplet != null)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(string.Format(NKCUtilString.GET_STRING_WORLDMAP_BUILDING_DIVE_CLEAR_ONE_PARAM, nKMDiveTemplet.IndexID));
			}
			result = false;
		}
		if (targetBuildingLevelTemplet.notBuildingTogether != 0 && cityData.GetBuildingData(targetBuildingLevelTemplet.notBuildingTogether) != null)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine();
			}
			NKMWorldMapBuildingTemplet.LevelTemplet cityBuildingTemplet = NKMWorldMapManager.GetCityBuildingTemplet(targetBuildingLevelTemplet.notBuildingTogether, 1);
			stringBuilder.Append(string.Format(NKCStringTable.GetString("SI_DP_WORLDMAP_NOT_BUILDING_TOGETHER"), cityBuildingTemplet.GetName()));
			result = false;
		}
		reason = stringBuilder.ToString();
		return result;
	}

	private void SetDataCommon(NKMWorldMapBuildingTemplet buildingTemplet, NKMWorldMapBuildingTemplet.LevelTemplet levelTemplet, OnSelectSlot onSelectSlot)
	{
		if (levelTemplet == null)
		{
			Debug.Log("NKMWorldMapBuildingTemplet.LevelTemplet is null");
			return;
		}
		m_BuildingID = levelTemplet.id;
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_world_map_renewal_building_icon", levelTemplet.iconPath);
		NKCUtil.SetImageSprite(m_imgIcon, orLoadAssetResource, bDisableIfSpriteNull: true);
		NKCUtil.SetLabelText(m_lbName, levelTemplet.GetName());
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, levelTemplet.level));
		NKCUtil.SetGameobjectActive(m_objMaxLevel, buildingTemplet.GetLevelTemplet(levelTemplet.level + 1) == null);
		NKCUtil.SetLabelText(m_lbInformation, levelTemplet.GetInfo());
		dOnSelectSlot = onSelectSlot;
	}

	public void SetFx(bool bShow)
	{
		NKCUtil.SetGameobjectActive(m_trEffect, bValue: false);
		if (bShow)
		{
			if (m_buildFX == null)
			{
				m_buildFX = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_UI_BRANCH_BUILD", "AB_FX_UI_BRANCH_BUILD");
				m_buildFX.m_Instant.transform.SetParent(m_trEffect);
				m_buildFX.m_Instant.transform.localPosition = Vector3.zero;
				m_buildFX.m_Instant.transform.localScale = Vector3.one;
			}
			NKCUtil.SetGameobjectActive(m_trEffect, bShow);
			NKCSoundManager.PlaySound("FX_UI_CONTRACT_SLOT_OPEN", 1f, 0f, 0f);
		}
		else if (m_buildFX != null)
		{
			NKCAssetResourceManager.CloseInstance(m_buildFX);
			m_buildFX = null;
		}
	}

	private void OnClick()
	{
		dOnSelectSlot?.Invoke(m_BuildingID);
	}
}
