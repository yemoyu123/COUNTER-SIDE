using Cs.Protocol;

namespace ClientPacket.Account;

public sealed class ZlongUserData : ISerializable
{
	public string userId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userId);
	}
}
