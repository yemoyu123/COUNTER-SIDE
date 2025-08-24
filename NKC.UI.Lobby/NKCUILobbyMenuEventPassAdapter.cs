using System.Collections.Generic;
using NKM;
using NKM.Templet;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuEventPassAdapter : NKCUILobbyMenuButtonBase
{
	public List<NKCUILobbyMenuEventPass> lstEventPass;

	public void Init(ContentsType contentsType = ContentsType.None)
	{
		foreach (NKCUILobbyMenuEventPass item in lstEventPass)
		{
			item.Init(contentsType);
		}
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		foreach (NKCUILobbyMenuEventPass item in lstEventPass)
		{
			item.UpdateContents(userData);
		}
	}

	protected override void UpdateLock()
	{
		m_bLocked = !NKCContentManager.IsContentsUnlocked(m_ContentsType);
		foreach (NKCUILobbyMenuEventPass item in lstEventPass)
		{
			item.UpdateLock(m_bLocked);
		}
	}

	public void UpdateEventPassEnabled()
	{
		foreach (NKCUILobbyMenuEventPass item in lstEventPass)
		{
			item.UpdateEventPassEnabled();
			item.UpdateLeftTime();
		}
	}
}
