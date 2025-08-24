using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_KAKAO_MISSION_REFRESH_STATE_ACK)]
public sealed class NKMPacket_KAKAO_MISSION_REFRESH_STATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public KakaoMissionData missionData = new KakaoMissionData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref missionData);
	}
}
