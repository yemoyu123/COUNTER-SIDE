namespace NKC.Patcher;

public static class PatchProcessStrategyExtensions
{
	public static bool ErrorOccurred(this IPatchProcessStrategy process)
	{
		return process.Status == IPatchProcessStrategy.ExecutionStatus.Fail;
	}

	public static bool SkipNextProcess(this IPatchProcessStrategy process)
	{
		return process.Status == IPatchProcessStrategy.ExecutionStatus.SkipNextProcess;
	}
}
