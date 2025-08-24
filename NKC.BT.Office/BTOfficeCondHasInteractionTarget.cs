using BehaviorDesigner.Runtime.Tasks;

namespace NKC.BT.Office;

public class BTOfficeCondHasInteractionTarget : BTOfficeConditionBase
{
	public override TaskStatus OnUpdate()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			return TaskStatus.Failure;
		}
		if (m_Character.HasInteractionTarget())
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}
}
