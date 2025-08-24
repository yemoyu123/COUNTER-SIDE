using System;
using System.Collections.Generic;
using ClientPacket.Game;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Pvp;

public sealed class ReplayData : ISerializable
{
	public sealed class EmoticonData : ISerializable
	{
		public float time;

		public NKMPacket_GAME_EMOTICON_NOT not = new NKMPacket_GAME_EMOTICON_NOT();

		void ISerializable.Serialize(IPacketStream stream)
		{
			stream.PutOrGet(ref time);
			stream.PutOrGet(ref not);
		}
	}

	public string replayName;

	public string replayVersion;

	public sbyte streamID;

	public int protocolVersion;

	public DateTime dateTime;

	public NKMGameData gameData;

	public NKMGameRuntimeData gameRuntimeData;

	public List<NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT> syncList = new List<NKMPacket_NPT_GAME_SYNC_DATA_PACK_NOT>();

	public PVP_RESULT pvpResult;

	public float gameEndTime;

	public NKMGameRecord gameRecord;

	public List<EmoticonData> emoticonList = new List<EmoticonData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref replayName);
		stream.PutOrGet(ref replayVersion);
		stream.PutOrGet(ref streamID);
		stream.PutOrGet(ref protocolVersion);
		stream.PutOrGet(ref dateTime);
		stream.PutOrGet(ref gameData);
		stream.PutOrGet(ref gameRuntimeData);
		stream.PutOrGet(ref syncList);
		stream.PutOrGetEnum(ref pvpResult);
		stream.PutOrGet(ref gameEndTime);
		stream.PutOrGet(ref gameRecord);
		stream.PutOrGet(ref emoticonList);
	}
}
