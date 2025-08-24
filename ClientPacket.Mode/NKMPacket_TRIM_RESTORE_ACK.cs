using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_TRIM_RESTORE_ACK)]
public sealed class NKMPacket_TRIM_RESTORE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref costItemData);
	}
}
