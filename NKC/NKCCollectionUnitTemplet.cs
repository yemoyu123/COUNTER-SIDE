using NKC.Templet.Base;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKCCollectionUnitTemplet : INKCTemplet
{
	public int Idx;

	public int m_UnitID;

	public string m_UnitStrID;

	private string m_UnitIntro;

	public NKM_UNIT_TYPE m_NKM_UNIT_TYPE;

	public string m_CutsceneLifetime_Start;

	public string m_CutsceneLifetime_End;

	public string m_CutsceneLifetime_BG;

	public string m_ContentsVersionStart = "";

	public string m_ContentsVersionEnd = "";

	public bool m_bExclude;

	public int Key => m_UnitID;

	public string UnitIntro => m_UnitIntro;

	public static NKCCollectionUnitTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCCollectionManager.cs", 32))
		{
			return null;
		}
		NKCCollectionUnitTemplet nKCCollectionUnitTemplet = new NKCCollectionUnitTemplet();
		cNKMLua.GetData("m_ContentsVersionStart", ref nKCCollectionUnitTemplet.m_ContentsVersionStart);
		cNKMLua.GetData("m_ContentsVersionEnd", ref nKCCollectionUnitTemplet.m_ContentsVersionEnd);
		int num = (int)(1u & (cNKMLua.GetData("Idx", ref nKCCollectionUnitTemplet.Idx) ? 1u : 0u) & (cNKMLua.GetData("m_UnitID", ref nKCCollectionUnitTemplet.m_UnitID) ? 1u : 0u) & (cNKMLua.GetData("m_UnitStrID", ref nKCCollectionUnitTemplet.m_UnitStrID) ? 1u : 0u) & (cNKMLua.GetData("m_NKM_UNIT_TYPE", ref nKCCollectionUnitTemplet.m_NKM_UNIT_TYPE) ? 1u : 0u)) & (cNKMLua.GetData("m_UnitIntro", ref nKCCollectionUnitTemplet.m_UnitIntro) ? 1 : 0);
		cNKMLua.GetData("m_CutsceneLifetime_Start", ref nKCCollectionUnitTemplet.m_CutsceneLifetime_Start);
		cNKMLua.GetData("m_CutsceneLifetime_End", ref nKCCollectionUnitTemplet.m_CutsceneLifetime_End);
		cNKMLua.GetData("m_CutsceneLifetime_BG", ref nKCCollectionUnitTemplet.m_CutsceneLifetime_BG);
		cNKMLua.GetData("m_bExclude", ref nKCCollectionUnitTemplet.m_bExclude);
		if (num == 0)
		{
			return null;
		}
		return nKCCollectionUnitTemplet;
	}

	public string GetUnitIntro()
	{
		return NKCStringTable.GetString(m_UnitIntro);
	}
}
