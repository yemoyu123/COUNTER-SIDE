using ClientPacket.Event;
using NKC.UI.Component;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.UI.Event;

public class NKCPopupEventRaceBetFinish : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "UI_SINGLE_RACE";

	private const string UI_ASSET_NAME = "UI_POPUP_SINGLE_RACE_FINISH";

	private static NKCPopupEventRaceBetFinish m_Instance;

	public GameObject m_ObjTeamBlue;

	public GameObject m_ObjTeamRed;

	public NKCComTMPUIText m_lbBetCount;

	public NKCUIComStateButton m_csbtnClose;

	private UnityAction m_OnCallBack;

	public static NKCPopupEventRaceBetFinish Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventRaceBetFinish>("UI_SINGLE_RACE", "UI_POPUP_SINGLE_RACE_FINISH", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventRaceBetFinish>();
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

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		NKCUtil.SetBindFunction(m_csbtnClose, CloseInternal);
	}

	public void Open(EventBetTeam selectTeam, int BetCnt, UnityAction callBack)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_ObjTeamBlue, selectTeam == EventBetTeam.TeamB);
		NKCUtil.SetGameobjectActive(m_ObjTeamRed, selectTeam == EventBetTeam.TeamA);
		NKCUtil.SetLabelText(m_lbBetCount, string.Format(NKCUtilString.GET_STRING_COUNTING_ONE_PARAM, BetCnt.ToString()));
		m_OnCallBack = callBack;
	}

	public override void OnBackButton()
	{
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_OnCallBack?.Invoke();
	}
}
