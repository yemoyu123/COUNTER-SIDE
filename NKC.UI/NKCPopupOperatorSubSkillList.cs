using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupOperatorSubSkillList : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_OPR_SUB_SKILL_RATE";

	private static NKCPopupOperatorSubSkillList m_Instance;

	public Text m_lbRate;

	public ScrollRect m_ScrollRect;

	[Header("\ufffd\ufffdư\ufffd\ufffd")]
	public NKCUIComStateButton m_csbtnOK;

	[Header("etc")]
	public EventTrigger m_evtTrigger;

	public List<int> m_IgnoreGroupNumber = new List<int> { 999 };

	private List<NKCUIOperatorPassiveSlot> m_lstVisibleSlot = new List<NKCUIOperatorPassiveSlot>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd";

	public static NKCPopupOperatorSubSkillList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupOperatorSubSkillList>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_OPR_SUB_SKILL_RATE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupOperatorSubSkillList>();
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

	public void InitUI()
	{
		NKCUtil.SetBindFunction(m_csbtnOK, base.Close);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		if (null == m_ScrollRect)
		{
			Debug.LogError("NKCPopupOperatorSubSkillList::InitUI - can not found m_ScrollRect");
		}
		else if (m_evtTrigger != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				Close();
			});
			m_evtTrigger.triggers.Add(entry);
		}
	}

	public void Open(int operatorID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorID);
		if (unitTempletBase == null)
		{
			Debug.Log("NKCPopupOperatorSubSkillList::Open - can not found Operator Data");
			return;
		}
		NKMOperatorRandomPassiveGroupTemplet nKMOperatorRandomPassiveGroupTemplet = NKMOperatorRandomPassiveGroupTemplet.Find(unitTempletBase.m_OprPassiveGroupID);
		if (nKMOperatorRandomPassiveGroupTemplet != null)
		{
			SetData(nKMOperatorRandomPassiveGroupTemplet.Groups);
		}
		UIOpened();
	}

	public void Open()
	{
		List<NKMOperatorRandomPassiveTemplet> list = new List<NKMOperatorRandomPassiveTemplet>();
		foreach (NKMOperatorRandomPassiveGroupTemplet value in NKMTempletContainer<NKMOperatorRandomPassiveGroupTemplet>.Values)
		{
			if (!m_IgnoreGroupNumber.Contains(value.GroupId))
			{
				list.AddRange(value.Groups);
			}
		}
		SetData(list);
		UIOpened();
	}

	private void SetData(List<NKMOperatorRandomPassiveTemplet> lstPassiveGroupData)
	{
		if (lstPassiveGroupData == null)
		{
			return;
		}
		float num = 100f / (float)lstPassiveGroupData.Count;
		NKCUtil.SetLabelText(m_lbRate, string.Format(NKCUtilString.GET_STRING_EQUIP_SET_RATE_INFO, num));
		foreach (NKMOperatorRandomPassiveTemplet lstPassiveGroupDatum in lstPassiveGroupData)
		{
			NKCUIOperatorPassiveSlot slot = GetSlot();
			if (slot != null)
			{
				NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(lstPassiveGroupDatum.operSkillId);
				if (skillTemplet != null)
				{
					slot.SetData(NKCUtil.GetSkillIconSprite(skillTemplet), NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID), lstPassiveGroupDatum.operSkillId, 1);
				}
				slot.OnSelect(bActive: false);
				m_lstVisibleSlot.Add(slot);
			}
		}
	}

	private NKCUIOperatorPassiveSlot GetSlot()
	{
		NKCUIOperatorPassiveSlot resource = NKCUIOperatorPassiveSlot.GetResource("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_OPR_SUB_SKILL_RATE_SLOT");
		if (resource != null)
		{
			NKCUtil.SetGameobjectActive(resource, bValue: true);
			resource.transform.SetParent(m_ScrollRect.content);
			resource.transform.localScale = Vector3.one;
			resource.Init();
			return resource;
		}
		return null;
	}

	public override void CloseInternal()
	{
		foreach (NKCUIOperatorPassiveSlot item in m_lstVisibleSlot)
		{
			NKCUtil.SetGameobjectActive(item.gameObject, bValue: false);
			item.DestoryInstance();
		}
		m_lstVisibleSlot.Clear();
		base.gameObject.SetActive(value: false);
	}
}
