using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class PvpCastingVoteData : ISerializable
{
	public List<int> unitIdList = new List<int>();

	public List<int> shipGroupIdList = new List<int>();

	public List<int> operatorIdList = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitIdList);
		stream.PutOrGet(ref shipGroupIdList);
		stream.PutOrGet(ref operatorIdList);
	}
}
