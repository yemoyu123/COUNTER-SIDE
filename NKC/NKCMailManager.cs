using System.Collections.Generic;
using ClientPacket.User;
using NKC.UI;
using NKM;
using NKM.Templet;

namespace NKC;

internal static class NKCMailManager
{
	private class LongComparer : IComparer<long>
	{
		public int Compare(long a, long b)
		{
			return b.CompareTo(a);
		}
	}

	public delegate void OnMailFlagChange(bool bHaveMail);

	public delegate void OnMailCountChange(int TotalCount);

	private static SortedList<long, NKMPostData> s_slstPostData = new SortedList<long, NKMPostData>(new LongComparer());

	private static int s_MailTotalCount;

	private static int s_MailAllowReceiveAllCount;

	public static OnMailFlagChange dOnMailFlagChange;

	public static OnMailCountChange dOnMailCountChange;

	private static bool s_bServerHaveNewMail = false;

	private const float MAIL_REFRESH_INTERVAL = 5f;

	private static float s_fRefreshMailTimer = 5f;

	private const float MAIL_NEXT_INTERVAL = 1f;

	private static float s_fNextMailTimer = 1f;

	private static bool s_bRefreshOnNextInterval = false;

	public static int GetReceivedMailCount()
	{
		if (s_slstPostData == null)
		{
			return 0;
		}
		return s_slstPostData.Count;
	}

	public static int GetTotalMailCount()
	{
		return s_MailTotalCount;
	}

	public static int GetAllowedReceiveAllCount()
	{
		return s_MailAllowReceiveAllCount;
	}

	public static NKMPostData GetMailByIndex(int index)
	{
		if (index < s_slstPostData.Count)
		{
			return s_slstPostData.Values[index];
		}
		return null;
	}

	public static NKMPostData GetMailByPostID(long postID)
	{
		if (s_slstPostData.TryGetValue(postID, out var value))
		{
			return value;
		}
		return null;
	}

	private static void NotifyToObservers()
	{
		if (dOnMailFlagChange != null)
		{
			dOnMailFlagChange(HasNewMail());
		}
		if (dOnMailCountChange != null)
		{
			dOnMailCountChange(s_MailTotalCount);
		}
	}

	public static bool HasNewMail()
	{
		if (!s_bServerHaveNewMail)
		{
			return GetTotalMailCount() > 0;
		}
		return true;
	}

	private static void SetRefreshOnNextInterval()
	{
		s_bRefreshOnNextInterval = true;
	}

	public static bool CanRefreshMail()
	{
		if (NKCScenManager.CurrentUserData() == null)
		{
			return false;
		}
		if (NKCScenManager.CurrentUserData().m_UserNickName == null)
		{
			return false;
		}
		return s_fRefreshMailTimer >= 5f;
	}

	public static bool CanGetNextMail()
	{
		if (s_fNextMailTimer >= 1f)
		{
			return GetTotalMailCount() > GetReceivedMailCount();
		}
		return false;
	}

	public static void Update(float deltaTime)
	{
		if (s_fNextMailTimer <= 1f)
		{
			s_fNextMailTimer += deltaTime;
		}
		if (s_fRefreshMailTimer <= 5f)
		{
			s_fRefreshMailTimer += deltaTime;
		}
		else if (s_bRefreshOnNextInterval)
		{
			RefreshMailList();
		}
	}

	public static void Cleanup()
	{
		s_fRefreshMailTimer = 5f;
		s_fNextMailTimer = 1f;
		s_slstPostData.Clear();
		s_MailTotalCount = 0;
		s_MailAllowReceiveAllCount = 0;
		s_bServerHaveNewMail = false;
	}

	public static void AddMail(List<NKMPostData> lstPostData, int newTotalCount)
	{
		s_MailAllowReceiveAllCount = 0;
		if (newTotalCount >= 0)
		{
			s_MailTotalCount = newTotalCount;
		}
		foreach (NKMPostData lstPostDatum in lstPostData)
		{
			s_slstPostData[lstPostDatum.postIndex] = lstPostDatum;
			if (NKMPostTemplet.Find(lstPostDatum.postId).AllowReceiveAll)
			{
				s_MailAllowReceiveAllCount++;
			}
		}
		NotifyToObservers();
	}

	private static long GetLastMailIndex()
	{
		if (s_slstPostData.Count != 0)
		{
			return s_slstPostData.Keys[s_slstPostData.Count - 1];
		}
		return 0L;
	}

	public static IEnumerable<NKMPostData> GetPostList()
	{
		return s_slstPostData.Values;
	}

	public static void OnPostReceive(NKMPacket_POST_RECEIVE_ACK sPacket)
	{
		if (sPacket.postCount >= 0)
		{
			s_MailTotalCount = sPacket.postCount;
		}
		if (sPacket.postIndex == 0L)
		{
			List<long> list = new List<long>();
			foreach (KeyValuePair<long, NKMPostData> s_slstPostDatum in s_slstPostData)
			{
				if (NKMPostTemplet.Find(s_slstPostDatum.Value.postId).AllowReceiveAll)
				{
					list.Add(s_slstPostDatum.Key);
				}
			}
			s_MailAllowReceiveAllCount -= list.Count;
			for (int i = 0; i < list.Count; i++)
			{
				s_slstPostData.Remove(list[i]);
			}
			s_bServerHaveNewMail = false;
			if (sPacket.postCount != 0)
			{
				SetRefreshOnNextInterval();
			}
		}
		else
		{
			RemoveMail(sPacket.postIndex);
		}
		NotifyToObservers();
	}

	private static void RemoveMail(long index)
	{
		if (s_slstPostData.ContainsKey(index))
		{
			if (NKMPostTemplet.Find(s_slstPostData[index].postId).AllowReceiveAll)
			{
				s_MailAllowReceiveAllCount--;
			}
			s_slstPostData.Remove(index);
		}
	}

	public static void CheckAndRemoveExpiredMail()
	{
		List<long> list = new List<long>();
		foreach (KeyValuePair<long, NKMPostData> s_slstPostDatum in s_slstPostData)
		{
			NKMPostData value = s_slstPostDatum.Value;
			if (value.expirationDate < NKMConst.Post.UnlimitedExpirationUtcDate && NKCSynchronizedTime.IsFinished(value.expirationDate))
			{
				list.Add(value.postIndex);
			}
		}
		foreach (long item in list)
		{
			RemoveMail(item);
		}
		if (list.Count > 0)
		{
			NotifyToObservers();
		}
	}

	public static void RefreshMailList()
	{
		if (CanRefreshMail())
		{
			s_bRefreshOnNextInterval = false;
			s_fRefreshMailTimer = 0f;
			s_slstPostData.Clear();
			s_bServerHaveNewMail = false;
			NKCPacketSender.Send_NKMPacket_POST_LIST_REQ(0L);
		}
	}

	public static void GetNextMail()
	{
		if (CanGetNextMail())
		{
			s_fNextMailTimer = 0f;
			s_bServerHaveNewMail = false;
			NKCPacketSender.Send_NKMPacket_POST_LIST_REQ(GetLastMailIndex());
		}
	}

	public static void OnNewMailNotify(int mailcount = 1)
	{
		s_bServerHaveNewMail = true;
		s_MailTotalCount += mailcount;
		if (NKCUIMail.IsInstanceOpen)
		{
			SetRefreshOnNextInterval();
		}
		NotifyToObservers();
	}
}
