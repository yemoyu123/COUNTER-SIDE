using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_ASYNC_PVP_START_GAME_ACK)]
public sealed class NKMPacket_ASYNC_PVP_START_GAME_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMGameData gameData;

	public AsyncPvpTarget refreshedTargetData = new AsyncPvpTarget();

	public List<AsyncPvpTarget> targetList = new List<AsyncPvpTarget>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref gameData);
		stream.PutOrGet(ref refreshedTargetData);
		stream.PutOrGet(ref targetList);
	}
}
