using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMResetCount : ISerializable
{
	public int groupId;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref groupId);
		stream.PutOrGet(ref count);
	}
}
