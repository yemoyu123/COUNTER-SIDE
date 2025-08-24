using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionMenuSubSlot : MonoBehaviour, ICollectionMenuButton
{
	public delegate void OnClicked();

	public delegate void OnRedDotChanged();

	public Text m_NameOn;

	public Text m_NameOff;

	public LayoutElement m_LayoutElement;

	public NKCUIComToggle m_Toggle;

	public GameObject m_RedDot;

	public float fActiveSpeed = 10f;

	private float m_fMaxHeight;

	private OnClicked m_dOnClicked;

	private OnRedDotChanged m_dOnRedDotChanged;

	public bool IsRedDotOn
	{
		get
		{
			if (m_RedDot != null)
			{
				return m_RedDot.activeSelf;
			}
			return false;
		}
	}

	public void Init(string name)
	{
		if (m_LayoutElement != null)
		{
			m_fMaxHeight = m_LayoutElement.preferredHeight;
			m_LayoutElement.preferredHeight = 0f;
		}
		NKCUtil.SetLabelText(m_NameOn, name);
		NKCUtil.SetLabelText(m_NameOff, name);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetToggleValueChangedDelegate(m_Toggle, OnValuechange);
		NKCUtil.SetGameobjectActive(m_RedDot, bValue: false);
	}

	public void SetOnClickFunc(OnClicked dOnClicked)
	{
		m_dOnClicked = dOnClicked;
	}

	public void SetOnRedDotChangedFunc(OnRedDotChanged dOnRedDotChanged)
	{
		m_dOnRedDotChanged = dOnRedDotChanged;
	}

	public void SetRedDotActive(bool value)
	{
		NKCUtil.SetGameobjectActive(m_RedDot, value);
		if (m_dOnRedDotChanged != null)
		{
			m_dOnRedDotChanged();
		}
	}

	public NKCUIComToggle GetToggle()
	{
		return m_Toggle;
	}

	public void SetActive(bool value, bool skipAni)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		StopAllCoroutines();
		StartCoroutine(ActiveButton(value, skipAni));
	}

	private IEnumerator ActiveButton(bool bActive, bool skipAni)
	{
		if (bActive)
		{
			if (skipAni)
			{
				m_LayoutElement.preferredHeight = m_fMaxHeight;
			}
			else
			{
				while (m_LayoutElement.preferredHeight < m_fMaxHeight)
				{
					m_LayoutElement.preferredHeight += fActiveSpeed;
					yield return new WaitForEndOfFrame();
				}
			}
		}
		else
		{
			if (skipAni)
			{
				m_LayoutElement.preferredHeight = 0f;
			}
			else
			{
				while (m_LayoutElement.preferredHeight > 0f)
				{
					m_LayoutElement.preferredHeight -= fActiveSpeed;
					yield return new WaitForEndOfFrame();
				}
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
		m_LayoutElement.preferredHeight = Mathf.Clamp(m_LayoutElement.preferredHeight, 0f, m_fMaxHeight);
		yield return null;
	}

	private void OnValuechange(bool bActive)
	{
		if (bActive && m_dOnClicked != null)
		{
			m_dOnClicked();
		}
	}
}
