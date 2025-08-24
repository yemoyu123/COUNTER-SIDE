using System.Collections.Generic;
using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMPvpBanResult : ISerializable
{
	public Dictionary<int, NKMBanData> unitBanList = new Dictionary<int, NKMBanData>();

	public Dictionary<int, NKMBanShipData> shipBanList = new Dictionary<int, NKMBanShipData>();

	public Dictionary<int, NKMBanOperatorData> operatorBanList = new Dictionary<int, NKMBanOperatorData>();

	public Dictionary<int, NKMUnitUpData> unitUpList = new Dictionary<int, NKMUnitUpData>();

	public Dictionary<int, NKMBanData> unitCastingBanList = new Dictionary<int, NKMBanData>();

	public Dictionary<int, NKMBanShipData> shipCastingBanList = new Dictionary<int, NKMBanShipData>();

	public Dictionary<int, NKMBanOperatorData> operatorCastingBanList = new Dictionary<int, NKMBanOperatorData>();

	public Dictionary<int, NKMBanData> unitFinalBanList = new Dictionary<int, NKMBanData>();

	public Dictionary<int, NKMBanShipData> shipFinalBanList = new Dictionary<int, NKMBanShipData>();

	public Dictionary<int, NKMBanOperatorData> operatorFinalBanList = new Dictionary<int, NKMBanOperatorData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitBanList);
		stream.PutOrGet(ref shipBanList);
		stream.PutOrGet(ref operatorBanList);
		stream.PutOrGet(ref unitUpList);
		stream.PutOrGet(ref unitCastingBanList);
		stream.PutOrGet(ref shipCastingBanList);
		stream.PutOrGet(ref operatorCastingBanList);
		stream.PutOrGet(ref unitFinalBanList);
		stream.PutOrGet(ref shipFinalBanList);
		stream.PutOrGet(ref operatorFinalBanList);
	}
}
