using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Defence;
using ClientPacket.Game;
using NKC.UI.Collection;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCPopupFierceUserInfo : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_LEADER_BOARD_DETAIL";

	private const string UI_ASSET_NAME = "AB_UI_LEADER_BOARD_DETAIL_INFO_POPUP";

	private static NKCPopupFierceUserInfo m_Instance;

	public NKCUIComStateButton m_csbtnClose;

	[Header("프로필")]
	public NKCUISlotProfile m_ProfileSlot;

	public TMP_Text m_lbLevel;

	public TMP_Text m_lbUserName;

	public TMP_Text m_lbUserCode;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public TMP_Text m_lbGuildName;

	public TMP_Text m_lbUserDesc;

	public GameObject m_objEmblem;

	public List<NKCUISlot> m_lstEmblem;

	public NKCUIComTitlePanel m_TitlePanel;

	public NKCUIComStateButton m_csbtnDeckCopy;

	[Header("상단 배경")]
	public Image m_imgTopBG;

	[Header("타이틀")]
	public TMP_Text m_lbTitle;

	[Header("소대 정보")]
	public Image m_imgShip;

	public NKCUIComStateButton m_btnShip;

	public NKCUIOperatorDeckSlot m_OperatorSlot;

	public List<NKCDeckViewUnitSlot> m_lstNKCDeckViewUnitSlot;

	public Text m_ArmyOperationPower;

	public Text m_ArmyAvgCost;

	[Header("격전지원")]
	public GameObject m_objFierceInfo;

	public RectTransform m_rtBossSlotParents;

	public TMP_Text m_lbBossLv;

	public TMP_Text m_lbBossName;

	public NKCUIFierceBattleBossListSlot m_slotBoss;

	[Header("패널티/점수")]
	public GameObject m_objNonePenalty;

	public Text m_lbPenaltyTitle;

	public Text m_lbBaseScore;

	public Text m_lbBonus;

	public Text m_lbTotalScore;

	public RectTransform m_rtResultSlotParent;

	public NKCUIFierceBattleSelfPenaltySumSlot m_pfbResultSlot;

	private List<NKCUIFierceBattleSelfPenaltySumSlot> m_lstSumSlots = new List<NKCUIFierceBattleSelfPenaltySumSlot>();

	[Header("디펜스")]
	public GameObject m_objDefence;

	public TMP_Text m_lbMyDefenceRank;

	public Text m_lbMyDefenceScore;

	public GameObject m_objMasking;

	public EventTrigger m_evt;

	private List<NKMUnitData> m_lstNKMUnitData = new List<NKMUnitData>();

	private List<NKMEquipItemData> m_lstEquipData = new List<NKMEquipItemData>();

	private NKMOperator m_operatorData;

	private NKMUnitData m_ShipUnitData;

	private NKMDummyDeckData m_ProfileDeckData;

	private NKMAsyncDeckData m_ProfileDeckDataAsync;

	private long m_lUserUID;

	public static NKCPopupFierceUserInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFierceUserInfo>("AB_UI_LEADER_BOARD_DETAIL", "AB_UI_LEADER_BOARD_DETAIL_INFO_POPUP", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupFierceUserInfo>();
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

	public override string MenuName => NKCUtilString.GET_STRING_FRIEND_INFO;

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

	public static bool IsHasInstance()
	{
		return m_Instance != null;
	}

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstNKCDeckViewUnitSlot[i];
			if (nKCDeckViewUnitSlot != null)
			{
				nKCDeckViewUnitSlot.Init(i);
			}
		}
		NKCUtil.SetBindFunction(m_csbtnClose, base.Close);
		m_OperatorSlot.Init(OnClickOperator);
		if (m_btnShip != null)
		{
			m_btnShip.PointerClick.RemoveAllListeners();
			m_btnShip.PointerClick.AddListener(OnClickShip);
		}
		NKCUtil.SetBindFunction(m_csbtnDeckCopy, OnClickDeckCopy);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKMPacket_FIERCE_PROFILE_ACK fierceProfileData)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objFierceInfo, bValue: true);
		NKCUtil.SetGameobjectActive(m_objDefence, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMasking, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnDeckCopy, NKMOpenTagManager.IsOpened("COPY_SQUAD"));
		NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_FIERCE_RECODE_TITLE);
		m_ProfileDeckData = null;
		m_ProfileDeckDataAsync = null;
		if (fierceProfileData != null)
		{
			m_ProfileDeckData = fierceProfileData.profileData.profileDeck;
			NKCUtil.SetImageSprite(m_imgTopBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_LEADER_BOARD_DETAIL_BG", "LEADER_BOARD_DETAIL_BG_FIERCE"));
			UpdateProfileData(fierceProfileData.commonProfile);
			SetFierceBossData(fierceProfileData.profileData);
			UpdateFierceData(fierceProfileData.profileData);
			NKCUtil.SetLabelText(m_lbUserDesc, NKCFilterManager.CheckBadChat(fierceProfileData.friendIntro));
			UpdateEmblem(fierceProfileData.profileData.emblems);
			UpdateGuildData(fierceProfileData.guildData);
		}
		if (null != m_rtResultSlotParent)
		{
			m_rtResultSlotParent.anchoredPosition = Vector2.zero;
		}
		if (!m_Instance.IsOpen)
		{
			UIOpened();
		}
	}

	public void OpenForDefence(NKMPacket_DEFENCE_PROFILE_ACK sPacket)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objFierceInfo, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDefence, bValue: true);
		NKCUtil.SetGameobjectActive(m_objMasking, NKCDefenceDungeonManager.NeedHideDeckInfo() && NKCScenManager.CurrentUserData().m_UserUID != sPacket.commonProfile.userUid);
		NKCUtil.SetGameobjectActive(m_csbtnDeckCopy, bValue: false);
		NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_DEF_RECORD_TITLE);
		m_ProfileDeckData = null;
		m_ProfileDeckDataAsync = null;
		if (sPacket != null)
		{
			NKCUtil.SetImageSprite(m_imgTopBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_LEADER_BOARD_DETAIL_BG", "LEADER_BOARD_DETAIL_BG_DEF"));
			UpdateProfileData(sPacket.commonProfile);
			UpdateDefenceData(sPacket.profileData, sPacket.rank, sPacket.rankPercent);
			NKCUtil.SetLabelText(m_lbUserDesc, NKCFilterManager.CheckBadChat(sPacket.friendIntro));
			UpdateEmblem(sPacket.profileData.emblems);
			UpdateGuildData(sPacket.guildData);
			m_ProfileDeckDataAsync = sPacket.profileData.profileDeck;
		}
		if (null != m_rtResultSlotParent)
		{
			m_rtResultSlotParent.anchoredPosition = Vector2.zero;
		}
		if (!m_Instance.IsOpen)
		{
			UIOpened();
		}
	}

	private void UpdateProfileData(NKMCommonProfile profile)
	{
		if (profile != null)
		{
			m_lUserUID = profile.userUid;
			m_ProfileSlot.SetProfiledata(profile, null);
			NKCUtil.SetLabelText(m_lbUserName, profile.nickname);
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_FRIEND_INFO_LEVEL_ONE_PARAM, profile.level));
			NKCUtil.SetLabelText(m_lbUserCode, NKCUtilString.GetFriendCode(profile.friendCode));
			m_TitlePanel?.SetData(profile);
		}
	}

	private void UpdateEmblem(List<NKMEmblemData> emblems)
	{
		if (emblems != null)
		{
			NKCUtil.SetGameobjectActive(m_objEmblem, bValue: true);
			for (int i = 0; i < m_lstEmblem.Count; i++)
			{
				NKCUISlot nKCUISlot = m_lstEmblem[i];
				if (i < emblems.Count && emblems[i] != null && emblems[i].id > 0 && NKMItemManager.GetItemMiscTempletByID(emblems[i].id) != null)
				{
					if (i <= 3)
					{
						nKCUISlot.SetMiscItemData(emblems[i].id, emblems[i].count, bShowName: false, bShowCount: true, bEnableLayoutElement: true, null);
					}
					else
					{
						nKCUISlot.SetEmpty();
					}
				}
				else
				{
					nKCUISlot.SetEmpty();
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEmblem, bValue: false);
		}
	}

	public bool IsSameProfile(long userUID)
	{
		return userUID == m_lUserUID;
	}

	public void SetFierceBossData(NKMFierceProfileData fierceProfile)
	{
		if (fierceProfile != null)
		{
			if (null != m_slotBoss)
			{
				m_slotBoss.SetData(fierceProfile.fierceBossGroupId);
				NKCUtil.SetBindFunction(m_slotBoss.m_csbtnBtn);
				m_slotBoss.SetHasRecord(fierceProfile.totalPoint > 0);
			}
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			int matchedBossLv = nKCFierceBattleSupportDataMgr.GetMatchedBossLv(fierceProfile.fierceBossGroupId, fierceProfile.fierceBossId);
			NKCUtil.SetLabelText(m_lbBossLv, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, matchedBossLv));
			NKCUtil.SetLabelText(m_lbBossName, nKCFierceBattleSupportDataMgr.GetCurBossName());
		}
	}

	public void UpdateFierceData(NKMFierceProfileData fierceProfile)
	{
		if (fierceProfile == null)
		{
			return;
		}
		ClearCurResultSlots();
		SetUnitList(fierceProfile.profileDeck);
		UpdateDeckInfo(fierceProfile.operationPower);
		UpdateDeckUnit(fierceProfile.profileDeck, bSetFIrstDeckLeader: false, bShowEmptySlot: true);
		int num = fierceProfile.totalPoint - fierceProfile.penaltyPoint;
		NKCUtil.SetLabelText(m_lbBaseScore, num.ToString());
		NKCUtil.SetLabelText(m_lbTotalScore, fierceProfile.totalPoint.ToString());
		NKCUtil.SetGameobjectActive(m_objNonePenalty, fierceProfile.penaltyIds.Count <= 0);
		NKCUtil.SetGameobjectActive(m_lbPenaltyTitle.gameObject, fierceProfile.penaltyIds.Count > 0);
		float num2 = 0f;
		foreach (int penaltyId in fierceProfile.penaltyIds)
		{
			NKMFiercePenaltyTemplet nKMFiercePenaltyTemplet = NKMFiercePenaltyTemplet.Find(penaltyId);
			if (nKMFiercePenaltyTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbPenaltyTitle, NKCUtilString.GET_STRING_FIERCE_PENALTY_TITLE_DEBUFF);
				NKCUIFierceBattleSelfPenaltySumSlot nKCUIFierceBattleSelfPenaltySumSlot = Object.Instantiate(m_pfbResultSlot);
				if (null != nKCUIFierceBattleSelfPenaltySumSlot)
				{
					nKCUIFierceBattleSelfPenaltySumSlot.SetData(nKMFiercePenaltyTemplet);
					nKCUIFierceBattleSelfPenaltySumSlot.transform.SetParent(m_rtResultSlotParent);
					m_lstSumSlots.Add(nKCUIFierceBattleSelfPenaltySumSlot);
				}
				num2 += nKMFiercePenaltyTemplet.FierceScoreRate;
			}
		}
		num2 *= 0.01f;
		if (num2 < 0f)
		{
			num2 *= -1f;
			NKCUtil.SetLabelText(m_lbBonus, string.Format(NKCUtilString.GET_STRING_FIERCE_PENALTY_SCORE_MINUS, num2));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbBonus, string.Format(NKCUtilString.GET_STRING_FIERCE_PENALTY_SCORE_PLUS, num2));
		}
	}

	private void UpdateDefenceData(NKMDefenceProfileData defenceProfileData, int rankNum, int rankPercent)
	{
		if (defenceProfileData != null)
		{
			ClearCurResultSlots();
			SetUnitList(defenceProfileData.profileDeck);
			UpdateDeckInfo(defenceProfileData.profileDeck.operationPower);
			UpdateDeckUnit(defenceProfileData.profileDeck);
			if (NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_DEFENCE, 0) != null)
			{
				NKCUtil.SetLabelText(m_lbMyDefenceRank, GetRankDesc(rankNum, rankPercent));
				NKCUtil.SetLabelText(m_lbMyDefenceScore, defenceProfileData.bestPoint.ToString());
			}
		}
	}

	private string GetRankDesc(int rankNum, int rankPercent)
	{
		if (rankNum != 0 && rankNum <= 100)
		{
			return string.Format(NKCUtilString.GET_FIERCE_RANK_IN_TOP_100_DESC_01, rankNum);
		}
		return string.Format(NKCUtilString.GET_FIERCE_RANK_DESC_01, rankPercent);
	}

	private void UpdateDeckInfo(int operationPower)
	{
		int num = 0;
		int num2 = 0;
		foreach (NKMUnitData lstNKMUnitDatum in m_lstNKMUnitData)
		{
			if (lstNKMUnitDatum != null)
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(lstNKMUnitDatum.m_UnitID);
				if (unitStatTemplet != null)
				{
					num += unitStatTemplet.GetRespawnCost(num2 == 0, null, null);
					num2++;
				}
			}
		}
		NKCUtil.SetLabelText(m_ArmyOperationPower, operationPower.ToString("N0"));
		NKCUtil.SetLabelText(m_ArmyAvgCost, $"{((num2 > 0) ? ((float)num / (float)num2) : 0f):0.00}");
	}

	private void UpdateDeckUnit(NKMDummyDeckData deckData, bool bSetFIrstDeckLeader = false, bool bShowEmptySlot = false)
	{
		m_operatorData = null;
		m_ShipUnitData = null;
		if (deckData.operatorUnit != null)
		{
			UpdateDeckUnit(deckData.operatorUnit.UnitId, deckData.operatorUnit.UnitLevel, deckData.Ship.UnitId, deckData.LeaderIndex, bShowUnitDetail: false, bSetFIrstDeckLeader, bShowEmptySlot);
		}
		else
		{
			UpdateDeckUnit(0, 0, deckData.Ship.UnitId, deckData.LeaderIndex, bShowUnitDetail: false, bSetFIrstDeckLeader, bShowEmptySlot);
		}
	}

	private void UpdateDeckUnit(NKMAsyncDeckData deckData, bool bSetFirstDeckLeader = false)
	{
		m_operatorData = deckData.operatorUnit;
		m_ShipUnitData = null;
		m_ShipUnitData = new NKMUnitData();
		m_ShipUnitData.FillDataFromAsyncUnitData(deckData.ship);
		if (deckData.operatorUnit != null)
		{
			UpdateDeckUnit(deckData.operatorUnit.id, deckData.operatorUnit.level, deckData.ship.unitId, deckData.leaderIndex, bShowUnitDetail: true, bSetFirstDeckLeader);
		}
		else
		{
			UpdateDeckUnit(0, 0, deckData.ship.unitId, deckData.leaderIndex, bShowUnitDetail: true, bSetFirstDeckLeader);
		}
	}

	private void UpdateDeckUnit(int operatorId, int operatorLevel, int shipId, int leaderIndex, bool bShowUnitDetail = false, bool bSetFirstDeckLeader = false, bool bShowEmptySlot = false)
	{
		if (m_lstNKMUnitData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipId);
			if (unitTempletBase != null)
			{
				m_imgShip.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
			}
			else
			{
				m_imgShip.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_TEXTURE", "NKM_DECK_VIEW_SHIP_UNKNOWN");
			}
			for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
			{
				NKCDeckViewUnitSlot slot = m_lstNKCDeckViewUnitSlot[i];
				if (i < m_lstNKMUnitData.Count)
				{
					NKMUnitData nKMUnitData = m_lstNKMUnitData[i];
					slot.SetData(nKMUnitData, bShowUnitDetail);
					slot.SetLeader(i == leaderIndex, bEffect: false);
					if ((nKMUnitData == null || (nKMUnitData != null && nKMUnitData.GetUnitTempletBase() == null)) && bShowEmptySlot)
					{
						slot.SetIconEtcDefault();
					}
					if (bShowUnitDetail)
					{
						slot.m_NKCUIComButton.PointerClick.RemoveAllListeners();
						slot.m_NKCUIComButton.PointerClick.AddListener(delegate
						{
							OnClickUnitSlot(slot.m_Index);
						});
					}
					if (bSetFirstDeckLeader && i == 0)
					{
						slot.SetLeader(bLeader: true, bEffect: false);
					}
				}
				else
				{
					slot.SetData(null, bShowUnitDetail);
				}
			}
			if (NKCOperatorUtil.IsHide())
			{
				NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: false);
			}
			else if (operatorId > 0)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(operatorId);
				if (unitTempletBase2 != null)
				{
					m_OperatorSlot.SetData(unitTempletBase2, operatorLevel);
				}
				else
				{
					m_OperatorSlot.SetEmpty();
				}
			}
			else
			{
				m_OperatorSlot.SetEmpty();
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: false);
		}
	}

	private void SetUnitList(NKMAsyncDeckData deckData)
	{
		m_lstNKMUnitData = new List<NKMUnitData>();
		m_lstEquipData = deckData.equips;
		for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
		{
			if (i < deckData.units.Count)
			{
				NKMAsyncUnitData nKMAsyncUnitData = deckData.units[i];
				NKMUnitData nKMUnitData = new NKMUnitData();
				if (nKMAsyncUnitData != null && nKMAsyncUnitData.unitId > 0)
				{
					nKMUnitData.FillDataFromAsyncUnitData(nKMAsyncUnitData);
					nKMUnitData.m_UnitUID = i;
				}
				m_lstNKMUnitData.Add(nKMUnitData);
			}
		}
	}

	private void SetUnitList(NKMDummyDeckData deckData)
	{
		m_lstNKMUnitData = new List<NKMUnitData>();
		m_lstEquipData = new List<NKMEquipItemData>();
		for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
		{
			if (i < deckData.List.Length)
			{
				NKMDummyUnitData nKMDummyUnitData = deckData.List[i];
				NKMUnitData nKMUnitData = new NKMUnitData();
				if (nKMDummyUnitData != null && nKMDummyUnitData.UnitId > 0)
				{
					nKMUnitData.FillDataFromDummy(nKMDummyUnitData);
					nKMUnitData.m_UnitUID = i;
				}
				m_lstNKMUnitData.Add(nKMUnitData);
			}
		}
	}

	private void OnClickUnitSlot(int index)
	{
		SelectDeckViewUnit(index, m_lstNKCDeckViewUnitSlot, m_lstNKMUnitData, m_lstEquipData);
	}

	public void SelectDeckViewUnit(int selectedIndex, List<NKCDeckViewUnitSlot> listNKCDeckViewUnitSlot, List<NKMUnitData> listNKMUnitData, List<NKMEquipItemData> listNKMEquipItemData)
	{
		for (int i = 0; i < listNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = listNKCDeckViewUnitSlot[i];
			if (i != selectedIndex)
			{
				nKCDeckViewUnitSlot.ButtonDeSelect();
			}
			else if (nKCDeckViewUnitSlot.m_NKMUnitData != null)
			{
				nKCDeckViewUnitSlot.ButtonSelect();
				NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(listNKMUnitData, i);
				NKCUICollectionUnitInfo.CheckInstanceAndOpen(nKCDeckViewUnitSlot.m_NKMUnitData, openOption, listNKMEquipItemData, NKCUICollectionUnitInfo.eCollectionState.CS_STATUS, isGauntlet: true, NKCUIUpsideMenu.eMode.Normal, bWillCloseUnderPopupOnOpen: false);
			}
		}
	}

	private void OnClickOperator(long unitUID)
	{
		if (m_operatorData != null)
		{
			NKCUICollectionOperatorInfo.Instance.Open(m_operatorData, null, NKCUICollectionOperatorInfo.eCollectionState.CS_STATUS, NKCUIUpsideMenu.eMode.Normal, isGauntlet: true);
		}
	}

	public void OnClickShip()
	{
		if (m_ShipUnitData != null)
		{
			NKCUICollectionShipInfo.CheckInstanceAndOpen(m_ShipUnitData, NKMDeckIndex.None, null, null, isGauntlet: true);
		}
	}

	public void OnClickDeckCopy()
	{
		if (m_ProfileDeckData != null)
		{
			int shipID = ((m_ProfileDeckData.Ship != null) ? m_ProfileDeckData.Ship.UnitId : 0);
			int operID = ((m_ProfileDeckData.operatorUnit != null) ? m_ProfileDeckData.operatorUnit.UnitId : 0);
			List<int> list = new List<int>();
			int leaderIndex = m_ProfileDeckData.LeaderIndex;
			for (int i = 0; i < 8; i++)
			{
				if (m_ProfileDeckData.List.Length <= i)
				{
					list.Add(0);
					continue;
				}
				NKMDummyUnitData nKMDummyUnitData = m_ProfileDeckData.List[i];
				if (nKMDummyUnitData == null)
				{
					list.Add(0);
				}
				else
				{
					list.Add(nKMDummyUnitData.UnitId);
				}
			}
			NKCPopupDeckCopy.MakeDeckCopyCode(shipID, operID, list, leaderIndex);
		}
		else
		{
			if (m_ProfileDeckDataAsync == null)
			{
				return;
			}
			int shipID2 = ((m_ProfileDeckDataAsync.ship != null) ? m_ProfileDeckDataAsync.ship.unitId : 0);
			int operID2 = ((m_ProfileDeckDataAsync.operatorUnit != null) ? m_ProfileDeckDataAsync.operatorUnit.id : 0);
			List<int> list2 = new List<int>();
			int leaderIndex2 = m_ProfileDeckDataAsync.leaderIndex;
			for (int j = 0; j < 8; j++)
			{
				if (m_ProfileDeckDataAsync.units.Count <= j)
				{
					list2.Add(0);
					continue;
				}
				NKMAsyncUnitData nKMAsyncUnitData = m_ProfileDeckDataAsync.units[j];
				if (nKMAsyncUnitData == null)
				{
					list2.Add(0);
				}
				else
				{
					list2.Add(nKMAsyncUnitData.unitId);
				}
			}
			NKCPopupDeckCopy.MakeDeckCopyCode(shipID2, operID2, list2, leaderIndex2);
		}
	}

	private void UpdateGuildData(NKMGuildSimpleData guildData)
	{
		if (!(m_objGuild == null))
		{
			NKCUtil.SetGameobjectActive(m_objGuild, guildData != null && guildData.guildUid > 0);
			if (m_objGuild.activeSelf && guildData != null)
			{
				m_BadgeUI.SetData(guildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, guildData.guildName);
			}
		}
	}

	private void ClearCurResultSlots()
	{
		if (m_lstSumSlots == null)
		{
			return;
		}
		for (int i = 0; i < m_lstSumSlots.Count; i++)
		{
			if (null != m_lstSumSlots[i])
			{
				Object.Destroy(m_lstSumSlots[i].gameObject);
				m_lstSumSlots[i] = null;
			}
		}
		m_lstSumSlots.Clear();
	}
}
