using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMFiercePenaltyTemplet : INKMTemplet
{
	private int penaltyId;

	private int bossPenaltyGroupID;

	private int penaltyGroupId;

	private string penaltyGroupName;

	private string penaltyGroupDesc;

	private string penaltyIcon;

	private string penaltyIconBG;

	private int penaltyLevel;

	private string condStrId;

	private int fierceScoreRate;

	private string penaltyLevelDesc;

	public NKMBattleConditionTemplet battleCondition;

	private static Dictionary<int, List<NKMFiercePenaltyTemplet>> BossPenaltyGroupData = new Dictionary<int, List<NKMFiercePenaltyTemplet>>();

	public int Key => penaltyId;

	public static IEnumerable<NKMFiercePenaltyTemplet> Values => NKMTempletContainer<NKMFiercePenaltyTemplet>.Values;

	public static Dictionary<int, List<NKMFiercePenaltyTemplet>> PenaltyMainGroups => BossPenaltyGroupData;

	public int BossPenaltyGroupID => bossPenaltyGroupID;

	public int PenaltyGroupID => penaltyGroupId;

	public float FierceScoreRate => fierceScoreRate;

	public string PenaltyGroupName => penaltyGroupName;

	public string PenaltyGroupDesc => penaltyGroupDesc;

	public string PenaltyIcon => penaltyIcon;

	public string PenaltyIconBG => penaltyIconBG;

	public int PenaltyLevel => penaltyLevel;

	public string PenaltyLevelDesc => penaltyLevelDesc;

	public static NKMFiercePenaltyTemplet Find(int penaltyId)
	{
		return NKMTempletContainer<NKMFiercePenaltyTemplet>.Find(penaltyId);
	}

	public static NKMFiercePenaltyTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFiercePenaltyTemplet.cs", 45))
		{
			return null;
		}
		NKMFiercePenaltyTemplet nKMFiercePenaltyTemplet = new NKMFiercePenaltyTemplet
		{
			penaltyId = lua.GetInt32("PenaltyID"),
			penaltyGroupId = lua.GetInt32("PenaltyGroupID"),
			penaltyGroupName = lua.GetString("PenaltyGroupName"),
			penaltyGroupDesc = lua.GetString("PenaltyGroupDesc"),
			penaltyIcon = lua.GetString("PenaltyIcon"),
			penaltyIconBG = lua.GetString("PenaltyIconBG"),
			penaltyLevel = lua.GetInt32("PenaltyLevel"),
			condStrId = lua.GetString("BCondStrID"),
			fierceScoreRate = lua.GetInt32("FierceScoreRate"),
			penaltyLevelDesc = lua.GetString("PenaltyLevelDesc")
		};
		lua.GetData("BossPenaltyGroupID", ref nKMFiercePenaltyTemplet.bossPenaltyGroupID);
		return nKMFiercePenaltyTemplet;
	}

	public void Join()
	{
		battleCondition = NKMBattleConditionManager.GetTempletByStrID(condStrId);
		if (battleCondition == null)
		{
			NKMTempletError.Add($"[NKMFiercePenaltyTemplet:{Key}] 배틀 컨디션 템플릿을 찾을 수 없음. condStrId:{condStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFiercePenaltyTemplet.cs", 74);
		}
		if (BossPenaltyGroupData.ContainsKey(BossPenaltyGroupID))
		{
			BossPenaltyGroupData[bossPenaltyGroupID].Add(this);
			return;
		}
		List<NKMFiercePenaltyTemplet> list = new List<NKMFiercePenaltyTemplet>();
		list.Add(this);
		BossPenaltyGroupData.Add(bossPenaltyGroupID, list);
	}

	public void Validate()
	{
		if (penaltyGroupId < 0)
		{
			NKMTempletError.Add($"[NKMFiercePenaltyTemplet:{Key}] 그룹 아이디 이상. penaltyGroupId:{penaltyGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFiercePenaltyTemplet.cs", 94);
		}
		if (bossPenaltyGroupID < 0)
		{
			NKMTempletError.Add($"[NKMFiercePenaltyTemplet:{Key}] 그룹 아이디 이상. bossPenaltyGroupID:{bossPenaltyGroupID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFiercePenaltyTemplet.cs", 99);
		}
		if (fierceScoreRate <= 0)
		{
			NKMTempletError.Add($"[NKMFiercePenaltyTemplet:{Key}] Score 데이터 이상. rate:{fierceScoreRate} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFiercePenaltyTemplet.cs", 104);
		}
	}
}
