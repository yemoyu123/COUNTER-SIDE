using System;

namespace NLua.Method;

internal class MethodArgs
{
	public int Index;

	public Type ParameterType;

	public ExtractValue ExtractValue;

	public bool IsParamsArray;
}
