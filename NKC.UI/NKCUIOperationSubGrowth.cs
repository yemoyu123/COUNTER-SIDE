using System.Collections.Generic;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubGrowth : MonoBehaviour
{
	public NKCUIComToggle m_tglSupply;

	public NKCUIComToggle m_tglChallenge;

	[Header("슬롯")]
	public NKCUIOperationSubGrowthEPSlot m_pfbSlot;

	public LoopHorizontalScrollRect m_loop;

	private Dictionary<EPISODE_CATEGORY, List<NKMEpisodeTempletV2>> m_dicData = new Dictionary<EPISODE_CATEGORY, List<NKMEpisodeTempletV2>>();

	private Stack<NKCUIOperationSubGrowthEPSlot> m_stkSlot = new Stack<NKCUIOperationSubGrowthEPSlot>();

	private EPISODE_CATEGORY m_CurCategory = EPISODE_CATEGORY.EC_SUPPLY;

	public void InitUI()
	{
		if (m_tglSupply != null)
		{
			m_tglSupply.OnValueChanged.RemoveAllListeners();
			m_tglSupply.OnValueChanged.AddListener(OnSupply);
			m_tglSupply.m_bGetCallbackWhileLocked = true;
		}
		if (m_tglChallenge != null)
		{
			m_tglChallenge.OnValueChanged.RemoveAllListeners();
			m_tglChallenge.OnValueChanged.AddListener(OnChallenge);
			m_tglChallenge.m_bGetCallbackWhileLocked = true;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		Canvas.ForceUpdateCanvases();
		m_loop.PrepareCells();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private RectTransform GetObject(int idx)
	{
		NKCUIOperationSubGrowthEPSlot nKCUIOperationSubGrowthEPSlot = null;
		nKCUIOperationSubGrowthEPSlot = ((m_stkSlot.Count <= 0) ? Object.Instantiate(m_pfbSlot, m_loop.content) : m_stkSlot.Pop());
		nKCUIOperationSubGrowthEPSlot.InitUI(OnClickSlot);
		return nKCUIOperationSubGrowthEPSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIOperationSubGrowthEPSlot component = tr.GetComponent<NKCUIOperationSubGrowthEPSlot>();
		if (!(component == null))
		{
			m_stkSlot.Push(component);
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		if (idx >= 0 && idx < m_dicData[m_CurCategory].Count)
		{
			NKCUIOperationSubGrowthEPSlot component = tr.GetComponent<NKCUIOperationSubGrowthEPSlot>();
			if (!(component == null))
			{
				NKCUtil.SetGameobjectActive(component, bValue: true);
				component.SetData(m_dicData[m_CurCategory][idx]);
			}
		}
	}

	private int CompByKey(NKMEpisodeGroupTemplet lhs, NKMEpisodeGroupTemplet rhs)
	{
		return lhs.Key.CompareTo(rhs.Key);
	}

	public void Open()
	{
		BuildTemplets();
		NKMEpisodeTempletV2 reservedEpisodeTemplet = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet();
		if (reservedEpisodeTemplet != null)
		{
			m_CurCategory = reservedEpisodeTemplet.m_EPCategory;
		}
		else if (NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeCategory() == EPISODE_CATEGORY.EC_SUPPLY || NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeCategory() == EPISODE_CATEGORY.EC_CHALLENGE)
		{
			m_CurCategory = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeCategory();
		}
		else
		{
			m_CurCategory = EPISODE_CATEGORY.EC_SUPPLY;
		}
		switch (m_CurCategory)
		{
		case EPISODE_CATEGORY.EC_SUPPLY:
			OnSupply(bValue: true);
			break;
		case EPISODE_CATEGORY.EC_CHALLENGE:
			OnChallenge(bValue: true);
			break;
		}
		m_tglSupply.Select(m_CurCategory == EPISODE_CATEGORY.EC_SUPPLY, bForce: true, bImmediate: true);
		m_tglChallenge.Select(m_CurCategory == EPISODE_CATEGORY.EC_CHALLENGE, bForce: true, bImmediate: true);
		if (reservedEpisodeTemplet != null)
		{
			NKCUIOperationNodeViewer.Instance.Open(reservedEpisodeTemplet);
		}
	}

	private void SetData()
	{
		m_loop.content.GetComponent<RectTransform>().pivot = Vector2.zero;
		m_loop.TotalCount = m_dicData[m_CurCategory].Count;
		m_loop.SetIndexPosition(0);
		m_loop.RefreshCellsForDynamicTotalCount();
		m_loop.horizontalScrollbarVisibility = LoopScrollRect.ScrollbarVisibility.AutoHide;
		if (NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedStageTemplet() == null)
		{
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetReservedEpisodeCategory(m_CurCategory);
		}
		NKCUIManager.UpdateUpsideMenu();
		TutorialCheck_Tab();
	}

	public string GetGuideTempletID()
	{
		return m_CurCategory switch
		{
			EPISODE_CATEGORY.EC_SUPPLY => "ARTICLE_OPERATION_SUPPLY", 
			EPISODE_CATEGORY.EC_CHALLENGE => "ARTICLE_OPERATION_CHALLENGE", 
			_ => "", 
		};
	}

	private void BuildTemplets()
	{
		List<NKMEpisodeGroupTemplet> list = new List<NKMEpisodeGroupTemplet>();
		foreach (NKMEpisodeGroupTemplet value in NKMTempletContainer<NKMEpisodeGroupTemplet>.Values)
		{
			if (value.GroupCategory == EPISODE_GROUP.EG_GROWTH)
			{
				list.Add(value);
			}
		}
		list.Sort(CompByKey);
		m_dicData.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = 0; j < list[i].lstEpisodeTemplet.Count; j++)
			{
				NKMEpisodeTempletV2 nKMEpisodeTempletV = list[i].lstEpisodeTemplet[j];
				if (nKMEpisodeTempletV != null && nKMEpisodeTempletV.IsOpen && nKMEpisodeTempletV.IsOpenedDayOfWeek() && nKMEpisodeTempletV.m_Difficulty == EPISODE_DIFFICULTY.NORMAL)
				{
					if (!m_dicData.ContainsKey(nKMEpisodeTempletV.m_EPCategory))
					{
						m_dicData.Add(nKMEpisodeTempletV.m_EPCategory, new List<NKMEpisodeTempletV2>());
					}
					m_dicData[nKMEpisodeTempletV.m_EPCategory].Add(list[i].lstEpisodeTemplet[j]);
				}
			}
		}
	}

	public void OnClickSlot(int episodeID)
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(episodeID, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV != null)
		{
			NKCUIOperationNodeViewer.Instance.Open(nKMEpisodeTempletV);
		}
	}

	private void OnSupply(bool bValue)
	{
		if (bValue && !m_tglSupply.m_bLock)
		{
			m_CurCategory = EPISODE_CATEGORY.EC_SUPPLY;
			SetData();
		}
	}

	private void OnChallenge(bool bValue)
	{
		if (bValue && !m_tglChallenge.m_bLock)
		{
			m_CurCategory = EPISODE_CATEGORY.EC_CHALLENGE;
			SetData();
		}
	}

	private void TutorialCheck_Tab()
	{
		if (m_CurCategory == EPISODE_CATEGORY.EC_SUPPLY)
		{
			NKCTutorialManager.TutorialRequired(TutorialPoint.Operation_Growth_Supply);
		}
		else if (m_CurCategory == EPISODE_CATEGORY.EC_CHALLENGE)
		{
			NKCTutorialManager.TutorialRequired(TutorialPoint.Operation_Growth_Challenge);
		}
		NKCTutorialManager.TutorialRequired(TutorialPoint.Operation_Growth);
	}
}
