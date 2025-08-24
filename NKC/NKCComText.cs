using UnityEngine.UI;

namespace NKC;

public class NKCComText : Text
{
	public string m_StringKey;

	public bool m_HideTextOutOfRect;

	public bool m_IgnoreNewlineChar;

	public override string text
	{
		get
		{
			return base.text;
		}
		set
		{
			base.text = value;
			if (m_HideTextOutOfRect)
			{
				base.text = NKCUtil.LabelLongTextCut(this);
			}
			if (m_IgnoreNewlineChar)
			{
				base.text = NKCUtil.RemoveLabelCharText(text, "\n");
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (!string.IsNullOrWhiteSpace(m_StringKey))
		{
			text = NKCStringTable.GetString(m_StringKey);
		}
		else if (!string.IsNullOrWhiteSpace(text) && m_HideTextOutOfRect)
		{
			base.text = NKCUtil.LabelLongTextCut(this);
		}
	}
}
