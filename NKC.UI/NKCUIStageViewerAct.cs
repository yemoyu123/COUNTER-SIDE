using System.Collections.Generic;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIStageViewerAct : MonoBehaviour
{
	[Header("!! 첫 스테이지부터 차례로 넣어야함 !!")]
	public List<NKCUIEPActDungeonSlot> m_lstItemSlot = new List<NKCUIEPActDungeonSlot>();

	public Vector2 GetTargetPos(int targetIndex, bool bUseNormalizedPos = true)
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
		if (component == null)
		{
			float num = m_lstItemSlot[0].transform.position.x;
			float num2 = m_lstItemSlot[m_lstItemSlot.Count - 1].transform.position.x;
			for (int i = 0; i < m_lstItemSlot.Count; i++)
			{
				float x = m_lstItemSlot[i].transform.position.x;
				if (x < num)
				{
					num = x;
				}
				if (x > num2)
				{
					num2 = x;
				}
			}
			Vector2 result = m_lstItemSlot[targetIndex].transform.position;
			if (bUseNormalizedPos)
			{
				return new Vector2((result.x - num) / (num2 - num), 0f);
			}
			return result;
		}
		float num3 = m_lstItemSlot[0].transform.position.x;
		float num4 = m_lstItemSlot[0].transform.position.x + component.preferredWidth * (float)(m_lstItemSlot.Count - 1);
		for (int j = 0; j < m_lstItemSlot.Count; j++)
		{
			float num5 = m_lstItemSlot[0].transform.position.x + component.preferredWidth * (float)j;
			if (num5 < num3)
			{
				num3 = num5;
			}
			if (num5 > num4)
			{
				num4 = num5;
			}
		}
		Vector2 result2 = new Vector2(m_lstItemSlot[0].transform.position.x + component.preferredWidth * (float)targetIndex, 0f);
		if (bUseNormalizedPos)
		{
			return new Vector2((result2.x - num3) / (num4 - num3), 0f);
		}
		return result2;
	}

	public Vector2 SetData(int EpisodeID, int ActID, EPISODE_DIFFICULTY Difficulty, IDungeonSlot.OnSelectedItemSlot onSelectedSlot, EPISODE_SCROLL_TYPE scrollType)
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(EpisodeID, Difficulty);
		if (nKMEpisodeTempletV == null)
		{
			return Vector2.zero;
		}
		if (!nKMEpisodeTempletV.m_DicStage.ContainsKey(ActID))
		{
			Log.Error($"{base.transform.parent.name} : List<EPExtraData> 찾을 수 없음 - EpisodeID {EpisodeID}, ActID {ActID}, Difficulty {Difficulty}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIStageViewerAct.cs", 93);
			return Vector2.zero;
		}
		int count = nKMEpisodeTempletV.m_DicStage[ActID].Count;
		if (count > m_lstItemSlot.Count)
		{
			Log.Error($"{base.transform.parent.name} : Stage 숫자가 프리팹 숫자보다 많음 - EpisodeID {EpisodeID}, ActID {ActID}, Difficulty {Difficulty}, StageCount : {count}, SlotCount : {m_lstItemSlot.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIStageViewerAct.cs", 101);
			return Vector2.zero;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		Vector3 vector3 = Vector3.zero;
		switch (scrollType)
		{
		case EPISODE_SCROLL_TYPE.HORIZONTAL:
			vector2.x = ((m_lstItemSlot.Count > 0) ? m_lstItemSlot[0].transform.localPosition.x : 0f);
			vector3.x = ((m_lstItemSlot.Count > 0) ? (m_lstItemSlot[m_lstItemSlot.Count - 1].transform.localPosition.x - vector2.x) : 0f);
			break;
		case EPISODE_SCROLL_TYPE.VERTICAL:
			vector2.y = ((m_lstItemSlot.Count > 0) ? m_lstItemSlot[0].transform.localPosition.y : 0f);
			vector3.y = ((m_lstItemSlot.Count > 0) ? (m_lstItemSlot[m_lstItemSlot.Count - 1].transform.localPosition.y - vector2.y) : 0f);
			break;
		case EPISODE_SCROLL_TYPE.FREE:
			vector2 = ((m_lstItemSlot.Count > 0) ? m_lstItemSlot[0].transform.localPosition : Vector3.zero);
			vector3 = ((m_lstItemSlot.Count > 0) ? (m_lstItemSlot[m_lstItemSlot.Count - 1].transform.localPosition - vector2) : Vector3.zero);
			break;
		}
		for (int i = 0; i < m_lstItemSlot.Count; i++)
		{
			NKCUIEPActDungeonSlot nKCUIEPActDungeonSlot = m_lstItemSlot[i];
			InitSlot(nKCUIEPActDungeonSlot);
			nKCUIEPActDungeonSlot.SetEnableNewMark(bValue: false);
			nKCUIEPActDungeonSlot.SetOnSelectedItemSlot(onSelectedSlot);
			if (i < count)
			{
				NKMStageTempletV2 nKMStageTempletV = nKMEpisodeTempletV.m_DicStage[ActID][i];
				bool flag = false;
				flag = NKMEpisodeMgr.CheckEpisodeMission(myUserData, nKMStageTempletV);
				if (!nKMStageTempletV.EnableByTag)
				{
					nKCUIEPActDungeonSlot.SetActive(bSet: false);
				}
				else if (nKMStageTempletV.m_StageBasicUnlockType == STAGE_BASIC_UNLOCK_TYPE.SBUT_OPEN)
				{
					if (flag)
					{
						vector = nKCUIEPActDungeonSlot.transform.localPosition;
						nKCUIEPActDungeonSlot.SetData(ActID, nKMStageTempletV.m_StageIndex, nKMStageTempletV.m_StageBattleStrID, bLock: false, Difficulty);
						if (!PlayerPrefs.HasKey($"{NKCScenManager.CurrentUserData().m_UserUID}_{nKMStageTempletV.m_StageBattleStrID}") && !myUserData.CheckStageCleared(nKMStageTempletV))
						{
							nKCUIEPActDungeonSlot.SetEnableNewMark(bValue: true);
						}
						else
						{
							nKCUIEPActDungeonSlot.SetEnableNewMark(bValue: false);
						}
						if (!nKCUIEPActDungeonSlot.IsActive())
						{
							nKCUIEPActDungeonSlot.SetActive(bSet: true);
						}
					}
					else if (nKCUIEPActDungeonSlot.IsActive())
					{
						nKCUIEPActDungeonSlot.SetActive(bSet: false);
					}
				}
				else if (nKMStageTempletV.m_StageBasicUnlockType == STAGE_BASIC_UNLOCK_TYPE.SBUT_LOCK)
				{
					nKCUIEPActDungeonSlot.SetData(ActID, nKMStageTempletV.m_StageIndex, nKMStageTempletV.m_StageBattleStrID, !flag, Difficulty);
					if (!nKCUIEPActDungeonSlot.CheckLock())
					{
						vector = nKCUIEPActDungeonSlot.transform.localPosition;
					}
					if (flag && !PlayerPrefs.HasKey($"{NKCScenManager.CurrentUserData().m_UserUID}_{nKMStageTempletV.m_StageBattleStrID}") && !myUserData.CheckStageCleared(nKMStageTempletV))
					{
						nKCUIEPActDungeonSlot.SetEnableNewMark(bValue: true);
					}
					else
					{
						nKCUIEPActDungeonSlot.SetEnableNewMark(bValue: false);
					}
					if (!nKCUIEPActDungeonSlot.IsActive())
					{
						nKCUIEPActDungeonSlot.SetActive(bSet: true);
					}
				}
				else if (nKCUIEPActDungeonSlot.IsActive())
				{
					nKCUIEPActDungeonSlot.SetActive(bSet: false);
				}
			}
			else if (nKCUIEPActDungeonSlot.IsActive())
			{
				nKCUIEPActDungeonSlot.SetActive(bSet: false);
			}
		}
		if (vector3 == Vector3.zero)
		{
			return Vector2.zero;
		}
		float x = ((vector3.x == 0f) ? 0f : ((vector.x - vector2.x) / vector3.x));
		float y = ((vector3.y == 0f) ? 0f : ((vector.y - vector2.y) / vector3.y));
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

	private void InitSlot(NKCUIEPActDungeonSlot cItemSlot)
	{
		_ = cItemSlot != null;
	}
}
