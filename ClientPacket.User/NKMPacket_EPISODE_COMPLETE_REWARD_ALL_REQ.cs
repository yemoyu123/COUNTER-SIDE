using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_EPISODE_COMPLETE_REWARD_ALL_REQ)]
public sealed class NKMPacket_EPISODE_COMPLETE_REWARD_ALL_REQ : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int episodeID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref episodeID);
	}
}
