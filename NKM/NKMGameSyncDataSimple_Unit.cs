using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMGameSyncDataSimple_Unit : ISerializable
{
	public short m_GameUnitUID;

	public short m_TargetUID;

	public short m_SubTargetUID;

	public bool m_bRight = true;

	public Dictionary<short, NKMBuffSyncData> m_dicBuffData = new Dictionary<short, NKMBuffSyncData>();

	public List<NKMUnitEventSyncData> m_listNKM_UNIT_EVENT_MARK = new List<NKMUnitEventSyncData>();

	public List<NKMUnitStatusTimeSyncData> m_listStatusTimeData = new List<NKMUnitStatusTimeSyncData>();

	public List<NKMUnitSyncData.InvokedTriggerInfo> m_listInvokedTrigger = new List<NKMUnitSyncData.InvokedTriggerInfo>();

	public Dictionary<string, int> m_dicEventVariables = new Dictionary<string, int>();

	public List<NKMUnitSyncData.ReactionSync> m_listUpdatedReaction = new List<NKMUnitSyncData.ReactionSync>();

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_GameUnitUID);
		stream.PutOrGet(ref m_TargetUID);
		stream.PutOrGet(ref m_SubTargetUID);
		stream.PutOrGet(ref m_bRight);
		stream.PutOrGet(ref m_dicBuffData);
		stream.PutOrGet(ref m_listNKM_UNIT_EVENT_MARK);
		stream.PutOrGet(ref m_listStatusTimeData);
		stream.PutOrGet(ref m_listInvokedTrigger);
		stream.PutOrGet(ref m_dicEventVariables);
		stream.PutOrGet(ref m_listUpdatedReaction);
	}
}
