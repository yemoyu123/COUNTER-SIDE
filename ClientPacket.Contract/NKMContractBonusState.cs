using Cs.Protocol;

namespace ClientPacket.Contract;

public sealed class NKMContractBonusState : ISerializable
{
	public int bonusGroupId;

	public int useCount;

	public int resetCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref bonusGroupId);
		stream.PutOrGet(ref useCount);
		stream.PutOrGet(ref resetCount);
	}
}
