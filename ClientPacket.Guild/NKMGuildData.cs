using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class NKMGuildData : ISerializable
{
	public long guildUid;

	public string name;

	public long badgeId;

	public int guildLevel;

	public long guildLevelExp;

	public GuildJoinType guildJoinType;

	public GuildState guildState;

	public DateTime closingTime;

	public string greeting;

	public string notice;

	public List<FriendListData> inviteList = new List<FriendListData>();

	public List<FriendListData> joinWaitingList = new List<FriendListData>();

	public List<NKMGuildMemberData> members = new List<NKMGuildMemberData>();

	public List<NKMGuildAttendanceData> attendanceList = new List<NKMGuildAttendanceData>();

	public long unionPoint;

	public string dungeonNotice;

	public GuildChatNoticeType chatNoticeType;

	public int renameCount;

	public DateTime latestRenameDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref name);
		stream.PutOrGet(ref badgeId);
		stream.PutOrGet(ref guildLevel);
		stream.PutOrGet(ref guildLevelExp);
		stream.PutOrGetEnum(ref guildJoinType);
		stream.PutOrGetEnum(ref guildState);
		stream.PutOrGet(ref closingTime);
		stream.PutOrGet(ref greeting);
		stream.PutOrGet(ref notice);
		stream.PutOrGet(ref inviteList);
		stream.PutOrGet(ref joinWaitingList);
		stream.PutOrGet(ref members);
		stream.PutOrGet(ref attendanceList);
		stream.PutOrGet(ref unionPoint);
		stream.PutOrGet(ref dungeonNotice);
		stream.PutOrGetEnum(ref chatNoticeType);
		stream.PutOrGet(ref renameCount);
		stream.PutOrGet(ref latestRenameDate);
	}
}
