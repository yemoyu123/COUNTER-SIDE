using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Event;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventRacePlay : NKCUIBase
{
	private enum RaceState
	{
		Ready,
		Play,
		End
	}

	public class NKCRaceEventData
	{
		public NKCAnimationActor m_Actor;

		public int m_capacity;

		public int m_startPoint;

		public NKCAnimationInstance m_AniSet;
	}

	private const string ASSET_BUNDLE_NAME = "UI_SINGLE_RACE";

	private const string UI_ASSET_NAME = "UI_SINGLE_RACE_PLAY";

	private static NKCPopupEventRacePlay m_Instance;

	[Header("Setting")]
	public string m_teamA_SDUnitName;

	public string m_teamB_SDUnitName;

	public string m_strBubbleHappyTeamA = "RACE_BUBBLE_2021_01_RED_01";

	public string m_strBubbleSadTeamA = "RACE_BUBBLE_2021_01_RED_02";

	public string m_strBubbleHappyTeamB = "RACE_BUBBLE_2021_01_BLUE_01";

	public string m_strBubbleSadTeamB = "RACE_BUBBLE_2021_01_BLUE_02";

	public string m_TameAUnitImageType = "SKIN";

	public int m_TeamAUnit = 117816;

	public string m_TameBUnitImageType = "SKIN";

	public int m_TeamBUnit = 116616;

	public List<Transform> m_lstEventPoint = new List<Transform>();

	public Animator m_AnimatorIntro;

	public Animator m_AnimatorGoal;

	public Image m_imgGoal;

	private const string ANIMATION_INTRO = "NKM_UI_EVENT_RACE_HUD_INTRO_INTRO";

	private const string ANIMATION_COUNTDOWN = "NKM_UI_EVENT_RACE_HUD_INTRO_COUNTDOWN";

	private const string ANIMATION_OUTRO = "NKM_UI_EVENT_RACE_HUD_INTRO_OUTRO";

	private const string ANIMATION_GOAL = "NKM_UI_EVENT_RACE_HUD_GOAL";

	private const string GOAL_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_RACE_SPRITE";

	private const string GOAL_RED = "NKM_UI_EVENT_RACE_HUD_GOAL_RED";

	private const string GOAL_BLUE = "NKM_UI_EVENT_RACE_HUD_GOAL_BLUE";

	public NKCASUISpineIllust m_UnitRed;

	public NKCASUISpineIllust m_UnitBlue;

	public NKCAnimationActor m_AnimationActor1;

	public NKCAnimationActor m_AnimationActor2;

	public Transform m_trMoveObjParent;

	public GameObject m_objSelectMark;

	public Image m_imgSelectMark;

	public NKCUIComStateButton m_btnSkip;

	public GameObject m_objBubbleLeft;

	public GameObject m_objBubbleRight;

	public Image m_imgBubbleLeft;

	public Image m_imgBubbleRight;

	[Header("\ufffd\u05fd\ufffdƮ\ufffdڵ\ufffd")]
	public int testIndexKey = 17;

	public EventBetTeam testTeam = EventBetTeam.TeamB;

	public bool btestWin = true;

	public float m_lerpValue = 2f;

	public float m_CountdownWaitSeconds = 3.3f;

	public float m_fArrowPosY = 320f;

	private const string ARROW_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_RACE_SPRITE";

	private const string BLUE_ARROW_SPRITE_NAME = "NKM_UI_EVENT_RACE_HUD_TEAM_BLUE";

	private const string RED_ARROW_SPRITE_NAME = "NKM_UI_EVENT_RACE_HUD_TEAM_RED";

	private NKCAnimationActor m_MyActor;

	private NKCAnimationActor m_OtherActor;

	private Dictionary<NKCASUIUnitIllust, Queue<List<NKCRaceEventData>>> m_dicRaceEventSet = new Dictionary<NKCASUIUnitIllust, Queue<List<NKCRaceEventData>>>();

	private Queue<List<NKCRaceEventData>> m_queueDataWin = new Queue<List<NKCRaceEventData>>();

	private Queue<List<NKCRaceEventData>> m_queueDataLose = new Queue<List<NKCRaceEventData>>();

	private List<NKCRaceEventData> m_lstUsedEventData = new List<NKCRaceEventData>();

	private List<NKCASUIUnitIllust> m_lstUsedObject = new List<NKCASUIUnitIllust>();

	private bool m_bUpdate;

	private bool m_bUserWin = true;

	private bool m_bFirstGoal = true;

	private EventBetTeam m_SelectedTeam;

	private NKMRewardData m_RewardData;

	private int m_iRaceIndex;

	private bool m_bMinimumGuarantee;

	public static NKCPopupEventRacePlay Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventRacePlay>("UI_SINGLE_RACE", "UI_SINGLE_RACE_PLAY", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventRacePlay>();
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

	public void InitUI()
	{
		NKCUtil.SetBindFunction(m_btnSkip, OnClickSkip);
	}

	public void TestRun()
	{
		NKMRewardData rewardData = new NKMRewardData();
		Open(testIndexKey, rewardData, btestWin, testTeam, bMinimumGuarantee: false);
	}

	public void Open(int iRaceIndex, NKMRewardData rewardData, bool bUserWin, EventBetTeam selectTeam, bool bMinimumGuarantee)
	{
		m_teamA_SDUnitName = GetSDUnitName(m_TameAUnitImageType, m_TeamAUnit);
		m_teamB_SDUnitName = GetSDUnitName(m_TameBUnitImageType, m_TeamBUnit);
		if (!NKCEventRaceAnimationManager.DataExist)
		{
			NKCEventRaceAnimationManager.LoadFromLua();
		}
		if (!NKCAnimationEventManager.DataExist)
		{
			NKCAnimationEventManager.LoadFromLua();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCSoundManager.StopAllSound();
		NKCSoundManager.StopMusic();
		NKCSoundManager.PlayMusic("UI_SPORT_RACE_02", bLoop: true);
		m_AnimatorIntro.Play("NKM_UI_EVENT_RACE_HUD_INTRO_INTRO");
		m_bUserWin = bUserWin;
		m_RewardData = rewardData;
		m_SelectedTeam = selectTeam;
		m_iRaceIndex = iRaceIndex;
		m_bMinimumGuarantee = bMinimumGuarantee;
		PrepareRace();
		UIOpened();
	}

	private List<int> GetRaceRandomKey()
	{
		return new List<int>
		{
			0,
			NKCSynchronizedTime.ServiceTime.Month,
			NKCSynchronizedTime.ServiceTime.Day,
			NKCScenManager.CurrentUserData().GetRaceData().CurEventID,
			NKCScenManager.CurrentUserData().GetRaceData().CurRaceIndex,
			ReverseNum(NKCSynchronizedTime.ServiceTime.Month),
			ReverseNum(NKCSynchronizedTime.ServiceTime.Day),
			ReverseNum(NKCScenManager.CurrentUserData().GetRaceData().CurEventID),
			ReverseNum(NKCScenManager.CurrentUserData().GetRaceData().CurRaceIndex)
		};
	}

	private int ReverseNum(int iNum)
	{
		int num = 0;
		while (iNum != 0)
		{
			int num2 = iNum % 10;
			num = num * 10 + num2;
			iNum /= 10;
		}
		return num;
	}

	private void PrepareRace()
	{
		ResetPosition();
		ResetData();
		float totalTime = 0f;
		float totalTime2 = 0f;
		List<int> raceRandomKey = GetRaceRandomKey();
		Debug.Log("<color=red>MakeRaceEventList : lstRaceEventType</color>");
		List<RaceEventType> list = MakeRaceEventList(out totalTime, raceRandomKey, 0);
		Debug.Log("<color=red>MakeRaceEventList2 : lstRaceEventType</color>");
		List<RaceEventType> list2 = MakeRaceEventList(out totalTime2, raceRandomKey, 1);
		while (totalTime == totalTime2)
		{
			list2 = MakeRaceEventList(out totalTime2, raceRandomKey, 1);
		}
		List<RaceEventType> list3 = ((totalTime < totalTime2) ? list : list2);
		List<RaceEventType> list4 = ((totalTime > totalTime2) ? list : list2);
		if (m_UnitRed != null)
		{
			m_UnitRed.Unload();
			m_UnitRed = null;
		}
		m_UnitRed = NKCResourceUtility.OpenSpineSD(m_teamA_SDUnitName);
		if (m_UnitRed != null)
		{
			m_UnitRed.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			m_UnitRed.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true);
			m_UnitRed.SetParent(GetActorByTeam(EventBetTeam.TeamA).m_trSDParent, worldPositionStays: false);
			RectTransform rectTransform = m_UnitRed.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector3.zero;
				rectTransform.localScale = Vector3.one;
			}
			GetActorByTeam(EventBetTeam.TeamA).SetSpineIllust(m_UnitRed);
		}
		if (m_UnitBlue != null)
		{
			m_UnitBlue.Unload();
			m_UnitBlue = null;
		}
		m_UnitBlue = NKCResourceUtility.OpenSpineSD(m_teamB_SDUnitName);
		if (m_UnitBlue != null)
		{
			m_UnitBlue.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			m_UnitBlue.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true);
			m_UnitBlue.SetParent(GetActorByTeam(EventBetTeam.TeamB).m_trSDParent, worldPositionStays: false);
			RectTransform rectTransform2 = m_UnitBlue.GetRectTransform();
			if (rectTransform2 != null)
			{
				rectTransform2.localPosition = Vector3.zero;
				rectTransform2.localScale = Vector3.one;
			}
			GetActorByTeam(EventBetTeam.TeamB).SetSpineIllust(m_UnitBlue);
		}
		if (m_SelectedTeam == EventBetTeam.TeamA)
		{
			m_MyActor = m_AnimationActor1;
			m_OtherActor = m_AnimationActor2;
		}
		else
		{
			m_MyActor = m_AnimationActor2;
			m_OtherActor = m_AnimationActor1;
		}
		NKCUtil.SetGameobjectActive(m_AnimationActor1, bValue: true);
		NKCUtil.SetGameobjectActive(m_AnimationActor2, bValue: true);
		m_queueDataWin.Clear();
		m_queueDataLose.Clear();
		m_dicRaceEventSet.Clear();
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		for (int i = 0; i < list3.Count; i++)
		{
			int capacity = NKCEventRaceAnimationManager.GetCapacity(list3[i]);
			List<NKCRaceEventData> item = MakeEventSet(list3[i], GetActorByWin(bWin: true).GetSpineIllust(), GetActorByWin(bWin: true), num, capacity);
			m_queueDataWin.Enqueue(item);
			num += capacity;
			stringBuilder.Append($"{list3[i]} - ");
		}
		m_dicRaceEventSet.Add(GetActorByWin(bWin: true).GetSpineIllust(), m_queueDataWin);
		Log.Debug("WinData  : " + stringBuilder.ToString(), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Event/NKCPopupEventRacePlay.cs", 343);
		stringBuilder.Clear();
		num = 0;
		for (int j = 0; j < list4.Count; j++)
		{
			int capacity2 = NKCEventRaceAnimationManager.GetCapacity(list4[j]);
			List<NKCRaceEventData> item2 = MakeEventSet(list4[j], GetActorByWin(bWin: false).GetSpineIllust(), GetActorByWin(bWin: false), num, capacity2);
			m_queueDataLose.Enqueue(item2);
			num += capacity2;
			stringBuilder.Append($"{list4[j]} - ");
		}
		m_dicRaceEventSet.Add(GetActorByWin(bWin: false).GetSpineIllust(), m_queueDataLose);
		Log.Debug("LoseData  : " + stringBuilder.ToString(), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Event/NKCPopupEventRacePlay.cs", 357);
		stringBuilder.Clear();
		if (m_SelectedTeam == EventBetTeam.TeamA)
		{
			m_objSelectMark.transform.SetParent(GetActorByTeam(EventBetTeam.TeamA).transform);
			NKCUtil.SetImageSprite(m_imgSelectMark, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", "NKM_UI_EVENT_RACE_HUD_TEAM_RED"));
		}
		else
		{
			m_objSelectMark.transform.SetParent(GetActorByTeam(EventBetTeam.TeamB).transform);
			NKCUtil.SetImageSprite(m_imgSelectMark, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", "NKM_UI_EVENT_RACE_HUD_TEAM_BLUE"));
		}
		NKCUtil.SetGameobjectActive(m_objSelectMark, bValue: true);
		m_objSelectMark.transform.localPosition = new Vector3(0f, m_fArrowPosY, 0f);
		m_AnimationActor1.transform.SetAsLastSibling();
		m_AnimationActor2.transform.SetAsLastSibling();
		StopAllCoroutines();
		StartCoroutine(StartRace());
	}

	private void ResetData()
	{
		m_bUpdate = false;
		m_bFirstGoal = true;
		foreach (KeyValuePair<NKCASUIUnitIllust, Queue<List<NKCRaceEventData>>> item in m_dicRaceEventSet)
		{
			if (item.Value != null && item.Value.Count > 0)
			{
				List<NKCRaceEventData> list = item.Value.Dequeue();
				for (int i = 0; i < list.Count; i++)
				{
					list[i].m_AniSet.Close();
				}
			}
		}
		m_dicRaceEventSet.Clear();
		for (int j = 0; j < m_lstUsedEventData.Count; j++)
		{
			m_lstUsedEventData[j].m_AniSet.Close();
		}
		m_lstUsedEventData.Clear();
		m_queueDataWin.Clear();
		m_queueDataLose.Clear();
		m_AnimationActor1.transform.localPosition = Vector3.zero;
		m_AnimationActor2.transform.localPosition = Vector3.zero;
	}

	public override void OnBackButton()
	{
	}

	public override void CloseInternal()
	{
		ResetData();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCSoundManager.StopAllSound();
		NKCSoundManager.StopMusic();
		NKCSoundManager.PlayScenMusic(NKCScenManager.GetScenManager().GetNowScenID());
	}

	private void Update()
	{
		if (!m_bUpdate || m_dicRaceEventSet.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<NKCASUIUnitIllust, Queue<List<NKCRaceEventData>>> item in m_dicRaceEventSet)
		{
			Queue<List<NKCRaceEventData>> value = item.Value;
			if (value.Count == 0)
			{
				continue;
			}
			if (item.Key == m_MyActor.GetSpineIllust())
			{
				m_trMoveObjParent.localPosition = new Vector3(0f - Mathf.Lerp(0f - m_trMoveObjParent.localPosition.x, m_MyActor.transform.localPosition.x, m_lerpValue * Time.deltaTime), m_trMoveObjParent.localPosition.y, m_trMoveObjParent.localPosition.z);
			}
			m_objSelectMark.transform.position = m_MyActor.m_trSDParent.position + new Vector3(0f, m_fArrowPosY, 0f);
			CheckBubble();
			List<NKCRaceEventData> list = value.Peek();
			if (list == null)
			{
				continue;
			}
			for (int i = 0; i < list.Count; i++)
			{
				NKCRaceEventData nKCRaceEventData = list[i];
				nKCRaceEventData.m_AniSet.Update(Time.deltaTime);
				if (!nKCRaceEventData.m_AniSet.IsFinished())
				{
					continue;
				}
				nKCRaceEventData.m_AniSet.RemoveEffect();
				m_lstUsedEventData.AddRange(value.Dequeue());
				Debug.LogWarning("Event End");
				if (value.Count == 0)
				{
					if (item.Key == m_MyActor.GetSpineIllust() == m_bUserWin)
					{
						item.Key.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_WIN);
						item.Key.SetTimeScale(1f);
					}
					else
					{
						item.Key.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
						item.Key.SetTimeScale(1f);
					}
					if (m_bFirstGoal)
					{
						m_bFirstGoal = false;
						StopAllCoroutines();
						StartCoroutine(EndRace());
					}
					else
					{
						NKCUtil.SetGameobjectActive(m_AnimatorIntro.gameObject, bValue: false);
						m_bUpdate = false;
					}
				}
				break;
			}
		}
	}

	private void CheckBubble()
	{
		_ = NKCScenManager.CurrentUserData().GetRaceData().CurEventID;
		if (Mathf.Abs(0f - m_trMoveObjParent.localPosition.x - m_OtherActor.transform.localPosition.x) > (float)(Screen.width / 2))
		{
			if (0f - m_trMoveObjParent.localPosition.x > m_OtherActor.transform.localPosition.x)
			{
				NKCUtil.SetGameobjectActive(m_objBubbleLeft, bValue: true);
				NKCUtil.SetGameobjectActive(m_objBubbleRight, bValue: false);
				if (m_SelectedTeam == EventBetTeam.TeamA)
				{
					NKCUtil.SetImageSprite(m_imgBubbleLeft, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_BUBBLE", m_strBubbleSadTeamB));
				}
				else
				{
					NKCUtil.SetImageSprite(m_imgBubbleLeft, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_BUBBLE", m_strBubbleSadTeamA));
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBubbleLeft, bValue: false);
				NKCUtil.SetGameobjectActive(m_objBubbleRight, bValue: true);
				if (m_SelectedTeam == EventBetTeam.TeamA)
				{
					NKCUtil.SetImageSprite(m_imgBubbleRight, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_BUBBLE", m_strBubbleHappyTeamB));
				}
				else
				{
					NKCUtil.SetImageSprite(m_imgBubbleRight, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_BUBBLE", m_strBubbleHappyTeamA));
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objBubbleLeft, bValue: false);
			NKCUtil.SetGameobjectActive(m_objBubbleRight, bValue: false);
		}
	}

	private IEnumerator EndRace()
	{
		NKCUtil.SetGameobjectActive(m_AnimatorIntro.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_AnimatorGoal.gameObject, bValue: true);
		if (m_SelectedTeam == EventBetTeam.TeamA)
		{
			if (m_bUserWin)
			{
				NKCUtil.SetImageSprite(m_imgGoal, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", "NKM_UI_EVENT_RACE_HUD_GOAL_RED"));
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgGoal, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", "NKM_UI_EVENT_RACE_HUD_GOAL_BLUE"));
			}
		}
		else if (!m_bUserWin)
		{
			NKCUtil.SetImageSprite(m_imgGoal, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", "NKM_UI_EVENT_RACE_HUD_GOAL_RED"));
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgGoal, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", "NKM_UI_EVENT_RACE_HUD_GOAL_BLUE"));
		}
		m_AnimatorGoal.Play("NKM_UI_EVENT_RACE_HUD_GOAL");
		yield return new WaitForSeconds(2.5f);
		while (m_bUpdate)
		{
			yield return null;
		}
		NKCSoundManager.StopAllSound();
		NKCSoundManager.StopMusic();
		Close();
		if (m_RewardData != null)
		{
			NKCPopupEventRaceResultV2.Instance.Open(m_iRaceIndex, m_RewardData, m_bUserWin, m_SelectedTeam, m_bMinimumGuarantee);
		}
	}

	private void OnClickSkip()
	{
		m_bUpdate = false;
		m_bFirstGoal = false;
		float x = m_lstEventPoint[m_lstEventPoint.Count - 1].localPosition.x;
		m_AnimationActor1.transform.localPosition = new Vector3(x, 0f, 0f);
		m_AnimationActor1.m_Ani.Play("UNIT_EVENT_RACE_SD_BASE");
		m_AnimationActor2.transform.localPosition = new Vector3(x, 0f, 0f);
		m_AnimationActor2.m_Ani.Play("UNIT_EVENT_RACE_SD_BASE");
		foreach (KeyValuePair<NKCASUIUnitIllust, Queue<List<NKCRaceEventData>>> item in m_dicRaceEventSet)
		{
			if (item.Key == m_MyActor.GetSpineIllust() == m_bUserWin)
			{
				item.Key.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_WIN);
			}
			else
			{
				item.Key.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			}
		}
		m_trMoveObjParent.localPosition = new Vector3(0f - m_MyActor.transform.localPosition.x, m_trMoveObjParent.localPosition.y, m_trMoveObjParent.localPosition.z);
		CheckBubble();
		StopAllCoroutines();
		StartCoroutine(EndRace());
	}

	private static string GetSDUnitName(string type, int id)
	{
		if (type != null && type == "SKIN")
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(id);
			if (skinTemplet == null)
			{
				return "";
			}
			return skinTemplet.m_SpineSDName;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(id);
		if (unitTempletBase == null)
		{
			return "";
		}
		return unitTempletBase.m_SpineSDName;
	}

	private void ResetPosition()
	{
		m_bUpdate = false;
		NKCEventRaceAnimationManager.LoadFromLua();
		NKCAnimationEventManager.LoadFromLua();
		m_objSelectMark.transform.SetParent(base.transform);
		NKCUtil.SetGameobjectActive(m_objSelectMark, bValue: false);
		NKCUtil.SetGameobjectActive(m_AnimatorIntro.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_AnimatorGoal.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBubbleLeft, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBubbleRight, bValue: false);
		if (m_UnitRed != null)
		{
			m_UnitRed.Unload();
			m_UnitRed = null;
		}
		if (m_UnitBlue != null)
		{
			m_UnitBlue.Unload();
			m_UnitBlue = null;
		}
		for (int i = 0; i < m_lstUsedObject.Count; i++)
		{
			m_lstUsedObject[i].Unload();
			m_lstUsedObject[i] = null;
		}
		m_lstUsedObject.Clear();
		m_trMoveObjParent.localPosition = Vector3.zero;
		if (m_AnimatorIntro != null)
		{
			m_AnimatorIntro.Play("NKM_UI_EVENT_RACE_HUD_INTRO_INTRO");
		}
		NKCUtil.SetGameobjectActive(m_AnimationActor1, bValue: false);
		NKCUtil.SetGameobjectActive(m_AnimationActor2, bValue: false);
		NKCUtil.SetGameobjectActive(m_AnimatorIntro.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_AnimatorGoal.gameObject, bValue: false);
		ResetData();
	}

	private NKCAnimationActor GetActorByTeam(EventBetTeam _SelectTeam)
	{
		if (_SelectTeam == EventBetTeam.TeamA)
		{
			return m_AnimationActor1;
		}
		return m_AnimationActor2;
	}

	private NKCAnimationActor GetActorByWin(bool bWin)
	{
		if (m_bUserWin == bWin)
		{
			return m_MyActor;
		}
		if (m_AnimationActor1 == m_MyActor)
		{
			return m_AnimationActor2;
		}
		return m_AnimationActor1;
	}

	private List<RaceEventType> MakeRaceEventList(out float totalTime, List<int> lstRaceKey, int iLineNumber)
	{
		List<RaceEventType> list = new List<RaceEventType>();
		int num = 0;
		int num2 = m_lstEventPoint.Count - 1;
		totalTime = 0f;
		while (num < m_lstEventPoint.Count - 1)
		{
			List<RaceEventType> usableRaceType = GetUsableRaceType(list, num, num2);
			if (usableRaceType.Count == 0)
			{
				Debug.LogError($"UsableType is null!! - StartIdx : {num}, remainCapacity : {num2}");
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < m_lstEventPoint.Count; i++)
				{
					stringBuilder.Append($" {m_lstEventPoint[i]}");
				}
				Debug.LogError(stringBuilder.ToString());
				break;
			}
			int num3 = 0;
			num3 = ((usableRaceType.Count != 1) ? ((lstRaceKey.Count <= num) ? (usableRaceType.Count / 2 - iLineNumber + num) : (lstRaceKey[num] % usableRaceType.Count - iLineNumber + num)) : 0);
			num3 = Mathf.Max(0, num3);
			if (num3 > usableRaceType.Count - 1)
			{
				num3 = usableRaceType.Count / 2 + iLineNumber;
			}
			RaceEventType raceEventType = usableRaceType[num3];
			list.Add(raceEventType);
			totalTime += NKCEventRaceAnimationManager.GetTotalTime(raceEventType);
			int capacity = NKCEventRaceAnimationManager.GetCapacity(raceEventType);
			num += capacity;
			num2 -= capacity;
		}
		return list;
	}

	private List<RaceEventType> GetUsableRaceType(List<RaceEventType> curRaceEventType, int startIndex, int remainCapacity)
	{
		List<RaceEventType> list = new List<RaceEventType>();
		for (int i = 0; i < 14; i++)
		{
			RaceEventType targetType = (RaceEventType)i;
			if (NKCEventRaceAnimationManager.Find(targetType) == null)
			{
				continue;
			}
			List<RaceEventType> list2 = curRaceEventType.FindAll((RaceEventType x) => x == targetType);
			if ((list2 == null || list2.Count < NKCEventRaceAnimationManager.GetMaxCount(targetType)) && startIndex >= NKCEventRaceAnimationManager.GetMinIndex(targetType) && startIndex <= NKCEventRaceAnimationManager.GetMaxIndex(targetType))
			{
				int capacity = NKCEventRaceAnimationManager.GetCapacity(targetType);
				if (remainCapacity >= capacity)
				{
					list.Add(targetType);
				}
			}
		}
		return list;
	}

	private List<NKCRaceEventData> MakeEventSet(RaceEventType eventType, NKCASUIUnitIllust mainChar, NKCAnimationActor actor, int startpoint, int capacity)
	{
		List<NKCRaceEventData> list = new List<NKCRaceEventData>();
		List<NKCEventRaceAnimationTemplet> list2 = NKCEventRaceAnimationManager.Find(eventType);
		for (int i = 0; i < list2.Count; i++)
		{
			NKCRaceEventData nKCRaceEventData = new NKCRaceEventData();
			List<NKCAnimationEventTemplet> list3 = NKCAnimationEventManager.Find(list2[i].m_AnimationEventSetID);
			if (list3 == null)
			{
				Log.Error("NKCAnimationEventTemplet is null - " + list2[i].m_AnimationEventSetID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Event/NKCPopupEventRacePlay.cs", 787);
				continue;
			}
			if (string.IsNullOrEmpty(list2[i].m_TargetObjName))
			{
				nKCRaceEventData.m_Actor = actor;
				actor.SetSpineIllust(mainChar);
			}
			else
			{
				string[] array = list2[i].m_TargetObjName.Split('@');
				if (array.Length == 2)
				{
					NKCASUISpineIllust nKCASUISpineIllust = NKCResourceUtility.OpenSpineSD(array[0], array[1]);
					nKCASUISpineIllust.SetParent(actor.transform.parent, worldPositionStays: false);
					RectTransform rectTransform = nKCASUISpineIllust.GetRectTransform();
					if (rectTransform != null)
					{
						NKCAnimationEventTemplet nKCAnimationEventTemplet = list3.Find((NKCAnimationEventTemplet x) => x.m_AniEventType == AnimationEventType.ANIMATION_SPINE && x.m_StartTime == 0f);
						if (nKCAnimationEventTemplet != null)
						{
							nKCASUISpineIllust.SetDefaultAnimation((NKCASUIUnitIllust.eAnimation)Enum.Parse(typeof(NKCASUIUnitIllust.eAnimation), nKCAnimationEventTemplet.m_StrValue));
						}
						Vector3 localPosition = NKCUtil.Lerp(m_lstEventPoint[startpoint].localPosition, m_lstEventPoint[startpoint + capacity].localPosition, list2[i].m_SpawnPosX);
						rectTransform.localPosition = localPosition;
						rectTransform.localScale = Vector3.one * list2[i].m_Size;
						rectTransform.gameObject.AddComponent<NKCAnimationActor>();
						nKCRaceEventData.m_Actor = rectTransform.GetComponent<NKCAnimationActor>();
						nKCRaceEventData.m_Actor.m_trSDParent = rectTransform;
						rectTransform.GetComponent<NKCAnimationActor>().SetSpineIllust(nKCASUISpineIllust);
					}
					m_lstUsedObject.Add(nKCASUISpineIllust);
				}
			}
			nKCRaceEventData.m_startPoint = startpoint;
			nKCRaceEventData.m_capacity = list2[i].m_SlotCapacity;
			nKCRaceEventData.m_AniSet = new NKCAnimationInstance(nKCRaceEventData.m_Actor, m_trMoveObjParent, list3, m_lstEventPoint[startpoint].localPosition, m_lstEventPoint[startpoint + capacity].localPosition);
			list.Add(nKCRaceEventData);
		}
		return list;
	}

	private IEnumerator StartRace()
	{
		m_AnimatorIntro.Play("NKM_UI_EVENT_RACE_HUD_INTRO_COUNTDOWN");
		yield return new WaitForSeconds(m_CountdownWaitSeconds);
		m_bUpdate = true;
		NKCSoundManager.StopAllSound();
		NKCSoundManager.StopMusic();
		NKCSoundManager.PlayMusic("UI_SPORT_RACE", bLoop: true);
		m_AnimatorIntro.Play("NKM_UI_EVENT_RACE_HUD_INTRO_OUTRO");
	}
}
