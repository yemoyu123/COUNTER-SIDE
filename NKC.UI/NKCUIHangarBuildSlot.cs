using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIHangarBuildSlot : MonoBehaviour
{
	public delegate void OnShipInfo(int shipID);

	[Header("함선 이미지")]
	public Image m_SHIP_CARD_IMG;

	public GameObject m_NKM_UI_HANGAR_BUILD_SLOT_LIST_BADGE;

	public NKCUIComStateButton m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPINFO_SHORTCUT_BUTTON;

	public GameObject m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_TAG_GET;

	public Text m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPNAME;

	public Image m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPINFO_GRADE;

	public Image m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPINFO_CLASS_ICON;

	public Text m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPINFO_CLASS_TEXT;

	public Text m_NKM_UI_HANGAR_BUILD_SLOT_LIST_BOTTOM_PASSIVE_TEXT;

	public NKCUIComStateButton[] m_lstSkillBtn;

	public Image[] m_lstSkillSlot;

	public NKCUIItemCostSlot[] m_lstItemSlot;

	public NKCUIComStateButton m_NKM_UI_HANGAR_BUILD_SLOT_LIST_BUTTON;

	[Header("잠금")]
	public GameObject m_BUILD_LOCK_BG;

	public GameObject m_NKM_UI_HANGAR_BUILD_SLOT_SHIP_NOTGET;

	public GameObject m_BUILD_LOCK_NOTICE;

	public Text m_BUILD_LOCK_NOTICE_TEXT;

	[Header("버튼들")]
	public GameObject m_BUTTON_BUILD_LOCK;

	public GameObject m_BUTTON_MATERIAL_LACK;

	public GameObject m_BUTTON_CRAFT;

	[Header("튜토리얼용")]
	public RectTransform m_rectTop;

	public RectTransform m_rectSkill;

	public RectTransform m_rectCost;

	private int m_ShipID;

	private UnityAction m_openAction;

	private UnityAction m_closeAction;

	private OnShipInfo dOnShipInfo;

	public int ShipID => m_ShipID;

	public void InitUI(UnityAction openAction, UnityAction closeAction, OnShipInfo ShowInfo)
	{
		NKCUtil.SetBindFunction(m_NKM_UI_HANGAR_BUILD_SLOT_LIST_BUTTON, OpenConfirmPopup);
		m_openAction = openAction;
		m_closeAction = closeAction;
		dOnShipInfo = ShowInfo;
	}

	private void OpenConfirmPopup()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		if (nKMUserData.m_ArmyData.GetCurrentShipCount() >= nKMUserData.m_ArmyData.m_MaxShipCount)
		{
			int count = 1;
			int resultCount;
			bool flag = !NKCAdManager.IsAdRewardInventory(NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP) || !NKMInventoryManager.CanExpandInventoryByAd(NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP, nKMUserData, count, out resultCount);
			if (!NKMInventoryManager.CanExpandInventory(NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP, nKMUserData, count, out resultCount) && flag)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_CANNOT_EXPAND_INVENTORY));
				return;
			}
			string expandDesc = NKCUtilString.GetExpandDesc(NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP, isFullMsg: true);
			NKCPopupInventoryAdd.SliderInfo sliderInfo = new NKCPopupInventoryAdd.SliderInfo
			{
				increaseCount = 1,
				maxCount = 60,
				currentCount = nKMUserData.m_ArmyData.m_MaxShipCount,
				inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP
			};
			NKCPopupInventoryAdd.Instance.Open(NKCUtilString.GET_STRING_INVENTORY_SHIP, expandDesc, sliderInfo, 100, 101, delegate(int value)
			{
				NKCPacketSender.Send_NKMPacket_INVENTORY_EXPAND_REQ(NKM_INVENTORY_EXPAND_TYPE.NIET_SHIP, value);
			}, showResource: true);
			return;
		}
		if (m_BUTTON_MATERIAL_LACK.activeSelf)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_HANGAR_CONFIRM_FAIL);
		}
		if (m_BUTTON_CRAFT.activeSelf)
		{
			NKCUIPopupHangarBuildConfirm.Instance.Open(m_ShipID, OnTryBuildShip, m_closeAction);
			if (m_openAction != null)
			{
				m_openAction();
			}
		}
	}

	public void SetData(NKMShipBuildTemplet data, bool IsNew = false)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < m_lstItemSlot.Length; i++)
		{
			if (i + 1 > data.BuildMaterialList.Count)
			{
				m_lstItemSlot[i].SetData(0, 0, 0L);
				continue;
			}
			BuildMaterial buildMaterial = data.BuildMaterialList[i];
			m_lstItemSlot[i].SetData(buildMaterial.m_ShipBuildMaterialID, buildMaterial.m_ShipBuildMaterialCount, nKMUserData.m_InventoryData.GetCountMiscItem(buildMaterial.m_ShipBuildMaterialID));
			if (flag)
			{
				flag = buildMaterial.m_ShipBuildMaterialCount <= nKMUserData.m_InventoryData.GetCountMiscItem(buildMaterial.m_ShipBuildMaterialID);
			}
		}
		if (!NKMShipManager.CanUnlockShip(nKMUserData, data))
		{
			NKCUtil.SetGameobjectActive(m_BUTTON_MATERIAL_LACK, bValue: false);
			NKCUtil.SetGameobjectActive(m_BUTTON_CRAFT, bValue: false);
			NKCUtil.SetGameobjectActive(m_BUTTON_BUILD_LOCK, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_HANGAR_BUILD_SLOT_SHIP_NOTGET, bValue: true);
			NKCUtil.SetGameobjectActive(m_BUILD_LOCK_NOTICE, bValue: true);
			NKCUtil.SetLabelText(m_BUILD_LOCK_NOTICE_TEXT, GetUnlockConditionText(data));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_BUTTON_BUILD_LOCK, bValue: false);
			NKCUtil.SetGameobjectActive(m_BUTTON_MATERIAL_LACK, !flag);
			NKCUtil.SetGameobjectActive(m_BUTTON_CRAFT, flag);
			NKCUtil.SetGameobjectActive(m_NKM_UI_HANGAR_BUILD_SLOT_SHIP_NOTGET, bValue: false);
			NKCUtil.SetGameobjectActive(m_BUILD_LOCK_NOTICE, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_HANGAR_BUILD_SLOT_LIST_BADGE, IsNew);
		bool bValue = false;
		foreach (KeyValuePair<long, NKMUnitData> item in NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyShip)
		{
			if (NKMShipManager.IsSameKindShip(item.Value.m_UnitID, data.ShipID))
			{
				bValue = true;
				break;
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_TAG_GET, bValue);
		m_ShipID = data.ShipID;
		NKMUnitTempletBase templetBase = NKMUnitManager.GetUnitTempletBase(data.ShipID);
		if (templetBase == null)
		{
			return;
		}
		Sprite sp = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, templetBase);
		NKCUtil.SetImageSprite(m_SHIP_CARD_IMG, sp);
		NKCUtil.SetLabelText(m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPNAME, templetBase.GetUnitName());
		NKCUtil.SetLabelText(m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPINFO_CLASS_TEXT, templetBase.GetUnitTitle());
		Sprite shipGradeSprite = NKCUtil.GetShipGradeSprite(templetBase.m_NKM_UNIT_GRADE);
		NKCUtil.SetImageSprite(m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPINFO_GRADE, shipGradeSprite, bDisableIfSpriteNull: true);
		Sprite orLoadUnitStyleIcon = NKCResourceUtility.GetOrLoadUnitStyleIcon(templetBase.m_NKM_UNIT_STYLE_TYPE, bSmall: true);
		NKCUtil.SetImageSprite(m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPINFO_CLASS_ICON, orLoadUnitStyleIcon, bDisableIfSpriteNull: true);
		for (int j = 0; j < m_lstSkillSlot.Length; j++)
		{
			NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(templetBase, j);
			if (shipSkillTempletByIndex != null)
			{
				NKCUtil.SetImageSprite(m_lstSkillSlot[j], NKCUtil.GetSkillIconSprite(shipSkillTempletByIndex));
				if (shipSkillTempletByIndex.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_PASSIVE)
				{
					NKCUtil.SetLabelText(m_NKM_UI_HANGAR_BUILD_SLOT_LIST_BOTTOM_PASSIVE_TEXT, shipSkillTempletByIndex.GetDesc());
				}
				NKCUtil.SetGameobjectActive(m_lstSkillSlot[j], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstSkillSlot[j], bValue: false);
			}
		}
		NKCUIComStateButton[] lstSkillBtn = m_lstSkillBtn;
		for (int k = 0; k < lstSkillBtn.Length; k++)
		{
			NKCUtil.SetBindFunction(lstSkillBtn[k], delegate
			{
				NKCPopupSkillFullInfo.ShipInstance.OpenForShip(data.ShipID, 0L);
			});
		}
		NKCUtil.SetBindFunction(m_NKM_UI_HANGAR_BUILD_SLOT_LIST_TOP_SHIPINFO_SHORTCUT_BUTTON, delegate
		{
			OnMoveShipInfo(templetBase.m_UnitID);
		});
	}

	private string GetUnlockConditionText(NKMShipBuildTemplet templet)
	{
		if (templet == null)
		{
			return "";
		}
		switch (templet.ShipBuildUnlockType)
		{
		case NKMShipBuildTemplet.BuildUnlockType.BUT_PLAYER_LEVEL:
			return string.Format(NKCUtilString.GET_STRING_SHIP_BUILD_CONDITION_FAIL_PLAYER_LEVEL, templet.UnlockValue);
		case NKMShipBuildTemplet.BuildUnlockType.BUT_SHIP_GET:
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(templet.UnlockValue);
			return string.Format(NKCUtilString.GET_STRING_SHIP_BUILD_CONDITION_FAIL_SHIP_COLLECT, unitTempletBase2.GetUnitName());
		}
		case NKMShipBuildTemplet.BuildUnlockType.BUT_DUNGEON_CLEAR:
			return GetUnlockConditionDiscription(templet.UnlockValue, templet.UnlockPathDesc);
		case NKMShipBuildTemplet.BuildUnlockType.BUT_WARFARE_CLEAR:
			return GetUnlockConditionDiscription(templet.UnlockValue, templet.UnlockPathDesc, bDungeon: false);
		case NKMShipBuildTemplet.BuildUnlockType.BUT_SHIP_LV100:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(templet.UnlockValue);
			return string.Format(NKCUtilString.GET_STRING_SHIP_BUILD_CONDITION_FAIL_SHIP_LEVEL, unitTempletBase.GetUnitName(), 100);
		}
		case NKMShipBuildTemplet.BuildUnlockType.BUT_SHADOW_CLEAR:
		{
			NKMShadowPalaceTemplet nKMShadowPalaceTemplet = NKMTempletContainer<NKMShadowPalaceTemplet>.Find(templet.UnlockValue);
			if (nKMShadowPalaceTemplet != null)
			{
				return string.Format(NKCUtilString.GET_STRING_SHIP_BUILD_CONDITION_FAIL_SHADOW_CLEAR, nKMShadowPalaceTemplet.PalaceName);
			}
			break;
		}
		}
		return "";
	}

	private string GetUnlockConditionDiscription(int key, string unlockPathDesc = "", bool bDungeon = true)
	{
		string result = "";
		if (bDungeon)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(key);
			if (dungeonTempletBase != null)
			{
				string text = "";
				string text2 = "";
				if (dungeonTempletBase.StageTemplet != null)
				{
					text = dungeonTempletBase.StageTemplet.ActId + "-" + dungeonTempletBase.StageTemplet.m_StageUINum;
					if (dungeonTempletBase.StageTemplet.EpisodeTemplet != null)
					{
						text2 = dungeonTempletBase.StageTemplet.EpisodeTemplet.GetEpisodeName();
					}
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = dungeonTempletBase.GetDungeonName();
				}
				result = ((!string.IsNullOrEmpty(text)) ? string.Format(NKCUtilString.GET_STRING_SHIP_BUILD_CONDITION_FAIL_DUNGEON_CLEAR_VER2, NKCStringTable.GetString(unlockPathDesc), text2, text) : string.Format(NKCUtilString.GET_STRING_SHIP_BUILD_CONDITION_FAIL_DUNGEON_CLEAR, dungeonTempletBase.GetDungeonName()));
			}
		}
		else
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(key);
			if (nKMWarfareTemplet != null)
			{
				NKMStageTempletV2 stageTemplet = nKMWarfareTemplet.StageTemplet;
				if (stageTemplet != null)
				{
					result = string.Format(NKCUtilString.GET_STRING_SHIP_BUILD_CONDITION_FAIL_WARFARE_CLEAR, stageTemplet.EpisodeId - 1, stageTemplet.ActId, stageTemplet.m_StageUINum);
				}
			}
		}
		return result;
	}

	private void OnMoveShipInfo(int shipID)
	{
		dOnShipInfo?.Invoke(shipID);
	}

	private void OnTryBuildShip(int shipID)
	{
		NKCUIPopupHangarBuildConfirm.Instance.Close();
		NKCPacketSender.Send_NKMPacket_SHIP_BUILD_REQ(shipID);
	}

	public RectTransform GetRect(string strValue)
	{
		return strValue.ToUpper() switch
		{
			"SHIP" => m_rectTop, 
			"SKILL" => m_rectSkill, 
			"COST" => m_rectCost, 
			"BUTTON" => m_NKM_UI_HANGAR_BUILD_SLOT_LIST_BUTTON.GetComponent<RectTransform>(), 
			_ => null, 
		};
	}
}
