using NKM.Shop;

namespace NKC.UI.Shop;

public interface IShopPrefab
{
	void SetData(ShopItemTemplet productTemplet);

	bool IsHideLockObject();
}
