using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClientPacket.Common;
using Cs.Logging;
using Cs.Protocol;
using NKC;
using NKC.Publisher;

namespace NKM;

[DataContract]
public class NKMInventoryData : Cs.Protocol.ISerializable
{
	public delegate void OnMiscInventoryUpdate(NKMItemMiscData itemData);

	public delegate void OnEquipUpdate(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipData);

	public delegate void OnRefreshDailyContents();

	public int m_MaxItemEqipCount = 100;

	[DataMember]
	private Dictionary<int, NKMItemMiscData> m_ItemMiscData = new Dictionary<int, NKMItemMiscData>();

	[DataMember]
	private Dictionary<long, NKMEquipItemData> m_ItemEquipData = new Dictionary<long, NKMEquipItemData>();

	[DataMember]
	private HashSet<int> m_ItemSkinData = new HashSet<int>();

	[DataMember]
	private Dictionary<int, NKMMiscCollectionData> m_dicMiscCollectionData = new Dictionary<int, NKMMiscCollectionData>();

	private Dictionary<int, long> m_dicItemUpdateCheck = new Dictionary<int, long>();

	public IReadOnlyDictionary<int, NKMItemMiscData> MiscItems => m_ItemMiscData;

	public IReadOnlyDictionary<long, NKMEquipItemData> EquipItems => m_ItemEquipData;

	public IEnumerable<int> SkinIds => m_ItemSkinData;

	public IReadOnlyDictionary<long, NKMEquipItemData> ItemEquipData => m_ItemEquipData;

	public event OnMiscInventoryUpdate dOnMiscInventoryUpdate;

	public event OnEquipUpdate dOnEquipUpdate;

	public event OnRefreshDailyContents dOnRefreshDailyContents;

	public List<NKMItemMiscData> GetEmblemData()
	{
		List<NKMItemMiscData> list = new List<NKMItemMiscData>();
		foreach (KeyValuePair<int, NKMItemMiscData> itemMiscDatum in m_ItemMiscData)
		{
			NKMItemMiscData value = itemMiscDatum.Value;
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(value.ItemID);
			if (itemMiscTempletByID != null && itemMiscTempletByID.IsEmblem())
			{
				list.Add(value);
			}
		}
		return list;
	}

	public void InitItemMisc(NKMItemMiscData MiscItemData)
	{
		NKMItemMiscData value = null;
		if (m_ItemMiscData.TryGetValue(MiscItemData.ItemID, out value))
		{
			value.CountFree += MiscItemData.CountFree;
			value.CountPaid += MiscItemData.CountPaid;
		}
		else
		{
			m_ItemMiscData.Add(MiscItemData.ItemID, MiscItemData);
		}
	}

	public void InitMiscCollectionData(NKMMiscCollectionData data)
	{
		if (NKMItemMiscTemplet.Find(data.MiscId) != null && !m_dicMiscCollectionData.TryGetValue(data.MiscId, out var _))
		{
			m_dicMiscCollectionData.Add(data.MiscId, data);
		}
	}

	public void AddMiscCollectionData(NKMMiscCollectionData data)
	{
		if (!m_dicMiscCollectionData.ContainsKey(data.MiscId))
		{
			m_dicMiscCollectionData.Add(data.MiscId, data);
		}
	}

	public NKMMiscCollectionData GetMiscCollectionData(int miscId)
	{
		m_dicMiscCollectionData.TryGetValue(miscId, out var value);
		return value;
	}

	public void InitItemEquip(NKMEquipItemData equip_item_data)
	{
		if (m_ItemEquipData.ContainsKey(equip_item_data.m_ItemUid))
		{
			Log.Error("AddEquipItem, duplicated equipItemUID : " + equip_item_data.m_ItemUid, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUserData.cs", 296);
		}
		else
		{
			m_ItemEquipData.Add(equip_item_data.m_ItemUid, equip_item_data);
		}
	}

	public void InitItemSkin(int skinID)
	{
		m_ItemSkinData.Add(skinID);
	}

	public void AddItemMisc(NKMItemMiscData MiscItemData)
	{
		if (MiscItemData != null)
		{
			AddItemMisc(MiscItemData.ItemID, MiscItemData.CountFree, MiscItemData.CountPaid, MiscItemData.RegDate);
		}
	}

	public void AddItemMisc(List<NKMItemMiscData> lstMiscItem)
	{
		foreach (NKMItemMiscData item in lstMiscItem)
		{
			AddItemMisc(item);
		}
	}

	public void AddItemEquip(IEnumerable<NKMEquipItemData> lstEquipItem)
	{
		foreach (NKMEquipItemData item in lstEquipItem)
		{
			if (item != null)
			{
				AddItemEquip(item);
			}
		}
	}

	public void AddItemSkin(IEnumerable<int> lstSkinID)
	{
		foreach (int item in lstSkinID)
		{
			AddItemSkin(item);
		}
	}

	public void AddItemSkin(int skinID)
	{
		if (!m_ItemSkinData.Contains(skinID))
		{
			m_ItemSkinData.Add(skinID);
		}
	}

	public void RemoveItemEquip(IEnumerable<long> lstUID)
	{
		if (lstUID == null)
		{
			return;
		}
		foreach (long item in lstUID)
		{
			RemoveItemEquip(item);
		}
	}

	public void UpdateItemInfo(List<NKMItemMiscData> lstMiscItem)
	{
		if (lstMiscItem == null)
		{
			return;
		}
		foreach (NKMItemMiscData item in lstMiscItem)
		{
			UpdateItemInfo(item);
		}
	}

	public void UpdateItemInfo(NKMItemMiscData itemData)
	{
		if (itemData != null)
		{
			UpdateItemInfo(itemData.ItemID, itemData.CountFree, itemData.CountPaid, itemData.RegDate);
		}
	}

	public NKMItemMiscData GetItemMisc(int itemID)
	{
		m_ItemMiscData.TryGetValue(itemID, out var value);
		return value;
	}

	public NKMItemMiscData GetItemMisc(NKMItemMiscTemplet templet)
	{
		m_ItemMiscData.TryGetValue(templet.m_ItemMiscID, out var value);
		return value;
	}

	public bool GetItemMisc(int itemId, out NKMItemMiscData result)
	{
		return m_ItemMiscData.TryGetValue(itemId, out result);
	}

	public NKMEquipItemData GetItemEquip(long itemUid)
	{
		NKMEquipItemData value = null;
		m_ItemEquipData.TryGetValue(itemUid, out value);
		return value;
	}

	public bool HasItemSkin(int skinID)
	{
		if (skinID == 0)
		{
			return true;
		}
		return m_ItemSkinData.Contains(skinID);
	}

	public long GetCountMiscItem(int itemID)
	{
		if (m_ItemMiscData.TryGetValue(itemID, out var value))
		{
			return value.TotalCount;
		}
		return 0L;
	}

	public long GetCountMiscItem(int itemID, bool isPaid)
	{
		if (m_ItemMiscData.TryGetValue(itemID, out var value))
		{
			if (!isPaid)
			{
				return value.CountFree;
			}
			return value.CountPaid;
		}
		return 0L;
	}

	public long GetCountMiscItem(NKMItemMiscTemplet templet)
	{
		if (m_ItemMiscData.TryGetValue(templet.m_ItemMiscID, out var value))
		{
			return value.TotalCount;
		}
		return 0L;
	}

	public long GetCountDailyTrainingTicket()
	{
		return GetCountMiscItem(4);
	}

	public long GetCountDailyTrainingTicket_A()
	{
		return GetCountMiscItem(15);
	}

	public long GetCountDailyTrainingTicket_B()
	{
		return GetCountMiscItem(16);
	}

	public long GetCountDailyTrainingTicket_C()
	{
		return GetCountMiscItem(17);
	}

	public int GetCountMiscItemTypes()
	{
		return m_ItemMiscData.Count;
	}

	public int GetCountMiscExceptCurrency()
	{
		int num = 0;
		foreach (KeyValuePair<int, NKMItemMiscData> itemMiscDatum in m_ItemMiscData)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemMiscDatum.Value.ItemID);
			if (itemMiscTempletByID == null)
			{
				Log.Error($"GetItemMiscTempletByID null itemID: {itemMiscDatum.Value.ItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUserData.cs", 473);
			}
			else if (!itemMiscTempletByID.IsHideInInven() && itemMiscDatum.Value.TotalCount > 0)
			{
				num++;
			}
		}
		return num;
	}

	public int GetCountEquipItemTypes()
	{
		return m_ItemEquipData.Count;
	}

	public int GetCountUnEquipedItem()
	{
		int num = 0;
		foreach (NKMEquipItemData value in m_ItemEquipData.Values)
		{
			if (value.m_OwnerUnitUID == -1)
			{
				num++;
			}
		}
		return num;
	}

	public int GetCountSkinItemTypes()
	{
		return m_ItemSkinData.Count;
	}

	public bool CanGetMoreEquipItem(int addCount)
	{
		return GetCountEquipItemTypes() + addCount <= m_MaxItemEqipCount;
	}

	public void GetUsableMiscItemCount(int itemID, long itemCount, out long countFree, out long countPaid)
	{
		countFree = 0L;
		countPaid = 0L;
		NKMItemMiscData itemMisc = GetItemMisc(itemID);
		if (itemMisc != null)
		{
			long num = itemCount;
			countFree = Math.Min(num, itemMisc.CountFree);
			num -= countFree;
			if (num != 0L)
			{
				countPaid = Math.Min(num, itemMisc.CountPaid);
			}
		}
	}

	public int GetEquipCountByEnchantLevel(int enchantLevel)
	{
		return m_ItemEquipData.Values.Where((NKMEquipItemData e) => e.m_EnchantLevel >= enchantLevel).Count();
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_MaxItemEqipCount);
		stream.PutOrGet(ref m_ItemMiscData);
		stream.PutOrGet(ref m_ItemEquipData);
		stream.PutOrGet(ref m_ItemSkinData);
		stream.PutOrGet(ref m_dicMiscCollectionData);
	}

	public void AddItemMisc(int itemID, long countFree, long countPaid, DateTime regDate)
	{
		long num = countFree;
		long num2 = countPaid;
		NKMItemMiscData value = null;
		if (m_ItemMiscData.TryGetValue(itemID, out value))
		{
			num += value.CountFree;
			num2 += value.CountPaid;
		}
		UpdateItemInfo(itemID, num, num2, regDate);
	}

	public void AddItemEquip(NKMEquipItemData equip_item_data)
	{
		if (m_ItemEquipData.ContainsKey(equip_item_data.m_ItemUid))
		{
			Log.Error("AddEquipItem, duplicated equipItemUID : " + equip_item_data.m_ItemUid, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMUserDataEx.cs", 56);
			return;
		}
		m_ItemEquipData.Add(equip_item_data.m_ItemUid, equip_item_data);
		this.dOnEquipUpdate?.Invoke(NKMUserData.eChangeNotifyType.Add, equip_item_data.m_ItemUid, equip_item_data);
	}

	public void UpdateItemEquip(NKMEquipItemData equip_item_data)
	{
		if (equip_item_data != null)
		{
			if (!m_ItemEquipData.ContainsKey(equip_item_data.m_ItemUid))
			{
				Log.Error("Tried to update nonexist item : " + equip_item_data.m_ItemUid, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMUserDataEx.cs", 72);
				return;
			}
			m_ItemEquipData[equip_item_data.m_ItemUid] = equip_item_data;
			this.dOnEquipUpdate?.Invoke(NKMUserData.eChangeNotifyType.Update, equip_item_data.m_ItemUid, equip_item_data);
		}
	}

	public void RemoveItemMisc(int itemID, long count)
	{
		NKMItemMiscData value = null;
		if (m_ItemMiscData.TryGetValue(itemID, out value) && value.TotalCount >= count)
		{
			GetUsableMiscItemCount(itemID, count, out var countFree, out var countPaid);
			UpdateItemInfo(itemID, value.CountFree - countFree, value.CountPaid - countPaid);
		}
	}

	public void RemoveItemEquip(long uid)
	{
		if (m_ItemEquipData.ContainsKey(uid))
		{
			m_ItemEquipData.Remove(uid);
			if (NKCScenManager.CurrentUserData().hasReservedHiddenOptionRerollData() && NKCScenManager.CurrentUserData().m_PotentialOptionCandidate.equipUid == uid)
			{
				NKCScenManager.CurrentUserData().SetEquipPotentialData(new NKMPotentialOptionChangeCandidate());
			}
			this.dOnEquipUpdate?.Invoke(NKMUserData.eChangeNotifyType.Remove, uid, null);
		}
	}

	public void UpdateItemInfo(int itemID, long countFree, long countPaid, DateTime regDate)
	{
		UpdateItemInfo(itemID, countFree, countPaid);
		if (m_ItemMiscData.TryGetValue(itemID, out var value))
		{
			value.RegDate = regDate;
		}
	}

	public void UpdateItemInfo(int itemID, long countFree, long countPaid)
	{
		NKMItemMiscData value = null;
		if (m_ItemMiscData.TryGetValue(itemID, out value))
		{
			value.CountFree = countFree;
			value.CountPaid = countPaid;
		}
		else
		{
			value = new NKMItemMiscData(itemID, countFree, countPaid);
			m_ItemMiscData.Add(itemID, value);
		}
		if (itemID == 2)
		{
			NKCPublisherModule.Push.UpdateLocalPush(NKC_GAME_OPTION_ALARM_GROUP.RESOURCE_SUPPLY_COMPLETE, bForce: true);
		}
		this.dOnMiscInventoryUpdate?.Invoke(value);
	}

	public void UpdateItemInfo(int itemID, long count, NKM_ITEM_PAYMENT_TYPE payment_type)
	{
		NKMItemMiscData value = null;
		if (m_ItemMiscData.TryGetValue(itemID, out value))
		{
			if (payment_type == NKM_ITEM_PAYMENT_TYPE.NIPT_FREE)
			{
				value.CountFree = count;
			}
			else
			{
				value.CountPaid = count;
			}
		}
		else
		{
			long countFree = 0L;
			long countPaid = 0L;
			if (payment_type == NKM_ITEM_PAYMENT_TYPE.NIPT_FREE)
			{
				countFree = count;
			}
			else
			{
				countPaid = count;
			}
			value = new NKMItemMiscData(itemID, countFree, countPaid);
			m_ItemMiscData.Add(itemID, value);
		}
		this.dOnMiscInventoryUpdate?.Invoke(value);
	}

	public bool IsFirstUpdate(int itemID, long itemCnt)
	{
		if (m_dicItemUpdateCheck.ContainsKey(itemID))
		{
			if (m_dicItemUpdateCheck[itemID] == itemCnt)
			{
				return false;
			}
			return true;
		}
		m_dicItemUpdateCheck.Add(itemID, itemCnt);
		return false;
	}

	public long GetPreviousItemCount(int itemID, long itemCnt)
	{
		long result = 0L;
		if (m_dicItemUpdateCheck.ContainsKey(itemID))
		{
			result = m_dicItemUpdateCheck[itemID];
			m_dicItemUpdateCheck[itemID] = itemCnt;
		}
		return result;
	}

	public int GetSameKindEquipCount(int equipID, int setOptionID = 0)
	{
		int num = 0;
		foreach (NKMEquipItemData value in m_ItemEquipData.Values)
		{
			if (value.m_ItemEquipID == equipID && (setOptionID == 0 || value.m_SetOptionId == setOptionID))
			{
				num++;
			}
		}
		return num;
	}

	public void RefreshDailyContens()
	{
		this.dOnRefreshDailyContents?.Invoke();
	}
}
