using Cs.Protocol;

namespace ClientPacket.Pvp;

public sealed class NKMPrivateGameConfig : ISerializable
{
	public bool applyEquipStat;

	public bool applyAllUnitMaxLevel;

	public bool applyBanUpSystem;

	public bool draftBanMode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref applyEquipStat);
		stream.PutOrGet(ref applyAllUnitMaxLevel);
		stream.PutOrGet(ref applyBanUpSystem);
		stream.PutOrGet(ref draftBanMode);
	}
}
