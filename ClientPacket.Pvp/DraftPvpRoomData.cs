using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Pvp;

public sealed class DraftPvpRoomData : ISerializable
{
	public sealed class DraftTeamData : ISerializable
	{
		public NKM_TEAM_TYPE teamType;

		public NKMUserProfileData userProfileData = new NKMUserProfileData();

		public List<int> globalBanUnitIdList = new List<int>();

		public List<int> globalBanShipGroupIdList = new List<int>();

		public List<NKMAsyncUnitData> pickUnitList = new List<NKMAsyncUnitData>();

		public int banishedUnitIndex;

		public NKMAsyncUnitData mainShip = new NKMAsyncUnitData();

		public NKMOperator operatorUnit;

		public int leaderIndex;

		void ISerializable.Serialize(IPacketStream stream)
		{
			stream.PutOrGetEnum(ref teamType);
			stream.PutOrGet(ref userProfileData);
			stream.PutOrGet(ref globalBanUnitIdList);
			stream.PutOrGet(ref globalBanShipGroupIdList);
			stream.PutOrGet(ref pickUnitList);
			stream.PutOrGet(ref banishedUnitIndex);
			stream.PutOrGet(ref mainShip);
			stream.PutOrGet(ref operatorUnit);
			stream.PutOrGet(ref leaderIndex);
		}
	}

	public NKM_GAME_TYPE gameType;

	public DRAFT_PVP_ROOM_STATE roomState;

	public DateTime stateEndTime;

	public NKM_TEAM_TYPE currentStateTeamType;

	public NKMAsyncUnitData selectedUnit = new NKMAsyncUnitData();

	public DraftTeamData draftTeamDataA = new DraftTeamData();

	public DraftTeamData draftTeamDataB = new DraftTeamData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref gameType);
		stream.PutOrGetEnum(ref roomState);
		stream.PutOrGet(ref stateEndTime);
		stream.PutOrGetEnum(ref currentStateTeamType);
		stream.PutOrGet(ref selectedUnit);
		stream.PutOrGet(ref draftTeamDataA);
		stream.PutOrGet(ref draftTeamDataB);
	}
}
