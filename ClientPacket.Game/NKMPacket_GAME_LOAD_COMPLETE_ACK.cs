using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_LOAD_COMPLETE_ACK)]
public sealed class NKMPacket_GAME_LOAD_COMPLETE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isIntrude;

	public NKMGameRuntimeData gameRuntimeData;

	public int rewardMultiply;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isIntrude);
		stream.PutOrGet(ref gameRuntimeData);
		stream.PutOrGet(ref rewardMultiply);
	}
}
