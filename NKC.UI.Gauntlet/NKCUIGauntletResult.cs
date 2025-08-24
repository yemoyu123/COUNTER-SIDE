using NKC.UI.Result;
using NKM;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletResult : NKCUIBase
{
	public delegate void OnClose();

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_RESULT";

	private OnClose dOnClose;

	private bool m_bInit;

	public Animator m_Animator;

	public Text m_lbScore;

	public Text m_lbAddScore;

	public NKCUILeagueTier m_NKCUILeagueTier;

	public NKCUILeagueTier m_NKCUILeagueTier_After;

	public Text m_lbPromoteTier;

	public GameObject m_objDemoteAlert;

	[Header("Fallback BG")]
	public GameObject m_objBGFallBack;

	private static NKCUIResult.BattleResultData m_BattleResultData;

	private const float DEFAULT_ANI_WAIT_TIME = 8f;

	private float m_fDefaultAniTime;

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public static void SetResultData(NKCUIResult.BattleResultData _BattleResultData)
	{
		m_BattleResultData = _BattleResultData;
	}

	public static NKCAssetResourceData OpenInstanceAsync()
	{
		return NKCUIBase.OpenInstanceAsync<NKCUIBaseSceneMenu>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_RESULT");
	}

	public static bool CheckInstanceLoaded(NKCAssetResourceData loadResourceData, out NKCUIGauntletResult retVal)
	{
		return NKCUIBase.CheckInstanceLoaded<NKCUIGauntletResult>(loadResourceData, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), out retVal);
	}

	public void CloseInstance()
	{
		NKCAssetResourceManager.CloseResource("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_RESULT");
		Object.Destroy(base.gameObject);
	}

	public void InitUI()
	{
		if (!m_bInit)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			m_bInit = true;
		}
	}

	public void Open(OnClose _dOnClose)
	{
		if (m_BattleResultData == null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		dOnClose = _dOnClose;
		UIOpened();
		if (!SetUI())
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		}
		else
		{
			NKCCamera.EnableBloom(bEnable: false);
		}
	}

	private bool CheckTierDiff()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		PvpState pvPData = GetPvPData(myUserData, m_BattleResultData.m_NKM_GAME_TYPE);
		if (pvPData == null)
		{
			return false;
		}
		if (NKCPVPManager.GetFinalTier(pvPData.LeagueTierID) != NKCPVPManager.GetFinalTier(m_BattleResultData.m_OrgPVPTier))
		{
			return true;
		}
		return false;
	}

	private bool SetUI()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		if (NKCPrivatePVPRoomMgr.LobbyData != null || NKCPrivatePVPRoomMgr.IsObserver(myUserData.m_UserUID))
		{
			Close();
			return true;
		}
		if (m_BattleResultData != null)
		{
			PvpState pvPData = GetPvPData(myUserData, m_BattleResultData.m_NKM_GAME_TYPE);
			if (pvPData != null)
			{
				NKCUtil.SetLabelText(m_lbScore, pvPData.Score.ToString());
				int num = pvPData.Score - m_BattleResultData.m_OrgPVPScore;
				if (num > 0)
				{
					NKCUtil.SetLabelText(m_lbAddScore, "+" + num);
				}
				else
				{
					NKCUtil.SetLabelText(m_lbAddScore, num.ToString());
				}
				int num2 = NKCPVPManager.FindPvPSeasonID(m_BattleResultData.m_NKM_GAME_TYPE, NKCSynchronizedTime.GetServerUTCTime());
				if (num2 == 0)
				{
					Close();
					return true;
				}
				m_NKCUILeagueTier_After.SetUI(NKCPVPManager.GetTierIconByTier(m_BattleResultData.m_NKM_GAME_TYPE, num2, pvPData.LeagueTierID), NKCPVPManager.GetTierNumberByTier(m_BattleResultData.m_NKM_GAME_TYPE, num2, pvPData.LeagueTierID));
				NKCUtil.SetLabelText(m_lbPromoteTier, NKCPVPManager.GetLeagueNameByTier(m_BattleResultData.m_NKM_GAME_TYPE, num2, pvPData.LeagueTierID));
				m_NKCUILeagueTier.SetUI(NKCPVPManager.GetTierIconByTier(m_BattleResultData.m_NKM_GAME_TYPE, num2, m_BattleResultData.m_OrgPVPTier), NKCPVPManager.GetTierNumberByTier(m_BattleResultData.m_NKM_GAME_TYPE, num2, m_BattleResultData.m_OrgPVPTier));
			}
			string text = "";
			m_fDefaultAniTime = 8f;
			if (m_BattleResultData.m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_WIN)
			{
				text = ((NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_REPLAY) && NKCReplayMgr.IsPlayingReplay()) ? "RESULT_REPLAY_WIN" : ((m_BattleResultData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_PRIVATE || m_BattleResultData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT) ? "RESULT_REPLAY_WIN" : ((!CheckTierDiff()) ? "RESULT_WIN" : "RESULT_PROMOTION")));
				SetBG();
			}
			else if (m_BattleResultData.m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_LOSE)
			{
				if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_REPLAY) && NKCReplayMgr.IsPlayingReplay())
				{
					text = "RESULT_REPLAY_LOSE";
				}
				else if (m_BattleResultData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_PRIVATE || m_BattleResultData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVP_EVENT)
				{
					text = "RESULT_REPLAY_LOSE";
				}
				else if (CheckTierDiff())
				{
					text = "RESULT_DEMOTE";
				}
				else
				{
					text = "RESULT_LOSE";
					NKCUtil.SetGameobjectActive(m_objDemoteAlert, IsPVPDemotionAlert(pvPData, m_BattleResultData.m_NKM_GAME_TYPE));
				}
			}
			else if (m_BattleResultData.m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_DRAW)
			{
				text = ((NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_REPLAY) && NKCReplayMgr.IsPlayingReplay()) ? "RESULT_REPLAY_DRAW" : ((m_BattleResultData.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_PVP_PRIVATE && m_BattleResultData.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_PVP_EVENT) ? "RESULT_DRAW" : "RESULT_REPLAY_DRAW"));
			}
			else
			{
				Debug.LogWarning("Gauntlet Unknown Result !!");
			}
			if (m_Animator != null && !string.IsNullOrEmpty(text))
			{
				m_Animator.SetTrigger(text);
			}
		}
		return true;
	}

	private void SetBG()
	{
		bool flag = NKCScenManager.GetScenManager().GetGameOptionData()?.UseVideoTexture ?? false;
		NKCUtil.SetGameobjectActive(m_objBGFallBack, !flag);
		if (flag)
		{
			NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
			if (subUICameraVideoPlayer != null)
			{
				subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
				subUICameraVideoPlayer.m_fMoviePlaySpeed = 1f;
				subUICameraVideoPlayer.Play("Gauntlet_BG.mp4", bLoop: true);
			}
		}
	}

	private void Update()
	{
		if (!base.IsOpen)
		{
			return;
		}
		if (m_Animator == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			dOnClose = null;
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_SERVER_GAME_DATA_AND_GO_LOBBY, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			Debug.Log("NKCUIGauntletResult.Update : m_Animator is null");
			return;
		}
		if (m_fDefaultAniTime > 0f)
		{
			m_fDefaultAniTime -= Time.deltaTime;
		}
		if (m_fDefaultAniTime <= 0f)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			dOnClose = null;
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCUtilString.GET_STRING_ERROR_SERVER_GAME_DATA_AND_GO_LOBBY, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
			Debug.Log("NKCUIGauntletResult.Update : Default Ani Time played");
		}
		else if (!(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))
		{
			Close();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
		if (dOnClose != null)
		{
			dOnClose();
		}
		m_BattleResultData = null;
	}

	public override void OnBackButton()
	{
	}

	private PvpState GetPvPData(NKMUserData userData, NKM_GAME_TYPE gameType)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
			return userData.m_LeagueData;
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			return userData.m_AsyncData;
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
			return userData.m_eventPvpData;
		default:
			return userData.m_PvpData;
		}
	}

	private bool IsPVPDemotionAlert(PvpState pvpData, NKM_GAME_TYPE gameType)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
			return NKCUtil.IsPVPDemotionAlert(gameType, pvpData);
		case NKM_GAME_TYPE.NGT_PVP_RANK:
			return NKCUtil.IsPVPDemotionAlert(gameType, pvpData);
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
			return false;
		default:
			return false;
		}
	}
}
