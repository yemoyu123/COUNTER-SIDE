namespace NKM.Templet.Base;

public interface INKMTemplet
{
	int Key { get; }

	void Join();

	void Validate();
}
