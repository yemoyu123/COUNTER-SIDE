using System;
using System.Collections.Generic;
using ClientPacket.User;
using NKM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobby3DV2 : MonoBehaviour
{
	public const string BLACK_BACKGROUND_BUNDLE = "AB_UI_BG_SPRITE_EBENUM";

	public const string BLACK_BACKGROUND_ASSET = "AB_UI_BG_SPRITE_EBENUM";

	public RectTransform m_rtRoot;

	public RectTransform m_rtBackgroundRoot;

	public List<NKCUICharacterView> m_lstCvLobbyUnit;

	public RectTransform m_rtMenuRoot;

	public CanvasGroup m_MenuCanvasGroup;

	public NKCUIPointExchangeLobby m_pointExchangeLobby;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public bool m_bUseSafeRectGap = true;

	public float m_fXGapFromRight = 50f;

	private UnityAction TouchCallback;

	private GameObject m_objBackground;

	private bool m_bUseBGDrag;

	private Vector3 m_vOriginalPos;

	private Vector3 m_vRelativePosToCamera;

	public void Init()
	{
		m_vOriginalPos = m_rtMenuRoot.position;
		AdjustPositionByScreenRatio();
		for (int i = 0; i < m_lstCvLobbyUnit.Count; i++)
		{
			m_lstCvLobbyUnit[i]?.Init(OnDrag);
		}
		m_pointExchangeLobby?.Init();
	}

	public void AdjustPositionByScreenRatio()
	{
		SetRelativePostionByCamera();
		RescaleBackground();
	}

	public void CleanUp()
	{
		for (int i = 0; i < m_lstCvLobbyUnit.Count; i++)
		{
			m_lstCvLobbyUnit[i]?.CleanUp();
		}
		if (m_objBackground != null)
		{
			UnityEngine.Object.Destroy(m_objBackground);
		}
	}

	public void SetData(NKMUserData userData)
	{
		if (userData != null)
		{
			SetUnitIllusts(userData);
			SetBackground(userData);
		}
		else
		{
			SetBackground(null);
		}
		m_pointExchangeLobby?.SetData();
	}

	private void SetUnitIllusts(NKMUserData userData)
	{
		for (int i = 0; i < m_lstCvLobbyUnit.Count; i++)
		{
			NKCUICharacterView nKCUICharacterView = m_lstCvLobbyUnit[i];
			if (!(nKCUICharacterView == null))
			{
				NKMBackgroundUnitInfo backgroundUnitInfo = userData.GetBackgroundUnitInfo(i);
				if (backgroundUnitInfo == null)
				{
					nKCUICharacterView.CloseCharacterIllust();
				}
				else
				{
					nKCUICharacterView.SetCharacterIllust(backgroundUnitInfo);
				}
			}
		}
	}

	private void SetBackground(NKMUserData userData)
	{
		NKCBackgroundTemplet nKCBackgroundTemplet = null;
		if (userData != null)
		{
			nKCBackgroundTemplet = NKCBackgroundTemplet.Find(userData.backGroundInfo.backgroundItemId);
		}
		if (nKCBackgroundTemplet == null)
		{
			nKCBackgroundTemplet = NKCBackgroundTemplet.Find(9001);
		}
		if (nKCBackgroundTemplet == null)
		{
			m_bUseBGDrag = true;
			SetBackground(NKMAssetName.ParseBundleName("AB_UI_BG_SPRITE_CITY_NIGHT", "AB_UI_BG_SPRITE_CITY_NIGHT"), bCamMove: true, NKCBackgroundTemplet.BgType.Background);
		}
		else
		{
			m_bUseBGDrag = nKCBackgroundTemplet.m_bBackground_CamMove;
			SetBackground(NKMAssetName.ParseBundleName(nKCBackgroundTemplet.m_Background_Prefab, nKCBackgroundTemplet.m_Background_Prefab), nKCBackgroundTemplet.m_bBackground_CamMove, nKCBackgroundTemplet.m_BgType);
		}
	}

	private void RescaleBackground()
	{
		if (m_objBackground == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKCBackgroundTemplet nKCBackgroundTemplet = null;
		if (nKMUserData != null)
		{
			nKCBackgroundTemplet = NKCBackgroundTemplet.Find(nKMUserData.backGroundInfo.backgroundItemId);
		}
		if (nKCBackgroundTemplet == null)
		{
			nKCBackgroundTemplet = NKCBackgroundTemplet.Find(9001);
		}
		bool flag = nKCBackgroundTemplet?.m_bBackground_CamMove ?? false;
		if ((nKCBackgroundTemplet?.m_BgType ?? NKCBackgroundTemplet.BgType.Background) == NKCBackgroundTemplet.BgType.CutsceneObject)
		{
			Transform transform = m_objBackground.transform.Find("Stage");
			if (transform == null)
			{
				transform = m_objBackground.transform;
			}
			if (transform != null)
			{
				RectTransform component = transform.GetComponent<RectTransform>();
				NKCCamera.SetPos(0f, 0f, -1000f);
				Vector2 vector = (flag ? new Vector2(200f, 200f) : Vector2.zero);
				Vector2 vector2 = new Vector2(Screen.width, Screen.height) + vector;
				float a = vector2.x / 1920f;
				float b = vector2.y / 1080f;
				float num = Mathf.Max(a, b);
				component.localScale = new Vector3(num, num, 1f);
				component.offsetMin = Vector2.zero;
				component.offsetMax = Vector2.zero;
				component.anchoredPosition = Vector2.zero;
			}
		}
		else
		{
			Transform transform2 = m_objBackground.transform.Find("Stretch/Background");
			if (transform2 != null)
			{
				NKCCamera.RescaleRectToCameraFrustrum(transform2.GetComponent<RectTransform>(), CameraMoveRectSize: flag ? new Vector2(200f, 200f) : Vector2.zero, targetCamera: NKCCamera.GetCamera(), farthestZPosition: -1000f);
			}
		}
	}

	private void SetBackground(NKMAssetName assetName, bool bCamMove, NKCBackgroundTemplet.BgType bgType)
	{
		if (m_objBackground != null)
		{
			UnityEngine.Object.Destroy(m_objBackground);
		}
		switch (bgType)
		{
		case NKCBackgroundTemplet.BgType.Background:
			m_objBackground = OpenBackgroundPrefab(assetName, bCamMove, m_rtBackgroundRoot, OnDrag, null);
			break;
		case NKCBackgroundTemplet.BgType.Image:
		{
			NKMAssetName assetName2 = new NKMAssetName("AB_UI_BG_SPRITE_EBENUM", "AB_UI_BG_SPRITE_EBENUM");
			m_objBackground = OpenBackgroundPrefab(assetName2, bCamMove, m_rtBackgroundRoot, OnDrag, null);
			if (m_objBackground != null)
			{
				Transform transform = m_objBackground.transform.Find("Stretch/Background");
				if (transform != null)
				{
					Image component = transform.GetComponent<Image>();
					Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(assetName);
					NKCUtil.SetImageSprite(component, orLoadAssetResource, bDisableIfSpriteNull: true);
				}
			}
			break;
		}
		case NKCBackgroundTemplet.BgType.CutsceneObject:
			m_objBackground = OpenBGCutscenePrefab(assetName, bCamMove, m_rtBackgroundRoot, OnDrag, null);
			break;
		}
	}

	public static GameObject OpenBackgroundPrefab(NKMAssetName assetName, bool bCamMove, Transform parent, UnityAction<PointerEventData> onDrag, UnityAction<PointerEventData> onTouchBG)
	{
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<GameObject>(assetName);
		if (nKCAssetResourceData == null)
		{
			return null;
		}
		GameObject gameObject = null;
		if (nKCAssetResourceData.GetAsset<GameObject>() != null)
		{
			gameObject = UnityEngine.Object.Instantiate(nKCAssetResourceData.GetAsset<GameObject>());
			if (null != gameObject)
			{
				gameObject.transform.SetParent(parent);
				Transform transform = gameObject.transform.Find("Stretch/Background");
				if (transform != null)
				{
					RectTransform component = transform.GetComponent<RectTransform>();
					if (null != component)
					{
						NKCCamera.SetPos(0f, 0f, -1000f);
						Vector2 cameraMoveRectSize = (bCamMove ? new Vector2(200f, 200f) : Vector2.zero);
						NKCCamera.RescaleRectToCameraFrustrum(component, NKCCamera.GetCamera(), cameraMoveRectSize, -1000f);
					}
					EventTrigger eventTrigger = transform.GetComponent<EventTrigger>();
					if (eventTrigger == null)
					{
						eventTrigger = transform.gameObject.AddComponent<EventTrigger>();
					}
					eventTrigger.triggers.Clear();
					if (onDrag != null)
					{
						EventTrigger.Entry entry = new EventTrigger.Entry();
						entry.eventID = EventTriggerType.Drag;
						entry.callback.AddListener(delegate(BaseEventData eventData)
						{
							PointerEventData arg = eventData as PointerEventData;
							onDrag(arg);
						});
						eventTrigger.triggers.Add(entry);
					}
					if (onTouchBG != null)
					{
						EventTrigger.Entry entry2 = new EventTrigger.Entry();
						entry2.eventID = EventTriggerType.PointerDown;
						entry2.callback.AddListener(delegate(BaseEventData eventData)
						{
							PointerEventData arg = eventData as PointerEventData;
							onTouchBG(arg);
						});
						eventTrigger.triggers.Add(entry2);
					}
				}
			}
		}
		NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
		return gameObject;
	}

	public static GameObject OpenBGCutscenePrefab(NKMAssetName assetName, bool bCamMove, Transform parent, UnityAction<PointerEventData> onDrag, UnityAction<PointerEventData> onTouchBG)
	{
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<GameObject>(assetName);
		if (nKCAssetResourceData == null)
		{
			return null;
		}
		GameObject gameObject = null;
		if (nKCAssetResourceData.GetAsset<GameObject>() != null)
		{
			gameObject = UnityEngine.Object.Instantiate(nKCAssetResourceData.GetAsset<GameObject>());
			if (gameObject != null)
			{
				gameObject.transform.SetParent(parent);
				Transform transform = gameObject.transform.Find("STAGE");
				RectTransform rectTransform = null;
				if (transform == null)
				{
					rectTransform = gameObject.GetComponent<RectTransform>();
				}
				else
				{
					rectTransform = transform.GetComponent<RectTransform>();
					if (gameObject.TryGetComponent<AspectRatioFitter>(out var component))
					{
						component.enabled = false;
						component.enabled = true;
					}
				}
				if (rectTransform != null)
				{
					NKCCamera.SetPos(0f, 0f, -1000f);
					Vector2 vector = (bCamMove ? new Vector2(200f, 200f) : Vector2.zero);
					Vector2 vector2 = new Vector2(Screen.width, Screen.height) + vector;
					float a = vector2.x / 1920f;
					float b = vector2.y / 1080f;
					float num = Mathf.Max(a, b);
					rectTransform.localScale = new Vector3(num, num, 1f);
					rectTransform.offsetMin = Vector2.zero;
					rectTransform.offsetMax = Vector2.zero;
					rectTransform.anchoredPosition = Vector2.zero;
				}
				EventTrigger eventTrigger = rectTransform.GetComponent<EventTrigger>();
				if (eventTrigger == null)
				{
					eventTrigger = rectTransform.gameObject.AddComponent<EventTrigger>();
				}
				eventTrigger.triggers.Clear();
				if (onDrag != null)
				{
					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.Drag;
					entry.callback.AddListener(delegate(BaseEventData eventData)
					{
						PointerEventData arg = eventData as PointerEventData;
						onDrag(arg);
					});
					eventTrigger.triggers.Add(entry);
				}
				if (onTouchBG != null)
				{
					EventTrigger.Entry entry2 = new EventTrigger.Entry();
					entry2.eventID = EventTriggerType.PointerDown;
					entry2.callback.AddListener(delegate(BaseEventData eventData)
					{
						PointerEventData arg = eventData as PointerEventData;
						onTouchBG(arg);
					});
					eventTrigger.triggers.Add(entry2);
				}
			}
		}
		NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
		return gameObject;
	}

	private void OnDrag(PointerEventData cPointerEventData)
	{
		if (m_bUseBGDrag)
		{
			float value = NKCCamera.GetPosNowX() - cPointerEventData.delta.x * 10f;
			float value2 = NKCCamera.GetPosNowY() - cPointerEventData.delta.y * 10f;
			value = Mathf.Clamp(value, -50f, 50f);
			value2 = Mathf.Clamp(value2, -50f, 50f);
			NKCCamera.TrackingPos(1f, value, value2);
		}
	}

	public void SetTouchCallback(UnityAction callback)
	{
		TouchCallback = callback;
	}

	private void OnTouch(BaseEventData cBaseEventData)
	{
		TouchCallback?.Invoke();
	}

	public bool CameraTracking()
	{
		return m_bUseBGDrag;
	}

	private void Update()
	{
		m_rtMenuRoot.position = NKCCamera.GetCamera().transform.position + m_vRelativePosToCamera;
	}

	public void SetRelativePostionByCamera()
	{
		float fieldOfView = NKCCamera.GetCamera().fieldOfView;
		float num = m_rtMenuRoot.GetWidth() * m_rtMenuRoot.localScale.x;
		float num2 = num * Mathf.Sin(m_rtMenuRoot.localEulerAngles.y * ((float)Math.PI / 180f));
		float num3 = 1000f - num2;
		float num4 = Mathf.Tan(fieldOfView * ((float)Math.PI / 180f) * 0.5f) * num3 * 2f;
		float num5 = NKCCamera.GetScreenRatio(bSafeRect: false) * num4;
		float num6 = num5 / (float)Screen.width;
		float num7 = num * Mathf.Cos(m_rtMenuRoot.localEulerAngles.y * ((float)Math.PI / 180f));
		float num8 = num5 * 0.5f - (num7 * (1f - m_rtMenuRoot.pivot.x) + m_fXGapFromRight * num6);
		if (m_bUseSafeRectGap)
		{
			float x = Screen.safeArea.x;
			float width = Screen.safeArea.width;
			float num9 = (float)Screen.width - (x + width);
			num8 -= num9 * num6;
		}
		m_vRelativePosToCamera = m_vOriginalPos - new Vector3(0f, 0f, -1000f);
		m_vRelativePosToCamera.x = num8;
	}
}
