using System.Collections.Generic;
using ClientPacket.Common;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletBanListV2 : NKCUIBase
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

	public class CompNKMBanOperData : IComparer<NKMBanOperatorData>
	{
		public int Compare(NKMBanOperatorData x, NKMBanOperatorData y)
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

	private enum TAB_TYPE
	{
		TAB_NONE,
		TAB_FINAL,
		TAB_ROTATION,
		TAB_CASTING,
		TAB_UP
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_POPUP_BANNED_LIST_NEW";

	private static NKCPopupGauntletBanListV2 m_Instance;

	[Header("공통")]
	public EventTrigger m_etBG;

	public NKCUIComStateButton m_csbtnClose;

	[Header("상단")]
	public NKCUIComStateButton m_csbtnGuide;

	public Text m_lbSubTitle;

	[Header("왼쪽")]
	public NKCUIComToggle m_ctglFinalBan;

	public NKCUIComToggle m_ctglRotationBan;

	public NKCUIComToggle m_ctglCastingBan;

	public NKCUIComToggle m_ctglUpUnit;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotFinalUnit;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotFinalShip;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotFinalOper;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotRotationUnit;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotRotationShip;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotRotationOper;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotCastingUnit;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotCastingShip;

	public NKCUIPopupGuideSubSlot m_GuideSubSlotCastingOper;

	[Header("오른쪽")]
	public LoopVerticalScrollRect m_lvsrUnit;

	public LoopVerticalScrollRect m_lvsrShip;

	public LoopVerticalScrollRect m_lvsrOper;

	public GameObject m_objUnitList;

	public GameObject m_objShipList;

	public GameObject m_objOperList;

	public NKCUIUnitSelectListSlot m_pfbUnitSlotForBan;

	public NKCUIShipSelectListSlot m_pfbShipSlotForBan;

	public NKCUIOperatorSelectListSlot m_pfbOperSlotForBan;

	private List<NKMBanData> m_lstNKMBanData = new List<NKMBanData>();

	private List<NKMBanShipData> m_lstNKMBanShipData = new List<NKMBanShipData>();

	private List<NKMBanOperatorData> m_lstNKMBanOperatorData = new List<NKMBanOperatorData>();

	private List<NKMUnitUpData> m_lstNKMUnitUpData = new List<NKMUnitUpData>();

	private Dictionary<int, NKMUnitTempletBase> m_dicUTB_ByShipGroupID = new Dictionary<int, NKMUnitTempletBase>();

	private const string UNIT = "UNIT";

	private const string SHIP = "SHIP";

	private const string OPER = "OPER";

	private Dictionary<TAB_TYPE, List<NKCUIPopupGuideSubSlot>> m_dicCastingBan = new Dictionary<TAB_TYPE, List<NKCUIPopupGuideSubSlot>>();

	private TAB_TYPE m_curTabType;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupGauntletBanList";

	public static NKCPopupGauntletBanListV2 Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGauntletBanListV2>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_POPUP_BANNED_LIST_NEW", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGauntletBanListV2>();
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
		NKCUtil.SetEventTriggerDelegate(m_etBG, base.Close);
		NKCUtil.SetBindFunction(m_csbtnClose, base.Close);
		m_lvsrUnit.dOnGetObject += GetUnitSlot;
		m_lvsrUnit.dOnReturnObject += ReturnUnitSlot;
		m_lvsrUnit.dOnProvideData += ProvideUnitSlotData;
		NKCUtil.SetScrollHotKey(m_lvsrUnit);
		m_lvsrShip.dOnGetObject += GetShipSlot;
		m_lvsrShip.dOnReturnObject += ReturnShipSlot;
		m_lvsrShip.dOnProvideData += ProvideShipSlotData;
		NKCUtil.SetScrollHotKey(m_lvsrShip);
		m_lvsrOper.dOnGetObject += GetOperSlot;
		m_lvsrOper.dOnReturnObject += ReturnOperSlot;
		m_lvsrOper.dOnProvideData += ProvideOperSlotData;
		NKCUtil.SetScrollHotKey(m_lvsrOper);
		m_lvsrUnit.PrepareCells();
		m_lvsrShip.PrepareCells();
		m_lvsrOper.PrepareCells();
		NKCUtil.SetBindFunction(m_csbtnGuide, OnClickGuide);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglFinalBan, delegate
		{
			OnClickTab(TAB_TYPE.TAB_FINAL);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglUpUnit, delegate
		{
			OnClickTab(TAB_TYPE.TAB_UP);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglCastingBan, delegate
		{
			OnClickTab(TAB_TYPE.TAB_CASTING);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_ctglRotationBan, delegate
		{
			OnClickTab(TAB_TYPE.TAB_ROTATION);
		});
		InitLeftMenu();
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
		if (component == null)
		{
			return;
		}
		if (idx < 0 && ((m_curTabType == TAB_TYPE.TAB_UP && idx >= m_lstNKMUnitUpData.Count) || idx >= m_lstNKMUnitUpData.Count))
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			return;
		}
		int unitID = ((m_curTabType == TAB_TYPE.TAB_UP) ? m_lstNKMUnitUpData[idx].unitId : m_lstNKMBanData[idx].m_UnitID);
		NKCUtil.SetGameobjectActive(component, bValue: true);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		component.SetEnableShowBan(bSet: true);
		switch (m_curTabType)
		{
		case TAB_TYPE.TAB_FINAL:
			component.SetBanDataType(NKCBanManager.BAN_DATA_TYPE.FINAL);
			break;
		case TAB_TYPE.TAB_CASTING:
			component.SetBanDataType(NKCBanManager.BAN_DATA_TYPE.CASTING);
			break;
		default:
			component.SetBanDataType(NKCBanManager.BAN_DATA_TYPE.ROTATION);
			break;
		}
		component.SetDataForBan(unitTempletBase, bEnableLayoutElement: true, null, m_curTabType == TAB_TYPE.TAB_UP);
		component.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
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
		switch (m_curTabType)
		{
		case TAB_TYPE.TAB_FINAL:
			component.SetBanDataType(NKCBanManager.BAN_DATA_TYPE.FINAL);
			break;
		case TAB_TYPE.TAB_CASTING:
			component.SetBanDataType(NKCBanManager.BAN_DATA_TYPE.CASTING);
			break;
		default:
			component.SetBanDataType(NKCBanManager.BAN_DATA_TYPE.ROTATION);
			break;
		}
		component.SetEnableShowBan(bSet: true);
		component.SetDataForBan(value, bEnableLayoutElement: true, null);
	}

	private RectTransform GetOperSlot(int index)
	{
		NKCUIOperatorSelectListSlot nKCUIOperatorSelectListSlot = Object.Instantiate(m_pfbOperSlotForBan);
		nKCUIOperatorSelectListSlot.Init();
		NKCUtil.SetGameobjectActive(nKCUIOperatorSelectListSlot, bValue: true);
		nKCUIOperatorSelectListSlot.transform.localScale = Vector3.one;
		return nKCUIOperatorSelectListSlot.GetComponent<RectTransform>();
	}

	private void ReturnOperSlot(Transform go)
	{
		go.SetParent(base.transform);
		Object.Destroy(go.gameObject);
	}

	private void ProvideOperSlotData(Transform tr, int idx)
	{
		NKCUIOperatorSelectListSlot component = tr.GetComponent<NKCUIOperatorSelectListSlot>();
		if (component == null)
		{
			return;
		}
		if (idx < 0 || idx >= m_lstNKMBanOperatorData.Count)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			return;
		}
		int operatorID = m_lstNKMBanOperatorData[idx].m_OperatorID;
		NKCUtil.SetGameobjectActive(component, bValue: true);
		NKMUnitManager.GetUnitTempletBase(operatorID);
		component.SetEnableShowBan(bSet: true);
		switch (m_curTabType)
		{
		case TAB_TYPE.TAB_FINAL:
			component.SetBanDataType(NKCBanManager.BAN_DATA_TYPE.FINAL);
			break;
		case TAB_TYPE.TAB_CASTING:
			component.SetBanDataType(NKCBanManager.BAN_DATA_TYPE.CASTING);
			break;
		default:
			component.SetBanDataType(NKCBanManager.BAN_DATA_TYPE.ROTATION);
			break;
		}
		component.SetDataForBan(NKCOperatorUtil.GetDummyOperator(m_lstNKMBanOperatorData[idx].m_OperatorID), bEnableLayoutElement: true, null);
		component.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open()
	{
		UIOpened();
		m_ctglFinalBan.Select(bSelect: true, bForce: true);
		OnClickTab(TAB_TYPE.TAB_FINAL, bForce: true);
	}

	private void InitLeftMenu()
	{
		string title = NKCStringTable.GetString("SI_PVP_POPUP_BAN_LIST_MENU_BAN_UNIT");
		string title2 = NKCStringTable.GetString("SI_PVP_POPUP_BAN_LIST_MENU_BAN_SHIP");
		string title3 = NKCStringTable.GetString("SI_PVP_POPUP_BAN_LIST_MENU_BAN_OPER");
		bool num = NKCOperatorUtil.IsActiveCastingBan();
		m_GuideSubSlotFinalUnit.Init(title, "UNIT", OnClickedSubSlot);
		m_GuideSubSlotFinalShip.Init(title2, "SHIP", OnClickedSubSlot);
		m_GuideSubSlotCastingUnit.Init(title, "UNIT", OnClickedSubSlot);
		m_GuideSubSlotCastingShip.Init(title2, "SHIP", OnClickedSubSlot);
		m_GuideSubSlotRotationUnit.Init(title, "UNIT", OnClickedSubSlot);
		m_GuideSubSlotRotationShip.Init(title2, "SHIP", OnClickedSubSlot);
		List<NKCUIPopupGuideSubSlot> list = new List<NKCUIPopupGuideSubSlot> { m_GuideSubSlotFinalUnit, m_GuideSubSlotFinalShip };
		List<NKCUIPopupGuideSubSlot> list2 = new List<NKCUIPopupGuideSubSlot> { m_GuideSubSlotCastingUnit, m_GuideSubSlotCastingShip };
		List<NKCUIPopupGuideSubSlot> list3 = new List<NKCUIPopupGuideSubSlot> { m_GuideSubSlotRotationUnit, m_GuideSubSlotRotationShip };
		if (num)
		{
			m_GuideSubSlotFinalOper.Init(title3, "OPER", OnClickedSubSlot);
			m_GuideSubSlotCastingOper.Init(title3, "OPER", OnClickedSubSlot);
			m_GuideSubSlotRotationOper.Init(title3, "OPER", OnClickedSubSlot);
			list.Add(m_GuideSubSlotFinalOper);
			list2.Add(m_GuideSubSlotCastingOper);
			list3.Add(m_GuideSubSlotRotationOper);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_GuideSubSlotFinalOper.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_GuideSubSlotCastingOper.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_GuideSubSlotRotationOper.gameObject, bValue: false);
		}
		m_dicCastingBan.Add(TAB_TYPE.TAB_FINAL, list);
		m_dicCastingBan.Add(TAB_TYPE.TAB_CASTING, list2);
		m_dicCastingBan.Add(TAB_TYPE.TAB_ROTATION, list3);
	}

	private void OnClickTab(TAB_TYPE tabType, bool bForce = false)
	{
		if (m_curTabType != tabType || bForce)
		{
			m_curTabType = tabType;
			OnClickedSubSlot("UNIT");
		}
	}

	public void OnClickedSubSlot(string ArticleID, int i = 0)
	{
		if (m_curTabType == TAB_TYPE.TAB_UP)
		{
			NKCUtil.SetLabelText(m_lbSubTitle, NKCUtilString.GET_STRING_GAUNTLET_BAN_POPUP_DESC_UP);
		}
		else if (string.Equals(ArticleID, "UNIT"))
		{
			NKCUtil.SetLabelText(m_lbSubTitle, NKCUtilString.GET_STRING_GAUNTLET_BAN_POPUP_DESC_UNIT);
		}
		else if (string.Equals(ArticleID, "SHIP"))
		{
			NKCUtil.SetLabelText(m_lbSubTitle, NKCUtilString.GET_STRING_GAUNTLET_BAN_POPUP_DESC_SHIP);
		}
		else if (string.Equals(ArticleID, "OPER"))
		{
			NKCUtil.SetLabelText(m_lbSubTitle, NKCUtilString.GET_STRING_GAUNTLET_BAN_POPUP_DESC_OPER);
		}
		foreach (KeyValuePair<TAB_TYPE, List<NKCUIPopupGuideSubSlot>> item in m_dicCastingBan)
		{
			foreach (NKCUIPopupGuideSubSlot item2 in item.Value)
			{
				NKCUtil.SetGameobjectActive(item2.gameObject, bValue: true);
				item2.OnActive(item.Key == m_curTabType);
				item2.OnSelectedObject(ArticleID);
			}
		}
		UpdateRightUI(m_curTabType, ArticleID);
	}

	private void UpdateRightUI(TAB_TYPE tabType, string articleID)
	{
		NKCUtil.SetGameobjectActive(m_objUnitList, string.Equals(articleID, "UNIT"));
		NKCUtil.SetGameobjectActive(m_objShipList, string.Equals(articleID, "SHIP"));
		NKCUtil.SetGameobjectActive(m_objOperList, string.Equals(articleID, "OPER"));
		if (tabType == TAB_TYPE.TAB_UP)
		{
			m_lstNKMUnitUpData = new List<NKMUnitUpData>(NKCBanManager.m_dicNKMUpData.Values);
			m_lstNKMUnitUpData.Sort(new CompNKMUnitUpData());
			m_lvsrUnit.TotalCount = m_lstNKMUnitUpData.Count;
		}
		else if (string.Equals(articleID, "UNIT"))
		{
			m_lstNKMBanData = null;
			switch (tabType)
			{
			case TAB_TYPE.TAB_ROTATION:
				m_lstNKMBanData = new List<NKMBanData>(NKCBanManager.GetBanData(NKCBanManager.BAN_DATA_TYPE.ROTATION).Values);
				break;
			case TAB_TYPE.TAB_CASTING:
				m_lstNKMBanData = new List<NKMBanData>(NKCBanManager.GetBanData(NKCBanManager.BAN_DATA_TYPE.CASTING).Values);
				break;
			default:
				m_lstNKMBanData = new List<NKMBanData>(NKCBanManager.GetBanData().Values);
				break;
			}
			m_lstNKMBanData.Sort(new CompNKMBanData());
			m_lvsrUnit.TotalCount = m_lstNKMBanData.Count;
		}
		else if (string.Equals(articleID, "SHIP"))
		{
			m_lstNKMBanShipData = null;
			switch (tabType)
			{
			case TAB_TYPE.TAB_ROTATION:
				m_lstNKMBanShipData = new List<NKMBanShipData>(NKCBanManager.GetBanDataShip(NKCBanManager.BAN_DATA_TYPE.ROTATION).Values);
				break;
			case TAB_TYPE.TAB_CASTING:
				m_lstNKMBanShipData = new List<NKMBanShipData>(NKCBanManager.GetBanDataShip(NKCBanManager.BAN_DATA_TYPE.CASTING).Values);
				break;
			default:
				m_lstNKMBanShipData = new List<NKMBanShipData>(NKCBanManager.GetBanDataShip().Values);
				break;
			}
			m_lstNKMBanShipData.Sort(new CompNKMBanShipData());
			m_lvsrShip.TotalCount = m_lstNKMBanShipData.Count;
		}
		else if (string.Equals(articleID, "OPER"))
		{
			m_lstNKMBanOperatorData = null;
			switch (tabType)
			{
			case TAB_TYPE.TAB_ROTATION:
				m_lstNKMBanOperatorData = new List<NKMBanOperatorData>(NKCBanManager.GetBanDataOperator(NKCBanManager.BAN_DATA_TYPE.ROTATION).Values);
				break;
			case TAB_TYPE.TAB_CASTING:
				m_lstNKMBanOperatorData = new List<NKMBanOperatorData>(NKCBanManager.GetBanDataOperator(NKCBanManager.BAN_DATA_TYPE.CASTING).Values);
				break;
			default:
				m_lstNKMBanOperatorData = new List<NKMBanOperatorData>(NKCBanManager.GetBanDataOperator().Values);
				break;
			}
			m_lstNKMBanOperatorData.Sort(new CompNKMBanOperData());
			m_lvsrOper.TotalCount = m_lstNKMBanOperatorData.Count;
		}
		m_dicUTB_ByShipGroupID.Clear();
		m_lvsrUnit.SetIndexPosition(0);
		m_lvsrShip.SetIndexPosition(0);
		m_lvsrOper.SetIndexPosition(0);
	}
}
