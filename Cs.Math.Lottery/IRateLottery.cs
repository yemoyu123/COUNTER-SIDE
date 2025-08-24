namespace Cs.Math.Lottery;

public interface IRateLottery<T> : IReadOnlyRateLottery<T>
{
	void AddCase(int rate, T value);
}
