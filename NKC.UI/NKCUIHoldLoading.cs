using UnityEngine;

namespace NKC.UI;

public class NKCUIHoldLoading : MonoBehaviour
{
	private const string BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string ASSET_NAME = "NKM_UI_POPUP_DETAIL_LOADING";

	private static NKCUIHoldLoading m_instance;

	private const string ANI_NAME = "NKM_UI_POPUP_DETAIL_LOADING";

	private Animator m_animator;

	private RectTransform m_RectToCalcTouchPos;

	public static NKCUIHoldLoading Instance
	{
		get
		{
			if (m_instance == null)
			{
				NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_DETAIL_LOADING");
				if (nKCAssetInstanceData.m_Instant == null)
				{
					Debug.LogError("NKM_UI_POPUP_DETAIL_LOADING 없음");
					return null;
				}
				m_instance = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIHoldLoading>();
				m_instance.Init();
			}
			return m_instance;
		}
	}

	public static bool IsOpen
	{
		get
		{
			if (m_instance != null)
			{
				return m_instance.gameObject.activeSelf;
			}
			return false;
		}
	}

	public void Init()
	{
		if (m_animator == null)
		{
			m_animator = GetComponent<Animator>();
		}
		if (m_RectToCalcTouchPos == null)
		{
			GameObject gameObject = new GameObject("goRectToCalcTouchPos", typeof(RectTransform));
			m_RectToCalcTouchPos = gameObject.GetComponent<RectTransform>();
			m_RectToCalcTouchPos.anchoredPosition = new Vector2(0f, 0f);
			m_RectToCalcTouchPos.offsetMax = new Vector2(0f, 0f);
			m_RectToCalcTouchPos.offsetMin = new Vector2(0f, 0f);
			m_RectToCalcTouchPos.anchorMax = new Vector2(1f, 1f);
			m_RectToCalcTouchPos.anchorMin = new Vector2(0f, 0f);
			m_RectToCalcTouchPos.SetWidth(Screen.width);
			m_RectToCalcTouchPos.SetHeight(Screen.height);
		}
		base.transform.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIOverlay));
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void Open(Vector2 touchPos, float waitTime = -1f)
	{
		SetPosition(touchPos);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_animator.Play("NKM_UI_POPUP_DETAIL_LOADING", -1, 0f);
		if (waitTime > 0f)
		{
			AnimationClip animationClip = m_animator.runtimeAnimatorController.animationClips[0];
			if (animationClip != null)
			{
				m_animator.speed = animationClip.length / waitTime;
			}
			else
			{
				m_animator.speed = 1f;
			}
		}
		else
		{
			m_animator.speed = 1f;
		}
	}

	public bool IsPlaying()
	{
		return m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void SetPosition(Vector2 touchPos)
	{
		RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RectToCalcTouchPos, touchPos, null, out var worldPoint);
		worldPoint.x -= m_RectToCalcTouchPos.GetWidth() * NKCUIManager.UIFrontCanvasSafeRectTransform.pivot.x;
		worldPoint.y -= m_RectToCalcTouchPos.GetHeight() * NKCUIManager.UIFrontCanvasSafeRectTransform.pivot.y;
		worldPoint.x -= worldPoint.x * (1f - NKCUIManager.UIFrontCanvasSafeRectTransform.GetWidth() / m_RectToCalcTouchPos.GetWidth());
		worldPoint.y -= worldPoint.y * (1f - NKCUIManager.UIFrontCanvasSafeRectTransform.GetHeight() / m_RectToCalcTouchPos.GetHeight());
		worldPoint.z = 0f;
		base.transform.localPosition = worldPoint;
	}
}
