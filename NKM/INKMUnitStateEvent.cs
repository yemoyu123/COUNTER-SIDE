using NKM.Unit;

namespace NKM;

public interface INKMUnitStateEvent : IEventConditionOwner
{
	bool bAnimTime { get; }

	bool bStateEnd { get; }

	float EventStartTime { get; }

	EventRollbackType RollbackType { get; }

	bool LoadFromLUA(NKMLua cNKMLua);
}
