using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Guild;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_GUILD_LOBBY : NKC_SCEN_BASIC
{
	private NKCAssetResourceData m_UILoadResourceData;

	private NKCUIGuildLobby m_NKCUIGuildLobby;

	private bool m_bChatDataRecved;

	private bool m_bCoopDataRecved;

	private bool m_bReservedMoveToCoop;

	private bool m_bReservedMoveToShop;

	private NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE m_eReservedTab = NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Info;

	private const float FIVE_SECONDS = 5f;

	private float m_deltaTime;

	public NKC_SCEN_GUILD_LOBBY()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GUILD_LOBBY;
	}

	public void SetReserveLobbyTab(NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE tab)
	{
		m_eReservedTab = tab;
	}

	public void ClearCacheData()
	{
		if (m_NKCUIGuildLobby != null)
		{
			m_NKCUIGuildLobby.CloseInstance();
			m_NKCUIGuildLobby = null;
		}
		m_bReservedMoveToCoop = false;
		m_bReservedMoveToShop = false;
	}

	public override void ScenDataReq()
	{
		if (NKCGuildManager.MyData.guildUid > 0)
		{
			m_bChatDataRecved = false;
			m_bCoopDataRecved = true;
			m_deltaTime = 0f;
			NKCPacketSender.Send_NKMPacket_GUILD_CHAT_LIST_REQ(NKCGuildManager.MyData.guildUid);
			if (NKCContentManager.CheckContentStatus(ContentsType.GUILD_DUNGEON, out var _) == NKCContentManager.eContentStatus.Open && (!NKCGuildCoopManager.m_bGuildCoopDataRecved || (NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC) && NKCGuildCoopManager.HasNextSessionData(NKCGuildCoopManager.m_NextSessionStartDateUTC))))
			{
				m_bCoopDataRecved = false;
				NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_INFO_REQ(NKCGuildManager.MyData.guildUid);
			}
			base.ScenDataReq();
		}
		else
		{
			Set_NKC_SCEN_STATE(NKC_SCEN_STATE.NSS_FAIL);
		}
	}

	public override void ScenDataReqWaitUpdate()
	{
		m_deltaTime += Time.deltaTime;
		if (m_deltaTime > 5f)
		{
			m_deltaTime = 0f;
			Set_NKC_SCEN_STATE(NKC_SCEN_STATE.NSS_FAIL);
		}
		else if (m_bChatDataRecved && m_bCoopDataRecved)
		{
			m_deltaTime = 0f;
			base.ScenDataReqWaitUpdate();
		}
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (m_NKCUIGuildLobby == null)
		{
			m_UILoadResourceData = NKCUIGuildLobby.OpenInstanceAsync();
		}
		else
		{
			m_UILoadResourceData = null;
		}
		m_bReservedMoveToCoop = false;
	}

	public override void ScenLoadUIComplete()
	{
		if (m_NKCUIGuildLobby == null && m_UILoadResourceData != null)
		{
			if (!NKCUIGuildLobby.CheckInstanceLoaded(m_UILoadResourceData, out m_NKCUIGuildLobby))
			{
				Debug.LogError("Error - NKC_SCEN_GUILD_LOBBY.ScenLoadUIComplete() : UI Load Failed!");
				return;
			}
			m_UILoadResourceData = null;
			m_NKCUIGuildLobby.InitUI();
			NKCUtil.SetGameobjectActive(m_NKCUIGuildLobby, bValue: false);
		}
		base.ScenLoadUIComplete();
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		if (m_NKCUIGuildLobby != null)
		{
			m_NKCUIGuildLobby.InitUI();
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: false);
		if (m_NKCUIGuildLobby != null)
		{
			m_NKCUIGuildLobby.Open(m_eReservedTab);
		}
		if (m_bReservedMoveToShop)
		{
			m_NKCUIGuildLobby.OpenShop();
		}
		m_eReservedTab = NKCUIGuildLobby.GUILD_LOBBY_UI_TYPE.Info;
		m_bReservedMoveToShop = false;
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_NKCUIGuildLobby != null)
		{
			m_NKCUIGuildLobby.Close();
		}
		ClearCacheData();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void SetChatDataRecved(bool bValue)
	{
		m_bChatDataRecved = bValue;
	}

	public void SetCoopDataRecved(bool bValue)
	{
		m_bCoopDataRecved = bValue;
		if (m_bReservedMoveToCoop && m_bCoopDataRecved && NKCGuildCoopManager.m_GuildDungeonState != GuildDungeonState.Invalid)
		{
			SetReserveMoveToCoopScen(bValue: false);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_COOP);
		}
		else if (m_NKCUIGuildLobby != null && m_NKCUIGuildLobby.IsOpen)
		{
			RefreshUI();
		}
	}

	public void SetReserveMoveToCoopScen(bool bValue)
	{
		m_bReservedMoveToCoop = bValue;
	}

	public void SetReserveMoveToShop(bool bValue)
	{
		m_bReservedMoveToShop = NKCContentManager.CheckContentStatus(ContentsType.GUILD_SHOP, out var _) == NKCContentManager.eContentStatus.Open && bValue;
	}

	public void OnRecv(List<FriendListData> list)
	{
		if (NKCPopupGuildInvite.IsInstanceOpen)
		{
			NKCPopupGuildInvite.Instance.OnRecv(list);
		}
	}

	public void RefreshUI()
	{
		if (m_NKCUIGuildLobby != null && m_NKCUIGuildLobby.gameObject.activeSelf)
		{
			m_NKCUIGuildLobby.OnGuildDataChanged();
		}
	}
}
