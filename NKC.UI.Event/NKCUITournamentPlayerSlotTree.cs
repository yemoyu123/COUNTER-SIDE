using System;
using ClientPacket.Common;
using ClientPacket.Game;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUITournamentPlayerSlotTree : MonoBehaviour
{
	public struct ProfileDataSet
	{
		public NKMTournamentProfileData profildData;

		public NKMTournamentProfileData profildDataPredict;

		public NKMTournamentGroups group;

		public int slotIndex;
	}

	public NKCUITournamentPlayerSlot m_playerA;

	public NKCUITournamentPlayerSlot m_playerB;

	public NKCUIComStateButton m_csbtnDetail;

	private NKCUITournamentPlayerSlotTree m_treeUpper;

	private NKCUITournamentPlayerSlotTree m_treeUnderA;

	private NKCUITournamentPlayerSlotTree m_treeUnderB;

	private bool m_isFinalGroup;

	public NKCUITournamentPlayerSlotTree TreeUpper
	{
		get
		{
			return m_treeUpper;
		}
		set
		{
			m_treeUpper = value;
		}
	}

	public NKCUITournamentPlayerSlotTree TreeUnderA
	{
		get
		{
			return m_treeUnderA;
		}
		set
		{
			m_treeUnderA = value;
		}
	}

	public NKCUITournamentPlayerSlotTree TreeUnderB
	{
		get
		{
			return m_treeUnderB;
		}
		set
		{
			m_treeUnderB = value;
		}
	}

	public void Init(Action<int, long> onClickSlot, bool isFinalGroup)
	{
		m_isFinalGroup = isFinalGroup;
		NKCUtil.SetButtonClickDelegate(m_csbtnDetail, OnClickDetail);
		m_playerA.Init(onClickSlot);
		m_playerB.Init(onClickSlot);
	}

	public void SetProfileDataA(ProfileDataSet profileDataA, bool showResult, bool leafPlayer, bool isCheerResultState)
	{
		m_playerA.SetData(profileDataA.slotIndex, profileDataA.profildData, profileDataA.profildDataPredict, profileDataA.group, showResult, leafPlayer);
		if (isCheerResultState && profileDataA.profildDataPredict == null)
		{
			m_playerA.SetAsBlankSlot();
		}
	}

	public void SetProfileDataB(ProfileDataSet profileDataB, bool showResult, bool leafPlayer, bool isCheerResultState)
	{
		m_playerB.SetData(profileDataB.slotIndex, profileDataB.profildData, profileDataB.profildDataPredict, profileDataB.group, showResult, leafPlayer);
		if (isCheerResultState && profileDataB.profildDataPredict == null)
		{
			m_playerB.SetAsBlankSlot();
		}
	}

	public void SetProfileDataA(ProfileDataSet profileDataA, bool showResult, bool leafPlayer, bool isCheerMode, bool isCheerResultState, bool showResultEx)
	{
		m_playerA.SetData(profileDataA.slotIndex, profileDataA.profildData, profileDataA.profildDataPredict, profileDataA.group, showResult, leafPlayer);
		if (isCheerResultState && profileDataA.profildDataPredict == null)
		{
			m_playerA.SetAsBlankSlot();
		}
		if (!isCheerMode && !isCheerResultState)
		{
			if (!showResult)
			{
				m_playerA.SetAsBlankSlot();
			}
			else if (!showResultEx)
			{
				m_playerA.HideResultIcon();
			}
		}
	}

	public void SetProfileDataB(ProfileDataSet profileDataB, bool showResult, bool leafPlayer, bool isCheerMode, bool isCheerResultState, bool showResultEx)
	{
		m_playerB.SetData(profileDataB.slotIndex, profileDataB.profildData, profileDataB.profildDataPredict, profileDataB.group, showResult, leafPlayer);
		if (isCheerResultState && profileDataB.profildDataPredict == null)
		{
			m_playerB.SetAsBlankSlot();
		}
		if (!isCheerMode && !isCheerResultState)
		{
			if (!showResult)
			{
				m_playerB.SetAsBlankSlot();
			}
			else if (!showResultEx)
			{
				m_playerB.HideResultIcon();
			}
		}
	}

	public void ReleaseData()
	{
		m_playerA?.ReleaseData();
		m_playerB?.ReleaseData();
	}

	public void SetSlotLink()
	{
		if (m_treeUnderA != null)
		{
			m_playerA.SetPlayerSlotUnder(m_treeUnderA.m_playerA, m_treeUnderA.m_playerB);
		}
		if (m_treeUnderB != null)
		{
			m_playerB.SetPlayerSlotUnder(m_treeUnderB.m_playerA, m_treeUnderB.m_playerB);
		}
	}

	public void SetCheerEnable(bool value)
	{
		m_playerA.SetCheerEnable(value);
		m_playerB.SetCheerEnable(value);
	}

	public void SetCheeringByUserUId(long userUId)
	{
		m_playerA.SetCheeringByUserUId(userUId);
		m_playerB.SetCheeringByUserUId(userUId);
	}

	public NKMTournamentProfileData GetOtherSlotProfileData(int slotIndex)
	{
		NKMTournamentProfileData result = null;
		if (m_playerA.SlotIndex == slotIndex)
		{
			if (m_playerB.ProfileData != null)
			{
				result = m_playerB.ProfileData;
			}
			else if (m_playerB.ProfileDataPredict != null)
			{
				result = m_playerB.ProfileDataPredict;
			}
		}
		else if (m_playerA.ProfileData != null)
		{
			result = m_playerA.ProfileData;
		}
		else if (m_playerA.ProfileDataPredict != null)
		{
			result = m_playerA.ProfileDataPredict;
		}
		return result;
	}

	public void SetDetailButtonActive()
	{
		bool flag = m_playerA.ProfileData != null || m_playerB.ProfileData != null;
		bool flag2 = m_playerA.ProfileDataPredict != null || m_playerB.ProfileDataPredict != null;
		bool flag3 = m_playerA.ProfileShowed() || m_playerB.ProfileShowed();
		NKCUtil.SetGameobjectActive(m_csbtnDetail, (flag || flag2) && flag3);
	}

	public bool IsCheering()
	{
		if (!m_playerA.IsCheering())
		{
			return m_playerB.IsCheering();
		}
		return true;
	}

	public bool IsBlankTree()
	{
		bool flag = false;
		if (m_playerA.ProfileData == null && m_playerB.ProfileData == null && m_playerA.ProfileDataPredict == null && m_playerB.ProfileDataPredict == null)
		{
			flag = true;
			if (m_treeUnderA != null)
			{
				flag &= m_treeUnderA.IsBlankTree();
			}
			if (m_treeUnderB != null)
			{
				flag &= m_treeUnderB.IsBlankTree();
			}
		}
		return flag;
	}

	public bool IsFullTree()
	{
		if (m_playerA.ProfileData != null && m_playerB.ProfileData != null)
		{
			return true;
		}
		bool flag = false;
		if (m_treeUnderA != null)
		{
			flag |= m_treeUnderA.IsFullTree();
		}
		bool flag2 = false;
		if (m_treeUnderB != null)
		{
			flag2 |= m_treeUnderB.IsFullTree();
		}
		return flag && flag2;
	}

	private bool IsFinal32(NKMTournamentState state)
	{
		NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(NKCTournamentManager.TournamentId);
		switch (state)
		{
		case NKMTournamentState.Ended:
		case NKMTournamentState.Final32:
		case NKMTournamentState.Final4:
		case NKMTournamentState.Closing:
			return true;
		case NKMTournamentState.Progressing:
			if (nKMTournamentTemplet != null)
			{
				return nKMTournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Final32) < NKCSynchronizedTime.ServiceTime;
			}
			return false;
		default:
			return false;
		}
	}

	private bool IsFinal4(NKMTournamentState state)
	{
		NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(NKCTournamentManager.TournamentId);
		switch (state)
		{
		case NKMTournamentState.Ended:
		case NKMTournamentState.Final4:
		case NKMTournamentState.Closing:
			return true;
		case NKMTournamentState.Progressing:
			if (nKMTournamentTemplet != null)
			{
				return nKMTournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Final4) < NKCSynchronizedTime.ServiceTime;
			}
			return false;
		default:
			return false;
		}
	}

	private void OnClickDetail()
	{
		int resultSlotIndex = (m_playerA.SlotIndex - 1) / 2;
		int num = 7;
		if (m_isFinalGroup && m_playerA.SlotIndex >= num)
		{
			NKMTournamentInfo tournamentInfo = NKCTournamentManager.GetTournamentInfo(NKMTournamentGroups.Finals);
			if (tournamentInfo == null)
			{
				tournamentInfo = NKCTournamentManager.GetTournamentInfo(NKMTournamentGroups.GlobalFinals);
			}
			resultSlotIndex = tournamentInfo.slotUserUid.Count - 1;
		}
		if (m_playerA.ProfileData != null || m_playerB.ProfileData != null)
		{
			NKCUITournamentPlayoff.GetInstanceGauntletAsyncReady().Open(resultSlotIndex, m_playerA.TournamentGroup, m_playerA.ProfileData, m_playerB.ProfileData, NKM_GAME_TYPE.NGT_PVE_SIMULATED, m_playerA.IsResultIconActivated() && m_playerB.IsResultIconActivated());
		}
		else if (m_playerA.ProfileDataPredict != null || m_playerB.ProfileDataPredict != null)
		{
			NKCUITournamentPlayoff.GetInstanceGauntletAsyncReady().Open(resultSlotIndex, m_playerA.TournamentGroup, m_playerA.ProfileDataPredict, m_playerB.ProfileDataPredict, NKM_GAME_TYPE.NGT_PVE_SIMULATED, replayActive: false);
		}
	}

	private void OnDestroy()
	{
		ReleaseData();
		m_treeUpper = null;
		m_treeUnderA = null;
		m_treeUnderB = null;
	}
}
