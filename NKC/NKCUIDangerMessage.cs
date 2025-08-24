using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDangerMessage : MonoBehaviour
{
	public Animator m_animator;

	public Text m_message;

	private const string ANI_INTRO = "AB_FX_UI_DANGER_MESSAGE_INTRO";

	private const string ANI_LOOP = "AB_FX_UI_DANGER_MESSAGE_LOOP";

	private const string ANI_OUTRO = "AB_FX_UI_DANGER_MESSAGE_OUTRO";

	private const string SOUND_NAME = "FX_UI_DUNGEON_NO_UNIT_WARNING";

	private Coroutine m_playCoroutine;

	private Coroutine m_stopCoroutine;

	private int m_soundUID;

	public void Play(string message)
	{
		Clear();
		m_message.text = message;
		base.gameObject.SetActive(value: true);
		if (base.gameObject.activeInHierarchy)
		{
			m_animator.Play("AB_FX_UI_DANGER_MESSAGE_INTRO");
			m_playCoroutine = StartCoroutine(Loop());
		}
	}

	public void Stop()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		ClearCoroutine();
		if (base.gameObject.activeInHierarchy)
		{
			m_animator.Play("AB_FX_UI_DANGER_MESSAGE_OUTRO");
			m_stopCoroutine = StartCoroutine(Outro());
			if (m_soundUID > 0)
			{
				NKCSoundManager.StopSound(m_soundUID);
				m_soundUID = 0;
			}
		}
	}

	public void Clear()
	{
		ClearCoroutine();
		if (m_soundUID > 0)
		{
			NKCSoundManager.StopSound(m_soundUID);
			m_soundUID = 0;
		}
	}

	private void ClearCoroutine()
	{
		if (m_playCoroutine != null)
		{
			StopCoroutine(m_playCoroutine);
		}
		m_playCoroutine = null;
		if (m_stopCoroutine != null)
		{
			StopCoroutine(m_stopCoroutine);
		}
		m_stopCoroutine = null;
	}

	private IEnumerator Loop()
	{
		if (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		m_animator.Play("AB_FX_UI_DANGER_MESSAGE_LOOP");
		m_soundUID = NKCSoundManager.PlaySound("FX_UI_DUNGEON_NO_UNIT_WARNING", 1f, 0f, 0f, bLoop: true);
	}

	private IEnumerator Outro()
	{
		if (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		base.gameObject.SetActive(value: false);
	}
}
