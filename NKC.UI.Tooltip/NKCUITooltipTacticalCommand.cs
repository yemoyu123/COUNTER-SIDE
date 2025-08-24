using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltipTacticalCommand : NKCUITooltipBase
{
	public Image m_imgIcon;

	public Text m_lbName;

	public override void Init()
	{
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.TacticalCommandData { TacticalCommandTemplet: var tacticalCommandTemplet }))
		{
			Debug.LogError("Tooltip TacticalCommandData is null");
			return;
		}
		NKCUtil.SetLabelText(m_lbName, tacticalCommandTemplet.GetTCName());
		m_imgIcon.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_TACTICAL_COMMAND_ICON", tacticalCommandTemplet.m_TCIconName);
	}
}
