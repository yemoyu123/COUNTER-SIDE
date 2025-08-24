namespace NKM;

public static class ContentsEnumExt
{
	public static bool IsAteam(this NKM_TEAM_TYPE teamType)
	{
		if (teamType != NKM_TEAM_TYPE.NTT_A1)
		{
			return teamType == NKM_TEAM_TYPE.NTT_A2;
		}
		return true;
	}

	public static bool IsStatHoldType(this NKM_SKILL_TYPE skillType)
	{
		if (skillType != NKM_SKILL_TYPE.NST_PASSIVE)
		{
			return skillType == NKM_SKILL_TYPE.NST_LEADER;
		}
		return true;
	}
}
