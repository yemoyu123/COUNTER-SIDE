using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ)]
public sealed class NKMPacket_GAME_OPTION_PLAY_CUTSCENE_REQ : ISerializable
{
	public bool isPlayCutscene;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isPlayCutscene);
	}
}
