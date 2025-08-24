using UnityEngine;

namespace NKC.UI.HUD;

public class NKCGameHudObserver : MonoBehaviour
{
	public NKCUIComButton m_cbtnChangeTeam;

	private NKCGameHud m_GameHud;

	public void InitUI(NKCGameHud hud)
	{
		m_GameHud = hud;
		NKCUtil.SetButtonClickDelegate(m_cbtnChangeTeam, ChangeTeamDeck);
		NKCUtil.SetGameobjectActive(m_cbtnChangeTeam, bValue: true);
	}

	public void ChangeTeamDeck()
	{
		m_GameHud.ChangeTeamDeck();
	}
}
