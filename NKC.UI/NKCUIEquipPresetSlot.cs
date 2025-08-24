using System;
using System.Collections.Generic;
using ClientPacket.Item;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEquipPresetSlot : MonoBehaviour
{
	[Serializable]
	public struct SlotEquipFx
	{
		public GameObject m_objEquipEffect;

		public DOTweenAnimation m_tweenEquipFx1;

		public DOTweenAnimation m_tweenEquipFx2;

		public void SetSlotEquipEffectState(bool activate)
		{
			NKCUtil.SetGameobjectActive(m_objEquipEffect, activate);
			if (activate)
			{
				m_tweenEquipFx1?.DORestart();
				m_tweenEquipFx2?.DORestart();
			}
		}
	}

	public delegate void OnClickAdd();

	public delegate void OnToggleCheck(NKMEquipPresetData presetData, bool bValue);

	public delegate void OnClickUp(NKMEquipPresetData presetData);

	public delegate void OnClickDown(NKMEquipPresetData presetData);

	public NKCUIComStateButton m_csbtnChangePresetName;

	public NKCUIComStateButton m_csbtnSaveUnitEquipSet;

	public NKCUIComStateButton m_csbtnApplyPreset;

	public NKCUIComStateButton m_csbtnApplyDisabled;

	public NKCUIComStateButton m_csbtnPresetAdd;

	public InputField m_inputFieldPresetName;

	public NKCUIComDragScrollInputField m_comDragScrollInputField;

	public GameObject m_objBasicSlot;

	public GameObject m_objAddSlot;

	[Header("장비 슬롯")]
	public NKCUISlotEquipPreset m_slotEquipWeapon;

	public NKCUISlotEquipPreset m_slotEquipDefense;

	public NKCUISlotEquipPreset m_slotEquipAcc;

	public NKCUISlotEquipPreset m_slotEquipAcc_2;

	[Header("프리셋 이동 화살표 버튼")]
	public NKCUIComStateButton m_csbtnUp;

	public NKCUIComStateButton m_csbtnDown;

	[Header("프리셋 제거 체크토글")]
	public NKCUIComToggle m_tglCheck;

	[Header("세트아이템 이펙트")]
	public GameObject m_objEquipSetFXWeapon;

	public Animator m_aniEquipSetFXWeapon;

	public GameObject m_objEquipSetFXDefence;

	public Animator m_aniEquipSetFXDefence;

	public GameObject m_objEquipSetFXAcc;

	public Animator m_aniEquipSetFXAcc;

	public GameObject m_objEquipSetFXAcc2;

	public Animator m_aniEquipSetFXAcc2;

	[Header("아이템 장착 이펙트")]
	public SlotEquipFx m_weaponSlotFx;

	public SlotEquipFx m_defenceSlotFx;

	public SlotEquipFx m_accSlotFx;

	public SlotEquipFx m_acc2SlotFx;

	[Header("프리셋 확장 이펙트")]
	public GameObject m_objPresetSlotFx;

	[Header("세트 아이템 상시 이펙트")]
	public Image m_setFxLoopWeapon;

	public Image m_setFxLoopDefence;

	public Image m_setFxLoopAcc;

	public Image m_setFxLoopAcc2;

	public Color m_setFxRed;

	public Color m_setFxBlue;

	public Color m_setFxYellow;

	private NKCAssetInstanceData m_InstanceData;

	private NKCUIEquipPreset m_cNKCUIEquipPreset;

	private int m_iPresetIndex;

	private NKM_EQUIP_PRESET_TYPE m_ePresetType;

	private List<long> m_listEquipUId = new List<long>();

	private string m_strPresetName;

	private static int m_iUpdatingIndex = -1;

	private static bool m_bSavedPreset = false;

	private static bool m_bShowSetItemFx = false;

	private bool m_bShowFierceInfo;

	private bool m_bNameChangeButtonClicked;

	private List<ITEM_EQUIP_POSITION> m_listUpdatedEquipPosition = new List<ITEM_EQUIP_POSITION>();

	private NKMEquipPresetData m_presetData;

	private OnClickAdd m_dOnClickAdd;

	private OnToggleCheck m_dOnToggleCheck;

	private OnClickUp m_dOnClickUp;

	private OnClickDown m_dOnClickDown;

	public static int UpdatingIndex
	{
		set
		{
			m_iUpdatingIndex = value;
		}
	}

	public static bool SavedPreset
	{
		set
		{
			m_bSavedPreset = value;
		}
	}

	public static bool ShowSetItemFx
	{
		set
		{
			m_bShowSetItemFx = value;
		}
	}

	public void Init()
	{
		m_slotEquipWeapon.Init();
		m_slotEquipDefense.Init();
		m_slotEquipAcc.Init();
		m_slotEquipAcc_2.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnPresetAdd, OnClickPresetAdd);
		NKCUtil.SetButtonClickDelegate(m_csbtnChangePresetName, OnClickNameChange);
		NKCUtil.SetButtonClickDelegate(m_csbtnSaveUnitEquipSet, OnClickSaveUnitEquip);
		NKCUtil.SetButtonClickDelegate(m_csbtnApplyPreset, OnClickApplyPreset);
		NKCUtil.SetButtonClickDelegate(m_csbtnApplyDisabled, OnClickApplyPreset);
		NKCUtil.SetButtonClickDelegate(m_csbtnUp, OnClickSlotUp);
		NKCUtil.SetButtonClickDelegate(m_csbtnDown, OnClickSlotDown);
		NKCUtil.SetToggleValueChangedDelegate(m_tglCheck, OnToggleSlotCheck);
		int num = 4;
		for (int i = 0; i < num; i++)
		{
			m_listEquipUId.Add(0L);
		}
		m_inputFieldPresetName.onValueChanged.RemoveAllListeners();
		m_inputFieldPresetName.onValueChanged.AddListener(OnInputNameChanged);
		m_inputFieldPresetName.onValidateInput = NKCFilterManager.FilterEmojiInput;
		m_inputFieldPresetName.onEndEdit.RemoveAllListeners();
		m_inputFieldPresetName.onEndEdit.AddListener(OnInputPresetName);
	}

	public static NKCUIEquipPresetSlot GetNewInstance(Transform parent, bool bMentoringSlot = false)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_equip_preset", "NKM_UI_POPUP_PRESET_LIST_SLOT");
		NKCUIEquipPresetSlot nKCUIEquipPresetSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIEquipPresetSlot>();
		if (nKCUIEquipPresetSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIEquipPresetSlot Prefab null!");
			return null;
		}
		nKCUIEquipPresetSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUIEquipPresetSlot.Init();
		if (parent != null)
		{
			nKCUIEquipPresetSlot.transform.SetParent(parent);
		}
		nKCUIEquipPresetSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUIEquipPresetSlot.gameObject.SetActive(value: false);
		return nKCUIEquipPresetSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SetData(int index, NKMEquipPresetData presetData, NKCUIEquipPreset cNKCUIEquipPreset, int newPresetIndexFrom, bool showFierceInfo, bool slotMoveState, bool bSlotRemoveState, LoopScrollRect parentScrollRect, OnClickAdd onClickAdd, OnClickUp onClickUp = null, OnClickDown onClickDown = null, OnToggleCheck onToggleCheck = null)
	{
		m_presetData = presetData;
		m_bShowFierceInfo = showFierceInfo;
		m_cNKCUIEquipPreset = cNKCUIEquipPreset;
		m_comDragScrollInputField.ScrollRect = parentScrollRect;
		m_dOnClickAdd = onClickAdd;
		m_dOnClickUp = onClickUp;
		m_dOnClickDown = onClickDown;
		m_dOnToggleCheck = onToggleCheck;
		m_weaponSlotFx.SetSlotEquipEffectState(activate: false);
		m_defenceSlotFx.SetSlotEquipEffectState(activate: false);
		m_accSlotFx.SetSlotEquipEffectState(activate: false);
		m_acc2SlotFx.SetSlotEquipEffectState(activate: false);
		NKCUtil.SetGameobjectActive(m_objPresetSlotFx, bValue: false);
		if (presetData != null)
		{
			m_iPresetIndex = presetData.presetIndex;
			m_ePresetType = presetData.presetType;
			m_strPresetName = presetData.presetName;
			((Text)m_inputFieldPresetName.placeholder).text = GetDefaultSlotName();
			m_inputFieldPresetName.text = presetData.presetName;
			if (m_iUpdatingIndex != m_iPresetIndex)
			{
				m_listUpdatedEquipPosition.Clear();
			}
			int count = presetData.equipUids.Count;
			for (int i = 0; i < count; i++)
			{
				if (presetData.equipUids[i] > 0 && m_listEquipUId.Count > i && m_listUpdatedEquipPosition.Contains((ITEM_EQUIP_POSITION)i) && !m_bSavedPreset && presetData.equipUids[i] == m_listEquipUId[i])
				{
					m_listUpdatedEquipPosition.Remove((ITEM_EQUIP_POSITION)i);
				}
			}
			ActivateSlotEffect(activate: true);
			m_listEquipUId.Clear();
			m_listEquipUId.AddRange(presetData.equipUids);
			int num = 4;
			for (int j = m_listEquipUId.Count; j < num; j++)
			{
				m_listEquipUId.Add(0L);
			}
			UpdateEquipItemSlot(m_listEquipUId[0], m_slotEquipWeapon, OnEmptyEquipWeaponSlotClick, m_objEquipSetFXWeapon, m_aniEquipSetFXWeapon, m_setFxLoopWeapon, ITEM_EQUIP_POSITION.IEP_WEAPON);
			UpdateEquipItemSlot(m_listEquipUId[1], m_slotEquipDefense, OnEmptyEquipDefSlotClick, m_objEquipSetFXDefence, m_aniEquipSetFXDefence, m_setFxLoopDefence, ITEM_EQUIP_POSITION.IEP_DEFENCE);
			UpdateEquipItemSlot(m_listEquipUId[2], m_slotEquipAcc, OnEmptyEquipAccSlotClick, m_objEquipSetFXAcc, m_aniEquipSetFXAcc, m_setFxLoopAcc, ITEM_EQUIP_POSITION.IEP_ACC);
			UpdateEquipItemSlot(m_listEquipUId[3], m_slotEquipAcc_2, OnEmptyEquipAcc2SlotClick, m_objEquipSetFXAcc2, m_aniEquipSetFXAcc2, m_setFxLoopAcc2, ITEM_EQUIP_POSITION.IEP_ACC2);
			if (m_iPresetIndex >= newPresetIndexFrom)
			{
				NKCUtil.SetGameobjectActive(m_objPresetSlotFx, bValue: true);
			}
			NKCUtil.SetGameobjectActive(m_objBasicSlot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objAddSlot, bValue: false);
			if (slotMoveState)
			{
				NKCUtil.SetGameobjectActive(m_csbtnApplyPreset, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnApplyDisabled, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnSaveUnitEquipSet, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnUp, bValue: true);
				NKCUtil.SetGameobjectActive(m_csbtnDown, bValue: true);
				NKCUtil.SetGameobjectActive(m_tglCheck, bValue: false);
				if (cNKCUIEquipPreset != null)
				{
					int tempPresetDataIndex = NKCEquipPresetDataManager.GetTempPresetDataIndex(m_presetData);
					m_csbtnUp?.SetLock(tempPresetDataIndex == 0);
					m_csbtnDown?.SetLock(tempPresetDataIndex == NKCEquipPresetDataManager.ListEquipPresetData.Count - 1);
					if (!cNKCUIEquipPreset.m_enableInterPageSlotMove)
					{
						int num2 = index % cNKCUIEquipPreset.m_maxSlotCountPerPage;
						m_csbtnUp?.SetLock(num2 == 0);
						if (num2 == cNKCUIEquipPreset.m_maxSlotCountPerPage - 1)
						{
							m_csbtnDown?.SetLock(value: true);
						}
					}
				}
				else
				{
					m_csbtnUp?.SetLock(value: false);
					m_csbtnDown?.SetLock(value: false);
				}
			}
			else if (bSlotRemoveState)
			{
				NKCUtil.SetGameobjectActive(m_tglCheck, bValue: true);
				m_tglCheck.SetLock(!CanResetPresetSlot(presetData));
				m_tglCheck.Select(NKCEquipPresetDataManager.GetRemoveTargetList().Contains(presetData.presetIndex), bForce: true);
				NKCUtil.SetGameobjectActive(m_csbtnApplyPreset, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnApplyDisabled, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnSaveUnitEquipSet, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnUp, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnDown, bValue: false);
			}
			else
			{
				bool flag = IsAppliableSet(showMsg: false);
				NKCUtil.SetGameobjectActive(m_csbtnApplyPreset, flag);
				NKCUtil.SetGameobjectActive(m_csbtnApplyDisabled, !flag);
				NKCUtil.SetGameobjectActive(m_csbtnSaveUnitEquipSet, bValue: true);
				NKCUtil.SetGameobjectActive(m_csbtnUp, bValue: false);
				NKCUtil.SetGameobjectActive(m_csbtnDown, bValue: false);
				NKCUtil.SetGameobjectActive(m_tglCheck, bValue: false);
			}
			if (m_iUpdatingIndex == m_iPresetIndex)
			{
				ResetUpdateIndex();
			}
		}
		else
		{
			m_iPresetIndex = index;
			m_ePresetType = NKM_EQUIP_PRESET_TYPE.NEPT_INVLID;
			m_inputFieldPresetName.text = "";
			m_listEquipUId.Clear();
			NKCUtil.SetGameobjectActive(m_objBasicSlot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objAddSlot, bValue: true);
		}
	}

	private bool CanResetPresetSlot(NKMEquipPresetData presetData)
	{
		if (presetData == null)
		{
			return false;
		}
		for (int i = 0; i < presetData.equipUids.Count; i++)
		{
			if (presetData.equipUids[i] > 0)
			{
				return true;
			}
		}
		if (!string.IsNullOrEmpty(presetData.presetName))
		{
			return true;
		}
		if (presetData.presetType != NKM_EQUIP_PRESET_TYPE.NEPT_NONE)
		{
			return true;
		}
		return false;
	}

	public static void ResetUpdateIndex()
	{
		m_iUpdatingIndex = -1;
		m_bSavedPreset = false;
		m_bShowSetItemFx = false;
	}

	private void ActivateSlotEffect(bool activate)
	{
		if (activate)
		{
			for (int i = 0; i < m_listUpdatedEquipPosition.Count; i++)
			{
				switch (m_listUpdatedEquipPosition[i])
				{
				case ITEM_EQUIP_POSITION.IEP_WEAPON:
					m_weaponSlotFx.SetSlotEquipEffectState(activate: true);
					break;
				case ITEM_EQUIP_POSITION.IEP_DEFENCE:
					m_defenceSlotFx.SetSlotEquipEffectState(activate: true);
					break;
				case ITEM_EQUIP_POSITION.IEP_ACC:
					m_accSlotFx.SetSlotEquipEffectState(activate: true);
					break;
				case ITEM_EQUIP_POSITION.IEP_ACC2:
					m_acc2SlotFx.SetSlotEquipEffectState(activate: true);
					break;
				}
			}
		}
		m_listUpdatedEquipPosition.Clear();
	}

	private void DeactivateAllSetItemEffect()
	{
		NKCUtil.SetGameobjectActive(m_objEquipSetFXWeapon, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEquipSetFXDefence, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEquipSetFXAcc, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEquipSetFXAcc2, bValue: false);
	}

	private void UpdateEquipItemSlot(long equipItemUID, NKCUISlotEquipPreset slot, NKCUISlot.OnClick func, GameObject effObj, Animator effAni, Image setLoopFx, ITEM_EQUIP_POSITION equipPosition)
	{
		bool flag = false;
		bool flag2 = false;
		bool disable = false;
		NKCUtil.SetGameobjectActive(effObj, bValue: true);
		NKCUtil.SetGameobjectActive(setLoopFx.gameObject, bValue: false);
		if (equipItemUID > 0)
		{
			NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(equipItemUID);
			if (itemEquip != null)
			{
				slot.Slot.SetData(NKCUISlot.SlotData.MakeEquipData(itemEquip), bShowName: false, bShowNumber: true, bEnableLayoutElement: false, null);
				slot.Slot.SetOnClick(delegate
				{
					NKMUnitData unitData = m_cNKCUIEquipPreset.UnitData;
					if (unitData != null)
					{
						DeactivateAllSetItemEffect();
						m_iUpdatingIndex = m_iPresetIndex;
						m_listUpdatedEquipPosition.Clear();
						m_listUpdatedEquipPosition.Add(equipPosition);
						NKCPopupItemEquipBox.OpenForPresetChange(unitData.m_UnitUID, equipItemUID, equipPosition, m_iPresetIndex, m_listEquipUId, m_bShowFierceInfo);
					}
				});
				flag = true;
				if (IsSetOptionActivated(itemEquip, equipPosition) && effAni != null)
				{
					NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(itemEquip.m_SetOptionId);
					if (equipSetOptionTemplet != null)
					{
						flag2 = true;
						effAni.SetTrigger(equipSetOptionTemplet.m_EquipSetIconEffect);
					}
				}
				NKMUnitTempletBase cNKMUnitTempletBase = null;
				if (itemEquip.m_OwnerUnitUID > 0)
				{
					NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
					if (unitFromUID != null)
					{
						cNKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
					}
				}
				slot.SetEquipUnitSprite(NKCResourceUtility.GetOrLoadMinimapFaceIcon(cNKMUnitTempletBase));
			}
		}
		if (!flag2 || !m_bShowSetItemFx || m_iUpdatingIndex != m_iPresetIndex)
		{
			NKCUtil.SetGameobjectActive(effObj, bValue: false);
		}
		if (!flag)
		{
			if (m_listEquipUId[(int)equipPosition] > 0)
			{
				Debug.LogWarning($"{equipPosition} EquipUid Data of Preset Index {m_iPresetIndex} not exist");
				m_listEquipUId[(int)equipPosition] = 0L;
				m_ePresetType = GetEquipPresetType();
			}
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_UNIT_INFO_SPRITE", "NKM_UI_UNIT_INFO_ITEM_EQUIP_SLOT_ADD");
			slot.Slot.SetCustomizedEmptySP(orLoadAssetResource);
			slot.SetEmpty(func);
		}
		slot.Slot.SetDisable(disable, NKCStringTable.GetString("SI_PF_FIERCE"));
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

	private void OnEmptyEquipSlotClick(ITEM_EQUIP_POSITION equipPos)
	{
		NKMUnitData unitData = m_cNKCUIEquipPreset.UnitData;
		if (unitData != null && !IsSeizedUnit(unitData))
		{
			DeactivateAllSetItemEffect();
			NKM_UNIT_STYLE_TYPE unitStyleType = GetUnitStyleType(m_ePresetType);
			m_iUpdatingIndex = m_iPresetIndex;
			m_listUpdatedEquipPosition.Clear();
			m_listUpdatedEquipPosition.Add(equipPos);
			NKCUtil.ChangePresetEquip(unitData.m_UnitUID, m_iPresetIndex, m_listEquipUId[(int)equipPos], m_listEquipUId, equipPos, unitStyleType, m_bShowFierceInfo);
		}
	}

	private bool IsSetOptionActivated(NKMEquipItemData cNKMEquipItemData, ITEM_EQUIP_POSITION equipPosition)
	{
		if (cNKMEquipItemData == null)
		{
			return false;
		}
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(cNKMEquipItemData.m_SetOptionId);
		if (equipSetOptionTemplet == null)
		{
			return false;
		}
		if (equipSetOptionTemplet.m_EquipSetPart == 1)
		{
			return true;
		}
		List<int> list = new List<int>();
		int count = m_listEquipUId.Count;
		for (int i = 0; i < count; i++)
		{
			NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_listEquipUId[i]);
			if (itemEquip != null && itemEquip.m_SetOptionId == cNKMEquipItemData.m_SetOptionId)
			{
				list.Add(i);
			}
		}
		if (equipSetOptionTemplet.m_EquipSetPart <= list.Count)
		{
			int num = 0;
			int count2 = list.Count;
			for (int j = 0; j < count2; j++)
			{
				if (++num == equipSetOptionTemplet.m_EquipSetPart)
				{
					if (list[j] >= (int)equipPosition)
					{
						return true;
					}
					num = 0;
				}
			}
		}
		return false;
	}

	private NKM_UNIT_STYLE_TYPE GetUnitStyleType(NKM_EQUIP_PRESET_TYPE presetType)
	{
		return presetType switch
		{
			NKM_EQUIP_PRESET_TYPE.NEPT_COUNTER => NKM_UNIT_STYLE_TYPE.NUST_COUNTER, 
			NKM_EQUIP_PRESET_TYPE.NEPT_SOLDIER => NKM_UNIT_STYLE_TYPE.NUST_SOLDIER, 
			NKM_EQUIP_PRESET_TYPE.NEPT_MECHANIC => NKM_UNIT_STYLE_TYPE.NUST_MECHANIC, 
			_ => NKM_UNIT_STYLE_TYPE.NUST_INVALID, 
		};
	}

	private string GetDefaultSlotName()
	{
		return string.Format(NKCUtilString.GET_STRING_EQUIP_PRESET_NAME, m_iPresetIndex + 1);
	}

	private NKM_EQUIP_PRESET_TYPE GetEquipPresetType()
	{
		NKM_EQUIP_PRESET_TYPE nKM_EQUIP_PRESET_TYPE = NKM_EQUIP_PRESET_TYPE.NEPT_NONE;
		int count = m_listEquipUId.Count;
		for (int i = 0; i < count; i++)
		{
			NKMEquipTemplet equipTempletFromEquipUId = GetEquipTempletFromEquipUId(m_listEquipUId[i]);
			if (equipTempletFromEquipUId != null)
			{
				switch (equipTempletFromEquipUId.m_EquipUnitStyleType)
				{
				case NKM_UNIT_STYLE_TYPE.NUST_COUNTER:
					nKM_EQUIP_PRESET_TYPE = NKM_EQUIP_PRESET_TYPE.NEPT_COUNTER;
					break;
				case NKM_UNIT_STYLE_TYPE.NUST_SOLDIER:
					nKM_EQUIP_PRESET_TYPE = NKM_EQUIP_PRESET_TYPE.NEPT_SOLDIER;
					break;
				case NKM_UNIT_STYLE_TYPE.NUST_MECHANIC:
					nKM_EQUIP_PRESET_TYPE = NKM_EQUIP_PRESET_TYPE.NEPT_MECHANIC;
					break;
				}
				if (nKM_EQUIP_PRESET_TYPE != NKM_EQUIP_PRESET_TYPE.NEPT_NONE)
				{
					break;
				}
			}
		}
		return nKM_EQUIP_PRESET_TYPE;
	}

	private NKMEquipTemplet GetEquipTempletFromEquipUId(long equipUId)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(equipUId);
		if (itemEquip == null)
		{
			return null;
		}
		return NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
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

	private bool IsEmptyAllEquipSlot()
	{
		bool result = true;
		int count = m_listEquipUId.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_listEquipUId[i] > 0)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private bool IsAppliableSet(bool showMsg)
	{
		NKM_UNIT_STYLE_TYPE unitStyleType = GetUnitStyleType(m_ePresetType);
		if (unitStyleType == NKM_UNIT_STYLE_TYPE.NUST_INVALID || IsEmptyAllEquipSlot())
		{
			if (showMsg)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EQUIP_PRESET_NONE, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
			return false;
		}
		NKMUnitData unitData = m_cNKCUIEquipPreset.UnitData;
		if (unitData == null)
		{
			return false;
		}
		if (IsSeizedUnit(unitData))
		{
			return false;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase == null)
		{
			return false;
		}
		if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE != unitStyleType)
		{
			if (showMsg)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EQUIP_PRESET_DIFFERENT_TYPE, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
			return false;
		}
		return true;
	}

	private void OnClickPresetAdd()
	{
		if (m_dOnClickAdd != null)
		{
			m_dOnClickAdd();
		}
	}

	private void OnInputNameChanged(string _string)
	{
		m_inputFieldPresetName.text = NKCFilterManager.CheckBadChat(m_inputFieldPresetName.text);
		if (m_inputFieldPresetName.text.Length >= NKMCommonConst.EQUIP_PRESET_NAME_MAX_LENGTH)
		{
			m_inputFieldPresetName.text = m_inputFieldPresetName.text.Substring(0, NKMCommonConst.EQUIP_PRESET_NAME_MAX_LENGTH);
		}
	}

	private void OnInputPresetName(string _string)
	{
		m_comDragScrollInputField.ActiveInput = false;
		m_inputFieldPresetName.enabled = false;
		m_inputFieldPresetName.text = NKCFilterManager.CheckBadChat(m_inputFieldPresetName.text);
		if (m_inputFieldPresetName.text == m_strPresetName)
		{
			if (NKCUIManager.FrontCanvas != null)
			{
				m_bNameChangeButtonClicked = RectTransformUtility.RectangleContainsScreenPoint(m_csbtnChangePresetName.GetComponent<RectTransform>(), Input.mousePosition, NKCUIManager.FrontCanvas.worldCamera);
			}
		}
		else if (m_inputFieldPresetName.text.Length >= NKMCommonConst.EQUIP_PRESET_NAME_MAX_LENGTH)
		{
			string presetName = m_inputFieldPresetName.text.Substring(0, NKMCommonConst.EQUIP_PRESET_NAME_MAX_LENGTH);
			NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_NAME_CHANGE_REQ(m_iPresetIndex, presetName);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_NAME_CHANGE_REQ(m_iPresetIndex, m_inputFieldPresetName.text);
		}
	}

	private void OnClickNameChange()
	{
		if (m_bNameChangeButtonClicked)
		{
			m_bNameChangeButtonClicked = false;
			return;
		}
		m_comDragScrollInputField.ActiveInput = true;
		m_inputFieldPresetName.enabled = true;
		m_inputFieldPresetName.Select();
		m_inputFieldPresetName.ActivateInputField();
	}

	private void OnClickSaveUnitEquip()
	{
		NKMUnitData unitData = m_cNKCUIEquipPreset.UnitData;
		if (unitData == null || IsSeizedUnit(unitData))
		{
			return;
		}
		if (unitData.GetEquipItemWeaponUid() <= 0 && unitData.GetEquipItemDefenceUid() <= 0 && unitData.GetEquipItemAccessoryUid() <= 0 && unitData.GetEquipItemAccessory2Uid() <= 0)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EQUIP_PRESET_UNIT_EQUIP_EMPTY, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		NKCPopupResourceConfirmBox.Instance.OpenForConfirm(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EQUIP_PRESET_SAVE_CONTENT, delegate
		{
			bool flag = true;
			m_listUpdatedEquipPosition.Clear();
			int num = 4;
			for (int i = 0; i < num; i++)
			{
				ITEM_EQUIP_POSITION iTEM_EQUIP_POSITION = (ITEM_EQUIP_POSITION)i;
				long equipUid = unitData.GetEquipUid(iTEM_EQUIP_POSITION);
				if (m_listEquipUId.Count <= i || equipUid != m_listEquipUId[i])
				{
					flag = false;
				}
				if (equipUid > 0)
				{
					m_listUpdatedEquipPosition.Add(iTEM_EQUIP_POSITION);
				}
			}
			m_iUpdatingIndex = m_iPresetIndex;
			if (flag)
			{
				m_bShowSetItemFx = true;
				UpdateEquipItemSlot(m_listEquipUId[0], m_slotEquipWeapon, OnEmptyEquipWeaponSlotClick, m_objEquipSetFXWeapon, m_aniEquipSetFXWeapon, m_setFxLoopWeapon, ITEM_EQUIP_POSITION.IEP_WEAPON);
				UpdateEquipItemSlot(m_listEquipUId[1], m_slotEquipDefense, OnEmptyEquipDefSlotClick, m_objEquipSetFXDefence, m_aniEquipSetFXDefence, m_setFxLoopDefence, ITEM_EQUIP_POSITION.IEP_DEFENCE);
				UpdateEquipItemSlot(m_listEquipUId[2], m_slotEquipAcc, OnEmptyEquipAccSlotClick, m_objEquipSetFXAcc, m_aniEquipSetFXAcc, m_setFxLoopAcc, ITEM_EQUIP_POSITION.IEP_ACC);
				UpdateEquipItemSlot(m_listEquipUId[3], m_slotEquipAcc_2, OnEmptyEquipAcc2SlotClick, m_objEquipSetFXAcc2, m_aniEquipSetFXAcc2, m_setFxLoopAcc2, ITEM_EQUIP_POSITION.IEP_ACC2);
				ActivateSlotEffect(activate: true);
				m_bShowSetItemFx = false;
				m_iUpdatingIndex = -1;
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ(m_iPresetIndex, unitData.m_UnitUID);
			}
		});
	}

	private void OnClickApplyPreset()
	{
		if (!IsAppliableSet(showMsg: true))
		{
			return;
		}
		NKMUnitData unitData = m_cNKCUIEquipPreset.UnitData;
		if (unitData == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMUnitManager.IsUnitBusy(myUserData, unitData, ignoreWorldmapState: true);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
			return;
		}
		bool flag = false;
		bool flag2 = unitData.IsUnlockAccessory2();
		bool flag3 = true;
		int num = 3;
		List<long> list = new List<long>();
		list.AddRange(unitData.EquipItemUids);
		int count = m_listEquipUId.Count;
		for (int i = 0; i < count; i++)
		{
			long num2 = m_listEquipUId[i];
			if (num2 <= 0)
			{
				continue;
			}
			NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(num2);
			if (itemEquip == null)
			{
				continue;
			}
			if (itemEquip.m_OwnerUnitUID > 0)
			{
				NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
				if (unitFromUID != null)
				{
					nKM_ERROR_CODE = NKMUnitManager.IsUnitBusy(myUserData, unitFromUID, ignoreWorldmapState: true);
					if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
					{
						NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
						return;
					}
				}
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet == null)
			{
				continue;
			}
			if (equipTemplet.IsPrivateEquip() && !equipTemplet.IsPrivateEquipForUnit(unitData.m_UnitID))
			{
				flag = true;
				continue;
			}
			if (list.Count > i)
			{
				list[i] = num2;
			}
			long equipUid = unitData.GetEquipUid((ITEM_EQUIP_POSITION)i);
			if ((i != num || flag2) && num2 != equipUid)
			{
				flag3 = false;
			}
		}
		if (flag)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EQUIP_PRESET_PRIVATE_EQUIP, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		if (!flag2 && m_listEquipUId.Count > num && m_listEquipUId[num] > 0)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EQUIP_PRESET_APPLY_SLOT_LOCKED, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		if (!flag3)
		{
			if (NKCUtil.IsPrivateEquipAlreadyEquipped(list))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_EQUIP_PRIVATE.ToString()));
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_APPLY_REQ(m_iPresetIndex, unitData.m_UnitUID);
			}
		}
	}

	private void OnClickSlotUp()
	{
		if (m_dOnClickUp != null)
		{
			m_dOnClickUp(m_presetData);
		}
	}

	private void OnClickSlotDown()
	{
		if (m_dOnClickDown != null)
		{
			m_dOnClickDown(m_presetData);
		}
	}

	private void OnToggleSlotCheck(bool bValue)
	{
		if (m_dOnToggleCheck != null)
		{
			m_dOnToggleCheck(m_presetData, bValue);
		}
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_presetData = null;
		m_cNKCUIEquipPreset = null;
		m_listEquipUId = null;
		m_strPresetName = null;
		m_listUpdatedEquipPosition = null;
	}
}
