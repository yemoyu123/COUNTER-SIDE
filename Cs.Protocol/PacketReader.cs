using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cs.Protocol.Detail;
using NKC;

namespace Cs.Protocol;

public sealed class PacketReader : IDisposable, IPacketStream
{
	private readonly BinaryReader reader;

	public PacketReader(BinaryReader reader)
	{
		this.reader = reader;
	}

	public PacketReader(byte[] buffer)
	{
		reader = new BinaryReader(new MemoryStream(buffer));
	}

	public void Dispose()
	{
		reader.Close();
	}

	public void PutOrGet(ref bool data)
	{
		data = reader.ReadBoolean();
	}

	public void PutOrGet(ref sbyte data)
	{
		data = reader.ReadSByte();
	}

	public void PutOrGet(ref byte data)
	{
		data = reader.ReadByte();
	}

	public void PutOrGet(ref short data)
	{
		data = (short)ZigZag.Decode32(ReadRawVarInt32());
	}

	public void PutOrGet(ref ushort data)
	{
		data = (ushort)ReadRawVarInt32();
	}

	public void PutOrGet(ref int data)
	{
		data = ZigZag.Decode32(ReadRawVarInt32());
	}

	public void PutOrGet(ref uint data)
	{
		data = ReadRawVarInt32();
	}

	public void PutOrGet(ref long data)
	{
		data = ZigZag.Decode64(ReadRawVarInt64());
	}

	public void PutOrGet(ref ulong data)
	{
		data = ReadRawVarInt64();
	}

	public void PutOrGet(ref float data)
	{
		data = reader.ReadSingle();
	}

	public void PutOrGet(ref double data)
	{
		data = reader.ReadDouble();
	}

	public void PutOrGet(ref string data)
	{
		data = GetString();
	}

	public void AsHalf(ref float data)
	{
		uint data2 = GetUint();
		data = data2.LowToFloat();
	}

	public void PutOrGet(ref byte[] data)
	{
		int count = GetInt();
		data = reader.ReadBytes(count);
	}

	public void PutOrGet(ref BitArray data)
	{
		ushort count = GetUshort();
		data = new BitArray(reader.ReadBytes(count));
	}

	public void PutOrGet(ref DateTime data)
	{
		data = GetDateTime();
	}

	public void PutOrGet(ref TimeSpan data)
	{
		data = GetTimeSpan();
	}

	public void PutOrGet(ref bool[] data)
	{
		ushort num = GetUshort();
		data = new bool[num];
		for (int i = 0; i < num; i++)
		{
			data[i] = GetBool();
		}
	}

	public void PutOrGet(ref int[] data)
	{
		ushort num = GetUshort();
		data = new int[num];
		for (int i = 0; i < num; i++)
		{
			data[i] = GetInt();
		}
	}

	public void PutOrGet(ref long[] data)
	{
		ushort num = GetUshort();
		data = new long[num];
		for (int i = 0; i < num; i++)
		{
			data[i] = GetLong();
		}
	}

	public void PutOrGet<T>(ref T[] data) where T : ISerializable
	{
		ushort num = GetUshort();
		data = new T[num];
		for (int i = 0; i < num; i++)
		{
			GetMessage<T>(out data[i]);
		}
	}

	public void PutOrGet(ref List<bool> data)
	{
		ushort num = GetUshort();
		data = new List<bool>(num);
		for (int i = 0; i < num; i++)
		{
			data.Add(GetBool());
		}
	}

	public void PutOrGet(ref List<byte> data)
	{
		ushort num = GetUshort();
		data = new List<byte>(num);
		for (int i = 0; i < num; i++)
		{
			data.Add(GetByte());
		}
	}

	public void PutOrGet(ref List<short> data)
	{
		ushort num = GetUshort();
		data = new List<short>(num);
		for (int i = 0; i < num; i++)
		{
			data.Add(GetShort());
		}
	}

	public void PutOrGet(ref List<int> data)
	{
		ushort num = GetUshort();
		data = new List<int>(num);
		for (int i = 0; i < num; i++)
		{
			data.Add(GetInt());
		}
	}

	public void PutOrGet(ref List<float> data)
	{
		ushort num = GetUshort();
		data = new List<float>(num);
		for (int i = 0; i < num; i++)
		{
			data.Add(GetFloat());
		}
	}

	public void PutOrGet(ref List<long> data)
	{
		ushort num = GetUshort();
		data = new List<long>(num);
		for (int i = 0; i < num; i++)
		{
			data.Add(GetLong());
		}
	}

	public void PutOrGet(ref List<string> data)
	{
		ushort num = GetUshort();
		data = new List<string>(num);
		for (int i = 0; i < num; i++)
		{
			data.Add(GetString());
		}
	}

	public void PutOrGet<T>(ref List<T> data) where T : ISerializable
	{
		ushort num = GetUshort();
		if (data != null)
		{
			data.Clear();
		}
		else if (NKCPacketObjectPool.IsManagedType(typeof(List<T>)))
		{
			data = (List<T>)NKCPacketObjectPool.OpenObject(typeof(List<T>));
		}
		else
		{
			data = new List<T>(num);
		}
		ICollection<T> collection = data;
		GetCollection(in collection, num);
	}

	public void PutOrGet(ref HashSet<short> data)
	{
		ushort num = GetUshort();
		data = new HashSet<short>();
		for (int i = 0; i < num; i++)
		{
			short item = GetShort();
			data.Add(item);
		}
	}

	public void PutOrGet(ref HashSet<int> data)
	{
		ushort num = GetUshort();
		data = new HashSet<int>();
		for (int i = 0; i < num; i++)
		{
			int item = GetInt();
			data.Add(item);
		}
	}

	public void PutOrGet(ref HashSet<string> data)
	{
		ushort num = GetUshort();
		data = new HashSet<string>();
		for (int i = 0; i < num; i++)
		{
			string item = GetString();
			data.Add(item);
		}
	}

	public void PutOrGet(ref HashSet<long> data)
	{
		ushort num = GetUshort();
		data = new HashSet<long>();
		for (int i = 0; i < num; i++)
		{
			long item = GetLong();
			data.Add(item);
		}
	}

	public void PutOrGet<T>(ref HashSet<T> data) where T : ISerializable
	{
		ushort count = GetUshort();
		if (data != null)
		{
			data.Clear();
		}
		else if (NKCPacketObjectPool.IsManagedType(typeof(HashSet<T>)))
		{
			data = (HashSet<T>)NKCPacketObjectPool.OpenObject(typeof(HashSet<T>));
		}
		else
		{
			data = new HashSet<T>();
		}
		ICollection<T> collection = data;
		GetCollection(in collection, count);
	}

	public void PutOrGetEnum<T>(ref HashSet<T> data) where T : Enum
	{
		data = new HashSet<T>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			int value = GetInt();
			T item = (T)Enum.ToObject(typeof(T), value);
			data.Add(item);
		}
	}

	public void PutOrGet(ref Dictionary<int, int> data)
	{
		data = new Dictionary<int, int>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			int key = GetInt();
			int value = GetInt();
			data.Add(key, value);
		}
	}

	public void PutOrGet(ref Dictionary<int, float> data)
	{
		data = new Dictionary<int, float>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			int key = GetInt();
			float value = GetFloat();
			data.Add(key, value);
		}
	}

	public void PutOrGet(ref Dictionary<long, int> data)
	{
		data = new Dictionary<long, int>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			long key = GetLong();
			int value = GetInt();
			data.Add(key, value);
		}
	}

	public void PutOrGet(ref Dictionary<long, long> data)
	{
		data = new Dictionary<long, long>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			long key = GetLong();
			long value = GetLong();
			data.Add(key, value);
		}
	}

	public void PutOrGet(ref Dictionary<long, float> data)
	{
		data = new Dictionary<long, float>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			long key = GetLong();
			float value = GetFloat();
			data.Add(key, value);
		}
	}

	public void PutOrGet(ref Dictionary<int, long> data)
	{
		data = new Dictionary<int, long>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			int key = GetInt();
			long value = GetLong();
			data.Add(key, value);
		}
	}

	public void PutOrGet(ref Dictionary<byte, byte> data)
	{
		data = new Dictionary<byte, byte>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			byte key = GetByte();
			byte value = GetByte();
			data.Add(key, value);
		}
	}

	public void PutOrGet(ref Dictionary<byte, long> data)
	{
		data = new Dictionary<byte, long>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			byte key = GetByte();
			long value = GetLong();
			data.Add(key, value);
		}
	}

	public void PutOrGet(ref Dictionary<string, int> data)
	{
		data = new Dictionary<string, int>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			string key = GetString();
			int value = GetInt();
			data.Add(key, value);
		}
	}

	public void PutOrGet<T>(ref Dictionary<byte, T> data) where T : ISerializable
	{
		if (data != null)
		{
			data.Clear();
		}
		else if (NKCPacketObjectPool.IsManagedType(typeof(Dictionary<byte, T>)))
		{
			data = (Dictionary<byte, T>)NKCPacketObjectPool.OpenObject(typeof(Dictionary<byte, T>));
		}
		else
		{
			data = new Dictionary<byte, T>();
		}
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			byte key = GetByte();
			GetMessage<T>(out var message);
			data.Add(key, message);
		}
	}

	public void PutOrGet<T>(ref Dictionary<short, T> data) where T : ISerializable
	{
		if (data != null)
		{
			data.Clear();
		}
		else if (NKCPacketObjectPool.IsManagedType(typeof(Dictionary<short, T>)))
		{
			data = (Dictionary<short, T>)NKCPacketObjectPool.OpenObject(typeof(Dictionary<short, T>));
		}
		else
		{
			data = new Dictionary<short, T>();
		}
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			short key = GetShort();
			GetMessage<T>(out var message);
			data.Add(key, message);
		}
	}

	public void PutOrGet<T>(ref Dictionary<int, T> data) where T : ISerializable
	{
		if (data != null)
		{
			data.Clear();
		}
		else if (NKCPacketObjectPool.IsManagedType(typeof(Dictionary<int, T>)))
		{
			data = (Dictionary<int, T>)NKCPacketObjectPool.OpenObject(typeof(Dictionary<int, T>));
		}
		else
		{
			data = new Dictionary<int, T>();
		}
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			int key = GetInt();
			GetMessage<T>(out var message);
			data.Add(key, message);
		}
	}

	public void PutOrGet<T>(ref Dictionary<long, T> data) where T : ISerializable
	{
		if (data != null)
		{
			data.Clear();
		}
		else if (NKCPacketObjectPool.IsManagedType(typeof(Dictionary<long, T>)))
		{
			data = (Dictionary<long, T>)NKCPacketObjectPool.OpenObject(typeof(Dictionary<long, T>));
		}
		else
		{
			data = new Dictionary<long, T>();
		}
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			long key = GetLong();
			GetMessage<T>(out var message);
			data.Add(key, message);
		}
	}

	public void PutOrGet<T>(ref Dictionary<string, T> data) where T : ISerializable
	{
		if (data != null)
		{
			data.Clear();
		}
		else if (NKCPacketObjectPool.IsManagedType(typeof(Dictionary<string, T>)))
		{
			data = (Dictionary<string, T>)NKCPacketObjectPool.OpenObject(typeof(Dictionary<string, T>));
		}
		else
		{
			data = new Dictionary<string, T>();
		}
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			string key = GetString();
			GetMessage<T>(out var message);
			data.Add(key, message);
		}
	}

	public void PutOrGetEnum<T>(ref T data) where T : Enum
	{
		int value = GetInt();
		data = (T)Enum.ToObject(typeof(T), value);
	}

	public void PutOrGetEnum<T>(ref List<T> data) where T : Enum
	{
		data = new List<T>();
		ushort num = GetUshort();
		for (int i = 0; i < num; i++)
		{
			int value = GetInt();
			T item = (T)Enum.ToObject(typeof(T), value);
			data.Add(item);
		}
	}

	public void PutOrGet<T>(ref T data) where T : ISerializable
	{
		GetMessage<T>(out data);
	}

	public bool GetBool()
	{
		return reader.ReadBoolean();
	}

	public sbyte GetSByte()
	{
		return reader.ReadSByte();
	}

	public byte GetByte()
	{
		return reader.ReadByte();
	}

	public short GetShort()
	{
		return (short)ZigZag.Decode32(ReadRawVarInt32());
	}

	public ushort GetUshort()
	{
		return (ushort)ReadRawVarInt32();
	}

	public int GetInt()
	{
		return ZigZag.Decode32(ReadRawVarInt32());
	}

	public uint GetUint()
	{
		return ReadRawVarInt32();
	}

	public long GetLong()
	{
		return ZigZag.Decode64(ReadRawVarInt64());
	}

	public ulong GetUlong()
	{
		return ReadRawVarInt64();
	}

	public float GetFloat()
	{
		return reader.ReadSingle();
	}

	public double GetDouble()
	{
		return reader.ReadDouble();
	}

	public int GetRawInt()
	{
		return reader.ReadInt32();
	}

	public uint GetRawUint()
	{
		return reader.ReadUInt32();
	}

	public long GetRawLong()
	{
		return reader.ReadInt64();
	}

	public string GetString()
	{
		short num = GetShort();
		if (num == -1)
		{
			return null;
		}
		byte[] bytes = reader.ReadBytes(num);
		return Encoding.UTF8.GetString(bytes);
	}

	public DateTime GetDateTime()
	{
		return DateTime.FromBinary(GetRawLong());
	}

	public TimeSpan GetTimeSpan()
	{
		return new TimeSpan(GetRawLong());
	}

	public void GetWithoutNullBit(ISerializable message)
	{
		message.Serialize(this);
	}

	private void GetCollection<T>(in ICollection<T> collection, int count) where T : ISerializable
	{
		for (int i = 0; i < count; i++)
		{
			if (!GetBool())
			{
				collection.Add(default(T));
				continue;
			}
			T item = (T)NKCPacketObjectPool.OpenObject(typeof(T));
			item.Serialize(this);
			collection.Add(item);
		}
	}

	private void GetMessage<T>(out T message) where T : ISerializable
	{
		if (!GetBool())
		{
			message = default(T);
			return;
		}
		message = (T)NKCPacketObjectPool.OpenObject(typeof(T));
		message.Serialize(this);
	}

	private uint ReadRawVarInt32()
	{
		int i = 0;
		uint num = 0u;
		for (; i < 32; i += 7)
		{
			byte b = reader.ReadByte();
			num |= (uint)((b & 0x7F) << i);
			if ((b & 0x80) == 0)
			{
				return num;
			}
		}
		throw new Exception("[PacketReader] Malformed Varint32");
	}

	private ulong ReadRawVarInt64()
	{
		int i = 0;
		ulong num = 0uL;
		for (; i < 64; i += 7)
		{
			byte b = reader.ReadByte();
			num |= (ulong)((long)(b & 0x7F) << i);
			if ((b & 0x80) == 0)
			{
				return num;
			}
		}
		throw new Exception("[PacketReader] Malformed Varint64");
	}
}
