namespace NKC;

public class NKC_SCEN_VOICE_LIST_UI_DATA
{
	public NKCAssetInstanceData m_NUF_VOICE_LIST_PREFAB;

	public NKC_SCEN_VOICE_LIST_UI_DATA()
	{
		Init();
	}

	public void Init()
	{
		if (m_NUF_VOICE_LIST_PREFAB != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NUF_VOICE_LIST_PREFAB);
		}
		m_NUF_VOICE_LIST_PREFAB = null;
	}
}
