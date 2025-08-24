using System;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class SurveyInfo : ISerializable
{
	public long surveyId;

	public int userLevel;

	public DateTime startDate;

	public DateTime endDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref surveyId);
		stream.PutOrGet(ref userLevel);
		stream.PutOrGet(ref startDate);
		stream.PutOrGet(ref endDate);
	}
}
