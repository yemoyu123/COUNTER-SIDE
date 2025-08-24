using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PRESET_ADD_ACK)]
public sealed class NKMPacket_OFFICE_PRESET_ADD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int totalPresetCount;

	public List<NKMItemMiscData> costItemDatas = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref totalPresetCount);
		stream.PutOrGet(ref costItemDatas);
	}
}
