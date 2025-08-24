using Cs.Protocol;

namespace NKM;

public class EQUIP_ITEM_STAT : ISerializable
{
	public NKM_STAT_TYPE type = NKM_STAT_TYPE.NST_RANDOM;

	public float stat_value;

	public float stat_level_value;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref type);
		stream.PutOrGet(ref stat_value);
		stream.PutOrGet(ref stat_level_value);
	}
}
