using System.Collections.Generic;
using Cs.Math.Lottery;

namespace NKM.Contract2;

public interface IRandomUnitPool
{
	string DebugName { get; }

	int TotalUnitCount { get; }

	IEnumerable<RandomUnitTempletV2> UnitTemplets { get; }

	IReadOnlyList<IEnumerable<RandomUnitTempletV2>> UnitsByPickGrade { get; }

	RandomUnitTempletV2 Decide(NKM_UNIT_PICK_GRADE pickGrade);

	IEnumerable<RandomUnitTempletV2> GetBonusCandidates();

	RatioLottery<RandomUnitTempletV2> GetLottery(NKM_UNIT_PICK_GRADE pickGrade);
}
