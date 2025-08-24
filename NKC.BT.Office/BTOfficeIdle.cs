using NKM;

namespace NKC.BT.Office;

public class BTOfficeIdle : BTOfficeActionBase
{
	public float MinIdleTime;

	public float MaxIdleTime;

	public string IdleAnimName;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		bActionSuccessFlag = true;
		if (MinIdleTime == 0f && MaxIdleTime == 0f && !string.IsNullOrEmpty(IdleAnimName))
		{
			float animTime = m_Character.GetAnimTime(IdleAnimName);
			NKCAnimationInstance idleInstance = GetIdleInstance(animTime, m_Character.transform.localPosition, IdleAnimName);
			m_Character.EnqueueAnimation(idleInstance);
		}
		else
		{
			NKCAnimationInstance idleInstance2 = GetIdleInstance(NKMRandom.Range(MinIdleTime, MaxIdleTime), m_Character.transform.localPosition, IdleAnimName);
			m_Character.EnqueueAnimation(idleInstance2);
		}
	}
}
