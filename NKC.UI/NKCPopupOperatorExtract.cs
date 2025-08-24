using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupOperatorExtract : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_OPR_EXTRACT";

	private static NKCPopupOperatorExtract m_Instance;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	public NKCComText m_lbCostCnt;

	public RectTransform m_rtOperatorExtractReward;

	public RectTransform m_rtRemoveReward;

	public NKCComTMPUIText m_lbOperatorExtractDesc;

	private List<long> m_lstExtractOperator = new List<long>();

	private List<NKCUISlot.SlotData> m_lstRemoveRewardSlotData = new List<NKCUISlot.SlotData>();

	private List<NKCUISlot.SlotData> m_lstExtractRewardSlotData = new List<NKCUISlot.SlotData>();

	private List<NKCUISlot> m_lstVisibleSlots = new List<NKCUISlot>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "\ufffd\ufffd\ufffd۷\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd";

	public static NKCPopupOperatorExtract Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupOperatorExtract>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_OPR_EXTRACT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupOperatorExtract>();
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

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		for (int i = 0; i < m_lstVisibleSlots.Count; i++)
		{
			if (!(null == m_lstVisibleSlots[i]))
			{
				Object.Destroy(m_lstVisibleSlots[i].gameObject);
			}
		}
		m_lstVisibleSlots.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		Close();
	}

	public void Open(List<long> lstOperators)
	{
		m_lstRemoveRewardSlotData.Clear();
		m_lstExtractRewardSlotData.Clear();
		m_lstVisibleSlots.Clear();
		m_lstExtractOperator.Clear();
		m_lstExtractOperator = lstOperators;
		NKCUtil.SetLabelText(m_lbOperatorExtractDesc, string.Format(NKCStringTable.GetString("SI_PF_OPERATOR_EXTRACT_TEXT_3"), lstOperators.Count));
		UpdateRewardUI();
		UIOpened();
	}

	private void InitUI()
	{
		NKCUtil.SetBindFunction(m_csbtnOK, OnConfirm);
		NKCUtil.SetBindFunction(m_csbtnCancel, CloseInternal);
	}

	private void UpdateRewardUI()
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
		int num = 0;
		foreach (long item in m_lstExtractOperator)
		{
			NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(item);
			if (operatorData == null)
			{
				continue;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
			if (unitTempletBase == null)
			{
				continue;
			}
			bool fromContract = operatorData.fromContract;
			for (int i = 0; i < unitTempletBase.RemoveRewards.Count; i++)
			{
				int iD = unitTempletBase.RemoveRewards[i].ID;
				int count = unitTempletBase.RemoveRewards[i].Count;
				if (dictionary.ContainsKey(iD))
				{
					dictionary[iD] += count;
				}
				else
				{
					dictionary.Add(iD, count);
				}
			}
			if (fromContract && unitTempletBase.RemoveRewardFromContract != null)
			{
				int iD2 = unitTempletBase.RemoveRewardFromContract.ID;
				int count2 = unitTempletBase.RemoveRewardFromContract.Count;
				if (dictionary.ContainsKey(iD2))
				{
					dictionary[iD2] += count2;
				}
				else
				{
					dictionary.Add(iD2, count2);
				}
			}
			int extractItemID = NKCOperatorUtil.GetExtractItemID(unitTempletBase.m_OprPassiveGroupID, operatorData.subSkill.id, unitTempletBase.m_NKM_UNIT_GRADE);
			if (dictionary2.ContainsKey(extractItemID))
			{
				dictionary2[extractItemID]++;
			}
			else
			{
				dictionary2.Add(extractItemID, 1);
			}
			NKCOperatorUtil.GetExtractPriceItem(unitTempletBase.m_NKM_UNIT_GRADE, out var _, out var value);
			num += value;
		}
		NKCUtil.SetLabelText(m_lbCostCnt, num.ToString());
		foreach (KeyValuePair<int, int> item2 in dictionary)
		{
			MakeRewardSlot(NKCUISlot.SlotData.MakeMiscItemData(item2.Key, item2.Value), m_rtRemoveReward);
		}
		foreach (KeyValuePair<int, int> item3 in dictionary2)
		{
			MakeRewardSlot(NKCUISlot.SlotData.MakeMiscItemData(item3.Key, item3.Value), m_rtOperatorExtractReward);
		}
	}

	private void MakeRewardSlot(NKCUISlot.SlotData slotData, RectTransform rtTaargetTransform)
	{
		NKCUISlot newInstance = NKCUISlot.GetNewInstance(rtTaargetTransform);
		newInstance.transform.localScale = Vector3.one;
		newInstance.Init();
		NKCUtil.SetGameobjectActive(newInstance.gameObject, bValue: true);
		m_lstVisibleSlots.Add(newInstance);
		newInstance.SetData(slotData, bShowName: false, bShowNumber: true, bEnableLayoutElement: true, null);
		newInstance.SetOpenItemBoxOnClick();
	}

	private void OnConfirm()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMArmyData armyData = myUserData.m_ArmyData;
		foreach (long item in m_lstExtractOperator)
		{
			if (!armyData.m_dicMyOperator.ContainsKey(item))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_NO_EXIST_UNIT);
				return;
			}
			NKM_ERROR_CODE canDeleteOperator = NKMUnitManager.GetCanDeleteOperator(armyData.GetOperatorFromUId(item), myUserData);
			switch (canDeleteOperator)
			{
			case NKM_ERROR_CODE.NEC_FAIL_UNIT_LOCKED:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_LOCKED);
				return;
			case NKM_ERROR_CODE.NEC_FAIL_UNIT_IN_DECK:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_IN_DECK);
				return;
			case NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_LOBBYUNIT:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL, NKCUtilString.GET_STRING_REMOVE_UNIT_FAIL_MAINUNIT);
				return;
			default:
				NKCPopupOKCancel.OpenOKBox(canDeleteOperator);
				return;
			case NKM_ERROR_CODE.NEC_OK:
				break;
			}
		}
		NKCPacketSender.Send_NKMPacket_OPERATOR_EXTRACT_REQ(m_lstExtractOperator);
	}
}
