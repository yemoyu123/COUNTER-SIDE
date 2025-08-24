using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_STRATEGY_PVP_REFRESH_NOT)]
public sealed class NKMPacket_STRATEGY_PVP_REFRESH_NOT : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public PvpState data;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref data);
	}
}
