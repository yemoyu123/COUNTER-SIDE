using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK)]
public sealed class NKMPacket_GAME_OPTION_PLAY_CUTSCENE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isPlayCutscene;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isPlayCutscene);
	}
}
