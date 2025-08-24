using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_CUTSCENE_DUNGEON_START_ACK)]
public sealed class NKMPacket_CUTSCENE_DUNGEON_START_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMStagePlayData stagePlayData = new NKMStagePlayData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref stagePlayData);
	}
}
