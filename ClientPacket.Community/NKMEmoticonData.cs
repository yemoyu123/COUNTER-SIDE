using Cs.Protocol;

namespace ClientPacket.Community;

public sealed class NKMEmoticonData : ISerializable
{
	public int emoticonId;

	public bool isFavorites;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref emoticonId);
		stream.PutOrGet(ref isFavorites);
	}
}
