using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetBundles;

public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
{
	public delegate void OnComplete();

	protected string m_AssetBundleName;

	protected string m_LevelName;

	protected bool m_IsAdditive;

	protected string m_DownloadingError;

	protected AsyncOperation m_Request;

	protected OnComplete m_OnComplete;

	public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive, OnComplete dOnComplete)
	{
		m_AssetBundleName = assetbundleName;
		m_LevelName = levelName;
		m_IsAdditive = isAdditive;
		m_OnComplete = dOnComplete;
	}

	public override bool Update()
	{
		if (m_Request != null)
		{
			return false;
		}
		if (AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError) != null)
		{
			if (m_IsAdditive)
			{
				m_Request = SceneManager.LoadSceneAsync(m_LevelName, LoadSceneMode.Additive);
			}
			else
			{
				m_Request = SceneManager.LoadSceneAsync(m_LevelName, LoadSceneMode.Single);
			}
			if (m_Request == null)
			{
				m_DownloadingError = "Asset Request failed";
			}
			m_Request.completed += delegate
			{
				m_OnComplete?.Invoke();
			};
			return false;
		}
		return true;
	}

	public override bool IsDone()
	{
		if (m_Request == null && m_DownloadingError != null)
		{
			Debug.LogError(m_DownloadingError);
			return true;
		}
		if (m_Request != null)
		{
			return m_Request.isDone;
		}
		return false;
	}
}
