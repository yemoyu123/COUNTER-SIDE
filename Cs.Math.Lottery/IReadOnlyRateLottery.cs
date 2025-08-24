namespace Cs.Math.Lottery;

public interface IReadOnlyRateLottery<T>
{
	int TotalRate { get; }

	T Decide();

	bool Decide(out T result);

	bool HasValue(T value);

	bool TryGetRatePercent(T value, out float ratePercent);
}
