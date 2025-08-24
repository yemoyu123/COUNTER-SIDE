using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUILeagueTier : MonoBehaviour
{
	public GameObject m_objBronze;

	public GameObject m_objSilver;

	public GameObject m_objGold;

	public GameObject m_objPlatinum;

	public GameObject m_objDiamond;

	public GameObject m_objMaster;

	public GameObject m_objGrandMaster;

	public GameObject m_objChallenger;

	public GameObject m_objEventmatch;

	public GameObject m_objBlind;

	public GameObject m_objPrivatePvp;

	public Text m_lbBronzeNumber;

	public Text m_lbSilverNumber;

	public Text m_lbGoldNumber;

	public Text m_lbPlatinumNumber;

	public Text m_lbDiamondNumber;

	public Text m_lbMasterNumber;

	public Text m_lbGrandMasterNumber;

	public Text m_lbChallengerNumber;

	public void SetDisableNormalTier()
	{
		NKCUtil.SetGameobjectActive(m_objBronze, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSilver, bValue: false);
		NKCUtil.SetGameobjectActive(m_objGold, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPlatinum, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDiamond, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMaster, bValue: false);
		NKCUtil.SetGameobjectActive(m_objGrandMaster, bValue: false);
		NKCUtil.SetGameobjectActive(m_objChallenger, bValue: false);
	}

	public void SetUI(LEAGUE_TIER_ICON leagueTierIcon, int leagueTierNum)
	{
		NKCUtil.SetGameobjectActive(m_objBronze, leagueTierIcon == LEAGUE_TIER_ICON.LTI_BRONZE);
		NKCUtil.SetGameobjectActive(m_objSilver, leagueTierIcon == LEAGUE_TIER_ICON.LTI_SILVER);
		NKCUtil.SetGameobjectActive(m_objGold, leagueTierIcon == LEAGUE_TIER_ICON.LTI_GOLD);
		NKCUtil.SetGameobjectActive(m_objPlatinum, leagueTierIcon == LEAGUE_TIER_ICON.LTI_PLATINUM);
		NKCUtil.SetGameobjectActive(m_objDiamond, leagueTierIcon == LEAGUE_TIER_ICON.LTI_DIAMOND);
		NKCUtil.SetGameobjectActive(m_objMaster, leagueTierIcon == LEAGUE_TIER_ICON.LTI_MASTER);
		NKCUtil.SetGameobjectActive(m_objGrandMaster, leagueTierIcon == LEAGUE_TIER_ICON.LTI_GRANDMASTER);
		NKCUtil.SetGameobjectActive(m_objChallenger, leagueTierIcon == LEAGUE_TIER_ICON.LTI_CHALLENGER);
		NKCUtil.SetGameobjectActive(m_objEventmatch, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBlind, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPrivatePvp, bValue: false);
		switch (leagueTierIcon)
		{
		case LEAGUE_TIER_ICON.LTI_BRONZE:
			m_lbBronzeNumber.text = leagueTierNum.ToString();
			break;
		case LEAGUE_TIER_ICON.LTI_SILVER:
			m_lbSilverNumber.text = leagueTierNum.ToString();
			break;
		case LEAGUE_TIER_ICON.LTI_GOLD:
			m_lbGoldNumber.text = leagueTierNum.ToString();
			break;
		case LEAGUE_TIER_ICON.LTI_PLATINUM:
			m_lbPlatinumNumber.text = leagueTierNum.ToString();
			break;
		case LEAGUE_TIER_ICON.LTI_DIAMOND:
			m_lbDiamondNumber.text = leagueTierNum.ToString();
			break;
		case LEAGUE_TIER_ICON.LTI_MASTER:
			m_lbMasterNumber.text = leagueTierNum.ToString();
			break;
		case LEAGUE_TIER_ICON.LTI_GRANDMASTER:
			m_lbGrandMasterNumber.text = leagueTierNum.ToString();
			break;
		case LEAGUE_TIER_ICON.LTI_CHALLENGER:
			m_lbChallengerNumber.text = leagueTierNum.ToString();
			break;
		}
	}

	public void SetUI(NKMPvpRankTemplet cNKMPvpRankTemplet)
	{
		if (cNKMPvpRankTemplet != null)
		{
			SetUI(cNKMPvpRankTemplet.LeagueTierIcon, cNKMPvpRankTemplet.LeagueTierIconNumber);
		}
	}

	public void SetUI(NKMLeaguePvpRankTemplet templet)
	{
		if (templet != null)
		{
			SetUI(templet.LeagueTierIcon, templet.LeagueTierIconNumber);
		}
	}

	public void SetEventmatchIcon(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objEventmatch, value);
	}

	public void SetBlind(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objBlind, bValue);
	}

	public void SetPrivatePvp(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objPrivatePvp, bValue);
	}
}
