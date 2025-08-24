using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_SURVEY_UPSERT_NOT)]
public sealed class NKMPacket_SURVEY_UPSERT_NOT : ISerializable
{
	public List<SurveyInfo> surveyInfos = new List<SurveyInfo>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref surveyInfos);
	}
}
