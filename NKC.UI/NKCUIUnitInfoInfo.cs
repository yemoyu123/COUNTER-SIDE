using System.Collections.Generic;
using ClientPacket.Item;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitInfoInfo : MonoBehaviour
{
	public Text m_lbPowerSummary;

	[Space]
	public NKCUIComStateButton m_GuideBtn;

	public string m_GuideStrID;

	public NKCUIUnitStatSlot m_slotHP;

	public NKCUIUnitStatSlot m_slotAttack;

	public NKCUIUnitStatSlot m_slotDefense;

	public NKCUIUnitStatSlot m_slotHitRate;

	public NKCUIUnitStatSlot m_slotCritHitRate;

	public NKCUIUnitStatSlot m_slotEvade;

	public NKCUIComButton m_cbtnUnitInfoDetailPopup;

	public NKCUIUnitInfoSkillPanel m_UISkillPanel;

	[Header("아이템")]
	public GameObject m_objEquipSlotParent;

	public NKCUISlot m_slotEquipWeapon;

	public NKCUISlot m_slotEquipDefense;

	public NKCUISlot m_slotEquipAcc;

	public NKCUISlot m_slotEquipAcc_2;

	[Space]
	public NKCUISlot m_slotEquipUnitReactor;

	[Header("세트아이템 이펙트")]
	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON;

	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE;

	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC;

	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02;

	[Header("장비 프리셋")]
	public NKCUIComStateButton m_csbtnEquipPreset;

	public NKCUIEquipPreset m_NKCUIEquipPreset;

	[Header("장비 팝업")]
	public GameObject m_objEquipInfo;

	public NKCUIInvenEquipSlot m_slotEquip;

	public NKCUIComStateButton m_btnUnEquip;

	public NKCUIComStateButton m_btnReinforce;

	public NKCUIComStateButton m_btnChange;

	public NKCUIComStateButton m_btnReactor;

	private long m_LatestSelectedEquipUID = -1L;

	private NKMUnitData m_UnitData;

	private bool m_bShowFierceInfo;

	[Header("자동 장착&해제")]
	public NKCUIComStateButton m_NKM_UI_UNIT_INFO_DESC_EQUIP_AUTO;

	public NKCUIComStateButton m_NKM_UI_UNIT_INFO_DESC_EQUIP_RESET;

	private List<long> m_lstAlreadySelectedEquipItem = new List<long>();

	public NKCUIEquipPreset EquipPreset => m_NKCUIEquipPreset;

	public void Init()
	{
		if (m_cbtnUnitInfoDetailPopup != null)
		{
			m_cbtnUnitInfoDetailPopup.PointerClick.RemoveAllListeners();
			m_cbtnUnitInfoDetailPopup.PointerClick.AddListener(OpenUnitInfoDetailPopup);
		}
		if (m_UISkillPanel != null)
		{
			m_UISkillPanel.Init();
			m_UISkillPanel.SetOpenPopupWhenSelected();
		}
		if (m_GuideBtn != null)
		{
			m_GuideBtn.PointerClick.RemoveAllListeners();
			m_GuideBtn.PointerClick.AddListener(delegate
			{
				NKCUIPopUpGuide.Instance.Open(m_GuideStrID);
			});
		}
		m_slotEquipWeapon.Init();
		m_slotEquipWeapon.SetEmpty(OnEmptyEquipWeaponSlotClick);
		m_slotEquipWeapon.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_CHANGE);
		m_slotEquipDefense.Init();
		m_slotEquipDefense.SetEmpty(OnEmptyEquipDefSlotClick);
		m_slotEquipDefense.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_CHANGE);
		m_slotEquipAcc.Init();
		m_slotEquipAcc.SetEmpty(OnEmptyEquipAccSlotClick);
		m_slotEquipAcc.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_CHANGE);
		m_slotEquipAcc_2.Init();
		m_slotEquipAcc_2.SetEmpty(OnEmptyEquipAcc2SlotClick);
		m_slotEquipAcc_2.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_CHANGE);
		m_slotEquipUnitReactor?.Init();
		NKCUtil.SetBindFunction(m_NKM_UI_UNIT_INFO_DESC_EQUIP_AUTO, delegate
		{
			AddAllEquipItem();
		});
		NKCUtil.SetBindFunction(m_NKM_UI_UNIT_INFO_DESC_EQUIP_RESET, ClearAllEquipItem);
		m_NKCUIEquipPreset?.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnEquipPreset, OnClickEquipPreset);
		NKCUtil.SetBindFunction(m_btnUnEquip, OnClickUnEquip);
		NKCUtil.SetBindFunction(m_btnReinforce, OnClickReinforce);
		NKCUtil.SetBindFunction(m_btnChange, OnClickChange);
		NKCUtil.SetBindFunction(m_btnReactor, OnClickReactor);
		NKCUtil.SetGameobjectActive(m_btnReactor, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REACTOR));
		NKCUtil.SetGameobjectActive(m_slotEquipUnitReactor, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REACTOR));
	}

	public void Clear()
	{
		m_NKCUIEquipPreset?.CloseUI();
		SetEnableEquipInfo(bEnable: false);
	}

	public void SetData(NKMUnitData unitData, bool bFierceInfo = false)
	{
		if (unitData != null)
		{
			m_lbPowerSummary.text = unitData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData).ToString("N0");
		}
		m_UISkillPanel.SetData(unitData);
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		if (unitStatTemplet != null)
		{
			bool bPvP = false;
			NKMStatData nKMStatData = new NKMStatData();
			nKMStatData.Init();
			nKMStatData.MakeBaseStat(null, bPvP, unitData, unitStatTemplet.m_StatData);
			nKMStatData.MakeBaseBonusFactor(unitData, NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.EquipItems, null, null);
			m_slotHP.SetStat(NKM_STAT_TYPE.NST_HP, nKMStatData, unitData);
			m_slotAttack.SetStat(NKM_STAT_TYPE.NST_ATK, nKMStatData, unitData);
			m_slotDefense.SetStat(NKM_STAT_TYPE.NST_DEF, nKMStatData, unitData);
			m_slotHitRate.SetStat(NKM_STAT_TYPE.NST_HIT, nKMStatData, unitData);
			m_slotCritHitRate.SetStat(NKM_STAT_TYPE.NST_CRITICAL, nKMStatData, unitData);
			m_slotEvade.SetStat(NKM_STAT_TYPE.NST_EVADE, nKMStatData, unitData);
			m_UnitData = unitData;
			UpdateEquipSlots();
			UpdateReactorSlot();
			m_bShowFierceInfo = bFierceInfo;
			m_NKCUIEquipPreset?.ChangeUnitData(m_UnitData);
			SetEnableEquipInfo(bEnable: false);
		}
	}

	private void OpenUnitInfoDetailPopup()
	{
		SetEnableEquipInfo(bEnable: false);
		if (NKCPopupUnitInfoDetail.IsInstanceOpen)
		{
			NKCPopupUnitInfoDetail.CheckInstanceAndClose();
		}
		else
		{
			NKCPopupUnitInfoDetail.InstanceOpen(m_UnitData);
		}
	}

	private void UpdateReactorSlot()
	{
		if (NKCReactorUtil.IsReactorUnit(m_UnitData.GetUnitTempletBase()))
		{
			if (NKCReactorUtil.CheckCanTryLevelUp(m_UnitData))
			{
				if (m_UnitData.reactorLevel == 0)
				{
					m_slotEquipUnitReactor.SetEmpty(OnReactorSlotClick);
					return;
				}
				NKMUnitReactorTemplet reactorTemplet = NKCReactorUtil.GetReactorTemplet(m_UnitData.GetUnitTempletBase());
				if (reactorTemplet != null)
				{
					NKCUISlot.SlotData slotData = new NKCUISlot.SlotData();
					slotData.eType = NKCUISlot.eSlotMode.UnitReactor;
					slotData.ID = m_UnitData.m_UnitID;
					slotData.UID = m_UnitData.m_UnitUID;
					slotData.Count = m_UnitData.reactorLevel;
					m_slotEquipUnitReactor.SetData(slotData, bEnableLayoutElement: true, OnReactorSlotClick);
				}
			}
			else
			{
				m_slotEquipUnitReactor.SetLockReactorNotPossible(delegate
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_TACTIC_CAN_NOT_TRY_LEVEL_UP);
				});
			}
		}
		else
		{
			m_slotEquipUnitReactor.SetLockReactorNotHasReactor();
		}
	}

	private void OnReactorSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (m_UnitData != null)
		{
			if (m_UnitData.reactorLevel == 0)
			{
				OnClickReactor();
				return;
			}
			if (m_LatestSelectedEquipUID == m_UnitData.m_UnitUID && m_objEquipInfo.activeSelf)
			{
				SetEnableReactorInfo(bEnable: false);
				return;
			}
			SetOffAllSlot();
			m_LatestSelectedEquipUID = m_UnitData.m_UnitUID;
			SetEnableReactorInfo(bEnable: true);
			SetSlotSelected();
			m_slotEquip.SetData(NKCReactorUtil.GetReactorTemplet(m_UnitData.GetUnitTempletBase()), m_UnitData.reactorLevel);
		}
	}

	public void UpdateEquipSlots()
	{
		SetEnableEquipInfo(bEnable: false);
		if (m_UnitData != null)
		{
			if (NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID).m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: true);
				UpdateEquipItemSlot(m_UnitData.GetEquipItemWeaponUid(), ref m_slotEquipWeapon, OnEmptyEquipWeaponSlotClick, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON);
				UpdateEquipItemSlot(m_UnitData.GetEquipItemDefenceUid(), ref m_slotEquipDefense, OnEmptyEquipDefSlotClick, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE);
				UpdateEquipItemSlot(m_UnitData.GetEquipItemAccessoryUid(), ref m_slotEquipAcc, OnEmptyEquipAccSlotClick, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC);
				UpdateEquipItemSlot(m_UnitData.GetEquipItemAccessory2Uid(), ref m_slotEquipAcc_2, OnEmptyEquipAcc2SlotClick, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02);
			}
			if (!m_UnitData.IsUnlockAccessory2())
			{
				m_slotEquipAcc_2.SetLock(OnSetLockMessage);
				m_slotEquipAcc_2.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
			}
		}
		else
		{
			m_slotEquipAcc_2.SetLock(OnSetLockMessage);
			m_slotEquipAcc_2.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
	}

	private void OnSetLockMessage(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EQUIP_ACC_2_LOCKED_DESC);
	}

	private void UpdateEquipItemSlot(long equipItemUID, ref NKCUISlot slot, NKCUISlot.OnClick func, GameObject effObj, Animator effAni)
	{
		bool flag = false;
		bool flag2 = false;
		NKCUtil.SetGameobjectActive(effObj, bValue: true);
		if (equipItemUID > 0)
		{
			NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(equipItemUID);
			if (itemEquip != null)
			{
				slot.SetData(NKCUISlot.SlotData.MakeEquipData(itemEquip), bShowName: false, bShowNumber: true, bEnableLayoutElement: false, null);
				slot.SetOnClick(OpenEquipBoxForChange);
				flag = true;
				if (NKMItemManager.IsActiveSetOptionItem(itemEquip) && effAni != null)
				{
					NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(itemEquip.m_SetOptionId);
					if (equipSetOptionTemplet != null)
					{
						flag2 = true;
						effAni.SetTrigger(equipSetOptionTemplet.m_EquipSetIconEffect);
					}
				}
			}
		}
		if (!flag2)
		{
			NKCUtil.SetGameobjectActive(effObj, bValue: false);
		}
		if (!flag)
		{
			slot.SetCustomizedEmptySP(GetCustomizedEquipEmptySP());
			slot.SetEmpty(func);
		}
		slot.SetUsedMark(bVal: false);
	}

	private void OnEmptyEquipSlotClick(ITEM_EQUIP_POSITION equipPos)
	{
		SetEnableEquipInfo(bEnable: false);
		if (m_UnitData != null && !IsSeizedUnit(m_UnitData))
		{
			NKCUtil.ChangeEquip(m_UnitData.m_UnitUID, equipPos, null, 0L, m_bShowFierceInfo);
		}
	}

	private void OnEmptyEquipWeaponSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnEmptyEquipSlotClick(ITEM_EQUIP_POSITION.IEP_WEAPON);
	}

	private void OnEmptyEquipDefSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnEmptyEquipSlotClick(ITEM_EQUIP_POSITION.IEP_DEFENCE);
	}

	private void OnEmptyEquipAccSlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnEmptyEquipSlotClick(ITEM_EQUIP_POSITION.IEP_ACC);
	}

	private void OnEmptyEquipAcc2SlotClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnEmptyEquipSlotClick(ITEM_EQUIP_POSITION.IEP_ACC2);
	}

	private Sprite GetCustomizedEquipEmptySP()
	{
		if (m_UnitData == null)
		{
			return null;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID);
		if (unitTempletBase == null)
		{
			return null;
		}
		NKM_UNIT_STYLE_TYPE nKM_UNIT_STYLE_TYPE = unitTempletBase.m_NKM_UNIT_STYLE_TYPE;
		if ((uint)(nKM_UNIT_STYLE_TYPE - 1) <= 2u)
		{
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_UNIT_INFO_SPRITE", "NKM_UI_UNIT_INFO_ITEM_EQUIP_SLOT_ADD");
		}
		return null;
	}

	public void OpenEquipBoxForChange(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (m_UnitData == null)
		{
			return;
		}
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(slotData.UID);
		if (itemEquip != null && itemEquip.m_OwnerUnitUID > 0)
		{
			if (m_LatestSelectedEquipUID == itemEquip.m_ItemUid && m_objEquipInfo.activeSelf)
			{
				SetEnableEquipInfo(bEnable: false);
				return;
			}
			SetOffAllSlot();
			m_LatestSelectedEquipUID = itemEquip.m_ItemUid;
			SetEnableEquipInfo(bEnable: true);
			SetSlotSelected();
			m_slotEquip.SetData(itemEquip, m_bShowFierceInfo);
		}
	}

	private void SetOffAllSlot()
	{
		m_slotEquipWeapon.SetSelected(bSelected: false);
		m_slotEquipDefense.SetSelected(bSelected: false);
		m_slotEquipAcc.SetSelected(bSelected: false);
		m_slotEquipAcc_2.SetSelected(bSelected: false);
		m_slotEquipUnitReactor.SetSelected(bSelected: false);
	}

	private void SetSlotSelected(bool bSelect = true)
	{
		if (m_LatestSelectedEquipUID > 0 && m_UnitData != null)
		{
			if (m_UnitData.GetEquipItemWeaponUid() == m_LatestSelectedEquipUID)
			{
				m_slotEquipWeapon.SetSelected(bSelect);
			}
			if (m_UnitData.GetEquipItemDefenceUid() == m_LatestSelectedEquipUID)
			{
				m_slotEquipDefense.SetSelected(bSelect);
			}
			if (m_UnitData.GetEquipItemAccessoryUid() == m_LatestSelectedEquipUID)
			{
				m_slotEquipAcc.SetSelected(bSelect);
			}
			if (m_UnitData.GetEquipItemAccessory2Uid() == m_LatestSelectedEquipUID)
			{
				m_slotEquipAcc_2.SetSelected(bSelect);
			}
			if (m_UnitData.m_UnitUID == m_LatestSelectedEquipUID)
			{
				m_slotEquipUnitReactor.SetSelected(bSelect);
			}
		}
	}

	public void UnActiveEffect()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02, bValue: false);
	}

	private bool IsSeizedUnit(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return true;
		}
		if (unitData.IsSeized)
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED);
			return true;
		}
		return false;
	}

	public bool IsPresetOpend()
	{
		if (m_NKCUIEquipPreset != null)
		{
			return m_NKCUIEquipPreset.IsOpened();
		}
		return false;
	}

	private void OnClickEquipPreset()
	{
		SetEnableEquipInfo(bEnable: false);
		if (m_NKCUIEquipPreset == null)
		{
			return;
		}
		if (m_NKCUIEquipPreset.IsOpened())
		{
			m_NKCUIEquipPreset.CloseUI();
		}
		else if (NKCScenManager.GetScenManager().GetMyUserData() != null)
		{
			if (NKCEquipPresetDataManager.HasData())
			{
				m_NKCUIEquipPreset.Open(m_UnitData, null, m_bShowFierceInfo);
			}
			else
			{
				NKCEquipPresetDataManager.RequestPresetData(_openUI: true);
			}
		}
	}

	public void OpenEquipPreset(List<NKMEquipPresetData> presetDatas)
	{
		SetEnableEquipInfo(bEnable: false);
		m_NKCUIEquipPreset?.Open(m_UnitData, presetDatas, m_bShowFierceInfo);
	}

	public bool EquipPresetOpened()
	{
		if (m_NKCUIEquipPreset != null && m_NKCUIEquipPreset.IsOpened())
		{
			return true;
		}
		return false;
	}

	public void SetEnableEquipInfo(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_objEquipInfo, bEnable);
		if (bEnable)
		{
			NKCUtil.SetGameobjectActive(m_btnUnEquip, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnReinforce, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnChange, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnReactor, bValue: false);
		}
		SetOffAllSlot();
	}

	private void OnClickUnEquip()
	{
		NKMItemManager.UnEquip(m_LatestSelectedEquipUID);
	}

	public void OnClickUnEquip(NKCUISlotEquip slot)
	{
		NKMItemManager.UnEquip(slot.GetEquipItemUID());
	}

	public void SetEnableReactorInfo(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_objEquipInfo, bEnable);
		NKCUtil.SetGameobjectActive(m_btnUnEquip, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnReinforce, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnChange, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnReactor, bValue: true);
		SetOffAllSlot();
	}

	public void OnClickReinforce()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
		}
		else if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_ENCHANT);
		}
		else if (m_LatestSelectedEquipUID > 0)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMItemManager.CanEnchantItem(NKCScenManager.GetScenManager().GetMyUserData(), m_LatestSelectedEquipUID);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
				return;
			}
			SetEnableEquipInfo(bEnable: false);
			NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_ENCHANT, m_LatestSelectedEquipUID);
		}
	}

	public void OnClickChange()
	{
		NKCUtil.ChangeEquip(m_UnitData.m_UnitUID, NKMItemManager.GetItemEquipPosition(m_LatestSelectedEquipUID), delegate
		{
			OnClickUnEquip();
		}, m_LatestSelectedEquipUID, m_bShowFierceInfo);
	}

	private void OnClickReactor()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REACTOR) && NKCReactorUtil.IsReactorUnit(m_UnitData.GetUnitTempletBase()))
		{
			NKCUIUnitReactor.Instance.Open(m_UnitData);
		}
	}

	public void AddAllEquipBySetOption(int setOptionID)
	{
		AddAllEquipItem(setOptionID);
	}

	private void AddAllEquipItem(int setOptionID = 0)
	{
		if (m_UnitData == null)
		{
			return;
		}
		if (m_UnitData.GetEquipItemWeaponUid() > 0 && m_UnitData.GetEquipItemDefenceUid() > 0 && m_UnitData.GetEquipItemAccessoryUid() > 0 && m_UnitData.GetEquipItemAccessory2Uid() > 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ALREADY_FULL_EQUIPMENT);
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitData.m_UnitID);
		if (unitTempletBase != null && IsPossibleUnitType(unitTempletBase.m_NKM_UNIT_STYLE_TYPE))
		{
			m_lstAlreadySelectedEquipItem.Clear();
			m_lstAlreadySelectedEquipItem.AddRange(m_UnitData.GetValidEquipUids());
			if (m_UnitData.GetEquipItemWeaponUid() <= 0)
			{
				UpdateEquipItem(unitTempletBase, ITEM_EQUIP_POSITION.IEP_WEAPON, setOptionID);
			}
			if (m_UnitData.GetEquipItemDefenceUid() <= 0)
			{
				UpdateEquipItem(unitTempletBase, ITEM_EQUIP_POSITION.IEP_DEFENCE, setOptionID);
			}
			if (m_UnitData.GetEquipItemAccessoryUid() <= 0)
			{
				UpdateEquipItem(unitTempletBase, ITEM_EQUIP_POSITION.IEP_ACC, setOptionID);
			}
			if (m_UnitData.GetEquipItemAccessory2Uid() <= 0 && m_UnitData.IsUnlockAccessory2())
			{
				UpdateEquipItem(unitTempletBase, ITEM_EQUIP_POSITION.IEP_ACC2, setOptionID);
			}
		}
	}

	private bool IsPossibleUnitType(NKM_UNIT_STYLE_TYPE targetType)
	{
		if ((uint)(targetType - 1) <= 2u)
		{
			return true;
		}
		Debug.LogError("현재 허용되지 않는 타입입니다. 추가 직업이 있을 경우, 추가 해주세요. : " + targetType);
		return false;
	}

	private void UpdateEquipItem(NKMUnitTempletBase unitTempletBase, ITEM_EQUIP_POSITION targetEquipPosition, int setOptionID)
	{
		if (unitTempletBase == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || myUserData.m_InventoryData == null)
		{
			return;
		}
		NKCEquipSortSystem.eFilterOption eFilterOption = NKCEquipSortSystem.eFilterOption.Nothing;
		NKCEquipSortSystem.eFilterOption eFilterOption2 = NKCEquipSortSystem.eFilterOption.Nothing;
		switch (unitTempletBase.m_NKM_UNIT_STYLE_TYPE)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_COUNTER:
			eFilterOption = NKCEquipSortSystem.eFilterOption.Equip_Counter;
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SOLDIER:
			eFilterOption = NKCEquipSortSystem.eFilterOption.Equip_Soldier;
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_MECHANIC:
			eFilterOption = NKCEquipSortSystem.eFilterOption.Equip_Mechanic;
			break;
		default:
			Debug.LogError($"지정되지 않은 타입입니다.{unitTempletBase.m_NKM_UNIT_STYLE_TYPE}");
			return;
		}
		switch (targetEquipPosition)
		{
		case ITEM_EQUIP_POSITION.IEP_WEAPON:
			eFilterOption2 = NKCEquipSortSystem.eFilterOption.Equip_Weapon;
			break;
		case ITEM_EQUIP_POSITION.IEP_DEFENCE:
			eFilterOption2 = NKCEquipSortSystem.eFilterOption.Equip_Armor;
			break;
		case ITEM_EQUIP_POSITION.IEP_ACC:
		case ITEM_EQUIP_POSITION.IEP_ACC2:
			eFilterOption2 = NKCEquipSortSystem.eFilterOption.Equip_Acc;
			break;
		default:
			Debug.LogError($"지정되지 않은 장착 타입입니다.{targetEquipPosition}");
			return;
		}
		NKCEquipSortSystem.EquipListOptions options = default(NKCEquipSortSystem.EquipListOptions);
		options.setOnlyIncludeEquipID = null;
		options.setExcludeEquipID = null;
		options.setExcludeEquipUID = null;
		options.setExcludeFilterOption = null;
		options.setFilterOption = new HashSet<NKCEquipSortSystem.eFilterOption>();
		options.setFilterOption.Add(NKCEquipSortSystem.eFilterOption.Equip_Unused);
		options.OwnerUnitID = unitTempletBase.m_UnitID;
		options.iTargetUnitID = unitTempletBase.m_UnitID;
		options.setFilterOption.Add(eFilterOption);
		if (eFilterOption2 != NKCEquipSortSystem.eFilterOption.Nothing)
		{
			options.setFilterOption.Add(eFilterOption2);
		}
		options.lstSortOption = NKCEquipSortSystem.GetDefaultSortOption(NKCPopupEquipSort.SORT_OPEN_TYPE.OPTION_WEIGHT);
		options.PreemptiveSortFunc = null;
		options.AdditionalExcludeFilterFunc = null;
		options.bHideEquippedItem = false;
		options.bPushBackUnselectable = true;
		options.bHideLockItem = false;
		options.bHideMaxLvItem = false;
		options.bLockMaxItem = false;
		options.bHideNotPossibleSetOptionItem = false;
		if (setOptionID > 0)
		{
			options.setFilterOption.Add(NKCEquipSortSystem.eFilterOption.Equip_Stat_SetOption);
			options.FilterStatType_01 = NKM_STAT_TYPE.NST_RANDOM;
			options.FilterStatType_02 = NKM_STAT_TYPE.NST_RANDOM;
			options.FilterStatType_Potential = NKM_STAT_TYPE.NST_RANDOM;
			options.FilterSetOptionID = setOptionID;
		}
		NKCEquipSortSystem nKCEquipSortSystem = new NKCEquipSortSystem(myUserData, options);
		if (nKCEquipSortSystem.SortedEquipList.Count <= 0)
		{
			return;
		}
		List<long> list = new List<long>();
		foreach (NKMEquipItemData sortedEquip in nKCEquipSortSystem.SortedEquipList)
		{
			if (NKMItemManager.GetEquipTemplet(sortedEquip.m_ItemEquipID).IsPrivateEquip())
			{
				list.Clear();
				list.AddRange(m_lstAlreadySelectedEquipItem);
				list.Add(sortedEquip.m_ItemUid);
				if (NKCUtil.IsPrivateEquipAlreadyEquipped(list))
				{
					continue;
				}
			}
			if (!m_lstAlreadySelectedEquipItem.Contains(sortedEquip.m_ItemUid))
			{
				SendEquipItem(bEquip: true, sortedEquip.m_ItemUid, targetEquipPosition);
				m_lstAlreadySelectedEquipItem.Add(sortedEquip.m_ItemUid);
				break;
			}
		}
	}

	private void ClearAllEquipItem()
	{
		if (m_UnitData != null && m_objEquipSlotParent.activeSelf)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMUnitManager.IsUnitBusy(NKCScenManager.GetScenManager().GetMyUserData(), m_UnitData, ignoreWorldmapState: true);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
				return;
			}
			SendEquipItem(bEquip: false, m_UnitData.GetEquipItemWeaponUid(), ITEM_EQUIP_POSITION.IEP_WEAPON);
			SendEquipItem(bEquip: false, m_UnitData.GetEquipItemDefenceUid(), ITEM_EQUIP_POSITION.IEP_DEFENCE);
			SendEquipItem(bEquip: false, m_UnitData.GetEquipItemAccessoryUid(), ITEM_EQUIP_POSITION.IEP_ACC);
			SendEquipItem(bEquip: false, m_UnitData.GetEquipItemAccessory2Uid(), ITEM_EQUIP_POSITION.IEP_ACC2);
		}
	}

	private void SendEquipItem(bool bEquip, long targetEquipUId, ITEM_EQUIP_POSITION equipPosition)
	{
		if (m_UnitData != null && targetEquipUId > 0)
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_EQUIP_REQ(bEquip, m_UnitData.m_UnitUID, targetEquipUId, equipPosition);
		}
	}
}
