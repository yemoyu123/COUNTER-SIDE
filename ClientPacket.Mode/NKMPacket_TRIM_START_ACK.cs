using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_TRIM_START_ACK)]
public sealed class NKMPacket_TRIM_START_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public TrimModeState trimModeState = new TrimModeState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref trimModeState);
	}
}
