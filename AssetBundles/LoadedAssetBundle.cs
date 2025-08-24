using System;
using System.IO;
using UnityEngine;

namespace AssetBundles;

public class LoadedAssetBundle
{
	public AssetBundle m_AssetBundle;

	public int m_ReferencedCount;

	public Stream m_Stream;

	public bool IsFullyLoaded;

	internal event Action unload;

	internal void OnUnload()
	{
		if (m_AssetBundle != null)
		{
			m_AssetBundle.Unload(unloadAllLoadedObjects: false);
		}
		if (this.unload != null)
		{
			this.unload();
		}
		m_Stream?.Dispose();
		m_AssetBundle = null;
		m_Stream = null;
		this.unload = null;
	}

	public LoadedAssetBundle(AssetBundle assetBundle, Stream stream)
	{
		m_AssetBundle = assetBundle;
		m_Stream = stream;
		m_ReferencedCount = 1;
	}
}
