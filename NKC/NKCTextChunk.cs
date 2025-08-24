using System.Collections.Generic;
using System.Text;

namespace NKC;

public class NKCTextChunk
{
	public NKC_TEXT_CHUNK_TYPE m_NKC_TEXT_CHUNK_TYPE;

	public string m_Text = "";

	public NKCTextChunk m_ChildTextChunk;

	private Queue<NKCTextChunk> m_qChunkPool = new Queue<NKCTextChunk>();

	private Queue<StringBuilder> m_qStringBuilder = new Queue<StringBuilder>();

	private NKCTextChunk OpenChunk()
	{
		if (m_qChunkPool.Count > 0)
		{
			return m_qChunkPool.Dequeue();
		}
		return new NKCTextChunk();
	}

	private void CloseChunk(NKCTextChunk cNKCTextChunk)
	{
		cNKCTextChunk.Init(this);
		m_qChunkPool.Enqueue(cNKCTextChunk);
	}

	private StringBuilder OpenStringBuilder()
	{
		if (m_qStringBuilder.Count > 0)
		{
			return m_qStringBuilder.Dequeue();
		}
		return new StringBuilder();
	}

	private void CloseStringBuilder(StringBuilder cStringBuilder)
	{
		cStringBuilder.Remove(0, cStringBuilder.Length);
		m_qStringBuilder.Enqueue(cStringBuilder);
	}

	public void Init(NKCTextChunk topChunk = null)
	{
		m_NKC_TEXT_CHUNK_TYPE = NKC_TEXT_CHUNK_TYPE.NTCT_TEXT;
		m_Text = "";
		if (topChunk == null)
		{
			topChunk = this;
		}
		if (m_ChildTextChunk != null)
		{
			topChunk.CloseChunk(m_ChildTextChunk);
			m_ChildTextChunk = null;
		}
	}

	public void TextAnalyze(string fullText)
	{
		Init();
		int textIndex = 0;
		TextAnalyze(this, fullText, ref textIndex);
	}

	private void TextAnalyze(NKCTextChunk topChunk, string fullText, ref int textIndex)
	{
		if (IsTag(fullText, textIndex))
		{
			FindTag(topChunk, fullText, ref textIndex);
			MakeNewChunk(topChunk, fullText, ref textIndex);
			return;
		}
		StringBuilder stringBuilder = topChunk.OpenStringBuilder();
		while (textIndex < fullText.Length)
		{
			if (IsTag(fullText, textIndex))
			{
				m_Text = stringBuilder.ToString();
				MakeNewChunk(topChunk, fullText, ref textIndex);
				topChunk.CloseStringBuilder(stringBuilder);
				return;
			}
			char value = fullText[textIndex];
			textIndex++;
			stringBuilder.Append(value);
		}
		m_Text = stringBuilder.ToString();
		topChunk.CloseStringBuilder(stringBuilder);
	}

	private void MakeNewChunk(NKCTextChunk topChunk, string fullText, ref int textIndex)
	{
		m_ChildTextChunk = topChunk.OpenChunk();
		m_ChildTextChunk.TextAnalyze(topChunk, fullText, ref textIndex);
	}

	private bool IsTag(string fullText, int textIndex)
	{
		if (textIndex >= fullText.Length)
		{
			return false;
		}
		if (fullText[textIndex] == '<')
		{
			if (textIndex + 1 < fullText.Length)
			{
				if (fullText[textIndex + 1] == 'c' || fullText[textIndex + 1] == 'C')
				{
					return true;
				}
				if (fullText[textIndex + 1] == 'b' || fullText[textIndex + 1] == 'B')
				{
					return true;
				}
				if (fullText[textIndex + 1] == '/')
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	private void FindTag(NKCTextChunk topChunk, string fullText, ref int textIndex)
	{
		if (textIndex >= fullText.Length || fullText[textIndex] != '<')
		{
			return;
		}
		if (fullText[textIndex + 1] == 'c' || fullText[textIndex + 1] == 'C')
		{
			GetTag(topChunk, fullText, ref textIndex);
			m_NKC_TEXT_CHUNK_TYPE = NKC_TEXT_CHUNK_TYPE.NTCT_COLOR;
		}
		else if (fullText[textIndex + 1] == 'b' || fullText[textIndex + 1] == 'B')
		{
			GetTag(topChunk, fullText, ref textIndex);
			m_NKC_TEXT_CHUNK_TYPE = NKC_TEXT_CHUNK_TYPE.NTCT_BOLD;
		}
		else if (fullText[textIndex + 1] == '/')
		{
			if (fullText[textIndex + 2] == 'c' || fullText[textIndex + 2] == 'C')
			{
				m_NKC_TEXT_CHUNK_TYPE = NKC_TEXT_CHUNK_TYPE.NTCT_COLOR_END;
			}
			else if (fullText[textIndex + 2] == 'b' || fullText[textIndex + 2] == 'B')
			{
				m_NKC_TEXT_CHUNK_TYPE = NKC_TEXT_CHUNK_TYPE.NTCT_BOLD_END;
			}
			GetTag(topChunk, fullText, ref textIndex);
		}
	}

	private void GetTag(NKCTextChunk topChunk, string fullText, ref int textIndex)
	{
		StringBuilder stringBuilder = topChunk.OpenStringBuilder();
		while (textIndex < fullText.Length)
		{
			char c = fullText[textIndex];
			textIndex++;
			stringBuilder.Append(c);
			if (c == '>')
			{
				break;
			}
		}
		m_Text = stringBuilder.ToString();
		topChunk.CloseStringBuilder(stringBuilder);
	}

	public void GetSpaceCount(int targetIndex, ref int spaceCount)
	{
		int remainIndex = targetIndex;
		GetSpaceCount(ref remainIndex, ref spaceCount);
	}

	private void GetSpaceCount(ref int remainIndex, ref int spaceCount)
	{
		if (m_NKC_TEXT_CHUNK_TYPE == NKC_TEXT_CHUNK_TYPE.NTCT_TEXT)
		{
			int num = 0;
			while (num < m_Text.Length)
			{
				if (m_Text[num] == ' ')
				{
					spaceCount++;
				}
				num++;
				if (remainIndex > 0)
				{
					remainIndex--;
					if (remainIndex == 0)
					{
						return;
					}
				}
			}
		}
		if (m_ChildTextChunk != null)
		{
			m_ChildTextChunk.GetSpaceCount(ref remainIndex, ref spaceCount);
		}
	}

	public void MakeText(int targetIndex, ref StringBuilder cStringBuilder)
	{
		int remainIndex = targetIndex;
		int colorOpenCount = 0;
		int boldOpenCount = 0;
		cStringBuilder.Remove(0, cStringBuilder.Length);
		if (remainIndex == 0)
		{
			cStringBuilder.Append("<color=#00000000>");
			colorOpenCount++;
		}
		MakeText(ref remainIndex, ref cStringBuilder, ref colorOpenCount, ref boldOpenCount);
		for (int i = 0; i < colorOpenCount; i++)
		{
			cStringBuilder.Append("</color>");
		}
		for (int j = 0; j < boldOpenCount; j++)
		{
			cStringBuilder.Append("</b>");
		}
	}

	private void MakeText(ref int remainIndex, ref StringBuilder cStringBuilder, ref int colorOpenCount, ref int boldOpenCount)
	{
		switch (m_NKC_TEXT_CHUNK_TYPE)
		{
		case NKC_TEXT_CHUNK_TYPE.NTCT_TEXT:
		{
			int num = 0;
			while (num < m_Text.Length)
			{
				cStringBuilder.Append(m_Text[num]);
				num++;
				if (remainIndex > 0)
				{
					remainIndex--;
					if (remainIndex == 0)
					{
						cStringBuilder.Append("<color=#00000000>");
						colorOpenCount++;
					}
				}
			}
			break;
		}
		case NKC_TEXT_CHUNK_TYPE.NTCT_COLOR:
			if (remainIndex > 0)
			{
				cStringBuilder.Append(m_Text);
				colorOpenCount++;
			}
			break;
		case NKC_TEXT_CHUNK_TYPE.NTCT_COLOR_END:
			if (remainIndex > 0)
			{
				cStringBuilder.Append(m_Text);
				colorOpenCount--;
			}
			break;
		case NKC_TEXT_CHUNK_TYPE.NTCT_BOLD:
			if (remainIndex > 0)
			{
				cStringBuilder.Append(m_Text);
				boldOpenCount++;
			}
			break;
		case NKC_TEXT_CHUNK_TYPE.NTCT_BOLD_END:
			if (remainIndex > 0)
			{
				cStringBuilder.Append(m_Text);
				boldOpenCount--;
			}
			break;
		}
		if (m_ChildTextChunk != null)
		{
			m_ChildTextChunk.MakeText(ref remainIndex, ref cStringBuilder, ref colorOpenCount, ref boldOpenCount);
		}
	}

	public int GetPureTextCount()
	{
		int textCount = 0;
		GetPureTextCount(ref textCount);
		return textCount;
	}

	public void ReplaceNameString()
	{
		ReplaceNameStringFromChunk(this);
	}

	private void ReplaceNameStringFromChunk(NKCTextChunk textChunk)
	{
		textChunk.m_Text = NKCUITypeWriter.ReplaceNameString(textChunk.m_Text, bBlock: false);
		if (textChunk.m_ChildTextChunk != null)
		{
			ReplaceNameStringFromChunk(textChunk.m_ChildTextChunk);
		}
	}

	private void GetPureTextCount(ref int textCount)
	{
		if (m_NKC_TEXT_CHUNK_TYPE == NKC_TEXT_CHUNK_TYPE.NTCT_TEXT)
		{
			textCount += m_Text.Length;
		}
		if (m_ChildTextChunk != null)
		{
			m_ChildTextChunk.GetPureTextCount(ref textCount);
		}
	}
}
