using System.Collections.Generic;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMAsyncUnitData : ISerializable
{
	public long unitUid;

	public int unitId;

	public int unitLevel;

	public int skinId;

	public int limitBreakLevel;

	public List<int> skillLevel = new List<int>();

	public List<int> statExp = new List<int>();

	public List<long> equipUids = new List<long>();

	public List<NKMShipCmdModule> shipModules = new List<NKMShipCmdModule>();

	public int tacticLevel;

	public int reactorLevel;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
		stream.PutOrGet(ref unitId);
		stream.PutOrGet(ref unitLevel);
		stream.PutOrGet(ref skinId);
		stream.PutOrGet(ref limitBreakLevel);
		stream.PutOrGet(ref skillLevel);
		stream.PutOrGet(ref statExp);
		stream.PutOrGet(ref equipUids);
		stream.PutOrGet(ref shipModules);
		stream.PutOrGet(ref tacticLevel);
		stream.PutOrGet(ref reactorLevel);
	}
}
