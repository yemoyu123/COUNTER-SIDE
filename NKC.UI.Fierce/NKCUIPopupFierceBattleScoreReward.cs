using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIPopupFierceBattleScoreReward : NKCUIBase
{
	private class PointRewardCompare : IComparer<NKMFiercePointRewardTemplet>
	{
		public int Compare(NKMFiercePointRewardTemplet x, NKMFiercePointRewardTemplet y)
		{
			return x.Step.CompareTo(y.Step);
		}
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_fierce_battle";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FIERCE_BATTLE_TOTAL_SCORE_REWARD";

	private static NKCUIPopupFierceBattleScoreReward m_Instance;

	public EventTrigger m_POPUP_FIERCE_BATTLE_TOTAL_SCORE_REWARD_Bg;

	public LoopScrollRect m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect;

	public NKCUIComStateButton m_NKM_UI_POPUP_CLOSE_BUTTON;

	public Text m_POPUP_FIERCE_BATTLE_TOTAL_SCORE_REWARD_MYSCORE_TEXT_1;

	public Text m_POPUP_FIERCE_BATTLE_TOTAL_SCORE_REWARD_MYSCORE_TEXT_2;

	private Color m_AllReceiveRewardOriginalColor;

	public Image m_imgAllReceiveReward;

	public Text m_lbAllReceiveReward;

	public NKCUIComStateButton m_csbtnAllReceiveReward;

	private List<NKMFiercePointRewardTemplet> m_lstPointReward;

	private NKCFierceBattleSupportDataMgr m_FierceMgr;

	private int m_iTotalPoint;

	public RectTransform m_Content;

	private Stack<NKCUIPopupFierceBattleScoreRewardSlot> m_stkScoreRewardSlot = new Stack<NKCUIPopupFierceBattleScoreRewardSlot>();

	private List<NKCUIPopupFierceBattleScoreRewardSlot> m_lstScoreRewardSlot = new List<NKCUIPopupFierceBattleScoreRewardSlot>();

	public static NKCUIPopupFierceBattleScoreReward Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupFierceBattleScoreReward>("ab_ui_nkm_ui_fierce_battle", "NKM_UI_POPUP_FIERCE_BATTLE_TOTAL_SCORE_REWARD", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupFierceBattleScoreReward>();
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

	public override string MenuName => "FIERCE_BATTLE_TOTAL_SCORE_REWARD";

	private static void CleanupInstance()
	{
		m_Instance.Clear();
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
		if (m_POPUP_FIERCE_BATTLE_TOTAL_SCORE_REWARD_Bg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_POPUP_FIERCE_BATTLE_TOTAL_SCORE_REWARD_Bg.triggers.Add(entry);
		}
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_CLOSE_BUTTON, CheckInstanceAndClose);
		if (m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect != null)
		{
			m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect.dOnGetObject += GetSlot;
			m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect.dOnReturnObject += ReturnSlot;
			m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect.dOnProvideData += ProvideData;
			m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect.PrepareCells();
			NKCUtil.SetScrollHotKey(m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect);
		}
		if (m_lbAllReceiveReward != null)
		{
			m_AllReceiveRewardOriginalColor = m_lbAllReceiveReward.color;
		}
		NKCUtil.SetBindFunction(m_csbtnAllReceiveReward, OnClickRecevieAllReward);
	}

	public void Open()
	{
		m_FierceMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		m_iTotalPoint = m_FierceMgr.GetTotalPoint();
		int pointRewardGroupID = m_FierceMgr.FierceTemplet.PointRewardGroupID;
		if (NKMFiercePointRewardTemplet.Groups.ContainsKey(pointRewardGroupID))
		{
			m_lstPointReward = NKMFiercePointRewardTemplet.Groups[pointRewardGroupID].ToList();
			m_lstPointReward.Sort(new PointRewardCompare());
		}
		m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect.TotalCount = m_lstPointReward.Count;
		m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect.SetIndexPosition(0);
		m_POPUP_FIERCE_BATTLE_RANK_REWARD_SLOT_ScrollRect.RefreshCells(bForce: true);
		NKCUtil.SetLabelText(m_POPUP_FIERCE_BATTLE_TOTAL_SCORE_REWARD_MYSCORE_TEXT_1, m_FierceMgr.GetTotalPoint().ToString("#,##0"));
		NKCUtil.SetLabelText(m_POPUP_FIERCE_BATTLE_TOTAL_SCORE_REWARD_MYSCORE_TEXT_2, m_FierceMgr.GetRankingTotalDesc());
		if (!m_FierceMgr.IsCanReceivePointReward())
		{
			NKCUtil.SetImageSprite(m_imgAllReceiveReward, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			NKCUtil.SetLabelTextColor(m_lbAllReceiveReward, NKCUtil.GetColor("#212122"));
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgAllReceiveReward, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW));
			NKCUtil.SetLabelTextColor(m_lbAllReceiveReward, m_AllReceiveRewardOriginalColor);
		}
		UIOpened();
	}

	private void OnClickRewardSlot(int rewardId)
	{
		NKCPacketSender.Send_NKMPacket_FIERCE_COMPLETE_POINT_REWARD_REQ(rewardId);
	}

	private void OnClickRecevieAllReward()
	{
		if (m_FierceMgr.IsCanReceivePointReward())
		{
			NKCPacketSender.Send_NKMPacket_FIERCE_COMPLETE_POINT_REWARD_ALL_REQ();
		}
	}

	private void Clear()
	{
		for (int i = 0; i < m_lstScoreRewardSlot.Count; i++)
		{
			m_lstScoreRewardSlot[i].DestoryInstance();
		}
		m_lstScoreRewardSlot.Clear();
		while (m_stkScoreRewardSlot.Count > 0)
		{
			NKCUIPopupFierceBattleScoreRewardSlot nKCUIPopupFierceBattleScoreRewardSlot = m_stkScoreRewardSlot.Pop();
			if (nKCUIPopupFierceBattleScoreRewardSlot != null)
			{
				nKCUIPopupFierceBattleScoreRewardSlot.DestoryInstance();
			}
		}
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
		NKCUIPopupFierceBattleScoreRewardSlot newInstance = NKCUIPopupFierceBattleScoreRewardSlot.GetNewInstance(m_Content, OnClickRewardSlot);
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
		if (index <= m_lstPointReward.Count)
		{
			NKCUIPopupFierceBattleScoreRewardSlot component = tr.GetComponent<NKCUIPopupFierceBattleScoreRewardSlot>();
			if (component != null)
			{
				component.SetData(type: m_FierceMgr.IsReceivedPointReward(m_lstPointReward[index].FiercePointRewardID) ? NKCUIPopupFierceBattleScoreRewardSlot.POINT_REWARD_SLOT_TYPE.COMPLETE : ((m_lstPointReward[index].Point <= m_iTotalPoint) ? NKCUIPopupFierceBattleScoreRewardSlot.POINT_REWARD_SLOT_TYPE.CAN_RECEVIE : NKCUIPopupFierceBattleScoreRewardSlot.POINT_REWARD_SLOT_TYPE.DISABLE), templet: m_lstPointReward[index]);
			}
		}
	}
}
