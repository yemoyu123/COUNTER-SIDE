using System.Collections.Generic;
using ClientPacket.Common;
using NKC.UI.Guild;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupAssistSelect : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ASSIST_SELECT";

	private static NKCUIPopupAssistSelect m_Instance;

	[Header("Assist User Profile")]
	public NKCUISlotProfile m_AssistUserProfile;

	public NKCUISlotTitle m_AssistUserProfileTitle;

	public Text m_lbAssistUserProfileLevel;

	public Text m_lbAssistUserProfileUserName;

	public Text m_lbAssistUserProfileUserUID;

	public NKCUIGuildBadge m_gbAssistUserProfileGuildBadge;

	public Text m_lbAssistUserProfileUserGuildName;

	[Space]
	public LoopScrollRect m_LoopScrollRect;

	public NKCUIComUnitSortOptions m_ComUnitSortOptions;

	public NKCUIComStateButton m_csbtnCancel;

	public NKCUIComStateButton m_csbtnOK;

	public GameObject m_objUserProfile;

	public GameObject m_objUserProfileEmpty;

	public NKCUIUnitSelectListSlotAssist m_pfbUnitSlotAssist;

	private Stack<NKCUIUnitSelectListSlotAssist> m_stkSlot = new Stack<NKCUIUnitSelectListSlotAssist>();

	public Transform m_rtSlotPool;

	private NKCUnitSortSystem m_UnitSortSystem;

	private readonly HashSet<NKCUnitSortSystem.eSortCategory> setSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>
	{
		NKCUnitSortSystem.eSortCategory.UnitPower,
		NKCUnitSortSystem.eSortCategory.Level,
		NKCUnitSortSystem.eSortCategory.UnitSummonCost,
		NKCUnitSortSystem.eSortCategory.UID
	};

	private readonly HashSet<NKCUnitSortSystem.eFilterCategory> setFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
	{
		NKCUnitSortSystem.eFilterCategory.UnitType,
		NKCUnitSortSystem.eFilterCategory.UnitRole,
		NKCUnitSortSystem.eFilterCategory.UnitTargetType,
		NKCUnitSortSystem.eFilterCategory.Rarity,
		NKCUnitSortSystem.eFilterCategory.Cost
	};

	private NKMDeckIndex m_curDeckIndex;

	private List<NKMSupportUnitProfileData> m_lstSupportUnitDatas;

	private long m_selectedUserUID;

	private List<NKCUIUnitSelectListSlotAssist> m_lstVisibleSlots = new List<NKCUIUnitSelectListSlotAssist>();

	public static NKCUIPopupAssistSelect Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupAssistSelect>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_ASSIST_SELECT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupAssistSelect>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => "Assist Unit";

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

	public override void CloseInternal()
	{
		m_lstVisibleSlots.Clear();
		base.gameObject.SetActive(value: false);
	}

	public static int GetCalculateOperatorPower(NKMUnitData unitData, NKMAsyncUnitEquipData asyncEquipData)
	{
		NKMEquipmentSet equipSetData = GetEquipSetData(asyncEquipData);
		if (equipSetData != null)
		{
			return unitData.CalculateUnitOperationPower(equipSetData);
		}
		return 0;
	}

	public static NKMEquipmentSet GetEquipSetData(NKMAsyncUnitEquipData asyncEquipData)
	{
		if (asyncEquipData == null)
		{
			return null;
		}
		NKMEquipItemData weapon = null;
		NKMEquipItemData defence = null;
		NKMEquipItemData accessory = null;
		NKMEquipItemData accessory2 = null;
		bool flag = true;
		foreach (NKMEquipItemData equip in asyncEquipData.equips)
		{
			if (equip == null)
			{
				continue;
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equip.m_ItemEquipID);
			if (equipTemplet == null)
			{
				continue;
			}
			switch (equipTemplet.m_ItemEquipPosition)
			{
			case ITEM_EQUIP_POSITION.IEP_WEAPON:
				weapon = equip;
				break;
			case ITEM_EQUIP_POSITION.IEP_DEFENCE:
				defence = equip;
				break;
			case ITEM_EQUIP_POSITION.IEP_ACC:
			case ITEM_EQUIP_POSITION.IEP_ACC2:
				if (flag)
				{
					accessory = equip;
					flag = false;
				}
				else
				{
					accessory2 = equip;
				}
				break;
			}
		}
		return new NKMEquipmentSet(weapon, defence, accessory, accessory2);
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnCancel, base.Close);
		NKCUtil.SetBindFunction(m_csbtnOK, OnBtnOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		if (null != m_LoopScrollRect)
		{
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
			m_LoopScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		}
		if (null != m_ComUnitSortOptions)
		{
			m_ComUnitSortOptions.Init(OnSortChanged, bIsCollection: true);
			m_ComUnitSortOptions.RegisterCategories(setFilterCategory, setSortCategory, bFavoriteFilterActive: false);
		}
		m_AssistUserProfile?.Init();
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
	}

	public void Open(List<NKMSupportUnitProfileData> lstSupportUnitDatas, NKMDeckIndex deckIndex, long lSupportUserUID)
	{
		m_curDeckIndex = deckIndex;
		m_lstSupportUnitDatas = lstSupportUnitDatas;
		UpdateSortOption();
		OnSortChanged(bResetScroll: true);
		OnSelectUser(lSupportUserUID);
		UIOpened();
	}

	private int SortUnitByUnitID(NKMUnitData lData, NKMUnitData rData)
	{
		return rData.m_UnitID.CompareTo(lData.m_UnitID);
	}

	private void UpdateSortOption()
	{
		List<NKMUnitData> list = new List<NKMUnitData>();
		foreach (NKMSupportUnitProfileData lstSupportUnitData in m_lstSupportUnitDatas)
		{
			if (lstSupportUnitData != null && lstSupportUnitData.supportUnitData != null && lstSupportUnitData.supportUnitData.asyncUnitEquip != null && lstSupportUnitData.supportUnitData.asyncUnitEquip.asyncUnit != null)
			{
				NKMAsyncUnitData asyncUnit = lstSupportUnitData.supportUnitData.asyncUnitEquip.asyncUnit;
				NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(asyncUnit.unitId, asyncUnit.unitLevel, (short)asyncUnit.limitBreakLevel, asyncUnit.tacticLevel, asyncUnit.reactorLevel);
				nKMUnitData.m_UnitUID = lstSupportUnitData.supportUnitData.userUid;
				nKMUnitData.m_UserUID = lstSupportUnitData.supportUnitData.userUid;
				nKMUnitData.unitPower = GetCalculateOperatorPower(nKMUnitData, lstSupportUnitData.supportUnitData.asyncUnitEquip);
				list.Add(nKMUnitData);
			}
		}
		NKCUnitSortSystem.UnitListOptions options = default(NKCUnitSortSystem.UnitListOptions);
		options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>
		{
			NKCUnitSortSystem.eSortOption.CustomAscend1,
			NKCUnitSortSystem.eSortOption.IDX_First,
			NKCUnitSortSystem.eSortOption.UID_First
		};
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.bIncludeUndeckableUnit = true;
		options.bHideTokenFiltering = true;
		options.lstCustomSortFunc = new Dictionary<NKCUnitSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>>();
		options.lstCustomSortFunc.Add(NKCUnitSortSystem.eSortCategory.Custom1, new KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>(NKCUtilString.GET_STRING_SORT_IDX, SortUnitByUnitID));
		m_UnitSortSystem = new NKCDummyUnitSort(options, list);
		m_ComUnitSortOptions.RegisterUnitSort(m_UnitSortSystem);
		m_ComUnitSortOptions.ResetUI();
	}

	private void OnSortChanged(bool bResetScroll)
	{
		m_LoopScrollRect.TotalCount = m_UnitSortSystem.SortedUnitList.Count;
		m_LoopScrollRect.SetIndexPosition(0);
		m_LoopScrollRect.RefreshCells(bForce: true);
	}

	private void OnBtnOK()
	{
		if (m_selectedUserUID == 0L)
		{
			return;
		}
		foreach (NKMSupportUnitProfileData lstSupportUnitData in m_lstSupportUnitDatas)
		{
			if (lstSupportUnitData.commonProfile.userUid == m_selectedUserUID)
			{
				NKCPacketSender.Send_NKMPacket_SET_DUNGEON_SUPPORT_UNIT_REQ(new NKMDungeonSupportData
				{
					userUid = m_selectedUserUID,
					asyncUnitEquip = lstSupportUnitData.supportUnitData.asyncUnitEquip,
					deckIndex = m_curDeckIndex
				});
				break;
			}
		}
	}

	private RectTransform GetSlot(int index)
	{
		if (m_stkSlot.Count > 0)
		{
			return m_stkSlot.Pop().GetComponent<RectTransform>();
		}
		NKCUIUnitSelectListSlotAssist nKCUIUnitSelectListSlotAssist = Object.Instantiate(m_pfbUnitSlotAssist);
		nKCUIUnitSelectListSlotAssist.Init();
		NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlotAssist, bValue: true);
		nKCUIUnitSelectListSlotAssist.transform.localScale = Vector3.one;
		nKCUIUnitSelectListSlotAssist.transform.SetParent(m_LoopScrollRect.content);
		return nKCUIUnitSelectListSlotAssist.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		go.SetParent(m_rtSlotPool);
		NKCUIUnitSelectListSlotAssist component = go.GetComponent<NKCUIUnitSelectListSlotAssist>();
		if (component != null)
		{
			m_lstVisibleSlots.Remove(component);
			m_stkSlot.Push(component);
		}
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		if (m_UnitSortSystem.SortedUnitList[idx] == null)
		{
			return;
		}
		NKCUIUnitSelectListSlotAssist component = tr.GetComponent<NKCUIUnitSelectListSlotAssist>();
		if (component == null)
		{
			return;
		}
		foreach (NKMSupportUnitProfileData lstSupportUnitData in m_lstSupportUnitDatas)
		{
			if (lstSupportUnitData.supportUnitData.userUid != m_UnitSortSystem.SortedUnitList[idx].m_UserUID)
			{
				continue;
			}
			NKMAsyncUnitData asyncUnit = lstSupportUnitData.supportUnitData.asyncUnitEquip.asyncUnit;
			NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(asyncUnit.unitId, asyncUnit.unitLevel, (short)asyncUnit.limitBreakLevel, asyncUnit.tacticLevel, asyncUnit.reactorLevel);
			nKMUnitData.m_SkinID = asyncUnit.skinId;
			List<NKMEquipItemData> equips = lstSupportUnitData.supportUnitData.asyncUnitEquip.equips;
			NKMEquipItemData weapon = null;
			NKMEquipItemData defence = null;
			NKMEquipItemData nKMEquipItemData = null;
			NKMEquipItemData accessory = null;
			foreach (NKMEquipItemData item in equips)
			{
				NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(item.m_ItemEquipID);
				if (equipTemplet == null)
				{
					continue;
				}
				switch (equipTemplet.m_ItemEquipPosition)
				{
				case ITEM_EQUIP_POSITION.IEP_WEAPON:
					weapon = item;
					break;
				case ITEM_EQUIP_POSITION.IEP_DEFENCE:
					defence = item;
					break;
				case ITEM_EQUIP_POSITION.IEP_ACC:
				case ITEM_EQUIP_POSITION.IEP_ACC2:
					if (nKMEquipItemData == null)
					{
						nKMEquipItemData = item;
					}
					else
					{
						accessory = item;
					}
					break;
				}
			}
			NKMEquipmentSet nKMEquipmentSet = new NKMEquipmentSet(weapon, defence, nKMEquipItemData, accessory);
			nKMUnitData.InitEquipItemFromDb(nKMEquipmentSet.WeaponUid, nKMEquipmentSet.DefenceUid, nKMEquipmentSet.AccessoryUid, nKMEquipmentSet.Accessory2Uid);
			component.SetData(nKMUnitData, nKMEquipmentSet, lstSupportUnitData.supportUnitData.userUid, OnSelectUser);
			m_lstVisibleSlots.Add(component);
		}
	}

	private void OnSelectUser(long selectedUserUID)
	{
		m_selectedUserUID = 0L;
		foreach (NKMSupportUnitProfileData lstSupportUnitData in m_lstSupportUnitDatas)
		{
			if (lstSupportUnitData != null && lstSupportUnitData.commonProfile != null && lstSupportUnitData.commonProfile.userUid == selectedUserUID)
			{
				m_AssistUserProfile.SetProfiledata(lstSupportUnitData.commonProfile, null);
				m_AssistUserProfileTitle.SetData(lstSupportUnitData.commonProfile.titleId, showEmpty: true, showLock: false);
				NKCUtil.SetLabelText(m_lbAssistUserProfileLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, lstSupportUnitData.commonProfile.level));
				NKCUtil.SetLabelText(m_lbAssistUserProfileUserName, lstSupportUnitData.commonProfile.nickname);
				NKCUtil.SetLabelText(m_lbAssistUserProfileUserUID, lstSupportUnitData.commonProfile.friendCode.ToString());
				NKCUtil.SetGameobjectActive(m_gbAssistUserProfileGuildBadge, lstSupportUnitData.guildData != null);
				if (lstSupportUnitData.guildData == null)
				{
					NKCUtil.SetLabelText(m_lbAssistUserProfileUserGuildName, "");
				}
				else
				{
					NKCUtil.SetLabelText(m_lbAssistUserProfileUserGuildName, lstSupportUnitData.guildData.guildName);
					m_gbAssistUserProfileGuildBadge?.SetData(lstSupportUnitData.guildData.badgeId);
				}
				m_selectedUserUID = selectedUserUID;
				break;
			}
		}
		foreach (NKCUIUnitSelectListSlotAssist lstVisibleSlot in m_lstVisibleSlots)
		{
			if (!(null == lstVisibleSlot))
			{
				lstVisibleSlot.SetSelect(lstVisibleSlot.SupportUserUID == selectedUserUID);
			}
		}
		NKCUtil.SetGameobjectActive(m_objUserProfile, m_selectedUserUID != 0);
		NKCUtil.SetGameobjectActive(m_objUserProfileEmpty, m_selectedUserUID == 0);
	}
}
