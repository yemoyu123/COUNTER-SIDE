using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_OTHER_LOADING_PROGRESS_NOT)]
public sealed class NKMPacket_OTHER_LOADING_PROGRESS_NOT : ISerializable
{
	public byte progress;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref progress);
	}
}
