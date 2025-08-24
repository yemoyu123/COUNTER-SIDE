using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComStarRank : MonoBehaviour
{
	public List<Image> m_lstStarImages;

	public Sprite m_spStarGray;

	public Sprite m_spStarYellow;

	public Sprite m_spStarPurple;

	public Sprite m_spStarBlue;

	public void SetStarRank(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			SetStarRank(0, 0, 0);
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			bool flag = NKMShipManager.IsMaxLimitBreak(unitData);
			SetStarRankManual(unitData.GetStarGrade(unitTempletBase), 6, flag ? 1 : 0);
		}
		else
		{
			NKMUnitLimitBreakManager.UnitLimitBreakStatusData unitLimitbreakStatus = NKMUnitLimitBreakManager.GetUnitLimitbreakStatus(unitData);
			int tier = (unitLimitbreakStatus.CurrentTierCompleted() ? unitLimitbreakStatus.Tier : (unitLimitbreakStatus.Tier - 1));
			SetStarRank(unitData.m_LimitBreakLevel, unitTempletBase.m_StarGradeMax, tier);
		}
	}

	public void SetStarRank(NKMUnitTempletBase templetBase, int level)
	{
		int limitBreakLevel = 0;
		if (level > 1)
		{
			limitBreakLevel = NKMUnitLimitBreakManager.GetMinLimitBreakLevelByUnitLevel(templetBase, level);
		}
		SetStarRank(limitBreakLevel, templetBase.m_StarGradeMax);
	}

	public void SetStarRankShip(int shipID, int limitBreakLevel)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipID);
		int tier = (NKMShipManager.IsMaxLimitBreak(shipID, limitBreakLevel) ? 1 : 0);
		SetStarRankManual(unitTempletBase.m_StarGradeMax, unitTempletBase.m_StarGradeMax, tier);
	}

	public void SetStarRank(int limitBreakLevel, int maxStarRank)
	{
		NKMLimitBreakTemplet lBInfo = NKMUnitLimitBreakManager.GetLBInfo(limitBreakLevel + 1);
		NKMLimitBreakTemplet lBInfo2 = NKMUnitLimitBreakManager.GetLBInfo(limitBreakLevel);
		int tier = ((lBInfo2 != null) ? ((lBInfo != null) ? (lBInfo.m_Tier - 1) : lBInfo2.m_Tier) : 0);
		SetStarRank(limitBreakLevel, maxStarRank, tier);
	}

	private void SetStarRank(int limitBreakLevel, int maxStarRank, int tier)
	{
		int starCount = maxStarRank;
		if (limitBreakLevel <= 3)
		{
			starCount = maxStarRank - 3 + limitBreakLevel;
		}
		SetStarRankManual(starCount, maxStarRank, tier);
	}

	private void SetStarRankManual(int starCount, int maxStarRank, int tier)
	{
		Sprite sp = tier switch
		{
			2 => m_spStarBlue, 
			1 => m_spStarPurple, 
			_ => m_spStarYellow, 
		};
		for (int i = 0; i < m_lstStarImages.Count; i++)
		{
			if (m_lstStarImages[i] == null)
			{
				continue;
			}
			Image image = m_lstStarImages[i];
			if (i < maxStarRank)
			{
				NKCUtil.SetGameobjectActive(image, bValue: true);
				if (i < starCount)
				{
					NKCUtil.SetImageSprite(image, sp);
				}
				else
				{
					NKCUtil.SetImageSprite(image, m_spStarGray);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(image, bValue: false);
			}
		}
	}

	public Image GetStarObjectImage(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index >= m_lstStarImages.Count)
		{
			return null;
		}
		return m_lstStarImages[index];
	}
}
