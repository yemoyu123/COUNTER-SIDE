using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIRearmamentProcessInfoSummary : MonoBehaviour
{
	[Header("info")]
	public GameObject m_objInfo;

	public Text m_lbRearmUnitName;

	public Text m_lbRearmUnitDesc;

	public Image m_imgRearmUnitFaceCard;

	[Header("Skill")]
	public GameObject m_objSkill;

	public Image m_imgRearmLeaderSkill;

	public List<Image> m_lstSkillImg;

	[Header("Etc")]
	public List<Image> m_lstPointColor;

	public string m_defaultColor = "#26216F";

	[Header("button")]
	public List<NKCUIComStateButton> m_csbtnSkill = new List<NKCUIComStateButton>();

	private bool bInit;

	private NKMUnitTempletBase m_CurUnitTempletBase;

	private void Init()
	{
		if (bInit)
		{
			return;
		}
		foreach (NKCUIComStateButton item in m_csbtnSkill)
		{
			NKCUtil.SetBindFunction(item, ClickSkillInfo);
		}
		bInit = true;
	}

	public void SetData(int unitID, bool bSkill = false)
	{
		Init();
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKCUtil.SetImageSprite(m_imgRearmUnitFaceCard, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase), bDisableIfSpriteNull: true);
		int num = 0;
		for (int i = 0; i < unitTempletBase.GetSkillCount(); i++)
		{
			string skillStrID = unitTempletBase.GetSkillStrID(i);
			if (string.IsNullOrEmpty(skillStrID))
			{
				continue;
			}
			NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(skillStrID, 1);
			if (skillTemplet != null)
			{
				if (skillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_LEADER)
				{
					NKCUtil.SetImageSprite(m_imgRearmLeaderSkill, NKCUtil.GetSkillIconSprite(skillTemplet));
					continue;
				}
				NKCUtil.SetImageSprite(m_lstSkillImg[num], NKCUtil.GetSkillIconSprite(skillTemplet));
				NKCUtil.SetGameobjectActive(m_lstSkillImg[num], bValue: true);
				num++;
			}
		}
		for (int j = num; j < m_lstSkillImg.Count; j++)
		{
			NKCUtil.SetGameobjectActive(m_lstSkillImg[j], bValue: false);
		}
		string hexRGB = m_defaultColor;
		foreach (NKMUnitRearmamentTemplet value in NKMTempletContainer<NKMUnitRearmamentTemplet>.Values)
		{
			if (value.Key == unitID)
			{
				if (!string.IsNullOrEmpty(value.Color))
				{
					hexRGB = value.Color;
				}
				break;
			}
		}
		foreach (Image item in m_lstPointColor)
		{
			Color color = NKCUtil.GetColor(hexRGB);
			color.a = item.color.a;
			NKCUtil.SetImageColor(item, color);
		}
		NKCUtil.SetLabelText(m_lbRearmUnitName, unitTempletBase.GetUnitTitle());
		NKCUtil.SetLabelText(m_lbRearmUnitDesc, unitTempletBase.GetUnitDesc());
		NKCUtil.SetGameobjectActive(m_objSkill, bSkill);
		NKCUtil.SetGameobjectActive(m_objInfo, !bSkill);
		m_CurUnitTempletBase = unitTempletBase;
	}

	private void ClickSkillInfo()
	{
		if (m_CurUnitTempletBase != null)
		{
			NKMUnitData nKMUnitData = new NKMUnitData();
			nKMUnitData.m_UnitID = m_CurUnitTempletBase.m_UnitID;
			nKMUnitData.m_UnitLevel = 1;
			nKMUnitData.m_LimitBreakLevel = 0;
			NKCPopupSkillFullInfo.UnitInstance.OpenForUnit(nKMUnitData, m_CurUnitTempletBase.GetUnitName(), m_CurUnitTempletBase.m_StarGradeMax, 0, m_CurUnitTempletBase.IsRearmUnit);
		}
	}
}
