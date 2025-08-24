using System.Collections.Generic;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMFierceProfileData : ISerializable
{
	public int fierceBossGroupId;

	public int fierceBossId;

	public NKMDummyDeckData profileDeck;

	public int operationPower;

	public int totalPoint;

	public int penaltyPoint;

	public List<int> penaltyIds = new List<int>();

	public List<NKMEmblemData> emblems = new List<NKMEmblemData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref fierceBossGroupId);
		stream.PutOrGet(ref fierceBossId);
		stream.PutOrGet(ref profileDeck);
		stream.PutOrGet(ref operationPower);
		stream.PutOrGet(ref totalPoint);
		stream.PutOrGet(ref penaltyPoint);
		stream.PutOrGet(ref penaltyIds);
		stream.PutOrGet(ref emblems);
	}
}
