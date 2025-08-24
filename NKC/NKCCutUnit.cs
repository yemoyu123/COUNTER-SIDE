using System.Collections.Generic;
using DG.Tweening;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCCutUnit : MonoBehaviour
{
	private const float DARK_COLOR = 0.25f;

	private const float UNIT_COLOR_CHANGE_TIME = 0.15f;

	private const float CRASH_DIST_DEFAULT_VALUE = 50f;

	private const float CRASH_DURATION_TIME = 0.025f;

	private const float BOUNCE_DIST_VALUE_Y = 50f;

	private NKCCutTemplet m_NKCCutTemplet;

	private GameObject m_goUnit;

	private RectTransform m_rectTransform;

	private Vector2 m_orgPos = new Vector2(0f, 0f);

	private Vector2 m_orgPosWithOffset = new Vector2(0f, 0f);

	private CanvasGroup m_canvasGroup;

	private NKCUICharacterView m_NKCUICharacterView;

	private string m_prefab = "";

	private int m_skinID;

	private int m_crash;

	private int m_skinOption;

	private string m_skinName;

	private NKMTrackingFloat m_NKMTrackingFloatUnitColor = new NKMTrackingFloat();

	private readonly NKMTrackingFloat m_NKMTrackingFloatUnitAlpha = new NKMTrackingFloat();

	private NKMTrackingVector2 m_NKMTrackingVector2Pos = new NKMTrackingVector2();

	private NKMTrackingVector2 m_NKMTrackingVector2Crash = new NKMTrackingVector2();

	private readonly NKMTrackingVector3 m_NKMTrackingVector3UnitScale = new NKMTrackingVector3();

	private Sequence m_bounceSequence;

	private bool m_bFinished = true;

	public GameObject GoUnit => m_goUnit;

	public RectTransform RectTransform => m_rectTransform;

	public NKCUICharacterView CharacterView => m_NKCUICharacterView;

	public NKCCutTemplet NKCCutTemplet
	{
		get
		{
			return m_NKCCutTemplet;
		}
		set
		{
			m_NKCCutTemplet = value;
		}
	}

	public NKCUICharacterView NKCCharacterView
	{
		get
		{
			return m_NKCUICharacterView;
		}
		set
		{
			m_NKCUICharacterView = value;
		}
	}

	public void BounceUnit(int count, float time)
	{
		m_bFinished = false;
		m_bounceSequence = DOTween.Sequence();
		for (int i = 0; i < count; i++)
		{
			m_bounceSequence.Append(m_rectTransform.DOAnchorPosY(m_orgPosWithOffset.y + 50f, time));
			m_bounceSequence.Append(m_rectTransform.DOAnchorPosY(m_orgPosWithOffset.y, time));
		}
		m_bounceSequence.OnComplete(FinishUnit);
		m_bounceSequence.Play();
	}

	public void ManualUpdate()
	{
		UpdateUnitCrash();
		UpdateCanvasGroupAlpha();
		UpdateUnitColor();
		UpdateUnitAlpha();
		UpdateUnitScale();
	}

	public void UpdateUnitPos()
	{
		if (m_NKCCutTemplet == null || m_bFinished)
		{
			return;
		}
		m_NKMTrackingVector2Pos.Update(Time.deltaTime);
		if (m_NKCCutTemplet.CutUnitAction == CutUnitPosAction.IN || m_NKCCutTemplet.CutUnitAction == CutUnitPosAction.OUT || m_NKCCutTemplet.CutUnitAction == CutUnitPosAction.MOVE)
		{
			if (m_NKMTrackingVector2Pos.IsTracking())
			{
				Vector2 anchoredPosition = m_rectTransform.anchoredPosition;
				anchoredPosition = m_NKMTrackingVector2Pos.GetNowValue();
				m_rectTransform.anchoredPosition = anchoredPosition;
			}
			else
			{
				FinishUnit();
			}
		}
	}

	public void UpdateUnitCrash()
	{
		if (m_crash > 0 && !(m_goUnit == null) && m_goUnit.activeSelf)
		{
			if (!m_NKMTrackingVector2Crash.IsTracking())
			{
				m_NKMTrackingVector2Crash.SetTracking(new Vector2(m_orgPosWithOffset.x + NKMRandom.Range(-50f * (float)m_crash / 100f, 50f * (float)m_crash / 100f), m_orgPosWithOffset.y + NKMRandom.Range(-50f * (float)m_crash / 100f, 50f * (float)m_crash / 100f)), 0.025f, TRACKING_DATA_TYPE.TDT_NORMAL);
			}
			m_NKMTrackingVector2Crash.Update(Time.deltaTime);
			NKMVector2 nowValue = m_NKMTrackingVector2Crash.GetNowValue();
			Vector2 anchoredPosition = new Vector2(nowValue.x, nowValue.y);
			m_rectTransform.anchoredPosition = anchoredPosition;
		}
	}

	public void UpdateCanvasGroupAlpha()
	{
		if (!(m_canvasGroup.alpha >= 1f) && m_goUnit.activeSelf)
		{
			m_canvasGroup.alpha += Time.deltaTime * 5f;
			if (m_canvasGroup.alpha >= 1f)
			{
				m_canvasGroup.alpha = 1f;
			}
		}
	}

	public void UpdateUnitColor()
	{
		if (m_NKMTrackingFloatUnitColor.IsTracking() && !(m_NKCUICharacterView == null) && m_NKCUICharacterView.HasCharacterIllust())
		{
			m_NKMTrackingFloatUnitColor.Update(Time.deltaTime);
			m_NKCUICharacterView.SetColor(m_NKMTrackingFloatUnitColor.GetNowValue(), m_NKMTrackingFloatUnitColor.GetNowValue(), m_NKMTrackingFloatUnitColor.GetNowValue(), -1f, bIncludeEffect: true);
		}
	}

	public void UpdateUnitAlpha()
	{
		if (m_NKMTrackingFloatUnitAlpha.IsTracking() && !(m_NKCUICharacterView == null) && m_NKCUICharacterView.HasCharacterIllust())
		{
			m_NKMTrackingFloatUnitAlpha.Update(Time.deltaTime);
			m_NKCUICharacterView.SetColor(1f, 1f, 1f, m_NKMTrackingFloatUnitAlpha.GetNowValue(), bIncludeEffect: true);
		}
	}

	public void UpdateUnitScale()
	{
		if (m_NKMTrackingVector3UnitScale.IsTracking() && !(m_rectTransform == null))
		{
			m_NKMTrackingVector3UnitScale.Update(Time.deltaTime);
			m_rectTransform.localScale = m_NKMTrackingVector3UnitScale.GetNowValue();
		}
	}

	public void StopUnitCrash()
	{
		if (m_crash > 0)
		{
			m_crash = 0;
			m_rectTransform.anchoredPosition = m_orgPosWithOffset;
		}
	}

	public void FinishUnit()
	{
		m_bFinished = true;
		m_crash = 0;
		m_rectTransform.anchoredPosition = m_orgPosWithOffset;
		if (m_bounceSequence.IsActive())
		{
			m_bounceSequence.Kill();
		}
		OnFinishedAlphaTracking();
		OnFinishedScaleTracking();
		m_canvasGroup.alpha = 1f;
		if (m_NKCCutTemplet != null && m_NKCCutTemplet.CutUnitAction == CutUnitPosAction.OUT && m_goUnit.activeSelf)
		{
			m_goUnit.SetActive(value: false);
		}
	}

	private void OnFinishedAlphaTracking()
	{
		if (m_NKMTrackingFloatUnitAlpha.IsTracking())
		{
			m_NKCUICharacterView.SetColor(1f, 1f, 1f, m_NKMTrackingFloatUnitAlpha.GetTargetValue(), bIncludeEffect: true);
			m_NKMTrackingFloatUnitAlpha.Init();
		}
	}

	private void OnFinishedScaleTracking()
	{
		if (m_NKMTrackingVector3UnitScale.IsTracking())
		{
			m_rectTransform.localScale = m_NKMTrackingVector3UnitScale.GetTargetValue();
			m_NKMTrackingVector3UnitScale.StopTracking();
		}
	}

	public void InitCutUnit()
	{
		m_rectTransform = GetComponent<RectTransform>();
		m_canvasGroup = GetComponent<CanvasGroup>();
		m_NKCUICharacterView = GetComponent<NKCUICharacterView>();
		m_goUnit = base.gameObject;
		m_orgPos = m_rectTransform.anchoredPosition;
	}

	public bool IsFinished()
	{
		if (!m_bFinished || m_NKMTrackingFloatUnitAlpha.IsTracking() || m_NKMTrackingVector3UnitScale.IsTracking())
		{
			return false;
		}
		return true;
	}

	public Color GetStartColor(bool bDarkStart)
	{
		if (bDarkStart)
		{
			NKCUICharacterView nKCUICharacterView = m_NKCUICharacterView;
			if ((object)nKCUICharacterView != null && nKCUICharacterView.GetCurrEffectType() == NKCUICharacterView.EffectType.Hologram)
			{
				return new Color(0.25f, 0.25f, 0.25f, 0.5f);
			}
			return new Color(0.25f, 0.25f, 0.25f, 1f);
		}
		return new Color(1f, 1f, 1f, 1f);
	}

	public Vector2 GetStartPos()
	{
		if (m_NKCCutTemplet.CutUnitAction == CutUnitPosAction.MOVE)
		{
			return NKCUICutScenUnitMgr.GetCutScenUnitMgr().GetUnitRectTransform(m_NKCCutTemplet.m_StartPosType).anchoredPosition;
		}
		return m_orgPosWithOffset + m_NKCCutTemplet.m_StartPos;
	}

	public Vector2 GetTargetPos()
	{
		return m_orgPosWithOffset + m_NKCCutTemplet.m_TargetPos;
	}

	public void SetUnit(NKCCutScenCharTemplet cNKCCutScenCharTemplet, NKCCutTemplet cNKCCutTemplet, Dictionary<string, int> dicSkin)
	{
		m_NKCCutTemplet = cNKCCutTemplet;
		m_NKMTrackingFloatUnitAlpha.StopTracking();
		m_NKMTrackingVector3UnitScale.StopTracking();
		m_NKMTrackingFloatUnitColor.StopTracking();
		if (cNKCCutTemplet.CutUnitAction == CutUnitPosAction.MOVE && cNKCCutTemplet.CutUnitPos != cNKCCutTemplet.m_StartPosType)
		{
			NKCUICutScenUnitMgr.GetCutScenUnitMgr().ClearUnitByPos(cNKCCutTemplet, cNKCCutTemplet.m_StartPosType);
		}
		m_bFinished = true;
		SetUnitIllust(cNKCCutScenCharTemplet, cNKCCutTemplet, dicSkin);
		SetUnitScaleByTime(cNKCCutTemplet.m_CharScale, cNKCCutTemplet.m_CharScaleTime);
		if (!m_goUnit.activeSelf)
		{
			m_goUnit.SetActive(value: true);
		}
		if (cNKCCutTemplet.m_bFlip)
		{
			m_rectTransform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			m_rectTransform.localScale = new Vector3(1f, 1f, 1f);
		}
		m_crash = cNKCCutTemplet.m_Crash;
		m_orgPosWithOffset = new Vector2(m_orgPos.x + cNKCCutTemplet.m_CharOffSet.x, m_orgPos.y + cNKCCutTemplet.m_CharOffSet.y);
		if (cNKCCutTemplet.CutUnitAction == CutUnitPosAction.IN || cNKCCutTemplet.CutUnitAction == CutUnitPosAction.MOVE)
		{
			SetUnitMovePos(GetStartPos(), GetTargetPos());
		}
		m_rectTransform.anchoredPosition = m_orgPosWithOffset;
		NKCUICutScenUnitMgr.GetCutScenUnitMgr().DarkenOtherUnitColor(m_NKCCutTemplet.CutUnitPos);
		if (cNKCCutTemplet.m_BounceCount > 0)
		{
			BounceUnit(cNKCCutTemplet.m_BounceCount, cNKCCutTemplet.m_BounceTime);
		}
	}

	public void ClearUnit(NKCCutTemplet cNKCCutTemplet, bool bNone, bool bCur)
	{
		m_NKMTrackingFloatUnitAlpha.StopTracking();
		m_NKMTrackingVector3UnitScale.StopTracking();
		m_rectTransform.localScale = Vector3.one;
		NKCCharacterView.SetColor(1f, 1f, 1f, 1f);
		if (bNone)
		{
			if (m_goUnit.activeSelf)
			{
				m_goUnit.SetActive(value: false);
			}
			if (m_NKCUICharacterView != null)
			{
				m_NKCUICharacterView.CleanupAllEffect();
			}
			FinishUnit();
		}
		else if (bCur)
		{
			m_NKCCutTemplet = cNKCCutTemplet;
			m_bFinished = true;
			m_crash = 0;
			m_rectTransform.anchoredPosition = m_orgPosWithOffset;
			m_rectTransform.localScale = Vector3.one;
			m_canvasGroup.alpha = 1f;
			DoClear(GetStartPos(), GetTargetPos());
		}
	}

	public void SetUnitIllust(NKCCutScenCharTemplet cNKCCutScenCharTemplet, NKCCutTemplet cNKCCutTemplet, Dictionary<string, int> dicSkin)
	{
		NKCUICharacterView nKCUICharacterView = null;
		NKCUICharacterView nKCUICharacterView2 = null;
		int skinID = m_skinID;
		nKCUICharacterView = m_NKCUICharacterView;
		string prefab = m_prefab;
		m_prefab = cNKCCutScenCharTemplet.m_PrefabStr;
		m_skinID = ((dicSkin != null && dicSkin.ContainsKey(m_prefab)) ? dicSkin[m_prefab] : 0);
		m_skinOption = ((cNKCCutScenCharTemplet.m_SkinOption > 0) ? (cNKCCutScenCharTemplet.m_SkinOption - 1) : 0);
		m_skinName = cNKCCutScenCharTemplet.m_SkinName;
		bool flag = false;
		if (m_NKCCutTemplet.CutUnitAction == CutUnitPosAction.DARK)
		{
			flag = true;
		}
		Color startColor = GetStartColor(flag);
		if (prefab != cNKCCutScenCharTemplet.m_PrefabStr || m_skinID != skinID)
		{
			nKCUICharacterView2 = nKCUICharacterView;
			bool flag2 = false;
			if (nKCUICharacterView2 != null && nKCUICharacterView2.HasCharacterIllust())
			{
				nKCUICharacterView2.SetColor(1f, 1f, 1f, 1f, bIncludeEffect: true);
				if (nKCUICharacterView2.IsDiffrentCharacter(cNKCCutScenCharTemplet.m_PrefabStr, m_skinID))
				{
					if (cNKCCutTemplet.m_bCharFadeOut)
					{
						SetUnitFadeOutByTime(1.5f);
					}
					else if (flag)
					{
						SetUnitAlpha(1f);
					}
					else if (cNKCCutTemplet.m_bCharFadeIn)
					{
						SetUnitFadeInByTime(1.5f);
					}
					else
					{
						WhitenUnit();
					}
				}
				flag2 = true;
			}
			if (nKCUICharacterView != null)
			{
				nKCUICharacterView.SetCharacterIllust(cNKCCutScenCharTemplet.m_PrefabStr, m_skinID, cNKCCutScenCharTemplet.m_Background, bVFX: false, bAsync: false, m_skinOption);
				nKCUICharacterView.SetColor(startColor.r, startColor.g, startColor.b, startColor.a, bIncludeEffect: true);
				if (!string.IsNullOrEmpty(m_skinName))
				{
					nKCUICharacterView.SetSkinOption(m_skinName);
				}
			}
			if (!flag2)
			{
				if (cNKCCutTemplet.m_bCharFadeOut)
				{
					SetUnitFadeOutByTime(1.5f);
				}
				else if (flag)
				{
					SetUnitAlpha(1f);
				}
				else if (cNKCCutTemplet.m_bCharFadeIn)
				{
					SetUnitFadeInByTime(1.5f);
				}
				else if (cNKCCutTemplet.CutUnitAction != CutUnitPosAction.MOVE)
				{
					WhitenUnit();
				}
			}
		}
		else
		{
			if (nKCUICharacterView != null)
			{
				nKCUICharacterView.SetColor(startColor.r, startColor.g, startColor.b, startColor.a, bIncludeEffect: true);
				nKCUICharacterView.SetCharacterIllustBackgroundEnable(cNKCCutScenCharTemplet.m_Background);
				if (!string.IsNullOrEmpty(m_skinName))
				{
					nKCUICharacterView.SetSkinOption(m_skinName);
				}
				else
				{
					nKCUICharacterView.SetSkinOption(m_skinOption);
				}
			}
			if (cNKCCutTemplet.m_bCharFadeOut)
			{
				SetUnitFadeOutByTime(1.5f);
			}
		}
		if (!(nKCUICharacterView != null))
		{
			return;
		}
		m_goUnit.transform.SetAsLastSibling();
		if (cNKCCutTemplet.CutUnitAction == CutUnitPosAction.MOVE)
		{
			NKCASUIUnitIllust unitIllust = NKCUICutScenUnitMgr.GetCutScenUnitMgr().GetUnitCharacterView(cNKCCutTemplet.m_StartPosType).GetUnitIllust();
			NKCCutTemplet unitCutTemplet = NKCUICutScenUnitMgr.GetCutScenUnitMgr().GetUnitCutTemplet(cNKCCutTemplet.m_StartPosType);
			if (unitIllust != null && unitCutTemplet != null && string.Equals(unitCutTemplet.m_Face, cNKCCutTemplet.m_Face))
			{
				float currentAnimationTime = unitIllust.GetCurrentAnimationTime();
				nKCUICharacterView.SetAnimationTrackTime(currentAnimationTime);
			}
		}
		if (!string.IsNullOrEmpty(cNKCCutTemplet.m_Face))
		{
			nKCUICharacterView.SetAnimation(cNKCCutTemplet.m_Face, cNKCCutTemplet.m_bFaceLoop);
		}
		if (cNKCCutTemplet.m_bCharHologramEffect)
		{
			nKCUICharacterView.PlayEffect(NKCUICharacterView.EffectType.Hologram);
		}
		nKCUICharacterView.SetPinup(cNKCCutTemplet.m_bCharPinup, cNKCCutTemplet.m_bCharPinupEasingTime);
	}

	public void SetUnitFadeInByTime(float time)
	{
		SetUnitFadeAlpha(time, fadeIn: true);
	}

	public void SetUnitFadeOutByTime(float time)
	{
		SetUnitFadeAlpha(time, fadeIn: false);
	}

	private void SetUnitFadeAlpha(float time, bool fadeIn)
	{
		if (m_NKMTrackingFloatUnitAlpha != null)
		{
			float nowValue = ((!fadeIn) ? 1 : 0);
			float targetVal = (fadeIn ? 1 : 0);
			m_NKMTrackingFloatUnitAlpha.SetNowValue(nowValue);
			m_NKMTrackingFloatUnitAlpha.SetTracking(targetVal, time, TRACKING_DATA_TYPE.TDT_NORMAL);
		}
	}

	public void SetUnitAlpha(float fAlpha)
	{
		m_canvasGroup.alpha = fAlpha;
	}

	public void WhitenUnit()
	{
		if (!(m_NKCUICharacterView == null) && m_NKCUICharacterView.HasCharacterIllust())
		{
			SetUnitColorMultiplier(0f, 1f, 0.2f, TRACKING_DATA_TYPE.TDT_NORMAL);
		}
	}

	public void DarkenUnit()
	{
		if (!(m_NKCUICharacterView == null) && m_NKCUICharacterView.HasCharacterIllust())
		{
			SetUnitColorMultiplier(m_NKCUICharacterView.GetColor().r, 0.25f, 0.15f, TRACKING_DATA_TYPE.TDT_NORMAL);
		}
	}

	public void SetUnitColorMultiplier(float nowValue, float targetValue, float trackingTime, TRACKING_DATA_TYPE trackingDataType)
	{
		m_NKMTrackingFloatUnitColor.SetNowValue(nowValue);
		m_NKMTrackingFloatUnitColor.SetTracking(targetValue, trackingTime, trackingDataType);
	}

	public void SetUnitScaleByTime(Vector2 m_vScaleXY, float fTime)
	{
		if ((m_vScaleXY.x != 0f || m_vScaleXY.y != 0f || fTime != 0f) && m_NKMTrackingVector3UnitScale != null && m_rectTransform != null)
		{
			NKMVector3 nKMVector = new NKMVector3(m_rectTransform.localScale.x, m_rectTransform.localScale.y, 1f);
			Vector2 vector = m_vScaleXY;
			m_NKMTrackingVector3UnitScale.SetNowValue(nKMVector);
			m_NKMTrackingVector3UnitScale.SetTracking(vector, fTime, TRACKING_DATA_TYPE.TDT_NORMAL);
		}
	}

	public void SetUnitMovePos(Vector2 startPos, Vector2 targetPos)
	{
		if (!(startPos == targetPos))
		{
			m_bFinished = false;
			m_NKMTrackingVector2Pos.SetNowValue(startPos);
			m_NKMTrackingVector2Pos.SetTracking(targetPos, m_NKCCutTemplet.m_TrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
		}
	}

	public void DoClear(Vector2 startPos, Vector2 targetPos)
	{
		if (m_NKCCutTemplet.CutUnitAction == CutUnitPosAction.PLACE)
		{
			if (m_goUnit.activeSelf)
			{
				m_goUnit.SetActive(value: false);
			}
		}
		else if (m_NKCCutTemplet.CutUnitAction == CutUnitPosAction.OUT)
		{
			SetUnitMovePos(startPos, targetPos);
			m_bFinished = false;
		}
		if (m_NKCUICharacterView != null)
		{
			m_NKCUICharacterView.CleanupAllEffect();
		}
	}

	public void Close()
	{
		FinishUnit();
		if (m_goUnit.activeSelf)
		{
			m_goUnit.SetActive(value: false);
		}
		if ((bool)m_NKCUICharacterView && m_NKCUICharacterView.HasCharacterIllust())
		{
			m_NKCUICharacterView.SetColor(1f, 1f, 1f, 1f, bIncludeEffect: true);
			m_NKCUICharacterView.CleanUp();
		}
		m_rectTransform.localScale = Vector3.one;
		m_NKCUICharacterView.SetColor(1f, 1f, 1f, 1f);
		m_prefab = "";
	}
}
