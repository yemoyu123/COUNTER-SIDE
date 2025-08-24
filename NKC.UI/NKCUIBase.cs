using System.Collections.Generic;
using ClientPacket.Office;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public abstract class NKCUIBase : MonoBehaviour
{
	public enum eMenutype
	{
		FullScreen,
		Popup,
		Overlay
	}

	public enum eTransitionEffectType
	{
		None,
		SmallLoading,
		FullScreenLoading,
		FadeInOut
	}

	protected bool m_bOpen;

	protected bool m_bHide;

	private static readonly List<int> DEFAULT_RESOURCE_LIST = new List<int> { 1, 2, 101 };

	public bool IsOpen => m_bOpen;

	public bool IsHidden => m_bHide;

	public abstract eMenutype eUIType { get; }

	public virtual NKCUIUpsideMenu.eMode eUpsideMenuMode
	{
		get
		{
			if (!IsFullScreenUI)
			{
				return NKCUIUpsideMenu.eMode.Invalid;
			}
			return NKCUIUpsideMenu.eMode.Normal;
		}
	}

	public bool IsFullScreenUI => eUIType == eMenutype.FullScreen;

	public bool IsPopupUI => eUIType == eMenutype.Popup;

	public virtual bool WillCloseUnderPopupOnOpen => IsFullScreenUI;

	public virtual eTransitionEffectType eTransitionEffect => eTransitionEffectType.None;

	public abstract string MenuName { get; }

	public virtual List<int> UpsideMenuShowResourceList => DEFAULT_RESOURCE_LIST;

	public virtual NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public virtual bool IgnoreBackButtonWhenOpen => false;

	public virtual bool DisableSubMenu => false;

	public virtual string GuideTempletID => "";

	public virtual bool PassHotkeyToNextUI => false;

	protected static NKCAssetResourceData OpenInstanceAsync<T>(string BundleName, string AssetName) where T : NKCUIBase
	{
		return NKCAssetResourceManager.OpenResource<GameObject>(BundleName, AssetName, bAsync: true);
	}

	public static T OpenInstance<T>(NKMAssetName assetName, NKCUIManager.eUIBaseRect baseRect, NKCUIManager.LoadedUIData.OnCleanupInstance onCleanupInstance) where T : NKCUIBase
	{
		return NKCUIManager.OpenNewInstance<T>(assetName, NKCUIManager.GetUIBaseRect(baseRect), onCleanupInstance).GetInstance<T>();
	}

	public static T OpenInstance<T>(string bundleName, string assetName, NKCUIManager.eUIBaseRect baseRect, NKCUIManager.LoadedUIData.OnCleanupInstance onCleanupInstance) where T : NKCUIBase
	{
		return NKCUIManager.OpenNewInstance<T>(bundleName, assetName, NKCUIManager.GetUIBaseRect(baseRect), onCleanupInstance).GetInstance<T>();
	}

	protected static bool CheckInstanceLoaded<T>(NKCAssetResourceData assetData, Transform parent, out T instance) where T : NKCUIBase
	{
		if (assetData.IsDone())
		{
			GameObject asset = assetData.GetAsset<GameObject>();
			if (asset != null)
			{
				GameObject gameObject = Object.Instantiate(asset, parent);
				instance = gameObject.GetComponent<T>();
				if (instance != null)
				{
					NKCUIManager.OpenUI(instance.gameObject);
				}
				return true;
			}
			instance = null;
			return true;
		}
		instance = null;
		return false;
	}

	public override string ToString()
	{
		return string.Format("{0}({1}){2}", MenuName, base.gameObject.name, (m_bOpen && m_bHide) ? "(Hide)" : "");
	}

	protected void UIOpened(bool bSetAnchoredPosToZero = true)
	{
		if (bSetAnchoredPosToZero)
		{
			RectTransform component = GetComponent<RectTransform>();
			if (component != null)
			{
				component.anchoredPosition = Vector2.zero;
			}
		}
		UIPrepare();
		UIReady();
	}

	protected void UIPrepare()
	{
		NKCUIManager.UIPrepare(this);
	}

	protected void UIReady()
	{
		NKCUIManager.UIReady(this);
	}

	public virtual void Activate()
	{
		base.gameObject.SetActive(value: true);
		m_bOpen = true;
		m_bHide = false;
		SetupScrollRects(base.gameObject);
	}

	protected void SetupScrollRects(GameObject go)
	{
		float scrollSensibility = NKCInputManager.ScrollSensibility;
		ScrollRect[] componentsInChildren = go.GetComponentsInChildren<ScrollRect>(includeInactive: true);
		foreach (ScrollRect scrollRect in componentsInChildren)
		{
			if (scrollRect.horizontal && !scrollRect.vertical)
			{
				scrollRect.scrollSensitivity = 0f - scrollSensibility;
			}
			else
			{
				scrollRect.scrollSensitivity = scrollSensibility;
			}
		}
	}

	protected void UpdateUpsideMenu()
	{
		NKCUIManager.UpdateUpsideMenu();
	}

	public virtual bool IsResetUpsideMenuWhenOpenAgain()
	{
		return false;
	}

	public virtual Vector3 GetBackgroundDimension()
	{
		return new Vector3(1024f, 1024f, 2000f);
	}

	public virtual Sprite GetBackgroundSprite()
	{
		return null;
	}

	public virtual Color GetBackgroundColor()
	{
		return Color.white;
	}

	public virtual void UpdateCamera()
	{
		if (!NKCCamera.IsTrackingCameraPos())
		{
			NKCCamera.TrackingPos(10f, NKMRandom.Range(-50f, 50f), NKMRandom.Range(-50f, 50f), NKMRandom.Range(-1000f, -900f));
		}
	}

	public void OnDragBackground(BaseEventData cBaseEventData)
	{
		PointerEventData pointerEventData = cBaseEventData as PointerEventData;
		float value = NKCCamera.GetPosNowX() - pointerEventData.delta.x * 10f;
		float value2 = NKCCamera.GetPosNowY() - pointerEventData.delta.y * 10f;
		value = Mathf.Clamp(value, -100f, 100f);
		value2 = Mathf.Clamp(value2, -100f, 100f);
		NKCCamera.TrackingPos(1f, value, value2);
	}

	public virtual void Initialize()
	{
		Debug.LogError($"{GetType()} : Initialize() Not Implemented!");
	}

	public virtual void OpenByShortcut(Dictionary<string, string> dicParam)
	{
		Debug.LogError($"{GetType()} : OpenByShortcut() Not Implemented!");
	}

	public void Close()
	{
		if (m_bOpen)
		{
			m_bOpen = false;
			NKCUIManager.UIClosed(this);
		}
	}

	internal void _ForceCloseInternal()
	{
		m_bOpen = false;
		CloseInternal();
	}

	public abstract void CloseInternal();

	public virtual void OnCloseInstance()
	{
	}

	public virtual void Hide()
	{
		m_bHide = true;
		base.gameObject.SetActive(value: false);
	}

	public virtual void UnHide()
	{
		m_bHide = false;
		base.gameObject.SetActive(value: true);
	}

	public virtual void OnBackButton()
	{
		Close();
	}

	public virtual bool OnHomeButton()
	{
		return true;
	}

	public virtual void OnUserLevelChanged(NKMUserData userData)
	{
	}

	public virtual void OnInventoryChange(NKMItemMiscData itemData)
	{
	}

	public virtual void OnInteriorInventoryUpdate(NKMInteriorData interiorData, bool bAdded)
	{
	}

	public virtual void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipItem)
	{
	}

	public virtual void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
	}

	public virtual void OnOperatorUpdate(NKMUserData.eChangeNotifyType eEventType, long uid, NKMOperator operatorData)
	{
	}

	public virtual void OnDeckUpdate(NKMDeckIndex deckIndex, NKMDeckData deckData)
	{
	}

	public virtual void OnCompanyBuffUpdate(NKMUserData userData)
	{
	}

	public virtual void OnScreenResolutionChanged()
	{
		LoopScrollRect[] componentsInChildren = GetComponentsInChildren<LoopScrollRect>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Repopulate();
		}
	}

	public virtual void OnGuildDataChanged()
	{
	}

	public virtual void OnMissionUpdated()
	{
	}

	public virtual bool OnHotkey(HotkeyEventType hotkey)
	{
		return false;
	}

	public virtual void OnHotkeyHold(HotkeyEventType hotkey)
	{
	}

	public virtual void OnHotkeyRelease(HotkeyEventType hotkey)
	{
	}

	protected static void SetGameObjectActive(GameObject targetObj, bool bValue)
	{
		if (targetObj != null && targetObj.activeSelf != bValue)
		{
			targetObj.SetActive(bValue);
		}
	}

	protected static void SetGameObjectActive(Transform targetTransform, bool bValue)
	{
		if (targetTransform != null && targetTransform.gameObject.activeSelf != bValue)
		{
			targetTransform.gameObject.SetActive(bValue);
		}
	}

	protected static void SetGameObjectActive(MonoBehaviour targetMono, bool bValue)
	{
		if (targetMono != null && targetMono.gameObject.activeSelf != bValue)
		{
			targetMono.gameObject.SetActive(bValue);
		}
	}

	protected static void SetLabelText(Text label, string msg)
	{
		if (label != null)
		{
			label.text = msg;
		}
	}

	protected static void SetLabelTextColor(Text label, Color col)
	{
		if (label != null)
		{
			label.color = col;
		}
	}

	protected static void SetLabelText(Text label, string msg, params object[] args)
	{
		if (label != null)
		{
			label.text = string.Format(msg, args);
		}
	}

	protected static void SetImageSprite(Image image, Sprite sp, bool bDisableIfSpriteNull = false)
	{
		if (image != null)
		{
			image.sprite = sp;
		}
		if (bDisableIfSpriteNull)
		{
			SetGameObjectActive(image, sp != null);
		}
	}

	protected static void SetImageColor(Image image, Color color)
	{
		if (image != null)
		{
			image.color = color;
		}
	}
}
