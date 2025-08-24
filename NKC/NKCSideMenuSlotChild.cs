using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCSideMenuSlotChild : MonoBehaviour
{
	public delegate void OnClicked(string key);

	public Text m_TEXT;

	public GameObject m_SELECTED;

	public GameObject m_LOCK;

	public GameObject m_PROGRESS;

	public GameObject m_COMPLETE;

	public NKCUIComStateButton m_BUTTON_CONTENT;

	public NKCUIComToggle m_Toggle;

	public LayoutElement m_LayoutElement;

	public float fActiveSpeed = 10f;

	private float m_fMaxHeight;

	private string m_strKey;

	private OnClicked dOnClicked;

	public string KEY => m_strKey;

	public void Init(string title, string key, RectTransform rtParent, OnClicked clicked)
	{
		base.gameObject.GetComponent<RectTransform>().SetParent(rtParent);
		NKCUtil.SetLabelText(m_TEXT, title);
		if (m_LayoutElement != null)
		{
			m_fMaxHeight = m_LayoutElement.preferredHeight;
			m_LayoutElement.preferredHeight = 0f;
		}
		m_strKey = key;
		dOnClicked = clicked;
		NKCUtil.SetGameobjectActive(m_SELECTED, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_Toggle != null)
		{
			m_Toggle.OnValueChanged.RemoveAllListeners();
			m_Toggle.OnValueChanged.AddListener(OnValuechange);
		}
	}

	private void OnValuechange(bool bActive)
	{
		if (bActive && dOnClicked != null)
		{
			dOnClicked(m_strKey);
		}
	}

	public void OnActive(bool Active)
	{
		Color color = NKCUtil.GetColor("#FFFFFF");
		if (!Active)
		{
			color.a = 0.6f;
		}
		NKCUtil.SetLabelTextColor(m_TEXT, color);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		StopAllCoroutines();
		StartCoroutine(ActiveButton(Active));
	}

	public void OnBtnClick()
	{
		if (dOnClicked != null)
		{
			dOnClicked(m_strKey);
		}
	}

	public bool OnSelected(string ARTICLE_ID)
	{
		NKCUtil.SetGameobjectActive(m_SELECTED, string.Equals(ARTICLE_ID, m_strKey));
		return string.Equals(ARTICLE_ID, m_strKey);
	}

	private IEnumerator ActiveButton(bool bActive)
	{
		if (bActive)
		{
			while (m_LayoutElement.preferredHeight < m_fMaxHeight)
			{
				m_LayoutElement.preferredHeight += fActiveSpeed;
				yield return new WaitForEndOfFrame();
			}
		}
		else
		{
			while (m_LayoutElement.preferredHeight > 0f)
			{
				m_LayoutElement.preferredHeight -= fActiveSpeed;
				yield return new WaitForEndOfFrame();
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
		m_LayoutElement.preferredHeight = Mathf.Clamp(m_LayoutElement.preferredHeight, 0f, m_fMaxHeight);
		yield return null;
	}

	public void SetProgressState(int storyCount, int clearCount)
	{
		NKCUtil.SetGameobjectActive(m_PROGRESS, storyCount > 0 && clearCount > 0 && storyCount > clearCount);
		NKCUtil.SetGameobjectActive(m_COMPLETE, storyCount > 0 && storyCount == clearCount);
	}
}
