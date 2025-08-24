using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorPopUpSkill : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_operator_info";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATOR_POPUP_SKILL";

	private static NKCUIOperatorPopUpSkill m_Instance;

	public NKCUIOperatorSkill m_MainSkill;

	public NKCUIOperatorSkill m_SubSkill;

	public NKCUIOperatorTacticalSkillCombo m_MainSkillCombo;

	public GameObject m_CurActivePassiveSkill;

	public GameObject m_CanChangeablePassiveSkill;

	public NKCUIComStateButton m_CancelBtn;

	public Text m_PopupTitle;

	public EventTrigger m_ClickPopupClose;

	public ScrollRect m_CanChangeableSkillScrollRect;

	public Text m_lbSkillCoolTime;

	private List<NKCUIOperatorPassiveSlot> m_lstVisibleSlot = new List<NKCUIOperatorPassiveSlot>();

	public static NKCUIOperatorPopUpSkill Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOperatorPopUpSkill>("ab_ui_nkm_ui_operator_info", "NKM_UI_OPERATOR_POPUP_SKILL", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIOperatorPopUpSkill>();
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

	public override string MenuName => NKCUtilString.GET_STRING_OPERATOR_SKILL_POPUP;

	public override eMenutype eUIType => eMenutype.Popup;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		Clear();
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_CancelBtn, base.Close);
		if (m_ClickPopupClose != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnEventPanelClick);
			m_ClickPopupClose.triggers.Add(entry);
		}
	}

	private void OnEventPanelClick(BaseEventData e)
	{
		Close();
	}

	private void Clear()
	{
		foreach (NKCUIOperatorPassiveSlot item in m_lstVisibleSlot)
		{
			NKCUtil.SetGameobjectActive(item.gameObject, bValue: false);
			item.DestoryInstance();
		}
		m_lstVisibleSlot.Clear();
	}

	public void Open(long operatorUID)
	{
		Open(NKCOperatorUtil.GetOperatorData(operatorUID));
	}

	public void Open(NKMOperator Operator)
	{
		if (Operator != null)
		{
			m_MainSkill.SetData(Operator.mainSkill.id, Operator.mainSkill.level);
			m_MainSkillCombo.SetData(Operator.id);
			NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(Operator.mainSkill.id);
			if (tacticalCommandTempletByID != null)
			{
				NKCUtil.SetLabelText(m_lbSkillCoolTime, string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), (int)tacticalCommandTempletByID.m_fCoolTime));
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(Operator.id);
			UpdateSubSkill(unitTempletBase.m_OprPassiveGroupID, Operator.subSkill.id, Operator.subSkill.level);
			NKCUtil.SetLabelText(m_PopupTitle, string.Format(NKCUtilString.GET_STRING_OPERATOR_SKILL_INFO_POPUP_TITLE, unitTempletBase.GetUnitName()));
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			UIOpened();
		}
	}

	public void OpenForCollection(int unitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(unitTempletBase.m_lstSkillStrID[0]);
		if (skillTemplet != null)
		{
			m_MainSkill.SetData(skillTemplet.m_OperSkillID, skillTemplet.m_MaxSkillLevel);
			m_MainSkillCombo.SetData(unitID);
			NKMTacticalCommandTemplet tacticalCommandTempletByStrID = NKMTacticalCommandManager.GetTacticalCommandTempletByStrID(skillTemplet.m_OperSkillTarget);
			if (tacticalCommandTempletByStrID != null)
			{
				NKCUtil.SetLabelText(m_lbSkillCoolTime, string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), (int)tacticalCommandTempletByStrID.m_fCoolTime));
			}
			NKCUtil.SetLabelText(m_PopupTitle, string.Format(NKCUtilString.GET_STRING_OPERATOR_SKILL_INFO_POPUP_TITLE, unitTempletBase.GetUnitName()));
			UpdateSubSkill(unitTempletBase.m_OprPassiveGroupID);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			UIOpened();
		}
	}

	private void UpdateSubSkill(int groupID, int subSkillID = 0, int SubSkillLevel = 0)
	{
		if (subSkillID != 0)
		{
			m_SubSkill.SetData(subSkillID, SubSkillLevel);
		}
		NKCUtil.SetGameobjectActive(m_CurActivePassiveSkill, subSkillID != 0);
		NKMOperatorRandomPassiveGroupTemplet nKMOperatorRandomPassiveGroupTemplet = NKMOperatorRandomPassiveGroupTemplet.Find(groupID);
		if (nKMOperatorRandomPassiveGroupTemplet == null || nKMOperatorRandomPassiveGroupTemplet.Groups.Count <= 0)
		{
			return;
		}
		foreach (NKMOperatorRandomPassiveTemplet group in nKMOperatorRandomPassiveGroupTemplet.Groups)
		{
			if (subSkillID == group.operSkillId)
			{
				continue;
			}
			NKCUIOperatorPassiveSlot slot = GetSlot();
			if (slot != null)
			{
				NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(group.operSkillId);
				if (skillTemplet != null)
				{
					slot.SetData(NKCUtil.GetSkillIconSprite(skillTemplet), NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID), group.operSkillId, skillTemplet.m_MaxSkillLevel);
				}
				m_lstVisibleSlot.Add(slot);
			}
		}
	}

	private NKCUIOperatorPassiveSlot GetSlot()
	{
		NKCUIOperatorPassiveSlot resource = NKCUIOperatorPassiveSlot.GetResource();
		if (resource != null)
		{
			NKCUtil.SetGameobjectActive(resource, bValue: true);
			resource.transform.SetParent(m_CanChangeableSkillScrollRect.content);
			resource.transform.localScale = Vector3.one;
			resource.Init();
			return resource;
		}
		return null;
	}
}
