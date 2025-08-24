using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_GAME_OPTION_CHANGE_ACK)]
public sealed class NKMPacket_GAME_OPTION_CHANGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public ActionCameraType actionCameraType;

	public bool isTrackCamera;

	public bool isViewSkillCutIn;

	public bool autoSyncFriendDeck;

	public NKM_GAME_AUTO_RESPAWN_TYPE defaultPvpAutoRespawn;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref actionCameraType);
		stream.PutOrGet(ref isTrackCamera);
		stream.PutOrGet(ref isViewSkillCutIn);
		stream.PutOrGet(ref autoSyncFriendDeck);
		stream.PutOrGetEnum(ref defaultPvpAutoRespawn);
	}
}
