using System.Collections.Generic;
using ClientPacket.Common;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletBanList : NKCUIBase
{
	public class CompNKMUnitUpData : IComparer<NKMUnitUpData>
	{
		public int Compare(NKMUnitUpData x, NKMUnitUpData y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (y.upLevel > x.upLevel)
			{
				return 1;
			}
			if (y.upLevel < x.upLevel)
			{
				return -1;
			}
			return 0;
		}
	}

	public class CompNKMBanData : IComparer<NKMBanData>
	{
		public int Compare(NKMBanData x, NKMBanData y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (y.m_BanLevel > x.m_BanLevel)
			{
				return 1;
			}
			if (y.m_BanLevel < x.m_BanLevel)
			{
				return -1;
			}
			return 0;
		}
	}

	public class CompNKMBanShipData : IComparer<NKMBanShipData>
	{
		public int Compare(NKMBanShipData x, NKMBanShipData y)
		{
			if (x == null)
			{
				return 1;
			}
			if (y == null)
			{
				return -1;
			}
			if (y.m_BanLevel > x.m_BanLevel)
			{
				return 1;
			}
			if (y.m_BanLevel < x.m_BanLevel)
			{
				return -1;
			}
			return 0;
		}
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_POPUP_BANNED_LIST";

	private static NKCPopupGauntletBanList m_Instance;

	[Header("공통")]
	public EventTrigger m_etBG;

	public NKCUIComStateButton m_csbtnClose;

	[Header("상단")]
	public NKCUIComStateButton m_csbtnGuide;

	public Text m_lbSubTitle;

	[Header("왼쪽")]
	public NKCUIComToggle m_ctglUnit;

	public NKCUIComToggle m_ctglShip;

	public NKCUIComToggle m_ctglUnitUp;

	[Header("오른쪽")]
	public LoopVerticalScrollRect m_lvsrUnit;

	public LoopVerticalScrollRect m_lvsrShip;

	public LoopVerticalScrollRect m_lvsrUnitUp;

	public GameObject m_objUnitList;

	public GameObject m_objShipList;

	public GameObject m_objUnitListUp;

	public NKCUIUnitSelectListSlot m_pfbUnitSlotForBan;

	public NKCUIShipSelectListSlot m_pfbShipSlotForBan;

	private List<NKMBanData> m_lstNKMBanData = new List<NKMBanData>();

	private List<NKMBanShipData> m_lstNKMBanShipData = new List<NKMBanShipData>();

	private List<NKMUnitUpData> m_lstNKMUnitUpData = new List<NKMUnitUpData>();

	private Dictionary<int, NKMUnitTempletBase> m_dicUTB_ByShipGroupID = new Dictionary<int, NKMUnitTempletBase>();

	private bool m_bFirstOpen = true;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupGauntletBanList";

	public static NKCPopupGauntletBanList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGauntletBanList>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_POPUP_BANNED_LIST", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGauntletBanList>();
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

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_etBG.triggers.Clear();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		NKCUtil.SetBindFunction(m_csbtnClose, base.Close);
		m_ctglUnit.OnValueChanged.RemoveAllListeners();
		m_ctglUnit.OnValueChanged.AddListener(OnValueChangedUnit);
		m_ctglShip.OnValueChanged.RemoveAllListeners();
		m_ctglShip.OnValueChanged.AddListener(OnValueChangedShip);
		m_ctglUnitUp.OnValueChanged.RemoveAllListeners();
		m_ctglUnitUp.OnValueChanged.AddListener(OnValueChangedUnitUp);
		m_lvsrUnit.dOnGetObject += GetUnitSlot;
		m_lvsrUnit.dOnReturnObject += ReturnUnitSlot;
		m_lvsrUnit.dOnProvideData += ProvideUnitSlotData;
		NKCUtil.SetScrollHotKey(m_lvsrUnit);
		m_lvsrShip.dOnGetObject += GetShipSlot;
		m_lvsrShip.dOnReturnObject += ReturnShipSlot;
		m_lvsrShip.dOnProvideData += ProvideShipSlotData;
		NKCUtil.SetScrollHotKey(m_lvsrShip);
		m_lvsrUnitUp.dOnGetObject += GetUnitSlot;
		m_lvsrUnitUp.dOnReturnObject += ReturnUnitSlot;
		m_lvsrUnitUp.dOnProvideData += ProvideUnitSlotDataUp;
		NKCUtil.SetScrollHotKey(m_lvsrUnitUp);
		NKCUtil.SetBindFunction(m_csbtnGuide, OnClickGuide);
	}

	private void OnClickGuide()
	{
		NKCUIPopUpGuide.Instance.Open("ARTICLE_PVP_BANLIST");
	}

	private RectTransform GetUnitSlot(int index)
	{
		NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = Object.Instantiate(m_pfbUnitSlotForBan);
		nKCUIUnitSelectListSlot.Init();
		NKCUtil.SetGameobjectActive(nKCUIUnitSelectListSlot, bValue: true);
		nKCUIUnitSelectListSlot.transform.localScale = Vector3.one;
		return nKCUIUnitSelectListSlot.GetComponent<RectTransform>();
	}

	private void ReturnUnitSlot(Transform go)
	{
		go.SetParent(base.transform);
		Object.Destroy(go.gameObject);
	}

	private void ProvideUnitSlotData(Transform tr, int idx)
	{
		NKCUIUnitSelectListSlotBase component = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (!(component == null))
		{
			if (idx < 0 || idx >= m_lstNKMBanData.Count)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(component, bValue: true);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_lstNKMBanData[idx].m_UnitID);
			component.SetEnableShowBan(bSet: true);
			component.SetDataForBan(unitTempletBase, bEnableLayoutElement: true, null);
			component.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		}
	}

	private void ProvideUnitSlotDataUp(Transform tr, int idx)
	{
		NKCUIUnitSelectListSlotBase component = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (!(component == null))
		{
			if (idx < 0 || idx >= m_lstNKMUnitUpData.Count)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(component, bValue: true);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_lstNKMUnitUpData[idx].unitId);
			component.SetEnableShowBan(bSet: true);
			component.SetDataForBan(unitTempletBase, bEnableLayoutElement: true, null, bUp: true);
			component.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		}
	}

	private RectTransform GetShipSlot(int index)
	{
		NKCUIShipSelectListSlot nKCUIShipSelectListSlot = Object.Instantiate(m_pfbShipSlotForBan);
		nKCUIShipSelectListSlot.Init();
		NKCUtil.SetGameobjectActive(nKCUIShipSelectListSlot, bValue: true);
		nKCUIShipSelectListSlot.transform.localScale = Vector3.one;
		return nKCUIShipSelectListSlot.GetComponent<RectTransform>();
	}

	private void ReturnShipSlot(Transform go)
	{
		go.SetParent(base.transform);
		Object.Destroy(go.gameObject);
	}

	private void ProvideShipSlotData(Transform tr, int idx)
	{
		NKCUIUnitSelectListSlotBase component = tr.GetComponent<NKCUIUnitSelectListSlotBase>();
		if (component == null)
		{
			return;
		}
		if (idx < 0 || idx >= m_lstNKMBanShipData.Count)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(component, bValue: true);
		NKMUnitTempletBase value = null;
		int shipGroupID = m_lstNKMBanShipData[idx].m_ShipGroupID;
		if (!m_dicUTB_ByShipGroupID.TryGetValue(shipGroupID, out value))
		{
			value = NKMUnitManager.GetUnitTempletBaseByShipGroupID(shipGroupID);
			m_dicUTB_ByShipGroupID.Add(shipGroupID, value);
		}
		component.SetEnableShowBan(bSet: true);
		component.SetDataForBan(value, bEnableLayoutElement: true, null);
	}

	private void OnValueChangedUnit(bool bSet)
	{
		if (bSet)
		{
			NKCUtil.SetGameobjectActive(m_objUnitList, bValue: true);
			NKCUtil.SetGameobjectActive(m_objShipList, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitListUp, bValue: false);
			m_lvsrUnit.RefreshCells();
			NKCUtil.SetLabelText(m_lbSubTitle, NKCStringTable.GetString("SI_DP_GAUNTLET_BAN_POPUP_DESC_UNIT"));
		}
	}

	private void OnValueChangedShip(bool bSet)
	{
		if (bSet)
		{
			NKCUtil.SetGameobjectActive(m_objUnitList, bValue: false);
			NKCUtil.SetGameobjectActive(m_objShipList, bValue: true);
			NKCUtil.SetGameobjectActive(m_objUnitListUp, bValue: false);
			m_lvsrShip.RefreshCells();
			NKCUtil.SetLabelText(m_lbSubTitle, NKCStringTable.GetString("SI_DP_GAUNTLET_BAN_POPUP_DESC_SHIP"));
		}
	}

	private void OnValueChangedUnitUp(bool bSet)
	{
		if (bSet)
		{
			NKCUtil.SetGameobjectActive(m_objUnitList, bValue: false);
			NKCUtil.SetGameobjectActive(m_objShipList, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitListUp, bValue: true);
			m_lvsrUnitUp.RefreshCells();
			NKCUtil.SetLabelText(m_lbSubTitle, NKCStringTable.GetString("SI_DP_GAUNTLET_UP_POPUP_DESC_UNIT"));
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open()
	{
		UIOpened();
		SetUIWhenOpen();
	}

	private void ResetData()
	{
		m_lstNKMBanData = new List<NKMBanData>(NKCBanManager.GetBanData(NKCBanManager.BAN_DATA_TYPE.ROTATION).Values);
		m_lstNKMBanData.Sort(new CompNKMBanData());
		m_lvsrUnit.TotalCount = m_lstNKMBanData.Count;
		m_lstNKMBanShipData = new List<NKMBanShipData>(NKCBanManager.GetBanDataShip(NKCBanManager.BAN_DATA_TYPE.ROTATION).Values);
		m_lstNKMBanShipData.Sort(new CompNKMBanShipData());
		m_lvsrShip.TotalCount = m_lstNKMBanShipData.Count;
		m_lstNKMUnitUpData = new List<NKMUnitUpData>(NKCBanManager.m_dicNKMUpData.Values);
		m_lstNKMUnitUpData.Sort(new CompNKMUnitUpData());
		m_lvsrUnitUp.TotalCount = m_lstNKMUnitUpData.Count;
		m_dicUTB_ByShipGroupID.Clear();
	}

	public void OnChangedBanList()
	{
		ResetData();
		m_lvsrUnit.SetIndexPosition(0);
		m_lvsrShip.SetIndexPosition(0);
		m_lvsrUnitUp.SetIndexPosition(0);
	}

	public void SetUIWhenOpen()
	{
		if (m_bFirstOpen)
		{
			NKCUtil.SetGameobjectActive(m_objUnitList, bValue: true);
			NKCUtil.SetGameobjectActive(m_objShipList, bValue: true);
			NKCUtil.SetGameobjectActive(m_objUnitListUp, bValue: true);
			m_lvsrUnit.PrepareCells();
			m_lvsrShip.PrepareCells();
			m_lvsrUnitUp.PrepareCells();
			ResetData();
			m_bFirstOpen = false;
		}
		m_lvsrUnit.SetIndexPosition(0);
		m_lvsrShip.SetIndexPosition(0);
		m_lvsrUnitUp.SetIndexPosition(0);
		m_ctglUnit.Select(bSelect: false, bForce: true);
		m_ctglUnit.Select(bSelect: true);
	}
}
