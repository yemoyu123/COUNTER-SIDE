using System;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)]
public sealed class NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMItemMiscData itemData;

	public DateTime chrageTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref itemData);
		stream.PutOrGet(ref chrageTime);
	}
}
