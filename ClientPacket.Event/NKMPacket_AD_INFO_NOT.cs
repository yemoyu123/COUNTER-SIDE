using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_AD_INFO_NOT)]
public sealed class NKMPacket_AD_INFO_NOT : ISerializable
{
	public List<NKMADItemRewardInfo> itemRewardInfos = new List<NKMADItemRewardInfo>();

	public List<NKM_INVENTORY_EXPAND_TYPE> inventoryExpandRewardInfos = new List<NKM_INVENTORY_EXPAND_TYPE>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref itemRewardInfos);
		stream.PutOrGetEnum(ref inventoryExpandRewardInfos);
	}
}
