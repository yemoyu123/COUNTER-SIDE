using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NLua.Extensions;

internal static class TypeExtensions
{
	public static bool HasMethod(this Type t, string name)
	{
		return t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Any((MethodInfo m) => m.Name == name);
	}

	public static bool HasAdditionOperator(this Type t)
	{
		if (t.IsPrimitive)
		{
			return true;
		}
		return t.HasMethod("op_Addition");
	}

	public static bool HasSubtractionOperator(this Type t)
	{
		if (t.IsPrimitive)
		{
			return true;
		}
		return t.HasMethod("op_Subtraction");
	}

	public static bool HasMultiplyOperator(this Type t)
	{
		if (t.IsPrimitive)
		{
			return true;
		}
		return t.HasMethod("op_Multiply");
	}

	public static bool HasDivisionOperator(this Type t)
	{
		if (t.IsPrimitive)
		{
			return true;
		}
		return t.HasMethod("op_Division");
	}

	public static bool HasModulusOperator(this Type t)
	{
		if (t.IsPrimitive)
		{
			return true;
		}
		return t.HasMethod("op_Modulus");
	}

	public static bool HasUnaryNegationOperator(this Type t)
	{
		if (t.IsPrimitive)
		{
			return true;
		}
		return t.GetMethod("op_UnaryNegation", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public) != null;
	}

	public static bool HasEqualityOperator(this Type t)
	{
		if (t.IsPrimitive)
		{
			return true;
		}
		return t.HasMethod("op_Equality");
	}

	public static bool HasLessThanOperator(this Type t)
	{
		if (t.IsPrimitive)
		{
			return true;
		}
		return t.HasMethod("op_LessThan");
	}

	public static bool HasLessThanOrEqualOperator(this Type t)
	{
		if (t.IsPrimitive)
		{
			return true;
		}
		return t.HasMethod("op_LessThanOrEqual");
	}

	public static MethodInfo[] GetMethods(this Type t, string name, BindingFlags flags)
	{
		return (from m in t.GetMethods(flags)
			where m.Name == name
			select m).ToArray();
	}

	public static MethodInfo[] GetExtensionMethods(this Type type, string name, IEnumerable<Assembly> assemblies = null)
	{
		List<Type> list = new List<Type>();
		list.AddRange(from t in type.Assembly.GetTypes()
			where t.IsPublic
			select t);
		if (assemblies != null)
		{
			foreach (Assembly assembly in assemblies)
			{
				if (!(assembly == type.Assembly))
				{
					list.AddRange(from t in assembly.GetTypes()
						where t.IsPublic && t.IsClass && t.IsSealed && t.IsAbstract && !t.IsNested
						select t);
				}
			}
		}
		return (from extensionType in list
			from method in extensionType.GetMethods(name, BindingFlags.Static | BindingFlags.Public)
			select new { extensionType, method } into t
			where t.method.IsDefined(typeof(ExtensionAttribute), inherit: false)
			where t.method.GetParameters()[0].ParameterType == type || t.method.GetParameters()[0].ParameterType.IsAssignableFrom(type) || type.GetInterfaces().Contains(t.method.GetParameters()[0].ParameterType)
			select t.method).ToArray();
	}

	public static MethodInfo GetExtensionMethod(this Type t, string name, IEnumerable<Assembly> assemblies = null)
	{
		MethodInfo[] array = t.GetExtensionMethods(name, assemblies).ToArray();
		if (array.Length == 0)
		{
			return null;
		}
		return array[0];
	}
}
