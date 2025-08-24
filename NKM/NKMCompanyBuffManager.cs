using System;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMCompanyBuffManager
{
	public static NKMCompanyBuffTemplet GetCompanyBuffTemplet(int companyBuffId)
	{
		return NKMTempletContainer<NKMCompanyBuffTemplet>.Find(companyBuffId);
	}

	public static DateTime GetExpireTime(int companyBuffId, DateTime current)
	{
		NKMCompanyBuffTemplet companyBuffTemplet = GetCompanyBuffTemplet(companyBuffId);
		if (companyBuffTemplet == null)
		{
			return DateTime.MinValue;
		}
		return current.AddMinutes(companyBuffTemplet.m_CompanyBuffTime);
	}
}
