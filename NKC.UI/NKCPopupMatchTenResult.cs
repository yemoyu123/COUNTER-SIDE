using NKC.UI.Component;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupMatchTenResult : NKCUIBase
{
	public delegate void OnClick();

	private const string ASSET_BUNDLE_NAME = "UI_SINGLE_MATCHTEN";

	private const string UI_ASSET_NAME = "UI_SINGLE_POPUP_MATCHTEN_RESULT";

	private static NKCPopupMatchTenResult m_Instance;

	public Image m_img;

	public GameObject m_objNewScore;

	public NKCComTMPUIText m_lbScore;

	public GameObject m_objNewRemainTime;

	public NKCComTMPUIText m_lbRemainTime;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnRestart;

	public Sprite m_sprHighScore;

	public Sprite m_sprMidScore;

	public Sprite m_sprLowScore;

	private OnClick m_dOnClickClose;

	private OnClick m_dOnClickRestart;

	public static NKCPopupMatchTenResult Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupMatchTenResult>("UI_SINGLE_MATCHTEN", "UI_SINGLE_POPUP_MATCHTEN_RESULT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupMatchTenResult>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_btnClose, OnClickClose);
		NKCUtil.SetButtonClickDelegate(m_btnRestart, OnClickRestart);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		OnClickClose();
	}

	public void Open(int bestScore, int bestRemainTime, int score, int remainTime, OnClick onClickRestart, OnClick onClickClose)
	{
		m_dOnClickRestart = onClickRestart;
		m_dOnClickClose = onClickClose;
		NKCUtil.SetGameobjectActive(m_objNewScore, bestScore < score);
		NKCUtil.SetGameobjectActive(m_objNewRemainTime, bestRemainTime < remainTime);
		NKCUtil.SetLabelText(m_lbScore, score.ToString());
		NKCUtil.SetLabelText(m_lbRemainTime, ((float)remainTime / 100f).ToString("F2"));
		NKMMatchTenTemplet nKMMatchTenTemplet = NKMMatchTenTemplet.Find(NKCMatchTenManager.GetTempletId());
		if (nKMMatchTenTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		SetBg(nKMMatchTenTemplet, score);
		UIOpened();
	}

	private void SetBg(NKMMatchTenTemplet templet, int score)
	{
		if (templet.m_PlayScoreHigh > 0 && score >= templet.m_PlayScoreHigh)
		{
			NKCUtil.SetImageSprite(m_img, m_sprHighScore);
		}
		else if (templet.m_PlayScoreMid > 0 && score >= templet.m_PlayScoreMid)
		{
			NKCUtil.SetImageSprite(m_img, m_sprMidScore);
		}
		else
		{
			NKCUtil.SetImageSprite(m_img, m_sprLowScore);
		}
	}

	private void OnClickRestart()
	{
		Close();
		m_dOnClickRestart();
	}

	private void OnClickClose()
	{
		Close();
		m_dOnClickClose();
	}
}
