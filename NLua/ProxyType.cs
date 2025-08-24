using System;
using System.Reflection;

namespace NLua;

public class ProxyType
{
	private readonly Type _proxy;

	public Type UnderlyingSystemType => _proxy;

	public ProxyType(Type proxy)
	{
		_proxy = proxy;
	}

	public override string ToString()
	{
		return "ProxyType(" + UnderlyingSystemType?.ToString() + ")";
	}

	public override bool Equals(object obj)
	{
		if (obj is Type)
		{
			return _proxy == (Type)obj;
		}
		if (obj is ProxyType)
		{
			return _proxy == ((ProxyType)obj).UnderlyingSystemType;
		}
		return _proxy.Equals(obj);
	}

	public override int GetHashCode()
	{
		return _proxy.GetHashCode();
	}

	public MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
	{
		return _proxy.GetMember(name, bindingAttr);
	}

	public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Type[] signature)
	{
		return _proxy.GetMethod(name, bindingAttr, null, signature, null);
	}
}
