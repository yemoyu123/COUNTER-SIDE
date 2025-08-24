using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_DATA_ACK)]
public sealed class NKMPacket_FIERCE_DATA_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int rankNumber;

	public int rankPercent;

	public HashSet<int> pointRewardHistory = new HashSet<int>();

	public bool isRankRewardGotten;

	public List<NKMFierceBoss> bossList = new List<NKMFierceBoss>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rankNumber);
		stream.PutOrGet(ref rankPercent);
		stream.PutOrGet(ref pointRewardHistory);
		stream.PutOrGet(ref isRankRewardGotten);
		stream.PutOrGet(ref bossList);
	}
}
