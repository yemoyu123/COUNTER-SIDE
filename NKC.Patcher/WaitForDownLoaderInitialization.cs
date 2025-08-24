using System.Collections;

namespace NKC.Patcher;

public class WaitForDownLoaderInitialization : IPatchProcessStrategy, IEnumerable
{
	public IPatchProcessStrategy.ExecutionStatus Status { get; private set; }

	public string ReasonOfFailure { get; private set; } = string.Empty;

	public IEnumerator GetEnumerator()
	{
		Status = IPatchProcessStrategy.ExecutionStatus.Success;
		ReasonOfFailure = string.Empty;
		if (NKCPatchDownloader.Instance == null)
		{
			if (NKCDefineManager.DEFINE_EXTRA_ASSET() || NKCDefineManager.DEFINE_ZLONG_CHN())
			{
				LegacyPatchDownloader.InitInstance(NKCPatcherManager.GetPatcherManager().ShowError);
			}
			else
			{
				NKCPatchParallelDownloader.InitInstance(NKCPatcherManager.GetPatcherManager().ShowError);
			}
		}
		NKCPatchDownloader.Instance.StopBackgroundDownload();
		if (NKCPatchChecker.PatcherVideoPlayer != null)
		{
			NKCPatchChecker.PatcherVideoPlayer.PlayVideo();
		}
		if (NKCDefineManager.DEFINE_USE_CHEAT())
		{
			NKCPatchUtility.ProcessPatchSkipTest(NKCPatchDownloader.Instance.LocalDownloadPath);
		}
		NKCPatchChecker.PatcherUI?.SetIntegrityCheckProgress();
		NKCPatchChecker.PatcherUI?.Progress();
		while (!NKCPatchDownloader.Instance.IsInit)
		{
			yield return null;
		}
	}
}
