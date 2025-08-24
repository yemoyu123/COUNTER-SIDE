using System;
using System.Collections.Generic;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKCCompanyBuffManager
{
	private const float BUFF_REFRESH_INTERVAL = 1f;

	private static float s_fRefreshBuffTimer;

	public static void RegisterCallback(NKMUserData userData)
	{
		if (userData != null)
		{
			userData.dOnUserLevelUpdate = (NKMUserData.OnUserLevelUpdate)Delegate.Combine(userData.dOnUserLevelUpdate, new NKMUserData.OnUserLevelUpdate(OnUserLevelChanged));
		}
	}

	private static void OnUserLevelChanged(NKMUserData userData)
	{
		RefreshBuffList();
	}

	public static void Update(float deltaTime)
	{
		if (s_fRefreshBuffTimer <= 1f)
		{
			s_fRefreshBuffTimer += deltaTime;
			return;
		}
		s_fRefreshBuffTimer = 0f;
		RefreshBuffList();
	}

	private static void RefreshBuffList()
	{
		if (NKCScenManager.CurrentUserData() != null)
		{
			NKCCompanyBuff.RemoveExpiredBuffs(NKCScenManager.CurrentUserData().m_companyBuffDataList);
		}
	}

	public static bool IsCurrentApplyBuff(NKMConst.Buff.BuffType buff)
	{
		if (NKCScenManager.CurrentUserData() != null)
		{
			foreach (NKMCompanyBuffData companyBuffData in NKCScenManager.CurrentUserData().m_companyBuffDataList)
			{
				NKMCompanyBuffTemplet companyBuffTemplet = NKMCompanyBuffManager.GetCompanyBuffTemplet(companyBuffData.Id);
				if (companyBuffTemplet == null)
				{
					continue;
				}
				foreach (NKMCompanyBuffInfo companyBuffInfo in companyBuffTemplet.m_CompanyBuffInfoList)
				{
					if (companyBuffInfo.m_CompanyBuffType == buff)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool IsCurrentApplyBuff(List<int> lstBuffIds)
	{
		if (NKCScenManager.CurrentUserData() != null)
		{
			int i;
			for (i = 0; i < lstBuffIds.Count; i++)
			{
				if (NKCScenManager.CurrentUserData().m_companyBuffDataList.Find((NKMCompanyBuffData x) => x.Id == lstBuffIds[i]) != null)
				{
					return true;
				}
			}
		}
		return false;
	}
}
