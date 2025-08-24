using System.Collections;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace NKC.UI;

public class NKCUIOperationIntro : NKCUIBase
{
	public delegate void NKCUIOICallBack();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_operation";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATION_INTRO";

	private static NKCUIOperationIntro m_Instance;

	private NKCUIOICallBack m_NKCUIOICallBack;

	public Text m_NKM_UI_OPERATION_INTRO_EPISODE_COUNT;

	public Text m_NKM_UI_OPERATION_INTRO_EPISODE_TITLE;

	private Coroutine m_coIntro;

	private bool m_bWaitingMovie;

	public static NKCUIOperationIntro Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOperationIntro>("ab_ui_nkm_ui_operation", "NKM_UI_OPERATION_INTRO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIOperationIntro>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => NKCUtilString.GET_STRING_MENU_NAME_OPERATION_INTRO;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

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
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private IEnumerator OperationIntroUIOpenProcess()
	{
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
			subUICameraVideoPlayer.m_fMoviePlaySpeed = 1f;
			m_bWaitingMovie = true;
			subUICameraVideoPlayer.Play("MainStream_Transition.mp4", bLoop: false, bPlaySound: true, VideoPlayMessageCallback);
			while (m_bWaitingMovie)
			{
				yield return null;
				if (Input.anyKeyDown && PlayerPrefs.GetInt("OPERATION_INTRO_SKIP", 0) == 1)
				{
					break;
				}
			}
			if (PlayerPrefs.GetInt("OPERATION_INTRO_SKIP", 0) == 0)
			{
				PlayerPrefs.SetInt("OPERATION_INTRO_SKIP", 1);
			}
		}
		if (m_NKCUIOICallBack != null)
		{
			m_NKCUIOICallBack();
		}
		m_NKCUIOICallBack = null;
		m_bWaitingMovie = false;
		m_coIntro = null;
	}

	public void Open(NKMStageTempletV2 stageTemplet, NKCUIOICallBack _NKCUIOICallBack)
	{
		if (stageTemplet == null)
		{
			return;
		}
		m_NKCUIOICallBack = _NKCUIOICallBack;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_bWaitingMovie = false;
		NKMEpisodeTempletV2 episodeTemplet = stageTemplet.EpisodeTemplet;
		if (episodeTemplet != null)
		{
			string text;
			if (stageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE)
			{
				text = episodeTemplet.GetEpisodeTitle() + " " + string.Format(NKCUtilString.GET_STRING_EP_TRAINING_NUMBER, stageTemplet.m_StageUINum);
			}
			else
			{
				bool flag = false;
				if (stageTemplet.m_STAGE_TYPE == STAGE_TYPE.ST_DUNGEON)
				{
					NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(stageTemplet.m_StageBattleStrID);
					if (dungeonTempletBase != null && dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
					{
						flag = true;
					}
				}
				text = ((!flag) ? (episodeTemplet.GetEpisodeTitle() + " " + stageTemplet.ActId + "-" + stageTemplet.m_StageUINum) : (episodeTemplet.GetEpisodeTitle() + " " + string.Format(NKCUtilString.GET_STRING_EP_CUTSCEN_NUMBER, stageTemplet.m_StageUINum)));
			}
			text = text.Replace("EP", "EPISODE");
			m_NKM_UI_OPERATION_INTRO_EPISODE_COUNT.text = text;
			switch (stageTemplet.m_STAGE_TYPE)
			{
			case STAGE_TYPE.ST_WARFARE:
			{
				NKMWarfareTemplet warfareTemplet = stageTemplet.WarfareTemplet;
				if (warfareTemplet != null)
				{
					m_NKM_UI_OPERATION_INTRO_EPISODE_TITLE.text = warfareTemplet.GetWarfareName();
				}
				else
				{
					m_NKM_UI_OPERATION_INTRO_EPISODE_TITLE.text = "";
				}
				break;
			}
			case STAGE_TYPE.ST_PHASE:
			{
				NKMPhaseTemplet phaseTemplet = stageTemplet.PhaseTemplet;
				if (phaseTemplet != null)
				{
					m_NKM_UI_OPERATION_INTRO_EPISODE_TITLE.text = phaseTemplet.GetName();
				}
				else
				{
					m_NKM_UI_OPERATION_INTRO_EPISODE_TITLE.text = "";
				}
				break;
			}
			default:
			{
				NKMDungeonTempletBase dungeonTempletBase2 = stageTemplet.DungeonTempletBase;
				if (dungeonTempletBase2 != null)
				{
					m_NKM_UI_OPERATION_INTRO_EPISODE_TITLE.text = dungeonTempletBase2.GetDungeonName();
				}
				else
				{
					m_NKM_UI_OPERATION_INTRO_EPISODE_TITLE.text = "";
				}
				break;
			}
			}
		}
		else
		{
			m_NKM_UI_OPERATION_INTRO_EPISODE_COUNT.text = "";
			m_NKM_UI_OPERATION_INTRO_EPISODE_TITLE.text = "";
		}
		UIOpened();
		m_coIntro = StartCoroutine(OperationIntroUIOpenProcess());
	}

	private void VideoPlayMessageCallback(NKCUIComVideoPlayer.eVideoMessage message)
	{
		switch (message)
		{
		case NKCUIComVideoPlayer.eVideoMessage.PlayFailed:
		case NKCUIComVideoPlayer.eVideoMessage.PlayComplete:
			m_bWaitingMovie = false;
			break;
		case NKCUIComVideoPlayer.eVideoMessage.PlayBegin:
			break;
		}
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		if (m_coIntro != null)
		{
			StopCoroutine(m_coIntro);
		}
		m_coIntro = null;
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
		m_NKCUIOICallBack = null;
	}

	public override void OnBackButton()
	{
		if (m_NKCUIOICallBack != null)
		{
			m_NKCUIOICallBack();
		}
		m_NKCUIOICallBack = null;
	}
}
