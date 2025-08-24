using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_RECOVER_ACK)]
public sealed class NKMPacket_WARFARE_RECOVER_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public WarfareSyncData warfareSyncData = new WarfareSyncData();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref warfareSyncData);
		stream.PutOrGet(ref costItemDataList);
	}
}
