using System;
using System.Collections;
using System.Collections.Generic;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUIDungeon : NKCUIResultSubUIBase
{
	public class MissionData
	{
		public DUNGEON_GAME_MISSION_TYPE eMissionType;

		public int iMissionValue;

		public bool bSuccess;
	}

	[Serializable]
	public class MissionUI
	{
		public GameObject m_objRoot;

		public GameObject m_objSuccess;

		public GameObject m_objGray;

		public GameObject m_objCheckSuccess;

		public GameObject m_objCheckFail;

		public Text m_lbMissionDesc;
	}

	[Header("Mission")]
	public List<MissionUI> m_lstMissionUI;

	public Color m_colMissionTextGray;

	public Color m_colMissionTextSuccess;

	[Header("길드 협력전 결과")]
	public GameObject m_objClearPoint;

	public Image m_imgClearPoint;

	public Text m_lbClearPoint;

	public NKCUISlot m_slotArtifact;

	public Text m_lbArtifactDesc;

	public GameObject m_objArtifactGetEffect;

	private List<MissionData> m_lstMissionData;

	private bool m_bFinished;

	public float DelayAfterMission = 0.2f;

	private void SetMissionGray(MissionUI ui, bool bShowMedal)
	{
		NKCUtil.SetGameobjectActive(ui.m_objSuccess, bValue: false);
		NKCUtil.SetGameobjectActive(ui.m_objGray, bShowMedal);
		NKCUtil.SetGameobjectActive(ui.m_objCheckSuccess, bValue: false);
		NKCUtil.SetGameobjectActive(ui.m_objCheckFail, !bShowMedal);
		ui.m_lbMissionDesc.color = m_colMissionTextGray;
	}

	private void SetMissionSuccess(MissionUI ui, bool bShowMedal)
	{
		NKCUtil.SetGameobjectActive(ui.m_objSuccess, bShowMedal);
		NKCUtil.SetGameobjectActive(ui.m_objGray, bValue: false);
		NKCUtil.SetGameobjectActive(ui.m_objCheckSuccess, !bShowMedal);
		NKCUtil.SetGameobjectActive(ui.m_objCheckFail, bValue: false);
		ui.m_lbMissionDesc.color = m_colMissionTextSuccess;
	}

	public void SetMissionData(MissionUI ui, MissionData data)
	{
		ui.m_lbMissionDesc.text = NKCUtilString.GetDGMissionText(data.eMissionType, data.iMissionValue);
	}

	public void SetData(List<MissionData> lstMissionData, bool bShowMedal = true, bool bIgnoreAutoClose = false, bool bShowClearPoint = false, float fClearPoint = 0f)
	{
		if (lstMissionData == null || lstMissionData.Count == 0)
		{
			Debug.Log("MissionData null!");
			base.ProcessRequired = false;
			return;
		}
		m_lstMissionData = lstMissionData;
		for (int i = 0; i < m_lstMissionUI.Count; i++)
		{
			MissionUI missionUI = m_lstMissionUI[i];
			if (i < lstMissionData.Count)
			{
				NKCUtil.SetGameobjectActive(missionUI.m_objRoot, lstMissionData[i].eMissionType != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE);
				SetMissionData(missionUI, lstMissionData[i]);
				if (m_lstMissionData[i].bSuccess)
				{
					SetMissionSuccess(missionUI, bShowMedal);
				}
				else
				{
					SetMissionGray(missionUI, bShowMedal);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(missionUI.m_objRoot, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objClearPoint, bShowClearPoint);
		if (m_objClearPoint.activeSelf)
		{
			NKCUtil.SetLabelText(m_lbClearPoint, string.Format("{0}%", (fClearPoint * 100f).ToString("N0")));
			m_imgClearPoint.fillAmount = fClearPoint;
			GuildDungeonArtifactTemplet nextArtifactTemplet = NKCGuildCoopManager.GetNextArtifactTemplet(NKCGuildCoopManager.GetLastPlayedArenaID());
			if (nextArtifactTemplet != null)
			{
				m_slotArtifact.SetData(NKCUISlot.SlotData.MakeGuildArtifactData(nextArtifactTemplet.GetArtifactId()));
				NKCUtil.SetLabelText(m_lbArtifactDesc, nextArtifactTemplet.GetDescFull());
				NKCUtil.SetGameobjectActive(m_objArtifactGetEffect, NKCGuildCoopManager.m_bGetArtifact);
			}
		}
		base.ProcessRequired = true;
		m_bIgnoreAutoClose = bIgnoreAutoClose;
	}

	public override bool IsProcessFinished()
	{
		return m_bFinished;
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		m_bFinished = false;
		yield return new WaitForSeconds(DelayAfterMission);
		m_bFinished = true;
	}

	public override void FinishProcess()
	{
		m_bFinished = true;
		if (base.ProcessRequired)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		}
	}
}
