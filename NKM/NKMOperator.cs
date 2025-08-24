using System;
using Cs.Protocol;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMOperator : ISerializable
{
	public long uid;

	public int id;

	public int level;

	public int exp;

	public NKMOperatorSkill mainSkill;

	public NKMOperatorSkill subSkill;

	public bool bLock;

	public bool fromContract;

	public int power;

	public DateTime regDate;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref id);
		stream.PutOrGet(ref uid);
		stream.PutOrGet(ref level);
		stream.PutOrGet(ref exp);
		stream.PutOrGet(ref bLock);
		stream.PutOrGet(ref mainSkill);
		stream.PutOrGet(ref subSkill);
		stream.PutOrGet(ref fromContract);
	}

	public static int CalculateOperationPower(int unitID, int level, int mainSkillID, int mainSkillLevel, int subSkillID, int subSkillLevel)
	{
		int result = 0;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase != null)
		{
			NKMOperatorSkillTemplet nKMOperatorSkillTemplet = NKMTempletContainer<NKMOperatorSkillTemplet>.Find(mainSkillID);
			if (nKMOperatorSkillTemplet == null)
			{
				return 0;
			}
			NKMOperatorSkillTemplet nKMOperatorSkillTemplet2 = NKMTempletContainer<NKMOperatorSkillTemplet>.Find(subSkillID);
			if (nKMOperatorSkillTemplet2 == null)
			{
				return 0;
			}
			if (mainSkillLevel == 0 || subSkillLevel == 0)
			{
				return 0;
			}
			float num = (float)mainSkillLevel / (float)nKMOperatorSkillTemplet.m_MaxSkillLevel * 3000f;
			float num2 = (float)subSkillLevel / (float)nKMOperatorSkillTemplet2.m_MaxSkillLevel * 3000f;
			float num3 = (float)level / (float)NKMCommonConst.OperatorConstTemplet.unitMaximumLevel * 3000f;
			float num4 = 3000f;
			switch (unitTempletBase.m_NKM_UNIT_GRADE)
			{
			case NKM_UNIT_GRADE.NUG_SR:
				num4 *= 0.6f;
				break;
			case NKM_UNIT_GRADE.NUG_R:
				num4 *= 0.3f;
				break;
			case NKM_UNIT_GRADE.NUG_N:
				num4 *= 0.1f;
				break;
			}
			result = (int)(num + num2 + num3 + num4 + 0.5f);
		}
		return result;
	}

	public int CalculateOperatorOperationPower()
	{
		return CalculateOperationPower(id, level, mainSkill.id, mainSkill.level, subSkill.id, subSkill.level);
	}

	public NKMTacticalCommandTemplet GetTacticalCommandTemplet()
	{
		if (mainSkill != null)
		{
			NKMOperatorSkillTemplet nKMOperatorSkillTemplet = NKMTempletContainer<NKMOperatorSkillTemplet>.Find(mainSkill.id);
			if (nKMOperatorSkillTemplet != null && nKMOperatorSkillTemplet.m_OperSkillType == OperatorSkillType.m_Tactical)
			{
				return NKMTacticalCommandManager.GetTacticalCommandTempletByStrID(nKMOperatorSkillTemplet.m_OperSkillTarget);
			}
		}
		return null;
	}
}
