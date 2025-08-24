using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_CONTRACT_PERMANENTLY_ACK)]
public sealed class NKMPacket_CONTRACT_PERMANENTLY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long unitUID;

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref costItemData);
	}
}
