namespace NKM;

public struct MissionUpdateData
{
	private NKM_MISSION_COND missionCond;

	private long value1;

	private long value2;

	private long value3;

	public MissionUpdateData(NKM_MISSION_COND missionCond, long value1, long value2 = 0L, long value3 = 0L)
	{
		this.missionCond = missionCond;
		this.value1 = value1;
		this.value2 = value2;
		this.value3 = value3;
	}

	public MissionUpdateData(NKM_MISSION_COND missionCond, long value1, long value2)
	{
		this.missionCond = missionCond;
		this.value1 = value1;
		this.value2 = value2;
		value3 = 0L;
	}

	public MissionUpdateData(NKM_MISSION_COND missionCond, long value1)
	{
		this.missionCond = missionCond;
		this.value1 = value1;
		value2 = 0L;
		value3 = 0L;
	}
}
