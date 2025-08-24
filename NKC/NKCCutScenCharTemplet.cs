using NKM;

namespace NKC;

public class NKCCutScenCharTemplet
{
	public string m_CharStrID = "";

	public string m_CharStr = "";

	public string m_PrefabStr = "";

	public bool m_Background;

	public int m_SkinOption;

	public string m_SkinName;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		string nationalPostfix = NKCStringTable.GetNationalPostfix(NKCStringTable.GetNationalCode());
		cNKMLua.GetData("m_CharStrID", ref m_CharStrID);
		cNKMLua.GetData("m_CharStr" + nationalPostfix, ref m_CharStr);
		cNKMLua.GetData("m_PrefabStr", ref m_PrefabStr);
		cNKMLua.GetData("m_Background", ref m_Background);
		cNKMLua.GetData("m_SkinOption", ref m_SkinOption);
		cNKMLua.GetData("m_SkinName", ref m_SkinName);
		return true;
	}
}
