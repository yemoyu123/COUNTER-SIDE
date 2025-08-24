using System;

namespace NLua;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class LuaHideAttribute : Attribute
{
}
