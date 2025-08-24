using System.Collections;
using UnityEngine;

namespace NKC.UI.Result;

public abstract class NKCUIResultSubUIBase : MonoBehaviour
{
	private const string LIST_OUT_SOUND_BUNDLE_NAME = "ab_sound";

	private const string LIST_OUT_SOUND_ASSET_NAME = "FX_UI_TITLE_OUT_TEST";

	private const float SKIP_DELAY = 0.1f;

	private const float MAX_ANI_WAIT_TIME = 5f;

	protected bool m_bIgnoreAutoClose;

	protected bool m_bPause;

	protected bool m_bWillPlayCountdown;

	private RectTransform m_RectTransform;

	protected bool m_bHadUserInput;

	public bool ProcessRequired { get; set; }

	public RectTransform RectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	protected abstract IEnumerator InnerProcess(bool bAutoSkip);

	public abstract bool IsProcessFinished();

	public abstract void FinishProcess();

	public virtual void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public IEnumerator Process(bool bAutoSkip = false)
	{
		if (!base.gameObject.activeSelf)
		{
			yield break;
		}
		StartCoroutine(InnerProcess(bAutoSkip));
		m_bPause = false;
		m_bHadUserInput = false;
		while (!IsProcessFinished())
		{
			if (m_bHadUserInput)
			{
				FinishProcess();
				break;
			}
			yield return null;
		}
	}

	public virtual void OnUserInput()
	{
		m_bHadUserInput = true;
	}

	public void SetPause(bool bValue)
	{
		m_bPause = bValue;
	}

	public void SetReserveCountdown(bool bValue)
	{
		m_bWillPlayCountdown = bValue;
	}

	public IEnumerator PlayCloseAnimation(Animator animator)
	{
		if (!(animator == null))
		{
			NKCSoundManager.PlaySound("FX_UI_TITLE_OUT_TEST", 1f, base.transform.position.x, 0f);
			animator.enabled = true;
			animator.Play("OUTRO");
			yield return WaitAniOrInput(animator);
		}
	}

	public IEnumerator WaitAniOrInput(Animator animator)
	{
		m_bHadUserInput = false;
		float deltaTime = 0f;
		if (animator == null)
		{
			yield return new WaitForSeconds(0.1f);
			while (!m_bHadUserInput)
			{
				yield return null;
				if (!m_bIgnoreAutoClose)
				{
					deltaTime += Time.deltaTime;
					if (deltaTime > 5f)
					{
						break;
					}
				}
			}
			yield break;
		}
		while (true)
		{
			yield return null;
			if (m_bHadUserInput || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				break;
			}
			if (!m_bIgnoreAutoClose)
			{
				deltaTime += Time.deltaTime;
				if (deltaTime > 5f)
				{
					break;
				}
			}
		}
	}

	protected IEnumerator WaitTimeOrUserInput(float waitTime = 5f)
	{
		float currentTime = 0f;
		m_bHadUserInput = false;
		if (waitTime == 0f)
		{
			yield break;
		}
		if (waitTime < 0f)
		{
			while (!m_bHadUserInput)
			{
				yield return null;
			}
			yield break;
		}
		while (currentTime < waitTime)
		{
			currentTime += Time.deltaTime;
			if (!m_bHadUserInput)
			{
				yield return null;
				continue;
			}
			break;
		}
	}
}
