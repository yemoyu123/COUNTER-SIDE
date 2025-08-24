using System;
using System.Collections.Generic;
using System.Reflection;
using Cs.Protocol;

namespace Cs.Engine.Network;

internal sealed class PacketHandler
{
	private readonly MethodInfo methodInfo_;

	private readonly ushort packetId_;

	public ushort PacketId => packetId_;

	public static IEnumerable<PacketHandler> Extract(Type containerType)
	{
		MethodInfo[] methods = containerType.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (containerType == typeof(Connection))
			{
				if (methodInfo.IsStatic)
				{
					continue;
				}
			}
			else if (!methodInfo.IsStatic)
			{
				continue;
			}
			if (methodInfo.Name != "OnRecv" || methodInfo.ReturnParameter.ParameterType != typeof(void))
			{
				continue;
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length == 1)
			{
				Type parameterType = parameters[0].ParameterType;
				ushort id = PacketController.Instance.GetId(parameterType);
				if (id != ushort.MaxValue)
				{
					yield return new PacketHandler(methodInfo, id);
				}
			}
		}
	}

	public void Execute(ISerializable message, Connection connection)
	{
		object obj = (methodInfo_.IsStatic ? null : connection);
		methodInfo_.Invoke(obj, new object[1] { message });
	}

	private PacketHandler(MethodInfo methodInfo, ushort packetId)
	{
		methodInfo_ = methodInfo;
		packetId_ = packetId;
	}
}
