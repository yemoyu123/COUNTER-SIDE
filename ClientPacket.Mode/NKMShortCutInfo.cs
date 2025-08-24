using Cs.Protocol;

namespace ClientPacket.Mode;

public sealed class NKMShortCutInfo : ISerializable
{
	public int gameType;

	public int stageId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref gameType);
		stream.PutOrGet(ref stageId);
	}
}
