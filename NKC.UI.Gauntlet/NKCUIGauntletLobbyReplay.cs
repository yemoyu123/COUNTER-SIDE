using System.Collections.Generic;
using ClientPacket.Pvp;
using Cs.Logging;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyReplay : MonoBehaviour
{
	public Animator m_animator;

	[Header("상단 탭")]
	public NKCUIComToggle m_ctglTabDefault;

	[Header("스크롤 관련")]
	public GameObject m_objScrollDefault;

	public LoopVerticalScrollRect m_lvsrDefault;

	public Transform m_trDefault;

	[Header("재생하기")]
	public NKCUIComStateButton m_csbtnPlaySelected;

	private bool m_bFirstOpen = true;

	private bool m_bPrepareLoopScrollCells;

	private List<ReplayData> m_listReplayData = new List<ReplayData>();

	public void Init()
	{
		NKCReplayMgr.IsReplayRecordingOpened();
		if (NKCReplayMgr.IsReplayOpened())
		{
			m_csbtnPlaySelected.PointerClick.RemoveAllListeners();
			m_ctglTabDefault.OnValueChanged.RemoveAllListeners();
			m_lvsrDefault.dOnGetObject += GetObject;
			m_lvsrDefault.dOnReturnObject += ReturnSlot;
			m_lvsrDefault.dOnProvideData += ProvideData;
			m_lvsrDefault.ContentConstraintCount = 1;
			NKCUtil.SetScrollHotKey(m_lvsrDefault);
		}
	}

	private void OnClickPlaySelected(int replayDataIndex)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.FIREBASE_CRASH_TEST))
		{
			OnClickPlaySelected(replayDataIndex);
		}
		if (m_listReplayData.Count <= replayDataIndex)
		{
			Log.Error($"[ReplayLobby] Play failed! index[{replayDataIndex}] count[{m_listReplayData.Count}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLobbyReplay.cs", 76);
		}
		else
		{
			NKCScenManager.GetScenManager().GetNKCReplayMgr().StartPlaying(m_listReplayData[replayDataIndex]);
		}
	}

	private void OnClickSelectReplayData(int replayDataIndex)
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.FIREBASE_CRASH_TEST))
		{
			((List<int>)null)[0] = 1;
		}
	}

	public RectTransform GetObject(int index)
	{
		return NKCUIGauntletReplaySlot.GetNewInstance(m_trDefault, OnClickSelectReplayData, OnClickPlaySelected).GetComponent<RectTransform>();
	}

	public void ReturnSlot(Transform tr)
	{
		NKCUIGauntletReplaySlot component = tr.GetComponent<NKCUIGauntletReplaySlot>();
		tr.SetParent(base.transform);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideData(Transform tr, int index)
	{
		NKCUIGauntletReplaySlot component = tr.GetComponent<NKCUIGauntletReplaySlot>();
		if (component != null)
		{
			if (m_listReplayData.Count <= index)
			{
				Debug.LogError($"Async PVP data 이상함. target : {m_listReplayData.Count} <= {index}");
			}
			component.SetUI(index, m_listReplayData[index]);
		}
	}

	private void RefreshScrollRect()
	{
		m_listReplayData.Clear();
		m_lvsrDefault.TotalCount = m_listReplayData.Count;
		m_lvsrDefault.RefreshCells();
	}

	public void SetUI()
	{
		if (NKCReplayMgr.IsReplayOpened())
		{
			if (!m_bPrepareLoopScrollCells)
			{
				NKCUtil.SetGameobjectActive(m_objScrollDefault, bValue: true);
				m_lvsrDefault.PrepareCells();
				m_bPrepareLoopScrollCells = true;
			}
			if (m_bFirstOpen)
			{
				m_bFirstOpen = false;
			}
			m_ctglTabDefault.Select(bSelect: false, bForce: true);
			m_ctglTabDefault.Select(bSelect: true);
			RefreshScrollRect();
		}
	}

	public void Close()
	{
		m_bFirstOpen = true;
		NKCPopupGauntletBanList.CheckInstanceAndClose();
	}

	public void ClearCacheData()
	{
		if (NKCReplayMgr.IsReplayOpened())
		{
			m_lvsrDefault.ClearCells();
		}
	}
}
