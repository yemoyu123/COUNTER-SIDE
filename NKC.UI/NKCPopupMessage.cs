using System.Collections;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupMessage : NKCUIBase
{
	public enum eMessagePosition
	{
		Top,
		Middle,
		Bottom,
		BottomIngame,
		TopIngame
	}

	public const float MESSAGE_STAY_TIME = 2f;

	public const float MESSAGE_FADE_TIME = 0.5f;

	public Animator m_Ani;

	public Text m_lbMessage;

	public RectTransform m_rtMessageRoot;

	public CanvasGroup m_CanvasGroup;

	public GameObject m_objFX;

	public TRACKING_DATA_TYPE m_eFadeType;

	private PopupMessage m_preemptiveMessage;

	private Queue<PopupMessage> m_queue = new Queue<PopupMessage>();

	public float m_fAnchoredPosY = 127f;

	private const float m_fAnchoredPosY_For_Ingame = 354f;

	private const float m_fAnchoredPosY_For_IngameTop = 317f;

	private bool m_bPlaying;

	public override eMenutype eUIType => eMenutype.Overlay;

	public override string MenuName => "Message";

	public void Open(PopupMessage msg)
	{
		base.gameObject.SetActive(value: true);
		if (!msg.m_bPreemptive)
		{
			m_queue.Enqueue(msg);
		}
		else
		{
			m_preemptiveMessage = msg;
		}
		if (msg.m_bPreemptive)
		{
			if (!m_bPlaying)
			{
				m_bPlaying = true;
				UIOpened();
			}
			StopAllCoroutines();
			StartCoroutine(ProcessPreemptiveMessage());
		}
		else if (!m_bPlaying)
		{
			m_bPlaying = true;
			UIOpened();
			NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
			StopAllCoroutines();
			StartCoroutine(ProcessNormalMessage());
		}
	}

	private IEnumerator ProcessPreemptiveMessage()
	{
		PopupMessage preemptiveMessage = m_preemptiveMessage;
		if (preemptiveMessage != null)
		{
			NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFX, preemptiveMessage.m_bShowFX);
			if (preemptiveMessage.m_bShowFX)
			{
				NKCSoundManager.PlaySound("FX_UI_DECK_SLOT_OPEN", 1f, 0f, 0f);
			}
			SetPosition(preemptiveMessage.m_messagePosition);
			m_lbMessage.text = preemptiveMessage.m_message;
			yield return new WaitForSeconds(preemptiveMessage.m_delayTime);
			yield return StartCoroutine(ProcessShowMessage());
			while (m_Ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
			m_preemptiveMessage = null;
			if (m_queue.Count > 0)
			{
				StartCoroutine(ProcessNormalMessage());
			}
			else
			{
				Close();
			}
		}
	}

	private IEnumerator ProcessNormalMessage()
	{
		while (m_preemptiveMessage != null)
		{
			yield return null;
		}
		while (m_queue.Count > 0)
		{
			PopupMessage popupMessage = m_queue.Dequeue();
			if (popupMessage == null)
			{
				break;
			}
			NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
			SetPosition(popupMessage.m_messagePosition);
			m_lbMessage.text = popupMessage.m_message;
			yield return new WaitForSeconds(popupMessage.m_delayTime);
			yield return StartCoroutine(ProcessShowMessage());
			while (m_Ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
		}
		Close();
	}

	private void SetPosition(eMessagePosition position)
	{
		switch (position)
		{
		case eMessagePosition.Top:
			m_rtMessageRoot.anchorMin = new Vector2(0f, 1f);
			m_rtMessageRoot.anchorMax = new Vector2(1f, 1f);
			m_rtMessageRoot.anchoredPosition = new Vector2(0f, 0f - m_fAnchoredPosY);
			break;
		case eMessagePosition.Middle:
			m_rtMessageRoot.anchorMin = new Vector2(0f, 0.5f);
			m_rtMessageRoot.anchorMax = new Vector2(1f, 0.5f);
			m_rtMessageRoot.anchoredPosition = new Vector2(0f, 0f);
			break;
		case eMessagePosition.Bottom:
			m_rtMessageRoot.anchorMin = new Vector2(0f, 0f);
			m_rtMessageRoot.anchorMax = new Vector2(1f, 0f);
			m_rtMessageRoot.anchoredPosition = new Vector2(0f, m_fAnchoredPosY);
			break;
		case eMessagePosition.BottomIngame:
			m_rtMessageRoot.anchorMin = new Vector2(0f, 0f);
			m_rtMessageRoot.anchorMax = new Vector2(1f, 0f);
			m_rtMessageRoot.anchoredPosition = new Vector2(0f, 354f);
			break;
		case eMessagePosition.TopIngame:
			m_rtMessageRoot.anchorMin = new Vector2(0f, 1f);
			m_rtMessageRoot.anchorMax = new Vector2(1f, 1f);
			m_rtMessageRoot.anchoredPosition = new Vector2(0f, -317f);
			break;
		}
	}

	private IEnumerator ProcessShowMessage()
	{
		NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: true);
		m_Ani.Play("NKM_UI_POPUP_MESSAGE", 0, 0f);
		yield return StartCoroutine(ProcessAlpha(0f, 1f, 0.5f));
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(ProcessAlpha(1f, 0f, 0.5f));
		NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
	}

	private IEnumerator ProcessCloseOnly(float timeFadeOut)
	{
		if (timeFadeOut > 0f)
		{
			yield return StartCoroutine(ProcessAlpha(m_CanvasGroup.alpha, 0f, timeFadeOut));
		}
		else
		{
			m_CanvasGroup.alpha = 0f;
		}
		yield return null;
		Close();
	}

	private IEnumerator ProcessAlpha(float startValue, float endValue, float time)
	{
		float fDeltaTime = 0f;
		m_CanvasGroup.alpha = startValue;
		yield return null;
		for (fDeltaTime += Time.deltaTime; fDeltaTime < time; fDeltaTime += Time.deltaTime)
		{
			float alpha = NKCUtil.TrackValue(m_eFadeType, startValue, endValue, fDeltaTime, time);
			m_CanvasGroup.alpha = alpha;
			yield return null;
		}
		m_CanvasGroup.alpha = endValue;
	}

	public override void CloseInternal()
	{
		m_CanvasGroup.alpha = 0f;
		m_bPlaying = false;
		m_queue.Clear();
		base.gameObject.SetActive(value: false);
	}
}
