using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCGameHudComboSlot : MonoBehaviour
{
	public Image m_img;

	public Text m_lbRespawnCost;

	public GameObject m_objArrow;

	public GameObject m_objActive;

	public Image m_imgActive;

	public Text m_lbRespawnCostActive;

	public void SetUI(NKMTacticalCombo cNKMTacticalCombo, bool bArrow = false)
	{
		if (cNKMTacticalCombo == null)
		{
			return;
		}
		bool flag = cNKMTacticalCombo.m_NKM_TACTICAL_COMBO_TYPE == NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_ROLE_TYPE || cNKMTacticalCombo.m_NKM_TACTICAL_COMBO_TYPE == NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_STYLE_TYPE || cNKMTacticalCombo.m_NKM_TACTICAL_COMBO_TYPE == NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_STR_ID;
		NKCUtil.SetGameobjectActive(m_img, flag);
		NKCUtil.SetGameobjectActive(m_imgActive, flag);
		NKCUtil.SetGameobjectActive(m_lbRespawnCost, !flag);
		NKCUtil.SetGameobjectActive(m_lbRespawnCostActive, !flag);
		if (cNKMTacticalCombo.m_NKM_TACTICAL_COMBO_TYPE == NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_ROLE_TYPE)
		{
			if (m_img != null)
			{
				m_img.sprite = NKCResourceUtility.GetOrLoadUnitRoleIcon(cNKMTacticalCombo.m_NKM_UNIT_ROLE_TYPE, bAwaken: true);
			}
		}
		else if (cNKMTacticalCombo.m_NKM_TACTICAL_COMBO_TYPE == NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_STYLE_TYPE)
		{
			if (m_img != null)
			{
				m_img.sprite = NKCResourceUtility.GetOrLoadUnitStyleIcon(cNKMTacticalCombo.m_NKM_UNIT_STYLE_TYPE, bSmall: true);
			}
		}
		else if (cNKMTacticalCombo.m_NKM_TACTICAL_COMBO_TYPE == NKM_TACTICAL_COMBO_TYPE.NTCBT_UNIT_STR_ID)
		{
			if (m_img != null)
			{
				int skinID = 0;
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMTacticalCombo.m_Value);
				m_img.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase, skinID);
			}
		}
		else if (cNKMTacticalCombo.m_NKM_TACTICAL_COMBO_TYPE == NKM_TACTICAL_COMBO_TYPE.NTCBT_RESPAWN_COST)
		{
			NKCUtil.SetLabelText(m_lbRespawnCost, cNKMTacticalCombo.m_ValueInt.ToString());
		}
		NKCUtil.SetGameobjectActive(m_objArrow, bArrow);
		if (m_imgActive != null && m_img != null)
		{
			m_imgActive.sprite = m_img.sprite;
		}
		if (m_lbRespawnCost != null)
		{
			NKCUtil.SetLabelText(m_lbRespawnCostActive, m_lbRespawnCost.text);
		}
	}

	public void SetComboSucess(bool bSuccess)
	{
		NKCUtil.SetGameobjectActive(m_objActive, bSuccess);
	}

	private void OnDestroy()
	{
		if (m_img != null)
		{
			m_img.sprite = null;
		}
		if (m_imgActive != null)
		{
			m_imgActive.sprite = null;
		}
	}
}
