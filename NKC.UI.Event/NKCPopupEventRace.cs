using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClientPacket.Event;
using Cs.Logging;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCPopupEventRace : NKCUIBase
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

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_PF_RACE";

	private const string UI_ASSET_NAME = "NKM_UI_EVENT_RACE_HUD";

	private static NKCPopupEventRace m_Instance;

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

	public NKCUIComStateButton m_btnSelectLine1;

	public NKCUIComStateButton m_btnSelectLine2;

	public GameObject m_objBubbleLeft;

	public GameObject m_objBubbleRight;

	public Image m_imgBubbleLeft;

	public Image m_imgBubbleRight;

	[Header("테스트코드")]
	public GameObject m_objTestParent;

	public NKCUIComStateButton m_btnTest;

	public InputField m_Input;

	public NKCUIComStateButton m_btnAdd;

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

	private NKMPACKET_RACE_START_ACK m_NKMPACKET_RACE_START_ACK;

	private List<NKCASUIUnitIllust> m_lstUsedObject = new List<NKCASUIUnitIllust>();

	private bool m_bUpdate;

	private bool m_bUserWin = true;

	private int m_SelectedLine;

	private RaceTeam m_SelectedTeam;

	private bool m_bFirstGoal = true;

	private string m_teamA_SDUnitName;

	private string m_teamB_SDUnitName;

	private bool m_bWaitForPacket;

	public static NKCPopupEventRace Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventRace>("AB_UI_NKM_UI_EVENT_PF_RACE", "NKM_UI_EVENT_RACE_HUD", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventRace>();
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
		NKCUtil.SetBindFunction(m_btnTest, ResetPosition);
		NKCUtil.SetBindFunction(m_btnAdd, OnClickAddAnimationEvent);
		NKCUtil.SetBindFunction(m_btnSelectLine1, OnClickSelectLine1);
		NKCUtil.SetBindFunction(m_btnSelectLine2, OnClickSelectLine2);
	}

	public void Open(RaceTeam selectedTeam, string teamA_SDUnitName, string teamB_SDUnitName)
	{
		ResetPosition();
		if (!NKCEventRaceAnimationManager.DataExist)
		{
			NKCEventRaceAnimationManager.LoadFromLua();
		}
		if (!NKCAnimationEventManager.DataExist)
		{
			NKCAnimationEventManager.LoadFromLua();
		}
		NKCUtil.SetGameobjectActive(m_objTestParent, NKCDefineManager.DEFINE_USE_CHEAT() && NKCScenManager.CurrentUserData().IsSuperUser());
		m_SelectedTeam = selectedTeam;
		m_teamA_SDUnitName = teamA_SDUnitName;
		m_teamB_SDUnitName = teamB_SDUnitName;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCSoundManager.StopAllSound();
		NKCSoundManager.StopMusic();
		NKCSoundManager.PlayMusic("UI_SPORT_RACE_02", bLoop: true);
		m_AnimatorIntro.Play("NKM_UI_EVENT_RACE_HUD_INTRO_INTRO");
		UIOpened();
	}

	private void ResetData()
	{
		m_bUpdate = false;
		m_bFirstGoal = true;
		m_bWaitForPacket = false;
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
		if (Mathf.Abs(0f - m_trMoveObjParent.localPosition.x - m_OtherActor.transform.localPosition.x) > (float)(Screen.width / 2))
		{
			if (0f - m_trMoveObjParent.localPosition.x > m_OtherActor.transform.localPosition.x)
			{
				NKCUtil.SetGameobjectActive(m_objBubbleLeft, bValue: true);
				NKCUtil.SetGameobjectActive(m_objBubbleRight, bValue: false);
				NKMEventRaceTemplet.Find(m_NKMPACKET_RACE_START_ACK.racePrivate.RaceId);
				_ = m_SelectedTeam;
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBubbleLeft, bValue: false);
				NKCUtil.SetGameobjectActive(m_objBubbleRight, bValue: true);
				NKMEventRaceTemplet.Find(m_NKMPACKET_RACE_START_ACK.racePrivate.RaceId);
				_ = m_SelectedTeam;
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
		if (m_NKMPACKET_RACE_START_ACK.racePrivate.SelectTeam == RaceTeam.TeamA)
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
		NKCPopupEventRaceResult.Instance.Open(m_NKMPACKET_RACE_START_ACK);
	}

	private void OnClickSelectLine1()
	{
		if (!m_bWaitForPacket)
		{
			m_bWaitForPacket = true;
			m_bUpdate = false;
			m_SelectedLine = 0;
			NKCPacketSender.Send_NKMPACKET_RACE_START_REQ(0);
		}
	}

	private void OnClickSelectLine2()
	{
		if (!m_bWaitForPacket)
		{
			m_bWaitForPacket = true;
			m_bUpdate = false;
			m_SelectedLine = 1;
			NKCPacketSender.Send_NKMPACKET_RACE_START_REQ(1);
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

	private NKCAnimationActor GetActorByTeam(bool bSelectedThisTeam)
	{
		switch (m_SelectedLine)
		{
		case 0:
			if (bSelectedThisTeam)
			{
				return m_AnimationActor1;
			}
			return m_AnimationActor2;
		default:
			if (bSelectedThisTeam)
			{
				return m_AnimationActor2;
			}
			return m_AnimationActor1;
		}
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

	private List<RaceEventType> MakeRaceEventList(out float totalTime)
	{
		List<RaceEventType> list = new List<RaceEventType>();
		System.Random random = new System.Random();
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
			RaceEventType raceEventType = usableRaceType[random.Next(0, usableRaceType.Count)];
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
				Log.Error("NKCAnimationEventTemplet is null - " + list2[i].m_AnimationEventSetID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Event/NKCPopupEventRace.cs", 621);
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

	public void OnRecv(NKMPACKET_RACE_START_ACK sPacket)
	{
		ResetData();
		m_NKMPACKET_RACE_START_ACK = sPacket;
		m_bUserWin = sPacket.isWin;
		float totalTime = 0f;
		float totalTime2 = 0f;
		List<RaceEventType> list = MakeRaceEventList(out totalTime);
		List<RaceEventType> list2 = MakeRaceEventList(out totalTime2);
		while (totalTime == totalTime2)
		{
			list2 = MakeRaceEventList(out totalTime2);
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
			m_UnitRed.SetParent(GetActorByTeam(m_SelectedTeam == RaceTeam.TeamA).m_trSDParent, worldPositionStays: false);
			RectTransform rectTransform = m_UnitRed.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector3.zero;
				rectTransform.localScale = Vector3.one;
			}
			GetActorByTeam(m_SelectedTeam == RaceTeam.TeamA).SetSpineIllust(m_UnitRed);
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
			m_UnitBlue.SetParent(GetActorByTeam(m_SelectedTeam == RaceTeam.TeamB).m_trSDParent, worldPositionStays: false);
			RectTransform rectTransform2 = m_UnitBlue.GetRectTransform();
			if (rectTransform2 != null)
			{
				rectTransform2.localPosition = Vector3.zero;
				rectTransform2.localScale = Vector3.one;
			}
			GetActorByTeam(m_SelectedTeam == RaceTeam.TeamB).SetSpineIllust(m_UnitBlue);
		}
		m_MyActor = ((m_SelectedLine == 0) ? m_AnimationActor1 : m_AnimationActor2);
		m_OtherActor = ((m_SelectedLine == 0) ? m_AnimationActor2 : m_AnimationActor1);
		NKCUtil.SetGameobjectActive(m_AnimationActor1, bValue: true);
		NKCUtil.SetGameobjectActive(m_AnimationActor2, bValue: true);
		m_queueDataWin.Clear();
		m_queueDataLose.Clear();
		m_dicRaceEventSet.Clear();
		int num = 0;
		for (int i = 0; i < list3.Count; i++)
		{
			int capacity = NKCEventRaceAnimationManager.GetCapacity(list3[i]);
			List<NKCRaceEventData> item = MakeEventSet(list3[i], GetActorByWin(bWin: true).GetSpineIllust(), GetActorByWin(bWin: true), num, capacity);
			m_queueDataWin.Enqueue(item);
			num += capacity;
		}
		m_dicRaceEventSet.Add(GetActorByWin(bWin: true).GetSpineIllust(), m_queueDataWin);
		num = 0;
		for (int j = 0; j < list4.Count; j++)
		{
			int capacity2 = NKCEventRaceAnimationManager.GetCapacity(list4[j]);
			List<NKCRaceEventData> item2 = MakeEventSet(list4[j], GetActorByWin(bWin: false).GetSpineIllust(), GetActorByWin(bWin: false), num, capacity2);
			m_queueDataLose.Enqueue(item2);
			num += capacity2;
		}
		m_dicRaceEventSet.Add(GetActorByWin(bWin: false).GetSpineIllust(), m_queueDataLose);
		if (m_SelectedTeam == RaceTeam.TeamA)
		{
			m_objSelectMark.transform.SetParent(GetActorByTeam(m_SelectedTeam == RaceTeam.TeamA).transform);
			NKCUtil.SetImageSprite(m_imgSelectMark, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", "NKM_UI_EVENT_RACE_HUD_TEAM_RED"));
		}
		else
		{
			m_objSelectMark.transform.SetParent(GetActorByTeam(m_SelectedTeam == RaceTeam.TeamB).transform);
			NKCUtil.SetImageSprite(m_imgSelectMark, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_EVENT_RACE_SPRITE", "NKM_UI_EVENT_RACE_HUD_TEAM_BLUE"));
		}
		NKCUtil.SetGameobjectActive(m_objSelectMark, bValue: true);
		m_objSelectMark.transform.localPosition = new Vector3(0f, m_fArrowPosY, 0f);
		m_AnimationActor1.transform.SetAsLastSibling();
		m_AnimationActor2.transform.SetAsLastSibling();
		StopAllCoroutines();
		StartCoroutine(StartRace());
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

	private void OnClickAddAnimationEvent()
	{
		ResetPosition();
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
			m_UnitRed.SetParent(m_AnimationActor1.m_trSDParent, worldPositionStays: false);
			RectTransform rectTransform = m_UnitRed.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector3.zero;
				rectTransform.localScale = Vector3.one;
			}
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
			m_UnitBlue.SetParent(m_AnimationActor2.m_trSDParent, worldPositionStays: false);
			RectTransform rectTransform2 = m_UnitBlue.GetRectTransform();
			if (rectTransform2 != null)
			{
				rectTransform2.localPosition = Vector3.zero;
				rectTransform2.localScale = Vector3.one;
			}
		}
		RaceEventType raceEventType = (RaceEventType)Enum.Parse(typeof(RaceEventType), m_Input.text.ToUpper());
		int num = m_lstEventPoint.Count - 1;
		int num2 = 0;
		m_AnimationActor1.SetSpineIllust(m_UnitRed);
		m_AnimationActor2.SetSpineIllust(m_UnitBlue);
		int num3 = 0;
		while (num2 < m_lstEventPoint.Count - 1)
		{
			int capacity = NKCEventRaceAnimationManager.GetCapacity(raceEventType);
			if (capacity > num)
			{
				raceEventType = RaceEventType.RUN;
				capacity = NKCEventRaceAnimationManager.GetCapacity(raceEventType);
			}
			List<NKCRaceEventData> item = MakeEventSet(raceEventType, m_UnitBlue, m_AnimationActor2, num2, capacity);
			m_queueDataLose.Enqueue(item);
			num2 += capacity;
			num -= capacity;
			num3++;
		}
		m_MyActor = m_AnimationActor2;
		m_OtherActor = m_AnimationActor1;
		NKCUtil.SetGameobjectActive(m_MyActor, bValue: true);
		m_dicRaceEventSet.Add(m_UnitBlue, m_queueDataLose);
		m_AnimationActor1.transform.SetAsLastSibling();
		m_AnimationActor2.transform.SetAsLastSibling();
		m_NKMPACKET_RACE_START_ACK = MakeDummyPacket();
		m_AnimatorIntro.Play("NKM_UI_EVENT_RACE_HUD_INTRO_OUTRO");
		NKCSoundManager.PlayMusic("UI_SPORT_RACE", bLoop: true);
		m_bUpdate = true;
	}

	private NKMPACKET_RACE_START_ACK MakeDummyPacket()
	{
		NKMPACKET_RACE_START_ACK nKMPACKET_RACE_START_ACK = new NKMPACKET_RACE_START_ACK();
		nKMPACKET_RACE_START_ACK.racePrivate = new NKMRacePrivate();
		using (IEnumerator<NKMEventRaceTemplet> enumerator = NKMEventRaceTemplet.Values.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				NKMEventRaceTemplet current = enumerator.Current;
				nKMPACKET_RACE_START_ACK.racePrivate.RaceId = current.Key;
			}
		}
		return nKMPACKET_RACE_START_ACK;
	}
}
