using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_LOAD_ACK)]
public sealed class NKMPacket_GAME_LOAD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMGameData gameData;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref gameData);
		stream.PutOrGet(ref costItemDataList);
	}
}
