using System;
using System.Collections.Generic;
using ClientPacket.WorldMap;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuWorldmap : NKCUILobbyMenuButtonBase
{
	[Serializable]
	public class WorldmapMissionMoniter
	{
		public enum eMode
		{
			TimeCount,
			CompletedMission
		}

		public GameObject m_objRoot;

		public Image m_imgBG;

		public Text m_lbText;

		public void SetColor(Color colBG, Color colText)
		{
			m_imgBG.color = colBG;
			m_lbText.color = colText;
		}

		public void SetCompletedText(int completedCount)
		{
			NKCUtil.SetGameobjectActive(m_objRoot, bValue: true);
			m_lbText.text = NKCUtilString.GET_STRING_MISSION_COMPLETE + " " + completedCount;
		}

		public void SetActive(bool value)
		{
			NKCUtil.SetGameobjectActive(m_objRoot, value);
		}

		public void UpdateTime(long finishTime)
		{
			m_lbText.text = NKCSynchronizedTime.GetTimeLeftString(finishTime);
		}
	}

	public NKCUIComStateButton m_csbtnMenu;

	public NKCUIComVideoTexture m_VideoTexture;

	public List<WorldmapMissionMoniter> m_lstWorldMapMissionMoniter;

	public GameObject m_objNoOngoingMission;

	public GameObject m_objEvent;

	public GameObject m_objDiveAlert;

	public GameObject m_objShadowAlert;

	public GameObject m_objShadowProgress;

	public Text m_txtShadowLife;

	public GameObject m_objFierceBattleAlert;

	public Text m_txtFierceBattleTimeLeft;

	public GameObject m_objTrimEnterCount;

	public Text m_txtTrimEnterCount;

	public GameObject m_objIsOngoingMission;

	public Text m_txtNumOnGoingMission;

	public GameObject m_objCompleteMission;

	public Text m_txtNumCompleteMission;

	public GameObject m_objHasMissionComplete;

	public GameObject m_objRedDot;

	public Color m_colCompletedBG;

	public Color m_colCompletedText;

	public Color m_colProgressBG;

	public Color m_colProgressText;

	private int m_iCompleteMissionCount;

	private List<long> m_lstCompleteTime = new List<long>();

	private float m_fUpdateTimer = 1f;

	private long GetCompleteTick(int slotIndex)
	{
		int num;
		if (m_iCompleteMissionCount > 0)
		{
			if (slotIndex == 0)
			{
				return -1L;
			}
			num = slotIndex - 1;
		}
		else
		{
			num = slotIndex;
		}
		if (num < m_lstCompleteTime.Count)
		{
			return m_lstCompleteTime[num];
		}
		return 0L;
	}

	public void Init(ContentsType contentsType)
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
			m_ContentsType = contentsType;
			m_iCompleteMissionCount = 0;
			NKCUtil.SetGameobjectActive(m_objEvent, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNoOngoingMission, bValue: true);
			NKCUtil.SetGameobjectActive(m_objHasMissionComplete, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objDiveAlert, bValue: false);
			NKCUtil.SetGameobjectActive(m_objShadowAlert, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFierceBattleAlert, bValue: false);
			NKCUtil.SetGameobjectActive(m_objIsOngoingMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCompleteMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTrimEnterCount, bValue: false);
			for (int i = 0; i < m_lstWorldMapMissionMoniter.Count; i++)
			{
				m_lstWorldMapMissionMoniter[i].SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		if (m_bLocked)
		{
			return;
		}
		m_fUpdateTimer -= Time.deltaTime;
		if (m_fUpdateTimer <= 0f)
		{
			m_fUpdateTimer = 1f;
			while (m_lstCompleteTime.Count > 0 && NKCSynchronizedTime.IsFinished(m_lstCompleteTime[0]))
			{
				m_lstCompleteTime.RemoveAt(0);
				m_iCompleteMissionCount++;
				SetNotify(value: true);
				UpdateCompleteMissionUI();
			}
			UpdateFierceTimeUI();
		}
	}

	protected override void SetNotify(bool value)
	{
		base.SetNotify(value);
		NKCUtil.SetGameobjectActive(m_objRedDot, value);
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		m_lstCompleteTime.Clear();
		m_iCompleteMissionCount = 0;
		foreach (KeyValuePair<int, NKMWorldMapCityData> item in userData.m_WorldmapData.worldMapCityDataMap)
		{
			NKMWorldMapCityData value = item.Value;
			if (value != null && value.HasMission())
			{
				if (value.IsMissionFinished(NKCSynchronizedTime.GetServerUTCTime()))
				{
					m_iCompleteMissionCount++;
				}
				else
				{
					m_lstCompleteTime.Add(value.worldMapMission.completeTime);
				}
			}
		}
		m_lstCompleteTime.Sort();
		UpdateCompleteMissionUI();
		if (m_iCompleteMissionCount > 0)
		{
			SetNotify(value: true);
			return;
		}
		if (userData.m_DiveGameData != null)
		{
			SetNotify(value: true);
		}
		SetNotify(value: false);
	}

	private void SetTimeUI()
	{
		if (m_lstWorldMapMissionMoniter == null)
		{
			return;
		}
		int num = 0;
		if (m_iCompleteMissionCount > 0)
		{
			num = 1;
			m_lstWorldMapMissionMoniter[0].SetActive(value: true);
			m_lstWorldMapMissionMoniter[0].SetColor(m_colCompletedBG, m_colCompletedText);
			m_lstWorldMapMissionMoniter[0].SetCompletedText(m_iCompleteMissionCount);
			NKCUtil.SetGameobjectActive(m_objHasMissionComplete, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objHasMissionComplete, bValue: false);
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		}
		for (int i = num; i < m_lstWorldMapMissionMoniter.Count; i++)
		{
			WorldmapMissionMoniter worldmapMissionMoniter = m_lstWorldMapMissionMoniter[i];
			long completeTick = GetCompleteTick(i);
			if (completeTick == 0L)
			{
				worldmapMissionMoniter.SetActive(value: false);
			}
			else if (completeTick >= 0)
			{
				worldmapMissionMoniter.SetActive(value: true);
				worldmapMissionMoniter.SetColor(m_colProgressBG, m_colProgressText);
				worldmapMissionMoniter.UpdateTime(completeTick);
			}
		}
		m_fUpdateTimer = 1f;
	}

	private void UpdateCompleteMissionUI()
	{
		int count = m_lstCompleteTime.Count;
		if (m_iCompleteMissionCount > 0)
		{
			NKCUtil.SetGameobjectActive(m_objNoOngoingMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objIsOngoingMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCompleteMission, bValue: true);
			NKCUtil.SetLabelText(m_txtNumCompleteMission, string.Format(NKCUtilString.GET_STRING_LOBBY_CITY_MISSION_COMPLETE, m_iCompleteMissionCount));
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: true);
		}
		else if (count > 0)
		{
			NKCUtil.SetGameobjectActive(m_objNoOngoingMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objIsOngoingMission, bValue: true);
			NKCUtil.SetGameobjectActive(m_objCompleteMission, bValue: false);
			NKCUtil.SetLabelText(m_txtNumOnGoingMission, string.Format(NKCUtilString.GET_STRING_LOBBY_CITY_MISSION_ONGOING, count));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objNoOngoingMission, bValue: true);
			NKCUtil.SetGameobjectActive(m_objIsOngoingMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objCompleteMission, bValue: false);
		}
	}

	private void UpdateTimeUI()
	{
		if (m_lstWorldMapMissionMoniter == null)
		{
			return;
		}
		for (int i = 0; i < m_lstWorldMapMissionMoniter.Count; i++)
		{
			WorldmapMissionMoniter worldmapMissionMoniter = m_lstWorldMapMissionMoniter[i];
			long completeTick = GetCompleteTick(i);
			if (completeTick > 0)
			{
				worldmapMissionMoniter.UpdateTime(completeTick);
			}
		}
	}

	private void UpdateFierceTimeUI()
	{
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (m_objFierceBattleAlert != null && m_objFierceBattleAlert.activeSelf && nKCFierceBattleSupportDataMgr != null)
		{
			DateTime dateTime = NKMTime.LocalToUTC(nKCFierceBattleSupportDataMgr.FierceTemplet.FierceGameEnd);
			NKCUtil.SetLabelText(m_txtFierceBattleTimeLeft, $"{NKCSynchronizedTime.GetTimeLeftString(dateTime)}{NKCUtilString.GET_STRING_LOBBY_FIERCEBATTLE_TIME_REMAIN}");
			if (NKCSynchronizedTime.IsFinished(dateTime))
			{
				NKCUtil.SetGameobjectActive(m_objFierceBattleAlert, bValue: false);
			}
		}
	}

	private void OnButton()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.WORLDMAP);
			return;
		}
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetShowIntro();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP, bForce: false);
	}

	public override void CleanUp()
	{
		if (m_VideoTexture != null)
		{
			m_VideoTexture.CleanUp();
		}
	}
}
