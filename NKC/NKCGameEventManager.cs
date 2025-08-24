using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Warfare;
using ClientPacket.WorldMap;
using NKC.Office;
using NKC.UI;
using NKC.UI.Guide;
using NKC.UI.HUD;
using NKC.UI.Office;
using NKC.UI.Result;
using NKC.UI.Warfare;
using NKC.UI.Worldmap;
using NKM;
using NKM.Contract2;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;

namespace NKC;

public static class NKCGameEventManager
{
	public enum GameEventType
	{
		INVALID,
		HIGHLIGHT_UI,
		TEXT,
		MESSAGE_BOX,
		MOVE_CAMERA,
		TUTORIAL_MARK_COMPLETE,
		TUTORIAL_NEXT,
		WAIT,
		TUTORIAL_UNIT_SKILL_GUIDE,
		TUTORIAL_UNIT_SUMMON_GUIDE,
		TUTORIAL_UNIT_RE_SUMMON_GUIDE,
		TUTORIAL_UNIT_HYPER_GUIDE,
		TUTORIAL_CLICK_UNIT_HYPER,
		TUTORIAL_HIGHLIGHT_DECK,
		TUTORIAL_HIGHLIGHT_UNIT,
		TUTORIAL_SHIP_SKILL_GUIDE,
		TUTORIAL_UNLOCK_SHIP_SKILL,
		TUTORIAL_HIGHLIGHT_SHIP_SKILL,
		UNLOCK_TUTORIAL_GAME_RE_RESPAWN,
		TUTORIAL_UNLOCK_DECK,
		TUTORIAL_UNIT_DEPLOY_AREA_GUIDE,
		TUTORIAL_SET_TALKER,
		TUTORIAL_SET_SCREEN_BG_ALPHA,
		TUTORIAL_TOUCH_UI,
		TUTORIAL_HIGHLIGHT_MAINSTREAM,
		TUTORIAL_HIGHLIGHT_EPISODE,
		TUTORIAL_TOUCH_DAILY,
		TUTORIAL_TOUCH_STAGE,
		TUTORIAL_TOUCH_ACT,
		TUTORIAL_UNLOCK_DECK_BUTTON,
		TUTORIAL_UNLOCK_DECK_BUTTON_LAST,
		TUTORIAL_SELECT_DECKVIEWER_DECK,
		TUTORIAL_SELECT_DECKVIEWER_SHIP,
		TUTORIAL_HIGHLIGHT_DECKVIEWER_DECK,
		TUTORIAL_SELECT_DECKVIEWERLIST_UNIT,
		TUTORIAL_SELECT_DECKVIEWERLIST_SHIP,
		TUTORIAL_SELECT_DECKVIEWERLIST_SLOTTYPE,
		TUTORIAL_CLICK_WARFARE_UNIT,
		TUTORIAL_CLICK_WARFARE_TILE,
		TUTORIAL_HIGHLIGHT_WARFARE_TILE,
		TUTORIAL_WARFARE_AUTO,
		TUTORIAL_SELECT_UNITLIST_UNIT,
		TUTORIAL_SELECT_UNITLIST_SHIP,
		TUTORIAL_SELECT_ITEMLIST_EQUIP,
		TUTORIAL_AUTO_GUIDE,
		TUTORIAL_TOUCH_ENHANCE_USE_SLOT,
		TUTORIAL_TOUCH_SKILL_LEVELUP_ICON,
		TUTORIAL_TOUCH_BACK_BUTTON,
		TUTORIAL_HIGHLIGHT_ACHIEVEMENT_SLOT,
		TUTORIAL_CLICK_ACHIEVEMENT_SLOT,
		TUTORIAL_HIGHLIGHT_COUNTERCASE,
		TUTORIAL_CLICK_COUNTERCASE,
		TUTORIAL_HIGHLIGHT_COUNTERCASELIST,
		TUTORIAL_CLICK_COUNTERCASELIST,
		TUTORIAL_HIGHLIGHT_FORGE_CRAFT_SLOT,
		TUTORIAL_CLICK_FORGE_CRAFT_SLOT,
		TUTORIAL_HIGHLIGHT_FORGE_CRAFT_MOLD,
		TUTORIAL_CLICK_FORGE_CRAFT_MOLD,
		TUTORIAL_HIGHLIGHT_HANGAR_BUILD_SLOT,
		TUTORIAL_CLICK_HANGAR_BUILD_SLOT,
		TUTORIAL_IMAGE_GUIDE,
		TUTORIAL_CONTRACT_FIND_BANNER,
		BEGIN_SUM_ITEMOPEN_RESULT,
		END_AND_SHOW_ITEMOPEN_RESULT,
		OPEN_MISC_ITEM_RANDOM_BOX,
		TUTORIAL_WORLDMAP_FIND_CITY,
		TUTORIAL_WORLDMAP_FIND_CITY_LEVEL,
		TUTORIAL_TOUCH_WORLDMAP_BUILDING_EMPLTY,
		TUTORIAL_HIGHLIGHT_WORLDMAP_BUILD_SLOT,
		RESET_WORLDMAP,
		TUTORIAL_HIGHLIGHT_SHAODW_PALACE_SLOT,
		REFRESH_SCENE,
		LOBBY_MENU_TAB,
		BASE_MENU_TYPE,
		PLAY_CUTSCENE,
		WAIT_SECONDS,
		RESULT_GET_UNIT,
		OPEN_NICKNAME_CHANGE_POPUP,
		MOVE_MINIMAP,
		OFFICE_HIGHLIGHT,
		OFFICE_TOUCH,
		OFFICE_UNITLIST_UNIT,
		OFFICE_ITEMLIST_FURNITURE,
		OFFICE_FURNITURE_HIGHLIGHT,
		OFFICE_FURNITURE_TOUCH,
		REARM_UNITLIST_UNIT,
		EXTRACT_SLOT_SELECT,
		EXTRACT_UNITLIST_UNIT,
		TOGGLE_UI_CANVAS,
		PLAY_MUSIC
	}

	public enum TextBoxPosType
	{
		DEFAULT,
		CENTERUP,
		CENTER,
		CENTERDOWN,
		RIGHTUP,
		RIGHTDOWN,
		LEFTUP,
		LEFTDOWN
	}

	public class NKCGameEventTemplet
	{
		public int EventID;

		public GameEventType EventType;

		public string Text = "";

		public int Value;

		public string StringValue = "";

		public bool LoadFromLUA(NKMLua cNKMLua)
		{
			if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameEventManager.cs", 198))
			{
				return false;
			}
			string nationalPostfix = NKCStringTable.GetNationalPostfix(NKCStringTable.GetNationalCode());
			int result = (int)(1u & (cNKMLua.GetData("EventID", ref EventID) ? 1u : 0u)) & (cNKMLua.GetData("EventType", ref EventType) ? 1 : 0);
			cNKMLua.GetData("Text" + nationalPostfix, ref Text);
			cNKMLua.GetData("Value", ref Value);
			cNKMLua.GetData("StringValue", ref StringValue);
			return (byte)result != 0;
		}
	}

	public delegate void OnEventFinish(bool bUnpause);

	private static OnEventFinish dOnEventFinish;

	private static Dictionary<int, List<NKCGameEventTemplet>> m_dicGameEventTemplet;

	private static Dictionary<int, List<NKCGameEventTemplet>> m_dicTutorialEventTemplet;

	private static List<NKCGameEventTemplet> m_lstCurrentEvent;

	private static int m_CurrentIndex;

	private static bool m_bIsPauseEvent;

	private static string m_strCurrentTalkInvenIcon;

	private const float DEFAULT_GUIDE_SCREEN_ALPHA = 0.85f;

	private static float m_fGuideScreenAlpha = 0.85f;

	private static List<NKMRewardData> lstCollectReward;

	private static float m_fWaitTime = 0f;

	public static bool RandomBoxDataCollecting { get; set; }

	public static void CollectResultData(NKMRewardData rewardData)
	{
		if (rewardData != null && lstCollectReward != null)
		{
			lstCollectReward.Add(rewardData);
		}
		if (GetCurrentEventType() == GameEventType.OPEN_MISC_ITEM_RANDOM_BOX)
		{
			ProcessEvent();
		}
	}

	private static NKCGameEventTemplet GetCurrentEventTemplet()
	{
		if (m_lstCurrentEvent == null)
		{
			return null;
		}
		if (m_CurrentIndex < 0)
		{
			return null;
		}
		if (m_CurrentIndex >= m_lstCurrentEvent.Count)
		{
			return null;
		}
		return m_lstCurrentEvent[m_CurrentIndex];
	}

	public static GameEventType GetCurrentEventType()
	{
		return GetCurrentEventTemplet()?.EventType ?? GameEventType.INVALID;
	}

	private static void LoadGameEvent()
	{
		LoadFromLua("ab_script", "LUA_GAME_EVENT_TEMPLET", "m_GameEventTable", out m_dicGameEventTemplet);
	}

	private static bool LoadFromLua(string bundleName, string fileName, string tableName, out Dictionary<int, List<NKCGameEventTemplet>> dicTemplet)
	{
		dicTemplet = new Dictionary<int, List<NKCGameEventTemplet>>();
		NKMLua nKMLua = new NKMLua();
		if (!NKMContentsVersionManager.CheckContentsVersion(nKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCGameEventManager.cs", 268))
		{
			return false;
		}
		if (nKMLua.LoadCommonPath(bundleName, fileName) && nKMLua.OpenTable(tableName))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKCGameEventTemplet nKCGameEventTemplet = new NKCGameEventTemplet();
				if (nKCGameEventTemplet.LoadFromLUA(nKMLua))
				{
					if (!dicTemplet.ContainsKey(nKCGameEventTemplet.EventID))
					{
						List<NKCGameEventTemplet> value = new List<NKCGameEventTemplet>();
						dicTemplet.Add(nKCGameEventTemplet.EventID, value);
					}
					dicTemplet[nKCGameEventTemplet.EventID].Add(nKCGameEventTemplet);
				}
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return true;
	}

	private static List<NKCGameEventTemplet> GetEventTempletList(int eventID)
	{
		if (m_dicGameEventTemplet == null)
		{
			LoadGameEvent();
		}
		if (m_dicGameEventTemplet.TryGetValue(eventID, out var value))
		{
			return value;
		}
		return null;
	}

	public static bool IsGameCameraStopRequired()
	{
		if (m_lstCurrentEvent == null)
		{
			return false;
		}
		if (m_bIsPauseEvent)
		{
			return true;
		}
		return false;
	}

	public static bool IsEventPlaying()
	{
		return m_lstCurrentEvent != null;
	}

	public static int GetCurrentEventID()
	{
		if (m_lstCurrentEvent == null)
		{
			return -1;
		}
		return m_lstCurrentEvent[0].EventID;
	}

	public static bool IsPauseEventPlaying()
	{
		if (m_lstCurrentEvent != null)
		{
			return m_bIsPauseEvent;
		}
		return false;
	}

	public static void Update(float deltaTime)
	{
		if (GetCurrentEventType() == GameEventType.WAIT_SECONDS)
		{
			m_fWaitTime -= deltaTime;
			if (m_fWaitTime < 0f)
			{
				ProcessEvent();
			}
		}
	}

	public static void EndCutScene()
	{
		NKCSoundManager.StopAllSound();
		NKCSoundManager.PlayScenMusic(NKCScenManager.GetScenManager().GetNowScenID());
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCScenManager.GetScenManager().GetGameClient().GetGameHud()
				.HUD_UNHIDE();
		}
		ProcessEvent();
	}

	public static void PlayGameEvent(int eventID, bool isPauseEvent, OnEventFinish onEventFinish)
	{
		if (m_lstCurrentEvent != null)
		{
			FinishEventTemplet(m_lstCurrentEvent[m_CurrentIndex]);
		}
		m_fGuideScreenAlpha = 0.85f;
		m_strCurrentTalkInvenIcon = "";
		m_bIsPauseEvent = isPauseEvent;
		dOnEventFinish = onEventFinish;
		m_lstCurrentEvent = GetEventTempletList(eventID);
		if (m_lstCurrentEvent != null)
		{
			m_CurrentIndex = -1;
			ProcessEvent();
		}
	}

	public static bool IsWaiting()
	{
		if (m_lstCurrentEvent == null)
		{
			return false;
		}
		if (m_CurrentIndex < m_lstCurrentEvent.Count && m_lstCurrentEvent[m_CurrentIndex].EventType == GameEventType.WAIT)
		{
			return true;
		}
		return false;
	}

	public static void WaitFinished()
	{
		if (IsWaiting())
		{
			ProcessEvent();
		}
	}

	public static void TutorialCompletePacketSent(int id)
	{
		if (m_lstCurrentEvent != null && m_CurrentIndex >= 0 && m_CurrentIndex < m_lstCurrentEvent.Count)
		{
			NKCGameEventTemplet nKCGameEventTemplet = m_lstCurrentEvent[m_CurrentIndex];
			if (nKCGameEventTemplet.EventType == GameEventType.TUTORIAL_MARK_COMPLETE && nKCGameEventTemplet.Value == id)
			{
				ProcessEvent();
			}
			else
			{
				Debug.LogError("Tutorial complete packet sent, but was not waiting");
			}
		}
	}

	public static void ResumeEvent()
	{
		if (IsEventPlaying())
		{
			FinishEventTemplet(m_lstCurrentEvent[m_CurrentIndex]);
			PlayEventTemplet(m_lstCurrentEvent[m_CurrentIndex]);
		}
	}

	private static void ProcessEvent()
	{
		if (m_lstCurrentEvent == null)
		{
			return;
		}
		if (m_CurrentIndex >= 0)
		{
			NKCGameEventTemplet nKCGameEventTemplet = m_lstCurrentEvent[m_CurrentIndex];
			FinishEventTemplet(nKCGameEventTemplet);
			if (nKCGameEventTemplet != null && nKCGameEventTemplet.EventType == GameEventType.OPEN_NICKNAME_CHANGE_POPUP)
			{
				NKCMMPManager.OnCustomEvent("15_username_creation");
			}
		}
		m_CurrentIndex++;
		if (m_CurrentIndex < m_lstCurrentEvent.Count)
		{
			PlayEventTemplet(m_lstCurrentEvent[m_CurrentIndex]);
			return;
		}
		m_lstCurrentEvent = null;
		m_CurrentIndex = -1;
		dOnEventFinish?.Invoke(m_bIsPauseEvent);
		dOnEventFinish = null;
	}

	private static void OpenTutorialGuide(Renderer targetRenderer, string text, UnityAction onComplete, NKCUIComRectScreen.ScreenExpand expandFlag = NKCUIComRectScreen.ScreenExpand.None)
	{
		NKCUIOverlayTutorialGuide.Instance.Open(targetRenderer, text, onComplete, expandFlag);
		NKCUIOverlayTutorialGuide.Instance.SetBGScreenAlpha(m_fGuideScreenAlpha);
	}

	private static void OpenTutorialGuide(RectTransform rtClickableArea, NKCUIOverlayTutorialGuide.ClickGuideType type, string text, UnityAction onComplete, bool bIsFromMidCanvas = false, NKCUIComRectScreen.ScreenExpand expandFlag = NKCUIComRectScreen.ScreenExpand.None)
	{
		NKCUIOverlayTutorialGuide.Instance.Open(rtClickableArea, type, text, onComplete, bIsFromMidCanvas, expandFlag);
		NKCUIOverlayTutorialGuide.Instance.SetBGScreenAlpha(m_fGuideScreenAlpha);
	}

	private static void PlayEventTemplet(NKCGameEventTemplet eventTemplet)
	{
		if (eventTemplet == null)
		{
			return;
		}
		switch (eventTemplet.EventType)
		{
		case GameEventType.TEXT:
			NKCUIOverlayTutorialGuide.Instance.Open(null, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet.Text, ProcessEvent, bIsFromMidCanvas: false, NKCUIComRectScreen.ScreenExpand.None, eventTemplet.Value);
			NKCUIOverlayTutorialGuide.Instance.SetBGScreenAlpha(m_fGuideScreenAlpha);
			break;
		case GameEventType.MESSAGE_BOX:
			NKCUIOverlayCharMessage.Instance.Open(eventTemplet.StringValue, eventTemplet.Text, eventTemplet.Value, ProcessEvent);
			NKCUIOverlayCharMessage.Instance.SetBGScreenAlpha(m_fGuideScreenAlpha);
			break;
		case GameEventType.HIGHLIGHT_UI:
		{
			GameObject gameObject5 = FindGameObject(eventTemplet.StringValue);
			if (gameObject5 == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform component3 = gameObject5.GetComponent<RectTransform>();
			bool bMiddleCanvas2 = eventTemplet.Value != 0;
			NKCUIOverlayTutorialGuide.ClickGuideType guideType = NKCUIOverlayTutorialGuide.ClickGuideType.None;
			OpenTutorialGuideBySettedFace(component3, guideType, eventTemplet, ProcessEvent, bMiddleCanvas2);
			break;
		}
		case GameEventType.WAIT_SECONDS:
			m_fWaitTime = eventTemplet.Value;
			OpenTutorialGuide(null, NKCUIOverlayTutorialGuide.ClickGuideType.None, "", null);
			break;
		case GameEventType.TUTORIAL_MARK_COMPLETE:
			if (NKCScenManager.CurrentUserData().m_MissionData.GetCompletedMissionData(eventTemplet.Value) != null)
			{
				ProcessEvent();
			}
			NKCTutorialManager.CompleteTutorial(eventTemplet.Value);
			break;
		case GameEventType.TUTORIAL_NEXT:
			NKCTutorialManager.PlayTutorial(eventTemplet.Value);
			break;
		case GameEventType.MOVE_CAMERA:
			NKCScenManager.GetScenManager().GetGameClient()?.TutorialForceCamMove((float)eventTemplet.Value / 100f);
			ProcessEvent();
			break;
		case GameEventType.TUTORIAL_HIGHLIGHT_UNIT:
		{
			NKCGameClient gameClient5 = NKCScenManager.GetScenManager().GetGameClient();
			gameClient5.SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
			NKMUnit nKMUnit3 = gameClient5.GetUnitChain().Find((NKMUnit x) => x.GetUnitData().m_UnitID == eventTemplet.Value);
			if (nKMUnit3 != null && nKMUnit3 is NKCUnitClient)
			{
				NKCUnitClient obj2 = nKMUnit3 as NKCUnitClient;
				float posX3 = obj2.GetUnitSyncData().m_PosX;
				float fMaxX3 = gameClient5.GetMapTemplet().m_fMaxX;
				if (fMaxX3 > 0f)
				{
					gameClient5.TutorialForceCamMove(posX3 / fMaxX3);
				}
				NKCASUnitSpineSprite nKCASUnitSpineSprite = obj2.GetNKCASUnitSpineSprite();
				if (nKCASUnitSpineSprite != null)
				{
					OpenTutorialGuide(nKCASUnitSpineSprite.m_MeshRenderer, eventTemplet.Text, ProcessEvent);
					break;
				}
				Debug.LogError($"GameEventManager {eventTemplet.EventType} - Target Unit(ID : {eventTemplet.Value} Has no renderer!");
				ProcessEvent();
			}
			else
			{
				Debug.LogError($"GameEventManager {eventTemplet.EventType} - Target Unit(ID : {eventTemplet.Value} Not Found!");
				ProcessEvent();
			}
			break;
		}
		case GameEventType.TUTORIAL_UNIT_SKILL_GUIDE:
		{
			NKCGameClient gameClient2 = NKCScenManager.GetScenManager().GetGameClient();
			gameClient2.SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
			NKMUnit nKMUnit2 = gameClient2.GetUnitChain().Find((NKMUnit x) => x.GetUnitData().m_UnitID == eventTemplet.Value);
			if (nKMUnit2 != null && nKMUnit2 is NKCUnitClient)
			{
				NKCUnitClient obj = nKMUnit2 as NKCUnitClient;
				GameObject objectUnitSkillGuage = obj.GetObjectUnitSkillGuage();
				float posX2 = obj.GetUnitSyncData().m_PosX;
				float fMaxX2 = gameClient2.GetMapTemplet().m_fMaxX;
				if (fMaxX2 > 0f)
				{
					gameClient2.TutorialForceCamMove(posX2 / fMaxX2);
				}
				OpenTutorialGuide(objectUnitSkillGuage.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet.Text, ProcessEvent, bIsFromMidCanvas: true);
			}
			else
			{
				Debug.LogError($"GameEventManager {eventTemplet.EventType} - Target Unit(ID : {eventTemplet.Value} Not Found!");
				ProcessEvent();
			}
			break;
		}
		case GameEventType.TUTORIAL_UNIT_HYPER_GUIDE:
		case GameEventType.TUTORIAL_CLICK_UNIT_HYPER:
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			gameClient.SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
			NKMUnit nKMUnit = gameClient.GetUnitChain().Find((NKMUnit x) => x.GetUnitData().m_UnitID == eventTemplet.Value);
			if (nKMUnit != null && nKMUnit is NKCUnitClient)
			{
				NKCUnitClient unitClient = nKMUnit as NKCUnitClient;
				GameObject objectUnitHyper = unitClient.GetObjectUnitHyper();
				if (unitClient.GetHyperSkillCoolRate() > 0f)
				{
					ProcessEvent();
					break;
				}
				float posX = unitClient.GetUnitSyncData().m_PosX;
				float fMaxX = gameClient.GetMapTemplet().m_fMaxX;
				if (fMaxX > 0f)
				{
					gameClient.TutorialForceCamMove(posX / fMaxX);
				}
				if (eventTemplet.EventType == GameEventType.TUTORIAL_CLICK_UNIT_HYPER)
				{
					OpenTutorialGuideBySettedFace(objectUnitHyper.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, null, bMiddleCanvas: true);
					NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
					{
						NKCUIOverlayCharMessage.CheckInstanceAndClose();
						NKCUIOverlayTutorialGuide.CheckInstanceAndClose();
						unitClient.UseManualSkill();
						ProcessEvent();
					});
				}
				else
				{
					OpenTutorialGuide(objectUnitHyper.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet.Text, ProcessEvent, bIsFromMidCanvas: true);
				}
			}
			else
			{
				Debug.LogError($"GameEventManager {eventTemplet.EventType} - Target Unit(ID : {eventTemplet.Value} Not Found!");
				ProcessEvent();
			}
			break;
		}
		case GameEventType.TUTORIAL_UNIT_SUMMON_GUIDE:
		{
			NKCGameHudDeckSlot hudDeckByUnitID3 = NKCScenManager.GetScenManager().GetGameClient().GetGameHud()
				.GetHudDeckByUnitID(eventTemplet.Value);
			if (hudDeckByUnitID3 != null)
			{
				if (!hudDeckByUnitID3.CanRespawn())
				{
					Debug.LogWarning("Unit Respawn impossibe. processing event");
					ProcessEvent();
					break;
				}
				hudDeckByUnitID3.SetActive(bActive: true, bEventControl: true);
				NKCUIOverlayTutorialGuide.ClickGuideType type3 = NKCUIOverlayTutorialGuide.ClickGuideType.DeckDrag;
				OpenTutorialGuide(hudDeckByUnitID3.m_rtBG, type3, eventTemplet.Text, ProcessEvent);
				if (!m_bIsPauseEvent)
				{
					NKCUIOverlayTutorialGuide.Instance.SetScreenActive(value: false);
				}
			}
			else
			{
				Debug.LogWarning($"GameEventManager {eventTemplet.EventType} - Target Unit(ID : {eventTemplet.Value} Not Found From HUD!");
				ProcessEvent();
			}
			break;
		}
		case GameEventType.TUTORIAL_UNIT_RE_SUMMON_GUIDE:
		{
			NKCGameHudDeckSlot hudDeckByUnitID2 = NKCScenManager.GetScenManager().GetGameClient().GetGameHud()
				.GetHudDeckByUnitID(eventTemplet.Value);
			if (hudDeckByUnitID2 != null)
			{
				hudDeckByUnitID2.SetActive(bActive: true, bEventControl: true);
				NKCUIOverlayTutorialGuide.ClickGuideType type2 = NKCUIOverlayTutorialGuide.ClickGuideType.DeckDrag;
				OpenTutorialGuide(hudDeckByUnitID2.m_rtBG, type2, eventTemplet.Text, ProcessEvent);
				if (!m_bIsPauseEvent)
				{
					NKCUIOverlayTutorialGuide.Instance.SetScreenActive(value: false);
				}
			}
			else
			{
				Debug.LogWarning($"GameEventManager {eventTemplet.EventType} - Target Unit(ID : {eventTemplet.Value} Not Found From HUD!");
				ProcessEvent();
			}
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_DECK:
		{
			NKCGameHudDeckSlot hudDeckByUnitID = NKCScenManager.GetScenManager().GetGameClient().GetGameHud()
				.GetHudDeckByUnitID(eventTemplet.Value);
			if (hudDeckByUnitID != null)
			{
				hudDeckByUnitID.SetActive(bActive: true, bEventControl: true);
				NKCUIOverlayTutorialGuide.ClickGuideType type = NKCUIOverlayTutorialGuide.ClickGuideType.None;
				RectTransform rectTransform = null;
				string stringValue = eventTemplet.StringValue;
				rectTransform = ((stringValue == null || !(stringValue == "ATKTYPE")) ? hudDeckByUnitID.m_rtBG : hudDeckByUnitID.GetRectATKMark());
				OpenTutorialGuide(rectTransform, type, eventTemplet.Text, ProcessEvent);
				if (!m_bIsPauseEvent)
				{
					NKCUIOverlayTutorialGuide.Instance.SetScreenActive(value: false);
				}
			}
			else
			{
				Debug.LogError($"GameEventManager {eventTemplet.EventType} - Target Unit(ID : {eventTemplet.Value} Not Found From HUD!");
				ProcessEvent();
			}
			break;
		}
		case GameEventType.TUTORIAL_SHIP_SKILL_GUIDE:
		case GameEventType.TUTORIAL_UNLOCK_SHIP_SKILL:
		case GameEventType.TUTORIAL_HIGHLIGHT_SHIP_SKILL:
		{
			NKCUIHudShipSkillDeck hudSkillBySkillID = NKCScenManager.GetScenManager().GetGameClient().GetGameHud()
				.GetHudSkillBySkillID(eventTemplet.Value);
			if (hudSkillBySkillID != null)
			{
				hudSkillBySkillID.SetActive(bActive: true, bEventControl: true);
				if (eventTemplet.EventType == GameEventType.TUTORIAL_UNLOCK_SHIP_SKILL)
				{
					ProcessEvent();
				}
				else
				{
					OpenTutorialGuide(type: (eventTemplet.EventType == GameEventType.TUTORIAL_SHIP_SKILL_GUIDE) ? NKCUIOverlayTutorialGuide.ClickGuideType.ShipSkill : NKCUIOverlayTutorialGuide.ClickGuideType.None, rtClickableArea: hudSkillBySkillID.m_rtSubRoot, text: eventTemplet.Text, onComplete: ProcessEvent);
				}
			}
			else
			{
				Debug.LogError($"GameEventManager {eventTemplet.EventType} - Target Skill(ID : {eventTemplet.Value} Not Found From HUD!");
				ProcessEvent();
			}
			break;
		}
		case GameEventType.UNLOCK_TUTORIAL_GAME_RE_RESPAWN:
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_GAME)
			{
				Debug.LogError("게임이 아닌데 튜토리얼 재소환 언락 이벤트를 시도");
				ProcessEvent();
			}
			else
			{
				NKCScenManager.GetScenManager().GetGameClient().UnlockTutorialReRespawn();
				ProcessEvent();
			}
			break;
		case GameEventType.TUTORIAL_AUTO_GUIDE:
		{
			NKCGameHud gameHud = NKCScenManager.GetScenManager().GetGameClient().GetGameHud();
			if (!(gameHud == null))
			{
				gameHud.SetAutoEnable();
				gameHud.ToggleAutoRespawn(bOn: false);
				NKCUIComButton autoButton = gameHud.GetAutoButton();
				if (!(autoButton == null))
				{
					SetButtonClickSteal(autoButton.GetComponent<RectTransform>(), eventTemplet);
				}
			}
			break;
		}
		case GameEventType.TUTORIAL_UNLOCK_DECK:
		{
			NKCGameHud gameHud2 = NKCScenManager.GetScenManager().GetGameClient().GetGameHud();
			if (gameHud2 != null)
			{
				gameHud2.ShowHud(bEventControl: true);
			}
			ProcessEvent();
			break;
		}
		case GameEventType.TUTORIAL_UNIT_DEPLOY_AREA_GUIDE:
		{
			NKCGameClient gameClient4 = NKCScenManager.GetScenManager().GetGameClient();
			gameClient4.SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_STOP);
			SpriteRenderer mapInvalidLandRenderer = gameClient4.GetMapInvalidLandRenderer();
			NKCUIComRectScreen.ScreenExpand expandFlag = NKCUIComRectScreen.ScreenExpand.Left | NKCUIComRectScreen.ScreenExpand.Right;
			OpenTutorialGuide(mapInvalidLandRenderer, eventTemplet.Text, ProcessEvent, expandFlag);
			NKCUIOverlayTutorialGuide.Instance.IsShowingInvalidMap = true;
			break;
		}
		case GameEventType.TUTORIAL_TOUCH_UI:
		{
			GameObject gameObject3 = FindGameObject(eventTemplet.StringValue);
			if (gameObject3 == null)
			{
				ProcessEvent();
				break;
			}
			bool bMiddleCanvas = eventTemplet.Value != 0;
			SetButtonClickSteal(gameObject3.GetComponent<RectTransform>(), eventTemplet, bMiddleCanvas);
			break;
		}
		case GameEventType.TUTORIAL_TOUCH_BACK_BUTTON:
			if (NKCUIManager.NKCUIUpsideMenu.btnBackButton == null)
			{
				ProcessEvent();
				break;
			}
			OpenTutorialGuideBySettedFace(NKCUIManager.NKCUIUpsideMenu.btnBackButton.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, null);
			NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
			{
				NKCUIOverlayCharMessage.CheckInstanceAndClose();
				NKCUIOverlayTutorialGuide.CheckInstanceAndClose();
				NKCUIManager.OnBackButton();
				if (!string.IsNullOrEmpty(NKCUIManager.NKCUIUpsideMenu.btnBackButton.m_SoundForPointClick))
				{
					NKCSoundManager.PlaySound(NKCUIManager.NKCUIUpsideMenu.btnBackButton.m_SoundForPointClick, 1f, 0f, 0f);
				}
				if (GetCurrentEventID() == eventTemplet.EventID)
				{
					ProcessEvent();
				}
			});
			break;
		case GameEventType.TUTORIAL_HIGHLIGHT_MAINSTREAM:
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OPERATION)
			{
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetTutorialMainstreamGuide(eventTemplet, ProcessEvent);
			}
			else
			{
				ProcessEvent();
			}
			break;
		case GameEventType.TUTORIAL_HIGHLIGHT_EPISODE:
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_OPERATION)
			{
				NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetTutorialMainstreamGuide(eventTemplet, ProcessEvent);
			}
			else
			{
				ProcessEvent();
			}
			break;
		case GameEventType.TUTORIAL_TOUCH_DAILY:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OPERATION)
			{
				ProcessEvent();
				break;
			}
			RectTransform dailyRect = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetDailyRect();
			if (dailyRect == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(dailyRect, eventTemplet);
			}
			break;
		}
		case GameEventType.TUTORIAL_TOUCH_STAGE:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OPERATION)
			{
				ProcessEvent();
				break;
			}
			RectTransform stageSlot = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetStageSlot(eventTemplet.Value);
			if (stageSlot == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(stageSlot, eventTemplet);
			}
			break;
		}
		case GameEventType.TUTORIAL_TOUCH_ACT:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OPERATION)
			{
				ProcessEvent();
				break;
			}
			RectTransform actSlot = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetActSlot(eventTemplet.Value);
			if (actSlot == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(actSlot, eventTemplet);
			}
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_WARFARE_TILE:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				ProcessEvent();
				break;
			}
			NKCWarfareGame warfareGame2 = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame();
			if (warfareGame2 == null)
			{
				ProcessEvent();
				break;
			}
			GameObject tileObject2 = warfareGame2.GetTileObject(eventTemplet.Value);
			if (tileObject2 == null)
			{
				ProcessEvent();
			}
			else
			{
				OpenTutorialGuideBySettedFace(tileObject2.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent, bMiddleCanvas: true);
			}
			break;
		}
		case GameEventType.TUTORIAL_CLICK_WARFARE_TILE:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				ProcessEvent();
				break;
			}
			NKCWarfareGame warfareGame = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame();
			if (warfareGame == null)
			{
				ProcessEvent();
				break;
			}
			GameObject tileObject = warfareGame.GetTileObject(eventTemplet.Value);
			if (tileObject == null)
			{
				ProcessEvent();
				break;
			}
			OpenTutorialGuideBySettedFace(tileObject.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, null, bMiddleCanvas: true);
			NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
			{
				NKCWarfareGame warfareGame5 = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame();
				if (warfareGame5 != null)
				{
					warfareGame5.ProcessTutorialTileTouchEvent(eventTemplet);
				}
				ProcessEvent();
			});
			break;
		}
		case GameEventType.TUTORIAL_CLICK_WARFARE_UNIT:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				ProcessEvent();
				break;
			}
			NKCWarfareGame warfareGame4 = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame();
			if (warfareGame4 == null)
			{
				ProcessEvent();
				break;
			}
			if (string.IsNullOrEmpty(eventTemplet.StringValue))
			{
				ProcessEvent();
				break;
			}
			NKM_WARFARE_MAP_TILE_TYPE enum2 = NKM_WARFARE_MAP_TILE_TYPE.NWMTT_NORMAL;
			if (!eventTemplet.StringValue.TryParse<NKM_WARFARE_MAP_TILE_TYPE>(out enum2))
			{
				ProcessEvent();
				break;
			}
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			Dictionary<int, WarfareUnitData>.ValueCollection values = warfareGameData.warfareTeamDataA.warfareUnitDataByUIDMap.Values;
			GameObject gameObject4 = null;
			foreach (WarfareUnitData item2 in values)
			{
				if (item2.hp <= 0f)
				{
					continue;
				}
				WarfareTileData tileData = warfareGameData.GetTileData(item2.tileIndex);
				if (tileData != null && tileData.tileType == enum2)
				{
					gameObject4 = warfareGame4.GetUnitObject(item2.warfareGameUnitUID);
					if (gameObject4 != null)
					{
						break;
					}
				}
			}
			if (gameObject4 == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(gameObject4.GetComponent<RectTransform>(), eventTemplet, bMiddleCanvas: true);
			}
			break;
		}
		case GameEventType.TUTORIAL_WARFARE_AUTO:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				ProcessEvent();
				break;
			}
			NKCWarfareGame warfareGame3 = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME().GetWarfareGame();
			if (warfareGame3 == null)
			{
				ProcessEvent();
				break;
			}
			bool flag = Convert.ToBoolean(eventTemplet.StringValue);
			if (NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bAutoWarfare == flag)
			{
				ProcessEvent();
				break;
			}
			warfareGame3.SetAutoForTutorial(flag);
			ProcessEvent();
			break;
		}
		case GameEventType.TUTORIAL_UNLOCK_DECK_BUTTON:
		{
			if (!NKCUIDeckViewer.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			NKCUIDeckViewer.Instance.SelectDeck(new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, 0));
			NKCDeckListButton deckListButton2 = NKCUIDeckViewer.Instance.m_NKCDeckViewList.GetDeckListButton(eventTemplet.Value);
			if (deckListButton2 == null)
			{
				ProcessEvent();
				break;
			}
			OpenTutorialGuide(deckListButton2.m_cbtnButton.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet.Text, null);
			NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
			{
				if (NKCScenManager.CurrentUserData().m_ArmyData.GetDeckData(NKM_DECK_TYPE.NDT_NORMAL, eventTemplet.Value) != null)
				{
					NKCUIDeckViewer.Instance.SelectDeck(new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, eventTemplet.Value));
				}
				else
				{
					NKCUIDeckViewer.Instance.DeckUnlockRequestPopup(new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, eventTemplet.Value));
				}
				ProcessEvent();
			});
			break;
		}
		case GameEventType.TUTORIAL_UNLOCK_DECK_BUTTON_LAST:
		{
			if (!NKCUIDeckViewer.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData == null)
			{
				ProcessEvent();
				break;
			}
			int unlockDeckCount = nKMUserData.m_ArmyData.GetUnlockedDeckCount(NKM_DECK_TYPE.NDT_NORMAL);
			if (unlockDeckCount == nKMUserData.m_ArmyData.GetMaxDeckCount(NKM_DECK_TYPE.NDT_NORMAL))
			{
				ProcessEvent();
				break;
			}
			NKCUIDeckViewer.Instance.SetDeckScroll(unlockDeckCount);
			NKCDeckListButton deckListButton = NKCUIDeckViewer.Instance.m_NKCDeckViewList.GetDeckListButton(unlockDeckCount);
			if (deckListButton == null)
			{
				ProcessEvent();
				break;
			}
			OpenTutorialGuide(deckListButton.m_cbtnButton.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet.Text, null);
			NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
			{
				NKCUIDeckViewer.Instance.DeckUnlockRequestPopup(new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, unlockDeckCount));
				ProcessEvent();
			});
			break;
		}
		case GameEventType.TUTORIAL_SELECT_DECKVIEWERLIST_UNIT:
		case GameEventType.TUTORIAL_SELECT_DECKVIEWERLIST_SHIP:
		{
			NKM_UNIT_TYPE type4 = ((eventTemplet.EventType == GameEventType.TUTORIAL_SELECT_DECKVIEWERLIST_SHIP) ? NKM_UNIT_TYPE.NUT_SHIP : NKM_UNIT_TYPE.NUT_NORMAL);
			if (!NKCUIDeckViewer.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			NKCUIUnitSelectListSlotBase slot4 = NKCUIDeckViewer.Instance.SetTutorialSelectUnit(type4, eventTemplet.Value);
			if (slot4 == null)
			{
				ProcessEvent();
				break;
			}
			OpenTutorialGuideBySettedFace(slot4.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, null);
			NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
			{
				slot4.InvokeClick();
				ProcessEvent();
			});
			break;
		}
		case GameEventType.TUTORIAL_SELECT_DECKVIEWER_SHIP:
		{
			if (!NKCUIDeckViewer.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			NKCUIComButton shipSelectButton = NKCUIDeckViewer.Instance.GetShipSelectButton();
			if (shipSelectButton == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(shipSelectButton.GetComponent<RectTransform>(), eventTemplet);
			}
			break;
		}
		case GameEventType.TUTORIAL_SELECT_DECKVIEWER_DECK:
		case GameEventType.TUTORIAL_HIGHLIGHT_DECKVIEWER_DECK:
		{
			if (!NKCUIDeckViewer.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = null;
			if (eventTemplet.Value >= NKCUIDeckViewer.Instance.m_NKCDeckViewUnit.m_listNKCDeckViewUnitSlot.Count)
			{
				ProcessEvent();
				break;
			}
			List<NKCDeckViewUnitSlot> listNKCDeckViewUnitSlot = NKCUIDeckViewer.Instance.m_NKCDeckViewUnit.m_listNKCDeckViewUnitSlot;
			if (string.IsNullOrEmpty(eventTemplet.StringValue))
			{
				nKCDeckViewUnitSlot = listNKCDeckViewUnitSlot[eventTemplet.Value];
			}
			else
			{
				switch (eventTemplet.StringValue)
				{
				case "UNIT":
				{
					List<NKCDeckViewUnitSlot> list3 = listNKCDeckViewUnitSlot.FindAll((NKCDeckViewUnitSlot v) => !v.IsEmpty());
					if (eventTemplet.Value < list3.Count)
					{
						nKCDeckViewUnitSlot = list3[eventTemplet.Value];
					}
					break;
				}
				case "EMPTY":
				{
					List<NKCDeckViewUnitSlot> list2 = listNKCDeckViewUnitSlot.FindAll((NKCDeckViewUnitSlot v) => v.IsEmpty());
					if (eventTemplet.Value < list2.Count)
					{
						nKCDeckViewUnitSlot = list2[eventTemplet.Value];
					}
					break;
				}
				}
			}
			if (nKCDeckViewUnitSlot == null)
			{
				ProcessEvent();
				break;
			}
			int index = listNKCDeckViewUnitSlot.IndexOf(nKCDeckViewUnitSlot);
			if (eventTemplet.EventType == GameEventType.TUTORIAL_SELECT_DECKVIEWER_DECK)
			{
				OpenTutorialGuideBySettedFace(nKCDeckViewUnitSlot.m_NKCUIComButton.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, null);
				NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
				{
					NKCUIDeckViewer.Instance.OnUnitClicked(index);
					ProcessEvent();
				});
			}
			else
			{
				OpenTutorialGuide(nKCDeckViewUnitSlot.m_NKCUIComButton.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet.Text, ProcessEvent);
			}
			break;
		}
		case GameEventType.TUTORIAL_SELECT_DECKVIEWERLIST_SLOTTYPE:
		{
			if (!NKCUIDeckViewer.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			if (!eventTemplet.StringValue.TryParse<NKCDeckViewUnitSelectList.SlotType>(out var @enum))
			{
				ProcessEvent();
				break;
			}
			NKCUIUnitSelectListSlotBase slot = NKCUIDeckViewer.Instance.GetTutorialSelectSlotType(@enum);
			if (slot == null)
			{
				ProcessEvent();
				break;
			}
			OpenTutorialGuideBySettedFace(slot.GetComponent<RectTransform>(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, null);
			NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
			{
				slot.InvokeClick();
				ProcessEvent();
			});
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_ACHIEVEMENT_SLOT:
		case GameEventType.TUTORIAL_CLICK_ACHIEVEMENT_SLOT:
		{
			if (!NKCUIMissionAchievement.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			RectTransform rectTransform4 = NKCUIMissionAchievement.Instance.GetRectTransformSlot(eventTemplet.Value);
			if (rectTransform4 == null)
			{
				ProcessEvent();
				break;
			}
			if (!string.IsNullOrEmpty(eventTemplet.StringValue))
			{
				Transform transform = rectTransform4.Find(eventTemplet.StringValue);
				if (transform != null)
				{
					rectTransform4 = transform.GetComponent<RectTransform>();
				}
			}
			bool flag2 = IsEnableTouch(rectTransform4);
			if (eventTemplet.EventType == GameEventType.TUTORIAL_CLICK_ACHIEVEMENT_SLOT && flag2)
			{
				SetButtonClickSteal(rectTransform4, eventTemplet);
			}
			else
			{
				OpenTutorialGuideBySettedFace(rectTransform4, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_COUNTERCASE:
		case GameEventType.TUTORIAL_CLICK_COUNTERCASE:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OPERATION)
			{
				ProcessEvent();
				break;
			}
			RectTransform counterCaseSlot = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetCounterCaseSlot(eventTemplet.Value);
			if (counterCaseSlot == null)
			{
				ProcessEvent();
			}
			else if (eventTemplet.EventType == GameEventType.TUTORIAL_CLICK_COUNTERCASE)
			{
				SetButtonClickSteal(counterCaseSlot, eventTemplet);
			}
			else
			{
				OpenTutorialGuideBySettedFace(counterCaseSlot, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_COUNTERCASELIST:
		case GameEventType.TUTORIAL_CLICK_COUNTERCASELIST:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OPERATION)
			{
				ProcessEvent();
				break;
			}
			NKCUICCNormalSlot counterCaseListItem = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetCounterCaseListItem(eventTemplet.Value);
			if (counterCaseListItem == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform targetRect2 = (string.IsNullOrEmpty(eventTemplet.StringValue) ? counterCaseListItem.GetComponent<RectTransform>() : counterCaseListItem.GetBtnRect());
			if (eventTemplet.EventType == GameEventType.TUTORIAL_CLICK_COUNTERCASELIST)
			{
				SetButtonClickSteal(targetRect2, eventTemplet);
			}
			else
			{
				OpenTutorialGuideBySettedFace(targetRect2, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_FORGE_CRAFT_SLOT:
		case GameEventType.TUTORIAL_CLICK_FORGE_CRAFT_SLOT:
		{
			if (!NKCUIForgeCraft.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			NKCUIForgeCraftSlot slot2 = NKCUIForgeCraft.Instance.GetSlot(eventTemplet.Value);
			if (slot2 == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform targetRect = (string.IsNullOrEmpty(eventTemplet.StringValue) ? slot2.GetComponent<RectTransform>() : slot2.GetButtonRect());
			if (eventTemplet.EventType == GameEventType.TUTORIAL_CLICK_FORGE_CRAFT_SLOT)
			{
				SetButtonClickSteal(targetRect, eventTemplet);
			}
			else
			{
				OpenTutorialGuideBySettedFace(targetRect, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_FORGE_CRAFT_MOLD:
		case GameEventType.TUTORIAL_CLICK_FORGE_CRAFT_MOLD:
		{
			if (!NKCUIForgeCraftMold.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			NKCUIForgeCraftMoldSlot moldSlot = NKCUIForgeCraftMold.Instance.GetMoldSlot(eventTemplet.Value);
			if (moldSlot == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform targetRect3 = (string.IsNullOrEmpty(eventTemplet.StringValue) ? moldSlot.GetComponent<RectTransform>() : moldSlot.GetButtonRect());
			if (eventTemplet.EventType == GameEventType.TUTORIAL_CLICK_FORGE_CRAFT_MOLD)
			{
				SetButtonClickSteal(targetRect3, eventTemplet);
			}
			else
			{
				OpenTutorialGuideBySettedFace(targetRect3, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_HANGAR_BUILD_SLOT:
		case GameEventType.TUTORIAL_CLICK_HANGAR_BUILD_SLOT:
		{
			if (!NKCUIHangarBuild.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			NKCUIHangarBuildSlot slot3 = NKCUIHangarBuild.Instance.GetSlot(eventTemplet.Value);
			if (slot3 == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform rectTransform3 = null;
			if (!string.IsNullOrEmpty(eventTemplet.StringValue))
			{
				rectTransform3 = slot3.GetRect(eventTemplet.StringValue);
			}
			if (rectTransform3 == null)
			{
				rectTransform3 = slot3.GetComponent<RectTransform>();
			}
			if (eventTemplet.EventType == GameEventType.TUTORIAL_CLICK_HANGAR_BUILD_SLOT)
			{
				SetButtonClickSteal(rectTransform3, eventTemplet);
			}
			else
			{
				OpenTutorialGuideBySettedFace(rectTransform3, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.TUTORIAL_TOUCH_ENHANCE_USE_SLOT:
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_BASE)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(NKCScenManager.GetScenManager().Get_SCEN_BASE().GetUILab()
					.GetEnhanceItemSlotRect(eventTemplet.Value), eventTemplet);
			}
			break;
		case GameEventType.TUTORIAL_TOUCH_SKILL_LEVELUP_ICON:
		{
			NKCUIUnitInfo openedUIByType4 = NKCUIManager.GetOpenedUIByType<NKCUIUnitInfo>();
			if (openedUIByType4 == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform skillLevelSlotRect = openedUIByType4.GetSkillLevelSlotRect(eventTemplet.Value);
			if (skillLevelSlotRect == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(skillLevelSlotRect, eventTemplet);
			}
			break;
		}
		case GameEventType.TUTORIAL_SELECT_UNITLIST_UNIT:
		{
			NKCUIUnitSelectList openedUIByType3 = NKCUIManager.GetOpenedUIByType<NKCUIUnitSelectList>();
			if (openedUIByType3 == null)
			{
				ProcessEvent();
				break;
			}
			NKCUnitSortSystem.UnitListOptions options3 = new NKCUnitSortSystem.UnitListOptions
			{
				eDeckType = NKM_DECK_TYPE.NDT_NORMAL,
				setExcludeUnitID = null,
				setOnlyIncludeUnitID = new HashSet<int>(),
				setDuplicateUnitID = null,
				setExcludeUnitUID = null,
				bExcludeLockedUnit = false,
				bExcludeDeckedUnit = false,
				setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>(),
				lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false),
				bDescending = true,
				bHideDeckedUnit = false,
				bPushBackUnselectable = true,
				bIncludeUndeckableUnit = true
			};
			options3.setOnlyIncludeUnitID.Add(eventTemplet.Value);
			string[] array = eventTemplet.StringValue.Split(',', ' ');
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				switch (array[num2])
				{
				case "INCLUDE_DECKED_UNIT":
					options3.bExcludeDeckedUnit = false;
					options3.bHideDeckedUnit = false;
					break;
				case "EXCLUDE_DECKED_UNIT":
					options3.bExcludeDeckedUnit = true;
					break;
				case "EXCLUDE_LOCKED_UNIT":
					options3.bExcludeLockedUnit = true;
					break;
				case "INCLUDE_LOCKED_UNIT":
					options3.bExcludeLockedUnit = false;
					break;
				case "HIGHEST_LEVEL":
					options3.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.Level_High };
					options3.bDescending = true;
					break;
				case "LOWEST_LEVEL":
					options3.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.Level_Low };
					options3.bDescending = false;
					break;
				case "UNSELECTED":
					if (options3.setExcludeUnitUID == null)
					{
						options3.setExcludeUnitUID = new HashSet<long>(openedUIByType3.GetSelectedUnitList());
					}
					else
					{
						options3.setExcludeUnitUID.UnionWith(openedUIByType3.GetSelectedUnitList());
					}
					break;
				case "UID_FIRST":
					options3.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.UID_First };
					options3.bDescending = true;
					break;
				case "UID_LAST":
					options3.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.UID_Last };
					options3.bDescending = true;
					break;
				case "FILTER_FUNC_CAN_LIMIT_BREAK":
					options3.AdditionalExcludeFilterFunc = NKMUnitLimitBreakManager.CanThisUnitLimitBreak;
					break;
				}
			}
			NKMUnitData nKMUnitData2 = new NKCUnitSort(NKCScenManager.CurrentUserData(), options3).AutoSelect(null);
			if (nKMUnitData2 == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(openedUIByType3.ScrollToUnitAndGetRect(nKMUnitData2.m_UnitUID), eventTemplet);
			}
			break;
		}
		case GameEventType.TUTORIAL_SELECT_UNITLIST_SHIP:
		{
			NKCUIUnitSelectList openedUIByType2 = NKCUIManager.GetOpenedUIByType<NKCUIUnitSelectList>();
			if (openedUIByType2 == null)
			{
				ProcessEvent();
				break;
			}
			NKCUnitSortSystem.UnitListOptions options2 = new NKCUnitSortSystem.UnitListOptions
			{
				eDeckType = NKM_DECK_TYPE.NDT_NORMAL,
				setExcludeUnitID = null,
				setOnlyIncludeUnitID = new HashSet<int>(),
				setDuplicateUnitID = null,
				setExcludeUnitUID = null,
				bExcludeLockedUnit = false,
				bExcludeDeckedUnit = false,
				setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>(),
				lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_SHIP, bIsCollection: false),
				bDescending = true,
				bHideDeckedUnit = false,
				bPushBackUnselectable = true,
				bIncludeUndeckableUnit = true
			};
			options2.setOnlyIncludeUnitID.Add(eventTemplet.Value);
			string[] array = eventTemplet.StringValue.Split(',', ' ');
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				switch (array[num2])
				{
				case "INCLUDE_DECKED_UNIT":
					options2.bExcludeDeckedUnit = false;
					options2.bHideDeckedUnit = false;
					break;
				case "EXCLUDE_DECKED_UNIT":
					options2.bExcludeDeckedUnit = true;
					break;
				case "EXCLUDE_LOCKED_UNIT":
					options2.bExcludeLockedUnit = true;
					break;
				case "INCLUDE_LOCKED_UNIT":
					options2.bExcludeLockedUnit = false;
					break;
				case "HIGHEST_LEVEL":
					options2.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.Level_High };
					options2.bDescending = true;
					break;
				case "LOWEST_LEVEL":
					options2.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.Level_Low };
					options2.bDescending = false;
					break;
				case "UNSELECTED":
					if (options2.setExcludeUnitUID == null)
					{
						options2.setExcludeUnitUID = new HashSet<long>(openedUIByType2.GetSelectedUnitList());
					}
					else
					{
						options2.setExcludeUnitUID.UnionWith(openedUIByType2.GetSelectedUnitList());
					}
					break;
				case "FILTER_FUNC_CAN_LIMIT_BREAK":
					options2.AdditionalExcludeFilterFunc = NKMUnitLimitBreakManager.CanThisUnitLimitBreak;
					break;
				}
			}
			NKMUnitData nKMUnitData = new NKCShipSort(NKCScenManager.CurrentUserData(), options2).AutoSelect(null);
			if (nKMUnitData == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(openedUIByType2.ScrollToUnitAndGetRect(nKMUnitData.m_UnitUID), eventTemplet);
			}
			break;
		}
		case GameEventType.TUTORIAL_SELECT_ITEMLIST_EQUIP:
		{
			NKCUIInventory openedUIByType = NKCUIManager.GetOpenedUIByType<NKCUIInventory>();
			if (openedUIByType == null)
			{
				ProcessEvent();
				break;
			}
			NKCEquipSortSystem.EquipListOptions options = default(NKCEquipSortSystem.EquipListOptions);
			options.setOnlyIncludeEquipID = new HashSet<int>();
			options.setOnlyIncludeEquipID.Add(eventTemplet.Value);
			options.bHideEquippedItem = false;
			options.bHideLockItem = false;
			options.bHideMaxLvItem = false;
			options.bLockMaxItem = false;
			options.bHideNotPossibleSetOptionItem = false;
			string[] array = eventTemplet.StringValue.Split(',', ' ');
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				switch (array[num2])
				{
				case "EXCLUDE_EQUIPPED_ITEM":
					options.bHideEquippedItem = true;
					break;
				case "EXCLUDE_LOCKED_ITEM":
					options.bHideLockItem = true;
					break;
				case "EXCLUDE_MAX_ITEM":
					options.bLockMaxItem = true;
					break;
				case "HIGHEST_LEVEL":
					options.lstSortOption = new List<NKCEquipSortSystem.eSortOption> { NKCEquipSortSystem.eSortOption.Enhance_High };
					break;
				case "LOWEST_LEVEL":
					options.lstSortOption = new List<NKCEquipSortSystem.eSortOption> { NKCEquipSortSystem.eSortOption.Enhance_Low };
					break;
				case "UID_FIRST":
					options.lstSortOption = new List<NKCEquipSortSystem.eSortOption> { NKCEquipSortSystem.eSortOption.UID_First };
					break;
				case "UID_LAST":
					options.lstSortOption = new List<NKCEquipSortSystem.eSortOption> { NKCEquipSortSystem.eSortOption.UID_Last };
					break;
				case "UNSELECTED":
					if (options.setExcludeEquipUID == null)
					{
						options.setExcludeEquipUID = openedUIByType.GetSelectedEquips();
					}
					else
					{
						options.setExcludeEquipUID.UnionWith(openedUIByType.GetSelectedEquips());
					}
					break;
				}
			}
			NKMEquipItemData nKMEquipItemData = new NKCEquipSortSystem(NKCScenManager.CurrentUserData(), options).AutoSelect(null);
			if (nKMEquipItemData == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(openedUIByType.ScrollToUnitAndGetRect(nKMEquipItemData.m_ItemUid), eventTemplet);
			}
			break;
		}
		case GameEventType.TUTORIAL_SET_TALKER:
			m_strCurrentTalkInvenIcon = eventTemplet.StringValue;
			ProcessEvent();
			break;
		case GameEventType.TUTORIAL_SET_SCREEN_BG_ALPHA:
			m_fGuideScreenAlpha = (float)eventTemplet.Value / 100f;
			if (m_fGuideScreenAlpha <= 0f)
			{
				m_fGuideScreenAlpha = 0.01f;
			}
			ProcessEvent();
			break;
		case GameEventType.TUTORIAL_IMAGE_GUIDE:
			NKCUIPopupTutorialImagePanel.Instance.Open(eventTemplet.StringValue, ProcessEvent);
			break;
		case GameEventType.BEGIN_SUM_ITEMOPEN_RESULT:
			RandomBoxDataCollecting = true;
			lstCollectReward = new List<NKMRewardData>();
			ProcessEvent();
			break;
		case GameEventType.END_AND_SHOW_ITEMOPEN_RESULT:
			NKCUIResult.Instance.OpenBoxGain(NKCScenManager.CurrentUserData().m_ArmyData, lstCollectReward, eventTemplet.Value, ProcessEvent);
			lstCollectReward = null;
			RandomBoxDataCollecting = false;
			break;
		case GameEventType.OPEN_MISC_ITEM_RANDOM_BOX:
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(eventTemplet.StringValue);
			if (nKMItemMiscTemplet == null)
			{
				ProcessEvent();
				break;
			}
			if (!nKMItemMiscTemplet.IsUsable())
			{
				ProcessEvent();
				break;
			}
			NKMItemMiscData itemMisc = NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(nKMItemMiscTemplet);
			if (itemMisc != null && itemMisc.TotalCount >= eventTemplet.Value)
			{
				NKCPacketSender.Send_NKMPacket_RANDOM_ITEM_BOX_OPEN_REQ(nKMItemMiscTemplet.m_ItemMiscID, eventTemplet.Value);
			}
			else
			{
				ProcessEvent();
			}
			break;
		}
		case GameEventType.TUTORIAL_CONTRACT_FIND_BANNER:
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_CONTRACT)
			{
				ProcessEvent();
				break;
			}
			if (ContractTempletBase.Find(eventTemplet.StringValue) == null)
			{
				ProcessEvent();
				break;
			}
			NKCScenManager.GetScenManager().GET_SCEN_CONTRACT().SelectRecruitBanner(eventTemplet.StringValue);
			ProcessEvent();
			break;
		case GameEventType.TUTORIAL_WORLDMAP_FIND_CITY:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WORLDMAP)
			{
				ProcessEvent();
				break;
			}
			NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(eventTemplet.StringValue);
			if (cityTemplet == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().GetCityRect(cityTemplet.m_ID), eventTemplet, bMiddleCanvas: true);
			}
			break;
		}
		case GameEventType.TUTORIAL_WORLDMAP_FIND_CITY_LEVEL:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WORLDMAP)
			{
				ProcessEvent();
				break;
			}
			NKMWorldMapData worldmapData = NKCScenManager.CurrentUserData().m_WorldmapData;
			if (worldmapData == null)
			{
				ProcessEvent();
				break;
			}
			Dictionary<int, NKMWorldMapCityData> worldMapCityDataMap = worldmapData.worldMapCityDataMap;
			if (worldMapCityDataMap == null)
			{
				ProcessEvent();
				break;
			}
			int num3 = -1;
			int value = eventTemplet.Value;
			foreach (KeyValuePair<int, NKMWorldMapCityData> item3 in worldMapCityDataMap)
			{
				if (item3.Value.level >= value)
				{
					num3 = item3.Value.cityID;
					break;
				}
			}
			if (num3 < 0)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().GetCityRect(num3), eventTemplet, bMiddleCanvas: true);
			}
			break;
		}
		case GameEventType.LOBBY_MENU_TAB:
			Debug.LogWarning("LOBBY_MENU_TAB : Obsolete Event!");
			NKCScenManager.GetScenManager().GetNowScenID();
			_ = 2;
			ProcessEvent();
			break;
		case GameEventType.BASE_MENU_TYPE:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_BASE)
			{
				ProcessEvent();
				break;
			}
			if (Enum.TryParse<NKCUIBaseSceneMenu.BaseSceneMenuType>(eventTemplet.StringValue, out var result3))
			{
				NKCScenManager.GetScenManager().Get_SCEN_BASE().SetBaseMenuType(result3);
			}
			ProcessEvent();
			break;
		}
		case GameEventType.PLAY_CUTSCENE:
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
			{
				if (!m_bIsPauseEvent)
				{
					Debug.LogError("CUTSCENE EVENT IN GAME PLAY MUST BE PAUSE EVENT!");
					ProcessEvent();
					break;
				}
				NKMUserData nKMUserData2 = NKCScenManager.CurrentUserData();
				NKCGameClient gameClient3 = NKCScenManager.GetScenManager().GetGameClient();
				if (gameClient3 != null && !NKCScenManager.CurrentUserData().m_UserOption.m_bPlayCutscene && nKMUserData2.CheckStageCleared(gameClient3.GetGameData()))
				{
					Debug.Log("Skipping cutscene..");
					ProcessEvent();
					break;
				}
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
			{
				NKCScenManager.GetScenManager().GetGameClient().GetGameHud()
					.HUD_HIDE(bHideCompletly: true);
				NKCUICutScenPlayer.Instance.LoadAndPlay(eventTemplet.StringValue, 0, EndCutScene, bAsync: false);
			}
			else
			{
				NKCUICutScenPlayer.Instance.LoadAndPlay(eventTemplet.StringValue, 0, EndCutScene);
			}
			break;
		case GameEventType.RESULT_GET_UNIT:
		{
			string[] array2 = eventTemplet.StringValue.Split(',', ' ');
			List<NKMUnitData> list = new List<NKMUnitData>();
			string[] array = array2;
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				if (int.TryParse(array[num2], out var result2) && NKMUnitManager.GetUnitTempletBase(result2) != null)
				{
					NKMUnitData item = new NKMUnitData(result2, 0L, islock: false, isPermanentContract: false, isSeized: false, fromContract: false);
					list.Add(item);
				}
			}
			if (list.Count == 0)
			{
				ProcessEvent();
				break;
			}
			NKMRewardData nKMRewardData = new NKMRewardData();
			nKMRewardData.SetUnitData(list);
			NKCUIResult.Instance.OpenRewardGain(NKCScenManager.CurrentUserData().m_ArmyData, nKMRewardData, NKCUtilString.GET_STRING_GET_UNIT, "", WaitFinished);
			ProcessEvent();
			break;
		}
		case GameEventType.OPEN_NICKNAME_CHANGE_POPUP:
			NKCPopupNickname.Instance.Open(ProcessEvent);
			break;
		case GameEventType.REFRESH_SCENE:
			NKCScenManager.GetScenManager().ScenChangeFade(NKCScenManager.GetScenManager().GetNowScenID());
			ProcessEvent();
			break;
		case GameEventType.RESET_WORLDMAP:
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WORLDMAP)
			{
				ProcessEvent();
				break;
			}
			if (NKCUIWorldMap.IsInstanceOpen)
			{
				NKCUIWorldMap.GetInstance().CloseCityManagementUI();
			}
			ProcessEvent();
			break;
		case GameEventType.TUTORIAL_TOUCH_WORLDMAP_BUILDING_EMPLTY:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WORLDMAP)
			{
				ProcessEvent();
				break;
			}
			if (!NKCUIWorldMap.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			if (!NKCUIWorldMap.GetInstance().m_UICityManagement.IsOpen)
			{
				ProcessEvent();
				break;
			}
			NKCUIWorldmapCityBuildPanel uICityBuilding = NKCUIWorldMap.GetInstance().m_UICityManagement.m_UICityBuilding;
			if (!uICityBuilding.gameObject.activeInHierarchy)
			{
				ProcessEvent();
				break;
			}
			RectTransform emptySlot = uICityBuilding.GetEmptySlot();
			if (emptySlot == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(emptySlot, eventTemplet);
			}
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_WORLDMAP_BUILD_SLOT:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_WORLDMAP)
			{
				ProcessEvent();
				break;
			}
			if (!NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().IsOpenPopupWorldMapNewBuildingList)
			{
				ProcessEvent();
				break;
			}
			NKCPopupWorldMapNewBuildingList popupWorldMapNewBuildingList = NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().PopupWorldMapNewBuildingList;
			if (popupWorldMapNewBuildingList == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform buildingSlot = popupWorldMapNewBuildingList.GetBuildingSlot(eventTemplet.Value);
			if (buildingSlot == null)
			{
				ProcessEvent();
			}
			else
			{
				OpenTutorialGuideBySettedFace(buildingSlot, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.TUTORIAL_HIGHLIGHT_SHAODW_PALACE_SLOT:
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_SHADOW_PALACE)
			{
				ProcessEvent();
				break;
			}
			if (!NKCUIShadowPalace.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			RectTransform palaceSlot = NKCUIShadowPalace.GetInstance().GetPalaceSlot(eventTemplet.Value);
			if (palaceSlot == null)
			{
				ProcessEvent();
			}
			else
			{
				OpenTutorialGuideBySettedFace(palaceSlot, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.MOVE_MINIMAP:
			if (!NKCUIOfficeMapFront.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			OpenTutorialGuide(null, NKCUIOverlayTutorialGuide.ClickGuideType.None, "", null);
			NKCUIOverlayTutorialGuide.Instance.SetBGScreenAlpha(0.01f);
			NKCUIOfficeMapFront.GetInstance().MoveMiniMap((float)eventTemplet.Value / 100f, ProcessEvent);
			break;
		case GameEventType.OFFICE_HIGHLIGHT:
		{
			if (!NKCUIOfficeMapFront.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			IOfficeMinimap currentMinimap2 = NKCUIOfficeMapFront.GetInstance().GetCurrentMinimap();
			if (currentMinimap2 == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform tileRectTransform2 = currentMinimap2.GetTileRectTransform(eventTemplet.Value);
			if (tileRectTransform2 == null)
			{
				ProcessEvent();
			}
			else
			{
				OpenTutorialGuideBySettedFace(tileRectTransform2, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.OFFICE_TOUCH:
		{
			if (!NKCUIOfficeMapFront.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			IOfficeMinimap currentMinimap = NKCUIOfficeMapFront.GetInstance().GetCurrentMinimap();
			if (currentMinimap == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform tileRectTransform = currentMinimap.GetTileRectTransform(eventTemplet.Value);
			if (tileRectTransform == null)
			{
				ProcessEvent();
			}
			else
			{
				OpenTutorialGuideBySettedFace(tileRectTransform, NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, ProcessEvent);
			}
			break;
		}
		case GameEventType.OFFICE_UNITLIST_UNIT:
		{
			if (!NKCUIPopupOfficeMemberEdit.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			NKCUIPopupOfficeMemberEdit.Instance.SortSpecifitUnitFirst(eventTemplet.Value);
			RectTransform rectTransformUnitSlot = NKCUIPopupOfficeMemberEdit.Instance.GetRectTransformUnitSlot(eventTemplet.Value);
			if (rectTransformUnitSlot == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(rectTransformUnitSlot, eventTemplet);
			}
			break;
		}
		case GameEventType.OFFICE_ITEMLIST_FURNITURE:
		{
			if (!NKCUIPopupOfficeInteriorSelect.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			RectTransform tutorialItemSlot = NKCUIPopupOfficeInteriorSelect.Instance.GetTutorialItemSlot(eventTemplet.Value);
			if (tutorialItemSlot == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(tutorialItemSlot, eventTemplet);
			}
			break;
		}
		case GameEventType.OFFICE_FURNITURE_HIGHLIGHT:
		{
			GameObject gameObject2 = FindGameObject(eventTemplet.StringValue);
			if (gameObject2 == null)
			{
				ProcessEvent();
				break;
			}
			NKCOfficeFuniture component2 = gameObject2.GetComponent<NKCOfficeFuniture>();
			if (component2 == null)
			{
				ProcessEvent();
				break;
			}
			RectTransform rectTransform2 = component2.MakeHighlightRect();
			Vector3 centerWorldPos2 = rectTransform2.GetCenterWorldPos();
			NKCCamera.SetPos(centerWorldPos2.x, centerWorldPos2.y);
			OpenTutorialGuideBySettedFace(rectTransform2, NKCUIOverlayTutorialGuide.ClickGuideType.None, eventTemplet, ProcessEvent, bMiddleCanvas: true);
			break;
		}
		case GameEventType.OFFICE_FURNITURE_TOUCH:
		{
			GameObject gameObject = FindGameObject(eventTemplet.StringValue);
			if (gameObject == null)
			{
				ProcessEvent();
				break;
			}
			NKCOfficeFuniture component = gameObject.GetComponent<NKCOfficeFuniture>();
			if (component == null)
			{
				ProcessEvent();
				break;
			}
			Vector3 centerWorldPos = component.MakeHighlightRect().GetCenterWorldPos();
			NKCCamera.SetPos(centerWorldPos.x, centerWorldPos.y);
			SetFurnitureButtonSteal(component, eventTemplet);
			break;
		}
		case GameEventType.REARM_UNITLIST_UNIT:
		{
			if (!NKCUIRearmament.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			RectTransform rearmSlotRectTransform = NKCUIRearmament.Instance.GetRearmSlotRectTransform(eventTemplet.Value);
			if (rearmSlotRectTransform == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(rearmSlotRectTransform, eventTemplet);
			}
			break;
		}
		case GameEventType.EXTRACT_SLOT_SELECT:
		{
			if (!NKCUIRearmament.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			RectTransform extractSlotRectTransform2 = NKCUIRearmament.Instance.GetExtractSlotRectTransform(eventTemplet.Value);
			if (extractSlotRectTransform2 == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(extractSlotRectTransform2, eventTemplet);
			}
			break;
		}
		case GameEventType.EXTRACT_UNITLIST_UNIT:
		{
			if (!NKCUIRearmament.IsInstanceOpen || !NKCUIUnitSelectList.IsInstanceOpen)
			{
				ProcessEvent();
				break;
			}
			RectTransform extractSlotRectTransform = NKCUIRearmament.Instance.GetExtractSlotRectTransform(eventTemplet.Value);
			if (extractSlotRectTransform == null)
			{
				ProcessEvent();
			}
			else
			{
				SetButtonClickSteal(extractSlotRectTransform, eventTemplet);
			}
			break;
		}
		case GameEventType.TOGGLE_UI_CANVAS:
		{
			if (!Enum.TryParse<NKCUIManager.eUIBaseRect>(eventTemplet.StringValue, out var result))
			{
				ProcessEvent();
				break;
			}
			NKCUtil.SetGameobjectActive(NKCUIManager.GetUIBaseRect(result), eventTemplet.Value != 0);
			ProcessEvent();
			break;
		}
		case GameEventType.PLAY_MUSIC:
		{
			float num = (float)eventTemplet.Value / 100f;
			if (num < 0f)
			{
				num = NKCSoundManager.GetMusicTime();
			}
			NKCSoundManager.PlayMusic(eventTemplet.StringValue, bLoop: true, 1f, bForce: true, num);
			ProcessEvent();
			break;
		}
		default:
			Debug.LogWarning("Not Implemented yet");
			ProcessEvent();
			break;
		case GameEventType.WAIT:
			break;
		}
	}

	private static void SetButtonClickSteal(RectTransform targetRect, NKCGameEventTemplet eventTemplet, bool bMiddleCanvas = false)
	{
		if (targetRect == null)
		{
			ProcessEvent();
			return;
		}
		NKCUIComButton comButton = targetRect.GetComponentInChildren<NKCUIComButton>();
		NKCUIComStateButton stateButton = targetRect.GetComponentInChildren<NKCUIComStateButton>();
		NKCUIComToggle comToggle = targetRect.GetComponentInChildren<NKCUIComToggle>();
		OpenTutorialGuideBySettedFace(targetRect, NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, null, bMiddleCanvas);
		NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
		{
			if (comButton != null && comButton.enabled)
			{
				comButton.PointerClick?.Invoke();
				if (!string.IsNullOrEmpty(comButton.m_SoundForPointClick))
				{
					NKCSoundManager.PlaySound(comButton.m_SoundForPointClick, 1f, 0f, 0f);
				}
			}
			if (stateButton != null && stateButton.enabled)
			{
				stateButton.PointerClick?.Invoke();
				if (!string.IsNullOrEmpty(stateButton.m_SoundForPointClick))
				{
					NKCSoundManager.PlaySound(stateButton.m_SoundForPointClick, 1f, 0f, 0f);
				}
			}
			if (comToggle != null && comToggle.enabled)
			{
				comToggle.Select(!comToggle.m_bChecked);
				if (!string.IsNullOrEmpty(comToggle.m_SoundForPointClick))
				{
					NKCSoundManager.PlaySound(comToggle.m_SoundForPointClick, 1f, 0f, 0f);
				}
			}
			if (GetCurrentEventID() == eventTemplet.EventID)
			{
				ProcessEvent();
			}
			else
			{
				FinishEventTemplet(eventTemplet);
			}
		});
	}

	private static void SetFurnitureButtonSteal(NKCOfficeFuniture furniture, NKCGameEventTemplet eventTemplet)
	{
		if (furniture == null)
		{
			ProcessEvent();
			return;
		}
		OpenTutorialGuideBySettedFace(furniture.MakeHighlightRect(), NKCUIOverlayTutorialGuide.ClickGuideType.Touch, eventTemplet, null, bMiddleCanvas: true);
		NKCUIOverlayTutorialGuide.Instance.SetStealInput(delegate
		{
			furniture.InvokeTouchEvent();
			if (GetCurrentEventID() == eventTemplet.EventID)
			{
				ProcessEvent();
			}
			else
			{
				FinishEventTemplet(eventTemplet);
			}
		});
	}

	public static void OpenTutorialGuideBySettedFace(RectTransform targetRect, NKCUIOverlayTutorialGuide.ClickGuideType guideType, NKCGameEventTemplet eventTemplet, UnityAction onComplete, bool bMiddleCanvas = false)
	{
		if (!string.IsNullOrWhiteSpace(m_strCurrentTalkInvenIcon))
		{
			OpenTutorialGuide(targetRect.GetComponent<RectTransform>(), guideType, "", onComplete, bMiddleCanvas);
			if (!string.IsNullOrWhiteSpace(eventTemplet.Text))
			{
				NKCUIOverlayCharMessage.Instance.Open(m_strCurrentTalkInvenIcon, eventTemplet.Text, 9999f, null);
				NKCUIOverlayCharMessage.Instance.SetBGScreenAlpha(m_fGuideScreenAlpha);
			}
		}
		else
		{
			OpenTutorialGuide(targetRect.GetComponent<RectTransform>(), guideType, eventTemplet.Text, onComplete, bMiddleCanvas);
		}
	}

	private static void FinishEventTemplet(NKCGameEventTemplet eventTemplet)
	{
		if (eventTemplet != null)
		{
			NKCUIOverlayTutorialGuide.CheckInstanceAndClose();
			NKCUIOverlayCharMessage.CheckInstanceAndClose();
			NKCUIPopupTutorialImagePanel.CheckInstanceAndClose();
			if (eventTemplet.EventType == GameEventType.PLAY_CUTSCENE)
			{
				NKCUICutScenPlayer.CheckInstanceAndClose();
				NKCTutorialManager.TutorialRequiredByLastPoint();
			}
		}
	}

	private static bool IsEnableTouch(RectTransform rect)
	{
		if (!rect.GetComponent<NKCUIComButton>() && !rect.GetComponent<NKCUIComStateButton>())
		{
			return rect.GetComponent<NKCUIComToggle>();
		}
		return true;
	}

	private static GameObject FindGameObject(string name)
	{
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject));
		if (array == null || array.Length == 0)
		{
			return null;
		}
		List<UnityEngine.Object> list = array.ToList().FindAll((UnityEngine.Object v) => v.name == name);
		if (list == null || list.Count == 0)
		{
			return null;
		}
		if (list.Count > 1)
		{
			for (int num = 0; num < list.Count; num++)
			{
				GameObject gameObject = list[num] as GameObject;
				if (gameObject.activeInHierarchy)
				{
					return gameObject;
				}
			}
		}
		return list[0] as GameObject;
	}

	public static void ClearEvent()
	{
		if (m_lstCurrentEvent != null)
		{
			if (m_CurrentIndex >= 0)
			{
				FinishEventTemplet(m_lstCurrentEvent[m_CurrentIndex]);
			}
			m_lstCurrentEvent = null;
			m_CurrentIndex = -1;
			dOnEventFinish?.Invoke(m_bIsPauseEvent);
			dOnEventFinish = null;
		}
	}

	public static void Drop()
	{
		m_dicGameEventTemplet = null;
	}
}
