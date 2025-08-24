namespace NKM;

public sealed class NKMEquipmentSet
{
	public readonly NKMEquipItemData Weapon;

	public readonly NKMEquipItemData Defence;

	public readonly NKMEquipItemData Accessory;

	public readonly NKMEquipItemData Accessory2;

	public long WeaponUid
	{
		get
		{
			if (Weapon == null)
			{
				return 0L;
			}
			return Weapon.m_ItemUid;
		}
	}

	public long DefenceUid
	{
		get
		{
			if (Defence == null)
			{
				return 0L;
			}
			return Defence.m_ItemUid;
		}
	}

	public long AccessoryUid
	{
		get
		{
			if (Accessory == null)
			{
				return 0L;
			}
			return Accessory.m_ItemUid;
		}
	}

	public long Accessory2Uid
	{
		get
		{
			if (Accessory2 == null)
			{
				return 0L;
			}
			return Accessory2.m_ItemUid;
		}
	}

	public NKMEquipmentSet(NKMEquipItemData weapon, NKMEquipItemData defence, NKMEquipItemData accessory, NKMEquipItemData accessory2)
	{
		Weapon = weapon;
		Defence = defence;
		Accessory = accessory;
		Accessory2 = accessory2;
	}
}
