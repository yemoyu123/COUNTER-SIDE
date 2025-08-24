using System;
using System.Collections;
using System.Collections.Generic;
using ClientPacket.Event;
using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSubUIBingo : NKCUIEventSubUIBase
{
	[Header("버튼")]
	public NKCUIComStateButton m_btnGuidePopup;

	public NKCUIComStateButton m_btnMissionPopup;

	public NKCUIComStateButton m_btnRewardPopup;

	public NKCUIComStateButton m_btnTry;

	public NKCUIComStateButton m_btnSpecialTry;

	[Header("빙고판(가로(0~5),세로(6~11),↘(12),↙(13)")]
	public GridLayoutGroup m_gridSlotParent;

	public NKCUIBingoSlot m_prefabSlot;

	public List<NKCUISlot> m_listLineReward;

	public List<GameObject> m_listCompleteLine;

	public GameObject m_objSpacialMode;

	[Header("뽑기 버튼")]
	public Text m_txtTryBtn;

	public GameObject m_objTryBtnEnable;

	public GameObject m_objTryBtnDisable;

	public Color m_colorTryEnable;

	public Color m_colorTryDisable;

	public Image m_imgTryItemIcon;

	public Text m_txtTryItemCount;

	public Text m_lbTryItemTotalCount;

	public string m_strMoveShopTabWhenComplete = "TAB_HR,7";

	private const string DEFAULT_SHOP_TAB = "TAB_HR,7";

	[Header("확정 뽑기 버튼")]
	public Text m_txtSpecialBtn;

	public GameObject m_objSpecialBtnEnable;

	public GameObject m_objSpecialBtnDisable;

	public GameObject m_objSpecialBtnCancel;

	public Color m_colorSpecialEnable;

	public Color m_colorSpecialDisable;

	public Color m_colorSpecialCancel;

	[Header("마일리지")]
	public Text m_txtMileage;

	[Header("숫자 획득 연출")]
	public GameObject m_objGetNum;

	public Animator m_aniGetNum;

	public Text m_txtGetNum;

	public GameObject m_objAlreadyGetNum;

	[Header("최종 보상")]
	public NKCUISlot m_lastReward;

	[Header("빨콩")]
	public GameObject m_objMissionRedDot;

	public GameObject m_objRewardRedDot;

	private List<NKCUIBingoSlot> m_listSlot = new List<NKCUIBingoSlot>();

	private NKMEventBingoTemplet m_bingoTemplet;

	private bool m_specialMode;

	private int m_specialIndex = -1;

	private bool m_bTouch;

	private bool m_bPrecessGetNum;

	private float m_waitSeconds;

	private Coroutine m_coroutineGetNum;

	private const float WAIT_TIME = 2f;

	public override void Init()
	{
		base.Init();
		if (m_btnGuidePopup != null)
		{
			m_btnGuidePopup.PointerClick.RemoveAllListeners();
			m_btnGuidePopup.PointerClick.AddListener(OnTouchGuidePopup);
		}
		if (m_btnMissionPopup != null)
		{
			m_btnMissionPopup.PointerClick.RemoveAllListeners();
			m_btnMissionPopup.PointerClick.AddListener(OnTouchMissionPopup);
		}
		if (m_btnRewardPopup != null)
		{
			m_btnRewardPopup.PointerClick.RemoveAllListeners();
			m_btnRewardPopup.PointerClick.AddListener(OnTouchRewardPopup);
		}
		if (m_btnTry != null)
		{
			m_btnTry.PointerClick.RemoveAllListeners();
			m_btnTry.PointerClick.AddListener(OnTouchTry);
		}
		if (m_btnSpecialTry != null)
		{
			m_btnSpecialTry.PointerClick.RemoveAllListeners();
			m_btnSpecialTry.PointerClick.AddListener(OnTouchSpecialTry);
		}
		if (m_listLineReward != null)
		{
			for (int i = 0; i < m_listLineReward.Count; i++)
			{
				m_listLineReward[i].Init();
			}
		}
		if (m_lastReward != null)
		{
			m_lastReward.Init();
		}
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		NKMEventBingoTemplet bingoTemplet = NKMEventManager.GetBingoTemplet(tabTemplet.m_EventID);
		if (bingoTemplet == null)
		{
			Debug.LogError($"BingoEvent - 잘못된 EventID : {tabTemplet.m_EventID}");
			return;
		}
		EventBingo bingoData = NKMEventManager.GetBingoData(tabTemplet.m_EventID);
		if (bingoData == null)
		{
			Debug.LogError($"BingoEvent - 빙고데이터가 없음 : {tabTemplet.m_EventID}");
			return;
		}
		m_tabTemplet = tabTemplet;
		m_bingoTemplet = bingoTemplet;
		m_specialMode = false;
		m_specialIndex = -1;
		SetData(bingoData);
	}

	public override void Close()
	{
		if (m_coroutineGetNum != null)
		{
			StopCoroutine(m_coroutineGetNum);
		}
		m_coroutineGetNum = null;
		m_bPrecessGetNum = false;
		m_specialMode = false;
		m_specialIndex = -1;
		if (m_listSlot != null)
		{
			for (int i = 0; i < m_listSlot.Count; i++)
			{
				m_listSlot[i].SetSelectFx(active: false);
				m_listSlot[i].SetGetFx(active: false);
			}
		}
		NKCUIManager.SetScreenInputBlock(bSet: false);
	}

	public override void Hide()
	{
		if (m_listSlot != null)
		{
			for (int i = 0; i < m_listSlot.Count; i++)
			{
				m_listSlot[i].SetSelectFx(active: false);
				m_listSlot[i].SetGetFx(active: false);
			}
		}
	}

	private void OnDisable()
	{
		if (m_listSlot != null)
		{
			for (int i = 0; i < m_listSlot.Count; i++)
			{
				m_listSlot[i].SetSelectFx(active: false);
				m_listSlot[i].SetGetFx(active: false);
			}
		}
	}

	public override bool OnBackButton()
	{
		if (m_bPrecessGetNum)
		{
			m_bTouch = true;
			return true;
		}
		return false;
	}

	private void SetData(EventBingo bingoData)
	{
		int bingoSize = m_bingoTemplet.m_BingoSize;
		InitBingoSlot(bingoSize);
		_ = bingoData.m_bingoInfo;
		if (m_listLineReward != null)
		{
			for (int i = 0; i < m_listLineReward.Count; i++)
			{
				NKCUISlot nKCUISlot = m_listLineReward[i];
				NKMEventBingoRewardTemplet bingoRewardTemplet = NKMEventManager.GetBingoRewardTemplet(m_bingoTemplet.m_BingoCompletRewardGroupID, BingoCompleteType.LINE_SINGLE, i + 1);
				if (bingoRewardTemplet == null)
				{
					NKCUtil.SetGameobjectActive(nKCUISlot, bValue: false);
					continue;
				}
				if (bingoRewardTemplet.rewards == null || bingoRewardTemplet.rewards.Count == 0)
				{
					NKCUtil.SetGameobjectActive(nKCUISlot, bValue: false);
					continue;
				}
				NKMRewardInfo nKMRewardInfo = bingoRewardTemplet.rewards[0];
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo.rewardType, nKMRewardInfo.ID, nKMRewardInfo.Count);
				nKCUISlot.SetData(data);
			}
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_bingoTemplet.m_BingoTryItemID);
		if (itemMiscTempletByID != null)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
			NKCUtil.SetImageSprite(m_imgTryItemIcon, orLoadMiscItemSmallIcon);
		}
		NKCUtil.SetLabelText(m_txtTryItemCount, m_bingoTemplet.m_BingoTryItemValue.ToString());
		if (m_lastReward != null)
		{
			NKMEventBingoRewardTemplet bingoLastRewardTemplet = NKMEventManager.GetBingoLastRewardTemplet(m_bingoTemplet.m_EventID);
			if (bingoLastRewardTemplet != null)
			{
				NKMRewardInfo nKMRewardInfo2 = bingoLastRewardTemplet.rewards[0];
				NKCUISlot.SlotData data2 = NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo2.rewardType, nKMRewardInfo2.ID, nKMRewardInfo2.Count);
				m_lastReward.SetData(data2);
			}
		}
		SetDateLimit();
		Refresh();
	}

	public override void Refresh()
	{
		EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		if (bingoData == null)
		{
			Close();
			return;
		}
		UpdateBingoSlot(bingoData);
		UpdateRewardSlot(bingoData);
		UpdateLine(bingoData);
		UpdateMileage(bingoData);
		UpdateSpecialButton(bingoData);
		UpdateTryButton(bingoData);
		UpdateLastReward(bingoData);
		UpdateRedDot();
		NKCUtil.SetGameobjectActive(m_objSpacialMode, m_specialMode);
		SetSlotSpecialMode(m_specialMode);
		SetSlotSelectFx(m_specialIndex);
		NKCUtil.SetGameobjectActive(m_objAlreadyGetNum, bValue: false);
		NKCUtil.SetGameobjectActive(m_objGetNum, bValue: false);
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		UpdateTryButton(bingoData);
		UpdateRedDot();
	}

	private void UpdateBingoSlot(EventBingo bingoData)
	{
		BingoInfo bingoInfo = bingoData.m_bingoInfo;
		int bingoSize = m_bingoTemplet.m_BingoSize;
		int num = 0;
		for (int i = 0; i < m_listSlot.Count; i++)
		{
			NKCUIBingoSlot nKCUIBingoSlot = m_listSlot[i];
			if ((double)i < Math.Pow(bingoSize, 2.0) && i < bingoInfo.tileValueList.Count)
			{
				if (m_bingoTemplet.MissionTiles.Contains(i))
				{
					nKCUIBingoSlot.SetData(i, ++num, bingoInfo.markTileIndexList.Contains(i), isMission: true);
				}
				else
				{
					nKCUIBingoSlot.SetData(i, bingoInfo.tileValueList[i], bingoInfo.markTileIndexList.Contains(i), isMission: false);
				}
				NKCUtil.SetGameobjectActive(nKCUIBingoSlot, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIBingoSlot, bValue: false);
			}
		}
	}

	private void UpdateRewardSlot(EventBingo bingoData)
	{
		if (m_listLineReward == null || bingoData == null)
		{
			return;
		}
		List<int> bingoLine = bingoData.GetBingoLine();
		for (int i = 0; i < m_listLineReward.Count; i++)
		{
			NKCUISlot nKCUISlot = m_listLineReward[i];
			bool flag = bingoLine.Contains(i);
			NKMEventBingoRewardTemplet rewardTemplet = NKMEventManager.GetBingoRewardTemplet(m_bingoTemplet.m_BingoCompletRewardGroupID, BingoCompleteType.LINE_SINGLE, i + 1);
			bool flag2 = false;
			if (rewardTemplet != null)
			{
				flag2 = NKMEventManager.IsReceiveableBingoReward(m_bingoTemplet.m_EventID, rewardTemplet.ZeroBaseTileIndex);
			}
			nKCUISlot.SetDisable(flag && !flag2);
			nKCUISlot.SetEventGet(flag && !flag2);
			nKCUISlot.SetRewardFx(flag && flag2);
			if (m_specialMode || flag2)
			{
				nKCUISlot.SetOnClick(delegate
				{
					OnTouchBingoReward(rewardTemplet.ZeroBaseTileIndex);
				});
			}
			else
			{
				nKCUISlot.SetOpenItemBoxOnClick();
			}
		}
	}

	private void UpdateLine(EventBingo bingoData)
	{
		if (m_listCompleteLine != null)
		{
			List<int> bingoLine = bingoData.GetBingoLine();
			for (int i = 0; i < m_listCompleteLine.Count; i++)
			{
				NKCUtil.SetGameobjectActive(m_listCompleteLine[i], bingoLine.Contains(i));
			}
		}
	}

	private void UpdateMileage(EventBingo bingoData)
	{
		if (!(m_txtMileage == null))
		{
			int mileage = bingoData.m_bingoInfo.mileage;
			int bingoSpecialTryRequireCnt = m_bingoTemplet.m_BingoSpecialTryRequireCnt;
			string arg = ((mileage < bingoSpecialTryRequireCnt) ? $"<color=#cd2121>{mileage}</color>" : mileage.ToString());
			m_txtMileage.text = string.Format(NKCUtilString.GET_STRING_EVENT_BINGO_MILEAGE, arg, bingoSpecialTryRequireCnt);
		}
	}

	private void UpdateSpecialButton(EventBingo bingoData)
	{
		int mileage = bingoData.m_bingoInfo.mileage;
		int bingoSpecialTryRequireCnt = m_bingoTemplet.m_BingoSpecialTryRequireCnt;
		bool flag = mileage >= bingoSpecialTryRequireCnt;
		bool flag2 = !m_specialMode && flag && bingoData.IsRemainNum();
		string msg;
		Color col;
		if (m_specialMode)
		{
			msg = NKCUtilString.GET_STRING_EVENT_BINGO_SPECIAL_CANCEL;
			col = m_colorSpecialCancel;
		}
		else if (flag2)
		{
			msg = NKCUtilString.GET_STRING_EVENT_BINGO_SPECIAL;
			col = m_colorSpecialEnable;
		}
		else
		{
			msg = NKCUtilString.GET_STRING_EVENT_BINGO_SPECIAL;
			col = m_colorSpecialDisable;
		}
		NKCUtil.SetGameobjectActive(m_objSpecialBtnEnable, !m_specialMode && flag2);
		NKCUtil.SetGameobjectActive(m_objSpecialBtnDisable, !m_specialMode && !flag2);
		NKCUtil.SetGameobjectActive(m_objSpecialBtnCancel, m_specialMode);
		NKCUtil.SetLabelTextColor(m_txtSpecialBtn, col);
		NKCUtil.SetLabelText(m_txtSpecialBtn, msg);
	}

	private void UpdateTryButton(EventBingo bingoData)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && bingoData != null)
		{
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(m_bingoTemplet.m_BingoTryItemID);
			NKCUtil.SetLabelText(m_lbTryItemTotalCount, NKCUtilString.GET_STRING_HAVE_COUNT_ONE_PARAM, countMiscItem);
			if (bingoData.IsRemainNum())
			{
				bool flag = nKMUserData.CheckPrice(m_bingoTemplet.m_BingoTryItemValue, m_bingoTemplet.m_BingoTryItemID);
				bool flag2 = !m_specialMode && flag;
				NKCUtil.SetGameobjectActive(m_objTryBtnEnable, flag2);
				NKCUtil.SetGameobjectActive(m_objTryBtnDisable, !flag2);
				NKCUtil.SetLabelText(m_txtTryBtn, NKCStringTable.GetString("SI_BINGO_BUTTON_GACHA_TEXT"));
				NKCUtil.SetLabelTextColor(m_txtTryBtn, flag2 ? m_colorTryEnable : m_colorTryDisable);
				NKCUtil.SetLabelTextColor(m_txtTryItemCount, flag2 ? m_colorTryEnable : m_colorTryDisable);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objTryBtnEnable, bValue: true);
				NKCUtil.SetGameobjectActive(m_objTryBtnDisable, bValue: false);
				NKCUtil.SetLabelText(m_txtTryBtn, NKCStringTable.GetString("SI_BINGO_BUTTON_SHOP"));
				NKCUtil.SetLabelTextColor(m_txtTryBtn, m_colorTryEnable);
				NKCUtil.SetLabelTextColor(m_txtTryItemCount, m_colorTryEnable);
			}
		}
	}

	private void UpdateLastReward(EventBingo bingoData)
	{
		if (m_lastReward != null)
		{
			NKMEventBingoRewardTemplet bingoLastRewardTemplet = NKMEventManager.GetBingoLastRewardTemplet(m_bingoTemplet.m_EventID);
			if (bingoLastRewardTemplet != null)
			{
				bool flag = bingoData.m_bingoInfo.rewardList.Contains(bingoLastRewardTemplet.ZeroBaseTileIndex);
				m_lastReward.SetEventGet(flag);
				m_lastReward.SetDisable(flag);
			}
		}
	}

	private void UpdateRedDot()
	{
		NKCUtil.SetGameobjectActive(m_objMissionRedDot, NKMEventManager.CheckRedDotBingoMission(m_bingoTemplet.m_EventID));
		NKCUtil.SetGameobjectActive(m_objRewardRedDot, NKMEventManager.CheckRedDotBingoSet(m_bingoTemplet.m_EventID));
	}

	private void InitBingoSlot(int bingoSize)
	{
		if (m_listSlot.Count <= 0)
		{
			int num = (int)Math.Pow(bingoSize, 2.0);
			for (int i = 0; i < num; i++)
			{
				NKCUIBingoSlot nKCUIBingoSlot = UnityEngine.Object.Instantiate(m_prefabSlot, m_gridSlotParent.transform);
				nKCUIBingoSlot.transform.localScale = Vector3.one;
				nKCUIBingoSlot.gameObject.SetActive(value: true);
				nKCUIBingoSlot.Init(OnMarkTile);
				m_listSlot.Add(nKCUIBingoSlot);
			}
			m_gridSlotParent.constraintCount = bingoSize;
		}
	}

	private void SetSlotSpecialMode(bool bMode)
	{
		NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		for (int i = 0; i < m_listSlot.Count; i++)
		{
			m_listSlot[i].SetSpecialMode(bMode);
		}
	}

	private void SetSlotSelectFx(int index)
	{
		for (int i = 0; i < m_listSlot.Count; i++)
		{
			m_listSlot[i].SetSelectFx(i == index);
		}
	}

	private void SetSlotGetFx(int index)
	{
		if (index < m_listSlot.Count)
		{
			m_listSlot[index].SetGetFx(active: false);
			m_listSlot[index].SetGetFx(active: true);
		}
	}

	private void OnTouchGuidePopup()
	{
		if (CheckEventTime() && !m_specialMode)
		{
			NKCPopupEventHelp.Instance.Open(m_bingoTemplet.m_EventID);
		}
	}

	private void OnTouchMissionPopup()
	{
		if (CheckEventTime() && !m_specialMode)
		{
			NKCPopupEventMission.Instance.Open(m_bingoTemplet, base.CheckEventTime, OnCompleteMission);
		}
	}

	private void OnTouchRewardPopup()
	{
		if (CheckEventTime() && !m_specialMode)
		{
			NKCPopupEventBingoReward.Instance.Open(m_bingoTemplet.m_EventID, OnTouchBingoReward);
		}
	}

	private void MoveToShop()
	{
		if (string.IsNullOrEmpty(m_strMoveShopTabWhenComplete))
		{
			m_strMoveShopTabWhenComplete = "TAB_HR,7";
		}
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_SHOP, m_strMoveShopTabWhenComplete);
	}

	private void OnTouchTry()
	{
		if (!CheckEventTime() || m_specialMode)
		{
			return;
		}
		EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		if (bingoData != null && !bingoData.IsRemainNum())
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_bingoTemplet.m_BingoTryItemID);
			if (itemMiscTempletByID != null && !string.IsNullOrEmpty(m_strMoveShopTabWhenComplete))
			{
				string content = NKCStringTable.GetString("SI_DP_BINGO_MOVETO_SHOP_REMAIN_EXCHANGE", itemMiscTempletByID.GetItemName());
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, content, MoveToShop);
			}
			else
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_BINGO_COMPLETE);
			}
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && nKMUserData.CheckPrice(m_bingoTemplet.m_BingoTryItemValue, m_bingoTemplet.m_BingoTryItemID))
		{
			if (NKMItemManager.GetItemMiscTempletByID(m_bingoTemplet.m_BingoTryItemID) == null)
			{
				Debug.LogError($"item templet is null - {m_bingoTemplet.m_BingoTryItemID}");
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_EVENT_BINGO_RANDOM_MARK_REQ(m_bingoTemplet.m_EventID);
			}
		}
		else
		{
			NKCShopManager.OpenItemLackPopup(m_bingoTemplet.m_BingoTryItemID, m_bingoTemplet.m_BingoTryItemValue);
		}
	}

	private void OnTouchSpecialTry()
	{
		if (!CheckEventTime())
		{
			return;
		}
		EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		int mileage = bingoData.m_bingoInfo.mileage;
		int bingoSpecialTryRequireCnt = m_bingoTemplet.m_BingoSpecialTryRequireCnt;
		if (mileage < bingoSpecialTryRequireCnt)
		{
			return;
		}
		if (!bingoData.IsRemainNum())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_BINGO_COMPLETE);
			return;
		}
		if (m_specialMode)
		{
			m_specialMode = false;
		}
		else
		{
			m_specialMode = true;
		}
		UpdateSpecialButton(bingoData);
		UpdateTryButton(bingoData);
		UpdateLine(bingoData);
		UpdateRewardSlot(bingoData);
		m_specialIndex = -1;
		SetSlotSpecialMode(m_specialMode);
		SetSlotSelectFx(m_specialIndex);
		NKCUtil.SetGameobjectActive(m_objSpacialMode, m_specialMode);
	}

	private void OnMarkTile(int index)
	{
		if (!CheckEventTime())
		{
			return;
		}
		if (!m_specialMode)
		{
			Debug.LogError("bingo - 확정뽑기 모드가 아닌데 어케함?");
		}
		else if (index == m_specialIndex)
		{
			EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
			if (bingoData != null)
			{
				int tileValue = bingoData.GetTileValue(index);
				string content = string.Format(NKCUtilString.GET_STRING_EVENT_BINGO_USE_MILEAGE, m_bingoTemplet.m_BingoSpecialTryRequireCnt, tileValue);
				string strPoint = string.Format(NKCUtilString.GET_STRING_EVENT_BINGO_REMAIN_MILEAGE, bingoData.m_bingoInfo.mileage);
				NKCPopupResourceTextConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, content, strPoint, delegate
				{
					NKCPacketSender.Send_NKMPacket_EVENT_BINGO_INDEX_MARK_REQ(m_bingoTemplet.m_EventID, index);
				});
			}
		}
		else
		{
			m_specialIndex = index;
			SetSlotSelectFx(index);
		}
	}

	private void OnTouchBingoReward(int rewardIndex)
	{
		if (CheckEventTime() && !m_specialMode && NKMEventManager.IsReceiveableBingoReward(m_bingoTemplet.m_EventID, rewardIndex))
		{
			Debug.Log($"bingo rewardIndex : {rewardIndex}");
			NKCPacketSender.Send_NKMPacket_EVENT_BINGO_REWARD_REQ(m_bingoTemplet.m_EventID, rewardIndex);
		}
	}

	private void OnCompleteMission(int eventID, int tileIndex)
	{
		if (eventID == m_bingoTemplet.m_EventID)
		{
			GetTile(tileIndex, bRandom: false);
		}
	}

	public void GetTile(int tileIndex, bool bRandom)
	{
		if (!bRandom)
		{
			SetSlotGetFx(tileIndex);
			m_specialMode = false;
			m_specialIndex = -1;
			Refresh();
		}
		else
		{
			SetRandomFX(tileIndex);
		}
	}

	public void SetRandomFX(int tileIndex)
	{
		EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		if (bingoData == null)
		{
			return;
		}
		int tileValue = bingoData.GetTileValue(tileIndex);
		if (tileValue <= 0)
		{
			NKCUtil.SetGameobjectActive(m_objGetNum, bValue: false);
			return;
		}
		bool flag = false;
		if (tileIndex < m_listSlot.Count)
		{
			flag = m_listSlot[tileIndex].IsHas;
		}
		NKCUtil.SetGameobjectActive(m_objAlreadyGetNum, flag);
		NKCUtil.SetLabelText(m_txtGetNum, tileValue.ToString());
		NKCUtil.SetGameobjectActive(m_objGetNum, bValue: true);
		if (m_coroutineGetNum != null)
		{
			StopCoroutine(m_coroutineGetNum);
		}
		m_coroutineGetNum = StartCoroutine(ProcessGetNum(tileIndex, flag));
	}

	private IEnumerator ProcessGetNum(int tileIndex, bool bAlreadyGet)
	{
		m_bTouch = false;
		m_bPrecessGetNum = true;
		m_waitSeconds = 0f;
		NKCUIManager.SetScreenInputBlock(bSet: true);
		if (m_aniGetNum != null)
		{
			yield return PlayGetNumIntro();
			while (!m_bTouch && m_waitSeconds < 2f)
			{
				m_waitSeconds += Time.deltaTime;
				yield return null;
			}
			yield return PlayGetNumOutro();
		}
		if (!bAlreadyGet)
		{
			SetSlotGetFx(tileIndex);
		}
		Refresh();
		NKCUIManager.SetScreenInputBlock(bSet: false);
		m_bPrecessGetNum = false;
	}

	private IEnumerator PlayGetNumIntro()
	{
		if (m_aniGetNum != null)
		{
			m_aniGetNum.Play("NKM_UI_EVENT_BINGO_GACHA_INTRO");
			yield return null;
			while (!m_bTouch && !(m_aniGetNum.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
			{
				yield return null;
			}
			m_aniGetNum.Play("NKM_UI_EVENT_BINGO_GACHA_ROOP");
		}
	}

	private IEnumerator PlayGetNumOutro()
	{
		if (m_aniGetNum != null)
		{
			m_aniGetNum.Play("NKM_UI_EVENT_BINGO_GACHA_OUTRO");
			yield return null;
			while (!m_bTouch && !(m_aniGetNum.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
			{
				yield return null;
			}
		}
	}

	private void Update()
	{
		if (Input.anyKeyDown)
		{
			m_bTouch = true;
		}
	}

	public void MarkBingo(List<NKMBingoTile> bingoList, bool bRandom)
	{
		if (bingoList == null)
		{
			return;
		}
		for (int i = 0; i < bingoList.Count; i++)
		{
			if (bingoList[i].eventId == m_bingoTemplet.m_EventID)
			{
				GetTile(bingoList[i].tileIndex, bRandom);
				break;
			}
		}
	}
}
