using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs.Math.Lottery;

public sealed class RateLottery<T> : IRateLottery<T>, IReadOnlyRateLottery<T>
{
	public readonly struct CaseData
	{
		public int Rate { get; }

		public int AccumulatedRate { get; }

		public T Value { get; }

		public float RatePercent => (float)Rate * 0.01f;

		public CaseData(int rate, int accumulatedRate, T value)
		{
			Rate = rate;
			AccumulatedRate = accumulatedRate;
			Value = value;
		}

		public override string ToString()
		{
			return $"{Value.ToString()} ({RatePercent:0.00}%)";
		}
	}

	private const int MaxRate = 10000;

	private readonly List<CaseData> cases = new List<CaseData>();

	private readonly T defaultValue;

	private int totalRate;

	public int TotalRate => totalRate;

	public int CaseCount => cases.Count;

	public bool HasDefaultRate => totalRate < 10000;

	public bool HasFullRate => totalRate == 10000;

	public IEnumerable<CaseData> Cases
	{
		get
		{
			if (!HasFullRate)
			{
				return cases.Union(new CaseData[1] { DefaultCase });
			}
			return cases;
		}
	}

	private CaseData DefaultCase => new CaseData(10000 - totalRate, 10000, defaultValue);

	public RateLottery(T defaultValue)
	{
		this.defaultValue = defaultValue;
	}

	public void AddCase(int rate, T value)
	{
		if (rate < 0 || totalRate + rate > 10000)
		{
			throw new Exception($"rate value overflow. current:{totalRate} add:{rate}");
		}
		if (rate != 0)
		{
			totalRate += rate;
			cases.Add(new CaseData(rate, totalRate, value));
		}
	}

	public bool Decide(out T result)
	{
		int num = RandomGenerator.Next(10000);
		foreach (CaseData @case in cases)
		{
			if (num < @case.AccumulatedRate)
			{
				result = @case.Value;
				return true;
			}
		}
		result = defaultValue;
		return false;
	}

	public T Decide()
	{
		Decide(out T result);
		return result;
	}

	public bool Decide(out CaseData result)
	{
		int num = RandomGenerator.Next(10000);
		foreach (CaseData @case in cases)
		{
			if (num < @case.AccumulatedRate)
			{
				result = @case;
				return true;
			}
		}
		result = DefaultCase;
		return false;
	}

	public CaseData DecideDetail()
	{
		Decide(out CaseData result);
		return result;
	}

	public bool TryGetRatePercent(T value, out float ratePercent)
	{
		foreach (CaseData @case in Cases)
		{
			if (@case.Value.Equals(value))
			{
				ratePercent = @case.RatePercent;
				return true;
			}
		}
		ratePercent = 0f;
		return false;
	}

	public bool HasValue(T value)
	{
		return cases.Any((CaseData e) => e.Value.Equals(value));
	}
}
