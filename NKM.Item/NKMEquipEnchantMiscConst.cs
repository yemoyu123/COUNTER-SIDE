using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Item;

public class NKMEquipEnchantMiscConst
{
	private readonly NKMEquipEnchantMaterial[] materials = new NKMEquipEnchantMaterial[4];

	public int MaxMaterialUsageLimit;

	public IReadOnlyList<NKMEquipEnchantMaterial> Materials => materials;

	public IEnumerable<int> MaterialIds => materials.Select((NKMEquipEnchantMaterial e) => e?.ItemId ?? 0);

	public NKMEquipEnchantMaterial GetMaterial(int materialId)
	{
		return materials.FirstOrDefault((NKMEquipEnchantMaterial e) => e.ItemId == materialId);
	}

	public void LoadFromLua(NKMLua lua)
	{
		lua.GetData("MaxMaterialUsageLimit", ref MaxMaterialUsageLimit);
		using (lua.OpenTable("Materials", "[Negotiation] loading Materials table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipEnchantMiscConst.cs", 67))
		{
			for (int i = 0; i < materials.Length; i++)
			{
				int iIndex = i + 1;
				if (lua.OpenTable(iIndex))
				{
					materials[i] = new NKMEquipEnchantMaterial(i);
					materials[i].LoadFromLua(lua);
					lua.CloseTable();
				}
			}
		}
	}

	public void Join()
	{
		NKMEquipEnchantMaterial[] array = materials;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Join();
		}
	}

	public void Validate()
	{
		if ((from e in materials
			group e by e.ItemId).Any((IGrouping<int, NKMEquipEnchantMaterial> e) => e.Count() > 1))
		{
			string text = string.Join(", ", materials.Select((NKMEquipEnchantMaterial e) => e.ItemId));
			NKMTempletError.Add("[EquipEnchantMisc] duplicated material exist. itemId:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipEnchantMiscConst.cs", 96);
		}
		if (materials.Any((NKMEquipEnchantMaterial e) => e.ItemId == 1))
		{
			NKMTempletError.Add("[EquipEnchantMisc] credit cannot be a material.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMEquipEnchantMiscConst.cs", 102);
		}
		NKMEquipEnchantMaterial[] array = materials;
		for (int num = 0; num < array.Length; num++)
		{
			array[num].Validate();
		}
	}
}
