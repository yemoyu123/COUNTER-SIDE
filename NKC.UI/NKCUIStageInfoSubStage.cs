using System.Collections.Generic;
using ClientPacket.User;
using Cs.Logging;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIStageInfoSubStage : NKCUIStageInfoSubBase
{
	private struct BattleCondition
	{
		public Image Img;

		public NKCUIComStateButton Btn;

		public BattleCondition(Image _img, NKCUIComStateButton _btn)
		{
			Img = _img;
			Btn = _btn;
		}
	}

	[Header("스테이지")]
	[Header("상단")]
	public NKCUIComToggle m_tglFavorite;

	public TMP_Text m_lbLevel;

	[Space]
	[Header("중단")]
	[Space]
	[Header("스테이지정보")]
	public GameObject m_objStageInfo;

	public NKCUIComToggle m_tglStageInfo;

	public NKCUIComToggle m_tglIngameInfo;

	[Space]
	public GameObject m_objMedalParent;

	public NKCUIComMedal m_Mission;

	[Space]
	[Header("인게임 정보")]
	public GameObject m_objIngameInfo;

	public NKCUIComStateButton m_btnEnemyLevel;

	public Text m_lbEnemyLevel;

	[Header("근원성")]
	public GameObject m_objWeak;

	public Image m_imgWeakMain;

	public Image m_imgWeakSub;

	[Header("전투환경")]
	public GameObject m_objBattleCond;

	public Image m_imgBattleCond;

	public Text m_lbBattleCondTitle;

	public Text m_lbBattleCondDesc;

	[Header("팀업")]
	public GameObject m_objTeamUP;

	public RectTransform m_rtTeamUpParent;

	[Space]
	[Header("하단")]
	public GameObject m_ObjStoryReplay;

	public NKCUIComToggle m_tglStoryReplay;

	public NKCUIComStateButton m_btnReady;

	public GameObject m_objReadyResourceParent;

	public Text m_lbReadyResource;

	public Image m_imgReadyResource;

	public Text m_lbReadyResourceLocked;

	public Image m_imgReadyResourceLocked;

	[Header("스킵")]
	public NKCUIComToggle m_tglSkip;

	public NKCUIOperationSkip m_NKCUIOperationSkip;

	[Header("버프 적용 유닛 노출 ID 범위")]
	public int m_iMinDisplayUnitID = 1001;

	public int m_iMaxDisplayUnitID = 10000;

	private List<BattleCondition> m_listBattleConditionSlot = new List<BattleCondition>();

	private List<NKCUISlot> m_lstTeamUpUnits = new List<NKCUISlot>();

	public override void InitUI(OnButton onButton)
	{
		base.InitUI(onButton);
		m_tglStageInfo.OnValueChanged.RemoveAllListeners();
		m_tglStageInfo.OnValueChanged.AddListener(OnStageInfo);
		m_tglIngameInfo.OnValueChanged.RemoveAllListeners();
		m_tglIngameInfo.OnValueChanged.AddListener(OnIngameInfo);
		m_btnEnemyLevel.PointerClick.RemoveAllListeners();
		m_btnEnemyLevel.PointerClick.AddListener(OnClickEnemyLevel);
		m_tglStoryReplay.OnValueChanged.RemoveAllListeners();
		m_tglStoryReplay.OnValueChanged.AddListener(OnStoryReplay);
		m_btnReady.PointerClick.RemoveAllListeners();
		m_btnReady.PointerClick.AddListener(base.OnOK);
		m_btnReady.m_bGetCallbackWhileLocked = true;
		m_btnReady.SetHotkey(HotkeyEventType.Confirm);
		m_tglFavorite.OnValueChanged.RemoveAllListeners();
		m_tglFavorite.OnValueChanged.AddListener(OnClickFavorite);
		NKCUtil.SetToggleValueChangedDelegate(m_tglSkip, OnTglSkip);
		NKCUtil.SetHotkey(m_tglSkip, HotkeyEventType.RotateLeft, null, bUpDownEvent: true);
		if (m_NKCUIOperationSkip != null)
		{
			m_NKCUIOperationSkip.Init(OnOperationSkipUpdated, OnClickOperationSkipClose);
		}
		NKCUtil.SetGameobjectActive(m_NKCUIOperationSkip, bValue: false);
		m_SkipCount = 1;
		m_bOperationSkip = false;
	}

	public void Close()
	{
		ClearTeamUPData();
	}

	public override void SetData(NKMStageTempletV2 stageTemplet, bool bFirstOpen = true)
	{
		base.SetData(stageTemplet, bFirstOpen);
		SetStageInfo();
		if (bFirstOpen)
		{
			m_tglStageInfo.Select(bSelect: true);
		}
		m_tglFavorite.Select(NKMEpisodeMgr.GetFavoriteStageList().ContainsValue(stageTemplet), bForce: true);
		NKCUtil.SetGameobjectActive(m_NKCUIOperationSkip, bValue: false);
		m_SkipCount = 1;
		m_bOperationSkip = false;
		if (m_tglSkip != null)
		{
			m_tglSkip.Select(bSelect: false, bForce: true);
		}
		NKCUtil.SetGameobjectActive(m_tglSkip, stageTemplet.m_bActiveBattleSkip);
		m_tglStoryReplay.Select(NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene, bForce: true);
		UpdateStageRequiredItem();
	}

	private void UpdateStageRequiredItem()
	{
		SetStageRequiredItem(m_objReadyResourceParent, m_imgReadyResource, m_lbReadyResource, m_StageTemplet);
		SetStageRequiredItem(m_objReadyResourceParent, m_imgReadyResourceLocked, m_lbReadyResourceLocked, m_StageTemplet);
	}

	private void SetStageInfo()
	{
		NKCUtil.SetGameobjectActive(m_objWeak, bValue: false);
		switch (m_StageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = m_StageTemplet.DungeonTempletBase;
			if (dungeonTempletBase == null)
			{
				break;
			}
			m_Mission?.SetData(dungeonTempletBase);
			NKCUtil.SetLabelText(m_lbEnemyLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, dungeonTempletBase.m_DungeonLevel));
			SetBattleConditionUI(dungeonTempletBase);
			NKCUtil.SetGameobjectActive(m_ObjStoryReplay, dungeonTempletBase.HasCutscen());
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, dungeonTempletBase.m_DungeonLevel));
			if (dungeonTempletBase.m_StageSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				NKCUtil.SetGameobjectActive(m_objWeak, dungeonTempletBase.m_StageSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
				NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(dungeonTempletBase.m_StageSourceTypeMain), bDisableIfSpriteNull: true);
				NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(dungeonTempletBase.m_StageSourceTypeSub), bDisableIfSpriteNull: true);
				break;
			}
			if (dungeonTempletBase.m_GuideSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				NKCUtil.SetGameobjectActive(m_objWeak, dungeonTempletBase.m_GuideSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
				NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(dungeonTempletBase.m_GuideSourceTypeMain), bDisableIfSpriteNull: true);
				NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(dungeonTempletBase.m_GuideSourceTypeSub), bDisableIfSpriteNull: true);
				break;
			}
			NKMDungeonTemplet dungeonTemplet2 = NKMDungeonManager.GetDungeonTemplet(dungeonTempletBase.m_DungeonStrID);
			if (dungeonTemplet2 != null)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(dungeonTemplet2.m_BossUnitStrID);
				if (unitTempletBase2 != null)
				{
					NKCUtil.SetGameobjectActive(m_objWeak, unitTempletBase2.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
					NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(unitTempletBase2.m_NKM_UNIT_SOURCE_TYPE), bDisableIfSpriteNull: true);
					NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(unitTempletBase2.m_NKM_UNIT_SOURCE_TYPE_SUB), bDisableIfSpriteNull: true);
				}
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
		{
			NKMPhaseTemplet phaseTemplet = m_StageTemplet.PhaseTemplet;
			if (phaseTemplet == null)
			{
				break;
			}
			for (int i = 0; i < phaseTemplet.PhaseList.List.Count; i++)
			{
				NKMDungeonTempletBase dungeon = phaseTemplet.PhaseList.List[i].Dungeon;
				if (dungeon.m_StageSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
				{
					NKCUtil.SetGameobjectActive(m_objWeak, dungeon.m_StageSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
					NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(dungeon.m_StageSourceTypeMain), bDisableIfSpriteNull: true);
					NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(dungeon.m_StageSourceTypeSub), bDisableIfSpriteNull: true);
					break;
				}
				if (dungeon.m_GuideSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
				{
					NKCUtil.SetGameobjectActive(m_objWeak, dungeon.m_GuideSourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
					NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(dungeon.m_GuideSourceTypeMain), bDisableIfSpriteNull: true);
					NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(dungeon.m_GuideSourceTypeSub), bDisableIfSpriteNull: true);
					break;
				}
				NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(dungeon.m_DungeonStrID);
				if (dungeonTemplet != null)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(dungeonTemplet.m_BossUnitStrID);
					if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
					{
						NKCUtil.SetGameobjectActive(m_objWeak, unitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
						NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE), bDisableIfSpriteNull: true);
						NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(unitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB), bDisableIfSpriteNull: true);
						break;
					}
				}
			}
			m_Mission?.SetData(phaseTemplet);
			NKCUtil.SetLabelText(m_lbEnemyLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, phaseTemplet.PhaseLevel));
			SetBattleConditionUI(phaseTemplet);
			NKCUtil.SetGameobjectActive(m_ObjStoryReplay, HasCutScen(phaseTemplet));
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, phaseTemplet.PhaseLevel));
			break;
		}
		default:
			Log.Warn($"던전 정보창에 등록되지 않은 타입 - StageID : {m_StageTemplet.Key}, StageType : {m_StageTemplet.m_STAGE_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIStageInfoSubStage.cs", 268);
			break;
		}
	}

	private void SetBattleConditionUI(NKMDungeonTempletBase dungeonTempletBase)
	{
		UpdateBattleConditionUI(dungeonTempletBase.BattleConditions);
	}

	private void SetBattleConditionUI(NKMPhaseTemplet phaseTemplet)
	{
		List<NKMBattleConditionTemplet> list = new List<NKMBattleConditionTemplet>();
		if (phaseTemplet != null)
		{
			foreach (NKMPhaseOrderTemplet item in phaseTemplet.PhaseList.List)
			{
				if (item.Dungeon == null || item.Dungeon.BattleConditions == null || item.Dungeon.BattleConditions.Count <= 0)
				{
					continue;
				}
				foreach (NKMBattleConditionTemplet battleConditionTemplet in item.Dungeon.BattleConditions)
				{
					if (list.FindIndex((NKMBattleConditionTemplet e) => e == battleConditionTemplet) == -1)
					{
						list.Add(battleConditionTemplet);
					}
				}
			}
		}
		UpdateBattleConditionUI(list);
	}

	private void UpdateBattleConditionUI(List<NKMBattleConditionTemplet> listBattleConditionTemplet)
	{
		listBattleConditionTemplet?.RemoveAll((NKMBattleConditionTemplet x) => x.m_bHide);
		if (listBattleConditionTemplet == null || listBattleConditionTemplet.Count == 0)
		{
			NKCUtil.SetGameobjectActive(m_objBattleCond, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTeamUP, bValue: false);
			return;
		}
		NKMBattleConditionTemplet nKMBattleConditionTemplet = listBattleConditionTemplet[0];
		if (nKMBattleConditionTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_objBattleCond, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTeamUP, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objBattleCond, bValue: true);
		if (nKMBattleConditionTemplet != null)
		{
			Sprite spriteBattleConditionICon = NKCUtil.GetSpriteBattleConditionICon(nKMBattleConditionTemplet);
			if (spriteBattleConditionICon != null)
			{
				NKCUtil.SetImageSprite(m_imgBattleCond, spriteBattleConditionICon);
			}
			NKCUtil.SetLabelText(m_lbBattleCondTitle, nKMBattleConditionTemplet.BattleCondName_Translated);
			NKCUtil.SetLabelText(m_lbBattleCondDesc, nKMBattleConditionTemplet.BattleCondDesc_Translated);
		}
		UpdateBattleConditionTeamUpUI(listBattleConditionTemplet);
	}

	private void UpdateBattleConditionTeamUpUI(List<NKMBattleConditionTemplet> listBattleConditionTemplet)
	{
		ClearTeamUPData();
		List<int> list = new List<int>();
		if (listBattleConditionTemplet != null && listBattleConditionTemplet.Count > 0)
		{
			List<string> list2 = new List<string>();
			foreach (NKMBattleConditionTemplet item in listBattleConditionTemplet)
			{
				if (item == null)
				{
					continue;
				}
				foreach (string item2 in item.AffectTeamUpID)
				{
					if (!list2.Contains(item2))
					{
						list2.Add(item2);
					}
				}
				foreach (int item3 in item.hashAffectUnitID)
				{
					if (!list.Contains(item3))
					{
						list.Add(item3);
					}
				}
			}
			List<NKMUnitTempletBase> list3 = new List<NKMUnitTempletBase>();
			if (list2.Count > 0)
			{
				foreach (string item4 in list2)
				{
					foreach (NKMUnitTempletBase item5 in NKMUnitManager.GetListTeamUPUnitTempletBase(item4))
					{
						if (!list3.Contains(item5))
						{
							list3.Add(item5);
						}
					}
				}
			}
			if (list3.Count == 0 && list.Count > 0)
			{
				foreach (int item6 in list)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item6);
					if (unitTempletBase != null && !list3.Contains(unitTempletBase))
					{
						list3.Add(unitTempletBase);
					}
				}
			}
			foreach (NKMUnitTempletBase item7 in list3)
			{
				if (item7.PickupEnableByTag && item7.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && item7.m_UnitID >= m_iMinDisplayUnitID && item7.m_UnitID <= m_iMaxDisplayUnitID && (item7.m_ShipGroupID == 0 || item7.m_ShipGroupID == item7.m_UnitID))
				{
					NKCUtil.SetGameobjectActive(m_objTeamUP, bValue: true);
					NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_rtTeamUpParent.transform);
					if (null != newInstance)
					{
						newInstance.transform.localPosition = Vector3.zero;
						newInstance.transform.localScale = Vector3.one;
						NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_UNIT, item7.m_UnitID, 1);
						NKCUtil.SetGameobjectActive(newInstance.gameObject, bValue: true);
						newInstance.SetData(data);
						m_lstTeamUpUnits.Add(newInstance);
					}
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objTeamUP, m_lstTeamUpUnits.Count > 0);
	}

	private void SetIngameInfo()
	{
	}

	private void ClearTeamUPData()
	{
		for (int i = 0; i < m_lstTeamUpUnits.Count; i++)
		{
			Object.Destroy(m_lstTeamUpUnits[i]);
			m_lstTeamUpUnits[i] = null;
		}
		m_lstTeamUpUnits.Clear();
	}

	private void SetStageRequiredItem(GameObject itemObject, Image itemIcon, Text itemCount, NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(itemObject, stageTemplet.m_StageReqItemID != 0 && stageTemplet.m_StageReqItemCount > 0);
		int eternium = stageTemplet.m_StageReqItemCount * m_SkipCount;
		if (stageTemplet.m_StageReqItemID == 2)
		{
			if (stageTemplet.WarfareTemplet != null)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringWarfare(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
			else if (stageTemplet.DungeonTempletBase != null)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
			else if (stageTemplet.PhaseTemplet != null)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
		}
		NKCUtil.SetLabelText(itemCount, eternium.ToString("#,##0"));
		if (stageTemplet.m_StageReqItemID > 0)
		{
			if (eternium > NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(stageTemplet.m_StageReqItemID))
			{
				NKCUtil.SetLabelTextColor(itemCount, Color.red);
			}
			else
			{
				NKCUtil.SetLabelTextColor(itemCount, Color.white);
			}
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(stageTemplet.m_StageReqItemID);
		if (itemMiscTempletByID != null)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
			NKCUtil.SetImageSprite(itemIcon, orLoadMiscItemSmallIcon);
		}
	}

	public void RefreshFavoriteState()
	{
		m_tglFavorite.Select(NKMEpisodeMgr.GetFavoriteStageList().ContainsValue(m_StageTemplet), bForce: true);
	}

	private void OnStageInfo(bool bValue)
	{
		if (bValue)
		{
			NKCUtil.SetGameobjectActive(m_objStageInfo, bValue: true);
			NKCUtil.SetGameobjectActive(m_objIngameInfo, bValue: false);
		}
	}

	private void OnIngameInfo(bool bValue)
	{
		if (bValue)
		{
			NKCUtil.SetGameobjectActive(m_objStageInfo, bValue: false);
			NKCUtil.SetGameobjectActive(m_objIngameInfo, bValue: true);
		}
	}

	private void OnClickEnemyLevel()
	{
		NKCPopupEnemyList.Instance.Open(m_StageTemplet);
	}

	private void OnStoryReplay(bool bValue)
	{
		NKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ nKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ = new NKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ();
		nKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ.isPlayCutscene = m_tglStoryReplay.m_bChecked;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OnClickFavorite(bool bValue)
	{
		if (bValue)
		{
			if (NKMEpisodeMgr.GetFavoriteStageList().ContainsValue(m_StageTemplet))
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_FAVORITES_STAGE_ID_DUPLICATE), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				m_tglFavorite.Select(bSelect: false, bForce: true);
			}
			else if (NKMEpisodeMgr.GetFavoriteStageList().Count >= NKMCommonConst.MaxStageFavoriteCount)
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_FAVORITES_STAGE_COUNT_MAX), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				m_tglFavorite.Select(bSelect: false, bForce: true);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_FAVORITES_STAGE_ADD_REQ(m_StageTemplet.Key);
			}
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_FAVORITES_STAGE_DELETE_REQ(m_StageTemplet.Key);
		}
	}

	private static bool HasCutScen(NKMPhaseTemplet templet)
	{
		if (templet.m_CutScenStrIDAfter.Length > 0 || templet.m_CutScenStrIDBefore.Length > 0)
		{
			return true;
		}
		foreach (NKMPhaseOrderTemplet item in templet.PhaseList.List)
		{
			if (item.Dungeon != null && item.Dungeon.HasCutscen())
			{
				return true;
			}
		}
		return false;
	}

	private void OnTglSkip(bool bSet)
	{
		if (bSet)
		{
			if (!m_StageTemplet.m_bActiveBattleSkip)
			{
				m_tglSkip.Select(bSelect: false);
				return;
			}
			if (!CheckCanSkip())
			{
				m_tglSkip.Select(bSelect: false);
				return;
			}
			m_bOperationSkip = true;
			UpdateStageRequiredItem();
			SetSkipCountUIData();
		}
		NKCUtil.SetGameobjectActive(m_NKCUIOperationSkip, bSet);
		if (!bSet)
		{
			m_SkipCount = 1;
			m_bOperationSkip = false;
			UpdateStageRequiredItem();
			SetSkipCountUIData();
		}
	}

	private bool CheckCanSkip()
	{
		if (!NKCScenManager.CurrentUserData().CheckStageCleared(m_StageTemplet))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_CLEAR_STAGE"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return false;
		}
		if (!NKMEpisodeMgr.GetMedalAllClear(NKCScenManager.CurrentUserData(), m_StageTemplet))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_INVALID_SKIP_CONDITION), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return false;
		}
		if (m_StageTemplet.EnterLimit > 0)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			int statePlayCnt = nKMUserData.GetStatePlayCnt(m_StageTemplet.Key);
			if (m_StageTemplet.EnterLimit - statePlayCnt <= 0)
			{
				int num = 0;
				if (nKMUserData != null)
				{
					num = nKMUserData.GetStageRestoreCnt(m_StageTemplet.Key);
				}
				if (!m_StageTemplet.Restorable)
				{
					NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_ENTER_LIMIT_OVER, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				}
				else if (num >= m_StageTemplet.RestoreLimit)
				{
					NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_WARFARE_GAEM_HUD_RESTORE_LIMIT_OVER_DESC, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
				}
				else
				{
					NKCPopupResourceWithdraw.Instance.OpenForRestoreEnterLimit(m_StageTemplet, delegate
					{
						NKCPacketSender.Send_NKMPacket_RESET_STAGE_PLAY_COUNT_REQ(m_StageTemplet.Key);
					}, num);
				}
				return false;
			}
		}
		if (!NKMEpisodeMgr.HasEnoughResource(m_StageTemplet))
		{
			return false;
		}
		return true;
	}

	private void SetSkipCountUIData()
	{
		if (m_NKCUIOperationSkip != null)
		{
			m_NKCUIOperationSkip.SetData(m_StageTemplet, m_SkipCount, bShowUseDeckTypeNotice: true);
		}
	}

	private void OnOperationSkipUpdated(int newCount)
	{
		m_SkipCount = newCount;
		UpdateStageRequiredItem();
	}

	private void OnClickOperationSkipClose()
	{
		m_tglSkip.Select(bSelect: false);
	}
}
