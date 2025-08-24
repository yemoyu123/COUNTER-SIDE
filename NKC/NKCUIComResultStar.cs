using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComResultStar : MonoBehaviour
{
	public List<Image> m_lstStarImages;

	public Sprite m_spStarYellow;

	public Sprite m_spStarPurple;

	public Sprite m_spStarBlue;

	public void SetTranscendence(int unitID, int limitBreakLevel)
	{
		if (NKMUnitManager.GetUnitTempletBase(unitID).IsShip())
		{
			if (NKMShipManager.IsMaxLimitBreak(unitID, limitBreakLevel))
			{
				foreach (Image lstStarImage in m_lstStarImages)
				{
					lstStarImage.sprite = m_spStarPurple;
				}
				return;
			}
			{
				foreach (Image lstStarImage2 in m_lstStarImages)
				{
					lstStarImage2.sprite = m_spStarYellow;
				}
				return;
			}
		}
		if (limitBreakLevel >= 13)
		{
			foreach (Image lstStarImage3 in m_lstStarImages)
			{
				lstStarImage3.sprite = m_spStarBlue;
			}
			return;
		}
		if (limitBreakLevel >= 8)
		{
			foreach (Image lstStarImage4 in m_lstStarImages)
			{
				lstStarImage4.sprite = m_spStarPurple;
			}
			return;
		}
		foreach (Image lstStarImage5 in m_lstStarImages)
		{
			lstStarImage5.sprite = m_spStarYellow;
		}
	}
}
