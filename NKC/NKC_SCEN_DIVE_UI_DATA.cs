namespace NKC;

public class NKC_SCEN_DIVE_UI_DATA
{
	public NKCAssetInstanceData m_NUM_DIVE_PREFAB;

	public NKCAssetInstanceData m_NUF_DIVE_PREFAB;

	public NKC_SCEN_DIVE_UI_DATA()
	{
		Init();
	}

	public void Init()
	{
		if (m_NUM_DIVE_PREFAB != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NUM_DIVE_PREFAB);
		}
		if (m_NUF_DIVE_PREFAB != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NUF_DIVE_PREFAB);
		}
		m_NUM_DIVE_PREFAB = null;
		m_NUF_DIVE_PREFAB = null;
	}
}
