using NKC.UI.Component;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUITournamentRankSlot : MonoBehaviour
{
	public GameObject m_objTournament;

	public GameObject m_objDraft;

	public GameObject m_objUnlimited;

	public NKCComTMPUIText m_lbSeasonTitle;

	public NKCUILeaderBoardSlotTop3 m_slot;

	public void Init()
	{
		m_slot.InitUI();
	}

	public void SetRankSlotType(LeaderBoardType boardType)
	{
		NKCUtil.SetGameobjectActive(m_objTournament, boardType == LeaderBoardType.BT_TOURNAMENT);
		NKCUtil.SetGameobjectActive(m_objDraft, boardType == LeaderBoardType.BT_LEAGUE);
		NKCUtil.SetGameobjectActive(m_objUnlimited, boardType == LeaderBoardType.BT_UNLIMITED);
	}

	public void SetData(string title, LeaderBoardSlotData data1, LeaderBoardSlotData data2, LeaderBoardSlotData data3, int criteria, NKCUILeaderBoardSlot.OnDragBegin onDragBegin)
	{
		NKCUtil.SetLabelText(m_lbSeasonTitle, title);
		m_slot.SetData(data1, data2, data3, criteria, onDragBegin);
	}
}
