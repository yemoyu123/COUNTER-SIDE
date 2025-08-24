using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_USE_UNIT_SKILL_ACK)]
public sealed class NKMPacket_GAME_USE_UNIT_SKILL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public short gameUnitUID;

	public sbyte skillStateID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref gameUnitUID);
		stream.PutOrGet(ref skillStateID);
	}
}
