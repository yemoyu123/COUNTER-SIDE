using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_LEAGUE_PVP_RANK_LIST_ACK)]
public sealed class NKMPacket_LEAGUE_PVP_RANK_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public RANK_TYPE rankType;

	public int myRank;

	public List<NKMUserSimpleProfileData> list = new List<NKMUserSimpleProfileData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref rankType);
		stream.PutOrGet(ref myRank);
		stream.PutOrGet(ref list);
	}
}
