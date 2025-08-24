using System;
using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_ASYNC_PVP_GAME_END_NOT)]
public sealed class NKMPacket_ASYNC_PVP_GAME_END_NOT : ISerializable
{
	public PVP_RESULT result;

	public PvpState pvpState;

	public NKMItemMiscData gainPointItem;

	public NKMGameRecord gameRecord;

	public List<NKMItemMiscData> costItem = new List<NKMItemMiscData>();

	public PvpSingleHistory history;

	public List<AsyncPvpTarget> targetList = new List<AsyncPvpTarget>();

	public DateTime pointChargeTime;

	public bool rankPvpOpen;

	public bool leaguePvpOpen;

	public int npcMaxOpenedTier;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref result);
		stream.PutOrGet(ref pvpState);
		stream.PutOrGet(ref gainPointItem);
		stream.PutOrGet(ref gameRecord);
		stream.PutOrGet(ref costItem);
		stream.PutOrGet(ref history);
		stream.PutOrGet(ref targetList);
		stream.PutOrGet(ref pointChargeTime);
		stream.PutOrGet(ref rankPvpOpen);
		stream.PutOrGet(ref leaguePvpOpen);
		stream.PutOrGet(ref npcMaxOpenedTier);
	}
}
