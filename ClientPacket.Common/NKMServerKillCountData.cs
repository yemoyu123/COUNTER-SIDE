using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMServerKillCountData : ISerializable
{
	public int killCountId;

	public long killCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref killCountId);
		stream.PutOrGet(ref killCount);
	}
}
