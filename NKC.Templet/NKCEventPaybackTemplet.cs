using NKM;
using NKM.Templet.Base;

namespace NKC.Templet;

public class NKCEventPaybackTemplet : INKMTemplet
{
	private int paybackEventId;

	private string openTag;

	private string intervalTag;

	private int missionTabId;

	private string shortCutShopId;

	private string bannerPrefabId;

	private string unitStrId = "";

	private int skinId;

	private int extraMissionID;

	public int Key => paybackEventId;

	public string IntervalTag => intervalTag;

	public string UnitStrId => unitStrId;

	public int SkinId => skinId;

	public int MissionTabId => missionTabId;

	public string BannerPrefabId => bannerPrefabId;

	public int ExtraMissionID => extraMissionID;

	public static NKCEventPaybackTemplet Find(int eventId)
	{
		return NKMTempletContainer<NKCEventPaybackTemplet>.Find(eventId);
	}

	public static NKCEventPaybackTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Templet/NKCEventPaybackTemplet.cs", 32))
		{
			return null;
		}
		NKCEventPaybackTemplet nKCEventPaybackTemplet = new NKCEventPaybackTemplet();
		int num = (int)(1u & (cNKMLua.GetData("PaybackEventID", ref nKCEventPaybackTemplet.paybackEventId) ? 1u : 0u) & (cNKMLua.GetData("OpenTag", ref nKCEventPaybackTemplet.openTag) ? 1u : 0u) & (cNKMLua.GetData("IntervalTag", ref nKCEventPaybackTemplet.intervalTag) ? 1u : 0u) & (cNKMLua.GetData("MissionTabID", ref nKCEventPaybackTemplet.missionTabId) ? 1u : 0u) & (cNKMLua.GetData("ShortCutShopID", ref nKCEventPaybackTemplet.shortCutShopId) ? 1u : 0u)) & (cNKMLua.GetData("BannerPrefabID", ref nKCEventPaybackTemplet.bannerPrefabId) ? 1 : 0);
		cNKMLua.GetData("ExtraMissionID", ref nKCEventPaybackTemplet.extraMissionID);
		cNKMLua.GetData("UnitStrID", ref nKCEventPaybackTemplet.unitStrId);
		cNKMLua.GetData("SkinID", ref nKCEventPaybackTemplet.skinId);
		if (num == 0)
		{
			return null;
		}
		return nKCEventPaybackTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
