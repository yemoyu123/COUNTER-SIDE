using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUICutScenTitleMgr : MonoBehaviour
{
	private enum CUTSCEN_TITLE_PLAY_STATE
	{
		CTPS_SUB,
		CTPS_MAIN,
		CTPS_END
	}

	private static NKCUICutScenTitleMgr m_scNKCUICutScenTitleMgr;

	private NKCUITypeWriter m_NKCUITypeWriter = new NKCUITypeWriter();

	public Text m_lbTitle;

	public Text m_lbSubTitle;

	public TextMeshProUGUI m_lbTmpTitle;

	public TextMeshProUGUI m_lbTmpSubTitle;

	public CanvasGroup m_CanvasGroup;

	public bool m_useTMPro;

	private Sequence m_Seq;

	private string m_titleStr = "";

	private string m_subTitleStr = "";

	private float m_fTitleTypeCoolTime = 0.15f;

	private float m_fSubTitleTypeCoolTime = 0.15f;

	private bool m_bFinished = true;

	private CUTSCEN_TITLE_PLAY_STATE m_CUTSCEN_TITLE_PLAY_STATE;

	private bool m_bPause;

	private bool m_bTitleFadeOut;

	private float m_fTitleFadeOutTime;

	private bool m_bDidFadeOut;

	public void SetPause(bool bPause)
	{
		m_bPause = bPause;
		if (UsingTMPro())
		{
			NKCUtil.SetGameobjectActive(m_lbTmpTitle, !m_bPause);
			NKCUtil.SetGameobjectActive(m_lbTmpSubTitle, !m_bPause);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbTitle, !m_bPause);
			NKCUtil.SetGameobjectActive(m_lbSubTitle, !m_bPause);
		}
	}

	public static void InitUI(GameObject goNKM_UI_CUTSCEN_PLAYER)
	{
		if (!(m_scNKCUICutScenTitleMgr != null))
		{
			m_scNKCUICutScenTitleMgr = goNKM_UI_CUTSCEN_PLAYER.transform.Find("NKM_UI_CUTSCEN_TITLE_MGR").gameObject.GetComponent<NKCUICutScenTitleMgr>();
			m_scNKCUICutScenTitleMgr.Close();
			m_scNKCUICutScenTitleMgr.m_NKCUITypeWriter.SetTypingSound(bUse: true);
			m_scNKCUICutScenTitleMgr.m_NKCUITypeWriter.SetSpaceSound(bUse: false);
		}
	}

	public void Reset()
	{
		SetPause(bPause: false);
		m_CanvasGroup.alpha = 1f;
		m_bTitleFadeOut = false;
		m_fTitleFadeOutTime = 0f;
		m_bDidFadeOut = false;
		if (m_Seq != null && m_Seq.IsActive() && m_Seq.IsPlaying())
		{
			m_Seq.Kill();
		}
		m_Seq = null;
	}

	public void ForceClear()
	{
		if (m_Seq != null && m_Seq.IsActive() && m_Seq.IsPlaying())
		{
			m_Seq.Kill();
		}
		m_Seq = null;
		m_CanvasGroup.alpha = 0f;
	}

	public static NKCUICutScenTitleMgr GetCutScenTitleMgr()
	{
		return m_scNKCUICutScenTitleMgr;
	}

	private void Update()
	{
		if (m_bPause || (m_CUTSCEN_TITLE_PLAY_STATE != CUTSCEN_TITLE_PLAY_STATE.CTPS_SUB && m_CUTSCEN_TITLE_PLAY_STATE != CUTSCEN_TITLE_PLAY_STATE.CTPS_MAIN))
		{
			return;
		}
		m_NKCUITypeWriter.Update();
		if (m_CUTSCEN_TITLE_PLAY_STATE == CUTSCEN_TITLE_PLAY_STATE.CTPS_SUB)
		{
			if (m_NKCUITypeWriter.IsTyping())
			{
				return;
			}
			if (m_titleStr.Length > 0)
			{
				if (UsingTMPro())
				{
					m_NKCUITypeWriter.Start(m_lbTmpTitle, "", m_titleStr, m_fTitleTypeCoolTime, _bTalkAppend: false);
				}
				else
				{
					m_NKCUITypeWriter.Start(m_lbTitle, "", m_titleStr, m_fTitleTypeCoolTime, _bTalkAppend: false);
				}
				m_CUTSCEN_TITLE_PLAY_STATE++;
			}
			else
			{
				m_CUTSCEN_TITLE_PLAY_STATE = CUTSCEN_TITLE_PLAY_STATE.CTPS_END;
				m_bFinished = true;
			}
		}
		else if (m_CUTSCEN_TITLE_PLAY_STATE == CUTSCEN_TITLE_PLAY_STATE.CTPS_MAIN && !m_NKCUITypeWriter.IsTyping())
		{
			m_bFinished = true;
			m_CUTSCEN_TITLE_PLAY_STATE++;
		}
	}

	private bool CheckFadeOutCond()
	{
		return m_bTitleFadeOut;
	}

	private void DoFadeOut()
	{
		if (m_Seq != null && m_Seq.IsActive() && m_Seq.IsPlaying())
		{
			m_Seq.Kill();
		}
		m_Seq = DOTween.Sequence();
		m_Seq.AppendInterval(m_fTitleFadeOutTime / 2f);
		m_Seq.Append(m_CanvasGroup.DOFade(0f, m_fTitleFadeOutTime / 2f));
	}

	public void Finish()
	{
		m_bFinished = true;
		m_NKCUITypeWriter.Finish();
		m_CUTSCEN_TITLE_PLAY_STATE = CUTSCEN_TITLE_PLAY_STATE.CTPS_END;
	}

	public bool IsFinshed()
	{
		return m_bFinished;
	}

	public void Open(bool bTitleFadeOut, float fTitleFadeOutTime, string subTitle, string title, float fSubTitleTypeCoolTime = 0.15f, float fTitleTypeCoolTime = 0.15f)
	{
		m_bTitleFadeOut = bTitleFadeOut;
		m_fTitleFadeOutTime = fTitleFadeOutTime;
		m_bDidFadeOut = false;
		m_CanvasGroup.alpha = 1f;
		m_titleStr = title;
		m_subTitleStr = subTitle;
		m_fSubTitleTypeCoolTime = fSubTitleTypeCoolTime;
		m_fTitleTypeCoolTime = fTitleTypeCoolTime;
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		NKCUtil.SetGameobjectActive(m_lbTitle, !UsingTMPro());
		NKCUtil.SetGameobjectActive(m_lbSubTitle, !UsingTMPro());
		NKCUtil.SetGameobjectActive(m_lbTmpTitle, UsingTMPro());
		NKCUtil.SetGameobjectActive(m_lbTmpSubTitle, UsingTMPro());
		if (UsingTMPro())
		{
			m_lbTmpTitle.SetText("");
			m_lbTmpSubTitle.SetText("");
		}
		else
		{
			m_lbTitle.text = "";
			m_lbSubTitle.text = "";
		}
		m_bFinished = false;
		if (subTitle.Length > 0)
		{
			m_CUTSCEN_TITLE_PLAY_STATE = CUTSCEN_TITLE_PLAY_STATE.CTPS_SUB;
			if (UsingTMPro())
			{
				m_NKCUITypeWriter.Start(m_lbTmpSubTitle, "", subTitle, m_fSubTitleTypeCoolTime, _bTalkAppend: false);
			}
			else
			{
				m_NKCUITypeWriter.Start(m_lbSubTitle, "", subTitle, m_fSubTitleTypeCoolTime, _bTalkAppend: false);
			}
		}
		else if (title.Length > 0)
		{
			m_CUTSCEN_TITLE_PLAY_STATE = CUTSCEN_TITLE_PLAY_STATE.CTPS_MAIN;
			if (UsingTMPro())
			{
				m_NKCUITypeWriter.Start(m_lbTmpTitle, "", m_titleStr, m_fTitleTypeCoolTime, _bTalkAppend: false);
			}
			else
			{
				m_NKCUITypeWriter.Start(m_lbTitle, "", m_titleStr, m_fTitleTypeCoolTime, _bTalkAppend: false);
			}
		}
		else
		{
			m_CUTSCEN_TITLE_PLAY_STATE = CUTSCEN_TITLE_PLAY_STATE.CTPS_END;
			m_bFinished = true;
		}
	}

	public void Close()
	{
		if (CheckFadeOutCond())
		{
			if (!m_bDidFadeOut)
			{
				DoFadeOut();
			}
			m_bDidFadeOut = true;
		}
		else
		{
			if (m_Seq != null && m_Seq.IsActive() && m_Seq.IsPlaying())
			{
				m_Seq.Kill();
			}
			m_Seq = null;
			m_CanvasGroup.alpha = 0f;
		}
		Finish();
	}

	private bool UsingTMPro()
	{
		if (m_useTMPro && m_lbTmpSubTitle != null)
		{
			return m_lbTmpTitle != null;
		}
		return false;
	}
}
