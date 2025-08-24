using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NKC.UI.Module;

public class NKCUIModuleResult : NKCUIBase, IPointerDownHandler, IEventSystemHandler
{
	public SkeletonGraphic m_SkeletonGraphic;

	public RectTransform m_rtScaleTargetRect;

	[Header("Sound(\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd)")]
	public string m_strBGM;

	public string m_strSound;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	public bool m_bFixResolution = true;

	private UnityAction dClose;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "NKCUIModuleResult \ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd";

	public static void CheckInstanceAndClose()
	{
		if (m_loadedUIData != null)
		{
			m_loadedUIData.CloseInstance();
			m_loadedUIData = null;
		}
	}

	public static NKCUIModuleResult MakeInstance(string bundleName, string assetName)
	{
		if (m_loadedUIData == null)
		{
			m_loadedUIData = NKCUIManager.OpenNewInstance<NKCUIModuleResult>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIOverlay, null);
		}
		if (m_loadedUIData == null)
		{
			return null;
		}
		NKCUIModuleResult instance = m_loadedUIData.GetInstance<NKCUIModuleResult>();
		if (null == instance)
		{
			return null;
		}
		return instance;
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void OnBackButton()
	{
		base.OnBackButton();
		if (!string.IsNullOrEmpty(m_strBGM))
		{
			NKCUIModuleHome.PlayBGMMusic();
		}
		dClose?.Invoke();
	}

	public void Open(UnityAction close = null)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		dClose = close;
		m_SkeletonGraphic.AnimationState.ClearTrack(0);
		m_SkeletonGraphic.AnimationState.SetAnimation(0, "BASE", loop: false);
		m_SkeletonGraphic.AnimationState.Complete += SetNextAni;
		if (!string.IsNullOrEmpty(m_strBGM))
		{
			NKCSoundManager.PlayMusic(m_strBGM, bLoop: true);
		}
		if (!string.IsNullOrEmpty(m_strSound))
		{
			NKCSoundManager.PlaySound(m_strSound, 1f, 0f, 0f);
		}
		if (m_bFixResolution)
		{
			NKCCamera.RescaleRectToCameraFrustrum(m_rtScaleTargetRect, NKCCamera.GetSubUICamera(), Vector2.zero, NKCCamera.GetSubUICamera().transform.position.z);
		}
		UIOpened();
	}

	private void SetNextAni(TrackEntry trackEntry)
	{
		OnBackButton();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		OnBackButton();
	}

	public override void OnHotkeyHold(HotkeyEventType hotkey)
	{
		if (hotkey == HotkeyEventType.Skip)
		{
			if (!NKCUIManager.IsTopmostUI(this))
			{
				return;
			}
			if (null != m_SkeletonGraphic)
			{
				m_SkeletonGraphic.AnimationState.TimeScale = 2f;
			}
		}
		if (hotkey == HotkeyEventType.Confirm)
		{
			OnBackButton();
		}
	}

	public override void OnHotkeyRelease(HotkeyEventType hotkey)
	{
		if (hotkey == HotkeyEventType.Skip && NKCUIManager.IsTopmostUI(this) && null != m_SkeletonGraphic)
		{
			m_SkeletonGraphic.AnimationState.TimeScale = 1f;
		}
	}
}
