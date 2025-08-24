using System.Collections.Generic;
using ClientPacket.Guild;
using ClientPacket.LeaderBoard;
using Cs.Logging;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildLobbyRank : MonoBehaviour
{
	private enum RANK_TYPE
	{
		NONE,
		LEVEL,
		COOP
	}

	public NKCUILeaderBoardSlot m_pfbSlot;

	public NKCUIComToggle m_tglLevel;

	public NKCUIComToggle m_tglCoop;

	public NKCUIComStateButton m_btnSeasonSelect;

	public Text m_lbScoreName;

	public LoopScrollRect m_loop;

	public Transform m_trSlotParent;

	public NKCUILeaderBoardSlot m_slotMyGuildRank;

	public GameObject m_objNone;

	private Stack<NKCUILeaderBoardSlot> m_stkSlot = new Stack<NKCUILeaderBoardSlot>();

	private List<LeaderBoardSlotData> m_lstLevelRank = new List<LeaderBoardSlotData>();

	private List<LeaderBoardSlotData> m_lstCoopRank = new List<LeaderBoardSlotData>();

	private int m_SeasonId;

	private RANK_TYPE m_CurRankType;

	private NKMLeaderBoardTemplet m_cNKMLeaderBoardTemplet;

	public void InitUI()
	{
		m_tglLevel.OnValueChanged.RemoveAllListeners();
		m_tglLevel.OnValueChanged.AddListener(OnValueChangedLevel);
		m_tglCoop.OnValueChanged.RemoveAllListeners();
		m_tglCoop.OnValueChanged.AddListener(OnValueChangedCoop);
		m_tglCoop.m_bGetCallbackWhileLocked = true;
		m_btnSeasonSelect.PointerClick.RemoveAllListeners();
		m_btnSeasonSelect.PointerClick.AddListener(OnClickSeasonSelect);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loop);
	}

	private RectTransform GetObject(int idx)
	{
		NKCUILeaderBoardSlot nKCUILeaderBoardSlot = null;
		nKCUILeaderBoardSlot = ((m_stkSlot.Count <= 0) ? Object.Instantiate(m_pfbSlot) : m_stkSlot.Pop());
		nKCUILeaderBoardSlot.transform.SetParent(m_trSlotParent);
		return nKCUILeaderBoardSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.gameObject.transform);
		NKCUILeaderBoardSlot component = tr.GetComponent<NKCUILeaderBoardSlot>();
		if (component != null)
		{
			m_stkSlot.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUILeaderBoardSlot component = tr.GetComponent<NKCUILeaderBoardSlot>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		tr.SetParent(m_trSlotParent);
		NKCUtil.SetGameobjectActive(tr, bValue: true);
		if (m_CurRankType == RANK_TYPE.COOP)
		{
			component.SetData(m_lstCoopRank[idx], m_cNKMLeaderBoardTemplet.m_BoardCriteria, null);
		}
		else if (m_CurRankType == RANK_TYPE.LEVEL)
		{
			component.SetData(m_lstLevelRank[idx], m_cNKMLeaderBoardTemplet.m_BoardCriteria, null);
		}
	}

	public void SetData()
	{
		NKCUtil.SetGameobjectActive(m_objNone, bValue: true);
		if (NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_RANKING))
		{
			if (NKCGuildCoopManager.m_GuildDungeonState == GuildDungeonState.Invalid)
			{
				m_tglCoop.Lock();
			}
			else
			{
				m_tglCoop.UnLock();
			}
			if (m_CurRankType == RANK_TYPE.NONE)
			{
				m_SeasonId = NKCGuildCoopManager.m_SeasonId;
				OnValueChangedLevel(bValue: true);
				m_tglLevel.Select(bSelect: true, bForce: true, bImmediate: true);
			}
			else
			{
				RefreshUI();
			}
		}
	}

	private void OnValueChangedLevel(bool bValue)
	{
		if (bValue && m_CurRankType != RANK_TYPE.LEVEL)
		{
			m_CurRankType = RANK_TYPE.LEVEL;
			m_cNKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_GUILD, 1);
			if (m_cNKMLeaderBoardTemplet == null)
			{
				Log.Error($"NKMLeaderBoardTemplet is null - BoardType : {LeaderBoardType.BT_GUILD}, criteria : {1}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCUIGuildLobbyRank.cs", 147);
			}
			else if (!NKCLeaderBoardManager.HasLeaderBoardData(m_cNKMLeaderBoardTemplet))
			{
				m_loop.TotalCount = 0;
				m_loop.RefreshCells();
				NKCLeaderBoardManager.SendReq(m_cNKMLeaderBoardTemplet, bAllReq: true);
			}
			else
			{
				RefreshUI();
			}
		}
	}

	private void OnValueChangedCoop(bool bValue)
	{
		if (m_tglCoop.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_CONSORTIUM_SEASON_OPEN_BEFORE_TOAST_TEXT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (bValue && m_CurRankType != RANK_TYPE.COOP)
		{
			m_CurRankType = RANK_TYPE.COOP;
			m_cNKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_GUILD, m_SeasonId);
			if (m_cNKMLeaderBoardTemplet == null)
			{
				Log.Error($"NKMLeaderBoardTemplet is null - BoardType : {LeaderBoardType.BT_GUILD}, criteria : {m_SeasonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCUIGuildLobbyRank.cs", 183);
			}
			else if (!NKCLeaderBoardManager.HasLeaderBoardData(m_cNKMLeaderBoardTemplet))
			{
				NKCLeaderBoardManager.SendReq(m_cNKMLeaderBoardTemplet, bAllReq: true);
			}
			else
			{
				RefreshUI();
			}
		}
	}

	private void OnClickSeasonSelect()
	{
		NKCPopupGuildRankSeasonSelect.Instance.Open(OnSeasonSelect, m_SeasonId);
	}

	private void OnSeasonSelect(int seasonId)
	{
		m_SeasonId = seasonId;
		m_cNKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_GUILD, seasonId);
		if (!NKCLeaderBoardManager.HasLeaderBoardData(m_cNKMLeaderBoardTemplet))
		{
			NKCLeaderBoardManager.SendReq(m_cNKMLeaderBoardTemplet, bAllReq: true);
		}
	}

	public void RefreshUI()
	{
		int boardId = 0;
		switch (m_CurRankType)
		{
		case RANK_TYPE.LEVEL:
			m_cNKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_GUILD, 1);
			if (m_cNKMLeaderBoardTemplet == null)
			{
				Log.Error($"NKMLeaderBoardTemplet is null - type : {LeaderBoardType.BT_GUILD}, criteria : {1}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCUIGuildLobbyRank.cs", 222);
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			}
			boardId = m_cNKMLeaderBoardTemplet.m_BoardID;
			m_lstLevelRank = NKCLeaderBoardManager.GetLeaderBoardData(m_cNKMLeaderBoardTemplet.m_BoardID);
			m_loop.TotalCount = m_lstLevelRank.Count;
			m_loop.SetIndexPosition(0);
			NKCUtil.SetLabelText(m_lbScoreName, NKCUtilString.GET_STRING_CONSORTIUM_RANKING_TOP_INFO_EXP);
			NKCUtil.SetGameobjectActive(m_objNone, m_lstLevelRank.Count == 0);
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, bValue: false);
			break;
		case RANK_TYPE.COOP:
			m_cNKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_GUILD, m_SeasonId);
			if (m_cNKMLeaderBoardTemplet == null)
			{
				Log.Error($"NKMLeaderBoardTemplet is null - type : {LeaderBoardType.BT_GUILD}, criteria : {m_SeasonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCUIGuildLobbyRank.cs", 244);
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			}
			boardId = m_cNKMLeaderBoardTemplet.m_BoardID;
			m_lstCoopRank = NKCLeaderBoardManager.GetLeaderBoardData(m_cNKMLeaderBoardTemplet.m_BoardID);
			m_loop.TotalCount = m_lstCoopRank.Count;
			m_loop.SetIndexPosition(0);
			NKCUtil.SetLabelText(m_lbScoreName, NKCUtilString.GET_STRING_CONSORTIUM_RANKING_TOP_INFO_DAMAGE);
			NKCUtil.SetGameobjectActive(m_objNone, m_lstCoopRank.Count == 0);
			NKCUtil.SetGameobjectActive(m_btnSeasonSelect, bValue: true);
			break;
		}
		int myRank;
		NKMGuildRankData rankData = NKCLeaderBoardManager.MakeMyGuildRankData(boardId, out myRank);
		m_slotMyGuildRank.SetData(LeaderBoardSlotData.MakeSlotData(rankData, myRank), m_cNKMLeaderBoardTemplet.m_BoardCriteria, null);
	}
}
