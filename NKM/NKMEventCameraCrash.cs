using NKM.Unit;

namespace NKM;

public class NKMEventCameraCrash : NKMUnitStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public NKM_CAMERA_CRASH_TYPE m_CameraCrashType = NKM_CAMERA_CRASH_TYPE.NCCT_DOWN;

	public float m_fCameraCrashSpeed;

	public float m_fCameraCrashAccel;

	public float m_fCameraCrashGap;

	public float m_fCameraCrashTime;

	public float m_fCrashRadius;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Client;

	public void DeepCopyFromSource(NKMEventCameraCrash source)
	{
		DeepCopy(source);
		m_CameraCrashType = source.m_CameraCrashType;
		m_fCameraCrashSpeed = source.m_fCameraCrashSpeed;
		m_fCameraCrashAccel = source.m_fCameraCrashAccel;
		m_fCameraCrashGap = source.m_fCameraCrashGap;
		m_fCameraCrashTime = source.m_fCameraCrashTime;
		m_fCrashRadius = source.m_fCrashRadius;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_CameraCrashType", ref m_CameraCrashType);
		cNKMLua.GetData("m_fCameraCrashSpeed", ref m_fCameraCrashSpeed);
		cNKMLua.GetData("m_fCameraCrashAccel", ref m_fCameraCrashAccel);
		cNKMLua.GetData("m_fCameraCrashGap", ref m_fCameraCrashGap);
		cNKMLua.GetData("m_fCameraCrashTime", ref m_fCameraCrashTime);
		cNKMLua.GetData("m_fCrashRadius", ref m_fCrashRadius);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ApplyEventCameraCrash(this);
	}
}
