using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_INFORM_MY_LOADING_PROGRESS_REQ)]
public sealed class NKMPacket_INFORM_MY_LOADING_PROGRESS_REQ : ISerializable
{
	public byte progress;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref progress);
	}
}
