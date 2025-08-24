using System.IO;
using UnityEngine;

namespace AssetBundles;

public class AssetBundleLoadFromFileOperation : AssetBundleDownloadOperation
{
	private string m_Path;

	private AssetBundleCreateRequest m_Request;

	private Stream m_Stream;

	protected override bool downloadIsDone
	{
		get
		{
			if (m_Request != null)
			{
				return m_Request.isDone;
			}
			return true;
		}
	}

	public AssetBundleLoadFromFileOperation(string assetBundleName, string path)
		: base(assetBundleName)
	{
		m_Stream = AssetBundleManager.OpenCryptoBundleFileStream(path);
		m_Request = AssetBundle.LoadFromStreamAsync(m_Stream);
		m_Path = path;
	}

	protected override void FinishDownload()
	{
		if (m_Request != null)
		{
			if (m_Request.assetBundle == null)
			{
				base.error = $"{base.assetBundleName} is not a valid asset bundle.";
			}
			else
			{
				base.assetBundle = new LoadedAssetBundle(m_Request.assetBundle, m_Stream);
			}
		}
		m_Request = null;
	}

	public override string GetSourceURL()
	{
		return m_Path;
	}
}
