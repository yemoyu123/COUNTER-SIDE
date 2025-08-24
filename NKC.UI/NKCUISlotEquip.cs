using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISlotEquip : NKCUISlot
{
	public delegate void OnSelectedEquipSlot(NKCUISlotEquip slot, NKMEquipItemData data);

	[Header("프리셋 태그")]
	public GameObject m_objPresetTag;

	[Header("장비 잠김 표시")]
	public GameObject m_objLock;

	[Header("장비 잠금 선택")]
	public GameObject m_objSelectLock;

	[Header("장비 분해 선택")]
	public GameObject m_objSelectDelete;

	[Header("장비 빈칸")]
	public GameObject m_objEquipEmpty;

	[Header("장비 보유 수")]
	public GameObject m_objEquipHaveCount;

	public Text m_lbEquipHaveCount;

	[Header("장착중 유닛")]
	public GameObject m_objUsedUnit;

	public Image m_imgUsedUnit;

	[Header("격전지원")]
	public GameObject m_NKM_UI_ITEM_EQUIP_FIERCE_BATTLE;

	public Text m_NKM_UI_UNIT_SELECT_LIST_FIERCE_BATTLE_TEXT;

	[Header("메세지")]
	public GameObject m_objMessage;

	public Text m_lbMessage;

	private OnSelectedEquipSlot m_OnSelectedSlot;

	private NKMEquipItemData m_cNKMEquipItemData;

	private bool m_bLockMaxItem;

	private NKCUIInvenEquipSlot.EQUIP_SLOT_STATE m_EQUIP_SLOT_STATE;

	public static NKCUISlotEquip GetNewInstance(Transform parent, OnSelectedEquipSlot selectedSlot = null)
	{
		NKCUISlotEquip component = NKCAssetResourceManager.OpenInstance<GameObject>("AB_INVEN_ICON", "AB_ICON_SLOT_EQUIP").m_Instant.GetComponent<NKCUISlotEquip>();
		if (component == null)
		{
			Debug.LogError("NKCUISlotEquip Prefab null!");
			return null;
		}
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	public static NKCUISlotEquip GetNewInstanceForEmpty(Transform parent, OnSelectedEquipSlot selectedSlot = null)
	{
		NKCUISlotEquip component = NKCAssetResourceManager.OpenInstance<GameObject>("AB_INVEN_ICON", "AB_ICON_SLOT_EQUIP").m_Instant.GetComponent<NKCUISlotEquip>();
		if (component == null)
		{
			Debug.LogError("NKCUISlotEquip Prefab null!");
			return null;
		}
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void SetData(NKMEquipItemData cNKMEquipItemData, OnSelectedEquipSlot selectedSlot = null, bool lockMaxItem = false, bool bSkipEquipBox = false, bool bShowFierceInfo = false, bool bPresetContained = false)
	{
		if (cNKMEquipItemData == null)
		{
			return;
		}
		m_cNKMEquipItemData = cNKMEquipItemData;
		SlotData data = SlotData.MakeEquipData(cNKMEquipItemData);
		SetData(data, bShowName: false, bShowNumber: true, bEnableLayoutElement: true, delegate
		{
			selectedSlot(this, m_cNKMEquipItemData);
		});
		SetOnSelectedEquipSlot(selectedSlot, lockMaxItem);
		NKCUtil.SetGameobjectActive(m_objLock, cNKMEquipItemData.m_bLock);
		bool flag = false;
		if (cNKMEquipItemData.m_OwnerUnitUID > 0)
		{
			NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(cNKMEquipItemData.m_OwnerUnitUID);
			if (unitFromUID != null)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
				if (unitTempletBase != null)
				{
					NKCUtil.SetGameobjectActive(m_objUsedUnit, bValue: true);
					flag = true;
					NKCUtil.SetImageSprite(m_imgUsedUnit, NKCResourceUtility.GetOrLoadMinimapFaceIcon(unitTempletBase));
				}
			}
		}
		if (!flag)
		{
			NKCUtil.SetGameobjectActive(m_objUsedUnit, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_FIERCE_BATTLE, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPresetTag, bPresetContained);
	}

	public override void TurnOffExtraUI()
	{
		base.TurnOffExtraUI();
		NKCUtil.SetGameobjectActive(m_objEquipHaveCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_ITEM_EQUIP_FIERCE_BATTLE, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUsedUnit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEquipHaveCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelectDelete, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPresetTag, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEquipEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMessage, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRelic, bValue: false);
	}

	public void SetEmptyMaterial(OnSelectedEquipSlot selectedSlot = null)
	{
		SetOnSelectedEquipSlot(selectedSlot);
		TurnOffExtraUI();
		NKCUtil.SetGameobjectActive(m_imgIcon, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbName, bValue: false);
		SetActiveCount(value: false);
		NKCUtil.SetGameobjectActive(m_lbItemAddCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStarRoot, bValue: false);
		if (m_sp_MatAdd != null)
		{
			m_imgBG.sprite = m_sp_MatAdd;
		}
		else
		{
			m_imgBG.sprite = m_spEmpty;
		}
	}

	public void SetOnSelectedEquipSlot(OnSelectedEquipSlot selectedSlot, bool lockMaxItem = false)
	{
		if (selectedSlot != null)
		{
			m_cbtnButton.PointerClick.RemoveAllListeners();
			m_OnSelectedSlot = selectedSlot;
			m_cbtnButton.PointerClick.AddListener(OnSelectedItemSlotImpl);
			m_bLockMaxItem = lockMaxItem;
		}
	}

	public void SetSlotMessage(bool bValue, string message, Color messageColor)
	{
		NKCUtil.SetGameobjectActive(m_objMessage, bValue);
		if (bValue)
		{
			NKCUtil.SetLabelText(m_lbMessage, message);
			NKCUtil.SetLabelTextColor(m_lbMessage, messageColor);
		}
	}

	private void OnSelectedItemSlotImpl()
	{
		if (m_OnSelectedSlot == null)
		{
			return;
		}
		if (m_cNKMEquipItemData != null)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(m_cNKMEquipItemData.m_ItemEquipID);
			if (m_bLockMaxItem && equipTemplet != null && m_cNKMEquipItemData.m_EnchantLevel >= NKMItemManager.GetMaxEquipEnchantLevel(equipTemplet.m_NKM_ITEM_TIER))
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("NEC_FAIL_EQUIP_ITEM_ENCHANT_MAX"));
				return;
			}
		}
		m_OnSelectedSlot(this, m_cNKMEquipItemData);
	}

	public void SetSlotState(NKCUIInvenEquipSlot.EQUIP_SLOT_STATE eEQUIP_SLOT_STATE)
	{
		m_EQUIP_SLOT_STATE = eEQUIP_SLOT_STATE;
		NKCUtil.SetGameobjectActive(m_objSelected, m_EQUIP_SLOT_STATE == NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_SELECTED);
		NKCUtil.SetGameobjectActive(m_objSelectDelete, m_EQUIP_SLOT_STATE == NKCUIInvenEquipSlot.EQUIP_SLOT_STATE.ESS_DELETE);
	}

	public void SetLock(bool bLock, bool bBig = false)
	{
		NKCUtil.SetGameobjectActive(m_objLock, bLock);
		NKCUtil.SetGameobjectActive(m_objSelectLock, bLock && bBig);
	}

	public void SetEmpty(OnSelectedEquipSlot selectedSlot = null, NKMEquipItemData cNKMEquipItemData = null)
	{
		SlotData data = SlotData.MakeEquipData(cNKMEquipItemData);
		SetData(data, bShowName: false, bShowNumber: true, bEnableLayoutElement: true, delegate
		{
			selectedSlot(this, cNKMEquipItemData);
		});
		TurnOffExtraUI();
		SetOnSelectedEquipSlot(selectedSlot);
		NKCUtil.SetGameobjectActive(m_objEquipEmpty, bValue: true);
		m_cNKMEquipItemData = cNKMEquipItemData;
	}

	public void SetUpgradeSlotState(NKC_EQUIP_UPGRADE_STATE state)
	{
		NKCUtil.SetGameobjectActive(m_objMessage, state != NKC_EQUIP_UPGRADE_STATE.UPGRADABLE);
		NKCUtil.SetLabelText(m_lbMessage, NKCUtilString.GetEquipUpgradeStateString(state));
		NKCUtil.SetLabelTextColor(m_lbMessage, (state == NKC_EQUIP_UPGRADE_STATE.UPGRADABLE) ? Color.white : Color.red);
	}

	public NKMEquipItemData GetNKMEquipItemData()
	{
		return m_cNKMEquipItemData;
	}

	public NKMEquipTemplet GetNKMEquipTemplet()
	{
		if (m_cNKMEquipItemData == null)
		{
			return null;
		}
		return NKMItemManager.GetEquipTemplet(m_cNKMEquipItemData.m_ItemEquipID);
	}

	public NKCUIInvenEquipSlot.EQUIP_SLOT_STATE Get_EQUIP_SLOT_STATE()
	{
		return m_EQUIP_SLOT_STATE;
	}

	public long GetEquipItemUID()
	{
		if (m_cNKMEquipItemData != null)
		{
			return m_cNKMEquipItemData.m_ItemUid;
		}
		return 0L;
	}
}
