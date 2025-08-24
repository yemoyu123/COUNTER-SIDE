using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_SHADOW_PALACE_START_ACK)]
public sealed class NKMPacket_SHADOW_PALACE_START_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int currentPalaceId;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public int rewardMultiply = 1;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref currentPalaceId);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref rewardMultiply);
	}
}
