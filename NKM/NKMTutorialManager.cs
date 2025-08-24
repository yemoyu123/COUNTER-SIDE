namespace NKM;

public class NKMTutorialManager
{
	public static bool IsTutorialCompleted(TutorialStep step, NKMUserData userData)
	{
		if (userData.IsSuperUser())
		{
			return true;
		}
		if (userData.m_MissionData.GetCompletedMissionData(GetMissionID(step)) != null)
		{
			return true;
		}
		return false;
	}

	private static int GetMissionID(TutorialStep step)
	{
		return (int)step;
	}
}
