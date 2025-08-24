using System.Collections;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupIllustView : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_UNIT_INFO";

	public const string UI_ASSET_NAME = "NKM_UI_POPUP_ILLUST_VIEW";

	public const string UI_ASSET_NAME_PC = "NKM_UI_POPUP_ILLUST_VIEW_PC";

	private static NKCUIPopupIllustView m_Instance;

	public NKCUICharacterView m_characterView;

	public Animator m_animator;

	public Transform m_unitRoot;

	public Mask m_mask;

	[Header("스크롤/확대/축소")]
	public ScrollRect m_scrollRect;

	public Transform m_content;

	[Header("BACKGROUND")]
	public GameObject m_objBG;

	[Header("BUTTONS")]
	public NKCUIComStateButton m_btnClose;

	private const string ANIMATION_INTRO_NAME = "NKM_UI_POPUP_ILLUST_VIEW_INTRO";

	private const string ANIMATION_OUTRO_NAME = "NKM_UI_POPUP_ILLUST_VIEW_OUTRO";

	[Header("CharacterView Position")]
	public Vector3 m_vecPositionShip = Vector3.zero;

	public Vector3 m_vecPositionUnit = Vector3.zero;

	[Header("PC Version - Whell Zoom")]
	public float m_fScaleVal = 0.1f;

	public static NKCUIPopupIllustView Instance
	{
		get
		{
			if (m_Instance == null)
			{
				if (NKCDefineManager.DEFINE_UNITY_STANDALONE())
				{
					m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupIllustView>("AB_UI_NKM_UI_UNIT_INFO", "NKM_UI_POPUP_ILLUST_VIEW_PC", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupIllustView>();
				}
				else
				{
					m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupIllustView>("AB_UI_NKM_UI_UNIT_INFO", "NKM_UI_POPUP_ILLUST_VIEW", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupIllustView>();
				}
				m_Instance.Init();
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

	public override string MenuName => "ILLUST VIEW";

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

	private void Init()
	{
		m_characterView.Init();
		m_characterView.SetMode(NKCUICharacterView.eMode.CharacterView, bAnimate: false);
		if (NKCDefineManager.DEFINE_UNITY_STANDALONE_WIN())
		{
			m_characterView.scrollSensibility = m_fScaleVal;
		}
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(OnClose);
	}

	public void Open(NKMUnitData unit)
	{
		if (unit != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unit.m_UnitID);
			if (unitTempletBase != null)
			{
				SetRootPosition(unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP);
				m_characterView.SetCharacterIllust(unit);
				Open();
			}
		}
	}

	public void Open(NKMOperator operatorData)
	{
		if (operatorData != null && NKMUnitManager.GetUnitTempletBase(operatorData.id) != null)
		{
			SetRootPosition(isShip: false);
			m_characterView.SetCharacterIllust(operatorData);
			Open();
		}
	}

	public void Open(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase != null)
		{
			SetRootPosition(unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP);
			m_characterView.SetCharacterIllust(unitTempletBase);
			Open();
		}
	}

	public void Open(NKMSkinTemplet skinTemplet)
	{
		SetRootPosition(isShip: false);
		m_characterView.SetCharacterIllust(skinTemplet);
		Open();
	}

	private void Open()
	{
		m_scrollRect.normalizedPosition = new Vector2(0.5f, 0.5f);
		m_content.localScale = Vector3.one;
		UIOpened();
		PlayAni("NKM_UI_POPUP_ILLUST_VIEW_INTRO");
		if (m_characterView?.m_srScrollRect != null)
		{
			m_characterView.m_srScrollRect.scrollSensitivity = 0f;
		}
	}

	private void SetRootPosition(bool isShip)
	{
		if (isShip)
		{
			if (!NKCDefineManager.DEFINE_UNITY_STANDALONE())
			{
				m_unitRoot.localRotation = Quaternion.Euler(0f, 0f, -90f);
			}
			m_unitRoot.localPosition = m_vecPositionShip;
		}
		else
		{
			m_unitRoot.localRotation = Quaternion.Euler(0f, 0f, 0f);
			m_unitRoot.localPosition = m_vecPositionUnit;
		}
	}

	public override void CloseInternal()
	{
		m_characterView.CleanUp();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClose()
	{
		PlayAni("NKM_UI_POPUP_ILLUST_VIEW_OUTRO", base.Close);
	}

	public override void OnHotkeyHold(HotkeyEventType hotkey)
	{
		switch (hotkey)
		{
		case HotkeyEventType.Plus:
			m_characterView.OnPinchZoom(Vector2.zero, Time.deltaTime * 0.5f);
			break;
		case HotkeyEventType.Minus:
			m_characterView.OnPinchZoom(Vector2.zero, (0f - Time.deltaTime) * 0.5f);
			break;
		}
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		if (hotkey == HotkeyEventType.ShowHotkey)
		{
			NKCUIComHotkeyDisplay.OpenInstance(base.transform, HotkeyEventType.Plus, HotkeyEventType.Minus);
		}
		return false;
	}

	private void PlayAni(string ani_name, UnityAction onFinish = null)
	{
		if (m_mask != null)
		{
			m_mask.enabled = true;
		}
		m_animator.Play(ani_name);
		StartCoroutine(OnCompleteAni(ani_name, onFinish));
	}

	private IEnumerator OnCompleteAni(string aniName, UnityAction onFinish)
	{
		if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
		{
			yield return null;
		}
		while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		if (m_mask != null)
		{
			m_mask.enabled = false;
		}
		onFinish?.Invoke();
	}
}
