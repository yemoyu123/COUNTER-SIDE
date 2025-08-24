using System.Collections;
using System.IO;
using Cs.Logging;
using UnityEngine.Networking;

namespace NKC.Patcher;

public class UnityWebRequestDownloader : NKCFileDownloader
{
	private const string _className = "UnityWebRequestDownloader";

	public UnityWebRequestDownloader(string serverBaseAddress, string localBaseAddress)
		: base(serverBaseAddress, localBaseAddress)
	{
	}

	public override IEnumerator DownloadFile(string relativeDownloadPath, string relativeTargetPath, long targetFileSize)
	{
		string url = m_strServerBaseAddress + relativeDownloadPath;
		string path = m_strLocalBaseAddress + relativeTargetPath;
		using UnityWebRequest webRequest = new UnityWebRequest(url);
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		webRequest.timeout = int.MaxValue;
		webRequest.downloadHandler = new DownloadHandlerFile(path);
		webRequest.SendWebRequest();
		while (!webRequest.isDone)
		{
			OnProgress((long)webRequest.downloadedBytes, targetFileSize);
			yield return null;
		}
		bool bSucceed = true;
		if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
		{
			bSucceed = false;
			Log.Error("[UnityWebRequestDownloader][DownloadFile] DownloadFail : " + webRequest.error, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Patcher/NKCFileDownloader.cs", 119);
		}
		OnComplete(bSucceed);
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			disposedValue = true;
		}
	}
}
