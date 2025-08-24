using System;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMUserExpTemplet : INKMTemplet
{
	public int m_iLevel;

	public int m_lExpRequired;

	public long m_lExpCumulated;

	public int m_iCreditMaxSupply;

	public int m_iEterniumMaxSupply;

	public long m_EterniumCap;

	public int m_EterniumLevelupReward;

	public long m_RechargeEternium;

	public string m_strLevelUpDesc;

	public float m_iCreditPerMinute;

	public float m_iEterniumPerMinute;

	public int Key => m_iLevel;

	public static NKMUserExpTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMUserExpTemplet nKMUserExpTemplet = new NKMUserExpTemplet();
		cNKMLua.GetData("m_iLevel", ref nKMUserExpTemplet.m_iLevel);
		cNKMLua.GetData("m_lExpRequired", ref nKMUserExpTemplet.m_lExpRequired);
		cNKMLua.GetData("m_lExpCumulated", ref nKMUserExpTemplet.m_lExpCumulated);
		cNKMLua.GetData("m_iCreditMaxSupply", ref nKMUserExpTemplet.m_iCreditMaxSupply);
		cNKMLua.GetData("m_iEterniumMaxSupply", ref nKMUserExpTemplet.m_iEterniumMaxSupply);
		cNKMLua.GetData("m_Eternium_MaxCap_Level", ref nKMUserExpTemplet.m_EterniumCap);
		cNKMLua.GetData("m_Eternium_Reward_Level", ref nKMUserExpTemplet.m_EterniumLevelupReward);
		cNKMLua.GetData("m_RechargeEternium", ref nKMUserExpTemplet.m_RechargeEternium);
		cNKMLua.GetData("m_strLevelUpDesc", ref nKMUserExpTemplet.m_strLevelUpDesc);
		nKMUserExpTemplet.m_iCreditPerMinute = (float)nKMUserExpTemplet.m_iCreditMaxSupply / 480f;
		nKMUserExpTemplet.m_iEterniumPerMinute = (float)nKMUserExpTemplet.m_iEterniumMaxSupply / 480f;
		return nKMUserExpTemplet;
	}

	public static NKMUserExpTemplet Find(int key)
	{
		return NKMTempletContainer<NKMUserExpTemplet>.Find(key);
	}

	public int CalculateAutoSupplyCredit(int durationMinute)
	{
		return Math.Min((int)Math.Truncate(m_iCreditPerMinute * (float)durationMinute), m_iCreditMaxSupply);
	}

	public int CalculateAutoSupplyEternium(int durationMinute)
	{
		return Math.Min((int)Math.Truncate(m_iEterniumPerMinute * (float)durationMinute), m_iEterniumMaxSupply);
	}

	public int CalcAutoSupplyEterniumRemainMinute(long remainValue)
	{
		return (int)Math.Truncate((float)remainValue / m_iEterniumPerMinute);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_iLevel <= 0)
		{
			Log.ErrorAndExit($"[AutoSupply] m_iCreditPerMinute < 0, id:{Key} m_iCreditPerMinute:{m_iCreditPerMinute}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUserExpTemplet.cs", 78);
		}
		if (m_lExpRequired < 0)
		{
			Log.ErrorAndExit($"[AutoSupply] m_lExpRequired < 0, id:{Key} m_lExpRequired:{m_lExpRequired}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUserExpTemplet.cs", 83);
		}
		if (m_lExpCumulated < 0)
		{
			Log.ErrorAndExit($"[AutoSupply] m_lExpCumulated < 0, id:{Key} m_lExpCumulated:{m_lExpCumulated}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUserExpTemplet.cs", 88);
		}
		if (m_iCreditPerMinute < 0f)
		{
			Log.ErrorAndExit($"[AutoSupply] m_iCreditPerMinute < 0, id:{Key} m_iCreditPerMinute:{m_iCreditPerMinute}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUserExpTemplet.cs", 93);
		}
		if (m_iCreditMaxSupply < 0)
		{
			Log.ErrorAndExit($"[AutoSupply] m_iCreditMaxSupply < 0, id:{Key} m_iCreditMaxSupply:{m_iCreditMaxSupply}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUserExpTemplet.cs", 98);
		}
		if (m_iEterniumPerMinute < 0f)
		{
			Log.ErrorAndExit($"[AutoSupply] m_iEterniumPerMinute < 0, id:{Key} m_iEterniumPerMinute:{m_iEterniumPerMinute}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUserExpTemplet.cs", 103);
		}
		if (m_iEterniumMaxSupply < 0)
		{
			Log.ErrorAndExit($"[AutoSupply] m_iEterniumMaxSupply < 0, id:{Key} m_iEterniumMaxSupply:{m_iEterniumMaxSupply}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUserExpTemplet.cs", 108);
		}
		if (m_RechargeEternium <= 0)
		{
			Log.ErrorAndExit($"[AutoSupply] m_RechargeEternium <= 0, id:{Key} m_RechargeEternium:{m_RechargeEternium}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUserExpTemplet.cs", 113);
		}
	}
}
