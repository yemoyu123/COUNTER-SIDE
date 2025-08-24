using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using Cs.Logging;
using NKC.Publisher;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace NKC.UI;

public class NKCUICutScenPlayer : NKCUIBase, IScrollHandler, IEventSystemHandler
{
	public delegate void CutScenCallBack();

	public delegate void OnVoice(string voiceFileName);

	public struct CutsceneLog
	{
		public string text;

		public string voice;

		public CutsceneLog(string text, string voice = null)
		{
			this.text = text;
			if (!string.IsNullOrEmpty(voice))
			{
				this.voice = voice;
			}
			else
			{
				this.voice = null;
			}
		}

		public void SetVoice(string voiceFileName)
		{
			if (!string.IsNullOrEmpty(voiceFileName))
			{
				voice = voiceFileName;
			}
		}
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_cutscen";

	private const string UI_ASSET_NAME = "NKM_UI_CUTSCEN_PLAYER";

	private static NKCUICutScenPlayer m_Instance;

	private const string NKM_LOCAL_SAVE_CUTSCEN_AUTO = "NKM_LOCAL_SAVE_CUTSCEN_AUTO";

	private const float DEFAULT_ADD_WAIT_TIME_FOR_AUTO = 1f;

	public const float CUTSCEN_TITLE_TYPYING_COOL_TIME = 0.15f;

	public GameObject m_objTopMenuParent;

	public NKCUIComToggle m_AutoToggle;

	public NKCUIComToggle m_PauseToggle;

	public NKCUIComButton m_SkipBtn;

	public NKCUIComButton m_LogBtn;

	public NKCUICutScenLogViewer m_logViewer;

	public NKCUIComButton m_NKM_UI_CUTSCEN_PLAYER;

	public NKCUIComButton m_NKM_UI_CUTSCEN_PLAYER_SKIP;

	public NKCUIComToggle m_NKM_UI_CUTSCEN_PLAYER_TALK_SKIP_AUTO;

	public GameObject m_objSelectionRoot;

	public NKCUIComStateButton[] m_aSelectionButtons;

	[Header("단축키 도움말")]
	public Transform m_trHotkeyHelpPosConfirm;

	public Transform m_trHotkeyHelpPosSkip;

	private NKCUICutScenTitleMgr m_NKCUICutScenTitleMgr;

	private NKCUICutScenBGMgr m_NKCUICutScenBGMgr;

	private NKCUICutScenUnitMgr m_NKCUICutScenUnitMgr;

	private static NKCUICutScenTalkBoxMgr.TalkBoxType m_CutScenTalkBoxMgrType;

	private NKCUICutScenImgMgr m_NKCUICutScenImgMgr;

	private List<NKCAssetResourceData> m_listNKCAssetResourceData = new List<NKCAssetResourceData>();

	private NKCUICutState m_cNKCUICutState = new NKCUICutState();

	private int m_NextCutIndex;

	private NKCCutScenTemplet m_NKCCutScenTemplet;

	private int m_stageID;

	private bool m_bPlaying;

	private CutScenCallBack m_Callback;

	private bool m_bWasBloom;

	private Queue<string> m_qNextCutScen = new Queue<string>();

	private NKCTextChunk m_NKCTextChunk = new NKCTextChunk();

	private const float m_ADD_WAIT_TIME_PER_ONE_WORD_BY_LONG_TALK_FOR_AUTO = 0.02f;

	private OnVoice dOnVoice;

	private List<CutsceneLog> m_lstLog = new List<CutsceneLog>();

	private const int LONG_TALK_CHECK_WORD_COUNT = 10;

	private const float MAX_ADD_WAIT_TIME_FOR_LONG_TALK = 3f;

	private Dictionary<string, List<int>> m_dicLoopSounds = new Dictionary<string, List<int>>();

	private Dictionary<string, int> m_dicForceSkin;

	public static NKCUICutScenPlayer Instance
	{
		get
		{
			InitiateInstance();
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

	private INKCUICutScenTalkBoxMgr m_NKCUICutScenTalkBoxMgr => NKCUICutScenTalkBoxMgr.GetCutScenTalkBoxMgr(m_CutScenTalkBoxMgrType);

	private float ADD_WAIT_TIME_PER_ONE_WORD_BY_LONG_TALK_FOR_AUTO
	{
		get
		{
			if (NKCScenManager.GetScenManager() != null && NKCScenManager.GetScenManager().GetGameOptionData() != null)
			{
				NKC_GAME_OPTION_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO cUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO = NKCScenManager.GetScenManager().GetGameOptionData().CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO;
				float num = 1f;
				switch (cUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO)
				{
				case NKC_GAME_OPTION_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO.FAST:
					num = NKCClientConst.NextTalkChangeSpeedWhenAuto_Fast;
					break;
				case NKC_GAME_OPTION_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO.NORMAL:
					num = NKCClientConst.NextTalkChangeSpeedWhenAuto_Normal;
					break;
				case NKC_GAME_OPTION_CUTSCEN_NEXT_TALK_CHANGE_SPEED_WHEN_AUTO.SLOW:
					num = NKCClientConst.NextTalkChangeSpeedWhenAuto_Slow;
					break;
				}
				return num * 0.02f;
			}
			return 0.02f;
		}
	}

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "컷신";

	public static void InitiateInstance()
	{
		if (m_Instance == null)
		{
			m_Instance = NKCUIManager.OpenNewInstance<NKCUICutScenPlayer>("ab_ui_nkm_ui_cutscen", "NKM_UI_CUTSCEN_PLAYER", NKCUIManager.eUIBaseRect.UIFrontCommonLow, CleanupInstance).GetInstance<NKCUICutScenPlayer>();
			m_Instance.InitUI();
		}
	}

	private void OnEnable()
	{
		NKCUICutScenTalkBoxMgr.InitUI(base.gameObject);
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
		NKCUICutScenTalkBoxMgr.OnCleanUp();
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public override void CloseInternal()
	{
		foreach (KeyValuePair<string, List<int>> dicLoopSound in m_dicLoopSounds)
		{
			List<int> value = dicLoopSound.Value;
			for (int i = 0; i < value.Count; i++)
			{
				NKCSoundManager.StopSound(value[i]);
			}
		}
		m_dicLoopSounds.Clear();
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.Stop();
		}
		m_lstLog.Clear();
		UnLoad();
		AttachToCutscenPlayer();
		NKCUIManager.SetUseFrontLowCanvas(bUse: false);
		base.gameObject.SetActive(value: false);
		NKCCamera.SetEnableSepiaToneSubUILowCam(bSet: false);
		m_cNKCUICutState.InitPerCut();
	}

	public override void OnCloseInstance()
	{
		UnLoad();
	}

	public override void OnBackButton()
	{
		StopWithCallBack();
	}

	public void InitUI()
	{
		NKCUICutScenTitleMgr.InitUI(base.gameObject);
		m_NKCUICutScenTitleMgr = NKCUICutScenTitleMgr.GetCutScenTitleMgr();
		NKCUICutScenBGMgr.InitUI(base.gameObject);
		m_NKCUICutScenBGMgr = NKCUICutScenBGMgr.GetCutScenBGMgr();
		NKCUICutScenUnitMgr.InitUI(base.gameObject);
		m_NKCUICutScenUnitMgr = NKCUICutScenUnitMgr.GetCutScenUnitMgr();
		NKCUICutScenImgMgr.InitUI(base.gameObject);
		m_NKCUICutScenImgMgr = NKCUICutScenImgMgr.GetCutScenImgMgr();
		NKCUtil.SetGameobjectActive(m_objTopMenuParent, bValue: true);
		NKCUtil.SetGameobjectActive(m_logViewer.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_PauseToggle != null)
		{
			m_PauseToggle.OnValueChanged.RemoveAllListeners();
			m_PauseToggle.OnValueChanged.AddListener(SetPause);
		}
		m_LogBtn.PointerClick.RemoveAllListeners();
		m_LogBtn.PointerClick.AddListener(OnClickLogBtn);
		NKCUtil.SetHotkey(m_LogBtn, HotkeyEventType.Up);
		if (m_NKM_UI_CUTSCEN_PLAYER != null)
		{
			m_NKM_UI_CUTSCEN_PLAYER.PointerClick.RemoveAllListeners();
			m_NKM_UI_CUTSCEN_PLAYER.PointerClick.AddListener(OnClickedPlayer);
		}
		if (m_NKM_UI_CUTSCEN_PLAYER_SKIP != null)
		{
			m_NKM_UI_CUTSCEN_PLAYER_SKIP.PointerClick.RemoveAllListeners();
			m_NKM_UI_CUTSCEN_PLAYER_SKIP.PointerClick.AddListener(OnClickedSkip);
		}
		if (m_NKM_UI_CUTSCEN_PLAYER_TALK_SKIP_AUTO != null)
		{
			m_NKM_UI_CUTSCEN_PLAYER_TALK_SKIP_AUTO.OnValueChanged.RemoveAllListeners();
			m_NKM_UI_CUTSCEN_PLAYER_TALK_SKIP_AUTO.OnValueChanged.AddListener(OnValueChangedAuto);
		}
		if (m_aSelectionButtons == null)
		{
			return;
		}
		for (int i = 0; i < m_aSelectionButtons.Length; i++)
		{
			if (!(m_aSelectionButtons[i] == null))
			{
				m_aSelectionButtons[i].m_DataInt = i;
				NKCUtil.SetButtonClickDelegate(m_aSelectionButtons[i], OnSelectionRoute);
			}
		}
	}

	private void OnClickLogBtn()
	{
		m_logViewer.OpenUI(m_lstLog, OnLogViewerClose, PlayVoice, m_NKCUICutScenTitleMgr.gameObject.activeInHierarchy, m_NKCUICutScenTalkBoxMgr.MyGameObject.activeInHierarchy, m_AutoToggle.m_bChecked, NKMStageTempletV2.Find(m_stageID));
		m_AutoToggle.Select(bSelect: false);
		NKCUtil.SetGameobjectActive(m_objTopMenuParent, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUICutScenTitleMgr.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUICutScenTalkBoxMgr.MyGameObject, bValue: false);
	}

	private void AddLog(NKCCutTemplet templet)
	{
		if (templet == null)
		{
			return;
		}
		string text = null;
		if (!string.IsNullOrEmpty(templet.m_Talk))
		{
			text = templet.m_Talk;
		}
		else if (!string.IsNullOrEmpty(templet.m_SubTitle))
		{
			text = templet.m_SubTitle;
		}
		else if (!string.IsNullOrEmpty(templet.m_Title))
		{
			text = templet.m_Title;
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (templet.m_bTalkAppend)
		{
			int index = m_lstLog.Count - 1;
			CutsceneLog value = m_lstLog[index];
			value.text = value.text + " " + text;
			value.SetVoice(templet.m_VoiceFileName);
			m_lstLog[index] = value;
		}
		else if (!string.IsNullOrEmpty(templet.m_CharStrID))
		{
			string text2 = "";
			NKCCutScenCharTemplet cutScenCharTempletByStrID = NKCCutScenManager.GetCutScenCharTempletByStrID(templet.m_CharStrID);
			if (cutScenCharTempletByStrID != null)
			{
				text2 = cutScenCharTempletByStrID.m_CharStr;
				if (NKCScenManager.CurrentUserData() != null)
				{
					text2 = text2.Replace("<usernickname>", NKCScenManager.CurrentUserData().m_UserNickName);
				}
			}
			m_lstLog.Add(new CutsceneLog(text2 + " : " + text, templet.m_VoiceFileName));
		}
		else
		{
			m_lstLog.Add(new CutsceneLog(text ?? "", templet.m_VoiceFileName));
		}
	}

	private void OnLogViewerClose(bool bEnableTitle, bool bEnableTalkBox, bool bAutoEnabled)
	{
		NKCUtil.SetGameobjectActive(m_objTopMenuParent, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKCUICutScenTitleMgr.gameObject, bEnableTitle);
		NKCUtil.SetGameobjectActive(m_NKCUICutScenTalkBoxMgr.MyGameObject, bEnableTalkBox);
	}

	public void SetPause(bool bPause)
	{
		if (bPause)
		{
			m_AutoToggle.Select(bSelect: false);
		}
		m_NKCUICutScenBGMgr.SetPause(bPause);
		m_NKCUICutScenTitleMgr.SetPause(bPause);
		m_NKCUICutScenUnitMgr.SetPause(bPause);
		m_NKCUICutScenImgMgr.SetPause(bPause);
		m_NKCUICutScenTalkBoxMgr.SetPause(bPause);
		NKCUtil.SetGameobjectActive(m_SkipBtn, !bPause);
		NKCUtil.SetGameobjectActive(m_AutoToggle, !bPause);
		NKCUtil.SetGameobjectActive(m_LogBtn, !bPause);
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null && subUICameraVideoPlayer.IsPlaying)
		{
			if (bPause)
			{
				subUICameraVideoPlayer.SetPlaybackSpeed(0f);
			}
			else
			{
				subUICameraVideoPlayer.SetPlaybackSpeed(1f);
			}
		}
	}

	public bool IsPlaying()
	{
		return m_bPlaying;
	}

	public void OnValueChangedAuto(bool bSet)
	{
		if (bSet)
		{
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_CUTSCEN_AUTO", 1);
		}
		else
		{
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_CUTSCEN_AUTO", 0);
		}
	}

	public void Load(string strID, bool bPreLoad = true, Dictionary<string, int> dicSkin = null)
	{
		NKCCutScenManager.LoadFromLUA_CutScene(strID);
		if (bPreLoad)
		{
			Debug.Log("Cutscene " + strID + " Preloading");
		}
		else
		{
			Debug.Log("Cutscene " + strID + " loading");
		}
		HashSet<string> hashSet = new HashSet<string>();
		HashSet<NKMAssetName> hashSet2 = new HashSet<NKMAssetName>();
		HashSet<NKMAssetName> hashSet3 = new HashSet<NKMAssetName>();
		HashSet<string> hashSet4 = new HashSet<string>();
		m_dicForceSkin = dicSkin;
		NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(strID);
		if (cutScenTemple == null)
		{
			return;
		}
		int num = 0;
		int count = cutScenTemple.m_listCutTemplet.Count;
		for (num = 0; num < count; num++)
		{
			NKCCutTemplet nKCCutTemplet = cutScenTemple.m_listCutTemplet[num];
			if (nKCCutTemplet == null)
			{
				continue;
			}
			if (!string.IsNullOrWhiteSpace(nKCCutTemplet.m_MovieName))
			{
				AssetBundleManager.GetRawFilePath("Movie/" + nKCCutTemplet.m_MovieName);
			}
			if (nKCCutTemplet.m_BGFileName.Length > 0 && nKCCutTemplet.m_BGFileName != "CLOSE")
			{
				if (nKCCutTemplet.m_bGameObjectBGType)
				{
					hashSet3.Add(new NKMAssetName(nKCCutTemplet.m_BGFileName, nKCCutTemplet.m_BGFileName));
				}
				else
				{
					string text = "AB_UI_NKM_UI_CUTSCEN_BG_" + nKCCutTemplet.m_BGFileName;
					hashSet2.Add(new NKMAssetName(text, text));
				}
			}
			if (nKCCutTemplet.m_CharStrID.Length > 0)
			{
				NKCCutScenCharTemplet cutScenCharTempletByStrID = NKCCutScenManager.GetCutScenCharTempletByStrID(nKCCutTemplet.m_CharStrID);
				if (cutScenCharTempletByStrID != null && cutScenCharTempletByStrID.m_PrefabStr.Length > 0)
				{
					hashSet.Add(cutScenCharTempletByStrID.m_PrefabStr);
				}
			}
			if (nKCCutTemplet.m_ImageName.Length > 0)
			{
				hashSet2.Add(new NKMAssetName("AB_UI_NKM_UI_CUTSCEN_IMG", "AB_UI_NKM_UI_CUTSCEN_IMG_" + nKCCutTemplet.m_ImageName));
			}
			if (nKCCutTemplet.m_StartBGMFileName.Length > 0)
			{
				hashSet4.Add(nKCCutTemplet.m_StartBGMFileName);
			}
			if (nKCCutTemplet.m_EndBGMFileName.Length > 0)
			{
				hashSet4.Add(nKCCutTemplet.m_EndBGMFileName);
			}
			if (nKCCutTemplet.m_StartFXSoundName.Length > 0)
			{
				hashSet4.Add(nKCCutTemplet.m_StartFXSoundName);
			}
			if (nKCCutTemplet.m_EndFXSoundName.Length > 0)
			{
				hashSet4.Add(nKCCutTemplet.m_EndFXSoundName);
			}
			if (nKCCutTemplet.m_Action == NKCCutTemplet.eCutsceneAction.PLAY_MUSIC)
			{
				string actionFirstToken = nKCCutTemplet.GetActionFirstToken();
				if (!string.IsNullOrEmpty(actionFirstToken))
				{
					hashSet4.Add(actionFirstToken);
				}
			}
			if (!string.IsNullOrWhiteSpace(nKCCutTemplet.m_VoiceFileName) && NKCAssetResourceManager.IsBundleExists(NKCAssetResourceManager.GetBundleName(nKCCutTemplet.m_VoiceFileName, bIgnoreNotFoundError: true), nKCCutTemplet.m_VoiceFileName))
			{
				hashSet4.Add(nKCCutTemplet.m_VoiceFileName);
			}
		}
		foreach (string item in hashSet)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item);
			NKCASUIUnitIllust closeObj = ((unitTempletBase == null || m_dicForceSkin == null || !m_dicForceSkin.ContainsKey(item)) ? NKCResourceUtility.OpenSpineIllustWithManualNaming(item, bPreLoad) : NKCResourceUtility.OpenSpineIllust(unitTempletBase, m_dicForceSkin[item], bPreLoad));
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(closeObj);
		}
		foreach (string item2 in hashSet4)
		{
			if (!string.IsNullOrEmpty(item2))
			{
				m_listNKCAssetResourceData.Add(NKCAssetResourceManager.OpenResource<AudioClip>(item2, bPreLoad));
			}
		}
		if (bPreLoad)
		{
			foreach (NKMAssetName item3 in hashSet3)
			{
				if (item3 != null && !string.IsNullOrEmpty(item3.m_AssetName) && !string.IsNullOrEmpty(item3.m_BundleName))
				{
					NKCResourceUtility.LoadAssetResourceTemp<GameObject>(item3.m_BundleName, item3.m_AssetName);
				}
			}
			{
				foreach (NKMAssetName item4 in hashSet2)
				{
					if (item4 != null && !string.IsNullOrEmpty(item4.m_AssetName) && !string.IsNullOrEmpty(item4.m_BundleName))
					{
						NKCResourceUtility.LoadAssetResourceTemp<Sprite>(item4.m_BundleName, item4.m_AssetName);
					}
				}
				return;
			}
		}
		foreach (NKMAssetName item5 in hashSet3)
		{
			if (item5 != null && !string.IsNullOrEmpty(item5.m_AssetName) && !string.IsNullOrEmpty(item5.m_BundleName))
			{
				m_listNKCAssetResourceData.Add(NKCAssetResourceManager.OpenResource<GameObject>(item5.m_BundleName, item5.m_AssetName));
			}
		}
		foreach (NKMAssetName item6 in hashSet2)
		{
			if (item6 != null && !string.IsNullOrEmpty(item6.m_AssetName) && !string.IsNullOrEmpty(item6.m_BundleName))
			{
				m_listNKCAssetResourceData.Add(NKCAssetResourceManager.OpenResource<Sprite>(item6.m_BundleName, item6.m_AssetName));
			}
		}
	}

	public void UnLoad()
	{
		Debug.Log("Cutscene unloading");
		int num = 0;
		for (num = 0; num < m_listNKCAssetResourceData.Count; num++)
		{
			NKCAssetResourceManager.CloseResource(m_listNKCAssetResourceData[num]);
		}
		m_listNKCAssetResourceData.Clear();
	}

	private void DetachToFrontLow()
	{
		m_NKCUICutScenBGMgr.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontLow), worldPositionStays: false);
		m_NKCUICutScenUnitMgr.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontLow), worldPositionStays: false);
	}

	private void AttachToCutscenPlayer()
	{
		m_NKCUICutScenBGMgr.transform.SetParent(base.gameObject.transform, worldPositionStays: false);
		m_NKCUICutScenUnitMgr.transform.SetParent(base.gameObject.transform, worldPositionStays: false);
	}

	public override void Hide()
	{
		AttachToCutscenPlayer();
		base.Hide();
	}

	public override void UnHide()
	{
		base.UnHide();
		DetachToFrontLow();
	}

	public void Play(NKCCutScenTemplet cNKCCutScenTemplet, int stageID, CutScenCallBack _callBack = null)
	{
		if (cNKCCutScenTemplet == null)
		{
			Log.Error("NKCUICutScenPlayer Cannot Play becuase templet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUICutScenPlayer.cs", 694);
			return;
		}
		m_NKCCutScenTemplet = cNKCCutScenTemplet;
		m_stageID = stageID;
		m_Callback = _callBack;
		if (m_logViewer.gameObject.activeSelf)
		{
			m_logViewer.gameObject.SetActive(value: false);
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		if (!m_objTopMenuParent.activeSelf)
		{
			m_objTopMenuParent.SetActive(value: true);
		}
		m_bWasBloom = NKCCamera.GetEnableBloom();
		NKCCamera.EnableBloom(bEnable: false);
		m_NextCutIndex = 0;
		m_NKCUICutScenBGMgr.Reset();
		m_NKCUICutScenTitleMgr.Reset();
		m_NKCUICutScenUnitMgr.Reset();
		m_NKCUICutScenImgMgr.Reset();
		m_NKCUICutScenTalkBoxMgr.ResetTalkBox();
		NKCUtil.SetGameobjectActive(m_NKCUICutScenBGMgr, bValue: true);
		NKCUtil.SetGameobjectActive(m_LogBtn, bValue: true);
		NKCUtil.SetGameobjectActive(m_PauseToggle, bValue: true);
		NKCUtil.SetGameobjectActive(m_SkipBtn, bValue: true);
		NKCUtil.SetGameobjectActive(m_AutoToggle, bValue: true);
		m_PauseToggle.Select(bSelect: false, bForce: true);
		NKCUtil.SetGameobjectActive(m_objSelectionRoot, bValue: false);
		m_bPlaying = true;
		if (PlayerPrefs.HasKey("NKM_LOCAL_SAVE_CUTSCEN_AUTO"))
		{
			if (PlayerPrefs.GetInt("NKM_LOCAL_SAVE_CUTSCEN_AUTO") == 1)
			{
				m_AutoToggle.Select(bSelect: true, bForce: true);
			}
			else
			{
				m_AutoToggle.Select(bSelect: false, bForce: true);
			}
		}
		else
		{
			m_AutoToggle.Select(bSelect: false, bForce: true);
			PlayerPrefs.SetInt("NKM_LOCAL_SAVE_CUTSCEN_AUTO", 0);
		}
		m_NKCUICutScenUnitMgr.Open();
		NKCUIFadeInOut.Close();
		NKCUIManager.SetUseFrontLowCanvas(bUse: true);
		DetachToFrontLow();
		NKCCamera.SetEnableSepiaToneSubUILowCam(bSet: false);
		m_lstLog.Clear();
		UIOpened();
		ApplyNextCut();
		NKCMMPManager.OnPlayCutScene(m_NKCCutScenTemplet.m_CutScenID);
	}

	public void Play(string strID, int stageID, CutScenCallBack _callBack = null)
	{
		NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(strID);
		if (cutScenTemple == null)
		{
			Log.Error("NKCUICutScenPlayer Cannot Play: " + strID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUICutScenPlayer.cs", 790);
		}
		else
		{
			Play(cutScenTemple, stageID, _callBack);
		}
	}

	public void Play(Queue<string> qStrID, int stageID, CutScenCallBack _callBack = null)
	{
		m_qNextCutScen = qStrID;
		m_stageID = stageID;
		if (m_qNextCutScen.Count > 0)
		{
			Play(m_qNextCutScen.Dequeue(), stageID, _callBack);
			return;
		}
		m_Callback = _callBack;
		CallBack();
	}

	public void LoadAndPlay(string strID, int stageID, CutScenCallBack _callBack = null, bool bAsync = true)
	{
		NKMPopUpBox.OpenWaitBox(NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		base.gameObject.SetActive(value: true);
		StartCoroutine(_LoadAndPlay(strID, stageID, _callBack, bAsync));
	}

	private IEnumerator _LoadAndPlay(string strID, int stageID, CutScenCallBack _callBack = null, bool bAsync = true)
	{
		UnLoad();
		Load(strID, bAsync);
		while (!NKCAssetResourceManager.IsLoadEnd())
		{
			yield return new WaitForSeconds(0.02f);
		}
		if (bAsync)
		{
			NKCResourceUtility.SwapResource();
		}
		NKMPopUpBox.CloseWaitBox();
		Play(strID, stageID, delegate
		{
			NKCSoundManager.StopAllSound();
			NKCSoundManager.PlayScenMusic(NKCScenManager.GetScenManager().GetNowScenID());
			_callBack?.Invoke();
		});
	}

	private bool IsCutFinished()
	{
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if ((!m_cNKCUICutState.m_bTitle || (m_cNKCUICutState.m_bTitle && m_NKCUICutScenTitleMgr.IsFinshed())) && (!m_cNKCUICutState.m_bFading || (m_cNKCUICutState.m_bFading && NKCUIFadeInOut.IsFinshed())) && (!m_cNKCUICutState.m_bTalk || (m_cNKCUICutState.m_bTalk && m_NKCUICutScenTalkBoxMgr.IsFinished)) && m_NKCUICutScenUnitMgr.IsFinished() && m_NKCUICutScenImgMgr.IsFinished() && m_NKCUICutScenBGMgr.IsFinished() && (!m_cNKCUICutState.m_bPlayVideo || (subUICameraVideoPlayer != null && !subUICameraVideoPlayer.IsPlayingOrPreparing() && !m_cNKCUICutState.m_bWaitSelection)) && (m_cNKCUICutState.m_VoiceUID < 0 || !m_AutoToggle.m_bChecked || !NKCSoundManager.IsPlayingVoice(m_cNKCUICutState.m_VoiceUID)))
		{
			return true;
		}
		return false;
	}

	private void InvalidVideoCallback()
	{
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.InvalidateCallBack();
		}
	}

	public void OnClickedPlayer()
	{
		if (m_logViewer.gameObject.activeSelf || m_NKCCutScenTemplet == null)
		{
			return;
		}
		if (m_PauseToggle.m_bChecked)
		{
			m_PauseToggle.Select(bSelect: false);
		}
		else
		{
			if (!m_cNKCUICutState.m_bWaitClick && (m_cNKCUICutState.m_bTitle || m_cNKCUICutState.m_bTalk))
			{
				return;
			}
			if (IsCutFinished() && m_cNKCUICutState.m_bWaitClick)
			{
				InvalidVideoCallback();
				if (m_NKCUICutScenTalkBoxMgr is NKCUICutScenTalkBoxMgrForCenterText { WaitForFadOut: not false, WaitForFadOut: not false } nKCUICutScenTalkBoxMgrForCenterText)
				{
					nKCUICutScenTalkBoxMgrForCenterText.StartFadeOut(ApplyNextCut);
				}
				else
				{
					ApplyNextCut();
				}
				return;
			}
			m_NKCUICutScenTitleMgr.Finish();
			m_NKCUICutScenTalkBoxMgr.Finish();
			NKCUIFadeInOut.Finish();
			m_NKCUICutScenUnitMgr.Finish();
			m_NKCUICutScenImgMgr.Finish();
			m_NKCUICutScenBGMgr.Finish();
			if (!m_cNKCUICutState.m_bWaitClick)
			{
				InvalidVideoCallback();
				ApplyNextCut();
			}
		}
	}

	public void OnClickedSkip()
	{
		if (m_cNKCUICutState.m_bPlayVideo)
		{
			SetMoviePause(bPause: true);
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CUTSCENE_MOVIE_SKIP_TITLE, NKCUtilString.GET_STRING_CUTSCENE_MOVIE_SKIP_DESC, Skip, delegate
			{
				SetMoviePause(bPause: false);
			});
		}
		else
		{
			Skip();
		}
	}

	private void Skip()
	{
		StopPure();
		if (!PlayIfNextExist())
		{
			CallBack();
		}
	}

	private void SetMoviePause(bool bPause)
	{
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			if (bPause)
			{
				subUICameraVideoPlayer.SetPlaybackSpeed(0f);
			}
			else
			{
				subUICameraVideoPlayer.SetPlaybackSpeed(1f);
			}
		}
	}

	public bool PlayIfNextExist()
	{
		if (m_qNextCutScen.Count > 0)
		{
			Play(m_qNextCutScen.Dequeue(), m_stageID);
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (!m_bPlaying)
		{
			return;
		}
		if (IsCutFinished() && !m_PauseToggle.m_bChecked)
		{
			if (!m_cNKCUICutState.m_bWaitClick || m_AutoToggle.m_bChecked)
			{
				if (m_cNKCUICutState.m_fWaitTime > 0f)
				{
					m_cNKCUICutState.m_fElapsedTimeWithoutAutoCalc += Time.deltaTime;
					if (m_cNKCUICutState.m_fElapsedTimeWithoutAutoCalc >= m_cNKCUICutState.m_fWaitTime)
					{
						ApplyNextCut();
					}
					else if (m_AutoToggle.m_bChecked && m_cNKCUICutState.m_fElapsedTimeWithoutAutoCalc >= m_cNKCUICutState.m_fWaitTime - m_cNKCUICutState.m_fAddWaitTimeForAuto)
					{
						m_NKCUICutScenUnitMgr.StopCrash();
					}
				}
				else
				{
					ApplyNextCut();
				}
			}
			else
			{
				PlayReservedSoundNMusic();
				m_NKCUICutScenUnitMgr.StopCrash();
			}
		}
		if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Skip))
		{
			m_cNKCUICutState.m_bWaitClick = true;
			OnClickedPlayer();
		}
	}

	private void PlayReservedSoundNMusic()
	{
		if (m_cNKCUICutState.m_EndBGMFileName.Length > 0)
		{
			NKCSoundManager.PlayMusic(m_cNKCUICutState.m_EndBGMFileName, bLoop: true);
			m_cNKCUICutState.m_EndBGMFileName = "";
		}
		if (m_cNKCUICutState.m_EndFXSoundName.Length > 0)
		{
			if (m_cNKCUICutState.m_EndFXSoundControl == NKC_CUTSCEN_SOUND_CONTROL.NCSC_ONE_TIME_PLAY)
			{
				NKCSoundManager.PlaySound(m_cNKCUICutState.m_EndFXSoundName, 1f, 0f, 0f);
			}
			else if (m_cNKCUICutState.m_EndFXSoundControl == NKC_CUTSCEN_SOUND_CONTROL.NCSC_LOOP_PLAY)
			{
				int soundUID = NKCSoundManager.PlaySound(m_cNKCUICutState.m_EndFXSoundName, 1f, 0f, 0f, bLoop: true);
				AddLoopSoundID(m_cNKCUICutState.m_EndFXSoundName, soundUID);
			}
			else if (m_cNKCUICutState.m_EndFXSoundControl == NKC_CUTSCEN_SOUND_CONTROL.NCSC_STOP)
			{
				StopSound(m_cNKCUICutState.m_EndFXSoundName);
			}
			m_cNKCUICutState.m_EndFXSoundName = "";
			m_cNKCUICutState.m_EndFXSoundControl = NKC_CUTSCEN_SOUND_CONTROL.NCSC_STOP;
		}
	}

	private void AddLoopSoundID(string name, int soundUID)
	{
		List<int> value = null;
		if (m_dicLoopSounds.TryGetValue(name, out value))
		{
			value.Add(soundUID);
			return;
		}
		value = new List<int>();
		value.Add(soundUID);
		m_dicLoopSounds.Add(name, value);
	}

	private void VideoPlayMessageCallback(NKCUIComVideoPlayer.eVideoMessage message)
	{
		switch (message)
		{
		case NKCUIComVideoPlayer.eVideoMessage.PlayFailed:
		case NKCUIComVideoPlayer.eVideoMessage.PlayComplete:
			NKCUtil.SetGameobjectActive(m_AutoToggle, bValue: true);
			NKCUtil.SetGameobjectActive(m_LogBtn, bValue: true);
			NKCUtil.SetGameobjectActive(m_PauseToggle, bValue: true);
			NKCUtil.SetGameobjectActive(m_objTopMenuParent, bValue: true);
			ApplyNextCut();
			break;
		case NKCUIComVideoPlayer.eVideoMessage.PlayBegin:
			NKCUtil.SetGameobjectActive(m_AutoToggle, bValue: false);
			NKCUtil.SetGameobjectActive(m_LogBtn, bValue: false);
			NKCUtil.SetGameobjectActive(m_PauseToggle, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTopMenuParent, m_cNKCUICutState.m_bMovieSkipEnable);
			break;
		}
	}

	private void ApplyNextCut()
	{
		PlayReservedSoundNMusic();
		m_NKCUICutScenUnitMgr.Finish();
		m_cNKCUICutState.InitPerCut();
		m_NKCUICutScenTitleMgr.Close();
		if (m_NKCCutScenTemplet.m_listCutTemplet.Count <= m_NextCutIndex)
		{
			StopPure();
			if (!PlayIfNextExist())
			{
				CallBack();
			}
			return;
		}
		NKCCutTemplet nKCCutTemplet = m_NKCCutScenTemplet.m_listCutTemplet[m_NextCutIndex];
		if (nKCCutTemplet != null)
		{
			if (nKCCutTemplet.m_bLooseShake)
			{
				NKCUIManager.StartLooseShake();
			}
			else
			{
				NKCUIManager.StopLooseShake();
			}
			if (ProcessCutsceneAction(nKCCutTemplet))
			{
				return;
			}
			if (nKCCutTemplet.m_FilterType != NKC_CUTSCEN_FILTER_TYPE.NCFT_NONE)
			{
				NKCCamera.SetEnableSepiaToneSubUILowCam(nKCCutTemplet.m_FilterType == NKC_CUTSCEN_FILTER_TYPE.NCFT_SEPIA);
			}
			if (!string.IsNullOrEmpty(nKCCutTemplet.m_MovieName))
			{
				m_cNKCUICutState.m_bPlayVideo = true;
				m_cNKCUICutState.m_bMovieSkipEnable = nKCCutTemplet.m_bMovieSkipEnable;
				NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
				if (subUICameraVideoPlayer != null)
				{
					subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
					subUICameraVideoPlayer.m_fMoviePlaySpeed = 1f;
					subUICameraVideoPlayer.Play(nKCCutTemplet.m_MovieName, bLoop: false, bPlaySound: true, VideoPlayMessageCallback, bForcePlay: true);
				}
			}
			else
			{
				NKCCamera.GetSubUICameraVideoPlayer()?.Stop();
			}
			if (nKCCutTemplet.m_Title.Length > 0 || nKCCutTemplet.m_SubTitle.Length > 0)
			{
				m_cNKCUICutState.m_bTitle = true;
				m_NKCUICutScenTitleMgr.Open(nKCCutTemplet.m_bTitleFadeOut, nKCCutTemplet.m_fTitleFadeOutTime, nKCCutTemplet.m_SubTitle, nKCCutTemplet.m_Title, nKCCutTemplet.m_fTitleTalkTime, nKCCutTemplet.m_fSubTitleTalkTime);
			}
			else if (nKCCutTemplet.m_bTitleClear)
			{
				m_NKCUICutScenTitleMgr.ForceClear();
			}
			if (nKCCutTemplet.m_CloseTalkBox)
			{
				m_NKCUICutScenTalkBoxMgr?.Close();
			}
			m_cNKCUICutState.m_bWaitClick = nKCCutTemplet.m_bWaitClick;
			if (nKCCutTemplet.m_fWaitTime > 0f)
			{
				m_cNKCUICutState.m_fWaitTime = nKCCutTemplet.m_fWaitTime;
			}
			if (nKCCutTemplet.m_BGFileName == "CLOSE")
			{
				m_NKCUICutScenBGMgr.CloseBG();
			}
			else if (nKCCutTemplet.m_BGFileName.Length > 0 || (nKCCutTemplet.m_bGameObjectBGType && nKCCutTemplet.m_GameObjectBGAniName.Length > 0))
			{
				m_NKCUICutScenBGMgr.Open(nKCCutTemplet.m_bGameObjectBGType, nKCCutTemplet.m_BGFileName, nKCCutTemplet.m_GameObjectBGAniName, nKCCutTemplet.m_bGameObjectBGLoop, nKCCutTemplet.m_fBGFadeInTime, nKCCutTemplet.m_easeBGFadeIn, nKCCutTemplet.m_colBGFadeInStart, nKCCutTemplet.m_colBGFadeIn, nKCCutTemplet.m_fBGFadeOutTime, nKCCutTemplet.m_easeBGFadeOut, nKCCutTemplet.m_colBGFadeOut);
			}
			if (nKCCutTemplet.m_BGCrash > 0 && nKCCutTemplet.m_fBGCrashTime > 0f)
			{
				m_NKCUICutScenBGMgr.SetCrash(nKCCutTemplet.m_BGCrash, nKCCutTemplet.m_fBGCrashTime);
			}
			else if (nKCCutTemplet.m_fBGAnITime > 0f && (nKCCutTemplet.m_bBGAniPos || nKCCutTemplet.m_bBGAniScale))
			{
				m_NKCUICutScenBGMgr.SetAni(nKCCutTemplet.m_bNoWaitBGAni, nKCCutTemplet.m_fBGAnITime, nKCCutTemplet.m_bBGAniPos, nKCCutTemplet.m_BGOffsetPose, nKCCutTemplet.m_tdtBGPos, nKCCutTemplet.m_bBGAniScale, nKCCutTemplet.m_BGOffsetScale, nKCCutTemplet.m_tdtBGScale);
			}
			if (nKCCutTemplet.m_fFadeTime > 0f)
			{
				m_cNKCUICutState.m_bFading = true;
				if (nKCCutTemplet.m_bFadeIn)
				{
					NKCUIFadeInOut.FadeIn(nKCCutTemplet.m_fFadeTime, null, nKCCutTemplet.m_bFadeWhite);
				}
				else
				{
					NKCUIFadeInOut.FadeOut(nKCCutTemplet.m_fFadeTime, null, nKCCutTemplet.m_bFadeWhite);
				}
			}
			else if (nKCCutTemplet.m_fFlashBangTime > 0f)
			{
				NKCUIFadeInOut.FadeIn(nKCCutTemplet.m_fFlashBangTime, null, bWhite: true);
			}
			if (nKCCutTemplet.m_bClear)
			{
				m_NKCUICutScenUnitMgr.ClearUnit(nKCCutTemplet);
			}
			else if (nKCCutTemplet.m_CharStrID.Length > 0)
			{
				NKCCutScenCharTemplet cutScenCharTempletByStrID = NKCCutScenManager.GetCutScenCharTempletByStrID(nKCCutTemplet.m_CharStrID);
				if (cutScenCharTempletByStrID != null)
				{
					m_NKCUICutScenUnitMgr.SetUnit(cutScenCharTempletByStrID, nKCCutTemplet, m_dicForceSkin);
				}
			}
			if (nKCCutTemplet.m_Talk.Length > 0)
			{
				string text = "";
				if (!string.IsNullOrEmpty(nKCCutTemplet.m_CharStrID))
				{
					NKCCutScenCharTemplet cutScenCharTempletByStrID2 = NKCCutScenManager.GetCutScenCharTempletByStrID(nKCCutTemplet.m_CharStrID);
					if (cutScenCharTempletByStrID2 != null)
					{
						text = cutScenCharTempletByStrID2.m_CharStr;
					}
				}
				NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
				if (NKCScenManager.GetScenManager() != null && myUserData != null)
				{
					string newValue = NKCUITypeWriter.ReplaceNameString(myUserData.m_UserNickName, bBlock: true);
					text = text.Replace("<usernickname>", newValue);
				}
				else
				{
					text = text.Replace("<usernickname>", "TESTCUT");
				}
				ChangeTalkBoxMgrType(nKCCutTemplet);
				m_NKCUICutScenTalkBoxMgr.Open(text, nKCCutTemplet.m_Talk, nKCCutTemplet.m_fTalkTime, nKCCutTemplet.m_bWaitClick, nKCCutTemplet.m_bTalkAppend);
				if (nKCCutTemplet.m_bTalkCenterFadeIn)
				{
					m_NKCUICutScenTalkBoxMgr.StartFadeIn(nKCCutTemplet.m_bTalkCenterFadeTime);
				}
				else if (nKCCutTemplet.m_bTalkCenterFadeOut)
				{
					m_NKCUICutScenTalkBoxMgr.FadeOutBooking(nKCCutTemplet.m_bTalkCenterFadeTime);
				}
				m_cNKCUICutState.m_bTalk = true;
				if (m_AutoToggle.m_bChecked)
				{
					m_cNKCUICutState.m_fAddWaitTimeForAuto = 1f;
					m_NKCTextChunk.TextAnalyze(nKCCutTemplet.m_Talk);
					int pureTextCount = m_NKCTextChunk.GetPureTextCount();
					if (pureTextCount > 10)
					{
						float num = (float)(pureTextCount - 10) * ADD_WAIT_TIME_PER_ONE_WORD_BY_LONG_TALK_FOR_AUTO;
						if (num >= 3f)
						{
							num = 3f;
						}
						m_cNKCUICutState.m_fAddWaitTimeForAuto += num;
					}
					m_cNKCUICutState.m_fWaitTime += m_cNKCUICutState.m_fAddWaitTimeForAuto;
				}
			}
			else if (m_NKCUICutScenUnitMgr.IsExistUnitInScen())
			{
				m_NKCUICutScenTalkBoxMgr.ClearTalk();
			}
			else
			{
				m_NKCUICutScenTalkBoxMgr.Close();
			}
			if (nKCCutTemplet.m_ImageName.Length > 0)
			{
				m_NKCUICutScenImgMgr.Open(nKCCutTemplet.m_ImageName, nKCCutTemplet.m_ImageOffsetPos, nKCCutTemplet.m_fImageScale);
			}
			else
			{
				m_NKCUICutScenImgMgr.Close();
			}
			if (nKCCutTemplet.m_StartBGMFileName.Length > 0)
			{
				NKCSoundManager.PlayMusic(nKCCutTemplet.m_StartBGMFileName, bLoop: true);
			}
			if (nKCCutTemplet.m_StartFXSoundName.Length > 0)
			{
				if (nKCCutTemplet.m_StartFXSoundControl == NKC_CUTSCEN_SOUND_CONTROL.NCSC_ONE_TIME_PLAY)
				{
					NKCSoundManager.PlaySound(nKCCutTemplet.m_StartFXSoundName, 1f, 0f, 0f);
				}
				else if (nKCCutTemplet.m_StartFXSoundControl == NKC_CUTSCEN_SOUND_CONTROL.NCSC_LOOP_PLAY)
				{
					int soundUID = NKCSoundManager.PlaySound(nKCCutTemplet.m_StartFXSoundName, 1f, 0f, 0f, bLoop: true);
					AddLoopSoundID(nKCCutTemplet.m_StartFXSoundName, soundUID);
				}
				else if (nKCCutTemplet.m_StartFXSoundControl == NKC_CUTSCEN_SOUND_CONTROL.NCSC_STOP)
				{
					StopSound(nKCCutTemplet.m_StartFXSoundName);
				}
			}
			m_cNKCUICutState.m_EndBGMFileName = nKCCutTemplet.m_EndBGMFileName;
			m_cNKCUICutState.m_EndFXSoundControl = nKCCutTemplet.m_EndFXSoundControl;
			m_cNKCUICutState.m_EndFXSoundName = nKCCutTemplet.m_EndFXSoundName;
			if (!string.IsNullOrWhiteSpace(nKCCutTemplet.m_VoiceFileName))
			{
				string bundleName = NKCAssetResourceManager.GetBundleName(nKCCutTemplet.m_VoiceFileName, bIgnoreNotFoundError: true);
				if (NKCAssetResourceManager.IsBundleExists(bundleName, nKCCutTemplet.m_VoiceFileName))
				{
					m_cNKCUICutState.m_VoiceUID = NKCSoundManager.PlayVoice(nKCCutTemplet.m_VoiceFileName, 0, bClearVoice: false, bIgnoreSameVoice: false, 1f, 0f, 0f);
				}
				else
				{
					Debug.LogWarning("컷신 보이스 " + nKCCutTemplet.m_VoiceFileName + "가 있지만 번들파일 " + bundleName + "이 존재하지 않음");
				}
			}
			AddLog(nKCCutTemplet);
		}
		m_NextCutIndex++;
	}

	public void PlayVoice(string voiceFileName)
	{
		if (m_cNKCUICutState.m_VoiceUID >= 0)
		{
			NKCSoundManager.StopSound(m_cNKCUICutState.m_VoiceUID);
		}
		m_cNKCUICutState.m_VoiceUID = NKCSoundManager.PlayVoice(voiceFileName, 0, bClearVoice: false, bIgnoreSameVoice: false, 1f, 0f, 0f);
	}

	private static void ChangeTalkBoxMgrType(NKCCutTemplet cTemplet)
	{
		if (cTemplet.m_bTalkCenterFadeIn || cTemplet.m_bTalkCenterFadeOut)
		{
			m_CutScenTalkBoxMgrType = NKCUICutScenTalkBoxMgr.TalkBoxType.CenterText;
		}
		else
		{
			m_CutScenTalkBoxMgrType = NKCUICutScenTalkBoxMgr.TalkBoxType.JapanNeeds;
		}
	}

	private bool ProcessCutsceneAction(NKCCutTemplet cTemplet)
	{
		if (cTemplet == null)
		{
			return false;
		}
		switch (cTemplet.m_Action)
		{
		case NKCCutTemplet.eCutsceneAction.NONE:
		case NKCCutTemplet.eCutsceneAction.MARK:
			return false;
		case NKCCutTemplet.eCutsceneAction.JUMP:
			JumpToMark(cTemplet.m_ActionStrKey);
			return true;
		case NKCCutTemplet.eCutsceneAction.SELECT:
			return ProcessSelection(m_NextCutIndex);
		case NKCCutTemplet.eCutsceneAction.PLAY_MUSIC:
		{
			string[] actionTokens = cTemplet.GetActionTokens();
			if (actionTokens != null && actionTokens.Length != 0)
			{
				string audioClipName = actionTokens[0];
				string s = actionTokens[1];
				float fStartTime = 0f;
				if (float.TryParse(s, out var result))
				{
					fStartTime = ((!(result < 0f)) ? result : NKCSoundManager.GetMusicTime());
				}
				NKCSoundManager.PlayMusic(audioClipName, bLoop: true, 1f, bForce: true, fStartTime);
				return false;
			}
			break;
		}
		}
		return false;
	}

	private void JumpToMark(string mark)
	{
		int num = m_NKCCutScenTemplet.m_listCutTemplet.FindIndex((NKCCutTemplet x) => x.m_Action == NKCCutTemplet.eCutsceneAction.MARK && x.m_ActionStrKey == mark);
		if (num < 0)
		{
			Debug.LogError("Mark " + mark + " not found!!");
		}
		else
		{
			m_NextCutIndex = num;
		}
	}

	private bool ProcessSelection(int currentIndex)
	{
		List<Tuple<string, string>> selectionRoutes = NKCCutScenManager.GetSelectionRoutes(m_NKCCutScenTemplet.m_CutScenID, currentIndex);
		if (selectionRoutes == null)
		{
			return false;
		}
		m_cNKCUICutState.m_bWaitSelection = true;
		m_cNKCUICutState.m_lstSelectionMark.Clear();
		for (int i = 0; i < m_aSelectionButtons.Length; i++)
		{
			if (i < selectionRoutes.Count)
			{
				NKCUtil.SetGameobjectActive(m_aSelectionButtons[i], bValue: true);
				m_cNKCUICutState.m_lstSelectionMark.Add(selectionRoutes[i]);
				m_aSelectionButtons[i]?.SetTitleText(selectionRoutes[i].Item2);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_aSelectionButtons[i], bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objSelectionRoot, bValue: true);
		return true;
	}

	private void OnSelectionRoute(int routeIndex)
	{
		m_cNKCUICutState.m_bWaitSelection = false;
		NKCUtil.SetGameobjectActive(m_objSelectionRoot, bValue: false);
		m_lstLog.Add(new CutsceneLog(NKCStringTable.GetString("SI_DP_CUTSCENE_LOG_SELECTION", m_cNKCUICutState.m_lstSelectionMark[routeIndex].Item2)));
		JumpToMark(m_cNKCUICutState.m_lstSelectionMark[routeIndex].Item1);
		ApplyNextCut();
	}

	private void StopSound(string name)
	{
		List<int> value = null;
		if (m_dicLoopSounds.TryGetValue(name, out value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				NKCSoundManager.StopSound(value[i]);
			}
			m_dicLoopSounds.Remove(name);
		}
	}

	private void PlayLastMusicNotPlayed()
	{
		string text = "";
		while (m_qNextCutScen.Count > 0)
		{
			text = m_qNextCutScen.Dequeue();
		}
		string text2 = "";
		if (text == "" && m_NKCCutScenTemplet != null)
		{
			if (m_cNKCUICutState.m_EndBGMFileName.Length > 0)
			{
				text2 = m_cNKCUICutState.m_EndBGMFileName;
			}
			m_NextCutIndex++;
			while (m_NKCCutScenTemplet.m_listCutTemplet.Count > m_NextCutIndex)
			{
				NKCCutTemplet nKCCutTemplet = m_NKCCutScenTemplet.m_listCutTemplet[m_NextCutIndex];
				if (nKCCutTemplet != null)
				{
					if (nKCCutTemplet.m_StartBGMFileName.Length > 0)
					{
						text2 = nKCCutTemplet.m_StartBGMFileName;
					}
					if (nKCCutTemplet.m_EndBGMFileName.Length > 0)
					{
						text2 = nKCCutTemplet.m_EndBGMFileName;
					}
					if (nKCCutTemplet.m_Action == NKCCutTemplet.eCutsceneAction.PLAY_MUSIC)
					{
						string actionFirstToken = nKCCutTemplet.GetActionFirstToken();
						if (!string.IsNullOrEmpty(actionFirstToken))
						{
							text2 = actionFirstToken;
						}
					}
				}
				m_NextCutIndex++;
			}
		}
		else if (text.Length > 0)
		{
			NKCCutScenTemplet cutScenTemple = NKCCutScenManager.GetCutScenTemple(text);
			if (cutScenTemple != null)
			{
				text2 = cutScenTemple.GetLastMusicAssetName();
			}
		}
		if (text2 != null && text2.Length > 0)
		{
			NKCSoundManager.PlayMusic(text2, bLoop: true);
		}
	}

	private void StopPure()
	{
		PlayLastMusicNotPlayed();
		if (base.IsOpen)
		{
			Close();
			NKCCamera.EnableBloom(m_bWasBloom);
			NKCUIFadeInOut.Close();
		}
		m_NKCUICutScenUnitMgr.Close();
		m_bPlaying = false;
		m_NKCUICutScenTitleMgr.Close();
		m_NKCUICutScenBGMgr.Close();
		m_NKCUICutScenTalkBoxMgr.Close();
		m_NKCUICutScenImgMgr.Close();
		m_qNextCutScen.Clear();
		NKCCamera.SetEnableSepiaToneSubUILowCam(bSet: false);
	}

	private void CallBack()
	{
		if (m_Callback != null)
		{
			CutScenCallBack callback = m_Callback;
			m_Callback = null;
			callback();
		}
		m_Callback = null;
	}

	public void StopWithInvalidatingCallBack()
	{
		StopPure();
		if (m_NKCCutScenTemplet != null)
		{
			if (m_NKCCutScenTemplet.m_CutScenID == 60)
			{
				NKCPublisherModule.Notice.OpenPromotionalBanner(NKCPublisherModule.NKCPMNotice.eOptionalBannerPlaces.EP2Act2Clear, null);
			}
			else if (m_NKCCutScenTemplet.m_CutScenID == 81)
			{
				NKCPublisherModule.Notice.OpenPromotionalBanner(NKCPublisherModule.NKCPMNotice.eOptionalBannerPlaces.EP2Clear, null);
			}
		}
		m_Callback = null;
	}

	public void StopWithCallBack()
	{
		if (m_logViewer.gameObject.activeInHierarchy)
		{
			m_logViewer.OnClickClose();
			return;
		}
		StopPure();
		CallBack();
	}

	public void OnScroll(PointerEventData data)
	{
		if (!NKCUIManager.IsTopmostUI(this))
		{
			return;
		}
		if (data.scrollDelta.y > 0f)
		{
			if (m_LogBtn.gameObject.activeInHierarchy)
			{
				OnClickLogBtn();
			}
		}
		else if (data.scrollDelta.y < 0f)
		{
			OnClickedPlayer();
		}
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		switch (hotkey)
		{
		case HotkeyEventType.Confirm:
			OnClickedPlayer();
			return true;
		case HotkeyEventType.ShowHotkey:
			NKCUIComHotkeyDisplay.OpenInstance(m_trHotkeyHelpPosConfirm, HotkeyEventType.Confirm);
			NKCUIComHotkeyDisplay.OpenInstance(m_trHotkeyHelpPosSkip, HotkeyEventType.Skip);
			return false;
		default:
			return false;
		}
	}

	public void ValidateBySimulate()
	{
		while (m_bPlaying)
		{
			if (m_cNKCUICutState.m_bPlayVideo)
			{
				Skip();
				m_cNKCUICutState.m_bPlayVideo = false;
			}
			if (m_cNKCUICutState.m_bWaitSelection)
			{
				Skip();
				m_cNKCUICutState.m_bWaitSelection = false;
			}
			OnClickedPlayer();
		}
		NKCSoundManager.StopAllSound();
	}
}
