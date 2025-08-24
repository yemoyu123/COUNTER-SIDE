using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Office;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;

namespace NKC.Office;

public class NKCOfficeData
{
	public delegate void OnInteriorInventoryUpdate(NKMInteriorData interiorData, bool bAdded);

	private List<int> m_lstOpenedSectionIds;

	private List<NKMOfficeRoom> m_lstRooms;

	private Dictionary<int, NKMInteriorData> m_dicInteriors = new Dictionary<int, NKMInteriorData>();

	private Dictionary<int, NKMOfficeRoom> m_dicRooms = new Dictionary<int, NKMOfficeRoom>();

	private List<NKMOfficePost> m_lstBizCard = new List<NKMOfficePost>();

	private List<NKMUserProfileData> m_lstRandomVisitor;

	private int m_randomVisitorIndex;

	private List<NKMOfficePreset> m_lstOfficePreset;

	private const float REFRESH_INTERVAL = 5f;

	private DateTime m_dtNextRefreshTime;

	private Dictionary<long, NKMOfficeState> m_dicFriendOfficeState = new Dictionary<long, NKMOfficeState>();

	private long m_lCurrentFriendUId;

	private NKMOfficeState m_currentFriendState;

	public IEnumerable<NKMOfficeRoom> Rooms => m_lstRooms;

	public NKMOfficePostState PostState { get; private set; }

	public int BizcardCount
	{
		get
		{
			if (m_lstBizCard == null)
			{
				return 0;
			}
			return m_lstBizCard.Count;
		}
	}

	public int RecvCountLeft
	{
		get
		{
			if (NKCSynchronizedTime.IsFinished(NKCSynchronizedTime.ToUtcTime(PostState.nextResetDate)))
			{
				return NKMCommonConst.Office.NameCard.DailyLimit;
			}
			return NKMCommonConst.Office.NameCard.DailyLimit - PostState.recvCount;
		}
	}

	public bool CanReceiveBizcard
	{
		get
		{
			if (NKCSynchronizedTime.IsFinished(NKCSynchronizedTime.ToUtcTime(PostState.nextResetDate)))
			{
				return true;
			}
			return NKMCommonConst.Office.NameCard.DailyLimit > PostState.recvCount;
		}
	}

	public bool CanSendBizcardBroadcast
	{
		get
		{
			if (NKCSynchronizedTime.IsFinished(NKCSynchronizedTime.ToUtcTime(PostState.nextResetDate)))
			{
				return true;
			}
			return !PostState.broadcastExecution;
		}
	}

	public bool IsVisiting => m_currentFriendState != null;

	public long CurrentFriendUid => m_lCurrentFriendUId;

	public event OnInteriorInventoryUpdate dOnInteriorInventoryUpdate;

	public bool CanRefreshOfficePost()
	{
		return NKCSynchronizedTime.IsFinished(m_dtNextRefreshTime);
	}

	public void TryRefreshOfficePost(bool bForce)
	{
		if ((bForce || CanRefreshOfficePost()) && NKCContentManager.IsContentsUnlocked(ContentsType.OFFICE))
		{
			m_dtNextRefreshTime = NKCSynchronizedTime.GetServerUTCTime(5.0);
			NKCPacketSender.Send_NKMPacket_OFFICE_POST_LIST_REQ(0L);
		}
	}

	public NKMOfficePost GetBizCard(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index >= BizcardCount)
		{
			return null;
		}
		return m_lstBizCard[index];
	}

	public void SetData(NKMMyOfficeState officeState)
	{
		if (officeState == null)
		{
			m_lstOpenedSectionIds = new List<int> { 101 };
			return;
		}
		m_lstOpenedSectionIds = officeState.openedSectionIds;
		m_lstRooms = officeState.rooms;
		m_dicInteriors.Clear();
		UpdateInteriorData(officeState.interiors);
		PostState = officeState.postState;
		m_lstOfficePreset = officeState.presets;
		m_dicRooms.Clear();
		if (m_lstRooms != null)
		{
			m_lstRooms.ForEach(delegate(NKMOfficeRoom e)
			{
				m_dicRooms.Add(e.id, e);
			});
		}
	}

	public void SetFriendData(long userUId, NKMOfficeState officeState)
	{
		if (!m_dicFriendOfficeState.ContainsKey(userUId))
		{
			m_dicFriendOfficeState.Add(userUId, officeState);
		}
		else
		{
			m_dicFriendOfficeState[userUId] = officeState;
		}
		m_lCurrentFriendUId = userUId;
		m_currentFriendState = officeState;
	}

	public NKMOfficeRoom GetFriendRoom(long userUID, int roomID)
	{
		if (m_dicFriendOfficeState.TryGetValue(userUID, out var value))
		{
			return value.rooms.Find((NKMOfficeRoom x) => x.id == roomID);
		}
		return null;
	}

	public void UpdateRoomData(NKMOfficeRoom officeRoom)
	{
		if (officeRoom != null)
		{
			if (m_lstRooms == null)
			{
				m_lstRooms = new List<NKMOfficeRoom>();
			}
			int num = m_lstRooms.FindIndex((NKMOfficeRoom e) => e.id == officeRoom.id);
			if (num >= 0 && num < m_lstRooms.Count)
			{
				m_lstRooms[num] = officeRoom;
			}
			else
			{
				m_lstRooms.Add(officeRoom);
			}
			if (m_dicRooms.ContainsKey(officeRoom.id))
			{
				m_dicRooms[officeRoom.id] = officeRoom;
			}
			else
			{
				m_dicRooms.Add(officeRoom.id, officeRoom);
			}
		}
	}

	public void UpdateSectionData(int sectionId, List<NKMOfficeRoom> officeRooms)
	{
		if (officeRooms == null)
		{
			return;
		}
		if (m_lstOpenedSectionIds == null)
		{
			m_lstOpenedSectionIds = new List<int>();
		}
		if (!m_lstOpenedSectionIds.Contains(sectionId))
		{
			m_lstOpenedSectionIds.Add(sectionId);
		}
		if (m_lstRooms == null)
		{
			m_lstRooms = new List<NKMOfficeRoom>();
		}
		int count = officeRooms.Count;
		int i = 0;
		while (i < count)
		{
			int num = m_lstRooms.FindIndex((NKMOfficeRoom e) => e.id == officeRooms[i].id);
			if (num >= 0 && num < m_lstRooms.Count)
			{
				m_lstRooms[num] = officeRooms[i];
			}
			else
			{
				m_lstRooms.Add(officeRooms[i]);
			}
			if (m_dicRooms.ContainsKey(officeRooms[i].id))
			{
				m_dicRooms[officeRooms[i].id] = officeRooms[i];
			}
			else
			{
				m_dicRooms.Add(officeRooms[i].id, officeRooms[i]);
			}
			int num2 = i + 1;
			i = num2;
		}
	}

	public NKMOfficeRoom GetOfficeRoom(int roomId)
	{
		if (IsVisiting)
		{
			return m_currentFriendState.rooms.Find((NKMOfficeRoom e) => e.id == roomId);
		}
		NKMOfficeRoom value = null;
		m_dicRooms.TryGetValue(roomId, out value);
		return value;
	}

	public int GetOpenedDormsCount()
	{
		if (IsVisiting)
		{
			return GetOpenedDormCount(m_currentFriendState.rooms);
		}
		return GetOpenedDormCount(m_lstRooms);
	}

	public int GetOpenedRoomCountInSection(int sectionId, ref int roomSequenceNumber, int roomId = 0)
	{
		int num = 0;
		if (m_lstRooms != null)
		{
			int count = m_lstRooms.Count;
			for (int i = 0; i < count; i++)
			{
				NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(m_lstRooms[i].id);
				if (nKMOfficeRoomTemplet != null)
				{
					if (nKMOfficeRoomTemplet.Type == NKMOfficeRoomTemplet.RoomType.Dorm)
					{
						num++;
					}
					if (nKMOfficeRoomTemplet.ID == roomId)
					{
						roomSequenceNumber = num;
					}
				}
			}
		}
		return num;
	}

	public bool IsOpenedSection(int sectionId)
	{
		if (IsVisiting)
		{
			return m_dicFriendOfficeState[m_lCurrentFriendUId].openedSectionIds.Contains(sectionId);
		}
		if (m_lstOpenedSectionIds == null)
		{
			return false;
		}
		return m_lstOpenedSectionIds.Contains(sectionId);
	}

	public bool IsOpenedRoom(int roomId)
	{
		if (IsVisiting)
		{
			return m_dicFriendOfficeState[m_lCurrentFriendUId].rooms.Find((NKMOfficeRoom e) => e.id == roomId) != null;
		}
		return m_dicRooms.ContainsKey(roomId);
	}

	public int GetUnitAssignedNumber(long unitUid, int roomId)
	{
		if (!m_dicRooms.ContainsKey(roomId))
		{
			return 0;
		}
		if (m_dicRooms[roomId].unitUids == null)
		{
			return 0;
		}
		return m_dicRooms[roomId].unitUids.FindIndex((long e) => e == unitUid) + 1;
	}

	public void UpdateInteriorData(IEnumerable<NKMInteriorData> lstData)
	{
		foreach (NKMInteriorData lstDatum in lstData)
		{
			UpdateInteriorData(lstDatum);
		}
	}

	public void AddInteriorData(IEnumerable<NKMInteriorData> lstData)
	{
		foreach (NKMInteriorData lstDatum in lstData)
		{
			AddInteriorData(lstDatum);
		}
	}

	public void AddInteriorData(NKMInteriorData data)
	{
		if (m_dicInteriors.TryGetValue(data.itemId, out var value))
		{
			value.count += data.count;
			this.dOnInteriorInventoryUpdate?.Invoke(data, bAdded: false);
		}
		else
		{
			this.dOnInteriorInventoryUpdate?.Invoke(data, bAdded: true);
			m_dicInteriors[data.itemId] = data;
		}
	}

	public void UpdateInteriorData(NKMInteriorData data)
	{
		bool bAdded = !m_dicInteriors.ContainsKey(data.itemId);
		m_dicInteriors[data.itemId] = data;
		this.dOnInteriorInventoryUpdate?.Invoke(data, bAdded);
	}

	public long GetInteriorCount(NKMOfficeInteriorTemplet templet)
	{
		if (m_dicInteriors.TryGetValue(templet.m_ItemMiscID, out var value))
		{
			return value.count;
		}
		return 0L;
	}

	public long GetInteriorCount(int itemID)
	{
		long freeInteriorCount = GetFreeInteriorCount(itemID);
		int num = 0;
		foreach (KeyValuePair<int, NKMOfficeRoom> dicRoom in m_dicRooms)
		{
			List<NKMOfficeFurniture> list = dicRoom.Value.furnitures.FindAll((NKMOfficeFurniture x) => x.itemId == itemID);
			num += list.Count;
		}
		return freeInteriorCount + num;
	}

	public IEnumerable<NKMInteriorData> GetAllInteriorData()
	{
		foreach (KeyValuePair<int, NKMInteriorData> dicInterior in m_dicInteriors)
		{
			yield return dicInterior.Value;
		}
	}

	public long GetFreeInteriorCount(int itemID)
	{
		if (m_dicInteriors.TryGetValue(itemID, out var value))
		{
			return value.count;
		}
		return 0L;
	}

	public int GetRoomInteriorScore(int roomID)
	{
		return GetOfficeRoom(roomID)?.interiorScore ?? 0;
	}

	public NKMOfficeUnitData GetFriendUnit(long userUID, long unitUId)
	{
		if (m_dicFriendOfficeState.TryGetValue(userUID, out var value))
		{
			return value.units.Find((NKMOfficeUnitData e) => e.unitUid == unitUId);
		}
		return null;
	}

	public int GetFriendUnitId(long unitUId)
	{
		return m_currentFriendState.units.Find((NKMOfficeUnitData e) => e.unitUid == unitUId)?.unitId ?? 0;
	}

	public NKMCommonProfile GetFriendProfile()
	{
		return m_currentFriendState?.commonProfile;
	}

	public void ResetFriendUId()
	{
		m_dicFriendOfficeState.Clear();
		m_lCurrentFriendUId = 0L;
		m_currentFriendState = null;
	}

	private int GetOpenedDormCount(List<NKMOfficeRoom> roomList)
	{
		if (roomList == null)
		{
			return 0;
		}
		int num = 0;
		int count = roomList.Count;
		for (int i = 0; i < count; i++)
		{
			NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(roomList[i].id);
			if (nKMOfficeRoomTemplet != null && nKMOfficeRoomTemplet.Type == NKMOfficeRoomTemplet.RoomType.Dorm)
			{
				num++;
			}
		}
		return num;
	}

	public void UpdatePostState(NKMOfficePostState postState)
	{
		PostState = postState;
	}

	public void UpdatePostList(List<NKMOfficePost> lstPost)
	{
		m_lstBizCard = lstPost;
	}

	public void UpdateRandomVisitor(List<NKMUserProfileData> lstVisitor)
	{
		m_lstRandomVisitor = lstVisitor;
		m_randomVisitorIndex = 0;
	}

	public NKMUserProfileData GetRandomVisitor()
	{
		if (m_lstRandomVisitor == null || m_lstRandomVisitor.Count == 0)
		{
			return null;
		}
		NKMUserProfileData result = m_lstRandomVisitor[m_randomVisitorIndex % m_lstRandomVisitor.Count];
		m_randomVisitorIndex++;
		return result;
	}

	public List<NKMUserProfileData> GetRandomVisitor(int count)
	{
		if (m_lstRandomVisitor.Count < count)
		{
			return m_lstRandomVisitor;
		}
		List<NKMUserProfileData> list = new List<NKMUserProfileData>();
		while (list.Count < count)
		{
			list.Add(m_lstRandomVisitor[m_randomVisitorIndex % m_lstRandomVisitor.Count]);
			m_randomVisitorIndex++;
		}
		return list;
	}

	public List<int> GetOpenedRoomIdList()
	{
		List<int> roomIdList = new List<int>();
		if (IsVisiting)
		{
			m_currentFriendState?.rooms.ForEach(delegate(NKMOfficeRoom e)
			{
				roomIdList.Add(e.id);
			});
		}
		else
		{
			m_lstRooms?.ForEach(delegate(NKMOfficeRoom e)
			{
				roomIdList.Add(e.id);
			});
		}
		roomIdList.Sort();
		return roomIdList;
	}

	public int GetPresetCount()
	{
		if (m_lstOfficePreset != null)
		{
			return m_lstOfficePreset.Count;
		}
		return 0;
	}

	public void SetPresetCount(int newCount)
	{
		while (m_lstOfficePreset.Count < newCount)
		{
			NKMOfficePreset nKMOfficePreset = new NKMOfficePreset();
			nKMOfficePreset.presetId = m_lstOfficePreset.Count;
			m_lstOfficePreset.Add(nKMOfficePreset);
		}
	}

	public void SetPreset(NKMOfficePreset preset)
	{
		if (preset != null && preset.presetId < m_lstOfficePreset.Count)
		{
			m_lstOfficePreset[preset.presetId] = preset;
		}
	}

	public NKMOfficePreset GetPreset(int index)
	{
		if (index < m_lstOfficePreset.Count)
		{
			return m_lstOfficePreset[index];
		}
		return null;
	}

	public void ChangePresetName(int index, string name)
	{
		if (index < m_lstOfficePreset.Count)
		{
			m_lstOfficePreset[index].name = name;
		}
	}
}
