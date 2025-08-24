using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMRaidTemplet : INKMTemplet
{
	private int m_StageID;

	private string m_StageStrID = "";

	private int m_RaidLevel;

	private int m_RaidTryCount;

	private int m_StageReqItemID;

	private int m_StageReqItemCount;

	private int m_DungeonID;

	private string m_DungeonStrID = "";

	private string m_FaceCardName = "";

	private string m_SpineIllustName = "";

	private string m_SpineSDName = "";

	private int m_AttendLimit;

	private string m_EventPointColorName = "";

	private float m_RaidDamageBasis;

	private int m_DeclineStageReqItemCount;

	private NKMDungeonTempletBase m_DungeonTempletBase;

	private string m_Difficulty = "";

	private int m_RewardRaidPoint_Victory;

	private int m_RewardRaidPoint_Fail;

	private readonly List<int> m_RewardRaidGroupIDList_Victory = new List<int>();

	private readonly List<int> m_RewardRaidGroupIDList_Fail = new List<int>();

	private readonly List<NKMRewardGroupTemplet> m_RewardRaidGroupTempletList_Victory = new List<NKMRewardGroupTemplet>();

	private readonly List<NKMRewardGroupTemplet> m_RewardRaidGroupTempletList_Fail = new List<NKMRewardGroupTemplet>();

	private string m_GuideShortCut = "";

	private int m_HelpStageReqItemCount;

	public int Key => m_StageID;

	public int StageID => m_StageID;

	public string StageStrID => m_StageStrID;

	public string DebugName => $"[{m_StageID}] {m_StageStrID}";

	public int RaidLevel => m_RaidLevel;

	public int RaidTryCount => m_RaidTryCount;

	public int StageReqItemID => m_StageReqItemID;

	public int StageReqItemCount => m_StageReqItemCount;

	public string FaceCardName => m_FaceCardName;

	public string SpineIllustName => m_SpineIllustName;

	public string SpineSDName => m_SpineSDName;

	public string Difficulty => m_Difficulty;

	public NKMDungeonTempletBase DungeonTempletBase => m_DungeonTempletBase;

	public bool HasAttendLimit => m_AttendLimit > 0;

	public int AttendLimit => m_AttendLimit;

	public string EventPointColorName => m_EventPointColorName;

	public int RewardRaidPoint_Victory => m_RewardRaidPoint_Victory;

	public int RewardRaidPoint_Fail => m_RewardRaidPoint_Fail;

	public float RaidDamageBasis => m_RaidDamageBasis;

	public int DeclineStageReqItemCount => m_DeclineStageReqItemCount;

	public int HelpStageReqItemCount => m_HelpStageReqItemCount;

	public IEnumerable<NKMRewardGroupTemplet> RewardRaidGroupTemplets_Victory => m_RewardRaidGroupTempletList_Victory;

	public IEnumerable<NKMRewardGroupTemplet> RewardRaidGroupTemplets_Fail => m_RewardRaidGroupTempletList_Fail;

	public string GuideShortCut => m_GuideShortCut;

	public static NKMRaidTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidTemplet.cs", 68))
		{
			return null;
		}
		NKMRaidTemplet nKMRaidTemplet = new NKMRaidTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_StageID", ref nKMRaidTemplet.m_StageID);
		flag &= cNKMLua.GetData("m_StageStrID", ref nKMRaidTemplet.m_StageStrID);
		flag &= cNKMLua.GetData("m_RaidLevel", ref nKMRaidTemplet.m_RaidLevel);
		flag &= cNKMLua.GetData("m_RaidTryCount", ref nKMRaidTemplet.m_RaidTryCount);
		flag &= cNKMLua.GetData("m_StageReqItemID", ref nKMRaidTemplet.m_StageReqItemID);
		flag &= cNKMLua.GetData("m_StageReqItemCount", ref nKMRaidTemplet.m_StageReqItemCount);
		flag &= cNKMLua.GetData("m_DungeonID", ref nKMRaidTemplet.m_DungeonID);
		flag &= cNKMLua.GetData("m_DungeonStrID", ref nKMRaidTemplet.m_DungeonStrID);
		flag &= cNKMLua.GetData("m_Difficulty", ref nKMRaidTemplet.m_Difficulty);
		flag &= cNKMLua.GetData("Raid_Damage_Basis", ref nKMRaidTemplet.m_RaidDamageBasis);
		flag &= cNKMLua.GetData("m_DeclineStageReqItemCount", ref nKMRaidTemplet.m_DeclineStageReqItemCount);
		flag &= cNKMLua.GetData("m_RewardRaidPoint_Victory", ref nKMRaidTemplet.m_RewardRaidPoint_Victory);
		flag &= cNKMLua.GetData("m_RewardRaidPoint_Fail", ref nKMRaidTemplet.m_RewardRaidPoint_Fail);
		for (int i = 0; i < 3; i++)
		{
			int rValue = 0;
			if (cNKMLua.GetData($"m_RewardRaidGroupID_Victory_{i + 1}", ref rValue))
			{
				nKMRaidTemplet.m_RewardRaidGroupIDList_Victory.Add(rValue);
			}
		}
		for (int j = 0; j < 3; j++)
		{
			int rValue2 = 0;
			if (cNKMLua.GetData($"m_RewardRaidGroupID_Fail_{j + 1}", ref rValue2))
			{
				nKMRaidTemplet.m_RewardRaidGroupIDList_Fail.Add(rValue2);
			}
		}
		cNKMLua.GetData("Attend_Limit", ref nKMRaidTemplet.m_AttendLimit);
		cNKMLua.GetData("m_EventPointColorName", ref nKMRaidTemplet.m_EventPointColorName);
		cNKMLua.GetData("m_FaceCardName", ref nKMRaidTemplet.m_FaceCardName);
		cNKMLua.GetData("GuideShortCut", ref nKMRaidTemplet.m_GuideShortCut);
		cNKMLua.GetData("m_HelpStageReqItemCount", ref nKMRaidTemplet.m_HelpStageReqItemCount);
		if (!flag)
		{
			return null;
		}
		return nKMRaidTemplet;
	}

	public static NKMRaidTemplet Find(int key)
	{
		return NKMTempletContainer<NKMRaidTemplet>.Find(key);
	}

	public void Join()
	{
		m_DungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_DungeonStrID);
		if (m_DungeonTempletBase == null)
		{
			NKMTempletError.Add("[" + DebugName + "] 던전 정보가 존재하지 않음 m_DungeonStrID:" + m_DungeonStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidTemplet.cs", 129);
		}
		foreach (int item in m_RewardRaidGroupIDList_Victory)
		{
			NKMRewardGroupTemplet rewardGroup = NKMRewardManager.GetRewardGroup(item);
			if (rewardGroup == null)
			{
				NKMTempletError.Add($"[{DebugName}] 승리 보상 그룹이 존재하지 않음 RewardGroupID:{item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidTemplet.cs", 137);
			}
			else
			{
				m_RewardRaidGroupTempletList_Victory.Add(rewardGroup);
			}
		}
		foreach (int item2 in m_RewardRaidGroupIDList_Fail)
		{
			NKMRewardGroupTemplet rewardGroup2 = NKMRewardManager.GetRewardGroup(item2);
			if (rewardGroup2 == null)
			{
				NKMTempletError.Add($"[{DebugName}] 패배 보상 그룹이 존재하지 않음 RewardGroupID:{item2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidTemplet.cs", 150);
			}
			else
			{
				m_RewardRaidGroupTempletList_Fail.Add(rewardGroup2);
			}
		}
	}

	public void Validate()
	{
		if (m_RewardRaidGroupTempletList_Victory.Count == 0)
		{
			NKMTempletError.Add("[" + DebugName + "] 승리 보상이 존재하지 않음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidTemplet.cs", 163);
		}
		if (m_RewardRaidGroupTempletList_Fail.Count == 0)
		{
			NKMTempletError.Add("[" + DebugName + "] 패배 보상이 존재하지 않음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidTemplet.cs", 168);
		}
		if (m_AttendLimit < 0 || m_AttendLimit > 15)
		{
			NKMTempletError.Add($"[{DebugName}] 입장 인원수 제한이 올바르지 않음:{m_AttendLimit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidTemplet.cs", 173);
		}
	}
}
