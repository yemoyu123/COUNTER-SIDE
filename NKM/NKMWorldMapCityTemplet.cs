using Cs.Logging;
using NKC;
using NKM.Templet.Base;

namespace NKM;

public class NKMWorldMapCityTemplet : INKMTemplet
{
	public enum CityType
	{
		CT_INVALID,
		CT_MINING,
		CT_RELAY,
		CT_DEFENCE
	}

	public int m_ID = -1;

	public string m_StrID = "";

	public string m_Name = "";

	public string m_NameEng = "";

	public CityType m_CityType;

	public int m_MaxLevel = 1;

	public string m_Title = "";

	public string m_Description = "";

	public int Key => m_ID;

	public static NKMWorldMapCityTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMWorldMapCityTemplet nKMWorldMapCityTemplet = new NKMWorldMapCityTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_CityID", ref nKMWorldMapCityTemplet.m_ID) ? 1u : 0u) & (cNKMLua.GetData("m_CityStrID", ref nKMWorldMapCityTemplet.m_StrID) ? 1u : 0u) & (cNKMLua.GetData("m_CityName", ref nKMWorldMapCityTemplet.m_Name) ? 1u : 0u) & (cNKMLua.GetData("m_CityNameEng", ref nKMWorldMapCityTemplet.m_NameEng) ? 1u : 0u) & (cNKMLua.GetData("m_CityType", ref nKMWorldMapCityTemplet.m_CityType) ? 1u : 0u)) & (cNKMLua.GetData("m_MaxLevel", ref nKMWorldMapCityTemplet.m_MaxLevel) ? 1 : 0);
		cNKMLua.GetData("m_Title", ref nKMWorldMapCityTemplet.m_Title);
		cNKMLua.GetData("m_Description", ref nKMWorldMapCityTemplet.m_Description);
		nKMWorldMapCityTemplet.CheckValidation();
		if (num == 0)
		{
			Log.Error($"NKMWorldMapCityTemplet Load Fail - {nKMWorldMapCityTemplet.m_ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMWorldMapManager.cs", 102);
			return null;
		}
		return nKMWorldMapCityTemplet;
	}

	private void CheckValidation()
	{
		if (m_CityType == CityType.CT_INVALID)
		{
			Log.ErrorAndExit($"[WorldMapCityTemplet] 도시 타입이 존재하지 않음 m_ID : {m_ID}, m_CityType : {m_CityType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMWorldMapManager.cs", 113);
		}
		for (int i = 1; i <= m_MaxLevel; i++)
		{
			if (NKMWorldMapManager.GetCityExpTable(i) == null)
			{
				Log.ErrorAndExit($"[WorldMapCityTemplet] 도시 레벨은 {i} 보다 클 수 없음 m_ID : {m_ID}, m_MaxLevel : {m_MaxLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMWorldMapManager.cs", 120);
			}
		}
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public string GetName()
	{
		return NKCStringTable.GetString(m_Name);
	}

	public string GetNameEng()
	{
		return NKCStringTable.GetString(m_NameEng);
	}

	public string GetTitle()
	{
		return NKCStringTable.GetString(m_Title);
	}

	public string GetDesc()
	{
		return NKCStringTable.GetString(m_Description);
	}
}
