using ClientPacket.Mode;
using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_DIVE : NKC_SCEN_BASIC
{
	private GameObject m_NUM_DIVE;

	private GameObject m_NUF_DIVE;

	private NKC_SCEN_DIVE_UI_DATA m_NKC_SCEN_DIVE_UI_DATA = new NKC_SCEN_DIVE_UI_DATA();

	private NKCDiveGame m_NKCDiveGame;

	public NKC_SCEN_DIVE()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_DIVE;
		m_NUM_DIVE = GameObject.Find("NUM_DIVE");
		m_NUF_DIVE = GameObject.Find("NUF_DIVE");
	}

	public NKCDiveGame GetDiveGame()
	{
		return m_NKCDiveGame;
	}

	public void OnLoginSuccess()
	{
		SetIntro(bSet: false);
		SetSectorAddEvent(bSet: false);
		SetSectorAddEventWhenStart(bSet: false);
	}

	public void SetIntro(bool bSet = true)
	{
		NKCDiveGame.SetIntro(bSet);
	}

	public void SetSectorAddEvent(bool bSet = true)
	{
		NKCDiveGame.SetSectorAddEvent(bSet);
	}

	public void SetSectorAddEventWhenStart(bool bSet = true)
	{
		NKCDiveGame.SetSectorAddEventWhenStart(bSet);
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		m_NUM_DIVE.SetActive(value: true);
		m_NUF_DIVE.SetActive(value: true);
		if (!m_bLoadedUI)
		{
			if (m_NKC_SCEN_DIVE_UI_DATA.m_NUM_DIVE_PREFAB == null)
			{
				m_NKC_SCEN_DIVE_UI_DATA.m_NUM_DIVE_PREFAB = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NUM_DIVE_PREFAB", bAsync: true);
			}
			if (m_NKC_SCEN_DIVE_UI_DATA.m_NUF_DIVE_PREFAB == null)
			{
				m_NKC_SCEN_DIVE_UI_DATA.m_NUF_DIVE_PREFAB = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NUF_DIVE_PREFAB", bAsync: true);
			}
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (!m_bLoadedUI)
		{
			m_NKC_SCEN_DIVE_UI_DATA.m_NUM_DIVE_PREFAB.m_Instant.transform.SetParent(m_NUM_DIVE.transform, worldPositionStays: false);
			m_NKC_SCEN_DIVE_UI_DATA.m_NUF_DIVE_PREFAB.m_Instant.transform.SetParent(m_NUF_DIVE.transform, worldPositionStays: false);
			m_NKCDiveGame = NKCDiveGame.Init();
		}
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_NKCDiveGame.Open();
	}

	public override void ScenEnd()
	{
		m_NKCDiveGame.Close();
		base.ScenEnd();
		m_NUM_DIVE.SetActive(value: false);
		m_NUF_DIVE.SetActive(value: false);
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_NKCDiveGame = null;
		m_NKC_SCEN_DIVE_UI_DATA.Init();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public void OnRecv(NKMPacket_DIVE_SUICIDE_ACK cNKMPacket_DIVE_SUICIDE_ACK)
	{
		m_NKCDiveGame.OnRecv(cNKMPacket_DIVE_SUICIDE_ACK);
	}

	public void OnRecv(NKMPacket_DIVE_SELECT_ARTIFACT_ACK cNKMPacket_DIVE_SELECT_ARTIFACT_ACK)
	{
		m_NKCDiveGame.OnRecv(cNKMPacket_DIVE_SELECT_ARTIFACT_ACK);
	}

	public void OnRecv(NKMPacket_DIVE_MOVE_FORWARD_ACK cNKMPacket_DIVE_MOVE_FORWARD_ACK)
	{
		m_NKCDiveGame.OnRecv(cNKMPacket_DIVE_MOVE_FORWARD_ACK);
	}

	public void OnRecv(NKMPacket_DIVE_GIVE_UP_ACK cNKMPacket_DIVE_GIVE_UP_ACK, bool bEventDive)
	{
		if (bEventDive)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReservedDiveReverseAni(bSet: true);
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
		}
		else
		{
			NKCUtil.SetDiveTargetEventID();
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DIVE_READY);
		}
	}

	public void OnRecv(NKMPacket_DIVE_AUTO_ACK cNKMPacket_DIVE_AUTO_ACK)
	{
		m_NKCDiveGame.OnRecv(cNKMPacket_DIVE_AUTO_ACK);
	}

	public void OnRecv(NKMPacket_DIVE_EXPIRE_NOT cNKMPacket_DIVE_EXPIRE_NOT)
	{
		m_NKCDiveGame.OnRecv(cNKMPacket_DIVE_EXPIRE_NOT);
	}

	public void TryGiveUp()
	{
		if (m_NKCDiveGame != null)
		{
			m_NKCDiveGame.GiveUp();
		}
	}

	public void TryTempLeave()
	{
		if (m_NKCDiveGame != null)
		{
			m_NKCDiveGame.TempLeave();
		}
	}

	public void DoAfterLogout()
	{
		NKCDiveGame.SetReservedUnitDieShow(bSet: false);
	}
}
