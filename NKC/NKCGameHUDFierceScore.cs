using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCGameHUDFierceScore : MonoBehaviour
{
	public enum SCORE_TYPE
	{
		FIERCE,
		TRIM
	}

	public Animator m_NUF_GAME_HUD_FIERCE_BATTLE;

	public Text m_FIERCE_BATTLE_SCORE;

	public GameObject m_FIERCE_BATTLE_SCORE_ADD_ANI;

	public Animator m_aniFIERCE_BATTLE_SCORE_ADD_ANI;

	public Text m_FIERCE_BATTLE_SCORE_ADD;

	public GameObject m_FIERCE_BATTLE_SCORE_TEXT;

	public GameObject m_TRIM_BATTLE_SCORE_TEXT;

	private bool bDisplayScore;

	private bool m_bFirstOpen;

	private int m_iTotalScore;

	private int m_iAddScore;

	private bool bAddAni;

	private float CONST_ANI_DELAY_TIME = 0.3f;

	private float m_fCurTime;

	private float m_fNextTime;

	private const string ANI_PLAY = "PLAY";

	public void SetData(int totalScore, SCORE_TYPE scoreType)
	{
		if (!m_bFirstOpen)
		{
			m_bFirstOpen = true;
			m_iTotalScore = totalScore;
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_SCORE_ADD_ANI, bValue: false);
			NKCUtil.SetLabelText(m_FIERCE_BATTLE_SCORE, m_iTotalScore.ToString());
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_SCORE_TEXT, scoreType == SCORE_TYPE.FIERCE);
			NKCUtil.SetGameobjectActive(m_TRIM_BATTLE_SCORE_TEXT, scoreType == SCORE_TYPE.TRIM);
			OnActive();
		}
		else if (totalScore > m_iTotalScore)
		{
			m_iAddScore = totalScore - m_iTotalScore;
		}
	}

	private void OnActive()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_NUF_GAME_HUD_FIERCE_BATTLE?.SetTrigger("PLAY");
		bDisplayScore = true;
		SetDelayTimer();
	}

	private void SetDelayTimer()
	{
		m_fCurTime = Time.deltaTime;
		m_fNextTime = m_fCurTime + CONST_ANI_DELAY_TIME;
	}

	private void Update()
	{
		if (!bDisplayScore)
		{
			return;
		}
		m_fCurTime += Time.deltaTime;
		if (m_iAddScore > 0 && m_fCurTime > m_fNextTime)
		{
			if (!bAddAni)
			{
				bAddAni = true;
				NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_SCORE_ADD_ANI, bValue: true);
			}
			m_iTotalScore += m_iAddScore;
			NKCUtil.SetLabelText(m_FIERCE_BATTLE_SCORE_ADD, "+ " + m_iAddScore);
			NKCUtil.SetLabelText(m_FIERCE_BATTLE_SCORE, m_iTotalScore.ToString());
			m_iAddScore = 0;
			m_aniFIERCE_BATTLE_SCORE_ADD_ANI?.SetTrigger("PLAY");
			SetDelayTimer();
		}
	}
}
