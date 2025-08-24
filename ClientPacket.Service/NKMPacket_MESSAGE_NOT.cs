using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_MESSAGE_NOT)]
public sealed class NKMPacket_MESSAGE_NOT : ISerializable
{
	public List<string> message = new List<string>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref message);
	}
}
