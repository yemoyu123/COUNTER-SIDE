using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_BAN_LIST_UPDATED_NOT)]
public sealed class NKMPacket_PVP_BAN_LIST_UPDATED_NOT : ISerializable
{
	public NKMPvpBanResult pvpBanResult = new NKMPvpBanResult();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref pvpBanResult);
	}
}
