using NKM;

namespace NKC;

public class NKCDamageEffectManager : NKMDamageEffectManager
{
	protected override NKMDamageEffect CreateDamageEffect()
	{
		return (NKCDamageEffect)m_NKMGame.GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCDamageEffect);
	}

	protected override void CloseDamageEffect(NKMDamageEffect cNKMDamageEffect)
	{
		m_NKMGame.GetObjectPool().CloseObj(cNKMDamageEffect);
	}
}
