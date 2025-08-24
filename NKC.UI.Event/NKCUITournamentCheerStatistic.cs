using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Game;
using Cs.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUITournamentCheerStatistic : NKCUIBase
{
	private struct StatisticInfoGroup
	{
		public NKMTournamentProfileData profileData;

		public float percent;

		public int index;
	}

	public const string UI_ASSET_BUNDLE_NAME = "UI_SINGLE_TOURNAMENT";

	public const string UI_ASSET_NAME = "UI_SINGLE_POPUP_TOURNAMENT_CHEERING";

	private static NKCUITournamentCheerStatistic m_Instance;

	public GameObject m_prefabSlot;

	public GameObject m_objNone;

	public TMP_Text m_groupName;

	public ScrollRect m_scrollRect;

	public NKCUIComStateButton m_btnClose;

	public static NKCUITournamentCheerStatistic Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUITournamentCheerStatistic>("UI_SINGLE_TOURNAMENT", "UI_SINGLE_POPUP_TOURNAMENT_CHEERING", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanUpInstance).GetInstance<NKCUITournamentCheerStatistic>();
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

	public override string MenuName => "TOURNAMENT CHEER STATISTIC";

	public override eMenutype eUIType => eMenutype.Popup;

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
		NKCUtil.SetButtonClickDelegate(m_btnClose, base.Close);
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMTournamentPredictionStatistics statistic, NKMTournamentGroups group)
	{
		base.gameObject.SetActive(value: true);
		switch (group)
		{
		case NKMTournamentGroups.GroupA:
		case NKMTournamentGroups.GlobalGroupA:
			NKCUtil.SetLabelText(m_groupName, NKCStringTable.GetString("SI_PF_TOURNAMENT_GROUP_A_TITLE"));
			break;
		case NKMTournamentGroups.GroupB:
		case NKMTournamentGroups.GlobalGroupB:
			NKCUtil.SetLabelText(m_groupName, NKCStringTable.GetString("SI_PF_TOURNAMENT_GROUP_B_TITLE"));
			break;
		case NKMTournamentGroups.GroupC:
		case NKMTournamentGroups.GlobalGroupC:
			NKCUtil.SetLabelText(m_groupName, NKCStringTable.GetString("SI_PF_TOURNAMENT_GROUP_C_TITLE"));
			break;
		case NKMTournamentGroups.GroupD:
		case NKMTournamentGroups.GlobalGroupD:
			NKCUtil.SetLabelText(m_groupName, NKCStringTable.GetString("SI_PF_TOURNAMENT_GROUP_D_TITLE"));
			break;
		case NKMTournamentGroups.Finals:
		case NKMTournamentGroups.GlobalFinals:
			NKCUtil.SetLabelText(m_groupName, NKCStringTable.GetString("SI_PF_TOURNAMENT_GROUP_FINAL"));
			break;
		default:
			NKCUtil.SetLabelText(m_groupName, "");
			break;
		}
		Dictionary<long, StatisticInfoGroup> dictionary = new Dictionary<long, StatisticInfoGroup>();
		if (statistic != null)
		{
			NKMTournamentInfo tournamentInfoPredict = NKCTournamentManager.GetTournamentInfoPredict(group);
			int num = 0;
			StatisticInfoGroup value = default(StatisticInfoGroup);
			foreach (KeyValuePair<long, float> kvPair in statistic.predictionStatistics)
			{
				if (kvPair.Key <= 0)
				{
					continue;
				}
				NKMTournamentProfileData nKMTournamentProfileData = statistic.userInfo.Find((NKMTournamentProfileData e) => e.commonProfile.userUid == kvPair.Key);
				if (nKMTournamentProfileData == null)
				{
					Log.Error($"UId: {kvPair.Key} ProfileData not exist in NKMTournamentPredictionStatistics.userInfo", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Tournament/NKCUITournamentCheerStatistic.cs", 125);
					continue;
				}
				value.profileData = nKMTournamentProfileData;
				value.percent = Mathf.Round(kvPair.Value * 10f) * 0.1f;
				if (tournamentInfoPredict != null)
				{
					value.index = tournamentInfoPredict.slotUserUid.FindIndex((long e) => e == kvPair.Key);
				}
				else
				{
					value.index = num;
				}
				dictionary.Add(kvPair.Key, value);
				num++;
			}
		}
		if (dictionary.Count > 0)
		{
			NKCUtil.SetGameobjectActive(m_scrollRect, bValue: true);
			NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
			List<KeyValuePair<long, StatisticInfoGroup>> list = (from e in dictionary
				orderby e.Value.percent descending, e.Value.index
				select e).ToList();
			int num2 = list.Count();
			int childCount = m_scrollRect.content.childCount;
			for (int num3 = 0; num3 < num2 - childCount; num3++)
			{
				Object.Instantiate(m_prefabSlot, m_scrollRect.content);
			}
			childCount = m_scrollRect.content.childCount;
			int num4 = 0;
			for (int num5 = 0; num5 < childCount; num5++)
			{
				Transform child = m_scrollRect.content.GetChild(num5);
				if (child == null)
				{
					continue;
				}
				if (num5 < num2)
				{
					child.gameObject.SetActive(value: true);
					NKCUITournamentCheerStatisticSlot component = child.GetComponent<NKCUITournamentCheerStatisticSlot>();
					if (num5 == 0 || list[num5 - 1].Value.percent != list[num5].Value.percent)
					{
						num4 = num5;
					}
					component?.SetData(list[num5].Value.profileData, list[num5].Value.percent, num4 + 1);
				}
				else
				{
					child.gameObject.SetActive(value: false);
				}
			}
			m_scrollRect.verticalNormalizedPosition = 1f;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_scrollRect, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNone, bValue: true);
		}
		UIOpened();
	}
}
