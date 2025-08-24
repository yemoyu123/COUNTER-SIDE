using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_ShipSkill : ISerializable
{
	public NKMUnitSyncData m_NKMGameUnitSyncData;

	public int m_ShipSkillID;

	public float m_SkillPosX;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_NKMGameUnitSyncData);
		stream.PutOrGet(ref m_ShipSkillID);
		stream.AsHalf(ref m_SkillPosX);
	}
}
