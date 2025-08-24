using System.Collections.Generic;

public static class CallBackHandler
{
	private static int _seedHandler = 0;

	private static Dictionary<int, object> _callbackDic = new Dictionary<int, object>();

	public static int RegisterCallBack(object callback)
	{
		if (callback == null)
		{
			return -1;
		}
		_seedHandler++;
		_callbackDic.Add(_seedHandler, callback);
		return _seedHandler;
	}

	public static T GetCallback<T>(int handle) where T : class
	{
		if (_callbackDic.ContainsKey(handle))
		{
			return (T)_callbackDic[handle];
		}
		return null;
	}

	public static void UnregisterCallback(int handle)
	{
		if (_callbackDic.ContainsKey(handle))
		{
			_callbackDic.Remove(handle);
		}
	}

	public static void ClearAll()
	{
		_callbackDic.Clear();
	}
}
