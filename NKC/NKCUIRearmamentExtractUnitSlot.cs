using NKM;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIRearmamentExtractUnitSlot : MonoBehaviour
{
	public GameObject m_objOn;

	public GameObject m_ObjOff;

	public GameObject m_objContractUnit;

	public Image m_imgFace;

	public Image m_imgBG;

	public Image m_imgClass;

	public void SetData(NKMUnitData unitData)
	{
		if (unitData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
			switch (unitTempletBase.m_NKM_UNIT_GRADE)
			{
			case NKM_UNIT_GRADE.NUG_SSR:
				NKCUtil.SetImageSprite(m_imgBG, NKCUtil.GetGuildArtifactBgProbImage(GuildDungeonArtifactTemplet.ArtifactProbType.HIGH));
				break;
			case NKM_UNIT_GRADE.NUG_SR:
				NKCUtil.SetImageSprite(m_imgBG, NKCUtil.GetGuildArtifactBgProbImage(GuildDungeonArtifactTemplet.ArtifactProbType.MIDDLE));
				break;
			default:
				NKCUtil.SetImageSprite(m_imgBG, null);
				break;
			}
			NKCUtil.SetImageSprite(m_imgClass, NKCResourceUtility.GetOrLoadUnitRoleIcon(unitTempletBase, bSmall: true));
			NKCUtil.SetImageSprite(m_imgFace, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitData), bDisableIfSpriteNull: true);
			NKCUtil.SetGameobjectActive(m_objContractUnit, unitData.FromContract);
		}
		NKCUtil.SetGameobjectActive(m_ObjOff, unitData == null);
		NKCUtil.SetGameobjectActive(m_objOn, unitData != null);
	}
}
