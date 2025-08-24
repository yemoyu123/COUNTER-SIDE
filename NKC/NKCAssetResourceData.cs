using System;
using AssetBundles;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCAssetResourceData
{
	public Type m_resType;

	public NKMAssetName m_NKMAssetName;

	protected UnityEngine.Object m_Asset;

	public int m_RefCount;

	public bool m_bAsync;

	public CallBackHandler callBack;

	public bool m_bLoadFail;

	public float m_fTime;

	public AssetBundleLoadAssetOperation m_Operation { get; protected set; }

	public bool GetLoadFail()
	{
		return m_bLoadFail;
	}

	public NKCAssetResourceData(string bundleName, string assetName, bool bAsync)
	{
		m_NKMAssetName = new NKMAssetName();
		Init(bundleName, assetName, bAsync);
	}

	public void Init(string bundleName, string assetName, bool bAsync)
	{
		Unload();
		m_NKMAssetName.m_BundleName = bundleName;
		m_NKMAssetName.m_AssetName = assetName;
		m_RefCount = 1;
		m_bAsync = bAsync;
		m_bLoadFail = false;
	}

	public void BeginLoad<T>() where T : UnityEngine.Object
	{
		m_resType = typeof(T);
		m_fTime = Time.time;
		if (m_bAsync)
		{
			m_Operation = AssetBundleManager.LoadAssetAsync(m_NKMAssetName.m_BundleName, m_NKMAssetName.m_AssetName, typeof(T));
		}
		else
		{
			m_Asset = AssetBundleManager.LoadAsset<T>(m_NKMAssetName.m_BundleName, m_NKMAssetName.m_AssetName);
		}
	}

	public T GetAsset<T>() where T : UnityEngine.Object
	{
		if (m_bAsync)
		{
			if (m_Operation == null)
			{
				Debug.LogError("Asset Load Operation Null! " + m_NKMAssetName.m_BundleName + " " + m_NKMAssetName.m_AssetName);
				m_bLoadFail = true;
				return null;
			}
			if (!IsDone())
			{
				Debug.LogWarning("Tried loading too soon, wait for IsDone()");
				return null;
			}
			if (m_Asset == null)
			{
				m_Asset = m_Operation.GetAsset<T>();
			}
		}
		return m_Asset as T;
	}

	public bool IsDone()
	{
		if (m_bAsync)
		{
			if (m_Operation == null)
			{
				return true;
			}
			return m_Operation.IsDone();
		}
		return true;
	}

	public void Unload()
	{
		if (m_NKMAssetName.m_BundleName.Length > 1)
		{
			AssetBundleManager.UnloadAssetBundle(m_NKMAssetName.m_BundleName);
		}
		m_resType = null;
		m_NKMAssetName.m_AssetName = "";
		m_NKMAssetName.m_BundleName = "";
		m_Asset = null;
		m_RefCount = 0;
		callBack = null;
		m_Operation = null;
	}

	public void ForceSyncLoad()
	{
		if (m_bAsync && m_Operation != null && !IsDone())
		{
			m_Operation = null;
			m_bAsync = false;
			m_Asset = AssetBundleManager.LoadAsset<UnityEngine.Object>(m_NKMAssetName.m_BundleName, m_NKMAssetName.m_AssetName);
		}
	}
}
