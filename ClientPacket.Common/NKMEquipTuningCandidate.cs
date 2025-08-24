using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMEquipTuningCandidate : ISerializable
{
	public long equipUid;

	public NKM_STAT_TYPE option1;

	public NKM_STAT_TYPE option2;

	public int setOptionId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipUid);
		stream.PutOrGetEnum(ref option1);
		stream.PutOrGetEnum(ref option2);
		stream.PutOrGet(ref setOptionId);
	}
}
