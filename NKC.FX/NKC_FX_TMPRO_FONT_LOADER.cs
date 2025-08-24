using TMPro;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_TMPRO_FONT_LOADER : MonoBehaviour
{
	public string BundleName = "ab_fx_font_unit_c_hb_miya";

	public string AssetName = "NK_TMFONT_IDOL";

	private TextMeshPro[] targets;

	private TMP_FontAsset fontAsset;

	private bool isLoaded;

	private void OnDestroy()
	{
		if (targets != null)
		{
			targets = null;
		}
		if (fontAsset != null)
		{
			fontAsset = null;
		}
		isLoaded = false;
	}

	private void Start()
	{
		LoadFontAsset();
	}

	private void LoadFontAsset()
	{
		if (isLoaded)
		{
			return;
		}
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<TMP_FontAsset>(BundleName, AssetName);
		if (nKCAssetResourceData != null)
		{
			fontAsset = nKCAssetResourceData.GetAsset<TMP_FontAsset>();
			if (fontAsset != null)
			{
				targets = GetComponentsInChildren<TextMeshPro>(includeInactive: true);
				if (targets != null && targets.Length != 0)
				{
					for (int i = 0; i < targets.Length; i++)
					{
						targets[i].font = fontAsset;
					}
					isLoaded = true;
				}
			}
			else
			{
				Debug.LogWarning("null fontAsset");
			}
		}
		else
		{
			Debug.LogWarning("null resource");
		}
	}
}
