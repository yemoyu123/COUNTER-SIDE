using System.Collections.Generic;
using DG.Tweening;
using NKC.UI;
using NKM;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUICutScenBGMgr : MonoBehaviour
{
	public Image m_imgBG;

	private bool m_bImgBGTweening;

	public RectTransform m_rtBG;

	public Image m_imgBG_2;

	private bool m_bImgBG_2_Tweening;

	public RectTransform m_rtBG_2;

	private float m_fBGFadeOutTime;

	private Ease m_easeBGFadeOut = Ease.Linear;

	private Color m_colBGFadeIn = new Color(1f, 1f, 1f, 1f);

	private Color m_colBGFadeOut = new Color(1f, 1f, 1f, 1f);

	private GameObject m_goBG;

	private RectTransform m_rtGoBG;

	private SkeletonGraphic m_sgGoBG;

	private bool m_bSGGoBGTweening;

	private GameObject m_goBG_2;

	private RectTransform m_rtGoBG_2;

	private SkeletonGraphic m_sgGoBG_2;

	private bool m_bSGGoBG_2_Tweening;

	private Vector3 m_OrgGOScale = new Vector3(1f, 1f, 1f);

	private float m_fElapsedTime;

	private float m_fCrashTime;

	private float m_Crash;

	private Vector2 m_orgPosAtResetToPlay = new Vector2(0f, 0f);

	private Vector3 m_orgScaleAtResetToPlay = new Vector3(1f, 1f, 1f);

	private Vector2 m_orgPos = new Vector2(0f, 0f);

	private bool m_bAniPos;

	private bool m_bAniScale;

	private Vector2 m_OffsetPos = new Vector2(0f, 0f);

	private Vector3 m_OffsetScale = new Vector3(1f, 1f, 1f);

	private NKMTrackingVector2 m_NKMTrackingVector2 = new NKMTrackingVector2();

	private NKMTrackingVector2 m_NKMTrackingVector2_2 = new NKMTrackingVector2();

	private NKMTrackingVector3 m_NKMTrackingVector3 = new NKMTrackingVector3();

	private static NKCUICutScenBGMgr m_scNKCUICutScenBGMgr;

	private const float CRASH_DIST_DEFAULT_VALUE = 50f;

	private bool m_bNoWaitBGAni;

	private bool m_bPause;

	private List<NKCAssetResourceData> m_lstNKCAssetResourceDataToClose = new List<NKCAssetResourceData>();

	public const string CLOSE_BG_RESERVED = "CLOSE";

	public void SetOrgPosAtResetToPlay(Vector2 orgPos)
	{
		m_orgPosAtResetToPlay = orgPos;
	}

	public void SetOrgScaleAtResetToPlay(Vector3 orgScale)
	{
		m_orgScaleAtResetToPlay = orgScale;
	}

	public void SetOrgPos(Vector2 orgPos)
	{
		m_orgPos = orgPos;
	}

	public static void InitUI(GameObject goNKM_UI_CUTSCEN_PLAYER)
	{
		if (!(m_scNKCUICutScenBGMgr != null))
		{
			m_scNKCUICutScenBGMgr = goNKM_UI_CUTSCEN_PLAYER.transform.Find("NKM_UI_CUTSCEN_BG_MGR").gameObject.GetComponent<NKCUICutScenBGMgr>();
			m_scNKCUICutScenBGMgr.SetOrgPosAtResetToPlay(m_scNKCUICutScenBGMgr.m_rtBG.anchoredPosition);
			m_scNKCUICutScenBGMgr.SetOrgScaleAtResetToPlay(m_scNKCUICutScenBGMgr.m_rtBG.localScale);
			m_scNKCUICutScenBGMgr.SetOrgPos(m_scNKCUICutScenBGMgr.m_rtBG.anchoredPosition);
			m_scNKCUICutScenBGMgr.Close();
		}
	}

	public void Reset()
	{
		m_NKMTrackingVector2.StopTracking();
		m_NKMTrackingVector2_2.StopTracking();
		m_NKMTrackingVector3.StopTracking();
		m_rtBG.anchoredPosition = m_orgPosAtResetToPlay;
		m_rtBG.localScale = m_orgScaleAtResetToPlay;
		m_orgPos = m_orgPosAtResetToPlay;
		m_bNoWaitBGAni = false;
		m_bAniPos = false;
		m_bAniScale = false;
		m_OffsetPos = new Vector2(0f, 0f);
		m_OffsetScale = new Vector3(1f, 1f, 1f);
		m_bPause = false;
		ClearGOBG();
		ClearGOBG_2();
		m_imgBG.enabled = false;
		m_imgBG_2.enabled = false;
		m_bImgBGTweening = false;
		m_bImgBG_2_Tweening = false;
		m_bSGGoBGTweening = false;
		m_bSGGoBG_2_Tweening = false;
		m_imgBG_2.enabled = false;
	}

	public static NKCUICutScenBGMgr GetCutScenBGMgr()
	{
		return m_scNKCUICutScenBGMgr;
	}

	private void SetPauseDoTweenObj(bool bPause, object obj)
	{
		if (obj != null && DOTween.IsTweening(obj))
		{
			if (bPause)
			{
				DOTween.Pause(obj);
			}
			else
			{
				DOTween.Play(obj);
			}
		}
	}

	public void SetPause(bool bPause)
	{
		m_bPause = bPause;
		SetPauseDoTweenObj(bPause, m_imgBG);
		SetPauseDoTweenObj(bPause, m_imgBG_2);
		SetPauseDoTweenObj(bPause, m_sgGoBG);
		if (m_sgGoBG != null)
		{
			if (bPause)
			{
				m_sgGoBG.AnimationState.TimeScale = 0f;
			}
			else
			{
				m_sgGoBG.AnimationState.TimeScale = 1f;
			}
		}
		SetPauseDoTweenObj(bPause, m_sgGoBG_2);
		if (m_sgGoBG_2 != null)
		{
			if (bPause)
			{
				m_sgGoBG_2.AnimationState.TimeScale = 0f;
			}
			else
			{
				m_sgGoBG_2.AnimationState.TimeScale = 1f;
			}
		}
	}

	private void CheckAndStartCrash()
	{
		if (!m_NKMTrackingVector2.IsTracking())
		{
			m_NKMTrackingVector2.SetTracking(new Vector2(m_orgPos.x + NKMRandom.Range(-50f * m_Crash / 100f, 50f * m_Crash / 100f), m_orgPos.y + NKMRandom.Range(-50f * m_Crash / 100f, 50f * m_Crash / 100f)), 0.025f, TRACKING_DATA_TYPE.TDT_NORMAL);
		}
		m_NKMTrackingVector2.Update(Time.deltaTime);
		NKMVector2 nowValue = m_NKMTrackingVector2.GetNowValue();
		Vector2 anchoredPosition = new Vector2(nowValue.x, nowValue.y);
		if (m_rtGoBG != null)
		{
			m_rtGoBG.anchoredPosition = anchoredPosition;
		}
		else
		{
			m_rtBG.anchoredPosition = anchoredPosition;
		}
	}

	private void Update()
	{
		if (m_bPause || !base.gameObject.activeSelf)
		{
			return;
		}
		if (m_fCrashTime > 0f && m_Crash > 0f)
		{
			m_fElapsedTime += Time.deltaTime;
			if (m_fElapsedTime >= m_fCrashTime)
			{
				StopCrash();
			}
			else
			{
				CheckAndStartCrash();
			}
			return;
		}
		if (m_bAniPos)
		{
			m_NKMTrackingVector2_2.Update(Time.deltaTime);
			if (m_NKMTrackingVector2_2.IsTracking())
			{
				if (m_rtGoBG != null)
				{
					m_rtGoBG.anchoredPosition = m_NKMTrackingVector2_2.GetNowUnityValue();
				}
				else
				{
					m_rtBG.anchoredPosition = m_NKMTrackingVector2_2.GetNowUnityValue();
				}
			}
			else
			{
				m_bAniPos = false;
				if (m_rtGoBG != null)
				{
					m_rtGoBG.anchoredPosition = m_OffsetPos;
					m_orgPos = m_rtGoBG.anchoredPosition;
				}
				else
				{
					m_rtBG.anchoredPosition = m_OffsetPos;
					m_orgPos = m_rtBG.anchoredPosition;
				}
			}
		}
		if (!m_bAniScale)
		{
			return;
		}
		m_NKMTrackingVector3.Update(Time.deltaTime);
		if (m_NKMTrackingVector3.IsTracking())
		{
			if (m_rtGoBG != null)
			{
				m_rtGoBG.localScale = new Vector3(m_OrgGOScale.x * m_NKMTrackingVector3.GetNowUnityValue().x, m_OrgGOScale.y * m_NKMTrackingVector3.GetNowUnityValue().y, m_OrgGOScale.z * m_NKMTrackingVector3.GetNowUnityValue().z);
			}
			else
			{
				m_rtBG.localScale = m_NKMTrackingVector3.GetNowUnityValue();
			}
			return;
		}
		m_bAniScale = false;
		if (m_rtGoBG != null)
		{
			m_rtGoBG.localScale = new Vector3(m_OrgGOScale.x * m_OffsetScale.x, m_OrgGOScale.y * m_OffsetScale.y, m_OrgGOScale.z * m_OffsetScale.z);
		}
		else
		{
			m_rtBG.localScale = m_OffsetScale;
		}
	}

	private void ClearGOBG()
	{
		if (m_goBG != null)
		{
			if (m_sgGoBG != null && m_bSGGoBGTweening)
			{
				m_sgGoBG.DOKill(complete: true);
			}
			m_goBG.transform.SetParent(null);
			Object.Destroy(m_goBG);
			m_goBG = null;
			m_rtGoBG = null;
			m_sgGoBG = null;
		}
		m_bSGGoBGTweening = false;
	}

	private void ClearGOBG_2()
	{
		if (m_goBG_2 != null)
		{
			if (m_sgGoBG_2 != null && DOTween.IsTweening(m_sgGoBG_2))
			{
				m_sgGoBG_2.DOKill();
			}
			m_goBG_2.transform.SetParent(null);
			Object.Destroy(m_goBG_2);
			m_goBG_2 = null;
			m_rtGoBG_2 = null;
			m_sgGoBG_2 = null;
		}
		m_bSGGoBG_2_Tweening = false;
	}

	private void ProcessGameObjectBG(GameObject tempGO)
	{
		if (m_goBG != null && m_fBGFadeOutTime > 0f)
		{
			m_goBG_2 = m_goBG;
			m_rtGoBG_2 = m_rtGoBG;
			m_sgGoBG_2 = m_sgGoBG;
			m_sgGoBG_2.color = m_sgGoBG.color;
			m_bSGGoBG_2_Tweening = true;
			m_sgGoBG_2.DOColor(m_colBGFadeOut, m_fBGFadeOutTime).SetEase(m_easeBGFadeOut).OnComplete(ClearGOBG_2);
		}
		else
		{
			ClearGOBG();
		}
		m_goBG = Object.Instantiate(tempGO);
		if (m_goBG != null)
		{
			m_sgGoBG = m_goBG.GetComponentInChildren<SkeletonGraphic>();
			m_goBG.transform.SetParent(base.gameObject.transform);
			Transform transform = m_goBG.transform.Find("STAGE");
			RectTransform rectTransform = null;
			if (transform == null)
			{
				rectTransform = m_goBG.GetComponent<RectTransform>();
			}
			else
			{
				rectTransform = transform.GetComponent<RectTransform>();
				AspectRatioFitter component = m_goBG.GetComponent<AspectRatioFitter>();
				component.enabled = false;
				component.enabled = true;
			}
			if (rectTransform != null)
			{
				m_rtGoBG = rectTransform;
				float a = NKCUIManager.UIFrontCanvasSafeRectTransform.GetWidth() / 1920f;
				float b = NKCUIManager.UIFrontCanvasSafeRectTransform.GetHeight() / 1080f;
				float num = Mathf.Max(a, b);
				m_OrgGOScale = new Vector3(num, num, 1f);
				rectTransform.localScale = new Vector3(m_OrgGOScale.x * m_OffsetScale.x, m_OrgGOScale.y * m_OffsetScale.y, m_OrgGOScale.z * m_OffsetScale.z);
				rectTransform.offsetMin = new Vector2(0f, 0f);
				rectTransform.offsetMax = new Vector2(0f, 0f);
				rectTransform.anchoredPosition = m_OffsetPos;
				m_orgPos = rectTransform.anchoredPosition;
			}
			else
			{
				m_rtGoBG = null;
				m_goBG.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

	public void Open(bool bGameObjectBGType, string bgFileName, string aniName, bool bGameObjectBGLoop, float fBGFadeInTime, Ease easeBGFadeIn, Color colBGFadeInStart, Color colBGFadeIn, float fBGFadeOutTime, Ease easeBGFadeOut, Color colBGFadeOut)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		m_colBGFadeIn = colBGFadeIn;
		NKCAssetResourceData nKCAssetResourceData = null;
		if (bGameObjectBGType)
		{
			m_imgBG.enabled = false;
			m_imgBG_2.enabled = false;
			_ = m_goBG != null;
			if ((!(m_goBG != null) || bgFileName.Length > 0 || aniName.Length <= 0) && bgFileName.Length > 0)
			{
				nKCAssetResourceData = NKCResourceUtility.GetAssetResource(bgFileName, bgFileName);
				if (nKCAssetResourceData != null)
				{
					GameObject asset = nKCAssetResourceData.GetAsset<GameObject>();
					ProcessGameObjectBG(asset);
				}
				else
				{
					Debug.Log("Cutscen GameObjectBGType load, name : " + bgFileName);
					nKCAssetResourceData = NKCAssetResourceManager.OpenResource<GameObject>(bgFileName, bgFileName);
					if (nKCAssetResourceData != null)
					{
						if (nKCAssetResourceData.m_RefCount <= 1)
						{
							Debug.LogWarning("Cutscen GameObjectBGType need to load in the past, name : " + bgFileName);
						}
						GameObject asset2 = nKCAssetResourceData.GetAsset<GameObject>();
						ProcessGameObjectBG(asset2);
						m_lstNKCAssetResourceDataToClose.Add(nKCAssetResourceData);
					}
				}
			}
			if (aniName.Length > 0 && m_goBG != null)
			{
				SkeletonGraphic componentInChildren = m_goBG.GetComponentInChildren<SkeletonGraphic>();
				if (componentInChildren != null)
				{
					componentInChildren.AnimationState.SetAnimation(0, aniName, bGameObjectBGLoop);
				}
			}
			if (m_sgGoBG != null)
			{
				if (fBGFadeInTime > 0f)
				{
					m_sgGoBG.color = colBGFadeInStart;
					m_bSGGoBGTweening = true;
					m_sgGoBG.DOColor(colBGFadeIn, fBGFadeInTime).SetEase(easeBGFadeIn).OnComplete(delegate
					{
						m_bSGGoBGTweening = false;
					});
				}
				else
				{
					m_sgGoBG.color = new Color(1f, 1f, 1f, 1f);
				}
			}
		}
		else
		{
			ClearGOBG();
			ClearGOBG_2();
			if (fBGFadeInTime > 0f)
			{
				m_imgBG.color = colBGFadeInStart;
				m_imgBG.DOColor(colBGFadeIn, fBGFadeInTime).SetEase(easeBGFadeIn).OnComplete(delegate
				{
					m_bImgBGTweening = false;
				});
				m_bImgBGTweening = true;
			}
			else
			{
				m_imgBG.color = new Color(1f, 1f, 1f, 1f);
			}
			if (m_fBGFadeOutTime > 0f)
			{
				m_imgBG_2.enabled = true;
				m_imgBG_2.color = m_imgBG.color;
				m_imgBG_2.sprite = m_imgBG.sprite;
				m_rtBG_2.anchorMin = m_rtBG.anchorMin;
				m_rtBG_2.anchorMax = m_rtBG.anchorMax;
				m_rtBG_2.anchoredPosition = m_rtBG.anchoredPosition;
				m_rtBG_2.sizeDelta = m_rtBG.sizeDelta;
				m_bImgBG_2_Tweening = true;
				m_imgBG_2.DOColor(m_colBGFadeOut, m_fBGFadeOutTime).SetEase(m_easeBGFadeOut).OnComplete(delegate
				{
					m_bImgBG_2_Tweening = false;
				});
			}
			else
			{
				m_imgBG_2.enabled = false;
			}
			m_fBGFadeOutTime = 0f;
			m_imgBG.enabled = true;
			string text = "AB_UI_NKM_UI_CUTSCEN_BG_" + bgFileName;
			nKCAssetResourceData = NKCResourceUtility.GetAssetResource(text, text);
			if (nKCAssetResourceData != null)
			{
				m_imgBG.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			}
			else
			{
				Debug.Log("Cutscen NormalBGType load, name : " + text);
				nKCAssetResourceData = NKCAssetResourceManager.OpenResource<Sprite>(text, text);
				if (nKCAssetResourceData != null)
				{
					if (nKCAssetResourceData.m_RefCount <= 1)
					{
						Debug.LogWarning("Cutscen NormalBGType need to load in the past, name : " + bgFileName);
					}
					m_imgBG.sprite = nKCAssetResourceData.GetAsset<Sprite>();
					m_lstNKCAssetResourceDataToClose.Add(nKCAssetResourceData);
				}
			}
		}
		m_fBGFadeOutTime = fBGFadeOutTime;
		m_colBGFadeOut = colBGFadeOut;
		m_easeBGFadeOut = easeBGFadeOut;
	}

	private bool IsCrashing()
	{
		return m_NKMTrackingVector2.IsTracking();
	}

	private bool IsAnimating()
	{
		if (m_bNoWaitBGAni)
		{
			return false;
		}
		if (!m_NKMTrackingVector2_2.IsTracking())
		{
			return m_NKMTrackingVector3.IsTracking();
		}
		return true;
	}

	public bool IsFinished()
	{
		if (m_bImgBGTweening)
		{
			return false;
		}
		if (m_bImgBG_2_Tweening)
		{
			return false;
		}
		if (m_bSGGoBGTweening)
		{
			return false;
		}
		if (m_bSGGoBG_2_Tweening)
		{
			return false;
		}
		if (!IsCrashing())
		{
			return !IsAnimating();
		}
		return false;
	}

	public void Finish()
	{
		StopCrash();
		if (m_imgBG != null && m_bImgBGTweening)
		{
			m_imgBG.color = m_colBGFadeIn;
			m_imgBG.DOKill(complete: true);
		}
		if (m_imgBG_2 != null && m_bImgBG_2_Tweening)
		{
			m_imgBG_2.DOKill(complete: true);
			m_imgBG_2.color = m_colBGFadeOut;
		}
		if (m_sgGoBG != null && m_bSGGoBGTweening)
		{
			m_sgGoBG.color = m_colBGFadeIn;
			m_sgGoBG.DOKill(complete: true);
		}
		if (m_sgGoBG_2 != null && m_bSGGoBG_2_Tweening)
		{
			m_sgGoBG_2.DOKill(complete: true);
		}
		if (!m_bNoWaitBGAni)
		{
			StopAni();
		}
	}

	private void StopCrash()
	{
		if (!(m_fCrashTime <= 0f))
		{
			m_NKMTrackingVector2.StopTracking();
			m_rtBG.anchoredPosition = m_orgPos;
			if (m_rtGoBG != null)
			{
				m_rtGoBG.anchoredPosition = m_orgPos;
			}
			m_Crash = 0f;
			m_fElapsedTime = 0f;
			m_fCrashTime = 0f;
		}
	}

	public void SetCrash(int crash, float fCrashTime)
	{
		if (crash > 0 && !(fCrashTime <= 0f))
		{
			StopAni();
			StopCrash();
			m_fCrashTime = fCrashTime;
			m_Crash = crash;
			CheckAndStartCrash();
		}
	}

	public void SetAni(bool bNoWaitBGAni, float fTime, bool bAniPos, Vector2 OffsetPos, TRACKING_DATA_TYPE tdtPos, bool bAniScale, Vector3 OffsetScale, TRACKING_DATA_TYPE tdtScale)
	{
		if (fTime == 0f || (!bAniPos && !bAniScale))
		{
			return;
		}
		m_bNoWaitBGAni = bNoWaitBGAni;
		m_bAniPos = bAniPos;
		m_bAniScale = bAniScale;
		m_OffsetPos = OffsetPos;
		m_OffsetScale = OffsetScale;
		if (m_bAniPos)
		{
			if (fTime <= 0.01f)
			{
				if (m_rtGoBG != null)
				{
					m_rtGoBG.anchoredPosition = m_OffsetPos;
				}
				else
				{
					m_rtBG.anchoredPosition = m_OffsetPos;
				}
			}
			Vector3 vector = default(Vector3);
			NKMVector2Extension.SetNowValue(NowValue: (!(m_rtGoBG != null)) ? ((Vector3)m_rtBG.anchoredPosition) : ((Vector3)m_rtGoBG.anchoredPosition), cNKMVector2: m_NKMTrackingVector2_2);
			m_NKMTrackingVector2_2.SetTracking(m_OffsetPos, fTime, tdtPos);
		}
		if (m_bAniScale)
		{
			Vector3 vector2 = default(Vector3);
			NKMVector3Extension.SetNowValue(NowValue: (!(m_rtGoBG != null)) ? m_rtBG.localScale : new Vector3(m_rtGoBG.localScale.x / m_OrgGOScale.x, m_rtGoBG.localScale.y / m_OrgGOScale.y, m_rtGoBG.localScale.z / m_OrgGOScale.z), cNKMVector3: m_NKMTrackingVector3);
			m_NKMTrackingVector3.SetTracking(OffsetScale, fTime, tdtScale);
		}
	}

	private void StopAni()
	{
		m_NKMTrackingVector2_2.StopTracking();
		m_NKMTrackingVector3.StopTracking();
		if (m_bAniPos)
		{
			if (m_rtGoBG != null)
			{
				Vector2 orgPos = (m_rtGoBG.anchoredPosition = m_OffsetPos);
				m_orgPos = orgPos;
			}
			else
			{
				Vector2 orgPos = (m_rtBG.anchoredPosition = m_OffsetPos);
				m_orgPos = orgPos;
			}
		}
		m_bAniPos = false;
		if (m_bAniScale)
		{
			if (m_rtGoBG != null)
			{
				m_rtGoBG.localScale = new Vector3(m_OrgGOScale.x * m_OffsetScale.x, m_OrgGOScale.y * m_OffsetScale.y, m_OrgGOScale.z * m_OffsetScale.z);
			}
			else
			{
				m_rtBG.localScale = m_OffsetScale;
			}
		}
		m_bAniScale = false;
	}

	public void CloseBG()
	{
		ClearGOBG();
		ClearGOBG_2();
		m_imgBG.enabled = false;
		m_imgBG_2.enabled = false;
	}

	public void Close()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		ClearGOBG();
		ClearGOBG_2();
		for (int i = 0; i < m_lstNKCAssetResourceDataToClose.Count; i++)
		{
			NKCAssetResourceManager.CloseResource(m_lstNKCAssetResourceDataToClose[i]);
		}
		m_lstNKCAssetResourceDataToClose.Clear();
	}
}
