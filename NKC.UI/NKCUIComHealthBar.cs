using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

[RequireComponent(typeof(Image))]
public class NKCUIComHealthBar : MonoBehaviour
{
	public Color m_colAllyHealth;

	public Color m_colEnemyHealth;

	public Color m_colBarrierColor;

	public float m_fBorderWidth = 2f;

	public float m_fBorderSmallWidth = 1f;

	public float m_fBigStep = 20000f;

	public float m_fSmallStep = 5000f;

	public float m_fSmallStepSize = 0.5f;

	public int BIG_STEP_THRESHOLD = 10;

	private Image image;

	private RectTransform imageRectTransform;

	private float m_fStepRatio = 1f;

	private Vector3[] buffer = new Vector3[4];

	private float m_HPMax;

	private bool m_bInit;

	protected void Awake()
	{
		Init();
	}

	public void Init()
	{
		if (!m_bInit)
		{
			m_bInit = true;
			image = GetComponent<Image>();
			imageRectTransform = image.GetComponent<RectTransform>();
			image.material = Object.Instantiate(image.material);
			image.material.SetFloat("_BigStep", m_fBigStep);
			image.material.SetFloat("_SmallStep", m_fSmallStep);
			image.material.SetFloat("_SmallStepSize", m_fSmallStepSize);
			image.material.SetColor("_BarrierColor", m_colBarrierColor);
		}
	}

	public void SetColor(bool bEnemy)
	{
		image.material.SetColor("_Color", bEnemy ? m_colEnemyHealth : m_colAllyHealth);
	}

	public void SetStepRatio(float ratio)
	{
		m_fStepRatio = ratio;
		image.material.SetFloat("_BigStep", m_fBigStep * m_fStepRatio);
		image.material.SetFloat("_SmallStep", m_fSmallStep * m_fStepRatio);
	}

	public void SetData(float currentHP, float barrier, float maxHP)
	{
		if (!(image == null))
		{
			m_HPMax = Mathf.Max(currentHP + barrier, maxHP);
			image.material.SetFloat("_Max", m_HPMax);
			image.material.SetFloat("_Percent", (m_HPMax != 0f) ? (currentHP / m_HPMax) : 0f);
			image.material.SetFloat("_BarrierPercent", (m_HPMax != 0f) ? (barrier / m_HPMax) : 0f);
			if ((float)BIG_STEP_THRESHOLD * m_fBigStep * m_fStepRatio < m_HPMax)
			{
				image.material.SetFloat("_SmallStepSize", 0f);
			}
			else
			{
				image.material.SetFloat("_SmallStepSize", m_fSmallStepSize);
			}
		}
	}

	private void Update()
	{
		imageRectTransform.GetWorldCorners(buffer);
		Vector3 vector = NKCCamera.GetCamera().WorldToScreenPoint(buffer[1]);
		float num = Mathf.Abs(NKCCamera.GetCamera().WorldToScreenPoint(buffer[2]).x - vector.x);
		image.material.SetFloat("_hpDotSize", m_HPMax * m_fBorderWidth / num);
		image.material.SetFloat("_hpDotSmallSize", m_HPMax * m_fBorderSmallWidth / num);
	}
}
