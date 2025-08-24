namespace NKM;

internal interface IProfilerProvider
{
	void BeginSample(string name);

	void EndSample();
}
