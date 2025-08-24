using System;
using System.Collections.Generic;
using Cs.Logging;
using NKC.Trim;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Trim;

public class NKCUIPopupTrimDungeon : NKCUIBase
{
	public const string UI_ASSET_BUNDLE_NAME = "ab_ui_trim";

	public const string UI_ASSET_NAME = "AB_UI_TRIM_DUNGEON";

	private static NKCUIPopupTrimDungeon m_Instance;

	public Text m_lbDungeonName;

	public Text m_lbDungeonDesc;

	public Text m_lbEnterLimit;

	public GameObject m_objRemainDate;

	public Text m_lbRemainDate;

	public Text m_lbTrimLevel;

	public Text m_lbTrimLevelScore;

	public Text m_lbRecommendedPower;

	public Image m_imgMap;

	public LoopScrollRect m_trimLevelScrollRect;

	public NKCUITrimSquadSlot[] m_squadSlot;

	public Transform m_battleCondParent;

	public NKCUITrimReward m_trimReward;

	public NKCUIComStateButton m_csbtnStart;

	public NKCUIComStateButton m_csbtnStartResource;

	public NKCUIComResourceButton m_comResourceButton;

	public float m_scrollTime;

	public GameObject m_objEnterLimitRoot;

	[Header("\ufffd\ufffdŵ")]
	public GameObject m_objSkip;

	public NKCUIOperationSkip m_operationSkip;

	public NKCUIComToggle m_tglSkip;

	private int m_trimId;

	private int m_clearedLevel;

	private int m_selectedGroup = 101;

	private int m_selectedLevel;

	private int m_maxTrimLevel;

	private int m_skipCount;

	private float m_dateUpdateTimerSec;

	private float m_dateUpdateTimerMin;

	private bool m_isSkip;

	private bool m_bShowIntervalTime;

	public static NKCUIPopupTrimDungeon Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupTrimDungeon>("ab_ui_trim", "AB_UI_TRIM_DUNGEON", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanUpInstance).GetInstance<NKCUIPopupTrimDungeon>();
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

	public override string MenuName => "Trim Dungeon";

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public int TrimId => m_trimId;

	public int SelectedGroup => m_selectedGroup;

	public int SelectedLevel => m_selectedLevel;

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
		if (m_squadSlot != null)
		{
			int num = m_squadSlot.Length;
			for (int i = 0; i < num; i++)
			{
				m_squadSlot[i]?.Init(i, OnDeckConfirm);
			}
		}
		if (m_trimLevelScrollRect != null)
		{
			m_trimLevelScrollRect.dOnGetObject += GetPresetSlot;
			m_trimLevelScrollRect.dOnReturnObject += ReturnPresetSlot;
			m_trimLevelScrollRect.dOnProvideData += ProvidePresetData;
			m_trimLevelScrollRect.ContentConstraintCount = 1;
			m_trimLevelScrollRect.TotalCount = 0;
			m_trimLevelScrollRect.PrepareCells();
		}
		m_trimReward?.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnStart, OnClickStart);
		NKCUtil.SetHotkey(m_csbtnStart, HotkeyEventType.Confirm);
		NKCUtil.SetButtonClickDelegate(m_csbtnStartResource, OnClickStart);
		NKCUtil.SetHotkey(m_csbtnStartResource, HotkeyEventType.Confirm);
		NKCUITrimUtility.InitBattleCondition(m_battleCondParent, showToolTip: true);
		m_comResourceButton?.Init();
		NKCUtil.SetToggleValueChangedDelegate(m_tglSkip, OnClickSkip);
		NKCUtil.SetHotkey(m_tglSkip, HotkeyEventType.RotateLeft, null, bUpDownEvent: true);
		m_operationSkip?.Init(OnOperationSkipUpdated, OnClickOperationSkipClose);
	}

	public override void CloseInternal()
	{
		NKCLocalDeckDataManager.Clear();
		m_tglSkip.Select(bSelect: false);
		base.gameObject.SetActive(value: false);
	}

	public void Open(int trimId)
	{
		m_trimId = trimId;
		m_dateUpdateTimerSec = 0f;
		m_dateUpdateTimerMin = 0f;
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(trimId);
		NKCLocalDeckDataManager.Clear();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMTrimTemplet != null)
		{
			int num = nKMTrimTemplet.TrimDungeonIds.Length;
			long userUId = nKMUserData?.m_UserUID ?? 0;
			for (int i = 0; i < num; i++)
			{
				NKCLocalDeckDataManager.LoadLocalDeckData(NKCUITrimUtility.GetTrimDeckKey(trimId, nKMTrimTemplet.TrimDungeonIds[i], userUId), i, 8);
			}
			NKCLocalDeckDataManager.SetDataLoadedState(value: true);
			NKCUtil.SetGameobjectActive(m_objSkip, nKMTrimTemplet.m_bActiveBattleSkip);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSkip, bValue: false);
		}
		RefreshUI(resetLevelTab: true);
		UIOpened();
	}

	public void RefreshUI(bool resetLevelTab)
	{
		m_maxTrimLevel = 0;
		m_selectedGroup = 0;
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(m_trimId);
		string text = null;
		string text2 = null;
		Sprite sp = null;
		int stageReqItemId = 0;
		int stageReqItemCount = 0;
		if (nKMTrimTemplet != null)
		{
			text = NKCStringTable.GetString(nKMTrimTemplet.TirmGroupName);
			text2 = NKCStringTable.GetString(nKMTrimTemplet.TirmGroupDesc);
			m_maxTrimLevel = nKMTrimTemplet.MaxTrimLevel;
			m_selectedGroup = nKMTrimTemplet.TrimPointGroup;
			sp = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_TRIM_MAP_IMG", nKMTrimTemplet.TrimGroupBGPrefab);
			stageReqItemId = nKMTrimTemplet.m_StageReqItemID;
			stageReqItemCount = nKMTrimTemplet.m_StageReqItemCount;
		}
		NKMUserData userData = NKCScenManager.CurrentUserData();
		m_clearedLevel = NKCUITrimUtility.GetClearedTrimLevel(userData, m_trimId);
		m_selectedLevel = Mathf.Min(m_maxTrimLevel, m_clearedLevel + 1);
		base.gameObject.SetActive(value: true);
		if (m_trimLevelScrollRect != null)
		{
			int num = Mathf.Min(m_maxTrimLevel, m_selectedLevel + 1);
			m_trimLevelScrollRect.TotalCount = num;
			if (resetLevelTab)
			{
				int num2 = Mathf.Max(0, num - 10);
				int num3 = Mathf.Max(m_selectedLevel - 1, 0);
				float time = Mathf.Max(0.2f, (float)(num3 - num2) / 20f * m_scrollTime);
				m_trimLevelScrollRect.SetIndexPosition(num2);
				m_trimLevelScrollRect.ScrollToCell(num3, time, LoopScrollRect.ScrollTarget.Top, delegate
				{
					m_trimLevelScrollRect.RefreshCells();
				});
			}
			else
			{
				m_trimLevelScrollRect.RefreshCells();
			}
		}
		NKCUtil.SetLabelText(m_lbDungeonName, string.IsNullOrEmpty(text) ? " - " : text);
		NKCUtil.SetLabelText(m_lbDungeonDesc, string.IsNullOrEmpty(text2) ? " - " : text2);
		NKCUtil.SetLabelText(m_lbTrimLevel, m_selectedLevel.ToString());
		NKCUtil.SetImageSprite(m_imgMap, sp, bDisableIfSpriteNull: true);
		NKMTrimIntervalTemplet trimInterval = NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime);
		NKCUtil.SetGameobjectActive(m_objEnterLimitRoot, NKCUITrimUtility.IsEnterCountLimited(trimInterval));
		if (NKCUITrimUtility.IsEnterCountLimited(trimInterval))
		{
			string enterLimitMsg = NKCUITrimUtility.GetEnterLimitMsg(trimInterval);
			NKCUtil.SetLabelText(m_lbEnterLimit, enterLimitMsg);
		}
		m_bShowIntervalTime = nKMTrimTemplet.ShowInterval;
		NKCUtil.SetGameobjectActive(m_objRemainDate, m_bShowIntervalTime);
		if (m_bShowIntervalTime)
		{
			string remainTimeStringExWithoutEnd = NKCUtilString.GetRemainTimeStringExWithoutEnd(NKCUITrimUtility.GetRemainDateMsg());
			NKCUtil.SetLabelText(m_lbRemainDate, remainTimeStringExWithoutEnd);
		}
		m_trimReward?.SetData(m_trimId, m_selectedLevel);
		NKCUITrimUtility.SetBattleCondition(m_battleCondParent, nKMTrimTemplet, m_selectedLevel, showToolTip: true);
		int recommendedPower = NKCUITrimUtility.GetRecommendedPower(m_selectedGroup, m_selectedLevel);
		NKCUtil.SetLabelText(m_lbRecommendedPower, recommendedPower.ToString("N0"));
		UpdateSquadSlot(m_selectedGroup, m_selectedLevel);
		UpdateStartButtonState(userData, stageReqItemId, stageReqItemCount);
		NKCUtil.SetGameobjectActive(m_operationSkip, bValue: false);
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		if (m_trimLevelScrollRect.content.childCount <= 0)
		{
			return false;
		}
		switch (hotkey)
		{
		case HotkeyEventType.Up:
		{
			int num4 = Mathf.Max(1, m_selectedLevel - 1);
			OnClickLevelSlot(num4, isLocked: false);
			int index2 = Mathf.Max(0, num4 - 1);
			Transform child2 = m_trimLevelScrollRect.GetChild(index2);
			RectTransform component4 = m_trimLevelScrollRect.GetComponent<RectTransform>();
			if (!(child2 != null) || !(component4 != null) || !component4.GetWorldRect().Contains(child2.position))
			{
				VerticalLayoutGroup component5 = m_trimLevelScrollRect.content.GetComponent<VerticalLayoutGroup>();
				float num5 = ((component5 != null) ? component5.spacing : 0f);
				RectTransform component6 = m_trimLevelScrollRect.content.GetChild(0).GetComponent<RectTransform>();
				float num6 = ((component6 != null) ? component6.GetHeight() : 0f);
				m_trimLevelScrollRect.MovePosition(new Vector2(0f, 0f - num6 - num5));
			}
			return true;
		}
		case HotkeyEventType.Down:
		{
			int num = Mathf.Min(m_maxTrimLevel, m_selectedLevel + 1);
			if (num <= Mathf.Min(m_maxTrimLevel, m_clearedLevel + 1))
			{
				OnClickLevelSlot(num, isLocked: false);
				int index = Mathf.Max(0, num - 1);
				Transform child = m_trimLevelScrollRect.GetChild(index);
				RectTransform component = m_trimLevelScrollRect.GetComponent<RectTransform>();
				if (!(child != null) || !(component != null) || !component.GetWorldRect().Contains(child.position))
				{
					VerticalLayoutGroup component2 = m_trimLevelScrollRect.content.GetComponent<VerticalLayoutGroup>();
					float num2 = ((component2 != null) ? component2.spacing : 0f);
					RectTransform component3 = m_trimLevelScrollRect.content.GetChild(0).GetComponent<RectTransform>();
					float num3 = ((component3 != null) ? component3.GetHeight() : 0f);
					m_trimLevelScrollRect.MovePosition(new Vector2(0f, num3 + num2));
				}
			}
			return true;
		}
		default:
			return false;
		}
	}

	private void Update()
	{
		if (!m_bShowIntervalTime)
		{
			return;
		}
		if (m_dateUpdateTimerSec > 1f)
		{
			DateTime remainDateMsg = NKCUITrimUtility.GetRemainDateMsg();
			if (NKCSynchronizedTime.GetTimeLeft(remainDateMsg).TotalMinutes >= 1.0 && m_dateUpdateTimerMin < 60f)
			{
				m_dateUpdateTimerSec = 0f;
				return;
			}
			string remainTimeStringExWithoutEnd = NKCUtilString.GetRemainTimeStringExWithoutEnd(remainDateMsg);
			NKCUtil.SetLabelText(m_lbRemainDate, remainTimeStringExWithoutEnd);
			m_dateUpdateTimerSec = 0f;
			m_dateUpdateTimerMin = 0f;
		}
		m_dateUpdateTimerSec += Time.deltaTime;
		m_dateUpdateTimerMin += Time.deltaTime;
	}

	private RectTransform GetPresetSlot(int index)
	{
		return NKCUITrimLevelSlot.GetNewInstance(null)?.GetComponent<RectTransform>();
	}

	private void ReturnPresetSlot(Transform tr)
	{
		NKCUITrimLevelSlot component = tr.GetComponent<NKCUITrimLevelSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	private void ProvidePresetData(Transform tr, int index)
	{
		NKCUITrimLevelSlot component = tr.GetComponent<NKCUITrimLevelSlot>();
		if (!(component == null))
		{
			component.SetData(m_trimId, index + 1, m_clearedLevel, OnClickLevelSlot);
			component.SetSelectedState(m_selectedLevel);
			component.SetLock(index + 1 > Mathf.Min(m_maxTrimLevel, m_clearedLevel + 1));
		}
	}

	private void UpdateSquadSlot(int trimGroup, int trimLevel)
	{
		if (m_squadSlot == null)
		{
			return;
		}
		int num = 3;
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(m_trimId);
		int num2 = m_squadSlot.Length;
		for (int i = 0; i < num2; i++)
		{
			if (!(m_squadSlot[i] == null))
			{
				if (i < num)
				{
					int trimDungeonId = ((nKMTrimTemplet != null) ? nKMTrimTemplet.TrimDungeonIds[i] : 0);
					m_squadSlot[i].SetActive(value: true);
					m_squadSlot[i].SetData(m_trimId, trimDungeonId, trimGroup, trimLevel);
				}
				else
				{
					m_squadSlot[i].SetActive(value: false);
				}
			}
		}
	}

	private void UpdateStartButtonState(NKMUserData userData, int stageReqItemId, int stageReqItemCount)
	{
		NKMTrimIntervalTemplet trimInterval = NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime);
		bool num = !NKCUITrimUtility.IsEnterCountLimited(trimInterval) || NKCUITrimUtility.IsEnterCountRemaining(trimInterval);
		bool flag = !NKCUITrimUtility.IsRestoreEnterCountLimited(trimInterval) || NKCUITrimUtility.IsRestoreEnterCountEnable(trimInterval, userData);
		bool flag2 = !num && flag;
		bool flag3 = stageReqItemId > 0 && stageReqItemCount > 0;
		NKCUtil.SetGameobjectActive(m_csbtnStart, !flag2 && !flag3);
		NKCUtil.SetGameobjectActive(m_csbtnStartResource, flag2 || flag3);
		if (flag2)
		{
			int restoreItemReqId = NKCUITrimUtility.GetRestoreItemReqId(trimInterval);
			int restoreItemReqCount = NKCUITrimUtility.GetRestoreItemReqCount(trimInterval, userData);
			m_comResourceButton?.SetData(restoreItemReqId, restoreItemReqCount);
			m_comResourceButton?.SetTitleText(NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_START_BUTTON_TEXT"));
		}
		else if (flag3)
		{
			int eternium = stageReqItemCount;
			if (stageReqItemId == 2)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
			m_comResourceButton?.SetData(stageReqItemId, eternium);
			m_comResourceButton?.SetTitleText(NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_START_TEXT"));
		}
	}

	private void StartBattle(List<NKMEventDeckData> deckDataList)
	{
		if (m_isSkip)
		{
			if (m_selectedLevel > m_clearedLevel)
			{
				NKCPopupOKCancel.OpenOKBox(NKCStringTable.GetString("SI_DP_NOTICE"), NKCUtilString.GET_STRING_CONTENTS_UNLOCK_CLEAR_STAGE);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_TRIM_DUNGEON_SKIP_REQ(m_trimId, m_selectedLevel, m_skipCount, deckDataList);
			}
		}
		else
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_TRIM_RESULT().SetUnitUId(NKCLocalDeckDataManager.GetLocalLeaderUnitUId(0));
			NKCPacketSender.Send_NKMPacket_TRIM_START_REQ(m_trimId, m_selectedLevel, deckDataList);
		}
	}

	private void SetSkipCountUIData()
	{
		int maxCount = 99;
		NKMTrimIntervalTemplet trimInterval = NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime);
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(m_trimId);
		if (NKCUITrimUtility.IsEnterCountLimited(trimInterval))
		{
			maxCount = NKCUITrimUtility.GetRemainEnterCount(trimInterval);
		}
		int num = 0;
		int eternium = 0;
		if (nKMTrimTemplet != null)
		{
			num = nKMTrimTemplet.m_StageReqItemID;
			eternium = nKMTrimTemplet.m_StageReqItemCount;
			if (num == 2)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
		}
		m_operationSkip?.SetData(NKMCommonConst.SkipCostMiscItemId, NKMCommonConst.SkipCostMiscItemCount, num, eternium, m_skipCount, 1, maxCount);
	}

	private void UpdateAttackCost(NKMTrimTemplet trimTemplet)
	{
		if (trimTemplet != null)
		{
			int eternium = trimTemplet.m_StageReqItemCount;
			if (trimTemplet.m_StageReqItemID == 2)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
			m_comResourceButton?.SetData(trimTemplet.m_StageReqItemID, m_skipCount * eternium);
		}
	}

	private bool HaveEnoughResource()
	{
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(m_trimId);
		if (nKMTrimTemplet != null)
		{
			if (nKMTrimTemplet.m_StageReqItemID == 2)
			{
				if (!NKCUtil.IsCanStartEterniumStage(nKMTrimTemplet.m_StageReqItemID, nKMTrimTemplet.m_StageReqItemCount, bCallLackPopup: true))
				{
					return false;
				}
			}
			else if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(nKMTrimTemplet.m_StageReqItemID) < nKMTrimTemplet.m_StageReqItemCount)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ATTACK_COST_IS_NOT_ENOUGH);
				return false;
			}
		}
		return true;
	}

	private void OnDeckConfirm()
	{
		UpdateSquadSlot(m_selectedGroup, m_selectedLevel);
	}

	private void OnClickLevelSlot(int trimLevel, bool isLocked)
	{
		if (!isLocked)
		{
			m_selectedLevel = trimLevel;
			m_trimLevelScrollRect?.RefreshCells();
			NKCUtil.SetLabelText(m_lbTrimLevel, trimLevel.ToString());
			m_trimReward?.SetData(m_trimId, m_selectedLevel);
			NKMTrimTemplet trimTemplet = NKMTrimTemplet.Find(m_trimId);
			NKCUITrimUtility.SetBattleCondition(m_battleCondParent, trimTemplet, m_selectedLevel, showToolTip: true);
			int recommendedPower = NKCUITrimUtility.GetRecommendedPower(m_selectedGroup, m_selectedLevel);
			NKCUtil.SetLabelText(m_lbRecommendedPower, recommendedPower.ToString("N0"));
			UpdateSquadSlot(m_selectedGroup, m_selectedLevel);
			m_tglSkip.Select(bSelect: false);
		}
	}

	private void OnClickStart()
	{
		if (NKCTrimManager.ProcessTrim())
		{
			return;
		}
		NKMTrimIntervalTemplet trimInterval = NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (trimInterval == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRIM_INTERVAL_END, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
			});
			return;
		}
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(m_trimId);
		if (nKMTrimTemplet != null && !NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in nKMTrimTemplet.m_UnlockInfo))
		{
			string message = NKCContentManager.MakeUnlockConditionString(in nKMTrimTemplet.m_UnlockInfo, bSimple: false);
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(message, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		if (NKCUITrimUtility.IsEnterCountLimited(trimInterval) && !NKCUITrimUtility.IsEnterCountRemaining(trimInterval))
		{
			if (NKCUITrimUtility.IsRestoreEnterCountEnable(trimInterval, nKMUserData))
			{
				int remainRestoreCount = NKCUITrimUtility.GetRemainRestoreCount(trimInterval, nKMUserData);
				int restoreLimitCount = NKCUITrimUtility.GetRestoreLimitCount(trimInterval);
				string content = string.Format(NKCUtilString.GET_STRING_TRIM_NOT_ENOUGH_TRY_COUNT_RESTORE, remainRestoreCount, restoreLimitCount);
				int restoreItemReqId = NKCUITrimUtility.GetRestoreItemReqId(trimInterval);
				int restoreItemReqCount = NKCUITrimUtility.GetRestoreItemReqCount(trimInterval, nKMUserData);
				NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, content, restoreItemReqId, restoreItemReqCount, delegate
				{
					NKCPacketSender.Send_NKMPacket_TRIM_RESTORE_REQ(trimInterval.TrimIntervalID);
				});
			}
			else
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRIM_NOT_ENOUGH_TRY_COUNT);
			}
			return;
		}
		Dictionary<int, NKMEventDeckData> allLocalDeckData = NKCLocalDeckDataManager.GetAllLocalDeckData();
		List<NKMEventDeckData> deckDataList = new List<NKMEventDeckData>();
		for (int num = 0; num < 3; num++)
		{
			if (allLocalDeckData.ContainsKey(num))
			{
				deckDataList.Add(allLocalDeckData[num]);
			}
		}
		if (deckDataList.Count < 3)
		{
			Log.Error("Not Enough Deck", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Trim/NKCUIPopupTrimDungeon.cs", 599);
			return;
		}
		for (int num2 = 0; num2 < 3; num2++)
		{
			if (deckDataList[num2].m_ShipUID <= 0)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_DECK_NO_SHIP));
				return;
			}
		}
		if (!HaveEnoughResource())
		{
			return;
		}
		for (int num3 = 0; num3 < 3; num3++)
		{
			bool flag = false;
			foreach (long value in deckDataList[num3].m_dicUnit.Values)
			{
				if (value > 0)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRIM_NOT_ENOUGH_SQUAD);
				return;
			}
		}
		if (!m_operationSkip.gameObject.activeSelf)
		{
			int recommendedPower = NKCUITrimUtility.GetRecommendedPower(m_selectedGroup, m_selectedLevel);
			for (int num4 = 0; num4 < 3; num4++)
			{
				if (NKCLocalDeckDataManager.GetOperationPower(num4, bPVP: false, bPossibleShowBan: false, bPossibleShowUp: false) < recommendedPower)
				{
					NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRIM_NOT_ENOUGH_POWER, delegate
					{
						StartBattle(deckDataList);
					});
					return;
				}
			}
		}
		StartBattle(deckDataList);
	}

	private void OnClickSkip(bool bSet)
	{
		NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(m_trimId);
		if (nKMTrimTemplet == null)
		{
			m_tglSkip.Select(bSelect: false);
			return;
		}
		m_skipCount = 1;
		if (bSet)
		{
			if (m_selectedLevel > m_clearedLevel)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_CONTENTS_UNLOCK_CLEAR_STAGE);
				m_tglSkip.Select(bSelect: false);
				return;
			}
			if (!HaveEnoughResource())
			{
				m_tglSkip.Select(bSelect: false);
				return;
			}
			m_isSkip = true;
			UpdateAttackCost(nKMTrimTemplet);
			SetSkipCountUIData();
		}
		if (!bSet)
		{
			m_isSkip = false;
			UpdateAttackCost(nKMTrimTemplet);
			SetSkipCountUIData();
		}
		NKCUtil.SetGameobjectActive(m_operationSkip, bSet);
	}

	private void OnOperationSkipUpdated(int newCount)
	{
		m_skipCount = newCount;
		NKMTrimTemplet trimTemplet = NKMTrimTemplet.Find(m_trimId);
		UpdateAttackCost(trimTemplet);
	}

	private void OnClickOperationSkipClose()
	{
		m_tglSkip.Select(bSelect: false);
	}
}
