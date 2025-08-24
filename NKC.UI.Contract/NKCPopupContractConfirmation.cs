using System.Collections.Generic;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Contract;

public class NKCPopupContractConfirmation : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_contract";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONTRACT_CONFIRMATION";

	private static NKCPopupContractConfirmation m_Instance;

	[Header("타이틀")]
	public Text NKM_UI_POPUP_OK_BOX_TOP_TEXT;

	public NKCUIComStateButton NKM_UI_POPUP_CLOSEBUTTON;

	public Text CONFIRMATION_INFO_TEXT;

	[Header("닫기")]
	public NKCUIComStateButton m_BG;

	private List<NKCAssetInstanceData> m_lstAssetInstanceData = new List<NKCAssetInstanceData>();

	public RectTransform m_Content;

	public ScrollRect m_ScrollRect;

	private const string asset_bundle = "ab_ui_unit_slot_card";

	private const string asset_name = "NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT";

	public static NKCPopupContractConfirmation Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupContractConfirmation>("ab_ui_nkm_ui_popup_contract", "NKM_UI_POPUP_CONTRACT_CONFIRMATION", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupContractConfirmation>();
				m_Instance.Init();
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

	public override string MenuName => NKCUtilString.GET_STRING_SHOP_SKIN_INFO;

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

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		foreach (NKCAssetInstanceData lstAssetInstanceDatum in m_lstAssetInstanceData)
		{
			NKCAssetResourceManager.CloseInstance(lstAssetInstanceDatum);
		}
		m_lstAssetInstanceData.Clear();
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(NKM_UI_POPUP_CLOSEBUTTON, base.Close);
		NKCUtil.SetBindFunction(m_BG, base.Close);
	}

	public void Open(int contractID)
	{
		Open(ContractTempletV2.Find(contractID));
	}

	public void Open(ContractTempletV2 templet)
	{
		if (templet == null || templet.m_ContractBonusCountGroupID == 0)
		{
			return;
		}
		if (templet.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			NKCUtil.SetLabelText(NKM_UI_POPUP_OK_BOX_TOP_TEXT, string.Format(NKCUtilString.GET_STRING_CONTRACT_CONFIRMATION_POPUP_TITLE_01, templet.GetContractName()));
			NKCUtil.SetLabelText(CONFIRMATION_INFO_TEXT, NKCUtilString.GET_STRING_CONTRACT_CONFIRM_BOTTOM_DESC);
			List<NKMUnitData> list = new List<NKMUnitData>();
			foreach (RandomUnitTempletV2 unitTemplet in templet.UnitPoolTemplet.UnitTemplets)
			{
				if (unitTemplet.PickUpTarget && unitTemplet.UnitTemplet.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR)
				{
					NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(unitTemplet.UnitTemplet.m_UnitID, 1, 0);
					if (nKMUnitData != null)
					{
						list.Add(nKMUnitData);
					}
				}
			}
			List<int> curSelectableUnitList = NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetCurSelectableUnitList(templet.Key);
			bool bPickUpUnit = true;
			foreach (NKMUnitData unitData in list)
			{
				if (curSelectableUnitList.Count > 0)
				{
					bPickUpUnit = curSelectableUnitList.Find((int e) => e == unitData.m_UnitID) > 0;
				}
				AddUnitSelectListSlot(unitData, bPickUpUnit);
			}
		}
		else
		{
			if (templet.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_OPERATOR)
			{
				Debug.LogError("채용 설정을 확인해주세요 - Invaild Unit Type");
				return;
			}
			NKCUtil.SetLabelText(NKM_UI_POPUP_OK_BOX_TOP_TEXT, string.Format(NKCUtilString.GET_STRING_CONTRACT_CONFIRMATION_POPUP_TITLE_01_OPERATOR, templet.GetContractName()));
			NKCUIBase.SetLabelText(CONFIRMATION_INFO_TEXT, NKCUtilString.GET_STRING_CONTRACT_CONFIRM_BOTTOM_DESC_OPERATOR);
			List<NKMOperator> list2 = new List<NKMOperator>();
			foreach (RandomUnitTempletV2 unitTemplet2 in templet.UnitPoolTemplet.UnitTemplets)
			{
				if (unitTemplet2.PickUpTarget && unitTemplet2.UnitTemplet.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR)
				{
					NKMOperator dummyOperator = NKCOperatorUtil.GetDummyOperator(unitTemplet2.UnitTemplet, bSetMaximum: true);
					if (dummyOperator != null)
					{
						list2.Add(dummyOperator);
					}
				}
			}
			List<int> curSelectableUnitList2 = NKCScenManager.GetScenManager().GetNKCContractDataMgr().GetCurSelectableUnitList(templet.Key);
			bool bPickUpUnit2 = true;
			foreach (NKMOperator operData in list2)
			{
				if (curSelectableUnitList2.Count > 0)
				{
					bPickUpUnit2 = curSelectableUnitList2.Find((int e) => e == operData.id) > 0;
				}
				AddUnitSelectListSlot(operData, bPickUpUnit2);
			}
		}
		base.gameObject.SetActive(value: true);
		UIOpened();
	}

	public void Open(CustomPickupContractTemplet templet)
	{
		if (templet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(NKM_UI_POPUP_OK_BOX_TOP_TEXT, string.Format(NKCUtilString.GET_STRING_CONTRACT_CONFIRMATION_POPUP_TITLE_01, templet.GetContractName()));
		NKCUtil.SetLabelText(CONFIRMATION_INFO_TEXT, NKCUtilString.GET_STRING_CONTRACT_CONFIRM_BOTTOM_DESC);
		foreach (RandomUnitTempletV2 unitTemplet in templet.UnitPoolTemplet.UnitTemplets)
		{
			if (unitTemplet.UnitTemplet.m_UnitID != templet.PickUpTargetUnitID)
			{
				continue;
			}
			if (templet.CustomPickUpType == CustomPickupContractTemplet.CUSTOM_PICK_UP_TYPE.OPERATOR)
			{
				NKMOperator dummyOperator = NKCOperatorUtil.GetDummyOperator(unitTemplet.UnitTemplet.m_UnitID);
				if (dummyOperator != null)
				{
					AddUnitSelectListSlot(dummyOperator, bPickUpUnit: true);
				}
			}
			else
			{
				NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(unitTemplet.UnitTemplet.m_UnitID, 1, 0);
				if (nKMUnitData != null)
				{
					AddUnitSelectListSlot(nKMUnitData, bPickUpUnit: true);
				}
			}
		}
		base.gameObject.SetActive(value: true);
		UIOpened();
	}

	private void AddUnitSelectListSlot(NKMUnitData unitData, bool bPickUpUnit)
	{
		if (unitData == null)
		{
			return;
		}
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_unit_slot_card", "NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT");
		if (!(nKCAssetInstanceData.m_Instant != null))
		{
			return;
		}
		NKCUIUnitSelectListSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIUnitSelectListSlot>();
		if (component != null)
		{
			component.Init(resetLocalScale: true);
			component.transform.SetParent(m_Content, worldPositionStays: false);
			component.SetDataForContractSelection(unitData, bHave: false);
			if (!bPickUpUnit)
			{
				component.SetSlotState(NKCUnitSortSystem.eUnitState.CHECKED);
			}
			m_lstAssetInstanceData.Add(nKCAssetInstanceData);
		}
	}

	private void AddUnitSelectListSlot(NKMOperator operatorData, bool bPickUpUnit)
	{
		if (operatorData == null)
		{
			return;
		}
		NKCUIOperatorSelectListSlot newInstance = NKCUIOperatorSelectListSlot.GetNewInstance(m_Content);
		if (null != newInstance)
		{
			NKCUtil.SetGameobjectActive(newInstance.gameObject, bValue: true);
			newInstance.SetDataForContractSelection(operatorData);
			if (!bPickUpUnit)
			{
				newInstance.SetSlotState(NKCUnitSortSystem.eUnitState.CHECKED);
			}
			m_lstAssetInstanceData.Add(newInstance.m_Instance);
		}
	}
}
