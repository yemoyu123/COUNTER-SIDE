using NKC;
using NKM.Unit;

namespace NKM;

public interface INKCUnitstateEventOneTime : INKMUnitStateEvent, IEventConditionOwner
{
	void ApplyEventClient(NKCGameClient cNKMGame, NKCUnitClient cNKMUnit);
}
