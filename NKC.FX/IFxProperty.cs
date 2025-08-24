using NKM;

namespace NKC.FX;

public interface IFxProperty
{
	void SetFxProperty(NKCUnitClient masterUnit, NKCUnitClient targetUnit, NKMDamageEffectData DEData);
}
