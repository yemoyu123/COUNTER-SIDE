using System.Collections.Generic;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Contract;

public sealed class MiscContractResult : ISerializable
{
	public int miscItemId;

	public List<NKMUnitData> units = new List<NKMUnitData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref miscItemId);
		stream.PutOrGet(ref units);
	}
}
