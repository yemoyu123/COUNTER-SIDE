using System.Collections.Generic;
using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public class NKMShipCmdSlot : ISerializable
{
	public HashSet<NKM_UNIT_STYLE_TYPE> targetStyleType;

	public HashSet<NKM_UNIT_ROLE_TYPE> targetRoleType;

	public NKM_STAT_TYPE statType;

	public float statValue;

	public bool isLock;

	public NKMShipCmdSlot()
	{
		targetStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();
		targetRoleType = new HashSet<NKM_UNIT_ROLE_TYPE>();
		statType = NKM_STAT_TYPE.NST_RANDOM;
		statValue = 0f;
		isLock = false;
	}

	public NKMShipCmdSlot(HashSet<NKM_UNIT_STYLE_TYPE> styleType, HashSet<NKM_UNIT_ROLE_TYPE> roleType, NKM_STAT_TYPE statType, float value, bool isLock)
	{
		targetStyleType = styleType;
		targetRoleType = roleType;
		this.statType = statType;
		statValue = value;
		this.isLock = isLock;
	}

	public bool CanApply(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return false;
		}
		NKMUnitTempletBase unitTempletBase = unitData.GetUnitTempletBase();
		if (unitTempletBase == null)
		{
			return false;
		}
		if (targetStyleType.Count > 0 && !targetStyleType.Contains(unitTempletBase.m_NKM_UNIT_STYLE_TYPE) && !targetStyleType.Contains(unitTempletBase.m_NKM_UNIT_STYLE_TYPE_SUB))
		{
			return false;
		}
		if (targetRoleType.Count > 0 && !targetRoleType.Contains(unitTempletBase.m_NKM_UNIT_ROLE_TYPE))
		{
			return false;
		}
		return true;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref targetStyleType);
		stream.PutOrGetEnum(ref targetRoleType);
		stream.PutOrGetEnum(ref statType);
		stream.PutOrGet(ref statValue);
		stream.PutOrGet(ref isLock);
	}
}
