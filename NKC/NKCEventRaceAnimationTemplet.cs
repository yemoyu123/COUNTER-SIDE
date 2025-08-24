using NKM;

namespace NKC;

public class NKCEventRaceAnimationTemplet
{
	public RaceEventType m_RaceEventType;

	public string m_AnimationEventSetID;

	public int m_SlotCapacity;

	public string m_TargetObjName = string.Empty;

	public float m_SpawnPosX;

	public float m_Size = 1f;

	public int m_MaxCount;

	public int m_MinIndex;

	public int m_MaxIndex;

	public RaceEventType Key => m_RaceEventType;

	public static NKCEventRaceAnimationTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCEventRaceAnimationTemplet.cs", 48))
		{
			return null;
		}
		NKCEventRaceAnimationTemplet nKCEventRaceAnimationTemplet = new NKCEventRaceAnimationTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_RaceEventType", ref nKCEventRaceAnimationTemplet.m_RaceEventType) ? 1u : 0u) & (cNKMLua.GetData("m_AnimationEventSetID", ref nKCEventRaceAnimationTemplet.m_AnimationEventSetID) ? 1u : 0u)) & (cNKMLua.GetData("m_SlotCapacity", ref nKCEventRaceAnimationTemplet.m_SlotCapacity) ? 1 : 0);
		cNKMLua.GetData("m_TargetObjName", ref nKCEventRaceAnimationTemplet.m_TargetObjName);
		cNKMLua.GetData("m_SpawnPosX", ref nKCEventRaceAnimationTemplet.m_SpawnPosX);
		cNKMLua.GetData("m_Size", ref nKCEventRaceAnimationTemplet.m_Size);
		cNKMLua.GetData("m_MaxCount", ref nKCEventRaceAnimationTemplet.m_MaxCount);
		cNKMLua.GetData("m_MinIndex", ref nKCEventRaceAnimationTemplet.m_MinIndex);
		cNKMLua.GetData("m_MaxIndex", ref nKCEventRaceAnimationTemplet.m_MaxIndex);
		if (num == 0)
		{
			return null;
		}
		return nKCEventRaceAnimationTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
