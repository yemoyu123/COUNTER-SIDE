namespace NKM;

public class NKMMapLayer
{
	public string m_LayerName = "";

	public float m_fMoveFactor;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_LayerName", ref m_LayerName);
		cNKMLua.GetData("m_fMoveFactor", ref m_fMoveFactor);
		return true;
	}
}
