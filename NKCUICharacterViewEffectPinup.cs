using DG.Tweening;
using NKC;
using UnityEngine;
using UnityEngine.UI;

public class NKCUICharacterViewEffectPinup : NKCUICharacterViewEffectBase
{
	private Mask m_rRectMask;

	private RectTransform m_rRectTransform;

	private Image m_PipupMask;

	private Image m_PipupBackGround;

	private const float m_fPipupActiveValueX = 300f;

	private const float m_fPipupActiveValueY = 3000f;

	private Sequence m_StartSequence;

	private Sequence m_EndSequence;

	private void Awake()
	{
		m_rRectMask = GetComponent<Mask>();
		Transform transform = base.transform.Find("PinupBackGround");
		if (transform != null)
		{
			m_PipupBackGround = transform.GetComponent<Image>();
		}
		m_PipupMask = base.transform.GetComponent<Image>();
		if (m_rRectMask != null)
		{
			m_rRectTransform = m_rRectMask.rectTransform;
		}
		SetActivePinupObject(bActive: false);
		m_StartSequence = DOTween.Sequence();
		m_EndSequence = DOTween.Sequence();
	}

	public override void SetEffect(NKCASUIUnitIllust unitIllust, Transform trOriginalRoot)
	{
		_ = m_rRectMask == null;
	}

	public override void CleanUp(NKCASUIUnitIllust unitIllust, Transform trOriginalRoot)
	{
		if (!(m_rRectMask == null))
		{
			SetActivePinupObject(bActive: false);
		}
	}

	public override void SetColor(Color color)
	{
	}

	public override void SetColor(float fR = -1f, float fG = -1f, float fB = -1f, float fA = -1f)
	{
	}

	public void StartPinupEffect(float fEasingTime)
	{
		if (!(m_rRectTransform == null) && !(m_rRectTransform == null) && !m_StartSequence.IsPlaying())
		{
			SetActivePinupObject(bActive: true);
			m_StartSequence.Kill();
			m_StartSequence.Append(m_rRectTransform.DOSizeDelta(new Vector3(300f, 3000f), fEasingTime).SetEase(Ease.InOutQuart));
		}
	}

	public void ClosePinupEffect(float fEasingTime)
	{
		if (!(m_rRectTransform == null) && !(m_rRectTransform == null))
		{
			SetActivePinupObject(bActive: true);
			m_EndSequence.Kill();
			m_EndSequence.Append(m_rRectTransform.DOSizeDelta(new Vector3(0f, 3000f), fEasingTime).SetEase(Ease.InOutQuart));
		}
	}

	public void SetDeActive()
	{
		SetActivePinupObject(bActive: false);
	}

	public RectTransform GetRectTransform()
	{
		return m_rRectTransform;
	}

	private void SetActivePinupObject(bool bActive)
	{
		if (m_PipupBackGround != null)
		{
			m_PipupBackGround.gameObject.SetActive(bActive);
		}
		if (m_PipupMask != null)
		{
			m_PipupMask.enabled = bActive;
		}
		if (m_rRectMask != null)
		{
			m_rRectMask.enabled = bActive;
		}
		if (!bActive && m_rRectTransform != null)
		{
			m_rRectTransform.DOKill();
			m_rRectTransform.SetSize(new Vector2(0f, 3000f));
		}
	}
}
