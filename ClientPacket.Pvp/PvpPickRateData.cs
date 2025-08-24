using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Pvp;

public sealed class PvpPickRateData : ISerializable
{
	public PvpPickType type;

	public List<int> playUnits = new List<int>();

	public List<int> winUnits = new List<int>();

	public List<int> banUnits = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref type);
		stream.PutOrGet(ref playUnits);
		stream.PutOrGet(ref winUnits);
		stream.PutOrGet(ref banUnits);
	}
}
