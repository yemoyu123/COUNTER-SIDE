using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PARTY_ACK)]
public sealed class NKMPacket_OFFICE_PARTY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int roomId;

	public List<NKMUnitData> units = new List<NKMUnitData>();

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref roomId);
		stream.PutOrGet(ref units);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref rewardData);
	}
}
