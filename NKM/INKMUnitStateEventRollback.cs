using NKM.Unit;

namespace NKM;

public interface INKMUnitStateEventRollback : INKMUnitStateEvent, IEventConditionOwner
{
	void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime);
}
