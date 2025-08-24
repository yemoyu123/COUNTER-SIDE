using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guide;

public class NKCUIPopupGuideSubSlot : MonoBehaviour
{
	public delegate void OnClicked(string ArticleID, int idx = 0);

	public Text m_TEXT;

	public GameObject m_DOT;

	public GameObject m_SELECTED;

	public NKCUIComStateButton m_BUTTON_CONTENT;

	public LayoutElement m_LayoutElement;

	public float fActiveSpeed = 10f;

	private float m_fMaxHeight;

	private string m_articleID;

	private OnClicked dOnClicked;

	public string ARTICLE_ID => m_articleID;

	public void Init(string title, string articleID, OnClicked clicked)
	{
		NKCUtil.SetLabelText(m_TEXT, title);
		if (m_LayoutElement != null)
		{
			m_fMaxHeight = m_LayoutElement.preferredHeight;
			m_LayoutElement.preferredHeight = 0f;
		}
		m_articleID = articleID;
		dOnClicked = clicked;
		NKCUtil.SetGameobjectActive(m_SELECTED, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetBindFunction(m_BUTTON_CONTENT, OnBtnClick);
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
			dOnClicked(m_articleID);
		}
	}

	public bool OnSelected(string ARTICLE_ID)
	{
		bool flag = string.Equals(ARTICLE_ID, m_articleID);
		m_BUTTON_CONTENT.Select(flag, bForce: true);
		return flag;
	}

	public void OnSelectedObject(string ARTICLE_ID)
	{
		NKCUtil.SetGameobjectActive(m_SELECTED, string.Equals(ARTICLE_ID, m_articleID));
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
}
