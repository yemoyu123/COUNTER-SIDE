using System.Text;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCComUITalkBox : MonoBehaviour
{
	private float m_BASE_WIDTH = 100f;

	private float m_BASE_HEIGHT = 100f;

	public float MAX_PREFERRED_WIDTH = 500f;

	public float TEXT_PADDING = 30f;

	public RectTransform m_AB_UI_TALK_BOX_BG_RectTransform;

	public RectTransform m_AB_UI_TALK_BOX_TEXT_RectTransform;

	public Text m_AB_UI_TALK_BOX_TEXT_Text;

	public NKC_UI_TALK_BOX_DIR m_NKC_UI_TALK_BOX_DIR;

	public int m_NextLineCount;

	public float m_fSpreadTime;

	public float m_fadeTime;

	private float m_fSpreadTimeNow;

	private int m_SpreadIndex;

	private string m_TextOrg = "";

	private string m_Text = "";

	private float m_PreferredWidth;

	private float m_PreferredHeight;

	private float m_fadeTimeNow;

	private HorizontalWrapMode m_bHorizontalOverflow = HorizontalWrapMode.Overflow;

	private Vector2 m_OffsetMin;

	private Vector2 m_OffsetMax;

	private float m_delayTime;

	private float m_delayTimeNow;

	private string m_ReservedText = "";

	public void Awake()
	{
		NKCUIComStringChanger component = m_AB_UI_TALK_BOX_TEXT_Text.GetComponent<NKCUIComStringChanger>();
		if (component != null)
		{
			component.Translate();
		}
		m_bHorizontalOverflow = m_AB_UI_TALK_BOX_TEXT_Text.horizontalOverflow;
		if (m_AB_UI_TALK_BOX_TEXT_RectTransform != null)
		{
			m_OffsetMin = m_AB_UI_TALK_BOX_TEXT_RectTransform.offsetMin;
			m_OffsetMax = m_AB_UI_TALK_BOX_TEXT_RectTransform.offsetMax;
		}
		m_TextOrg = m_AB_UI_TALK_BOX_TEXT_Text.text;
		SetText(m_TextOrg, m_fadeTime);
		SetDir(m_NKC_UI_TALK_BOX_DIR);
	}

	public void OnEnable()
	{
		SetText(m_TextOrg, m_fadeTime);
		SetDir(m_NKC_UI_TALK_BOX_DIR);
	}

	public void SetText(string text, float fadeTime = 0f, float delayTime = 0f)
	{
		m_TextOrg = text;
		text = NKCUtil.TextSplitLine(text, m_AB_UI_TALK_BOX_TEXT_Text, 500f);
		if (m_Text.CompareTo(text) != 0)
		{
			if (m_NextLineCount > 0)
			{
				StringBuilder builder = NKMString.GetBuilder();
				int num = 0;
				for (int i = 0; i < text.Length; i++)
				{
					builder.Append(text[i]);
					if (text[i] == '\n')
					{
						num = 0;
						continue;
					}
					num++;
					if (num >= m_NextLineCount)
					{
						builder.Append('\n');
						num = 0;
					}
				}
				m_Text = builder.ToString();
			}
			else
			{
				m_Text = text;
			}
			m_AB_UI_TALK_BOX_TEXT_Text.text = m_Text;
			ReSize();
		}
		if (delayTime > 0f)
		{
			m_delayTime = delayTime;
			m_delayTimeNow = 0f;
		}
		if (m_fSpreadTime > 0f)
		{
			m_fSpreadTimeNow = 0f;
			m_SpreadIndex = 0;
			m_AB_UI_TALK_BOX_TEXT_Text.text = "";
			m_fadeTime = fadeTime;
			m_fadeTimeNow = 0f;
		}
	}

	public void Update()
	{
		if (m_Text.Length == 0)
		{
			return;
		}
		if (m_delayTime > 0f && m_delayTimeNow < m_delayTime)
		{
			m_delayTimeNow += Time.deltaTime;
		}
		else if (m_fSpreadTime > 0f && m_SpreadIndex < m_Text.Length)
		{
			m_fSpreadTimeNow += Time.deltaTime;
			if (m_fSpreadTimeNow >= m_fSpreadTime)
			{
				m_SpreadIndex++;
				StringBuilder builder = NKMString.GetBuilder();
				builder.AppendFormat("{0}<color=00000000>{1}</color>", m_Text.Substring(0, m_SpreadIndex), m_Text.Substring(m_SpreadIndex));
				m_AB_UI_TALK_BOX_TEXT_Text.text = builder.ToString();
				m_fSpreadTimeNow = 0f;
			}
		}
		else if (m_fadeTime > 0f && m_fadeTime > m_fadeTimeNow)
		{
			m_fadeTimeNow += Time.deltaTime;
			if (m_fadeTime <= m_fadeTimeNow)
			{
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			}
		}
	}

	private void OnDisable()
	{
		if (m_fadeTime > 0f)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
	}

	private void ReSize()
	{
		m_PreferredWidth = m_AB_UI_TALK_BOX_TEXT_Text.preferredWidth;
		m_PreferredHeight = m_AB_UI_TALK_BOX_TEXT_Text.preferredHeight;
		if (m_PreferredWidth > MAX_PREFERRED_WIDTH)
		{
			if (m_AB_UI_TALK_BOX_TEXT_RectTransform != null)
			{
				m_AB_UI_TALK_BOX_TEXT_RectTransform.offsetMin = new Vector2(TEXT_PADDING, TEXT_PADDING);
				m_AB_UI_TALK_BOX_TEXT_RectTransform.offsetMax = new Vector2(0f - TEXT_PADDING, 0f);
			}
			m_PreferredHeight += m_PreferredWidth / (MAX_PREFERRED_WIDTH - TEXT_PADDING * 2f) * ((float)m_AB_UI_TALK_BOX_TEXT_Text.fontSize + m_AB_UI_TALK_BOX_TEXT_Text.lineSpacing);
			m_PreferredWidth = MAX_PREFERRED_WIDTH;
			m_AB_UI_TALK_BOX_TEXT_Text.horizontalOverflow = HorizontalWrapMode.Wrap;
		}
		else
		{
			if (m_AB_UI_TALK_BOX_TEXT_RectTransform != null)
			{
				m_AB_UI_TALK_BOX_TEXT_RectTransform.offsetMin = m_OffsetMin;
				m_AB_UI_TALK_BOX_TEXT_RectTransform.offsetMax = m_OffsetMax;
			}
			m_AB_UI_TALK_BOX_TEXT_Text.horizontalOverflow = m_bHorizontalOverflow;
		}
		m_AB_UI_TALK_BOX_BG_RectTransform.SetWidth(m_PreferredWidth + m_BASE_WIDTH);
		m_AB_UI_TALK_BOX_BG_RectTransform.SetHeight(m_PreferredHeight + m_BASE_HEIGHT);
	}

	public void SetDir(NKC_UI_TALK_BOX_DIR eNKC_UI_TALK_BOX_DIR)
	{
		m_NKC_UI_TALK_BOX_DIR = eNKC_UI_TALK_BOX_DIR;
		switch (m_NKC_UI_TALK_BOX_DIR)
		{
		case NKC_UI_TALK_BOX_DIR.NTBD_RIGHT:
			NKCUtil.SetGameObjectLocalScale(m_AB_UI_TALK_BOX_BG_RectTransform, 1f, 1f, 1f);
			NKCUtil.SetGameObjectLocalScale(m_AB_UI_TALK_BOX_TEXT_RectTransform, 1f, 1f, 1f);
			break;
		case NKC_UI_TALK_BOX_DIR.NTBD_LEFT:
			NKCUtil.SetGameObjectLocalScale(m_AB_UI_TALK_BOX_BG_RectTransform, -1f, 1f, 1f);
			NKCUtil.SetGameObjectLocalScale(m_AB_UI_TALK_BOX_TEXT_RectTransform, -1f, 1f, 1f);
			break;
		case NKC_UI_TALK_BOX_DIR.NTBD_RIGHT_DOWN:
			NKCUtil.SetGameObjectLocalScale(m_AB_UI_TALK_BOX_BG_RectTransform, 1f, -1f, 1f);
			NKCUtil.SetGameObjectLocalScale(m_AB_UI_TALK_BOX_TEXT_RectTransform, 1f, -1f, 1f);
			break;
		case NKC_UI_TALK_BOX_DIR.NTBD_LEFT_DOWN:
			NKCUtil.SetGameObjectLocalScale(m_AB_UI_TALK_BOX_BG_RectTransform, -1f, -1f, 1f);
			NKCUtil.SetGameObjectLocalScale(m_AB_UI_TALK_BOX_TEXT_RectTransform, -1f, -1f, 1f);
			break;
		}
	}

	public void ReserveText(string text)
	{
		m_ReservedText = text;
	}

	public void ShowReservedText(float fadeTime = 0f)
	{
		if (!string.IsNullOrEmpty(m_ReservedText))
		{
			SetText(m_ReservedText, fadeTime);
			m_ReservedText = "";
		}
	}
}
