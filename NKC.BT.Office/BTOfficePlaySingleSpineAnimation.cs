using BehaviorDesigner.Runtime.Tasks;

namespace NKC.BT.Office;

public class BTOfficePlaySingleSpineAnimation : BTOfficeActionBase
{
	public string AnimName;

	public bool bInvert;

	public bool bNow;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		bActionSuccessFlag = true;
		m_Character.EnqueueSimpleAni(AnimName, bNow, bInvert);
	}

	public override TaskStatus OnUpdate()
	{
		if (!bActionSuccessFlag)
		{
			return TaskStatus.Failure;
		}
		if (m_Character.PlayAnimCompleted())
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}
}
