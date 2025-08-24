using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupSelectionConfirmOperatorSkill : NKCUIBase
{
	public delegate void dOnSelect(int id, int subSkillID);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_selection";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATOR_SKILL_SELECTION";

	private static NKCPopupSelectionConfirmOperatorSkill m_Instance;

	public Text m_PopupTitle;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnOK;

	public Text m_lbSkillCoolTime;

	public NKCUIOperatorSkill m_MainSkill;

	public NKCUIOperatorSkill m_SubSkill;

	public GameObject m_objSelectNone;

	public NKCUIOperatorTacticalSkillCombo m_MainSkillCombo;

	public ScrollRect m_CanChangeableSkillScrollRect;

	private NKMItemMiscTemplet m_NKMItemMiscTemplet;

	private int m_targetOperatorID;

	public EventTrigger m_ClickPopupClose;

	private dOnSelect m_dOnSelect;

	private int m_iSelectSubSkillID;

	private List<NKCUIOperatorPassiveSlot> m_lstVisibleSlot = new List<NKCUIOperatorPassiveSlot>();

	public static NKCPopupSelectionConfirmOperatorSkill Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupSelectionConfirmOperatorSkill>("ab_ui_nkm_ui_unit_selection", "NKM_UI_OPERATOR_SKILL_SELECTION", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupSelectionConfirmOperatorSkill>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

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
		Clear();
	}

	public void InitUI()
	{
		NKCUtil.SetBindFunction(m_csbtnClose, base.Close);
		NKCUtil.SetBindFunction(m_csbtnOK, OnConfirm);
		if (m_ClickPopupClose != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnEventPanelClick);
			m_ClickPopupClose.triggers.Add(entry);
		}
	}

	public void Open(NKMItemMiscTemplet itemMiscTemplet, int operId, dOnSelect callBack)
	{
		if (itemMiscTemplet != null)
		{
			m_NKMItemMiscTemplet = itemMiscTemplet;
			m_targetOperatorID = operId;
			m_dOnSelect = callBack;
			m_iSelectSubSkillID = 0;
			UpdateUI();
			UIOpened();
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

	private void UpdateUI()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_targetOperatorID);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelText(m_PopupTitle, string.Format(NKCUtilString.GET_STRING_OPERATOR_SKILL_INFO_POPUP_TITLE, unitTempletBase.GetUnitName()));
		}
		int skillLevel = 1;
		int skillLv = 1;
		List<int> list = new List<int>();
		if (m_NKMItemMiscTemplet.m_CustomBoxId > 0)
		{
			NKMCustomBoxTemplet nKMCustomBoxTemplet = NKMCustomBoxTemplet.Find(m_NKMItemMiscTemplet.m_CustomBoxId);
			if (nKMCustomBoxTemplet != null)
			{
				if (nKMCustomBoxTemplet.CustomOperatorSkillIds.Count > 0)
				{
					list = nKMCustomBoxTemplet.CustomOperatorSkillIds;
				}
				if (nKMCustomBoxTemplet.TacticUpdate > 0)
				{
					skillLevel = nKMCustomBoxTemplet.TacticUpdate;
				}
				if (nKMCustomBoxTemplet.SkillLevel > 0)
				{
					skillLv = nKMCustomBoxTemplet.SkillLevel;
				}
			}
		}
		NKMOperatorSkillTemplet mainSkill = NKCOperatorUtil.GetMainSkill(m_targetOperatorID);
		if (mainSkill != null)
		{
			m_MainSkill.SetData(mainSkill.m_OperSkillID, skillLevel);
			NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(mainSkill.m_OperSkillID);
			if (tacticalCommandTempletByID != null)
			{
				NKCUtil.SetLabelText(m_lbSkillCoolTime, string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), (int)tacticalCommandTempletByID.m_fCoolTime));
			}
		}
		m_MainSkillCombo.SetData(m_targetOperatorID);
		NKCUtil.SetGameobjectActive(m_objSelectNone, m_iSelectSubSkillID == 0);
		if (m_iSelectSubSkillID == 0)
		{
			m_csbtnOK.Lock();
		}
		if (unitTempletBase == null)
		{
			return;
		}
		NKMOperatorRandomPassiveGroupTemplet nKMOperatorRandomPassiveGroupTemplet = NKMOperatorRandomPassiveGroupTemplet.Find(unitTempletBase.m_OprPassiveGroupID);
		if (nKMOperatorRandomPassiveGroupTemplet == null)
		{
			return;
		}
		foreach (NKMOperatorRandomPassiveTemplet group in nKMOperatorRandomPassiveGroupTemplet.Groups)
		{
			if (list.Count > 0 && !list.Contains(group.operSkillId))
			{
				continue;
			}
			NKCUIOperatorPassiveSlot slot = GetSlot();
			if (slot != null)
			{
				NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(group.operSkillId);
				if (skillTemplet != null)
				{
					slot.SetData(NKCUtil.GetSkillIconSprite(skillTemplet), NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID), group.operSkillId, skillLv);
				}
				slot.SetCallBack(OnSelectSubSkill);
				slot.OnSelect(bActive: false);
				m_lstVisibleSlot.Add(slot);
			}
		}
	}

	private NKCUIOperatorPassiveSlot GetSlot()
	{
		NKCUIOperatorPassiveSlot resource = NKCUIOperatorPassiveSlot.GetResource(bBig: true);
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

	public void OnSelectSubSkill(int skillID, int skillLv)
	{
		m_csbtnOK.UnLock();
		NKCUtil.SetGameobjectActive(m_objSelectNone, bValue: false);
		m_SubSkill.SetData(skillID, skillLv);
		m_iSelectSubSkillID = skillID;
		foreach (NKCUIOperatorPassiveSlot item in m_lstVisibleSlot)
		{
			item.OnSelect(item.Key == m_iSelectSubSkillID);
		}
	}

	private void OnConfirm()
	{
		if (m_iSelectSubSkillID != 0)
		{
			Close();
			m_dOnSelect?.Invoke(m_targetOperatorID, m_iSelectSubSkillID);
		}
	}
}
