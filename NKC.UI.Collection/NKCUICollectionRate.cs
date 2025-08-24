using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionRate : MonoBehaviour
{
	[Header("사원 수집률")]
	public Image m_Img_NKM_UI_COLLECTION_RATE_GAUGE_BAR;

	public Text m_txt_NKM_UI_COLLECTION_RATE_TEXT_COLLECTION_RATE;

	public Text m_txt_NKM_UI_COLLECTION_RATE_TEXT_PERCENT;

	public Text m_NKM_UI_COLLECTION_RATE_TEXT_COUNT;

	public Transform m_NKM_UI_COLLECTION_RATE_GAUGE_BG_01;

	public Transform m_NKM_UI_COLLECTION_RATE_GAUGE_HANDLE;

	public TMP_Text m_COLLECTION_RATE_TEXT_TITLE;

	public TMP_Text m_COLLECTION_RATE_TEXT_PERCENT;

	public TMP_Text m_COLLECTION_RATE_TEXT_COUNT;

	private int m_preAngle;

	public void Init()
	{
		if (null != m_Img_NKM_UI_COLLECTION_RATE_GAUGE_BAR)
		{
			m_Img_NKM_UI_COLLECTION_RATE_GAUGE_BAR.fillAmount = 0f;
		}
		NKCUtil.SetLabelText(m_txt_NKM_UI_COLLECTION_RATE_TEXT_PERCENT, "0");
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_RATE_TEXT_COUNT, "0/0");
	}

	public void SetData(NKCUICollectionGeneral.CollectionType type, int iCur, int iTotal)
	{
		string collectionRateStrByType = NKCUtilString.GetCollectionRateStrByType(type);
		NKCUtil.SetLabelText(m_txt_NKM_UI_COLLECTION_RATE_TEXT_COLLECTION_RATE, collectionRateStrByType);
		NKCUtil.SetLabelText(m_COLLECTION_RATE_TEXT_TITLE, collectionRateStrByType);
		float value = Mathf.Floor(iCur) / Mathf.Floor(iTotal);
		value = Mathf.Clamp(value, 0f, 1f);
		if (null != m_Img_NKM_UI_COLLECTION_RATE_GAUGE_BAR)
		{
			m_Img_NKM_UI_COLLECTION_RATE_GAUGE_BAR.fillAmount = value;
		}
		string msg = Mathf.FloorToInt(value * 100f) + "%";
		NKCUtil.SetLabelText(m_txt_NKM_UI_COLLECTION_RATE_TEXT_PERCENT, msg);
		NKCUtil.SetLabelText(m_COLLECTION_RATE_TEXT_PERCENT, msg);
		string msg2 = $"{iCur}/{iTotal}";
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_RATE_TEXT_COUNT, msg2);
		NKCUtil.SetLabelText(m_COLLECTION_RATE_TEXT_COUNT, msg2);
		if (m_NKM_UI_COLLECTION_RATE_GAUGE_BG_01 != null)
		{
			int num = Mathf.FloorToInt(value * 360f);
			int num2 = num - m_preAngle;
			m_preAngle = num;
			m_NKM_UI_COLLECTION_RATE_GAUGE_HANDLE?.RotateAround(m_NKM_UI_COLLECTION_RATE_GAUGE_BG_01.position, Vector3.back, num2);
		}
	}
}
