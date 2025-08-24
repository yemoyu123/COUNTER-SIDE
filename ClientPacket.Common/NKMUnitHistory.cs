using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMUnitHistory : ISerializable
{
	public int unitId;

	public int maxLevel;

	public int maxLoyalty;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitId);
		stream.PutOrGet(ref maxLevel);
		stream.PutOrGet(ref maxLoyalty);
	}
}
