using ClientPacket.Common;
using Cs.Protocol;

namespace NKM;

public class NKMUserOption : ISerializable
{
	public bool m_bAutoRespawn;

	public ActionCameraType m_ActionCameraType = ActionCameraType.All;

	public bool m_bTrackCamera = true;

	public bool m_bViewSkillCutIn = true;

	public bool m_bAutoWarfare;

	public bool m_bAutoWarfareRepair = true;

	public bool m_bPlayCutscene;

	public bool m_bAutoDive;

	public NKM_GAME_SPEED_TYPE m_eSpeedType;

	public NKM_GAME_AUTO_SKILL_TYPE m_eAutoSkillType = NKM_GAME_AUTO_SKILL_TYPE.NGST_OFF_HYPER;

	public bool m_bAutoSyncFriendDeck = true;

	public NKM_GAME_AUTO_RESPAWN_TYPE m_bDefaultPvpAutoRespawn;

	public PrivatePvpInvitation privatePvpInvitation;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_bAutoRespawn);
		stream.PutOrGetEnum(ref m_ActionCameraType);
		stream.PutOrGet(ref m_bTrackCamera);
		stream.PutOrGet(ref m_bViewSkillCutIn);
		stream.PutOrGet(ref m_bAutoWarfare);
		stream.PutOrGet(ref m_bAutoWarfareRepair);
		stream.PutOrGet(ref m_bPlayCutscene);
		stream.PutOrGet(ref m_bAutoDive);
		stream.PutOrGetEnum(ref m_eSpeedType);
		stream.PutOrGetEnum(ref m_eAutoSkillType);
		stream.PutOrGet(ref m_bAutoSyncFriendDeck);
		stream.PutOrGetEnum(ref m_bDefaultPvpAutoRespawn);
		stream.PutOrGetEnum(ref privatePvpInvitation);
	}
}
