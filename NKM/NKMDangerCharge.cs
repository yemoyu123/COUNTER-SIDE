namespace NKM;

public class NKMDangerCharge
{
	public float m_fChargeTime = -1f;

	public float m_fCancelDamageRate;

	public int m_CancelHitCount;

	public string m_SuccessState = "";

	public string m_CancelState = "";

	public void DeepCopyFromSource(NKMDangerCharge source)
	{
		m_fChargeTime = source.m_fChargeTime;
		m_fCancelDamageRate = source.m_fCancelDamageRate;
		m_CancelHitCount = source.m_CancelHitCount;
		m_SuccessState = source.m_SuccessState;
		m_CancelState = source.m_CancelState;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_fChargeTime", ref m_fChargeTime);
		cNKMLua.GetData("m_fCancelDamageRate", ref m_fCancelDamageRate);
		cNKMLua.GetData("m_CancelHitCount", ref m_CancelHitCount);
		cNKMLua.GetData("m_SuccessState", ref m_SuccessState);
		cNKMLua.GetData("m_CancelState", ref m_CancelState);
		return true;
	}
}
