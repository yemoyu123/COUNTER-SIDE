using System.Collections.Generic;
using NKM.Templet;

namespace NKM.Game;

public class NKMEventConditionBuffCount : NKMEventConditionDetail
{
	public NKMMinMaxInt m_BuffCount = new NKMMinMaxInt(-1, -1);

	private bool m_bDebuff;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_bDebuff", ref m_bDebuff);
		m_BuffCount.LoadFromLua(cNKMLua, "m_BuffCount");
		return true;
	}

	public override bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner)
	{
		int num = 0;
		foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in cNKMUnit.GetUnitFrameData().m_dicBuffData)
		{
			NKMBuffData value = dicBuffDatum.Value;
			NKMBuffTemplet nKMBuffTemplet = value.m_NKMBuffTemplet;
			if (nKMBuffTemplet.m_bBuffCount && value.m_BuffSyncData.m_bAffect && nKMBuffTemplet.m_bDebuff == m_bDebuff)
			{
				num++;
			}
		}
		return m_BuffCount.IsBetween(num, negativeIsTrue: true);
	}

	public override NKMEventConditionDetail Clone()
	{
		NKMEventConditionBuffCount nKMEventConditionBuffCount = new NKMEventConditionBuffCount();
		nKMEventConditionBuffCount.m_BuffCount.DeepCopyFromSource(m_BuffCount);
		nKMEventConditionBuffCount.m_bDebuff = m_bDebuff;
		return nKMEventConditionBuffCount;
	}

	public override bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		return NKMEventConditionV2.ValidateNKMMinMax(m_BuffCount, "[NKMEventConditionBuffCount] m_BuffCount\ufffd\ufffd \ufffdǹ\u033e\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd");
	}
}
