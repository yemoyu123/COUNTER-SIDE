using NKM;

namespace NKC;

public class NKCAnimationEventTemplet
{
	public string m_AniEventStrID = string.Empty;

	public float m_StartTime;

	public AnimationEventType m_AniEventType;

	public string m_StrValue = string.Empty;

	public float m_FloatValue;

	public bool m_BoolValue;

	public float m_FloatValue2;

	public string Key => m_AniEventStrID;

	public static NKCAnimationEventTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCAnimationEventTemplet.cs", 51))
		{
			return null;
		}
		NKCAnimationEventTemplet nKCAnimationEventTemplet = new NKCAnimationEventTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_AniEventStrID", ref nKCAnimationEventTemplet.m_AniEventStrID) ? 1u : 0u) & (cNKMLua.GetData("m_StartTime", ref nKCAnimationEventTemplet.m_StartTime) ? 1u : 0u)) & (cNKMLua.GetData("m_AniEventType", ref nKCAnimationEventTemplet.m_AniEventType) ? 1 : 0);
		cNKMLua.GetData("m_StrValue", ref nKCAnimationEventTemplet.m_StrValue);
		cNKMLua.GetData("m_FloatValue", ref nKCAnimationEventTemplet.m_FloatValue);
		cNKMLua.GetData("m_BoolValue", ref nKCAnimationEventTemplet.m_BoolValue);
		cNKMLua.GetData("m_FloatValue2", ref nKCAnimationEventTemplet.m_FloatValue2);
		if (num == 0)
		{
			return null;
		}
		return nKCAnimationEventTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
