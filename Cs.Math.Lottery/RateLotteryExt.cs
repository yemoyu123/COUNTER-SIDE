namespace Cs.Math.Lottery;

public static class RateLotteryExt
{
	public static void AddCase<T>(this IRateLottery<T> self, params (int rate, T value)[] cases)
	{
		for (int i = 0; i < cases.Length; i++)
		{
			var (rate, value) = cases[i];
			self.AddCase(rate, value);
		}
	}
}
