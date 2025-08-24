using UnityEngine;

namespace NKC.UI.HUD;

public class NKCGameHudObjects : MonoBehaviour
{
	[Header("SCEN_GAME")]
	public GameObject m_NUF_BEFORE_HUD_EFFECT;

	public GameObject m_NUF_BEFORE_HUD_CONTROL_EFFECT_ANCHOR;

	public GameObject m_NUF_BEFORE_HUD_CONTROL_EFFECT;

	public GameObject m_NUF_AFTER_HUD_EFFECT;

	[Header("for HUD")]
	public GameObject m_NUF_GAME_HUD_UI_EMOTICON;

	public GameObject m_NUF_GAME_HUD_UI_PAUSE;

	[Header("GameHUD")]
	public NKCGameHud m_GameHud;
}
