using System.Collections.Generic;
using ClientPacket.LeaderBoard;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIPopUpFierceBattleRankSide : MonoBehaviour
{
	public Text m_BossName;

	public NKCUIComStateButton m_BUTTON_X;

	public LoopScrollRect m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect;

	public GameObject m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_NODATA;

	private Stack<NKCUIFierceBattleBossPersonalRankSlot> m_stk = new Stack<NKCUIFierceBattleBossPersonalRankSlot>();

	private List<NKCUIFierceBattleBossPersonalRankSlot> m_lstVisible = new List<NKCUIFierceBattleBossPersonalRankSlot>();

	public void Init()
	{
		if (m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect != null)
		{
			m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect.dOnGetObject += GetObject;
			m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect.dOnProvideData += ProvideData;
			m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect.dOnReturnObject += ReturnObject;
			m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect.PrepareCells();
		}
		NKCUtil.SetBindFunction(m_BUTTON_X, delegate
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		});
	}

	public void Open()
	{
		bool flag = false;
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr != null)
		{
			string targetBossName = nKCFierceBattleSupportDataMgr.GetTargetBossName(nKCFierceBattleSupportDataMgr.CurBossGroupID, nKCFierceBattleSupportDataMgr.GetCurSelectedBossLv());
			NKCUtil.SetLabelText(m_BossName, targetBossName);
			if (nKCFierceBattleSupportDataMgr.IsHasFierceRankingData(All: true))
			{
				m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect.TotalCount = Mathf.Min(nKCFierceBattleSupportDataMgr.GetBossGroupRankingDataCnt(), 50);
				m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect.RefreshCells();
				flag = true;
			}
		}
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect.gameObject, flag);
		NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_NODATA, !flag);
	}

	public void Clear()
	{
		for (int i = 0; i < m_lstVisible.Count; i++)
		{
			m_stk.Push(m_lstVisible[i]);
		}
		while (m_stk.Count > 0)
		{
			m_stk.Pop()?.DestoryInstance();
		}
	}

	private RectTransform GetObject(int index)
	{
		NKCUIFierceBattleBossPersonalRankSlot nKCUIFierceBattleBossPersonalRankSlot = null;
		nKCUIFierceBattleBossPersonalRankSlot = ((m_stk.Count <= 0) ? NKCUIFierceBattleBossPersonalRankSlot.GetNewInstance(m_FIERCE_BATTLE_BOSS_PERSONAL_RANK_ScrollRect.content.transform) : m_stk.Pop());
		m_lstVisible.Add(nKCUIFierceBattleBossPersonalRankSlot);
		return nKCUIFierceBattleBossPersonalRankSlot?.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUIFierceBattleBossPersonalRankSlot component = tr.GetComponent<NKCUIFierceBattleBossPersonalRankSlot>();
		m_lstVisible.Remove(component);
		m_stk.Push(component);
		tr.SetParent(base.transform);
		NKCUtil.SetGameobjectActive(component, bValue: false);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCUIFierceBattleBossPersonalRankSlot component = tr.GetComponent<NKCUIFierceBattleBossPersonalRankSlot>();
		int rank = idx + 1;
		NKMFierceData fierceRankingData = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().GetFierceRankingData(idx);
		if (fierceRankingData == null)
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
		else
		{
			component.SetData(fierceRankingData, rank);
		}
	}
}
