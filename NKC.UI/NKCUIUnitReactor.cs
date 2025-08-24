using System;
using System.Collections;
using System.Collections.Generic;
using ClientPacket.Unit;
using DG.Tweening;
using NKC.UI.Component;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitReactor : NKCUIBase
{
	private enum UI_STATUS
	{
		US_NONE,
		US_INTRO,
		US_READY,
		US_BACK,
		US_INFO,
		US_BACK2
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_reactor";

	private const string UI_ASSET_NAME = "AB_UI_REACTOR";

	private static NKCUIUnitReactor m_Instance;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd")]
	public NKCUIReactorLevelSlot m_curReactorLevelSlot;

	public NKCComTMPUIText m_lbReactorName;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd")]
	public List<NKCUIComToggle> m_lstTablSlot;

	public List<GameObject> m_lstTabCompletMark;

	[Header("FX")]
	public GameObject m_objFXUnlock;

	public GameObject m_objFXLevelUp;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdų \ufffd\ufffd\ufffd\ufffd")]
	public Image m_imgInvenIcon;

	public Image m_imgSkillIcon;

	public NKCComTMPUIText m_lbSkillTitle;

	public NKCComTMPUIText m_lbSkillDesc;

	[Space]
	public Image m_imgInvenIcon2;

	public Image m_imgSkillIcon2;

	public NKCComTMPUIText m_lbSkillTitle2;

	public NKCComTMPUIText m_lbSkillDesc2;

	[Header("\ufffd߾\ufffd UI")]
	public Image m_imgReactorIllust;

	public NKCUIComStateButton m_csbtnInfo;

	public GameObject m_objReactorLock;

	public Image m_imgReactorLock;

	public Color m_ReactorLockColor;

	[Space]
	public GameObject m_objAuraFX_LV01;

	public GameObject m_objAuraFX_LV02;

	public GameObject m_objAuraFX_LV03;

	public GameObject m_objAuraFX_LV04;

	public GameObject m_objAuraFX_LV05;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffdϴ\ufffd - step1")]
	public GameObject m_objBase;

	public List<NKCUIItemCostSlot> m_lstCostItemSlot;

	public NKCUIComStateButton m_csbtnReady;

	public NKCUIReactorLevelSlot m_ReactorSlotBefore;

	public NKCUIReactorLevelSlot m_ReactorSlotAfter;

	[Space]
	public GameObject m_objComplete;

	public NKCUIReactorLevelSlot m_ReactorSlotComplete;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffdϴ\ufffd - step2")]
	public List<NKCUIItemCostSlot> m_lstCostItemSlot1;

	public NKCUIReactorLevelSlot m_ReactorLevelSlot;

	public NKCComTMPUIText m_lbReactorName1;

	public NKCUIComStateButton m_csbtnRun;

	[Header("\ufffd\ufffd\ufffd\ufffd - step3")]
	public NKCComTMPUIText m_lbReactorName2;

	public Image m_imgReactorTergetUnitIcon;

	public NKCComTMPUIText m_lbReactorUnitTitle;

	public NKCComTMPUIText m_lbReactorUnitName;

	public NKCComTMPUIText m_lbReactorUnitDesc;

	[Header("\ufffd\u05b4ϸ\ufffd\ufffd\u033c\ufffd")]
	public Animator m_Ani;

	public float m_fUpgradeDelayStep1 = 0.1f;

	public float m_fUpgradeDelayStep2 = 0.3f;

	public float m_fUpgradeDelayStep3 = 0.6f;

	private NKMUnitData m_UnitData;

	private NKMReactorSkillTemplet[] m_ReactorSkillTemplets;

	private NKMUnitReactorTemplet m_curReactorTemplet;

	private int m_iSelectLevel;

	private UI_STATUS m_curStatus;

	private const string ANI_TRIGGER_INTRO = "INTRO";

	private const string ANI_TRIGGER_READY = "01_TO_02";

	private const string ANI_TRIGGER_BACK = "02_TO_01";

	private const string ANI_TRIGGER_INFO = "01_TO_03";

	private const string ANI_TRIGGER_BACK2 = "03_TO_01";

	private bool m_bAlreadySendLevelUpReq;

	public static NKCUIUnitReactor Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIUnitReactor>("ab_ui_reactor", "AB_UI_REACTOR", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIUnitReactor>();
			}
			m_Instance.Initialize();
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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => "ARTICLE_UNIT_REACTOR";

	public override string MenuName => NKCUtilString.GET_STRING_TACTIC_UPDATE;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.LeftsideOnly;

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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void Initialize()
	{
		int num = 0;
		foreach (NKCUIComToggle item in m_lstTablSlot)
		{
			_ = item;
			int iNum = num;
			NKCUtil.SetToggleValueChangedDelegate(m_lstTablSlot[iNum], delegate
			{
				OnClickTab(iNum);
			});
			num++;
		}
		m_csbtnReady.m_bGetCallbackWhileLocked = true;
		NKCUtil.SetBindFunction(m_csbtnReady, OnClickReady);
		NKCUtil.SetBindFunction(m_csbtnInfo, OnClickInfo);
		NKCUtil.SetBindFunction(m_csbtnRun, OnClickOK);
	}

	public override void OnBackButton()
	{
		switch (m_curStatus)
		{
		case UI_STATUS.US_READY:
			UpdateStatus(UI_STATUS.US_BACK);
			break;
		case UI_STATUS.US_INFO:
			UpdateStatus(UI_STATUS.US_BACK2);
			break;
		default:
			base.OnBackButton();
			break;
		}
	}

	public override void Hide()
	{
		base.Hide();
		NKCUtil.SetGameobjectActive(m_objFXUnlock, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFXLevelUp, bValue: false);
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		base.OnUnitUpdate(eEventType, eUnitType, uid, unitData);
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		base.OnInventoryChange(itemData);
		UpdateCostUI();
	}

	public void Open(NKMUnitData unitData)
	{
		if (unitData == null || unitData.GetUnitTempletBase() == null)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = unitData.GetUnitTempletBase();
		NKMUnitReactorTemplet nKMUnitReactorTemplet = NKMUnitReactorTemplet.Find(unitTempletBase.m_ReactorId);
		if (nKMUnitReactorTemplet != null)
		{
			m_UnitData = unitData;
			m_curReactorTemplet = NKCReactorUtil.GetReactorTemplet(unitTempletBase);
			m_ReactorSkillTemplets = nKMUnitReactorTemplet.GetSkillTemplets();
			for (int i = 0; i < m_lstTablSlot.Count; i++)
			{
				m_lstTablSlot[i]?.SetLock(!IsUsableSkillLevel(i));
				m_lstTablSlot[i].m_bGetCallbackWhileLocked = true;
			}
			NKCUtil.SetGameobjectActive(m_objFXUnlock, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFXLevelUp, bValue: false);
			NKCUtil.SetLabelText(m_lbReactorName, NKCStringTable.GetString(m_curReactorTemplet.ReactorName));
			NKCUtil.SetLabelText(m_lbReactorName1, NKCStringTable.GetString(m_curReactorTemplet.ReactorName));
			NKCUtil.SetImageSprite(m_imgReactorIllust, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_reactor_illust", m_curReactorTemplet.ReactorIllust));
			m_bAlreadySendLevelUpReq = false;
			int num = (IsMaximumLevel(m_UnitData.reactorLevel) ? (m_UnitData.reactorLevel - 1) : m_UnitData.reactorLevel);
			m_lstTablSlot[num].Select(bSelect: true, bForce: true);
			OnClickTab(num);
			UpdateUI(bInitData: true);
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			UpdateStatus(UI_STATUS.US_INTRO, bForce: true);
			UIOpened();
			CheckTutorial();
		}
	}

	private void OnClickTab(int iLevel)
	{
		if (!IsUsableSkillLevel(iLevel))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_UNIT_REACTOR_LOCK_MSG);
			return;
		}
		m_iSelectLevel = iLevel;
		if (!IsMaximumLevel(m_iSelectLevel))
		{
			UpdateRightUI(m_iSelectLevel);
		}
		if (m_UnitData.reactorLevel > 0 && m_UnitData.reactorLevel > m_iSelectLevel)
		{
			m_ReactorSlotComplete.SetLevel(m_iSelectLevel + 1);
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBase, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
			NKCUtil.SetGameobjectActive(m_objBase, bValue: true);
			if (!IsMaximumLevel(m_iSelectLevel))
			{
				m_ReactorSlotBefore.SetLevel(m_iSelectLevel);
				m_ReactorSlotAfter.SetLevel(m_iSelectLevel + 1);
			}
		}
		m_ReactorLevelSlot.SetLevel(m_iSelectLevel + 1);
		m_csbtnReady.SetLock(m_iSelectLevel != m_UnitData.reactorLevel, bForce: true);
		UpdateCostUI();
	}

	private void UpdateRightUI(int iLevel)
	{
		if (m_ReactorSkillTemplets[iLevel] == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(m_ReactorSkillTemplets[iLevel].BaseSkillStrId))
		{
			NKCUtil.SetImageSprite(m_imgInvenIcon, NKCResourceUtility.GetRewardInvenIcon(NKM_REWARD_TYPE.RT_UNIT, m_UnitData.m_UnitID));
			NKCUtil.SetImageSprite(m_imgInvenIcon2, NKCResourceUtility.GetRewardInvenIcon(NKM_REWARD_TYPE.RT_UNIT, m_UnitData.m_UnitID));
			NKCUtil.SetGameobjectActive(m_imgInvenIcon, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgInvenIcon2, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgSkillIcon, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgSkillIcon2, bValue: false);
			NKCUtil.SetLabelText(m_lbSkillTitle, NKCUtilString.GET_STRING_UNIT_REACTOR_SKILL_TITLE_STAT);
			NKCUtil.SetLabelText(m_lbSkillTitle2, NKCUtilString.GET_STRING_UNIT_REACTOR_SKILL_TITLE_STAT);
		}
		else
		{
			int maxSkillLevel = NKMUnitSkillManager.GetMaxSkillLevel(m_ReactorSkillTemplets[iLevel].BaseSkillStrId);
			NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(m_ReactorSkillTemplets[iLevel].BaseSkillStrId, maxSkillLevel);
			if (skillTemplet != null)
			{
				NKCUtil.SetImageSprite(m_imgSkillIcon, NKCUtil.GetSkillIconSprite(skillTemplet));
				NKCUtil.SetImageSprite(m_imgSkillIcon2, NKCUtil.GetSkillIconSprite(skillTemplet));
				string skillTypeName = NKCUtilString.GetSkillTypeName(skillTemplet.m_NKM_SKILL_TYPE);
				NKCUtil.SetLabelText(m_lbSkillTitle, string.Format(NKCUtilString.GET_STRING_UNIT_REACTOR_SKILL_TITLE_SKILL, skillTypeName, skillTemplet.GetSkillName()));
				NKCUtil.SetLabelText(m_lbSkillTitle2, string.Format(NKCUtilString.GET_STRING_UNIT_REACTOR_SKILL_TITLE_SKILL, skillTypeName, skillTemplet.GetSkillName()));
			}
			NKCUtil.SetImageSprite(m_imgInvenIcon, NKCResourceUtility.GetRewardInvenIcon(NKM_REWARD_TYPE.RT_UNIT, m_UnitData.m_UnitID));
			NKCUtil.SetImageSprite(m_imgInvenIcon2, NKCResourceUtility.GetRewardInvenIcon(NKM_REWARD_TYPE.RT_UNIT, m_UnitData.m_UnitID));
			NKCUtil.SetGameobjectActive(m_imgInvenIcon, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgInvenIcon2, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgSkillIcon, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgSkillIcon2, bValue: true);
		}
		NKCUtil.SetLabelText(m_lbSkillDesc, NKCStringTable.GetString(m_ReactorSkillTemplets[iLevel].SkillDesc));
		NKCUtil.SetLabelText(m_lbSkillDesc2, NKCStringTable.GetString(m_ReactorSkillTemplets[iLevel].SkillDesc));
	}

	private void UpdateStatus(UI_STATUS _newStatus, bool bForce = false)
	{
		if (m_curStatus != _newStatus || bForce)
		{
			switch (_newStatus)
			{
			case UI_STATUS.US_INTRO:
				m_Ani.SetTrigger("INTRO");
				break;
			case UI_STATUS.US_READY:
				m_Ani.SetTrigger("01_TO_02");
				break;
			case UI_STATUS.US_BACK:
				m_Ani.SetTrigger("02_TO_01");
				break;
			case UI_STATUS.US_INFO:
				m_Ani.SetTrigger("01_TO_03");
				break;
			case UI_STATUS.US_BACK2:
				m_Ani.SetTrigger("03_TO_01");
				break;
			default:
				Debug.Log($"<color=red>UpdateAni : Not Set Ani Status : {m_curStatus}</color>");
				break;
			}
			m_curStatus = _newStatus;
		}
	}

	private IEnumerator OnCompleteUI()
	{
		yield return new WaitForSeconds(m_fUpgradeDelayStep1);
		int iLevel = (IsMaximumLevel(m_UnitData.reactorLevel) ? (m_UnitData.reactorLevel - 1) : m_UnitData.reactorLevel);
		OnClickTab(iLevel);
		UpdateUI(bInitData: true);
		yield return new WaitForSeconds(m_fUpgradeDelayStep2);
		UpdateStatus(UI_STATUS.US_BACK);
		yield return new WaitForSeconds(m_fUpgradeDelayStep3);
		m_bAlreadySendLevelUpReq = false;
		yield return null;
	}

	private void OnClickReady()
	{
		if (IsPossibleReactorLevelUpgrade())
		{
			UpdateStatus(UI_STATUS.US_READY);
		}
	}

	private void OnClickOK()
	{
		if (!m_bAlreadySendLevelUpReq)
		{
			m_bAlreadySendLevelUpReq = true;
			NKCPacketSender.Send_NKMPacket_UNIT_REACTOR_LEVELUP_REQ(m_UnitData.m_UnitUID);
		}
	}

	private bool IsPossibleReactorLevelUpgrade()
	{
		if (m_UnitData.m_UnitLevel < 110)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_TACTIC_CAN_NOT_TRY_LEVEL_UP);
			return false;
		}
		if (m_iSelectLevel != m_UnitData.reactorLevel)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_UNIT_REACTOR_NOT_MATCH_TARGET_LEVEL);
			return false;
		}
		if (IsMaximumLevel(m_iSelectLevel))
		{
			return false;
		}
		NKMReactorSkillTemplet nKMReactorSkillTemplet = m_ReactorSkillTemplets[m_iSelectLevel];
		if (nKMReactorSkillTemplet == null)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_UNIT_REACTOR_NOT_FIND_SKILL_DATA);
			return false;
		}
		NKMUnitTempletBase unitTempletBase = m_UnitData.GetUnitTempletBase();
		for (int i = 0; i < unitTempletBase.GetSkillCount(); i++)
		{
			int skillLevelByIndex = m_UnitData.GetSkillLevelByIndex(i);
			int maxSkillLevel = NKMUnitSkillManager.GetMaxSkillLevel(unitTempletBase.GetSkillStrID(i));
			if (skillLevelByIndex < maxSkillLevel)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_UNIT_REACTOR_BASE_SKILL_NOT_MAX_LEVEL_01);
				return false;
			}
		}
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		for (int j = 0; j < m_lstCostItemSlot.Count && nKMReactorSkillTemplet.ReqItems.Count > j; j++)
		{
			MiscItemUnit miscItemUnit = nKMReactorSkillTemplet.ReqItems[j];
			if (miscItemUnit != null && inventoryData.GetCountMiscItem(miscItemUnit.ItemId) < miscItemUnit.Count)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_UNIT_REACTOR_UPGRADE_COST_ITEM_NOT_ENOUGH);
				return false;
			}
		}
		return true;
	}

	private bool IsMaximumLevel(int iSelectLevel)
	{
		if (m_ReactorSkillTemplets != null)
		{
			if (m_ReactorSkillTemplets.Length > iSelectLevel)
			{
				return m_ReactorSkillTemplets[iSelectLevel] == null;
			}
			return true;
		}
		return false;
	}

	private bool IsUsableSkillLevel(int iSelectLevel)
	{
		if (!IsMaximumLevel(iSelectLevel) && m_ReactorSkillTemplets != null && (m_ReactorSkillTemplets.Length > iSelectLevel || m_ReactorSkillTemplets[iSelectLevel] != null))
		{
			return m_ReactorSkillTemplets[iSelectLevel].EnableByTag;
		}
		return false;
	}

	private void UpdateUI(bool bInitData = false)
	{
		if (bInitData)
		{
			m_iSelectLevel = m_UnitData.reactorLevel;
			int num = ((m_UnitData.reactorLevel == NKMCommonConst.ReactorMaxLevel) ? (NKMCommonConst.ReactorMaxLevel - 1) : m_UnitData.reactorLevel);
			if (!IsUsableSkillLevel(num))
			{
				num = Math.Max(0, num - 1);
			}
			m_lstTablSlot[num].Select(bSelect: true, bForce: true);
			m_curReactorLevelSlot.SetLevel(m_iSelectLevel);
			UpdateRightUI(num);
			NKCUtil.SetImageColor(m_imgReactorIllust, (m_iSelectLevel == 0) ? m_ReactorLockColor : Color.white);
			NKCUtil.SetGameobjectActive(m_objReactorLock, m_iSelectLevel == 0);
			for (int i = 0; i < m_lstTabCompletMark.Count; i++)
			{
				NKCUtil.SetGameobjectActive(m_lstTabCompletMark[i], i < m_iSelectLevel);
			}
		}
		if (m_UnitData.reactorLevel == 0)
		{
			m_imgReactorLock.DOFade(1f, 0f);
		}
		NKCUtil.SetGameobjectActive(m_objAuraFX_LV01, m_UnitData.reactorLevel == 1);
		NKCUtil.SetGameobjectActive(m_objAuraFX_LV02, m_UnitData.reactorLevel == 2);
		NKCUtil.SetGameobjectActive(m_objAuraFX_LV03, m_UnitData.reactorLevel == 3);
		NKCUtil.SetGameobjectActive(m_objAuraFX_LV04, m_UnitData.reactorLevel == 4);
		NKCUtil.SetGameobjectActive(m_objAuraFX_LV05, m_UnitData.reactorLevel == 5);
		if (!IsMaximumLevel(m_UnitData.reactorLevel))
		{
			m_ReactorSlotBefore.SetLevel(m_iSelectLevel);
			m_ReactorSlotAfter.SetLevel(m_iSelectLevel + 1);
		}
		UpdateCostUI();
	}

	private void UpdateCostUI()
	{
		if (m_iSelectLevel >= m_ReactorSkillTemplets.Length)
		{
			return;
		}
		NKMReactorSkillTemplet nKMReactorSkillTemplet = m_ReactorSkillTemplets[m_iSelectLevel];
		if (nKMReactorSkillTemplet == null)
		{
			return;
		}
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		for (int i = 0; i < m_lstCostItemSlot.Count; i++)
		{
			MiscItemUnit miscItemUnit = null;
			if (nKMReactorSkillTemplet.ReqItems.Count > i)
			{
				miscItemUnit = nKMReactorSkillTemplet.ReqItems[i];
			}
			if (miscItemUnit == null)
			{
				NKCUtil.SetGameobjectActive(m_lstCostItemSlot[i], bValue: false);
				NKCUtil.SetGameobjectActive(m_lstCostItemSlot1[i], bValue: false);
				continue;
			}
			long countMiscItem = inventoryData.GetCountMiscItem(miscItemUnit.ItemId);
			m_lstCostItemSlot[i].SetData(miscItemUnit.ItemId, (int)miscItemUnit.Count, countMiscItem);
			m_lstCostItemSlot1[i].SetData(miscItemUnit.ItemId, (int)miscItemUnit.Count, countMiscItem);
			NKCUtil.SetGameobjectActive(m_lstCostItemSlot[i], bValue: true);
			NKCUtil.SetGameobjectActive(m_lstCostItemSlot1[i], bValue: true);
		}
	}

	private void OnClickInfo()
	{
		if (m_curReactorTemplet != null && m_UnitData != null)
		{
			if (string.IsNullOrEmpty(m_curReactorTemplet.ReactorDesc))
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_UNIT_REACTOR_INFO_NOT_OPEN);
				return;
			}
			NKCUtil.SetLabelText(m_lbReactorName2, NKCStringTable.GetString(m_curReactorTemplet.ReactorName));
			NKCUtil.SetImageSprite(m_imgReactorTergetUnitIcon, NKCResourceUtility.GetRewardInvenIcon(NKM_REWARD_TYPE.RT_UNIT, m_UnitData.m_UnitID));
			NKCUtil.SetLabelText(m_lbReactorUnitName, m_UnitData.GetUnitTempletBase().GetUnitName());
			NKCUtil.SetLabelText(m_lbReactorUnitTitle, m_UnitData.GetUnitTempletBase().GetUnitTitle());
			NKCUtil.SetLabelText(m_lbReactorUnitDesc, NKCStringTable.GetString(m_curReactorTemplet.ReactorDesc));
			UpdateStatus(UI_STATUS.US_INFO);
		}
	}

	public void OnRecv(NKMPacket_UNIT_REACTOR_LEVELUP_ACK sPacket)
	{
		if (sPacket.unitData.m_UnitUID == m_UnitData.m_UnitUID)
		{
			m_UnitData = sPacket.unitData;
			if (m_UnitData.reactorLevel == 1)
			{
				NKCUtil.SetGameobjectActive(m_objFXUnlock, bValue: false);
				NKCUtil.SetGameobjectActive(m_objFXUnlock, bValue: true);
				NKCSoundManager.PlaySound("FX_UI_REACTOR_OPEN", 1f, 0f, 0f);
				m_imgReactorLock.DOFade(0f, m_fUpgradeDelayStep1 + m_fUpgradeDelayStep2);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objFXLevelUp, bValue: false);
				NKCUtil.SetGameobjectActive(m_objFXLevelUp, bValue: true);
				NKCSoundManager.PlaySound("FX_UI_REACTOR_LEVEL_UP", 1f, 0f, 0f);
			}
			m_ReactorSlotComplete.SetLevel(m_UnitData.reactorLevel);
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBase, bValue: false);
			StartCoroutine(OnCompleteUI());
		}
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Reactor);
	}
}
