using UnityEngine;

namespace NKC;

public class NKCUILobbyMenuPanel : MonoBehaviour
{
	public CanvasGroup m_CanvasGroup;

	public Animator m_Animator;

	public float alpha
	{
		get
		{
			if (!(m_CanvasGroup != null))
			{
				return 0f;
			}
			return m_CanvasGroup.alpha;
		}
		set
		{
			if (m_CanvasGroup != null)
			{
				m_CanvasGroup.alpha = value;
			}
		}
	}

	private void Awake()
	{
		if (m_CanvasGroup == null)
		{
			m_CanvasGroup = GetComponent<CanvasGroup>();
		}
		if (m_Animator == null)
		{
			m_Animator = GetComponent<Animator>();
		}
	}

	public void PlayIntroAnimation()
	{
		if (m_Animator != null)
		{
			m_Animator.SetTrigger("Intro");
		}
	}

	public void PlaySelectAnimation(bool bSelected)
	{
		if (m_Animator != null)
		{
			m_Animator.SetBool("Active", bSelected);
		}
	}
}
