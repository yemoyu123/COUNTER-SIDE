using NKC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyNamespace;

public class NKCUICutScenTalkBoxMgrForJapan : MonoBehaviour, INKCUICutScenTalkBoxMgr
{
	public GameObject m_gTalkerNameLine;

	public Text m_lbTalkerName;

	public Text m_lbTalk;

	public TextMeshProUGUI m_lbTmpTalkerName;

	public TextMeshProUGUI m_lbTmpTalk;

	private string m_goalTalk;

	public GameObject m_goTalkNext;

	public CanvasGroup m_CanvasGroup;

	public bool useTMPro;

	private NKCUITypeWriter m_NKCUITypeWriter = new NKCUITypeWriter();

	private bool m_bFinished = true;

	private bool m_bWaitClick;

	private bool m_bOpenStateBeforePause;

	private bool m_bPause;

	private bool m_bTempFlag;

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

	public NKCUICutScenTalkBoxMgr.TalkBoxType MyBoxType => NKCUICutScenTalkBoxMgr.TalkBoxType.JapanNeeds;

	private void Update()
	{
		if (m_bPause || m_bFinished)
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
			m_CanvasGroup.alpha = 0f;
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
		NKCUtil.SetGameobjectActive(m_gTalkerNameLine, !string.IsNullOrEmpty(_TalkerName));
		NKCUtil.SetGameobjectActive(m_lbTmpTalkerName, UsingTMPText());
		NKCUtil.SetGameobjectActive(m_lbTmpTalk, UsingTMPText());
		NKCUtil.SetGameobjectActive(m_lbTalkerName, !UsingTMPText());
		NKCUtil.SetGameobjectActive(m_lbTalk, !UsingTMPText());
		if (UsingTMPText())
		{
			if (m_bTempFlag)
			{
				m_lbTmpTalkerName.SetText(NKCUITypeWriter.ReplaceNameString(_TalkerName, bBlock: false));
			}
			else
			{
				m_lbTmpTalkerName.SetText(NKCUITypeWriter.ReplaceNameString(_TalkerName, bBlock: false) + " ");
			}
			m_bTempFlag = !m_bTempFlag;
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTalkerName, NKCUITypeWriter.ReplaceNameString(_TalkerName, bBlock: false));
		}
		if (fCoolTime <= 0f)
		{
			if (UsingTMPText())
			{
				m_lbTmpTalk.SetText(_Talk);
			}
			else
			{
				m_lbTalk.text = _Talk;
			}
			m_bFinished = true;
			m_CanvasGroup.alpha = 1f;
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
			if (UsingTMPText())
			{
				m_lbTmpTalk.SetText("");
			}
			else
			{
				m_lbTalk.text = "";
			}
		}
		if (UsingTMPText())
		{
			m_NKCUITypeWriter.Start(m_lbTmpTalk, m_goalTalk, fCoolTime, _bTalkAppend);
		}
		else
		{
			m_NKCUITypeWriter.Start(m_lbTalk, m_goalTalk, fCoolTime, _bTalkAppend);
		}
	}

	public void StartFadeIn(float Time)
	{
	}

	public void FadeOutBooking(float time)
	{
	}

	public void ClearTalk()
	{
		NKCUtil.SetLabelText(m_lbTalk, string.Empty);
		NKCUtil.SetLabelText(m_lbTalkerName, string.Empty);
		m_lbTmpTalk?.SetText(string.Empty);
		m_lbTmpTalkerName?.SetText(string.Empty);
		NKCUtil.SetGameobjectActive(m_gTalkerNameLine, bValue: false);
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(this, bValue: false);
	}

	public void OnChange()
	{
		Close();
	}

	public bool UsingTMPText()
	{
		if (useTMPro && m_lbTmpTalk != null)
		{
			return m_lbTmpTalkerName != null;
		}
		return false;
	}
}
