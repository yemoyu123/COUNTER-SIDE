using System;
using System.Collections.Generic;
using ClientPacket.LeaderBoard;
using NKC.UI.Component;
using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIFierceBattleSupport : NKCUIBase
{
	private enum DIFFICULT_LEVEL
	{
		EASY,
		NORMAL,
		HARD,
		EXPERT
	}

	private enum UI_STATUS
	{
		US_NONE,
		US_INTRO,
		US_READY,
		US_BACK,
		US_RESULT,
		US_INTRO_IDLE,
		US_READY_IDLE,
		US_RESULT_IDLE
	}

	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_fierce_battle";

	public const string UI_ASSET_NAME = "NKM_UI_FIERCE_BATTLE";

	[Header("boss")]
	public NKCUICharacterView m_FIERCE_BATTLE_BOSS_IllustView;

	public Text m_BossLv;

	public Text m_BossName;

	public RectTransform m_FIERCE_BATTLE_BOSS_SLOT_LIST;

	public Image m_Background;

	[Header("ani")]
	public Animator m_AB_UI_NKM_UI_FIERCE_BATTLE_CONTENT;

	[Space]
	public GameObject m_FIERCE_BATTLE_BOSS_INFO_TYPE_01;

	public GameObject m_FIERCE_BATTLE_BOSS_INFO_TYPE_02;

	public GameObject m_FIERCE_BATTLE_BOSS_INFO_TYPE_03;

	public GameObject m_BOTTOM_Type_01;

	public GameObject m_BOTTOM_Type_02;

	[Header("info")]
	public Text m_FIERCE_BATTLE_TOTAL_SCORE_TEXT_1;

	public Text m_FIERCE_BATTLE_TOTAL_SCORE_TEXT_2;

	public Text m_FIERCE_BATTLE_TIME_TEXT;

	[Header("wait ui")]
	public Text m_Disc;

	[Header("ready ui")]
	public Text m_FIERCE_BATTLE_NOW_BOSS_SCORE_TEXT;

	public Text m_FIERCE_BATTLE_NOW_BOSS_SCORE_TEXT_2;

	public NKCUIComStateButton m_FIERCE_BATTLE_NOW_BOSS_SCORE_BUTTON_RESET;

	[Header("Battle Environment")]
	public NKCUIComBattleEnvironmentList m_comBattleEnvironment;

	[Header("result ui")]
	public Text m_FIERCE_BATTLE_NOW_BOSS_SCORE_TEXT_3;

	public Text m_FIERCE_BATTLE_NOW_BOSS_SCORE_TEXT_4;

	public NKCUIComStateButton m_FIERCE_BATTLE_NOW_BOSS_SCORE_BUTTON_RESET_2;

	public LoopScrollRect m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect;

	[Header("button ui")]
	public NKCUIComStateButton m_FIERCE_BATTLE_BUTTON_SCORE_REWARD;

	public GameObject m_FIERCE_BATTLE_BUTTON_SCORE_REWARD_REDDOT;

	public NKCUIComStateButton m_FIERCE_BATTLE_BUTTON_RANK_INFO;

	public NKCUIComStateButton m_FIERCE_BATTLE_BUTTON_RANK_SHORTCUT;

	public GameObject m_FIERCE_BATTLE_BUTTON_RANK_SHORTCUT_REDDOT;

	public NKCUIComStateButton m_FIERCE_BATTLE_BUTTON_BOSS_PERSONAL_RANK;

	public NKCUIComStateButton m_csbtnShop;

	[Space]
	public GameObject m_FIERCE_BATTLE_TOP3_LIST_ScrollRect;

	public GameObject m_FIERCE_BATTLE_TOP3_LIST_NODATA;

	public List<NKCUIFierceBattleBossPersonalRankSlot> m_lstRankSlot;

	[Header("출격 버튼")]
	public GameObject m_BUTTON_Root;

	public GameObject m_FIERCE_BATTLE_BUTTON_EnterLimit;

	public Text m_EnterLimit_TEXT;

	public NKCUIComStateButton m_FIERCE_BATTLE_BUTTON_READY;

	public NKCUIComStateButton m_FIERCE_BATTLE_BUTTON_START;

	public GameObject m_FIERCE_BATTLE_BUTTON_READY_Disable;

	public GameObject m_FIERCE_BATTLE_BUTTON_READY_Normal;

	public GameObject m_objDifficultToggleGroup;

	public List<NKCUIComToggle> m_lstDifficultToggle = new List<NKCUIComToggle>();

	public NKCUIComToggle m_tlgSelectDifficult;

	public GameObject m_objDifficultSelect;

	public NKCComTMPUIText[] m_arrlbSelectedDifficults;

	[Header("작전 상태")]
	public GameObject m_FIERCE_BATTLE_NOTICE;

	public GameObject m_FIERCE_BATTLE_NOTICE_1;

	public GameObject m_FIERCE_BATTLE_NOTICE_2;

	[Header("BestLineUp")]
	public NKCUIComToggle m_MY_BEST_DECK_Toggle;

	public GameObject m_DECK_List;

	public GameObject m_FIERCE_BATTLE_MY_BEST_DECK_NODATA_TEXT;

	public Image m_ANIM_SHIP_IMG;

	public List<NKCDeckViewUnitSlot> m_lstNKCDeckViewUnitSlot;

	[Header("nightmare")]
	public GameObject m_FIERCE_BATTLE_NIGHTMARE;

	public GameObject m_NightmareMode;

	public GameObject m_NightmareModeBGOFF;

	public GameObject m_NightmareModeBGON;

	public Animator m_aniNightMare;

	public NKCComText m_lbNightmareMode;

	[Header("Self Penalty UI")]
	public NKCUIComStateButton m_csbtnSelfPenalty;

	public GameObject m_objSelfPenaltyBtnNornal;

	public GameObject m_objSelfPenaltyBtnDisable;

	public GameObject m_objSelfPenaltyDesc;

	public Text m_lbSelfPenaltyDesc;

	private RectTransform m_rtFierceBossIllust;

	private NKCFierceBattleSupportDataMgr m_FierceDataMgr;

	private NKMFierceTemplet m_curFierceTemplet;

	private int m_icurFierceBossGroupID;

	private const string ANI_INTRO = "INTRO";

	private const string ANI_READY = "READY";

	private const string ANI_RESULT = "RESULT";

	private const string ANI_BACK = "BACK";

	private const string ANI_INTRO_IDLE = "INTRO_IDLE";

	private const string ANI_READY_IDLE = "READY_IDLE";

	private const string ANI_RESULT_IDLE = "RESULT_IDLE";

	private UI_STATUS m_curUIStatus;

	private bool m_bNightMareMode;

	private bool m_bFierceActivateTimeCnt;

	private Stack<NKCUIFierceBattleBossPersonalRankSlot> m_stk = new Stack<NKCUIFierceBattleBossPersonalRankSlot>();

	private List<NKCUIFierceBattleBossPersonalRankSlot> m_lstVisible = new List<NKCUIFierceBattleBossPersonalRankSlot>();

	public override string MenuName => NKCUtilString.GET_STRING_FIERCE;

	public override string GuideTempletID => "ARTICLE_FIERCE_INFO";

	public override List<int> UpsideMenuShowResourceList => new List<int> { 25, 1, 2, 101 };

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		Clear();
	}

	public override void OnBackButton()
	{
		if (m_curUIStatus == UI_STATUS.US_READY)
		{
			UpdateUIAni(UI_STATUS.US_BACK);
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		if (NKCTutorialManager.TutorialRequired(TutorialStep.FierceLobby))
		{
			ResetUI();
			UpdateUIAni(UI_STATUS.US_INTRO_IDLE, bForce: true);
			return;
		}
		switch (m_curUIStatus)
		{
		case UI_STATUS.US_INTRO:
			UpdateUIAni(UI_STATUS.US_INTRO_IDLE, bForce: true);
			break;
		case UI_STATUS.US_READY:
			UpdateUIAni(UI_STATUS.US_READY_IDLE, bForce: true);
			break;
		case UI_STATUS.US_RESULT:
			UpdateUIAni(UI_STATUS.US_RESULT_IDLE, bForce: true);
			break;
		case UI_STATUS.US_BACK:
			break;
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		base.OnInventoryChange(itemData);
	}

	public void Init()
	{
		NKCUtil.SetToggleValueChangedDelegate(m_MY_BEST_DECK_Toggle, OnClickBestLineUp);
		NKCUtil.SetBindFunction(m_FIERCE_BATTLE_BUTTON_READY, OnClickReady);
		NKCUtil.SetBindFunction(m_FIERCE_BATTLE_BUTTON_START, OnClickPrepare);
		NKCUtil.SetBindFunction(m_FIERCE_BATTLE_BUTTON_SCORE_REWARD, OnClickPointReward);
		NKCUtil.SetBindFunction(m_FIERCE_BATTLE_BUTTON_RANK_INFO, OnClickRankReward);
		NKCUtil.SetBindFunction(m_FIERCE_BATTLE_BUTTON_RANK_SHORTCUT, OnClickPopUpLeaderBoard);
		NKCUtil.SetBindFunction(m_FIERCE_BATTLE_BUTTON_BOSS_PERSONAL_RANK, OnClickOpenRankingPopup);
		NKCUtil.SetButtonClickDelegate(m_csbtnShop, OnShop);
		NKCUtil.SetBindFunction(m_csbtnSelfPenalty, OnClickSelfPenalty);
		m_curUIStatus = UI_STATUS.US_NONE;
		m_FierceDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect != null)
		{
			m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect.dOnGetObject += GetObject;
			m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect.dOnReturnObject += ReturnObject;
			m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect.dOnProvideData += ProvideData;
			m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect.PrepareCells();
		}
		for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
		{
			m_lstNKCDeckViewUnitSlot[i].Init(i, bEnableDrag: false);
		}
		foreach (NKCUIFierceBattleBossPersonalRankSlot item in m_lstRankSlot)
		{
			if (item != null)
			{
				item.Init();
			}
		}
		if (m_FIERCE_BATTLE_BOSS_IllustView != null)
		{
			m_rtFierceBossIllust = m_FIERCE_BATTLE_BOSS_IllustView.GetComponent<RectTransform>();
		}
		NKCUtil.SetToggleValueChangedDelegate(m_lstDifficultToggle[0], delegate
		{
			OnClickDifficultTab(DIFFICULT_LEVEL.EASY);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_lstDifficultToggle[1], delegate
		{
			OnClickDifficultTab(DIFFICULT_LEVEL.NORMAL);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_lstDifficultToggle[2], delegate
		{
			OnClickDifficultTab(DIFFICULT_LEVEL.HARD);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_lstDifficultToggle[3], delegate
		{
			OnClickDifficultTab(DIFFICULT_LEVEL.EXPERT);
		});
		NKCUtil.SetToggleValueChangedDelegate(m_tlgSelectDifficult, OnClickSelectDifficult);
	}

	private void Clear()
	{
		for (int i = 0; i < m_lstVisible.Count; i++)
		{
			if (null != m_lstVisible[i])
			{
				m_stk.Push(m_lstVisible[i]);
			}
		}
		while (m_stk.Count > 0)
		{
			m_stk.Pop()?.DestoryInstance();
		}
	}

	public void Open()
	{
		try
		{
			if (!m_FierceDataMgr.IsCanAccessFierce())
			{
				return;
			}
			m_curFierceTemplet = m_FierceDataMgr.FierceTemplet;
			m_icurFierceBossGroupID = m_curFierceTemplet.FierceBossGroupIdList[0];
			if (!NKMFierceBossGroupTemplet.Groups.ContainsKey(m_icurFierceBossGroupID))
			{
				Debug.LogError($"NKCUIFierceBattleSupport::Open() - plz check fierce boss group data - boss group id : {m_icurFierceBossGroupID}");
				return;
			}
			if (m_FierceDataMgr.GetStatus() == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE)
			{
				m_bFierceActivateTimeCnt = true;
			}
			else if (m_FierceDataMgr.IsPossibleRankReward())
			{
				NKCPacketSender.Send_NKMPacket_FIERCE_COMPLETE_RANK_REWARD_REQ();
			}
			m_tlgSelectDifficult.Select(bSelect: false, bForce: true, bImmediate: true);
			UpdateFierceRanking();
			ResetUI();
			UIOpened();
		}
		catch (Exception ex)
		{
			Debug.LogError("[Error]NKCUIFierceBattleSupport::Open() Failed with exception : " + ex.Message);
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_UI_LOADING_ERROR, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
	}

	public void ResetUI()
	{
		UpdateMainUI();
		UpdateSideUI();
		if (m_FierceDataMgr.GetStatus() == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE)
		{
			UpdateUIAni(UI_STATUS.US_INTRO);
		}
		else
		{
			UpdateUIAni(UI_STATUS.US_RESULT);
		}
		if (m_MY_BEST_DECK_Toggle != null)
		{
			m_MY_BEST_DECK_Toggle.Select(bSelect: false);
		}
		NKCUtil.SetGameobjectActive(m_DECK_List, bValue: false);
	}

	private void UpdateUIAni(UI_STATUS newAni, bool bForce = false)
	{
		if (m_curUIStatus == newAni && !bForce)
		{
			return;
		}
		switch (newAni)
		{
		case UI_STATUS.US_INTRO:
			m_AB_UI_NKM_UI_FIERCE_BATTLE_CONTENT.SetTrigger("INTRO");
			break;
		case UI_STATUS.US_INTRO_IDLE:
			m_AB_UI_NKM_UI_FIERCE_BATTLE_CONTENT.SetTrigger("INTRO_IDLE");
			newAni = UI_STATUS.US_INTRO;
			break;
		case UI_STATUS.US_READY:
			m_AB_UI_NKM_UI_FIERCE_BATTLE_CONTENT.SetTrigger("READY");
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_NIGHTMARE, m_bNightMareMode);
			if (m_bNightMareMode)
			{
				m_aniNightMare.SetTrigger("INTRO");
			}
			break;
		case UI_STATUS.US_READY_IDLE:
			m_AB_UI_NKM_UI_FIERCE_BATTLE_CONTENT.SetTrigger("READY_IDLE");
			newAni = UI_STATUS.US_READY;
			break;
		case UI_STATUS.US_BACK:
			m_AB_UI_NKM_UI_FIERCE_BATTLE_CONTENT.SetTrigger("BACK");
			newAni = UI_STATUS.US_INTRO;
			if (m_bNightMareMode)
			{
				m_aniNightMare.SetTrigger("BACK");
			}
			break;
		case UI_STATUS.US_RESULT:
			m_AB_UI_NKM_UI_FIERCE_BATTLE_CONTENT.SetTrigger("RESULT");
			break;
		case UI_STATUS.US_RESULT_IDLE:
			m_AB_UI_NKM_UI_FIERCE_BATTLE_CONTENT.SetTrigger("RESULT_IDLE");
			newAni = UI_STATUS.US_RESULT;
			break;
		}
		m_curUIStatus = newAni;
		NKCUtil.SetGameobjectActive(m_csbtnSelfPenalty.gameObject, m_curUIStatus == UI_STATUS.US_READY);
		UpdatePenaltyUI();
		Debug.Log($"<color=green>update ani : {m_curUIStatus}</color>");
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BOSS_INFO_TYPE_01, m_curUIStatus == UI_STATUS.US_INTRO);
		NKCUtil.SetGameobjectActive(m_BOTTOM_Type_01, m_curUIStatus == UI_STATUS.US_INTRO || m_curUIStatus == UI_STATUS.US_RESULT);
		NKCUtil.SetGameobjectActive(m_BOTTOM_Type_02, m_curUIStatus == UI_STATUS.US_READY);
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BOSS_INFO_TYPE_02, m_curUIStatus == UI_STATUS.US_READY);
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BOSS_INFO_TYPE_03, m_curUIStatus == UI_STATUS.US_RESULT);
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BUTTON_READY_Disable, m_curUIStatus == UI_STATUS.US_RESULT);
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BUTTON_READY_Normal, m_curUIStatus == UI_STATUS.US_INTRO);
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BUTTON_READY, m_curUIStatus == UI_STATUS.US_INTRO || m_curUIStatus == UI_STATUS.US_RESULT);
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BUTTON_START, m_curUIStatus == UI_STATUS.US_READY);
		NKCUtil.SetGameobjectActive(m_objDifficultSelect, m_curUIStatus != UI_STATUS.US_RESULT);
	}

	private void UpdateMainUI()
	{
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BUTTON_RANK_SHORTCUT_REDDOT, bValue: false);
		if (m_curFierceTemplet != null)
		{
			NKCUtil.SetLabelText(m_FIERCE_BATTLE_TOTAL_SCORE_TEXT_1, m_FierceDataMgr.GetTotalPoint().ToString());
			NKCUtil.SetLabelText(m_FIERCE_BATTLE_TOTAL_SCORE_TEXT_2, m_FierceDataMgr.GetRankingTotalDesc());
			m_FierceDataMgr.GetRecommandOperationPower();
		}
	}

	private void UpdateSideUI(int targetBossID = 0)
	{
		if (m_curFierceTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_NIGHTMARE, bValue: false);
		int num = 0;
		if (targetBossID != 0 && NKMFierceBossGroupTemplet.Groups.ContainsKey(m_icurFierceBossGroupID))
		{
			foreach (NKMFierceBossGroupTemplet item in NKMFierceBossGroupTemplet.Groups[m_icurFierceBossGroupID])
			{
				if (item.FierceBossID == targetBossID)
				{
					num = item.Level;
				}
			}
		}
		if (num == 0)
		{
			num = m_FierceDataMgr.GetClearLevel(m_icurFierceBossGroupID);
		}
		switch (num)
		{
		case 0:
		case 1:
			OnClickDifficultTab(DIFFICULT_LEVEL.EASY);
			break;
		case 2:
			OnClickDifficultTab(DIFFICULT_LEVEL.NORMAL);
			break;
		case 3:
			OnClickDifficultTab(DIFFICULT_LEVEL.HARD);
			break;
		case 4:
			OnClickDifficultTab(DIFFICULT_LEVEL.EXPERT);
			break;
		}
		UpdateBossUI();
		NKCUtil.SetLabelText(m_FIERCE_BATTLE_NOW_BOSS_SCORE_TEXT, m_FierceDataMgr.GetMaxPoint().ToString("#,##0"));
		NKCUtil.SetLabelText(m_FIERCE_BATTLE_NOW_BOSS_SCORE_TEXT_3, m_FierceDataMgr.GetMaxPoint().ToString());
		NKCUtil.SetLabelText(m_FIERCE_BATTLE_NOW_BOSS_SCORE_TEXT_2, m_FierceDataMgr.GetRankingDesc());
		NKCUtil.SetLabelText(m_FIERCE_BATTLE_NOW_BOSS_SCORE_TEXT_4, m_FierceDataMgr.GetRankingDesc());
		UpdateBattleCond();
		UpdateFierceBattleRank();
		if (m_FierceDataMgr.GetStatus() == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE)
		{
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BUTTON_EnterLimit, m_curFierceTemplet.DailyEnterLimit > 0);
		}
		NKCUtil.SetGameobjectActive(m_BUTTON_Root, m_FierceDataMgr.GetStatus() == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE);
		UpdatePointRewardRedDot();
		UpdatePenaltyUI();
		NKMEventDeckData bestLineUp = m_FierceDataMgr.GetBestLineUp();
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_MY_BEST_DECK_NODATA_TEXT, bestLineUp == null || bestLineUp.m_dicUnit.Count <= 0);
	}

	private void UpdateBattleCond()
	{
		if (!(m_comBattleEnvironment == null))
		{
			List<NKMBattleConditionTemplet> curBattleCondition = m_FierceDataMgr.GetCurBattleCondition();
			List<int> curPreConditionGroup = m_FierceDataMgr.GetCurPreConditionGroup();
			m_comBattleEnvironment.InitData(null, curBattleCondition, curPreConditionGroup);
			m_comBattleEnvironment.UpdateData(null, null, null, 0L);
			m_comBattleEnvironment.Open();
		}
	}

	private void UpdatePenaltyUI()
	{
		UpdateNightMareMode();
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_NIGHTMARE, m_bNightMareMode);
		NKCUtil.SetImageSprite(m_Background, NKCUtil.GetSpriteFierceBattleBackgroud(m_bNightMareMode));
		NKCUtil.SetGameobjectActive(m_NightmareMode, m_bNightMareMode && m_curUIStatus == UI_STATUS.US_READY);
		NKCUtil.SetGameobjectActive(m_NightmareModeBGOFF, !m_bNightMareMode || m_curUIStatus != UI_STATUS.US_READY);
		NKCUtil.SetGameobjectActive(m_NightmareModeBGON, m_bNightMareMode && m_curUIStatus == UI_STATUS.US_READY);
		NKCUtil.SetGameobjectActive(m_objSelfPenaltyBtnNornal, m_bNightMareMode);
		NKCUtil.SetGameobjectActive(m_objSelfPenaltyBtnDisable, !m_bNightMareMode);
		string text = m_FierceDataMgr.GetStringCurBossSelfPenalty();
		if (!m_bNightMareMode || string.IsNullOrEmpty(text))
		{
			text = "-";
		}
		NKCUtil.SetLabelText(m_lbSelfPenaltyDesc, text);
	}

	public void UpdateFierceBattleRank()
	{
		bool flag = m_FierceDataMgr.IsHasFierceRankingData();
		switch (m_FierceDataMgr.GetStatus())
		{
		case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE:
			if (flag)
			{
				for (int i = 0; i < m_lstRankSlot.Count; i++)
				{
					NKMFierceData fierceRankingData = m_FierceDataMgr.GetFierceRankingData(i);
					if (fierceRankingData == null)
					{
						NKCUtil.SetGameobjectActive(m_lstRankSlot[i].gameObject, bValue: false);
						continue;
					}
					NKCUtil.SetGameobjectActive(m_lstRankSlot[i].gameObject, bValue: true);
					m_lstRankSlot[i].SetData(fierceRankingData, i + 1);
				}
			}
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_TOP3_LIST_ScrollRect, flag);
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_TOP3_LIST_NODATA, !flag);
			break;
		case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_REWARD:
		case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_COMPLETE:
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_NOTICE_2, !m_FierceDataMgr.IsPossibleRankReward());
			if (flag)
			{
				m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect.TotalCount = Mathf.Min(m_FierceDataMgr.GetBossGroupRankingDataCnt(), 50);
			}
			else
			{
				m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect.TotalCount = 0;
			}
			m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect.SetIndexPosition(0);
			break;
		}
	}

	private void OnClickBestLineUp(bool bSet)
	{
		bool flag = false;
		if (bSet)
		{
			NKMEventDeckData bestLineUp = m_FierceDataMgr.GetBestLineUp();
			if (bestLineUp != null && bestLineUp.m_dicUnit.Count > 0)
			{
				flag = true;
				NKMUnitData shipFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetShipFromUID(bestLineUp.m_ShipUID);
				if (shipFromUID != null)
				{
					NKMUnitTempletBase unitTempletBase = shipFromUID.GetUnitTempletBase();
					if (unitTempletBase != null)
					{
						NKCUtil.SetImageSprite(m_ANIM_SHIP_IMG, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase));
					}
				}
				for (int i = 0; i < 8; i++)
				{
					if (!(m_lstNKCDeckViewUnitSlot[i] == null) && bestLineUp.m_dicUnit.Count > i)
					{
						NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(bestLineUp.m_dicUnit[i]);
						m_lstNKCDeckViewUnitSlot[i].SetData(unitFromUID, bEnableButton: false);
						if (unitFromUID == null)
						{
							m_lstNKCDeckViewUnitSlot[i].SetIconEtcDefault();
						}
					}
				}
			}
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_MY_BEST_DECK_NODATA_TEXT, !flag);
			NKCUtil.SetGameobjectActive(m_DECK_List, flag);
		}
		else
		{
			NKMEventDeckData bestLineUp2 = m_FierceDataMgr.GetBestLineUp();
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_MY_BEST_DECK_NODATA_TEXT, bestLineUp2 == null || bestLineUp2.m_dicUnit.Count <= 0);
			NKCUtil.SetGameobjectActive(m_DECK_List, bValue: false);
		}
	}

	private void UpdateBossUI()
	{
		UpdateBossInfo();
		NKCUtil.SetLabelText(m_Disc, m_FierceDataMgr.GetCurBossDesc());
		NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(m_FierceDataMgr.GetTargetDungeonID());
		if (dungeonTemplet != null)
		{
			m_FIERCE_BATTLE_BOSS_IllustView.SetCharacterIllust(dungeonTemplet.m_BossUnitStrID, 0, bEnableBackground: true, bVFX: true, bAsync: false);
		}
		NKMFierceBossGroupTemplet bossGroupTemplet = m_FierceDataMgr.GetBossGroupTemplet();
		if (bossGroupTemplet != null && m_rtFierceBossIllust != null)
		{
			m_rtFierceBossIllust.localScale = new Vector2(bossGroupTemplet.UI_BossPrefabScale, bossGroupTemplet.UI_BossPrefabScale);
			if (bossGroupTemplet.UI_BossPrefabFlip)
			{
				m_rtFierceBossIllust.localRotation = Quaternion.Euler(0f, -180f, 0f);
			}
			else
			{
				m_rtFierceBossIllust.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			if (m_FIERCE_BATTLE_BOSS_IllustView.m_rectIllustRoot != null)
			{
				m_FIERCE_BATTLE_BOSS_IllustView.m_rectIllustRoot.localPosition = new Vector2(bossGroupTemplet.UI_BossPrefabPos.x, bossGroupTemplet.UI_BossPrefabPos.y);
			}
		}
	}

	private void UpdateBossInfo()
	{
		string curSelectedBossLvDesc = m_FierceDataMgr.GetCurSelectedBossLvDesc(m_icurFierceBossGroupID);
		NKCUtil.SetLabelText(m_BossLv, curSelectedBossLvDesc);
		NKCUtil.SetLabelText(m_BossName, m_FierceDataMgr.GetCurBossName());
	}

	private void Update()
	{
		if (!m_bFierceActivateTimeCnt || m_curFierceTemplet == null)
		{
			return;
		}
		if (!NKCSynchronizedTime.IsFinished(NKMTime.LocalToUTC(m_curFierceTemplet.FierceGameEnd)))
		{
			NKCUtil.SetLabelText(m_FIERCE_BATTLE_TIME_TEXT, m_FierceDataMgr.GetLeftTimeString());
			if (m_curFierceTemplet.DailyEnterLimit > 0)
			{
				string msg = string.Format(NKCUtilString.GET_FIERCE_ENTER_LIMIT, m_curFierceTemplet.DailyEnterLimit - NKCScenManager.CurrentUserData().GetStatePlayCnt(NKMFierceConst.StageId, IsServiceTime: true), m_curFierceTemplet.DailyEnterLimit);
				NKCUtil.SetLabelText(m_EnterLimit_TEXT, msg);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_FIERCE_BATTLE_TIME_TEXT, NKCUtilString.GET_FIERCE_ACTIVATE_SEASON_END);
			m_bFierceActivateTimeCnt = false;
		}
	}

	private void OnClickPointReward()
	{
		NKCUIPopupFierceBattleScoreReward.Instance.Open();
	}

	private void OnClickRankReward()
	{
		NKCUIPopupFierceBattleRewardInfo.Instance.Open(LeaderBoardType.BT_FIERCE);
	}

	private void OnClickPopUpLeaderBoard()
	{
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_FIERCE, 0);
		if (nKMLeaderBoardTemplet != null)
		{
			NKCPopupLeaderBoardSingle.Instance.OpenSingle(nKMLeaderBoardTemplet);
		}
	}

	private void OnClickOpenRankingPopup()
	{
		if (m_FierceDataMgr.GetStatus() == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE)
		{
			NKCPacketSender.Send_NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ(m_FierceDataMgr.CurBossGroupID, bIsAll: true);
		}
	}

	private void OnShop()
	{
		NKCUIShop.ShopShortcut("TAB_SEASON_FIERCE_POINT");
	}

	private void OnClickSelfPenalty()
	{
		if (m_FierceDataMgr.GetCurSelectedBossLv() < 3)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_FIERCE_POPUP_SELF_PENALTY_BLOCK_TEXT);
		}
		else
		{
			NKCPopupFierceBattleSelfPenalty.Instance.Open(m_curFierceTemplet.FierceID, UpdatePenaltyUI);
		}
	}

	public void RefreshLeaderBoard()
	{
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BUTTON_RANK_SHORTCUT_REDDOT, bValue: false);
		if (NKCPopupLeaderBoardSingle.IsInstanceOpen)
		{
			NKCPopupLeaderBoardSingle.Instance.RefreshUI();
		}
	}

	private void OnClickReady()
	{
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr != null && nKCFierceBattleSupportDataMgr.GetStatus() == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE)
		{
			UpdateUIAni(UI_STATUS.US_READY);
		}
	}

	private void OnClickPrepare()
	{
		if (m_FierceDataMgr.IsCanStart())
		{
			m_FierceDataMgr.SendPenaltyReq();
			int targetDungeonID = m_FierceDataMgr.GetTargetDungeonID();
			if (targetDungeonID == 0)
			{
				Debug.Log("던전 정보를 확인 할 수 없습니다. NKCUIFierceBattleSupport::OnClickPrepare()");
				return;
			}
			NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(NKMDungeonManager.GetDungeonTempletBase(targetDungeonID), DeckContents.FIERCE_BATTLE_SUPPORT);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
		}
	}

	private void UpdateFierceRanking()
	{
		foreach (int fierceBossGroupId in m_curFierceTemplet.FierceBossGroupIdList)
		{
			NKCPacketSender.Send_NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_REQ(fierceBossGroupId);
		}
	}

	public void UpdatePointRewardRedDot()
	{
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BUTTON_SCORE_REWARD_REDDOT, m_FierceDataMgr.IsCanReceivePointReward());
	}

	private RectTransform GetObject(int index)
	{
		NKCUIFierceBattleBossPersonalRankSlot nKCUIFierceBattleBossPersonalRankSlot = null;
		nKCUIFierceBattleBossPersonalRankSlot = ((m_stk.Count <= 0) ? NKCUIFierceBattleBossPersonalRankSlot.GetNewInstance(m_FIERCE_BATTLE_BOSS_RANK_FINAL_ScrollRect.content.transform) : m_stk.Pop());
		m_lstVisible.Add(nKCUIFierceBattleBossPersonalRankSlot);
		return nKCUIFierceBattleBossPersonalRankSlot?.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIFierceBattleBossPersonalRankSlot component = tr.GetComponent<NKCUIFierceBattleBossPersonalRankSlot>();
		m_lstVisible.Remove(component);
		m_stk.Push(component);
		NKCUtil.SetGameobjectActive(component, bValue: false);
		tr.SetParent(base.transform);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIFierceBattleBossPersonalRankSlot component = tr.GetComponent<NKCUIFierceBattleBossPersonalRankSlot>();
		int rank = idx + 1;
		NKMFierceData fierceRankingData = m_FierceDataMgr.GetFierceRankingData(idx);
		if (fierceRankingData == null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
		else
		{
			component.SetData(fierceRankingData, rank);
		}
	}

	private void OnClickDifficultTab(DIFFICULT_LEVEL DifficultLv)
	{
		if (m_lstDifficultToggle.Count - 1 < (int)DifficultLv)
		{
			return;
		}
		int num = 0;
		foreach (NKMFierceBossGroupTemplet item in NKMFierceBossGroupTemplet.Groups[m_icurFierceBossGroupID])
		{
			if (num == (int)DifficultLv)
			{
				m_FierceDataMgr.SetCurBossID(item.FierceBossID);
				Debug.Log($"<color=red>OnClickDifficultTab : difLV : {(int)DifficultLv}, groupID : {item.FierceBossGroupID}, bossID : {item.FierceBossID}</color>");
				break;
			}
			num++;
		}
		m_lstDifficultToggle[(int)DifficultLv].Select(bSelect: true, bForce: true);
		string msg = "";
		switch (DifficultLv)
		{
		case DIFFICULT_LEVEL.EASY:
			msg = NKCStringTable.GetString("SI_DP_FIERCE_LEVEL_EASY");
			break;
		case DIFFICULT_LEVEL.NORMAL:
			msg = NKCStringTable.GetString("SI_DP_FIERCE_LEVEL_NORMAL");
			break;
		case DIFFICULT_LEVEL.HARD:
			msg = NKCStringTable.GetString("SI_DP_FIERCE_LEVEL_HARD");
			m_lbNightmareMode.text = NKCStringTable.GetString("SI_DP_FIERCE_HARD_MODE");
			break;
		case DIFFICULT_LEVEL.EXPERT:
			msg = NKCStringTable.GetString("SI_DP_FIERCE_LEVEL_EXPERT");
			m_lbNightmareMode.text = NKCStringTable.GetString("SI_DP_FIERCE_EXPERT_MODE");
			break;
		}
		NKCComTMPUIText[] arrlbSelectedDifficults = m_arrlbSelectedDifficults;
		for (int i = 0; i < arrlbSelectedDifficults.Length; i++)
		{
			NKCUtil.SetLabelText(arrlbSelectedDifficults[i], msg);
		}
		m_tlgSelectDifficult.Select(bSelect: false, bForce: true, bImmediate: true);
		OnClickSelectDifficult(bSelect: false);
		UpdateNightMareMode();
		UpdateBossInfo();
		UpdateBattleCond();
	}

	private void UpdateNightMareMode()
	{
		int curSelectedBossLv = m_FierceDataMgr.GetCurSelectedBossLv(m_icurFierceBossGroupID);
		m_bNightMareMode = curSelectedBossLv > 2 && m_FierceDataMgr.GetSelfPenalty().Count > 0;
	}

	private void OnClickSelectDifficult(bool bSelect)
	{
		NKCUtil.SetGameobjectActive(m_objDifficultToggleGroup, bSelect);
	}
}
