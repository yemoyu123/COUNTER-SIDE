using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_GAME_OPTION_CHANGE_REQ)]
public sealed class NKMPacket_GAME_OPTION_CHANGE_REQ : ISerializable
{
	public ActionCameraType actionCameraType;

	public bool isTrackCamera;

	public bool isViewSkillCutIn;

	public bool autoSyncFriendDeck;

	public NKM_GAME_AUTO_RESPAWN_TYPE defaultPvpAutoRespawn;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref actionCameraType);
		stream.PutOrGet(ref isTrackCamera);
		stream.PutOrGet(ref isViewSkillCutIn);
		stream.PutOrGet(ref autoSyncFriendDeck);
		stream.PutOrGetEnum(ref defaultPvpAutoRespawn);
	}
}
