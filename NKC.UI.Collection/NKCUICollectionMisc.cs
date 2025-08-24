using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionMisc : MonoBehaviour
{
	public int minColumnCount = 5;

	private NKM_ITEM_MISC_TYPE m_Type;

	private NKCUICollectionGeneral.CollectionType m_eCTType;

	[Space]
	public NKCUICollectionMiscItemInfo m_miscItemInfo;

	public NKCUICollectionRate m_collectionRate;

	public GridLayoutGroup m_GridLayoutGroup;

	public LoopScrollRect m_LoopScrollRect;

	public NKCUIComUnitSortOptions m_unitSortOptions;

	[Header("\ufffdϰ\ufffd\ufffdϷ\ufffd \ufffd\ufffdư")]
	public NKCUIComStateButton m_RewardAllButton;

	public GameObject m_RewardAllRedDot;

	public GameObject m_RewardAllComplete;

	private List<int> m_IdList = new List<int>();

	private List<int> m_IdFilterList = new List<int>();

	private List<int> m_IdHaveList = new List<int>();

	private Dictionary<(NKM_REWARD_TYPE, int), int> m_rewardData = new Dictionary<(NKM_REWARD_TYPE, int), int>();

	private NKCUnitSortSystem m_unitSortSystem;

	private HashSet<NKCUnitSortSystem.eFilterCategory> m_filterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>();

	private HashSet<NKCUnitSortSystem.eSortCategory> m_sortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>();

	private NKCUIComDrag m_comDrag;

	private int m_selectedId;

	private Vector2 m_scrollRectOffsetMin;

	private Vector2 m_scrollRectOffsetMinUp;

	private float m_scrollPositionMaintain;

	private float m_miscItemInfoPanelY;

	private void Awake()
	{
		m_miscItemInfo.Close();
	}

	public void Init(NKM_ITEM_MISC_TYPE type, NKCUICollectionGeneral.CollectionType ctType)
	{
		m_Type = type;
		m_eCTType = ctType;
		m_miscItemInfo?.InitUI();
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetPresetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnPresetSlot;
			m_LoopScrollRect.dOnProvideData += ProvidePresetData;
			m_LoopScrollRect.dOnRepopulate += CalculateContentRectSize;
			m_LoopScrollRect.onValueChanged.AddListener(OnChangeScroll);
			CalculateContentRectSize();
			m_LoopScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_LoopScrollRect);
			m_scrollRectOffsetMin = m_LoopScrollRect.viewRect.offsetMin;
			m_comDrag = m_LoopScrollRect.GetComponent<NKCUIComDrag>();
			m_comDrag.BeginDrag.AddListener(OnDragBeginScrollRect);
			m_comDrag.Drag.AddListener(OnDraggingScrollRect);
			m_comDrag.EndDrag.AddListener(OnDragEndScrollRect);
		}
		m_filterCategory.Add(NKCUnitSortSystem.eFilterCategory.Have);
		m_sortCategory.Add(NKCUnitSortSystem.eSortCategory.IDX);
		NKCUnitSortSystem.UnitListOptions options = default(NKCUnitSortSystem.UnitListOptions);
		options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>();
		options.lstCustomSortFunc = new Dictionary<NKCUnitSortSystem.eSortCategory, KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>>();
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		string sortName = NKCUnitSortSystem.GetSortName(NKCUnitSortSystem.GetSortOptionByCategory(NKCUnitSortSystem.eSortCategory.IDX, bDescending: false));
		options.lstCustomSortFunc.Add(NKCUnitSortSystem.eSortCategory.Custom1, new KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>(sortName, null));
		options.bHideTokenFiltering = false;
		m_unitSortSystem = new NKCGenericUnitSort(NKCScenManager.CurrentUserData(), options, new List<NKMUnitData>());
		m_unitSortOptions.Init(OnSorted, bIsCollection: false);
		m_unitSortOptions.RegisterUnitSort(m_unitSortSystem);
		m_unitSortOptions.m_NKCPopupSort.m_bUseDefaultSortAdd = false;
		m_IdList = NKCCollectionManager.GetMiscIdList(m_Type);
		m_IdList.Sort(SortInit);
		NKCUtil.SetButtonClickDelegate(m_RewardAllButton, OnClickGetRewardAll);
		RectTransform component = m_miscItemInfo.GetComponent<RectTransform>();
		m_scrollRectOffsetMinUp = m_scrollRectOffsetMin;
		m_scrollRectOffsetMinUp.y += component.GetHeight() / 2f + component.anchoredPosition.y - m_scrollRectOffsetMin.y;
		m_miscItemInfoPanelY = m_miscItemInfo.transform.position.y;
	}

	public void Open()
	{
		m_selectedId = 0;
		m_miscItemInfo?.Close();
		m_LoopScrollRect.TotalCount = m_IdList.Count;
		m_IdFilterList.Clear();
		m_IdFilterList.AddRange(m_IdList);
		m_IdHaveList.Clear();
		m_IdHaveList = NKCCollectionManager.GetMiscHaveList(m_IdList);
		m_collectionRate?.SetData(m_eCTType, m_IdHaveList.Count, m_IdList.Count);
		ResetSortUI();
		UpdateReward(initScroll: true);
		m_LoopScrollRect.SetAutoResize(minColumnCount);
		ScrollRectBottomReturn();
	}

	public void UpdateList()
	{
		m_IdList = NKCCollectionManager.GetMiscIdList(m_Type);
		m_IdList.Sort(SortInit);
		m_miscItemInfo?.Close();
		m_IdHaveList.Clear();
		m_IdHaveList = NKCCollectionManager.GetMiscHaveList(m_IdList);
		m_collectionRate?.SetData(m_eCTType, m_IdHaveList.Count, m_IdList.Count);
		OnSorted(bResetScroll: false);
	}

	public void UpdateReward(bool initScroll = false)
	{
		if (m_miscItemInfo.IsOpened())
		{
			m_miscItemInfo?.UpdateRewardCompleted();
		}
		m_rewardData.Clear();
		foreach (int miscRewardEnabledId in NKCCollectionManager.GetMiscRewardEnabledIdList(m_IdHaveList))
		{
			NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet = NKMCollectionV2MiscTemplet.Find(miscRewardEnabledId);
			if (nKMCollectionV2MiscTemplet != null)
			{
				if (m_rewardData.ContainsKey((nKMCollectionV2MiscTemplet.RewardType, nKMCollectionV2MiscTemplet.RewardId)))
				{
					m_rewardData[(nKMCollectionV2MiscTemplet.RewardType, nKMCollectionV2MiscTemplet.RewardId)] += nKMCollectionV2MiscTemplet.RewardValue;
				}
				else
				{
					m_rewardData.Add((nKMCollectionV2MiscTemplet.RewardType, nKMCollectionV2MiscTemplet.RewardId), nKMCollectionV2MiscTemplet.RewardValue);
				}
			}
		}
		bool flag = m_rewardData.Count > 0;
		NKCUtil.SetGameobjectActive(m_RewardAllRedDot, flag);
		NKCUtil.SetGameobjectActive(m_RewardAllComplete, !flag);
		if (initScroll)
		{
			m_LoopScrollRect.SetIndexPosition(0);
		}
		else
		{
			m_LoopScrollRect.RefreshCells();
		}
	}

	public void RefreshScrollRect()
	{
		m_LoopScrollRect.RefreshCells();
	}

	public void Clear()
	{
		m_IdList?.Clear();
		m_IdList = null;
		m_IdFilterList?.Clear();
		m_IdFilterList = null;
		m_IdHaveList?.Clear();
		m_IdHaveList = null;
		m_rewardData?.Clear();
		m_rewardData = null;
		m_LoopScrollRect?.ClearCells();
		m_unitSortSystem = null;
		m_filterCategory?.Clear();
		m_filterCategory = null;
		m_sortCategory?.Clear();
		m_sortCategory = null;
	}

	private void ScrollRectBottomUp()
	{
		m_LoopScrollRect.viewRect.offsetMin = m_scrollRectOffsetMinUp;
	}

	private void ScrollRectBottomReturn()
	{
		m_LoopScrollRect.viewRect.offsetMin = m_scrollRectOffsetMin;
	}

	private void ResetSortUI()
	{
		m_unitSortSystem.Descending = false;
		m_unitSortSystem.FilterSet?.Clear();
		if (m_unitSortSystem.lstSortOption == null)
		{
			m_unitSortSystem.lstSortOption = new List<NKCUnitSortSystem.eSortOption>();
		}
		m_unitSortSystem.lstSortOption.Clear();
		m_unitSortSystem.lstSortOption.Add(NKCUnitSortSystem.eSortOption.CustomAscend1);
		m_unitSortOptions.RegisterCategories(m_filterCategory, m_sortCategory, bFavoriteFilterActive: false);
		m_unitSortOptions.ResetUI();
	}

	private void OnSorted(bool bResetScroll)
	{
		m_IdFilterList.Clear();
		m_IdFilterList.AddRange(m_IdList);
		if (m_unitSortSystem.lstSortOption == null || m_unitSortSystem.lstSortOption.Count <= 0 || m_unitSortSystem.lstSortOption[0] == NKCUnitSortSystem.eSortOption.None)
		{
			return;
		}
		bool flag = m_unitSortSystem.FilterSet.Contains(NKCUnitSortSystem.eFilterOption.Have);
		bool flag2 = m_unitSortSystem.FilterSet.Contains(NKCUnitSortSystem.eFilterOption.NotHave);
		if (flag || flag2)
		{
			NKMInventoryData nKMInventoryData = NKCScenManager.CurrentUserData()?.m_InventoryData;
			for (int num = m_IdFilterList.Count - 1; num >= 0; num--)
			{
				bool flag3 = nKMInventoryData == null || nKMInventoryData.GetCountMiscItem(m_IdList[num]) <= 0;
				bool flag4 = true;
				NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet = NKMCollectionV2MiscTemplet.Find(m_IdList[num]);
				if (flag3 && nKMCollectionV2MiscTemplet != null && nKMCollectionV2MiscTemplet.DefaultCollection)
				{
					flag3 = false;
				}
				if (flag && !flag3)
				{
					flag4 = false;
				}
				if (flag2 && flag3)
				{
					flag4 = false;
				}
				if (flag4)
				{
					m_IdFilterList.RemoveAt(num);
				}
			}
		}
		if (m_unitSortSystem.lstSortOption[0] == NKCUnitSortSystem.eSortOption.IDX_First || m_unitSortSystem.lstSortOption[0] == NKCUnitSortSystem.eSortOption.IDX_Last)
		{
			m_IdFilterList.Sort(SortId);
		}
		else
		{
			m_IdFilterList.Sort(SortInit);
		}
		m_LoopScrollRect.TotalCount = m_IdFilterList.Count;
		m_LoopScrollRect.SetIndexPosition(0);
	}

	private int SortId(int e1, int e2)
	{
		if (e1 > e2)
		{
			if (!m_unitSortSystem.Descending)
			{
				return 1;
			}
			return -1;
		}
		if (e1 < e2)
		{
			if (!m_unitSortSystem.Descending)
			{
				return -1;
			}
			return 1;
		}
		return 0;
	}

	private int SortInit(int e1, int e2)
	{
		NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet = NKMCollectionV2MiscTemplet.Find(e1);
		NKMCollectionV2MiscTemplet nKMCollectionV2MiscTemplet2 = NKMCollectionV2MiscTemplet.Find(e2);
		if (nKMCollectionV2MiscTemplet == null || nKMCollectionV2MiscTemplet2 == null)
		{
			return 0;
		}
		if (nKMCollectionV2MiscTemplet.SortIndex > nKMCollectionV2MiscTemplet2.SortIndex)
		{
			if (!m_unitSortSystem.Descending)
			{
				return 1;
			}
			return -1;
		}
		if (nKMCollectionV2MiscTemplet.SortIndex < nKMCollectionV2MiscTemplet2.SortIndex)
		{
			if (!m_unitSortSystem.Descending)
			{
				return -1;
			}
			return 1;
		}
		return 0;
	}

	private void OnClickSlot(int index, int itemId, RectTransform slotRT)
	{
		m_selectedId = itemId;
		m_LoopScrollRect.RefreshCells();
		float num = slotRT.transform.position.y - slotRT.GetHeight() / 2f;
		float num2 = slotRT.transform.position.y + slotRT.GetHeight() / 2f;
		float num3 = m_LoopScrollRect.viewRect.transform.position.y + m_LoopScrollRect.viewRect.GetHeight() / 2f;
		RectTransform component = m_miscItemInfo.GetComponent<RectTransform>();
		float num4 = m_miscItemInfoPanelY + component.GetHeight() / 2f;
		if (num < num4)
		{
			m_LoopScrollRect.MovePosition(new Vector2(0f, num4 - num));
			if (m_LoopScrollRect.normalizedPosition.y >= 1f)
			{
				ScrollRectBottomUp();
			}
		}
		if (num2 > num3)
		{
			m_LoopScrollRect.MovePosition(new Vector2(0f, num3 - num2));
		}
		m_miscItemInfo?.Open(itemId, m_eCTType, ScrollRectBottomReturn);
	}

	private RectTransform GetPresetSlot(int index)
	{
		return NKCUICollectionMiscItemSlot.GetNewInstance(null)?.GetComponent<RectTransform>();
	}

	private void ReturnPresetSlot(Transform tr)
	{
		NKCUICollectionMiscItemSlot component = tr.GetComponent<NKCUICollectionMiscItemSlot>();
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
		NKCUICollectionMiscItemSlot component = tr.GetComponent<NKCUICollectionMiscItemSlot>();
		if (!(component == null) && m_IdFilterList.Count > index)
		{
			NKMCollectionV2MiscTemplet templet = NKMCollectionV2MiscTemplet.Find(m_IdFilterList[index]);
			component.SetData(index, m_IdFilterList[index], templet, m_eCTType, OnClickSlot);
			component.SetSelected(m_selectedId == m_IdFilterList[index]);
		}
	}

	private void CalculateContentRectSize()
	{
		m_LoopScrollRect.SetAutoResize(minColumnCount);
	}

	private void OnClickGetRewardAll()
	{
		if (!m_RewardAllRedDot.activeSelf)
		{
			return;
		}
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		foreach (KeyValuePair<(NKM_REWARD_TYPE, int), int> rewardDatum in m_rewardData)
		{
			NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeRewardTypeData(rewardDatum.Key.Item1, rewardDatum.Key.Item2, rewardDatum.Value);
			list.Add(item);
		}
		if (list.Count > 0)
		{
			NKCUIPopupCollectionRewardConfirm.Instance.Open(list, delegate
			{
				NKCPacketSender.NKMPacket_MISC_COLLECTION_REWARD_ALL_REQ(m_Type);
			});
		}
	}

	private void OnDragBeginScrollRect(PointerEventData eventData)
	{
		if (m_miscItemInfo.IsOpened())
		{
			m_miscItemInfo.Close();
			if (m_LoopScrollRect.verticalNormalizedPosition > 1f)
			{
				m_scrollPositionMaintain = m_LoopScrollRect.verticalNormalizedPosition;
			}
		}
	}

	private void OnDraggingScrollRect(PointerEventData eventData)
	{
		if (m_scrollPositionMaintain > 0f)
		{
			m_LoopScrollRect.verticalNormalizedPosition = m_scrollPositionMaintain;
		}
	}

	private void OnDragEndScrollRect(PointerEventData eventData)
	{
		m_scrollPositionMaintain = -1f;
	}

	private void OnChangeScroll(Vector2 value)
	{
		if ((Input.mouseScrollDelta.y != 0f || NKCInputManager.IsHotkeyPressed(HotkeyEventType.Up) || NKCInputManager.IsHotkeyPressed(HotkeyEventType.Down)) && m_miscItemInfo.IsOpened())
		{
			m_miscItemInfo.Close();
		}
	}
}
