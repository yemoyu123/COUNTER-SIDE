using System;
using ClientPacket.Mode;
using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_DIVE_READY : NKC_SCEN_BASIC
{
	private GameObject m_NUF_DIVE_READY;

	private NKC_SCEN_DIVE_READY_UI_DATA m_NKC_SCEN_DIVE_READY_UI_DATA = new NKC_SCEN_DIVE_READY_UI_DATA();

	private NKCUIDeckViewer m_NKCUIDeckView;

	private NKCUIDiveReady m_NKCUIDiveReady;

	private bool m_bGetResetTicketChargeDate;

	private DateTime m_ResetTicketChargeDate;

	private int m_reservedCityID;

	private int m_reservedEventDiveID;

	public void SetResetTicketChargeDate(DateTime _ResetTicketChargeDate)
	{
		m_bGetResetTicketChargeDate = true;
		m_ResetTicketChargeDate = _ResetTicketChargeDate;
	}

	public NKC_SCEN_DIVE_READY()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_DIVE_READY;
		m_NUF_DIVE_READY = GameObject.Find("NUF_DIVE_READY");
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		m_NUF_DIVE_READY.SetActive(value: true);
		if (!m_bLoadedUI && m_NKC_SCEN_DIVE_READY_UI_DATA.m_NUF_LOGIN_PREFAB == null)
		{
			m_NKC_SCEN_DIVE_READY_UI_DATA.m_NUF_LOGIN_PREFAB = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NUF_DIVE_READY_PREFAB", bAsync: true);
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (!m_bLoadedUI)
		{
			m_NKC_SCEN_DIVE_READY_UI_DATA.m_NUF_LOGIN_PREFAB.m_Instant.transform.SetParent(m_NUF_DIVE_READY.transform, worldPositionStays: false);
			m_NKCUIDeckView = NKCUIDeckViewer.Instance;
			m_NKCUIDiveReady = NKCUIDiveReady.InitUI();
		}
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		m_NKCUIDeckView.LoadComplete();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		DoAfterGettingResetTicketChargeDate();
	}

	public void Refresh()
	{
		if (m_NKCUIDiveReady != null && m_NKCUIDiveReady.IsOpen)
		{
			m_NKCUIDiveReady.Refresh();
		}
	}

	public void DoAfterGettingResetTicketChargeDate()
	{
		if (m_NKCUIDiveReady != null)
		{
			m_NKCUIDiveReady.Open(m_reservedCityID, m_reservedEventDiveID, m_ResetTicketChargeDate);
		}
	}

	public void DoAfterLogout()
	{
		m_bGetResetTicketChargeDate = false;
	}

	public void SetTargetEventID(int cityID, int eventDiveID)
	{
		m_reservedCityID = cityID;
		m_reservedEventDiveID = eventDiveID;
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_NKCUIDeckView.IsOpen)
		{
			m_NKCUIDeckView.Close();
		}
		m_NKCUIDiveReady.Close();
		m_NUF_DIVE_READY.SetActive(value: false);
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_NKCUIDeckView = null;
		m_NKCUIDiveReady = null;
		m_NKC_SCEN_DIVE_READY_UI_DATA.Init();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
		if (!NKCCamera.IsTrackingCameraPos())
		{
			NKCCamera.TrackingPos(10f, NKMRandom.Range(-50f, 50f), NKMRandom.Range(-50f, 50f), NKMRandom.Range(-1000f, -900f));
		}
	}

	public void OnRecv(NKMPacket_DIVE_GIVE_UP_ACK cNKMPacket_DIVE_GIVE_UP_ACK)
	{
		m_NKCUIDiveReady.OnRecv(cNKMPacket_DIVE_GIVE_UP_ACK);
	}

	public void OnRecv(NKMPacket_DIVE_EXPIRE_NOT cNKMPacket_DIVE_EXPIRE_NOT)
	{
		m_NKCUIDiveReady.OnRecv(cNKMPacket_DIVE_EXPIRE_NOT);
	}
}
