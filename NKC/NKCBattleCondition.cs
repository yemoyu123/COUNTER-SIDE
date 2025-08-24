using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCBattleCondition
{
	private List<NKCAssetInstanceData> m_lstBCObj = new List<NKCAssetInstanceData>();

	private List<NKMBattleConditionTemplet> m_lstNKMBattleConditionTemplet = new List<NKMBattleConditionTemplet>();

	public void Init()
	{
		Close();
	}

	public void Close()
	{
		for (int i = 0; i < m_lstNKMBattleConditionTemplet.Count; i++)
		{
			if (m_lstNKMBattleConditionTemplet[i] != null)
			{
				m_lstNKMBattleConditionTemplet[i] = null;
			}
		}
		for (int j = 0; j < m_lstBCObj.Count; j++)
		{
			NKCAssetResourceManager.CloseInstance(m_lstBCObj[j]);
			m_lstBCObj[j] = null;
		}
	}

	public void Load(List<int> lstBC)
	{
		Init();
		if (lstBC.Count <= 0)
		{
			m_lstNKMBattleConditionTemplet.Clear();
			return;
		}
		for (int i = 0; i < lstBC.Count; i++)
		{
			NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(lstBC[i]);
			if (templetByID != null && !string.IsNullOrEmpty(templetByID.BattleCondMapStrID))
			{
				NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_ENV", templetByID.BattleCondMapStrID, bAsync: true);
				if (nKCAssetInstanceData != null)
				{
					m_lstNKMBattleConditionTemplet.Add(templetByID);
					m_lstBCObj.Add(nKCAssetInstanceData);
				}
			}
		}
	}

	public void LoadComplete()
	{
		if (m_lstBCObj.Count <= 0)
		{
			return;
		}
		foreach (NKCAssetInstanceData item in m_lstBCObj)
		{
			if (item != null)
			{
				if (!item.m_Instant.activeSelf)
				{
					item.m_Instant.SetActive(value: true);
				}
				item.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUM_GAME_BATTLE_EFFECT()
					.transform);
					item.m_Instant.transform.localPosition = Vector3.zero;
					item.m_Instant.transform.localScale = Vector3.one;
				}
			}
		}

		public bool IsBC()
		{
			return m_lstNKMBattleConditionTemplet.Count > 0;
		}

		public void EnableDelayedBC(NKMBattleConditionTemplet battleConditionTemplet)
		{
			if (battleConditionTemplet != null && !string.IsNullOrEmpty(battleConditionTemplet.BattleCondMapStrID))
			{
				NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_ENV", battleConditionTemplet.BattleCondMapStrID);
				if (nKCAssetInstanceData != null)
				{
					m_lstNKMBattleConditionTemplet.Add(battleConditionTemplet);
					m_lstBCObj.Add(nKCAssetInstanceData);
					NKCUtil.SetGameobjectActive(nKCAssetInstanceData.m_Instant, bValue: true);
					nKCAssetInstanceData.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
						.Get_NUM_GAME_BATTLE_EFFECT()
						.transform);
						nKCAssetInstanceData.m_Instant.transform.localPosition = Vector3.zero;
						nKCAssetInstanceData.m_Instant.transform.localScale = Vector3.one;
					}
				}
			}
		}
