using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUICharacterViewEffectHologram : NKCUICharacterViewEffectBase
{
	private NKCAssetInstanceData m_asHologramEffect;

	private GameObject m_objRectMask;

	private RectMask2D m_RectMask;

	private CanvasGroup canvasGroup;

	private const float HOLOGRAM_MASK_SIZE_X = 750f;

	private const float HOLOGRAM_MASK_SIZE_Y = 3000f;

	private const float HOLOGRAM_OPEN_CLOSE_TIME = 0.33f;

	public override Color EffectColor { get; } = new Color(0.1058f, 0.6f, 1f, 1f);

	public override void SetEffect(NKCASUIUnitIllust unitIllust, Transform trOriginalRoot)
	{
		if (m_objRectMask == null)
		{
			m_objRectMask = new GameObject("Hologram RectMask", typeof(RectTransform), typeof(RectMask2D));
			m_RectMask = m_objRectMask.GetComponent<RectMask2D>();
		}
		if (m_asHologramEffect == null)
		{
			m_asHologramEffect = NKCAssetResourceManager.OpenInstance<GameObject>("ab_fx_ui_hologram", "AB_FX_UI_HOLOGRAM");
		}
		RectTransform component = m_objRectMask.GetComponent<RectTransform>();
		component.SetParent(trOriginalRoot);
		component.anchoredPosition = Vector2.zero;
		unitIllust.SetParent(component, worldPositionStays: true);
		unitIllust.SetColor(EffectColor);
		unitIllust.SetEffectMaterial(NKCUICharacterView.EffectType.Hologram);
		if (m_asHologramEffect != null && m_asHologramEffect.m_Instant != null)
		{
			m_asHologramEffect.m_Instant.transform.SetParent(trOriginalRoot, worldPositionStays: false);
			canvasGroup = m_asHologramEffect.m_Instant.GetComponent<CanvasGroup>();
		}
		HologramIn();
	}

	public void HologramIn()
	{
		RectTransform component = m_objRectMask.GetComponent<RectTransform>();
		component.SetSize(new Vector2(0f, 3000f));
		m_RectMask.enabled = true;
		if (m_asHologramEffect != null && m_asHologramEffect.m_Instant != null)
		{
			m_asHologramEffect.m_Instant.SetActive(value: true);
			Animator component2 = m_asHologramEffect.m_Instant.GetComponent<Animator>();
			if (component2 != null)
			{
				component2.SetTrigger("Start");
				component2.ResetTrigger("Finish");
			}
			m_asHologramEffect.m_Instant.transform.localPosition = Vector3.zero;
		}
		component.DOKill();
		component.DOSizeDelta(new Vector2(750f, 3000f), 0.33f).OnComplete(delegate
		{
			m_RectMask.enabled = false;
		});
	}

	public void HologramOut()
	{
		if (m_asHologramEffect != null && m_asHologramEffect != null && m_asHologramEffect.m_Instant != null)
		{
			Animator component = m_asHologramEffect.m_Instant.GetComponent<Animator>();
			if (component != null)
			{
				component.ResetTrigger("Start");
				component.SetTrigger("Finish");
			}
		}
		RectTransform component2 = m_objRectMask.GetComponent<RectTransform>();
		m_RectMask.enabled = true;
		component2.DOKill();
		component2.DOSizeDelta(new Vector2(0f, 3000f), 0.33f).OnComplete(delegate
		{
			if (m_asHologramEffect != null && m_asHologramEffect.m_Instant != null)
			{
				m_asHologramEffect.m_Instant.SetActive(value: false);
			}
		});
	}

	public override void CleanUp(NKCASUIUnitIllust unitIllust, Transform trOriginalRoot)
	{
		if (unitIllust != null)
		{
			unitIllust.SetColor(Color.white);
			unitIllust.SetParent(trOriginalRoot, worldPositionStays: true);
			unitIllust.SetDefaultMaterial();
		}
		if (m_asHologramEffect != null)
		{
			m_asHologramEffect.Close();
			m_asHologramEffect.Unload();
		}
		m_objRectMask.GetComponent<RectTransform>().DOKill();
		Object.Destroy(m_objRectMask);
	}

	public override void SetColor(Color color)
	{
		if (canvasGroup != null)
		{
			canvasGroup.alpha = color.a;
		}
	}

	public override void SetColor(float fR = -1f, float fG = -1f, float fB = -1f, float fA = -1f)
	{
		if (canvasGroup != null && fA >= 0f)
		{
			canvasGroup.alpha = fA;
		}
	}
}
