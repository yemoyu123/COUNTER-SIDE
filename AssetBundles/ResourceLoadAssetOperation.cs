using UnityEngine;

namespace AssetBundles;

public class ResourceLoadAssetOperation : AssetBundleLoadAssetOperation
{
	private Object m_Asset;

	public ResourceLoadAssetOperation(string resourcePath)
	{
		m_Asset = Resources.Load(resourcePath);
		if (m_Asset == null)
		{
			Debug.LogError(resourcePath + " not found from internal resources");
		}
	}

	public override T GetAsset<T>()
	{
		return m_Asset as T;
	}

	public override bool Update()
	{
		return false;
	}

	public override bool IsDone()
	{
		return true;
	}
}
