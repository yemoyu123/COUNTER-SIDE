using System.Text;
using ClientPacket.Warfare;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuOperation : NKCUILobbyMenuButtonBase
{
	private const int MAX_MISSIONNAME_COUNT = 9;

	public NKCUIComStateButton m_csbtnMenu;

	public NKCUIComStateButton m_csbtnMoveToOngoingMission;

	public GameObject m_objNoOngoingMission;

	public Text m_lbMissionOnProgress;

	public GameObject m_objEvent;

	public NKCUIComVideoTexture m_VideoTexture;

	private NKMUserData m_UserData;

	public void Init()
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
		}
		if (m_csbtnMoveToOngoingMission != null)
		{
			m_csbtnMoveToOngoingMission.PointerClick.RemoveAllListeners();
			m_csbtnMoveToOngoingMission.PointerClick.AddListener(OnMoveToOngoingMission);
		}
	}

	private void UpdateWFOnGoingUI(NKMUserData userData)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData == null)
		{
			return;
		}
		bool flag = false;
		if (warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
			if (nKMWarfareTemplet != null)
			{
				flag = true;
				NKCUtil.SetLabelText(m_lbMissionOnProgress, MakeWFString(nKMWarfareTemplet));
			}
		}
		NKCUtil.SetGameobjectActive(m_csbtnMoveToOngoingMission, flag);
		NKCUtil.SetGameobjectActive(m_objNoOngoingMission, !flag);
		SetNotify(flag);
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		m_UserData = userData;
		NKCUtil.SetGameobjectActive(m_objEvent, bValue: false);
		if (m_VideoTexture != null && !m_VideoTexture.IsPlaying)
		{
			m_VideoTexture.Play(bLoop: true);
		}
		UpdateWFOnGoingUI(userData);
	}

	private string MakeWFString(NKMWarfareTemplet cNKMWarfareTemplet)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string wFEpisodeNumber = NKCUtilString.GetWFEpisodeNumber(cNKMWarfareTemplet);
		if (!string.IsNullOrEmpty(wFEpisodeNumber))
		{
			stringBuilder.Append("<color=#888888>");
			stringBuilder.Append(wFEpisodeNumber);
			stringBuilder.Append("</color> ");
		}
		if (cNKMWarfareTemplet.GetWarfareName().Length > 9)
		{
			stringBuilder.Append(cNKMWarfareTemplet.GetWarfareName().Substring(0, 9));
			stringBuilder.Append("..");
		}
		else
		{
			stringBuilder.Append(cNKMWarfareTemplet.GetWarfareName());
		}
		return stringBuilder.ToString();
	}

	private void OnButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION, bForce: false);
	}

	private void OnMoveToOngoingMission()
	{
		if (NKCScenManager.GetScenManager().WarfareGameData.warfareGameState == NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			return;
		}
		int warfareTempletID = NKCScenManager.GetScenManager().WarfareGameData.warfareTempletID;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareTempletID);
		if (nKMWarfareTemplet == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_WARFARE_TEMPLET));
			Debug.LogError("can't find warfare templet, warfare templet ID : " + warfareTempletID);
			return;
		}
		if (nKMWarfareTemplet.MapTemplet == null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, NKCStringTable.GetString(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_FIND_WARFARE_MAP_TEMPLET));
			Debug.LogError("can't find warfare map templet, warfare templet ID : " + warfareTempletID);
			return;
		}
		NKC_SCEN_WARFARE_GAME nKC_SCEN_WARFARE_GAME = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME();
		if (nKC_SCEN_WARFARE_GAME != null)
		{
			int warfareTempletID2 = NKCScenManager.GetScenManager().WarfareGameData.warfareTempletID;
			nKC_SCEN_WARFARE_GAME.SetWarfareStrID(NKCWarfareManager.GetWarfareStrID(warfareTempletID2));
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WARFARE_GAME);
		}
	}

	public override void CleanUp()
	{
		if (m_VideoTexture != null)
		{
			m_VideoTexture.CleanUp();
		}
	}
}
