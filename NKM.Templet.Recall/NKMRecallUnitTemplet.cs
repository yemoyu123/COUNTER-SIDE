using System;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet.Recall;

public sealed class NKMRecallUnitTemplet : INKMTemplet
{
	private NKMIntervalTemplet intervalTemplet;

	private string exchangeDateStrId;

	public int Index { get; private set; }

	public int ItemMiscId { get; private set; }

	public int ItemValue { get; private set; }

	public int UnitId { get; private set; }

	public int UnitExchangeGroupId { get; private set; }

	public int Key => UnitId;

	public NKMIntervalTemplet IntervalTemplet => intervalTemplet;

	public static NKMRecallUnitTemplet Find(int unitId)
	{
		return NKMTempletContainer<NKMRecallUnitTemplet>.Find(unitId);
	}

	public static NKMRecallUnitTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallUnitTemplet.cs", 25))
		{
			return null;
		}
		NKMRecallUnitTemplet nKMRecallUnitTemplet = new NKMRecallUnitTemplet();
		int rValue = 0;
		bool data = cNKMLua.GetData("Index", ref rValue);
		nKMRecallUnitTemplet.Index = rValue;
		bool num = data & cNKMLua.GetData("ExchangeDateStrID", ref nKMRecallUnitTemplet.exchangeDateStrId);
		int rValue2 = 0;
		bool num2 = num & cNKMLua.GetData("UnitStrID", ref rValue2);
		nKMRecallUnitTemplet.UnitId = rValue2;
		int rValue3 = 0;
		bool num3 = num2 & cNKMLua.GetData("UnitExchangeGroupID", ref rValue3);
		nKMRecallUnitTemplet.UnitExchangeGroupId = rValue3;
		int rValue4 = 0;
		bool num4 = num3 & cNKMLua.GetData("ItemMiscID", ref rValue4);
		nKMRecallUnitTemplet.ItemMiscId = rValue4;
		int rValue5 = 0;
		bool num5 = num4 & cNKMLua.GetData("ItemValue", ref rValue5);
		nKMRecallUnitTemplet.ItemValue = rValue5;
		if (!num5)
		{
			Log.ErrorAndExit("[NKMRecallUnitTemplet] lua loading failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallUnitTemplet.cs", 56);
			return null;
		}
		return nKMRecallUnitTemplet;
	}

	public void Join()
	{
		intervalTemplet = NKMIntervalTemplet.Find(exchangeDateStrId);
		if (intervalTemplet == null)
		{
			Log.ErrorAndExit("[NKMRecallUnitTemplet] interval templet is null, interval str id: " + exchangeDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallUnitTemplet.cs", 68);
		}
	}

	public void Validate()
	{
		throw new NotImplementedException();
	}
}
