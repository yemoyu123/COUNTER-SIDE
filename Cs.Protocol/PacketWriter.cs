using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cs.Engine.Network.Buffer;
using Cs.Engine.Util;
using Cs.Protocol.Detail;

namespace Cs.Protocol;

public sealed class PacketWriter : IDisposable, IPacketStream
{
	private readonly BinaryWriter writer;

	public PacketWriter(BinaryWriter writer)
	{
		this.writer = writer;
	}

	public static ZeroCopyBuffer ToBufferWithoutNullBit(ISerializable data)
	{
		ZeroCopyBuffer zeroCopyBuffer = new ZeroCopyBuffer();
		using PacketWriter packetWriter = new PacketWriter(zeroCopyBuffer.GetWriter());
		packetWriter.PutWithoutNullBit(data);
		return zeroCopyBuffer;
	}

	public void Dispose()
	{
		writer.Dispose();
	}

	public void PutOrGet(ref bool data)
	{
		writer.Write(data);
	}

	public void PutOrGet(ref sbyte data)
	{
		writer.Write(data);
	}

	public void PutOrGet(ref byte data)
	{
		writer.Write(data);
	}

	public void PutOrGet(ref short data)
	{
		WriteRawVarint32(ZigZag.Encode32(data));
	}

	public void PutOrGet(ref ushort data)
	{
		WriteRawVarint32(data);
	}

	public void PutOrGet(ref int data)
	{
		WriteRawVarint32(ZigZag.Encode32(data));
	}

	public void PutOrGet(ref uint data)
	{
		WriteRawVarint32(data);
	}

	public void PutOrGet(ref long data)
	{
		WriteRawVarint64(ZigZag.Encode64(data));
	}

	public void PutOrGet(ref ulong data)
	{
		WriteRawVarint64(data);
	}

	public void PutOrGet(ref float data)
	{
		writer.Write(data);
	}

	public void PutOrGet(ref double data)
	{
		writer.Write(data);
	}

	public void PutOrGet(ref string data)
	{
		PutString(data);
	}

	public void AsHalf(ref float data)
	{
		uint data2 = data.FloatToLow();
		PutOrGet(ref data2);
	}

	public void PutOrGet(ref byte[] data)
	{
		if (data == null)
		{
			PutInt(0);
			return;
		}
		PutInt(data.Length);
		writer.Write(data);
	}

	public void PutOrGet(ref BitArray data)
	{
		byte[] array = data.ToByteArray();
		PutUshort((ushort)array.Length);
		writer.Write(array);
	}

	public void PutOrGet(ref DateTime data)
	{
		PutRawLong(data.ToBinary());
	}

	public void PutOrGet(ref TimeSpan data)
	{
		PutRawLong(data.Ticks);
	}

	public void PutOrGet(ref bool[] data)
	{
		PutUshort((ushort)data.Length);
		for (int i = 0; i < data.Length; i++)
		{
			PutBool(data[i]);
		}
	}

	public void PutOrGet(ref int[] data)
	{
		PutUshort((ushort)data.Length);
		for (int i = 0; i < data.Length; i++)
		{
			PutInt(data[i]);
		}
	}

	public void PutOrGet(ref long[] data)
	{
		PutUshort((ushort)data.Length);
		for (int i = 0; i < data.Length; i++)
		{
			PutLong(data[i]);
		}
	}

	public void PutOrGet<T>(ref T[] data) where T : ISerializable
	{
		PutUshort((ushort)data.Length);
		for (int i = 0; i < data.Length; i++)
		{
			PutMessage(data[i]);
		}
	}

	public void PutOrGet(ref List<bool> data)
	{
		PutUshort((ushort)data.Count);
		foreach (bool datum in data)
		{
			PutBool(datum);
		}
	}

	public void PutOrGet(ref List<byte> data)
	{
		PutUshort((ushort)data.Count);
		foreach (byte datum in data)
		{
			PutByte(datum);
		}
	}

	public void PutOrGet(ref List<short> data)
	{
		PutUshort((ushort)data.Count);
		foreach (short datum in data)
		{
			PutShort(datum);
		}
	}

	public void PutOrGet(ref List<int> data)
	{
		if (data == null)
		{
			PutUshort(0);
			return;
		}
		PutUshort((ushort)data.Count);
		foreach (int datum in data)
		{
			PutInt(datum);
		}
	}

	public void PutOrGet(ref List<float> data)
	{
		PutUshort((ushort)data.Count);
		foreach (float datum in data)
		{
			PutFloat(datum);
		}
	}

	public void PutOrGet(ref List<long> data)
	{
		PutUshort((ushort)data.Count);
		foreach (long datum in data)
		{
			PutLong(datum);
		}
	}

	public void PutOrGet(ref List<string> data)
	{
		PutUshort((ushort)data.Count);
		foreach (string datum in data)
		{
			PutString(datum);
		}
	}

	public void PutOrGet<T>(ref List<T> data) where T : ISerializable
	{
		if (data == null)
		{
			PutUshort(0);
			return;
		}
		PutUshort((ushort)data.Count);
		foreach (T datum in data)
		{
			PutMessage(datum);
		}
	}

	public void PutOrGet(ref HashSet<short> data)
	{
		PutUshort((ushort)data.Count);
		foreach (short datum in data)
		{
			PutShort(datum);
		}
	}

	public void PutOrGet(ref HashSet<int> data)
	{
		PutUshort((ushort)data.Count);
		foreach (int datum in data)
		{
			PutInt(datum);
		}
	}

	public void PutOrGet(ref HashSet<string> data)
	{
		PutUshort((ushort)data.Count);
		foreach (string datum in data)
		{
			PutString(datum);
		}
	}

	public void PutOrGet(ref HashSet<long> data)
	{
		PutUshort((ushort)data.Count);
		foreach (long datum in data)
		{
			PutLong(datum);
		}
	}

	public void PutOrGet<T>(ref HashSet<T> data) where T : ISerializable
	{
		if (data == null)
		{
			PutUshort(0);
			return;
		}
		PutUshort((ushort)data.Count);
		foreach (T datum in data)
		{
			PutMessage(datum);
		}
	}

	public void PutOrGetEnum<T>(ref HashSet<T> data) where T : Enum
	{
		PutUshort((ushort)data.Count);
		foreach (T datum in data)
		{
			PutInt((int)Convert.ChangeType(datum, typeof(int)));
		}
	}

	public void PutOrGet(ref Dictionary<int, int> data)
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<int, int> datum in data)
		{
			PutInt(datum.Key);
			PutInt(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<int, float> data)
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<int, float> datum in data)
		{
			PutInt(datum.Key);
			PutFloat(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<long, int> data)
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<long, int> datum in data)
		{
			PutLong(datum.Key);
			PutInt(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<long, long> data)
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<long, long> datum in data)
		{
			PutLong(datum.Key);
			PutLong(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<long, float> data)
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<long, float> datum in data)
		{
			PutLong(datum.Key);
			PutFloat(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<int, long> data)
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<int, long> datum in data)
		{
			PutInt(datum.Key);
			PutLong(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<byte, byte> data)
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<byte, byte> datum in data)
		{
			PutByte(datum.Key);
			PutByte(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<byte, long> data)
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<byte, long> datum in data)
		{
			PutByte(datum.Key);
			PutLong(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<string, int> data)
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<string, int> datum in data)
		{
			PutString(datum.Key);
			PutInt(datum.Value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<byte, T> data) where T : ISerializable
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<byte, T> datum in data)
		{
			PutByte(datum.Key);
			PutMessage(datum.Value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<short, T> data) where T : ISerializable
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<short, T> datum in data)
		{
			PutShort(datum.Key);
			PutMessage(datum.Value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<int, T> data) where T : ISerializable
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<int, T> datum in data)
		{
			PutInt(datum.Key);
			PutMessage(datum.Value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<long, T> data) where T : ISerializable
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<long, T> datum in data)
		{
			PutLong(datum.Key);
			PutMessage(datum.Value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<string, T> data) where T : ISerializable
	{
		PutUshort((ushort)data.Count);
		foreach (KeyValuePair<string, T> datum in data)
		{
			PutString(datum.Key);
			PutMessage(datum.Value);
		}
	}

	public void PutOrGetEnum<T>(ref T data) where T : Enum
	{
		PutInt((int)Convert.ChangeType(data, typeof(int)));
	}

	public void PutOrGetEnum<T>(ref List<T> data) where T : Enum
	{
		PutUshort((ushort)data.Count);
		for (int i = 0; i < data.Count; i++)
		{
			PutInt((int)Convert.ChangeType(data[i], typeof(int)));
		}
	}

	public void PutOrGet<T>(ref T data) where T : ISerializable
	{
		PutMessage(data);
	}

	public void PutBool(bool data)
	{
		writer.Write(data);
	}

	public void PutSByte(sbyte data)
	{
		writer.Write(data);
	}

	public void PutByte(byte data)
	{
		writer.Write(data);
	}

	public void PutShort(short data)
	{
		WriteRawVarint32(ZigZag.Encode32(data));
	}

	public void PutUshort(ushort data)
	{
		WriteRawVarint32(data);
	}

	public void PutInt(int data)
	{
		WriteRawVarint32(ZigZag.Encode32(data));
	}

	public void PutUint(uint data)
	{
		WriteRawVarint32(data);
	}

	public void PutLong(long data)
	{
		WriteRawVarint64(ZigZag.Encode64(data));
	}

	public void PutUlong(ulong data)
	{
		WriteRawVarint64(data);
	}

	public void PutFloat(float data)
	{
		writer.Write(data);
	}

	public void PutDouble(double data)
	{
		writer.Write(data);
	}

	public void PutRawInt(int data)
	{
		writer.Write(data);
	}

	public void PutRawUint(uint data)
	{
		writer.Write(data);
	}

	public void PutRawLong(long data)
	{
		writer.Write(data);
	}

	public void PutString(string data)
	{
		if (data == null)
		{
			PutShort(-1);
			return;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		PutShort((short)bytes.Length);
		writer.Write(bytes);
	}

	public void PutWithoutNullBit(ISerializable message)
	{
		message.Serialize(this);
	}

	public void PutMessage(ISerializable message)
	{
		if (message == null)
		{
			PutBool(data: false);
			return;
		}
		PutBool(data: true);
		message.Serialize(this);
	}

	private void WriteRawVarint32(uint value)
	{
		while (value > 127)
		{
			writer.Write((byte)((value & 0x7F) | 0x80));
			value >>= 7;
		}
		writer.Write((byte)value);
	}

	private void WriteRawVarint64(ulong value)
	{
		while (value > 127)
		{
			writer.Write((byte)((value & 0x7F) | 0x80));
			value >>= 7;
		}
		writer.Write((byte)value);
	}
}
