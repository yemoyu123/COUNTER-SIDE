using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cs.Engine.Network.Buffer;
using Cs.Engine.Util;
using Cs.Protocol.Detail;

namespace Cs.Protocol;

public sealed class PacketSizeChecker : IPacketStream
{
	private int size;

	public int Size => size;

	public void PutOrGet(ref bool data)
	{
		size++;
	}

	public void PutOrGet(ref sbyte data)
	{
		size++;
	}

	public void PutOrGet(ref byte data)
	{
		size++;
	}

	public void PutOrGet(ref short data)
	{
		CheckInt32(data);
	}

	public void PutOrGet(ref ushort data)
	{
		CheckUint32(data);
	}

	public void PutOrGet(ref int data)
	{
		CheckInt32(data);
	}

	public void PutOrGet(ref uint data)
	{
		CheckUint32(data);
	}

	public void PutOrGet(ref long data)
	{
		CheckInt64(data);
	}

	public void PutOrGet(ref ulong data)
	{
		CheckUint64(data);
	}

	public void PutOrGet(ref float data)
	{
		size += 4;
	}

	public void PutOrGet(ref double data)
	{
		size += 8;
	}

	public void PutOrGet(ref string data)
	{
		CheckString(data);
	}

	public void CheckRawInt32(int data)
	{
		size += 4;
	}

	public void CheckRawUint32(uint data)
	{
		size += 4;
	}

	public void AsHalf(ref float data)
	{
		uint data2 = data.FloatToLow();
		PutOrGet(ref data2);
	}

	public void PutOrGet(ref byte[] data)
	{
		CheckInt32(data.Length);
		size += data.Length;
	}

	public void PutOrGet(ZeroCopyBuffer data)
	{
		int num = data.CalcTotalSize();
		CheckInt32(num);
		size += num;
	}

	public void PutOrGet(ref BitArray data)
	{
		ushort num = (ushort)data.ToByteArray().Length;
		CheckUint32(num);
		size += num;
	}

	public void PutOrGet(ref DateTime data)
	{
		size += 8;
	}

	public void PutOrGet(ref TimeSpan data)
	{
		size += 8;
	}

	public void PutOrGet(ref bool[] data)
	{
		ushort num = (ushort)data.Length;
		CheckUint32(num);
		size += num;
	}

	public void PutOrGet(ref int[] data)
	{
		CheckUint32((ushort)data.Length);
		int[] array = data;
		foreach (int data2 in array)
		{
			CheckInt32(data2);
		}
	}

	public void PutOrGet(ref long[] data)
	{
		CheckUint32((ushort)data.Length);
		long[] array = data;
		foreach (long data2 in array)
		{
			CheckInt64(data2);
		}
	}

	public void PutOrGet<T>(ref T[] data) where T : ISerializable
	{
		CheckUint32((ushort)data.Length);
		for (int i = 0; i < data.Length; i++)
		{
			CheckMessage(data[i]);
		}
	}

	public void PutOrGet(ref List<bool> data)
	{
		CheckUint32((ushort)data.Count);
		size += data.Count;
	}

	public void PutOrGet(ref List<byte> data)
	{
		CheckUint32((ushort)data.Count);
		size += data.Count;
	}

	public void PutOrGet(ref List<short> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (short datum in data)
		{
			CheckInt32(datum);
		}
	}

	public void PutOrGet(ref List<int> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (int datum in data)
		{
			CheckInt32(datum);
		}
	}

	public void PutOrGet(ref List<float> data)
	{
		CheckUint32((ushort)data.Count);
		size += 4 * data.Count;
	}

	public void PutOrGet(ref List<long> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (long datum in data)
		{
			CheckInt64(datum);
		}
	}

	public void PutOrGet(ref List<string> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (string datum in data)
		{
			CheckString(datum);
		}
	}

	public void PutOrGet<T>(ref List<T> data) where T : ISerializable
	{
		CheckUint32((ushort)data.Count);
		foreach (T datum in data)
		{
			CheckMessage(datum);
		}
	}

	public void PutOrGet(ref HashSet<short> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (short datum in data)
		{
			CheckInt32(datum);
		}
	}

	public void PutOrGet(ref HashSet<int> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (int datum in data)
		{
			CheckInt32(datum);
		}
	}

	public void PutOrGet(ref HashSet<string> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (string datum in data)
		{
			CheckString(datum);
		}
	}

	public void PutOrGet(ref HashSet<long> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (long datum in data)
		{
			CheckInt64(datum);
		}
	}

	public void PutOrGet<T>(ref HashSet<T> data) where T : ISerializable
	{
		CheckUint32((ushort)data.Count);
		foreach (T datum in data)
		{
			CheckMessage(datum);
		}
	}

	public void PutOrGetEnum<T>(ref HashSet<T> data) where T : Enum
	{
		CheckUint32((ushort)data.Count);
		foreach (T datum in data)
		{
			CheckEnum(datum);
		}
	}

	public void PutOrGet(ref Dictionary<int, int> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<int, int> datum in data)
		{
			CheckInt32(datum.Key);
			CheckInt32(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<int, float> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<int, float> datum in data)
		{
			CheckInt32(datum.Key);
			CheckFloat(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<int, long> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<int, long> datum in data)
		{
			CheckInt32(datum.Key);
			CheckInt64(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<long, int> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<long, int> datum in data)
		{
			CheckInt64(datum.Key);
			CheckInt32(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<long, long> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<long, long> datum in data)
		{
			CheckInt64(datum.Key);
			CheckInt64(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<long, float> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<long, float> datum in data)
		{
			CheckInt64(datum.Key);
			CheckFloat(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<byte, byte> data)
	{
		CheckUint32((ushort)data.Count);
		size += data.Count * 2;
	}

	public void PutOrGet(ref Dictionary<byte, long> data)
	{
		CheckUint32((ushort)data.Count);
		size += data.Count;
		foreach (KeyValuePair<byte, long> datum in data)
		{
			CheckInt64(datum.Value);
		}
	}

	public void PutOrGet(ref Dictionary<string, int> data)
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<string, int> datum in data)
		{
			CheckString(datum.Key);
			CheckInt32(datum.Value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<byte, T> data) where T : ISerializable
	{
		CheckUint32((ushort)data.Count);
		size += data.Count;
		foreach (T value in data.Values)
		{
			CheckMessage(value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<short, T> data) where T : ISerializable
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<short, T> datum in data)
		{
			CheckInt32(datum.Key);
			CheckMessage(datum.Value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<int, T> data) where T : ISerializable
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<int, T> datum in data)
		{
			CheckInt32(datum.Key);
			CheckMessage(datum.Value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<long, T> data) where T : ISerializable
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<long, T> datum in data)
		{
			CheckInt64(datum.Key);
			CheckMessage(datum.Value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<string, T> data) where T : ISerializable
	{
		CheckUint32((ushort)data.Count);
		foreach (KeyValuePair<string, T> datum in data)
		{
			CheckString(datum.Key);
			CheckMessage(datum.Value);
		}
	}

	public void PutOrGetEnum<T>(ref T data) where T : Enum
	{
		CheckEnum(data);
	}

	public void PutOrGetEnum<T>(ref List<T> list) where T : Enum
	{
		CheckUint32((ushort)list.Count);
		foreach (T item in list)
		{
			CheckEnum(item);
		}
	}

	public void PutOrGet<T>(ref T data) where T : ISerializable
	{
		CheckMessage(data);
	}

	public void CheckMessage(ISerializable message)
	{
		size++;
		message?.Serialize(this);
	}

	private static int ComputeRawVarint32Size(uint value)
	{
		if ((value & 0xFFFFFF80u) == 0)
		{
			return 1;
		}
		if ((value & 0xFFFFC000u) == 0)
		{
			return 2;
		}
		if ((value & 0xFFE00000u) == 0)
		{
			return 3;
		}
		if ((value & 0xF0000000u) == 0)
		{
			return 4;
		}
		return 5;
	}

	private static int ComputeRawVarint64Size(ulong value)
	{
		if ((value & 0xFFFFFFFFFFFFFF80uL) == 0L)
		{
			return 1;
		}
		if ((value & 0xFFFFFFFFFFFFC000uL) == 0L)
		{
			return 2;
		}
		if ((value & 0xFFFFFFFFFFE00000uL) == 0L)
		{
			return 3;
		}
		if ((value & 0xFFFFFFFFF0000000uL) == 0L)
		{
			return 4;
		}
		if ((value & 0xFFFFFFF800000000uL) == 0L)
		{
			return 5;
		}
		if ((value & 0xFFFFFC0000000000uL) == 0L)
		{
			return 6;
		}
		if ((value & 0xFFFE000000000000uL) == 0L)
		{
			return 7;
		}
		if ((value & 0xFF00000000000000uL) == 0L)
		{
			return 8;
		}
		if ((value & 0x8000000000000000uL) == 0L)
		{
			return 9;
		}
		return 10;
	}

	private void CheckString(string data)
	{
		if (data == null)
		{
			CheckInt32(-1);
			return;
		}
		CheckInt32(data.Length);
		size += Encoding.UTF8.GetByteCount(data);
	}

	private void CheckInt32(int data)
	{
		size += ComputeRawVarint32Size(ZigZag.Encode32(data));
	}

	private void CheckUint32(uint data)
	{
		size += ComputeRawVarint32Size(data);
	}

	private void CheckInt64(long data)
	{
		size += ComputeRawVarint64Size(ZigZag.Encode64(data));
	}

	private void CheckUint64(ulong data)
	{
		size += ComputeRawVarint64Size(data);
	}

	private void CheckFloat(float data)
	{
		size += 4;
	}

	private void CheckEnum<T>(T data) where T : Enum
	{
		int data2 = (int)Convert.ChangeType(data, typeof(int));
		CheckInt32(data2);
	}
}
