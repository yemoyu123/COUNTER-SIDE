using NKC.Office;

namespace BehaviorDesigner.Runtime;

public class BTSharedTargetUnitType : BTSharedEnum<ActTargetType>
{
	public static implicit operator BTSharedTargetUnitType(ActTargetType value)
	{
		return new BTSharedTargetUnitType
		{
			Value = value
		};
	}
}
