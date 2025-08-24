using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_ASYNC_PVP_TARGET_LIST_ACK)]
public sealed class NKMPacket_ASYNC_PVP_TARGET_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<AsyncPvpTarget> targetList = new List<AsyncPvpTarget>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref targetList);
	}
}
