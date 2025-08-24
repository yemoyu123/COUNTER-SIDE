using System.Collections.Generic;
using System.Linq;
using NKM.Contract2;
using NKM.Templet.Base;

namespace NKM;

public class NKMReactorSkillTemplet : INKMTemplet
{
	private readonly List<MiscItemUnit> reqItems = new List<MiscItemUnit>();

	private List<SkillStatData> statDatas = new List<SkillStatData>(5);

	private int idx;

	private int reactorId;

	private string reactorLevelDesc;

	private string baseSkillStrId;

	private string IconStr;

	private float cooltimeSecond;

	private float empowerFactor;

	private string openTag;

	public int Key => idx;

	public int ReactorId => reactorId;

	public string SkillDesc => reactorLevelDesc;

	public string SkillIcon => IconStr;

	public List<SkillStatData> StatDatas => statDatas;

	public static IEnumerable<NKMReactorSkillTemplet> Values => NKMTempletContainer<NKMReactorSkillTemplet>.Values;

	public IReadOnlyList<MiscItemUnit> ReqItems => reqItems;

	public string BaseSkillStrId => baseSkillStrId;

	public bool EnableByTag
	{
		get
		{
			if (!string.IsNullOrEmpty(openTag))
			{
				return NKMOpenTagManager.IsOpened(openTag);
			}
			return true;
		}
	}

	public static NKMReactorSkillTemplet Find(int key)
	{
		return NKMTempletContainer<NKMReactorSkillTemplet>.Find(key);
	}

	public static NKMReactorSkillTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMReactorSkillTemplet.cs", 41))
		{
			return null;
		}
		int @int = lua.GetInt32("ReactorID");
		NKMReactorSkillTemplet nKMReactorSkillTemplet = new NKMReactorSkillTemplet
		{
			idx = lua.GetInt32("IDX"),
			reactorId = @int
		};
		lua.GetData("ReactorLevelDesc", ref nKMReactorSkillTemplet.reactorLevelDesc);
		lua.GetData("BaseSkillStrID", ref nKMReactorSkillTemplet.baseSkillStrId);
		lua.GetData("UnitSkillIconStrID", ref nKMReactorSkillTemplet.IconStr);
		lua.GetData("m_fCooltimeSecond", ref nKMReactorSkillTemplet.cooltimeSecond);
		lua.GetData("m_fEmpowerFactor", ref nKMReactorSkillTemplet.empowerFactor);
		lua.GetData("OpenTag", ref nKMReactorSkillTemplet.openTag);
		for (int i = 0; i < 5; i++)
		{
			string text = (i + 1).ToString();
			NKM_STAT_TYPE result = NKM_STAT_TYPE.NST_END;
			if (!lua.GetData("m_NKM_STAT_TYPE" + text, ref result) || result == NKM_STAT_TYPE.NST_END)
			{
				break;
			}
			float statValue = 0f;
			NKMUnitStatManager.LoadStat(lua, "m_fStatValue" + text, "m_fStatRate" + text, ref result, ref statValue);
			nKMReactorSkillTemplet.statDatas.Add(new SkillStatData(result, statValue));
		}
		for (int j = 0; j < NKMCommonConst.ReactorMaxReqItemCount; j++)
		{
			int rValue = -1;
			int rValue2 = -1;
			if ((1u & (lua.GetData($"LevelUpReqItemID_{j + 1}", ref rValue) ? 1u : 0u) & (lua.GetData($"LevelUpReqItemValue_{j + 1}", ref rValue2) ? 1u : 0u)) != 0)
			{
				nKMReactorSkillTemplet.reqItems.Add(new MiscItemUnit(rValue, rValue2));
			}
		}
		return nKMReactorSkillTemplet;
	}

	public virtual void Join()
	{
		foreach (MiscItemUnit reqItem in reqItems)
		{
			reqItem.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMReactorSkillTemplet.cs", 102);
		}
	}

	public virtual void Validate()
	{
		if (idx <= 0)
		{
			NKMTempletError.Add($"[ReactorSkillTemplet:{reactorId}] idx\ufffd\ufffd\ufffd\ufffd \ufffd\u033b\ufffd idx:{idx}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMReactorSkillTemplet.cs", 110);
		}
		if (reactorId <= 0)
		{
			NKMTempletError.Add($"[ReactorSkillTemplet:{reactorId}] reactorId \ufffd\ufffd\ufffd\ufffd \ufffd\u033b\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMReactorSkillTemplet.cs", 115);
		}
		if (NKMUnitReactorTemplet.Find(reactorId) == null)
		{
			NKMTempletError.Add($"[ReactorSkillTemplet:{reactorId}] NKMUnitReactorTemplet\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMReactorSkillTemplet.cs", 121);
		}
		if (string.IsNullOrEmpty(reactorLevelDesc))
		{
			NKMTempletError.Add($"[ReactorSkillTemplet:{reactorId}] reactorLevelDesc \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd. reactorLevelDesc:{reactorLevelDesc}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMReactorSkillTemplet.cs", 126);
		}
		int[] array = (from e in reqItems
			group e by e.ItemId into e
			where e.Count() > 1
			select e.Key).ToArray();
		if (array.Any())
		{
			string arg = string.Join(", ", array);
			NKMTempletError.Add($"[ReactorSkillTemplet:{reactorId}] \ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd \ufffdߺ\ufffd\ufffdǴ\ufffd \ufffd\u05f8\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd duplicateItemId:{arg}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMReactorSkillTemplet.cs", 137);
		}
		if (!reqItems.Any())
		{
			NKMTempletError.Add($"[ReactorSkillTemplet:{reactorId}] \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdʿ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMReactorSkillTemplet.cs", 142);
		}
	}
}
