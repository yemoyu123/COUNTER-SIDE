using System.Collections.Generic;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCUICutScenLogViewer : MonoBehaviour
{
	public delegate void OnButton(bool bTitleEnabled, bool bTalkBoxEnabled, bool bAutoEnabled);

	public NKCUICutScenLogViewerSlot m_pfbSlot;

	public LoopScrollRect m_loopScroll;

	public Transform m_trSlotParent;

	public NKCUIComStateButton m_btnClose;

	public Text m_txtEP;

	public Text m_txtStage;

	private List<NKCUICutScenLogViewerSlot> m_lstSlot = new List<NKCUICutScenLogViewerSlot>();

	private Stack<NKCUICutScenLogViewerSlot> m_stkSlot = new Stack<NKCUICutScenLogViewerSlot>();

	private List<NKCUICutScenPlayer.CutsceneLog> m_lstData = new List<NKCUICutScenPlayer.CutsceneLog>();

	private bool m_bInitComplete;

	private bool m_bTitleEnabled;

	private bool m_bTalkBoxEnabled;

	private bool m_bAutoEnabled;

	private OnButton dOnCloseButton;

	private NKCUICutScenPlayer.OnVoice dOnVoice;

	private void NullReferenceCheck()
	{
		if (m_pfbSlot == null)
		{
			Debug.LogError("m_slot is null");
		}
		if (m_loopScroll == null)
		{
			Debug.LogError("m_loopScroll is null");
		}
		if (m_btnClose == null)
		{
			Debug.LogError("m_btnClose is null");
		}
	}

	private void InitUI()
	{
		NullReferenceCheck();
		m_loopScroll.SetUseHack(bSet: false);
		m_loopScroll.SetNoMakeSlotTwo(bSet: true);
		m_loopScroll.dOnGetObject += GetObject;
		m_loopScroll.dOnReturnObject += ReturnObject;
		m_loopScroll.dOnProvideData += ProvideData;
		m_loopScroll.dOnScrollEvent += OnScrollEvent;
		m_loopScroll.ContentConstraintCount = 1;
		m_loopScroll.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loopScroll);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(OnClickClose);
		m_bInitComplete = true;
	}

	public void OpenUI(List<NKCUICutScenPlayer.CutsceneLog> lstDesc, OnButton onCloseButton, NKCUICutScenPlayer.OnVoice onVoice, bool bTitleEnabled, bool bTalkBoxEnabled, bool bAutoEnabled, NKMStageTempletV2 stageTemplet)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		dOnCloseButton = onCloseButton;
		dOnVoice = onVoice;
		m_bTitleEnabled = bTitleEnabled;
		m_bTalkBoxEnabled = bTalkBoxEnabled;
		m_bAutoEnabled = bAutoEnabled;
		m_lstData.Clear();
		m_lstData.AddRange(lstDesc);
		m_loopScroll.TotalCount = m_lstData.Count;
		m_loopScroll.SetIndexPosition(m_lstData.Count - 1);
		m_txtEP.text = "";
		m_txtStage.text = "";
		if (stageTemplet == null)
		{
			return;
		}
		string text = string.Empty;
		switch (stageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_WARFARE:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(stageTemplet.m_StageBattleStrID);
			if (nKMWarfareTemplet != null)
			{
				text = nKMWarfareTemplet.GetWarfareName();
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
			if (stageTemplet.PhaseTemplet != null)
			{
				text = stageTemplet.PhaseTemplet.GetName();
			}
			break;
		default:
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(stageTemplet.m_StageBattleStrID);
			if (dungeonTempletBase != null)
			{
				text = dungeonTempletBase.GetDungeonName();
			}
			break;
		}
		}
		if (!string.IsNullOrEmpty(text))
		{
			m_txtEP.text = stageTemplet.EpisodeTemplet.GetEpisodeTitle();
			m_txtStage.text = $"{stageTemplet.ActId}-{stageTemplet.m_StageUINum} {text}";
		}
	}

	public void OnClickClose()
	{
		m_lstData.Clear();
		dOnCloseButton?.Invoke(m_bTitleEnabled, m_bTalkBoxEnabled, m_bAutoEnabled);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private RectTransform GetObject(int index)
	{
		if (m_stkSlot.Count == 0)
		{
			NKCUICutScenLogViewerSlot nKCUICutScenLogViewerSlot = Object.Instantiate(m_pfbSlot, m_trSlotParent);
			m_lstSlot.Add(nKCUICutScenLogViewerSlot);
			nKCUICutScenLogViewerSlot.transform.localScale = Vector3.one;
			NKCUtil.SetGameobjectActive(nKCUICutScenLogViewerSlot.gameObject, bValue: true);
			return nKCUICutScenLogViewerSlot.GetComponent<RectTransform>();
		}
		NKCUICutScenLogViewerSlot nKCUICutScenLogViewerSlot2 = m_stkSlot.Pop();
		m_lstSlot.Add(nKCUICutScenLogViewerSlot2);
		NKCUtil.SetGameobjectActive(nKCUICutScenLogViewerSlot2.gameObject, bValue: true);
		nKCUICutScenLogViewerSlot2.transform.SetParent(m_trSlotParent);
		return nKCUICutScenLogViewerSlot2.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		NKCUICutScenLogViewerSlot component = go.GetComponent<NKCUICutScenLogViewerSlot>();
		if (m_lstSlot.Contains(component))
		{
			m_lstSlot.Remove(component);
		}
		if (!m_stkSlot.Contains(component))
		{
			m_stkSlot.Push(component);
		}
		NKCUtil.SetGameobjectActive(component.gameObject, bValue: false);
		go.SetParent(base.transform);
	}

	private void ProvideData(Transform transform, int idx)
	{
		NKCUICutScenLogViewerSlot component = transform.GetComponent<NKCUICutScenLogViewerSlot>();
		NKCUtil.SetGameobjectActive(component.gameObject, bValue: true);
		if (idx < 0 || idx >= m_lstData.Count)
		{
			component.SetData(new NKCUICutScenPlayer.CutsceneLog(""), dOnVoice);
		}
		else
		{
			component.SetData(m_lstData[idx], dOnVoice);
		}
	}

	private void OnScrollEvent(PointerEventData data, Vector2 normalizedPositionBefore, Vector2 normalizedPositionAfter)
	{
		if (data.scrollDelta.y < 0f && normalizedPositionBefore.y >= 0.999f && normalizedPositionAfter.y >= 1f)
		{
			OnClickClose();
		}
	}
}
