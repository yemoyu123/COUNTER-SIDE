using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_PHASE_START_ACK)]
public sealed class NKMPacket_PHASE_START_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public PhaseModeState state = new PhaseModeState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref state);
	}
}
