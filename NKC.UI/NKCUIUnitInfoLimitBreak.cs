using System.Collections.Generic;
using NKC.PacketHandler;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitInfoLimitBreak : MonoBehaviour
{
	public delegate List<NKMUnitData> GetUnitList();

	[Header("일반")]
	public GameObject m_objNormalRoot;

	public NKCUIComStarRank m_comStarBefore;

	public NKCUIComStarRank m_comStarAfter;

	public RectTransform m_rtStarEffect;

	public Text m_lbMaxLevelBefore;

	public Text m_lbMaxLevelAfter;

	public NKCUIItemCostSlot m_lbRequiredLevel;

	public Text m_lbGrowthInfo;

	public GameObject m_NKM_UI_LAB_TRANSCENDENCE_INFO_DETAIL_02;

	[Header("초월")]
	public GameObject m_objTranscendenceRoot;

	public NKCUIComStarRank m_comStarRankTC;

	public GameObject m_objTranscendenceFxPurple;

	public GameObject m_objTranscendenceFxYellow;

	public Text m_lbTCLevel;

	public NKCUIComTextUnitLevel m_lbTCMaxLevelBefore;

	public NKCUIComTextUnitLevel m_lbTCMaxLevelAfter;

	public Text m_lbTCPowerupRate;

	[Header("초월 레벨 최대일 때")]
	public GameObject m_objMaxLevelRoot;

	public NKCUIComStarRank m_comStarRankMaxLevel;

	[Header("초월각성 레벨 최대일 때")]
	public GameObject m_objTCMaxLevelRoot;

	public NKCUIComStarRank m_comStarRankTCMaxLevel;

	[Header("비었을 때")]
	public GameObject m_objEmptyRoot;

	[Header("대체 아이템 정보")]
	public NKCUIItemCostSlot m_lbCreditRequired;

	public List<NKCUIItemCostSlot> m_lstSubstituteItemUI;

	[Header("시작 버튼")]
	public NKCUIComStateButton m_csbtnLimitBreak;

	public GameObject m_btnUILimitbreak;

	public GameObject m_btnUITranscendence;

	public List<Image> m_lstEnterBtnIcon;

	public List<Text> m_lstEnterBtnText;

	[Header("기타")]
	public GameObject m_objLimitbreakResult;

	public NKCUIComStateButton m_csbtnInformation;

	public GameObject m_objLimitBreakCost;

	[Header("재무장")]
	public GameObject m_objRearmUI;

	public NKCUIComStateButton m_csbtnRearmament;

	public Text m_lbRearmBtn;

	public Image m_imgRearmBtn;

	public NKCUIRearmamentSubUISelectList m_RearmSubUI;

	private NKMUnitData m_targetUnitData;

	private NKMLimitBreakTemplet m_targetLBTemplet;

	[Header("전술 업데이트")]
	public NKCUITacticUpdateLevelSlot m_tacticUpdateLevelSlot;

	public NKCUIComStateButton m_csbtnTacticUpdate;

	private GetUnitList m_GetUnitListCallBack;

	private NKCUITacticUpdate.dLastSelectedUnitData m_LastSelectedUnitCallBack;

	private NKCUIUnitSelectList m_UIUnitSelectList;

	private int m_iRearmTargetUnitID;

	private NKCUIUnitSelectList UnitSelectList
	{
		get
		{
			if (m_UIUnitSelectList == null)
			{
				m_UIUnitSelectList = NKCUIUnitSelectList.OpenNewInstance();
			}
			return m_UIUnitSelectList;
		}
	}

	public void Init(GetUnitList callBack, NKCUITacticUpdate.dLastSelectedUnitData lastUnitCallBack)
	{
		m_RearmSubUI.Init(SelectedRearmUnit);
		NKCUtil.SetBindFunction(m_csbtnLimitBreak, OnClickLimitBreak);
		NKCUtil.SetBindFunction(m_csbtnInformation, OnClickInformation);
		NKCUtil.SetBindFunction(m_csbtnRearmament, OnClickMoveToRearmment);
		NKCUtil.SetBindFunction(m_csbtnTacticUpdate, OnClickTacticUpdate);
		m_GetUnitListCallBack = callBack;
		m_LastSelectedUnitCallBack = lastUnitCallBack;
	}

	public void Clear()
	{
		if (m_UIUnitSelectList != null && NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_UNIT_LIST)
		{
			UnitSelectList.Close();
		}
		m_UIUnitSelectList = null;
	}

	public void SetData(NKMUnitData unitData)
	{
		m_targetUnitData = unitData;
		NKMUnitLimitBreakManager.UnitLimitBreakStatusData unitLimitbreakStatus = NKMUnitLimitBreakManager.GetUnitLimitbreakStatus(unitData);
		bool flag = NKCRearmamentUtil.IsCanRearmamentUnit(m_targetUnitData.m_UnitUID) && NKCRearmamentUtil.IsCanUseContent();
		NKCUtil.SetGameobjectActive(m_objMaxLevelRoot, unitLimitbreakStatus.Tier == 0 && unitLimitbreakStatus.Status == NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max);
		NKCUtil.SetGameobjectActive(m_objNormalRoot, unitLimitbreakStatus.Tier == 0 && unitLimitbreakStatus.Status != NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max);
		NKCUtil.SetGameobjectActive(m_objTCMaxLevelRoot, unitLimitbreakStatus.Tier > 0 && unitLimitbreakStatus.Status == NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max);
		NKCUtil.SetGameobjectActive(m_objTranscendenceRoot, unitLimitbreakStatus.Tier > 0 && unitLimitbreakStatus.Status != NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max);
		NKCUtil.SetGameobjectActive(m_objEmptyRoot, unitLimitbreakStatus.Status == NKMUnitLimitBreakManager.UnitLimitBreakStatus.Invalid);
		NKCUtil.SetGameobjectActive(m_csbtnRearmament, flag);
		m_csbtnRearmament.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.REARM));
		NKCUtil.SetGameobjectActive(m_csbtnLimitBreak, bValue: true);
		UpdateRearmUI(flag);
		UpdateTacticUpdateUI();
		NKCUtil.SetGameobjectActive(m_btnUILimitbreak, unitLimitbreakStatus.Tier == 0);
		NKCUtil.SetGameobjectActive(m_btnUITranscendence, unitLimitbreakStatus.Tier > 0);
		NKMLimitBreakTemplet lBInfo = NKMUnitLimitBreakManager.GetLBInfo(m_targetUnitData.m_LimitBreakLevel);
		NKMLimitBreakTemplet nKMLimitBreakTemplet = (m_targetLBTemplet = NKMUnitLimitBreakManager.GetLBInfo(m_targetUnitData.m_LimitBreakLevel + 1));
		UpdateSubstituteItemData();
		NKCScenManager.CurrentUserData();
		switch (unitLimitbreakStatus.Status)
		{
		case NKMUnitLimitBreakManager.UnitLimitBreakStatus.Invalid:
			m_comStarRankMaxLevel.SetStarRank(0, 0);
			m_targetLBTemplet = null;
			m_lbRequiredLevel.SetData(0, 0, 0L);
			LockLimitBreakButton(value: true);
			return;
		case NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max:
			m_comStarRankMaxLevel.SetStarRank(m_targetUnitData);
			m_comStarRankTCMaxLevel.SetStarRank(m_targetUnitData);
			m_targetLBTemplet = null;
			m_lbRequiredLevel.SetData(0, 0, 0L);
			LockLimitBreakButton(value: true);
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_targetUnitData.m_UnitID);
		if (unitLimitbreakStatus.Tier > 0)
		{
			bool flag2 = nKMLimitBreakTemplet == null || NKMUnitLimitBreakManager.GetLBInfo(m_targetUnitData.m_LimitBreakLevel + 2) == null;
			m_comStarRankTC?.SetStarRank(m_targetUnitData.m_LimitBreakLevel + 1, unitTempletBase.m_StarGradeMax);
			NKCUtil.SetGameobjectActive(m_objTranscendenceFxYellow, !flag2);
			NKCUtil.SetGameobjectActive(m_objTranscendenceFxPurple, flag2);
			if (lBInfo != null)
			{
				m_lbTCMaxLevelBefore.SetText(string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, lBInfo.m_iMaxLevel), NKMUnitLimitBreakManager.GetLBTier(m_targetUnitData));
			}
			if (nKMLimitBreakTemplet != null)
			{
				int num = nKMLimitBreakTemplet.m_iLBRank - 3;
				NKCUtil.SetLabelText(m_lbTCLevel, NKCUtilString.GET_STRING_LIMITBREAK_TRANSCENDENCE_LEVEL_ONE_PARAM, num);
				m_lbTCMaxLevelAfter.SetText(string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKMLimitBreakTemplet.m_iMaxLevel), NKMUnitLimitBreakManager.GetLBTier(m_targetUnitData.m_LimitBreakLevel + 1));
				m_lbRequiredLevel?.SetData(910, nKMLimitBreakTemplet.m_iRequiredLevel, m_targetUnitData.m_UnitLevel);
			}
			else
			{
				Debug.LogError("Next LBTemplet Not Found!");
				NKCUtil.SetLabelText(m_lbTCLevel, NKCUtilString.GET_STRING_LIMITBREAK_TRANSCENDENCE_LEVEL_ONE_PARAM, 0);
			}
			float num2 = NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(m_targetUnitData.m_LimitBreakLevel + 1) - NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(m_targetUnitData.m_LimitBreakLevel);
			NKCUtil.SetLabelText(m_lbTCPowerupRate, string.Format(NKCUtilString.GET_STRING_LIMITBREAK_GROWTH_INFO_ONE_PARAM, num2 * 100f));
		}
		else
		{
			m_comStarBefore.SetStarRank(m_targetUnitData.m_LimitBreakLevel, unitTempletBase.m_StarGradeMax);
			m_comStarAfter.SetStarRank(m_targetUnitData.m_LimitBreakLevel + 1, unitTempletBase.m_StarGradeMax);
			GameObject gameObject = m_comStarAfter.GetStarObjectImage(unitTempletBase.m_StarGradeMax - 3 + m_targetUnitData.m_LimitBreakLevel)?.gameObject;
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
				m_lbRequiredLevel?.SetData(910, nKMLimitBreakTemplet.m_iRequiredLevel, m_targetUnitData.m_UnitLevel);
			}
			float num3 = NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(m_targetUnitData.m_LimitBreakLevel + 1) - NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(m_targetUnitData.m_LimitBreakLevel);
			m_lbGrowthInfo.text = string.Format(NKCUtilString.GET_STRING_LIMITBREAK_GROWTH_INFO_ONE_PARAM, num3 * 100f);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_LAB_TRANSCENDENCE_INFO_DETAIL_02, m_targetUnitData.m_LimitBreakLevel + 1 == 3);
		List<NKMItemMiscData> lstCost;
		NKM_ERROR_CODE nKM_ERROR_CODE = CanLimitBreak(m_targetUnitData, out lstCost);
		LockLimitBreakButton(nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK);
	}

	public void UpdateSubstituteItemData()
	{
		SetSubstituteItemData(m_targetUnitData);
	}

	private void SetSubstituteItemData(NKMUnitData targetUnitData)
	{
		NKMLimitBreakItemTemplet lBSubstituteItemInfo = NKMUnitLimitBreakManager.GetLBSubstituteItemInfo(targetUnitData);
		bool flag = IsPossibleLimitBreakStatusUnit(targetUnitData);
		for (int i = 0; i < m_lstSubstituteItemUI.Count; i++)
		{
			NKCUIItemCostSlot nKCUIItemCostSlot = m_lstSubstituteItemUI[i];
			if (flag && lBSubstituteItemInfo != null && i < lBSubstituteItemInfo.m_lstRequiredItem.Count)
			{
				int itemID = lBSubstituteItemInfo.m_lstRequiredItem[i].itemID;
				if (m_targetLBTemplet != null)
				{
					int num = lBSubstituteItemInfo.m_lstRequiredItem[i].count * m_targetLBTemplet.m_iUnitRequirement;
					if (num > 0)
					{
						nKCUIItemCostSlot.SetData(itemID, num, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemID));
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
		if (lBSubstituteItemInfo != null && IsPossibleLimitBreakStatusUnit(targetUnitData))
		{
			m_lbCreditRequired.SetData(1, lBSubstituteItemInfo.m_CreditReq, NKCScenManager.CurrentUserData().GetCredit());
		}
		else
		{
			m_lbCreditRequired.SetData(0, 0, 0L);
		}
	}

	private bool IsPossibleLimitBreakStatusUnit(NKMUnitData unitData)
	{
		NKMUnitLimitBreakManager.UnitLimitBreakStatus status = NKMUnitLimitBreakManager.GetUnitLimitbreakStatus(unitData).Status;
		if (status == NKMUnitLimitBreakManager.UnitLimitBreakStatus.Invalid || status == NKMUnitLimitBreakManager.UnitLimitBreakStatus.Max)
		{
			return false;
		}
		return true;
	}

	private void LockLimitBreakButton(bool value)
	{
		if (value)
		{
			m_csbtnLimitBreak.Lock();
		}
		else
		{
			m_csbtnLimitBreak.UnLock();
		}
		foreach (Image item in m_lstEnterBtnIcon)
		{
			NKCUtil.SetImageColor(item, NKCUtil.GetButtonUIColor(!value));
		}
		foreach (Text item2 in m_lstEnterBtnText)
		{
			NKCUtil.SetLabelTextColor(item2, NKCUtil.GetButtonUIColor(!value));
		}
	}

	private void UpdateLimitBreakButton()
	{
		List<NKMItemMiscData> lstCost;
		NKM_ERROR_CODE nKM_ERROR_CODE = CanLimitBreak(m_targetUnitData, out lstCost);
		LockLimitBreakButton(nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK);
	}

	private NKM_ERROR_CODE CanLimitBreak(NKMUnitData targetUnit, out List<NKMItemMiscData> lstCost)
	{
		lstCost = new List<NKMItemMiscData>();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (targetUnit == null || nKMUserData == null || nKMUserData.m_ArmyData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (targetUnit.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED;
		}
		return nKMUserData.m_ArmyData.GetUnitDeckState(targetUnit) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKMUnitLimitBreakManager.CanLimitBreak(nKMUserData, targetUnit, out lstCost), 
		};
	}

	public void OnUnitUpdate(long uid, NKMUnitData unitData)
	{
		if (m_targetUnitData != null && uid == m_targetUnitData.m_UnitUID)
		{
			SetData(unitData);
		}
	}

	private void UpdateResource()
	{
		UpdateSubstituteItemData();
	}

	private void OnClickLimitBreak()
	{
		if (NKCUIUnitInfo.IsInstanceOpen && NKCUIUnitInfo.Instance.IsBlockedUnit())
		{
			return;
		}
		if (!m_csbtnLimitBreak.m_bLock)
		{
			if (m_targetUnitData != null)
			{
				RunLimitBreak();
			}
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null || m_targetUnitData == null)
		{
			return;
		}
		List<NKMItemMiscData> lstCost;
		NKM_ERROR_CODE nKM_ERROR_CODE = CanLimitBreak(m_targetUnitData, out lstCost);
		NKMLimitBreakItemTemplet lBSubstituteItemInfo = NKMUnitLimitBreakManager.GetLBSubstituteItemInfo(m_targetUnitData);
		if (lBSubstituteItemInfo == null)
		{
			return;
		}
		switch (nKM_ERROR_CODE)
		{
		case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM:
		{
			NKMLimitBreakTemplet lBInfo = NKMUnitLimitBreakManager.GetLBInfo(m_targetUnitData.m_LimitBreakLevel + 1);
			if (lBInfo == null || lBSubstituteItemInfo.m_lstRequiredItem == null)
			{
				break;
			}
			for (int i = 0; i < lBSubstituteItemInfo.m_lstRequiredItem.Count; i++)
			{
				int itemID = lBSubstituteItemInfo.m_lstRequiredItem[i].itemID;
				int num = lBSubstituteItemInfo.m_lstRequiredItem[i].count * lBInfo.m_iUnitRequirement;
				long num2 = 0L;
				if (nKMUserData != null)
				{
					num2 = nKMUserData.m_InventoryData.GetCountMiscItem(itemID);
				}
				if (num2 < num)
				{
					NKCShopManager.OpenItemLackPopup(itemID, num);
					break;
				}
			}
			break;
		}
		case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT:
			if (nKMUserData != null && nKMUserData.GetCredit() < lBSubstituteItemInfo.m_CreditReq)
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

	private void OnClickInformation()
	{
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_INFORMATION, NKCUtilString.GET_STRING_LIMITBTEAK_INFO);
	}

	private void OnClickMoveToRearmment()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.REARM))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.REARM);
			return;
		}
		NKCUIRearmament.Instance.SetReserveRearmData(m_iRearmTargetUnitID, m_targetUnitData.m_UnitUID);
		NKCUIRearmament.Instance.Open(NKCUIRearmament.REARM_TYPE.RT_PROCESS);
	}

	private void RunLimitBreak()
	{
		if (m_targetUnitData != null)
		{
			NKMUnitLimitBreakManager.UnitLimitBreakStatusData unitLimitbreakStatus = NKMUnitLimitBreakManager.GetUnitLimitbreakStatus(m_targetUnitData);
			List<NKCUISlot.SlotData> lstSlot = MakeSlotData(m_targetUnitData);
			string content = ((unitLimitbreakStatus.Tier <= 0) ? NKCUtilString.GET_STRING_LIMITBREAK_CONFIRM : NKCUtilString.GET_STRING_LIMITBREAK_CONFIRM_AWAKEN);
			NKCPopupResourceConfirmBox.Instance.OpenItemSlotList(NKCUtilString.GET_STRING_NOTICE, content, lstSlot, delegate
			{
				NKCPacketSender.Send_Packet_NKMPacket_LIMIT_BREAK_UNIT_REQ(m_targetUnitData.m_UnitUID);
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
				int num = itemRequirement.count * lBInfo.m_iUnitRequirement;
				if (num > 0)
				{
					NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeMiscItemData(itemRequirement.itemID, num);
					list.Add(item);
				}
			}
		}
		return list;
	}

	private void UpdateRearmUI(bool bShow)
	{
		if (!bShow)
		{
			NKCUtil.SetGameobjectActive(m_objRearmUI, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRearmUI, bValue: true);
		if (m_targetUnitData != null)
		{
			m_RearmSubUI.SetData(m_targetUnitData.m_UnitID, 0, 0L);
		}
	}

	private void SelectedRearmUnit(int unitID)
	{
		m_iRearmTargetUnitID = unitID;
	}

	private void UpdateTacticUpdateUI()
	{
		m_tacticUpdateLevelSlot.SetLevel(m_targetUnitData.tacticLevel);
	}

	private void OnClickTacticUpdate()
	{
		if (m_targetUnitData == null)
		{
			return;
		}
		NKM_UNIT_STYLE_TYPE nKM_UNIT_STYLE_TYPE = m_targetUnitData.GetUnitTempletBase().m_NKM_UNIT_STYLE_TYPE;
		if ((uint)(nKM_UNIT_STYLE_TYPE - 1) <= 2u)
		{
			List<NKMUnitData> lstUnitData = new List<NKMUnitData>();
			if (NKCUIUnitSelectList.IsInstanceOpen && m_GetUnitListCallBack != null)
			{
				lstUnitData = m_GetUnitListCallBack?.Invoke();
			}
			NKCUITacticUpdate.Instance.Open(m_targetUnitData, lstUnitData, m_LastSelectedUnitCallBack);
		}
	}
}
