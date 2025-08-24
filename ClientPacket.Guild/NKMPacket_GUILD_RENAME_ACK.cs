using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_RENAME_ACK)]
public sealed class NKMPacket_GUILD_RENAME_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string prevName;

	public string newName;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref prevName);
		stream.PutOrGet(ref newName);
	}
}
