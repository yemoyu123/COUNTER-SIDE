using System.Collections.Generic;
using NKC.UI;
using NKM;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupUnitConfirm : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_UNIT_CONFIRM";

	private static NKCPopupUnitConfirm m_Instance;

	public NKCUIUnitSelectListSlot m_UnitSlot;

	public Text m_lbTitle;

	public Text m_lbDesc;

	public Text m_lbDesc2;

	public NKCUIComStateButton m_csbtnOk;

	public NKCUIComStateButton m_csbtnCancel;

	public EventTrigger m_eventTrigger;

	private UnityAction m_dOnOK;

	private UnityAction m_dOnCancel;

	public static NKCPopupUnitConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupUnitConfirm>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_UNIT_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupUnitConfirm>();
				m_Instance.Initialize();
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
		base.gameObject.SetActive(value: false);
	}

	public override void Initialize()
	{
		NKCUtil.SetBindFunction(m_csbtnOk, OnClickOK);
		NKCUtil.SetBindFunction(m_csbtnCancel, OnClickCancel);
		NKCUtil.SetHotkey(m_csbtnOk, HotkeyEventType.Confirm);
		m_UnitSlot.Init();
	}

	public void Open(long unitUID, string strTitle, string strDesc, string strDesc2, UnityAction onOK, UnityAction onCancel = null)
	{
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(unitUID);
		Open(unitFromUID, strTitle, strDesc, strDesc2, onOK, onCancel);
	}

	public void Open(NKMUnitData targetUnit, string strTitle, string strDesc, string strDesc2, UnityAction onOK, UnityAction onCancel = null)
	{
		m_dOnOK = onOK;
		m_dOnCancel = onCancel;
		NKCUtil.SetLabelText(m_lbTitle, strTitle);
		NKCUtil.SetLabelText(m_lbDesc, strDesc);
		NKCUtil.SetLabelText(m_lbDesc2, strDesc2);
		NKCUtil.SetGameobjectActive(m_lbDesc2.gameObject, !string.IsNullOrEmpty(strDesc2));
		m_UnitSlot.SetData(targetUnit, NKMDeckIndex.None, bEnableLayoutElement: true, null);
		NKCUtil.SetEventTriggerDelegate(m_eventTrigger, base.Close);
		UIOpened();
	}

	public void Open(List<NKMUnitData> lstTargetUnits, string strTitle, string strDesc, string strDesc2, UnityAction onOK, UnityAction onCancel = null)
	{
		m_dOnOK = onOK;
		m_dOnCancel = onCancel;
		NKCUtil.SetLabelText(m_lbTitle, strTitle);
		NKCUtil.SetLabelText(m_lbDesc, strDesc);
		NKCUtil.SetLabelText(m_lbDesc2, strDesc2);
		NKCUtil.SetGameobjectActive(m_lbDesc2.gameObject, !string.IsNullOrEmpty(strDesc2));
		m_UnitSlot.SetData(lstTargetUnits[0], NKMDeckIndex.None, bEnableLayoutElement: true, null);
		m_UnitSlot.SetTacticSelectUnitCnt(lstTargetUnits.Count);
		NKCUtil.SetEventTriggerDelegate(m_eventTrigger, base.Close);
		UIOpened();
	}

	private void OnClickOK()
	{
		m_dOnOK?.Invoke();
		Close();
	}

	private void OnClickCancel()
	{
		m_dOnCancel?.Invoke();
		Close();
	}
}
