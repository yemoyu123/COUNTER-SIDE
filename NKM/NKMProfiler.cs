namespace NKM;

internal static class NKMProfiler
{
	private sealed class NullProvider : IProfilerProvider
	{
		public void BeginSample(string name)
		{
		}

		public void EndSample()
		{
		}
	}

	private static IProfilerProvider provider = new NullProvider();

	internal static void SetProvider(IProfilerProvider provider)
	{
		NKMProfiler.provider = provider;
	}

	internal static void BeginSample(string name)
	{
		provider.BeginSample(name);
	}

	internal static void EndSample()
	{
		provider.EndSample();
	}
}
