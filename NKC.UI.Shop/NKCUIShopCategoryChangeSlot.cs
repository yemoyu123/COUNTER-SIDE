using NKC.Templet;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCUIShopCategoryChangeSlot : MonoBehaviour
{
	public delegate void OnSelectCategory(NKCShopManager.ShopTabCategory category);

	public NKCShopManager.ShopTabCategory m_eCategory;

	public NKCUIComStateButton m_Button;

	public Image m_Image;

	public Text m_lbName;

	public GameObject m_objReddotRoot;

	public GameObject m_objReddot_RED;

	public GameObject m_objReddot_YELLOW;

	public Text m_lbReddotCount;

	private OnSelectCategory dOnSelectCategory;

	public void Init(OnSelectCategory onSelectCategory)
	{
		NKCUtil.SetButtonClickDelegate(m_Button, OnButton);
		NKCShopCategoryTemplet nKCShopCategoryTemplet = NKCShopCategoryTemplet.Find(m_eCategory);
		dOnSelectCategory = onSelectCategory;
		SetData(nKCShopCategoryTemplet);
		if (nKCShopCategoryTemplet == null)
		{
			Debug.LogError($"Shop Category Templet for {m_eCategory} not exist!");
		}
	}

	public void SetData(NKCShopCategoryTemplet templet)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(this, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(this, bValue: true);
		NKCUtil.SetImageSprite(m_Image, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_shop_thumbnail", templet.m_ThumbnailImg, tryParseAssetName: true));
		NKCUtil.SetLabelText(m_lbName, NKCStringTable.GetString(templet.m_TabCategoryName));
		CheckReddot();
	}

	private void OnButton()
	{
		dOnSelectCategory?.Invoke(m_eCategory);
	}

	public void CheckReddot()
	{
		int num = 0;
		ShopReddotType shopReddotType = ShopReddotType.NONE;
		NKCShopCategoryTemplet nKCShopCategoryTemplet = NKCShopCategoryTemplet.Find(m_eCategory);
		if (nKCShopCategoryTemplet == null)
		{
			return;
		}
		for (int i = 0; i < nKCShopCategoryTemplet.m_UseTabID.Count; i++)
		{
			num += NKCShopManager.CheckTabReddotCount(out var reddotType, nKCShopCategoryTemplet.m_UseTabID[i]);
			if (shopReddotType < reddotType)
			{
				shopReddotType = reddotType;
			}
		}
		NKCUtil.SetShopReddotImage(shopReddotType, m_objReddotRoot, m_objReddot_RED, m_objReddot_YELLOW);
		NKCUtil.SetShopReddotLabel(shopReddotType, m_lbReddotCount, num);
	}
}
