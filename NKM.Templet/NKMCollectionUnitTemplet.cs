using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMCollectionUnitTemplet : INKMTemplet
{
	public int Idx;

	public int m_UnitID;

	public string m_UnitStrID;

	private string m_UnitIntro;

	public NKM_UNIT_TYPE m_NKM_UNIT_TYPE;

	public string m_CutsceneLifetime_Start;

	public string m_CutsceneLifetime_End;

	public int Key => m_UnitID;

	public static IEnumerable<NKMCollectionUnitTemplet> Values => NKMTempletContainer<NKMCollectionUnitTemplet>.Values;

	public static IEnumerable<NKMCollectionUnitTemplet> EnableValues => Values.Where((NKMCollectionUnitTemplet e) => e.UnitTemplet.CollectionEnableByTag);

	public NKMUnitTempletBase UnitTemplet { get; private set; }

	public static NKMCollectionUnitTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionUnitTemplet.cs", 25))
		{
			return null;
		}
		NKMCollectionUnitTemplet nKMCollectionUnitTemplet = new NKMCollectionUnitTemplet();
		int num = (int)(1u & (cNKMLua.GetData("Idx", ref nKMCollectionUnitTemplet.Idx) ? 1u : 0u) & (cNKMLua.GetData("m_UnitID", ref nKMCollectionUnitTemplet.m_UnitID) ? 1u : 0u) & (cNKMLua.GetData("m_UnitStrID", ref nKMCollectionUnitTemplet.m_UnitStrID) ? 1u : 0u) & (cNKMLua.GetData("m_NKM_UNIT_TYPE", ref nKMCollectionUnitTemplet.m_NKM_UNIT_TYPE) ? 1u : 0u)) & (cNKMLua.GetData("m_UnitIntro", ref nKMCollectionUnitTemplet.m_UnitIntro) ? 1 : 0);
		cNKMLua.GetData("m_CutsceneLifetime_Start", ref nKMCollectionUnitTemplet.m_CutsceneLifetime_Start);
		cNKMLua.GetData("m_CutsceneLifetime_End", ref nKMCollectionUnitTemplet.m_CutsceneLifetime_End);
		if (num == 0)
		{
			return null;
		}
		return nKMCollectionUnitTemplet;
	}

	public void Join()
	{
		UnitTemplet = NKMUnitManager.GetUnitTempletBase(m_UnitID);
		if (UnitTemplet == null)
		{
			Log.ErrorAndExit($"[CollectionUnitTemplet] invalid unitId:{m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMCollectionUnitTemplet.cs", 45);
		}
		else
		{
			UnitTemplet.CollectionUnitTemplet = this;
		}
	}

	public void Validate()
	{
	}
}
