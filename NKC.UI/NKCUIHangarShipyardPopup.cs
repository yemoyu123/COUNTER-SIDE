using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIHangarShipyardPopup : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_HANGAR";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_HANGAR_SHIPYARD";

	private static NKCUIHangarShipyardPopup m_Instance;

	[Header("공용")]
	public NKCUIItemCostSlot[] m_costSlot;

	public NKCUIComButton NKM_UI_POPUP_CANCEL;

	public NKCUIComButton NKM_UI_POPUP_OK;

	public Text m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TITLE;

	public Text m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TEXT;

	public Text m_txt_NKM_UI_HANGAR_SHIPYARD_ITEM_TITLE_TEXT;

	public GameObject NKM_UI_POPUP_HANGAR_SHIPYARD_CONTENTS_LEVELUP;

	public GameObject NKM_UI_POPUP_HANGAR_SHIPYARD_CONTENTS_UPGRADE;

	public GameObject m_objLimitBreak;

	[Header("함선 레벨업 관련")]
	public Text m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_CONTENTS_LevelUp_Perv;

	public Text m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_CONTENTS_LevelUp_Next;

	[Header("레벨업 어빌리티")]
	public Text m_PREV_SHIP_ABILITY_POWER_COUNT;

	public Text m_PREV_SHIP_ABILITY_ATK_COUNT;

	public Text m_PREV_SHIP_ABILITY_HP_COUNT;

	public Text m_PREV_SHIP_ABILITY_DEF_COUNT;

	public Text m_NEXT_SHIP_ABILITY_POWER_COUNT;

	public Text m_NEXT_SHIP_ABILITY_ATK_COUNT;

	public Text m_NEXT_SHIP_ABILITY_ATK_COUNT_ADD;

	public Text m_NEXT_SHIP_ABILITY_HP_COUNT;

	public Text m_NEXT_SHIP_ABILITY_HP_COUNT_ADD;

	public Text m_NEXT_SHIP_ABILITY_DEF_COUNT;

	public Text m_NEXT_SHIP_ABILITY_DEF_COUNT_ADD;

	[Header("함선 개장 관련")]
	public List<GameObject> m_lstCurStar;

	public List<GameObject> m_lstNextStar;

	public RectTransform m_NKM_UI_SHIPYARD_Upgrade_INFO_STAR_EFFECT;

	[Header("개장 스킬 아이콘")]
	public NKCUIShipSkillSlot[] m_PrevSkillSlot;

	public NKCUIShipSkillSlot[] m_NextSkillSlot;

	[Header("함선 초월 관련")]
	public Text m_lbPrevLimitBreakLevel;

	public Text m_lbNextLimitBreakLevel;

	public NKCUISlot m_costShipSlot;

	public List<GameObject> m_lstModuleBefore = new List<GameObject>();

	public List<GameObject> m_lstModuleAfter = new List<GameObject>();

	public Text m_lbNewModule;

	private NKCUIOpenAnimator m_openAni;

	private NKCUISlot.SlotData m_costShipSlotData;

	private UnityAction dOnConfirm;

	private UnityAction dOnConfirmLevelUp;

	private UnityAction dOnConfirmUpgrade;

	private UnityAction dOnConfirmLimitBreak;

	public static NKCUIHangarShipyardPopup Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIHangarShipyardPopup>("AB_UI_NKM_UI_HANGAR", "NKM_UI_POPUP_HANGAR_SHIPYARD", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIHangarShipyardPopup>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_MENU_NAME_SHIPYARD_CONFIRM;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.ResourceOnly;

	public override List<int> UpsideMenuShowResourceList => base.UpsideMenuShowResourceList;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Init()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_costShipSlot?.Init();
		NKCUIItemCostSlot[] costSlot = m_costSlot;
		for (int i = 0; i < costSlot.Length; i++)
		{
			NKCUtil.SetGameobjectActive(costSlot[i], bValue: false);
		}
		if (NKM_UI_POPUP_CANCEL != null)
		{
			NKM_UI_POPUP_CANCEL.PointerClick.RemoveAllListeners();
			NKM_UI_POPUP_CANCEL.PointerClick.AddListener(ButtonCancel);
		}
		if (NKM_UI_POPUP_OK != null)
		{
			NKM_UI_POPUP_OK.PointerClick.RemoveAllListeners();
			NKM_UI_POPUP_OK.PointerClick.AddListener(ButtonOk);
			NKCUtil.SetHotkey(NKM_UI_POPUP_OK, HotkeyEventType.Confirm);
		}
		m_openAni = new NKCUIOpenAnimator(base.gameObject);
		for (int j = 0; j < m_PrevSkillSlot.Length; j++)
		{
			m_PrevSkillSlot[j].Init(null, bCallToolTip: true);
		}
		for (int k = 0; k < m_NextSkillSlot.Length; k++)
		{
			m_NextSkillSlot[k].Init(null, bCallToolTip: true);
		}
	}

	public void Open(NKCUIShipInfoRepair.ShipRepairInfo targetShipData, UnityAction TryShipLevelUp, UnityAction TryShipUpgrade, UnityAction TryShipLimitBreak, NKCUISlot.SlotData costShipSlotData = null)
	{
		if (targetShipData != null && targetShipData.ShipData != null)
		{
			dOnConfirmLevelUp = TryShipLevelUp;
			dOnConfirmUpgrade = TryShipUpgrade;
			dOnConfirmLimitBreak = TryShipLimitBreak;
			m_costShipSlotData = costShipSlotData;
			if (m_openAni != null)
			{
				m_openAni.PlayOpenAni();
			}
			if (targetShipData.eRepairState == NKCUIShipInfoRepair.RepairState.LevelUp)
			{
				dOnConfirm = dOnConfirmLevelUp;
			}
			else if (targetShipData.eRepairState == NKCUIShipInfoRepair.RepairState.Upgrade)
			{
				dOnConfirm = dOnConfirmUpgrade;
			}
			else if (targetShipData.eRepairState == NKCUIShipInfoRepair.RepairState.LimitBreak)
			{
				dOnConfirm = dOnConfirmLimitBreak;
			}
			UpdateData(targetShipData);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(NKM_UI_POPUP_HANGAR_SHIPYARD_CONTENTS_LEVELUP, targetShipData.eRepairState == NKCUIShipInfoRepair.RepairState.LevelUp);
			NKCUtil.SetGameobjectActive(m_NKM_UI_SHIPYARD_Upgrade_INFO_STAR_EFFECT.gameObject, targetShipData.eRepairState == NKCUIShipInfoRepair.RepairState.Upgrade);
			NKCUtil.SetGameobjectActive(NKM_UI_POPUP_HANGAR_SHIPYARD_CONTENTS_UPGRADE, targetShipData.eRepairState == NKCUIShipInfoRepair.RepairState.Upgrade);
			NKCUtil.SetGameobjectActive(m_objLimitBreak, targetShipData.eRepairState == NKCUIShipInfoRepair.RepairState.LimitBreak);
			UIOpened();
		}
	}

	private void UpdateData(NKCUIShipInfoRepair.ShipRepairInfo curShipData)
	{
		if (NKCUtil.IsNullObject(curShipData.ShipData))
		{
			return;
		}
		UpdateMaterialSlot(curShipData);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(curShipData.ShipData.m_UnitID);
		if (curShipData.eRepairState == NKCUIShipInfoRepair.RepairState.LevelUp)
		{
			NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_CONTENTS_LevelUp_Perv, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, curShipData.ShipData.m_UnitLevel));
			NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_CONTENTS_LevelUp_Next, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, curShipData.iTargetLevel));
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData == null || nKMUserData.m_InventoryData == null)
			{
				return;
			}
			NKCUtil.SetLabelText(m_PREV_SHIP_ABILITY_POWER_COUNT, curShipData.ShipData.CalculateOperationPower(nKMUserData.m_InventoryData, curShipData.iCurShipLevel).ToString("N0"));
			NKCUtil.SetLabelText(m_PREV_SHIP_ABILITY_ATK_COUNT, Mathf.RoundToInt(curShipData.fCurShipAtk).ToString());
			NKCUtil.SetLabelText(m_PREV_SHIP_ABILITY_HP_COUNT, Mathf.RoundToInt(curShipData.fCurShipHP).ToString());
			NKCUtil.SetLabelText(m_PREV_SHIP_ABILITY_DEF_COUNT, Mathf.RoundToInt(curShipData.fCurShipDef).ToString());
			NKCUtil.SetLabelText(m_NEXT_SHIP_ABILITY_POWER_COUNT, curShipData.ShipData.CalculateOperationPower(nKMUserData.m_InventoryData, curShipData.iTargetLevel).ToString("N0"));
			NKCUtil.SetLabelText(m_NEXT_SHIP_ABILITY_ATK_COUNT, Mathf.RoundToInt(curShipData.fNextShipAtk).ToString());
			NKCUtil.SetLabelText(m_NEXT_SHIP_ABILITY_ATK_COUNT_ADD, $"(+{Mathf.RoundToInt(curShipData.fNextShipAtk) - Mathf.RoundToInt(curShipData.fCurShipAtk)})");
			NKCUtil.SetLabelText(m_NEXT_SHIP_ABILITY_HP_COUNT, Mathf.RoundToInt(curShipData.fNextShipHP).ToString());
			NKCUtil.SetLabelText(m_NEXT_SHIP_ABILITY_HP_COUNT_ADD, $"(+{Mathf.RoundToInt(curShipData.fNextShipHP) - Mathf.RoundToInt(curShipData.fCurShipHP)})");
			NKCUtil.SetLabelText(m_NEXT_SHIP_ABILITY_DEF_COUNT, Mathf.RoundToInt(curShipData.fNextShipDef).ToString());
			NKCUtil.SetLabelText(m_NEXT_SHIP_ABILITY_DEF_COUNT_ADD, $"(+{Mathf.RoundToInt(curShipData.fNextShipDef) - Mathf.RoundToInt(curShipData.fCurShipDef)})");
		}
		else if (curShipData.eRepairState == NKCUIShipInfoRepair.RepairState.Upgrade)
		{
			NKCUtil.SetStarRank(m_lstCurStar, curShipData.iCurStar, 6);
			NKCUtil.SetStarRank(m_lstNextStar, curShipData.iNextStar, 6);
			if (m_NKM_UI_SHIPYARD_Upgrade_INFO_STAR_EFFECT != null)
			{
				m_NKM_UI_SHIPYARD_Upgrade_INFO_STAR_EFFECT.localPosition = m_lstNextStar[curShipData.iNextStar - 1].GetComponent<RectTransform>().localPosition;
			}
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(curShipData.ShipData.m_UnitID);
			NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(curShipData.iNextShipID);
			if (unitTempletBase2 == null || unitTempletBase3 == null)
			{
				Debug.LogError($"curShipTemplet is null : {unitTempletBase2 == null} // ship id info : {curShipData.ShipData.m_UnitID}, next : {curShipData.iNextShipID}");
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				if (m_PrevSkillSlot[i] == null || m_NextSkillSlot[i] == null)
				{
					Debug.LogError($"m_PrevSkillSlot is null : cnt {i}");
					break;
				}
				NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(unitTempletBase2, i);
				NKMShipSkillTemplet shipSkillTempletByIndex2 = NKMShipSkillManager.GetShipSkillTempletByIndex(unitTempletBase3, i);
				if (shipSkillTempletByIndex != null && shipSkillTempletByIndex2 != null)
				{
					m_PrevSkillSlot[i].SetData(shipSkillTempletByIndex);
					if (string.Equals(shipSkillTempletByIndex.m_ShipSkillStrID, shipSkillTempletByIndex2.m_ShipSkillStrID))
					{
						m_NextSkillSlot[i].SetData(shipSkillTempletByIndex2);
					}
					else
					{
						m_NextSkillSlot[i].SetData(shipSkillTempletByIndex2, NKCUIShipSkillSlot.eShipSkillSlotStatus.ENHANCE);
					}
					NKCUtil.SetGameobjectActive(m_PrevSkillSlot[i].gameObject, bValue: true);
					NKCUtil.SetGameobjectActive(m_NextSkillSlot[i].gameObject, bValue: true);
				}
				else if (shipSkillTempletByIndex == null && shipSkillTempletByIndex2 == null)
				{
					NKCUtil.SetGameobjectActive(m_PrevSkillSlot[i].gameObject, bValue: false);
					NKCUtil.SetGameobjectActive(m_NextSkillSlot[i].gameObject, bValue: false);
				}
				else if (shipSkillTempletByIndex == null && shipSkillTempletByIndex2 != null)
				{
					NKCUtil.SetGameobjectActive(m_PrevSkillSlot[i].gameObject, bValue: false);
					NKCUtil.SetGameobjectActive(m_NextSkillSlot[i].gameObject, bValue: true);
					m_NextSkillSlot[i].SetData(shipSkillTempletByIndex2, NKCUIShipSkillSlot.eShipSkillSlotStatus.NEW);
				}
			}
		}
		else if (curShipData.eRepairState == NKCUIShipInfoRepair.RepairState.LimitBreak)
		{
			NKCUtil.SetLabelText(m_lbPrevLimitBreakLevel, string.Format(NKCUtilString.GET_STRING_SHIP_LIMITBREAK_GRADE, curShipData.ShipData.m_LimitBreakLevel));
			NKCUtil.SetLabelText(m_lbNextLimitBreakLevel, string.Format(NKCUtilString.GET_STRING_SHIP_LIMITBREAK_GRADE, curShipData.ShipData.m_LimitBreakLevel + 1));
			NKCUtil.SetLabelText(m_lbNewModule, string.Format(NKCUtilString.GET_STRING_SHIP_LIMITBREAK_GRADE_COMMANDMODULE_UNLOCK, curShipData.ShipData.m_LimitBreakLevel + 1));
			for (int j = 0; j < m_lstModuleBefore.Count; j++)
			{
				NKCUtil.SetGameobjectActive(m_lstModuleBefore[j], j < curShipData.ShipData.m_LimitBreakLevel);
				NKCUtil.SetGameobjectActive(m_lstModuleAfter[j], j < curShipData.ShipData.m_LimitBreakLevel + 1);
			}
		}
		SetTitleText(curShipData.eRepairState, unitTempletBase.GetUnitName());
	}

	private void UpdateMaterialSlot(NKCUIShipInfoRepair.ShipRepairInfo curShipInfo)
	{
		NKCUtil.SetGameobjectActive(m_costShipSlot, curShipInfo.eRepairState == NKCUIShipInfoRepair.RepairState.LimitBreak);
		if (curShipInfo.eRepairState == NKCUIShipInfoRepair.RepairState.LimitBreak)
		{
			m_costShipSlot.SetData(m_costShipSlotData);
			m_costShipSlot.SetOnClickAction(default(NKCUISlot.SlotClickType));
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKCUtil.SetGameobjectActive(m_costSlot[0], curShipInfo.iNeedCredit > 0);
		if (curShipInfo.iNeedCredit > 0)
		{
			m_costSlot[0].SetData(1, curShipInfo.iNeedCredit, nKMUserData.GetCredit());
		}
		Dictionary<int, int>.Enumerator enumerator = curShipInfo.dicMaterialList.GetEnumerator();
		bool flag = true;
		for (int i = 1; i < m_costSlot.Length; i++)
		{
			NKCUIItemCostSlot nKCUIItemCostSlot = m_costSlot[i];
			flag = enumerator.MoveNext();
			NKCUtil.SetGameobjectActive(nKCUIItemCostSlot, flag);
			if (flag)
			{
				long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(enumerator.Current.Key);
				nKCUIItemCostSlot.SetData(enumerator.Current.Key, enumerator.Current.Value, countMiscItem);
			}
			else
			{
				nKCUIItemCostSlot.SetData(0, 0, 0L);
			}
		}
	}

	private void ButtonCancel()
	{
		Close();
	}

	private void ButtonOk()
	{
		if (!NKM_UI_POPUP_OK.m_bLock)
		{
			if (dOnConfirm != null)
			{
				dOnConfirm();
			}
			Close();
		}
	}

	private void SetTitleText(NKCUIShipInfoRepair.RepairState state, string shipName = "")
	{
		switch (state)
		{
		case NKCUIShipInfoRepair.RepairState.LevelUp:
			NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TITLE, NKCUtilString.GET_STRING_HANGAR_LVUP);
			if (string.IsNullOrEmpty(shipName))
			{
				NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TEXT, NKCUtilString.GET_STRING_HANGAR_SHIPYARD_POPUP_LEVEL_UP_TEXT);
			}
			else
			{
				NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TEXT, shipName + NKCUtilString.GET_STRING_HANGAR_SHIPYARD_POPUP_DESC_LEVEL_UP);
			}
			NKCUtil.SetLabelText(m_txt_NKM_UI_HANGAR_SHIPYARD_ITEM_TITLE_TEXT, NKCUtilString.GET_STRING_HANGAR_SHIPYARD_POPUP_LEVEL_UP_MISC_TEXT);
			break;
		case NKCUIShipInfoRepair.RepairState.Upgrade:
			NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TITLE, NKCUtilString.GET_STRING_HANGAR_SHIPYARD_POPUP_UPGRADE_TITLE);
			if (string.IsNullOrEmpty(shipName))
			{
				NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TEXT, NKCUtilString.GET_STRING_HANGAR_SHIPYARD_POPUP_UPGRADE_TEXT);
			}
			else
			{
				NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TEXT, shipName + NKCUtilString.GET_STRING_HANGAR_SHIPYARD_POPUP_DESC_UPGRADE);
			}
			NKCUtil.SetLabelText(m_txt_NKM_UI_HANGAR_SHIPYARD_ITEM_TITLE_TEXT, NKCUtilString.GET_STRING_HANGAR_SHIPYARD_POPUP_UPGRADE_MISC_TEXT);
			break;
		case NKCUIShipInfoRepair.RepairState.LimitBreak:
			NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TITLE, NKCUtilString.GET_STRING_SHIP_LIMITBREAK);
			NKCUtil.SetLabelText(m_txt_NKM_UI_POPUP_HANGAR_SHIPYARD_TEXT, NKCUtilString.GET_STRING_SHIP_LIMITBREAK_POPUP_DESC);
			NKCUtil.SetLabelText(m_txt_NKM_UI_HANGAR_SHIPYARD_ITEM_TITLE_TEXT, NKCUtilString.GET_STRING_HANGAR_SHIPYARD_POPUP_UPGRADE_MISC_TEXT);
			break;
		}
	}

	private void Update()
	{
		if (m_openAni != null)
		{
			m_openAni.Update();
		}
	}
}
