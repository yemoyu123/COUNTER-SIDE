using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_USE_SERVICE_ACK)]
public sealed class NKMPacket_WARFARE_GAME_USE_SERVICE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int warfareGameUnitUID;

	public NKM_WARFARE_SERVICE_TYPE warfareServiceType;

	public NKMItemMiscData costItemData;

	public float hp;

	public byte supply;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref warfareGameUnitUID);
		stream.PutOrGetEnum(ref warfareServiceType);
		stream.PutOrGet(ref costItemData);
		stream.PutOrGet(ref hp);
		stream.PutOrGet(ref supply);
	}
}
