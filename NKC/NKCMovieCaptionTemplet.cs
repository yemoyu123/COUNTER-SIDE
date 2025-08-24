using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;

namespace NKC;

public class NKCMovieCaptionTemplet : INKMTemplet
{
	public int m_Idx;

	public string m_Caption;

	public string m_StringKey;

	public int m_StartSecond;

	public int m_ShowSecond;

	public bool m_bHideBackground;

	private const int DEFAULT_DISPLAY_CAPTION_TIME = 3;

	public int Key => m_Idx;

	public static IEnumerable<NKCMovieCaptionTemplet> Values => NKMTempletContainer<NKCMovieCaptionTemplet>.Values;

	public static NKCMovieCaptionTemplet LoadFromLUA(NKMLua lua)
	{
		NKCMovieCaptionTemplet nKCMovieCaptionTemplet = new NKCMovieCaptionTemplet();
		int num = 1 & (lua.GetData("m_Idx", ref nKCMovieCaptionTemplet.m_Idx) ? 1 : 0);
		string nationalPostfix = NKCStringTable.GetNationalPostfix(NKCStringTable.GetNationalCode());
		int num2 = (int)((uint)num & (lua.GetData("m_Caption" + nationalPostfix, ref nKCMovieCaptionTemplet.m_Caption) ? 1u : 0u) & (lua.GetData("m_StringKey", ref nKCMovieCaptionTemplet.m_StringKey) ? 1u : 0u)) & (lua.GetData("m_StartSecond", ref nKCMovieCaptionTemplet.m_StartSecond) ? 1 : 0);
		if (!lua.GetData("m_ShowSecond", ref nKCMovieCaptionTemplet.m_ShowSecond))
		{
			nKCMovieCaptionTemplet.m_ShowSecond = 3;
		}
		lua.GetData("m_bHideBackground", ref nKCMovieCaptionTemplet.m_bHideBackground);
		if (num2 == 0)
		{
			return null;
		}
		return nKCMovieCaptionTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
