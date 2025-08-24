using System.Collections.Generic;
using Cs.Logging;
using NKC.UI.Fierce;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupRewardInfoToggle : NKCUIPopupFierceBattleRewardInfo
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_LEADER_BOARD_DETAIL";

	private const string UI_ASSET_NAME = "AB_UI_LEADER_BOARD_DETAIL_REWARD_INFO_TOGGLE";

	private static NKCPopupRewardInfoToggle m_Instance;

	public List<NKCUIComToggle> m_lstToggle;

	private Dictionary<NKCUIComToggle, int> m_dicToggle = new Dictionary<NKCUIComToggle, int>();

	private Dictionary<int, List<RankUIRewardData>> m_dicData = new Dictionary<int, List<RankUIRewardData>>();

	private List<string> m_lstTitleStrID = new List<string>();

	private List<string> m_lstDescStrID = new List<string>();

	private bool m_bShowTitle = true;

	private bool m_bShowDesc = true;

	private bool m_bShowMyRank = true;

	public new static NKCPopupRewardInfoToggle Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupRewardInfoToggle>("AB_UI_LEADER_BOARD_DETAIL", "AB_UI_LEADER_BOARD_DETAIL_REWARD_INFO_TOGGLE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupRewardInfoToggle>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public override void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance.Clear();
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
		m_dicToggle.Clear();
		for (int i = 0; i < m_lstToggle.Count; i++)
		{
			BindToggle(m_lstToggle[i], i);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		Canvas.ForceUpdateCanvases();
		Init();
	}

	private void BindToggle(NKCUIComToggle toggle, int idx)
	{
		m_dicToggle.Add(toggle, idx);
		toggle.OnValueChanged.RemoveAllListeners();
		toggle.OnValueChanged.AddListener(delegate(bool bValue)
		{
			OnClickToggle(toggle, bValue);
		});
	}

	public override void CloseInternal()
	{
		m_dicData.Clear();
		m_lstTitleStrID.Clear();
		m_lstDescStrID.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		Clear();
	}

	public void Open(REWARD_SLOT_TYPE rewardSlotType, Dictionary<int, List<RankUIRewardData>> dicData, List<string> lstTabStrID, List<string> lstTitle, List<string> lstDesc, bool bShowTitle = true, bool bShowDesc = true, bool bShowMyRank = true)
	{
		if (dicData.Count != lstTabStrID.Count)
		{
			Log.Error($"\ufffdԷµ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdٸ\ufffd - \ufffd\ufffd\ufffd\ufffd\ufffdͼ\ufffd : {dicData.Count}\ufffd\ufffd, \ufffd\ufffd Ÿ\ufffd\ufffdƲ : {lstTabStrID.Count}\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCPopupRewardInfoToggle.cs", 97);
		}
		m_bPrepared = false;
		m_dicData = dicData;
		m_lstTitleStrID = lstTitle;
		m_lstDescStrID = lstDesc;
		m_bShowTitle = bShowTitle;
		m_bShowDesc = bShowDesc;
		m_bShowMyRank = bShowMyRank;
		m_RewardSlotType = rewardSlotType;
		foreach (KeyValuePair<NKCUIComToggle, int> item in m_dicToggle)
		{
			if (item.Value < lstTabStrID.Count)
			{
				NKCUtil.SetGameobjectActive(item.Key, bValue: true);
				item.Key.SetTitleText(NKCStringTable.GetString(lstTabStrID[item.Value]));
			}
			else
			{
				NKCUtil.SetGameobjectActive(item.Key, bValue: false);
			}
		}
		if (dicData.Count > 0)
		{
			SetData(0);
		}
		UIOpened();
	}

	private void SetData(int idx)
	{
		foreach (KeyValuePair<NKCUIComToggle, int> item in m_dicToggle)
		{
			item.Key.Select(item.Value == idx, bForce: true);
		}
		string title = "";
		if (m_bShowTitle && m_lstTitleStrID.Count > idx)
		{
			title = m_lstTitleStrID[idx];
		}
		string desc = "";
		if (m_bShowDesc && m_lstDescStrID.Count > idx)
		{
			desc = m_lstDescStrID[idx];
		}
		SetData(m_dicData[idx], m_bShowTitle, title, m_bShowDesc, desc, m_bShowMyRank);
	}

	private void OnClickToggle(NKCUIComToggle toggle, bool bValue)
	{
		if (bValue)
		{
			m_dicToggle.TryGetValue(toggle, out var value);
			SetData(value);
		}
	}
}
