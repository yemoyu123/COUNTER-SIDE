using Spine.Unity;
using UnityEngine;

namespace NKC.UI;

public class NKCUIComTextureLoader : MonoBehaviour, INKCUIValidator
{
	public string m_BundleName;

	public string m_AssetName;

	private bool m_bValidate;

	public SkeletonGraphic m_TagetSkeleton;

	private void Awake()
	{
		Validate();
	}

	public void Validate()
	{
		if (m_bValidate)
		{
			return;
		}
		m_bValidate = true;
		if (null == m_TagetSkeleton || null == m_TagetSkeleton.skeletonDataAsset)
		{
			return;
		}
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<Texture>(m_BundleName, m_AssetName);
		if (nKCAssetResourceData == null)
		{
			return;
		}
		Texture asset = nKCAssetResourceData.GetAsset<Texture>();
		if (null == asset)
		{
			return;
		}
		AtlasAssetBase[] atlasAssets = m_TagetSkeleton.skeletonDataAsset.atlasAssets;
		foreach (AtlasAssetBase atlasAssetBase in atlasAssets)
		{
			if (null == atlasAssetBase)
			{
				continue;
			}
			foreach (Material material in atlasAssetBase.Materials)
			{
				material.mainTexture = asset;
			}
		}
	}
}
