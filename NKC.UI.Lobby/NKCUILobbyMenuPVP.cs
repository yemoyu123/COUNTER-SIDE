using ClientPacket.Pvp;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuPVP : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objNotify;

	public Text m_lbSeasonDesc;

	public Text m_lbPVPAsyncTicketCount;

	public Text m_lbRemainPVPPoint;

	public GameObject m_objPVPPointIcon;

	public GameObject m_objLeagueOpenTag;

	private float m_fPrevUpdateTime;

	public void Init(ContentsType contentsType)
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
			m_ContentsType = contentsType;
			NKCUtil.SetLabelText(m_lbSeasonDesc, string.Empty);
			NKCUtil.SetLabelText(m_lbPVPAsyncTicketCount, string.Empty);
			NKCUtil.SetLabelText(m_lbRemainPVPPoint, string.Empty);
			NKCUtil.SetGameobjectActive(m_objPVPPointIcon, bValue: false);
		}
	}

	private void Update()
	{
		if (!m_bLocked && m_fPrevUpdateTime + 1f < Time.time)
		{
			ContentsUpdate(NKCScenManager.GetScenManager().GetMyUserData());
			m_fPrevUpdateTime = Time.time;
		}
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		bool flag = NKCAlarmManager.CheckPVPNotify(userData);
		NKCUtil.SetGameobjectActive(m_objNotify, flag);
		NKMPvpRankSeasonTemplet pvpRankSeasonTemplet = NKCPVPManager.GetPvpRankSeasonTemplet(NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime()));
		if (pvpRankSeasonTemplet != null)
		{
			if (!NKCSynchronizedTime.IsFinished(pvpRankSeasonTemplet.EndDate))
			{
				NKCUtil.SetLabelText(m_lbSeasonDesc, NKCUtilString.GetRemainTimeString(pvpRankSeasonTemplet.EndDate, 1));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbSeasonDesc, NKCUtilString.GET_STRING_TIME_CLOSING);
			}
		}
		if (userData != null && userData.m_AsyncData != null)
		{
			long countMiscItem = NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(13);
			NKCUtil.SetLabelText(m_lbPVPAsyncTicketCount, string.Format($"{countMiscItem}/{NKMPvpCommonConst.Instance.AsyncTicketMaxCount}"));
		}
		if (userData != null)
		{
			long countMiscItem2 = userData.m_InventoryData.GetCountMiscItem(6);
			int cHARGE_POINT_MAX_COUNT = NKMPvpCommonConst.Instance.CHARGE_POINT_MAX_COUNT;
			NKCUtil.SetLabelText(m_lbRemainPVPPoint, $"{countMiscItem2}/{cHARGE_POINT_MAX_COUNT}");
		}
		NKCUtil.SetGameobjectActive(m_objLeagueOpenTag, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPVPPointIcon, bValue: true);
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY().ProcessPVPPointCharge();
		SetNotify(flag);
	}

	private void OnButton()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.PVP);
		}
		else
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_INTRO, bForce: false);
		}
	}

	public void OnRecv(NKMPacket_PVP_CHARGE_POINT_REFRESH_ACK cNKMPacket_PVP_CHARGE_POINT_REFRESH_ACK)
	{
		UpdateData(NKCScenManager.GetScenManager().GetMyUserData());
	}
}
