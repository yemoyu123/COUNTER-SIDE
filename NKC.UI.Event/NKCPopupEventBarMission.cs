using System;
using System.Collections;
using System.Collections.Generic;
using NKC.UI.NPC;
using NKM;
using NKM.Templet;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventBarMission : NKCUIBase
{
	[Serializable]
	public struct SDAnimatioSetup
	{
		public string aniName;

		public bool loop;
	}

	public delegate void OnClose();

	public static string ASSET_BUNDLE_NAME = "event_gremory_bar";

	public static string UI_ASSET_NAME = "POPUP_EVENT_GREMORY_BAR_MOMO";

	private static NKCPopupEventBarMission m_Instance;

	public LoopScrollRect m_LoopScrollRect;

	public EventTrigger m_eventTriggerBg;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnCompleteAll;

	public NKCUIComStateButton m_csbtnErrandRawardList;

	public NKCUIComMissionGroup m_comMissionGroup;

	public NKCUIEventBarMissionGroupList m_comMissionGroupList;

	public GameObject m_objErrandPanel;

	public Image m_imgErrandGauge;

	public NKCUINPCSpineIllust m_SpineIllust;

	public SkeletonGraphic m_SpineSD;

	public Animator m_aniGoErrandPopup;

	[Header("Spine UnitId")]
	public int m_unitId;

	public NKCASUIUnitIllust.eAnimation[] m_aniStandBy;

	public NKCASUIUnitIllust.eAnimation[] m_aniMissionNotCompleted;

	public NKCASUIUnitIllust.eAnimation[] m_aniErrandNotEnabled;

	public NKCASUIUnitIllust.eAnimation[] m_aniErrandCompleted;

	public bool allowRepeat;

	[Header("SD Animation")]
	public SDAnimatioSetup[] m_SDAnimation;

	[Header("대사 창")]
	public Animator m_aniScript;

	public CanvasGroup m_scriptCanvasGroup;

	public GameObject m_objScriptRoot;

	public Text m_lbUnitName;

	public Text m_lbScriptMsg;

	public float m_showScriptTime;

	public float m_scriptIdleTimer;

	public NKCUIComStateButton m_scriptButton;

	[Header("미션 슬롯 에셋 이름")]
	public string m_missionSlotBundleName;

	public string m_missionSlotAssetName;

	[Header("이벤트 상점 숏컷")]
	public NKCUIComStateButton m_csbtnEventShop;

	public string m_shortCutParam;

	private NKCUITypeWriter m_typeWriter = new NKCUITypeWriter();

	private List<NKMMissionTemplet> missionTempletList;

	private int m_errandRewardMissionTabId;

	private int m_cocktailRewardMissionGroupId;

	private int m_prevAniIndex;

	private long m_userUId;

	private float m_fScriptTimer;

	private float m_fScriptStanbyTimer;

	private Coroutine m_scriptCoroutine;

	private Coroutine m_goErrandCoroutine;

	private string m_bgmName;

	private float m_bgmVolume;

	private OnClose m_dOnClose;

	public static NKCPopupEventBarMission Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventBarMission>(ASSET_BUNDLE_NAME, UI_ASSET_NAME, NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventBarMission>();
				if (UI_ASSET_NAME == "POPUP_EVENT_GREMORY_BAR_MOMO")
				{
					m_Instance.InitUI("event_gremory_bar", "POPUP_EVENT_GREMORY_BAR_MOMO_REWARD_SLOT");
				}
				else
				{
					m_Instance.InitUI("ui_single_cafe", "POPUP_UI_SINGLE_CAFE_REWARD_SLOT");
				}
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

	public override string MenuName => "Momo Mission";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override bool WillCloseUnderPopupOnOpen => false;

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

	private void InitUI(string slotBundleName, string slotAssetName)
	{
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetPresetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnPresetSlot;
			m_LoopScrollRect.dOnProvideData += ProvidePresetData;
			m_LoopScrollRect.ContentConstraintCount = 1;
			m_LoopScrollRect.PrepareCells();
			m_LoopScrollRect.TotalCount = 0;
			m_LoopScrollRect.RefreshCells();
		}
		NKCUtil.SetScrollHotKey(m_LoopScrollRect);
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, OnClickClose);
		NKCUtil.SetButtonClickDelegate(m_csbtnCompleteAll, OnClickCompleteAll);
		NKCUtil.SetButtonClickDelegate(m_csbtnErrandRawardList, OnClickShowErrandRward);
		NKCUtil.SetButtonClickDelegate(m_scriptButton, OnClickScriptPanel);
		NKCUtil.SetButtonClickDelegate(m_csbtnEventShop, OnClickEventShop);
		m_comMissionGroup?.Init();
		m_comMissionGroupList?.Init(slotBundleName, slotAssetName);
		if (m_SpineIllust != null)
		{
			m_SpineIllust.m_dOnTouch = OnCharacterTouch;
		}
		base.gameObject.SetActive(value: false);
	}

	public override void CloseInternal()
	{
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
		if (m_goErrandCoroutine != null)
		{
			StopCoroutine(m_goErrandCoroutine);
			m_goErrandCoroutine = null;
		}
		if (m_dOnClose != null)
		{
			m_dOnClose();
		}
		HideScript();
		NKCUIVoiceManager.StopVoice();
		base.gameObject.SetActive(value: false);
	}

	public void Open(int errandRewardMissionTabId, int cocktailRewardMissionGroupId, OnClose onClose = null)
	{
		m_errandRewardMissionTabId = errandRewardMissionTabId;
		m_cocktailRewardMissionGroupId = cocktailRewardMissionGroupId;
		m_dOnClose = onClose;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool flag = nKMUserData == null || m_userUId != nKMUserData.m_UserUID;
		m_userUId = nKMUserData?.m_UserUID ?? 0;
		if (missionTempletList == null || flag)
		{
			missionTempletList = BuildAllMissionTempletListByTab(m_errandRewardMissionTabId);
		}
		NKCUtil.SetGameobjectActive(m_objErrandPanel, bValue: false);
		base.gameObject.SetActive(value: true);
		m_comMissionGroup?.SetData(m_cocktailRewardMissionGroupId, OnMomoGoErrand, OnMomoCannotGoErrand);
		m_comMissionGroupList?.CloseImmediately();
		RefreshScrollRect();
		int num = PlayRandomAnimation(m_aniStandBy);
		if (m_unitId > 0)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_LOBBY_CONNECT, m_unitId);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_unitId);
		NKCUtil.SetLabelText(m_lbUnitName, (unitTempletBase != null) ? unitTempletBase.GetUnitName() : "");
		ShowScript((num == 0) ? NKCUtilString.GET_STRING_GREMORY_MOMO_HELLO_1 : NKCUtilString.GET_STRING_GREMORY_MOMO_HELLO_2);
		m_bgmName = NKCSoundManager.CurrentMusicName;
		m_bgmVolume = NKCSoundManager.GetNowMusicLocalVolume();
		UIOpened();
	}

	public override void Hide()
	{
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
		if (m_goErrandCoroutine != null)
		{
			StopCoroutine(m_goErrandCoroutine);
			m_goErrandCoroutine = null;
		}
		HideScript();
		NKCUIVoiceManager.StopVoice();
		NKCUtil.SetGameobjectActive(m_objErrandPanel, bValue: false);
		base.Hide();
	}

	public override void UnHide()
	{
		base.UnHide();
		NKCSoundManager.PlayMusic(m_bgmName, bLoop: true, m_bgmVolume, bForce: true);
	}

	private void RefreshScrollRect()
	{
		if (!(m_LoopScrollRect == null) && missionTempletList != null)
		{
			m_LoopScrollRect.TotalCount = missionTempletList.Count;
			m_LoopScrollRect.SetIndexPosition(0);
			RefreshCompleteAllState();
		}
	}

	private void RefreshCompleteAllState()
	{
		bool flag = false;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			flag = NKCScenManager.CurrentUserData().m_MissionData.CheckCompletableMission(nKMUserData, m_errandRewardMissionTabId);
		}
		m_csbtnCompleteAll?.SetLock(!flag);
	}

	public void RefreshMission()
	{
		missionTempletList = BuildAllMissionTempletListByTab(m_errandRewardMissionTabId);
		RefreshScrollRect();
		m_comMissionGroup?.SetData(m_cocktailRewardMissionGroupId, OnMomoGoErrand, OnMomoCannotGoErrand);
		if (m_comMissionGroupList != null && m_comMissionGroupList.IsOpened())
		{
			m_comMissionGroupList.Refresh();
		}
	}

	public override void OnBackButton()
	{
		if (CanClose())
		{
			Close();
		}
	}

	private void Update()
	{
		m_typeWriter.Update();
		if ((m_objScriptRoot == null || !m_objScriptRoot.activeSelf) && (m_SpineIllust == null || m_SpineIllust.GetCurrentAnimationName() == "IDLE"))
		{
			m_fScriptStanbyTimer += Time.deltaTime;
			if (m_fScriptStanbyTimer >= m_scriptIdleTimer)
			{
				int num = PlayRandomAnimation(m_aniStandBy);
				if (num >= 0)
				{
					ShowScript((num == 0) ? NKCUtilString.GET_STRING_GREMORY_MOMO_HELLO_1 : NKCUtilString.GET_STRING_GREMORY_MOMO_HELLO_2);
				}
			}
		}
		if (Input.anyKeyDown)
		{
			m_fScriptStanbyTimer = 0f;
		}
	}

	private List<NKMMissionTemplet> BuildAllMissionTempletListByTab(int tabID)
	{
		List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(tabID);
		missionTempletListByType.Sort(NKMMissionManager.Comparer);
		return missionTempletListByType;
	}

	private void ShowScript(string message)
	{
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
			if (m_aniScript != null)
			{
				m_aniScript.Play("EVNET_GREMORY_BAR_GREMORY_SCRIPT_IDLE", -1, 0f);
			}
		}
		if (!(m_objScriptRoot == null))
		{
			NKCUtil.SetGameobjectActive(m_objScriptRoot, bValue: true);
			if (m_lbScriptMsg != null)
			{
				m_typeWriter.Start(m_lbScriptMsg, message, 0f, _bTalkAppend: false);
			}
			m_fScriptTimer = m_showScriptTime;
			if (m_scriptCoroutine == null)
			{
				m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
			}
		}
	}

	private void HideScript()
	{
		m_fScriptStanbyTimer = 0f;
		NKCUtil.SetGameobjectActive(m_objScriptRoot, bValue: false);
		NKCUtil.SetLabelText(m_lbScriptMsg, "");
	}

	private IEnumerator IOnShowRequestScript()
	{
		while (m_fScriptTimer > 0f)
		{
			m_fScriptTimer -= Time.deltaTime;
			yield return null;
		}
		yield return new WaitWhile(() => m_typeWriter.IsTyping());
		yield return new WaitWhile(() => NKCUIVoiceManager.IsPlayingVoice());
		if (m_aniScript != null)
		{
			m_aniScript?.SetTrigger("OUTRO");
		}
		if (m_scriptCanvasGroup != null)
		{
			yield return new WaitWhile(() => m_scriptCanvasGroup.alpha > 0f);
		}
		HideScript();
	}

	private int PlayRandomAnimation(NKCASUIUnitIllust.eAnimation[] animations)
	{
		int num = animations.Length;
		int num2 = -1;
		if (animations != null && num > 0 && m_SpineIllust != null)
		{
			num2 = GetRandomAnimation(num);
			if (num2 >= 0)
			{
				string animationName = NKCASUIUnitIllust.GetAnimationName(animations[num2]);
				m_SpineIllust.SetAnimation(animationName);
			}
		}
		return num2;
	}

	private int GetRandomAnimation(int aniCount)
	{
		if (aniCount <= 0)
		{
			return -1;
		}
		int num = -1;
		if (allowRepeat)
		{
			int num2 = UnityEngine.Random.Range(0, aniCount);
			if (num2 < aniCount)
			{
				m_prevAniIndex = num2;
				num = num2;
			}
		}
		else
		{
			List<int> list = new List<int>();
			for (int i = 0; i < aniCount; i++)
			{
				if (i != m_prevAniIndex)
				{
					list.Add(i);
				}
			}
			int num3 = UnityEngine.Random.Range(0, list.Count);
			if (num3 < list.Count)
			{
				num = list[num3];
			}
			if (aniCount == 1)
			{
				num = 0;
			}
			if (num >= 0 && num < aniCount)
			{
				m_prevAniIndex = num;
			}
		}
		return num;
	}

	private void OnCharacterTouch()
	{
		PlayTouchVoice();
	}

	private void PlayTouchVoice()
	{
		List<NKCVoiceTemplet> voiceTempletByType = NKCUIVoiceManager.GetVoiceTempletByType(VOICE_TYPE.VT_TOUCH);
		List<NKCVoiceTemplet> list = voiceTempletByType.FindAll((NKCVoiceTemplet v) => v.Condition == VOICE_CONDITION.VC_NONE);
		if (list.Count <= 0)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_unitId);
		if (unitTempletBase == null)
		{
			return;
		}
		int skinID = 0;
		List<NKCVoiceTemplet> list2 = new List<NKCVoiceTemplet>();
		VOICE_BUNDLE flag = VOICE_BUNDLE.UNIT | VOICE_BUNDLE.COMMON;
		string unitStrID = unitTempletBase.m_UnitStrID;
		string baseUnitStrID = "";
		if (unitTempletBase.BaseUnit != null)
		{
			baseUnitStrID = unitTempletBase.BaseUnit.m_UnitStrID;
		}
		list2 = list.FindAll((NKCVoiceTemplet v) => NKCUIVoiceManager.CheckAsset(unitStrID, 0, v.FileName, flag, IgnoreVoiceBundleCheck: false));
		if (list2.Count == 0 && !string.IsNullOrEmpty(baseUnitStrID))
		{
			list2 = list.FindAll((NKCVoiceTemplet v) => NKCUIVoiceManager.CheckAsset(baseUnitStrID, 0, v.FileName, flag, IgnoreVoiceBundleCheck: false));
			if (list2.Count > 0)
			{
				unitStrID = baseUnitStrID;
			}
		}
		if (list2.Count <= 0)
		{
			return;
		}
		if (list2.Exists((NKCVoiceTemplet v) => v.Condition != VOICE_CONDITION.VC_NONE))
		{
			list2 = list2.FindAll((NKCVoiceTemplet v) => v.Condition != VOICE_CONDITION.VC_NONE);
		}
		int num = NKMRandom.Range(0, list2.Count);
		if (num < list2.Count)
		{
			string voiceCaption = NKCUtilString.GetVoiceCaption(NKCUIVoiceManager.PlayOnUI(unitStrID, skinID, voiceTempletByType[num].FileName, 100f, flag));
			if (!string.IsNullOrEmpty(voiceCaption))
			{
				ShowScript(voiceCaption);
			}
		}
	}

	private RectTransform GetPresetSlot(int index)
	{
		return NKCUIMissionAchieveSlot.GetNewInstance(null, m_missionSlotBundleName, m_missionSlotAssetName)?.GetComponent<RectTransform>();
	}

	private void ReturnPresetSlot(Transform tr)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	private void ProvidePresetData(Transform tr, int index)
	{
		NKCUIMissionAchieveSlot component = tr.GetComponent<NKCUIMissionAchieveSlot>();
		if (!(component == null))
		{
			component.SetData(missionTempletList[index], OnClickMove, OnClickComplete, null, OnClickLocked);
		}
	}

	private bool CanClose()
	{
		if (m_objErrandPanel.activeSelf)
		{
			return false;
		}
		if (!m_comMissionGroupList.IsClosed())
		{
			m_comMissionGroupList.Close();
			return false;
		}
		return true;
	}

	private void OnClickMove(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (cNKCUIMissionAchieveSlot == null)
		{
			return;
		}
		NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (nKMMissionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(nKMMissionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, NKCScenManager.CurrentUserData()))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, RefreshMission, NKCUtilString.GET_STRING_CONFIRM);
			}
			else
			{
				NKCContentManager.MoveToShortCut(nKMMissionTemplet.m_ShortCutType, nKMMissionTemplet.m_ShortCut);
			}
		}
	}

	private void OnClickComplete(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (cNKCUIMissionAchieveSlot == null)
		{
			return;
		}
		NKMMissionTemplet nKMMissionTemplet = cNKCUIMissionAchieveSlot.GetNKMMissionTemplet();
		if (nKMMissionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(nKMMissionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			if (NKMMissionManager.IsMissionTabExpired(missionTabTemplet, NKCScenManager.CurrentUserData()))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MISSION_EXPIRED, RefreshMission, NKCUtilString.GET_STRING_CONFIRM);
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(cNKCUIMissionAchieveSlot.GetNKMMissionTemplet());
			}
		}
	}

	private void OnClickLocked(NKCUIMissionAchieveSlot cNKCUIMissionAchieveSlot)
	{
		if (!(cNKCUIMissionAchieveSlot == null))
		{
			NKCUIVoiceManager.StopVoice();
			ShowScript(NKCUtilString.GET_STRING_GREMORY_MOMO_IGNORE_MISSION);
			PlayRandomAnimation(m_aniMissionNotCompleted);
		}
	}

	private void OnClickCompleteAll()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKMUserMissionData missionData = NKCScenManager.GetScenManager().GetMyUserData().m_MissionData;
			if (missionData != null && missionData.CheckCompletableMission(myUserData, m_errandRewardMissionTabId))
			{
				NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_ALL_REQ(m_errandRewardMissionTabId);
			}
		}
	}

	private void OnClickShowErrandRward()
	{
		if (m_comMissionGroupList.IsOpened())
		{
			m_comMissionGroupList.Close();
		}
		else
		{
			m_comMissionGroupList.Open(NKCUIEventBarMissionGroupList.MissionType.MissionGroupId, m_cocktailRewardMissionGroupId);
		}
	}

	private void OnMomoGoErrand(NKMMissionTemplet missionTemplet)
	{
		if (missionTemplet != null)
		{
			m_goErrandCoroutine = StartCoroutine(IMomoGoErrand(missionTemplet));
		}
	}

	private IEnumerator IMomoGoErrand(NKMMissionTemplet missionTemplet)
	{
		NKCUtil.SetImageFillAmount(m_imgErrandGauge, 0f);
		NKCUtil.SetGameobjectActive(m_objErrandPanel, bValue: true);
		if (m_aniGoErrandPopup != null)
		{
			float startTime = m_aniGoErrandPopup.GetCurrentAnimatorStateInfo(0).normalizedTime;
			if (m_SDAnimation != null && m_SDAnimation.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, m_SDAnimation.Length);
				m_SpineSD?.AnimationState.SetAnimation(0, m_SDAnimation[num].aniName, m_SDAnimation[num].loop);
			}
			if (m_imgErrandGauge != null)
			{
				while (m_imgErrandGauge.fillAmount < 1f)
				{
					yield return null;
				}
			}
			NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(missionTemplet);
			while (m_aniGoErrandPopup.GetCurrentAnimatorStateInfo(0).normalizedTime - startTime < 1f)
			{
				yield return null;
			}
		}
		NKCUtil.SetGameobjectActive(m_objErrandPanel, bValue: false);
	}

	private void OnMomoCannotGoErrand(bool allMissionCompleted)
	{
		NKCUIVoiceManager.StopVoice();
		if (allMissionCompleted)
		{
			ShowScript(NKCUtilString.GET_STRING_GREMORY_MOMO_COMPLETE_ERRAND);
			PlayRandomAnimation(m_aniErrandCompleted);
		}
		else
		{
			ShowScript(NKCUtilString.GET_STRING_GREMORY_MOMO_IGNORE_ERRAND);
			PlayRandomAnimation(m_aniErrandNotEnabled);
		}
		m_fScriptTimer = m_showScriptTime;
		if (m_scriptCoroutine == null)
		{
			m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
		}
	}

	private void OnClickClose()
	{
		if (CanClose())
		{
			Close();
		}
	}

	private void OnClickScriptPanel()
	{
		if (m_typeWriter.IsTyping())
		{
			m_typeWriter.Finish();
			return;
		}
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
		HideScript();
	}

	private void OnClickEventShop()
	{
		if (!string.IsNullOrEmpty(m_shortCutParam))
		{
			NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_SHOP, m_shortCutParam);
		}
	}

	private void OnDestroy()
	{
		if (missionTempletList != null)
		{
			missionTempletList.Clear();
			missionTempletList = null;
		}
	}
}
