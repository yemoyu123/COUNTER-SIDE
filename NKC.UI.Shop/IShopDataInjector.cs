using NKM.Shop;

namespace NKC.UI.Shop;

public interface IShopDataInjector
{
	void TriggerInjectData(ShopItemTemplet productTemplet);
}
