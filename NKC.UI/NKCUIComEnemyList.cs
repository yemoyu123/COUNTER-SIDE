using System.Collections.Generic;
using System.Linq;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComEnemyList : MonoBehaviour
{
	public Text m_lbEnemyLevel;

	public GameObject m_NKM_UI_OPERATION_POPUP_ENEMY_Content;

	private List<NKCDeckViewEnemySlot> m_lstNKCDeckViewEnemySlot = new List<NKCDeckViewEnemySlot>();

	private float m_fInitPosXOfEnemyContent;

	private string m_StageBattleStrID;

	private bool m_bInitComplete;

	private NKM_UNIT_SOURCE_TYPE m_sourceTypeMain;

	private NKM_UNIT_SOURCE_TYPE m_sourceTypeSub;

	public void InitUI()
	{
		m_fInitPosXOfEnemyContent = m_NKM_UI_OPERATION_POPUP_ENEMY_Content.transform.localPosition.x;
		m_bInitComplete = true;
	}

	public void SetData(int stageID)
	{
		SetData(NKMStageTempletV2.Find(stageID));
	}

	public void SetData(NKMStageTempletV2 stageTemplet)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		if (stageTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_StageBattleStrID = stageTemplet.m_StageBattleStrID;
		(NKM_UNIT_SOURCE_TYPE, NKM_UNIT_SOURCE_TYPE) sourceTypes = stageTemplet.GetSourceTypes();
		SetEnemyListUI(Get_dicEnemyUnitStrIDs(), sourceTypes.Item1, sourceTypes.Item2);
		SetEnemyLevel();
	}

	public void SetData(NKMDungeonTempletBase cNKMDungeonTempletBase)
	{
		if (!m_bInitComplete)
		{
			InitUI();
		}
		if (cNKMDungeonTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_StageBattleStrID = cNKMDungeonTempletBase.m_DungeonStrID;
		Dictionary<string, NKCEnemyData> enemyUnits = NKMDungeonManager.GetEnemyUnits(cNKMDungeonTempletBase);
		SetEnemyListUI(enemyUnits, cNKMDungeonTempletBase.m_StageSourceTypeMain, cNKMDungeonTempletBase.m_StageSourceTypeSub);
		SetEnemyLevel();
	}

	private void SetEnemyLevel()
	{
		int num = 0;
		bool flag = true;
		NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_StageBattleStrID);
		if (nKMWarfareTemplet != null)
		{
			num = nKMWarfareTemplet.m_WarfareLevel;
			NKCUtil.SetLabelText(m_lbEnemyLevel, string.Format(NKCUtilString.GET_STRING_DUNGEON_LEVEL_ONE_PARAM, num));
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_StageBattleStrID);
		if (dungeonTempletBase != null)
		{
			if (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
			{
				flag = false;
			}
			if (dungeonTempletBase.StageTemplet != null && (dungeonTempletBase.StageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE || dungeonTempletBase.StageTemplet.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL))
			{
				flag = false;
			}
			num = dungeonTempletBase.m_DungeonLevel;
			if (flag)
			{
				NKCUtil.SetLabelText(m_lbEnemyLevel, string.Format(NKCUtilString.GET_STRING_DUNGEON_LEVEL_ONE_PARAM, num));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbEnemyLevel, "");
			}
		}
		else
		{
			NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(m_StageBattleStrID);
			if (nKMPhaseTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbEnemyLevel, string.Format(NKCUtilString.GET_STRING_DUNGEON_LEVEL_ONE_PARAM, nKMPhaseTemplet.PhaseLevel));
			}
		}
	}

	private Dictionary<string, NKCEnemyData> Get_dicEnemyUnitStrIDs()
	{
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(m_StageBattleStrID);
		if (nKMStageTempletV != null)
		{
			return NKMDungeonManager.GetEnemyUnits(nKMStageTempletV);
		}
		return NKMDungeonManager.GetEnemyUnits(NKMDungeonManager.GetDungeonTempletBase(m_StageBattleStrID));
	}

	private void SetEnemyListUI(Dictionary<string, NKCEnemyData> dicEnemyUnitStrIDs, NKM_UNIT_SOURCE_TYPE sourceTypeMain = NKM_UNIT_SOURCE_TYPE.NUST_NONE, NKM_UNIT_SOURCE_TYPE sourceTypeSub = NKM_UNIT_SOURCE_TYPE.NUST_NONE)
	{
		Vector3 localPosition = m_NKM_UI_OPERATION_POPUP_ENEMY_Content.transform.localPosition;
		m_NKM_UI_OPERATION_POPUP_ENEMY_Content.transform.localPosition = new Vector3(m_fInitPosXOfEnemyContent, localPosition.y, localPosition.z);
		if (dicEnemyUnitStrIDs == null)
		{
			return;
		}
		m_sourceTypeMain = sourceTypeMain;
		m_sourceTypeSub = sourceTypeSub;
		List<NKCEnemyData> list = new List<NKCEnemyData>(dicEnemyUnitStrIDs.Values);
		list.Sort(new NKCEnemyData.CompNED());
		for (int num = list.Count - 1; num >= 0; num--)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(list[num].m_UnitStrID);
			if (nKMUnitTempletBase == null || nKMUnitTempletBase.m_bHideBattleResult)
			{
				list.RemoveAt(num);
			}
		}
		int num2 = 0;
		if (list.Count > m_lstNKCDeckViewEnemySlot.Count)
		{
			int count = m_lstNKCDeckViewEnemySlot.Count;
			for (num2 = 0; num2 < list.Count - count; num2++)
			{
				NKCDeckViewEnemySlot newInstance = NKCDeckViewEnemySlot.GetNewInstance(m_NKM_UI_OPERATION_POPUP_ENEMY_Content.transform);
				if (!(newInstance == null))
				{
					newInstance.Init(num2 + count);
					newInstance.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
					Vector3 localPosition2 = newInstance.gameObject.transform.localPosition;
					newInstance.gameObject.transform.localPosition = new Vector3(localPosition2.x, localPosition2.y, 0f);
					m_lstNKCDeckViewEnemySlot.Add(newInstance);
				}
			}
		}
		for (num2 = 0; num2 < list.Count; num2++)
		{
			NKCDeckViewEnemySlot nKCDeckViewEnemySlot = m_lstNKCDeckViewEnemySlot[num2];
			nKCDeckViewEnemySlot.SetEnemyData(NKMUnitManager.GetUnitTempletBase(list[num2].m_UnitStrID), list[num2]);
			nKCDeckViewEnemySlot.m_NKCUIComButton.PointerClick.RemoveAllListeners();
			nKCDeckViewEnemySlot.m_NKCUIComButton.PointerClick.AddListener(OnClickEnemySlot);
			NKCUtil.SetGameobjectActive(nKCDeckViewEnemySlot.gameObject, bValue: true);
			if (sourceTypeMain != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				nKCDeckViewEnemySlot.OverrideUnitSourceType(sourceTypeMain, sourceTypeSub);
			}
		}
		for (; num2 < m_lstNKCDeckViewEnemySlot.Count; num2++)
		{
			NKCUtil.SetGameobjectActive(m_lstNKCDeckViewEnemySlot[num2].gameObject, bValue: false);
		}
	}

	private void OnClickEnemySlot()
	{
		foreach (NKCDeckViewEnemySlot item in m_lstNKCDeckViewEnemySlot)
		{
			item.m_NKCUIComButton.Select(bSelect: false);
		}
		NKCPopupEnemyList.Instance.Open(Get_dicEnemyUnitStrIDs().Values.ToList(), m_sourceTypeMain, m_sourceTypeSub);
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_lstNKCDeckViewEnemySlot.Count; i++)
		{
			m_lstNKCDeckViewEnemySlot[i].CloseInstance();
		}
		m_lstNKCDeckViewEnemySlot.Clear();
	}
}
