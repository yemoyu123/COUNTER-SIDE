using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupScoreReward : NKCUIBase
{
	public delegate void OnClickReward(int rewardId);

	public delegate bool IsReceivedPointReward(int rewardId);

	private class PointRewardCompare : IComparer<NKMScoreRewardTemplet>
	{
		public int Compare(NKMScoreRewardTemplet x, NKMScoreRewardTemplet y)
		{
			return x.m_Step.CompareTo(y.m_Step);
		}
	}

	public EventTrigger m_BgEventTrigger;

	public LoopScrollRect m_loop;

	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSE_BUTTON;

	public Text m_lbTotalScore;

	public Text m_lbMyRankPercent;

	public NKCUIComStateButton m_csbtnAllReceiveReward;

	public RectTransform m_Content;

	public string m_SLOT_BUNDLE_NAME = "UI_SINGLE_MATCHTEN";

	public string m_SLOT_ASSET_NAME = "UI_SINGLE_POPUP_MATCHTEN_REWARD_SLOT";

	public string m_SLOT_BUNDLE_NAME_SWORD = "UI_SINGLE_SWORDTRAINING";

	public string m_SLOT_ASSET_NAME_SWORD = "UI_SINGLE_POPUP_SWORDTRAINING_REWARD_SLOT";

	private OnClickReward m_dOnClickReward;

	private OnClickReward m_dOnClickReceiveAll;

	private IsReceivedPointReward m_dIsReceivedPointReward;

	private Stack<NKCPopupScoreRewardSlot> m_stkScoreRewardSlot = new Stack<NKCPopupScoreRewardSlot>();

	private List<NKCPopupScoreRewardSlot> m_lstScoreRewardSlot = new List<NKCPopupScoreRewardSlot>();

	private List<NKMScoreRewardTemplet> m_lstScoreRewardTemplet = new List<NKMScoreRewardTemplet>();

	private int m_iBestScore;

	private int m_ScoreRewardGroupID;

	private bool m_bSwordGame;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static NKCPopupScoreReward MakeInstance(string bundleName, string assetName)
	{
		NKCPopupScoreReward instance = NKCUIManager.OpenNewInstance<NKCPopupScoreReward>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCPopupScoreReward>();
		instance.InitUI();
		return instance;
	}

	private void InitUI()
	{
		if (m_BgEventTrigger != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				Close();
			});
			m_BgEventTrigger.triggers.Add(entry);
		}
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_CLOSE_BUTTON, base.Close);
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

	public void Open(int scoreRewardGroupID, int myBestScore, string rankDesc, IsReceivedPointReward isReceivedPointReward, OnClickReward onClickReward, OnClickReward onClickReceiveAll, bool bIsSwordGame = false)
	{
		m_dOnClickReward = onClickReward;
		m_dOnClickReceiveAll = onClickReceiveAll;
		m_dIsReceivedPointReward = isReceivedPointReward;
		m_ScoreRewardGroupID = scoreRewardGroupID;
		if (NKMScoreRewardTemplet.Groups.ContainsKey(m_ScoreRewardGroupID))
		{
			m_lstScoreRewardTemplet = NKMScoreRewardTemplet.Groups[m_ScoreRewardGroupID].ToList();
			m_lstScoreRewardTemplet.Sort(new PointRewardCompare());
		}
		m_iBestScore = myBestScore;
		m_bSwordGame = bIsSwordGame;
		NKCUtil.SetLabelText(m_lbTotalScore, m_iBestScore.ToString());
		if (!string.IsNullOrEmpty(rankDesc))
		{
			NKCUtil.SetLabelText(m_lbMyRankPercent, rankDesc);
		}
		SetData();
		UIOpened();
	}

	public void SetData()
	{
		m_loop.TotalCount = m_lstScoreRewardTemplet.Count;
		m_loop.SetIndexPosition(0);
		m_loop.RefreshCells(bForce: true);
		if (!IsCanReceiveReward())
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

	public bool IsCanReceiveReward()
	{
		foreach (NKMScoreRewardTemplet item in m_lstScoreRewardTemplet)
		{
			if (item.m_Score <= m_iBestScore && !m_dIsReceivedPointReward(item.m_ScoreRewardID))
			{
				return true;
			}
		}
		return false;
	}

	public RectTransform GetSlot(int index)
	{
		if (m_stkScoreRewardSlot.Count > 0)
		{
			NKCPopupScoreRewardSlot nKCPopupScoreRewardSlot = m_stkScoreRewardSlot.Pop();
			if (nKCPopupScoreRewardSlot != null)
			{
				return nKCPopupScoreRewardSlot.GetComponent<RectTransform>();
			}
		}
		string bundleName = (m_bSwordGame ? m_SLOT_BUNDLE_NAME_SWORD : m_SLOT_BUNDLE_NAME);
		string assetName = (m_bSwordGame ? m_SLOT_ASSET_NAME_SWORD : m_SLOT_ASSET_NAME);
		NKCPopupScoreRewardSlot newInstance = NKCPopupScoreRewardSlot.GetNewInstance(bundleName, assetName, m_Content, OnClickRewardSlot);
		if (newInstance != null)
		{
			m_lstScoreRewardSlot.Add(newInstance);
			return newInstance.GetComponent<RectTransform>();
		}
		return null;
	}

	public void ReturnSlot(Transform tr)
	{
		NKCPopupScoreRewardSlot component = tr.GetComponent<NKCPopupScoreRewardSlot>();
		tr.SetParent(base.transform);
		m_stkScoreRewardSlot.Push(component);
	}

	public void ProvideData(Transform tr, int index)
	{
		if (index > m_lstScoreRewardTemplet.Count)
		{
			return;
		}
		NKCPopupScoreRewardSlot component = tr.GetComponent<NKCPopupScoreRewardSlot>();
		if (component != null)
		{
			NKCPopupScoreRewardSlot.POINT_REWARD_SLOT_TYPE type = (m_dIsReceivedPointReward(m_lstScoreRewardTemplet[index].m_ScoreRewardID) ? NKCPopupScoreRewardSlot.POINT_REWARD_SLOT_TYPE.COMPLETE : ((m_lstScoreRewardTemplet[index].m_Score <= m_iBestScore) ? NKCPopupScoreRewardSlot.POINT_REWARD_SLOT_TYPE.CAN_RECEVIE : NKCPopupScoreRewardSlot.POINT_REWARD_SLOT_TYPE.DISABLE));
			List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
			for (int i = 0; i < m_lstScoreRewardTemplet[index].m_ScoreReward.Count; i++)
			{
				NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeRewardTypeData(m_lstScoreRewardTemplet[index].m_ScoreReward[i].rewardType, m_lstScoreRewardTemplet[index].m_ScoreReward[i].ID, m_lstScoreRewardTemplet[index].m_ScoreReward[i].Count);
				list.Add(item);
			}
			component.SetData(m_lstScoreRewardTemplet[index].m_ScoreRewardID, m_lstScoreRewardTemplet[index].m_ScoreDescStrID, list, type);
		}
	}

	private void OnClickRewardSlot(int rewardId)
	{
		m_dOnClickReward?.Invoke(rewardId);
	}

	private void OnClickRecevieAllReward()
	{
		m_dOnClickReceiveAll(0);
	}
}
