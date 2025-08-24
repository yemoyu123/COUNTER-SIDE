using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIChangeLobbyBackground : MonoBehaviour
{
	private enum Mode
	{
		Background,
		FlashBack
	}

	public delegate void OnChangeBackground(int itemID);

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComToggle m_tglNormal;

	public NKCUIComToggle m_tglFlashback;

	public LoopScrollRect m_srScroll;

	public NKCUISlot m_pfbSlot;

	public GameObject m_objEmptyFlashback;

	private List<int> m_lstBackgroundTempletID = new List<int>();

	private int m_currentBGID;

	private Mode m_eMode;

	private OnChangeBackground dOnChangeBackground;

	public void Init(OnChangeBackground onChangeBackground)
	{
		dOnChangeBackground = onChangeBackground;
		m_srScroll.dOnGetObject += GetObject;
		m_srScroll.dOnReturnObject += ReturnObject;
		m_srScroll.dOnProvideData += ProvideData;
		m_srScroll.PrepareCells();
		NKCUtil.SetScrollHotKey(m_srScroll);
		m_srScroll.SetAutoResize(4);
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, Close);
		NKCUtil.SetToggleValueChangedDelegate(m_tglNormal, OnTglNormal);
		NKCUtil.SetToggleValueChangedDelegate(m_tglFlashback, OnTglFlashback);
	}

	private void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void UpdateData(int selectedBG)
	{
		if (selectedBG == 0)
		{
			selectedBG = 9001;
		}
		m_currentBGID = selectedBG;
		SelectTab(m_eMode);
	}

	private void SelectTab(Mode mode)
	{
		m_eMode = mode;
		switch (m_eMode)
		{
		case Mode.Background:
			m_tglNormal.Select(bSelect: true, bForce: true);
			break;
		case Mode.FlashBack:
			m_tglFlashback.Select(bSelect: true, bForce: true);
			break;
		}
		BuildBackgroundList(mode);
		m_srScroll.TotalCount = m_lstBackgroundTempletID.Count;
		m_srScroll.SetIndexPosition(0);
		NKCUtil.SetGameobjectActive(m_objEmptyFlashback, m_eMode == Mode.FlashBack && m_lstBackgroundTempletID.Count == 0);
	}

	private void BuildBackgroundList(Mode mode)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKMInventoryData inventoryData = nKMUserData.m_InventoryData;
		m_lstBackgroundTempletID.Clear();
		foreach (NKCBackgroundTemplet value in NKMTempletContainer<NKCBackgroundTemplet>.Values)
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(value.m_ItemMiscID);
			if (nKMItemMiscTemplet == null)
			{
				continue;
			}
			NKM_ITEM_MISC_TYPE itemMiscType = nKMItemMiscTemplet.m_ItemMiscType;
			if (itemMiscType != NKM_ITEM_MISC_TYPE.IMT_BACKGROUND)
			{
				if (itemMiscType != NKM_ITEM_MISC_TYPE.IMT_INTERIOR || mode != Mode.Background)
				{
					continue;
				}
				NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMItemMiscTemplet.FindInterior(value.m_ItemMiscID);
				if (nKMOfficeInteriorTemplet == null || nKMOfficeInteriorTemplet.Target != InteriorTarget.Background || nKMUserData.OfficeData.GetInteriorCount(value.m_ItemMiscID) <= 0)
				{
					continue;
				}
			}
			else
			{
				if (value.m_ItemMiscID != 9001 && inventoryData.GetItemMisc(value.m_ItemMiscID) == null)
				{
					continue;
				}
				switch (mode)
				{
				case Mode.Background:
					if (value.IsFlashbackBG)
					{
						continue;
					}
					break;
				case Mode.FlashBack:
					if (!value.IsFlashbackBG)
					{
						continue;
					}
					break;
				}
			}
			m_lstBackgroundTempletID.Add(value.Key);
		}
	}

	private void OnTglNormal(bool value)
	{
		if (value)
		{
			SelectTab(Mode.Background);
		}
	}

	private void OnTglFlashback(bool value)
	{
		if (value)
		{
			SelectTab(Mode.FlashBack);
		}
	}

	private RectTransform GetObject(int index)
	{
		if (m_pfbSlot != null)
		{
			return Object.Instantiate(m_pfbSlot)?.GetComponent<RectTransform>();
		}
		return NKCUISlot.GetNewInstance(null)?.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		tr.SetParent(null);
		Object.Destroy(tr.gameObject);
	}

	private void ProvideData(Transform tr, int index)
	{
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		if (!(component == null))
		{
			NKCBackgroundTemplet nKCBackgroundTemplet = NKCBackgroundTemplet.Find(m_lstBackgroundTempletID[index]);
			component.SetMiscItemData(nKCBackgroundTemplet.m_ItemMiscID, 1L, bShowName: false, bShowCount: false, bEnableLayoutElement: true, OnTouchSlot);
			component.SetSelected(nKCBackgroundTemplet.m_ItemMiscID == m_currentBGID);
		}
	}

	private void OnTouchSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		int num = slotData.ID;
		if (num == 9001)
		{
			num = 0;
		}
		dOnChangeBackground(num);
	}
}
