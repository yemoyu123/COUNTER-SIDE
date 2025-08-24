using System.Collections.Generic;
using NKC.UI;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCDiveGameHUDArtifact : MonoBehaviour
{
	public delegate void dOnFinishScrollToArtifactDummySlot();

	public GameObject m_objNormalBG;

	public GameObject m_objHurdleBG;

	public Animator m_Animator;

	public NKCUIComStateButton m_csbtnOpen;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComToggle m_csbtnTabChange;

	public NKCUIComStateButton m_csbtnHelp;

	public LoopScrollRect m_LoopScrollRect;

	public NKCUIDiveGameArtifactSlot m_pfbNKCUIDiveGameArtifactSlot;

	private Stack<NKCUIDiveGameArtifactSlot> m_stkNKCUIDiveGameArtifactSlot = new Stack<NKCUIDiveGameArtifactSlot>();

	public Text m_lbTotalViewDesc;

	private bool m_bClosed = true;

	private bool m_bEachView = true;

	private bool m_bFirstOpen = true;

	private bool m_bDummySlot;

	private dOnFinishScrollToArtifactDummySlot m_dOnFinishScrollToArtifactDummySlot;

	private void OnFinishScrollToArtifactDummySlot()
	{
		if (m_dOnFinishScrollToArtifactDummySlot != null)
		{
			m_dOnFinishScrollToArtifactDummySlot();
		}
	}

	public void InitUI(dOnFinishScrollToArtifactDummySlot _dOnFinishScrollToArtifactDummySlot)
	{
		m_dOnFinishScrollToArtifactDummySlot = _dOnFinishScrollToArtifactDummySlot;
		m_csbtnOpen.PointerClick.RemoveAllListeners();
		m_csbtnOpen.PointerClick.AddListener(Open);
		m_csbtnClose.PointerClick.RemoveAllListeners();
		m_csbtnClose.PointerClick.AddListener(CloseWithAnimate);
		m_csbtnTabChange.OnValueChanged.RemoveAllListeners();
		m_csbtnTabChange.OnValueChanged.AddListener(OnChangedViewTab);
		m_csbtnHelp.PointerClick.RemoveAllListeners();
		m_csbtnHelp.PointerClick.AddListener(OnClickHelp);
		m_LoopScrollRect.dOnGetObject += GetArtifactSlot;
		m_LoopScrollRect.dOnReturnObject += ReturnArtifactSlot;
		m_LoopScrollRect.dOnProvideData += ProvideArtifactSlotData;
	}

	public void RefreshInvenry()
	{
		NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
		if (diveGameData != null)
		{
			m_LoopScrollRect.TotalCount = diveGameData.Player.PlayerBase.Artifacts.Count;
			m_LoopScrollRect.RefreshCells();
			UpdateTotalViewTextUI();
		}
	}

	public RectTransform GetArtifactSlot(int index)
	{
		NKCUIDiveGameArtifactSlot nKCUIDiveGameArtifactSlot = null;
		if (m_stkNKCUIDiveGameArtifactSlot.Count > 0)
		{
			nKCUIDiveGameArtifactSlot = m_stkNKCUIDiveGameArtifactSlot.Pop();
		}
		else
		{
			nKCUIDiveGameArtifactSlot = Object.Instantiate(m_pfbNKCUIDiveGameArtifactSlot);
			nKCUIDiveGameArtifactSlot.InitUI();
		}
		return nKCUIDiveGameArtifactSlot.GetComponent<RectTransform>();
	}

	public void ReturnArtifactSlot(Transform tr)
	{
		NKCUIDiveGameArtifactSlot component = tr.GetComponent<NKCUIDiveGameArtifactSlot>();
		m_stkNKCUIDiveGameArtifactSlot.Push(component);
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
	}

	public void ProvideArtifactSlotData(Transform tr, int index)
	{
		NKCUIDiveGameArtifactSlot component = tr.GetComponent<NKCUIDiveGameArtifactSlot>();
		NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
		if (diveGameData != null)
		{
			if (index >= diveGameData.Player.PlayerBase.Artifacts.Count)
			{
				component.SetData(null);
				return;
			}
			if (m_bDummySlot && index == diveGameData.Player.PlayerBase.Artifacts.Count - 1)
			{
				component.SetData(null);
				return;
			}
			NKMDiveArtifactTemplet data = NKMDiveArtifactTemplet.Find(diveGameData.Player.PlayerBase.Artifacts[index]);
			component.SetData(data);
		}
	}

	public void SetDummySlot()
	{
		NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
		if (diveGameData != null)
		{
			m_bDummySlot = true;
			m_LoopScrollRect.TotalCount = diveGameData.Player.PlayerBase.Artifacts.Count;
			m_LoopScrollRect.RefreshCells();
			m_LoopScrollRect.ScrollToCell(m_LoopScrollRect.TotalCount - 1, 0.2f, LoopScrollRect.ScrollTarget.Top, OnFinishScrollToArtifactDummySlot);
		}
	}

	public void InvalidDummySlot()
	{
		m_bDummySlot = false;
	}

	public Vector3 GetLastItemSlotImgPos()
	{
		Transform lastActivatedItem = m_LoopScrollRect.GetLastActivatedItem();
		if (lastActivatedItem != null)
		{
			NKCUIDiveGameArtifactSlot component = lastActivatedItem.GetComponent<NKCUIDiveGameArtifactSlot>();
			if (component != null)
			{
				return component.m_NKCUISlot.transform.position;
			}
		}
		return new Vector3(0f, 0f, 0f);
	}

	private void OnClickHelp()
	{
		NKCUIPopUpGuide.Instance.Open("ARTICLE_DIVE_ARTIFACT");
	}

	public void ResetUI(bool bHurdle = false)
	{
		m_bClosed = true;
		m_bEachView = false;
		m_csbtnTabChange.Select(bSelect: true, bForce: true);
		if (m_bFirstOpen)
		{
			m_LoopScrollRect.PrepareCells();
		}
		m_bFirstOpen = false;
		NKCUtil.SetGameobjectActive(m_objNormalBG, !bHurdle);
		NKCUtil.SetGameobjectActive(m_objHurdleBG, bHurdle);
		int totalCount = 0;
		NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
		if (diveGameData != null)
		{
			totalCount = diveGameData.Player.PlayerBase.Artifacts.Count;
		}
		m_LoopScrollRect.velocity = new Vector2(0f, 0f);
		m_LoopScrollRect.TotalCount = totalCount;
		m_LoopScrollRect.SetIndexPosition(0);
		UpdateTotalViewTextUI();
	}

	public void Open()
	{
		if (base.gameObject.activeSelf && m_bClosed)
		{
			NKCUtil.SetGameobjectActive(m_csbtnOpen, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnClose, bValue: true);
			if (!m_bEachView)
			{
				m_Animator.Play("NKM_UI_DIVE_PROCESS_SQUAD_LEFT_ARTIFACT_CONTENT_OPEN");
			}
			else
			{
				m_Animator.Play("NKM_UI_DIVE_PROCESS_SQUAD_LEFT_ARTIFACT_CONTENT_OPEN_EACH");
			}
		}
	}

	public void UpdateTotalViewTextUI()
	{
		NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
		if (diveGameData != null)
		{
			NKCUtil.SetLabelText(m_lbTotalViewDesc, NKCUtilString.GetDiveArtifactTotalViewDesc(diveGameData.Player.PlayerBase.Artifacts));
		}
	}

	private void OnChangedViewTab(bool bChecked)
	{
		m_bEachView = !bChecked;
		if (m_bEachView)
		{
			m_Animator.Play("NKM_UI_DIVE_PROCESS_SQUAD_LEFT_ARTIFACT_CONTENT_OPENTOEACH");
		}
		else
		{
			m_Animator.Play("NKM_UI_DIVE_PROCESS_SQUAD_LEFT_ARTIFACT_CONTENT_EACHTOOPEN");
		}
	}

	private void CloseWithAnimate()
	{
		Close(bAnimate: true);
	}

	public void Close(bool bAnimate)
	{
		m_bClosed = true;
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_csbtnOpen, bValue: true);
		NKCUtil.SetGameobjectActive(m_csbtnClose, bValue: false);
		if (bAnimate)
		{
			if (!m_bEachView)
			{
				m_Animator.Play("NKM_UI_DIVE_PROCESS_SQUAD_LEFT_ARTIFACT_CONTENT_CLOSE");
			}
			else
			{
				m_Animator.Play("NKM_UI_DIVE_PROCESS_SQUAD_LEFT_ARTIFACT_CONTENT_CLOSE_EACH");
			}
		}
		else
		{
			m_Animator.Play("NKM_UI_DIVE_PROCESS_SQUAD_LEFT_ARTIFACT_CONTENT_CLOSE_IDLE");
		}
	}
}
