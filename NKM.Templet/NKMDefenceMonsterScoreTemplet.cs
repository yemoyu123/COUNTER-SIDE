using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMDefenceMonsterScoreTemplet : INKMTemplet
{
	private static Dictionary<int, List<NKMDefenceMonsterScoreTemplet>> MonsterScoreGroups = new Dictionary<int, List<NKMDefenceMonsterScoreTemplet>>();

	public int ScoreGroupID { get; private set; }

	public int Index { get; private set; }

	public int Level { get; private set; }

	public int Score { get; private set; }

	public int Key => Index;

	public static IReadOnlyDictionary<int, List<NKMDefenceMonsterScoreTemplet>> Groups => MonsterScoreGroups;

	public static NKMDefenceMonsterScoreTemplet LoadFromLua(NKMLua lua)
	{
		int rValue = 0;
		int rValue2 = 0;
		int rValue3 = 0;
		int rValue4 = 0;
		if ((1u | (lua.GetData("INDEX", ref rValue) ? 1u : 0u) | (lua.GetData("m_ClearScoreGroup", ref rValue2) ? 1u : 0u) | (lua.GetData("m_MonsterLevel", ref rValue3) ? 1u : 0u) | (lua.GetData("m_MonsterClearScore", ref rValue4) ? 1u : 0u)) == 0)
		{
			NKMTempletError.Add($"[NKMDefenceMonsterScoreTemplet :{rValue}] data is invalid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceMonsterScoreTemplet.cs", 37);
			return null;
		}
		return new NKMDefenceMonsterScoreTemplet
		{
			ScoreGroupID = rValue2,
			Index = rValue,
			Level = rValue3,
			Score = rValue4
		};
	}

	public void Join()
	{
		if (!MonsterScoreGroups.ContainsKey(ScoreGroupID))
		{
			MonsterScoreGroups.Add(ScoreGroupID, new List<NKMDefenceMonsterScoreTemplet>());
			MonsterScoreGroups[ScoreGroupID].Add(this);
		}
		else
		{
			MonsterScoreGroups[ScoreGroupID].Add(this);
		}
		if (MonsterScoreGroups[ScoreGroupID].Any())
		{
			MonsterScoreGroups[ScoreGroupID] = MonsterScoreGroups[ScoreGroupID].OrderBy((NKMDefenceMonsterScoreTemplet e) => e.Level).ToList();
		}
	}

	public void Validate()
	{
	}
}
