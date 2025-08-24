using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_OPERATOR_LEVELUP_REQ)]
public sealed class NKMPacket_OPERATOR_LEVELUP_REQ : ISerializable
{
	public long targetUnitUid;

	public List<MiscItemData> materials = new List<MiscItemData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetUnitUid);
		stream.PutOrGet(ref materials);
	}
}
