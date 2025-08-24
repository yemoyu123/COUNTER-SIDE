using System.Collections.Generic;

namespace NKC;

public class NKCUnitReviewManager
{
	public static bool m_bReceivedUnitReviewBanList = false;

	public static List<long> m_lstBannedUserUid = new List<long>();

	public static void SetBanList(List<long> lstBanUser)
	{
		if (lstBanUser != null)
		{
			m_lstBannedUserUid = lstBanUser;
		}
	}

	public static void AddBanList(long userUid)
	{
		if (!m_lstBannedUserUid.Contains(userUid))
		{
			m_lstBannedUserUid.Add(userUid);
		}
	}

	public static void RemoveBanList(long userUid)
	{
		if (m_lstBannedUserUid.Contains(userUid))
		{
			m_lstBannedUserUid.Remove(userUid);
		}
	}

	public static bool IsBannedUser(long userUid)
	{
		return m_lstBannedUserUid.Contains(userUid);
	}
}
