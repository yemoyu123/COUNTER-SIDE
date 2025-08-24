using ClientPacket.Pvp;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyEvent : MonoBehaviour
{
	public NKCUIGauntletLobbyEventDeckData m_eventDeckData;

	public GameObject m_objUseRankPvpDeck;

	public Text m_lbSeasonName;

	public Text m_lbSeasonDesc;

	public Image m_imgSeasonArt;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public NKCUIGauntletLobbyRightSideEvent m_RightSide;

	[Header("\ufffd\ufffdư")]
	public GameObject m_ObjGlobalBan;

	public NKCUIComStateButton m_csbtnGlobalBan;

	private float m_fPrevUpdateTime;

	public void Init()
	{
		m_RightSide?.InitUI();
		m_eventDeckData?.Init();
		NKCUtil.SetBindFunction(m_csbtnGlobalBan, OnClickGlobalBan);
	}

	private void Update()
	{
		if (m_fPrevUpdateTime + 1f < Time.time)
		{
			m_fPrevUpdateTime = Time.time;
			m_RightSide.UpdateRankPVPPointUI();
		}
	}

	public void SetUI()
	{
		if (m_RightSide != null)
		{
			m_RightSide.UpdateNowSeasonPVPInfoUI(NKM_GAME_TYPE.NGT_PVP_EVENT);
			m_RightSide.UpdateEventPVPUI();
		}
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (m_eventDeckData != null)
		{
			bool flag = eventPvpSeasonTemplet?.EventDeckTemplet != null;
			m_eventDeckData.SetActive(flag);
			NKCUtil.SetGameobjectActive(m_objUseRankPvpDeck, !flag);
			if (flag)
			{
				m_eventDeckData.SetData(eventPvpSeasonTemplet.EventDeckTemplet);
			}
		}
		string msg = "";
		string msg2 = "";
		bool bValue = false;
		if (eventPvpSeasonTemplet != null)
		{
			if (m_objUseRankPvpDeck.activeSelf && eventPvpSeasonTemplet.DraftBanPick)
			{
				NKCUtil.SetGameobjectActive(m_objUseRankPvpDeck, bValue: false);
			}
			msg = NKCStringTable.GetString(eventPvpSeasonTemplet.SeasonName);
			msg2 = NKCStringTable.GetString(eventPvpSeasonTemplet.SeasonDesc);
			bValue = eventPvpSeasonTemplet.DraftBanPick;
		}
		NKCUtil.SetLabelText(m_lbSeasonName, msg);
		NKCUtil.SetLabelText(m_lbSeasonDesc, msg2);
		NKCUtil.SetImageSprite(m_imgSeasonArt, NKCEventPvpMgr.GetLobbySeasonArt());
		NKCUtil.SetGameobjectActive(m_ObjGlobalBan, bValue);
		if (NKCEventPvpMgr.EventPvpRewardInfo == null)
		{
			if (eventPvpSeasonTemplet != null)
			{
				NKCPacketSender.Send_NKMPacket_EVENT_PVP_SEASON_INFO_REQ(eventPvpSeasonTemplet.SeasonId);
			}
		}
		else
		{
			m_RightSide?.UpdateRewardRedDot();
		}
		m_RightSide?.UpdateRankPVPPointUI();
	}

	public void OnRecvEventPvpSeasonInfo()
	{
		m_RightSide?.OnRecvEventPvpSeasonInfo();
	}

	public void OnRecvEventPvpReward()
	{
		m_RightSide?.RefreshReward();
	}

	public void OnRecv(NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)
	{
		m_RightSide?.UpdateRankPVPPointUI();
	}

	public void ClearCacheData()
	{
		NKCEventPvpMgr.EventDeckData = null;
		NKCEventPvpMgr.EventPvpRewardInfo = null;
		m_RightSide?.OnCloseInstance();
	}

	public bool CanClose()
	{
		return m_RightSide.CanClose();
	}

	public void Close()
	{
	}

	private void OnClickGlobalBan()
	{
		NKMEventPvpSeasonTemplet eventPvpSeasonTemplet = NKCEventPvpMgr.GetEventPvpSeasonTemplet();
		if (eventPvpSeasonTemplet == null || eventPvpSeasonTemplet.DraftBanPick)
		{
			NKCPopupGauntletBan.Instance.Open();
		}
	}
}
