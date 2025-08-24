using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_BET_RESULT_ACK)]
public sealed class NKMPACKET_EVENT_BET_RESULT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isWin;

	public bool isApplyDividentRate;

	public int eventIndex;

	public EventBetTeam selectTeam;

	public NKMRewardData rewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isWin);
		stream.PutOrGet(ref isApplyDividentRate);
		stream.PutOrGet(ref eventIndex);
		stream.PutOrGetEnum(ref selectTeam);
		stream.PutOrGet(ref rewardData);
	}
}
