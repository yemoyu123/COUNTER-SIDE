using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_SET_LEADER_ACK)]
public sealed class NKMPacket_WORLDMAP_SET_LEADER_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int cityID;

	public long leaderUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref leaderUID);
	}
}
