using System;
using System.IO;
using System.Reflection;
using Cs.Engine;
using Cs.Engine.Network.Buffer;
using Cs.Engine.Network.Buffer.Detail;
using Cs.Engine.Util;
using Cs.Logging;

namespace Cs.Protocol;

public struct Packet
{
	private const uint HeadFence = 2864434397u;

	private const uint TailFence = 287454020u;

	private const int CompressThreshold = 1024;

	private const ushort MinHeaderSize = 12;

	private long sequence;

	private ushort packetId;

	private bool compressed;

	private ZeroCopyBuffer buffer;

	private int totalLength;

	public ushort PacketId => packetId;

	public static Packet? Pack(ISerializable data, long sequence)
	{
		Packet value = new Packet
		{
			sequence = sequence,
			packetId = PacketController.Instance.GetId(data)
		};
		if (value.packetId == ushort.MaxValue)
		{
			Log.Error(MethodBase.GetCurrentMethod().Name + " invalid data", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Cs.Protocol/Packet.cs", 36);
			return null;
		}
		value.buffer = PacketWriter.ToBufferWithoutNullBit(data);
		if (value.buffer.CalcTotalSize() > 1024)
		{
			value.buffer.Lz4Compress();
			value.compressed = true;
		}
		else
		{
			value.buffer.Encrypt();
		}
		value.totalLength = value.CalcTotalLength();
		return value;
	}

	internal static bool ProcessRecv(MemoryPipe pipe, RecvController recvController)
	{
		int i = 0;
		Stream readStream = pipe.GetReadStream();
		using (BinaryReader binaryReader = new BinaryReader(readStream))
		{
			using PacketReader packetReader = new PacketReader(binaryReader);
			int num3;
			for (; pipe.Length >= i + 12; i += num3)
			{
				readStream.Seek(i, SeekOrigin.Begin);
				if (binaryReader.ReadUInt32() != 2864434397u)
				{
					Log.Error("invalid head fence.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Cs.Protocol/Packet.cs", 73);
					return false;
				}
				int num = binaryReader.ReadInt32();
				if (num <= 12)
				{
					Log.Error($"invalid packet length:{num} headerSize:{(ushort)12}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Cs.Protocol/Packet.cs", 80);
					return false;
				}
				if (pipe.Length < i + num)
				{
					break;
				}
				packetReader.GetLong();
				ushort id = packetReader.GetUshort();
				bool flag = packetReader.GetBool();
				int num2 = (int)readStream.Position - i;
				num3 = num - num2;
				readStream.Seek(i + num - 4, SeekOrigin.Begin);
				if (binaryReader.ReadUInt32() != 287454020)
				{
					Log.Error("invalid tail fence. packetId:" + PacketController.Instance.GetIdStr(id), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Cs.Protocol/Packet.cs", 100);
					return false;
				}
				i += num2;
				readStream.Seek(i, SeekOrigin.Begin);
				ISerializable serializable = Extract(packetReader, id, flag);
				if (serializable == null)
				{
					return false;
				}
				recvController.Enqueue(serializable, id);
			}
		}
		pipe.Adavnce(i);
		return true;
	}

	public static ISerializable Extract(PacketReader reader, ushort packetId, bool compressed)
	{
		ISerializable serializable = PacketController.Instance.Create(packetId);
		if (serializable == null)
		{
			Log.Error("deserializing failed. id:" + PacketController.Instance.GetIdStr(packetId), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Cs.Protocol/Packet.cs", 127);
			return null;
		}
		try
		{
			byte[] data = null;
			reader.PutOrGet(ref data);
			if (compressed)
			{
				ZeroCopyBuffer zeroCopyBuffer = Lz4Util.Decompress(data);
				using (zeroCopyBuffer.Hold())
				{
					using PacketReader packetReader = new PacketReader(zeroCopyBuffer.GetReader());
					packetReader.GetWithoutNullBit(serializable);
				}
			}
			else
			{
				Crypto.Encrypt(data, data.Length);
				using PacketReader packetReader2 = new PacketReader(data);
				packetReader2.GetWithoutNullBit(serializable);
			}
			return serializable;
		}
		catch (Exception ex)
		{
			Log.Error("exception:" + ex.Message + " id:" + PacketController.Instance.GetIdStr(packetId), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/COMMON/Cs.Protocol/Packet.cs", 158);
			return null;
		}
	}

	public void WriteTo(SendBuffer sendBuffer)
	{
		using BinaryWriter writer = sendBuffer.GetWriter();
		using PacketWriter packetWriter = new PacketWriter(writer);
		packetWriter.PutRawUint(2864434397u);
		packetWriter.PutRawInt(totalLength);
		packetWriter.PutOrGet(ref sequence);
		packetWriter.PutOrGet(ref packetId);
		packetWriter.PutOrGet(ref compressed);
		packetWriter.PutInt(buffer.CalcTotalSize());
		sendBuffer.Absorb(buffer);
		packetWriter.PutRawUint(287454020u);
	}

	private int CalcTotalLength()
	{
		PacketSizeChecker packetSizeChecker = new PacketSizeChecker();
		packetSizeChecker.CheckRawUint32(2864434397u);
		packetSizeChecker.CheckRawInt32(totalLength);
		packetSizeChecker.PutOrGet(ref sequence);
		packetSizeChecker.PutOrGet(ref packetId);
		packetSizeChecker.PutOrGet(ref compressed);
		packetSizeChecker.PutOrGet(buffer);
		packetSizeChecker.CheckRawUint32(287454020u);
		return packetSizeChecker.Size;
	}
}
