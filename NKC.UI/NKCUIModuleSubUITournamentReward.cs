using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Logging;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIModuleSubUITournamentReward : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "UI_SINGLE_TOURNAMENT";

	private const string UI_ASSET_NAME = "UI_SINGLE_POPUP_TOURNAMENT_REWARD";

	private static NKCUIModuleSubUITournamentReward m_Instance;

	public Text m_lbDesc;

	public Text m_lbRankRewardDesc;

	public List<NKCUISlot> m_lstRankRewardSlot;

	public GameObject m_objRankReward;

	public Text m_lbPredictRewardDesc;

	public List<NKCUISlot> m_lstPredictRewardSlot;

	public GameObject m_objPredictReward;

	public NKCUIComStateButton m_btnOK;

	public static NKCUIModuleSubUITournamentReward Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIModuleSubUITournamentReward>("UI_SINGLE_TOURNAMENT", "UI_SINGLE_POPUP_TOURNAMENT_REWARD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIModuleSubUITournamentReward>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static bool isOpen()
	{
		if (m_Instance != null)
		{
			return m_Instance.IsOpen;
		}
		return false;
	}

	private void InitUI()
	{
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(base.Close);
		for (int i = 0; i < m_lstRankRewardSlot.Count; i++)
		{
			m_lstRankRewardSlot[i].Init();
		}
		for (int j = 0; j < m_lstPredictRewardSlot.Count; j++)
		{
			m_lstPredictRewardSlot[j].Init();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(string desc, int hitCount, NKMRewardData predictRewardData, int winCount, NKMRewardData rankRewardData, NKMTournamentGroups groupIndex)
	{
		if (desc != null)
		{
			NKCUtil.SetLabelText(m_lbDesc, desc);
		}
		NKCUtil.SetLabelText(m_lbPredictRewardDesc, string.Format(NKCUtilString.GET_STRING_TOURNAMENT_REWARD_PREDICT_DESC, hitCount));
		switch (groupIndex)
		{
		case NKMTournamentGroups.None:
			NKCUtil.SetLabelText(m_lbRankRewardDesc, NKCUtilString.GET_STRING_TOURNAMENT_REWARD_RANK_DESC_QUALIFY);
			break;
		case NKMTournamentGroups.GroupA:
		case NKMTournamentGroups.GroupB:
		case NKMTournamentGroups.GroupC:
		case NKMTournamentGroups.GroupD:
		case NKMTournamentGroups.GlobalGroupA:
		case NKMTournamentGroups.GlobalGroupB:
		case NKMTournamentGroups.GlobalGroupC:
		case NKMTournamentGroups.GlobalGroupD:
			NKCUtil.SetLabelText(m_lbRankRewardDesc, GetRoundByWinCount(winCount));
			break;
		case NKMTournamentGroups.Finals:
		case NKMTournamentGroups.GlobalFinals:
			NKCUtil.SetLabelText(m_lbRankRewardDesc, string.Format(NKCUtilString.GET_STRING_TOURNAMENT_REWARD_RANK_DESC_FINAL, winCount));
			break;
		}
		List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(predictRewardData);
		if (list.Count > 0)
		{
			NKCUtil.SetGameobjectActive(m_objPredictReward, bValue: true);
			for (int i = 0; i < m_lstPredictRewardSlot.Count; i++)
			{
				if (i < list.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstPredictRewardSlot[i], bValue: true);
					m_lstPredictRewardSlot[i].SetData(list[i]);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstPredictRewardSlot[i], bValue: false);
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objPredictReward, bValue: false);
		}
		List<NKCUISlot.SlotData> list2 = NKCUISlot.MakeSlotDataListFromReward(rankRewardData);
		if (list2.Count > 0)
		{
			NKCUtil.SetGameobjectActive(m_objRankReward, bValue: true);
			for (int j = 0; j < m_lstRankRewardSlot.Count; j++)
			{
				if (j < list2.Count)
				{
					NKCUtil.SetGameobjectActive(m_lstRankRewardSlot[j], bValue: true);
					m_lstRankRewardSlot[j].SetData(list2[j]);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstRankRewardSlot[j], bValue: false);
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRankReward, bValue: false);
		}
		if (list.Count == 0 && list2.Count == 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
		else
		{
			UIOpened();
		}
	}

	private string GetRoundByWinCount(int winCount)
	{
		switch (winCount)
		{
		case 0:
			return NKCUtilString.GET_STRING_TOURNAMENT_REWARD_RANK_DESC_QUALIFY;
		case 1:
			return NKCUtilString.GET_STRING_TOURNAMENT_REWARD_RANK_DESC_GROUP_32;
		case 2:
			return NKCUtilString.GET_STRING_TOURNAMENT_REWARD_RANK_DESC_GROUP_16;
		case 3:
			return NKCUtilString.GET_STRING_TOURNAMENT_REWARD_RANK_DESC_GROUP_8;
		default:
			Log.Error($"\ufffd\u00b8\ufffdȽ\ufffd\ufffd : {winCount}, \ufffd\uec2d?", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCUIModuleSubUITournamentReward.cs", 186);
			return "";
		}
	}
}
