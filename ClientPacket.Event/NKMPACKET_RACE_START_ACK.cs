using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_RACE_START_ACK)]
public sealed class NKMPACKET_RACE_START_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isWin;

	public int selectLine;

	public NKMRacePrivate racePrivate = new NKMRacePrivate();

	public List<NKMItemMiscData> costItemList = new List<NKMItemMiscData>();

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isWin);
		stream.PutOrGet(ref selectLine);
		stream.PutOrGet(ref racePrivate);
		stream.PutOrGet(ref costItemList);
		stream.PutOrGet(ref rewardData);
	}
}
