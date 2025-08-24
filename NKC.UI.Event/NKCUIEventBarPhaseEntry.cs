using System.Collections;
using System.Collections.Generic;
using NKC.UI.NPC;
using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventBarPhaseEntry : MonoBehaviour
{
	public NKCUIEventBarCocktailSlot[] m_EventBarCocktailSlotArray;

	public NKCUINPCSpineIllust m_npcSpineIllust;

	public GameObject m_objBarLock;

	public GameObject m_objLaughFx;

	public ScrollRect m_scrollRect;

	public NKCUIComStateButton m_csbtnMomoErrand;

	public GameObject m_objErrandRedDot;

	[Header("리퀘스트 대사 창 유지 시간")]
	public float m_showScriptTime;

	[Header("바 유닛 ID")]
	public int m_unitID;

	public int m_skinID;

	[Header("칵테일 버블")]
	public NKCUIEventBarBubble m_eventBarBubble;

	[Header("대사 창")]
	public Animator m_aniScript;

	public CanvasGroup m_scriptCanvasGroup;

	public GameObject m_objScriptRoot;

	public GameObject m_objScriptType1;

	public GameObject m_objScriptType2;

	public Text m_lbUnitName;

	public Text m_lbType1Msg;

	public Text m_lbType2Msg;

	public NKCUIComStateButton m_csbtnGive;

	public NKCUIComStateButton m_csbtnStay;

	public NKCUIComStateButton m_csbtnScriptPanel;

	[Header("모모 공물 보상 미션 TabId")]
	public int m_errandRewardMissionTabId;

	[Header("칵테일 재료 보상 미션 GroupId")]
	public int m_cocktailRewardMissionGroupId;

	[Header("대사")]
	public string m_Request;

	public string m_GiveDesc;

	public string m_GiveEnd;

	public string m_GiveCancel;

	public string m_SelectCocktail;

	public string m_WrongCocktail;

	public string m_NeedMoreCocktail;

	private NKCUITypeWriter m_typeWriter = new NKCUITypeWriter();

	private List<int> cockTailIDList = new List<int>();

	private int m_eventID;

	private int m_selectedCocktailID;

	private float m_fScriptTimer;

	private Coroutine m_scriptCoroutine;

	private bool showDeliverFinishScript;

	public void Init()
	{
		if (m_EventBarCocktailSlotArray != null)
		{
			int num = m_EventBarCocktailSlotArray.Length;
			for (int i = 0; i < num; i++)
			{
				m_EventBarCocktailSlotArray[i].Init();
			}
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnGive, OnClickGive);
		NKCUtil.SetButtonClickDelegate(m_csbtnStay, OnClickStay);
		NKCUtil.SetButtonClickDelegate(m_csbtnMomoErrand, OnClickMomoErrand);
		NKCUtil.SetButtonClickDelegate(m_csbtnScriptPanel, OnClickScriptPanel);
		NKCEventBarManager.RewardPopupUnitID = m_unitID;
		NKCEventBarManager.RewardPopupSkinID = m_skinID;
		m_eventBarBubble?.Init();
		if (m_npcSpineIllust != null)
		{
			m_npcSpineIllust.m_dOnTouch = OnCharacterTouch;
		}
		if (m_scrollRect != null)
		{
			NKCUtil.SetScrollHotKey(m_scrollRect);
			m_scrollRect.scrollSensitivity = NKCInputManager.ScrollSensibility;
		}
	}

	public void SetData(int eventID)
	{
		m_eventID = eventID;
		if (cockTailIDList.Count <= 0)
		{
			foreach (NKMEventBarTemplet value in NKMEventBarTemplet.Values)
			{
				if (value.EventID == eventID && !cockTailIDList.Contains(value.RewardItemId))
				{
					cockTailIDList.Add(value.RewardItemId);
				}
			}
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_unitID);
		m_eventBarBubble?.Hide();
		NKCUtil.SetLabelText(m_lbUnitName, (unitTempletBase != null) ? unitTempletBase.GetUnitName() : "");
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
		HideScript();
		if (NKCEventBarManager.RemainDeliveryLimitValue <= 0)
		{
			showDeliverFinishScript = true;
			NKCUtil.SetGameobjectActive(m_objLaughFx, bValue: true);
			m_npcSpineIllust?.SetDefaultAnimation(NKCASUIUnitIllust.GetAnimationName(NKCASUIUnitIllust.eAnimation.UNIT_LAUGH));
		}
		else
		{
			showDeliverFinishScript = false;
			m_eventBarBubble?.Show(NKCEventBarManager.DailyCocktailItemID, hideStart: true, OnTouchBubble);
			NKCUtil.SetGameobjectActive(m_objLaughFx, bValue: false);
			m_npcSpineIllust?.SetDefaultAnimation(NKCASUIUnitIllust.GetAnimationName(NKCASUIUnitIllust.eAnimation.UNIT_IDLE));
		}
		m_selectedCocktailID = 0;
		SetCocktailInfo();
		bool bValue = IsMomoMissionRedDotActive();
		NKCUtil.SetGameobjectActive(m_objErrandRedDot, bValue);
		if (m_scrollRect != null)
		{
			m_scrollRect.verticalNormalizedPosition = 1f;
		}
	}

	public void Refresh()
	{
		SetData(m_eventID);
	}

	public void Close()
	{
		if (cockTailIDList != null)
		{
			cockTailIDList.Clear();
		}
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
	}

	public void Hide()
	{
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
		HideScript();
	}

	private void Update()
	{
		m_typeWriter.Update();
		if (showDeliverFinishScript)
		{
			string message = NKCStringTable.GetString(m_GiveEnd);
			ShowScriptType1(message);
			m_fScriptTimer = m_showScriptTime;
			if (m_scriptCoroutine == null)
			{
				m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
			}
			showDeliverFinishScript = false;
		}
	}

	private void SetCocktailInfo()
	{
		if (m_EventBarCocktailSlotArray != null)
		{
			int count = cockTailIDList.Count;
			int num = m_EventBarCocktailSlotArray.Length;
			for (int i = 0; i < num; i++)
			{
				if (count <= i)
				{
					NKCUtil.SetGameobjectActive(m_EventBarCocktailSlotArray[i], bValue: false);
					continue;
				}
				NKCUtil.SetGameobjectActive(m_EventBarCocktailSlotArray[i], bValue: true);
				m_EventBarCocktailSlotArray[i].SetData(cockTailIDList[i], OnSelectCocktailSlot);
			}
		}
		NKCUtil.SetGameobjectActive(m_objBarLock, NKCEventBarManager.RemainDeliveryLimitValue <= 0);
	}

	private void ShowScriptType1(string message)
	{
		NKCUtil.SetGameobjectActive(m_objScriptRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objScriptType1, bValue: true);
		NKCUtil.SetGameobjectActive(m_objScriptType2, bValue: false);
		m_typeWriter.Start(m_lbType1Msg, message, 0f, _bTalkAppend: false);
	}

	private void ShowScriptType2(string message)
	{
		NKCUtil.SetGameobjectActive(m_objScriptRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objScriptType1, bValue: false);
		NKCUtil.SetGameobjectActive(m_objScriptType2, bValue: true);
		m_typeWriter.Start(m_lbType2Msg, message, 0f, _bTalkAppend: false);
		ToggleScriptType2Buttons(value: true);
	}

	private void HideScript()
	{
		NKCUtil.SetGameobjectActive(m_objScriptRoot, bValue: false);
		NKCUtil.SetLabelText(m_lbType1Msg, "");
		NKCUtil.SetLabelText(m_lbType2Msg, "");
	}

	private void OnCharacterTouch()
	{
		NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_TOUCH, m_unitID);
		if (NKCEventBarManager.RemainDeliveryLimitValue > 0)
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(NKCEventBarManager.DailyCocktailItemID);
			string message = string.Format(NKCStringTable.GetString(m_Request), (nKMItemMiscTemplet != null) ? nKMItemMiscTemplet.GetItemName() : "");
			ShowScriptType1(message);
			m_fScriptTimer = m_showScriptTime;
			if (m_scriptCoroutine == null)
			{
				m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
			}
		}
		else
		{
			string message2 = NKCStringTable.GetString(m_GiveEnd);
			ShowScriptType1(message2);
			m_fScriptTimer = m_showScriptTime;
			if (m_scriptCoroutine == null)
			{
				m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
			}
		}
		if (NKCEventBarManager.RemainDeliveryLimitValue > 0 && m_eventBarBubble != null && !m_eventBarBubble.gameObject.activeSelf)
		{
			m_eventBarBubble?.Show(NKCEventBarManager.DailyCocktailItemID, hideStart: false, OnTouchBubble);
		}
		m_eventBarBubble?.ResetAnimation();
	}

	private IEnumerator IOnShowRequestScript()
	{
		while (m_fScriptTimer > 0f)
		{
			m_fScriptTimer -= Time.deltaTime;
			yield return null;
		}
		yield return new WaitWhile(() => m_typeWriter.IsTyping());
		m_aniScript?.SetTrigger("OUTRO");
		yield return new WaitWhile(() => m_scriptCanvasGroup.alpha > 0f);
		HideScript();
		m_scriptCoroutine = null;
	}

	private void ToggleScriptType2Buttons(bool value)
	{
		NKCUtil.SetGameobjectActive(m_csbtnStay, value);
		NKCUtil.SetGameobjectActive(m_csbtnGive, value);
	}

	private long GetInventoryCocktailCount(int cocktailID)
	{
		long result = 0L;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			result = nKMUserData.m_InventoryData.GetCountMiscItem(cocktailID);
		}
		return result;
	}

	private int GetDailycocktailCount(int cocktailID)
	{
		return NKMEventBarTemplet.Find(cocktailID)?.DeliveryValue ?? 0;
	}

	private string GetCocktailName(int cocktailID)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(cocktailID);
		if (itemMiscTempletByID != null)
		{
			return itemMiscTempletByID.GetItemName();
		}
		return "";
	}

	private bool IsMomoMissionRedDotActive()
	{
		bool result = false;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			bool num = nKMUserData.m_MissionData.CheckCompletableMission(nKMUserData, m_errandRewardMissionTabId);
			bool flag = false;
			NKMMissionData missionDataByGroupId = nKMUserData.m_MissionData.GetMissionDataByGroupId(m_cocktailRewardMissionGroupId);
			if (missionDataByGroupId != null)
			{
				flag = NKCScenManager.CurrentUserData().m_MissionData.CheckCompletableMission(nKMUserData, missionDataByGroupId.tabId);
			}
			result = num || flag;
		}
		return result;
	}

	private void OnTouchBubble()
	{
		string message = string.Format(NKCStringTable.GetString(m_SelectCocktail), GetCocktailName(NKCEventBarManager.DailyCocktailItemID), GetDailycocktailCount(NKCEventBarManager.DailyCocktailItemID));
		ShowScriptType1(message);
		m_fScriptTimer = m_showScriptTime;
		if (m_scriptCoroutine == null)
		{
			m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
		}
	}

	private void OnMomoPopupClose()
	{
		bool bValue = IsMomoMissionRedDotActive();
		NKCUtil.SetGameobjectActive(m_objErrandRedDot, bValue);
	}

	private void OnSelectCocktailSlot(GameObject slot)
	{
		if (m_EventBarCocktailSlotArray == null || NKCEventBarManager.RemainDeliveryLimitValue <= 0)
		{
			return;
		}
		m_selectedCocktailID = 0;
		int num = m_EventBarCocktailSlotArray.Length;
		for (int i = 0; i < num; i++)
		{
			if (!slot.Equals(m_EventBarCocktailSlotArray[i].gameObject))
			{
				m_EventBarCocktailSlotArray[i].SetSelected(value: false);
			}
			else if (m_EventBarCocktailSlotArray[i].Toggle())
			{
				m_selectedCocktailID = m_EventBarCocktailSlotArray[i].CockTailID;
			}
		}
		string text = "";
		if (m_selectedCocktailID > 0)
		{
			text = NKCStringTable.GetString(m_GiveDesc);
			ShowScriptType2(text);
		}
		else
		{
			text = string.Format(NKCStringTable.GetString(m_SelectCocktail), GetCocktailName(NKCEventBarManager.DailyCocktailItemID), GetDailycocktailCount(NKCEventBarManager.DailyCocktailItemID));
			ShowScriptType1(text);
			m_fScriptTimer = m_showScriptTime;
			if (m_scriptCoroutine == null)
			{
				m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
			}
		}
		m_fScriptTimer = m_showScriptTime;
		if (m_scriptCoroutine == null)
		{
			m_scriptCoroutine = StartCoroutine(IOnShowRequestScript());
		}
	}

	private void OnClickGive()
	{
		NKMEventBarTemplet nKMEventBarTemplet = NKMEventBarTemplet.Find(m_selectedCocktailID);
		if (nKMEventBarTemplet == null)
		{
			return;
		}
		if (m_selectedCocktailID == 0)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(NKCEventBarManager.DailyCocktailItemID);
			string arg = ((itemMiscTempletByID == null) ? "" : itemMiscTempletByID.GetItemName());
			m_fScriptTimer = m_showScriptTime;
			ShowScriptType2(string.Format(NKCStringTable.GetString(m_SelectCocktail), arg, nKMEventBarTemplet.DeliveryValue));
			return;
		}
		if (m_selectedCocktailID != NKCEventBarManager.DailyCocktailItemID)
		{
			m_fScriptTimer = m_showScriptTime;
			ShowScriptType2(NKCStringTable.GetString(m_WrongCocktail));
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		if (nKMUserData.m_InventoryData.GetCountMiscItem(m_selectedCocktailID) < nKMEventBarTemplet.DeliveryValue)
		{
			m_fScriptTimer = m_showScriptTime;
			ShowScriptType2(string.Format(NKCStringTable.GetString(m_NeedMoreCocktail), nKMEventBarTemplet.DeliveryValue));
			return;
		}
		NKCPacketSender.Send_NKMPacket_EVENT_BAR_GET_REWARD_REQ(m_selectedCocktailID);
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
		HideScript();
	}

	private void OnClickStay()
	{
		m_fScriptTimer = m_showScriptTime;
		ShowScriptType1(NKCStringTable.GetString(m_GiveCancel));
	}

	private void OnClickMomoErrand()
	{
		NKMEventTabTemplet nKMEventTabTemplet = NKMEventTabTemplet.Find(m_eventID);
		if (nKMEventTabTemplet != null)
		{
			if (nKMEventTabTemplet.m_EventBannerPrefabName == "EVENT_GREMORY_BAR")
			{
				NKCPopupEventBarMission.ASSET_BUNDLE_NAME = "event_gremory_bar";
				NKCPopupEventBarMission.UI_ASSET_NAME = "POPUP_EVENT_GREMORY_BAR_MOMO";
			}
			else
			{
				NKCPopupEventBarMission.ASSET_BUNDLE_NAME = "ui_single_cafe";
				NKCPopupEventBarMission.UI_ASSET_NAME = "POPUP_UI_SINGLE_CAFE_MISSION";
			}
			NKCPopupEventBarMission.Instance.Open(m_errandRewardMissionTabId, m_cocktailRewardMissionGroupId, OnMomoPopupClose);
		}
	}

	private void OnClickScriptPanel()
	{
		if (m_objScriptType2.activeSelf)
		{
			return;
		}
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

	private void OnDestroy()
	{
		if (cockTailIDList != null)
		{
			cockTailIDList.Clear();
			cockTailIDList = null;
		}
		m_typeWriter = null;
		if (m_scriptCoroutine != null)
		{
			StopCoroutine(m_scriptCoroutine);
			m_scriptCoroutine = null;
		}
	}
}
