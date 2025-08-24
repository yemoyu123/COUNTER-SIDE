using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class EquipProfileInfo : ISerializable
{
	public long itemUid;

	public int ItemId;

	public int enchantLevel;

	public NKM_STAT_TYPE statType;

	public float statValue;

	public NKM_STAT_TYPE statType2;

	public float statValue2;

	public int setOptionId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref itemUid);
		stream.PutOrGet(ref ItemId);
		stream.PutOrGet(ref enchantLevel);
		stream.PutOrGetEnum(ref statType);
		stream.PutOrGet(ref statValue);
		stream.PutOrGetEnum(ref statType2);
		stream.PutOrGet(ref statValue2);
		stream.PutOrGet(ref setOptionId);
	}
}
