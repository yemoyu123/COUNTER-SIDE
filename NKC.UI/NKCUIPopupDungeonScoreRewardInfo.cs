using System.Collections.Generic;
using System.Linq;
using NKC.UI.Fierce;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupDungeonScoreRewardInfo : NKCUIBase
{
	private class PointRewardCompare : IComparer<NKMDefenceScoreRewardTemplet>
	{
		public int Compare(NKMDefenceScoreRewardTemplet x, NKMDefenceScoreRewardTemplet y)
		{
			return x.Step.CompareTo(y.Step);
		}
	}

	private const string ASSET_BUNDLE_NAME = "UI_EVENT_MD_POPUP_SINGLE_DEF_2";

	private const string UI_ASSET_NAME = "UI_EVENT_MD_POPUP_SIGNLE_DEF_2_REWARD_SCORE";

	private static NKCUIPopupDungeonScoreRewardInfo m_Instance;

	public EventTrigger m_BgEventTrigger;

	public LoopScrollRect m_loop;

	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSE_BUTTON;

	public Text m_lbTotalScore;

	public Text m_lbMyRankPercent;

	public NKCUIComStateButton m_csbtnAllReceiveReward;

	public RectTransform m_Content;

	public string m_SLOT_BUNDLE_NAME = "UI_EVENT_MD_POPUP_SINGLE_DEF_2";

	public string m_SLOT_ASSET_NAME = "UI_EVENT_MD_POPUP_SIGNLE_DEF_2_REWARD_SCORE_SLOT";

	private Stack<NKCUIPopupFierceBattleScoreRewardSlot> m_stkScoreRewardSlot = new Stack<NKCUIPopupFierceBattleScoreRewardSlot>();

	private List<NKCUIPopupFierceBattleScoreRewardSlot> m_lstScoreRewardSlot = new List<NKCUIPopupFierceBattleScoreRewardSlot>();

	private List<NKMDefenceScoreRewardTemplet> m_lstPointRewardTemplet = new List<NKMDefenceScoreRewardTemplet>();

	private int m_iTotalPoint;

	public static NKCUIPopupDungeonScoreRewardInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupDungeonScoreRewardInfo>("UI_EVENT_MD_POPUP_SINGLE_DEF_2", "UI_EVENT_MD_POPUP_SIGNLE_DEF_2_REWARD_SCORE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupDungeonScoreRewardInfo>();
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

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance.Clear();
		m_Instance = null;
	}

	public virtual void CheckInstanceAndClose()
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

	private void Init()
	{
		if (m_BgEventTrigger != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_BgEventTrigger.triggers.Add(entry);
		}
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_CLOSE_BUTTON, CheckInstanceAndClose);
		if (m_loop != null)
		{
			m_loop.dOnGetObject += GetSlot;
			m_loop.dOnReturnObject += ReturnSlot;
			m_loop.dOnProvideData += ProvideData;
			m_loop.PrepareCells();
			NKCUtil.SetScrollHotKey(m_loop);
		}
		NKCUtil.SetBindFunction(m_csbtnAllReceiveReward, OnClickRecevieAllReward);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Clear()
	{
	}

	public void Open()
	{
		int scoreRewardGroupID = NKMDefenceTemplet.Find(NKCDefenceDungeonManager.m_DefenceTempletId).m_ScoreRewardGroupID;
		if (NKMDefenceScoreRewardTemplet.Groups.ContainsKey(scoreRewardGroupID))
		{
			m_lstPointRewardTemplet = NKMDefenceScoreRewardTemplet.Groups[scoreRewardGroupID].ToList();
			m_lstPointRewardTemplet.Sort(new PointRewardCompare());
		}
		m_iTotalPoint = NKCDefenceDungeonManager.m_BestClearScore;
		NKCUtil.SetLabelText(m_lbTotalScore, m_iTotalPoint.ToString());
		NKCUtil.SetLabelText(m_lbMyRankPercent, NKCDefenceDungeonManager.GetRankingTotalDesc());
		SetData();
		UIOpened();
	}

	public void SetData()
	{
		m_loop.TotalCount = m_lstPointRewardTemplet.Count;
		m_loop.SetIndexPosition(0);
		m_loop.RefreshCells(bForce: true);
		if (!NKCDefenceDungeonManager.IsCanReceivePointReward())
		{
			m_csbtnAllReceiveReward.Lock();
		}
		else
		{
			m_csbtnAllReceiveReward.UnLock();
		}
	}

	public void Refresh()
	{
		SetData();
	}

	public RectTransform GetSlot(int index)
	{
		if (m_stkScoreRewardSlot.Count > 0)
		{
			NKCUIPopupFierceBattleScoreRewardSlot nKCUIPopupFierceBattleScoreRewardSlot = m_stkScoreRewardSlot.Pop();
			if (nKCUIPopupFierceBattleScoreRewardSlot != null)
			{
				return nKCUIPopupFierceBattleScoreRewardSlot.GetComponent<RectTransform>();
			}
		}
		NKCUIPopupFierceBattleScoreRewardSlot newInstance = NKCUIPopupFierceBattleScoreRewardSlot.GetNewInstance(m_SLOT_BUNDLE_NAME, m_SLOT_ASSET_NAME, m_Content, OnClickRewardSlot);
		if (newInstance != null)
		{
			m_lstScoreRewardSlot.Add(newInstance);
			return newInstance.GetComponent<RectTransform>();
		}
		return null;
	}

	public void ReturnSlot(Transform tr)
	{
		NKCUIPopupFierceBattleScoreRewardSlot component = tr.GetComponent<NKCUIPopupFierceBattleScoreRewardSlot>();
		tr.SetParent(base.transform);
		m_stkScoreRewardSlot.Push(component);
	}

	public void ProvideData(Transform tr, int index)
	{
		if (index > m_lstPointRewardTemplet.Count)
		{
			return;
		}
		NKCUIPopupFierceBattleScoreRewardSlot component = tr.GetComponent<NKCUIPopupFierceBattleScoreRewardSlot>();
		if (component != null)
		{
			NKCUIPopupFierceBattleScoreRewardSlot.POINT_REWARD_SLOT_TYPE type = (NKCDefenceDungeonManager.IsReceivedPointReward(m_lstPointRewardTemplet[index].DefenceScoreRewardID) ? NKCUIPopupFierceBattleScoreRewardSlot.POINT_REWARD_SLOT_TYPE.COMPLETE : ((m_lstPointRewardTemplet[index].Score <= m_iTotalPoint) ? NKCUIPopupFierceBattleScoreRewardSlot.POINT_REWARD_SLOT_TYPE.CAN_RECEVIE : NKCUIPopupFierceBattleScoreRewardSlot.POINT_REWARD_SLOT_TYPE.DISABLE));
			List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
			for (int i = 0; i < m_lstPointRewardTemplet[index].Rewards.Count; i++)
			{
				NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeRewardTypeData(m_lstPointRewardTemplet[index].Rewards[i].rewardType, m_lstPointRewardTemplet[index].Rewards[i].ID, m_lstPointRewardTemplet[index].Rewards[i].Count);
				list.Add(item);
			}
			component.SetData(m_lstPointRewardTemplet[index].DefenceScoreRewardID, m_lstPointRewardTemplet[index].ScoreDescStrID, list, type);
		}
	}

	private void OnClickRewardSlot(int rewardId)
	{
		NKCPacketSender.Send_NKMPacket_DEFENCE_SCORE_REWARD_REQ(rewardId);
	}

	private void OnClickRecevieAllReward()
	{
		if (NKCDefenceDungeonManager.IsCanReceivePointReward())
		{
			NKCPacketSender.Send_NKMPacket_DEFENCE_SCORE_REWARD_ALL_REQ();
		}
	}
}
