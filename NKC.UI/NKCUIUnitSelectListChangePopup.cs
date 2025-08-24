using System;
using System.Collections.Generic;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitSelectListChangePopup : NKCUIBase
{
	[Serializable]
	public struct SimpleSkillInfo
	{
		public Image Icon;

		public Text Level;

		public NKCUIComStateButton Button;

		public GameObject Lock;
	}

	[Serializable]
	public struct StatInfo
	{
		public Text Before;

		public Text After;

		public Text Compare;
	}

	public delegate void OnUnitChangePopupOK();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_change_popup";

	private const string UI_ASSET_NAME = "NKM_UI_UNIT_CHANGE_POPUP";

	private static NKCUIUnitSelectListChangePopup m_Instance;

	public NKCUIComButton m_cbtnOK;

	public NKCUIComButton m_cbtnCancel;

	public NKCUIUnitSelectListSlot m_slotBefore;

	public NKCUIUnitSelectListSlot m_slotAfter;

	public List<SimpleSkillInfo> m_listSkillInfoBefore;

	public List<SimpleSkillInfo> m_listSkillInfoAfter;

	public StatInfo m_statCount;

	public StatInfo m_statHP;

	public StatInfo m_statATK;

	public StatInfo m_statDEF;

	public GameObject m_objLeaderSkillBeforeUnit;

	public GameObject m_objLeaderSkillAfterUnit;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private OnUnitChangePopupOK dOnOK;

	public static NKCUIUnitSelectListChangePopup Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIUnitSelectListChangePopup>("ab_ui_nkm_ui_unit_change_popup", "NKM_UI_UNIT_CHANGE_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIUnitSelectListChangePopup>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "유닛 변경 확인";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_cbtnOK, OnOkButton);
		NKCUtil.SetHotkey(m_cbtnOK, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_cbtnCancel, base.Close);
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		m_slotBefore.Init(resetLocalScale: true);
		m_slotAfter.Init(resetLocalScale: true);
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMUnitData beforeUnit, NKMDeckIndex beforeUnitDeckIndex, NKMUnitData afterUnit, NKMDeckIndex afterUnitDeckIndex, OnUnitChangePopupOK onOK, bool bEnableShowBan = false, bool bEnableShowUpUnit = false)
	{
		if (beforeUnit == null || afterUnit == null)
		{
			Debug.LogError("[UnitSelectListChangePopup] UnitData is null");
			return;
		}
		GetUnitStat(beforeUnit, out var respawnCount, out var statHP, out var statATK, out var statDEF);
		GetUnitStat(afterUnit, out var respawnCount2, out var statHP2, out var statATK2, out var statDEF2);
		SetStatInfo(respawnCount, respawnCount2, m_statCount);
		SetStatInfo(statHP, statHP2, m_statHP);
		SetStatInfo(statATK, statATK2, m_statATK);
		SetStatInfo(statDEF, statDEF2, m_statDEF);
		SetSkillInfo(beforeUnit, m_listSkillInfoBefore);
		SetSkillInfo(afterUnit, m_listSkillInfoAfter);
		NKCUtil.SetGameobjectActive(m_objLeaderSkillBeforeUnit, NKCRearmamentUtil.IsHasLeaderSkill(beforeUnit));
		NKCUtil.SetGameobjectActive(m_objLeaderSkillAfterUnit, NKCRearmamentUtil.IsHasLeaderSkill(afterUnit));
		base.gameObject.SetActive(value: true);
		dOnOK = onOK;
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
		m_slotBefore.SetEnableShowBan(bEnableShowBan);
		m_slotBefore.SetEnableShowUpUnit(bEnableShowUpUnit);
		m_slotBefore.SetData(beforeUnit, beforeUnitDeckIndex, bEnableLayoutElement: false, null);
		m_slotBefore.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		m_slotAfter.SetEnableShowBan(bEnableShowBan);
		m_slotAfter.SetEnableShowUpUnit(bEnableShowUpUnit);
		m_slotAfter.SetData(afterUnit, afterUnitDeckIndex, bEnableLayoutElement: false, null);
		m_slotAfter.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnOkButton()
	{
		Close();
		if (dOnOK != null)
		{
			NKCSoundManager.PlaySound("FX_UI_DECK_UNIIT_SELECT", 1f, 0f, 0f);
			dOnOK();
		}
	}

	private void SetSkillInfo(NKMUnitData unitData, List<SimpleSkillInfo> skillInfoList)
	{
		bool flag = false;
		int num = 1;
		NKMUnitTempletBase templetBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		List<NKMUnitSkillTemplet> unitAllSkillTempletList = NKMUnitSkillManager.GetUnitAllSkillTempletList(unitData);
		int num2 = 0;
		for (int i = 0; i < unitAllSkillTempletList.Count; i++)
		{
			NKMUnitSkillTemplet skillTemplet = unitAllSkillTempletList[i];
			bool flag2 = NKMUnitSkillManager.IsLockedSkill(skillTemplet.m_ID, unitData.m_LimitBreakLevel);
			if (!flag2)
			{
				if (skillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_LEADER)
				{
					flag = true;
					num2 = 0;
				}
				else
				{
					num2 = num;
				}
				SimpleSkillInfo simpleSkillInfo = skillInfoList[num2];
				simpleSkillInfo.Icon.sprite = NKCUtil.GetSkillIconSprite(skillTemplet);
				simpleSkillInfo.Icon.enabled = true;
				simpleSkillInfo.Level.text = (flag2 ? "" : string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, skillTemplet.m_Level));
				NKCUtil.SetGameobjectActive(simpleSkillInfo.Lock, flag2);
				int unitStarGradeMax = templetBase.m_StarGradeMax;
				int limitBreakLevel = unitData.m_LimitBreakLevel;
				simpleSkillInfo.Button.PointerDown.RemoveAllListeners();
				simpleSkillInfo.Button.PointerDown.AddListener(delegate(PointerEventData e)
				{
					OnSkillTooltip(skillTemplet, unitStarGradeMax, limitBreakLevel, templetBase.StopDefaultCoolTime, e);
				});
				if (skillTemplet.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_LEADER)
				{
					num++;
				}
			}
		}
		if (!flag)
		{
			SimpleSkillInfo simpleSkillInfo2 = skillInfoList[0];
			simpleSkillInfo2.Icon.enabled = false;
			simpleSkillInfo2.Level.text = "";
			NKCUtil.SetGameobjectActive(simpleSkillInfo2.Lock, bValue: false);
			simpleSkillInfo2.Button.PointerDown.RemoveAllListeners();
		}
		for (int num3 = num; num3 < skillInfoList.Count; num3++)
		{
			SimpleSkillInfo simpleSkillInfo3 = skillInfoList[num3];
			simpleSkillInfo3.Icon.enabled = false;
			simpleSkillInfo3.Level.text = "";
			NKCUtil.SetGameobjectActive(simpleSkillInfo3.Lock, bValue: false);
			simpleSkillInfo3.Button.PointerDown.RemoveAllListeners();
		}
	}

	private void OnSkillTooltip(NKMUnitSkillTemplet unitSkillTemplet, int unitStarGradeMax, int unitLimitBreakLevel, bool bIsFury, PointerEventData eventData)
	{
		if (unitSkillTemplet != null)
		{
			NKCUITooltip.Instance.Open(unitSkillTemplet, eventData.position, unitStarGradeMax, unitLimitBreakLevel, bIsFury);
		}
	}

	private void GetUnitStat(NKMUnitData unitData, out int respawnCount, out int statHP, out int statATK, out int statDEF)
	{
		bool bPvP = false;
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		NKMStatData nKMStatData = new NKMStatData();
		nKMStatData.Init();
		nKMStatData.MakeBaseStat(null, bPvP, unitData, unitStatTemplet.m_StatData);
		nKMStatData.MakeBaseBonusFactor(unitData, NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.EquipItems, null, null);
		respawnCount = unitStatTemplet.m_RespawnCount;
		statHP = (int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_HP) + (int)nKMStatData.GetBaseBonusStat(NKM_STAT_TYPE.NST_HP);
		statATK = (int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_ATK) + (int)nKMStatData.GetBaseBonusStat(NKM_STAT_TYPE.NST_ATK);
		statDEF = (int)nKMStatData.GetStatBase(NKM_STAT_TYPE.NST_DEF) + (int)nKMStatData.GetBaseBonusStat(NKM_STAT_TYPE.NST_DEF);
	}

	private void SetStatInfo(int statBefore, int statAfter, StatInfo statInfo)
	{
		string hexRGB = "#A3FF66";
		string hexRGB2 = "#FF3D40";
		string format = "<size=20>{0}</size>{1}";
		statInfo.Before.text = statBefore.ToString();
		statInfo.After.text = statAfter.ToString();
		int num = statAfter - statBefore;
		if (num > 0)
		{
			statInfo.Compare.text = string.Format(format, "▲", num.ToString());
			statInfo.Compare.color = NKCUtil.GetColor(hexRGB);
		}
		else if (num < 0)
		{
			statInfo.Compare.text = string.Format(format, "▼", num.ToString());
			statInfo.Compare.color = NKCUtil.GetColor(hexRGB2);
		}
		else
		{
			statInfo.Compare.text = "";
		}
	}
}
