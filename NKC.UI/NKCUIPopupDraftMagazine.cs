using System.Linq;
using ClientPacket.Pvp;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIPopupDraftMagazine : NKCUIBase
{
	public const string UI_ASSET_BUNDLE_NAME = "AB_UI_EVENT_MD";

	public const string UI_ASSET_NAME = "UI_POPUP_MAGAZINE";

	private static NKCUIPopupDraftMagazine m_Instance;

	public NKCUIComStateButton m_btnClose;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject[] m_UnitSlotParent;

	public NKCUIUnitSelectListSlot[] m_UnitSlot;

	public static NKCUIPopupDraftMagazine Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupDraftMagazine>("AB_UI_EVENT_MD", "UI_POPUP_MAGAZINE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanUpInstance).GetInstance<NKCUIPopupDraftMagazine>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	private static void CleanUpInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		for (int i = 0; i < m_UnitSlot.Count(); i++)
		{
			m_UnitSlot[i].Init();
		}
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(base.Close);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open()
	{
		PvpPickRateData pickRateData = NKCLeaguePVPMgr.GetPickRateData(PvpPickType.None);
		if (pickRateData.playUnits.Count > 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(pickRateData.playUnits[0]);
			NKCUtil.SetGameobjectActive(m_UnitSlotParent[0], bValue: true);
			m_UnitSlot[0].SetData(unitTempletBase, 0, bEnableLayoutElement: true, null);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_UnitSlotParent[0], bValue: false);
		}
		if (pickRateData.winUnits.Count > 0)
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(pickRateData.winUnits[0]);
			NKCUtil.SetGameobjectActive(m_UnitSlotParent[1], bValue: true);
			m_UnitSlot[1].SetData(unitTempletBase2, 0, bEnableLayoutElement: true, null);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_UnitSlotParent[1], bValue: false);
		}
		if (pickRateData.banUnits.Count > 0)
		{
			NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(pickRateData.banUnits[0]);
			NKCUtil.SetGameobjectActive(m_UnitSlotParent[2], bValue: true);
			m_UnitSlot[2].SetData(unitTempletBase3, 0, bEnableLayoutElement: true, null);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_UnitSlotParent[2], bValue: false);
		}
		UIOpened();
	}
}
