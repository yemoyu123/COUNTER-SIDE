using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIDiveGameArtifactSlot : MonoBehaviour
{
	public NKCUISlot m_NKCUISlot;

	public Text m_lbName;

	public Text m_lbDesc;

	public void InitUI()
	{
		m_NKCUISlot.Init();
	}

	public void SetData(NKMDiveArtifactTemplet cNKMDiveArtifactTemplet)
	{
		if (cNKMDiveArtifactTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: false);
			NKCUtil.SetLabelText(m_lbName, "");
			NKCUtil.SetLabelText(m_lbDesc, "");
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: true);
			m_NKCUISlot.SetDiveArtifactData(NKCUISlot.SlotData.MakeDiveArtifactData(cNKMDiveArtifactTemplet.ArtifactID), bShowName: false, bShowCount: false, bEnableLayoutElement: true, null);
			m_NKCUISlot.SetOnClickAction(default(NKCUISlot.SlotClickType));
			NKCUtil.SetLabelText(m_lbName, cNKMDiveArtifactTemplet.ArtifactName_Translated);
			NKCUtil.SetLabelText(m_lbDesc, cNKMDiveArtifactTemplet.ArtifactMiscDesc_1_Translated);
		}
	}
}
