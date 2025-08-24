using Cs.Protocol;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_AUTO_REQ)]
public sealed class NKMPacket_WARFARE_GAME_AUTO_REQ : ISerializable
{
	public bool isAuto;

	public bool isAutoRepair;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isAuto);
		stream.PutOrGet(ref isAutoRepair);
	}
}
