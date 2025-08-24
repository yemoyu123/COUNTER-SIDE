using DG.Tweening;
using NKM.Event;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventBarBubble : MonoBehaviour
{
	public delegate void OnTouchBubble();

	public Animator m_aniBubble;

	public CanvasGroup m_canvasGroup;

	public Image m_imgCocktail;

	public Text m_lbDeliveryCount;

	public NKCUIComStateButton m_csbtnButton;

	public float m_hideTime;

	public float m_moveYValue;

	public float m_moveYDuration;

	public float m_rewindAniTime;

	private Vector3 m_position;

	private float m_timer;

	private Tween m_tween;

	private bool m_hideStart;

	private OnTouchBubble m_onTouchBubble;

	public void Init()
	{
		m_position = base.transform.position;
		NKCUtil.SetButtonClickDelegate(m_csbtnButton, OnClickBubble);
	}

	public void Show(int cocktailID, bool hideStart, OnTouchBubble onTouchBubble)
	{
		m_timer = 0f;
		if (m_tween != null)
		{
			m_tween.Kill();
			m_tween = null;
		}
		base.transform.position = m_position;
		float endValue = m_position.y + m_moveYValue;
		m_tween = base.transform.DOMoveY(endValue, m_moveYDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
		Sprite orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(cocktailID);
		NKCUtil.SetImageSprite(m_imgCocktail, orLoadMiscItemIcon, bDisableIfSpriteNull: true);
		int num = NKMEventBarTemplet.Find(cocktailID)?.DeliveryValue ?? 0;
		NKCUtil.SetLabelText(m_lbDeliveryCount, $"x{num}");
		base.gameObject.SetActive(value: true);
		m_csbtnButton?.SetLock(hideStart);
		m_onTouchBubble = onTouchBubble;
		m_hideStart = hideStart;
		NKCUtil.SetGameobjectActive(m_canvasGroup?.gameObject, !m_hideStart);
	}

	public void Hide()
	{
		if (m_tween != null)
		{
			m_tween.Kill();
			m_tween = null;
		}
		base.gameObject.SetActive(value: false);
	}

	public void ResetAnimation()
	{
		if (base.gameObject.activeSelf && !(m_aniBubble == null) && !(m_csbtnButton == null) && m_csbtnButton.m_bLock)
		{
			m_aniBubble.Rebind();
			m_aniBubble.Update(0f);
			m_timer = 0f;
			m_csbtnButton.SetLock(value: false);
			m_hideStart = false;
			NKCUtil.SetGameobjectActive(m_canvasGroup?.gameObject, bValue: true);
		}
	}

	private void Update()
	{
		if (m_aniBubble == null)
		{
			return;
		}
		if (m_hideStart)
		{
			m_timer += Time.deltaTime;
			if (m_timer >= m_hideTime)
			{
				NKCUtil.SetGameobjectActive(m_canvasGroup?.gameObject, bValue: true);
				ResetAnimation();
			}
		}
		else if (m_aniBubble.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
		{
			m_csbtnButton?.SetLock(value: true);
			m_timer += Time.deltaTime;
			if (m_timer >= m_hideTime)
			{
				ResetAnimation();
			}
		}
	}

	private void OnClickBubble()
	{
		if (m_onTouchBubble != null)
		{
			m_onTouchBubble();
		}
		if (!(m_aniBubble == null) && m_rewindAniTime < m_aniBubble.GetCurrentAnimatorStateInfo(0).normalizedTime)
		{
			m_aniBubble.Rebind();
			m_aniBubble.Play("INTRO", 0, m_rewindAniTime);
			m_timer = 0f;
		}
	}
}
