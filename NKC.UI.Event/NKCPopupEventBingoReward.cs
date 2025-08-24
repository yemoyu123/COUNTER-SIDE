using System.Collections.Generic;
using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventBingoReward : NKCUIBase
{
	public delegate void OnComplete(int reward_index);

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_EVENT_BINGO_REWARD";

	private static NKCPopupEventBingoReward m_Instance;

	public NKCUIComStateButton m_close;

	public NKCUIComButton m_back;

	[Header("최종보상")]
	public NKCUISlot m_rewardSlot;

	public Text m_txtRewardName;

	[Header("리스트")]
	public List<NKCPopupEventBingoRewardSlot> m_listSlot;

	private int m_eventID;

	private OnComplete dOnComplete;

	public static NKCPopupEventBingoReward Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventBingoReward>("AB_UI_NKM_UI_EVENT", "NKM_UI_POPUP_EVENT_BINGO_REWARD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventBingoReward>();
				m_Instance.Init();
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

	public override string MenuName => "Bingo Reward";

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

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		if (m_close != null)
		{
			m_close.PointerClick.RemoveAllListeners();
			m_close.PointerClick.AddListener(base.Close);
		}
		if (m_back != null)
		{
			m_back.PointerClick.RemoveAllListeners();
			m_back.PointerClick.AddListener(base.Close);
		}
		if (m_listSlot != null)
		{
			for (int i = 0; i < m_listSlot.Count; i++)
			{
				m_listSlot[i].Init(OnTouchComplete);
			}
		}
		if (m_rewardSlot != null)
		{
			m_rewardSlot.Init();
		}
	}

	public void Open(int eventID, OnComplete onComplete)
	{
		m_eventID = eventID;
		dOnComplete = onComplete;
		SetData(m_eventID);
		UIOpened();
	}

	public void Refresh()
	{
		SetData(m_eventID);
	}

	private void SetData(int eventID)
	{
		List<NKMEventBingoRewardTemplet> bingoRewardTempletList = NKMEventManager.GetBingoRewardTempletList(eventID);
		if (bingoRewardTempletList == null)
		{
			return;
		}
		EventBingo bingoData = NKMEventManager.GetBingoData(eventID);
		if (bingoData == null)
		{
			return;
		}
		bingoRewardTempletList = bingoRewardTempletList.FindAll((NKMEventBingoRewardTemplet v) => v.m_BingoCompletType == BingoCompleteType.LINE_SET);
		int count = bingoData.GetBingoLine().Count;
		if (m_listSlot != null)
		{
			for (int num = 0; num < m_listSlot.Count; num++)
			{
				NKCPopupEventBingoRewardSlot nKCPopupEventBingoRewardSlot = m_listSlot[num];
				if (num < bingoRewardTempletList.Count)
				{
					NKMEventBingoRewardTemplet nKMEventBingoRewardTemplet = bingoRewardTempletList[num];
					nKCPopupEventBingoRewardSlot.SetData(nKMEventBingoRewardTemplet, count, bingoData.m_bingoInfo.rewardList.Contains(nKMEventBingoRewardTemplet.ZeroBaseTileIndex));
					NKCUtil.SetGameobjectActive(nKCPopupEventBingoRewardSlot, bValue: true);
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCPopupEventBingoRewardSlot, bValue: false);
				}
			}
		}
		if (m_rewardSlot != null)
		{
			NKMEventBingoRewardTemplet bingoLastRewardTemplet = NKMEventManager.GetBingoLastRewardTemplet(eventID);
			if (bingoLastRewardTemplet != null)
			{
				NKMRewardInfo nKMRewardInfo = bingoLastRewardTemplet.rewards[0];
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(nKMRewardInfo.rewardType, nKMRewardInfo.ID, nKMRewardInfo.Count);
				m_rewardSlot.SetData(data);
				bool flag = bingoData.m_bingoInfo.rewardList.Contains(bingoLastRewardTemplet.ZeroBaseTileIndex);
				m_rewardSlot.SetEventGet(flag);
				m_rewardSlot.SetDisable(flag);
				NKCUtil.SetLabelText(m_txtRewardName, m_rewardSlot.GetName());
				NKCUtil.SetGameobjectActive(m_rewardSlot, bValue: true);
			}
			else
			{
				NKCUtil.SetLabelText(m_txtRewardName, "");
				NKCUtil.SetGameobjectActive(m_rewardSlot, bValue: false);
			}
		}
	}

	private void OnTouchComplete(NKMEventBingoRewardTemplet rewardTemplet)
	{
		if (rewardTemplet != null)
		{
			dOnComplete?.Invoke(rewardTemplet.ZeroBaseTileIndex);
		}
	}
}
