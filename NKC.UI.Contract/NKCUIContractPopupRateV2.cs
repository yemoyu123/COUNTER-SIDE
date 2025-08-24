using System.Collections;
using System.Collections.Generic;
using NKM;
using NKM.Contract2;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Contract;

public class NKCUIContractPopupRateV2 : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_contract";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONTRACT_RATE";

	private static NKCUIContractPopupRateV2 m_Instance;

	[Header("타이틀")]
	public Text m_NKM_UI_POPUP_OK_BOX_TOP_TEXT;

	[Header("버튼")]
	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSEBUTTON;

	public NKCUIComStateButton m_BG;

	[Header("우측 스크롤")]
	public VerticalLayoutGroup NKM_UI_POPUP_CONTRACT_SLOT_layoutgroup;

	public RectTransform m_rtm_NKM_UI_POPUP_CONTRACT_RATE_SCROLL;

	public LoopVerticalScrollRect m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL;

	[Header("등급별 확률")]
	public Text m_NKM_UI_POPUP_CONTRACT_SETTING_3_SSR_COUNT;

	public Text m_NKM_UI_POPUP_CONTRACT_SETTING_3_SR_COUNT;

	public Text m_NKM_UI_POPUP_CONTRACT_SETTING_3_R_COUNT;

	public Text m_NKM_UI_POPUP_CONTRACT_SETTING_3_N_COUNT;

	[Header("픽업 확률")]
	public RectTransform m_rtNKM_UI_POPUP_CONTRACT_RATE_FEATURED;

	public RectTransform m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL_Contents;

	[Header("기본 확률 슬롯")]
	public GameObject m_objNKM_UI_POPUP_CONTRACT_RATE_FEATURED_BASE;

	public NKCUIContractPopupRateSlot m_NKM_UI_POPUP_CONTRACT_RATE_FEATURED_BASE;

	[Header("추가 슬롯")]
	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_FEATURED;

	public NKCUIContractPopupRateSlot m_NKM_UI_POPUP_CONTRACT_RATE_SLOT;

	[Space]
	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_TEXT_TWN;

	[Header("확률 구분 탭")]
	public GameObject m_objMultiTab;

	public NKCUIComToggle m_tlgMultiTabl01;

	public List<NKCComText> m_lstMultiTabTitle01;

	public NKCUIComToggle m_tlgMultiTabl02;

	public List<NKCComText> m_lstMultiTabTitle02;

	public NKCUIComToggle m_tlgMultiTabl03;

	public List<NKCComText> m_lstMultiTabTitle03;

	private List<GameObject> m_DummyPickUpSlot = new List<GameObject>();

	private List<ContractUnitSlotData> m_lstPickUnitSlot = new List<ContractUnitSlotData>();

	private List<ContractUnitSlotData> m_lstUnitSlot = new List<ContractUnitSlotData>();

	private RandomGradeTempletV2 m_RandomGradeTemplet;

	private IRandomUnitPool m_UnitPoolTemplet;

	private int m_iCustomPickUpTempletKey;

	private List<CustomPickupContractTemplet> m_lstCustomPickContractTemplets = new List<CustomPickupContractTemplet>();

	private bool m_bShowOnlyCustomTargetUnit;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "세부 확률";

	public static NKCUIContractPopupRateV2 Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIContractPopupRateV2>("ab_ui_nkm_ui_popup_contract", "NKM_UI_POPUP_CONTRACT_RATE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIContractPopupRateV2>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override void CloseInternal()
	{
		for (int i = 0; i < m_DummyPickUpSlot.Count; i++)
		{
			if (m_DummyPickUpSlot[i] != null)
			{
				Object.Destroy(m_DummyPickUpSlot[i]);
				m_DummyPickUpSlot[i] = null;
			}
		}
		if (m_iCustomPickUpTempletKey != 0)
		{
			CustomPickupContractTemplet.GetDummyCalculateTemplet(m_iCustomPickUpTempletKey, 0);
			m_iCustomPickUpTempletKey = 0;
		}
		m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.ClearCells();
		m_DummyPickUpSlot.Clear();
		m_bShowOnlyCustomTargetUnit = false;
		m_lstCustomPickContractTemplets.Clear();
		base.gameObject.SetActive(value: false);
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public void Init()
	{
		if (m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL != null)
		{
			m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.dOnGetObject += MakeProbabilitySlot;
			m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.dOnReturnObject += ReturnProbabilitySlot;
			m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.dOnProvideData += ProvideProbabilitySlotData;
			NKCUtil.SetScrollHotKey(m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL);
		}
		NKCUtil.SetBindFunction(m_BG, base.Close);
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_CLOSEBUTTON, base.Close);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CONTRACT_RATE_TEXT_TWN, NKCStringTable.GetNationalCode() == NKM_NATIONAL_CODE.NNC_TRADITIONAL_CHINESE);
		NKCUtil.SetToggleValueChangedDelegate(m_tlgMultiTabl01, delegate
		{
			OnToggleTab(0);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_tlgMultiTabl02, delegate
		{
			OnToggleTab(1);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_tlgMultiTabl03, delegate
		{
			OnToggleTab(2);
		});
	}

	private RectTransform MakeProbabilitySlot(int index)
	{
		return Object.Instantiate(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT).GetComponent<RectTransform>();
	}

	private void ReturnProbabilitySlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(base.transform);
		Object.Destroy(go.gameObject);
	}

	private void ProvideProbabilitySlotData(Transform tr, int idx)
	{
		NKCUIContractPopupRateSlot component = tr.GetComponent<NKCUIContractPopupRateSlot>();
		if (component != null)
		{
			if (m_lstUnitSlot.Count <= idx || idx < 0)
			{
				Debug.LogError($"m_lstUnitSlot - 잘못된 인덱스 입니다, {idx}");
			}
			else
			{
				component.SetData(m_lstUnitSlot[idx], m_lstUnitSlot[idx].Pickup, m_bShowOnlyCustomTargetUnit);
			}
		}
	}

	public void Open(IRandomUnitPool unitPoolTemple, RandomGradeTempletV2 randomGradeTemplet, string strContractName, int iCustomTargetUnitID = 0)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_bShowOnlyCustomTargetUnit)
		{
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_OK_BOX_TOP_TEXT, NKCUtilString.GET_STRING_CONTRACT_POPUP_RATE_DETAIL_PICKUP_ONLY_TITLE);
		}
		else
		{
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_OK_BOX_TOP_TEXT, string.Format(NKCUtilString.GET_STRING_CONTRACT_POPUP_RATE_DETAIL_PERCENT_01, strContractName));
		}
		m_RandomGradeTemplet = randomGradeTemplet;
		m_UnitPoolTemplet = unitPoolTemple;
		m_lstUnitSlot.Clear();
		m_lstPickUnitSlot.Clear();
		foreach (RandomUnitTempletV2 unitTemplet in unitPoolTemple.UnitTemplets)
		{
			if (!m_bShowOnlyCustomTargetUnit || unitTemplet.CustomPickupTarget)
			{
				bool flag = ((iCustomTargetUnitID == 0) ? unitTemplet.PickUpTarget : (unitTemplet.UnitTemplet.m_UnitID == iCustomTargetUnitID));
				ContractUnitSlotData item = new ContractUnitSlotData(unitTemplet.UnitTemplet.m_UnitID, unitTemplet.UnitTemplet.m_NKM_UNIT_GRADE, unitTemplet.UnitTemplet.m_NKM_UNIT_STYLE_TYPE, unitTemplet.UnitTemplet.m_NKM_UNIT_ROLE_TYPE, unitTemplet.UnitTemplet.m_bAwaken, unitTemplet.UnitTemplet.IsRearmUnit, unitTemplet.UnitTemplet.GetUnitName(), unitTemplet.FinalRatePercent, unitTemplet.RatioUpTarget, flag);
				if (flag)
				{
					m_lstPickUnitSlot.Add(item);
				}
				else
				{
					m_lstUnitSlot.Add(item);
				}
			}
		}
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_SETTING_3_SSR_COUNT, $"{m_RandomGradeTemplet.FinalRateSsrPercent}%");
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_SETTING_3_SR_COUNT, $"{m_RandomGradeTemplet.FinalRateSrPercent}%");
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_SETTING_3_R_COUNT, $"{m_RandomGradeTemplet.FinalRateRPercent}%");
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_SETTING_3_N_COUNT, $"{m_RandomGradeTemplet.FinalRateNPercent}%");
		UpdatePickUpTable();
		if (m_rtm_NKM_UI_POPUP_CONTRACT_RATE_SCROLL != null)
		{
			m_rtm_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.transform.SetAsLastSibling();
		}
		StartCoroutine(WaitForSetupChildLayout());
		UIOpened();
	}

	public void Open(ContractTempletV2 templet)
	{
		if (templet != null)
		{
			m_bShowOnlyCustomTargetUnit = false;
			NKCUtil.SetGameobjectActive(m_objMultiTab, bValue: false);
			Open(templet.UnitPoolTemplet, templet.RandomGradeTemplet, templet.GetContractName());
		}
	}

	public void Open(CustomPickupContractTemplet customPickTemplet)
	{
		if (customPickTemplet == null)
		{
			return;
		}
		CustomPickupContractTemplet dummyCalculateTemplet = CustomPickupContractTemplet.GetDummyCalculateTemplet(customPickTemplet.Key, customPickTemplet.PickUpTargetUnitID);
		if (dummyCalculateTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objMultiTab, m_lstCustomPickContractTemplets.Count > 1);
		m_iCustomPickUpTempletKey = customPickTemplet.Key;
		List<int> list = new List<int>();
		dummyCalculateTemplet.UnitPoolTemplet.RecalculateUnitTemplets();
		if (!m_bShowOnlyCustomTargetUnit)
		{
			foreach (RandomUnitTempletV2 unitTemplet in dummyCalculateTemplet.UnitPoolTemplet.UnitTemplets)
			{
				if (unitTemplet != null && unitTemplet.UnitTemplet.m_bAwaken && customPickTemplet.PickUpTargetUnitID != unitTemplet.UnitTemplet.m_UnitID)
				{
					list.Add(unitTemplet.UnitTemplet.m_UnitID);
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objMultiTab, m_lstCustomPickContractTemplets.Count > 1);
		Open(dummyCalculateTemplet.GetUnitPoolTemplet(list), dummyCalculateTemplet.RandomGradeTemplet, dummyCalculateTemplet.GetContractName(), dummyCalculateTemplet.PickUpTargetUnitID);
	}

	public void Open(List<CustomPickupContractTemplet> lstCustomPickupTemplet)
	{
		if (lstCustomPickupTemplet.Count == 0)
		{
			return;
		}
		for (int i = 0; i < lstCustomPickupTemplet.Count; i++)
		{
			string contractName = lstCustomPickupTemplet[i].GetContractName();
			if (i == 0)
			{
				foreach (NKCComText item in m_lstMultiTabTitle01)
				{
					item.text = contractName;
				}
			}
			if (i == 1)
			{
				foreach (NKCComText item2 in m_lstMultiTabTitle02)
				{
					item2.text = contractName;
				}
			}
			if (i != 2)
			{
				continue;
			}
			foreach (NKCComText item3 in m_lstMultiTabTitle03)
			{
				item3.text = contractName;
			}
		}
		m_bShowOnlyCustomTargetUnit = true;
		m_lstCustomPickContractTemplets = lstCustomPickupTemplet;
		m_tlgMultiTabl01.Select(bSelect: true, bForce: true);
		OnToggleTab(0);
	}

	public void Open(NKMItemMiscTemplet contractItemTemplet)
	{
		if (!contractItemTemplet.IsContractItem || contractItemTemplet.MiscContractTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_OK_BOX_TOP_TEXT, string.Format(NKCUtilString.GET_STRING_CONTRACT_POPUP_RATE_DETAIL_PERCENT_01, contractItemTemplet.GetItemName()));
		m_RandomGradeTemplet = contractItemTemplet.MiscContractTemplet.RandomGradeTemplet;
		m_UnitPoolTemplet = contractItemTemplet.MiscContractTemplet.UnitPoolTemplet;
		m_lstUnitSlot.Clear();
		m_lstPickUnitSlot.Clear();
		foreach (RandomUnitTempletV2 unitTemplet in m_UnitPoolTemplet.UnitTemplets)
		{
			ContractUnitSlotData item = new ContractUnitSlotData(unitTemplet.UnitTemplet.m_UnitID, unitTemplet.UnitTemplet.m_NKM_UNIT_GRADE, unitTemplet.UnitTemplet.m_NKM_UNIT_STYLE_TYPE, unitTemplet.UnitTemplet.m_NKM_UNIT_ROLE_TYPE, unitTemplet.UnitTemplet.m_bAwaken, unitTemplet.UnitTemplet.IsRearmUnit, unitTemplet.UnitTemplet.GetUnitName(), unitTemplet.FinalRatePercent, unitTemplet.RatioUpTarget, unitTemplet.PickUpTarget);
			if (unitTemplet.PickUpTarget)
			{
				m_lstPickUnitSlot.Add(item);
			}
			else
			{
				m_lstUnitSlot.Add(item);
			}
		}
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_SETTING_3_SSR_COUNT, $"{m_RandomGradeTemplet.FinalRateSsrPercent}%");
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_SETTING_3_SR_COUNT, $"{m_RandomGradeTemplet.FinalRateSrPercent}%");
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_SETTING_3_R_COUNT, $"{m_RandomGradeTemplet.FinalRateRPercent}%");
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_SETTING_3_N_COUNT, $"{m_RandomGradeTemplet.FinalRateNPercent}%");
		UpdatePickUpTable();
		if (m_rtm_NKM_UI_POPUP_CONTRACT_RATE_SCROLL != null)
		{
			m_rtm_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.transform.SetAsLastSibling();
		}
		StartCoroutine(WaitForSetupChildLayout());
		UIOpened();
	}

	private IEnumerator WaitForSetupChildLayout()
	{
		yield return new WaitForEndOfFrame();
		m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.PrepareCells();
		m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.TotalCount = (m_bShowOnlyCustomTargetUnit ? m_lstUnitSlot.Count : (m_UnitPoolTemplet.TotalUnitCount - m_lstPickUnitSlot.Count));
		m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.velocity = new Vector2(0f, 0f);
		m_NKM_UI_POPUP_CONTRACT_RATE_SCROLL.SetIndexPosition(0);
	}

	private void UpdatePickUpTable()
	{
		if (m_lstPickUnitSlot.Count >= 4)
		{
			List<ContractUnitSlotData> list = new List<ContractUnitSlotData>();
			list.AddRange(m_lstPickUnitSlot);
			list.AddRange(m_lstUnitSlot);
			m_lstPickUnitSlot.Clear();
			m_lstUnitSlot = list;
		}
		if (m_lstPickUnitSlot.Count <= 0)
		{
			NKCUtil.SetGameobjectActive(m_objNKM_UI_POPUP_CONTRACT_RATE_FEATURED_BASE, bValue: false);
			return;
		}
		for (int i = 0; i < m_lstPickUnitSlot.Count; i++)
		{
			if (m_lstPickUnitSlot[i] == null)
			{
				Debug.LogError($"Error - UpdatePickUpTable : Slot Index : {i}");
				continue;
			}
			if (i == 0)
			{
				NKCUtil.SetGameobjectActive(m_objNKM_UI_POPUP_CONTRACT_RATE_FEATURED_BASE, bValue: true);
				m_NKM_UI_POPUP_CONTRACT_RATE_FEATURED_BASE.SetData(m_lstPickUnitSlot[i], bPickup: true);
				continue;
			}
			GameObject gameObject = Object.Instantiate(m_NKM_UI_POPUP_CONTRACT_RATE_FEATURED);
			if (gameObject != null)
			{
				gameObject.transform.SetParent(m_rtNKM_UI_POPUP_CONTRACT_RATE_FEATURED, worldPositionStays: false);
				NKCUIContractPopupRateSlot componentInChildren = gameObject.GetComponentInChildren<NKCUIContractPopupRateSlot>();
				if (componentInChildren != null)
				{
					componentInChildren.SetData(m_lstPickUnitSlot[i], bPickup: true);
				}
				m_DummyPickUpSlot.Add(gameObject);
			}
		}
	}

	private void OnToggleTab(int tabNumber)
	{
		if (m_lstCustomPickContractTemplets.Count >= tabNumber && m_lstCustomPickContractTemplets[tabNumber] != null)
		{
			Open(m_lstCustomPickContractTemplets[tabNumber]);
		}
	}
}
