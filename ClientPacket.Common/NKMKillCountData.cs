using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMKillCountData : ISerializable
{
	public int killCountId;

	public long killCount;

	public int userCompleteStep;

	public int serverCompleteStep;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref killCountId);
		stream.PutOrGet(ref killCount);
		stream.PutOrGet(ref userCompleteStep);
		stream.PutOrGet(ref serverCompleteStep);
	}
}
