using System;
using System.Collections.Generic;
using ClientPacket.Office;
using Cs.Logging;
using DG.Tweening;
using NKC.Loading;
using NKC.UI.Gauntlet;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIManager
{
	public interface IInventoryChangeObserver
	{
		int Handle { get; set; }

		void OnInventoryChange(NKMItemMiscData itemData);

		void OnInteriorInventoryUpdate(NKMInteriorData interiorData, bool bAdded);

		void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipData);
	}

	public enum eUIBaseRect
	{
		UIFrontLow,
		UIFrontCommonLow,
		UIFrontCommon,
		UIFrontPopup,
		UIMidCanvas,
		UIFrontCanvas,
		UIOverlay
	}

	private enum eUITransitionProcess : short
	{
		Idle,
		Preparing,
		FinishedAndWaiting
	}

	public enum eUIUnloadFlag
	{
		DEFAULT,
		ON_PLAY_GAME,
		ONLY_MEMORY_SHORTAGE,
		NEVER_UNLOAD
	}

	public class LoadedUIData
	{
		public delegate void OnCleanupInstance();

		public readonly int key;

		public readonly string bundleName;

		public readonly string assetName;

		public eUIUnloadFlag eUnloadFlag;

		public OnCleanupInstance dOnCleanupInstance;

		private NKCAssetResourceData assetData;

		private NKCUIBase instance;

		private Transform parent;

		public bool IsOpenReserved { get; set; }

		public bool IsLoadComplete
		{
			get
			{
				if (assetData == null)
				{
					return false;
				}
				if (instance != null)
				{
					return true;
				}
				return assetData.IsDone();
			}
		}

		public bool HasInstance => instance != null;

		public bool HasAssetResourceData => assetData != null;

		public bool IsUIOpen
		{
			get
			{
				if (instance != null)
				{
					return instance.IsOpen;
				}
				return false;
			}
		}

		public LoadedUIData(int _key, NKCAssetResourceData _assetData, Transform _parent, OnCleanupInstance onCleanupInstance)
		{
			key = _key;
			assetData = _assetData;
			parent = _parent;
			instance = null;
			eUnloadFlag = eUIUnloadFlag.DEFAULT;
			dOnCleanupInstance = onCleanupInstance;
			if (assetData == null)
			{
				Debug.LogError("assetData is Null!");
				bundleName = "";
				assetName = "";
			}
			else
			{
				bundleName = assetData.m_NKMAssetName.m_BundleName;
				assetName = assetData.m_NKMAssetName.m_AssetName;
			}
		}

		public NKCUIBase GetInstance()
		{
			if (instance != null)
			{
				return instance;
			}
			if (assetData == null)
			{
				return null;
			}
			if (!assetData.IsDone())
			{
				return null;
			}
			GameObject asset = assetData.GetAsset<GameObject>();
			if (asset != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(asset, parent);
				instance = gameObject.GetComponent<NKCUIBase>();
				if (instance != null)
				{
					if (instance.GetComponent<Canvas>() == null)
					{
						instance.gameObject.AddComponent<Canvas>().additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
						GraphicRaycaster graphicRaycaster = instance.gameObject.AddComponent<GraphicRaycaster>();
						graphicRaycaster.ignoreReversedGraphics = true;
						graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
					}
					OpenUI(instance.gameObject);
					eUnloadFlag = instance.UnloadFlag;
				}
			}
			else
			{
				instance = null;
			}
			return instance;
		}

		public T GetInstance<T>() where T : NKCUIBase
		{
			if (instance != null)
			{
				return instance as T;
			}
			if (assetData == null)
			{
				return null;
			}
			if (!assetData.IsDone())
			{
				return null;
			}
			GameObject asset = assetData.GetAsset<GameObject>();
			if (asset != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(asset, parent);
				instance = gameObject.GetComponent<T>();
				if (instance != null)
				{
					if (instance.GetComponent<Canvas>() == null)
					{
						instance.gameObject.AddComponent<Canvas>().additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
						GraphicRaycaster graphicRaycaster = instance.gameObject.AddComponent<GraphicRaycaster>();
						graphicRaycaster.ignoreReversedGraphics = true;
						graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
					}
					OpenUI(instance.gameObject);
					eUnloadFlag = instance.UnloadFlag;
				}
			}
			else
			{
				instance = null;
			}
			return instance as T;
		}

		public void CloseInstance()
		{
			NKCAssetResourceManager.CloseResource(assetData);
			assetData = null;
			if (instance != null)
			{
				if (instance.IsOpen)
				{
					instance.Close();
				}
				instance.OnCloseInstance();
				UnityEngine.Object.Destroy(instance.gameObject);
			}
			dOnCleanupInstance?.Invoke();
			dOnCleanupInstance = null;
			instance = null;
			parent = null;
			InstanceClosed(this);
		}

		public bool CheckLoadAndGetInstance<T>(out T ins) where T : NKCUIBase
		{
			if (IsLoadComplete)
			{
				ins = GetInstance<T>();
			}
			else
			{
				ins = null;
			}
			return ins != null;
		}

		public override string ToString()
		{
			if (IsLoadComplete)
			{
				return $"{key} : {bundleName}.{assetName} ({eUnloadFlag})";
			}
			return $"{key} : {bundleName}.{assetName} Loading..";
		}
	}

	public delegate void Action<T>(T ui);

	public static int m_sHandle = 0;

	public static Dictionary<int, IInventoryChangeObserver> m_dicInventoryObserver = new Dictionary<int, IInventoryChangeObserver>();

	private static GameObject m_NKM_SCEN_UI = null;

	public static Transform m_TR_NKM_WAIT_INSTANT;

	public static GameObject m_NUF_GAME_TOUCH_OBJECT;

	private static GameObject m_NKM_SCEN_UI_BACK_Canvas = null;

	private static CanvasScaler m_NKM_SCEN_UI_BACK_Canvas_CanvasScaler = null;

	private static GameObject m_NKM_SCEN_UI_MID_Canvas = null;

	private static CanvasScaler m_NKM_SCEN_UI_MID_Canvas_CanvasScaler = null;

	private static GameObject m_NKM_SCEN_UI_FRONT_LOW_Canvas = null;

	private static CanvasScaler m_NKM_SCEN_UI_FRONT_LOW_Canvas_CanvasScaler = null;

	private static GameObject m_NKM_SCEN_UI_FRONT_Canvas = null;

	private static CanvasScaler m_NKM_SCEN_UI_FRONT_Canvas_CanvasScaler = null;

	private static GameObject m_NKM_SCEN_UI_FRONT_LOW = null;

	private static Image m_NKM_SCEN_UI_FRONT_LOW_Image = null;

	private static GameObject m_NKM_SCEN_UI_FRONT = null;

	private static Image m_NKM_SCEN_UI_FRONT_Image = null;

	private static RectTransform m_rectBackground;

	private static Image m_imgBackground;

	private static RectTransform m_NKM_SCEN_UI_FRONT_RectTransform = null;

	private static RectTransform m_NKM_SCEN_UI_FRONT_LOW_RectTransform = null;

	private static NKCUILooseShaker m_NKCUILooseShaker;

	private static RectTransform m_NUF_DRAG = null;

	private static RectTransform m_rtCommonLowRoot = null;

	private static RectTransform m_rtCommonRoot = null;

	private static RectTransform m_rtFrontLowRoot = null;

	private static NKCUIBase s_currentFullScreenUIBase = null;

	private static bool s_bUseCameraFunctions = false;

	private static NKCUIPowerSaveMode m_NKCUIPowerSaveMode = null;

	private static int m_ScreenWidth;

	private static int m_ScreenHeight;

	private static ScreenOrientation m_CurrentDeviceOrientation;

	private static RectTransform m_rtPopupRoot;

	private static RectTransform m_rtOverlayRoot;

	private static GameObject m_NUF_BLOCK_SCREEN_INPUT;

	private static Stack<NKCUIBase> m_stkUI = new Stack<NKCUIBase>();

	private static HashSet<NKCUIBase> m_setOverlayUI = new HashSet<NKCUIBase>();

	private static NKCUIBase m_PreparingUI;

	private static eUITransitionProcess m_eUITransitionState = eUITransitionProcess.Idle;

	private static int UIKeySeed = 0;

	private static Dictionary<int, LoadedUIData> s_dicLoadedUI = new Dictionary<int, LoadedUIData>();

	public static CanvasGroup FrontCanvasGroup { get; private set; }

	public static RectTransform UIFrontCanvasSafeRectTransform => m_NKM_SCEN_UI_FRONT_RectTransform;

	public static RectTransform UIFrontLowCanvasSafeRectTransform => m_NKM_SCEN_UI_FRONT_LOW_RectTransform;

	public static NKCUIUpsideMenu NKCUIUpsideMenu { get; private set; }

	public static NKCPopupMessage NKCPopupMessage { get; private set; }

	public static NKCUIOverlayCaption NKCUIOverlayCaption { get; private set; }

	public static NKCUIGauntletResult NKCUIGauntletResult { get; set; }

	public static NKCUILoadingScreen LoadingUI { get; private set; }

	public static RectTransform rectMidCanvas { get; private set; }

	public static RectTransform rectFrontCanvas { get; private set; }

	public static Canvas FrontCanvas { get; private set; }

	public static void RegisterInventoryObserver(IInventoryChangeObserver observer)
	{
		observer.Handle = m_sHandle;
		m_dicInventoryObserver.Add(m_sHandle, observer);
		m_sHandle++;
	}

	public static void UnregisterInventoryObserver(IInventoryChangeObserver observer)
	{
		m_dicInventoryObserver.Remove(observer.Handle);
		if (m_dicInventoryObserver.Count == 0)
		{
			m_sHandle = 0;
		}
	}

	public static void SetUseFrontLowCanvas(bool bUse)
	{
		NKCUtil.SetGameobjectActive(m_NKM_SCEN_UI_FRONT_LOW_Canvas, bUse);
		if (NKCCamera.GetSubUILowCamera() != null)
		{
			NKCCamera.GetSubUILowCamera().enabled = bUse;
		}
	}

	public static CanvasScaler GetUIFrontCanvasScaler()
	{
		return m_NKM_SCEN_UI_FRONT_Canvas_CanvasScaler;
	}

	public static RectTransform Get_NUF_DRAG()
	{
		return m_NUF_DRAG;
	}

	public static NKCUIPowerSaveMode GetNKCUIPowerSaveMode()
	{
		return m_NKCUIPowerSaveMode;
	}

	public static RectTransform GetUIBaseRect(eUIBaseRect type)
	{
		return type switch
		{
			eUIBaseRect.UIFrontLow => m_rtFrontLowRoot, 
			eUIBaseRect.UIFrontCommonLow => m_rtCommonLowRoot, 
			eUIBaseRect.UIFrontCommon => m_rtCommonRoot, 
			eUIBaseRect.UIFrontPopup => m_rtPopupRoot, 
			eUIBaseRect.UIMidCanvas => rectMidCanvas, 
			eUIBaseRect.UIFrontCanvas => rectFrontCanvas, 
			eUIBaseRect.UIOverlay => m_rtOverlayRoot, 
			_ => null, 
		};
	}

	public static void SetScreenInputBlock(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NUF_BLOCK_SCREEN_INPUT, bSet);
	}

	public static bool CheckScreenInputBlock()
	{
		return m_NUF_BLOCK_SCREEN_INPUT.activeSelf;
	}

	private NKCUIManager()
	{
	}

	public static void Init()
	{
		if (NKCDefineManager.DEFINE_USE_CHEAT())
		{
			DOTween.Init(true, true, LogBehaviour.Verbose);
		}
		else
		{
			DOTween.Init(true, true, LogBehaviour.ErrorsOnly);
		}
		DOTween.useSafeMode = true;
		m_NKM_SCEN_UI = GameObject.Find("NKM_SCEN_UI");
		m_TR_NKM_WAIT_INSTANT = m_NKM_SCEN_UI.transform.Find("NKM_WAIT_INSTANT");
		m_NUF_GAME_TOUCH_OBJECT = m_NKM_SCEN_UI.transform.Find("NKM_SCEN_UI_FRONT_Canvas/NKM_SCEN_UI_FRONT/NUF_GAME_TOUCH_OBJECT").gameObject;
		m_NKM_SCEN_UI_BACK_Canvas = GameObject.Find("NKM_SCEN_UI_BACK_Canvas");
		m_NKM_SCEN_UI_BACK_Canvas_CanvasScaler = m_NKM_SCEN_UI_BACK_Canvas.GetComponent<CanvasScaler>();
		m_NKM_SCEN_UI_BACK_Canvas_CanvasScaler.enabled = true;
		m_NKM_SCEN_UI_MID_Canvas = GameObject.Find("NKM_SCEN_UI_MID_Canvas");
		m_NKM_SCEN_UI_MID_Canvas_CanvasScaler = m_NKM_SCEN_UI_MID_Canvas.GetComponent<CanvasScaler>();
		rectMidCanvas = m_NKM_SCEN_UI_MID_Canvas.GetComponent<RectTransform>();
		m_NKM_SCEN_UI_MID_Canvas_CanvasScaler.enabled = true;
		m_NKM_SCEN_UI_FRONT_LOW_Canvas = GameObject.Find("NKM_SCEN_UI_FRONT_LOW_Canvas");
		m_NKM_SCEN_UI_FRONT_LOW_Canvas_CanvasScaler = m_NKM_SCEN_UI_FRONT_LOW_Canvas.GetComponent<CanvasScaler>();
		m_NKM_SCEN_UI_FRONT_LOW_Canvas_CanvasScaler.enabled = true;
		m_NKM_SCEN_UI_FRONT_LOW = m_NKM_SCEN_UI_FRONT_LOW_Canvas.transform.Find("NKM_SCEN_UI_FRONT_LOW").gameObject;
		m_rtFrontLowRoot = m_NKM_SCEN_UI_FRONT_LOW.GetComponent<RectTransform>();
		m_NKM_SCEN_UI_FRONT_LOW_Image = m_NKM_SCEN_UI_FRONT_LOW.GetComponent<Image>();
		m_NKM_SCEN_UI_FRONT_LOW_Image.enabled = false;
		m_NKM_SCEN_UI_FRONT_LOW_RectTransform = m_NKM_SCEN_UI_FRONT_LOW.GetComponent<RectTransform>();
		m_NKCUILooseShaker = m_NKM_SCEN_UI_FRONT_LOW.AddComponent<NKCUILooseShaker>();
		m_NKM_SCEN_UI_FRONT_Canvas = GameObject.Find("NKM_SCEN_UI_FRONT_Canvas");
		rectFrontCanvas = m_NKM_SCEN_UI_FRONT_Canvas.GetComponent<RectTransform>();
		FrontCanvas = m_NKM_SCEN_UI_FRONT_Canvas.GetComponent<Canvas>();
		m_NKM_SCEN_UI_FRONT_Canvas_CanvasScaler = m_NKM_SCEN_UI_FRONT_Canvas.GetComponent<CanvasScaler>();
		m_NKM_SCEN_UI_FRONT_Canvas_CanvasScaler.enabled = true;
		FrontCanvasGroup = m_NKM_SCEN_UI_FRONT_Canvas.GetComponent<CanvasGroup>();
		FrontCanvasGroup.alpha = 1f;
		m_NKM_SCEN_UI_FRONT = m_NKM_SCEN_UI_FRONT_Canvas.transform.Find("NKM_SCEN_UI_FRONT").gameObject;
		m_NKM_SCEN_UI_FRONT_Image = m_NKM_SCEN_UI_FRONT.GetComponent<Image>();
		m_NKM_SCEN_UI_FRONT_Image.enabled = false;
		m_NKM_SCEN_UI_FRONT_RectTransform = m_NKM_SCEN_UI_FRONT.GetComponent<RectTransform>();
		m_rectBackground = m_NKM_SCEN_UI_MID_Canvas.transform.Find("NUM_Background").GetComponent<RectTransform>();
		if (m_rectBackground != null)
		{
			m_imgBackground = m_rectBackground.GetComponent<Image>();
			EventTrigger component = m_rectBackground.GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.Drag;
			entry.callback.AddListener(delegate(BaseEventData eventData)
			{
				OnDragBackground(eventData);
			});
			component.triggers.Add(entry);
		}
		else
		{
			Debug.LogError("Background Object not found!");
		}
		GameObject gameObject = GameObject.Find("NUF_POPUP_Panel");
		if (gameObject != null)
		{
			m_rtPopupRoot = gameObject.GetComponent<RectTransform>();
		}
		else
		{
			Debug.LogError("UIManager : Popup Root not found!");
		}
		GameObject gameObject2 = GameObject.Find("NUF_OVERLAY_Panel");
		if (gameObject2 != null)
		{
			m_rtOverlayRoot = gameObject2.GetComponent<RectTransform>();
		}
		else
		{
			Debug.LogError("UIManager : Overlay Root not found!");
		}
		SetAspect();
		m_NUF_DRAG = GameObject.Find("NUF_DRAG").GetComponent<RectTransform>();
		m_rtCommonRoot = OpenUI("NUF_COMMON_Panel").GetComponent<RectTransform>();
		m_rtCommonLowRoot = OpenUI("NUF_COMMON_LOW_Panel").GetComponent<RectTransform>();
		NKCUIUpsideMenu = OpenUI<NKCUIUpsideMenu>("NKM_UI_UPSIDE_MENU");
		NKCUIUpsideMenu.InitUI();
		LoadingUI = OpenUI<NKCUILoadingScreen>("NUF_LOADING_Panel");
		LoadingUI.Init();
		NKCUtil.SetGameobjectActive(LoadingUI, bValue: false);
		NKCPopupOKCancel.InitUI();
		NKCPopupMessage = OpenUI<NKCPopupMessage>("NKM_UI_POPUP_MESSAGE");
		NKCPopupMessage.gameObject.SetActive(value: false);
		NKCUIOverlayCaption = OpenUI<NKCUIOverlayCaption>("NKM_UI_OVERLAY_CAPTION");
		NKCUIOverlayCaption.gameObject.SetActive(value: false);
		m_NKCUIPowerSaveMode = OpenUI<NKCUIPowerSaveMode>("NKM_UI_SLEEP_MODE");
		NKCUtil.SetGameobjectActive(m_NKCUIPowerSaveMode, bValue: false);
		NKCUIFadeInOut.InitUI();
		NKCCutScenManager.Init();
		NKCDescMgr.Init();
		m_NUF_BLOCK_SCREEN_INPUT = OpenUI("NUF_BLOCK_SCREEN_INPUT");
		NKCUtil.SetGameobjectActive(m_NUF_BLOCK_SCREEN_INPUT, bValue: false);
		SetUseFrontLowCanvas(bUse: false);
		m_ScreenWidth = Screen.width;
		m_ScreenHeight = Screen.height;
		m_CurrentDeviceOrientation = Screen.orientation;
		Log.Info($"Screen:{Screen.currentResolution.width}, {Screen.currentResolution.height}, {Screen.width}, {Screen.height}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIManager.cs", 335);
		Log.Info($"Screen.safeArea:{Screen.safeArea.x}, {Screen.safeArea.y}, {Screen.safeArea.width}, {Screen.safeArea.height}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIManager.cs", 336);
	}

	public static void SetAspect()
	{
		if (NKCCamera.GetCamera() == null)
		{
			return;
		}
		NKCCamera.GetCamera().ResetAspect();
		if (NKCCamera.GetCamera().aspect >= 1.777f)
		{
			if (m_NKM_SCEN_UI_BACK_Canvas_CanvasScaler != null)
			{
				m_NKM_SCEN_UI_BACK_Canvas_CanvasScaler.matchWidthOrHeight = 1f;
			}
			if (m_NKM_SCEN_UI_MID_Canvas_CanvasScaler != null)
			{
				m_NKM_SCEN_UI_MID_Canvas_CanvasScaler.matchWidthOrHeight = 1f;
			}
			if (m_NKM_SCEN_UI_FRONT_LOW_Canvas_CanvasScaler != null)
			{
				m_NKM_SCEN_UI_FRONT_LOW_Canvas_CanvasScaler.matchWidthOrHeight = 1f;
			}
			if (m_NKM_SCEN_UI_FRONT_Canvas_CanvasScaler != null)
			{
				m_NKM_SCEN_UI_FRONT_Canvas_CanvasScaler.matchWidthOrHeight = 1f;
			}
		}
		else
		{
			if (m_NKM_SCEN_UI_BACK_Canvas_CanvasScaler != null)
			{
				m_NKM_SCEN_UI_BACK_Canvas_CanvasScaler.matchWidthOrHeight = 0f;
			}
			if (m_NKM_SCEN_UI_MID_Canvas_CanvasScaler != null)
			{
				m_NKM_SCEN_UI_MID_Canvas_CanvasScaler.matchWidthOrHeight = 0f;
			}
			if (m_NKM_SCEN_UI_FRONT_LOW_Canvas_CanvasScaler != null)
			{
				m_NKM_SCEN_UI_FRONT_LOW_Canvas_CanvasScaler.matchWidthOrHeight = 0f;
			}
			if (m_NKM_SCEN_UI_FRONT_Canvas_CanvasScaler != null)
			{
				m_NKM_SCEN_UI_FRONT_Canvas_CanvasScaler.matchWidthOrHeight = 0f;
			}
		}
	}

	public static GameObject OpenUI(string uiName)
	{
		GameObject gameObject = GameObject.Find(uiName);
		OpenUI(gameObject);
		return gameObject;
	}

	public static T OpenUI<T>(string uiName) where T : MonoBehaviour
	{
		GameObject gameObject = GameObject.Find(uiName);
		if (gameObject != null)
		{
			T component = gameObject.GetComponent<T>();
			OpenUI(gameObject);
			return component;
		}
		return null;
	}

	public static GameObject OpenUI(GameObject ui)
	{
		if (ui != null)
		{
			Transform component = ui.GetComponent<Transform>();
			if (component != null)
			{
				Vector3 position = component.position;
				position.Set(0f, 0f, 0f);
				component.position = position;
			}
			RectTransform component2 = ui.GetComponent<RectTransform>();
			if (component2 != null)
			{
				component2.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				component2.anchoredPosition = Vector2.zero;
			}
			NKCUIComSafeArea component3 = ui.GetComponent<NKCUIComSafeArea>();
			if (component3 != null)
			{
				component3.SetSafeAreaUI();
			}
		}
		return ui;
	}

	public static void Update(float deltaTime)
	{
		NKCUIFadeInOut.Update(deltaTime);
		NKMPopUpBox.Update();
		if (m_eUITransitionState == eUITransitionProcess.FinishedAndWaiting && NKCUIFadeInOut.IsFinshed())
		{
			OnTransitionComplete();
		}
		if (s_bUseCameraFunctions && s_currentFullScreenUIBase != null)
		{
			s_currentFullScreenUIBase.UpdateCamera();
		}
		if (m_ScreenWidth != Screen.width || m_ScreenHeight != Screen.height || m_CurrentDeviceOrientation != Screen.orientation)
		{
			m_ScreenWidth = Screen.width;
			m_ScreenHeight = Screen.height;
			m_CurrentDeviceOrientation = Screen.orientation;
			OnScreenResolutionChanged();
		}
	}

	private static void SetBackground(NKCUIBase currentFullscreenUI)
	{
		Sprite backgroundSprite = currentFullscreenUI.GetBackgroundSprite();
		if (backgroundSprite != null)
		{
			s_bUseCameraFunctions = true;
			Vector3 backgroundDimension = currentFullscreenUI.GetBackgroundDimension();
			m_rectBackground.SetWidth(backgroundDimension.x);
			m_rectBackground.SetHeight(backgroundDimension.y);
			m_rectBackground.localPosition = new Vector3(0f, 0f, backgroundDimension.z);
			NKCUtil.SetImageSprite(m_imgBackground, backgroundSprite, bDisableIfSpriteNull: true);
			NKCUtil.SetImageColor(m_imgBackground, currentFullscreenUI.GetBackgroundColor());
		}
		else
		{
			s_bUseCameraFunctions = false;
			NKCUtil.SetImageSprite(m_imgBackground, null, bDisableIfSpriteNull: true);
		}
	}

	private static void OnDragBackground(BaseEventData cBaseEventData)
	{
		if (s_currentFullScreenUIBase != null)
		{
			s_currentFullScreenUIBase.OnDragBackground(cBaseEventData);
		}
	}

	public static void StartLooseShake()
	{
		if (m_NKCUILooseShaker != null)
		{
			m_NKCUILooseShaker.StartShake();
		}
	}

	public static void StopLooseShake()
	{
		if (m_NKCUILooseShaker != null)
		{
			m_NKCUILooseShaker.StopShake();
		}
	}

	public static void UIPrepare(NKCUIBase openedUI)
	{
		m_PreparingUI = openedUI;
		m_eUITransitionState = eUITransitionProcess.Preparing;
		switch (openedUI.eTransitionEffect)
		{
		case NKCUIBase.eTransitionEffectType.FadeInOut:
			NKCUIFadeInOut.FadeOut(0.1f, null, bWhite: false, 7f);
			break;
		case NKCUIBase.eTransitionEffectType.FullScreenLoading:
			if (NKCScenManager.GetScenManager() != null)
			{
				NKCScenManager.GetScenManager().SetActiveLoadingUI(NKCLoadingScreenManager.eGameContentsType.DEFAULT);
			}
			break;
		case NKCUIBase.eTransitionEffectType.SmallLoading:
			NKMPopUpBox.OpenSmallWaitBox();
			break;
		}
	}

	public static void UIReady(NKCUIBase openedUI)
	{
		m_eUITransitionState = eUITransitionProcess.FinishedAndWaiting;
		NKCUIBase.eTransitionEffectType eTransitionEffect = openedUI.eTransitionEffect;
		if (eTransitionEffect == NKCUIBase.eTransitionEffectType.None || eTransitionEffect != NKCUIBase.eTransitionEffectType.FadeInOut)
		{
			OnTransitionComplete();
		}
		else if (NKCUIFadeInOut.IsFinshed())
		{
			OnTransitionComplete();
		}
	}

	private static void OnTransitionComplete()
	{
		switch (m_PreparingUI.eTransitionEffect)
		{
		case NKCUIBase.eTransitionEffectType.FadeInOut:
			NKCUIFadeInOut.FadeIn(0.1f);
			break;
		case NKCUIBase.eTransitionEffectType.FullScreenLoading:
			if (NKCScenManager.GetScenManager() != null)
			{
				NKCScenManager.GetScenManager().CloseLoadingUI();
			}
			break;
		case NKCUIBase.eTransitionEffectType.SmallLoading:
			NKMPopUpBox.CloseWaitBox();
			break;
		}
		m_eUITransitionState = eUITransitionProcess.Idle;
		UIOpened(m_PreparingUI);
		m_PreparingUI = null;
	}

	public static void UIOpened(NKCUIBase openedUI)
	{
		if (m_eUITransitionState == eUITransitionProcess.Preparing)
		{
			Debug.LogWarning("Trying UIOpen while another is processing. ignoring open");
			return;
		}
		if (m_stkUI.Contains(openedUI))
		{
			if (openedUI.IsResetUpsideMenuWhenOpenAgain())
			{
				SetUpsideMenuState(openedUI);
			}
			Debug.LogError("[NKCUIManager]Undefined behaivor : 이미 열려서 UI 스택에 등록되어있는 UI를 다시 열려고 시도");
			return;
		}
		if (openedUI.IsFullScreenUI)
		{
			s_currentFullScreenUIBase = openedUI;
			SetBackground(openedUI);
			if (openedUI.WillCloseUnderPopupOnOpen)
			{
				while (m_stkUI.Count > 0)
				{
					if (m_stkUI.Peek().IsFullScreenUI)
					{
						m_stkUI.Peek().Hide();
						break;
					}
					m_stkUI.Pop()._ForceCloseInternal();
				}
			}
			else
			{
				foreach (NKCUIBase item in m_stkUI)
				{
					item.Hide();
					if (item.IsFullScreenUI)
					{
						break;
					}
				}
			}
		}
		switch (openedUI.eUIType)
		{
		case NKCUIBase.eMenutype.FullScreen:
			if (openedUI.transform.parent == GetUIBaseRect(eUIBaseRect.UIFrontCommon))
			{
				openedUI.transform.SetAsLastSibling();
			}
			break;
		case NKCUIBase.eMenutype.Popup:
			if (m_rtPopupRoot != null)
			{
				openedUI.transform.SetParent(m_rtPopupRoot, worldPositionStays: false);
				openedUI.transform.SetAsLastSibling();
			}
			break;
		case NKCUIBase.eMenutype.Overlay:
			if (m_rtOverlayRoot != null)
			{
				openedUI.transform.SetParent(m_rtOverlayRoot, worldPositionStays: false);
				openedUI.transform.SetAsLastSibling();
			}
			m_setOverlayUI.Add(openedUI);
			openedUI.Activate();
			return;
		}
		SetUpsideMenuState(openedUI, bOpen: true);
		m_stkUI.Push(openedUI);
		openedUI.Activate();
	}

	public static void RegisterUICallback(NKMUserData userData)
	{
		NKCUIUpsideMenu.RegisterUserdataCallback(userData);
		if (userData != null)
		{
			userData.m_InventoryData.dOnMiscInventoryUpdate += OnInventoryChange;
			userData.m_InventoryData.dOnEquipUpdate += OnEquipChange;
			NKMArmyData armyData = userData.m_ArmyData;
			armyData.dOnUnitUpdate = (NKMArmyData.OnUnitUpdate)Delegate.Combine(armyData.dOnUnitUpdate, new NKMArmyData.OnUnitUpdate(OnUnitUpdate));
			NKMArmyData armyData2 = userData.m_ArmyData;
			armyData2.dOnOperatorUpdate = (NKMArmyData.OnOperatorUpdate)Delegate.Combine(armyData2.dOnOperatorUpdate, new NKMArmyData.OnOperatorUpdate(OnOperatorUpdate));
			NKMArmyData armyData3 = userData.m_ArmyData;
			armyData3.dOnDeckUpdate = (NKMArmyData.OnDeckUpdate)Delegate.Combine(armyData3.dOnDeckUpdate, new NKMArmyData.OnDeckUpdate(OnDeckUpdate));
			userData.dOnUserLevelUpdate = (NKMUserData.OnUserLevelUpdate)Delegate.Combine(userData.dOnUserLevelUpdate, new NKMUserData.OnUserLevelUpdate(OnUserLevelChanged));
			userData.dOnCompanyBuffUpdate = (NKMUserData.OnCompanyBuffUpdate)Delegate.Combine(userData.dOnCompanyBuffUpdate, new NKMUserData.OnCompanyBuffUpdate(OnCompanyBuffUpdate));
			userData.OfficeData.dOnInteriorInventoryUpdate += OnInteriorInventoryUpdate;
		}
	}

	private static void OnUserLevelChanged(NKMUserData userData)
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnUserLevelChanged(userData);
		}
	}

	private static void OnInventoryChange(NKMItemMiscData itemData)
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnInventoryChange(itemData);
		}
		foreach (IInventoryChangeObserver value in m_dicInventoryObserver.Values)
		{
			value?.OnInventoryChange(itemData);
		}
	}

	private static void OnInteriorInventoryUpdate(NKMInteriorData interiorData, bool bAdded)
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnInteriorInventoryUpdate(interiorData, bAdded);
		}
		foreach (IInventoryChangeObserver value in m_dicInventoryObserver.Values)
		{
			value?.OnInteriorInventoryUpdate(interiorData, bAdded);
		}
	}

	private static void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipData)
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnEquipChange(eType, equipUID, equipData);
		}
		foreach (IInventoryChangeObserver value in m_dicInventoryObserver.Values)
		{
			value?.OnEquipChange(eType, equipUID, equipData);
		}
	}

	private static void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnUnitUpdate(eEventType, eUnitType, uid, unitData);
		}
	}

	private static void OnOperatorUpdate(NKMUserData.eChangeNotifyType eEventType, long uid, NKMOperator operatorData)
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnOperatorUpdate(eEventType, uid, operatorData);
		}
	}

	private static void OnDeckUpdate(NKMDeckIndex deckIndex, NKMDeckData deckData)
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnDeckUpdate(deckIndex, deckData);
		}
	}

	private static void OnCompanyBuffUpdate(NKMUserData userData)
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnCompanyBuffUpdate(userData);
		}
	}

	public static bool OnHotkey(HotkeyEventType hotkey)
	{
		if (m_stkUI.Count > 0)
		{
			foreach (NKCUIBase item in m_stkUI)
			{
				if (item.OnHotkey(hotkey) && hotkey != HotkeyEventType.ShowHotkey)
				{
					return true;
				}
				if (!item.PassHotkeyToNextUI)
				{
					break;
				}
			}
		}
		return false;
	}

	public static void OnHotkeyHold(HotkeyEventType hotkey)
	{
		if (m_stkUI.Count > 0)
		{
			m_stkUI.Peek().OnHotkeyHold(hotkey);
		}
	}

	public static void OnHotKeyRelease(HotkeyEventType hotkey)
	{
		if (m_stkUI.Count > 0)
		{
			m_stkUI.Peek().OnHotkeyRelease(hotkey);
		}
	}

	public static NKCUIBase FindRootUIBase(Transform tr)
	{
		while (tr.parent != null)
		{
			tr = tr.parent;
			NKCUIBase component = tr.GetComponent<NKCUIBase>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public static void OnScreenResolutionChanged()
	{
		SetAspect();
		NKCUIComSafeArea.RevertCalculatedSafeArea();
		NKCUIComSafeArea.InitSafeArea();
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			NKCUIComSafeArea[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren<NKCUIComSafeArea>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (!(componentsInChildren[j] == null) && componentsInChildren[j].enabled)
				{
					if (componentsInChildren[j].CheckInit())
					{
						componentsInChildren[j].Rollback();
					}
					componentsInChildren[j].SetSafeAreaBase();
				}
			}
		}
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnScreenResolutionChanged();
		}
	}

	public static void OnGuildDataChanged()
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnGuildDataChanged();
		}
	}

	public static void OnMissionUpdated()
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			item.OnMissionUpdated();
		}
	}

	public static void UIClosed(NKCUIBase closedUI)
	{
		if (closedUI.eUIType == NKCUIBase.eMenutype.Overlay)
		{
			if (!m_setOverlayUI.Contains(closedUI))
			{
				Debug.LogWarning("Closed UI asked to closed again");
				return;
			}
			m_setOverlayUI.Remove(closedUI);
			closedUI.CloseInternal();
			return;
		}
		if (!m_stkUI.Contains(closedUI))
		{
			Debug.LogError("FATAL : closing UI(" + closedUI.gameObject.name + " : " + closedUI.MenuName + ") not registered in UIManager.");
			closedUI.CloseInternal();
			return;
		}
		while (m_stkUI.Count > 0)
		{
			NKCUIBase nKCUIBase = m_stkUI.Pop();
			nKCUIBase._ForceCloseInternal();
			if (nKCUIBase == closedUI)
			{
				break;
			}
		}
		if (closedUI.IsFullScreenUI)
		{
			foreach (NKCUIBase item in m_stkUI)
			{
				item.UnHide();
				if (item.IsFullScreenUI)
				{
					s_currentFullScreenUIBase = item;
					SetBackground(item);
					break;
				}
			}
		}
		UpdateUpsideMenu();
	}

	public static void UpdateUpsideMenu()
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			if (item.eUpsideMenuMode != NKCUIUpsideMenu.eMode.Invalid)
			{
				SetUpsideMenuState(item);
				break;
			}
		}
	}

	private static void SetUpsideMenuState(NKCUIBase targetUI, bool bOpen = false)
	{
		if (targetUI.eUpsideMenuMode == NKCUIUpsideMenu.eMode.Invalid)
		{
			return;
		}
		if (targetUI.eUpsideMenuMode == NKCUIUpsideMenu.eMode.Disable)
		{
			NKCUIUpsideMenu.Close();
			return;
		}
		string name;
		switch (targetUI.eUpsideMenuMode)
		{
		case NKCUIUpsideMenu.eMode.Disable:
		case NKCUIUpsideMenu.eMode.ResourceOnly:
		case NKCUIUpsideMenu.eMode.Invalid:
			name = ((s_currentFullScreenUIBase != null) ? s_currentFullScreenUIBase.MenuName : targetUI.MenuName);
			break;
		default:
			name = targetUI.MenuName;
			break;
		}
		NKCUIUpsideMenu.Open(targetUI.UpsideMenuShowResourceList, targetUI.eUpsideMenuMode, name, targetUI.GuideTempletID, targetUI.DisableSubMenu);
	}

	public static void OnHomeButton()
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			if (!item.OnHomeButton())
			{
				return;
			}
		}
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
	}

	public static void OnBackButton()
	{
		if (m_eUITransitionState != eUITransitionProcess.Idle || !NKCUIFadeInOut.IsFinshed())
		{
			return;
		}
		foreach (NKCUIBase item in m_stkUI)
		{
			if (!item.IsHidden && item.IgnoreBackButtonWhenOpen)
			{
				return;
			}
		}
		foreach (NKCUIBase item2 in m_setOverlayUI)
		{
			if (!item2.IsHidden && item2.IgnoreBackButtonWhenOpen)
			{
				return;
			}
		}
		Log.Debug("[OnBackButton] back button pressed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIManager.cs", 1176);
		if (m_stkUI.Count > 0)
		{
			m_stkUI.Peek().OnBackButton();
		}
		else if (NKCScenManager.GetScenManager() != null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
		}
	}

	private static int GetUIKey()
	{
		return ++UIKeySeed;
	}

	public static LoadedUIData GetInstance(string bundleName, string assetName, bool OpenedUIOnly)
	{
		foreach (LoadedUIData value in s_dicLoadedUI.Values)
		{
			if (value.bundleName == bundleName && value.assetName == assetName)
			{
				if (OpenedUIOnly)
				{
					_ = value.IsUIOpen;
					return value;
				}
				return value;
			}
		}
		return null;
	}

	public static bool IsValid(LoadedUIData uiData)
	{
		if (uiData == null)
		{
			return false;
		}
		if (!s_dicLoadedUI.ContainsKey(uiData.key))
		{
			return false;
		}
		if (!uiData.HasAssetResourceData)
		{
			return false;
		}
		return true;
	}

	public static LoadedUIData OpenNewInstance<T>(NKMAssetName assetName, Transform parent, LoadedUIData.OnCleanupInstance onCleanupInstance) where T : NKCUIBase
	{
		return OpenNewInstance<T>(assetName.m_BundleName, assetName.m_AssetName, parent, onCleanupInstance, bAsync: false);
	}

	public static LoadedUIData OpenNewInstance<T>(NKMAssetName assetName, eUIBaseRect parent, LoadedUIData.OnCleanupInstance onCleanupInstance) where T : NKCUIBase
	{
		return OpenNewInstance<T>(assetName.m_BundleName, assetName.m_AssetName, GetUIBaseRect(parent), onCleanupInstance, bAsync: false);
	}

	public static LoadedUIData OpenNewInstance<T>(string BundleName, string AssetName, Transform parent, LoadedUIData.OnCleanupInstance onCleanupInstance) where T : NKCUIBase
	{
		return OpenNewInstance<T>(BundleName, AssetName, parent, onCleanupInstance, bAsync: false);
	}

	public static LoadedUIData OpenNewInstance<T>(string BundleName, string AssetName, eUIBaseRect parent, LoadedUIData.OnCleanupInstance onCleanupInstance) where T : NKCUIBase
	{
		return OpenNewInstance<T>(BundleName, AssetName, GetUIBaseRect(parent), onCleanupInstance, bAsync: false);
	}

	public static LoadedUIData OpenNewInstanceAsync<T>(string BundleName, string AssetName, Transform parent, LoadedUIData.OnCleanupInstance onCleanupInstance) where T : NKCUIBase
	{
		return OpenNewInstance<T>(BundleName, AssetName, parent, onCleanupInstance, bAsync: true);
	}

	public static LoadedUIData OpenNewInstanceAsync<T>(string BundleName, string AssetName, eUIBaseRect parent, LoadedUIData.OnCleanupInstance onCleanupInstance) where T : NKCUIBase
	{
		return OpenNewInstance<T>(BundleName, AssetName, GetUIBaseRect(parent), onCleanupInstance, bAsync: true);
	}

	private static LoadedUIData OpenNewInstance<T>(string BundleName, string AssetName, Transform parent, LoadedUIData.OnCleanupInstance onCleanupInstance, bool bAsync) where T : NKCUIBase
	{
		LoadedUIData reuseableUIData = GetReuseableUIData<T>(BundleName, AssetName);
		if (reuseableUIData != null)
		{
			return reuseableUIData;
		}
		NKCAssetResourceData assetData = NKCAssetResourceManager.OpenResource<GameObject>(BundleName, AssetName, bAsync);
		LoadedUIData loadedUIData = new LoadedUIData(GetUIKey(), assetData, parent, onCleanupInstance);
		s_dicLoadedUI.Add(loadedUIData.key, loadedUIData);
		loadedUIData.IsOpenReserved = bAsync;
		return loadedUIData;
	}

	private static LoadedUIData GetReuseableUIData<T>(string BundleName, string AssetName) where T : NKCUIBase
	{
		foreach (LoadedUIData value in s_dicLoadedUI.Values)
		{
			if (string.Equals(value.bundleName, BundleName, StringComparison.InvariantCultureIgnoreCase) && value.assetName == AssetName && IsValid(value) && (!value.HasInstance || (value.GetInstance<T>() != null && !value.GetInstance<T>().IsOpen)))
			{
				return value;
			}
		}
		return null;
	}

	public static LoadedUIData GetUIData(int key)
	{
		if (s_dicLoadedUI.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	private static void InstanceClosed(LoadedUIData uiData)
	{
		if (uiData != null)
		{
			if (!s_dicLoadedUI.ContainsKey(uiData.key))
			{
				Debug.LogWarning("bad UI Key. UI Already unloaded maybe?");
			}
			else
			{
				s_dicLoadedUI.Remove(uiData.key);
			}
		}
	}

	public static void OnSceneOpenComplete()
	{
		foreach (KeyValuePair<int, LoadedUIData> item in s_dicLoadedUI)
		{
			LoadedUIData value = item.Value;
			if (value.IsLoadComplete)
			{
				value.IsOpenReserved = false;
			}
		}
	}

	public static bool IsUnloadUIOnScenChange()
	{
		return NKCScenManager.GetScenManager().GetSystemMemorySize() < 4000;
	}

	public static void OnScenEnd(eUIUnloadFlag unloadFlag)
	{
		NKCPopupMessageToastSimple.CheckInstanceAndClose();
		UnloadAllUI(unloadFlag);
	}

	public static void UnloadAllUI(eUIUnloadFlag unloadFlag, bool dontCloseOpenedUI = false)
	{
		List<int> list = new List<int>();
		bool flag = unloadFlag == eUIUnloadFlag.DEFAULT && !IsUnloadUIOnScenChange();
		foreach (KeyValuePair<int, LoadedUIData> item in s_dicLoadedUI)
		{
			LoadedUIData value = item.Value;
			if (value.HasInstance)
			{
				NKCUIBase instance = value.GetInstance<NKCUIBase>();
				if (instance != null)
				{
					if ((dontCloseOpenedUI && (value.IsOpenReserved || instance.IsOpen)) || (instance.eUIType == NKCUIBase.eMenutype.Overlay && instance.IsOpen))
					{
						continue;
					}
					if (instance.IsOpen)
					{
						instance.Close();
					}
					if (flag)
					{
						continue;
					}
					if (value.eUnloadFlag <= unloadFlag)
					{
						instance.OnCloseInstance();
						UnityEngine.Object.Destroy(instance.gameObject);
						value.dOnCleanupInstance?.Invoke();
						value.dOnCleanupInstance = null;
					}
				}
			}
			if (value.eUnloadFlag <= unloadFlag)
			{
				list.Add(item.Key);
				NKCAssetResourceManager.CloseResource(value.bundleName, value.assetName);
			}
		}
		foreach (int item2 in list)
		{
			s_dicLoadedUI.Remove(item2);
		}
	}

	public static bool CheckUIOpenError()
	{
		bool result = false;
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, LoadedUIData> item in s_dicLoadedUI)
		{
			LoadedUIData value = item.Value;
			if (value.HasInstance)
			{
				NKCUIBase instance = value.GetInstance<NKCUIBase>();
				if (instance != null && instance.gameObject.activeInHierarchy && !instance.IsOpen)
				{
					result = true;
					instance.OnCloseInstance();
					UnityEngine.Object.Destroy(instance.gameObject);
					value.dOnCleanupInstance?.Invoke();
					value.dOnCleanupInstance = null;
					list.Add(item.Key);
					NKCAssetResourceManager.CloseResource(value.bundleName, value.assetName);
				}
			}
		}
		foreach (int item2 in list)
		{
			s_dicLoadedUI.Remove(item2);
		}
		return result;
	}

	public static T GetOpenedUIByType<T>() where T : NKCUIBase
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			if (item is T)
			{
				return item as T;
			}
		}
		return null;
	}

	public static List<T> GetOpenedUIsByType<T>() where T : NKCUIBase
	{
		List<T> list = new List<T>();
		foreach (NKCUIBase item in m_stkUI)
		{
			T val = item as T;
			if (val != null)
			{
				list.Add(val);
			}
		}
		return list;
	}

	public static bool GetAnyUIOpenedByType<T>() where T : NKCUIBase
	{
		return GetOpenedUIsByType<T>().Count > 0;
	}

	public static void ForAllUI<T>(Action<T> action, bool bOpenOnly = true) where T : NKCUIBase
	{
		if (action == null)
		{
			return;
		}
		foreach (NKCUIBase item in m_stkUI)
		{
			T val = item as T;
			if (val != null && (!bOpenOnly || val.IsOpen))
			{
				action(val);
			}
		}
	}

	public static void CloseAllByType<T>() where T : NKCUIBase
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			T val = item as T;
			if (val != null && val.IsOpen)
			{
				val.Close();
			}
		}
	}

	public static bool IsAnyPopupOpened()
	{
		foreach (NKCUIBase item in m_stkUI)
		{
			if (item.eUIType == NKCUIBase.eMenutype.Popup)
			{
				return true;
			}
		}
		return false;
	}

	public static void SetAsTopmost(NKCUIBase ui, bool bForce = false)
	{
		if (!ui.IsOpen || !ui.IsFullScreenUI || (!bForce && s_currentFullScreenUIBase == ui))
		{
			return;
		}
		while (m_stkUI.Count > 0)
		{
			if (m_stkUI.Peek() != ui)
			{
				m_stkUI.Pop()._ForceCloseInternal();
				continue;
			}
			ui.UnHide();
			s_currentFullScreenUIBase = ui;
			SetBackground(ui);
			break;
		}
		UpdateUpsideMenu();
	}

	public static NKCUIBase.eMenutype GetTopmostUIType()
	{
		return m_stkUI.Peek().eUIType;
	}

	public static NKCUIUpsideMenu.eMode GetTopmoseUIUpsidemenuMode()
	{
		if (m_stkUI.Count > 0)
		{
			return m_stkUI.Peek().eUpsideMenuMode;
		}
		return NKCUIUpsideMenu.eMode.Invalid;
	}

	public static bool CanUIProcessHotkey(NKCUIBase targetUI)
	{
		if (targetUI == null)
		{
			return false;
		}
		if (m_stkUI.Count == 0)
		{
			return false;
		}
		foreach (NKCUIBase item in m_stkUI)
		{
			if (item == targetUI)
			{
				return true;
			}
			if (!item.PassHotkeyToNextUI)
			{
				return false;
			}
		}
		return false;
	}

	public static bool IsTopmostUI(NKCUIBase ui)
	{
		if (ui == null)
		{
			return false;
		}
		if (m_stkUI.Count == 0)
		{
			return false;
		}
		return m_stkUI.Peek() == ui;
	}

	public static void CloseAllOverlay()
	{
		foreach (NKCUIBase item in new List<NKCUIBase>(m_setOverlayUI))
		{
			item.Close();
		}
	}

	public static void CloseAllPopup()
	{
		while (m_stkUI.Count > 0 && m_stkUI.Peek().IsPopupUI)
		{
			m_stkUI.Pop()._ForceCloseInternal();
		}
	}
}
