using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_TRIM_INTERVAL_INFO_NOT)]
public sealed class NKMPacket_TRIM_INTERVAL_INFO_NOT : ISerializable
{
	public int trimIntervalId;

	public NKMTrimIntervalData trimIntervalData = new NKMTrimIntervalData();

	public List<NKMTrimClearData> trimClearList = new List<NKMTrimClearData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref trimIntervalId);
		stream.PutOrGet(ref trimIntervalData);
		stream.PutOrGet(ref trimClearList);
	}
}
