namespace NLua.Method;

public class LuaEventHandler
{
	public LuaFunction Handler;

	public void HandleEvent(object[] args)
	{
		Handler.Call(args);
	}
}
