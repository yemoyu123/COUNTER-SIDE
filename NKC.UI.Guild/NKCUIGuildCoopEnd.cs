using ClientPacket.Guild;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildCoopEnd : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONSORTIUM_COOP_END";

	private static NKCUIGuildCoopEnd m_Instance;

	public Text m_lbTitle;

	public Text m_lbSubTitle;

	public Text m_lbRemainTime;

	public NKCUIComStateButton m_btnSeasonReward;

	public GameObject m_objSeasonRewardRedDot;

	private bool m_bIsSeasonEnd = true;

	private float m_fDeltaTime;

	private bool m_bDataRequested;

	public static NKCUIGuildCoopEnd Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIGuildCoopEnd>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_POPUP_CONSORTIUM_COOP_END", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), null).GetInstance<NKCUIGuildCoopEnd>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "";

	public override string GuideTempletID => "ARTICLE_GUILD_DUNGEON";

	private void InitUI()
	{
		m_btnSeasonReward.PointerClick.RemoveAllListeners();
		m_btnSeasonReward.PointerClick.AddListener(OnClickSeasonReward);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
	}

	public void Open()
	{
		m_bIsSeasonEnd = NKCGuildCoopManager.m_GuildDungeonState == GuildDungeonState.SeasonOut;
		m_fDeltaTime = 0f;
		if (m_bIsSeasonEnd)
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_POPUP_CONSORTIUM_COOP_END_SUB_02_TEXT);
			NKCUtil.SetLabelText(m_lbSubTitle, NKCUtilString.GET_STRING_POPUP_CONSORTIUM_COOP_END_SUB_01_TEXT);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_POPUP_CONSORTIUM_COOP_SESSION_END_SUB_02_TEXT);
			NKCUtil.SetLabelText(m_lbSubTitle, NKCUtilString.GET_STRING_POPUP_CONSORTIUM_COOP_SESSION_END_SUB_01_TEXT);
		}
		RefreshSeasonRewardRedDot();
		SetRemainTime();
		UIOpened();
	}

	private void SetRemainTime()
	{
		if (m_bIsSeasonEnd)
		{
			NKCUtil.SetLabelText(m_lbRemainTime, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_SEASION_END_INFORMATION_TEXT, NKCUtilString.GetRemainTimeString(NKCGuildCoopManager.m_NextSessionStartDateUTC, 2)));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRemainTime, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_SESSION_END_INFORMATION_TEXT, NKCUtilString.GetRemainTimeString(NKCGuildCoopManager.m_NextSessionStartDateUTC, 2)));
		}
	}

	private void OnClickSeasonReward()
	{
		NKCPopupGuildCoopSeasonReward.Instance.Open(RefreshSeasonRewardRedDot);
	}

	private void RefreshSeasonRewardRedDot()
	{
		NKCUtil.SetGameobjectActive(m_objSeasonRewardRedDot, NKCGuildCoopManager.CheckSeasonRewardEnable());
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > 1f)
		{
			m_fDeltaTime -= 1f;
			SetRemainTime();
			if (NKCSynchronizedTime.IsFinished(NKCGuildCoopManager.m_NextSessionStartDateUTC) && !m_bDataRequested)
			{
				m_bDataRequested = true;
				NKCGuildCoopManager.ResetGuildCoopState();
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
			}
		}
	}
}
