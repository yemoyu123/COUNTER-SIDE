using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Contract;

public sealed class NKMSelectableContractState : ISerializable
{
	public int contractId;

	public List<int> unitIdList = new List<int>();

	public int unitPoolChangeCount;

	public bool isActive;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref contractId);
		stream.PutOrGet(ref unitIdList);
		stream.PutOrGet(ref unitPoolChangeCount);
		stream.PutOrGet(ref isActive);
	}
}
