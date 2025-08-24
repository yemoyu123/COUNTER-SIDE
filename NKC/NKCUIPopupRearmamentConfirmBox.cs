using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine.UI;

namespace NKC;

public class NKCUIPopupRearmamentConfirmBox : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_rearm";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_REARM_FINAL_CONFIRM";

	private static NKCUIPopupRearmamentConfirmBox m_Instance;

	public NKCUIUnitSelectListSlot m_BaseUnitSlot;

	public NKCUIUnitSelectListSlot m_TargetUnitSlot;

	public Text m_lbDesc;

	public NKCUIComStateButton m_btnOK;

	public NKCUIComStateButton m_btnCancel;

	private long m_lResourceRearmUID;

	private int m_iTargetRearmID;

	public static NKCUIPopupRearmamentConfirmBox Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupRearmamentConfirmBox>("ab_ui_rearm", "AB_UI_POPUP_REARM_FINAL_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupRearmamentConfirmBox>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_REARM_CONFIRM_POPIP_BOX_TITLE;

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

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_btnOK, OnClickOK);
		NKCUtil.SetHotkey(m_btnOK, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_btnCancel, base.Close);
		m_BaseUnitSlot?.Init(resetLocalScale: true);
		m_TargetUnitSlot?.Init(resetLocalScale: true);
	}

	public void Open(long resourceUnitUID, int TargetUnitID)
	{
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(resourceUnitUID);
		if (unitFromUID != null)
		{
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.m_UnitID = TargetUnitID;
			nKMUnitData.m_SkinID = 0;
			nKMUnitData.m_UnitLevel = 1;
			nKMUnitData.tacticLevel = unitFromUID.tacticLevel;
			m_BaseUnitSlot.SetDataForRearm(unitFromUID, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bEnableLayoutElement: false, null, bShowEqup: false, bShowLevel: true);
			m_TargetUnitSlot.SetDataForRearm(nKMUnitData, new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE), bEnableLayoutElement: false, null, bShowEqup: false, bShowLevel: true);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(TargetUnitID);
			string msg = string.Format(NKCUtilString.GET_STRING_REARM_CONFIRM_POPUP_FINAL_BOX_DESC, unitTempletBase.GetUnitName());
			NKCUtil.SetLabelText(m_lbDesc, msg);
			m_lResourceRearmUID = resourceUnitUID;
			m_iTargetRearmID = TargetUnitID;
			UIOpened();
		}
	}

	private void OnClickOK()
	{
		NKCPacketSender.Send_NKMPacket_REARMAMENT_UNIT_REQ(m_lResourceRearmUID, m_iTargetRearmID);
	}
}
