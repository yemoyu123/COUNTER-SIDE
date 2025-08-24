using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Unit;

public static class NKMUnitExpTableContainer
{
	private readonly struct TableKey
	{
		public NKM_UNIT_GRADE UnitGrade { get; }

		public int RearmGrade { get; }

		public bool IsAwaken { get; }

		public TableKey(NKM_UNIT_GRADE unitGrade, int rearmGrade, bool isAwaken)
		{
			UnitGrade = unitGrade;
			RearmGrade = rearmGrade;
			IsAwaken = isAwaken;
		}
	}

	private static readonly NKMUnitExpTable[,,] Tables = new NKMUnitExpTable[4, 3, 2];

	public static NKMUnitExpTable DefaultTable { get; } = new NKMUnitExpTable(NKM_UNIT_GRADE.NUG_COUNT, 0, isAwaken: false);

	public static void LoadFromLua(bool server)
	{
		LoadDefaultExpTable(server);
		LoadRearmExpTable(server);
		Join();
		Validate();
	}

	public static NKMUnitExpTemplet Get(NKM_UNIT_GRADE unitGrade, int rearmGrade, int unitLevel, bool isAwaken)
	{
		int num = (isAwaken ? 1 : 0);
		NKMUnitExpTable nKMUnitExpTable = Tables[(int)unitGrade, rearmGrade, num];
		if (nKMUnitExpTable == null)
		{
			Log.Error($"[ExpTable] invalid table key. unitGrade:{unitGrade} rearmGrade:{rearmGrade} isAwaken:{isAwaken}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTableContainer.cs", 28);
			return null;
		}
		return nKMUnitExpTable.Get(unitLevel);
	}

	public static NKMUnitExpTable GetTable(NKM_UNIT_GRADE unitGrade, int rearmGrade, bool isAwaken)
	{
		int num = (isAwaken ? 1 : 0);
		return Tables[(int)unitGrade, rearmGrade, num];
	}

	private static void LoadDefaultExpTable(bool server)
	{
		string bundleName = "AB_SCRIPT_UNIT_DATA";
		string text = (server ? "AB_SCRIPT_UNIT_DATA/LUA_UNIT_EXP_TABLE" : "LUA_UNIT_EXP_TABLE");
		string text2 = "m_UnitExpTable";
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath(bundleName, text) || !nKMLua.OpenTable(text2))
		{
			Log.ErrorAndExit("[ExpTable] loading file failed. fileName:" + text + " tablName:" + text2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTableContainer.cs", 52);
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTableContainer.cs", 58))
			{
				num++;
				nKMLua.CloseTable();
			}
			else
			{
				DefaultTable.LoadData(nKMLua);
				num++;
				nKMLua.CloseTable();
			}
		}
		for (int i = 0; i < 4; i++)
		{
			Tables[i, 0, 0] = DefaultTable;
			Tables[i, 0, 1] = DefaultTable;
		}
	}

	private static void LoadRearmExpTable(bool server)
	{
		string bundleName = "AB_SCRIPT_UNIT_DATA";
		string text = (server ? "AB_SCRIPT_UNIT_DATA/LUA_REARM_UNIT_EXP_TABLE" : "LUA_REARM_UNIT_EXP_TABLE");
		string text2 = "REARM_UNIT_EXP";
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.LoadCommonPath(bundleName, text) || !nKMLua.OpenTable(text2))
		{
			Log.ErrorAndExit("[ExpTable] loading file failed. fileName:" + text + " tablName:" + text2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTableContainer.cs", 88);
		}
		int num = 1;
		while (nKMLua.OpenTable(num))
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTableContainer.cs", 94))
			{
				num++;
				nKMLua.CloseTable();
				continue;
			}
			NKM_UNIT_GRADE nKM_UNIT_GRADE = nKMLua.GetEnum<NKM_UNIT_GRADE>("m_NKM_UNIT_GRADE");
			int @int = nKMLua.GetInt32("m_RearmGrade");
			bool boolean = nKMLua.GetBoolean("m_bAwaken");
			int num2 = (int)nKM_UNIT_GRADE;
			int num3 = (boolean ? 1 : 0);
			if (nKM_UNIT_GRADE < NKM_UNIT_GRADE.NUG_N || num2 >= Tables.GetLength(0))
			{
				NKMTempletError.Add($"[ExpTable] invalid unitGrade:{nKM_UNIT_GRADE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTableContainer.cs", 108);
			}
			else if (@int < 0 || @int >= Tables.GetLength(1))
			{
				NKMTempletError.Add($"[ExpTable] invalid rearmGrade:{@int}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTableContainer.cs", 112);
			}
			else
			{
				NKMUnitExpTable nKMUnitExpTable = Tables[num2, @int, num3];
				if (nKMUnitExpTable == null)
				{
					nKMUnitExpTable = new NKMUnitExpTable(nKM_UNIT_GRADE, @int, boolean);
					Tables[num2, @int, num3] = nKMUnitExpTable;
				}
				nKMUnitExpTable.LoadData(nKMLua);
			}
			num++;
			nKMLua.CloseTable();
		}
	}

	private static void Join()
	{
		DefaultTable.Join();
		TableKey[] array = new TableKey[7]
		{
			new TableKey(NKM_UNIT_GRADE.NUG_R, 1, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SR, 1, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SR, 2, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SSR, 1, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SSR, 2, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SSR, 1, isAwaken: true),
			new TableKey(NKM_UNIT_GRADE.NUG_SSR, 2, isAwaken: true)
		};
		for (int i = 0; i < array.Length; i++)
		{
			TableKey tableKey = array[i];
			GetTable(tableKey.UnitGrade, tableKey.RearmGrade, tableKey.IsAwaken).Join();
		}
	}

	private static void Validate()
	{
		DefaultTable.Validate();
		TableKey[] array = new TableKey[7]
		{
			new TableKey(NKM_UNIT_GRADE.NUG_R, 1, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SR, 1, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SR, 2, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SSR, 1, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SSR, 2, isAwaken: false),
			new TableKey(NKM_UNIT_GRADE.NUG_SSR, 1, isAwaken: true),
			new TableKey(NKM_UNIT_GRADE.NUG_SSR, 2, isAwaken: true)
		};
		for (int i = 0; i < array.Length; i++)
		{
			TableKey tableKey = array[i];
			GetTable(tableKey.UnitGrade, tableKey.RearmGrade, tableKey.IsAwaken).Validate();
		}
	}

	public static void Drop()
	{
		DefaultTable.Drop();
		NKMUnitExpTable[,,] tables = Tables;
		int upperBound = tables.GetUpperBound(0);
		int upperBound2 = tables.GetUpperBound(1);
		int upperBound3 = tables.GetUpperBound(2);
		for (int i = tables.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = tables.GetLowerBound(1); j <= upperBound2; j++)
			{
				for (int k = tables.GetLowerBound(2); k <= upperBound3; k++)
				{
					tables[i, j, k]?.Drop();
				}
			}
		}
	}
}
