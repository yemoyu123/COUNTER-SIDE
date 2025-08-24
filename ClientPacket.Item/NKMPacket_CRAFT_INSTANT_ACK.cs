using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CRAFT_INSTANT_ACK)]
public sealed class NKMPacket_CRAFT_INSTANT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int moldId;

	public int moldCount;

	public List<NKMItemMiscData> materialItemDataList = new List<NKMItemMiscData>();

	public NKMResetCount resetCount = new NKMResetCount();

	public NKMRewardData createdRewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref moldId);
		stream.PutOrGet(ref moldCount);
		stream.PutOrGet(ref materialItemDataList);
		stream.PutOrGet(ref resetCount);
		stream.PutOrGet(ref createdRewardData);
	}
}
