using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PRESET_ADD_REQ)]
public sealed class NKMPacket_OFFICE_PRESET_ADD_REQ : ISerializable
{
	public int addPresetCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref addPresetCount);
	}
}
