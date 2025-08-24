namespace NKC.UI.HUD;

internal interface IGameHudAlert
{
	void OnStart();

	void OnUpdate();

	void OnCleanup();

	bool IsFinished();
}
