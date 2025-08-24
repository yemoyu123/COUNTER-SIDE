using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using NKM.Guild;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Guild;

public class NKCUIGuildCoopBack : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	public delegate void OnClickArena(GuildDungeonInfoTemplet templet);

	public delegate void OnClickBoss(int bossID);

	public List<NKCUIGuildCoopArena> m_lstArenaPin = new List<NKCUIGuildCoopArena>();

	public NKCUIGuildCoopBoss m_RaidPin = new NKCUIGuildCoopBoss();

	public float m_fCameraXPosAddValue = 150f;

	public float m_fCameraZPosZoomIn = -300f;

	public float m_fCameraZPosZoomOut = -676f;

	public Vector2 m_vCameraMoveRange;

	private bool m_bEnableDrag = true;

	private Vector2 currentCameraPos;

	private GuildSeasonTemplet m_GuildSeasonTemplet;

	private GuildSeasonTemplet.SessionData m_CurSessionData;

	private Dictionary<int, NKCUIGuildCoopArena> m_dicArena = new Dictionary<int, NKCUIGuildCoopArena>();

	public void SetEnableDrag(bool bSet)
	{
		m_bEnableDrag = bSet;
	}

	public void Init(int seasonID, OnClickArena onClickArena, OnClickBoss onClickBoss)
	{
		m_GuildSeasonTemplet = GuildDungeonTempletManager.GetGuildSeasonTemplet(NKCGuildCoopManager.m_SeasonId);
		if (m_GuildSeasonTemplet == null)
		{
			Log.Error($"GuildSeasonTepmlet is null - id : {seasonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/GuildCoop/NKCUIGuildCoopBack.cs", 38);
			return;
		}
		m_CurSessionData = m_GuildSeasonTemplet.GetCurrentSession(ServiceTime.Recent);
		if (m_CurSessionData.SessionId == 0)
		{
			Log.Error($"SessionData is null - SeasonId : {seasonID}, curTime : {ServiceTime.Recent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/GuildCoop/NKCUIGuildCoopBack.cs", 45);
			return;
		}
		GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		List<GuildDungeonInfoTemplet> dungeonInfoList = GuildDungeonTempletManager.GetDungeonInfoList(m_GuildSeasonTemplet.GetSeasonDungeonGroup());
		List<int> lstDungeonId = m_CurSessionData.templet.GetDungeonList();
		int i;
		for (i = 0; i < m_lstArenaPin.Count; i++)
		{
			m_lstArenaPin[i].InitUI(onClickArena);
			GuildDungeonInfoTemplet guildDungeonInfoTemplet = dungeonInfoList.FindAll((GuildDungeonInfoTemplet x) => x.GetArenaIndex() == i + 1).Find((GuildDungeonInfoTemplet x) => lstDungeonId.Contains(x.GetSeasonDungeonId()));
			m_lstArenaPin[i].SetData(guildDungeonInfoTemplet);
			if (guildDungeonInfoTemplet != null)
			{
				m_dicArena.Add(guildDungeonInfoTemplet.GetArenaIndex(), m_lstArenaPin[i]);
			}
		}
		if (m_RaidPin != null)
		{
			m_RaidPin.InitUI(onClickBoss);
		}
		base.gameObject.SetActive(value: false);
	}

	public void SetData()
	{
		List<GuildDungeonInfoTemplet> dungeonInfoList = GuildDungeonTempletManager.GetDungeonInfoList(m_GuildSeasonTemplet.GetSeasonDungeonGroup());
		List<int> lstDungeonId = m_CurSessionData.templet.GetDungeonList();
		int i;
		for (i = 0; i < m_lstArenaPin.Count; i++)
		{
			GuildDungeonInfoTemplet data = dungeonInfoList.FindAll((GuildDungeonInfoTemplet x) => x.GetArenaIndex() == i + 1).Find((GuildDungeonInfoTemplet x) => lstDungeonId.Contains(x.GetSeasonDungeonId()));
			m_lstArenaPin[i].SetData(data);
		}
		List<GuildRaidTemplet> raidTempletList = GuildDungeonTempletManager.GetRaidTempletList(m_GuildSeasonTemplet.GetSeasonRaidGroup());
		m_RaidPin.SetData(raidTempletList?.Find((GuildRaidTemplet x) => x.GetStageId() == NKCGuildCoopManager.m_cGuildRaidTemplet.GetStageId()), raidTempletList.Max((GuildRaidTemplet e) => e.GetStageIndex()));
		NKCCamera.StopTrackingCamera();
		NKCCamera.GetTrackingPos().SetNowValue(0f, 0f, m_fCameraZPosZoomOut);
		base.gameObject.SetActive(value: true);
	}

	public Vector3 GetTargetPosition(int arenaIdx, bool bIsArena = true)
	{
		if (!bIsArena)
		{
			return new Vector3(m_RaidPin.transform.position.x, m_RaidPin.transform.position.y, m_fCameraZPosZoomOut);
		}
		if (m_dicArena.ContainsKey(arenaIdx))
		{
			return new Vector3(m_dicArena[arenaIdx].transform.position.x, m_dicArena[arenaIdx].transform.position.y, m_fCameraZPosZoomOut);
		}
		return new Vector3(0f, 0f, m_fCameraZPosZoomOut);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	public void OnDrag(PointerEventData pointData)
	{
		if (m_bEnableDrag)
		{
			float value = NKCCamera.GetPosNowX() - pointData.delta.x * 10f;
			float value2 = NKCCamera.GetPosNowY() - pointData.delta.y * 10f;
			value = Mathf.Clamp(value, 0f - m_vCameraMoveRange.x, m_vCameraMoveRange.x);
			value2 = Mathf.Clamp(value2, 0f - m_vCameraMoveRange.y, m_vCameraMoveRange.y);
			NKCCamera.TrackingPos(1f, value, value2);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
	}

	public void RefreshArenaSlot(int idx)
	{
		if (m_dicArena.ContainsKey(idx))
		{
			m_dicArena[idx].Refresh();
		}
	}

	public void RefreshBossSlot()
	{
		m_RaidPin.Refresh();
	}
}
