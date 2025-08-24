using System;

namespace NKM.Templet;

public interface IIntervalTemplet
{
	bool IsValid { get; }

	bool IsValidTime(DateTime serviceTime);

	DateTime CalcStartDate(DateTime current);

	DateTime CalcEndDate(DateTime current);
}
