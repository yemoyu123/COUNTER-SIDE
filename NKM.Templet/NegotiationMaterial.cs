using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NegotiationMaterial
{
	private int miscItemId;

	private int exp;

	private int loyalty;

	private int credit;

	public NKMItemMiscTemplet ItemTemplet { get; private set; }

	public int Index { get; }

	public int ItemId => miscItemId;

	public int Exp => exp;

	public int Loyalty => loyalty;

	public int Credit => credit;

	internal NegotiationMaterial(int index)
	{
		Index = index;
	}

	public bool LoadFromLua(NKMLua lua)
	{
		int num = (int)(1u & (lua.GetData("ItemId", ref miscItemId) ? 1u : 0u) & (lua.GetData("Exp", ref exp) ? 1u : 0u) & (lua.GetData("Loyalty", ref loyalty) ? 1u : 0u)) & (lua.GetData("Credit", ref credit) ? 1 : 0);
		if (num == 0)
		{
			Log.Error("[Negotiation] loading material data failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationMaterial.cs", 28);
		}
		return (byte)num != 0;
	}

	public void Join()
	{
		ItemTemplet = NKMItemManager.GetItemMiscTempletByID(miscItemId);
		if (ItemTemplet == null)
		{
			NKMTempletError.Add($"[Negotiation] invalid material item id:{miscItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationMaterial.cs", 46);
		}
	}

	public void Validate()
	{
		if (exp <= 0)
		{
			NKMTempletError.Add($"[Negotiaton] material has invalid exp. itemId:{miscItemId} exp:{exp}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationMaterial.cs", 54);
		}
		if (loyalty <= 0)
		{
			NKMTempletError.Add($"[Negotiaton] material has invalid loyalty. itemId:{miscItemId} loyalty:{loyalty}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationMaterial.cs", 59);
		}
		if (credit <= 0)
		{
			NKMTempletError.Add($"[Negotiaton] material has invalid credit. itemId:{miscItemId} credit:{credit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationMaterial.cs", 64);
		}
	}
}
