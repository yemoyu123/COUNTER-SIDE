using System;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_BLOCK_MUTE_NOT)]
public sealed class NKMPacket_BLOCK_MUTE_NOT : ISerializable
{
	public long userUid;

	public DateTime endDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref endDate);
	}
}
