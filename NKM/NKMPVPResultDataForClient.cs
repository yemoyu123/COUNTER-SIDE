using System;
using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMPVPResultDataForClient : ISerializable
{
	public PVP_RESULT result;

	public PvpState myInfo;

	public PvpSingleHistory history;

	public NKMItemMiscData pvpPoint;

	public DateTime pvpPointChargeTime;

	public bool rankPvpOpen;

	public bool leaguePvpOpen;

	public List<NKMItemMiscData> pvpChargePoint = new List<NKMItemMiscData>();

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref result);
		stream.PutOrGet(ref myInfo);
		stream.PutOrGet(ref history);
		stream.PutOrGet(ref pvpPoint);
		stream.PutOrGet(ref pvpPointChargeTime);
		stream.PutOrGet(ref rankPvpOpen);
		stream.PutOrGet(ref leaguePvpOpen);
		stream.PutOrGet(ref pvpChargePoint);
	}
}
