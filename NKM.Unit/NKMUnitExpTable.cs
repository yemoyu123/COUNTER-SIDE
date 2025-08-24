using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Unit;

public sealed class NKMUnitExpTable
{
	private readonly NKMUnitExpTemplet[] levels = new NKMUnitExpTemplet[120];

	public IEnumerable<NKMUnitExpTemplet> Levels => levels;

	public NKM_UNIT_GRADE UnitGrade { get; private set; }

	public int RearmGrade { get; private set; }

	public bool IsAwaken { get; private set; }

	private string LogHeader => $"[ExpTable(unitGrade:{UnitGrade} rearmGrade:{RearmGrade} isAwaken:{IsAwaken})]";

	public NKMUnitExpTable(NKM_UNIT_GRADE unitGrade, int rearmGrade, bool isAwaken)
	{
		UnitGrade = unitGrade;
		RearmGrade = rearmGrade;
		IsAwaken = isAwaken;
	}

	public NKMUnitExpTemplet Get(int level)
	{
		int num = level - 1;
		if (num < 0 || num > levels.Length - 1)
		{
			NKMTempletError.Add($"{LogHeader} 잘못된 유닛 레벨:{level}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTable.cs", 30);
			return null;
		}
		return levels[num];
	}

	internal void LoadData(NKMLua lua)
	{
		NKMUnitExpTemplet nKMUnitExpTemplet = NKMUnitExpTemplet.LoadFromLUA(lua);
		int num = nKMUnitExpTemplet.m_iLevel - 1;
		if (num < 0 || num >= levels.Length)
		{
			NKMTempletError.Add($"{LogHeader} 잘못된 유닛 레벨:{nKMUnitExpTemplet.m_iLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTable.cs", 44);
		}
		else if (levels[num] != null)
		{
			NKMTempletError.Add($"{LogHeader} 중복된 유닛 레벨:{nKMUnitExpTemplet.m_iLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTable.cs", 50);
		}
		else
		{
			levels[num] = nKMUnitExpTemplet;
		}
	}

	internal void Join()
	{
		for (int i = 0; i < levels.Length - 1; i++)
		{
			NKMUnitExpTemplet nKMUnitExpTemplet = levels[i];
			NKMUnitExpTemplet nKMUnitExpTemplet2 = levels[i + 1];
			nKMUnitExpTemplet.m_iExpRequired = nKMUnitExpTemplet2.m_iExpCumulated - nKMUnitExpTemplet.m_iExpCumulated;
		}
	}

	internal void Validate()
	{
		bool flag = false;
		for (int i = 0; i < levels.Length; i++)
		{
			if (levels[i] == null)
			{
				NKMTempletError.Add($"{LogHeader} 레벨 데이터 없음. unitLevel:{i + 1}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTable.cs", 74);
				flag = true;
			}
		}
		if (flag)
		{
			return;
		}
		for (int j = 0; j < levels.Length - 1; j++)
		{
			NKMUnitExpTemplet nKMUnitExpTemplet = levels[j];
			NKMUnitExpTemplet nKMUnitExpTemplet2 = levels[j + 1];
			if (nKMUnitExpTemplet.m_iExpCumulated >= nKMUnitExpTemplet2.m_iExpCumulated)
			{
				NKMTempletError.Add($"{LogHeader} {nKMUnitExpTemplet.m_iLevel} 레벨 경험치 총합 {nKMUnitExpTemplet.m_iExpCumulated} >= {nKMUnitExpTemplet2.m_iLevel} 레벨 경험치 총합 {nKMUnitExpTemplet2.m_iExpCumulated}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTable.cs", 90);
			}
		}
		NKMUnitExpTemplet nKMUnitExpTemplet3 = levels.Last();
		if (nKMUnitExpTemplet3.m_iExpRequired != 0)
		{
			NKMTempletError.Add($"{LogHeader} 마지막 요구 경험치가 0이어야 함. exp:{nKMUnitExpTemplet3.m_iExpRequired}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/NKMUnitExpTable.cs", 97);
		}
	}

	public void Drop()
	{
		for (int i = 0; i < levels.Length; i++)
		{
			levels[i] = null;
		}
	}
}
