using System;
using NKC.UI.HUD;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOverlayTutorialGuide : NKCUIBase
{
	public enum ClickGuideType
	{
		None,
		DeckDrag,
		ShipSkill,
		Touch
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_tutorial";

	private const string UI_ASSET_NAME = "NKM_UI_TUTORIAL_GUIDE";

	private static NKCUIOverlayTutorialGuide m_Instance;

	public GameObject m_objMessageRoot;

	public Text m_lbMessage;

	public RectTransform m_rtDragDeck;

	public RectTransform m_rtDragDeckHighlightArea;

	public RectTransform m_rtDragShipSkill;

	public RectTransform m_rtDragShipSkillHightlightArea;

	public RectTransform m_rtTouch;

	public NKCUIComRectScreen m_UIScreen;

	private UnityAction dOnComplete;

	private ClickGuideType m_CurrentType;

	private float m_fOpenTime;

	private float m_fTargetOpenTime;

	private NKCUIComButton m_btnClickTarget;

	private NKCUIComStateButton m_sbtnClickTarget;

	public static NKCUIOverlayTutorialGuide Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOverlayTutorialGuide>("ab_ui_nkm_ui_tutorial", "NKM_UI_TUTORIAL_GUIDE", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance).GetInstance<NKCUIOverlayTutorialGuide>();
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

	public override eMenutype eUIType => eMenutype.Overlay;

	public override string MenuName => NKCUtilString.GET_STRING_TUTORIAL_GUIDE;

	public override bool IgnoreBackButtonWhenOpen => true;

	public bool IsShowingInvalidMap { get; set; }

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
		Cleanup();
		base.gameObject.SetActive(value: false);
	}

	public override void OnBackButton()
	{
	}

	private void Cleanup()
	{
		m_sbtnClickTarget?.PointerClick.RemoveListener(OnPressButton);
		m_btnClickTarget?.PointerClick.RemoveListener(OnPressButton);
		m_sbtnClickTarget = null;
		m_btnClickTarget = null;
		if (m_UIScreen != null)
		{
			m_UIScreen.CleanUp();
		}
	}

	public void Open(Renderer targetRenderer, string text, UnityAction onComplete, NKCUIComRectScreen.ScreenExpand expandFlag = NKCUIComRectScreen.ScreenExpand.None)
	{
		dOnComplete = onComplete;
		m_fOpenTime = 0f;
		m_fTargetOpenTime = 0f;
		NKCUtil.SetGameobjectActive(m_rtDragDeck, bValue: false);
		NKCUtil.SetGameobjectActive(m_rtDragShipSkill, bValue: false);
		NKCUtil.SetGameobjectActive(m_rtTouch, bValue: false);
		m_CurrentType = ClickGuideType.None;
		if (string.IsNullOrEmpty(text))
		{
			NKCUtil.SetGameobjectActive(m_objMessageRoot, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objMessageRoot, bValue: true);
			NKCUtil.SetLabelText(m_lbMessage, text);
		}
		m_UIScreen.SetScreen(targetRenderer, bAttach: true, expandFlag);
		IsShowingInvalidMap = false;
		UIOpened();
		Canvas component = NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIOverlay).GetComponent<Canvas>();
		if (component != null)
		{
			component.overrideSorting = !component.overrideSorting;
			component.overrideSorting = false;
		}
	}

	public void Open(RectTransform rtClickableArea, ClickGuideType type, string text, UnityAction onComplete, bool bIsFromMidCanvas = false, NKCUIComRectScreen.ScreenExpand expandFlag = NKCUIComRectScreen.ScreenExpand.None, float fOpenTime = 0f)
	{
		dOnComplete = onComplete;
		m_fOpenTime = 0f;
		m_fTargetOpenTime = fOpenTime;
		NKCUtil.SetGameobjectActive(m_rtDragDeck, type == ClickGuideType.DeckDrag);
		NKCUtil.SetGameobjectActive(m_rtDragShipSkill, type == ClickGuideType.ShipSkill);
		NKCUtil.SetGameobjectActive(m_rtTouch, type == ClickGuideType.Touch);
		RectTransform rectTransform = null;
		RectTransform target = null;
		switch (type)
		{
		case ClickGuideType.DeckDrag:
		{
			NKCGameHud gameHud2 = NKCScenManager.GetScenManager().GetGameClient().GetGameHud();
			gameHud2.dOnUseDeck = (NKCGameHud.OnUseDeck)Delegate.Combine(gameHud2.dOnUseDeck, new NKCGameHud.OnUseDeck(OnHudUseDeck));
			target = m_rtDragDeckHighlightArea;
			rectTransform = m_rtDragDeck;
			break;
		}
		case ClickGuideType.ShipSkill:
		{
			NKCGameHud gameHud = NKCScenManager.GetScenManager().GetGameClient().GetGameHud();
			gameHud.dOnUseSkill = (NKCGameHud.OnUseSkill)Delegate.Combine(gameHud.dOnUseSkill, new NKCGameHud.OnUseSkill(OnHudUseSkill));
			rectTransform = m_rtDragShipSkill;
			target = m_rtDragShipSkillHightlightArea;
			break;
		}
		case ClickGuideType.Touch:
			if (rtClickableArea == null)
			{
				Debug.LogError("Target rect not exist!");
				OnPressButton();
				return;
			}
			m_btnClickTarget = rtClickableArea.GetComponent<NKCUIComButton>();
			if (m_btnClickTarget != null)
			{
				m_btnClickTarget.PointerClick.AddListener(OnPressButton);
			}
			m_sbtnClickTarget = rtClickableArea.GetComponent<NKCUIComStateButton>();
			if (m_sbtnClickTarget != null)
			{
				m_sbtnClickTarget.PointerClick.AddListener(OnPressButton);
			}
			rectTransform = m_rtTouch;
			target = rtClickableArea;
			break;
		case ClickGuideType.None:
			rectTransform = null;
			target = rtClickableArea;
			break;
		}
		m_CurrentType = type;
		if (rectTransform != null && rtClickableArea != null)
		{
			rectTransform.pivot = rtClickableArea.pivot;
			rectTransform.position = rtClickableArea.position;
		}
		if (string.IsNullOrWhiteSpace(text))
		{
			NKCUtil.SetGameobjectActive(m_objMessageRoot, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objMessageRoot, bValue: true);
			NKCUtil.SetLabelText(m_lbMessage, text);
		}
		NKCUtil.SetGameobjectActive(m_UIScreen, bValue: true);
		m_UIScreen.SetScreen(target, bIsFromMidCanvas, bAttach: true);
		if (type == ClickGuideType.None)
		{
			m_UIScreen.SetTouchSteal(null);
		}
		IsShowingInvalidMap = false;
		UIOpened();
		Canvas component = NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIOverlay).GetComponent<Canvas>();
		if (component != null)
		{
			component.overrideSorting = !component.overrideSorting;
			component.overrideSorting = false;
		}
	}

	private void SetGuidePositionByScreen(RectTransform guideRect)
	{
		if (!(guideRect == null))
		{
			guideRect.pivot = m_UIScreen.m_rtCenter.pivot;
			guideRect.position = m_UIScreen.m_rtCenter.position;
		}
	}

	public void SetStealInput(UnityAction<BaseEventData> onInput)
	{
		m_UIScreen.SetTouchSteal(onInput);
	}

	public void SetScreenActive(bool value)
	{
		NKCUtil.SetGameobjectActive(m_UIScreen, value);
	}

	private void Update()
	{
		if (m_fTargetOpenTime > 0f)
		{
			m_fOpenTime += Time.deltaTime;
			if (m_fOpenTime >= m_fTargetOpenTime)
			{
				dOnComplete?.Invoke();
			}
			return;
		}
		switch (m_CurrentType)
		{
		case ClickGuideType.None:
			if (Input.anyKeyDown)
			{
				dOnComplete?.Invoke();
			}
			break;
		case ClickGuideType.Touch:
			SetGuidePositionByScreen(m_rtTouch);
			break;
		case ClickGuideType.DeckDrag:
		case ClickGuideType.ShipSkill:
			break;
		}
	}

	private void OnPressButton()
	{
		Cleanup();
		dOnComplete?.Invoke();
	}

	private void OnHudUseDeck(int deckIndex)
	{
		NKCGameHud gameHud = NKCScenManager.GetScenManager().GetGameClient().GetGameHud();
		gameHud.dOnUseDeck = (NKCGameHud.OnUseDeck)Delegate.Remove(gameHud.dOnUseDeck, new NKCGameHud.OnUseDeck(OnHudUseDeck));
		dOnComplete?.Invoke();
	}

	private void OnHudUseSkill(int deckIndex)
	{
		NKCGameHud gameHud = NKCScenManager.GetScenManager().GetGameClient().GetGameHud();
		gameHud.dOnUseSkill = (NKCGameHud.OnUseSkill)Delegate.Remove(gameHud.dOnUseSkill, new NKCGameHud.OnUseSkill(OnHudUseSkill));
		dOnComplete?.Invoke();
	}

	public void SetBGScreenAlpha(float alpha)
	{
		m_UIScreen.SetAlpha(alpha);
	}
}
