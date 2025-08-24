using NKC.UI.Component;
using UnityEngine;

namespace NKC.UI.Tooltip;

public class NKCUITooltipText : NKCUITooltipBase
{
	public NKCComTMPUIText m_desc;

	public override void Init()
	{
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.TextData textData))
		{
			Debug.LogError("Tooltip textData is null");
		}
		else
		{
			m_desc.text = textData.Text;
		}
	}
}
