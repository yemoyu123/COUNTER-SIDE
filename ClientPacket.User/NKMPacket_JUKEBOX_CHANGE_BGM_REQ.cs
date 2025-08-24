using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_JUKEBOX_CHANGE_BGM_REQ)]
public sealed class NKMPacket_JUKEBOX_CHANGE_BGM_REQ : ISerializable
{
	public NKM_BGM_TYPE bgmType;

	public int bgmId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref bgmType);
		stream.PutOrGet(ref bgmId);
	}
}
