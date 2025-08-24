using System;
using DG.Tweening;
using NKC;
using UnityEngine;
using UnityEngine.UI;

public class NKCUICutScenTalkBoxMgrForCenterText : MonoBehaviour, INKCUICutScenTalkBoxMgr
{
	public Text m_lbTalk;

	public Image m_NKM_UI_CUTSCEN_PLAYER_TALK_BOX;

	public GameObject m_goTalkNext;

	public CanvasGroup m_CanvasGroup;

	private readonly NKCUITypeWriter m_NKCUITypeWriter = new NKCUITypeWriter();

	private string m_goalTalk;

	private bool m_bFinished = true;

	private bool m_bWaitClick;

	private bool m_bOpenStateBeforePause;

	private bool m_bPause;

	private bool m_bFinishedFade;

	private float m_fFadeOutTime;

	public bool WaitForFadOut => m_fFadeOutTime > 0f;

	public GameObject MyGameObject
	{
		get
		{
			if (!(base.gameObject == null))
			{
				return base.gameObject;
			}
			return null;
		}
	}

	public bool IsFinished => m_bFinished;

	public NKCUICutScenTalkBoxMgr.TalkBoxType MyBoxType => NKCUICutScenTalkBoxMgr.TalkBoxType.CenterText;

	private void Update()
	{
		if (m_bPause || m_bFinished || !m_bFinishedFade)
		{
			return;
		}
		if (m_CanvasGroup.alpha < 1f)
		{
			m_CanvasGroup.alpha += Time.deltaTime * 3f;
			if (m_CanvasGroup.alpha >= 1f)
			{
				m_CanvasGroup.alpha = 1f;
			}
		}
		m_NKCUITypeWriter.Update();
		if (!m_NKCUITypeWriter.IsTyping())
		{
			if (!m_goTalkNext.activeSelf && m_bWaitClick)
			{
				m_goTalkNext.SetActive(value: true);
			}
			if (m_CanvasGroup.alpha >= 1f)
			{
				m_bFinished = true;
			}
		}
	}

	public void SetPause(bool bPause)
	{
		if (!(base.gameObject == null))
		{
			if (bPause)
			{
				m_bOpenStateBeforePause = base.gameObject.activeSelf;
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(base.gameObject, m_bOpenStateBeforePause);
			}
			m_bPause = bPause;
		}
	}

	public void ResetTalkBox()
	{
		SetPause(bPause: false);
		m_bOpenStateBeforePause = false;
	}

	public void Finish()
	{
		m_NKCUITypeWriter.Finish();
		m_bFinished = true;
		if (!m_goTalkNext.activeSelf && m_bWaitClick)
		{
			m_goTalkNext.SetActive(value: true);
		}
		m_CanvasGroup.alpha = 1f;
	}

	public void Open(string _TalkerName, string _Talk, float fCoolTime, bool bWaitClick, bool _bTalkAppend)
	{
		if (base.gameObject == null)
		{
			return;
		}
		if (!base.gameObject.activeSelf)
		{
			m_CanvasGroup.alpha = 1f;
			base.gameObject.SetActive(value: true);
		}
		m_bWaitClick = bWaitClick;
		if (_bTalkAppend)
		{
			m_goalTalk += _Talk;
		}
		else
		{
			m_goalTalk = _Talk;
		}
		if (fCoolTime <= 0f)
		{
			m_lbTalk.text = _Talk;
			m_bFinished = true;
			if (!m_goTalkNext.activeSelf && bWaitClick)
			{
				m_goTalkNext.SetActive(value: true);
			}
			return;
		}
		if (m_goTalkNext.activeSelf)
		{
			m_goTalkNext.SetActive(value: false);
		}
		m_bFinished = false;
		if (!_bTalkAppend)
		{
			m_lbTalk.text = "";
		}
		m_NKCUITypeWriter.Start(m_lbTalk, m_goalTalk, fCoolTime, _bTalkAppend);
	}

	public void StartFadeIn(float time)
	{
		if (!m_bFinishedFade)
		{
			m_bFinishedFade = false;
			if (DOTween.IsTweening(m_NKM_UI_CUTSCEN_PLAYER_TALK_BOX))
			{
				m_NKM_UI_CUTSCEN_PLAYER_TALK_BOX.DOKill();
			}
			Sequence s = DOTween.Sequence();
			s.Append(m_NKM_UI_CUTSCEN_PLAYER_TALK_BOX.DOColor(new Color(1f, 1f, 1f, 0f), 0f));
			s.AppendInterval(0.1f);
			s.Append(m_NKM_UI_CUTSCEN_PLAYER_TALK_BOX.DOColor(new Color(1f, 1f, 1f, 0.7f), time).SetEase(Ease.OutSine).OnComplete(delegate
			{
				m_bFinishedFade = true;
			}));
		}
	}

	public void FadeOutBooking(float time)
	{
		m_fFadeOutTime = time;
	}

	public void StartFadeOut(Action onFadeComplete)
	{
		ClearTalk();
		m_NKM_UI_CUTSCEN_PLAYER_TALK_BOX.DOFade(0f, m_fFadeOutTime).SetEase(Ease.OutSine).OnComplete(delegate
		{
			onFadeComplete?.Invoke();
			m_fFadeOutTime = 0f;
		});
	}

	public void ClearTalk()
	{
		NKCUtil.SetLabelText(m_lbTalk, string.Empty);
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(this, bValue: false);
		m_bFinishedFade = false;
		m_fFadeOutTime = 0f;
	}

	public void OnChange()
	{
		Close();
	}

	public bool UsingTMPText()
	{
		return false;
	}
}
