using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_PENALTY_ACK)]
public sealed class NKMPacket_FIERCE_PENALTY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int fierceBossId;

	public List<int> penaltyIds = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref fierceBossId);
		stream.PutOrGet(ref penaltyIds);
	}
}
