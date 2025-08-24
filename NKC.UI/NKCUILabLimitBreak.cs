using System.Collections.Generic;
using NKC.PacketHandler;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILabLimitBreak : MonoBehaviour
{
	public delegate void OnTryLimitBreak(long targetUnitUID);

	[Header("초월 가능할 때")]
	public GameObject m_objNormalRoot;

	public List<GameObject> lstObjStarBefore;

	public List<GameObject> lstObjStarAfter;

	public RectTransform m_rtStarEffect;

	public Text m_lbMaxLevelBefore;

	public Text m_lbMaxLevelAfter;

	public NKCUIItemCostSlot m_lbRequiredLevel;

	public Text m_lbGrowthInfo;

	public GameObject m_NKM_UI_LAB_TRANSCENDENCE_INFO_DETAIL_02;

	[Header("초월 각성 할 때")]
	public GameObject m_objTranscendenceRoot;

	public NKCUIComStarRank m_comStarRankTC;

	public GameObject m_objTranscendenceFxPurple;

	public GameObject m_objTranscendenceFxYellow;

	public Text m_lbTCLevel;

	public NKCUIComTextUnitLevel m_lbTCMaxLevelBefore;

	public Text m_lbTCMaxLevelAfter;

	public Text m_lbTCPowerupRate;

	[Header("초월 레벨 최대일 때")]
	public GameObject m_objMaxLevelRoot;

	public List<GameObject> lstObjStarMaxLevel;

	[Header("초월각성 레벨 최대일 때")]
	public GameObject m_objTCMaxLevelRoot;

	public List<GameObject> m_lstObjStarTCMaxLevel;

	[Header("비었을 때")]
	public GameObject m_objEmptyRoot;

	[Header("재료 선택 슬롯")]
	public List<NKCUIUnitSelectListSlot> m_lstUISelectSlot;

	[Header("대체 아이템 정보")]
	public NKCUIItemCostSlot m_lbCreditRequired;

	public List<NKCUIItemCostSlot> m_lstSubstituteItemUI;

	[Header("시작 버튼")]
	public NKCUIComStateButton m_csbtnLimitBreak;

	public NKCUIComStateButton m_csbtnTranscendence;

	[Header("기타")]
	public NKCUIComStateButton m_csbtnInformation;

	private OnTryLimitBreak dOnTryLimitBreak;

	private NKMUnitData m_targetUnitData;

	private NKMLimitBreakTemplet m_targetLBTemplet;

	private NKMUserData UserData => NKCScenManager.CurrentUserData();

	public void Init(OnTryLimitBreak onTryLimitBreak)
	{
		dOnTryLimitBreak = onTryLimitBreak;
		foreach (NKCUIUnitSelectListSlot item in m_lstUISelectSlot)
		{
			item.Init();
			item.SetDenied(bEnableLayoutElement: false, null);
		}
		m_csbtnLimitBreak.PointerClick.RemoveAllListeners();
		m_csbtnLimitBreak.PointerClick.AddListener(OnClickLimitBreak);
		m_csbtnTranscendence.PointerClick.RemoveAllListeners();
		m_csbtnTranscendence.PointerClick.AddListener(OnClickLimitBreak);
		m_csbtnInformation.PointerClick.RemoveAllListeners();
		m_csbtnInformation.PointerClick.AddListener(OnClickInformation);
	}

	public void Cleanup()
	{
		m_targetUnitData = null;
		m_targetLBTemplet = null;
	}

	public void SetData(NKMUnitData targetUnitData, NKMUserData userData)
	{
		_ = userData.m_ArmyData;
		m_targetUnitData = targetUnitData;
		NKMUnitLimitBreakManager.UnitLimitBreakStatusData unitLimitbreakStatus = NKMUnitLimitBreakManager.GetUnitLimitbreakStatus(m_targetUnitData);
		NKCUtil.SetGameobjectActive(m_objMaxLevelRoot, unitLimitbreakStatus.Tier == 0 && unitLimitbreakStatus.Status == NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max);
		NKCUtil.SetGameobjectActive(m_objNormalRoot, unitLimitbreakStatus.Tier == 0 && unitLimitbreakStatus.Status != NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max);
		NKCUtil.SetGameobjectActive(m_objTCMaxLevelRoot, unitLimitbreakStatus.Tier > 0 && unitLimitbreakStatus.Status == NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max);
		NKCUtil.SetGameobjectActive(m_objTranscendenceRoot, unitLimitbreakStatus.Tier > 0 && unitLimitbreakStatus.Status != NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max);
		NKCUtil.SetGameobjectActive(m_objEmptyRoot, unitLimitbreakStatus.Status == NKMUnitLimitBreakManager.UnitLimitBreakStatus.Invalid);
		NKCUtil.SetGameobjectActive(m_csbtnLimitBreak, unitLimitbreakStatus.Tier == 0);
		NKCUtil.SetGameobjectActive(m_csbtnTranscendence, unitLimitbreakStatus.Tier > 0);
		switch (unitLimitbreakStatus.Status)
		{
		case NKMUnitLimitBreakManager.UnitLimitBreakStatus.Invalid:
			NKCUtil.SetStarRank(lstObjStarMaxLevel, 0, 0);
			m_targetLBTemplet = null;
			m_lbRequiredLevel.SetData(0, 0, 0L);
			SetSubstituteItemData(targetUnitData, userData.m_InventoryData);
			LockLimitBreakButton(value: true);
			return;
		case NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max:
			NKCUtil.SetStarRank(lstObjStarMaxLevel, targetUnitData);
			NKCUtil.SetStarRank(m_lstObjStarTCMaxLevel, targetUnitData);
			m_targetLBTemplet = null;
			m_lbRequiredLevel.SetData(0, 0, 0L);
			SetSubstituteItemData(targetUnitData, userData.m_InventoryData);
			LockLimitBreakButton(value: true);
			return;
		}
		NKMLimitBreakTemplet lBInfo = NKMUnitLimitBreakManager.GetLBInfo(targetUnitData.m_LimitBreakLevel);
		NKMLimitBreakTemplet nKMLimitBreakTemplet = (m_targetLBTemplet = NKMUnitLimitBreakManager.GetLBInfo(targetUnitData.m_LimitBreakLevel + 1));
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(targetUnitData.m_UnitID);
		if (unitLimitbreakStatus.Tier > 0)
		{
			bool flag = nKMLimitBreakTemplet == null || NKMUnitLimitBreakManager.GetLBInfo(targetUnitData.m_LimitBreakLevel + 2) == null;
			m_comStarRankTC?.SetStarRank(targetUnitData.m_LimitBreakLevel + 1, unitTempletBase.m_StarGradeMax);
			NKCUtil.SetGameobjectActive(m_objTranscendenceFxYellow, !flag);
			NKCUtil.SetGameobjectActive(m_objTranscendenceFxPurple, flag);
			if (lBInfo != null)
			{
				m_lbTCMaxLevelBefore.SetText(string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, lBInfo.m_iMaxLevel), NKMUnitLimitBreakManager.GetLBTier(targetUnitData));
			}
			if (nKMLimitBreakTemplet != null)
			{
				int num = nKMLimitBreakTemplet.m_iLBRank - 3;
				NKCUtil.SetLabelText(m_lbTCLevel, NKCUtilString.GET_STRING_LIMITBREAK_TRANSCENDENCE_LEVEL_ONE_PARAM, num);
				NKCUtil.SetLabelText(m_lbTCMaxLevelAfter, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKMLimitBreakTemplet.m_iMaxLevel));
				m_lbRequiredLevel?.SetData(910, nKMLimitBreakTemplet.m_iRequiredLevel, targetUnitData.m_UnitLevel);
			}
			else
			{
				Debug.LogError("Next LBTemplet Not Found!");
				NKCUtil.SetLabelText(m_lbTCLevel, NKCUtilString.GET_STRING_LIMITBREAK_TRANSCENDENCE_LEVEL_ONE_PARAM, 0);
			}
			float num2 = NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(targetUnitData.m_LimitBreakLevel + 1) - NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(targetUnitData.m_LimitBreakLevel);
			NKCUtil.SetLabelText(m_lbTCPowerupRate, string.Format(NKCUtilString.GET_STRING_LIMITBREAK_GROWTH_INFO_ONE_PARAM, num2 * 100f));
		}
		else
		{
			int starGrade = targetUnitData.GetStarGrade(unitTempletBase);
			NKCUtil.SetStarRank(lstObjStarBefore, starGrade, unitTempletBase.m_StarGradeMax);
			NKCUtil.SetStarRank(lstObjStarAfter, starGrade + 1, unitTempletBase.m_StarGradeMax);
			GameObject gameObject = lstObjStarAfter[starGrade];
			if (gameObject != null)
			{
				NKCUtil.SetGameobjectActive(m_rtStarEffect.gameObject, bValue: true);
				m_rtStarEffect.SetParent(gameObject.transform);
				m_rtStarEffect.anchoredPosition = Vector2.zero;
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_rtStarEffect.gameObject, bValue: false);
			}
			if (lBInfo != null)
			{
				m_lbMaxLevelBefore.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, lBInfo.m_iMaxLevel);
			}
			if (nKMLimitBreakTemplet != null)
			{
				m_lbMaxLevelAfter.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKMLimitBreakTemplet.m_iMaxLevel);
				m_lbRequiredLevel?.SetData(910, nKMLimitBreakTemplet.m_iRequiredLevel, targetUnitData.m_UnitLevel);
			}
			float num3 = NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(targetUnitData.m_LimitBreakLevel + 1) - NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(targetUnitData.m_LimitBreakLevel);
			m_lbGrowthInfo.text = string.Format(NKCUtilString.GET_STRING_LIMITBREAK_GROWTH_INFO_ONE_PARAM, num3 * 100f);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_LAB_TRANSCENDENCE_INFO_DETAIL_02, targetUnitData.m_LimitBreakLevel + 1 == 3);
		SetSubstituteItemData(targetUnitData, userData.m_InventoryData);
		List<NKMItemMiscData> lstCost;
		NKM_ERROR_CODE nKM_ERROR_CODE = CanLimitBreak(UserData, m_targetUnitData, out lstCost);
		LockLimitBreakButton(nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK);
	}

	public void UpdateSubstituteItemData()
	{
		SetSubstituteItemData(m_targetUnitData, NKCScenManager.CurrentUserData().m_InventoryData);
	}

	private void SetSubstituteItemData(NKMUnitData targetUnitData, NKMInventoryData inventory)
	{
		NKMLimitBreakItemTemplet lBSubstituteItemInfo = NKMUnitLimitBreakManager.GetLBSubstituteItemInfo(targetUnitData);
		for (int i = 0; i < m_lstSubstituteItemUI.Count; i++)
		{
			NKCUIItemCostSlot nKCUIItemCostSlot = m_lstSubstituteItemUI[i];
			if (lBSubstituteItemInfo != null && i < lBSubstituteItemInfo.m_lstRequiredItem.Count)
			{
				int itemID = lBSubstituteItemInfo.m_lstRequiredItem[i].itemID;
				if (m_targetLBTemplet != null)
				{
					int count = lBSubstituteItemInfo.m_lstRequiredItem[i].count;
					if (count > 0)
					{
						nKCUIItemCostSlot.SetData(itemID, count, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemID));
						NKCUtil.SetGameobjectActive(nKCUIItemCostSlot, bValue: true);
					}
					else
					{
						NKCUtil.SetGameobjectActive(nKCUIItemCostSlot, bValue: false);
					}
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIItemCostSlot, bValue: false);
			}
		}
		UpdateRequiredCredit(targetUnitData);
	}

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (itemData.ItemID == 1)
		{
			UpdateRequiredCredit(m_targetUnitData);
			UpdateLimitBreakButton();
			return;
		}
		foreach (NKCUIItemCostSlot item in m_lstSubstituteItemUI)
		{
			if (item.ItemID == itemData.ItemID)
			{
				UpdateSubstituteItemData();
				UpdateLimitBreakButton();
				break;
			}
		}
	}

	private void UpdateRequiredCredit(NKMUnitData targetUnitData)
	{
		NKMLimitBreakItemTemplet lBSubstituteItemInfo = NKMUnitLimitBreakManager.GetLBSubstituteItemInfo(targetUnitData);
		if (lBSubstituteItemInfo != null)
		{
			m_lbCreditRequired.SetData(1, lBSubstituteItemInfo.m_CreditReq, NKCScenManager.CurrentUserData().GetCredit());
		}
		else
		{
			m_lbCreditRequired.SetData(0, 0, 0L);
		}
	}

	private bool CheckConsumedUnit(List<long> listUnitUID)
	{
		for (int i = 0; i < listUnitUID.Count; i++)
		{
			if (CheckConsumedUnit(listUnitUID[i]))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckConsumedUnit(long unitUID)
	{
		if (UserData != null)
		{
			NKMUnitData unitFromUID = UserData.m_ArmyData.GetUnitFromUID(unitUID);
			if (unitFromUID != null)
			{
				return unitFromUID.m_LimitBreakLevel > 0;
			}
		}
		return true;
	}

	private bool CheckSelectedUnitWarning(long selectUID, List<long> selectedList, out string msg)
	{
		msg = string.Empty;
		if (CheckConsumedUnit(selectUID))
		{
			msg = NKCUtilString.GET_STRING_LIMITBREAK_WARNING_SELECT_UNIT;
			return true;
		}
		return false;
	}

	private void LockLimitBreakButton(bool value)
	{
		if (value)
		{
			m_csbtnLimitBreak.Lock();
			m_csbtnTranscendence.Lock();
		}
		else
		{
			m_csbtnLimitBreak.UnLock();
			m_csbtnTranscendence.UnLock();
		}
	}

	private void UpdateLimitBreakButton()
	{
		List<NKMItemMiscData> lstCost;
		NKM_ERROR_CODE nKM_ERROR_CODE = CanLimitBreak(UserData, m_targetUnitData, out lstCost);
		LockLimitBreakButton(nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK);
	}

	private NKM_ERROR_CODE CanLimitBreak(NKMUserData userData, NKMUnitData targetUnit, out List<NKMItemMiscData> lstCost)
	{
		lstCost = new List<NKMItemMiscData>();
		if (targetUnit == null || userData == null || userData.m_ArmyData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (targetUnit.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED;
		}
		return userData.m_ArmyData.GetUnitDeckState(targetUnit) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKMUnitLimitBreakManager.CanLimitBreak(userData, targetUnit, out lstCost), 
		};
	}

	public void UnitUpdated(long uid, NKMUnitData unitData)
	{
		if (m_targetUnitData != null && uid == m_targetUnitData.m_UnitUID)
		{
			SetData(unitData, UserData);
		}
	}

	public void OnClickLimitBreak()
	{
		if (!m_csbtnLimitBreak.m_bLock || !m_csbtnTranscendence.m_bLock)
		{
			if (dOnTryLimitBreak != null && m_targetUnitData != null)
			{
				RunLimitBreak();
			}
		}
		else
		{
			if (UserData == null || m_targetUnitData == null)
			{
				return;
			}
			List<NKMItemMiscData> lstCost;
			NKM_ERROR_CODE nKM_ERROR_CODE = CanLimitBreak(UserData, m_targetUnitData, out lstCost);
			NKMLimitBreakItemTemplet lBSubstituteItemInfo = NKMUnitLimitBreakManager.GetLBSubstituteItemInfo(m_targetUnitData);
			if (lBSubstituteItemInfo == null)
			{
				return;
			}
			switch (nKM_ERROR_CODE)
			{
			case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM:
			{
				if (NKMUnitLimitBreakManager.GetLBInfo(m_targetUnitData.m_LimitBreakLevel + 1) == null || lBSubstituteItemInfo.m_lstRequiredItem == null)
				{
					break;
				}
				for (int i = 0; i < lBSubstituteItemInfo.m_lstRequiredItem.Count; i++)
				{
					int itemID = lBSubstituteItemInfo.m_lstRequiredItem[i].itemID;
					int count = lBSubstituteItemInfo.m_lstRequiredItem[i].count;
					long num = 0L;
					if (UserData != null)
					{
						num = UserData.m_InventoryData.GetCountMiscItem(itemID);
					}
					if (num < count)
					{
						NKCShopManager.OpenItemLackPopup(itemID, count);
						break;
					}
				}
				break;
			}
			case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT:
				if (UserData != null && UserData.GetCredit() < lBSubstituteItemInfo.m_CreditReq)
				{
					NKCShopManager.OpenItemLackPopup(1, lBSubstituteItemInfo.m_CreditReq);
				}
				break;
			case NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING:
			case NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING:
				NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
				break;
			case NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_LIMIT_BREAK_TEMPLET_NULL:
				break;
			default:
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCPacketHandlers.GetErrorMessage(nKM_ERROR_CODE));
				break;
			}
		}
	}

	public void OnClickInformation()
	{
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_INFORMATION, NKCUtilString.GET_STRING_LIMITBTEAK_INFO);
	}

	private void RunLimitBreak()
	{
		if (dOnTryLimitBreak != null && m_targetUnitData != null)
		{
			NKMUnitLimitBreakManager.UnitLimitBreakStatusData unitLimitbreakStatus = NKMUnitLimitBreakManager.GetUnitLimitbreakStatus(m_targetUnitData);
			List<NKCUISlot.SlotData> lstSlot = MakeSlotData(m_targetUnitData);
			string content = ((unitLimitbreakStatus.Status != NKMUnitLimitBreakManager.UnitLimitBreakStatus.CanLimitBreak || unitLimitbreakStatus.Tier <= 0) ? NKCUtilString.GET_STRING_LIMITBREAK_CONFIRM : NKCUtilString.GET_STRING_LIMITBREAK_CONFIRM_AWAKEN);
			NKCPopupResourceConfirmBox.Instance.OpenItemSlotList(NKCUtilString.GET_STRING_NOTICE, content, lstSlot, delegate
			{
				dOnTryLimitBreak(m_targetUnitData.m_UnitUID);
			}, null, mustShowNum: true);
		}
	}

	private List<NKCUISlot.SlotData> MakeSlotData(NKMUnitData targetUnitData)
	{
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		NKMLimitBreakTemplet lBInfo = NKMUnitLimitBreakManager.GetLBInfo(targetUnitData.m_LimitBreakLevel + 1);
		NKMLimitBreakItemTemplet lBSubstituteItemInfo = NKMUnitLimitBreakManager.GetLBSubstituteItemInfo(targetUnitData);
		if (lBSubstituteItemInfo != null && lBInfo != null)
		{
			list.Add(NKCUISlot.SlotData.MakeMiscItemData(1, lBSubstituteItemInfo.m_CreditReq));
			for (int i = 0; i < lBSubstituteItemInfo.m_lstRequiredItem.Count; i++)
			{
				NKMLimitBreakItemTemplet.ItemRequirement itemRequirement = lBSubstituteItemInfo.m_lstRequiredItem[i];
				int count = itemRequirement.count;
				if (count > 0)
				{
					NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeMiscItemData(itemRequirement.itemID, count);
					list.Add(item);
				}
			}
		}
		return list;
	}
}
