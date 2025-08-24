using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_PICK_RATE_ACK)]
public sealed class NKMPacket_PVP_PICK_RATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKM_GAME_TYPE gameType;

	public List<PvpPickRateData> pickRates = new List<PvpPickRateData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref gameType);
		stream.PutOrGet(ref pickRates);
	}
}
