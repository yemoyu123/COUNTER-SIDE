using System;

namespace NLua;

[AttributeUsage(AttributeTargets.Method)]
public sealed class LuaGlobalAttribute : Attribute
{
	public string Name { get; set; }

	public string Description { get; set; }
}
