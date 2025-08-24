using Cs.Protocol;

namespace NKM;

public class NKMDynamicRespawnUnitData : ISerializable
{
	public NKMUnitData m_NKMUnitData = new NKMUnitData();

	public short m_MasterGameUnitUID;

	public bool m_bLoadedServer;

	public bool m_bLoadedClient;

	public virtual void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_NKMUnitData);
		stream.PutOrGet(ref m_MasterGameUnitUID);
		stream.PutOrGet(ref m_bLoadedServer);
		stream.PutOrGet(ref m_bLoadedClient);
	}

	public void DeepCopyFromSource(NKMDynamicRespawnUnitData source)
	{
		m_NKMUnitData.DeepCopyFrom(source.m_NKMUnitData);
		m_MasterGameUnitUID = source.m_MasterGameUnitUID;
		m_bLoadedServer = source.m_bLoadedServer;
		m_bLoadedClient = source.m_bLoadedClient;
	}
}
