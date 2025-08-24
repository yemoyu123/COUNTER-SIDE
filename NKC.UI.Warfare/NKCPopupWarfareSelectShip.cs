using System.Collections.Generic;
using ClientPacket.Community;
using ClientPacket.Warfare;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Warfare;

public class NKCPopupWarfareSelectShip : NKCUIBase
{
	private enum ShipType
	{
		player,
		dungeon,
		supporter
	}

	public delegate void OnSetFlagShipButton(int gameUnitUID);

	public delegate void OnCancelBatchButton(int gameUnitUID);

	public delegate void OnDeckViewBtn(int gameUnitUID);

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_WARFARE_SELECT";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_WARFARE_SELECT_SHIP";

	public GameObject m_NKM_UI_POPUP_TOP_INFO;

	public GameObject m_NKM_UI_POPUP_TOP_INFO_ENEMY;

	public GameObject m_NKM_UI_POPUP_SHIP_INFO;

	public GameObject m_NKM_UI_POPUP_SHIP_INFO_ENEMY;

	public GameObject m_UNIT_DECK_AREA;

	public GameObject m_ENEMY_DECK_AREA_ScrollView;

	public GameObject m_NKM_UI_POPUP_MENU1_ON;

	public GameObject m_NKM_UI_POPUP_MENU2;

	public Image m_ImgUnit;

	public Text m_NKM_UI_POPUP_INFO_TEXT;

	public Text m_NKM_UI_POPUP_INFO_TEXT_2;

	public Text m_NKM_UI_POPUP_TOP_TEXT;

	public Text m_NKM_UI_POPUP_TOP_TEXT2;

	public Text m_NKM_UI_POPUP_ENEMY_INFO_TEXT2;

	public Text m_NKM_UI_POPUP_ENEMY_INFO_TEXT3;

	public Text m_NKM_UI_POPUP_ENEMY_INFO_TEXT6;

	public Text m_NKM_UI_POPUP_ENEMY_INFO_TEXT7;

	public Text m_NKM_UI_POPUP_ENEMY_INFO_TEXT8;

	public List<NKCDeckViewUnitSlot> m_lstNKCDeckViewUnitSlot;

	public NKCUIShipInfoSummary m_UIShipInfo;

	public Text m_txtShipPower;

	public GameObject m_ENEMY_DECK_AREA_Content;

	public GameObject m_NKM_UI_POPUP_INFO_BATTLEPOINT_1;

	public GameObject m_NKM_UI_POPUP_INFO_BATTLEPOINT_2;

	public Image m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1;

	public Image m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2;

	public Image m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3;

	public GameObject m_objSquadPower;

	public Text m_lbAvgPower;

	public Text m_lbCost;

	[Header("Button")]
	public NKCUIComButton m_NKM_UI_POPUP_MENU1_ON_Btn;

	public NKCUIComButton m_NKM_UI_POPUP_MENU2_Btn;

	public NKCUIComButton m_NKM_UI_POPUP_MENU3_Btn;

	public NKCUIComButton m_NKM_UI_POPUP_MENU4_Btn;

	public NKCUIComButton m_NKM_UI_POPUP_MENU1_OFF;

	public EventTrigger m_NKM_UI_POPUP_BG;

	public EventTrigger m_NKM_UI_POPUP_CANCEL;

	[Header("Friend")]
	public GameObject m_objFriendRoot;

	public Text m_txtFriendCode;

	public Text m_txtFriendDesc;

	[Header("Cheat")]
	public EventTrigger m_etDungeonClearRewardCheat;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private OnSetFlagShipButton m_dOnSetFlagShipButton;

	private OnCancelBatchButton m_dOnCancelBatchButton;

	private OnDeckViewBtn m_dOnDeckViewBtn;

	private NKMDeckIndex m_sNKMDeckIndex;

	private int m_WarfareGameUnitUID;

	private int m_ShipUnitID;

	private WarfareSupporterListData m_friendData;

	private int m_DungeonID;

	private List<NKCDeckViewUnitSlot> m_lstDeckViewUnitSlot = new List<NKCDeckViewUnitSlot>();

	[Header("전투 환경")]
	public GameObject m_NKM_UI_OPERATION_BC;

	public Image NKM_UI_OPERATION_POPUP_SCENARIO_BC_ICON;

	[Header("오퍼레이터")]
	public GameObject m_objOperator;

	public NKCUIOperatorDeckSlot m_Operator;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_WARFARE_SELECT_SHIP_POPUP;

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstNKCDeckViewUnitSlot[i];
			if (nKCDeckViewUnitSlot != null)
			{
				nKCDeckViewUnitSlot.Init(i);
			}
		}
		m_NKM_UI_POPUP_MENU1_ON_Btn.PointerClick.RemoveAllListeners();
		m_NKM_UI_POPUP_MENU1_ON_Btn.PointerClick.AddListener(OnSetFlagShipButton_);
		m_NKM_UI_POPUP_MENU2_Btn.PointerClick.RemoveAllListeners();
		m_NKM_UI_POPUP_MENU2_Btn.PointerClick.AddListener(OnCancelBatchButton_);
		m_NKM_UI_POPUP_MENU3_Btn.PointerClick.RemoveAllListeners();
		m_NKM_UI_POPUP_MENU3_Btn.PointerClick.AddListener(OnCloseBtn);
		NKCUtil.SetHotkey(m_NKM_UI_POPUP_MENU3_Btn, HotkeyEventType.Confirm);
		m_NKM_UI_POPUP_MENU4_Btn.PointerClick.RemoveAllListeners();
		m_NKM_UI_POPUP_MENU4_Btn.PointerClick.AddListener(OnDeckViewBtn_);
		m_NKM_UI_POPUP_MENU1_OFF.PointerClick.RemoveAllListeners();
		m_NKM_UI_POPUP_MENU1_OFF.PointerClick.AddListener(OnSetFlagShipButton_);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			OnCloseBtn();
		});
		m_NKM_UI_POPUP_BG.triggers.Add(entry);
		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerClick;
		entry2.callback.AddListener(delegate
		{
			OnCloseBtn();
		});
		m_NKM_UI_POPUP_CANCEL.triggers.Add(entry2);
		base.gameObject.SetActive(value: false);
	}

	public void OpenForMyShipInDive(NKMDeckIndex sNKMDeckIndex)
	{
		m_sNKMDeckIndex = sNKMDeckIndex;
		m_dOnSetFlagShipButton = null;
		m_dOnCancelBatchButton = null;
		m_dOnDeckViewBtn = null;
		base.gameObject.SetActive(value: true);
		SetUIByDataInDive(bMyShip: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		SetBattleEnvironment();
		UIOpened();
	}

	public void OpenForMyShipInWarfare(NKMDeckIndex sNKMDeckIndex, int gameUnitUID, int shipUnitID)
	{
		m_sNKMDeckIndex = sNKMDeckIndex;
		m_WarfareGameUnitUID = gameUnitUID;
		m_ShipUnitID = shipUnitID;
		m_dOnSetFlagShipButton = null;
		m_dOnCancelBatchButton = null;
		m_dOnDeckViewBtn = null;
		base.gameObject.SetActive(value: true);
		SetUIByDataInWarfare(ShipType.player, bPlaying: true);
		m_NKCUIOpenAnimator.PlayOpenAni();
		SetBattleEnvironment();
		UIOpened();
	}

	public void OpenForMyShipInWarfare(NKMDeckIndex sNKMDeckIndex, int gameUnitUID, int shipUnitID, OnSetFlagShipButton _dOnSetFlagShipButton, OnCancelBatchButton _dOnCancelBatchButton, OnDeckViewBtn _dOnDeckViewBtn)
	{
		m_sNKMDeckIndex = sNKMDeckIndex;
		m_WarfareGameUnitUID = gameUnitUID;
		m_ShipUnitID = shipUnitID;
		m_dOnSetFlagShipButton = _dOnSetFlagShipButton;
		m_dOnCancelBatchButton = _dOnCancelBatchButton;
		m_dOnDeckViewBtn = _dOnDeckViewBtn;
		base.gameObject.SetActive(value: true);
		SetUIByDataInWarfare(ShipType.player);
		m_NKCUIOpenAnimator.PlayOpenAni();
		SetBattleEnvironment();
		UIOpened();
	}

	public void OpenForSupporterInWarfare(WarfareSupporterListData friendData, int gameUnitUID, OnCancelBatchButton _dOnCancelBatchButton = null)
	{
		m_friendData = friendData;
		m_WarfareGameUnitUID = gameUnitUID;
		m_dOnSetFlagShipButton = null;
		m_dOnCancelBatchButton = _dOnCancelBatchButton;
		m_dOnDeckViewBtn = null;
		base.gameObject.SetActive(value: true);
		SetUIByDataInWarfare(ShipType.supporter, _dOnCancelBatchButton == null);
		m_NKCUIOpenAnimator.PlayOpenAni();
		SetBattleEnvironment();
		UIOpened();
	}

	public void OpenForEnemy(int dungeonID, string battleConditionStrID = "")
	{
		m_DungeonID = dungeonID;
		base.gameObject.SetActive(value: true);
		SetUIByDataInWarfare(ShipType.dungeon);
		m_NKCUIOpenAnimator.PlayOpenAni();
		m_dOnSetFlagShipButton = null;
		m_dOnCancelBatchButton = null;
		m_dOnDeckViewBtn = null;
		SetBattleEnvironment(battleConditionStrID);
		UIOpened();
	}

	private void SetBattleEnvironment(string battleConditionStrID = "")
	{
		if (!string.IsNullOrEmpty(battleConditionStrID))
		{
			NKMBattleConditionTemplet cNKMBattleConditionTemplet = NKMBattleConditionManager.GetTempletByStrID(battleConditionStrID);
			bool flag = cNKMBattleConditionTemplet != null && cNKMBattleConditionTemplet.BattleCondID != 0 && !cNKMBattleConditionTemplet.m_bHide;
			if (flag)
			{
				NKCUtil.SetImageSprite(NKM_UI_OPERATION_POPUP_SCENARIO_BC_ICON, NKCUtil.GetSpriteBattleConditionICon(cNKMBattleConditionTemplet));
				NKCUIComStateButton component = NKM_UI_OPERATION_POPUP_SCENARIO_BC_ICON.gameObject.GetComponent<NKCUIComStateButton>();
				if (component != null)
				{
					component.PointerDown.RemoveAllListeners();
					component.PointerDown.AddListener(delegate(PointerEventData e)
					{
						NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, cNKMBattleConditionTemplet.BattleCondName_Translated, cNKMBattleConditionTemplet.BattleCondDesc_Translated, e.position);
					});
				}
			}
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_BC, flag);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_BC, bValue: false);
		}
	}

	private void SetShipNameText(string name)
	{
		if (m_NKM_UI_POPUP_INFO_TEXT != null)
		{
			m_NKM_UI_POPUP_INFO_TEXT.text = name;
		}
	}

	private void SetShipTypeText(string name)
	{
		if (m_NKM_UI_POPUP_INFO_TEXT_2 != null)
		{
			m_NKM_UI_POPUP_INFO_TEXT_2.text = name;
		}
	}

	private void SetDeckViewUnitSlotCount(int count)
	{
		while (m_lstDeckViewUnitSlot.Count < count)
		{
			NKCDeckViewUnitSlot newInstance = NKCDeckViewUnitSlot.GetNewInstance(m_ENEMY_DECK_AREA_Content.transform);
			if (!(newInstance == null))
			{
				newInstance.Init(m_lstDeckViewUnitSlot.Count, bEnableDrag: false);
				newInstance.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
				m_lstDeckViewUnitSlot.Add(newInstance);
			}
		}
	}

	public void SetUIByDataInDive(bool bMyShip)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_TOP_INFO, bMyShip);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_TOP_INFO_ENEMY, !bMyShip);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHIP_INFO, bMyShip);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHIP_INFO_ENEMY, !bMyShip);
		NKCUtil.SetGameobjectActive(m_UNIT_DECK_AREA, bMyShip);
		NKCUtil.SetGameobjectActive(m_ENEMY_DECK_AREA_ScrollView, !bMyShip);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_MENU1_ON, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_MENU2, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_MENU4_Btn.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSquadPower, bMyShip);
		if (!bMyShip)
		{
			return;
		}
		int num = 0;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(m_sNKMDeckIndex);
		if (deckData != null)
		{
			m_NKM_UI_POPUP_TOP_TEXT.text = string.Format(NKCUtilString.GET_STRING_SQUAD_ONE_PARAM, NKCUtilString.GetDeckNumberString(m_sNKMDeckIndex));
			int num2 = m_sNKMDeckIndex.m_iIndex + 1;
			m_NKM_UI_POPUP_TOP_TEXT2.text = string.Format(NKCUtilString.GET_STRING_SQUAD_TWO_PARAM, num2, NKCUtilString.GetRankNumber(num2).ToUpper());
			for (num = 0; num < m_lstNKCDeckViewUnitSlot.Count; num++)
			{
				if (num < 8)
				{
					m_lstNKCDeckViewUnitSlot[num].SetData(myUserData.m_ArmyData.GetUnitFromUID(deckData.m_listDeckUnitUID[num]), bEnableButton: false);
					if (num == deckData.m_LeaderIndex)
					{
						m_lstNKCDeckViewUnitSlot[num].SetLeader(bLeader: true, bEffect: false);
					}
				}
			}
			NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
			NKMUnitTempletBase nKMUnitTempletBase = null;
			if (shipFromUID != null)
			{
				nKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
			}
			NKCUtil.SetGameobjectActive(m_objOperator, !NKCOperatorUtil.IsHide());
			if (deckData.m_OperatorUID != 0L)
			{
				m_Operator.SetData(NKCOperatorUtil.GetOperatorData(deckData.m_OperatorUID));
			}
			else if (NKCOperatorUtil.IsHide())
			{
				m_Operator.SetHide();
			}
			else
			{
				m_Operator.SetEmpty();
			}
			if (nKMUnitTempletBase != null)
			{
				SetShipNameText(nKMUnitTempletBase.GetUnitName());
				SetShipTypeText(nKMUnitTempletBase.GetUnitTitle());
				Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, nKMUnitTempletBase);
				Sprite sprite2 = null;
				if (sprite != null)
				{
					sprite2 = sprite;
				}
				if (sprite2 == null)
				{
					NKCAssetResourceData assetResourceUnitInvenIconEmpty = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
					if (assetResourceUnitInvenIconEmpty != null)
					{
						m_ImgUnit.sprite = assetResourceUnitInvenIconEmpty.GetAsset<Sprite>();
					}
					else
					{
						m_ImgUnit.sprite = null;
					}
				}
				else
				{
					m_ImgUnit.sprite = sprite2;
				}
			}
			int armyAvarageOperationPower = myUserData.m_ArmyData.GetArmyAvarageOperationPower(m_sNKMDeckIndex);
			NKCUtil.SetLabelText(m_lbAvgPower, armyAvarageOperationPower.ToString("N0"));
			NKCUtil.SetLabelText(m_lbCost, $"{myUserData.m_ArmyData.CalculateDeckAvgSummonCost(m_sNKMDeckIndex):0.00}");
			m_UIShipInfo.SetShipData(shipFromUID, nKMUnitTempletBase, m_sNKMDeckIndex);
			NKCUtil.SetLabelText(m_txtShipPower, (shipFromUID != null) ? shipFromUID.CalculateOperationPower(myUserData.m_InventoryData).ToString("N0") : "");
		}
		else
		{
			m_NKM_UI_POPUP_TOP_TEXT.text = "";
			for (num = 0; num < m_lstNKCDeckViewUnitSlot.Count; num++)
			{
				if (num < 8)
				{
					m_lstNKCDeckViewUnitSlot[num].SetData(null, bEnableButton: false);
				}
			}
			m_UIShipInfo.SetShipData(null, null, NKMDeckIndex.None);
			NKCUtil.SetLabelText(m_txtShipPower, "");
		}
		NKMDiveGameData diveGameData = NKCScenManager.GetScenManager().GetMyUserData().m_DiveGameData;
		if (diveGameData == null)
		{
			return;
		}
		NKMDiveSquad squad = diveGameData.Player.GetSquad(m_sNKMDeckIndex.m_iIndex);
		if (squad != null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_1, squad.Supply >= 1);
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_2, squad.Supply >= 2);
			float fRatio = squad.CurHp / squad.MaxHp;
			fRatio = GetProperRatioValue(fRatio);
			if (fRatio > 0.6f)
			{
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(1f, 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(1f, 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(GetProperRatioValue((fRatio - 0.6f) / 0.4f), 1f, 1f);
			}
			else if (fRatio > 0.3f)
			{
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(1f, 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(GetProperRatioValue((fRatio - 0.3f) / 0.3f), 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(0f, 1f, 1f);
			}
			else
			{
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(GetProperRatioValue(fRatio / 0.3f), 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(0f, 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(0f, 1f, 1f);
			}
		}
	}

	private float GetProperRatioValue(float fRatio)
	{
		if (fRatio < 0f)
		{
			fRatio = 0f;
		}
		if (fRatio > 1f)
		{
			fRatio = 1f;
		}
		return fRatio;
	}

	private void SetUIByDataInWarfare(ShipType shipType, bool bPlaying = false)
	{
		bool flag = shipType == ShipType.player;
		bool flag2 = shipType != ShipType.dungeon;
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_TOP_INFO, flag2);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_TOP_INFO_ENEMY, !flag2);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHIP_INFO, flag2);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHIP_INFO_ENEMY, !flag2);
		NKCUtil.SetGameobjectActive(m_UNIT_DECK_AREA, flag2);
		NKCUtil.SetGameobjectActive(m_ENEMY_DECK_AREA_ScrollView, !flag2);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_MENU1_ON, flag && !bPlaying);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_MENU2, flag2 && !bPlaying);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_MENU4_Btn.gameObject, flag && !bPlaying);
		NKCUtil.SetGameobjectActive(m_objSquadPower, flag2);
		NKCUtil.SetGameobjectActive(m_objFriendRoot, shipType == ShipType.supporter);
		NKCUtil.SetGameobjectActive(m_objOperator, !NKCOperatorUtil.IsHide() && shipType == ShipType.player);
		switch (shipType)
		{
		case ShipType.player:
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(m_ShipUnitID);
			if (unitTempletBase2 != null)
			{
				SetShipNameText(unitTempletBase2.GetUnitName());
				SetShipTypeText(unitTempletBase2.GetUnitTitle());
				Sprite sprite2 = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase2);
				Sprite sprite3 = null;
				if (sprite2 != null)
				{
					sprite3 = sprite2;
				}
				if (sprite3 == null)
				{
					NKCAssetResourceData assetResourceUnitInvenIconEmpty2 = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
					if (assetResourceUnitInvenIconEmpty2 != null)
					{
						m_ImgUnit.sprite = assetResourceUnitInvenIconEmpty2.GetAsset<Sprite>();
					}
					else
					{
						m_ImgUnit.sprite = null;
					}
				}
				else
				{
					m_ImgUnit.sprite = sprite3;
				}
			}
			int num2 = 0;
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(m_sNKMDeckIndex);
			if (deckData != null)
			{
				m_NKM_UI_POPUP_TOP_TEXT.text = string.Format(NKCUtilString.GET_STRING_SQUAD_ONE_PARAM, NKCUtilString.GetDeckNumberString(m_sNKMDeckIndex));
				int num3 = m_sNKMDeckIndex.m_iIndex + 1;
				m_NKM_UI_POPUP_TOP_TEXT2.text = string.Format(NKCUtilString.GET_STRING_SQUAD_TWO_PARAM, num3, NKCUtilString.GetRankNumber(num3).ToUpper());
				for (num2 = 0; num2 < m_lstNKCDeckViewUnitSlot.Count; num2++)
				{
					if (num2 < 8)
					{
						m_lstNKCDeckViewUnitSlot[num2].SetData(myUserData.m_ArmyData.GetUnitFromUID(deckData.m_listDeckUnitUID[num2]), bEnableButton: false);
						if (num2 == deckData.m_LeaderIndex)
						{
							m_lstNKCDeckViewUnitSlot[num2].SetLeader(bLeader: true, bEffect: false);
						}
					}
				}
				NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
				NKMUnitTempletBase shipTempletBase = null;
				if (shipFromUID != null)
				{
					shipTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
				}
				NKCUtil.SetGameobjectActive(m_objOperator, !NKCOperatorUtil.IsHide());
				if (deckData.m_OperatorUID != 0L)
				{
					m_Operator.SetData(NKCOperatorUtil.GetOperatorData(deckData.m_OperatorUID));
				}
				else if (NKCOperatorUtil.IsHide())
				{
					m_Operator.SetHide();
				}
				else
				{
					m_Operator.SetEmpty();
				}
				int armyAvarageOperationPower = myUserData.m_ArmyData.GetArmyAvarageOperationPower(m_sNKMDeckIndex);
				NKCUtil.SetLabelText(m_lbAvgPower, armyAvarageOperationPower.ToString("N0"));
				NKCUtil.SetLabelText(m_lbCost, $"{myUserData.m_ArmyData.CalculateDeckAvgSummonCost(m_sNKMDeckIndex):0.00}");
				m_UIShipInfo.SetShipData(shipFromUID, shipTempletBase, m_sNKMDeckIndex);
				NKCUtil.SetLabelText(m_txtShipPower, (shipFromUID != null) ? shipFromUID.CalculateOperationPower(myUserData.m_InventoryData).ToString("N0") : "");
			}
			else
			{
				m_NKM_UI_POPUP_TOP_TEXT.text = "";
				for (num2 = 0; num2 < m_lstNKCDeckViewUnitSlot.Count; num2++)
				{
					if (num2 < 8)
					{
						m_lstNKCDeckViewUnitSlot[num2].SetData(null, bEnableButton: false);
					}
				}
				m_UIShipInfo.SetShipData(null, null, NKMDeckIndex.None);
				NKCUtil.SetLabelText(m_txtShipPower, "");
			}
			if (bPlaying)
			{
				WarfareUnitData unitData2 = NKCScenManager.GetScenManager().WarfareGameData.GetUnitData(m_WarfareGameUnitUID);
				if (unitData2 != null)
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_1, unitData2.supply >= 1);
					NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_2, unitData2.supply >= 2);
					float fRatio2 = unitData2.hp / unitData2.hpMax;
					fRatio2 = GetProperRatioValue(fRatio2);
					if (fRatio2 > 0.6f)
					{
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(1f, 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(1f, 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(GetProperRatioValue((fRatio2 - 0.6f) / 0.4f), 1f, 1f);
					}
					else if (fRatio2 > 0.3f)
					{
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(1f, 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(GetProperRatioValue((fRatio2 - 0.3f) / 0.3f), 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(0f, 1f, 1f);
					}
					else
					{
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(GetProperRatioValue(fRatio2 / 0.3f), 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(0f, 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(0f, 1f, 1f);
					}
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_1, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_2, bValue: true);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(1f, 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(1f, 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			break;
		}
		case ShipType.supporter:
		{
			m_NKM_UI_POPUP_TOP_TEXT.text = m_friendData.commonProfile.nickname;
			m_NKM_UI_POPUP_TOP_TEXT2.text = "";
			m_txtFriendCode.text = NKCUtilString.GetFriendCode(m_friendData.commonProfile.friendCode);
			m_txtFriendDesc.text = m_friendData.message;
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_friendData.deckData.GetShipUnitId());
			if (unitTempletBase != null)
			{
				SetShipNameText(unitTempletBase.GetUnitName());
				SetShipTypeText(unitTempletBase.GetUnitTitle());
				Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
				if (sprite == null)
				{
					NKCAssetResourceData assetResourceUnitInvenIconEmpty = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
					if (assetResourceUnitInvenIconEmpty != null)
					{
						m_ImgUnit.sprite = assetResourceUnitInvenIconEmpty.GetAsset<Sprite>();
					}
					else
					{
						m_ImgUnit.sprite = null;
					}
				}
				else
				{
					m_ImgUnit.sprite = sprite;
				}
			}
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.FillDataFromDummy(m_friendData.deckData.Ship);
			nKMUnitData.m_UnitID = m_friendData.deckData.GetShipUnitId();
			m_UIShipInfo.SetShipData(nKMUnitData, unitTempletBase);
			NKCUtil.SetLabelText(m_txtShipPower, nKMUnitData.CalculateOperationPower(null).ToString("N0"));
			NKCUtil.SetLabelText(m_lbAvgPower, m_friendData.deckData.CalculateOperationPower().ToString("N0"));
			NKCUtil.SetLabelText(m_lbCost, $"{m_friendData.deckData.CalculateSummonCost():0.00}");
			for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
			{
				if (i >= 8 || i >= m_friendData.deckData.List.Length)
				{
					continue;
				}
				NKMDummyUnitData nKMDummyUnitData = m_friendData.deckData.List[i];
				if (nKMDummyUnitData == null)
				{
					m_lstNKCDeckViewUnitSlot[i].SetData(null, bEnableButton: false);
					continue;
				}
				NKMUnitData nKMUnitData2 = new NKMUnitData();
				nKMUnitData2.FillDataFromDummy(nKMDummyUnitData);
				m_lstNKCDeckViewUnitSlot[i].SetData(nKMUnitData2, bEnableButton: false);
				if (m_friendData.deckData.LeaderIndex == i)
				{
					m_lstNKCDeckViewUnitSlot[i].SetLeader(bLeader: true, bEffect: false);
				}
			}
			if (bPlaying)
			{
				WarfareUnitData unitData = NKCScenManager.GetScenManager().WarfareGameData.GetUnitData(m_WarfareGameUnitUID);
				if (unitData != null)
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_1, unitData.supply >= 1);
					NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_2, unitData.supply >= 2);
					float fRatio = unitData.hp / unitData.hpMax;
					fRatio = GetProperRatioValue(fRatio);
					if (fRatio > 0.6f)
					{
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(1f, 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(1f, 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(GetProperRatioValue((fRatio - 0.6f) / 0.4f), 1f, 1f);
					}
					else if (fRatio > 0.3f)
					{
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(1f, 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(GetProperRatioValue((fRatio - 0.3f) / 0.3f), 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(0f, 1f, 1f);
					}
					else
					{
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(GetProperRatioValue(fRatio / 0.3f), 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(0f, 1f, 1f);
						m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(0f, 1f, 1f);
					}
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_1, bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_INFO_BATTLEPOINT_2, bValue: true);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_1.transform.localScale = new Vector3(1f, 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_2.transform.localScale = new Vector3(1f, 1f, 1f);
				m_NKM_UI_POPUP_INFO_ICON_HP_GAUGE_3.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			break;
		}
		case ShipType.dungeon:
		{
			m_NKM_UI_POPUP_ENEMY_INFO_TEXT2.text = NKCUtilString.GET_STRING_WARFARE_POPUP_ENEMY_INFO_KILL;
			m_NKM_UI_POPUP_ENEMY_INFO_TEXT3.text = "";
			NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(m_DungeonID);
			if (dungeonTemplet != null && dungeonTemplet.m_DungeonTempletBase != null)
			{
				Dictionary<string, NKCEnemyData> enemyUnits = NKMDungeonManager.GetEnemyUnits(dungeonTemplet.m_DungeonTempletBase);
				SetDeckViewUnitSlotCount(enemyUnits.Count);
				List<NKCEnemyData> list = new List<NKCEnemyData>(enemyUnits.Values);
				list.Sort(new NKCEnemyData.CompNED());
				int num = 0;
				for (num = 0; num < list.Count; num++)
				{
					NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstDeckViewUnitSlot[num];
					nKCDeckViewUnitSlot.SetEnemyData(NKMUnitManager.GetUnitTempletBase(list[num].m_UnitStrID), list[num]);
					NKCUtil.SetGameobjectActive(nKCDeckViewUnitSlot.gameObject, bValue: true);
				}
				for (; num < m_lstDeckViewUnitSlot.Count; num++)
				{
					NKCUtil.SetGameobjectActive(m_lstDeckViewUnitSlot[num], bValue: false);
				}
				if (dungeonTemplet.m_DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_WAVE)
				{
					m_NKM_UI_POPUP_ENEMY_INFO_TEXT2.text = NKCUtilString.GET_STRING_WARFARE_POPUP_ENEMY_INFO_WAVE;
					m_NKM_UI_POPUP_ENEMY_INFO_TEXT3.text = string.Format(NKCUtilString.GET_STRING_WARFARE_POPUP_ENEMY_INFO_WAVE_ONE_PARAM, dungeonTemplet.m_listDungeonWave.Count);
				}
				m_NKM_UI_POPUP_ENEMY_INFO_TEXT8.text = NKCUtilString.GetDGMissionText(DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR, 0);
				DUNGEON_GAME_MISSION_TYPE dGMissionType_ = dungeonTemplet.m_DungeonTempletBase.m_DGMissionType_1;
				DUNGEON_GAME_MISSION_TYPE dGMissionType_2 = dungeonTemplet.m_DungeonTempletBase.m_DGMissionType_2;
				int dGMissionValue_ = dungeonTemplet.m_DungeonTempletBase.m_DGMissionValue_1;
				int dGMissionValue_2 = dungeonTemplet.m_DungeonTempletBase.m_DGMissionValue_2;
				m_NKM_UI_POPUP_ENEMY_INFO_TEXT7.text = NKCUtilString.GetDGMissionText(dGMissionType_, dGMissionValue_);
				m_NKM_UI_POPUP_ENEMY_INFO_TEXT6.text = NKCUtilString.GetDGMissionText(dGMissionType_2, dGMissionValue_2);
			}
			break;
		}
		}
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void CloseWarfareSelectShipPopup()
	{
		Close();
	}

	public void OnCloseBtn()
	{
		Close();
	}

	public void OnSetFlagShipButton_()
	{
		Close();
		if (m_dOnSetFlagShipButton != null)
		{
			m_dOnSetFlagShipButton(m_WarfareGameUnitUID);
		}
	}

	public void OnCancelBatchButton_()
	{
		Close();
		if (m_dOnCancelBatchButton != null)
		{
			m_dOnCancelBatchButton(m_WarfareGameUnitUID);
		}
	}

	public void OnDeckViewBtn_()
	{
		Close();
		if (m_dOnDeckViewBtn != null)
		{
			m_dOnDeckViewBtn(m_WarfareGameUnitUID);
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_lstDeckViewUnitSlot.Count; i++)
		{
			m_lstDeckViewUnitSlot[i].CloseInstance();
		}
		m_lstDeckViewUnitSlot.Clear();
	}
}
