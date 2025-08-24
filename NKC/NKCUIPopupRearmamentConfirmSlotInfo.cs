using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIPopupRearmamentConfirmSlotInfo : MonoBehaviour
{
	public Text m_lbClassType;

	public Image m_imgRarityIcon;

	public GameObject m_objFX_SSR;

	public GameObject m_objFX_SR;

	public GameObject m_objFX_R;

	public GameObject m_objFX_N;

	public GameObject m_objFX_AwakeSSR;

	public GameObject m_objFX_AwakeSR;

	public Text m_lbUnitName;

	public Text m_lbRole;

	public Image m_imgRole;

	public NKCUIComTextUnitLevel m_lbLevel;

	public Text m_lbMoveType;

	public Image m_imgMoveType;

	public Text m_lbAttackType;

	public Image m_imgAttack;

	public GameObject m_objTag;

	public Text m_lbTag;

	public Image m_imgTag;

	public void SetData(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase != null)
		{
			NKCUtil.SetLabelTextColor(m_lbClassType, NKCUtil.GetColorForUnitGrade(unitTempletBase.m_NKM_UNIT_GRADE));
			NKCUtil.SetLabelText(m_lbClassType, NKCUtilString.GetUnitStyleMarkString(unitTempletBase));
			NKCUtil.SetLabelText(m_lbUnitName, unitTempletBase.GetUnitName());
			NKCUtil.SetLabelText(m_lbRole, NKCUtilString.GetRoleText(unitTempletBase));
			NKCUtil.SetImageSprite(m_imgRole, NKCResourceUtility.GetOrLoadUnitRoleIcon(unitTempletBase, bSmall: true));
			NKCUtil.SetImageSprite(m_imgMoveType, NKCUtil.GetMoveTypeImg(unitTempletBase.m_bAirUnit));
			NKCUtil.SetLabelText(m_lbMoveType, NKCUtilString.GetMoveTypeText(unitTempletBase.m_bAirUnit));
			NKCUtil.SetImageSprite(m_imgAttack, NKCResourceUtility.GetOrLoadUnitAttackTypeIcon(unitTempletBase, bSmall: true));
			NKCUtil.SetLabelText(m_lbAttackType, NKCUtilString.GetAtkTypeText(unitTempletBase));
			string unitAbilityName = NKCUtilString.GetUnitAbilityName(unitTempletBase.m_UnitID);
			NKCUtil.SetGameobjectActive(m_objTag, !string.IsNullOrEmpty(unitAbilityName));
			NKCUtil.SetLabelText(m_lbTag, unitAbilityName);
			if (m_lbLevel != null)
			{
				int unitMaxLevel = NKCExpManager.GetUnitMaxLevel(unitData);
				m_lbLevel.SetText(NKCStringTable.GetString("SI_PF_POPUP_REARM_CONFIRM_LEVEL_ALL", unitData.m_UnitLevel, unitMaxLevel), unitData);
			}
			SetGrade(unitTempletBase.m_NKM_UNIT_GRADE, unitTempletBase.m_bAwaken);
		}
	}

	private void SetGrade(NKM_UNIT_GRADE unitGrade, bool bAwake)
	{
		NKCUtil.SetImageSprite(m_imgRarityIcon, NKCUtil.GetSpriteUnitGrade(unitGrade));
		NKCUtil.SetGameobjectActive(m_objFX_N, unitGrade == NKM_UNIT_GRADE.NUG_N);
		NKCUtil.SetGameobjectActive(m_objFX_R, unitGrade == NKM_UNIT_GRADE.NUG_R);
		NKCUtil.SetGameobjectActive(m_objFX_SR, unitGrade == NKM_UNIT_GRADE.NUG_SR);
		NKCUtil.SetGameobjectActive(m_objFX_SSR, unitGrade == NKM_UNIT_GRADE.NUG_SSR);
		NKCUtil.SetGameobjectActive(m_objFX_AwakeSR, bAwake && unitGrade == NKM_UNIT_GRADE.NUG_SR);
		NKCUtil.SetGameobjectActive(m_objFX_AwakeSSR, bAwake && unitGrade == NKM_UNIT_GRADE.NUG_SSR);
	}
}
