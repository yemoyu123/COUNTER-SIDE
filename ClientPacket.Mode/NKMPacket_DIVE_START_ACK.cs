using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_START_ACK)]
public sealed class NKMPacket_DIVE_START_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int cityID;

	public NKMDiveGameData diveGameData;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref diveGameData);
		stream.PutOrGet(ref costItemDataList);
	}
}
