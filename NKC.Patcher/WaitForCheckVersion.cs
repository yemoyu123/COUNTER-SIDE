using System.Collections;
using Cs.Engine.Util;
using Cs.Logging;
using NKC.Publisher;
using UnityEngine;

namespace NKC.Patcher;

public class WaitForCheckVersion : IPatchProcessStrategy, IEnumerable
{
	public IPatchProcessStrategy.ExecutionStatus Status { get; private set; }

	public string ReasonOfFailure { get; private set; } = string.Empty;

	private void StopBg()
	{
		if (NKCPatchChecker.PatcherVideoPlayer != null)
		{
			NKCPatchChecker.PatcherVideoPlayer.StopBG();
		}
	}

	public IEnumerator GetEnumerator()
	{
		Status = IPatchProcessStrategy.ExecutionStatus.Success;
		ReasonOfFailure = string.Empty;
		Log.Debug("[PatcherManager] Stop BGM", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchProcessStrategy/PatcherProcessStrategies.cs", 379);
		if (NKCDefineManager.DEFINE_CHECKVERSION())
		{
			if (ContentsVersionChecker.VersionAckReceived)
			{
				StopBg();
				yield break;
			}
			float versionReqRetryTime = 0f;
			int versionRequestRetryCount = 0;
			while (!ContentsVersionChecker.VersionAckReceived)
			{
				versionReqRetryTime -= Time.deltaTime;
				if (versionReqRetryTime <= 0f)
				{
					NKCPatcherManager.GetPatcherManager().ShowRequestTimer(bShow: true);
					string serviceIP = NKCConnectionInfo.ServiceIP;
					Log.Debug("IPatchProcessStrategy Trying to retrieve server tag from " + serviceIP, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCPatchProcessStrategy/PatcherProcessStrategies.cs", 403);
					yield return ContentsVersionChecker.GetVersion(serviceIP, -1, bUseLocalSaveLastServerInfoToGetTags: false);
					versionReqRetryTime = ContentsVersionChecker.RetryInterval;
					versionRequestRetryCount++;
				}
				if (versionRequestRetryCount >= 1)
				{
					break;
				}
				yield return null;
			}
			if (ContentsVersionChecker.Ack != null)
			{
				NKCPublisherModule.Statistics.LogClientAction(NKCPublisherModule.NKCPMStatistics.eClientAction.Patch_TagProvided);
			}
			NKCPatcherManager.GetPatcherManager().ShowRequestTimer(bShow: false);
			StopBg();
		}
		else
		{
			StopBg();
		}
	}
}
