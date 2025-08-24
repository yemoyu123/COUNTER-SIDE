using Cs.Protocol;

namespace ClientPacket.Contract;

public sealed class NKMCustomPickupContract : ISerializable
{
	public int customPickupId;

	public int totalUseCount;

	public int customPickupTargetUnitId;

	public int currentSelectCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref customPickupId);
		stream.PutOrGet(ref totalUseCount);
		stream.PutOrGet(ref customPickupTargetUnitId);
		stream.PutOrGet(ref currentSelectCount);
	}
}
