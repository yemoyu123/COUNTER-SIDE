using System;
using System.IO;
using Cs.Engine.Network.Buffer;
using NKC;

namespace Cs.Protocol;

public static class SerializerExt
{
	public static T DeepCopy<T>(this T source) where T : class, ISerializable, new()
	{
		ZeroCopyBuffer zeroCopyBuffer = PacketWriter.ToBufferWithoutNullBit(source);
		using (zeroCopyBuffer.Hold())
		{
			using PacketReader stream = new PacketReader(zeroCopyBuffer.GetReader());
			T val = NKCPacketObjectPool.OpenObject<T>();
			val.Serialize(stream);
			return val;
		}
	}

	public static void DeepCopyFrom<T>(this T copied, T source) where T : class, ISerializable
	{
		ZeroCopyBuffer zeroCopyBuffer = PacketWriter.ToBufferWithoutNullBit(source);
		using (zeroCopyBuffer.Hold())
		{
			using PacketReader stream = new PacketReader(zeroCopyBuffer.GetReader());
			copied.Serialize(stream);
		}
	}

	public static void SaveToFile<T>(this T data, string filePath, string fileName) where T : class, ISerializable
	{
		ZeroCopyBuffer zeroCopyBuffer = PacketWriter.ToBufferWithoutNullBit(data);
		using (zeroCopyBuffer.Hold())
		{
			zeroCopyBuffer.WriteToFile(filePath, fileName);
		}
	}

	public static void ReadFromFile<T>(this T data, string fullFilePath) where T : class, ISerializable
	{
		if (File.Exists(fullFilePath))
		{
			using (PacketReader packetReader = new PacketReader(File.ReadAllBytes(fullFilePath)))
			{
				packetReader.GetWithoutNullBit(data);
			}
		}
	}

	public static string ToBase64<T>(this T data) where T : class, ISerializable
	{
		return PacketWriter.ToBufferWithoutNullBit(data).ToBase64();
	}

	public static bool FromBase64<T>(this T data, string base64) where T : class, ISerializable
	{
		try
		{
			using (PacketReader packetReader = new PacketReader(Convert.FromBase64String(base64)))
			{
				packetReader.GetWithoutNullBit(data);
			}
			return true;
		}
		catch
		{
		}
		return false;
	}
}
