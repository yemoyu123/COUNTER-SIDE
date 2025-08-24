using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Negotiation;

[PacketId(ClientPacketId.kNKMPacket_NEGOTIATE_REQ)]
public sealed class NKMPacket_NEGOTIATE_REQ : ISerializable
{
	public long unitUid;

	public List<MiscItemData> materials = new List<MiscItemData>();

	public NEGOTIATE_BOSS_SELECTION negotiateBossSelection;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
		stream.PutOrGet(ref materials);
		stream.PutOrGetEnum(ref negotiateBossSelection);
	}
}
