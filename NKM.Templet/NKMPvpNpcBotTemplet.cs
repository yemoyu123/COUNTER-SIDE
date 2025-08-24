using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMPvpNpcBotTemplet : INKMTemplet
{
	private int m_Idx;

	private string m_OpenTag;

	private int m_Tier;

	private string m_Name;

	private string m_UnlockReqType;

	private int m_UnlockReqValue;

	private int m_AccountLevel;

	private int m_GainBaseScore;

	private NKMDummyUnitData m_ShipData;

	private NKMDummyUnitData[] m_UnitData = new NKMDummyUnitData[8];

	private static Dictionary<int, List<NKMPvpNpcBotTemplet>> NpcBotGroups;

	public int Key => m_Idx;

	public int Tier => m_Tier;

	public string Name => m_Name;

	public string UnlockReqType => m_UnlockReqType;

	public int UnlockReqValue => m_UnlockReqValue;

	public int AccountLevel => m_AccountLevel;

	public IEnumerable<NKMUnitData> UnitData { get; private set; }

	public NKMUnitData ShipData { get; private set; }

	public static IEnumerable<NKMPvpNpcBotTemplet> Values => NKMTempletContainer<NKMPvpNpcBotTemplet>.Values;

	public static IReadOnlyDictionary<int, List<NKMPvpNpcBotTemplet>> Groups => NpcBotGroups;

	public static NKMPvpNpcBotTemplet Find(int key)
	{
		return NKMTempletContainer<NKMPvpNpcBotTemplet>.Find((NKMPvpNpcBotTemplet x) => x.Key == key);
	}

	public static NKMPvpNpcBotTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpNpcBotTemplet.cs", 38))
		{
			return null;
		}
		NKMPvpNpcBotTemplet nKMPvpNpcBotTemplet = new NKMPvpNpcBotTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("INDEX", ref nKMPvpNpcBotTemplet.m_Idx);
		flag &= cNKMLua.GetData("m_OpenTag", ref nKMPvpNpcBotTemplet.m_OpenTag);
		flag &= cNKMLua.GetData("TIER", ref nKMPvpNpcBotTemplet.m_Tier);
		flag &= cNKMLua.GetData("m_GainBaseScore", ref nKMPvpNpcBotTemplet.m_GainBaseScore);
		flag &= cNKMLua.GetData("NAME", ref nKMPvpNpcBotTemplet.m_Name);
		flag &= cNKMLua.GetData("m_UnlockReqType", ref nKMPvpNpcBotTemplet.m_UnlockReqType);
		flag &= cNKMLua.GetData("m_UnlockReqValue", ref nKMPvpNpcBotTemplet.m_UnlockReqValue);
		flag &= cNKMLua.GetData("ACCOUNT_LEVEL", ref nKMPvpNpcBotTemplet.m_AccountLevel);
		nKMPvpNpcBotTemplet.m_ShipData = new NKMDummyUnitData();
		flag &= cNKMLua.GetData("SLOT_UNIT_ID_SHIP", ref nKMPvpNpcBotTemplet.m_ShipData.UnitId);
		flag &= cNKMLua.GetData("SLOT_UNIT_LEVEL_SHIP", ref nKMPvpNpcBotTemplet.m_ShipData.UnitLevel);
		for (int i = 0; i < 8; i++)
		{
			flag &= cNKMLua.GetData($"SLOT_UNIT_ID_{i + 1}", ref nKMPvpNpcBotTemplet.m_UnitData[i].UnitId);
			flag &= cNKMLua.GetData($"SLOT_UNIT_LEVEL_{i + 1}", ref nKMPvpNpcBotTemplet.m_UnitData[i].UnitLevel);
			flag &= cNKMLua.GetData($"SLOT_UNIT_LIMIT_{i + 1}", ref nKMPvpNpcBotTemplet.m_UnitData[i].LimitBreakLevel);
		}
		if (!flag)
		{
			return null;
		}
		return nKMPvpNpcBotTemplet;
	}

	public void Join()
	{
		ShipData = m_ShipData.ToUnitData(NpcUid.Get());
		List<NKMUnitData> list = new List<NKMUnitData>();
		NKMDummyUnitData[] unitData = m_UnitData;
		foreach (NKMDummyUnitData nKMDummyUnitData in unitData)
		{
			list.Add(nKMDummyUnitData.ToUnitData(NpcUid.Get()));
		}
		UnitData = list;
	}

	public void Validate()
	{
		if (m_GainBaseScore < 0)
		{
			NKMTempletError.Add($"[NKMPvpNpcBot] Invalid GainBaseScore idx:{m_Idx} score:{m_GainBaseScore}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpNpcBotTemplet.cs", 84);
		}
		if (NKMUnitTempletBase.Find(m_ShipData.UnitId) == null)
		{
			NKMTempletError.Add($"[NKMPvpNpcBot] Invalid ship idx:{m_Idx} unitId:{m_ShipData.UnitId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpNpcBotTemplet.cs", 90);
		}
		NKMDummyUnitData[] unitData = m_UnitData;
		foreach (NKMDummyUnitData nKMDummyUnitData in unitData)
		{
			if (NKMUnitTempletBase.Find(nKMDummyUnitData.UnitId) == null)
			{
				NKMTempletError.Add($"[NKMPvpNpcBot] Invalid unit idx:{m_Idx} unitId:{nKMDummyUnitData.UnitId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpNpcBotTemplet.cs", 98);
			}
		}
	}

	public static void MakeGroup()
	{
		NpcBotGroups = (from e in Values
			group e by e.m_Tier).ToDictionary((IGrouping<int, NKMPvpNpcBotTemplet> e) => e.Key, (IGrouping<int, NKMPvpNpcBotTemplet> e) => e.ToList());
	}
}
