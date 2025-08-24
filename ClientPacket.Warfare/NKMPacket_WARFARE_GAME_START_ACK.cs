using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_START_ACK)]
public sealed class NKMPacket_WARFARE_GAME_START_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public WarfareGameData warfareGameData = new WarfareGameData();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref warfareGameData);
		stream.PutOrGet(ref costItemDataList);
	}
}
