using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComQuantityCounter : MonoBehaviour
{
	public delegate int CalcMaxCount();

	public delegate void ClickPlus();

	public delegate void ClickMins();

	public NKCUIComStateButton m_csbtnPlus;

	public NKCUIComStateButton m_csbtnMinus;

	public NKCUIComStateButton m_csbtnMax;

	public Text m_lbCount;

	private int m_currentCount;

	private int m_consumeCount;

	private int m_ownCount;

	private int m_maxCount;

	private CalcMaxCount m_dCalcMaxCount;

	private ClickPlus m_dOnClickPlus;

	private ClickMins m_dOnClickMinus;

	public int CurrentCount => m_currentCount;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnPlus, OnClickPlus);
		NKCUtil.SetButtonClickDelegate(m_csbtnMinus, OnClickMinus);
		NKCUtil.SetButtonClickDelegate(m_csbtnMax, OnClickMax);
	}

	public void SetCountData(int consumeCount, int ownCount, CalcMaxCount dCalcMaxCount = null, ClickPlus dOnClickPlus = null, ClickMins dOnClickMinus = null)
	{
		m_consumeCount = consumeCount;
		m_ownCount = ownCount;
		m_dCalcMaxCount = dCalcMaxCount;
		m_dOnClickPlus = dOnClickPlus;
		m_dOnClickMinus = dOnClickMinus;
		if (m_dCalcMaxCount != null)
		{
			m_maxCount = m_dCalcMaxCount();
		}
		else
		{
			m_maxCount = ownCount / consumeCount;
		}
		m_currentCount = Mathf.Min(1, m_maxCount);
		if (m_maxCount <= 0)
		{
			m_lbCount.color = Color.red;
		}
		else
		{
			m_lbCount.color = Color.white;
		}
		UpdateCountText();
	}

	public void UpdateOwnCount(int ownCount)
	{
		m_ownCount = ownCount;
		if (m_dCalcMaxCount != null)
		{
			m_maxCount = m_dCalcMaxCount();
		}
		else
		{
			m_maxCount = ownCount / m_consumeCount;
		}
		if (m_maxCount <= 0)
		{
			m_lbCount.color = Color.red;
		}
		else
		{
			m_lbCount.color = Color.white;
		}
		if (m_currentCount > m_maxCount)
		{
			m_currentCount = m_maxCount;
		}
		UpdateCountText();
	}

	public void Release()
	{
		m_dCalcMaxCount = null;
		m_dOnClickPlus = null;
		m_dOnClickMinus = null;
	}

	private void UpdateCountText()
	{
		int num = Mathf.Max(1, m_currentCount);
		NKCUtil.SetLabelText(m_lbCount, num.ToString());
	}

	private void OnClickPlus()
	{
		m_currentCount++;
		if (m_currentCount > m_maxCount)
		{
			m_currentCount = m_maxCount;
		}
		UpdateCountText();
		if (m_dOnClickPlus != null)
		{
			m_dOnClickPlus();
		}
	}

	private void OnClickMinus()
	{
		m_currentCount--;
		if (m_currentCount <= 0)
		{
			m_currentCount = m_maxCount;
		}
		UpdateCountText();
		if (m_dOnClickMinus != null)
		{
			m_dOnClickMinus();
		}
	}

	private void OnClickMax()
	{
		m_currentCount = m_maxCount;
		UpdateCountText();
	}
}
