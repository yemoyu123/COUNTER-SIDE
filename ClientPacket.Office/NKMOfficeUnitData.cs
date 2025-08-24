using Cs.Protocol;

namespace ClientPacket.Office;

public sealed class NKMOfficeUnitData : ISerializable
{
	public long unitUid;

	public int unitId;

	public int skinId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
		stream.PutOrGet(ref unitId);
		stream.PutOrGet(ref skinId);
	}
}
