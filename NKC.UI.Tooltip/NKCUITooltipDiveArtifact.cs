using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltipDiveArtifact : NKCUITooltipBase
{
	public Text m_lbName;

	public Text m_lbDesc;

	public override void Init()
	{
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.DiveArtifactData diveArtifactData))
		{
			Debug.LogError("Tooltip DiveArtifactData is null");
			return;
		}
		NKMDiveArtifactTemplet nKMDiveArtifactTemplet = NKMDiveArtifactTemplet.Find(diveArtifactData.Slot.ID);
		if (nKMDiveArtifactTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbName, nKMDiveArtifactTemplet.ArtifactName_Translated);
			NKCUtil.SetLabelText(m_lbDesc, nKMDiveArtifactTemplet.ArtifactMiscDesc_1_Translated);
		}
	}
}
