using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_TRIM_END_ACK)]
public sealed class NKMPacket_TRIM_END_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isFirst;

	public int bestScore;

	public TrimModeState trimModeState = new TrimModeState();

	public NKMTrimClearData trimClearData = new NKMTrimClearData();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isFirst);
		stream.PutOrGet(ref bestScore);
		stream.PutOrGet(ref trimModeState);
		stream.PutOrGet(ref trimClearData);
		stream.PutOrGet(ref costItemDataList);
	}
}
