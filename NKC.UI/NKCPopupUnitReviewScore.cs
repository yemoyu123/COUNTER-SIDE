using ClientPacket.Community;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupUnitReviewScore : NKCUIBase
{
	public delegate void OnVoteScore(int votedScore);

	private const int MAX_VOTED_COUNT = 9999;

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_review";

	private const string UI_ASSET_NAME = "NKM_UI_UNIT_REVIEW_POPUP_AVG_SCORE_UPLOAD";

	private static NKCPopupUnitReviewScore m_Instance;

	public Text m_lbTitle;

	public NKCUIComToggle m_tglScore_1;

	public NKCUIComToggle m_tglScore_2;

	public NKCUIComToggle m_tglScore_3;

	public NKCUIComToggle m_tglScore_4;

	public NKCUIComToggle m_tglScore_5;

	public Text m_lbCount;

	public NKCUIComStateButton m_btnOk;

	public NKCUIComStateButton m_btnCancel;

	public NKCUIComStateButton m_btnClose;

	private OnVoteScore dOnVoteScore;

	private bool m_bInitComplete;

	private int m_unitID;

	private int m_nLastVotedScore;

	private int m_selectedScore;

	private NKCUIOpenAnimator m_openAni;

	public static NKCPopupUnitReviewScore Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupUnitReviewScore>("ab_ui_nkm_ui_unit_review", "NKM_UI_UNIT_REVIEW_POPUP_AVG_SCORE_UPLOAD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupUnitReviewScore>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_UNIT_REVIEW_SCORE;

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

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void InitUI()
	{
		m_tglScore_1.OnValueChanged.RemoveAllListeners();
		m_tglScore_1.OnValueChanged.AddListener(OnClickScore_1);
		m_tglScore_2.OnValueChanged.RemoveAllListeners();
		m_tglScore_2.OnValueChanged.AddListener(OnClickScore_2);
		m_tglScore_3.OnValueChanged.RemoveAllListeners();
		m_tglScore_3.OnValueChanged.AddListener(OnClickScore_3);
		m_tglScore_4.OnValueChanged.RemoveAllListeners();
		m_tglScore_4.OnValueChanged.AddListener(OnClickScore_4);
		m_tglScore_5.OnValueChanged.RemoveAllListeners();
		m_tglScore_5.OnValueChanged.AddListener(OnClickScore_5);
		m_btnOk.PointerClick.RemoveAllListeners();
		m_btnOk.PointerClick.AddListener(OnClickOk);
		NKCUtil.SetHotkey(m_btnOk, HotkeyEventType.Confirm);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(OnClickClose);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(OnClickClose);
		m_openAni = new NKCUIOpenAnimator(base.gameObject);
		m_bInitComplete = true;
	}

	public void OpenUI(NKMUnitReviewScoreData scoreData, OnVoteScore onVoteScore)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		if (m_openAni != null)
		{
			m_openAni.PlayOpenAni();
		}
		if (scoreData == null)
		{
			scoreData = new NKMUnitReviewScoreData();
		}
		if (onVoteScore != null)
		{
			dOnVoteScore = onVoteScore;
		}
		m_nLastVotedScore = scoreData.myScore;
		NKCUtil.SetLabelText(m_lbTitle, MenuName);
		string msg = ((scoreData.votedCount > 9999) ? string.Format(NKCUtilString.GET_STRING_UNIT_REVIEW_SCORE_VOTE_PLUS_ONE_PARAM, scoreData.votedCount) : string.Format(NKCUtilString.GET_STRING_UNIT_REVIEW_SCORE_VOTE_ONE_PARAM, scoreData.votedCount));
		NKCUtil.SetLabelText(m_lbCount, msg);
		switch (m_nLastVotedScore)
		{
		default:
			OnClickScore_1(bSelect: true);
			break;
		case 2:
			OnClickScore_2(bSelect: true);
			break;
		case 3:
			OnClickScore_3(bSelect: true);
			break;
		case 4:
			OnClickScore_4(bSelect: true);
			break;
		case 5:
			OnClickScore_5(bSelect: true);
			break;
		}
		UIOpened();
	}

	private void OnClickScore_1(bool bSelect)
	{
		if (bSelect)
		{
			m_selectedScore = 1;
		}
		m_tglScore_1.Select(bSelect, bForce: true, bImmediate: true);
	}

	private void OnClickScore_2(bool bSelect)
	{
		if (bSelect)
		{
			m_selectedScore = 2;
		}
		m_tglScore_2.Select(bSelect, bForce: true, bImmediate: true);
	}

	private void OnClickScore_3(bool bSelect)
	{
		if (bSelect)
		{
			m_selectedScore = 3;
		}
		m_tglScore_3.Select(bSelect, bForce: true, bImmediate: true);
	}

	private void OnClickScore_4(bool bSelect)
	{
		if (bSelect)
		{
			m_selectedScore = 4;
		}
		m_tglScore_4.Select(bSelect, bForce: true, bImmediate: true);
	}

	private void OnClickScore_5(bool bSelect)
	{
		if (bSelect)
		{
			m_selectedScore = 5;
		}
		m_tglScore_5.Select(bSelect, bForce: true, bImmediate: true);
	}

	private void OnClickOk()
	{
		if (m_nLastVotedScore == m_selectedScore)
		{
			Close();
			return;
		}
		if (dOnVoteScore != null)
		{
			dOnVoteScore(m_selectedScore);
		}
		Close();
	}

	private void OnClickClose()
	{
		Close();
	}

	private void Update()
	{
		if (base.IsOpen && m_openAni != null)
		{
			m_openAni.Update();
		}
	}
}
