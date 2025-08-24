namespace Cs.Math.Lottery;

public interface IRatioLottery<T> : IReadonlyLottery<T>
{
	void AddCase(int ratio, T value);
}
