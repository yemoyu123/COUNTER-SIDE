using Cs.Protocol;

namespace NKM.Templet;

public class NKMRewardUnitExpData : ISerializable
{
	public long m_UnitUid;

	public int m_Exp;

	public int m_BonusExp;

	public int m_BonusRatio;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_UnitUid);
		stream.PutOrGet(ref m_Exp);
		stream.PutOrGet(ref m_BonusExp);
		stream.PutOrGet(ref m_BonusRatio);
	}
}
