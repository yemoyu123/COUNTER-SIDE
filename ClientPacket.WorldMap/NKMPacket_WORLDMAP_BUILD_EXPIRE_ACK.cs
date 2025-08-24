using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_BUILD_EXPIRE_ACK)]
public sealed class NKMPacket_WORLDMAP_BUILD_EXPIRE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int cityID;

	public int buildID;

	public NKMItemMiscData itemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref buildID);
		stream.PutOrGet(ref itemData);
	}
}
