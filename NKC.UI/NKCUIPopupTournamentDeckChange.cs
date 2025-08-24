using System.Collections.Generic;
using Cs.Math;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupTournamentDeckChange : NKCUIBase
{
	public delegate void OnConfirm();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_deck_view";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_DECK_VIEW_TOURNAMENT";

	private static NKCUIPopupTournamentDeckChange m_Instance;

	public Text m_lbTitle;

	public Text m_lbDesc;

	[Space]
	public Image m_imgShip;

	public NKCUIOperatorDeckSlot m_NKCDeckViewOperator;

	public List<NKCDeckViewUnitSlot> m_lstUnitSlot;

	public Text m_lbOperationPower;

	public Text m_lbCost;

	[Space]
	public Image m_imgShipAfter;

	public GameObject m_objShipChanged;

	public NKCUIOperatorDeckSlot m_NKCDeckViewOperatorAfter;

	public GameObject m_objOperatorChanged;

	public List<NKCDeckViewUnitSlot> m_lstUnitSlotAfter;

	public Text m_lbOperationPowerAfter;

	public GameObject m_objPowerChanged;

	public Text m_lbCostAfter;

	public GameObject m_objCostChanged;

	[Space]
	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnConfirm;

	private OnConfirm m_dOnConfirm;

	public static NKCUIPopupTournamentDeckChange Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupTournamentDeckChange>("ab_ui_nkm_ui_deck_view", "NKM_UI_POPUP_DECK_VIEW_TOURNAMENT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupTournamentDeckChange>();
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

	public override string MenuName => "";

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

	public static bool CheckInstance()
	{
		return m_Instance != null;
	}

	private void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnConfirm.PointerClick.RemoveAllListeners();
		m_btnConfirm.PointerClick.AddListener(OnClickConfirm);
		m_NKCDeckViewOperator.Init();
		for (int i = 0; i < m_lstUnitSlot.Count; i++)
		{
			m_lstUnitSlot[i].Init(i, bEnableDrag: false);
		}
		m_NKCDeckViewOperatorAfter.Init();
		for (int j = 0; j < m_lstUnitSlotAfter.Count; j++)
		{
			m_lstUnitSlotAfter[j].Init(j, bEnableDrag: false);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(OnConfirm dOnConfirm, string title = "", string desc = "")
	{
		m_dOnConfirm = dOnConfirm;
		if (m_lbTitle != null && !string.IsNullOrEmpty(title))
		{
			NKCUtil.SetLabelText(m_lbTitle, title);
		}
		if (m_lbDesc != null && !string.IsNullOrEmpty(desc))
		{
			NKCUtil.SetLabelText(m_lbDesc, desc);
		}
		SetLastDeckData();
		SetNewDeckData();
		UIOpened();
	}

	private void SetLastDeckData()
	{
		if (NKCTournamentManager.m_TournamentApplyDeckData == null)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(NKCTournamentManager.m_TournamentApplyDeckData.ship.unitId);
		if (unitTempletBase != null)
		{
			NKCUtil.SetImageSprite(m_imgShip, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase));
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgShip, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_TEXTURE", "NKM_DECK_VIEW_SHIP_UNKNOWN"));
		}
		m_NKCDeckViewOperator.SetData(NKCTournamentManager.m_TournamentApplyDeckData.operatorUnit);
		for (int i = 0; i < m_lstUnitSlot.Count; i++)
		{
			NKMUnitData nKMUnitData = new NKMUnitData();
			if (NKCTournamentManager.m_TournamentApplyDeckData.units[i] != null)
			{
				nKMUnitData.FillDataFromAsyncUnitData(NKCTournamentManager.m_TournamentApplyDeckData.units[i]);
			}
			m_lstUnitSlot[i].SetData(nKMUnitData);
			m_lstUnitSlot[i].SetLeader(NKCTournamentManager.m_TournamentApplyDeckData.leaderIndex == i, bEffect: false);
		}
		NKCUtil.SetLabelText(m_lbCost, $"{NKCTournamentManager.CalculateDeckAvgSummonCost():0.00}");
		NKCUtil.SetLabelText(m_lbOperationPower, NKCTournamentManager.m_TournamentApplyDeckData.operationPower.ToString());
	}

	private void SetNewDeckData()
	{
		NKMArmyData nKMArmyData = NKCScenManager.CurrentArmyData();
		NKMDeckData deckData = nKMArmyData.GetDeckData(NKM_DECK_TYPE.NDT_TOURNAMENT, 0);
		if (deckData != null)
		{
			NKMUnitData shipFromUID = nKMArmyData.GetShipFromUID(deckData.m_ShipUID);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID);
			NKCUtil.SetImageSprite(m_imgShipAfter, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase), bDisableIfSpriteNull: true);
			NKCUtil.SetGameobjectActive(m_objShipChanged, NKCTournamentManager.IsShipChanged(shipFromUID));
			NKMOperator operatorFromUId = nKMArmyData.GetOperatorFromUId(deckData.m_OperatorUID);
			m_NKCDeckViewOperatorAfter.SetData(operatorFromUId);
			NKCUtil.SetGameobjectActive(m_objOperatorChanged, NKCTournamentManager.IsOperatorChanged(operatorFromUId));
			for (int i = 0; i < m_lstUnitSlotAfter.Count; i++)
			{
				NKMUnitData unitFromUID = nKMArmyData.GetUnitFromUID(deckData.m_listDeckUnitUID[i]);
				m_lstUnitSlotAfter[i].SetData(unitFromUID, bEnableButton: false);
				m_lstUnitSlotAfter[i].SetLeader(deckData.m_LeaderIndex == i, bEffect: false);
				m_lstUnitSlotAfter[i].SetDataChanged(NKCTournamentManager.IsUnitChanged(i, unitFromUID));
			}
			float num = nKMArmyData.CalculateDeckAvgSummonCost(new NKMDeckIndex(NKM_DECK_TYPE.NDT_TOURNAMENT, 0));
			NKCUtil.SetLabelText(m_lbCostAfter, $"{num:0.00}");
			NKCUtil.SetGameobjectActive(m_objCostChanged, !num.IsNearlyEqual(NKCTournamentManager.CalculateDeckAvgSummonCost()));
			int armyAvarageOperationPower = nKMArmyData.GetArmyAvarageOperationPower(deckData);
			NKCUtil.SetLabelText(m_lbOperationPowerAfter, armyAvarageOperationPower.ToString());
			NKCUtil.SetGameobjectActive(m_objPowerChanged, armyAvarageOperationPower != NKCTournamentManager.m_TournamentApplyDeckData.operationPower);
		}
	}

	private void OnClickConfirm()
	{
		m_dOnConfirm();
	}
}
