using System;
using System.Collections.Generic;
using ClientPacket.Community;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCDeckViewSupportList : MonoBehaviour
{
	public class SupportData
	{
		public WarfareSupporterListData Data;

		public bool isGuest;

		public bool isGuild;
	}

	public delegate void OnSelectSlot();

	public delegate void OnConfirmBtn(long selectedCode);

	public LoopVerticalScrollRect m_scrollRect;

	public NKCUIDeckViewSupportSlot m_slotPrefab;

	public NKCUIComButton m_btnSelect;

	public NKCUIComButton m_btnSelect_off;

	public Text m_txtCount;

	[Header("정렬 옵션")]
	public NKCUIComToggle m_tgSortTypeMenu;

	public GameObject m_objSortSelect;

	public NKCPopupSort m_NKCPopupSort;

	public Text m_txtSortType;

	public Text m_txtSelectedSortType;

	private long m_selectedCode;

	private Stack<NKCUIDeckViewSupportSlot> m_slotPool = new Stack<NKCUIDeckViewSupportSlot>();

	private List<NKCUIDeckViewSupportSlot> m_slotList = new List<NKCUIDeckViewSupportSlot>();

	private List<SupportData> m_dataList = new List<SupportData>();

	private OnSelectSlot dOnSelectSlot;

	private OnConfirmBtn dOnConfirmBtn;

	private NKCUIDeckViewer.DeckViewerOption.IsValidSupport dIsValidSupport;

	private NKCUnitSortSystem.eSortOption m_currentSortOption = NKCUnitSortSystem.eSortOption.Power_High;

	public bool IsOpen { get; private set; }

	public void Init(NKCUIDeckViewer deckViewer, OnSelectSlot onSelectSlot, OnConfirmBtn onConfirmBtn)
	{
		IsOpen = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_scrollRect.dOnGetObject += GetSlot;
		m_scrollRect.dOnReturnObject += ReturnSlot;
		m_scrollRect.dOnProvideData += ProvideData;
		m_scrollRect.ContentConstraintCount = 1;
		m_scrollRect.PrepareCells();
		NKCUtil.SetScrollHotKey(m_scrollRect, deckViewer);
		dOnSelectSlot = onSelectSlot;
		dOnConfirmBtn = onConfirmBtn;
		m_btnSelect.PointerClick.RemoveAllListeners();
		m_btnSelect.PointerClick.AddListener(OnConfirmButton);
		m_tgSortTypeMenu.OnValueChanged.RemoveAllListeners();
		m_tgSortTypeMenu.OnValueChanged.AddListener(OnSortMenuOpen);
	}

	public void Open(List<WarfareSupporterListData> list, NKCUIDeckViewer.DeckViewerOption.IsValidSupport validSupport)
	{
		if (!IsOpen)
		{
			IsOpen = true;
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			dIsValidSupport = validSupport;
			m_dataList.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				WarfareSupporterListData warfareSupporterListData = list[i];
				bool isGuest = !NKCFriendManager.IsFriend(warfareSupporterListData.commonProfile.friendCode);
				bool isGuild = NKCGuildManager.IsGuildMember(warfareSupporterListData.commonProfile.friendCode);
				SupportData item = new SupportData
				{
					Data = warfareSupporterListData,
					isGuest = isGuest,
					isGuild = isGuild
				};
				m_dataList.Add(item);
			}
			m_scrollRect.TotalCount = m_dataList.Count;
			m_scrollRect.SetIndexPosition(0);
			m_scrollRect.RefreshCells(bForce: true);
			RefreshList();
			m_txtCount.text = $"{m_dataList.Count}/{60}";
			if (m_selectedCode == 0L && m_dataList.Count > 0)
			{
				m_selectedCode = m_dataList[0].Data.commonProfile.friendCode;
			}
		}
	}

	public void Close()
	{
		if (IsOpen)
		{
			IsOpen = false;
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
	}

	public void Clear()
	{
		m_selectedCode = 0L;
		m_dataList.Clear();
	}

	public WarfareSupporterListData GetSelectedData()
	{
		return m_dataList.Find((SupportData v) => v.Data.commonProfile.friendCode == m_selectedCode)?.Data;
	}

	public void UpdateSelectUI()
	{
		for (int i = 0; i < m_slotList.Count; i++)
		{
			NKCUIDeckViewSupportSlot nKCUIDeckViewSupportSlot = m_slotList[i];
			nKCUIDeckViewSupportSlot.SelectUI(nKCUIDeckViewSupportSlot.SuppoterCode == m_selectedCode);
		}
		UpdateButtonUI();
	}

	private void UpdateButtonUI()
	{
		WarfareSupporterListData selectedData = GetSelectedData();
		bool flag = false;
		if (selectedData != null)
		{
			flag = !IsCooltime(selectedData);
		}
		bool flag2 = m_dataList.Count > 0;
		bool flag3 = (dIsValidSupport?.Invoke(m_selectedCode) ?? true) && flag && flag2;
		NKCUtil.SetGameobjectActive(m_btnSelect, flag3);
		NKCUtil.SetGameobjectActive(m_btnSelect_off, !flag3);
	}

	private void SelectSlot(long selectedCode)
	{
		m_selectedCode = selectedCode;
		dOnSelectSlot?.Invoke();
	}

	private void OnConfirmButton()
	{
		dOnConfirmBtn?.Invoke(m_selectedCode);
	}

	private bool IsCooltime(WarfareSupporterListData friend)
	{
		TimeSpan timeSpan = NKCSynchronizedTime.GetServerUTCTime() - friend.lastUsedDate;
		return (TimeSpan.FromHours(12.0) - timeSpan).TotalSeconds > 0.0;
	}

	private void RefreshList()
	{
		NKCUtil.SetLabelText(m_txtSortType, NKCUnitSortSystem.GetSortName(m_currentSortOption));
		NKCUtil.SetLabelText(m_txtSelectedSortType, NKCUnitSortSystem.GetSortName(m_currentSortOption));
		m_dataList.Sort(Compare);
		m_scrollRect.RefreshCells();
	}

	private RectTransform GetSlot(int index)
	{
		if (m_slotPool.Count > 0)
		{
			NKCUIDeckViewSupportSlot nKCUIDeckViewSupportSlot = m_slotPool.Pop();
			NKCUtil.SetGameobjectActive(nKCUIDeckViewSupportSlot, bValue: true);
			RectTransform component = nKCUIDeckViewSupportSlot.GetComponent<RectTransform>();
			m_slotList.Add(nKCUIDeckViewSupportSlot);
			return component;
		}
		NKCUIDeckViewSupportSlot nKCUIDeckViewSupportSlot2 = UnityEngine.Object.Instantiate(m_slotPrefab);
		nKCUIDeckViewSupportSlot2.transform.SetParent(m_scrollRect.content);
		nKCUIDeckViewSupportSlot2.Init();
		NKCUtil.SetGameobjectActive(nKCUIDeckViewSupportSlot2, bValue: true);
		RectTransform component2 = nKCUIDeckViewSupportSlot2.GetComponent<RectTransform>();
		component2.localScale = Vector3.one;
		m_slotList.Add(nKCUIDeckViewSupportSlot2);
		return component2;
	}

	private void ReturnSlot(Transform trans)
	{
		NKCUtil.SetGameobjectActive(trans, bValue: false);
		NKCUIDeckViewSupportSlot component = trans.GetComponent<NKCUIDeckViewSupportSlot>();
		m_slotList.Remove(component);
		m_slotPool.Push(component);
		trans.SetParent(base.transform);
	}

	private void ProvideData(Transform trans, int index)
	{
		NKCUIDeckViewSupportSlot component = trans.GetComponent<NKCUIDeckViewSupportSlot>();
		SupportData supportData = m_dataList[index];
		component.SetData(supportData.Data, supportData.isGuest, SelectSlot);
		component.SelectUI(supportData.Data.commonProfile.friendCode == m_selectedCode);
		NKCUtil.SetGameobjectActive(component, bValue: true);
	}

	private void OnSortMenuOpen(bool bValue)
	{
		m_NKCPopupSort.OpenSquadSortMenu(m_currentSortOption, OnSort, bValue);
		SetOpenSortingMenu(bValue);
	}

	private void OnSort(List<NKCUnitSortSystem.eSortOption> sortOptionList)
	{
		m_currentSortOption = sortOptionList[0];
		SetOpenSortingMenu(bOpen: false);
		RefreshList();
		UpdateSelectUI();
	}

	public void SetOpenSortingMenu(bool bOpen, bool bAnimate = true)
	{
		m_tgSortTypeMenu.Select(bOpen, bForce: true);
		m_NKCPopupSort.StartRectMove(bOpen, bAnimate);
	}

	private int Compare(SupportData spt_a, SupportData spt_b)
	{
		if (spt_a.isGuest != spt_b.isGuest)
		{
			return spt_a.isGuest.CompareTo(spt_b.isGuest);
		}
		if (spt_a.isGuild != spt_b.isGuild)
		{
			return spt_b.isGuild.CompareTo(spt_a.isGuild);
		}
		WarfareSupporterListData data = spt_a.Data;
		WarfareSupporterListData data2 = spt_b.Data;
		bool flag = !IsCooltime(data);
		bool flag2 = !IsCooltime(data2);
		if (flag != flag2)
		{
			return flag2.CompareTo(flag);
		}
		if (m_currentSortOption == NKCUnitSortSystem.eSortOption.Player_Level_High || m_currentSortOption == NKCUnitSortSystem.eSortOption.Player_Level_Low)
		{
			if (data.commonProfile.level != data2.commonProfile.level)
			{
				return data2.commonProfile.level.CompareTo(data.commonProfile.level);
			}
		}
		else if ((m_currentSortOption == NKCUnitSortSystem.eSortOption.LoginTime_Latly || m_currentSortOption == NKCUnitSortSystem.eSortOption.LoginTime_Old) && data.lastLoginDate != data2.lastLoginDate)
		{
			return data2.lastLoginDate.CompareTo(data.lastLoginDate);
		}
		float num = data.deckData.CalculateOperationPower();
		float num2 = data2.deckData.CalculateOperationPower();
		if (num != num2)
		{
			return num2.CompareTo(num);
		}
		return data.commonProfile.friendCode.CompareTo(data2.commonProfile.friendCode);
	}
}
