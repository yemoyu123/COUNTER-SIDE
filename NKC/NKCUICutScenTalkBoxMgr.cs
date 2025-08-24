using MyNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUICutScenTalkBoxMgr : MonoBehaviour, INKCUICutScenTalkBoxMgr
{
	public enum TalkBoxType
	{
		Default,
		JapanNeeds,
		CenterText
	}

	public Text m_lbTalkerName;

	public Text m_lbTalk;

	public TextMeshProUGUI m_lbTmpTalkerName;

	public TextMeshProUGUI m_lbTmpTalk;

	private string m_goalTalk;

	public GameObject m_goTalkNext;

	public CanvasGroup m_CanvasGroup;

	public bool m_useTMPro;

	private NKCUITypeWriter m_NKCUITypeWriter = new NKCUITypeWriter();

	private static NKCUICutScenTalkBoxMgr m_NKCUICutScenTalkBoxMgr;

	private static NKCUICutScenTalkBoxMgrForJapan m_NKCUICutScenTalkBoxMgrForJapan;

	private static NKCUICutScenTalkBoxMgrForCenterText m_NKCUICutScenTalkBoxMgrForCenterText;

	private static INKCUICutScenTalkBoxMgr _curCutScenTalkBox;

	private bool m_bFinished = true;

	private bool m_bWaitClick;

	private bool m_bOpenStateBeforePause;

	private bool m_bPause;

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

	public TalkBoxType MyBoxType => TalkBoxType.JapanNeeds;

	public static INKCUICutScenTalkBoxMgr GetCutScenTalkBoxMgr(TalkBoxType talkBoxType)
	{
		if (_curCutScenTalkBox != null && _curCutScenTalkBox.MyBoxType == talkBoxType)
		{
			return _curCutScenTalkBox;
		}
		switch (talkBoxType)
		{
		case TalkBoxType.Default:
		case TalkBoxType.JapanNeeds:
			_curCutScenTalkBox?.OnChange();
			if (m_NKCUICutScenTalkBoxMgrForJapan == null)
			{
				_curCutScenTalkBox = m_NKCUICutScenTalkBoxMgr;
			}
			else
			{
				_curCutScenTalkBox = m_NKCUICutScenTalkBoxMgrForJapan;
			}
			break;
		case TalkBoxType.CenterText:
			_curCutScenTalkBox?.OnChange();
			if (m_NKCUICutScenTalkBoxMgrForJapan == null)
			{
				_curCutScenTalkBox = m_NKCUICutScenTalkBoxMgr;
			}
			else
			{
				_curCutScenTalkBox = m_NKCUICutScenTalkBoxMgrForCenterText;
			}
			break;
		default:
			return m_NKCUICutScenTalkBoxMgr;
		}
		return _curCutScenTalkBox;
	}

	public static void InitUI(GameObject goNKM_UI_CUTSCEN_PLAYER)
	{
		m_NKCUICutScenTalkBoxMgr = goNKM_UI_CUTSCEN_PLAYER.transform.Find("NKM_UI_CUTSCEN_PLAYER_TALK_BOX_MGR").gameObject.GetComponent<NKCUICutScenTalkBoxMgr>();
		m_NKCUICutScenTalkBoxMgrForJapan = goNKM_UI_CUTSCEN_PLAYER.transform.Find("NKM_UI_CUTSCEN_PLAYER_TALK_BOX_MGR_FOR_JAPAN")?.gameObject.GetComponent<NKCUICutScenTalkBoxMgrForJapan>();
		m_NKCUICutScenTalkBoxMgrForCenterText = goNKM_UI_CUTSCEN_PLAYER.transform.Find("NKM_UI_CUTSCEN_PLAYER_TALK_BOX_MGR_FOR_CENTER_TEXT")?.gameObject.GetComponent<NKCUICutScenTalkBoxMgrForCenterText>();
		if (m_NKCUICutScenTalkBoxMgr != null)
		{
			m_NKCUICutScenTalkBoxMgr.Close();
		}
		if (m_NKCUICutScenTalkBoxMgrForJapan != null)
		{
			m_NKCUICutScenTalkBoxMgrForJapan.Close();
		}
		if (m_NKCUICutScenTalkBoxMgrForCenterText != null)
		{
			m_NKCUICutScenTalkBoxMgrForCenterText.Close();
		}
	}

	public static void OnCleanUp()
	{
		m_NKCUICutScenTalkBoxMgr = null;
		m_NKCUICutScenTalkBoxMgrForJapan = null;
		m_NKCUICutScenTalkBoxMgrForCenterText = null;
		_curCutScenTalkBox = null;
	}

	public void ResetTalkBox()
	{
		SetPause(bPause: false);
		m_bOpenStateBeforePause = false;
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
		NKCUtil.SetGameobjectActive(m_lbTalk, !UsingTMPText());
		NKCUtil.SetGameobjectActive(m_lbTmpTalk, UsingTMPText());
		if (UsingTMPText())
		{
			m_lbTmpTalkerName.SetText(NKCUITypeWriter.ReplaceNameString(_TalkerName, bBlock: false));
		}
		else
		{
			m_lbTalkerName.text = NKCUITypeWriter.ReplaceNameString(_TalkerName, bBlock: false);
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
			m_NKCUITypeWriter.Start(m_lbTmpTalk, _TalkerName, m_goalTalk, fCoolTime, _bTalkAppend);
		}
		else
		{
			m_NKCUITypeWriter.Start(m_lbTalk, _TalkerName, m_goalTalk, fCoolTime, _bTalkAppend);
		}
	}

	public void StartFadeIn(float time)
	{
	}

	public void FadeOutBooking(float time)
	{
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

	public void ClearTalk()
	{
		m_lbTalk.text = "";
		m_lbTalkerName.text = "";
		m_lbTmpTalk.SetText("");
		m_lbTmpTalkerName.SetText("");
	}

	public void Close()
	{
		if (base.gameObject != null && base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void OnChange()
	{
		Close();
	}

	public bool UsingTMPText()
	{
		if (m_useTMPro)
		{
			return m_lbTmpTalk != null;
		}
		return false;
	}
}
