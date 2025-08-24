using System;

namespace NKM;

public class NKMAssetName : IEquatable<NKMAssetName>
{
	public string m_BundleName = string.Empty;

	public string m_AssetName = string.Empty;

	public NKMAssetName()
	{
		Init();
	}

	public static NKMAssetName ParseBundleName(string defaultBundleName, string assetName)
	{
		if (string.IsNullOrEmpty(assetName))
		{
			return new NKMAssetName(defaultBundleName, assetName);
		}
		if (!assetName.Contains("@"))
		{
			return new NKMAssetName(defaultBundleName, assetName);
		}
		char[] separator = new char[1] { '@' };
		string[] array = assetName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length > 1)
		{
			return new NKMAssetName(array[0], array[1]);
		}
		return new NKMAssetName(defaultBundleName, assetName);
	}

	public static NKMAssetName ParseBundleName(string defaultBundleName, string assetName, string assetNamePrefix)
	{
		if (string.IsNullOrEmpty(assetName))
		{
			return new NKMAssetName(defaultBundleName, assetName);
		}
		char[] separator = new char[1] { '@' };
		string[] array = assetName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length > 1)
		{
			return new NKMAssetName(array[0], assetNamePrefix + array[1]);
		}
		return new NKMAssetName(defaultBundleName, assetNamePrefix + assetName);
	}

	public NKMAssetName(string bundleName, string assetName)
	{
		if (string.IsNullOrEmpty(assetName) || !assetName.Contains("@"))
		{
			m_BundleName = bundleName;
			m_AssetName = assetName;
			return;
		}
		char[] separator = new char[1] { '@' };
		string[] array = assetName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length > 1)
		{
			m_BundleName = array[0];
			m_AssetName = array[1];
		}
		else
		{
			m_BundleName = bundleName;
			m_AssetName = assetName;
		}
	}

	public void Init()
	{
		m_BundleName = string.Empty;
		m_AssetName = string.Empty;
	}

	public void DeepCopyFromSource(NKMAssetName source)
	{
		m_BundleName = source.m_BundleName;
		m_AssetName = source.m_AssetName;
	}

	public void LoadFromLua(NKMLua cNKMLua)
	{
		cNKMLua.GetData(1, ref m_BundleName);
		cNKMLua.GetData(2, ref m_AssetName);
	}

	public bool LoadFromLua(NKMLua cNKMLua, string pKey)
	{
		if (cNKMLua.OpenTable(pKey))
		{
			LoadFromLua(cNKMLua);
			cNKMLua.CloseTable();
			return true;
		}
		return false;
	}

	public bool LoadFromLua(NKMLua cNKMLua, int index)
	{
		if (cNKMLua.OpenTable(index))
		{
			LoadFromLua(cNKMLua);
			cNKMLua.CloseTable();
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return m_BundleName.GetHashCode() * 31 + m_AssetName.GetHashCode();
	}

	public override string ToString()
	{
		return $"{m_BundleName}@{m_AssetName}";
	}

	public bool Equals(NKMAssetName other)
	{
		if (m_BundleName == other.m_BundleName)
		{
			return m_AssetName == other.m_AssetName;
		}
		return false;
	}
}
