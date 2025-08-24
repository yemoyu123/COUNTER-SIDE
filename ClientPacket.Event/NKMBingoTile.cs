using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMBingoTile : ISerializable
{
	public int eventId;

	public int tileIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventId);
		stream.PutOrGet(ref tileIndex);
	}
}
