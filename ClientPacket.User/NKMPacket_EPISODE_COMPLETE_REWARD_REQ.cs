using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_EPISODE_COMPLETE_REWARD_REQ)]
public sealed class NKMPacket_EPISODE_COMPLETE_REWARD_REQ : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int episodeID;

	public int episodeDifficulty;

	public sbyte rewardIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref episodeID);
		stream.PutOrGet(ref episodeDifficulty);
		stream.PutOrGet(ref rewardIndex);
	}
}
