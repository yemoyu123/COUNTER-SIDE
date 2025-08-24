using NKM;
using NKM.Templet;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupScoutConfirm : NKCUIBase
{
	public delegate void OnConfirm(int count);

	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_UNIT_SCOUT";

	private static NKCUIPopupScoutConfirm s_Instance;

	public Text m_lbTargetUnit;

	public NKCUIScoutUnitPiece m_UnitPiece;

	public NKCUIUnitSelectListSlot m_UnitSlot;

	public NKCUIComStateButton m_csbtnCancel;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnOK;

	public EventTrigger m_evtBackground;

	public NKCUIComQuantityCounter m_comQuantityCounter;

	private NKMPieceTemplet m_pieceTemplet;

	private OnConfirm dOnConfirm;

	public static NKCUIPopupScoutConfirm Instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupScoutConfirm>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_UNIT_SCOUT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupScoutConfirm>();
				s_Instance.Init();
			}
			return s_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_Instance != null)
			{
				return s_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCStringTable.GetString("SI_PF_PERSONNEL_SCOUT_TEXT");

	private static void CleanupInstance()
	{
		s_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (s_Instance != null && s_Instance.IsOpen)
		{
			s_Instance.Close();
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		m_pieceTemplet = null;
		m_comQuantityCounter?.Release();
	}

	private void Init()
	{
		m_UnitSlot?.Init();
		m_UnitSlot?.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		m_csbtnOK?.PointerClick.RemoveAllListeners();
		m_csbtnOK?.PointerClick.AddListener(OnBtnConfirm);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		m_csbtnClose?.PointerClick.RemoveAllListeners();
		m_csbtnClose?.PointerClick.AddListener(base.Close);
		m_csbtnCancel?.PointerClick.RemoveAllListeners();
		m_csbtnCancel?.PointerClick.AddListener(base.Close);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_evtBackground?.triggers.Add(entry);
		m_comQuantityCounter?.Init();
	}

	public void Open(NKMPieceTemplet templet, OnConfirm onConfirm)
	{
		if (templet != null)
		{
			m_pieceTemplet = templet;
			dOnConfirm = onConfirm;
			base.gameObject.SetActive(value: true);
			m_UnitPiece?.SetData(templet);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(templet.m_PieceGetUintId);
			m_UnitSlot?.SetData(unitTempletBase, 1, bEnableLayoutElement: true, null);
			NKCUtil.SetLabelText(m_lbTargetUnit, NKCStringTable.GetString("SI_PF_PERSONNEL_SCOUT_CONFIRM_TEXT"), unitTempletBase.GetUnitName());
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			int ownCount = (int)((nKMUserData != null) ? nKMUserData.m_InventoryData.GetCountMiscItem(templet.m_PieceId) : 0);
			m_comQuantityCounter?.SetCountData(templet.m_PieceReq, ownCount, CalcMaxCount, OnClickPlus, OnClickMinus);
			UIOpened();
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemData.ItemID);
		if (itemMiscTempletByID != null && itemMiscTempletByID.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_PIECE)
		{
			m_UnitPiece?.SetData(m_pieceTemplet, m_comQuantityCounter.CurrentCount);
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			int ownCount = (int)((nKMUserData != null) ? nKMUserData.m_InventoryData.GetCountMiscItem(m_pieceTemplet.m_PieceId) : 0);
			m_comQuantityCounter.UpdateOwnCount(ownCount);
		}
	}

	private int CalcMaxCount()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		int num = (int)nKMUserData.m_InventoryData.GetCountMiscItem(m_pieceTemplet.m_PieceId);
		int num2 = num / m_pieceTemplet.m_PieceReq;
		bool flag = nKMUserData.m_ArmyData.IsCollectedUnit(m_pieceTemplet.m_PieceGetUintId);
		if (num2 > 0 && !flag && (num2 - 1) * m_pieceTemplet.m_PieceReq + m_pieceTemplet.m_PieceReqFirst > num)
		{
			num2--;
		}
		if (num2 <= 0)
		{
			num2 = 0;
		}
		return num2;
	}

	private void OnClickPlus()
	{
		m_UnitPiece?.SetData(m_pieceTemplet, m_comQuantityCounter.CurrentCount);
	}

	private void OnClickMinus()
	{
		m_UnitPiece?.SetData(m_pieceTemplet, m_comQuantityCounter.CurrentCount);
	}

	private void OnBtnConfirm()
	{
		Close();
		if (m_comQuantityCounter.CurrentCount > 0)
		{
			dOnConfirm?.Invoke(m_comQuantityCounter.CurrentCount);
		}
	}
}
