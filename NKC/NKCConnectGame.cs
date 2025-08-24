using System;
using ClientPacket.Service;
using Cs.Logging;
using NKC.PacketHandler;
using NKC.Publisher;
using NKM;
using UnityEngine;

namespace NKC;

public sealed class NKCConnectGame : NKCConnectBase
{
	private enum NKC_CONNECT_LOBBY_STATE
	{
		NCLS_INIT,
		NCLS_CONNECTING,
		NCLS_CONNECTED,
		NCLS_LOGGED_IN
	}

	private const float HEARTBEAT_TIME_OUT = 16f;

	private const float HEARTBEAT_INTERVAL = 10f;

	private readonly NKMPacket_HEART_BIT_REQ heartbitReq_ = new NKMPacket_HEART_BIT_REQ();

	private NKC_CONNECT_LOBBY_STATE state_;

	private float heartBitTimeOutNow_ = 16f;

	private float heartBitIntervalNow_ = 10f;

	private string accessToken;

	private float m_fPingTime;

	private string m_ReconnectKey = "";

	private bool m_bEnable = true;

	public bool HasLoggedIn => state_ == NKC_CONNECT_LOBBY_STATE.NCLS_LOGGED_IN;

	public float GetPingTime()
	{
		return m_fPingTime;
	}

	public void SetReconnectKey(string key)
	{
		if (key == null)
		{
			m_ReconnectKey = "";
		}
		else
		{
			m_ReconnectKey = key;
		}
	}

	public string GetReconnectKey()
	{
		return m_ReconnectKey;
	}

	public NKCConnectGame()
		: base(typeof(NKCPacketHandlersLobby))
	{
	}

	public void SetEnable(bool bSet)
	{
		m_bEnable = bSet;
	}

	public override void ResetConnection()
	{
		base.ResetConnection();
		heartBitTimeOutNow_ = 16f;
		heartBitIntervalNow_ = 10f;
		ChangeState(NKC_CONNECT_LOBBY_STATE.NCLS_INIT);
	}

	public void SetAccessToken(string token)
	{
		accessToken = token;
	}

	public override void LoginComplete()
	{
		base.LoginComplete();
		ChangeState(NKC_CONNECT_LOBBY_STATE.NCLS_LOGGED_IN);
	}

	public void ConnectToLobbyServer()
	{
		ChangeState(NKC_CONNECT_LOBBY_STATE.NCLS_CONNECTING);
		Connect();
	}

	public void Send_JOIN_LOBBY_REQ()
	{
		if (m_bEnable && base.IsConnected)
		{
			ChangeState(NKC_CONNECT_LOBBY_STATE.NCLS_CONNECTED);
			Send(NKCPublisherModule.Auth.MakeGameServerLoginReqPacket(accessToken), NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
		}
	}

	public override void Update()
	{
		base.Update();
		if (base.IsConnected)
		{
			NKCHeartbeatSupporter.Instance.Update();
			HeartBitProcess();
		}
	}

	public void Reconnect()
	{
		if (m_bEnable)
		{
			Log.Info("disconnect all connection. trying reconnect to login server.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCConnectGame.cs", 105);
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAME || NKCScenManager.GetScenManager().GetGameClient() == null || NKCScenManager.GetScenManager().GetGameClient().GetGameData() == null || NKCScenManager.GetScenManager().GetGameClient().GetGameData()
				.GetGameType() != NKM_GAME_TYPE.NGT_DEV)
			{
				NKCScenManager.GetScenManager().GetConnectLogin().ResetConnection();
				NKCScenManager.GetScenManager().GetConnectGame().ResetConnection();
				NKCScenManager.GetScenManager().GetConnectLogin().AuthToLoginServer();
			}
		}
	}

	public void ResetHeartbitTimeout(float fTimeOutTime = 16f)
	{
		heartBitTimeOutNow_ = fTimeOutTime;
	}

	protected override void OnConnectedMainThread()
	{
		base.OnConnectedMainThread();
		if (state_ == NKC_CONNECT_LOBBY_STATE.NCLS_CONNECTING)
		{
			Send_JOIN_LOBBY_REQ();
		}
	}

	protected override void OnConnectFailedMainThread()
	{
		base.OnConnectFailedMainThread();
		NKCScenManager.GetScenManager().SetAppEnableConnectCheckTime(1f);
	}

	protected override void OnDisconnectedMainThread()
	{
		if (state_ == NKC_CONNECT_LOBBY_STATE.NCLS_CONNECTED || state_ == NKC_CONNECT_LOBBY_STATE.NCLS_LOGGED_IN)
		{
			base.OnDisconnectedMainThread();
			NKCScenManager.GetScenManager().SetAppEnableConnectCheckTime(1f);
		}
	}

	private void HeartBitProcess()
	{
		if (base.IsConnected && state_ == NKC_CONNECT_LOBBY_STATE.NCLS_LOGGED_IN)
		{
			heartBitIntervalNow_ -= Time.deltaTime;
			if (heartBitIntervalNow_ <= 0f)
			{
				heartBitIntervalNow_ = 10f;
				heartbitReq_.time = DateTime.Now.Ticks;
				Send(heartbitReq_, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
			}
			heartBitTimeOutNow_ -= Time.deltaTime;
			if (heartBitTimeOutNow_ <= 0f)
			{
				Debug.Log("[HeartBitProcess] HeartBit timeout occurred!");
				NKCScenManager.GetScenManager().SetAppEnableConnectCheckTime(1f);
			}
		}
	}

	public void SetPingTime(long pingTime)
	{
		DateTime dateTime = new DateTime(pingTime);
		m_fPingTime = (float)(DateTime.Now - dateTime).TotalSeconds;
	}

	private void ChangeState(NKC_CONNECT_LOBBY_STATE eNKC_CONNECT_LOBBY_STATE)
	{
		state_ = eNKC_CONNECT_LOBBY_STATE;
		Log.Info($"NKCConnectLobby:StateChange {eNKC_CONNECT_LOBBY_STATE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCConnectGame.cs", 208);
	}
}
