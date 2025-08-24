using Cs.GameLog.CountryDescription;
using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMWorldMapMissionTemplet : INKMTemplet
{
	public enum WorldMapMissionType
	{
		[CountryDescription("알수없음", CountryCode.KOR)]
		WMT_INVALID,
		[CountryDescription("파견", CountryCode.KOR)]
		WMT_EXPLORE,
		[CountryDescription("방위", CountryCode.KOR)]
		WMT_DEFENCE,
		[CountryDescription("채굴", CountryCode.KOR)]
		WMT_MINING,
		[CountryDescription("사무", CountryCode.KOR)]
		WMT_OFFICE
	}

	public enum WorldMapMissionRank
	{
		WMMR_S,
		WMMR_A,
		WMMR_B,
		WMMR_C
	}

	public int m_ID = -1;

	public string m_MissionName = "";

	public int m_MissionLevel;

	public int m_WorldmapMissionPoolID;

	public int m_WorldmapMissionRatio;

	public WorldMapMissionRank m_eMissionRank;

	public int m_ReqManagerLevel;

	public int m_MissionTimeInMinutes = 1;

	public int m_RewardCityEXP;

	public int m_RewardUnitExp;

	public int m_RewardCredit;

	public int m_RewardEternium;

	public int m_RewardInformation;

	public NKM_REWARD_TYPE m_CompleteRewardType;

	public int m_CompleteRewardID;

	public string m_CompleteRewardStrID = "";

	public int m_CompleteRewardQuantity;

	public WorldMapMissionType m_eMissionType;

	public string m_WorldMapMissionThumbnailFile = "";

	public int m_WorldmapEventRatio;

	public int m_WorldmapEventGroup;

	public bool m_bEnableMission;

	public int Key => m_ID;

	public static NKMWorldMapMissionTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapMissionTemplet.cs", 72))
		{
			return null;
		}
		NKMWorldMapMissionTemplet nKMWorldMapMissionTemplet = new NKMWorldMapMissionTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_WorldmapMissionID", ref nKMWorldMapMissionTemplet.m_ID) ? 1u : 0u) & (cNKMLua.GetData("m_WorldmapMissionName", ref nKMWorldMapMissionTemplet.m_MissionName) ? 1u : 0u) & (cNKMLua.GetData("m_WorldmapMissionPoolID", ref nKMWorldMapMissionTemplet.m_WorldmapMissionPoolID) ? 1u : 0u) & (cNKMLua.GetData("m_WorldMapMissionLevel", ref nKMWorldMapMissionTemplet.m_MissionLevel) ? 1u : 0u) & (cNKMLua.GetData("m_WorldmapMissionRatio", ref nKMWorldMapMissionTemplet.m_WorldmapMissionRatio) ? 1u : 0u) & (cNKMLua.GetData("m_WorldMapMissionRank", ref nKMWorldMapMissionTemplet.m_eMissionRank) ? 1u : 0u) & (cNKMLua.GetData("m_ReqManagerLevel", ref nKMWorldMapMissionTemplet.m_ReqManagerLevel) ? 1u : 0u) & (cNKMLua.GetData("m_MissionTime", ref nKMWorldMapMissionTemplet.m_MissionTimeInMinutes) ? 1u : 0u) & (cNKMLua.GetData("m_RewardCityEXP", ref nKMWorldMapMissionTemplet.m_RewardCityEXP) ? 1u : 0u) & (cNKMLua.GetData("m_RewardUnitEXP", ref nKMWorldMapMissionTemplet.m_RewardUnitExp) ? 1u : 0u) & (cNKMLua.GetData("m_RewardEternium", ref nKMWorldMapMissionTemplet.m_RewardEternium) ? 1u : 0u) & (cNKMLua.GetData("m_RewardCredit", ref nKMWorldMapMissionTemplet.m_RewardCredit) ? 1u : 0u)) & (cNKMLua.GetData("m_RewardInformation", ref nKMWorldMapMissionTemplet.m_RewardInformation) ? 1 : 0);
		cNKMLua.GetData("m_CompleteReward_Type", ref nKMWorldMapMissionTemplet.m_CompleteRewardType);
		cNKMLua.GetData("m_CompleteReward_ID", ref nKMWorldMapMissionTemplet.m_CompleteRewardID);
		cNKMLua.GetData("m_CompleteReward_StrID", ref nKMWorldMapMissionTemplet.m_CompleteRewardStrID);
		cNKMLua.GetData("m_CompleteRewardQuantity", ref nKMWorldMapMissionTemplet.m_CompleteRewardQuantity);
		cNKMLua.GetData("m_WorldMapMissionThumbnailFile", ref nKMWorldMapMissionTemplet.m_WorldMapMissionThumbnailFile);
		if (((uint)num & (cNKMLua.GetData("m_WorldmapMission_Type", ref nKMWorldMapMissionTemplet.m_eMissionType) ? 1u : 0u) & (cNKMLua.GetData("m_WorldmapEventRatio", ref nKMWorldMapMissionTemplet.m_WorldmapEventRatio) ? 1u : 0u) & (cNKMLua.GetData("m_WorldmapEventGroup", ref nKMWorldMapMissionTemplet.m_WorldmapEventGroup) ? 1u : 0u) & (cNKMLua.GetData("m_bEnableMission", ref nKMWorldMapMissionTemplet.m_bEnableMission) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return nKMWorldMapMissionTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_eMissionType == WorldMapMissionType.WMT_INVALID)
		{
			Log.ErrorAndExit($"[WorldMapMissionTemplet] 월드맵 미션 타입이 존재하지 않음 m_ID : {m_ID}, m_eMissionType : {m_eMissionType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapMissionTemplet.cs", 119);
		}
		if (m_CompleteRewardType != NKM_REWARD_TYPE.RT_NONE && m_CompleteRewardID > 0 && (!NKMRewardTemplet.IsValidReward(m_CompleteRewardType, m_CompleteRewardID) || m_CompleteRewardQuantity <= 0))
		{
			Log.ErrorAndExit($"[WorldMapMissionTemplet] 월드맵 미션 완료 보상 정보가 존재하지 않음 m_ID : {m_ID}, m_CompleteRewardType : {m_CompleteRewardType}, m_CompleteRewardID : {m_CompleteRewardID}, m_CompleteRewardQuantity : {m_CompleteRewardQuantity}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWorldMapMissionTemplet.cs", 126);
		}
	}

	public string GetMissionName()
	{
		return NKCStringTable.GetString(m_MissionName);
	}
}
