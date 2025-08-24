using Cs.Protocol;
using NKM.Templet.Base;

namespace NKM;

public class NKMOperatorSkill : ISerializable
{
	public int id;

	public byte level;

	public int exp;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref id);
		stream.PutOrGet(ref level);
		stream.PutOrGet(ref exp);
	}

	public NKMBattleConditionTemplet GetBattleCondTemplet()
	{
		NKMOperatorSkillTemplet nKMOperatorSkillTemplet = NKMTempletContainer<NKMOperatorSkillTemplet>.Find(id);
		if (nKMOperatorSkillTemplet == null)
		{
			return null;
		}
		if (nKMOperatorSkillTemplet.m_OperSkillType != OperatorSkillType.m_Passive)
		{
			return null;
		}
		return NKMBattleConditionManager.GetTempletByStrID(nKMOperatorSkillTemplet.m_OperSkillTarget);
	}
}
