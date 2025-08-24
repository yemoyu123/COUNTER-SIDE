namespace NKC;

public class NKC_SCEN_CUTSCEN_SIM_UI_DATA
{
	public NKCAssetInstanceData m_NUF_CUTSCEN_SIM_PREFAB;

	public NKC_SCEN_CUTSCEN_SIM_UI_DATA()
	{
		Init();
	}

	public void Init()
	{
		if (m_NUF_CUTSCEN_SIM_PREFAB != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NUF_CUTSCEN_SIM_PREFAB);
		}
		m_NUF_CUTSCEN_SIM_PREFAB = null;
	}
}
