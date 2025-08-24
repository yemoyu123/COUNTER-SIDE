using System.Collections.Generic;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIStagePrefabAct : MonoBehaviour
{
	[Header("!! 첫 스테이지부터 차례로 넣어야함 !!")]
	public List<NKCUIStagePrefabNode> m_lstItemSlot = new List<NKCUIStagePrefabNode>();

	public float m_fPadding = -300f;

	private int m_EpisodeID;

	public int GetEpisodeID()
	{
		return m_EpisodeID;
	}

	public Vector2 GetTargetPos(int targetIndex, EPISODE_SCROLL_TYPE scrollType, bool bUseNormalized)
	{
		if (m_lstItemSlot.Count <= 1)
		{
			return Vector2.zero;
		}
		if (targetIndex < 0)
		{
			targetIndex = 0;
		}
		else if (targetIndex >= m_lstItemSlot.Count)
		{
			targetIndex = m_lstItemSlot.Count - 1;
		}
		LayoutElement component = m_lstItemSlot[0].GetComponent<LayoutElement>();
		if (component == null || !component.enabled)
		{
			float num = ((scrollType == EPISODE_SCROLL_TYPE.HORIZONTAL) ? m_lstItemSlot[0].transform.position.x : m_lstItemSlot[0].transform.position.y);
			float num2 = ((scrollType == EPISODE_SCROLL_TYPE.HORIZONTAL) ? m_lstItemSlot[m_lstItemSlot.Count - 1].transform.position.x : m_lstItemSlot[m_lstItemSlot.Count - 1].transform.position.y);
			for (int i = 0; i < m_lstItemSlot.Count; i++)
			{
				float num3 = ((scrollType == EPISODE_SCROLL_TYPE.HORIZONTAL) ? m_lstItemSlot[i].transform.position.x : m_lstItemSlot[i].transform.position.y);
				if (num3 < num)
				{
					num = num3;
				}
				if (num3 > num2)
				{
					num2 = num3;
				}
			}
			Vector2 result = m_lstItemSlot[targetIndex].transform.position;
			if (scrollType != EPISODE_SCROLL_TYPE.HORIZONTAL)
			{
				_ = (result.y - num) / (num2 - num);
			}
			else
			{
				_ = (result.x - num) / (num2 - num);
			}
			if (bUseNormalized)
			{
				if (scrollType != EPISODE_SCROLL_TYPE.HORIZONTAL)
				{
					return new Vector2(0f, (result.y - num) / (num2 - num));
				}
				return new Vector2((result.x - num) / (num2 - num), 0f);
			}
			return result;
		}
		float num4 = ((scrollType == EPISODE_SCROLL_TYPE.HORIZONTAL) ? m_lstItemSlot[0].transform.position.x : m_lstItemSlot[0].transform.position.y);
		float num5 = ((scrollType == EPISODE_SCROLL_TYPE.HORIZONTAL) ? (m_lstItemSlot[0].transform.position.x + component.preferredWidth * (float)(m_lstItemSlot.Count - 1)) : (m_lstItemSlot[0].transform.position.y + component.preferredHeight * (float)(m_lstItemSlot.Count - 1)));
		for (int j = 0; j < m_lstItemSlot.Count; j++)
		{
			float num6 = ((scrollType == EPISODE_SCROLL_TYPE.HORIZONTAL) ? (m_lstItemSlot[0].transform.position.x + component.preferredWidth * (float)j) : (m_lstItemSlot[0].transform.position.y + component.preferredHeight * (float)j));
			if (num6 < num4)
			{
				num4 = num6;
			}
			if (num6 > num5)
			{
				num5 = num6;
			}
		}
		Vector2 vector = ((scrollType == EPISODE_SCROLL_TYPE.HORIZONTAL) ? new Vector2(m_lstItemSlot[0].transform.position.x + component.preferredWidth * (float)targetIndex, 0f) : new Vector2(0f, m_lstItemSlot[0].transform.position.y + component.preferredHeight * (float)targetIndex));
		float num7 = ((scrollType == EPISODE_SCROLL_TYPE.HORIZONTAL) ? ((vector.x - num4) / (num5 - num4)) : ((vector.y - num4) / (num5 - num4)));
		if (bUseNormalized)
		{
			if (scrollType != EPISODE_SCROLL_TYPE.HORIZONTAL)
			{
				return new Vector2(0f, (vector.y - num4) / (num5 - num4));
			}
			return new Vector2((vector.x - num4) / (num5 - num4), 0f);
		}
		float num8 = num5 - num4;
		float num9 = m_fPadding + num8 / 2f - num8 * num7;
		if (scrollType != EPISODE_SCROLL_TYPE.HORIZONTAL)
		{
			return new Vector2(0f, num9);
		}
		return new Vector2(num9, 0f);
	}

	public Vector2 SetData(int EpisodeID, int ActID, EPISODE_DIFFICULTY Difficulty, IDungeonSlot.OnSelectedItemSlot onSelectedSlot, EPISODE_SCROLL_TYPE scrollType, bool bNormalizedPos = true)
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(EpisodeID, Difficulty);
		if (nKMEpisodeTempletV == null)
		{
			return Vector2.zero;
		}
		if (!nKMEpisodeTempletV.m_DicStage.ContainsKey(ActID))
		{
			Log.Error($"액트를 찾을 수 없음 - EpisodeID {EpisodeID}, ActID {ActID}, Difficulty {Difficulty}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIStagePrefabAct.cs", 103);
			return Vector2.zero;
		}
		int count = nKMEpisodeTempletV.m_DicStage[ActID].Count;
		if (count > m_lstItemSlot.Count)
		{
			Log.Error($"Stage 숫자가 프리팹 숫자보다 많음 - EpisodeID {EpisodeID}, ActID {ActID}, Difficulty {Difficulty}, DungeonCount : {count}, SlotCount : {m_lstItemSlot.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIStagePrefabAct.cs", 111);
			return Vector2.zero;
		}
		if (m_lstItemSlot.Contains(null))
		{
			Log.Error($"m_lstItemSlot 에 노드 링크가 잘못됨 - EpisodeID {EpisodeID}, ActID {ActID}, Difficulty {Difficulty}, index : {m_lstItemSlot.FindIndex(null)}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIStagePrefabAct.cs", 117);
			return Vector2.zero;
		}
		if (m_lstItemSlot.Count == 0)
		{
			Log.Error($"m_lstItemSlot 에 노드가 없음 - EpisodeID {EpisodeID}, ActID {ActID}, Difficulty {Difficulty}, SlotCount : {m_lstItemSlot.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIStagePrefabAct.cs", 123);
			return Vector2.zero;
		}
		m_EpisodeID = EpisodeID;
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		Vector3 localPosition = m_lstItemSlot[m_lstItemSlot.Count - 1].transform.localPosition;
		Vector3 localPosition2 = m_lstItemSlot[0].transform.localPosition;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < m_lstItemSlot.Count; i++)
		{
			if (m_lstItemSlot[i].transform.localPosition.x < localPosition2.x)
			{
				localPosition2.x = m_lstItemSlot[i].transform.localPosition.x;
			}
			if (m_lstItemSlot[i].transform.localPosition.x > localPosition.x)
			{
				localPosition.x = m_lstItemSlot[i].transform.localPosition.x;
			}
			if (m_lstItemSlot[i].transform.localPosition.y < localPosition2.y)
			{
				localPosition2.y = m_lstItemSlot[i].transform.localPosition.y;
			}
			if (m_lstItemSlot[i].transform.localPosition.y > localPosition.y)
			{
				localPosition.y = m_lstItemSlot[i].transform.localPosition.y;
			}
		}
		zero = localPosition - localPosition2;
		for (int j = 0; j < m_lstItemSlot.Count; j++)
		{
			NKCUIStagePrefabNode nKCUIStagePrefabNode = m_lstItemSlot[j];
			InitSlot(nKCUIStagePrefabNode);
			nKCUIStagePrefabNode.SetEnableNewMark(bValue: false);
			nKCUIStagePrefabNode.SetOnSelectedItemSlot(onSelectedSlot);
			if (j < count)
			{
				NKMStageTempletV2 nKMStageTempletV = nKMEpisodeTempletV.m_DicStage[ActID][j];
				bool flag = false;
				flag = NKMEpisodeMgr.CheckEpisodeMission(myUserData, nKMStageTempletV);
				if (!nKMStageTempletV.EnableByTag)
				{
					NKCUtil.SetGameobjectActive(nKCUIStagePrefabNode, bValue: false);
				}
				else if (nKMStageTempletV.m_StageBasicUnlockType == STAGE_BASIC_UNLOCK_TYPE.SBUT_OPEN)
				{
					if (flag)
					{
						localPosition = nKCUIStagePrefabNode.transform.localPosition;
						nKCUIStagePrefabNode.SetData(nKMStageTempletV, onSelectedSlot);
						if (!PlayerPrefs.HasKey($"{NKCScenManager.CurrentUserData().m_UserUID}_{nKMStageTempletV.m_StageBattleStrID}") && !myUserData.CheckStageCleared(nKMStageTempletV))
						{
							nKCUIStagePrefabNode.SetEnableNewMark(bValue: true);
						}
						else
						{
							nKCUIStagePrefabNode.SetEnableNewMark(bValue: false);
						}
						if (!nKCUIStagePrefabNode.IsActive())
						{
							NKCUtil.SetGameobjectActive(nKCUIStagePrefabNode, bValue: true);
						}
					}
					else if (nKCUIStagePrefabNode.IsActive())
					{
						NKCUtil.SetGameobjectActive(nKCUIStagePrefabNode, bValue: false);
					}
				}
				else if (nKMStageTempletV.m_StageBasicUnlockType == STAGE_BASIC_UNLOCK_TYPE.SBUT_LOCK)
				{
					nKCUIStagePrefabNode.SetData(nKMStageTempletV, onSelectedSlot);
					if (!nKCUIStagePrefabNode.CheckLock())
					{
						localPosition = nKCUIStagePrefabNode.transform.localPosition;
					}
					if (flag && !PlayerPrefs.HasKey($"{NKCScenManager.CurrentUserData().m_UserUID}_{nKMStageTempletV.m_StageBattleStrID}") && !myUserData.CheckStageCleared(nKMStageTempletV))
					{
						nKCUIStagePrefabNode.SetEnableNewMark(bValue: true);
					}
					else
					{
						nKCUIStagePrefabNode.SetEnableNewMark(bValue: false);
					}
					if (!nKCUIStagePrefabNode.IsActive())
					{
						NKCUtil.SetGameobjectActive(nKCUIStagePrefabNode, bValue: true);
					}
				}
				else if (nKCUIStagePrefabNode.IsActive())
				{
					NKCUtil.SetGameobjectActive(nKCUIStagePrefabNode, bValue: false);
				}
			}
			else if (nKCUIStagePrefabNode.IsActive())
			{
				NKCUtil.SetGameobjectActive(nKCUIStagePrefabNode, bValue: false);
			}
		}
		if (zero == Vector3.zero)
		{
			return Vector2.zero;
		}
		float x = ((zero.x == 0f) ? 0f : ((localPosition.x - localPosition2.x) / zero.x));
		float y = ((zero.y == 0f) ? 0f : ((localPosition.y - localPosition2.y) / zero.y));
		return new Vector2(x, y);
	}

	public void SelectNode(NKMStageTempletV2 stageTemplet)
	{
		for (int i = 0; i < m_lstItemSlot.Count; i++)
		{
			if (stageTemplet != null)
			{
				m_lstItemSlot[i].SetSelectNode(m_lstItemSlot[i].GetStageIndex() == stageTemplet.m_StageIndex);
			}
			else
			{
				m_lstItemSlot[i].SetSelectNode(bValue: false);
			}
		}
	}

	private void InitSlot(NKCUIStagePrefabNode cItemSlot)
	{
		_ = cItemSlot != null;
	}
}
