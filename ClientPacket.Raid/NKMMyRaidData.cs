using Cs.Protocol;

namespace ClientPacket.Raid;

public sealed class NKMMyRaidData : ISerializable
{
	public long raidUID;

	public int stageID;

	public int cityID;

	public float curHP;

	public float maxHP;

	public bool isCoop;

	public bool isNew;

	public long expireDate;

	public int seasonID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref raidUID);
		stream.PutOrGet(ref stageID);
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref curHP);
		stream.PutOrGet(ref maxHP);
		stream.PutOrGet(ref isCoop);
		stream.PutOrGet(ref isNew);
		stream.PutOrGet(ref expireDate);
		stream.PutOrGet(ref seasonID);
	}
}
