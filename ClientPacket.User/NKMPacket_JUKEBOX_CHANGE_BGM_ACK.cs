using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_JUKEBOX_CHANGE_BGM_ACK)]
public sealed class NKMPacket_JUKEBOX_CHANGE_BGM_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMJukeboxData jukeboxData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref jukeboxData);
	}
}
