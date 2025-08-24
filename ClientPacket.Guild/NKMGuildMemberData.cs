using System;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class NKMGuildMemberData : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public DateTime createdAt;

	public GuildMemberGrade grade;

	public DateTime lastOnlineTime;

	public string greeting;

	public DateTime lastAttendanceDate;

	public long weeklyContributionPoint;

	public long totalContributionPoint;

	public bool hasOffice;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref createdAt);
		stream.PutOrGetEnum(ref grade);
		stream.PutOrGet(ref lastOnlineTime);
		stream.PutOrGet(ref greeting);
		stream.PutOrGet(ref lastAttendanceDate);
		stream.PutOrGet(ref weeklyContributionPoint);
		stream.PutOrGet(ref totalContributionPoint);
		stream.PutOrGet(ref hasOffice);
	}
}
