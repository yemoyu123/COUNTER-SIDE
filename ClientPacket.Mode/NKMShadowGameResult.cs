using Cs.Protocol;
using NKM.Templet;

namespace ClientPacket.Mode;

public sealed class NKMShadowGameResult : ISerializable
{
	public int palaceId;

	public NKMPalaceDungeonData dungeonData = new NKMPalaceDungeonData();

	public NKMRewardData rewardData;

	public bool newRecord;

	public int currentDungeonId;

	public int life;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref palaceId);
		stream.PutOrGet(ref dungeonData);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref newRecord);
		stream.PutOrGet(ref currentDungeonId);
		stream.PutOrGet(ref life);
	}
}
