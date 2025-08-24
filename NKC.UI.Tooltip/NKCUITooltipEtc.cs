using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltipEtc : NKCUITooltipBase
{
	public Text m_lbName;

	public Text m_lbDesc;

	public override void Init()
	{
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.EtcData etcData))
		{
			Debug.LogError("Tooltip EtcData is null");
			return;
		}
		NKCUtil.SetLabelText(m_lbName, etcData.m_Title);
		NKCUtil.SetLabelText(m_lbDesc, etcData.m_Desc);
	}

	public override void SetData(string title, string desc)
	{
		NKCUtil.SetLabelText(m_lbName, title);
		NKCUtil.SetLabelText(m_lbDesc, desc);
	}
}
