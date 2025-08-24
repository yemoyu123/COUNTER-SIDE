using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Contract2;

public class SelectableUnitTemplet
{
	public string UnitStringId { get; }

	public int Ratio { get; }

	public NKMUnitTempletBase UnitTemplet { get; private set; }

	public SelectableUnitTemplet(string unitStringId, int ratio)
	{
		UnitStringId = unitStringId;
		Ratio = ratio;
	}

	public void Join()
	{
		UnitTemplet = NKMUnitManager.GetUnitTempletBase(UnitStringId);
		if (UnitTemplet == null)
		{
			NKMTempletError.Add("[SelectableUnitTemplet] invalid unitId:" + UnitStringId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitTemplet.cs", 25);
		}
	}

	public void Validate()
	{
		if (UnitTemplet != null)
		{
			UnitTemplet.Validate();
			if (UnitTemplet.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL)
			{
				NKMTempletError.Add($"[RandomUnitTemplet] 선별채용 유닛 에러. 타입값이 비정상 unitId:{UnitTemplet.m_UnitID} unitType:{UnitTemplet.m_NKM_UNIT_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitTemplet.cs", 37);
			}
			if (UnitTemplet.m_bMonster)
			{
				NKMTempletError.Add($"[RandomUnitTemplet] 선별채용 유닛 에러. 몬스터 타입인 유닛. unitId:{UnitTemplet.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitTemplet.cs", 42);
			}
			if (NKMCollectionUnitTemplet.Values.FirstOrDefault((NKMCollectionUnitTemplet e) => e.Key == UnitTemplet.m_BaseUnitID) == null)
			{
				NKMTempletError.Add($"[RandomUnitTemplet] 선별채용 유닛 에러. 도감에 포함되지 않은 유닛. unitId:{UnitTemplet.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/SelectableUnitTemplet.cs", 47);
			}
		}
	}
}
