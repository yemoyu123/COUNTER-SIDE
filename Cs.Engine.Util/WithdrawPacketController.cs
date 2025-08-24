using System;
using System.Collections;
using ClientPacket.Account;
using Cs.Core.Core;
using Cs.Engine.Network;
using Cs.Logging;
using Cs.Protocol;
using NKC;
using NKC.PacketHandler;
using NKC.Publisher;

namespace Cs.Engine.Util;

internal class WithdrawPacketController : IDisposable
{
	private static readonly TimeSpan timeout = TimeSpan.FromSeconds(10.0);

	private AtomicFlag onProcess = new AtomicFlag(initialValue: false);

	private bool ackReceived;

	private ISerializable reqPacket;

	private static WithdrawPacketController m_instance = null;

	public bool AckReceived => ackReceived;

	public ISerializable Ack { get; private set; }

	public IEnumerator WithdrawPacketProcess(string serverAddress, int serverPort)
	{
		onProcess.On();
		Ack = null;
		ackReceived = false;
		m_instance = this;
		reqPacket = NKCPublisherModule.Auth.MakeWithdrawReqPacket();
		Connection connection = Connection.Create(serverAddress, serverPort, "WithdrawPacketController", OnConnected, timeout);
		connection.RegisterHandler(typeof(WithdrawPacketController));
		connection.OnDisconnected += OnDisconnected;
		while (onProcess.IsOn)
		{
			yield return null;
			connection.ProcessResponses();
		}
		connection.Dispose();
		if (Ack == null)
		{
			Log.ErrorAndExit("[WithdrawPacketController] Failed to notice withdrawal.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Util/WithdrawPacketController.cs", 55);
		}
		else
		{
			Log.Info($"[WithdrawPacketController] Success to request withdrawal. ip: {serverAddress}, port: {serverPort}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Util/WithdrawPacketController.cs", 59);
		}
	}

	private void OnConnected(Connection connection)
	{
		if (connection != null && reqPacket != null)
		{
			connection.Send(reqPacket);
		}
	}

	private void OnDisconnected(Connection connection)
	{
		NKMPopUpBox.OpenWaitBox(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		onProcess.Off();
	}

	public static void OnRecv(NKMPacket_GAMEBASE_LEAVE_ACK sPacket)
	{
		if (!NKCPacketHandlers.Check_NKM_ERROR_CODE(sPacket.errorCode))
		{
			m_instance.Ack = null;
		}
		else
		{
			m_instance.Ack = sPacket;
		}
		m_instance.ackReceived = true;
		m_instance.onProcess.Off();
	}

	public void Dispose()
	{
		m_instance = null;
		Ack = null;
		reqPacket = null;
	}
}
