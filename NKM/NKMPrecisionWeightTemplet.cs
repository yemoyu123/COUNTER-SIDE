using System.Collections.Generic;
using System.Linq;
using Cs.Math.Lottery;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMPrecisionWeightTemplet : INKMTemplet, INKMTempletEx
{
	public struct PrecisionWeightEntity
	{
		public int WeightId { get; }

		public int Precision { get; }

		public int Weight { get; }

		private PrecisionWeightEntity(int weightId, int precision, int weight)
		{
			WeightId = weightId;
			Precision = precision;
			Weight = weight;
		}

		public static PrecisionWeightEntity Create(NKMLua lua)
		{
			int @int = lua.GetInt32("PrecisionWeightId");
			int int2 = lua.GetInt32("Precision");
			int int3 = lua.GetInt32("Weight");
			return new PrecisionWeightEntity(@int, int2, int3);
		}
	}

	private const int AllCandidateCount = 101;

	private readonly HashSet<int> precisions = new HashSet<int>();

	private readonly RatioLottery<int> lottery = new RatioLottery<int>();

	private readonly Dictionary<int, int> datas = new Dictionary<int, int>();

	public static Dictionary<int, List<PrecisionWeightEntity>> m_dicPrecisionWeight = new Dictionary<int, List<PrecisionWeightEntity>>();

	public int Key { get; }

	public IReadonlyLottery<int> Lottery => lottery;

	private string DebugName => $"[PrecisionWeight:{Key}]";

	private NKMPrecisionWeightTemplet(IGrouping<int, PrecisionWeightEntity> group)
	{
		Key = group.Key;
		foreach (PrecisionWeightEntity item in group)
		{
			if (precisions.Contains(item.Precision))
			{
				NKMTempletError.Add($"[PrecisionWeight] 동일 그룹에 중복된 precision값 존재. weightId:{item.WeightId} precision:{item.Precision}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPrecisionWeightTemplet.cs", 23);
				continue;
			}
			precisions.Add(item.Precision);
			lottery.AddCase(item.Weight, item.Precision);
			datas.Add(item.Precision, item.Weight);
		}
	}

	public int GetWeightToPrecision(int precision)
	{
		return datas[precision];
	}

	public static void LoadFromLua()
	{
		(from e in NKMTempletLoader.LoadCommonPath("AB_SCRIPT", "AB_SCRIPT_ITEM_TEMPLET/LUA_ITEM_EQUIP_PRECISION_WEIGHT", "PRECISION_WEIGHT", PrecisionWeightEntity.Create)
			group e by e.WeightId into e
			select new NKMPrecisionWeightTemplet(e)).AddToContainer();
	}

	public static NKMPrecisionWeightTemplet Find(int key)
	{
		return NKMTempletContainer<NKMPrecisionWeightTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
		int[] array = (from e in Enumerable.Range(0, 101)
			where !precisions.Contains(e)
			select e).ToArray();
		if (array.Any())
		{
			string text = string.Join(", ", array);
			NKMTempletError.Add(DebugName + " 그룹에 누락된 precision 데이터:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPrecisionWeightTemplet.cs", 64);
		}
	}

	public static void LoadFromLuaForClient()
	{
		m_dicPrecisionWeight = (from e in NKMTempletLoader.LoadCommonPath("AB_SCRIPT_ITEM_TEMPLET", "LUA_ITEM_EQUIP_PRECISION_WEIGHT", "PRECISION_WEIGHT", PrecisionWeightEntity.Create)
			where e.Weight > 0
			group e by e.WeightId).ToDictionary((IGrouping<int, PrecisionWeightEntity> e) => e.Key, (IGrouping<int, PrecisionWeightEntity> e) => e.ToList());
	}

	public void PostJoin()
	{
	}
}
