using System.Collections.Generic;
using System.Linq;
using NKM.Contract2;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMUnitRearmamentTemplet : INKMTemplet
{
	public const int MaxRearmGrade = 2;

	private readonly List<MiscItemUnit> rearmamentUseItems = new List<MiscItemUnit>();

	private int rearmId;

	private int baseUnitId;

	private int rearmTargetUnitId;

	private int rearmUnitId;

	public string RearmStringId { get; private set; }

	public string OpenTag { get; private set; }

	public string Color { get; private set; }

	public int RearmGrade { get; private set; }

	public bool EnableByTag => NKMOpenTagManager.IsOpened(OpenTag);

	public NKMUnitTempletBase BaseUnitTemplet { get; private set; }

	public NKMUnitTempletBase FromUnitTemplet { get; private set; }

	public NKMUnitTempletBase ToUnitTemplet { get; private set; }

	public int Key => rearmId;

	public IReadOnlyList<MiscItemUnit> RearmamentUseItems => rearmamentUseItems;

	public static NKMUnitRearmamentTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 36))
		{
			return null;
		}
		NKMUnitRearmamentTemplet nKMUnitRearmamentTemplet = new NKMUnitRearmamentTemplet
		{
			rearmId = lua.GetInt32("m_RearmID"),
			RearmStringId = lua.GetString("m_RearmStringID"),
			OpenTag = lua.GetString("m_OpenTag"),
			Color = lua.GetString("m_RearmColor"),
			baseUnitId = lua.GetInt32("m_BaseUnitID"),
			rearmTargetUnitId = lua.GetInt32("m_RearmTargetUnitID"),
			rearmUnitId = lua.GetInt32("m_RearmUnitID"),
			RearmGrade = lua.GetInt32("m_RearmGrade")
		};
		for (int i = 0; i < NKMCommonConst.RearmamentCostItemCount; i++)
		{
			int rValue = -1;
			int rValue2 = -1;
			if ((1u & (lua.GetData($"m_RearmUseItemID{i + 1}", ref rValue) ? 1u : 0u) & (lua.GetData($"m_RearmUseItemValue{i + 1}", ref rValue2) ? 1u : 0u)) != 0)
			{
				nKMUnitRearmamentTemplet.rearmamentUseItems.Add(new MiscItemUnit(rValue, rValue2));
			}
		}
		return nKMUnitRearmamentTemplet;
	}

	public void Join()
	{
		BaseUnitTemplet = NKMTempletContainer<NKMUnitTempletBase>.Find(baseUnitId);
		if (BaseUnitTemplet == null)
		{
			NKMTempletError.Add($"[RearmamentTemple:{rearmId}] 잘못된 baseUnitId:{baseUnitId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 77);
		}
		FromUnitTemplet = NKMTempletContainer<NKMUnitTempletBase>.Find(rearmTargetUnitId);
		if (FromUnitTemplet == null)
		{
			NKMTempletError.Add($"[RearmamentTemple:{rearmId}] 잘못된 rearmTargetUnitId:{rearmTargetUnitId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 83);
		}
		ToUnitTemplet = NKMTempletContainer<NKMUnitTempletBase>.Find(rearmUnitId);
		if (ToUnitTemplet == null)
		{
			NKMTempletError.Add($"[RearmamentTemple:{rearmId}] 잘못된 rearmUnitId:{rearmUnitId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 89);
		}
	}

	public void Validate()
	{
		if (RearmGrade < 0 || RearmGrade > NKMCommonConst.RearmamentMaxGrade)
		{
			NKMTempletError.Add($"[Rearmament] 재무장 등급이 비정상 m_RearmID:{rearmId}, m_RearmGrade:{RearmGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 98);
		}
		foreach (MiscItemUnit rearmamentUseItem in rearmamentUseItems)
		{
			rearmamentUseItem.Join("/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 103);
		}
		if (BaseUnitTemplet != BaseUnitTemplet.BaseUnit)
		{
			NKMTempletError.Add($"[RearmamentTemple:{rearmId}] {BaseUnitTemplet.DebugName}는 baseUnit이 아닙니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 108);
		}
		if (BaseUnitTemplet != FromUnitTemplet.BaseUnit)
		{
			NKMTempletError.Add($"[RearmamentTemple:{rearmId}] rearmTargetUnitId의 baseUnit 정보가 일치하지 않음. baseUnitId:{BaseUnitTemplet.DebugName} rearmTarget.BaseUnitId:{FromUnitTemplet.BaseUnit.DebugName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 113);
		}
		if (BaseUnitTemplet != ToUnitTemplet.BaseUnit)
		{
			NKMTempletError.Add($"[RearmamentTemple:{rearmId}] rearmUnitId baseUnit 정보가 일치하지 않음. baseUnitId:{BaseUnitTemplet.DebugName} rearmUnit.BaseUnitId:{ToUnitTemplet.BaseUnit.DebugName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 118);
		}
		if (FromUnitTemplet.m_RearmGrade + 1 != ToUnitTemplet.m_RearmGrade)
		{
			NKMTempletError.Add($"[RearmamentTemple:{rearmId}] 대상 유닛들의 재무장 등급차이가 +1이 아님. rearmGrade:{FromUnitTemplet.m_RearmGrade} -> {ToUnitTemplet.m_RearmGrade}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 123);
		}
		int[] array = (from e in rearmamentUseItems
			group e by e.ItemId into e
			where e.Count() > 1
			select e.Key).ToArray();
		if (array.Any())
		{
			string arg = string.Join(", ", array);
			NKMTempletError.Add($"[Rearmament] 재료 아이템 중 중복되는 항목이 존재 rearmId:{rearmId} duplicateItemId:{arg}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 134);
		}
		if (!rearmamentUseItems.Any())
		{
			NKMTempletError.Add($"[Rearmament] 재료 아이템 없음. rearmId:{rearmId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 139);
		}
		if (EnableByTag && !FromUnitTemplet.CanRearmament())
		{
			NKMTempletError.Add($"[Rearmament:{rearmId}] fromUnit에 사용 불가한 유닛 지정. fromUnitId:{FromUnitTemplet.DebugName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 144);
		}
		if (EnableByTag && !ToUnitTemplet.CanRearmament())
		{
			NKMTempletError.Add($"[Rearmament:{rearmId}] toUnit에 사용 불가한 유닛 지정. toUnit에Id:{ToUnitTemplet.DebugName}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMUnitRearmamentTemplet.cs", 149);
		}
	}
}
