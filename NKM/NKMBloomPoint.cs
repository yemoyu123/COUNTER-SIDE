namespace NKM;

public class NKMBloomPoint
{
	public NKMAssetName m_LensFlareName = new NKMAssetName();

	public float m_fBloomPointX;

	public float m_fBloomPointY;

	public float m_fBloomAddIntensity;

	public float m_fBloomAddThreshHold;

	public float m_fBloomDistance;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_LensFlareName.LoadFromLua(cNKMLua, "m_LensFlareName");
		cNKMLua.GetData("m_fBloomPointX", ref m_fBloomPointX);
		cNKMLua.GetData("m_fBloomPointY", ref m_fBloomPointY);
		cNKMLua.GetData("m_fBloomAddIntensity", ref m_fBloomAddIntensity);
		cNKMLua.GetData("m_fBloomAddThreshHold", ref m_fBloomAddThreshHold);
		cNKMLua.GetData("m_fBloomDistance", ref m_fBloomDistance);
		return true;
	}
}
