using System;
using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKC.PacketHandler;
using NKC.UI;
using NKM;
using UnityEngine;
using UnityEngine.UI;

public class NKCUISelectionEquipDetail : NKCUIBase
{
	public enum OPTION_LIST_TYPE
	{
		STAT_1,
		STAT_2,
		SETOPTION,
		POTENTIAL
	}

	private enum SLOT_INDEX
	{
		SLOT1 = 1,
		SLOT2
	}

	public delegate void OnConfirmTuningOption(NKM_STAT_TYPE newType);

	public delegate void OnConfirmTuningSetOption(int newSetID);

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_SELECTION";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SELECTION_EQUIP_OPTION";

	private static NKCUISelectionEquipDetail m_Instance;

	public NKCUIInvenEquipSlot m_slotEquip;

	public NKCUIComToggle m_tglStat_1;

	public GameObject m_objStat_1_Checked;

	public NKCUIComToggle m_tglStat_2;

	public GameObject m_objStat_2_Checked;

	public NKCUIComToggle m_tglSetOption;

	public GameObject m_objSetOption_Checked;

	public NKCUIComToggle m_tglPotential;

	public GameObject m_objPotential_Checked;

	public LoopScrollRect m_loop;

	public NKCUISelectionEquipDetailSlot m_pfbSlot;

	public NKCUIComToggleGroup m_tglGroup;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnConfirm;

	public GameObject m_objSocketMax;

	private NKMItemMiscTemplet m_MiscTemplet;

	private NKMEquipTemplet m_EquipTemplet;

	private NKMEquipItemData m_SelectedEquipData = new NKMEquipItemData();

	private Stack<NKCUISelectionEquipDetailSlot> m_stkSlot = new Stack<NKCUISelectionEquipDetailSlot>();

	private OPTION_LIST_TYPE m_CurOptionListType;

	private IReadOnlyList<NKMEquipRandomStatTemplet> m_lstRandomStatGroup;

	private List<int> m_lstSetOptionGroup;

	private IReadOnlyList<NKMPotentialOptionTemplet> m_lstPotentialOption;

	private IReadOnlyList<NKMPotentialOptionTemplet> m_lstPotentialOption2;

	private int m_PotentialOptionKey;

	private int m_PotentialOptionKey2;

	private bool m_bBinaryConfirmChange;

	private OnConfirmTuningOption m_dConfirmOption;

	private OnConfirmTuningSetOption m_dConfirmSetOption;

	private NKM_STAT_TYPE m_CurEquipItemStatType;

	private int m_iOriEquipDataSetOptionID;

	public static NKCUISelectionEquipDetail Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUISelectionEquipDetail>("AB_UI_NKM_UI_POPUP_SELECTION", "NKM_UI_POPUP_SELECTION_EQUIP_OPTION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUISelectionEquipDetail>();
				m_Instance.InitUI();
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

	public override string MenuName => "";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetToggleValueChangedDelegate(m_tglStat_1, OnTglStat_1);
		NKCUtil.SetToggleValueChangedDelegate(m_tglStat_2, OnTglStat_2);
		NKCUtil.SetToggleValueChangedDelegate(m_tglSetOption, OnTglSetOption);
		NKCUtil.SetToggleValueChangedDelegate(m_tglPotential, OnTglPotential);
		NKCUtil.SetBindFunction(m_btnClose, base.Close);
		NKCUtil.SetBindFunction(m_btnConfirm, OnClickConfirm);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKMItemMiscTemplet miscTemplet, int targetEquipId)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_MiscTemplet = miscTemplet;
		m_EquipTemplet = NKMItemManager.GetEquipTemplet(targetEquipId);
		if (m_EquipTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_SelectedEquipData = NKCEquipSortSystem.MakeTempEquipData(m_EquipTemplet.m_ItemEquipID);
		if (m_EquipTemplet.IsRelic())
		{
			m_SelectedEquipData.potentialOptions = new List<NKMPotentialOption>();
			m_SelectedEquipData.potentialOptions.Add(new NKMPotentialOption());
			m_SelectedEquipData.potentialOptions[0].statType = NKM_STAT_TYPE.NST_RANDOM;
			if (m_EquipTemplet.GetPotentialOptionGroupID2() > 0)
			{
				m_SelectedEquipData.potentialOptions.Add(new NKMPotentialOption());
				m_SelectedEquipData.potentialOptions[1].statType = NKM_STAT_TYPE.NST_RANDOM;
			}
		}
		m_PotentialOptionKey = 0;
		m_PotentialOptionKey2 = 0;
		m_bBinaryConfirmChange = false;
		NKCUtil.SetGameobjectActive(m_tglPotential, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSocketMax, m_MiscTemplet != null && m_MiscTemplet.ChangePotenFirstOptionMax);
		SelectFirstOption();
		SetEquipData();
		SetToggleState();
		SetScrollData(m_CurOptionListType);
		UIOpened();
	}

	public void Open(long EquipItemUID, int optionIdx, bool SetOption, OnConfirmTuningOption optionCallBack, OnConfirmTuningSetOption setOptionCallBack)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(EquipItemUID);
		if (itemEquip == null)
		{
			return;
		}
		m_EquipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (m_EquipTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_SelectedEquipData = NKCEquipSortSystem.MakeDummyEquipData(EquipItemUID);
		if (m_SelectedEquipData.m_Stat != null && !SetOption && m_SelectedEquipData.m_Stat.Count > optionIdx)
		{
			m_CurEquipItemStatType = m_SelectedEquipData.m_Stat[optionIdx].type;
		}
		m_iOriEquipDataSetOptionID = m_SelectedEquipData.m_SetOptionId;
		m_PotentialOptionKey = 0;
		m_PotentialOptionKey2 = 0;
		m_bBinaryConfirmChange = true;
		m_dConfirmOption = optionCallBack;
		m_dConfirmSetOption = setOptionCallBack;
		NKCUtil.SetGameobjectActive(m_objSocketMax, bValue: false);
		SelectOption(optionIdx, SetOption);
		SetEquipData();
		SetToggleState(optionIdx, SetOption);
		SetScrollData(m_CurOptionListType);
		UIOpened();
	}

	private void SetEquipData()
	{
		m_slotEquip.SetData(m_SelectedEquipData);
	}

	private void SetToggleState()
	{
		if (m_MiscTemplet == null)
		{
			return;
		}
		bool flag = true;
		m_tglStat_1.SetLock(!m_MiscTemplet.ChangeStat || !NKMEquipTuningManager.IsChangeableStatGroup(m_EquipTemplet.m_StatGroupID));
		m_tglStat_1.Select(m_CurOptionListType == OPTION_LIST_TYPE.STAT_1, bForce: true);
		NKCUtil.SetGameobjectActive(m_objStat_1_Checked, !m_tglStat_1.m_bLock && m_SelectedEquipData.m_Stat[1].type != NKM_STAT_TYPE.NST_RANDOM);
		flag &= m_tglStat_1.m_bLock || m_objStat_1_Checked.activeSelf;
		m_tglStat_2.SetLock(!m_MiscTemplet.ChangeStat || !NKMEquipTuningManager.IsChangeableStatGroup(m_EquipTemplet.m_StatGroupID_2));
		m_tglStat_2.Select(m_CurOptionListType == OPTION_LIST_TYPE.STAT_2, bForce: true);
		NKCUtil.SetGameobjectActive(m_objStat_2_Checked, !m_tglStat_2.m_bLock && m_SelectedEquipData.m_Stat[2].type != NKM_STAT_TYPE.NST_RANDOM);
		flag &= m_tglStat_2.m_bLock || m_objStat_2_Checked.activeSelf;
		m_tglSetOption.SetLock(!m_MiscTemplet.ChangeSetOption);
		m_tglSetOption.Select(m_CurOptionListType == OPTION_LIST_TYPE.SETOPTION, bForce: true);
		NKCUtil.SetGameobjectActive(m_objSetOption_Checked, !m_tglSetOption.m_bLock && m_SelectedEquipData.m_SetOptionId > 0);
		flag &= m_tglSetOption.m_bLock || m_objSetOption_Checked.activeSelf;
		m_tglPotential.SetLock(!m_MiscTemplet.ChangePotenOption || m_SelectedEquipData.potentialOptions.Count <= 0 || m_SelectedEquipData.potentialOptions[0] == null);
		m_tglPotential.Select(m_CurOptionListType == OPTION_LIST_TYPE.POTENTIAL, bForce: true);
		bool flag2 = true;
		for (int i = 0; i < m_SelectedEquipData.potentialOptions.Count; i++)
		{
			if (m_SelectedEquipData.potentialOptions[i].statType == NKM_STAT_TYPE.NST_RANDOM)
			{
				flag2 = false;
			}
		}
		NKCUtil.SetGameobjectActive(m_objPotential_Checked, !m_tglPotential.m_bLock && m_SelectedEquipData.potentialOptions.Count > 0 && flag2);
		flag &= m_tglPotential.m_bLock || m_objPotential_Checked.activeSelf;
		m_btnConfirm.SetLock(!flag);
	}

	private void SetToggleState(int OptionIdx, bool SetOption)
	{
		switch (OptionIdx)
		{
		case 1:
			m_tglStat_1.SetLock(!NKMEquipTuningManager.IsChangeableStatGroup(m_EquipTemplet.m_StatGroupID));
			m_tglStat_1.Select(m_CurOptionListType == OPTION_LIST_TYPE.STAT_1, bForce: true);
			NKCUtil.SetGameobjectActive(m_objStat_1_Checked, !m_tglStat_1.m_bLock && m_SelectedEquipData.m_Stat[1].type != NKM_STAT_TYPE.NST_RANDOM);
			break;
		case 2:
			m_tglStat_2.SetLock(!NKMEquipTuningManager.IsChangeableStatGroup(m_EquipTemplet.m_StatGroupID_2));
			m_tglStat_2.Select(m_CurOptionListType == OPTION_LIST_TYPE.STAT_2, bForce: true);
			NKCUtil.SetGameobjectActive(m_objStat_2_Checked, !m_tglStat_2.m_bLock && m_SelectedEquipData.m_Stat[2].type != NKM_STAT_TYPE.NST_RANDOM);
			break;
		default:
			m_tglSetOption.Select(m_CurOptionListType == OPTION_LIST_TYPE.SETOPTION, bForce: true);
			NKCUtil.SetGameobjectActive(m_objSetOption_Checked, !m_tglSetOption.m_bLock && m_SelectedEquipData.m_SetOptionId > 0);
			break;
		}
		NKCUtil.SetGameobjectActive(m_tglStat_1, OptionIdx == 1);
		NKCUtil.SetGameobjectActive(m_tglStat_2, OptionIdx == 2);
		NKCUtil.SetGameobjectActive(m_tglSetOption, SetOption);
		NKCUtil.SetGameobjectActive(m_tglPotential, bValue: false);
	}

	private void SetScrollData(OPTION_LIST_TYPE listType, bool bResetScroll = false)
	{
		m_CurOptionListType = listType;
		switch (listType)
		{
		case OPTION_LIST_TYPE.STAT_1:
			m_lstRandomStatGroup = NKMEquipTuningManager.GetEquipRandomStatGroupList(m_EquipTemplet.m_StatGroupID);
			m_loop.TotalCount = m_lstRandomStatGroup.Count;
			m_tglGroup.m_MaxMultiCount = 1;
			break;
		case OPTION_LIST_TYPE.STAT_2:
			m_lstRandomStatGroup = NKMEquipTuningManager.GetEquipRandomStatGroupList(m_EquipTemplet.m_StatGroupID_2);
			m_loop.TotalCount = m_lstRandomStatGroup.Count;
			m_tglGroup.m_MaxMultiCount = 1;
			break;
		case OPTION_LIST_TYPE.SETOPTION:
			m_lstSetOptionGroup = m_EquipTemplet.m_lstSetGroup;
			m_loop.TotalCount = m_lstSetOptionGroup.Count;
			m_tglGroup.m_MaxMultiCount = 1;
			break;
		case OPTION_LIST_TYPE.POTENTIAL:
		{
			NKMPotentialOptionGroupTemplet nKMPotentialOptionGroupTemplet = NKMPotentialOptionGroupTemplet.Find(m_EquipTemplet.GetPotentialOptionGroupID());
			if (nKMPotentialOptionGroupTemplet != null)
			{
				m_tglGroup.m_MaxMultiCount = ((m_EquipTemplet.GetPotentialOptionGroupID2() <= 0) ? 1 : 2);
				m_lstPotentialOption = nKMPotentialOptionGroupTemplet.OptionList;
				nKMPotentialOptionGroupTemplet = NKMPotentialOptionGroupTemplet.Find(m_EquipTemplet.GetPotentialOptionGroupID2());
				if (nKMPotentialOptionGroupTemplet != null)
				{
					m_lstPotentialOption2 = nKMPotentialOptionGroupTemplet.OptionList;
					m_loop.TotalCount = m_lstPotentialOption.Count + m_lstPotentialOption2.Count;
				}
				else
				{
					m_loop.TotalCount = m_lstPotentialOption.Count;
				}
			}
			break;
		}
		}
		if (bResetScroll)
		{
			m_loop.SetIndexPosition(0);
			return;
		}
		m_loop.SetIndexPosition(0);
		m_loop.RefreshCells(bForce: true);
	}

	private void SelectFirstOption()
	{
		if (m_MiscTemplet.ChangeStat)
		{
			if (NKMEquipTuningManager.IsChangeableStatGroup(m_EquipTemplet.m_StatGroupID))
			{
				m_CurOptionListType = OPTION_LIST_TYPE.STAT_1;
				m_tglStat_1.Select(bSelect: true, bForce: true);
			}
			else if (NKMEquipTuningManager.IsChangeableStatGroup(m_EquipTemplet.m_StatGroupID_2))
			{
				m_CurOptionListType = OPTION_LIST_TYPE.STAT_2;
				m_tglStat_2.Select(bSelect: true, bForce: true);
			}
		}
		else if (m_MiscTemplet.ChangeSetOption)
		{
			m_CurOptionListType = OPTION_LIST_TYPE.SETOPTION;
			m_tglSetOption.Select(bSelect: true, bForce: true);
		}
		else if (m_MiscTemplet.ChangePotenOption)
		{
			m_CurOptionListType = OPTION_LIST_TYPE.POTENTIAL;
			m_tglPotential.Select(bSelect: true, bForce: true);
		}
		else
		{
			Log.Error($"NKMItemMiscTemplet - {m_MiscTemplet.Key}, \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdɼ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffdµ\ufffd \ufffd\u02fe\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUISelectionEquipDetail.cs", 348);
		}
	}

	private void SelectOption(int OptionIdx, bool SetOption)
	{
		if (OptionIdx == 1 && NKMEquipTuningManager.IsChangeableStatGroup(m_EquipTemplet.m_StatGroupID))
		{
			m_CurOptionListType = OPTION_LIST_TYPE.STAT_1;
			m_tglStat_1.Select(bSelect: true, bForce: true);
		}
		if (OptionIdx == 2 && NKMEquipTuningManager.IsChangeableStatGroup(m_EquipTemplet.m_StatGroupID_2))
		{
			m_CurOptionListType = OPTION_LIST_TYPE.STAT_2;
			m_tglStat_2.Select(bSelect: true, bForce: true);
		}
		if (SetOption)
		{
			m_CurOptionListType = OPTION_LIST_TYPE.SETOPTION;
			m_tglSetOption.Select(bSelect: true, bForce: true);
		}
		NKCUtil.SetGameobjectActive(m_tglPotential, bValue: false);
	}

	private RectTransform GetObject(int idx)
	{
		NKCUISelectionEquipDetailSlot nKCUISelectionEquipDetailSlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCUISelectionEquipDetailSlot = m_stkSlot.Pop();
		}
		else
		{
			nKCUISelectionEquipDetailSlot = UnityEngine.Object.Instantiate(m_pfbSlot, m_loop.content);
			nKCUISelectionEquipDetailSlot.InitUI(OnSelectOptionSlot, m_tglGroup);
		}
		NKCUtil.SetGameobjectActive(nKCUISelectionEquipDetailSlot.gameObject, bValue: false);
		return nKCUISelectionEquipDetailSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUISelectionEquipDetailSlot component = tr.GetComponent<NKCUISelectionEquipDetailSlot>();
		if (!(component == null))
		{
			NKCUtil.SetGameobjectActive(component.gameObject, bValue: false);
			m_stkSlot.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUISelectionEquipDetailSlot component = tr.GetComponent<NKCUISelectionEquipDetailSlot>();
		if (component == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(component.gameObject, bValue: true);
		switch (m_CurOptionListType)
		{
		case OPTION_LIST_TYPE.STAT_1:
			component.SetData(m_CurOptionListType, m_lstRandomStatGroup[idx].m_StatType, m_SelectedEquipData.m_Stat[1].type == m_lstRandomStatGroup[idx].m_StatType);
			if (m_bBinaryConfirmChange)
			{
				component.SetLock(m_lstRandomStatGroup[idx].m_StatType == m_CurEquipItemStatType);
			}
			else
			{
				component.SetLock(bLock: false);
			}
			break;
		case OPTION_LIST_TYPE.STAT_2:
			component.SetData(m_CurOptionListType, m_lstRandomStatGroup[idx].m_StatType, m_SelectedEquipData.m_Stat[2].type == m_lstRandomStatGroup[idx].m_StatType);
			if (m_bBinaryConfirmChange)
			{
				component.SetLock(m_lstRandomStatGroup[idx].m_StatType == m_CurEquipItemStatType);
			}
			else
			{
				component.SetLock(bLock: false);
			}
			break;
		case OPTION_LIST_TYPE.SETOPTION:
			component.SetData(m_lstSetOptionGroup[idx], m_SelectedEquipData.m_SetOptionId == m_lstSetOptionGroup[idx]);
			if (m_bBinaryConfirmChange)
			{
				component.SetLock(m_iOriEquipDataSetOptionID == m_lstSetOptionGroup[idx]);
			}
			else
			{
				component.SetLock(bLock: false);
			}
			break;
		case OPTION_LIST_TYPE.POTENTIAL:
			if (idx < m_lstPotentialOption.Count)
			{
				component.SetData(m_lstPotentialOption[idx], 0, m_SelectedEquipData.potentialOptions[0].statType == m_lstPotentialOption[idx].StatType);
			}
			else
			{
				component.SetData(m_lstPotentialOption2[idx - m_lstPotentialOption.Count], 1, m_SelectedEquipData.potentialOptions[1].statType == m_lstPotentialOption2[idx - m_lstPotentialOption.Count].StatType);
			}
			break;
		}
	}

	private void OnTglStat_1(bool bValue)
	{
		if (bValue)
		{
			SetScrollData(OPTION_LIST_TYPE.STAT_1, bResetScroll: true);
		}
	}

	private void OnTglStat_2(bool bValue)
	{
		if (bValue)
		{
			SetScrollData(OPTION_LIST_TYPE.STAT_2, bResetScroll: true);
		}
	}

	private void OnTglSetOption(bool bValue)
	{
		if (bValue)
		{
			SetScrollData(OPTION_LIST_TYPE.SETOPTION, bResetScroll: true);
		}
	}

	private void OnTglPotential(bool bValue)
	{
		if (bValue)
		{
			SetScrollData(OPTION_LIST_TYPE.POTENTIAL, bResetScroll: true);
		}
	}

	private void OnSelectOptionSlot(OPTION_LIST_TYPE optionListType, NKM_STAT_TYPE statType, int setOptionId, int potentialOptionKey)
	{
		m_CurOptionListType = optionListType;
		switch (optionListType)
		{
		case OPTION_LIST_TYPE.STAT_1:
			m_SelectedEquipData.m_Stat[1].type = statType;
			break;
		case OPTION_LIST_TYPE.STAT_2:
			m_SelectedEquipData.m_Stat[2].type = statType;
			break;
		case OPTION_LIST_TYPE.SETOPTION:
			m_SelectedEquipData.m_SetOptionId = setOptionId;
			break;
		case OPTION_LIST_TYPE.POTENTIAL:
			if (m_tglGroup.m_MaxMultiCount == 1)
			{
				m_SelectedEquipData.potentialOptions[0].statType = statType;
				m_PotentialOptionKey = potentialOptionKey;
				break;
			}
			switch (setOptionId)
			{
			case 0:
				m_PotentialOptionKey = potentialOptionKey;
				break;
			case 1:
				m_PotentialOptionKey2 = potentialOptionKey;
				break;
			}
			if (setOptionId < m_SelectedEquipData.potentialOptions.Count)
			{
				m_SelectedEquipData.potentialOptions[setOptionId].statType = statType;
			}
			break;
		}
		EquipDataStatValueUpdate(statType);
		RefreshUI();
	}

	private void EquipDataStatValueUpdate(NKM_STAT_TYPE statType)
	{
		if (!m_bBinaryConfirmChange)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(m_SelectedEquipData.m_ItemEquipID);
		if (equipTemplet != null)
		{
			int statGroupID = ((m_CurOptionListType == OPTION_LIST_TYPE.STAT_1) ? equipTemplet.m_StatGroupID : equipTemplet.m_StatGroupID_2);
			int precision = ((m_CurOptionListType == OPTION_LIST_TYPE.STAT_1) ? m_SelectedEquipData.m_Precision : m_SelectedEquipData.m_Precision2);
			float num = 0f;
			NKMEquipRandomStatTemplet equipRandomStat = NKMEquipTuningManager.GetEquipRandomStat(statGroupID, statType);
			if (equipRandomStat != null)
			{
				num = equipRandomStat.CalcResultStat(precision);
			}
			if (NKCUIForgeTuning.IsPercentStat(equipRandomStat))
			{
				num = (float)Math.Round(new decimal(num) * 1000m) / 1000f;
			}
			if (m_CurOptionListType == OPTION_LIST_TYPE.STAT_1)
			{
				m_SelectedEquipData.m_Stat[1].stat_value = num;
			}
			if (m_CurOptionListType == OPTION_LIST_TYPE.STAT_2)
			{
				m_SelectedEquipData.m_Stat[2].stat_value = num;
			}
		}
	}

	private void RefreshUI()
	{
		SetEquipData();
		SetToggleState();
		SetScrollData(m_CurOptionListType);
	}

	private void OnClickConfirm()
	{
		if (m_bBinaryConfirmChange)
		{
			switch (m_CurOptionListType)
			{
			case OPTION_LIST_TYPE.STAT_1:
				if (m_CurEquipItemStatType == m_SelectedEquipData.m_Stat[1].type)
				{
					NKCPopupMessageManager.AddPopupMessage(NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_INVALID_EQUIP_OPTION_DUPLICATE));
				}
				else
				{
					m_dConfirmOption?.Invoke(m_SelectedEquipData.m_Stat[1].type);
				}
				break;
			case OPTION_LIST_TYPE.STAT_2:
				if (m_CurEquipItemStatType == m_SelectedEquipData.m_Stat[2].type)
				{
					NKCPopupMessageManager.AddPopupMessage(NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_INVALID_EQUIP_OPTION_DUPLICATE));
				}
				else
				{
					m_dConfirmOption?.Invoke(m_SelectedEquipData.m_Stat[2].type);
				}
				break;
			case OPTION_LIST_TYPE.SETOPTION:
				m_dConfirmSetOption?.Invoke(m_SelectedEquipData.m_SetOptionId);
				break;
			}
		}
		else
		{
			List<NKM_STAT_TYPE> list = new List<NKM_STAT_TYPE>();
			list.Add(m_SelectedEquipData.m_Stat[1].type);
			list.Add(m_SelectedEquipData.m_Stat[2].type);
			NKCPopupSelectionConfirm.Instance.Open(m_MiscTemplet, m_SelectedEquipData.m_ItemEquipID, 1L, m_SelectedEquipData.m_SetOptionId, 0, list, m_PotentialOptionKey, m_PotentialOptionKey2);
		}
	}
}
