using System;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_USER_PROFILE_UPDATED_NOT)]
public sealed class NKMPacket_GUILD_USER_PROFILE_UPDATED_NOT : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public DateTime lastOnlineTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref lastOnlineTime);
	}
}
