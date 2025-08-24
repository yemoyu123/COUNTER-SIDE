using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupDiveArtifactGetSlot : MonoBehaviour
{
	public NKCUISlot m_NKCUISlot;

	public Text m_lbArtifactName;

	public Text m_lbArtifactDesc;

	public NKCUIComStateButton m_csbtnSelect;

	public GameObject m_objNormal;

	public GameObject m_objDisabled;

	public GameObject m_objNoMoreExistArtifact;

	public GameObject m_objReturnItem;

	public Text m_lbReturnItemCount;

	public Image m_imgReturnItemIcon;

	private int m_Index = -1;

	public void InitUI(int index)
	{
		m_Index = index;
		NKCUtil.SetGameobjectActive(m_objDisabled, bValue: false);
		m_csbtnSelect.PointerClick.RemoveAllListeners();
		m_csbtnSelect.PointerClick.AddListener(OnClickSelect);
		m_NKCUISlot.SetUseBigImg(bSet: true);
	}

	public void SetData(NKMDiveArtifactTemplet cNKMDiveArtifactTemplet)
	{
		NKCUtil.SetGameobjectActive(m_objDisabled, bValue: false);
		if (cNKMDiveArtifactTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_objNoMoreExistArtifact, bValue: true);
			NKCUtil.SetGameobjectActive(m_objNormal, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objNoMoreExistArtifact, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNormal, bValue: true);
		NKCUtil.SetLabelText(m_lbArtifactName, cNKMDiveArtifactTemplet.ArtifactName_Translated);
		NKCUtil.SetLabelText(m_lbArtifactDesc, cNKMDiveArtifactTemplet.ArtifactMiscDesc_1_Translated);
		m_NKCUISlot.SetData(NKCUISlot.SlotData.MakeDiveArtifactData(cNKMDiveArtifactTemplet.ArtifactID), bShowName: false, bShowNumber: false, bEnableLayoutElement: true, null);
		NKCUtil.SetGameobjectActive(m_objReturnItem, cNKMDiveArtifactTemplet.RewardId > 0);
		NKCUtil.SetLabelText(m_lbReturnItemCount, cNKMDiveArtifactTemplet.RewardQuantity.ToString("#,##0"));
		Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(cNKMDiveArtifactTemplet.RewardId);
		NKCUtil.SetImageSprite(m_imgReturnItemIcon, orLoadMiscItemSmallIcon, bDisableIfSpriteNull: true);
	}

	public void OnClickSelect()
	{
		if (!(m_NKCUISlot == null) && m_NKCUISlot.GetSlotData() != null)
		{
			NKCPacketSender.Send_NKMPacket_DIVE_SELECT_ARTIFACT_REQ(m_NKCUISlot.GetSlotData().ID);
			NKC_SCEN_DIVE nKC_SCEN_DIVE = NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE();
			nKC_SCEN_DIVE.GetDiveGame().SetLastSelectedArtifactSlotIndex(m_Index);
			if (nKC_SCEN_DIVE.GetDiveGame().IsOpenNKCPopupDiveArtifactGet)
			{
				nKC_SCEN_DIVE.GetDiveGame().NKCPopupDiveArtifactGet.InvalidAuto();
			}
		}
	}
}
