using System;
using System.Collections.Generic;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Game;

public class NKMEventConditionBuff : NKMEventConditionDetail
{
	public NKMMinMaxInt m_BuffLevel = new NKMMinMaxInt(-1, -1);

	public NKMMinMaxInt m_BuffOverlapCount = new NKMMinMaxInt(-1, -1);

	public bool m_bIgnore;

	private short m_BuffID = -1;

	private string m_BuffStrID;

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		if (m_BuffID < 0 && !CacheBuffID(bErrorLog: true))
		{
			return false;
		}
		bool flag = HasBuff(cNKMUnit);
		if (!m_bIgnore)
		{
			return flag;
		}
		return !flag;
	}

	private bool HasBuff(NKMUnit cNKMUnit)
	{
		foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum in cNKMUnit.GetUnitSyncData().m_dicBuffData)
		{
			NKMBuffSyncData value = dicBuffDatum.Value;
			if (Math.Abs(value.m_BuffID) == m_BuffID)
			{
				if (!m_BuffLevel.IsBetween(value.m_BuffStatLevel, negativeIsTrue: true))
				{
					return false;
				}
				if (!m_BuffOverlapCount.IsBetween(value.m_OverlapCount, negativeIsTrue: true))
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	private bool CacheBuffID(bool bErrorLog)
	{
		NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(m_BuffStrID);
		if (buffTempletByStrID != null)
		{
			m_BuffID = buffTempletByStrID.m_BuffID;
		}
		else if (bErrorLog)
		{
			NKMTempletError.Add("[EventConditionBuff] \ufffd\ufffd\ufffd\ufffd " + m_BuffStrID + "\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 819);
			return false;
		}
		return true;
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionBuff nKMEventConditionBuff = new NKMEventConditionBuff();
		nKMEventConditionBuff.m_BuffLevel.DeepCopyFromSource(m_BuffLevel);
		nKMEventConditionBuff.m_BuffOverlapCount.DeepCopyFromSource(m_BuffOverlapCount);
		nKMEventConditionBuff.m_bIgnore = m_bIgnore;
		nKMEventConditionBuff.m_BuffID = m_BuffID;
		nKMEventConditionBuff.m_BuffStrID = m_BuffStrID;
		return nKMEventConditionBuff;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		if ((1u & (cNKMLua.GetData("m_BuffStrID", ref m_BuffStrID) ? 1u : 0u)) == 0)
		{
			NKMTempletError.Add("[EventConditionBuff] m_BuffStrID \ufffd\ufffd\ufffd\ufffd \ufffdԷµ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEventConditionV2.cs", 846);
			return false;
		}
		m_BuffLevel.LoadFromLua(cNKMLua, "m_BuffLevel");
		m_BuffOverlapCount.LoadFromLua(cNKMLua, "m_BuffOverlapCount");
		cNKMLua.GetData("m_bIgnore", ref m_bIgnore);
		CacheBuffID(bErrorLog: false);
		return true;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return true;
	}
}
