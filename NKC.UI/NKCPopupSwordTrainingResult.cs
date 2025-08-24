using NKC.UI.Component;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupSwordTrainingResult : NKCUIBase
{
	public delegate void OnClick();

	private const string ASSET_BUNDLE_NAME = "UI_SINGLE_SWORDTRAINING";

	private const string UI_ASSET_NAME = "UI_SINGLE_POPUP_SWORDTRAINING_RESULT";

	private static NKCPopupSwordTrainingResult m_Instance;

	public GameObject m_objNewScore;

	public NKCComTMPUIText m_lbScore;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnRestart;

	public NKCComTMPUIText m_lbUserName;

	public NKCComTMPUIText m_lbUserUID;

	private OnClick m_dOnClickClose;

	private OnClick m_dOnClickRestart;

	public static NKCPopupSwordTrainingResult Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupSwordTrainingResult>("UI_SINGLE_SWORDTRAINING", "UI_SINGLE_POPUP_SWORDTRAINING_RESULT", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupSwordTrainingResult>();
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

	public void Open(int bestScore, int score, OnClick onClickRestart, OnClick onClickClose)
	{
		m_dOnClickRestart = onClickRestart;
		m_dOnClickClose = onClickClose;
		NKCUtil.SetGameobjectActive(m_objNewScore, bestScore < score);
		NKCUtil.SetLabelText(m_lbScore, score.ToString());
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKCUtil.SetLabelText(m_lbUserName, nKMUserData.m_UserNickName);
		NKCUtil.SetLabelText(m_lbUserUID, NKCUtilString.GetFriendCode(nKMUserData.m_FriendCode));
		UIOpened();
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
