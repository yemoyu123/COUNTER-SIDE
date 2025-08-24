using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Shop;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class ShopLevelUpPackageGroupTemplet : INKMTemplet
{
	private readonly int packageID;

	private readonly List<ShopLevelUpPackageGroupData> datas = new List<ShopLevelUpPackageGroupData>();

	private int maxLevelRequire;

	private ShopItemTemplet shopTemplet;

	public int Key => packageID;

	public int MaxLevelRequire => maxLevelRequire;

	public ShopItemTemplet ShopTemplet => shopTemplet;

	public ShopLevelUpPackageGroupTemplet(int packageID, List<ShopLevelUpPackageGroupData> datas)
	{
		if (datas == null || datas.Count == 0)
		{
			Log.ErrorAndExit($"invalid data list. packageID:{packageID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopLevelUpPackageGroupTemplet.cs", 20);
			return;
		}
		this.packageID = packageID;
		this.datas.AddRange(datas);
		maxLevelRequire = datas.Max((ShopLevelUpPackageGroupData e) => e.LevelRequire);
	}

	public static ShopLevelUpPackageGroupTemplet Find(int key)
	{
		return NKMTempletContainer<ShopLevelUpPackageGroupTemplet>.Find(key);
	}

	public static void ValidateServerOnly()
	{
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (value.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.LEVELUP_PACKAGE && Find(value.m_PurchaseEventValue) == null)
			{
				NKMTempletError.Add($"[ShopTemplet] 상점 레벨업 패키지 정보가 존재하지 않음 m_ProductID : {value.m_ProductID}, m_PurchaseEventValue : {value.m_PurchaseEventValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopLevelUpPackageGroupTemplet.cs", 43);
			}
		}
	}

	public IEnumerable<ShopLevelUpPackageGroupData> GetGroupDatas(int minLevel, int maxLevel)
	{
		return datas.Where((ShopLevelUpPackageGroupData e) => e.LevelRequire > minLevel && e.LevelRequire <= maxLevel);
	}

	public void Join()
	{
		foreach (ShopLevelUpPackageGroupData data in datas)
		{
			data.Join();
		}
		IEnumerable<ShopItemTemplet> source = NKMTempletContainer<ShopItemTemplet>.Values.Where((ShopItemTemplet e) => e.m_bEnabled && e.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.LEVELUP_PACKAGE && e.m_PurchaseEventValue == packageID);
		if (source.Count() <= 0)
		{
			NKMTempletError.Add($"[ShopLevelUpPackageTemplet] 해당 레벨업 패키지 ID와 연결된 상품이 존재하지 않음 m_PackageID : {packageID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopLevelUpPackageGroupTemplet.cs", 66);
		}
		if (source.Count() > 1)
		{
			NKMTempletError.Add(string.Format("[ShopLevelUpPackageTemplet] 동일한 레벨업 패키지 ID를 가지는 상품이 두개 이상 존재함 m_PackageID : {0}, ProductIds : {1}", packageID, string.Join(",", source.Select((ShopItemTemplet e) => e.m_ProductID))), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopLevelUpPackageGroupTemplet.cs", 71);
		}
		shopTemplet = source.FirstOrDefault();
	}

	public void Validate()
	{
		foreach (ShopLevelUpPackageGroupData data in datas)
		{
			data.Validate();
		}
	}
}
