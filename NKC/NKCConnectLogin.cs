using Cs.Logging;
using Cs.Protocol;
using NKC.PacketHandler;
using NKC.Publisher;
using NKC.UI;
using NKM;

namespace NKC;

public sealed class NKCConnectLogin : NKCConnectBase
{
	private enum NKC_CONNECT_LOGIN_STATE
	{
		NCLS_INIT,
		NCLS_CONNECTING_FOR_LOGIN,
		NCLS_CONNECTED_AND_LOGIN,
		NCLS_CONNECTED_AND_READY_TO_JOIN_LOBBY
	}

	private NKC_CONNECT_LOGIN_STATE state_;

	private bool m_bEnable = true;

	public NKCConnectLogin()
		: base(typeof(NKCPacketHandlersLogin))
	{
	}

	public void SetEnable(bool bSet)
	{
		m_bEnable = bSet;
	}

	public override void ResetConnection()
	{
		base.ResetConnection();
		StateChange(NKC_CONNECT_LOGIN_STATE.NCLS_INIT);
	}

	public void AuthToLoginServer()
	{
		StateChange(NKC_CONNECT_LOGIN_STATE.NCLS_CONNECTING_FOR_LOGIN);
		Connect();
	}

	public void Send_LOGIN_REQ()
	{
		if (m_bEnable && base.IsConnected)
		{
			ISerializable serializable = NKCPublisherModule.Auth.MakeLoginServerLoginReqPacket();
			if (serializable == null)
			{
				NKMPopUpBox.CloseWaitBox();
				ResetConnection();
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_TRY_AGAIN);
			}
			else
			{
				StateChange(NKC_CONNECT_LOGIN_STATE.NCLS_CONNECTED_AND_LOGIN);
				Send(serializable, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
			}
		}
	}

	public override void LoginComplete()
	{
		if (base.IsConnected)
		{
			base.LoginComplete();
			StateChange(NKC_CONNECT_LOGIN_STATE.NCLS_CONNECTED_AND_READY_TO_JOIN_LOBBY);
		}
	}

	protected override void OnConnectedMainThread()
	{
		base.OnConnectedMainThread();
		if (state_ == NKC_CONNECT_LOGIN_STATE.NCLS_CONNECTING_FOR_LOGIN)
		{
			Send_LOGIN_REQ();
		}
	}

	protected override void OnConnectFailedMainThread()
	{
		base.OnConnectFailedMainThread();
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_LOGIN)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_FAIL_CONNECT, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_FAIL_CONNECT);
		}
	}

	protected override void OnDisconnectedMainThread()
	{
		base.OnDisconnectedMainThread();
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_LOGIN)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_DECONNECT, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_LOGIN);
			});
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_DECONNECT);
		}
	}

	private void StateChange(NKC_CONNECT_LOGIN_STATE eNKC_CONNECT_LOGIN_STATE)
	{
		Log.Info($"{base.TypeName} StateChange {state_} -> {eNKC_CONNECT_LOGIN_STATE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCConnectLogin.cs", 130);
		state_ = eNKC_CONNECT_LOGIN_STATE;
	}
}
