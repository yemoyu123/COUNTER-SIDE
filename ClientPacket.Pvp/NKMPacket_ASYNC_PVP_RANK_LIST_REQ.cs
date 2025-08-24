using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_ASYNC_PVP_RANK_LIST_REQ)]
public sealed class NKMPacket_ASYNC_PVP_RANK_LIST_REQ : ISerializable
{
	public RANK_TYPE rankType;

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref rankType);
		stream.PutOrGet(ref isAll);
	}
}
