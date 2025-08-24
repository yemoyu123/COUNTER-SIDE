using System;
using System.Linq;
using System.Reflection;
using NKM.Templet.Base;

namespace NKC;

internal static class NKCTempletContainerUtil
{
	private static readonly Type[] containerTypes;

	private static readonly Type[] containerTypesEx;

	static NKCTempletContainerUtil()
	{
		Type openedContainerType = typeof(NKMTempletContainer<>);
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		containerTypes = (from templetType in executingAssembly.GetTypes().Where(Filter)
			select openedContainerType.MakeGenericType(templetType)).ToArray();
		containerTypesEx = (from templetType in executingAssembly.GetTypes().Where(FilterEx)
			select openedContainerType.MakeGenericType(templetType)).ToArray();
		static bool Filter(Type type)
		{
			if (type.GetInterface("INKMTemplet") != null)
			{
				return type.GetCustomAttribute<SkipDerivedClassJoinAttribute>() == null;
			}
			return false;
		}
		static bool FilterEx(Type type)
		{
			if (type.GetInterface("INKMTempletEx") != null)
			{
				return type.GetCustomAttribute<SkipDerivedClassJoinAttribute>() == null;
			}
			return false;
		}
	}

	public static void InvokeJoin()
	{
		Type[] array = containerTypes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetMethod("Join").Invoke(null, null);
		}
	}

	public static void InvokePostJoin()
	{
		Type[] array = containerTypesEx;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetMethod("PostJoin").Invoke(null, null);
		}
	}

	public static void InvokeValidate()
	{
		Type[] array = containerTypes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetMethod("Validate").Invoke(null, null);
		}
	}

	public static void InvokeDrop()
	{
		Type[] array = containerTypes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetMethod("Drop").Invoke(null, null);
		}
	}
}
