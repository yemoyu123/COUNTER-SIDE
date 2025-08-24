using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMShipModuleCandidate : ISerializable
{
	public long shipUid;

	public int moduleId;

	public NKMShipCmdModule slotCandidate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shipUid);
		stream.PutOrGet(ref moduleId);
		stream.PutOrGet(ref slotCandidate);
	}
}
