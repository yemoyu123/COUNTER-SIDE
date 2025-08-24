using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Negotiation;
using Cs.Logging;
using Cs.Math.Lottery;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NegotiationTemplet
{
	private readonly NegotiationMaterial[] materials = new NegotiationMaterial[3];

	private readonly RateLottery<NEGOTIATE_RESULT> normalLottery = new RateLottery<NEGOTIATE_RESULT>(NEGOTIATE_RESULT.COMPLETE);

	private readonly RateLottery<NEGOTIATE_RESULT> bonusLottery = new RateLottery<NEGOTIATE_RESULT>(NEGOTIATE_RESULT.COMPLETE);

	public int Passion_CreditDecreasePercent;

	public int Normal_ResultSuccessPercent;

	public int Bonus_CreditIncreasePercent;

	public int Bonus_LoyaltyIncreasePercent;

	public int Bonus_ResultSuccessPercent;

	public int Result_SuccessAdditionalExpPercent;

	public int Result_PermanentDealBonusPercent;

	public int MaxMaterialUsageLimit;

	public IReadOnlyList<NegotiationMaterial> Materials => materials;

	public IEnumerable<NegotiationMaterial> ValidMaterials => materials.Where((NegotiationMaterial e) => e != null);

	public void LoadFromLua(NKMLua lua)
	{
		lua.GetData("MaxMaterialUsageLimit", ref MaxMaterialUsageLimit);
		using (lua.OpenTable("Materials", "[Negotiation] loading Materials table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 34))
		{
			for (int i = 0; i < materials.Length; i++)
			{
				int iIndex = i + 1;
				if (lua.OpenTable(iIndex))
				{
					materials[i] = new NegotiationMaterial(i);
					materials[i].LoadFromLua(lua);
					lua.CloseTable();
				}
			}
		}
		bool flag = true;
		using (lua.OpenTable("Selections", "[Negotiation] loading Selections table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 49))
		{
			using (lua.OpenTable("Passion", "[Negotiaton] loading Selections.Passion table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 51))
			{
				flag &= lua.GetData("CreditDecreasePercent", ref Passion_CreditDecreasePercent);
			}
			using (lua.OpenTable("Normal", "[Negotiation] loading Selections.Normal table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 56))
			{
				flag &= lua.GetData("ResultSuccessPercent", ref Normal_ResultSuccessPercent);
			}
			using (lua.OpenTable("Bonus", "[Negotiation] loading Selections.Bonus table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 61))
			{
				flag &= lua.GetData("CreditIncreasePercent", ref Bonus_CreditIncreasePercent);
				flag &= lua.GetData("LoyaltyIncreasePercent", ref Bonus_LoyaltyIncreasePercent);
				flag &= lua.GetData("ResultSuccessPercent", ref Bonus_ResultSuccessPercent);
			}
		}
		using (lua.OpenTable("ResultOptions", "[Negotiation] loading ResultOptions table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 69))
		{
			flag &= lua.GetData("SuccessAdditionalExpPercent", ref Result_SuccessAdditionalExpPercent);
			flag &= lua.GetData("PermanentDealExpBonusPercent", ref Result_PermanentDealBonusPercent);
		}
		if (!flag)
		{
			Log.ErrorAndExit("[Negotiation] loading data failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 77);
		}
	}

	public void Join()
	{
		foreach (NegotiationMaterial validMaterial in ValidMaterials)
		{
			validMaterial.Join();
		}
		normalLottery.AddCase(Normal_ResultSuccessPercent * 100, NEGOTIATE_RESULT.SUCCESS);
		bonusLottery.AddCase(Bonus_ResultSuccessPercent * 100, NEGOTIATE_RESULT.SUCCESS);
	}

	public void Validate()
	{
		foreach (NegotiationMaterial validMaterial in ValidMaterials)
		{
			validMaterial.Validate();
		}
		if ((from e in ValidMaterials
			group e by e.ItemId).Any((IGrouping<int, NegotiationMaterial> e) => e.Count() > 1))
		{
			string text = string.Join(", ", ValidMaterials.Select((NegotiationMaterial e) => e.ItemId));
			NKMTempletError.Add("[Negotiation] duplicated material exist. itemId:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 103);
		}
		if (ValidMaterials.Any((NegotiationMaterial e) => e.ItemId == 1))
		{
			NKMTempletError.Add("[Negotiation] credit cannot be a material.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 109);
		}
		if (Passion_CreditDecreasePercent <= 0 || Passion_CreditDecreasePercent >= 100)
		{
			NKMTempletError.Add($"[Negotiation] invalid Passion_CreditDecreasePercent:{Passion_CreditDecreasePercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 115);
		}
		if (Normal_ResultSuccessPercent <= 0 || Normal_ResultSuccessPercent >= 100)
		{
			NKMTempletError.Add($"[Negotiation] invalid Normal_ResultSuccessPercent:{Normal_ResultSuccessPercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 120);
		}
		if (Bonus_CreditIncreasePercent <= 0 || Bonus_CreditIncreasePercent >= 100)
		{
			NKMTempletError.Add($"[Negotiation] invalid Bonus_CreditIncreasePercent:{Bonus_CreditIncreasePercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 125);
		}
		if (Bonus_LoyaltyIncreasePercent <= 0 || Bonus_LoyaltyIncreasePercent > 100)
		{
			NKMTempletError.Add($"[Negotiation] invalid Bonus_LoyaltyIncreasePercent:{Bonus_LoyaltyIncreasePercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 130);
		}
		if (Bonus_ResultSuccessPercent <= 0 || Bonus_ResultSuccessPercent >= 100)
		{
			NKMTempletError.Add($"[Negotiation] invalid Bonus_ResultSuccessPercent:{Bonus_ResultSuccessPercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 135);
		}
		if (Result_SuccessAdditionalExpPercent <= 0 || Result_SuccessAdditionalExpPercent >= 100)
		{
			NKMTempletError.Add($"[Negotiation] invalid Result_SuccessAdditionalExpPercent:{Result_SuccessAdditionalExpPercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 140);
		}
		if (Result_PermanentDealBonusPercent <= 0 || Result_PermanentDealBonusPercent >= 100)
		{
			NKMTempletError.Add($"[Negotiation] invalid Result_PermanentDealBonusPercent:{Result_PermanentDealBonusPercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 145);
		}
		if (Bonus_ResultSuccessPercent <= Normal_ResultSuccessPercent)
		{
			NKMTempletError.Add($"[Negotiaton] Bonus_ResultSuccessPercent({Bonus_ResultSuccessPercent}) should be greater than Normal_ResultSuccessPercent({Normal_ResultSuccessPercent})", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 151);
		}
		if (Result_SuccessAdditionalExpPercent + Result_PermanentDealBonusPercent >= 100)
		{
			NKMTempletError.Add("[Negotiaton] exp modification percent is too high.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Negotiation/NegotiationTemplet.cs", 157);
		}
	}

	public bool TryGetMaterial(int itemId, out NegotiationMaterial materialTemplet)
	{
		materialTemplet = materials.FirstOrDefault((NegotiationMaterial e) => e.ItemId == itemId);
		return materialTemplet != null;
	}

	public int CalcCreditCost(NEGOTIATE_BOSS_SELECTION selection, int creditCount)
	{
		return selection switch
		{
			NEGOTIATE_BOSS_SELECTION.OK => creditCount, 
			NEGOTIATE_BOSS_SELECTION.PASSION => (int)((float)(creditCount * (100 - Passion_CreditDecreasePercent)) * 0.01f), 
			NEGOTIATE_BOSS_SELECTION.RAISE => (int)((float)(creditCount * (100 + Bonus_CreditIncreasePercent)) * 0.01f), 
			_ => throw new Exception($"unknown enum value:{selection}"), 
		};
	}

	public NEGOTIATE_RESULT DecideResult(NEGOTIATE_BOSS_SELECTION selection)
	{
		return selection switch
		{
			NEGOTIATE_BOSS_SELECTION.PASSION => NEGOTIATE_RESULT.COMPLETE, 
			NEGOTIATE_BOSS_SELECTION.OK => normalLottery.Decide(), 
			NEGOTIATE_BOSS_SELECTION.RAISE => bonusLottery.Decide(), 
			_ => throw new Exception($"unknown enum value:{selection}"), 
		};
	}
}
