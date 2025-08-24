using System;
using ClientPacket.Common;
using ClientPacket.Game;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using TMPro;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUITournamentPlayerSlot : MonoBehaviour
{
	public enum BattleResult
	{
		NONE,
		WIN,
		LOSE
	}

	public GameObject m_objNone;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objProfileRoot;

	public NKCUIComTitlePanel m_titlePanel;

	public NKCUISlotProfile m_slotProfile;

	public GameObject m_objGuildRoot;

	public NKCUIGuildBadge m_guildBadge;

	public TMP_Text m_lbGuildName;

	public TMP_Text m_lbLevel;

	public TMP_Text m_lbName;

	[Header("\ufffd\ufffdĪ \ufffd\ufffd\ufffd")]
	public GameObject m_objResultFx;

	public GameObject m_objWin;

	public GameObject m_objLose;

	[Header("etc")]
	public GameObject m_objTagRoot;

	public GameObject m_objSelect;

	public GameObject m_objMyself;

	public NKCUIComStateButton m_btnSlot;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objCheerRoot;

	public GameObject m_objCheering;

	public GameObject m_objCheerSuccess;

	public GameObject m_objCheerFail;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd±\ufffd")]
	public GameObject m_objCountryTag;

	public GameObject m_objKorea;

	public GameObject m_objGlobal;

	private NKMTournamentProfileData m_profileData;

	private NKMTournamentProfileData m_profileDataPredict;

	private BattleResult m_battleResult;

	private Action<int, long> m_onClickSlot;

	private NKCUITournamentPlayerSlot m_playerSlotUpper;

	private NKCUITournamentPlayerSlot m_playerSlotUpper_sub;

	private NKCUITournamentPlayerSlot m_playerSlotUnderA;

	private NKCUITournamentPlayerSlot m_playerSlotUnderB;

	private int m_slotIndex;

	private NKMTournamentGroups m_tournamentGroup;

	private bool m_showBattleResult;

	public NKMTournamentProfileData ProfileData => m_profileData;

	public NKMTournamentProfileData ProfileDataPredict => m_profileDataPredict;

	public int SlotIndex => m_slotIndex;

	public NKMTournamentGroups TournamentGroup => m_tournamentGroup;

	public BattleResult Result => m_battleResult;

	public void Init(Action<int, long> onClickSlot)
	{
		NKCUtil.SetButtonClickDelegate(m_btnSlot, OnClickSlot);
		m_onClickSlot = onClickSlot;
	}

	public void SetData(int slotIndex, NKMTournamentProfileData profileData, NKMTournamentProfileData profileDataPredict, NKMTournamentGroups tournamentGroup, bool showResult = false, bool leafPlayer = true)
	{
		m_slotIndex = slotIndex;
		m_profileData = profileData;
		m_profileDataPredict = profileDataPredict;
		m_tournamentGroup = tournamentGroup;
		m_battleResult = BattleResult.NONE;
		if (m_btnSlot != null)
		{
			m_btnSlot.enabled = false;
		}
		NKCUtil.SetGameobjectActive(m_objProfileRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objResultFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objWin, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLose, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTagRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMyself, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCheerRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objCheering, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCheerSuccess, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCheerFail, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCountryTag, bValue: false);
		if (profileData == null && profileDataPredict == null)
		{
			NKCUtil.SetGameobjectActive(m_objNone, bValue: true);
			return;
		}
		long num = 0L;
		if (profileData != null && (leafPlayer || showResult))
		{
			SetProfile(profileData);
			if (m_playerSlotUpper != null && m_playerSlotUpper.ProfileData != null)
			{
				num = m_playerSlotUpper.ProfileData.commonProfile.userUid;
			}
			if (num > 0 && showResult)
			{
				if (profileData.commonProfile.userUid == num)
				{
					m_battleResult = BattleResult.WIN;
				}
				else
				{
					m_battleResult = BattleResult.LOSE;
				}
			}
			NKCUtil.SetGameobjectActive(m_objResultFx, m_battleResult != BattleResult.NONE);
			NKCUtil.SetGameobjectActive(m_objWin, m_battleResult == BattleResult.WIN);
			NKCUtil.SetGameobjectActive(m_objLose, m_battleResult == BattleResult.LOSE);
		}
		else if (profileDataPredict != null)
		{
			SetProfile(profileDataPredict);
		}
		if (profileDataPredict == null)
		{
			return;
		}
		long num2 = 0L;
		if (m_playerSlotUpper != null && m_playerSlotUpper.ProfileDataPredict != null)
		{
			num2 = m_playerSlotUpper.ProfileDataPredict.commonProfile.userUid;
		}
		if ((m_tournamentGroup == NKMTournamentGroups.Finals || m_tournamentGroup == NKMTournamentGroups.GlobalFinals) && (m_slotIndex == 7 || m_slotIndex == 8))
		{
			NKMTournamentInfo tournamentInfoPredict = NKCTournamentManager.GetTournamentInfoPredict(m_tournamentGroup);
			if (tournamentInfoPredict != null && tournamentInfoPredict.slotUserUid.Count > 0)
			{
				int count = tournamentInfoPredict.slotUserUid.Count;
				num2 = tournamentInfoPredict.slotUserUid[count - 1];
			}
		}
		if (m_battleResult == BattleResult.NONE)
		{
			bool bValue = profileDataPredict.commonProfile.userUid == num2;
			NKCUtil.SetGameobjectActive(m_objCheering, bValue);
		}
		else if (profileDataPredict.commonProfile.userUid == num2 && profileDataPredict.commonProfile.userUid == profileData.commonProfile.userUid)
		{
			bool flag = num == num2;
			NKCUtil.SetGameobjectActive(m_objCheerSuccess, flag);
			NKCUtil.SetGameobjectActive(m_objCheerFail, !flag);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objCheerSuccess, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCheerFail, bValue: false);
		}
	}

	public void SetDataEx(NKMTournamentProfileData profileData, NKMTournamentProfileData profileDataPredict)
	{
		m_profileData = profileData;
		m_profileDataPredict = profileDataPredict;
	}

	public void SetPredictData(NKMTournamentProfileData profileDataPredict)
	{
		bool activeSelf = m_objCheering.gameObject.activeSelf;
		bool flag = IsPredictDataEqual(profileDataPredict);
		SetData(m_slotIndex, m_profileData, profileDataPredict, m_tournamentGroup);
		NKCUtil.SetGameobjectActive(m_objCheering, activeSelf && flag);
		if (activeSelf && m_playerSlotUpper != null && !m_playerSlotUpper.IsPredictDataEqual(profileDataPredict))
		{
			m_playerSlotUpper.SetPredictData(null);
		}
	}

	public bool IsPredictDataEqual(NKMTournamentProfileData profileDataPredict)
	{
		if (profileDataPredict == null || m_profileDataPredict == null)
		{
			return false;
		}
		return profileDataPredict.commonProfile.userUid == m_profileDataPredict.commonProfile.userUid;
	}

	public void ReleaseData()
	{
		m_profileData = null;
		m_profileDataPredict = null;
	}

	public void SetPlayerSlotUnder(NKCUITournamentPlayerSlot playerSlotUnderA, NKCUITournamentPlayerSlot playerSlotUnderB)
	{
		m_playerSlotUnderA = playerSlotUnderA;
		m_playerSlotUnderB = playerSlotUnderB;
		m_playerSlotUnderA?.SetPlayerSlotUpper(this);
		m_playerSlotUnderB?.SetPlayerSlotUpper(this);
	}

	public void SetAsBlankSlot()
	{
		NKCUtil.SetGameobjectActive(m_objProfileRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objResultFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objWin, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLose, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTagRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMyself, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCheerRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objCheering, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCheerSuccess, bValue: false);
		NKCUtil.SetGameobjectActive(m_objCheerFail, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNone, bValue: true);
		NKCUtil.SetGameobjectActive(m_objCountryTag, bValue: false);
	}

	public bool IsBlankSlot()
	{
		bool flag = false;
		if (m_profileData == null && m_profileDataPredict == null)
		{
			flag = true;
			if (m_playerSlotUnderA != null)
			{
				flag &= m_playerSlotUnderA.IsBlankSlot();
			}
			if (m_playerSlotUnderB != null)
			{
				flag &= m_playerSlotUnderB.IsBlankSlot();
			}
		}
		return flag;
	}

	public void HideResultIcon()
	{
		NKCUtil.SetGameobjectActive(m_objCheerRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objResultFx, bValue: false);
		NKCUtil.SetGameobjectActive(m_objWin, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLose, bValue: false);
	}

	public bool ProfileShowed()
	{
		return !m_objNone.activeSelf;
	}

	private void SetPlayerSlotUpper(NKCUITournamentPlayerSlot playerSlotUpper)
	{
		if (m_playerSlotUpper != null)
		{
			m_playerSlotUpper_sub = playerSlotUpper;
		}
		else
		{
			m_playerSlotUpper = playerSlotUpper;
		}
	}

	public void SetCheeringByUserUId(long userUId)
	{
		if (m_profileData != null)
		{
			NKCUtil.SetGameobjectActive(m_objCheering, m_profileData.commonProfile.userUid == userUId);
		}
		else if (m_profileDataPredict != null)
		{
			NKCUtil.SetGameobjectActive(m_objCheering, m_profileDataPredict.commonProfile.userUid == userUId);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objCheering, bValue: false);
		}
	}

	public void SetCheerEnable(bool value)
	{
		if (!value || m_battleResult != BattleResult.NONE)
		{
			NKCUtil.SetGameobjectActive(m_objSelect, bValue: false);
			m_btnSlot.enabled = false;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSelect, m_profileData != null || m_profileDataPredict != null);
			m_btnSlot.enabled = m_objSelect.gameObject.activeSelf;
		}
	}

	public bool IsCheering()
	{
		return m_objCheering.activeSelf;
	}

	public bool IsCheerSuccess()
	{
		if (m_objCheerRoot.activeSelf)
		{
			return m_objCheerSuccess.activeSelf;
		}
		return false;
	}

	public bool IsResultIconActivated()
	{
		if (!m_objWin.activeSelf)
		{
			return m_objLose.activeSelf;
		}
		return true;
	}

	private void SetProfile(NKMTournamentProfileData profileData)
	{
		if (profileData != null)
		{
			NKCUtil.SetGameobjectActive(m_objNone, bValue: false);
			NKCUtil.SetGameobjectActive(m_objProfileRoot, bValue: true);
			m_titlePanel?.SetData(profileData.commonProfile.titleId);
			m_slotProfile?.SetProfiledata(profileData.commonProfile, null);
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, profileData.commonProfile.level));
			NKCUtil.SetLabelText(m_lbName, profileData.commonProfile.nickname);
			NKCUtil.SetGameobjectActive(m_objGuildRoot, profileData.guildData != null && profileData.guildData.guildUid > 0);
			if (profileData.guildData != null)
			{
				m_guildBadge?.SetData(profileData.guildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, profileData.guildData.guildName);
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			NKCUtil.SetGameobjectActive(m_objMyself, nKMUserData != null && nKMUserData.UserProfileData.commonProfile.userUid == profileData.commonProfile.userUid);
			NKCUtil.SetGameobjectActive(m_objCountryTag, profileData.countryCode != NKMTournamentCountryCode.None);
			NKCUtil.SetGameobjectActive(m_objKorea, profileData.countryCode == NKMTournamentCountryCode.KR);
			NKCUtil.SetGameobjectActive(m_objGlobal, profileData.countryCode == NKMTournamentCountryCode.GL);
		}
	}

	private void OnClickSlot()
	{
		long arg = 0L;
		if (m_profileData != null)
		{
			m_playerSlotUpper?.SetPredictData(m_profileData);
			arg = m_profileData.commonProfile.userUid;
		}
		else if (m_profileDataPredict != null)
		{
			m_playerSlotUpper?.SetPredictData(m_profileDataPredict);
			arg = m_profileDataPredict.commonProfile.userUid;
		}
		if (m_onClickSlot != null)
		{
			m_onClickSlot(m_slotIndex, arg);
		}
	}

	private void OnDestroy()
	{
		ReleaseData();
		m_playerSlotUpper = null;
		m_playerSlotUnderA = null;
		m_playerSlotUnderB = null;
	}
}
