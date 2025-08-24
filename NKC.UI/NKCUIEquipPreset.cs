using System.Collections.Generic;
using ClientPacket.Item;
using DG.Tweening;
using DG.Tweening.Core;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEquipPreset : MonoBehaviour
{
	public DOTweenVisualManager m_doTweenVisualManager;

	public NKCUIComStateButton m_csbtnInfomation;

	public NKCUIComStateButton m_csbtnPresetAdd;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComToggle m_tglRemovePreset;

	public NKCUIComStateButton m_btnCancelRemoveMode;

	public NKCUIComToggle m_tglPresetFilter;

	public NKCUIComToggle m_tglOrderEdit;

	public LoopScrollRect m_LoopScrollRect;

	public Text m_lbPresetNumber;

	public string m_strGuideArticleID;

	[Header("페이지 설정")]
	public int m_maxSlotCountPerPage;

	public bool m_enableInterPageSlotMove;

	public NKCUIComToggle[] m_tglPageArray;

	private List<NKMEquipPresetData> m_listFilteredEquipPresetData = new List<NKMEquipPresetData>();

	private NKMUnitData m_cNKMUnitData;

	private long m_userUID;

	private int m_iNewPresetIndexFrom;

	private int m_iCurrentPage;

	private bool m_bShowFierceInfo;

	private bool m_bPresetRemoveState;

	private bool m_bPresetFilterState;

	private bool m_bInitLoopScroll;

	private bool m_bMoveSlotState;

	public NKMUnitData UnitData => m_cNKMUnitData;

	public static NKMEquipPresetData GetNewEmptyPresetData()
	{
		return new NKMEquipPresetData
		{
			presetIndex = 0,
			presetType = NKM_EQUIP_PRESET_TYPE.NEPT_NONE,
			presetName = "",
			equipUids = { 0L, 0L, 0L, 0L }
		};
	}

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnInfomation, OnClickInformation);
		NKCUtil.SetButtonClickDelegate(m_csbtnPresetAdd, OnClickPresetAdd);
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, OnClickClose);
		NKCUtil.SetToggleValueChangedDelegate(m_tglRemovePreset, OnToggleRemovePreset);
		NKCUtil.SetToggleValueChangedDelegate(m_tglPresetFilter, OnTogglePresetFilter);
		NKCUtil.SetToggleValueChangedDelegate(m_tglOrderEdit, OnToggleOrderEdit);
		NKCUtil.SetButtonClickDelegate(m_btnCancelRemoveMode, OnClickCancelRemoveMode);
		if (m_tglPageArray != null)
		{
			int num = m_tglPageArray.Length;
			for (int i = 0; i < num; i++)
			{
				int page = i;
				NKCUtil.SetToggleValueChangedDelegate(m_tglPageArray[i], delegate
				{
					OnClickPage(page);
				});
			}
		}
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMUnitData cNKMUnitData, List<NKMEquipPresetData> presetData, bool showFierceInfo)
	{
		if (cNKMUnitData == null || (presetData != null && presetData.Count <= 0) || (presetData == null && NKCEquipPresetDataManager.ListEquipPresetData == null))
		{
			return;
		}
		if (NKCPopupUnitInfoDetail.IsInstanceOpen)
		{
			NKCPopupUnitInfoDetail.Instance.Close();
		}
		m_doTweenVisualManager.onEnableBehaviour = OnEnableBehaviour.Restart;
		base.gameObject.SetActive(value: true);
		m_doTweenVisualManager.onEnableBehaviour = OnEnableBehaviour.None;
		if (!m_bInitLoopScroll)
		{
			m_LoopScrollRect.dOnGetObject += GetPresetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnPresetSlot;
			m_LoopScrollRect.dOnProvideData += ProvidePresetData;
			m_LoopScrollRect.ContentConstraintCount = 1;
			m_LoopScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScrollRect);
			m_bInitLoopScroll = true;
		}
		m_cNKMUnitData = cNKMUnitData;
		m_iNewPresetIndexFrom = presetData?.Count ?? NKCEquipPresetDataManager.ListEquipPresetData.Count;
		m_bShowFierceInfo = showFierceInfo;
		m_bPresetRemoveState = false;
		m_tglRemovePreset?.Select(m_bPresetRemoveState, bForce: true);
		NKCUtil.SetGameobjectActive(m_btnCancelRemoveMode, m_bPresetRemoveState);
		m_tglOrderEdit?.SetLock(m_bPresetRemoveState);
		m_tglPresetFilter?.SetLock(m_bPresetRemoveState);
		m_bPresetFilterState = false;
		m_tglRemovePreset?.SetLock(m_bPresetFilterState);
		m_tglOrderEdit?.SetLock(m_bPresetFilterState);
		m_tglPresetFilter?.Select(m_bPresetFilterState, bForce: true);
		m_bMoveSlotState = false;
		m_tglRemovePreset?.SetLock(m_bMoveSlotState);
		m_tglOrderEdit?.Select(m_bMoveSlotState, bForce: true);
		m_tglPresetFilter?.SetLock(m_bMoveSlotState);
		long num = NKCScenManager.CurrentUserData()?.m_UserUID ?? 0;
		if (m_userUID != num && m_tglPageArray != null)
		{
			m_iCurrentPage = 0;
			m_userUID = num;
			if (m_tglPageArray.Length != 0 && m_tglPageArray[0] != null)
			{
				m_tglPageArray[0].Select(bSelect: true, bForce: true);
			}
		}
		NKCEquipPresetDataManager.ListTempPresetData.Clear();
		UpdatePageLockState(m_iNewPresetIndexFrom);
		UpdatePresetData(presetData, presetData != null);
	}

	public void ChangeUnitData(NKMUnitData unitData)
	{
		m_cNKMUnitData = unitData;
		RefreshScrollRect(setScrollPosition: false);
		if (m_bPresetFilterState)
		{
			m_LoopScrollRect.Rebuild(CanvasUpdate.PostLayout);
		}
	}

	public void UpdatePresetData(List<NKMEquipPresetData> listPresetData, bool setScrollPositon, int scrollIndex = 0, bool forceRefresh = false)
	{
		if (listPresetData != null)
		{
			NKCEquipPresetDataManager.ListEquipPresetData = listPresetData;
		}
		UpdatePresetNumber();
		RefreshScrollRect(setScrollPositon, scrollIndex, forceRefresh);
	}

	public void UpdatePresetSlot(NKMEquipPresetData presetData, bool registerAll = false)
	{
		if (NKCEquipPresetDataManager.ListEquipPresetData.Count > presetData.presetIndex)
		{
			NKCEquipPresetDataManager.ListEquipPresetData[presetData.presetIndex] = presetData;
		}
		else
		{
			Debug.LogWarning("Updating Preset Slot Index over index range");
		}
		if (m_bMoveSlotState)
		{
			NKCEquipPresetDataManager.UpdateTempPresetSlotData(presetData);
		}
		NKCUIEquipPresetSlot.ShowSetItemFx = true;
		NKCUIEquipPresetSlot.SavedPreset = registerAll;
		RefreshScrollRect(setScrollPosition: false);
	}

	public void AddPresetSlot(int totalPresetCount)
	{
		if (NKCEquipPresetDataManager.ListEquipPresetData.Count >= totalPresetCount)
		{
			return;
		}
		int num = totalPresetCount - NKCEquipPresetDataManager.ListEquipPresetData.Count;
		m_iNewPresetIndexFrom = totalPresetCount - num;
		for (int i = 0; i < num; i++)
		{
			NKMEquipPresetData newEmptyPresetData = GetNewEmptyPresetData();
			newEmptyPresetData.presetIndex = m_iNewPresetIndexFrom + i;
			NKCEquipPresetDataManager.ListEquipPresetData.Add(newEmptyPresetData);
			if (m_bMoveSlotState)
			{
				NKCEquipPresetDataManager.ListTempPresetData.Add(newEmptyPresetData);
			}
		}
		UpdatePageLockState(NKCEquipPresetDataManager.ListEquipPresetData.Count);
		UpdatePresetData(null, setScrollPositon: false);
	}

	public void UpdatePresetName(int presetIndex, string presetName)
	{
		NKMEquipPresetData nKMEquipPresetData = NKCEquipPresetDataManager.ListEquipPresetData.Find((NKMEquipPresetData e) => e.presetIndex == presetIndex);
		if (nKMEquipPresetData != null)
		{
			nKMEquipPresetData.presetName = presetName;
			RefreshScrollRect(setScrollPosition: false);
		}
	}

	public List<long> GetListEquipUid(int presetIndex)
	{
		if (NKCEquipPresetDataManager.ListEquipPresetData.Count <= presetIndex)
		{
			return null;
		}
		return NKCEquipPresetDataManager.ListEquipPresetData[presetIndex].equipUids;
	}

	public bool IsOpened()
	{
		return base.gameObject.activeSelf;
	}

	public void CloseUI()
	{
		NKCEquipPresetDataManager.ListTempPresetData.Clear();
		NKCUIEquipPresetSlot.ResetUpdateIndex();
		base.gameObject.SetActive(value: false);
	}

	private void RefreshScrollRect(bool setScrollPosition, int scrollIndex = 0, bool forceRefresh = false)
	{
		NKCUtil.SetGameobjectActive(m_tglOrderEdit, !m_bPresetRemoveState);
		NKCUtil.SetGameobjectActive(m_tglPresetFilter, !m_bPresetRemoveState);
		NKCUtil.SetGameobjectActive(m_btnCancelRemoveMode, m_bPresetRemoveState);
		if (NKCEquipPresetDataManager.ListEquipPresetData == null || (!forceRefresh && !base.gameObject.activeSelf))
		{
			return;
		}
		m_listFilteredEquipPresetData.Clear();
		List<NKMEquipPresetData> presetDataListForPage = NKCEquipPresetDataManager.GetPresetDataListForPage(m_iCurrentPage, m_maxSlotCountPerPage, m_bMoveSlotState);
		if (m_bPresetFilterState)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_cNKMUnitData.m_UnitID);
			if (unitTempletBase == null)
			{
				return;
			}
			int count = presetDataListForPage.Count;
			for (int i = 0; i < count; i++)
			{
				NKM_EQUIP_PRESET_TYPE presetType = presetDataListForPage[i].presetType;
				bool flag = true;
				switch (presetType)
				{
				case NKM_EQUIP_PRESET_TYPE.NEPT_COUNTER:
					flag &= unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_COUNTER;
					break;
				case NKM_EQUIP_PRESET_TYPE.NEPT_SOLDIER:
					flag &= unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_SOLDIER;
					break;
				case NKM_EQUIP_PRESET_TYPE.NEPT_MECHANIC:
					flag &= unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_MECHANIC;
					break;
				}
				if (flag)
				{
					m_listFilteredEquipPresetData.Add(presetDataListForPage[i]);
				}
			}
		}
		else
		{
			m_listFilteredEquipPresetData.AddRange(presetDataListForPage);
		}
		NKCEquipPresetDataManager.RefreshEquipUidHash();
		m_LoopScrollRect.TotalCount = ((presetDataListForPage.Count >= m_maxSlotCountPerPage) ? m_listFilteredEquipPresetData.Count : Mathf.Min(m_listFilteredEquipPresetData.Count + 1, m_maxSlotCountPerPage));
		m_LoopScrollRect.StopMovement();
		if (setScrollPosition)
		{
			m_LoopScrollRect.SetIndexPosition(scrollIndex);
			if (forceRefresh)
			{
				m_LoopScrollRect.RefreshCells(forceRefresh);
			}
		}
		else
		{
			m_LoopScrollRect.RefreshCellsForDynamicTotalCount(forceRefresh);
		}
	}

	private RectTransform GetPresetSlot(int index)
	{
		return NKCUIEquipPresetSlot.GetNewInstance(null)?.GetComponent<RectTransform>();
	}

	private void UpdatePresetNumber()
	{
		if (base.gameObject.activeSelf)
		{
			NKCUtil.SetLabelText(m_lbPresetNumber, $"{NKCEquipPresetDataManager.ListEquipPresetData.Count}/{NKMCommonConst.EQUIP_PRESET_MAX_COUNT}");
		}
	}

	private void UpdatePageLockState(int showSlotCount)
	{
		if (m_tglPageArray == null)
		{
			return;
		}
		int num = m_tglPageArray.Length;
		for (int i = 0; i < num; i++)
		{
			if (!(m_tglPageArray[i] == null))
			{
				m_tglPageArray[i].SetLock(showSlotCount < m_maxSlotCountPerPage * i);
			}
		}
	}

	private void ReturnPresetSlot(Transform tr)
	{
		NKCUIEquipPresetSlot component = tr.GetComponent<NKCUIEquipPresetSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	private void ProvidePresetData(Transform tr, int index)
	{
		NKCUIEquipPresetSlot component = tr.GetComponent<NKCUIEquipPresetSlot>();
		if (component == null)
		{
			return;
		}
		if (m_listFilteredEquipPresetData.Count > index)
		{
			_ = m_listFilteredEquipPresetData[index].equipUids.Count;
			component.SetData(index, m_listFilteredEquipPresetData[index], this, m_iNewPresetIndexFrom, m_bShowFierceInfo, m_bMoveSlotState, m_bPresetRemoveState, m_LoopScrollRect, OnClickPresetAdd, OnClickSlotUp, OnClickSlotDown, OnToggleSlotCheck);
			if (index == m_listFilteredEquipPresetData.Count - 1 && NKCEquipPresetDataManager.IsLastPage(m_iCurrentPage, m_maxSlotCountPerPage))
			{
				m_iNewPresetIndexFrom = NKCEquipPresetDataManager.ListEquipPresetData.Count;
			}
		}
		else
		{
			component.SetData(NKCEquipPresetDataManager.ListEquipPresetData.Count, null, this, m_iNewPresetIndexFrom, m_bShowFierceInfo, m_bMoveSlotState, m_bPresetRemoveState, m_LoopScrollRect, OnClickPresetAdd);
		}
	}

	private void OnClickPresetAdd()
	{
		if (NKCEquipPresetDataManager.ListEquipPresetData.Count >= NKMCommonConst.EQUIP_PRESET_MAX_COUNT)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EQUIP_PRESET_SLOT_FULL);
			return;
		}
		NKCPopupInventoryAdd.SliderInfo sliderInfo = new NKCPopupInventoryAdd.SliderInfo
		{
			increaseCount = 1,
			maxCount = NKMCommonConst.EQUIP_PRESET_MAX_COUNT,
			currentCount = NKCEquipPresetDataManager.ListEquipPresetData.Count,
			inventoryType = NKM_INVENTORY_EXPAND_TYPE.NIET_NONE
		};
		NKCPopupInventoryAdd.Instance.Open(NKCUtilString.GET_STRING_EQUIP_PRESET_ADD_TITLE, NKCUtilString.GET_STRING_EQUIP_PRESET_ADD_CONTENT, sliderInfo, NKMCommonConst.EQUIP_PRESET_EXPAND_COST_VALUE, NKMCommonConst.EQUIP_PRESET_EXPAND_COST_ITEM_ID, delegate(int value)
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_ADD_REQ(value);
		});
	}

	private void OnClickInformation()
	{
		NKCUIPopUpGuide.Instance.Open(m_strGuideArticleID);
	}

	private void OnToggleRemovePreset(bool bValue)
	{
		if (bValue)
		{
			NKCEquipPresetDataManager.ResetRemoveTargetIndexList();
		}
		else if (NKCEquipPresetDataManager.GetRemoveTargetList().Count > 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EQUIP_PRESET_CLEAR_DESC, SendPresetClearReq, CancelClear);
			return;
		}
		m_bPresetRemoveState = bValue;
		RefreshScrollRect(setScrollPosition: false);
	}

	private void SendPresetClearReq()
	{
		m_bPresetRemoveState = false;
		NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_CLEAR_REQ(NKCEquipPresetDataManager.GetRemoveTargetList());
	}

	private void CancelClear()
	{
		m_tglRemovePreset.Select(bSelect: true, bForce: true);
	}

	private void OnTogglePresetFilter(bool value)
	{
		m_tglRemovePreset?.SetLock(value);
		m_tglOrderEdit?.SetLock(value);
		m_bPresetFilterState = value;
		RefreshScrollRect(value);
	}

	private void OnToggleOrderEdit(bool value)
	{
		m_tglRemovePreset?.SetLock(value);
		m_tglPresetFilter?.SetLock(value);
		m_bMoveSlotState = value;
		if (value)
		{
			NKCEquipPresetDataManager.ListTempPresetData.Clear();
			NKCEquipPresetDataManager.ListTempPresetData.AddRange(NKCEquipPresetDataManager.ListEquipPresetData);
			RefreshScrollRect(setScrollPosition: false);
			return;
		}
		List<NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ.PresetIndexData> movedSlotIndexList = NKCEquipPresetDataManager.GetMovedSlotIndexList();
		if (movedSlotIndexList.Count > 0)
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_CHANGE_INDEX_REQ(movedSlotIndexList);
		}
		else
		{
			RefreshScrollRect(setScrollPosition: false);
		}
		NKCEquipPresetDataManager.ListTempPresetData.Clear();
	}

	private void OnClickCancelRemoveMode()
	{
		NKCEquipPresetDataManager.ResetRemoveTargetIndexList();
		m_tglRemovePreset.Select(bSelect: false);
	}

	private void OnClickPage(int page)
	{
		m_iCurrentPage = page;
		RefreshScrollRect(setScrollPosition: true);
	}

	private void OnClickSlotUp(NKMEquipPresetData presetData)
	{
		int tempPresetDataIndex = NKCEquipPresetDataManager.GetTempPresetDataIndex(presetData);
		if (m_enableInterPageSlotMove || tempPresetDataIndex % m_maxSlotCountPerPage != 0)
		{
			NKCEquipPresetDataManager.SwapTempPresetData(tempPresetDataIndex - 1, tempPresetDataIndex);
			RefreshScrollRect(setScrollPosition: false);
		}
	}

	private void OnClickSlotDown(NKMEquipPresetData presetData)
	{
		int tempPresetDataIndex = NKCEquipPresetDataManager.GetTempPresetDataIndex(presetData);
		if (m_enableInterPageSlotMove || tempPresetDataIndex % m_maxSlotCountPerPage != m_maxSlotCountPerPage - 1)
		{
			NKCEquipPresetDataManager.SwapTempPresetData(tempPresetDataIndex, tempPresetDataIndex + 1);
			RefreshScrollRect(setScrollPosition: false);
		}
	}

	private void OnToggleSlotCheck(NKMEquipPresetData presetData, bool bValue)
	{
		if (bValue)
		{
			NKCEquipPresetDataManager.AddRemoveTargetIndex(presetData.presetIndex);
		}
		else
		{
			NKCEquipPresetDataManager.RemoveTargetIndex(presetData.presetIndex);
		}
	}

	public void OnClickClose()
	{
		CloseUI();
	}

	private void OnDestroy()
	{
		m_csbtnPresetAdd = null;
		m_csbtnClose = null;
		m_tglPresetFilter = null;
		m_LoopScrollRect = null;
		m_lbPresetNumber = null;
		m_listFilteredEquipPresetData?.Clear();
		m_listFilteredEquipPresetData = null;
		m_cNKMUnitData = null;
	}
}
