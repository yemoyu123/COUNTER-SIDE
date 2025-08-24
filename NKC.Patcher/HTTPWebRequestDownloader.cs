using System.Collections;
using System.IO;
using System.Net;

namespace NKC.Patcher;

public class HTTPWebRequestDownloader : NKCFileDownloader
{
	private string ResumeCode;

	private byte[] downBuffer;

	private const int iBufferSize = 1048576;

	public HTTPWebRequestDownloader(string serverBaseAddress, string localBaseAddress, string versionCode)
		: base(serverBaseAddress, localBaseAddress)
	{
		ResumeCode = versionCode;
		downBuffer = new byte[1048576];
	}

	public override IEnumerator DownloadFile(string relativeDownloadPath, string relativeTargetPath, long targetFileSize)
	{
		string requestUriString = m_strServerBaseAddress + relativeDownloadPath;
		string targetPath = m_strLocalBaseAddress + relativeTargetPath;
		string targetTempPath = targetPath + "." + ResumeCode;
		string directoryName = Path.GetDirectoryName(targetPath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		bool flag = !string.IsNullOrEmpty(ResumeCode);
		FileInfo fileInfo = new FileInfo(targetTempPath);
		if (fileInfo.Exists && fileInfo.Length >= targetFileSize)
		{
			File.Delete(targetTempPath);
		}
		using (FileStream fileStream = new FileStream(targetTempPath, flag ? FileMode.Append : FileMode.Create))
		{
			int currentFileLength = (int)fileStream.Length;
			HttpWebRequest httpWebRequest = WebRequest.Create(requestUriString) as HttpWebRequest;
			if (flag && 0 < currentFileLength)
			{
				httpWebRequest.AddRange(currentFileLength);
			}
			httpWebRequest.Timeout = 10000;
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream smRespStream = httpWebResponse.GetResponseStream();
			_ = httpWebResponse.ContentLength;
			long maxSize = httpWebResponse.ContentLength;
			int num;
			while ((num = smRespStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
			{
				fileStream.Write(downBuffer, 0, num);
				currentFileLength += num;
				if (maxSize > 0)
				{
					OnProgress(currentFileLength, maxSize);
				}
				if (fileStream.Length == targetFileSize)
				{
					break;
				}
				yield return null;
			}
		}
		File.Copy(targetTempPath, targetPath, overwrite: true);
		File.Delete(targetTempPath);
		OnComplete(bSucceed: true);
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			disposedValue = true;
		}
	}
}
