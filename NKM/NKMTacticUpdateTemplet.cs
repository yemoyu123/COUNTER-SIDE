using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMTacticUpdateTemplet : INKMTemplet
{
	public class TacticUpdateData : INKMTemplet
	{
		public int m_TacticGroup;

		public int m_TacticPhase;

		public NKM_STAT_TYPE m_StatType;

		public float m_StatValue;

		public string m_StatIcon;

		public string m_StringKey;

		public int Key => m_TacticGroup;

		public static TacticUpdateData LoadFromLua(NKMLua lua)
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTacticUpdateTemplet.cs", 28))
			{
				return null;
			}
			TacticUpdateData tacticUpdateData = new TacticUpdateData();
			if (!(lua.GetData("TacticGroup", ref tacticUpdateData.m_TacticGroup) & lua.GetData("TacticPhase", ref tacticUpdateData.m_TacticPhase) & lua.GetDataEnum<NKM_STAT_TYPE>("StatType", out tacticUpdateData.m_StatType) & lua.GetData("StatValue", ref tacticUpdateData.m_StatValue) & lua.GetData("StatIcon", ref tacticUpdateData.m_StatIcon) & lua.GetData("StringKey", ref tacticUpdateData.m_StringKey)))
			{
				return null;
			}
			return tacticUpdateData;
		}

		public void Join()
		{
		}

		public void Validate()
		{
			if (m_TacticPhase < 1 || m_TacticPhase > 6)
			{
				NKMTempletError.Add($"[NKMTacticUpdateTemplet] Invalid TacticPhase:{m_TacticPhase}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTacticUpdateTemplet.cs", 55);
			}
			if (m_StatValue < -10000f || m_StatValue > 10000f)
			{
				NKMTempletError.Add($"[NKMTacticUpdateTemplet:{m_TacticPhase}] Invalid StatValue:{m_StatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMTacticUpdateTemplet.cs", 60);
			}
		}
	}

	public const int MaxTacticLevel = 6;

	public const int DefaultTacticGroup = 0;

	public Dictionary<int, TacticUpdateData> m_dicTacticData = new Dictionary<int, TacticUpdateData>();

	public int Key { get; }

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKMTacticUpdateTemplet>.Load(from e in NKMTempletLoader<TacticUpdateData>.LoadGroup("AB_SCRIPT", "LUA_TACTIC_UPDATE_TEMPLET", "TACTIC_UPDATE_TEMPLET", TacticUpdateData.LoadFromLua)
			select new NKMTacticUpdateTemplet(e.Key, e.Value), null);
	}

	public static NKMTacticUpdateTemplet Find(int groupID)
	{
		return NKMTempletContainer<NKMTacticUpdateTemplet>.Find(groupID);
	}

	public static NKMTacticUpdateTemplet Find(NKMUnitTempletBase unitTemplet)
	{
		return NKMTempletContainer<NKMTacticUpdateTemplet>.Find(unitTemplet?.m_TacticGroup ?? 0);
	}

	public static TacticUpdateData Find(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return null;
		}
		return Find(unitData.GetUnitTempletBase().m_TacticGroup, unitData.tacticLevel);
	}

	public static TacticUpdateData Find(NKMUnitTempletBase unitTempletBase, int tacticLevel)
	{
		if (unitTempletBase == null)
		{
			return Find(0, tacticLevel);
		}
		return Find(unitTempletBase.m_TacticGroup, tacticLevel);
	}

	public static TacticUpdateData Find(int groupID, int tacticLevel)
	{
		NKMTacticUpdateTemplet nKMTacticUpdateTemplet = NKMTempletContainer<NKMTacticUpdateTemplet>.Find(groupID);
		if (nKMTacticUpdateTemplet == null)
		{
			nKMTacticUpdateTemplet = NKMTempletContainer<NKMTacticUpdateTemplet>.Find(0);
		}
		return nKMTacticUpdateTemplet.GetData(tacticLevel);
	}

	public NKMTacticUpdateTemplet(int tacticGroup, IEnumerable<TacticUpdateData> lstTacticData)
	{
		Key = tacticGroup;
		foreach (TacticUpdateData lstTacticDatum in lstTacticData)
		{
			m_dicTacticData.Add(lstTacticDatum.m_TacticPhase, lstTacticDatum);
		}
	}

	public TacticUpdateData GetData(int tacticLevel)
	{
		if (m_dicTacticData.TryGetValue(tacticLevel, out var value))
		{
			return value;
		}
		return null;
	}

	public virtual void Join()
	{
	}

	public virtual void Validate()
	{
		foreach (TacticUpdateData value in m_dicTacticData.Values)
		{
			value.Validate();
		}
	}
}
