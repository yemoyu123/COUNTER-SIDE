namespace NKC;

public class NKCUniConnect
{
	public static void DisconnectTest()
	{
		NKCScenManager.GetScenManager().GetConnectLogin().SimulateDisconnect();
		NKCScenManager.GetScenManager().GetConnectGame().SimulateDisconnect();
	}
}
