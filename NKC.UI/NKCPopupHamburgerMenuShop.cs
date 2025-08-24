using NKM;
using NKM.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupHamburgerMenuShop : NKCPopupHamburgerMenuSimpleButton
{
	public GameObject m_objReddot_RED;

	public GameObject m_objReddot_YELLOW;

	public Text m_lbReddotCount;

	protected override void ContentsUpdate(NKMUserData userData)
	{
		ShopReddotType reddotType;
		int reddotCount = NKCShopManager.CheckTabReddotCount(out reddotType);
		NKCUtil.SetShopReddotImage(reddotType, m_objReddot, m_objReddot_RED, m_objReddot_YELLOW);
		NKCUtil.SetShopReddotLabel(reddotType, m_lbReddotCount, reddotCount);
	}
}
