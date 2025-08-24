using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionUnitCRFSlot : MonoBehaviour
{
	public TMP_Text m_TypeText;

	public TMP_Text m_ValueText;

	public Slider m_Gauge;

	public NKCUICollectionProfileToolTip m_profileToopTip;

	private Coroutine m_gaugeCoroutine;

	public void Init()
	{
		m_profileToopTip?.Init();
		NKCUtil.SetSliderMinMax(m_Gauge, 0f, 1f);
		NKCUtil.SetSliderValue(m_Gauge, 0f);
	}

	public void SetData(string type, string value, float ratio)
	{
		string msg = NKCStringTable.GetString(type);
		string msg2 = NKCStringTable.GetString(value);
		NKCUtil.SetLabelText(m_TypeText, msg);
		NKCUtil.SetLabelText(m_ValueText, msg2);
		if (m_gaugeCoroutine != null)
		{
			StopCoroutine(m_gaugeCoroutine);
		}
		m_gaugeCoroutine = StartCoroutine(IGaugeUpdate(ratio));
		m_profileToopTip?.SetDescData(type);
	}

	private IEnumerator IGaugeUpdate(float ratio)
	{
		yield return null;
		m_Gauge?.DOValue(ratio, 0.5f);
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}

	public void ResetGauge()
	{
		NKCUtil.SetSliderValue(m_Gauge, 0f);
	}
}
