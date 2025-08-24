using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Item;

public class NKMEquipEnchantMaterial
{
	private int miscItemId;

	private int exp;

	public NKMItemMiscTemplet ItemTemplet { get; private set; }

	public int Index { get; }

	public int ItemId => miscItemId;

	public int Exp => exp;

	internal NKMEquipEnchantMaterial(int index)
	{
		Index = index;
	}

	public bool LoadFromLua(NKMLua lua)
	{
		int num = (int)(1u & (lua.GetData("ItemId", ref miscItemId) ? 1u : 0u)) & (lua.GetData("Exp", ref exp) ? 1 : 0);
		if (num == 0)
		{
			Log.Error($"[EquipEnchant] loading material data failed. misc id : {miscItemId}, exp : {Exp}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipEnchantMiscConst.cs", 30);
		}
		return (byte)num != 0;
	}

	public void Join()
	{
		ItemTemplet = NKMItemManager.GetItemMiscTempletByID(miscItemId);
		if (ItemTemplet == null)
		{
			NKMTempletError.Add($"[EquipEnchant] invalid material item id:{miscItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipEnchantMiscConst.cs", 41);
		}
	}

	public void Validate()
	{
		if (exp <= 0)
		{
			NKMTempletError.Add($"[EquipEnchant] material has invalid exp. itemId:{miscItemId} exp:{exp}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipEnchantMiscConst.cs", 49);
		}
	}
}
