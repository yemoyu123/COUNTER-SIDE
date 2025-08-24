using NKM;

namespace NKC;

public class NKCScenMusicData
{
	public NKM_SCEN_ID m_NKM_SCEN_ID;

	public string m_MusicName = "";

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_NKM_SCEN_ID", ref m_NKM_SCEN_ID);
		cNKMLua.GetData("m_MusicName", ref m_MusicName);
		return true;
	}

	public void DeepCopyFromSource(NKCScenMusicData source)
	{
		m_NKM_SCEN_ID = source.m_NKM_SCEN_ID;
		m_MusicName = source.m_MusicName;
	}
}
