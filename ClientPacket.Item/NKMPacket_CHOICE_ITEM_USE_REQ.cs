using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CHOICE_ITEM_USE_REQ)]
public sealed class NKMPacket_CHOICE_ITEM_USE_REQ : ISerializable
{
	public int itemId;

	public int rewardId;

	public int count;

	public int setOptionId;

	public int subSkillId;

	public List<NKM_STAT_TYPE> statTypes = new List<NKM_STAT_TYPE>();

	public int potentialOptionId;

	public int potentialOption2Id;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref itemId);
		stream.PutOrGet(ref rewardId);
		stream.PutOrGet(ref count);
		stream.PutOrGet(ref setOptionId);
		stream.PutOrGet(ref subSkillId);
		stream.PutOrGetEnum(ref statTypes);
		stream.PutOrGet(ref potentialOptionId);
		stream.PutOrGet(ref potentialOption2Id);
	}
}
