using NKM.Templet;

namespace NKM.Game;

public abstract class NKMEventConditionDetail
{
	public bool Inverse;

	public abstract bool CheckCondition(NKMGame cNKMGame, NKMUnit cNKMUnit, NKMUnit cUnitConditionOwner);

	public abstract NKMEventConditionDetail Clone();

	public abstract bool LoadFromLUA(NKMLua cNKMLua);

	public abstract bool Validate(NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet);
}
