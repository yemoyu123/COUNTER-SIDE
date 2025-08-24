using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUINumberController : MonoBehaviour
{
	public delegate void OnChangeNumber(int changedNumber);

	public NKCUIComButton m_cbtnUp;

	public NKCUIComButton m_cbtnDown;

	public Image m_imgNumber;

	public Text m_lbNumber;

	private List<Sprite> m_lstNumberSprite;

	private OnChangeNumber dOnChangeNumber;

	private int _Number;

	public int Number
	{
		get
		{
			return _Number;
		}
		private set
		{
			SetNumber(value, bInvokeCallback: true);
		}
	}

	public void SetNumber(int value, bool bInvokeCallback = false)
	{
		if (value > 9)
		{
			value = 0;
		}
		if (value < 0)
		{
			value = 9;
		}
		if (value != _Number)
		{
			_Number = value;
			if (m_imgNumber != null && m_lstNumberSprite != null)
			{
				m_imgNumber.sprite = m_lstNumberSprite[_Number];
			}
			if (m_lbNumber != null)
			{
				m_lbNumber.text = value.ToString(CultureInfo.InvariantCulture);
			}
			if (bInvokeCallback && dOnChangeNumber != null)
			{
				dOnChangeNumber(value);
			}
		}
	}

	public void Init(List<Sprite> lstNumberSprite, OnChangeNumber onChangeNumber)
	{
		m_cbtnUp.PointerClick.RemoveAllListeners();
		m_cbtnUp.PointerClick.AddListener(OnBtnUp);
		m_cbtnDown.PointerClick.RemoveAllListeners();
		m_cbtnDown.PointerClick.AddListener(OnBtnDown);
		m_lstNumberSprite = lstNumberSprite;
		dOnChangeNumber = onChangeNumber;
	}

	public void OnBtnUp()
	{
		Number++;
	}

	public void OnBtnDown()
	{
		Number--;
	}
}
