using UnityEngine;
using UnityEngine.UI;

namespace NKC.Game;

public class NKCGameDebugAttackBox : MonoBehaviour
{
	public Transform m_trAttackBox;

	public SpriteRenderer m_srAttackBox;

	public Text m_lbAttackData;

	private bool m_bShowText = true;

	public float m_fYOffset = 50f;

	private Color m_colUnit = new Color(1f, 0.35f, 0f);

	private Color m_colDE = Color.cyan;
}
