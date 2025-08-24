using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_TACTIC_UPDATE_ACK)]
public sealed class NKMPacket_UNIT_TACTIC_UPDATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData unitData;

	public List<long> consumeUnitUids = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitData);
		stream.PutOrGet(ref consumeUnitUids);
	}
}
