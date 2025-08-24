using System.Collections;
using AssetBundles;

namespace NKC.Patcher;

public class WaitForAssetBundleInitialize : IPatchProcessStrategy, IEnumerable
{
	public IPatchProcessStrategy.ExecutionStatus Status => IPatchProcessStrategy.ExecutionStatus.Success;

	public string ReasonOfFailure => string.Empty;

	public IEnumerator GetEnumerator()
	{
		yield return AssetBundleManager.Initialize();
	}
}
