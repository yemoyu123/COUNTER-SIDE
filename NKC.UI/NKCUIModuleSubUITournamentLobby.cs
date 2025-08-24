using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Core.Util;
using NKC.UI.Event;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIModuleSubUITournamentLobby : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "UI_SINGLE_TOURNAMENT";

	private const string UI_ASSET_NAME = "UI_SINGLE_TOURNAMENT_LOBBY";

	private static NKCUIModuleSubUITournamentLobby m_Instance;

	public NKCUIModuleSubUITournamentTryout m_Tryout;

	public NKCUITournamentPlayoff m_Playoff;

	public NKCUIComToggle m_tglTryout;

	public NKCUIComToggle m_tglPlayoff;

	private float m_fDeltaTime;

	public static NKCUIModuleSubUITournamentLobby Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIModuleSubUITournamentLobby>("UI_SINGLE_TOURNAMENT", "UI_SINGLE_TOURNAMENT_LOBBY", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIModuleSubUITournamentLobby>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "";

	public override List<int> UpsideMenuShowResourceList => new List<int>();

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public static bool CheckInstance()
	{
		return m_Instance != null;
	}

	private void InitUI()
	{
		m_Tryout.InitUI();
		m_Playoff.Init();
		m_tglTryout.OnValueChanged.RemoveAllListeners();
		m_tglTryout.OnValueChanged.AddListener(OnClickTryout);
		m_tglTryout.m_bGetCallbackWhileLocked = true;
		m_tglPlayoff.OnValueChanged.RemoveAllListeners();
		m_tglPlayoff.OnValueChanged.AddListener(OnClickPlayoff);
		m_tglPlayoff.m_bGetCallbackWhileLocked = true;
	}

	public override void CloseInternal()
	{
		m_Playoff.ReleaseData();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		if (m_Playoff.IsCheerMode())
		{
			m_Playoff.ToggleCheerMode(resetCheering: true);
		}
		else if (m_Playoff.IsCheerResuleMode())
		{
			m_Playoff.Open(keepCheerState: false);
		}
		else
		{
			base.OnBackButton();
		}
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_fDeltaTime = 0f;
		SetData();
		if (ServiceTime.Now > NKCTournamentManager.GetTournamentStateStartDate(NKMTournamentState.Final32))
		{
			m_tglPlayoff.Select(bSelect: true, bForce: true);
			OnClickPlayoff(bValue: true);
		}
		else
		{
			m_tglTryout.Select(bSelect: true, bForce: true);
			OnClickTryout(bValue: true);
		}
		UIOpened();
	}

	public void OpenTryout()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetData();
		m_tglTryout.Select(bSelect: true, bForce: true);
		OnClickTryout(bValue: true);
		UIOpened();
	}

	public bool IsPlayoffOpened()
	{
		return m_Playoff.gameObject.activeSelf;
	}

	public void ShowPlayoff(bool keepCheerState)
	{
		m_Tryout.Close();
		m_Playoff.Open(keepCheerState);
	}

	public void StartCheerCoolTime()
	{
		m_Playoff.StartCheerCoolTime();
	}

	private void SetData()
	{
		m_tglTryout.UnLock();
		if (!CanEnterPlayoff())
		{
			m_tglPlayoff.Lock();
		}
		else
		{
			m_tglPlayoff.UnLock();
		}
	}

	private bool CanEnterPlayoff()
	{
		NKMTournamentState tournamentState = NKCTournamentManager.GetTournamentState();
		NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(NKCTournamentManager.TournamentId);
		switch (tournamentState)
		{
		case NKMTournamentState.Ended:
		case NKMTournamentState.Final32:
		case NKMTournamentState.Final4:
		case NKMTournamentState.Closing:
			return true;
		case NKMTournamentState.Progressing:
			if (nKMTournamentTemplet != null)
			{
				return nKMTournamentTemplet.GetTournamentStateStartDate(NKMTournamentState.Final32) < ServiceTime.Now;
			}
			return false;
		default:
			return false;
		}
	}

	private void OnClickTryout(bool bValue)
	{
		if (bValue)
		{
			m_Tryout.Open();
			m_Playoff.Close();
		}
	}

	private void OnClickPlayoff(bool bValue)
	{
		if (m_tglPlayoff.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_TOURNAMENT_GROUP_ENTER_FAIL, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (bValue)
		{
			NKCPacketSender.Send_NKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_REQ(NKCTournamentManager.TournamentId);
		}
	}

	public void Refresh()
	{
		SetData();
		if (m_Tryout.gameObject.activeInHierarchy)
		{
			m_Tryout.Refresh();
		}
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > 1f)
		{
			m_fDeltaTime -= 1f;
			if (NKCTournamentManager.m_TournamentInfoChanged)
			{
				NKCTournamentManager.SetTournamentInfoChanged(bChanged: false);
				NKCPacketSender.Send_NKMPacket_TOURNAMENT_INFO_REQ();
			}
		}
	}
}
