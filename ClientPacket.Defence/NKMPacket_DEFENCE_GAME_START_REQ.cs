using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_GAME_START_REQ)]
public sealed class NKMPacket_DEFENCE_GAME_START_REQ : ISerializable
{
	public int defenceTempletId;

	public NKMEventDeckData eventDeckData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref defenceTempletId);
		stream.PutOrGet(ref eventDeckData);
	}
}
