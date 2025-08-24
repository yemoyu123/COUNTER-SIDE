using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Negotiation;
using NKC.UI.Trim;
using NKM;
using NKM.Unit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitInfoNegotiation : MonoBehaviour
{
	[Header("Outro")]
	public NKCComUITalkBox m_UIOutroTalkBox;

	[Header("레벨/경험치")]
	public GameObject m_objLevelPrev;

	public Text m_lbLevelBefore;

	public Text m_lbLevelAfter;

	public GameObject m_objAlreadyMaxLevel;

	public GameObject m_objExpParent;

	public Image m_imgExpBarAfter;

	public Image m_imgExpBarAfterAdd;

	public Text m_lbCurExpCount;

	public Text m_lbCurMaxExpCount;

	public Text m_lbEarnExpCount;

	[Header("애사심")]
	public Text m_lbLoyalityBefore;

	public Image m_imgLoyalityBarBefore;

	public Text m_lbLoyaltyEarn;

	public Text m_lbPermanentBonus;

	[Header("재료")]
	public List<Text> m_lstMaterialUseCount = new List<Text>(3);

	public List<NKCUIItemCostSlot> m_lstUIItemReq = new List<NKCUIItemCostSlot>(3);

	public List<Text> m_lstMaterialHaveCount = new List<Text>(3);

	public Text m_lbCreditReq;

	public GameObject m_objDiscountEvent;

	public Text m_lbDiscountEvent;

	public NKCUIComStateButton m_csbtnStart;

	private NKMUnitData m_TargetUnitData;

	public GameObject m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_BG_OFF;

	public Image m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_ICON;

	public Text m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_TEXT;

	[Header("재료")]
	public NKCUIComStateButton m_csbtnMaterial_1_Add;

	public NKCUIComStateButton m_csbtnMaterial_1_Minus;

	public NKCUIComStateButton m_csbtnMaterial_2_Add;

	public NKCUIComStateButton m_csbtnMaterial_2_Minus;

	public NKCUIComStateButton m_csbtnMaterial_3_Add;

	public NKCUIComStateButton m_csbtnMaterial_3_Minus;

	[Header("애니메이션")]
	public Animator m_LevelUpEffect;

	[Header("말풍선")]
	public Animator m_aniTalkBox;

	public NKCComUITalkBox m_TalkBox;

	[Space]
	public NKCUIComStateButton m_csbtn_NEGO_RATE_BUTTON;

	private Color TEXT_COLOR_YELLOW = new Color(1f, 69f / 85f, 0.23137255f);

	private List<MiscItemData> m_lstMaterials = new List<MiscItemData>(3);

	private bool m_bMaxLevelBreak;

	private int m_expectedLevel;

	private int m_expectedExp;

	private float m_currentExpPercent;

	private float m_expectedExpPercent;

	private bool m_bReservedLevelUpFx;

	private NEGOTIATE_RESULT m_NegotiateResult = NEGOTIATE_RESULT.COMPLETE;

	private bool m_bMaxLoyalty;

	private const int LOYALTY_MAX = 100;

	private float m_loyaltyFillAmount;

	private int m_earnLoyalty;

	private bool m_bLevelUp;

	private const string TALKBOX_PLAY_TRIGGER = "Speech";

	public const float PRESS_GAP_MAX = 0.3f;

	public const float PRESS_GAP_MIN = 0.01f;

	public const float DAMPING = 0.8f;

	private float m_fDelay = 0.5f;

	private float m_fHoldTime;

	private int m_iChangeValue;

	private bool m_bPress;

	private bool m_bWasHold;

	private int targetId;

	private string FX_SLOT_0 = "FX_LEVELUP_SLOT_0";

	private string FX_SLOT_1 = "FX_LEVELUP_SLOT_1";

	private string FX_SLOT_2 = "FX_LEVELUP_SLOT_2";

	private string BOOL_KEY_LEVELUP = "IsLevelup";

	private string BOOL_KEY_SUCCESS = "IsLevelupSuccess";

	public static NEGOTIATE_BOSS_SELECTION LastUserSelection { get; private set; }

	public void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnStart, OnStartButton);
		for (int i = 0; i < 3; i++)
		{
			if (i < NKMCommonConst.Negotiation.Materials.Count)
			{
				MiscItemData miscItemData = new MiscItemData();
				miscItemData.itemId = NKMCommonConst.Negotiation.Materials[i].ItemTemplet.m_ItemMiscID;
				miscItemData.count = 0;
				m_lstMaterials.Add(miscItemData);
			}
		}
		m_csbtnMaterial_1_Add.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_1_Add.PointerDown.AddListener(OnPlusDown_1);
		m_csbtnMaterial_1_Add.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_1_Add.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_2_Add.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_2_Add.PointerDown.AddListener(OnPlusDown_2);
		m_csbtnMaterial_2_Add.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_2_Add.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_3_Add.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_3_Add.PointerDown.AddListener(OnPlusDown_3);
		m_csbtnMaterial_3_Add.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_3_Add.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_1_Minus.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_1_Minus.PointerDown.AddListener(OnMinusDown_1);
		m_csbtnMaterial_1_Minus.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_1_Minus.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_2_Minus.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_2_Minus.PointerDown.AddListener(OnMinusDown_2);
		m_csbtnMaterial_2_Minus.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_2_Minus.PointerUp.AddListener(OnButtonUp);
		m_csbtnMaterial_3_Minus.PointerDown.RemoveAllListeners();
		m_csbtnMaterial_3_Minus.PointerDown.AddListener(OnMinusDown_3);
		m_csbtnMaterial_3_Minus.PointerUp.RemoveAllListeners();
		m_csbtnMaterial_3_Minus.PointerUp.AddListener(OnButtonUp);
		NKCUtil.SetBindFunction(m_csbtnMaterial_1_Add, delegate
		{
			OnChangeCount(NKMCommonConst.Negotiation.Materials[0].ItemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_2_Add, delegate
		{
			OnChangeCount(NKMCommonConst.Negotiation.Materials[1].ItemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_3_Add, delegate
		{
			OnChangeCount(NKMCommonConst.Negotiation.Materials[2].ItemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_1_Minus, delegate
		{
			OnChangeCount(NKMCommonConst.Negotiation.Materials[0].ItemId, bPlus: false);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_2_Minus, delegate
		{
			OnChangeCount(NKMCommonConst.Negotiation.Materials[1].ItemId, bPlus: false);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_3_Minus, delegate
		{
			OnChangeCount(NKMCommonConst.Negotiation.Materials[2].ItemId, bPlus: false);
		});
		if (m_csbtn_NEGO_RATE_BUTTON != null)
		{
			m_csbtn_NEGO_RATE_BUTTON.PointerDown.RemoveAllListeners();
			m_csbtn_NEGO_RATE_BUTTON.PointerDown.AddListener(OnButtonDownOptionRateInfo);
		}
		NKCUtil.SetGameobjectActive(m_csbtn_NEGO_RATE_BUTTON, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPEN_TAG_RATE_INFO));
	}

	public void OnDisable()
	{
		NKCUtil.SetGameobjectActive(m_TalkBox, bValue: false);
		ResetMaterialCount();
		RefreshNegotiateInfo(m_TargetUnitData);
		RefreshNegotiateUI(m_TargetUnitData);
	}

	public void SetData(NKMUnitData unitData, bool bSkipTalkBox = false)
	{
		bool flag = m_TargetUnitData != null && m_TargetUnitData.m_UnitUID != unitData.m_UnitUID;
		m_TargetUnitData = unitData;
		if (!bSkipTalkBox)
		{
			if (flag)
			{
				ResetMaterialCount();
			}
			SetTalkBox();
		}
		m_bMaxLevelBreak = false;
		m_bMaxLoyalty = false;
		m_bReservedLevelUpFx = false;
		m_bLevelUp = false;
		SetUIItemReq(unitData);
		ResetMaterialCount();
		RefreshNegotiateInfo(unitData);
		RefreshNegotiateUI(unitData);
		ShowRequestItem();
		NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT);
	}

	private void SetTalkBox()
	{
		if (m_TargetUnitData != null && !m_TargetUnitData.GetUnitTempletBase().IsTrophy)
		{
			if (m_TalkBox != null)
			{
				NKCNegotiateManager.SpeechType type = NKCNegotiateManager.SpeechType.Ready;
				if (m_bReservedLevelUpFx)
				{
					type = NKCNegotiateManager.GetSpeechType(m_NegotiateResult);
				}
				string speech = NKCNegotiateManager.GetSpeech(m_TargetUnitData, type, bCheckVoiceBundle: true);
				if (!string.IsNullOrEmpty(speech))
				{
					SetReserveTalkBox(speech);
					NKCUtil.SetGameobjectActive(m_TalkBox, bValue: true);
					m_aniTalkBox?.SetTrigger("Speech");
					m_TalkBox.ShowReservedText();
				}
				else
				{
					SetReserveTalkBox("");
					NKCUtil.SetGameobjectActive(m_TalkBox, bValue: false);
				}
			}
			OnPlayVoice(!m_bReservedLevelUpFx);
			OnPlayFX(m_bLevelUp);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_TalkBox, bValue: false);
		}
	}

	private void OnPlayVoice(bool bPlayReadyVoice)
	{
		if (bPlayReadyVoice)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_NEGOTITATE_READY, m_TargetUnitData);
			return;
		}
		switch (m_NegotiateResult)
		{
		case NEGOTIATE_RESULT.SUCCESS:
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_NEGOTITATE_SUCCESS_GREAT, m_TargetUnitData);
			break;
		case NEGOTIATE_RESULT.COMPLETE:
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_NEGOTITATE_SUCCESS, m_TargetUnitData);
			break;
		}
	}

	public void ReserveUnitData(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			m_TargetUnitData = unitData;
		}
	}

	private void SetUIItemReq(NKMUnitData unitData)
	{
		if (m_lstUIItemReq == null)
		{
			return;
		}
		if (unitData != null)
		{
			for (int i = 0; i < m_lstUIItemReq.Count; i++)
			{
				NKCUtil.SetLabelText(m_lstMaterialUseCount[i], m_lstMaterials[i].count.ToString());
				m_lstUIItemReq[i].SetData(m_lstMaterials[i].itemId, 0, 0L, bShowTooltip: false, bShowBG: false);
				NKCUtil.SetLabelText(m_lstMaterialHaveCount[i], NKCUtilString.GET_STRING_HAVE_COUNT_ONE_PARAM, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_lstMaterials[i].itemId));
			}
		}
		else
		{
			for (int j = 0; j < m_lstUIItemReq.Count; j++)
			{
				NKCUtil.SetLabelText(m_lstMaterialUseCount[j], "0");
				m_lstUIItemReq[j].SetData(m_lstMaterials[j].itemId, 0, 0L, bShowTooltip: false, bShowBG: false);
				NKCUtil.SetLabelText(m_lstMaterialHaveCount[j], NKCUtilString.GET_STRING_HAVE_COUNT_ONE_PARAM, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_lstMaterials[j].itemId));
			}
		}
	}

	public void ShowRequestItem()
	{
		for (int i = 0; i < m_lstUIItemReq.Count; i++)
		{
			if (i < m_lstMaterialUseCount.Count && i < m_lstMaterialHaveCount.Count && i < m_lstMaterials.Count)
			{
				NKCUtil.SetLabelText(m_lstMaterialUseCount[i], m_lstMaterials[i].count.ToString());
				NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_lstMaterials[i].itemId);
				m_lstUIItemReq[i].SetData(m_lstMaterials[i].itemId, 0, 0L, bShowTooltip: false, bShowBG: false);
				NKCUtil.SetLabelText(m_lstMaterialHaveCount[i], NKCUtilString.GET_STRING_HAVE_COUNT_ONE_PARAM, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_lstMaterials[i].itemId));
			}
		}
	}

	private void ResetMaterialCount()
	{
		for (int i = 0; i < m_lstMaterials.Count; i++)
		{
			m_lstMaterials[i].count = 0;
		}
	}

	private void RefreshNegotiateInfo(NKMUnitData unitData)
	{
		CalcExpChange(unitData);
		CalcLoyaltyChange(unitData);
	}

	private void RefreshNegotiateUI(NKMUnitData unitData)
	{
		UpdateExpUI(unitData);
		UpdateLoyaltyUI(unitData);
		SetCredit(unitData);
		ShowRequestItem();
		UpdateStartButton();
		UpdateMaterialInfo();
	}

	private void CalcLoyaltyChange(NKMUnitData unitData)
	{
		int num = 0;
		m_earnLoyalty = 0;
		if (unitData != null)
		{
			num = unitData.loyalty / 100;
			int num2 = Math.Min(100, (unitData.loyalty + NKCNegotiateManager.GetNegotiateLoyalty(m_lstMaterials)) / 100);
			m_earnLoyalty = num2 - num;
		}
		m_loyaltyFillAmount = (float)(num + m_earnLoyalty) / 100f;
		if (num + m_earnLoyalty >= 100)
		{
			if (m_iChangeValue > 0 && !m_bMaxLoyalty)
			{
				UpdateLoyaltyUI(unitData);
				m_bPress = false;
			}
			m_bMaxLoyalty = true;
		}
		else
		{
			m_bMaxLoyalty = false;
		}
	}

	private void UpdateLoyaltyUI(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			NKCUtil.SetLabelText(m_lbLoyalityBefore, $"{0} / {100}");
			NKCUtil.SetLabelText(m_lbLoyaltyEarn, $"+{0}");
			NKCUtil.SetGameobjectActive(m_lbLoyaltyEarn, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbPermanentBonus, bValue: false);
			if (m_imgLoyalityBarBefore != null)
			{
				m_imgLoyalityBarBefore.fillAmount = 0f;
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbLoyalityBefore, $"{unitData.loyalty / 100} / {100}");
			if (m_imgLoyalityBarBefore != null)
			{
				m_imgLoyalityBarBefore.fillAmount = m_loyaltyFillAmount;
			}
			NKCUtil.SetLabelText(m_lbLoyaltyEarn, $"+{m_earnLoyalty}");
			NKCUtil.SetGameobjectActive(m_lbLoyaltyEarn, !unitData.IsPermanentContract);
			NKCUtil.SetGameobjectActive(m_lbPermanentBonus, unitData.IsPermanentContract);
		}
	}

	private void CalcExpChange(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			UpdateExpUI(unitData);
			return;
		}
		NKMUnitExpTemplet unitExpTemplet = NKCExpManager.GetUnitExpTemplet(unitData);
		NKCExpManager.CalculateFutureUnitExpAndLevel(unitData, NKCNegotiateManager.GetNegotiateExp(m_lstMaterials, unitData.IsPermanentContract), out m_expectedLevel, out m_expectedExp);
		NKMUnitExpTemplet nKMUnitExpTemplet = NKMUnitExpTemplet.FindByUnitId(unitData.m_UnitID, m_expectedLevel);
		m_currentExpPercent = (float)unitData.m_iUnitLevelEXP / (float)unitExpTemplet.m_iExpRequired;
		m_expectedExpPercent = (float)m_expectedExp / (float)nKMUnitExpTemplet.m_iExpRequired;
		int num = NKCExpManager.CalculateNeedExpForUnitMaxLevel(unitData);
		if (NKCNegotiateManager.GetNegotiateExp(m_lstMaterials, unitData.IsPermanentContract) >= num)
		{
			if (m_iChangeValue > 0 && !m_bMaxLevelBreak)
			{
				UpdateExpUI(unitData);
				m_bPress = false;
			}
			m_bMaxLevelBreak = true;
		}
		else
		{
			m_bMaxLevelBreak = false;
		}
	}

	private void UpdateExpUI(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			NKCUtil.SetGameobjectActive(m_objLevelPrev, bValue: true);
			NKCUtil.SetGameobjectActive(m_objAlreadyMaxLevel, bValue: false);
			NKCUtil.SetGameobjectActive(m_objExpParent, bValue: true);
			NKCUtil.SetLabelText(m_lbLevelBefore, "-");
			NKCUtil.SetLabelText(m_lbLevelAfter, "-");
			NKCUtil.SetLabelText(m_lbCurExpCount, "-");
			NKCUtil.SetLabelText(m_lbCurMaxExpCount, "/ -");
			NKCUtil.SetLabelText(m_lbEarnExpCount, "");
			if (m_imgExpBarAfter != null)
			{
				m_imgExpBarAfter.fillAmount = 0f;
			}
			if (m_imgExpBarAfterAdd != null)
			{
				m_imgExpBarAfterAdd.fillAmount = 0f;
			}
			return;
		}
		NKMUnitExpTemplet unitExpTemplet = NKCExpManager.GetUnitExpTemplet(unitData);
		int num = NKCExpManager.CalculateNeedExpForUnitMaxLevel(unitData);
		int num2 = NKCNegotiateManager.GetNegotiateExp(m_lstMaterials, unitData.IsPermanentContract);
		if (num2 >= num)
		{
			num2 = num;
		}
		NKCUtil.SetLabelText(m_lbLevelAfter, m_expectedLevel.ToString());
		bool flag = unitData.m_UnitLevel == NKCExpManager.GetUnitMaxLevel(unitData);
		NKCUtil.SetGameobjectActive(m_objLevelPrev, !flag);
		NKCUtil.SetGameobjectActive(m_objAlreadyMaxLevel, flag);
		NKCUtil.SetGameobjectActive(m_objExpParent, !flag);
		if (flag)
		{
			NKCUtil.SetLabelText(m_lbEarnExpCount, "");
			NKCUtil.SetLabelTextColor(m_lbEarnExpCount, TEXT_COLOR_YELLOW);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbLevelBefore, unitData.m_UnitLevel.ToString());
			NKCUtil.SetLabelText(m_lbCurExpCount, (unitData.m_iUnitLevelEXP + num2).ToString());
			NKCUtil.SetLabelText(m_lbCurMaxExpCount, $"/ {unitExpTemplet.m_iExpRequired}");
			NKCUtil.SetLabelText(m_lbEarnExpCount, string.Format(NKCUtilString.GET_STRING_EXP_PLUS_ONE_PARAM, num2));
			if (m_expectedLevel == NKCExpManager.GetUnitMaxLevel(unitData))
			{
				NKCUtil.SetLabelTextColor(m_lbEarnExpCount, Color.red);
				m_expectedExpPercent = 1f;
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_lbEarnExpCount, TEXT_COLOR_YELLOW);
			}
		}
		if (m_imgExpBarAfterAdd != null)
		{
			m_imgExpBarAfterAdd.fillAmount = m_expectedExpPercent;
		}
		if (m_expectedLevel > unitData.m_UnitLevel)
		{
			if (m_imgExpBarAfter != null)
			{
				m_imgExpBarAfter.fillAmount = 1f;
			}
			if (m_imgExpBarAfterAdd != null)
			{
				m_imgExpBarAfterAdd.transform.SetAsLastSibling();
			}
		}
		else if (m_imgExpBarAfter != null)
		{
			m_imgExpBarAfter.fillAmount = m_currentExpPercent;
			m_imgExpBarAfter.transform.SetAsLastSibling();
		}
	}

	private void SetCredit(NKMUnitData unitData)
	{
		if (!(m_lbCreditReq == null))
		{
			NKCUtil.SetGameobjectActive(m_objDiscountEvent, NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT));
			if (m_objDiscountEvent != null && m_objDiscountEvent.activeSelf)
			{
				NKCUtil.SetLabelText(m_lbDiscountEvent, string.Format(NKCStringTable.GetString("SI_DP_EVENT_BUFF_LABEL_NEGOTIATION_CREDIT_DISCOUNT_DESC"), NKCCompanyBuff.GetTotalRatio(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT)));
			}
			if (unitData != null)
			{
				NKCUtil.SetLabelText(m_lbCreditReq, NKCNegotiateManager.GetNegotiateSalary(m_lstMaterials).ToString("N0"));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbCreditReq, "0");
			}
		}
	}

	private void OnStartButton()
	{
		if (m_TargetUnitData == null || (NKCUIUnitInfo.IsInstanceOpen && NKCUIUnitInfo.Instance.IsBlockedUnit()))
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < m_lstMaterials.Count; i++)
		{
			if (m_lstMaterials[i].count > 0)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_NOT_ENOUGH_NEGOTIATE_MATERIALS);
		}
		else
		{
			StartNegotiate(m_lstMaterials);
		}
	}

	private void UpdateStartButton()
	{
		bool flag = false;
		for (int i = 0; i < m_lstMaterials.Count; i++)
		{
			if (m_lstMaterials[i].count > 0)
			{
				flag = true;
				break;
			}
		}
		if (flag && NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(1) < NKCNegotiateManager.GetNegotiateSalary(m_lstMaterials))
		{
			flag = false;
		}
		m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_TEXT.color = NKCUtil.GetButtonUIColor(flag && m_TargetUnitData != null);
		m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_ICON.color = NKCUtil.GetButtonUIColor(flag && m_TargetUnitData != null);
		if (!flag)
		{
			m_csbtnStart.Lock();
		}
		else if (NKCNegotiateManager.CanStartNegotiate(NKCScenManager.CurrentUserData(), m_TargetUnitData, NEGOTIATE_BOSS_SELECTION.OK, m_lstMaterials) == NKM_ERROR_CODE.NEC_OK)
		{
			m_csbtnStart.UnLock();
		}
		else
		{
			m_csbtnStart.Lock();
		}
	}

	private void UpdateMaterialInfo()
	{
		NKCUtil.SetGameobjectActive(m_csbtnMaterial_1_Minus, m_lstMaterials[0].count > 0);
		NKCUtil.SetGameobjectActive(m_csbtnMaterial_2_Minus, m_lstMaterials[1].count > 0);
		NKCUtil.SetGameobjectActive(m_csbtnMaterial_3_Minus, m_lstMaterials[2].count > 0);
		for (int i = 0; i < 3; i++)
		{
			if (!(m_lstMaterialUseCount[i] == null) && m_lstMaterials[i] != null)
			{
				NKCUtil.SetGameobjectActive(m_lstMaterialUseCount[i], m_lstMaterials[i].count > 0);
			}
		}
	}

	public void OnUnitUpdate(long uid, NKMUnitData unitData)
	{
		if (m_TargetUnitData != null && uid == m_TargetUnitData.m_UnitUID)
		{
			SetData(unitData, bSkipTalkBox: true);
		}
	}

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (itemData.ItemID == 1)
		{
			SetCredit(m_TargetUnitData);
			UpdateStartButton();
		}
		else
		{
			ShowRequestItem();
			UpdateStartButton();
		}
	}

	public void OnCompanyBuffChanged()
	{
		SetCredit(m_TargetUnitData);
		ShowRequestItem();
		UpdateStartButton();
	}

	public void OnButtonDownOptionRateInfo(PointerEventData pointEventData)
	{
		NKCUITrimToolTip.Instance.Open(NKCStringTable.GetString("SI_PF_PERSONNEL_NEGOTIATE_RATE_INFO"), pointEventData.position, 32);
	}

	private void OnMinusDown_1(PointerEventData eventData)
	{
		if (!NKCUIUnitInfo.IsInstanceOpen || !NKCUIUnitInfo.Instance.IsBlockedUnit())
		{
			targetId = NKMCommonConst.Negotiation.Materials[0].ItemId;
			m_iChangeValue = -1;
			m_bPress = true;
			m_fDelay = 0.3f;
			m_fHoldTime = 0f;
			m_bWasHold = false;
			m_bMaxLevelBreak = false;
		}
	}

	private void OnPlusDown_1(PointerEventData eventData)
	{
		if (!NKCUIUnitInfo.IsInstanceOpen || !NKCUIUnitInfo.Instance.IsBlockedUnit())
		{
			targetId = NKMCommonConst.Negotiation.Materials[0].ItemId;
			m_iChangeValue = 1;
			m_bPress = true;
			m_fDelay = 0.3f;
			m_fHoldTime = 0f;
			m_bWasHold = false;
		}
	}

	private void OnMinusDown_2(PointerEventData eventData)
	{
		if (!NKCUIUnitInfo.IsInstanceOpen || !NKCUIUnitInfo.Instance.IsBlockedUnit())
		{
			targetId = NKMCommonConst.Negotiation.Materials[1].ItemId;
			m_iChangeValue = -1;
			m_bPress = true;
			m_fDelay = 0.3f;
			m_fHoldTime = 0f;
			m_bWasHold = false;
			m_bMaxLevelBreak = false;
		}
	}

	private void OnPlusDown_2(PointerEventData eventData)
	{
		if (!NKCUIUnitInfo.IsInstanceOpen || !NKCUIUnitInfo.Instance.IsBlockedUnit())
		{
			targetId = NKMCommonConst.Negotiation.Materials[1].ItemId;
			m_iChangeValue = 1;
			m_bPress = true;
			m_fDelay = 0.3f;
			m_fHoldTime = 0f;
			m_bWasHold = false;
		}
	}

	private void OnMinusDown_3(PointerEventData eventData)
	{
		if (!NKCUIUnitInfo.IsInstanceOpen || !NKCUIUnitInfo.Instance.IsBlockedUnit())
		{
			targetId = NKMCommonConst.Negotiation.Materials[2].ItemId;
			m_iChangeValue = -1;
			m_bPress = true;
			m_fDelay = 0.3f;
			m_fHoldTime = 0f;
			m_bWasHold = false;
			m_bMaxLevelBreak = false;
		}
	}

	private void OnPlusDown_3(PointerEventData eventData)
	{
		if (!NKCUIUnitInfo.IsInstanceOpen || !NKCUIUnitInfo.Instance.IsBlockedUnit())
		{
			targetId = NKMCommonConst.Negotiation.Materials[2].ItemId;
			m_iChangeValue = 1;
			m_bPress = true;
			m_fDelay = 0.3f;
			m_fHoldTime = 0f;
			m_bWasHold = false;
		}
	}

	private void OnButtonUp()
	{
		m_iChangeValue = 0;
		m_fDelay = 0.3f;
		m_bPress = false;
	}

	public void OnUpdateButtonHold()
	{
		if (!m_bPress || m_TargetUnitData == null || (m_bMaxLevelBreak && m_bMaxLoyalty && m_iChangeValue > 0))
		{
			return;
		}
		m_fHoldTime += Time.deltaTime;
		if (m_fHoldTime >= m_fDelay)
		{
			m_fHoldTime = 0f;
			m_fDelay *= 0.8f;
			int num = ((!(m_fDelay < 0.01f)) ? 1 : 5);
			m_fDelay = Mathf.Clamp(m_fDelay, 0.01f, 0.3f);
			MiscItemData miscItemData = m_lstMaterials.Find((MiscItemData e) => e.itemId == targetId);
			if (miscItemData == null)
			{
				return;
			}
			m_bWasHold = true;
			for (int num2 = 0; num2 < num; num2++)
			{
				if (!m_bPress)
				{
					break;
				}
				if (m_bMaxLevelBreak && m_bMaxLoyalty)
				{
					break;
				}
				miscItemData.count += m_iChangeValue;
				if (m_iChangeValue < 0 && miscItemData.count < 0)
				{
					miscItemData.count = 0;
					m_bPress = false;
				}
				if (m_iChangeValue > 0)
				{
					int notSelectedTotalCount = 0;
					m_lstMaterials.ForEach(delegate(MiscItemData x)
					{
						if (x.itemId != targetId)
						{
							notSelectedTotalCount += x.count;
						}
					});
					int val = Math.Max(0, NKMCommonConst.Negotiation.MaxMaterialUsageLimit - notSelectedTotalCount);
					val = Math.Min(val, (int)NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(miscItemData.itemId));
					if (miscItemData.count >= val)
					{
						miscItemData.count = val;
						m_bPress = false;
					}
				}
				RefreshNegotiateInfo(m_TargetUnitData);
			}
		}
		RefreshNegotiateUI(m_TargetUnitData);
	}

	public void OnChangeCount(int targetItemId, bool bPlus = true)
	{
		targetId = targetItemId;
		if (m_bWasHold)
		{
			m_bWasHold = false;
		}
		else
		{
			if (m_TargetUnitData == null || (NKCUIUnitInfo.IsInstanceOpen && NKCUIUnitInfo.Instance.IsBlockedUnit()))
			{
				return;
			}
			if (m_bMaxLevelBreak && m_bMaxLoyalty)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_MAX_LEVEL_LOYALTY);
				return;
			}
			MiscItemData miscItemData = m_lstMaterials.Find((MiscItemData e) => e.itemId == targetId);
			if (miscItemData == null)
			{
				return;
			}
			if (bPlus)
			{
				int notSelectedTotalCount = 0;
				m_lstMaterials.ForEach(delegate(MiscItemData x)
				{
					if (x.itemId != targetId)
					{
						notSelectedTotalCount += x.count;
					}
				});
				int val = Math.Max(0, NKMCommonConst.Negotiation.MaxMaterialUsageLimit - notSelectedTotalCount);
				val = Math.Min(val, (int)NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(miscItemData.itemId));
				if (miscItemData.count >= val)
				{
					if (miscItemData.count + notSelectedTotalCount >= NKMCommonConst.Negotiation.MaxMaterialUsageLimit)
					{
						NKCPopupMessageManager.AddPopupMessage(NKM_ERROR_CODE.NEC_FAIL_NEGOTIATION_INVALID_MATERIAL_COUNT);
					}
					else
					{
						NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_NOT_ENOUGH_NEGOTIATE_MATERIALS);
					}
					return;
				}
			}
			else
			{
				int num = 0;
				if (miscItemData.count <= num)
				{
					return;
				}
			}
			miscItemData.count += (bPlus ? 1 : (-1));
			RefreshNegotiateInfo(m_TargetUnitData);
			RefreshNegotiateUI(m_TargetUnitData);
		}
	}

	public void Clear()
	{
		m_TargetUnitData = null;
	}

	private void OnPlayFX(bool bLevelUp)
	{
		if (bLevelUp)
		{
			NKCSoundManager.PlaySound("FX_UI_ACCOUNT_LEVEL_UP", 1f, 0f, 0f);
		}
		m_LevelUpEffect.SetBool(BOOL_KEY_LEVELUP, bLevelUp);
		bool flag = false;
		for (int i = 0; i < m_lstMaterials.Count; i++)
		{
			if (m_lstMaterials[i] != null)
			{
				if (m_lstMaterials[i].count > 0 && i == 0)
				{
					flag = true;
					m_LevelUpEffect.SetTrigger(FX_SLOT_0);
				}
				if (m_lstMaterials[i].count > 0 && i == 1)
				{
					flag = true;
					m_LevelUpEffect.SetTrigger(FX_SLOT_1);
				}
				if (m_lstMaterials[i].count > 0 && i == 2)
				{
					flag = true;
					m_LevelUpEffect.SetTrigger(FX_SLOT_2);
				}
			}
		}
		if (flag)
		{
			NKCSoundManager.PlaySound("FX_UI_NEGOTIATE_SUCCESS_02", 1f, 0f, 0f);
		}
		if (m_NegotiateResult == NEGOTIATE_RESULT.SUCCESS)
		{
			m_LevelUpEffect.SetTrigger(BOOL_KEY_SUCCESS);
			m_NegotiateResult = NEGOTIATE_RESULT.COMPLETE;
		}
	}

	private void SetReserveTalkBox(string text)
	{
		m_TalkBox.ReserveText(text);
	}

	private void StartNegotiate(List<MiscItemData> lstMaterials)
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKCNegotiateManager.CanStartNegotiate(NKCScenManager.CurrentUserData(), m_TargetUnitData, NEGOTIATE_BOSS_SELECTION.OK, lstMaterials);
		switch (nKM_ERROR_CODE)
		{
		case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_ITEM:
			NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
			break;
		case NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT:
		{
			long negotiateSalary = NKCNegotiateManager.GetNegotiateSalary(lstMaterials);
			NKCShopManager.OpenItemLackPopup(1, (int)negotiateSalary);
			break;
		}
		case NKM_ERROR_CODE.NEC_OK:
		{
			m_lstMaterials = lstMaterials;
			int unitMaxLevel = NKCExpManager.GetUnitMaxLevel(m_TargetUnitData);
			if (m_TargetUnitData.m_UnitLevel >= unitMaxLevel)
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_NEGOTIATE_LEVEL_MAX, delegate
				{
					NKCPacketSender.Send_NKMPacket_NEGOTIATE_REQ2(m_TargetUnitData, NEGOTIATE_BOSS_SELECTION.OK, m_lstMaterials);
				});
				break;
			}
			NKCExpManager.CalculateFutureUnitExpAndLevel(m_TargetUnitData, NKCNegotiateManager.GetNegotiateExp(m_lstMaterials, m_TargetUnitData.IsPermanentContract), out var Level, out var _);
			if (Level >= unitMaxLevel)
			{
				int num = NKCExpManager.CalculateNeedExpForUnitMaxLevel(m_TargetUnitData);
				int num2 = 0;
				for (int num3 = 0; num3 < m_lstMaterials.Count; num3++)
				{
					if (m_lstMaterials[num3].count <= 0)
					{
						continue;
					}
					for (int num4 = 0; num4 < NKMCommonConst.Negotiation.Materials.Count; num4++)
					{
						if (NKMCommonConst.Negotiation.Materials[num4].ItemId == m_lstMaterials[num3].itemId)
						{
							num2 += NKMCommonConst.Negotiation.Materials[num4].Exp * m_lstMaterials[num3].count;
							break;
						}
					}
				}
				if (m_TargetUnitData.IsPermanentContract)
				{
					num2 = num2 * 120 / 100;
				}
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, string.Format(NKCUtilString.GET_STRING_NEGOTIATE_OVER_MAX_LEVEL_ONE_PARAM, num2 - num), delegate
				{
					NKCPacketSender.Send_NKMPacket_NEGOTIATE_REQ2(m_TargetUnitData, NEGOTIATE_BOSS_SELECTION.OK, m_lstMaterials);
				});
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_NEGOTIATE_REQ2(m_TargetUnitData, NEGOTIATE_BOSS_SELECTION.OK, m_lstMaterials);
			}
			break;
		}
		}
	}

	public void OnCompanyBuffUpdate()
	{
		OnCompanyBuffChanged();
	}

	public void RefreshUIForReconnect()
	{
		if (m_TargetUnitData != null)
		{
			m_TargetUnitData = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(m_TargetUnitData.m_UnitUID);
		}
	}

	public void ReserveLevelUpFx(NKCNegotiateManager.NegotiateResultUIData negotiateResult)
	{
		m_bReservedLevelUpFx = true;
		m_NegotiateResult = negotiateResult.NegotiateResult;
		m_bLevelUp = negotiateResult.UnitLevelBefore < negotiateResult.UnitLevelAfter;
	}
}
