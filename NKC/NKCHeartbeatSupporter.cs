using System;
using System.Threading;
using ClientPacket.Service;
using Cs.Logging;
using NKM;
using UnityEngine;

namespace NKC;

public sealed class NKCHeartbeatSupporter
{
	private const float DeactivateThresholdSec = 3f;

	private Thread HeartBitThread;

	private bool isActivated;

	private float activationDeltaTime;

	public static NKCHeartbeatSupporter Instance { get; } = new NKCHeartbeatSupporter();

	public void Update()
	{
		if (!NeedToSupport())
		{
			return;
		}
		if (HeartBitThread == null)
		{
			Log.Info("[HeartBit] create thread", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCHeartbeatSupporter.cs", 29);
			HeartBitThread = new Thread(ThreadEntry);
			HeartBitThread.Start();
		}
		else if (!HeartBitThread.IsAlive)
		{
			Log.Warn("[HeartBit] thread is dead. create new thread.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCHeartbeatSupporter.cs", 35);
			HeartBitThread = new Thread(ThreadEntry);
			HeartBitThread.Start();
		}
		if (isActivated)
		{
			activationDeltaTime += Time.deltaTime;
			if (activationDeltaTime > 3f)
			{
				isActivated = false;
			}
		}
	}

	public void StartSupport()
	{
		isActivated = true;
		activationDeltaTime = 0f;
	}

	public void EndSupport()
	{
		isActivated = false;
		activationDeltaTime = 0f;
	}

	private static bool NeedToSupport()
	{
		if (NKCDefineManager.DEFINE_ANDROID())
		{
			if (!NKMContentsVersionManager.HasCountryTag(CountryTagType.CHN) && !NKMContentsVersionManager.HasCountryTag(CountryTagType.SEA))
			{
				return NKMContentsVersionManager.HasCountryTag(CountryTagType.TWN);
			}
			return true;
		}
		return false;
	}

	private static void ThreadEntry()
	{
		try
		{
			NKMPacket_HEART_BIT_REQ nKMPacket_HEART_BIT_REQ = new NKMPacket_HEART_BIT_REQ();
			long num = 0L;
			while (true)
			{
				Thread.Sleep(TimeSpan.FromSeconds(20.0));
				if (!Instance.isActivated)
				{
					continue;
				}
				NKCConnectGame connectGame = NKCScenManager.GetScenManager().GetConnectGame();
				if (connectGame != null)
				{
					long sendSequence = connectGame.SendSequence;
					if (num == sendSequence)
					{
						nKMPacket_HEART_BIT_REQ.time = DateTime.Now.Ticks;
						connectGame.RawSend(nKMPacket_HEART_BIT_REQ);
					}
					else
					{
						num = sendSequence;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex.Message, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCHeartbeatSupporter.cs", 112);
		}
	}
}
