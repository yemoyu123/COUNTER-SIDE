using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_RESET_STAGE_PLAY_COUNT_ACK)]
public sealed class NKMPacket_RESET_STAGE_PLAY_COUNT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMStagePlayData stagePlayData = new NKMStagePlayData();

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref stagePlayData);
		stream.PutOrGet(ref costItemData);
	}
}
