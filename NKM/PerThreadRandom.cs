using System;

namespace NKM;

public class PerThreadRandom
{
	[ThreadStatic]
	private static Random random_;

	public static Random Instance
	{
		get
		{
			if (random_ == null)
			{
				random_ = new Random();
			}
			return random_;
		}
	}
}
