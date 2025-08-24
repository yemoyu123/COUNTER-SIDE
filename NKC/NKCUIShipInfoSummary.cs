using System.Collections.Generic;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIShipInfoSummary : MonoBehaviour
{
	public Image m_imgShipType;

	public Image m_imgShipTypeSmall;

	public Text m_lbShipType;

	public Text m_lbName;

	public Image m_imgGrade;

	public List<GameObject> m_lstGrade;

	public NKCUIComStarRank m_StarRank;

	public Text m_lbLevel;

	public Text m_lbMaxLevel;

	public GameObject m_objMaxLevel;

	public List<Image> m_imgSkillIcon;

	public GameObject m_objPassive;

	public Text m_lbPassive;

	public GameObject m_objBan;

	public Text m_lbBanLevel;

	public Text m_lbStartCost;

	public void SetShipData(NKMUnitData shipData, NKMUnitTempletBase shipTempletBase, bool bInDeck = false)
	{
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		if (shipTempletBase != null)
		{
			if (m_imgShipType != null)
			{
				Sprite shipTypeSprite = GetShipTypeSprite(shipTempletBase.m_NKM_UNIT_STYLE_TYPE, isSmall: false);
				m_imgShipType.sprite = shipTypeSprite;
				NKCUtil.SetGameobjectActive(m_imgShipType, shipTypeSprite != null);
			}
			if (m_imgShipTypeSmall != null)
			{
				Sprite shipTypeSprite2 = GetShipTypeSprite(shipTempletBase.m_NKM_UNIT_STYLE_TYPE, isSmall: true);
				m_imgShipTypeSmall.sprite = shipTypeSprite2;
				NKCUtil.SetGameobjectActive(m_imgShipTypeSmall, shipTypeSprite2 != null && !bInDeck);
			}
			NKCUtil.SetLabelText(m_lbShipType, NKCUtilString.GetUnitStyleName(shipTempletBase.m_NKM_UNIT_STYLE_TYPE));
			if (m_lbShipType != null && !bInDeck)
			{
				m_lbShipType.color = NKCUtil.GetColorForUnitGrade(shipTempletBase.m_NKM_UNIT_GRADE);
			}
			NKCUtil.SetLabelText(m_lbName, shipTempletBase.GetUnitName());
			SetGrade(shipTempletBase.m_NKM_UNIT_GRADE);
			bool flag = false;
			NKCUtil.SetGameobjectActive(m_objPassive, bValue: true);
			int num = 0;
			for (int i = 0; i < shipTempletBase.GetSkillCount(); i++)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(shipTempletBase, i);
				if (shipSkillTempletByIndex != null)
				{
					if (num < m_imgSkillIcon.Count && shipSkillTempletByIndex.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_PASSIVE)
					{
						m_imgSkillIcon[num].enabled = true;
						m_imgSkillIcon[num].sprite = NKCUtil.GetSkillIconSprite(shipSkillTempletByIndex);
						num++;
					}
					if (!flag)
					{
						flag = true;
						NKCUtil.SetLabelText(m_lbPassive, shipSkillTempletByIndex.GetBuildDesc());
					}
				}
			}
			if (!flag)
			{
				NKCUtil.SetLabelText(m_lbPassive, NKCStringTable.GetString("SI_MENU_EXCEPTION_SHIP_PASSIVE_EMPTY"));
			}
			for (int j = 0; j < shipTempletBase.GetSkillCount(); j++)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex2 = NKMShipSkillManager.GetShipSkillTempletByIndex(shipTempletBase, j);
				if (shipSkillTempletByIndex2 != null && num < m_imgSkillIcon.Count && shipSkillTempletByIndex2.m_NKM_SKILL_TYPE != NKM_SKILL_TYPE.NST_PASSIVE)
				{
					m_imgSkillIcon[num].enabled = true;
					m_imgSkillIcon[num].sprite = NKCUtil.GetSkillIconSprite(shipSkillTempletByIndex2);
					num++;
				}
			}
			for (int k = num; k < m_imgSkillIcon.Count; k++)
			{
				m_imgSkillIcon[k].enabled = false;
			}
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
			{
				bool num2 = NKCBanManager.IsBanShip(shipTempletBase.m_ShipGroupID);
				bool flag2 = NKCUtil.CheckPossibleShowBan(NKCUIDeckViewer.DeckViewerMode.PrivatePvPReady);
				if (num2 && flag2)
				{
					NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
					int shipBanLevel = NKCBanManager.GetShipBanLevel(shipTempletBase.m_ShipGroupID);
					NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, shipBanLevel));
					NKCUtil.SetLabelTextColor(m_lbBanLevel, Color.red);
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgShipType, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgShipTypeSmall, bValue: false);
			NKCUtil.SetLabelText(m_lbShipType, "");
			NKCUtil.SetLabelText(m_lbPassive, "");
			NKCUtil.SetLabelText(m_lbName, NKCUtilString.GET_STRING_UNKNOWN);
			NKCUtil.SetGameobjectActive(m_objPassive, bValue: false);
			for (int l = 0; l < m_imgSkillIcon.Count; l++)
			{
				m_imgSkillIcon[l].enabled = false;
			}
		}
		if (shipData != null)
		{
			if (m_lbLevel != null)
			{
				NKCUIComTextUnitLevel nKCUIComTextUnitLevel = m_lbLevel as NKCUIComTextUnitLevel;
				if (nKCUIComTextUnitLevel != null)
				{
					nKCUIComTextUnitLevel.SetLevel(shipData, 0);
					NKCUtil.SetLabelText(nKCUIComTextUnitLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, nKCUIComTextUnitLevel.text));
				}
				else
				{
					m_lbLevel.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, shipData.m_UnitLevel.ToString());
				}
			}
			int unitMaxLevel = NKCExpManager.GetUnitMaxLevel(shipData);
			NKCUtil.SetLabelText(m_lbMaxLevel, $"/ {unitMaxLevel}");
			NKCUtil.SetGameobjectActive(m_objMaxLevel, shipData.m_UnitLevel >= unitMaxLevel);
			NKCUtil.SetLabelText(m_lbStartCost, NKCUtil.GetShipStartCost(shipData.GetStarGrade(shipTempletBase)).ToString());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbLevel, "");
			NKCUtil.SetLabelText(m_lbMaxLevel, "");
			NKCUtil.SetGameobjectActive(m_objMaxLevel, bValue: false);
			NKCUtil.SetLabelText(m_lbStartCost, "");
		}
		m_StarRank?.SetStarRank(shipData);
	}

	private void SetGrade(NKM_UNIT_GRADE grade)
	{
		for (int i = 0; i < m_lstGrade.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstGrade[i], i == (int)grade);
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COMMON_ICON", grade switch
		{
			NKM_UNIT_GRADE.NUG_R => "NKM_UI_COMMON_RANK_R", 
			NKM_UNIT_GRADE.NUG_SR => "NKM_UI_COMMON_RANK_SR", 
			NKM_UNIT_GRADE.NUG_SSR => "NKM_UI_COMMON_RANK_SSR", 
			_ => "NKM_UI_COMMON_RANK_N", 
		});
		if (orLoadAssetResource == null)
		{
			Debug.LogError("Rarity sprite not found!");
		}
		NKCUtil.SetImageSprite(m_imgGrade, orLoadAssetResource, bDisableIfSpriteNull: true);
	}

	public void SetShipData(NKMUnitData shipData, NKMUnitTempletBase shipTempletBase, NKMDeckIndex deckIndex, bool bInDeck = false)
	{
		SetShipData(shipData, shipTempletBase, bInDeck);
	}

	private Sprite GetShipTypeSprite(NKM_UNIT_STYLE_TYPE type, bool isSmall)
	{
		return NKCResourceUtility.GetOrLoadUnitStyleIcon(type, isSmall);
	}
}
