using System;
using System.Collections.Generic;
using NKC.Office;
using NKC.Templet;
using NKC.Templet.Office;
using NKM;
using NKM.EventPass;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Office;
using NKM.Templet.Recall;
using NKM.Unit;
using UnityEngine;

namespace NKC.Util;

public static class NKCTempletUtility
{
	public static void PostJoin()
	{
		NKMMissionManager.PostJoin();
		NKMAttendanceManager.PostJoin();
		NKCLoginCutSceneManager.PostJoin();
		NKCTempletContainerUtil.InvokePostJoin();
		ShopTabTempletContainer.PostJoin();
		NKCPVPManager.PostJoin();
		NKMRewardManager.InvokeJoin();
	}

	public static void CleanupAllTemplets()
	{
		NKCStringTable.Clear();
		NKCTempletContainerUtil.InvokeDrop();
		NKMOpenTagManager.Drop();
		NKMUnitMissionStepTemplet.Drop();
		NKMPotentialOptionTemplet.Drop();
		ShopTabTempletContainer.Drop();
		NKMOfficeGradeTemplet.Drop();
		NKMUnitExpTableContainer.Drop();
		NKCOfficeFurnitureInteractionTemplet.Drop();
		NKCOfficeUnitInteractionTemplet.Drop();
		NKCShopManager.Drop();
		NKCOfficeManager.Drop();
		NKCShopCustomTabTemplet.Drop();
		NKMEventPassRewardTemplet.Drop();
		NKCGameEventManager.Drop();
		NKMRecallTemplet.Drop();
		NKMRecallUnitExchangeTemplet.Drop();
	}

	public static T PickRatio<T>(List<T> lstTargets, Func<T, int> ratioSelector) where T : class
	{
		if (lstTargets == null || lstTargets.Count == 0)
		{
			return null;
		}
		if (lstTargets.Count == 1)
		{
			return lstTargets[0];
		}
		int num = 0;
		foreach (T lstTarget in lstTargets)
		{
			num += ratioSelector(lstTarget);
		}
		int num2 = UnityEngine.Random.Range(0, num);
		foreach (T lstTarget2 in lstTargets)
		{
			num2 -= ratioSelector(lstTarget2);
			if (num2 < 0)
			{
				return lstTarget2;
			}
		}
		return null;
	}

	public static T PickRatio<T>(IEnumerable<T> lstTargets, Func<T, int> ratioSelector) where T : class
	{
		if (lstTargets == null)
		{
			return null;
		}
		int num = 0;
		foreach (T lstTarget in lstTargets)
		{
			num += ratioSelector(lstTarget);
		}
		int num2 = UnityEngine.Random.Range(0, num);
		foreach (T lstTarget2 in lstTargets)
		{
			num2 -= ratioSelector(lstTarget2);
			if (num2 < 0)
			{
				return lstTarget2;
			}
		}
		return null;
	}

	public static void InvokeJoin()
	{
		NKCTempletContainerUtil.InvokeJoin();
	}
}
