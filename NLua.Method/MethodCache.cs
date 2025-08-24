using System.Reflection;

namespace NLua.Method;

internal class MethodCache
{
	private MethodBase _cachedMethod;

	public bool IsReturnVoid;

	public object[] args;

	public int[] outList;

	public MethodArgs[] argTypes;

	public MethodBase cachedMethod
	{
		get
		{
			return _cachedMethod;
		}
		set
		{
			_cachedMethod = value;
			MethodInfo methodInfo = value as MethodInfo;
			if (methodInfo != null)
			{
				IsReturnVoid = methodInfo.ReturnType == typeof(void);
			}
		}
	}

	public MethodCache()
	{
		args = new object[0];
		argTypes = new MethodArgs[0];
		outList = new int[0];
	}
}
