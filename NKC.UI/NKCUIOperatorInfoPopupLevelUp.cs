using System;
using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorInfoPopupLevelUp : MonoBehaviour
{
	public delegate void OnStart(List<MiscItemData> lstMaterials);

	public Text m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_PREV_LV_COUNT;

	public Text NKM_UI_PERSONNEL_NEGOTIATE_UNIT_NEXT_LV_COUNT;

	public Image m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE;

	public Image m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE_UP;

	public Text m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_EXPTEXT;

	public Text m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_EXPTEXT_1;

	public Text m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_EXPTEXT_2;

	public Text m_NKM_UI_PERSONNEL_NEGOTIATE_RESULT_EXP_COUNT;

	public GameObject m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_LEVELMAX_TEXT;

	public Text m_STAT_HP_NUMBER;

	public Text m_STAT_ATT_NUMBER;

	public Text m_STAT_DEF_NUMBER;

	public Text m_STAT_SKILL_COOL_NUMBER;

	public GameObject m_NKM_UI_OPERATOR_INFO_DESC_BUTTON;

	public NKCUIComStateButton m_BUTTON_SKILLUP;

	public NKCUIComStateButton m_BUTTON_LEVELUP;

	[Header("재료")]
	public NKCUIComStateButton m_csbtnMaterial_1_Add;

	public NKCUIComStateButton m_csbtnMaterial_1_Minus;

	public NKCUIComStateButton m_csbtnMaterial_2_Add;

	public NKCUIComStateButton m_csbtnMaterial_2_Minus;

	public NKCUIComStateButton m_csbtnMaterial_3_Add;

	public NKCUIComStateButton m_csbtnMaterial_3_Minus;

	[Space]
	public List<Text> m_lstMaterialUseCount = new List<Text>(3);

	public List<NKCUIItemCostSlot> m_lstUIItemReq = new List<NKCUIItemCostSlot>(3);

	public List<Text> m_lstMaterialHaveCount = new List<Text>(3);

	public Text m_COUNT;

	public GameObject m_NKM_UI_OPERATOR_INFO_NEGOTIATE_REQUIRE;

	public NKCUIComStateButton m_NKM_UI_OPERATOR_INFO_NEGOTIATE_REQUIRE_START;

	public GameObject m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_BG_OFF;

	public Image m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_ICON;

	public Text m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_TEXT;

	[Header("애니메이션")]
	public Animator m_LevelUpEffect;

	private NKMOperator m_OperatorData;

	private OnStart dOnStart;

	private Color TEXT_COLOR_YELLOW = new Color(1f, 69f / 85f, 0.23137255f);

	private List<MiscItemData> m_lstMaterials = new List<MiscItemData>(3);

	private bool m_bMaxLevelBreak;

	private int m_iExpectLevel;

	private int m_iEarnExp;

	private bool m_bWaitResultPacket;

	private string FX_SLOT_0 = "FX_LEVELUP_SLOT_0";

	private string FX_SLOT_1 = "FX_LEVELUP_SLOT_1";

	private string FX_SLOT_2 = "FX_LEVELUP_SLOT_2";

	private string BOOL_KEY_LEVELUP = "IsLevelup";

	public const float PRESS_GAP_MAX = 0.35f;

	public const float PRESS_GAP_MIN = 0.01f;

	public const float DAMPING = 0.8f;

	private float m_fDelay = 0.5f;

	private float m_fHoldTime;

	private int m_iChangeValue;

	private bool m_bPress;

	private bool m_bWasHold;

	private int targetId;

	public void Init(OnStart onStart)
	{
		NKCUtil.SetBindFunction(m_NKM_UI_OPERATOR_INFO_NEGOTIATE_REQUIRE_START, OnConfirm);
		dOnStart = onStart;
		for (int i = 0; i < NKMCommonConst.OperatorConstTemplet.list.Length; i++)
		{
			MiscItemData miscItemData = new MiscItemData();
			miscItemData.itemId = NKMCommonConst.OperatorConstTemplet.list[i].itemId;
			miscItemData.count = 0;
			m_lstMaterials.Add(miscItemData);
		}
		if (m_lstMaterials.Count < 3)
		{
			Debug.LogError("오퍼레이터 연봉 협상의 소모 재화가 부족합니다.");
			return;
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
			OnChangeCount(m_lstMaterials[0].itemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_2_Add, delegate
		{
			OnChangeCount(m_lstMaterials[1].itemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_3_Add, delegate
		{
			OnChangeCount(m_lstMaterials[2].itemId);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_1_Minus, delegate
		{
			OnChangeCount(m_lstMaterials[0].itemId, bPlus: false);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_2_Minus, delegate
		{
			OnChangeCount(m_lstMaterials[1].itemId, bPlus: false);
		});
		NKCUtil.SetBindFunction(m_csbtnMaterial_3_Minus, delegate
		{
			OnChangeCount(m_lstMaterials[2].itemId, bPlus: false);
		});
	}

	public void SetData(NKMOperator operatorData, bool bShowLevelUpFX = false)
	{
		if (operatorData != null)
		{
			m_bWaitResultPacket = false;
			if (bShowLevelUpFX && m_OperatorData != null && m_OperatorData.uid == operatorData.uid)
			{
				OnPlayFX(m_OperatorData.level < operatorData.level);
			}
			m_OperatorData = operatorData;
			ResetMaterialCount();
			RefreshData();
			RefreshUI();
		}
	}

	public void ResetResourceIcon()
	{
		for (int i = 0; i < m_lstUIItemReq.Count; i++)
		{
			if (m_lstMaterials.Count < i)
			{
				m_lstUIItemReq[i].SetData(m_lstMaterials[i].itemId, 0, 0L, bShowTooltip: false, bShowBG: false);
			}
		}
	}

	public void Refresh()
	{
		if (m_OperatorData != null)
		{
			ResetMaterialCount();
			RefreshData();
			RefreshUI();
		}
	}

	private void ResetMaterialCount()
	{
		foreach (MiscItemData lstMaterial in m_lstMaterials)
		{
			lstMaterial.count = 0;
		}
	}

	private void RefreshData()
	{
		m_bMaxLevelBreak = false;
		UpdateExpData();
	}

	private void RefreshUI()
	{
		UpdateCredit();
		ShowRequestItem();
		UpdateExpUI();
		UpdateStat();
		UpdateStartButton();
	}

	private void UpdateStat()
	{
		if (m_OperatorData != null)
		{
			string statPercentageString = NKCOperatorUtil.GetStatPercentageString(m_OperatorData.id, m_OperatorData.level, NKM_STAT_TYPE.NST_HP);
			string statPercentageString2 = NKCOperatorUtil.GetStatPercentageString(m_OperatorData.id, m_OperatorData.level, NKM_STAT_TYPE.NST_ATK);
			string statPercentageString3 = NKCOperatorUtil.GetStatPercentageString(m_OperatorData.id, m_OperatorData.level, NKM_STAT_TYPE.NST_DEF);
			string statPercentageString4 = NKCOperatorUtil.GetStatPercentageString(m_OperatorData.id, m_OperatorData.level, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE);
			if (m_iExpectLevel > m_OperatorData.level)
			{
				string statPercentageString5 = NKCOperatorUtil.GetStatPercentageString(m_OperatorData.id, m_iExpectLevel, NKM_STAT_TYPE.NST_HP);
				string statPercentageString6 = NKCOperatorUtil.GetStatPercentageString(m_OperatorData.id, m_iExpectLevel, NKM_STAT_TYPE.NST_ATK);
				string statPercentageString7 = NKCOperatorUtil.GetStatPercentageString(m_OperatorData.id, m_iExpectLevel, NKM_STAT_TYPE.NST_DEF);
				string statPercentageString8 = NKCOperatorUtil.GetStatPercentageString(m_OperatorData.id, m_iExpectLevel, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE);
				NKCUtil.SetLabelText(m_STAT_HP_NUMBER, statPercentageString + " > " + statPercentageString5);
				NKCUtil.SetLabelText(m_STAT_ATT_NUMBER, statPercentageString2 + " > " + statPercentageString6);
				NKCUtil.SetLabelText(m_STAT_DEF_NUMBER, statPercentageString3 + " > " + statPercentageString7);
				NKCUtil.SetLabelText(m_STAT_SKILL_COOL_NUMBER, statPercentageString4 + " > " + statPercentageString8);
			}
			else
			{
				NKCUtil.SetLabelText(m_STAT_HP_NUMBER, statPercentageString ?? "");
				NKCUtil.SetLabelText(m_STAT_ATT_NUMBER, statPercentageString2 ?? "");
				NKCUtil.SetLabelText(m_STAT_DEF_NUMBER, statPercentageString3 ?? "");
				NKCUtil.SetLabelText(m_STAT_SKILL_COOL_NUMBER, statPercentageString4 ?? "");
			}
		}
	}

	private void UpdateExpData()
	{
		if (m_OperatorData == null)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_OperatorData.id);
		m_iExpectLevel = 0;
		int num = NKCOperatorUtil.CalculateNeedExpForUnitMaxLevel(m_OperatorData, unitTempletBase.m_NKM_UNIT_GRADE);
		m_iEarnExp = NKCOperatorUtil.CalcNegotiationTotalExp(m_lstMaterials);
		if (m_iEarnExp >= num)
		{
			if (m_iChangeValue > 0 && !m_bMaxLevelBreak)
			{
				m_bPress = false;
			}
			m_bMaxLevelBreak = true;
			m_iEarnExp = num;
		}
	}

	private void UpdateExpUI()
	{
		if (m_OperatorData == null)
		{
			if (m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE != null)
			{
				m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE.fillAmount = 0f;
			}
			if (m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE != null)
			{
				m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE_UP.fillAmount = 0f;
			}
			return;
		}
		int requiredExp = NKCOperatorUtil.GetRequiredExp(m_OperatorData);
		int Exp = 0;
		int num = 0;
		NKCOperatorUtil.CalculateFutureOperatorExpAndLevel(m_OperatorData, NKCOperatorUtil.CalcNegotiationTotalExp(m_lstMaterials), out m_iExpectLevel, out Exp);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_OperatorData.id);
		if (unitTempletBase != null)
		{
			num = NKCOperatorUtil.GetRequiredUnitExp(unitTempletBase.m_NKM_UNIT_GRADE, m_iExpectLevel);
		}
		float fillAmount = (float)m_OperatorData.exp / (float)requiredExp;
		float fillAmount2 = (float)Exp / (float)num;
		NKCUtil.SetLabelText(NKM_UI_PERSONNEL_NEGOTIATE_UNIT_NEXT_LV_COUNT, m_iExpectLevel.ToString());
		bool flag = m_OperatorData.level == NKMCommonConst.OperatorConstTemplet.unitMaximumLevel;
		NKCUtil.SetGameobjectActive(m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_EXPTEXT, !flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_EXPTEXT_1, !flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_EXPTEXT_2, !flag);
		NKCUtil.SetGameobjectActive(m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_LEVELMAX_TEXT, flag);
		if (flag)
		{
			NKCUtil.SetLabelText(m_NKM_UI_PERSONNEL_NEGOTIATE_RESULT_EXP_COUNT, "");
			NKCUtil.SetLabelTextColor(m_NKM_UI_PERSONNEL_NEGOTIATE_RESULT_EXP_COUNT, TEXT_COLOR_YELLOW);
			NKCUtil.SetLabelText(m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_PREV_LV_COUNT, NKMCommonConst.OperatorConstTemplet.unitMaximumLevel.ToString());
			fillAmount = 1f;
		}
		else
		{
			NKCUtil.SetLabelText(m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_PREV_LV_COUNT, m_OperatorData.level.ToString());
			NKCUtil.SetLabelText(m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_EXPTEXT_1, (m_OperatorData.exp + m_iEarnExp).ToString());
			NKCUtil.SetLabelText(m_NKM_UI_PERSONNEL_NEGOTIATE_READY_EXP_EXPTEXT_2, $"/ {requiredExp}");
			NKCUtil.SetLabelText(m_NKM_UI_PERSONNEL_NEGOTIATE_RESULT_EXP_COUNT, string.Format(NKCUtilString.GET_STRING_EXP_PLUS_ONE_PARAM, m_iEarnExp));
			if (m_iExpectLevel == NKMCommonConst.OperatorConstTemplet.unitMaximumLevel)
			{
				NKCUtil.SetLabelTextColor(m_NKM_UI_PERSONNEL_NEGOTIATE_RESULT_EXP_COUNT, Color.red);
				fillAmount2 = 1f;
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_NKM_UI_PERSONNEL_NEGOTIATE_RESULT_EXP_COUNT, TEXT_COLOR_YELLOW);
			}
		}
		if (m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE_UP != null)
		{
			m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE_UP.fillAmount = fillAmount2;
		}
		if (m_iExpectLevel > m_OperatorData.level)
		{
			if (m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE != null)
			{
				m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE.fillAmount = 1f;
			}
			if (m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE_UP != null)
			{
				m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE_UP.transform.SetAsLastSibling();
			}
		}
		else if (m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE != null)
		{
			m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE.fillAmount = fillAmount;
			m_NKM_UI_PERSONNEL_NEGOTIATE_UNIT_LV_GAUGE.transform.SetAsLastSibling();
		}
	}

	private void OnConfirm()
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
		if (!flag)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_NOT_ENOUGH_NEGOTIATE_MATERIALS);
			return;
		}
		int num = NKCOperatorUtil.CalcNegotiationCostCredit(m_lstMaterials);
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(1) < num)
		{
			NKCPopupItemLack.Instance.OpenItemMiscLackPopup(1, num);
		}
		else if (!m_bWaitResultPacket)
		{
			dOnStart?.Invoke(m_lstMaterials);
			m_bWaitResultPacket = true;
		}
	}

	private void UpdateCredit()
	{
		if (m_OperatorData != null)
		{
			NKCUtil.SetLabelText(m_COUNT, NKCOperatorUtil.CalcNegotiationCostCredit(m_lstMaterials).ToString("N0"));
		}
	}

	private void UpdateStartButton()
	{
		if (m_OperatorData == null)
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
		NKCUtil.SetGameobjectActive(m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_BG_OFF, !flag);
		m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_TEXT.color = NKCUtil.GetButtonUIColor(flag);
		m_NKM_UI_PERSONNEL_NEGOTIATE_REQUIRE_START_ICON.color = NKCUtil.GetButtonUIColor(flag);
		if (!flag)
		{
			m_NKM_UI_OPERATOR_INFO_NEGOTIATE_REQUIRE_START.Lock();
		}
		else
		{
			m_NKM_UI_OPERATOR_INFO_NEGOTIATE_REQUIRE_START.UnLock();
		}
	}

	private void OnPlayFX(bool bLevelUp)
	{
		if (bLevelUp)
		{
			NKCSoundManager.PlaySound("FX_UI_ACCOUNT_LEVEL_UP", 1f, 0f, 0f);
		}
		m_LevelUpEffect.SetBool(BOOL_KEY_LEVELUP, bLevelUp);
		for (int i = 0; i < m_lstMaterials.Count; i++)
		{
			if (m_lstMaterials[i] != null)
			{
				if (m_lstMaterials[i].count > 0 && i == 0)
				{
					NKCSoundManager.PlaySound("FX_UI_UNIT_GET_MAIN_SSR", 1f, 0f, 0f);
					m_LevelUpEffect.SetTrigger(FX_SLOT_0);
				}
				if (m_lstMaterials[i].count > 0 && i == 1)
				{
					NKCSoundManager.PlaySound("FX_UI_UNIT_GET_MAIN_SSR", 1f, 0f, 0f);
					m_LevelUpEffect.SetTrigger(FX_SLOT_1);
				}
				if (m_lstMaterials[i].count > 0 && i == 2)
				{
					NKCSoundManager.PlaySound("FX_UI_UNIT_GET_MAIN_SSR", 1f, 0f, 0f);
					m_LevelUpEffect.SetTrigger(FX_SLOT_2);
				}
			}
		}
	}

	public void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (itemData.ItemID == 1)
		{
			UpdateCredit();
			UpdateStartButton();
		}
		else
		{
			ShowRequestItem();
			UpdateStartButton();
		}
	}

	public void ShowRequestItem()
	{
		for (int i = 0; i < m_lstUIItemReq.Count; i++)
		{
			if (i < m_lstMaterialUseCount.Count && i < m_lstMaterialHaveCount.Count && i < m_lstMaterials.Count)
			{
				NKCUtil.SetLabelText(m_lstMaterialUseCount[i], m_lstMaterials[i].count.ToString());
				m_lstUIItemReq[i].SetData(m_lstMaterials[i].itemId, 0, 0L, bShowTooltip: false, bShowBG: false);
				NKCUtil.SetLabelText(m_lstMaterialHaveCount[i], NKCUtilString.GET_STRING_HAVE_COUNT_ONE_PARAM, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_lstMaterials[i].itemId));
			}
		}
	}

	public void Update()
	{
		OnUpdateButtonHold();
	}

	private void OnClickPlus(int slotCnt)
	{
		if (slotCnt >= 0 && NKMCommonConst.OperatorConstTemplet.list.Length >= slotCnt)
		{
			targetId = NKMCommonConst.OperatorConstTemplet.list[slotCnt].itemId;
			m_iChangeValue = 1;
			m_bPress = true;
			m_fDelay = 0.35f;
			m_fHoldTime = 0f;
			m_bWasHold = false;
		}
	}

	private void OnClickMinus(int slotCnt)
	{
		if (slotCnt >= 0 && NKMCommonConst.OperatorConstTemplet.list.Length >= slotCnt)
		{
			targetId = NKMCommonConst.OperatorConstTemplet.list[slotCnt].itemId;
			m_iChangeValue = -1;
			m_bPress = true;
			m_fDelay = 0.35f;
			m_fHoldTime = 0f;
			m_bWasHold = false;
			m_bMaxLevelBreak = false;
		}
	}

	private void OnMinusDown_1(PointerEventData eventData)
	{
		OnClickMinus(0);
	}

	private void OnPlusDown_1(PointerEventData eventData)
	{
		OnClickPlus(0);
	}

	private void OnMinusDown_2(PointerEventData eventData)
	{
		OnClickMinus(1);
	}

	private void OnPlusDown_2(PointerEventData eventData)
	{
		OnClickPlus(1);
	}

	private void OnMinusDown_3(PointerEventData eventData)
	{
		OnClickMinus(2);
	}

	private void OnPlusDown_3(PointerEventData eventData)
	{
		OnClickPlus(2);
	}

	private void OnButtonUp()
	{
		m_iChangeValue = 0;
		m_fDelay = 0.35f;
		m_bPress = false;
	}

	private void OnUpdateButtonHold()
	{
		if (!m_bPress || m_OperatorData == null || (m_bMaxLevelBreak && m_iChangeValue > 0))
		{
			return;
		}
		if (m_bMaxLevelBreak)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_MAX_LEVEL_LOYALTY);
			return;
		}
		m_fHoldTime += Time.deltaTime;
		if (m_fHoldTime >= m_fDelay)
		{
			m_fHoldTime = 0f;
			m_fDelay *= 0.8f;
			int num = ((!(m_fDelay < 0.01f)) ? 1 : 5);
			m_fDelay = Mathf.Clamp(m_fDelay, 0.01f, 0.35f);
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
				if (m_bMaxLevelBreak)
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
				RefreshData();
			}
		}
		RefreshUI();
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
			if (m_OperatorData == null)
			{
				return;
			}
			if (m_bMaxLevelBreak)
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
			RefreshData();
			RefreshUI();
		}
	}
}
