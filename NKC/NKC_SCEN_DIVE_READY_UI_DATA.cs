namespace NKC;

public class NKC_SCEN_DIVE_READY_UI_DATA
{
	public NKCAssetInstanceData m_NUF_LOGIN_PREFAB;

	public NKC_SCEN_DIVE_READY_UI_DATA()
	{
		Init();
	}

	public void Init()
	{
		if (m_NUF_LOGIN_PREFAB != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NUF_LOGIN_PREFAB);
		}
		m_NUF_LOGIN_PREFAB = null;
	}
}
