using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShadowPalaceRank : MonoBehaviour
{
	public Animator m_ani;

	public Image m_imgBG;

	[Header("Info")]
	public Text m_txtPalaceNum;

	public Text m_txtPalaceName;

	[Header("Rank")]
	public LoopScrollRect m_scrollRect;

	public NKCUIShadowPalaceRankSlot m_prefabSlot;

	public NKCUIShadowPalaceRankSlot m_myRank;

	[Header("Button")]
	public NKCUIComStateButton m_btnClose;

	private Stack<NKCUIShadowPalaceRankSlot> m_stkSlotPool = new Stack<NKCUIShadowPalaceRankSlot>();

	private List<LeaderBoardSlotData> m_lstRankData = new List<LeaderBoardSlotData>();

	public void Init()
	{
		if (m_scrollRect != null)
		{
			m_scrollRect.dOnGetObject += OnGetObject;
			m_scrollRect.dOnProvideData += OnProvideData;
			m_scrollRect.dOnReturnObject += OnReturnObject;
			m_scrollRect.PrepareCells();
		}
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(OnClose);
		m_ani.Play("NKM_UI_SHADOW_INFO_OUTRO_IDLE");
		m_imgBG.raycastTarget = false;
	}

	public void SetData(string title, string desc, List<LeaderBoardSlotData> lstRankData, LeaderBoardSlotData myRankData)
	{
		NKCUtil.SetLabelText(m_txtPalaceNum, title);
		NKCUtil.SetLabelText(m_txtPalaceName, desc);
		m_lstRankData = lstRankData;
		m_scrollRect.TotalCount = lstRankData.Count;
		m_scrollRect.RefreshCells(bForce: true);
		m_scrollRect.SetIndexPosition(0);
		m_myRank.Init();
		m_myRank.SetData(myRankData, myRankData.rank, bMyRank: true);
	}

	public void SetData(string title, string desc, int boardID)
	{
		SetData(title, desc, NKCLeaderBoardManager.GetLeaderBoardData(boardID), NKCLeaderBoardManager.GetMyRankSlotData(boardID));
	}

	public void SetData(NKMShadowPalaceTemplet palaceTemplet, List<LeaderBoardSlotData> lstRankData, LeaderBoardSlotData myRankData, int myRank)
	{
		NKCUtil.SetLabelText(m_txtPalaceNum, NKCUtilString.GET_SHADOW_PALACE_NUMBER, palaceTemplet.PALACE_NUM_UI);
		NKCUtil.SetLabelText(m_txtPalaceName, palaceTemplet.PalaceName);
		m_lstRankData = lstRankData;
		m_scrollRect.TotalCount = lstRankData.Count;
		m_scrollRect.RefreshCells(bForce: true);
		m_scrollRect.SetIndexPosition(0);
		m_myRank.Init();
		m_myRank.SetData(myRankData, myRank, bMyRank: true);
	}

	public void PlayAni(bool bOpen)
	{
		if (bOpen)
		{
			m_ani.Play("NKM_UI_SHADOW_INFO_INTRO");
			m_imgBG.raycastTarget = true;
		}
		else
		{
			m_ani.Play("NKM_UI_SHADOW_INFO_OUTRO");
			m_imgBG.raycastTarget = false;
		}
	}

	private RectTransform OnGetObject(int index)
	{
		if (m_stkSlotPool.Count > 0)
		{
			return m_stkSlotPool.Pop().GetComponent<RectTransform>();
		}
		NKCUIShadowPalaceRankSlot nKCUIShadowPalaceRankSlot = Object.Instantiate(m_prefabSlot);
		nKCUIShadowPalaceRankSlot.transform.SetParent(m_scrollRect.content);
		nKCUIShadowPalaceRankSlot.Init();
		return nKCUIShadowPalaceRankSlot.GetComponent<RectTransform>();
	}

	private void OnProvideData(Transform tr, int idx)
	{
		NKCUIShadowPalaceRankSlot component = tr.GetComponent<NKCUIShadowPalaceRankSlot>();
		if (!(component == null))
		{
			component.SetData(m_lstRankData[idx], idx + 1, bMyRank: false);
		}
	}

	private void OnReturnObject(Transform go)
	{
		if (!(GetComponent<NKCUIShadowPalaceRankSlot>() != null))
		{
			NKCUtil.SetGameobjectActive(go, bValue: false);
			go.SetParent(base.transform);
			m_stkSlotPool.Push(go.GetComponent<NKCUIShadowPalaceRankSlot>());
		}
	}

	private void OnClose()
	{
		PlayAni(bOpen: false);
	}
}
