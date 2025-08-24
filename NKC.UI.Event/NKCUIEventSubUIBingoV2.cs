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

public class NKCUIEventSubUIBingoV2 : NKCUIEventSubUIBase
{
	[Header("\ufffd\ufffdư")]
	public NKCUIComStateButton m_btnGuidePopup;

	public NKCUIComStateButton m_btnMissionPopup;

	public NKCUIComStateButton m_btnTry;

	public NKCUIComStateButton m_btnSpecialTry;

	public NKCUIComStateButton m_btnCancelSpecialTry;

	public GameObject m_objShop;

	public NKCUIComStateButton m_btnShop;

	public NKCUIComStateButton m_btnRewardAllCollect;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd(\ufffd\ufffd\ufffd\ufffd(0~5),\ufffd\ufffd\ufffd\ufffd(6~11),\ufffd\ufffd(12),\ufffd\ufffd(13)")]
	public GridLayoutGroup m_gridSlotParent;

	public NKCUIBingoSlot m_prefabSlot;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public List<NKCUISlot> m_lstLineRewardHorizontal;

	public List<NKCUISlot> m_lstLineRewardVertical;

	[Tooltip("\ufffd\ufffd")]
	public NKCUISlot m_slotLineRewardDiagonalUpperLeft;

	[Tooltip("\ufffd\ufffd")]
	public NKCUISlot m_slotLineRewardDiagonalUpperRight;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffdϷ\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public List<GameObject> m_lstLineHorizontal;

	public List<GameObject> m_lstLineVertical;

	[Tooltip("\ufffd\ufffd")]
	public GameObject m_LineDiagonalUpperLeft;

	[Tooltip("\ufffd\ufffd")]
	public GameObject m_LineDiagonalUpperRight;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdڿ\ufffd")]
	public NKCUIComItemCount m_TryItemMyCount;

	public Text m_lbMyMileage;

	[Header("\ufffdؽ\ufffdƮ \ufffd÷\ufffd")]
	public Color m_colOK;

	public Color m_colNotEnough;

	[Header("\ufffd\u0331\ufffd \ufffd\ufffdư")]
	public Image m_imgTryItemIcon;

	public Text m_txtTryItemCount;

	private const string DEFAULT_SHOP_TAB = "TAB_HR,7";

	[Header("\ufffd\ufffd\ufffdϸ\ufffd\ufffd\ufffd")]
	public Text m_lbRequireMileage;

	public GameObject m_objSpacialMode;

	[Header("\ufffd\ufffd\ufffd\ufffd ȹ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objGetNum;

	public Animator m_aniGetNum;

	public Text m_txtGetNum;

	public GameObject m_objAlreadyGetNum;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public List<NKCUIEventSubUIBingoV2CompleteRewardSlot> m_lstCompleteReward;

	public Text m_lbCompleteLineCount;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objMissionRedDot;

	public GameObject m_objRewardRedDot;

	private List<NKCUIBingoSlot> m_listSlot = new List<NKCUIBingoSlot>();

	private NKMEventBingoTemplet m_bingoTemplet;

	private bool m_specialMode;

	private HashSet<int> m_hsSpecialIndex = new HashSet<int>();

	private bool m_bTouch;

	private bool m_bPrecessGetNum;

	private float m_waitSeconds;

	private Coroutine m_coroutineGetNum;

	private const float WAIT_TIME = 2f;

	public IEnumerable<NKCUISlot> LineRewards
	{
		get
		{
			foreach (NKCUISlot item in m_lstLineRewardHorizontal)
			{
				yield return item;
			}
			foreach (NKCUISlot item2 in m_lstLineRewardVertical)
			{
				yield return item2;
			}
			yield return m_slotLineRewardDiagonalUpperLeft;
			yield return m_slotLineRewardDiagonalUpperRight;
		}
	}

	public IEnumerable<GameObject> CompleteLines
	{
		get
		{
			foreach (GameObject item in m_lstLineHorizontal)
			{
				yield return item;
			}
			foreach (GameObject item2 in m_lstLineVertical)
			{
				yield return item2;
			}
			yield return m_LineDiagonalUpperLeft;
			yield return m_LineDiagonalUpperRight;
		}
	}

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
		NKCUtil.SetButtonClickDelegate(m_btnCancelSpecialTry, CancelSpecialTry);
		NKCUtil.SetButtonClickDelegate(m_btnShop, MoveToShop);
		NKCUtil.SetButtonClickDelegate(m_btnRewardAllCollect, OnRewardAllCollect);
		foreach (NKCUISlot lineReward in LineRewards)
		{
			if (lineReward != null)
			{
				lineReward.Init();
			}
		}
		foreach (NKCUIEventSubUIBingoV2CompleteRewardSlot item in m_lstCompleteReward)
		{
			if (item != null)
			{
				item.Init(OnTouchBingoReward);
			}
		}
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		NKMEventBingoTemplet bingoTemplet = NKMEventManager.GetBingoTemplet(tabTemplet.m_EventID);
		if (bingoTemplet == null)
		{
			Debug.LogError($"BingoEvent - \ufffd߸\ufffd\ufffd\ufffd EventID : {tabTemplet.m_EventID}");
			return;
		}
		EventBingo bingoData = NKMEventManager.GetBingoData(tabTemplet.m_EventID);
		if (bingoData == null)
		{
			Debug.LogError($"BingoEvent - \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdͰ\ufffd \ufffd\ufffd\ufffd\ufffd : {tabTemplet.m_EventID}");
			return;
		}
		m_tabTemplet = tabTemplet;
		m_bingoTemplet = bingoTemplet;
		m_specialMode = false;
		m_hsSpecialIndex.Clear();
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
		m_hsSpecialIndex.Clear();
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
		if (m_specialMode)
		{
			CancelSpecialTry();
			return true;
		}
		return false;
	}

	private void SetData(EventBingo bingoData)
	{
		int bingoSize = m_bingoTemplet.m_BingoSize;
		InitBingoSlot(bingoSize);
		_ = bingoData.m_bingoInfo;
		int num = 0;
		foreach (NKCUISlot lineReward in LineRewards)
		{
			NKMEventBingoRewardTemplet bingoRewardTemplet = NKMEventManager.GetBingoRewardTemplet(m_bingoTemplet.m_BingoCompletRewardGroupID, BingoCompleteType.LINE_SINGLE, num + 1);
			if (bingoRewardTemplet == null)
			{
				NKCUtil.SetGameobjectActive(lineReward, bValue: false);
				continue;
			}
			if (bingoRewardTemplet.rewards == null || bingoRewardTemplet.rewards.Count == 0)
			{
				NKCUtil.SetGameobjectActive(lineReward, bValue: false);
				continue;
			}
			NKMRewardInfo nKMRewardInfo = bingoRewardTemplet.rewards[0];
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo.rewardType, nKMRewardInfo.ID, nKMRewardInfo.Count);
			lineReward.SetData(data);
			num++;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_bingoTemplet.m_BingoTryItemID);
		if (itemMiscTempletByID != null)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
			NKCUtil.SetImageSprite(m_imgTryItemIcon, orLoadMiscItemSmallIcon);
		}
		SetCompleteRewardData(bingoData);
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
		UpdateBottomButtons(bingoData);
		SetCompleteRewardData(bingoData);
		UpdateRedDot();
		NKCUtil.SetGameobjectActive(m_objSpacialMode, m_specialMode);
		SetSlotSpecialMode(m_specialMode);
		SetSlotSelectFx(m_hsSpecialIndex);
		NKCUtil.SetGameobjectActive(m_objAlreadyGetNum, bValue: false);
		NKCUtil.SetGameobjectActive(m_objGetNum, bValue: false);
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		UpdateBottomButtons(bingoData);
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
		if (bingoData == null)
		{
			return;
		}
		List<int> bingoLine = bingoData.GetBingoLine();
		int num = 0;
		foreach (NKCUISlot lineReward in LineRewards)
		{
			bool flag = bingoLine.Contains(num);
			NKMEventBingoRewardTemplet rewardTemplet = NKMEventManager.GetBingoRewardTemplet(m_bingoTemplet.m_BingoCompletRewardGroupID, BingoCompleteType.LINE_SINGLE, num + 1);
			bool flag2 = false;
			if (rewardTemplet != null)
			{
				flag2 = NKMEventManager.IsReceiveableBingoReward(m_bingoTemplet.m_EventID, rewardTemplet.ZeroBaseTileIndex);
			}
			lineReward.SetDisable(flag && !flag2);
			lineReward.SetEventGet(flag && !flag2);
			lineReward.SetRewardFx(flag && flag2);
			if (m_specialMode || flag2)
			{
				lineReward.SetOnClick(delegate
				{
					OnTouchBingoReward(rewardTemplet.ZeroBaseTileIndex);
				});
			}
			else
			{
				lineReward.SetOpenItemBoxOnClick();
			}
			num++;
		}
	}

	private void UpdateLine(EventBingo bingoData)
	{
		List<int> bingoLine = bingoData.GetBingoLine();
		int num = 0;
		foreach (GameObject completeLine in CompleteLines)
		{
			NKCUtil.SetGameobjectActive(completeLine, bingoLine.Contains(num));
			num++;
		}
	}

	private void UpdateMileage(EventBingo bingoData)
	{
		int num = ((!m_specialMode) ? 1 : m_hsSpecialIndex.Count);
		int mileage = bingoData.m_bingoInfo.mileage;
		int num2 = m_bingoTemplet.m_BingoSpecialTryRequireCnt * num;
		NKCUtil.SetLabelText(m_lbRequireMileage, num2.ToString());
		NKCUtil.SetLabelTextColor(m_lbRequireMileage, (mileage >= num2) ? m_colOK : m_colNotEnough);
		NKCUtil.SetLabelText(m_lbMyMileage, mileage.ToString());
	}

	private void UpdateBottomButtons(EventBingo bingoData)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && bingoData != null)
		{
			if (m_TryItemMyCount != null)
			{
				m_TryItemMyCount.SetData(nKMUserData, m_bingoTemplet.m_BingoTryItemID);
			}
			long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(m_bingoTemplet.m_BingoTryItemID);
			NKCUtil.SetLabelText(m_txtTryItemCount, m_bingoTemplet.m_BingoTryItemValue.ToString());
			NKCUtil.SetLabelTextColor(m_txtTryItemCount, (countMiscItem >= m_bingoTemplet.m_BingoTryItemValue) ? m_colOK : m_colNotEnough);
			if (bingoData.IsRemainNum())
			{
				if (m_specialMode)
				{
					NKCUtil.SetGameobjectActive(m_btnTry, bValue: false);
					NKCUtil.SetGameobjectActive(m_btnSpecialTry, bValue: true);
					NKCUtil.SetGameobjectActive(m_btnCancelSpecialTry, bValue: true);
					NKCUtil.SetGameobjectActive(m_objShop, bValue: false);
					NKCUtil.SetGameobjectActive(m_btnRewardAllCollect, bValue: true);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_btnTry, bValue: true);
					NKCUtil.SetGameobjectActive(m_btnSpecialTry, bValue: true);
					NKCUtil.SetGameobjectActive(m_btnCancelSpecialTry, bValue: false);
					NKCUtil.SetGameobjectActive(m_objShop, bValue: false);
					NKCUtil.SetGameobjectActive(m_btnRewardAllCollect, bValue: true);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_btnTry, bValue: false);
				NKCUtil.SetGameobjectActive(m_btnSpecialTry, bValue: false);
				NKCUtil.SetGameobjectActive(m_btnCancelSpecialTry, bValue: false);
				NKCUtil.SetGameobjectActive(m_objShop, bValue: true);
				NKCUtil.SetGameobjectActive(m_btnRewardAllCollect, bValue: true);
			}
		}
		if (m_btnRewardAllCollect != null)
		{
			bool flag = NKMEventManager.CheckRedDotBingoSingle(m_bingoTemplet.m_EventID) || NKMEventManager.CheckRedDotBingoSet(m_bingoTemplet.m_EventID);
			m_btnRewardAllCollect.SetLock(!flag);
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

	private void SetSlotSelectFx(HashSet<int> hsSelection)
	{
		for (int i = 0; i < m_listSlot.Count; i++)
		{
			m_listSlot[i].SetSelectFx(hsSelection.Contains(i));
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

	private void MoveToShop()
	{
		NKCContentManager.MoveToShortCut(m_tabTemplet.m_ShortCutType, m_tabTemplet.m_ShortCut);
	}

	private void OnRewardAllCollect()
	{
		NKCPacketSender.Send_NKMPacket_EVENT_BINGO_REWARD_ALL_REQ(m_bingoTemplet.m_EventID);
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
			if (itemMiscTempletByID != null && m_tabTemplet.m_ShortCutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE)
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
		if (!m_specialMode)
		{
			int mileage = bingoData.m_bingoInfo.mileage;
			int bingoSpecialTryRequireCnt = m_bingoTemplet.m_BingoSpecialTryRequireCnt;
			if (mileage < bingoSpecialTryRequireCnt)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_EVENT_BINGO_LACK_POINT_POPUP"));
				return;
			}
			if (!bingoData.IsRemainNum())
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EVENT_BINGO_COMPLETE);
				return;
			}
			m_specialMode = true;
			m_hsSpecialIndex.Clear();
			UpdateBottomButtons(bingoData);
			UpdateLine(bingoData);
			UpdateRewardSlot(bingoData);
			SetSlotSpecialMode(m_specialMode);
			SetSlotSelectFx(m_hsSpecialIndex);
			UpdateMileage(bingoData);
			NKCUtil.SetGameobjectActive(m_objSpacialMode, m_specialMode);
		}
		else if (m_hsSpecialIndex.Count <= 0)
		{
			CancelSpecialTry();
		}
		else
		{
			OnSpecialConfirm();
		}
	}

	private void CancelSpecialTry()
	{
		EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		m_specialMode = false;
		m_hsSpecialIndex.Clear();
		UpdateBottomButtons(bingoData);
		UpdateLine(bingoData);
		UpdateRewardSlot(bingoData);
		UpdateMileage(bingoData);
		UpdateBottomButtons(bingoData);
		SetSlotSpecialMode(m_specialMode);
		SetSlotSelectFx(m_hsSpecialIndex);
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
			Debug.LogError("bingo - Ȯ\ufffd\ufffd\ufffd\u0331\ufffd \ufffd\ufffd尡 \ufffdƴѵ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd?");
			return;
		}
		if (m_hsSpecialIndex.Contains(index))
		{
			m_hsSpecialIndex.Remove(index);
		}
		else
		{
			m_hsSpecialIndex.Add(index);
		}
		EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		UpdateMileage(bingoData);
		SetSlotSelectFx(m_hsSpecialIndex);
	}

	private void OnSpecialConfirm()
	{
		EventBingo bingoData = NKMEventManager.GetBingoData(m_bingoTemplet.m_EventID);
		if (bingoData == null)
		{
			return;
		}
		int num = m_bingoTemplet.m_BingoSpecialTryRequireCnt * m_hsSpecialIndex.Count;
		if (bingoData.m_bingoInfo.mileage >= num)
		{
			string content = string.Format(NKCStringTable.GetString("SI_DP_EVENT_BINGO_GET_QUESTION_NUMBER_SELECT_POPUP_V2"), num, m_hsSpecialIndex.Count);
			string strPoint = string.Format(NKCUtilString.GET_STRING_EVENT_BINGO_REMAIN_MILEAGE, bingoData.m_bingoInfo.mileage);
			NKCPopupResourceTextConfirmBox.Instance.Open(NKCUtilString.GET_STRING_NOTICE, content, strPoint, delegate
			{
				NKCPacketSender.Send_NKMPacket_EVENT_BINGO_INDEX_MARK_REQ(m_bingoTemplet.m_EventID, m_hsSpecialIndex);
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_EVENT_BINGO_LACK_POINT_POPUP"));
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

	private void SetCompleteRewardData(EventBingo bingoData)
	{
		if (bingoData == null)
		{
			return;
		}
		List<NKMEventBingoRewardTemplet> bingoRewardTempletList = NKMEventManager.GetBingoRewardTempletList(m_bingoTemplet.m_EventID);
		if (bingoRewardTempletList == null)
		{
			return;
		}
		bingoRewardTempletList = bingoRewardTempletList.FindAll((NKMEventBingoRewardTemplet v) => v.m_BingoCompletType == BingoCompleteType.LINE_SET);
		int count = bingoData.GetBingoLine().Count;
		int num = bingoRewardTempletList.Count - 1;
		int bingoCompletTypeValue = bingoRewardTempletList[num].m_BingoCompletTypeValue;
		for (int num2 = 0; num2 < m_lstCompleteReward.Count; num2++)
		{
			NKCUIEventSubUIBingoV2CompleteRewardSlot nKCUIEventSubUIBingoV2CompleteRewardSlot = m_lstCompleteReward[num2];
			if (num2 < bingoRewardTempletList.Count)
			{
				NKMEventBingoRewardTemplet rewardTemplet = bingoRewardTempletList[num2];
				nKCUIEventSubUIBingoV2CompleteRewardSlot.SetData(rewardTemplet, bingoData, num2 == num);
				NKCUtil.SetGameobjectActive(nKCUIEventSubUIBingoV2CompleteRewardSlot, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUIEventSubUIBingoV2CompleteRewardSlot, bValue: false);
			}
		}
		NKCUtil.SetLabelText(m_lbCompleteLineCount, NKCStringTable.GetString("SI_PF_EVENT_BINGO_LINE_REWARD", count, bingoCompletTypeValue));
	}

	public void GetTile(int tileIndex, bool bRandom)
	{
		if (!bRandom)
		{
			SetSlotGetFx(tileIndex);
			m_specialMode = false;
			m_hsSpecialIndex.Clear();
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
			}
		}
	}
}
