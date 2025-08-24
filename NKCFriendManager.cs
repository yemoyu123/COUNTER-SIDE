using System.Collections.Generic;
using ClientPacket.Common;
using NKC;
using NKM;
using NKM.Templet;

public static class NKCFriendManager
{
	private static List<long> m_friendList = new List<long>();

	private static List<long> m_receivdREQList = new List<long>();

	private static List<long> m_blockList = new List<long>();

	private static bool m_bFriendChanged = false;

	private static bool m_bReceivedREQChanged = false;

	private static bool m_bBlockChanged = false;

	private static List<FriendListData> m_friendListData = new List<FriendListData>();

	public static List<long> FriendList => m_friendList;

	public static List<long> ReceivedREQList => m_receivdREQList;

	public static List<long> BlockList => m_blockList;

	public static List<FriendListData> FriendListData => m_friendListData;

	public static bool IsDirty
	{
		get
		{
			if (!m_bFriendChanged && !m_bReceivedREQChanged)
			{
				return m_bBlockChanged;
			}
			return true;
		}
	}

	public static void Initialize()
	{
		m_bFriendChanged = false;
		m_bReceivedREQChanged = false;
		m_bBlockChanged = false;
		m_friendList.Clear();
		m_friendListData.Clear();
		m_receivdREQList.Clear();
		m_blockList.Clear();
	}

	public static void SetFriendList(List<FriendListData> list)
	{
		m_friendListData = list;
		m_friendList.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			m_friendList.Add(list[i].commonProfile.friendCode);
		}
		m_bFriendChanged = false;
	}

	public static void AddFriend(FriendListData friendListData)
	{
		m_friendListData.Add(friendListData);
		m_friendList.Add(friendListData.commonProfile.friendCode);
		m_bFriendChanged = true;
	}

	public static void AddFriend(long friendCode)
	{
		if (!m_friendList.Contains(friendCode))
		{
			m_friendList.Add(friendCode);
			m_bFriendChanged = true;
		}
	}

	public static void DeleteFriend(long friendCode)
	{
		int num = m_friendListData.FindIndex((FriendListData x) => x.commonProfile.friendCode == friendCode);
		if (num >= 0)
		{
			m_friendListData.RemoveAt(num);
			m_bFriendChanged = true;
		}
		if (m_friendList.Contains(friendCode))
		{
			m_friendList.Remove(friendCode);
			m_bFriendChanged = true;
		}
	}

	public static bool IsFriend(long friendCode)
	{
		return m_friendList.Contains(friendCode);
	}

	public static int GetFriendCount()
	{
		return m_friendListData.Count;
	}

	public static void SetReceivedREQList(List<FriendListData> list)
	{
		m_receivdREQList.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			if (!m_receivdREQList.Contains(list[i].commonProfile.friendCode))
			{
				m_receivdREQList.Add(list[i].commonProfile.friendCode);
			}
		}
		m_bReceivedREQChanged = false;
		if (list.Count > 0)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().SetAddReceivedNew(bSet: true);
			NKCScenManager.GetScenManager().Get_SCEN_HOME().SetFriendNewIcon(bSet: true);
		}
		else
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().SetAddReceivedNew(bSet: false);
			NKCScenManager.GetScenManager().Get_SCEN_HOME().SetFriendNewIcon(bSet: false);
		}
	}

	public static void AddReceivedREQ(long friendCode)
	{
		if (!m_receivdREQList.Contains(friendCode))
		{
			m_receivdREQList.Add(friendCode);
			m_bReceivedREQChanged = true;
		}
		if (m_receivdREQList.Count > 0)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME().SetFriendNewIcon(bSet: true);
		}
	}

	public static void RemoveReceivedREQ(long friendCode)
	{
		if (m_receivdREQList.Contains(friendCode))
		{
			m_receivdREQList.Remove(friendCode);
			m_bReceivedREQChanged = true;
		}
		if (m_receivdREQList.Count > 0)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().SetAddReceivedNew(bSet: true);
			NKCScenManager.GetScenManager().Get_SCEN_HOME().SetFriendNewIcon(bSet: true);
		}
		else
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FRIEND().SetAddReceivedNew(bSet: false);
			NKCScenManager.GetScenManager().Get_SCEN_HOME().SetFriendNewIcon(bSet: false);
		}
	}

	public static void SetBlockList(List<FriendListData> list)
	{
		m_blockList.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			if (!m_blockList.Contains(list[i].commonProfile.friendCode))
			{
				m_blockList.Add(list[i].commonProfile.friendCode);
			}
		}
		m_bBlockChanged = false;
	}

	public static void AddBlockUser(long friendCode)
	{
		if (!m_blockList.Contains(friendCode))
		{
			m_blockList.Add(friendCode);
			m_bBlockChanged = true;
		}
	}

	public static void RemoveBlockUser(long friendCode)
	{
		if (m_blockList.Contains(friendCode))
		{
			m_blockList.Remove(friendCode);
			m_bBlockChanged = true;
		}
	}

	public static bool IsBlockedUser(long friendCode)
	{
		return m_blockList.Contains(friendCode);
	}

	public static bool RefreshFriendData()
	{
		if (NKCContentManager.CheckContentStatus(ContentsType.FRIENDS, out var _) != NKCContentManager.eContentStatus.Open)
		{
			return false;
		}
		bool result = false;
		if (m_bFriendChanged)
		{
			NKCPacketSender.Send_NKMPacket_FRIEND_LIST_REQ(NKM_FRIEND_LIST_TYPE.FRIEND);
			result = true;
		}
		if (m_bReceivedREQChanged)
		{
			NKCPacketSender.Send_NKMPacket_FRIEND_LIST_REQ(NKM_FRIEND_LIST_TYPE.RECEIVE_REQUEST);
			result = true;
		}
		if (m_bBlockChanged)
		{
			NKCPacketSender.Send_NKMPacket_FRIEND_LIST_REQ(NKM_FRIEND_LIST_TYPE.BLOCKER);
			result = true;
		}
		return result;
	}
}
