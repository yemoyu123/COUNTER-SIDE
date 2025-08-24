using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIModuleSubUITournamentRank : NKCUIBase
{
	public NKCUITournamentRankSlot m_pfbSlot;

	public LoopScrollRect m_loop;

	public TMP_Text m_lbTitle;

	public NKCUIComStateButton m_btnClose;

	private Stack<NKCUITournamentRankSlot> m_stkSlot = new Stack<NKCUITournamentRankSlot>();

	private List<NKCUITournamentRankSlot> m_lstVisible = new List<NKCUITournamentRankSlot>();

	private List<List<LeaderBoardSlotData>> m_lstSlotData = new List<List<LeaderBoardSlotData>>();

	private LeaderBoardType m_LeaderBoardType = LeaderBoardType.BT_NONE;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static NKCUIModuleSubUITournamentRank OpenInstance(string bundleName, string assetName)
	{
		NKCUIModuleSubUITournamentRank instance = NKCUIManager.OpenNewInstance<NKCUIModuleSubUITournamentRank>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCUIModuleSubUITournamentRank>();
		if ((object)instance != null)
		{
			instance.InitUI();
			return instance;
		}
		return instance;
	}

	private void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(List<List<LeaderBoardSlotData>> lstData, LeaderBoardType leaderBoardType)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_LeaderBoardType = leaderBoardType;
		m_lstSlotData = lstData;
		m_loop.TotalCount = m_lstSlotData.Count;
		m_loop.RefreshCells();
		UIOpened();
	}

	private RectTransform GetObject(int idx)
	{
		NKCUITournamentRankSlot nKCUITournamentRankSlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCUITournamentRankSlot = m_stkSlot.Pop();
		}
		else
		{
			nKCUITournamentRankSlot = Object.Instantiate(m_pfbSlot, m_loop.content);
			nKCUITournamentRankSlot.Init();
		}
		m_lstVisible.Add(nKCUITournamentRankSlot);
		NKCUtil.SetGameobjectActive(nKCUITournamentRankSlot, bValue: false);
		return nKCUITournamentRankSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUITournamentRankSlot component = tr.GetComponent<NKCUITournamentRankSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		m_lstVisible.Remove(component);
		m_stkSlot.Push(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUITournamentRankSlot component = tr.GetComponent<NKCUITournamentRankSlot>();
		if (m_lstSlotData[idx].Count < 3)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
			return;
		}
		component.SetRankSlotType(m_LeaderBoardType);
		if (m_LeaderBoardType == LeaderBoardType.BT_TOURNAMENT)
		{
			NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(m_lstSlotData[idx][0].memberCount);
			if (nKMTournamentTemplet == null)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetData(nKMTournamentTemplet.GetTournamentSeasonTitle(), m_lstSlotData[idx][0], m_lstSlotData[idx][1], m_lstSlotData[idx][2], 0, null);
		}
		else if (m_LeaderBoardType == LeaderBoardType.BT_LEAGUE || m_LeaderBoardType == LeaderBoardType.BT_UNLIMITED)
		{
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(m_lstSlotData[idx][0].memberCount);
			if (nKMLeaguePvpRankSeasonTemplet == null)
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetData(nKMLeaguePvpRankSeasonTemplet.GetSeasonStrId(), m_lstSlotData[idx][0], m_lstSlotData[idx][1], m_lstSlotData[idx][2], 0, null);
		}
		else
		{
			Log.Error($"{m_LeaderBoardType} Ÿ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdʿ\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCUIModuleSubUITournamentRank.cs", 141);
		}
	}
}
