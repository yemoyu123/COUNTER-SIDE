using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMEmblemData : ISerializable
{
	public int id;

	public long count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref id);
		stream.PutOrGet(ref count);
	}
}
