using System.Collections.Generic;
using Cs.Logging;
using Cs.Math.Lottery;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMOperatorRandomPassiveGroupTemplet : INKMTemplet
{
	private readonly int groupId;

	private readonly RatioLottery<NKMOperatorRandomPassiveTemplet> lottery = new RatioLottery<NKMOperatorRandomPassiveTemplet>();

	public List<NKMOperatorRandomPassiveTemplet> Groups = new List<NKMOperatorRandomPassiveTemplet>();

	public int Key => groupId;

	public int GroupId => groupId;

	public NKMOperatorRandomPassiveGroupTemplet(int groupId, List<NKMOperatorRandomPassiveTemplet> datas)
	{
		if (datas == null || datas.Count == 0)
		{
			Log.ErrorAndExit($"Invalid OperatorRandomPassiveGroup's groupId:{groupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorRandomPassiveGroupTemplet.cs", 21);
			return;
		}
		this.groupId = groupId;
		foreach (NKMOperatorRandomPassiveTemplet data in datas)
		{
			lottery.AddCase(data.ratio, data);
			Groups.Add(data);
		}
	}

	public static NKMOperatorRandomPassiveGroupTemplet Find(int key)
	{
		return NKMTempletContainer<NKMOperatorRandomPassiveGroupTemplet>.Find(key);
	}

	public NKMOperatorRandomPassiveTemplet Decide()
	{
		return lottery.Decide();
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
