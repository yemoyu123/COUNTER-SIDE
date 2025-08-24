using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUITypeWriter
{
	private Text m_lbTarget;

	private TextMeshProUGUI m_tmpTarget;

	private float m_fCoolTime;

	private float m_fElapsedTime;

	private int m_strCurrentIndex = 1;

	private bool m_bTyping;

	private NKCTextChunk m_NKCTextChunk = new NKCTextChunk();

	private NKCTextChunk m_NKCTextChunkForName = new NKCTextChunk();

	private StringBuilder m_cStringBuilder = new StringBuilder();

	private string m_FinalText = "";

	private int m_PureWordCount;

	private bool m_bUseTypingSound;

	private bool m_bUseSpaceSound = true;

	private int m_SpaceCount;

	public void SetTypingSound(bool bUse)
	{
		m_bUseTypingSound = bUse;
	}

	public void SetSpaceSound(bool bUse)
	{
		m_bUseSpaceSound = bUse;
	}

	public void Start(Text lbTarget, string nameStr, string goalStr, float fCoolTime, bool _bTalkAppend)
	{
		m_cStringBuilder.Remove(0, m_cStringBuilder.Length);
		if (nameStr != null && nameStr.Length > 0)
		{
			m_cStringBuilder.AppendFormat("<color=#FFFFFFFF>{0}</color>: {1}", nameStr, goalStr);
		}
		else
		{
			m_cStringBuilder.AppendFormat(goalStr);
		}
		m_FinalText = m_cStringBuilder.ToString();
		m_FinalText = NKCUtil.TextSplitLine(m_FinalText, lbTarget);
		m_NKCTextChunk.TextAnalyze(m_FinalText);
		m_NKCTextChunk.ReplaceNameString();
		m_FinalText = ReplaceNameString(m_FinalText, bBlock: false);
		m_PureWordCount = m_NKCTextChunk.GetPureTextCount();
		m_lbTarget = lbTarget;
		m_fCoolTime = fCoolTime;
		if (!_bTalkAppend)
		{
			m_lbTarget.text = "";
		}
		m_bTyping = true;
		if (!_bTalkAppend)
		{
			m_strCurrentIndex = 1;
			if (nameStr != null)
			{
				m_NKCTextChunkForName.TextAnalyze(nameStr);
				m_NKCTextChunkForName.ReplaceNameString();
				m_strCurrentIndex += m_NKCTextChunkForName.GetPureTextCount();
			}
			m_SpaceCount = 0;
		}
		m_fElapsedTime = 0f;
	}

	public void Start(Text lbTarget, string goalStr, float fCoolTime, bool _bTalkAppend)
	{
		m_cStringBuilder.Remove(0, m_cStringBuilder.Length);
		m_cStringBuilder.AppendFormat(goalStr);
		m_FinalText = m_cStringBuilder.ToString();
		m_FinalText = NKCUtil.TextSplitLine(m_FinalText, lbTarget);
		m_NKCTextChunk.TextAnalyze(m_FinalText);
		m_NKCTextChunk.ReplaceNameString();
		m_FinalText = ReplaceNameString(m_FinalText, bBlock: false);
		m_PureWordCount = m_NKCTextChunk.GetPureTextCount();
		m_lbTarget = lbTarget;
		m_fCoolTime = fCoolTime;
		if (!_bTalkAppend)
		{
			m_lbTarget.text = "";
		}
		m_bTyping = true;
		if (!_bTalkAppend)
		{
			m_strCurrentIndex = 1;
			m_SpaceCount = 0;
		}
		m_fElapsedTime = 0f;
	}

	public void Start(TextMeshProUGUI Target, string nameStr, string goalStr, float fCoolTime, bool _bTalkAppend)
	{
		if (!_bTalkAppend)
		{
			m_strCurrentIndex = 1;
			if (nameStr != null)
			{
				string text = ReplaceNameString(nameStr, bBlock: false);
				TMP_TextInfo textInfo = Target.GetTextInfo(text);
				m_strCurrentIndex += textInfo.characterCount;
			}
			m_SpaceCount = 0;
		}
		m_cStringBuilder.Remove(0, m_cStringBuilder.Length);
		if (nameStr != null && nameStr.Length > 0)
		{
			m_cStringBuilder.AppendFormat("<color=#FFFFFFFF>{0}</color>: {1}", nameStr, goalStr);
		}
		else
		{
			m_cStringBuilder.AppendFormat(goalStr);
		}
		m_FinalText = m_cStringBuilder.ToString();
		m_FinalText = NKCUtil.TextSplitLine(m_FinalText, Target);
		m_FinalText = ReplaceNameString(m_FinalText, bBlock: false);
		TMP_TextInfo textInfo2 = Target.GetTextInfo(m_FinalText);
		m_PureWordCount = textInfo2.characterCount;
		m_tmpTarget = Target;
		m_fCoolTime = fCoolTime;
		if (!_bTalkAppend)
		{
			m_tmpTarget.maxVisibleCharacters = 0;
		}
		m_bTyping = true;
		m_fElapsedTime = 0f;
	}

	public void Start(TextMeshProUGUI Target, string goalStr, float fCoolTime, bool _bTalkAppend)
	{
		m_cStringBuilder.Remove(0, m_cStringBuilder.Length);
		m_cStringBuilder.AppendFormat(goalStr);
		m_FinalText = m_cStringBuilder.ToString();
		m_FinalText = NKCUtil.TextSplitLine(m_FinalText, Target);
		m_FinalText = ReplaceNameString(m_FinalText, bBlock: false);
		TMP_TextInfo textInfo = Target.GetTextInfo(m_FinalText);
		m_PureWordCount = textInfo.characterCount;
		m_tmpTarget = Target;
		m_fCoolTime = fCoolTime;
		if (!_bTalkAppend)
		{
			m_tmpTarget.maxVisibleCharacters = 0;
		}
		m_bTyping = true;
		if (!_bTalkAppend)
		{
			m_strCurrentIndex = 1;
			m_SpaceCount = 0;
		}
		m_fElapsedTime = 0f;
	}

	public static string ReplaceNameString(string targetString, bool bBlock)
	{
		if (bBlock)
		{
			targetString = targetString.Replace("<", "@111@");
			targetString = targetString.Replace(">", "@222@");
		}
		else
		{
			targetString = targetString.Replace("@111@", "<");
			targetString = targetString.Replace("@222@", ">");
		}
		return targetString;
	}

	public void Update()
	{
		if (!m_bTyping)
		{
			return;
		}
		if (m_strCurrentIndex <= m_PureWordCount && m_fElapsedTime >= m_fCoolTime)
		{
			if (m_strCurrentIndex <= m_PureWordCount)
			{
				if (m_lbTarget != null)
				{
					m_NKCTextChunk.MakeText(m_strCurrentIndex, ref m_cStringBuilder);
					string text = m_cStringBuilder.ToString();
					m_lbTarget.text = text;
				}
				if (m_tmpTarget != null)
				{
					m_tmpTarget.maxVisibleCharacters = m_strCurrentIndex;
				}
				bool flag = false;
				if (m_bUseTypingSound && !m_bUseSpaceSound)
				{
					int spaceCount = 0;
					if (m_lbTarget != null)
					{
						m_NKCTextChunk.GetSpaceCount(m_strCurrentIndex, ref spaceCount);
					}
					if (m_tmpTarget != null)
					{
						spaceCount = GetTextMeshProSpaceCount(m_strCurrentIndex);
					}
					if (spaceCount != m_SpaceCount)
					{
						flag = true;
					}
					m_SpaceCount = spaceCount;
				}
				m_strCurrentIndex++;
				if (m_bUseTypingSound && !flag)
				{
					NKCSoundManager.PlaySound("TYPING", 1f, 0f, 0f);
				}
			}
			m_fElapsedTime = 0f;
			if (m_strCurrentIndex > m_PureWordCount)
			{
				m_bTyping = false;
				return;
			}
		}
		m_fElapsedTime += Time.deltaTime;
	}

	public void Finish()
	{
		if (m_lbTarget != null)
		{
			m_lbTarget.text = m_FinalText;
		}
		if (m_tmpTarget != null)
		{
			m_tmpTarget.maxVisibleCharacters = m_PureWordCount;
		}
		m_strCurrentIndex = m_PureWordCount;
		m_bTyping = false;
	}

	public bool IsTyping()
	{
		return m_bTyping;
	}

	private int GetTextMeshProSpaceCount(int strCurrentIndex)
	{
		int num = 0;
		if (m_tmpTarget != null)
		{
			TMP_CharacterInfo[] characterInfo = m_tmpTarget.textInfo.characterInfo;
			int num2 = 0;
			int characterCount = m_tmpTarget.textInfo.characterCount;
			while (num2 < characterCount)
			{
				if (characterInfo[num2].character == ' ')
				{
					num++;
				}
				num2++;
				if (num2 == strCurrentIndex)
				{
					break;
				}
			}
		}
		return num;
	}
}
