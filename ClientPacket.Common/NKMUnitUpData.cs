using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMUnitUpData : ISerializable
{
	public int unitId;

	public byte upLevel;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitId);
		stream.PutOrGet(ref upLevel);
	}
}
