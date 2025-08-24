using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_UI_SCEN_CHANGED_REQ)]
public sealed class NKMPacket_UI_SCEN_CHANGED_REQ : ISerializable
{
	public NKM_SCEN_ID scenID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref scenID);
	}
}
