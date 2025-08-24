using System;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_JOIN_DISABLETIME_UPDATED_NOT)]
public sealed class NKMPacket_GUILD_JOIN_DISABLETIME_UPDATED_NOT : ISerializable
{
	public DateTime joinDisableTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref joinDisableTime);
	}
}
