using NKM;

namespace NKC;

public class NKCEffectReserveData : NKMObjectPoolData
{
	public NKCASEffect m_NKCASEffect;

	public float m_PosX;

	public float m_PosY;

	public float m_PosZ;

	public bool m_bNotStart;

	public float m_fReserveTime;

	public NKCEffectReserveData()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCEffectReserveData;
	}

	public override void Unload()
	{
		m_NKCASEffect = null;
	}
}
