using System;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CLOSE_ACK)]
public sealed class NKMPacket_GUILD_CLOSE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public DateTime closingTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref closingTime);
	}
}
