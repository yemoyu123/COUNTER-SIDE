using NKC.UI.Component;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorSkill : MonoBehaviour
{
	public Image m_SkillIcon;

	public NKCComTMPUIText m_lbSkillName;

	public NKCComTMPUIText m_lbSkillDesc;

	public NKCComTMPUIText m_lbSkillLevel;

	public NKCComText m_lbSkillLevel0;

	public NKCUIComStateButton m_csbtnButton;

	public GameObject m_objTacticCoolTime;

	public Text m_lbTacticSkillCoolTime;

	private int m_skillID;

	private int m_skillLevel;

	public void SetData(int skillID, int skillLevel, bool Banned = false)
	{
		NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(skillID);
		SetData(skillTemplet, skillLevel, Banned);
	}

	public void SetData(NKMOperatorSkillTemplet skillTemplet, int skillLevel, bool Banned = false)
	{
		if (skillTemplet == null)
		{
			m_skillID = 0;
			SetRandomSkillIcon();
			return;
		}
		NKCUtil.SetImageSprite(m_SkillIcon, NKCUtil.GetSkillIconSprite(skillTemplet));
		NKCUtil.SetLabelText(m_lbSkillLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, skillLevel));
		NKCUtil.SetLabelTextColor(m_lbSkillLevel, Banned ? NKCOperatorUtil.BAN_COLOR_RED : Color.white);
		NKCUtil.SetLabelText(m_lbSkillLevel0, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, skillLevel));
		NKCUtil.SetLabelTextColor(m_lbSkillLevel0, Banned ? NKCOperatorUtil.BAN_COLOR_RED : Color.white);
		NKCUtil.SetLabelText(m_lbSkillName, NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID));
		NKCUtil.SetLabelText(m_lbSkillDesc, NKCOperatorUtil.MakeOperatorSkillDesc(skillTemplet, skillLevel));
		m_skillID = skillTemplet.m_OperSkillID;
		m_skillLevel = skillLevel;
		if (m_csbtnButton != null)
		{
			m_csbtnButton.PointerDown.RemoveAllListeners();
			m_csbtnButton.PointerDown.AddListener(OnPointDownSkill);
		}
		NKMTacticalCommandTemplet tacticalCommandTempletByStrID = NKMTacticalCommandManager.GetTacticalCommandTempletByStrID(skillTemplet.m_OperSkillTarget);
		if (tacticalCommandTempletByStrID != null)
		{
			NKCUtil.SetLabelText(m_lbTacticSkillCoolTime, string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), (int)tacticalCommandTempletByStrID.m_fCoolTime));
		}
		NKCUtil.SetGameobjectActive(m_objTacticCoolTime, tacticalCommandTempletByStrID != null);
	}

	public void SetDataForCollection(int OperatorID, int level = -1)
	{
		m_skillID = 0;
		m_skillLevel = 1;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(OperatorID);
		if (unitTempletBase == null || unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			return;
		}
		if (unitTempletBase.m_lstSkillStrID != null && unitTempletBase.m_lstSkillStrID.Count > 0)
		{
			NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(unitTempletBase.m_lstSkillStrID[0]);
			if (skillTemplet != null)
			{
				m_skillID = skillTemplet.m_OperSkillID;
				m_skillLevel = ((level < 0) ? skillTemplet.m_MaxSkillLevel : level);
				NKCUtil.SetImageSprite(m_SkillIcon, NKCUtil.GetSkillIconSprite(skillTemplet));
				NKCUtil.SetLabelText(m_lbSkillLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_skillLevel));
				NKCUtil.SetLabelText(m_lbSkillName, NKCStringTable.GetString(skillTemplet.m_OperSkillNameStrID));
				NKCUtil.SetLabelText(m_lbSkillDesc, NKCOperatorUtil.MakeOperatorSkillDesc(skillTemplet, m_skillLevel));
			}
		}
		if (m_csbtnButton != null)
		{
			m_csbtnButton.PointerDown.RemoveAllListeners();
			m_csbtnButton.PointerDown.AddListener(OnPointDownSkill);
		}
	}

	public void SetRandomSkillIcon()
	{
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_tactical_command_icon", "OPR_RANDOM");
		NKCUtil.SetImageSprite(m_SkillIcon, orLoadAssetResource);
		NKCUtil.SetLabelText(m_lbSkillLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, 1));
	}

	private void OnPointDownSkill(PointerEventData eventData)
	{
		NKCUITooltip.Instance.Open(m_skillID, m_skillLevel, eventData.position);
	}
}
