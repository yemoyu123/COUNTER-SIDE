using System;
using System.Collections;
using System.Diagnostics;
using ClientPacket.Account;
using Cs.Core.Core;
using Cs.Engine.Network;
using Cs.Logging;
using Cs.Protocol;
using NKC;
using NKM;
using UnityEngine;

namespace Cs.Engine.Util;

internal static class ContentsVersionChecker
{
	private static readonly TimeSpan timeout;

	private static AtomicFlag onProcess;

	private static int versionReqCount;

	private static bool versionAckReceived;

	public static bool VersionAckReceived => versionAckReceived;

	public static float RetryInterval => 10f;

	public static NKMPacket_CONTENTS_VERSION_ACK Ack { get; private set; }

	static ContentsVersionChecker()
	{
		timeout = TimeSpan.FromSeconds(10.0);
		onProcess = new AtomicFlag(initialValue: false);
		versionReqCount = 0;
		versionAckReceived = false;
		PacketController.Instance.Initialize();
	}

	public static IEnumerator GetVersion(string serverAddress, int serverPort = -1, bool bUseLocalSaveLastServerInfoToGetTags = true)
	{
		if (serverPort == -1)
		{
			serverPort = NKCConnectionInfo.ServicePort;
		}
		if (bUseLocalSaveLastServerInfoToGetTags)
		{
			string text = PlayerPrefs.GetString("LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_IP");
			int num = PlayerPrefs.GetInt("LOCAL_SAVE_CONTENTS_TAG_LAST_SERVER_PORT");
			Log.Info($"LoadTagInfo IP[{text}] PORT[{num}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Util/ContentsVersionChecker.cs", 44);
			if (text.Length > 1)
			{
				serverAddress = text;
			}
			if (num > 0)
			{
				serverPort = num;
			}
			if (!NKCDefineManager.DEFINE_SERVICE())
			{
				NKCConnectionInfo.SetLoginServerInfo(NKCConnectionInfo.CurrentLoginServerType, serverAddress, serverPort);
			}
		}
		onProcess.On();
		Ack = null;
		versionAckReceived = false;
		Log.Info("VersionChecker, serverAddress : " + serverAddress + ", port : " + serverPort, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Util/ContentsVersionChecker.cs", 62);
		Stopwatch stopwatch = Stopwatch.StartNew();
		Connection connection = Connection.Create(serverAddress, serverPort, "versionChecker", OnConnected, timeout);
		connection.RegisterHandler(typeof(ContentsVersionChecker));
		connection.OnDisconnected += OnDisconnected;
		while (onProcess.IsOn && stopwatch.Elapsed < timeout)
		{
			yield return null;
			connection.ProcessResponses();
		}
		connection.Dispose();
		if (Ack == null)
		{
			Log.ErrorAndExit($"[ContentsVersionChecker] get version failed. elapsed:{stopwatch.ElapsedMilliseconds}msec", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Util/ContentsVersionChecker.cs", 85);
		}
		else
		{
			Log.Info($"[ContentsVersionChecker] get version success. elapsed:{stopwatch.ElapsedMilliseconds}msec", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Util/ContentsVersionChecker.cs", 89);
		}
	}

	private static void OnConnected(Connection connection)
	{
		if (connection == null)
		{
			Log.Error("[ContentsVersionChecker] _ connection is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Util/ContentsVersionChecker.cs", 96);
			return;
		}
		versionReqCount++;
		connection.Send(new NKMPacket_CONTENTS_VERSION_REQ());
	}

	private static void OnDisconnected(Connection connection)
	{
		onProcess.Off();
	}

	public static void OnRecv(NKMPacket_CONTENTS_VERSION_ACK ack)
	{
		if (ack.errorCode != NKM_ERROR_CODE.NEC_OK)
		{
			Log.ErrorAndExit($"[ContentsVersionChecker] errorCode:{ack.errorCode}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Util/ContentsVersionChecker.cs", 113);
			return;
		}
		NKMContentsVersionManager.Drop();
		Log.Info("[ContentsVersion] version:" + ack.contentsVersion + " tag:" + string.Join(", ", ack.contentsTag), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Cs.Engine/Util/ContentsVersionChecker.cs", 120);
		foreach (string item in ack.contentsTag)
		{
			NKMContentsVersionManager.AddTag(item);
		}
		NKCContentsVersionManager.SaveTagToLocal();
		Ack = ack;
		NKCSynchronizedTime.OnRecv(ack.utcTime, ack.utcOffset);
		versionAckReceived = true;
		onProcess.Off();
	}
}
