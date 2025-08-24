namespace Cs.Math.Lottery;

public interface IReadonlyLottery<T>
{
	int TotalRatio { get; }

	int Count { get; }

	T this[int index] { get; }

	T Decide();
}
