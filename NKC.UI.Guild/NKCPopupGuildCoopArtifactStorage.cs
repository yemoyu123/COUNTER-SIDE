using UnityEngine;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopArtifactStorage : MonoBehaviour
{
	public NKCUIComStateButton m_btnClose;

	public NKCUIComGuildArtifactContent m_ArtifactContent;

	public void InitUI()
	{
		m_ArtifactContent.Init();
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(Close);
	}

	public void Close()
	{
		m_ArtifactContent.Close();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_ArtifactContent.SetData(NKCGuildCoopManager.GetMyArtifactDictionary());
	}
}
