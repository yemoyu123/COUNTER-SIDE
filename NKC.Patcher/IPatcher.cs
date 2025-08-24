using System.Collections;

namespace NKC.Patcher;

public interface IPatcher
{
	bool PatchSuccess { get; }

	string ReasonOfFailure { get; }

	void SetActive(bool active);

	IEnumerator ProcessPatch();
}
