using System.Collections.Generic;
using NKM.Templet;

namespace NKM;

public class NKMFindTargetData
{
	public float m_fFindTargetTime = 0.1f;

	public bool m_bTargetNoChange;

	public NKM_FIND_TARGET_TYPE m_FindTargetType = NKM_FIND_TARGET_TYPE.NFTT_NEAR_ENEMY;

	public float m_fSeeRange;

	public bool m_bNoBackTarget;

	public bool m_bNoFrontTarget;

	public bool m_bCanTargetBoss = true;

	public bool m_bUseUnitSize;

	public HashSet<NKM_UNIT_ROLE_TYPE> m_hsFindTargetRolePriority = new HashSet<NKM_UNIT_ROLE_TYPE>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_hsFindTargetStylePriority = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public bool m_bPriorityOnly;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		_ = 1;
		cNKMLua.GetData("m_FindTargetType", ref m_FindTargetType);
		cNKMLua.GetData("m_fFindTargetTime", ref m_fFindTargetTime);
		cNKMLua.GetData("m_bTargetNoChange", ref m_bTargetNoChange);
		cNKMLua.GetData("m_fSeeRange", ref m_fSeeRange);
		cNKMLua.GetData("m_bNoBackTarget", ref m_bNoBackTarget);
		cNKMLua.GetData("m_bNoFrontTarget", ref m_bNoFrontTarget);
		cNKMLua.GetData("m_bCanTargetBoss", ref m_bCanTargetBoss);
		cNKMLua.GetDataListEnum("m_hsFindTargetRolePriority", m_hsFindTargetRolePriority);
		cNKMLua.GetDataListEnum("m_hsFindTargetStylePriority", m_hsFindTargetStylePriority);
		cNKMLua.GetData("m_bPriorityOnly", ref m_bPriorityOnly);
		cNKMLua.GetData("m_bUseUnitSize", ref m_bUseUnitSize);
		return true;
	}

	public static bool LoadFromLUA(NKMLua cNKMLua, string tableName, out NKMFindTargetData data)
	{
		if (cNKMLua.OpenTable(tableName))
		{
			data = new NKMFindTargetData();
			bool result = data.LoadFromLUA(cNKMLua);
			cNKMLua.CloseTable();
			return result;
		}
		data = null;
		return false;
	}

	public static void DeepCopyFrom(NKMFindTargetData source, out NKMFindTargetData target)
	{
		if (source == null)
		{
			target = null;
			return;
		}
		target = new NKMFindTargetData();
		target.DeepCopyFrom(source);
	}

	public void DeepCopyFrom(NKMFindTargetData source)
	{
		m_FindTargetType = source.m_FindTargetType;
		m_fFindTargetTime = source.m_fFindTargetTime;
		m_bTargetNoChange = source.m_bTargetNoChange;
		m_fSeeRange = source.m_fSeeRange;
		m_bNoBackTarget = source.m_bNoBackTarget;
		m_bNoFrontTarget = source.m_bNoFrontTarget;
		m_bCanTargetBoss = source.m_bCanTargetBoss;
		m_hsFindTargetRolePriority.Clear();
		m_hsFindTargetRolePriority.UnionWith(source.m_hsFindTargetRolePriority);
		m_hsFindTargetStylePriority.Clear();
		m_hsFindTargetStylePriority.UnionWith(source.m_hsFindTargetStylePriority);
		m_bPriorityOnly = source.m_bPriorityOnly;
		m_bUseUnitSize = source.m_bUseUnitSize;
	}
}
