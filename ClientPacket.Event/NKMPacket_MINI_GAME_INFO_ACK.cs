using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_MINI_GAME_INFO_ACK)]
public sealed class NKMPacket_MINI_GAME_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMMiniGameData miniGameData;

	public List<int> rewardIds = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref miniGameData);
		stream.PutOrGet(ref rewardIds);
	}
}
