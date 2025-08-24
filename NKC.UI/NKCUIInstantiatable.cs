using UnityEngine;

namespace NKC.UI;

public class NKCUIInstantiatable : MonoBehaviour
{
	protected static T OpenInstance<T>(string BundleName, string AssetName, Transform parent) where T : NKCUIInstantiatable
	{
		GameObject asset = NKCAssetResourceManager.OpenResource<GameObject>(BundleName, AssetName).GetAsset<GameObject>();
		if (asset != null)
		{
			T component = Object.Instantiate(asset, parent).GetComponent<T>();
			if (component != null)
			{
				NKCUIManager.OpenUI(component.gameObject);
			}
			return component;
		}
		return null;
	}

	protected void CloseInstance(string BundleName, string AssetName)
	{
		NKCAssetResourceManager.CloseResource(BundleName, AssetName);
		Object.Destroy(base.gameObject);
	}
}
