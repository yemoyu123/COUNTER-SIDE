using Cs.Protocol;

namespace NKM;

public class NKMDamageData : ISerializable
{
	public long m_GameUnitUIDAttacker;

	public bool m_bAttackCountOver;

	public int m_FinalDamage;

	public NKM_DAMAGE_RESULT_TYPE m_NKM_DAMAGE_RESULT_TYPE;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_GameUnitUIDAttacker);
		stream.PutOrGet(ref m_bAttackCountOver);
		stream.PutOrGet(ref m_FinalDamage);
		stream.PutOrGetEnum(ref m_NKM_DAMAGE_RESULT_TYPE);
	}
}
