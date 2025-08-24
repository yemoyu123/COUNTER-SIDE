using System.Collections;
using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace NKC.UI;

public class NKCUIPopupRearmamentResult : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_rearm";

	private const string UI_ASSET_NAME = "AB_UI_REARM_RESULT";

	private static NKCUIPopupRearmamentResult m_Instance;

	public NKCUICharInfoSummary m_CharSummary;

	public NKCUICharacterView m_CharView;

	public Animator m_Ani;

	public Image m_imgRank;

	public Text m_lbUnitName;

	public Text m_lbLeftUnitName;

	public List<GameObject> m_lstHideObject = new List<GameObject>();

	[Header("리더스킬")]
	public Image m_imgLeaderIcon;

	public Text m_lbLeaderSkillLv;

	public Text m_lbLeaderSkillName;

	[Header("스킬")]
	public List<strResultSkillInfo> m_lstSkillResult;

	[Header("재무장 확인")]
	public NKCUIComStateButton m_csbtnOK;

	private NKMUnitData m_ResultUnitData;

	private NKCUIComVideoCamera m_VideoPlayer;

	[Header("연출 영상 재생 속도")]
	public float MOVIE_PLAY_SPEED = 1.5f;

	[Header("재무장 연출 동영상")]
	public string REARM_MOVIE = "Rearmament_Intro.mp4";

	[Header("재무장 연출 음악")]
	public string REARM_INTRO_SOUND = "";

	private int m_introSoundUID;

	public float fAniDisplayedTime = 10f;

	public static NKCUIPopupRearmamentResult Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupRearmamentResult>("ab_ui_rearm", "AB_UI_REARM_RESULT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPopupRearmamentResult>();
				m_Instance?.Init();
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

	public override string MenuName => NKCUtilString.GET_STRING_REARM_CONFIRM_POPUP_TITLE;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		m_VideoPlayer = null;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnOK, CloseAfterOpenUnitInfo);
	}

	public void Open(NKMUnitData resultUnitData)
	{
		if (resultUnitData == null)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(resultUnitData.m_UnitID);
		if (unitTempletBase == null)
		{
			return;
		}
		SetResultUI(bActive: false);
		int num = (NKCRearmamentUtil.GetRearmamentTemplet(resultUnitData.m_UnitID).RearmGrade + 1) * 5;
		int num2 = 0;
		List<NKMUnitSkillTemplet> unitAllSkillTempletList = NKMUnitSkillManager.GetUnitAllSkillTempletList(resultUnitData);
		for (int i = 0; i < unitAllSkillTempletList.Count; i++)
		{
			NKMUnitSkillTemplet nKMUnitSkillTemplet = unitAllSkillTempletList[i];
			if (nKMUnitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_LEADER)
			{
				NKCUtil.SetImageSprite(m_imgLeaderIcon, NKCUtil.GetSkillIconSprite(nKMUnitSkillTemplet));
				NKCUtil.SetLabelText(m_lbLeaderSkillName, nKMUnitSkillTemplet.GetSkillName());
				NKCUtil.SetLabelText(m_lbLeaderSkillLv, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKMUnitSkillTemplet.m_Level));
				continue;
			}
			NKCUtil.SetImageSprite(m_lstSkillResult[num2].imgIcon, NKCUtil.GetSkillIconSprite(nKMUnitSkillTemplet));
			NKCUtil.SetLabelText(m_lstSkillResult[num2].lbName, nKMUnitSkillTemplet.GetSkillName());
			string msg = string.Format(NKCUtilString.GET_STRING_REARM_RESULT_POPUP_SKILL_LEVEL_BEFORE, nKMUnitSkillTemplet.m_Level, num - 5);
			NKCUtil.SetLabelText(m_lstSkillResult[num2].lbBeforeLvInfo, msg);
			string msg2 = string.Format(NKCUtilString.GET_STRING_REARM_RESULT_POPUP_SKILL_LEVEL_AFTER, nKMUnitSkillTemplet.m_Level, num);
			NKCUtil.SetLabelText(m_lstSkillResult[num2].lbAfterLvInfo, msg2);
			num2++;
		}
		NKCUtil.SetLabelText(m_lbUnitName, unitTempletBase.GetUnitTitle());
		NKCUtil.SetLabelText(m_lbLeftUnitName, unitTempletBase.GetUnitName());
		NKCUtil.SetImageSprite(m_imgRank, NKCUtil.GetSpriteUnitGrade(unitTempletBase.m_NKM_UNIT_GRADE));
		m_CharView.CloseCharacterIllust();
		m_CharView.SetCharacterIllust(unitTempletBase);
		m_CharSummary.SetData(resultUnitData);
		m_ResultUnitData = resultUnitData;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		IntroMovie();
		UIOpened();
	}

	private void SetResultUI(bool bActive)
	{
		foreach (GameObject item in m_lstHideObject)
		{
			NKCUtil.SetGameobjectActive(item, bActive);
		}
	}

	private void IntroMovie()
	{
		m_VideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (m_VideoPlayer != null)
		{
			m_VideoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
			m_VideoPlayer.m_fMoviePlaySpeed = MOVIE_PLAY_SPEED;
			NKCSoundManager.StopMusic();
			m_VideoPlayer.Play(REARM_MOVIE, bLoop: false, bPlaySound: true, VideoPlayMessageCallback, bForcePlay: true);
			if (!string.IsNullOrEmpty(REARM_INTRO_SOUND))
			{
				m_introSoundUID = NKCSoundManager.PlaySound(REARM_INTRO_SOUND, 1f, 0f, 0f);
			}
		}
	}

	private void VideoPlayMessageCallback(NKCUIComVideoPlayer.eVideoMessage message)
	{
		switch (message)
		{
		case NKCUIComVideoPlayer.eVideoMessage.PlayFailed:
		case NKCUIComVideoPlayer.eVideoMessage.PlayComplete:
			NKCSoundManager.StopSound(m_introSoundUID);
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OFFICE)
			{
				NKCSoundManager.PlayScenMusic(NKM_SCEN_ID.NSI_OFFICE);
			}
			else
			{
				NKCSoundManager.PlayScenMusic(NKM_SCEN_ID.NSI_HOME);
			}
			m_VideoPlayer?.Stop();
			m_introSoundUID = 0;
			SetResultUI(bActive: true);
			PlayAni();
			break;
		case NKCUIComVideoPlayer.eVideoMessage.PlayBegin:
			break;
		}
	}

	private void PlayAni()
	{
		m_Ani.SetTrigger("INTRO");
		StartCoroutine(OpenUnitInfo(fAniDisplayedTime));
	}

	private IEnumerator OpenUnitInfo(float fDelay)
	{
		yield return new WaitForSeconds(fDelay);
		CloseAfterOpenUnitInfo();
	}

	public void CloseAfterOpenUnitInfo()
	{
		Close();
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_UNIT_LIST && NKCUIRearmament.IsInstanceOpen)
		{
			NKCUIRearmament.Instance.Close();
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OFFICE && NKCUIRearmament.IsInstanceOpen)
		{
			NKCUIRearmament.Instance.Open();
		}
		NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(new List<long> { m_ResultUnitData.m_UnitUID });
		NKCUIUnitInfo.Instance.Open(m_ResultUnitData, null, openOption);
	}
}
