namespace NKA.Service;

public interface IService
{
	void BindService();

	void UnbindService();

	void OnPause(bool pauseState);

	bool IsValid();
}
