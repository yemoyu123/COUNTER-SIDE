using Cs.Protocol;

namespace ClientPacket.Game;

public sealed class NKMRaidBossResultData : ISerializable
{
	public float initHp;

	public float curHP;

	public float maxHp;

	public float damage;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref initHp);
		stream.PutOrGet(ref curHP);
		stream.PutOrGet(ref maxHp);
		stream.PutOrGet(ref damage);
	}
}
