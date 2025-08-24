using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_GAME_AUTO_ACK)]
public sealed class NKMPacket_WARFARE_GAME_AUTO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isAuto;

	public bool isAutoRepair;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isAuto);
		stream.PutOrGet(ref isAutoRepair);
	}
}
