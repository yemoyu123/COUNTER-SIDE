using System.Collections.Generic;
using ClientPacket.Unit;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public static class NKCUnitMissionManager
{
	public static List<NKMUnitMissionData> completedUnitMissionList;

	public static List<NKMUnitMissionData> rewardEnableUnitMissionList;

	private static Dictionary<NKM_UNIT_GRADE, List<NKMUnitMissionStepTemplet>> unitMissionStepTempletDic;

	public static void Init()
	{
		if (unitMissionStepTempletDic != null && unitMissionStepTempletDic.Count > 0)
		{
			return;
		}
		unitMissionStepTempletDic = new Dictionary<NKM_UNIT_GRADE, List<NKMUnitMissionStepTemplet>>();
		foreach (NKMUnitMissionTemplet value in NKMTempletContainer<NKMUnitMissionTemplet>.Values)
		{
			if (!unitMissionStepTempletDic.ContainsKey(value.UnitGrade))
			{
				unitMissionStepTempletDic.Add(value.UnitGrade, new List<NKMUnitMissionStepTemplet>());
			}
			int count = value.Steps.Count;
			for (int i = 0; i < count; i++)
			{
				unitMissionStepTempletDic[value.UnitGrade].Add(value.Steps[i]);
			}
		}
		foreach (KeyValuePair<NKM_UNIT_GRADE, List<NKMUnitMissionStepTemplet>> item in unitMissionStepTempletDic)
		{
			item.Value.Sort(delegate(NKMUnitMissionStepTemplet e1, NKMUnitMissionStepTemplet e2)
			{
				if (e1.StepId > e2.StepId)
				{
					return 1;
				}
				return (e1.StepId < e2.StepId) ? (-1) : 0;
			});
		}
	}

	public static void SetCompletedUnitMissionData(List<NKMUnitMissionData> completedUnitMissions)
	{
		completedUnitMissionList = completedUnitMissions;
	}

	public static void SetRewardUnitMissionData(List<NKMUnitMissionData> rewardEnableUnitMissions)
	{
		rewardEnableUnitMissionList = rewardEnableUnitMissions;
	}

	public static void UpdateCompletedUnitMissionData(NKMUnitMissionData completedMissionData)
	{
		if (completedMissionData == null)
		{
			return;
		}
		if (GetCompletedUnitMissionData(completedMissionData.unitId, completedMissionData.missionId, completedMissionData.stepId) == null)
		{
			if (completedUnitMissionList == null)
			{
				completedUnitMissionList = new List<NKMUnitMissionData>();
			}
			completedUnitMissionList.Add(completedMissionData);
		}
		if (rewardEnableUnitMissionList != null)
		{
			int num = rewardEnableUnitMissionList.FindIndex((NKMUnitMissionData e) => e.missionId == completedMissionData.missionId && e.unitId == completedMissionData.unitId && e.stepId == completedMissionData.stepId);
			if (num >= 0 && num < rewardEnableUnitMissionList.Count)
			{
				rewardEnableUnitMissionList.RemoveAt(num);
			}
		}
	}

	public static void UpdateCompletedUnitMissionData(List<NKMUnitMissionData> missionDataList)
	{
		int count = missionDataList.Count;
		for (int i = 0; i < count; i++)
		{
			UpdateCompletedUnitMissionData(missionDataList[i]);
		}
	}

	public static void UpdateRewardEnableMissionData(List<NKMUnitMissionData> missionDataList)
	{
		if (rewardEnableUnitMissionList == null)
		{
			rewardEnableUnitMissionList = missionDataList;
			return;
		}
		int count = missionDataList.Count;
		int i = 0;
		while (i < count)
		{
			if (rewardEnableUnitMissionList.Find((NKMUnitMissionData e) => e.unitId == missionDataList[i].unitId && e.missionId == missionDataList[i].missionId && e.stepId == missionDataList[i].stepId) == null)
			{
				rewardEnableUnitMissionList.Add(missionDataList[i]);
			}
			int num = i + 1;
			i = num;
		}
	}

	private static NKMUnitMissionData GetCompletedUnitMissionData(int unitId, int missionId, int stepId)
	{
		if (completedUnitMissionList == null)
		{
			return null;
		}
		return completedUnitMissionList.Find((NKMUnitMissionData e) => e.unitId == unitId && e.missionId == missionId && e.stepId == stepId);
	}

	private static NKMUnitMissionData GetRewardEnableUnitMissionData(int unitId, int missionId, int stepId)
	{
		if (rewardEnableUnitMissionList == null)
		{
			return null;
		}
		return rewardEnableUnitMissionList.Find((NKMUnitMissionData e) => e.unitId == unitId && e.missionId == missionId && e.stepId == stepId);
	}

	public static void GetUnitMissionAchievedCount(int unitId, ref int total, ref int achieved)
	{
		if (NKMUnitManager.GetUnitTempletBase(unitId) == null)
		{
			return;
		}
		List<NKMUnitMissionStepTemplet> unitMissionStepTempletList = GetUnitMissionStepTempletList(unitId);
		int count = unitMissionStepTempletList.Count;
		total += count;
		for (int i = 0; i < count; i++)
		{
			if (GetCompletedUnitMissionData(unitId, unitMissionStepTempletList[i].Owner.MissionId, unitMissionStepTempletList[i].StepId) != null)
			{
				achieved++;
			}
		}
	}

	public static void GetUnitMissionRewardEnableCount(int unitId, ref int total, ref int completed, ref int rewardEnable)
	{
		if (NKMUnitManager.GetUnitTempletBase(unitId) == null)
		{
			return;
		}
		List<NKMUnitMissionStepTemplet> unitMissionStepTempletList = GetUnitMissionStepTempletList(unitId);
		int count = unitMissionStepTempletList.Count;
		total += count;
		for (int i = 0; i < count; i++)
		{
			if (GetCompletedUnitMissionData(unitId, unitMissionStepTempletList[i].Owner.MissionId, unitMissionStepTempletList[i].StepId) != null)
			{
				completed++;
			}
			if (GetRewardEnableUnitMissionData(unitId, unitMissionStepTempletList[i].Owner.MissionId, unitMissionStepTempletList[i].StepId) != null)
			{
				rewardEnable++;
			}
		}
	}

	public static List<NKMUnitMissionStepTemplet> GetUnitMissionStepTempletList(int unitId)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitId);
		if (unitTempletBase == null)
		{
			return new List<NKMUnitMissionStepTemplet>();
		}
		if (unitMissionStepTempletDic == null || !unitMissionStepTempletDic.ContainsKey(unitTempletBase.m_NKM_UNIT_GRADE))
		{
			return new List<NKMUnitMissionStepTemplet>();
		}
		return unitMissionStepTempletDic[unitTempletBase.m_NKM_UNIT_GRADE];
	}

	public static bool GetOpenTagCollectionMission()
	{
		return NKMOpenTagManager.IsOpened("TAG_COLLECTION_MISSION");
	}

	public static bool GetOpenTagCollectionTeamUp()
	{
		return NKMOpenTagManager.IsOpened("TAG_COLLECTION_TEAMUP_REWARD");
	}

	public static bool HasRewardEnableMission()
	{
		if (!GetOpenTagCollectionMission())
		{
			return false;
		}
		if (rewardEnableUnitMissionList != null)
		{
			return rewardEnableUnitMissionList.Count > 0;
		}
		return false;
	}

	public static bool HasRewardEnableMission(int unitId)
	{
		List<NKMUnitMissionStepTemplet> unitMissionStepTempletList = GetUnitMissionStepTempletList(unitId);
		int count = unitMissionStepTempletList.Count;
		for (int i = 0; i < count; i++)
		{
			if (GetRewardEnableUnitMissionData(unitId, unitMissionStepTempletList[i].Owner.MissionId, unitMissionStepTempletList[i].StepId) != null)
			{
				return true;
			}
		}
		return false;
	}

	public static int GetRewardEnableStepId(int unitId, int missionId)
	{
		if (rewardEnableUnitMissionList == null || rewardEnableUnitMissionList.Count <= 0)
		{
			return 0;
		}
		List<NKMUnitMissionData> list = rewardEnableUnitMissionList.FindAll((NKMUnitMissionData e) => e.unitId == unitId && e.missionId == missionId);
		if (list == null || list.Count <= 0)
		{
			return 0;
		}
		list.Sort(delegate(NKMUnitMissionData e1, NKMUnitMissionData e2)
		{
			if (e1.stepId > e2.stepId)
			{
				return 1;
			}
			return (e1.stepId < e2.stepId) ? (-1) : 0;
		});
		return list[0].stepId;
	}

	public static NKMMissionManager.MissionStateData GetMissionState(int unitId, NKMUnitMissionStepTemplet unitMissionStepTemplet)
	{
		if (unitMissionStepTemplet == null)
		{
			return new NKMMissionManager.MissionStateData(NKMMissionManager.MissionState.LOCKED);
		}
		NKMUnitMissionData completedUnitMissionData = GetCompletedUnitMissionData(unitId, unitMissionStepTemplet.Owner.MissionId, unitMissionStepTemplet.StepId);
		int num = 0;
		if (completedUnitMissionData != null)
		{
			num = unitMissionStepTemplet.MissionValue;
			return new NKMMissionManager.MissionStateData(NKMMissionManager.MissionState.COMPLETED, num);
		}
		List<NKMUnitData> list = NKCScenManager.CurrentUserData()?.m_ArmyData.GetUnitListByUnitID(unitId);
		int count = list.Count;
		switch (unitMissionStepTemplet.Owner.MissionCondition)
		{
		case NKM_MISSION_COND.UNIT_GROWTH_LEVEL:
		{
			for (int l = 0; l < count; l++)
			{
				if (num < list[l].m_UnitLevel)
				{
					num = list[l].m_UnitLevel;
				}
			}
			break;
		}
		case NKM_MISSION_COND.UNIT_GROWTH_LOYALTY:
		{
			for (int j = 0; j < count; j++)
			{
				if (num < list[j].loyalty)
				{
					num = list[j].loyalty;
				}
			}
			break;
		}
		case NKM_MISSION_COND.UNIT_GROWTH_PERMANENT:
		{
			for (int k = 0; k < count; k++)
			{
				if (list[k].IsPermanentContract)
				{
					num = 1;
					break;
				}
			}
			break;
		}
		case NKM_MISSION_COND.UNIT_GROWTH_TACTICAL:
		{
			for (int i = 0; i < count; i++)
			{
				if (num < list[i].tacticLevel)
				{
					num = list[i].tacticLevel;
				}
			}
			break;
		}
		}
		num = Mathf.Min(num, unitMissionStepTemplet.MissionValue);
		if (GetRewardEnableUnitMissionData(unitId, unitMissionStepTemplet.Owner.MissionId, unitMissionStepTemplet.StepId) != null)
		{
			return new NKMMissionManager.MissionStateData(NKMMissionManager.MissionState.CAN_COMPLETE, unitMissionStepTemplet.MissionValue);
		}
		return new NKMMissionManager.MissionStateData(NKMMissionManager.MissionState.ONGOING, num);
	}
}
