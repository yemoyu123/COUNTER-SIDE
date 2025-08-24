using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class GameUnitData : ISerializable
{
	public NKMUnitData unit;

	public List<NKMEquipItemData> equip_item_list = new List<NKMEquipItemData>();

	public GameUnitData()
	{
	}

	public GameUnitData(NKMUnitData unitData, NKMInventoryData inventoryData)
	{
		NKMUnitData copied = new NKMUnitData();
		copied.DeepCopyFrom(unitData);
		unit = copied;
		foreach (long validEquipUid in unit.GetValidEquipUids())
		{
			NKMEquipItemData itemEquip = inventoryData.GetItemEquip(validEquipUid);
			if (itemEquip != null)
			{
				NKMEquipItemData nKMEquipItemData = new NKMEquipItemData();
				nKMEquipItemData.DeepCopyFrom(itemEquip);
				equip_item_list.Add(nKMEquipItemData);
			}
		}
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unit);
		stream.PutOrGet(ref equip_item_list);
	}
}
