using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMReplayLink : ISerializable
{
	public string url;

	public string checksum;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref url);
		stream.PutOrGet(ref checksum);
	}
}
