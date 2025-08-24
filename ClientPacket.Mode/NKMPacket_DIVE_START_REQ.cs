using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_START_REQ)]
public sealed class NKMPacket_DIVE_START_REQ : ISerializable
{
	public int cityID;

	public int stageID;

	public List<int> deckIndexeList = new List<int>();

	public bool isDiveStorm;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref stageID);
		stream.PutOrGet(ref deckIndexeList);
		stream.PutOrGet(ref isDiveStorm);
	}
}
