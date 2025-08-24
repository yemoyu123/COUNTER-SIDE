using System.Collections;
using System.IO;
using AssetBundles;
using DG.Tweening;
using NKC.Patcher;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace NKC;

public class NKCUILoginViewerMsg : MonoBehaviour
{
	public CanvasGroup m_cgRoot;

	public Text m_lbMessage;

	private bool m_bRunningPolling;

	private float m_fLastUpdateTime = float.MinValue;

	private const float POLLING_PERIOD = 30f;

	public void ForceUpdateMsg()
	{
		if (!m_bRunningPolling)
		{
			m_fLastUpdateTime = Time.time;
			StartCoroutine(UpdateLoginMsg());
		}
	}

	private void Start()
	{
		InvalidMsg();
	}

	private void SetMsg(string msg)
	{
		if (m_lbMessage == null || m_cgRoot == null || string.IsNullOrWhiteSpace(msg))
		{
			InvalidMsg();
			return;
		}
		if (m_lbMessage.text != msg)
		{
			m_cgRoot.alpha = 0f;
			m_cgRoot.DOFade(1f, 1f);
		}
		else
		{
			m_cgRoot.alpha = 1f;
		}
		NKCUtil.SetLabelText(m_lbMessage, msg);
	}

	private void InvalidMsg()
	{
		if (m_lbMessage != null)
		{
			m_lbMessage.text = "";
		}
		if (m_cgRoot != null)
		{
			m_cgRoot.alpha = 0f;
		}
	}

	private void Update()
	{
		if (!m_bRunningPolling && 30f + m_fLastUpdateTime < Time.time)
		{
			m_fLastUpdateTime = Time.time;
			StartCoroutine(UpdateLoginMsg());
		}
	}

	private IEnumerator UpdateLoginMsg()
	{
		m_bRunningPolling = true;
		string localDownloadPath = AssetBundleManager.GetLocalDownloadPath();
		string text = Application.streamingAssetsPath + "/CSConfigServerAddress.txt";
		bool bSuccessGetMsgFromServer = false;
		if (NKCPatchUtility.IsFileExists(text))
		{
			Debug.Log("CSConfigServerAddress exist");
			string aJSON = ((!text.Contains("jar:")) ? File.ReadAllText(text) : BetterStreamingAssets.ReadAllText(NKCAssetbundleInnerStream.GetJarRelativePath(text)));
			JSONNode jSONNode = JSONNode.Parse(aJSON);
			if (jSONNode != null)
			{
				string url = jSONNode["address"];
				string targetFileName = Path.Combine(localDownloadPath, "CSConfig.txt");
				if (!Directory.Exists(Path.GetDirectoryName(targetFileName)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
				}
				using UnityWebRequest uwr = new UnityWebRequest(url);
				uwr.method = "GET";
				DownloadHandlerFile downloadHandlerFile = new DownloadHandlerFile(targetFileName);
				downloadHandlerFile.removeFileOnAbort = true;
				uwr.downloadHandler = downloadHandlerFile;
				yield return uwr.SendWebRequest();
				bool flag = false;
				if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
				{
					flag = true;
				}
				if (flag)
				{
					Debug.Log(uwr.error);
					m_bRunningPolling = false;
					InvalidMsg();
					yield break;
				}
				Debug.Log("Download saved to: " + targetFileName.Replace("/", "\\") + "\r\n" + uwr.error);
				if (NKCPatchUtility.IsFileExists(targetFileName))
				{
					aJSON = File.ReadAllText(targetFileName);
					JSONNode jSONNode2 = JSONNode.Parse(aJSON);
					if (jSONNode2 != null)
					{
						string aKey = "LOGIN_SCREEN_MSG_" + NKCStringTable.GetNationalCode();
						if (jSONNode2[aKey] != null && !string.IsNullOrWhiteSpace(jSONNode2[aKey]))
						{
							bSuccessGetMsgFromServer = true;
							SetMsg(jSONNode2[aKey]);
						}
					}
				}
			}
		}
		else
		{
			Debug.Log("CSConfigServerAddress not exist");
		}
		if (!bSuccessGetMsgFromServer)
		{
			InvalidMsg();
		}
		m_bRunningPolling = false;
	}
}
