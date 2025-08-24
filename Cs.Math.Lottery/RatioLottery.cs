using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cs.Math.Lottery;

public sealed class RatioLottery<T> : IRatioLottery<T>, IReadonlyLottery<T>, IEnumerable<T>, IEnumerable
{
	private readonly struct CaseData
	{
		public int Ratio { get; }

		public int AccumulatedRatio { get; }

		public T Value { get; }

		public CaseData(int ratio, int accumulatedRatio, T value)
		{
			Ratio = ratio;
			AccumulatedRatio = accumulatedRatio;
			Value = value;
		}
	}

	private readonly List<CaseData> cases = new List<CaseData>();

	private int totalRatio;

	public int TotalRatio => totalRatio;

	public int Count => cases.Count;

	public IEnumerable<T> CaseValues => cases.Select((CaseData e) => e.Value);

	public T this[int index] => cases[index].Value;

	public void AddCase(int ratio, T value)
	{
		if (ratio < 0)
		{
			throw new Exception($"invalid ratio data. ratio:{ratio}");
		}
		totalRatio += ratio;
		cases.Add(new CaseData(ratio, totalRatio, value));
	}

	public T Decide()
	{
		int num = RandomGenerator.Next(TotalRatio);
		foreach (CaseData @case in cases)
		{
			if (num < @case.AccumulatedRatio)
			{
				return @case.Value;
			}
		}
		throw new Exception($"[RatioLottery] pick failed. randomValue:{num} totalRatio:{totalRatio}");
	}

	public IEnumerator<T> GetEnumerator()
	{
		return CaseValues.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)CaseValues).GetEnumerator();
	}
}
