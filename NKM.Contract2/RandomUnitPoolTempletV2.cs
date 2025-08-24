using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Math.Lottery;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Contract2;

public class RandomUnitPoolTempletV2 : INKMTemplet, IRandomUnitPool
{
	private readonly Dictionary<string, RandomUnitTempletV2> unitTemplets = new Dictionary<string, RandomUnitTempletV2>();

	private Dictionary<string, RandomUnitTempletV2> openedUnitTemplets = new Dictionary<string, RandomUnitTempletV2>();

	private readonly List<RandomUnitTempletV2>[] unitsByGrade = new List<RandomUnitTempletV2>[4];

	private readonly RatioLottery<RandomUnitTempletV2>[] lotteries = new RatioLottery<RandomUnitTempletV2>[EnumUtil<NKM_UNIT_PICK_GRADE>.Count];

	private bool joined;

	public int Key { get; }

	public string StringId { get; }

	public string DebugName => $"[{Key}] {StringId}";

	public int TotalUnitCount => openedUnitTemplets.Count;

	public IEnumerable<RandomUnitTempletV2> UnitTemplets => openedUnitTemplets.Values;

	public IReadOnlyList<IEnumerable<RandomUnitTempletV2>> UnitsByPickGrade => lotteries;

	public RandomUnitPoolTempletV2(int key, string stringId)
	{
		Key = key;
		StringId = stringId;
		for (int i = 0; i < unitsByGrade.Length; i++)
		{
			unitsByGrade[i] = new List<RandomUnitTempletV2>();
		}
		for (int j = 0; j < lotteries.Length; j++)
		{
			lotteries[j] = new RatioLottery<RandomUnitTempletV2>();
		}
	}

	public static void LoadFile()
	{
		string bundleName = "AB_SCRIPT";
		string text = "LUA_CONTRACT_UNIT_POOL";
		string text2 = "UNIT_POOL";
		Dictionary<int, RandomUnitPoolTempletV2> dictionary = new Dictionary<int, RandomUnitPoolTempletV2>();
		using (NKMLua nKMLua = new NKMLua())
		{
			if (!nKMLua.LoadCommonPath(bundleName, text) || !nKMLua.OpenTable(text2))
			{
				Log.ErrorAndExit("[RandomUnitPool] loading file failed. fileName:" + text + " tablName:" + text2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomUnitPoolTempletV2.cs", 56);
			}
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				if (NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomUnitPoolTempletV2.cs", 62))
				{
					if (!nKMLua.GetData("m_UnitPoolId", out var rValue, 0))
					{
						Log.ErrorAndExit($"[ContractUnitPool] loading key failed. id:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomUnitPoolTempletV2.cs", 66);
					}
					if (!dictionary.TryGetValue(rValue, out var value))
					{
						if (!nKMLua.GetData("m_UnitPoolStrId", out var rValue2, null))
						{
							Log.ErrorAndExit($"[ContractUnitPool] loading key failed. id:{rValue} strId:{rValue2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomUnitPoolTempletV2.cs", 73);
						}
						value = new RandomUnitPoolTempletV2(rValue, rValue2);
						dictionary.Add(rValue, value);
					}
					value.LoadFromLua(nKMLua);
				}
				num++;
				nKMLua.CloseTable();
			}
		}
		NKMTempletContainer<RandomUnitPoolTempletV2>.Load(dictionary.Values, (RandomUnitPoolTempletV2 e) => e.StringId);
	}

	public static RandomUnitPoolTempletV2 Find(string key)
	{
		return NKMTempletContainer<RandomUnitPoolTempletV2>.Find(key);
	}

	public static RandomUnitPoolTempletV2 Find(int key)
	{
		return NKMTempletContainer<RandomUnitPoolTempletV2>.Find(key);
	}

	public void LoadFromLua(NKMLua lua)
	{
		string rValue;
		int num = 1 & (lua.GetData("m_UnitStrId", out rValue, null) ? 1 : 0);
		lua.GetData("m_PickupTarget", out var rbValue, defValue: false);
		int rValue2;
		int num2 = num & (lua.GetData("m_Ratio", out rValue2, 0) ? 1 : 0);
		lua.GetData("m_RatioupTarget", out var rbValue2, defValue: false);
		lua.GetData("m_CustomPickupTarget", out var rbValue3, defValue: false);
		if (num2 == 0)
		{
			NKMTempletError.Add($"[RandomUnitPool] loading failed. id:{Key} strId:{StringId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomUnitPoolTempletV2.cs", 105);
		}
		else if (unitTemplets.ContainsKey(rValue))
		{
			NKMTempletError.Add($"[RandomUnitPool] unit id duplicated. id:{Key} strId:{StringId} unitId:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/RandomUnitPoolTempletV2.cs", 111);
		}
		else
		{
			unitTemplets.Add(rValue, new RandomUnitTempletV2(rValue, rValue2, rbValue, rbValue2, rbValue3));
		}
	}

	public void Join()
	{
		if (!joined)
		{
			joined = true;
			RecalculateUnitTemplets();
		}
	}

	public void Validate()
	{
		this.ValidateCommon();
	}

	public RandomUnitTempletV2 Decide(NKM_UNIT_PICK_GRADE pickGrade)
	{
		return lotteries[(int)pickGrade].Decide();
	}

	public IEnumerable<RandomUnitTempletV2> GetBonusCandidates()
	{
		RatioLottery<RandomUnitTempletV2> ratioLottery = lotteries[7];
		if (ratioLottery == null)
		{
			return Array.Empty<RandomUnitTempletV2>();
		}
		return ratioLottery;
	}

	public RatioLottery<RandomUnitTempletV2> GetLottery(NKM_UNIT_PICK_GRADE pickGrade)
	{
		return lotteries[(int)pickGrade];
	}

	public void RecalculateUnitTemplets(List<int> lstExculudeUnits = null)
	{
		List<RandomUnitTempletV2>[] array = unitsByGrade;
		for (int i = 0; i < array.Length; i++)
		{
			array[i]?.Clear();
		}
		foreach (NKM_UNIT_PICK_GRADE value in Enum.GetValues(typeof(NKM_UNIT_PICK_GRADE)))
		{
			lotteries[(int)value] = new RatioLottery<RandomUnitTempletV2>();
		}
		openedUnitTemplets.Clear();
		foreach (RandomUnitTempletV2 value2 in unitTemplets.Values)
		{
			value2.Join();
			bool flag = false;
			if (value2.PickUpTarget && value2.UnitTemplet.PickupEnableByTag)
			{
				flag = true;
			}
			else if (value2.RatioUpTarget && value2.UnitTemplet.PickupEnableByTag)
			{
				flag = true;
			}
			else if (!value2.PickUpTarget && !value2.RatioUpTarget && value2.UnitTemplet.ContractEnableByTag)
			{
				flag = true;
			}
			if (flag && (lstExculudeUnits == null || !lstExculudeUnits.Contains(value2.UnitTemplet.m_UnitID)))
			{
				int nKM_UNIT_GRADE = (int)value2.UnitTemplet.m_NKM_UNIT_GRADE;
				unitsByGrade[nKM_UNIT_GRADE].Add(value2);
				nKM_UNIT_GRADE = (int)value2.PickGrade;
				lotteries[nKM_UNIT_GRADE].AddCase(value2.Ratio, value2);
				openedUnitTemplets.Add(value2.UnitStringId, value2);
			}
		}
	}

	public RatioLottery<RandomUnitTempletV2>[] GetCustomPickupLotteries(CustomPickupContractTemplet customPickupContractTemplet, int customPickupUnitId)
	{
		RatioLottery<RandomUnitTempletV2>[] array = new RatioLottery<RandomUnitTempletV2>[EnumUtil<NKM_UNIT_PICK_GRADE>.Count];
		foreach (NKM_UNIT_PICK_GRADE value in Enum.GetValues(typeof(NKM_UNIT_PICK_GRADE)))
		{
			array[(int)value] = new RatioLottery<RandomUnitTempletV2>();
		}
		foreach (RandomUnitTempletV2 value2 in unitTemplets.Values)
		{
			bool flag = false;
			if (value2.PickUpTarget)
			{
				continue;
			}
			if (value2.UnitTemplet.ContractEnableByTag)
			{
				flag = true;
			}
			if (flag)
			{
				int num = MakeNewIndex(value2.UnitTemplet.m_UnitID, value2.UnitTemplet.m_NKM_UNIT_GRADE);
				if (num < 0)
				{
					return null;
				}
				if ((customPickupContractTemplet?.CustomPickUpType ?? CustomPickupContractTemplet.CUSTOM_PICK_UP_TYPE.BASIC) != CustomPickupContractTemplet.CUSTOM_PICK_UP_TYPE.AWAKEN || num != 6 || !value2.UnitTemplet.m_bAwaken)
				{
					array[num].AddCase(value2.Ratio, value2);
				}
			}
		}
		return array;
		int MakeNewIndex(int unitId, NKM_UNIT_GRADE unitGrade)
		{
			bool flag2 = unitId == customPickupUnitId;
			switch (unitGrade)
			{
			case NKM_UNIT_GRADE.NUG_N:
				if (!flag2)
				{
					return 0;
				}
				return 1;
			case NKM_UNIT_GRADE.NUG_R:
				if (!flag2)
				{
					return 2;
				}
				return 3;
			case NKM_UNIT_GRADE.NUG_SR:
				if (!flag2)
				{
					return 4;
				}
				return 5;
			case NKM_UNIT_GRADE.NUG_SSR:
				if (!flag2)
				{
					return 6;
				}
				return 7;
			default:
				return -1;
			}
		}
	}

	public bool ValidateCustomPickupUnit()
	{
		if (!UnitTemplets.Any((RandomUnitTempletV2 e) => e.CustomPickupTarget))
		{
			return false;
		}
		return true;
	}
}
