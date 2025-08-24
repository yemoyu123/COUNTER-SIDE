using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMDefenceProfileData : ISerializable
{
	public int defenceId;

	public int bestPoint;

	public NKMAsyncDeckData profileDeck = new NKMAsyncDeckData();

	public List<NKMEmblemData> emblems = new List<NKMEmblemData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref defenceId);
		stream.PutOrGet(ref bestPoint);
		stream.PutOrGet(ref profileDeck);
		stream.PutOrGet(ref emblems);
	}
}
