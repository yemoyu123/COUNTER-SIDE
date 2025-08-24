using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

[RequireComponent(typeof(Image))]
public class NKCUIComSpriteLoader : MonoBehaviour, INKCUIValidator
{
	public string m_BundleName;

	public string m_AssetName;

	private bool m_bValidate;

	private Image m_image;

	private void Awake()
	{
		Validate();
	}

	public void Validate()
	{
		if (!m_bValidate)
		{
			m_bValidate = true;
			if (m_image == null)
			{
				m_image = GetComponent<Image>();
			}
			if (NKCStringTable.GetNationalCode() != NKM_NATIONAL_CODE.NNC_KOREA && NKCAssetResourceManager.IsAssetInLocBundleCheckAll(m_BundleName, m_AssetName))
			{
				NKCUtil.SetImageSprite(m_image, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(m_BundleName, m_AssetName));
			}
			else if (m_image != null && m_image.sprite == null)
			{
				m_image.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(m_BundleName, m_AssetName);
			}
		}
	}
}
