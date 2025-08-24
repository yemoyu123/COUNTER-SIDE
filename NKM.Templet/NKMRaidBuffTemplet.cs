using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMRaidBuffTemplet : INKMTemplet
{
	private int ID;

	private string RAID_BUFF_NAME = "";

	private int LEVEL;

	private int raidBuffCost;

	private readonly HashSet<string> BUFF_STR_ID_LIST = new HashSet<string>();

	private float COST_CHARGE_RATE;

	private readonly HashSet<NKMBuffTemplet> BUFF_TEMPLET_LIST = new HashSet<NKMBuffTemplet>();

	public int Key => (ID, LEVEL).GetHashCode();

	public string RaidBuffName => RAID_BUFF_NAME;

	public int Level => LEVEL;

	public int RaidBuffCost => raidBuffCost;

	public HashSet<NKMBuffTemplet> BuffTempletList => BUFF_TEMPLET_LIST;

	public float CostChargeRate => COST_CHARGE_RATE;

	public static NKMRaidBuffTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMRaidBuffTemplet nKMRaidBuffTemplet = new NKMRaidBuffTemplet();
		int num = (int)(1u & (cNKMLua.GetData("ID", ref nKMRaidBuffTemplet.ID) ? 1u : 0u) & (cNKMLua.GetData("RAID_BUFF_NAME", ref nKMRaidBuffTemplet.RAID_BUFF_NAME) ? 1u : 0u) & (cNKMLua.GetData("LEVEL", ref nKMRaidBuffTemplet.LEVEL) ? 1u : 0u)) & (cNKMLua.GetData("RAID_BUFF_COST", ref nKMRaidBuffTemplet.raidBuffCost) ? 1 : 0);
		cNKMLua.GetData("BUFF_STR_ID_LIST", nKMRaidBuffTemplet.BUFF_STR_ID_LIST);
		cNKMLua.GetData("COST_CHARGE_RATE", ref nKMRaidBuffTemplet.COST_CHARGE_RATE);
		if (num == 0)
		{
			return null;
		}
		return nKMRaidBuffTemplet;
	}

	public static NKMRaidBuffTemplet Find(int id, int level)
	{
		return NKMTempletContainer<NKMRaidBuffTemplet>.Find((id, level).GetHashCode());
	}

	public void Join()
	{
		foreach (string item in BUFF_STR_ID_LIST)
		{
			NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(item);
			if (buffTempletByStrID == null)
			{
				Log.ErrorAndExit("[RaidBuffTemplet] 버프 정보가 존재하지 않음 RaidBuffName:" + RaidBuffName + ", BUFF_STR_ID:" + item, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidBuffTemplet.cs", 47);
			}
			BUFF_TEMPLET_LIST.Add(buffTempletByStrID);
		}
	}

	public void Validate()
	{
	}
}
