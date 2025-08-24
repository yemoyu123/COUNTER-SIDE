using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMSkillLevelData : ISerializable
{
	public int normalLv;

	public int passiveLv;

	public int specialLv;

	public int ultimateLv;

	public int leaderLv;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref normalLv);
		stream.PutOrGet(ref passiveLv);
		stream.PutOrGet(ref specialLv);
		stream.PutOrGet(ref ultimateLv);
		stream.PutOrGet(ref leaderLv);
	}
}
