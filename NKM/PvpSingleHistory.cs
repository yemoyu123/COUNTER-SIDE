using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace NKM;

public class PvpSingleHistory : ISerializable, IComparable<PvpSingleHistory>
{
	public long gameUid;

	public long idx;

	public int MyUserLevel;

	public int TargetUserLevel;

	public string TargetNickName;

	public PVP_RESULT Result;

	public int GainScore;

	public int MyTier;

	public int MyScore;

	public int TargetTier;

	public int TargetScore;

	public long RegdateTick;

	public NKMAsyncDeckData MyDeckData;

	public NKMAsyncDeckData TargetDeckData;

	public NKM_GAME_TYPE GameType;

	public long TargetFriendCode;

	public long SourceGuildUid;

	public string SourceGuildName;

	public long SourceGuildBadgeId;

	public long TargetGuildUid;

	public string TargetGuildName;

	public long TargetGuildBadgeId;

	public List<int> myBanUnitIds = new List<int>();

	public List<int> targetBanUnitIds = new List<int>();

	public bool forfeitured;

	public int TargetTitleId;

	public List<int> myBanShipIds = new List<int>();

	public List<int> targetBanShipIds = new List<int>();

	public int CompareTo(PvpSingleHistory other)
	{
		return other.RegdateTick.CompareTo(RegdateTick);
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref gameUid);
		stream.PutOrGet(ref MyUserLevel);
		stream.PutOrGet(ref TargetUserLevel);
		stream.PutOrGet(ref TargetNickName);
		stream.PutOrGetEnum(ref Result);
		stream.PutOrGet(ref GainScore);
		stream.PutOrGet(ref MyTier);
		stream.PutOrGet(ref MyScore);
		stream.PutOrGet(ref TargetTier);
		stream.PutOrGet(ref TargetScore);
		stream.PutOrGet(ref RegdateTick);
		stream.PutOrGet(ref MyDeckData);
		stream.PutOrGet(ref TargetDeckData);
		stream.PutOrGetEnum(ref GameType);
		stream.PutOrGet(ref TargetFriendCode);
		stream.PutOrGet(ref SourceGuildUid);
		stream.PutOrGet(ref SourceGuildName);
		stream.PutOrGet(ref SourceGuildBadgeId);
		stream.PutOrGet(ref TargetGuildUid);
		stream.PutOrGet(ref TargetGuildName);
		stream.PutOrGet(ref TargetGuildBadgeId);
		stream.PutOrGet(ref myBanUnitIds);
		stream.PutOrGet(ref targetBanUnitIds);
		stream.PutOrGet(ref forfeitured);
		stream.PutOrGet(ref TargetTitleId);
		stream.PutOrGet(ref myBanShipIds);
		stream.PutOrGet(ref targetBanShipIds);
	}
}
