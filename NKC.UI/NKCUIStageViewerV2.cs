using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIStageViewerV2 : MonoBehaviour, INKCUIStageViewer
{
	[Header("!! 첫 액트부터 차례로 넣어야함 !!")]
	public List<NKCUIStagePrefabAct> m_lstNormalAct = new List<NKCUIStagePrefabAct>();

	public List<NKCUIStagePrefabAct> m_lstHardAct = new List<NKCUIStagePrefabAct>();

	public bool m_bUseNormalizedPos = true;

	private bool m_bUseEpSlot;

	private int m_EpisodeID;

	private int m_ActID;

	private EPISODE_DIFFICULTY m_Difficulty;

	private IDungeonSlot.OnSelectedItemSlot m_dOnSelectedSlot;

	private EPISODE_SCROLL_TYPE m_ScrollType;

	public bool UseNormalizedPos()
	{
		return m_bUseNormalizedPos;
	}

	public int GetCurActID()
	{
		return m_ActID;
	}

	public void ResetPosition(Transform parent)
	{
		base.transform.SetParent(parent, worldPositionStays: false);
		GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		GetComponent<RectTransform>().localScale = Vector3.one;
	}

	public int GetActCount(EPISODE_DIFFICULTY Difficulty)
	{
		if (Difficulty == EPISODE_DIFFICULTY.NORMAL)
		{
			return m_lstNormalAct.Count;
		}
		return m_lstHardAct.Count;
	}

	public Vector2 GetTargetPos(int slotIndex, EPISODE_SCROLL_TYPE scrollType, bool bUseNormalized)
	{
		for (int i = 0; i < m_lstNormalAct.Count; i++)
		{
			if (m_lstNormalAct[i].gameObject.activeSelf)
			{
				return m_lstNormalAct[i].GetTargetPos(slotIndex, scrollType, bUseNormalized);
			}
		}
		for (int j = 0; j < m_lstHardAct.Count; j++)
		{
			if (m_lstHardAct[j].gameObject.activeSelf)
			{
				return m_lstHardAct[j].GetTargetPos(slotIndex, scrollType, bUseNormalized);
			}
		}
		return Vector2.one;
	}

	public void SetActive(bool bValue)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue);
	}

	public void SetSelectNode(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet != null)
		{
			List<NKCUIStagePrefabAct> list = new List<NKCUIStagePrefabAct>();
			if (stageTemplet.m_Difficulty == EPISODE_DIFFICULTY.NORMAL)
			{
				list = m_lstNormalAct;
			}
			else if (stageTemplet.m_Difficulty == EPISODE_DIFFICULTY.HARD)
			{
				list = m_lstHardAct;
			}
			if (!stageTemplet.EpisodeTemplet.UseEpSlot())
			{
				list[stageTemplet.ActId - 1].SelectNode(stageTemplet);
				return;
			}
			int index = list.FindIndex((NKCUIStagePrefabAct x) => x.GetEpisodeID() == stageTemplet.EpisodeId);
			list[index].SelectNode(stageTemplet);
			return;
		}
		for (int num = 0; num < m_lstNormalAct.Count; num++)
		{
			if (m_lstNormalAct[num].gameObject.activeSelf)
			{
				m_lstNormalAct[num].SelectNode(null);
			}
		}
		for (int num2 = 0; num2 < m_lstHardAct.Count; num2++)
		{
			if (m_lstHardAct[num2].gameObject.activeSelf)
			{
				m_lstHardAct[num2].SelectNode(null);
			}
		}
	}

	public Vector2 SetData(bool bUseEpSlot, int EpisodeID, int ActID, EPISODE_DIFFICULTY Difficulty, IDungeonSlot.OnSelectedItemSlot onSelectedSlot, EPISODE_SCROLL_TYPE scrollType)
	{
		m_bUseEpSlot = bUseEpSlot;
		m_EpisodeID = EpisodeID;
		m_ActID = ActID;
		m_Difficulty = Difficulty;
		m_dOnSelectedSlot = onSelectedSlot;
		m_ScrollType = scrollType;
		for (int i = 0; i < m_lstNormalAct.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstNormalAct[i], bValue: false);
		}
		for (int j = 0; j < m_lstHardAct.Count; j++)
		{
			NKCUtil.SetGameobjectActive(m_lstHardAct[j], bValue: false);
		}
		List<NKCUIStagePrefabAct> list = new List<NKCUIStagePrefabAct>();
		switch (Difficulty)
		{
		case EPISODE_DIFFICULTY.NORMAL:
			list = m_lstNormalAct;
			break;
		case EPISODE_DIFFICULTY.HARD:
			list = m_lstHardAct;
			break;
		}
		if (!bUseEpSlot)
		{
			if (NKMEpisodeTempletV2.Find(EpisodeID, Difficulty) != null)
			{
				if (ActID > list.Count || ActID < 0)
				{
					NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
					return Vector2.zero;
				}
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
				NKCUtil.SetGameobjectActive(list[ActID - 1], bValue: true);
				return list[ActID - 1].SetData(EpisodeID, ActID, Difficulty, onSelectedSlot, scrollType, m_bUseNormalizedPos);
			}
		}
		else
		{
			NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(EpisodeID);
			if (nKMEpisodeGroupTemplet != null)
			{
				int index = nKMEpisodeGroupTemplet.lstEpisodeTemplet.FindIndex((NKMEpisodeTempletV2 x) => x.m_EpisodeID == ActID);
				NKMEpisodeTempletV2 nKMEpisodeTempletV = nKMEpisodeGroupTemplet.lstEpisodeTemplet.Find((NKMEpisodeTempletV2 x) => x.m_EpisodeID == ActID);
				if (nKMEpisodeTempletV != null)
				{
					NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
					NKCUtil.SetGameobjectActive(list[index], bValue: true);
					return list[index].SetData(nKMEpisodeTempletV.m_EpisodeID, 1, Difficulty, onSelectedSlot, scrollType, m_bUseNormalizedPos);
				}
			}
		}
		return Vector2.zero;
	}

	public void RefreshData()
	{
		SetData(m_bUseEpSlot, m_EpisodeID, m_ActID, m_Difficulty, m_dOnSelectedSlot, m_ScrollType);
	}
}
