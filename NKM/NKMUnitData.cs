using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Protocol;
using NKM.Templet;
using NKM.Templet.Office;
using NKM.Unit;

namespace NKM;

[DataContract]
public sealed class NKMUnitData : Cs.Protocol.ISerializable
{
	private bool isPermanentContract;

	private bool isSeized;

	private bool fromContract;

	private int officeRoomId;

	private OfficeGrade officeGrade;

	private DateTime officeGaugeStartTime;

	public long m_UnitUID;

	public long m_UserUID;

	[DataMember]
	public int m_UnitID;

	public int m_UnitLevel;

	public int m_iUnitLevelEXP;

	public int m_SkinID;

	public float m_fInjury;

	public int[] m_aUnitSkillLevel = new int[5];

	public short m_LimitBreakLevel;

	public bool m_bLock;

	public List<int> m_listStatEXP = new List<int>();

	public List<short> m_listGameUnitUID = new List<short>();

	public List<short> m_listGameUnitUIDChange = new List<short>();

	public List<float> m_listNearTargetRange = new List<float>();

	public long m_DungeonRespawnUnitTempletUID;

	public NKMDungeonRespawnUnitTemplet m_DungeonRespawnUnitTemplet;

	public float m_fLastRespawnTime = -1f;

	public float m_fLastDieTime = -1f;

	public DateTime m_regDate = DateTime.MinValue;

	public bool m_bSummonUnit;

	public int loyalty;

	public bool isFavorite;

	public int unitPower;

	public int tacticLevel;

	public int reactorLevel;

	private long[] m_EquipItemList = new long[4];

	public List<NKMShipCmdModule> ShipCommandModule = new List<NKMShipCmdModule>();

	public const int ACC_2_UNLOCK_LEVEL = 3;

	public bool IsPermanentContract => isPermanentContract;

	public IReadOnlyList<long> EquipItemUids => m_EquipItemList;

	public bool IsSeized => isSeized;

	public bool FromContract => fromContract;

	public int OfficeRoomId => officeRoomId;

	public OfficeGrade OfficeGrade => officeGrade;

	public DateTime OfficeGaugeStartTime => officeGaugeStartTime;

	public bool IsActiveUnit
	{
		get
		{
			if (!m_bLock && m_UnitLevel <= 1 && m_SkinID == 0 && tacticLevel <= 0 && m_LimitBreakLevel <= 0 && !isPermanentContract && !m_aUnitSkillLevel.Any((int x) => x > 1))
			{
				return EquipItemUids.Any((long x) => x != 0);
			}
			return true;
		}
	}

	public NKMUnitData()
	{
		for (int i = 0; i <= 5; i++)
		{
			m_listStatEXP.Add(0);
		}
		Init();
	}

	public NKMUnitData(long userUid, int unitId, long unitUid, short limitBreakLv, int loyalty, bool isPermanentContract, bool isLock)
	{
		for (int i = 0; i <= 5; i++)
		{
			m_listStatEXP.Add(0);
		}
		Init();
		m_UserUID = userUid;
		m_UnitID = unitId;
		m_UnitUID = unitUid;
		m_LimitBreakLevel = limitBreakLv;
		this.loyalty = loyalty;
		this.isPermanentContract = isPermanentContract;
		m_bLock = isLock;
		FillSkillLevelByUnitID(m_UnitID);
	}

	public NKMUnitData(int unit_id, long unit_uid, bool islock, bool isPermanentContract, bool isSeized, bool fromContract)
	{
		for (int i = 0; i <= 5; i++)
		{
			m_listStatEXP.Add(0);
		}
		Init();
		m_UnitID = unit_id;
		m_UnitUID = unit_uid;
		m_bLock = islock;
		this.isPermanentContract = isPermanentContract;
		this.isSeized = isSeized;
		this.fromContract = fromContract;
		FillSkillLevelByUnitID(m_UnitID);
	}

	public NKMUnitData(int unit_id, long unit_uid, bool islock, bool isPermanentContract, bool isSeized, bool fromContract, bool isFavorite)
	{
		for (int i = 0; i <= 5; i++)
		{
			m_listStatEXP.Add(0);
		}
		Init();
		m_UnitID = unit_id;
		m_UnitUID = unit_uid;
		m_bLock = islock;
		this.isPermanentContract = isPermanentContract;
		this.isSeized = isSeized;
		this.fromContract = fromContract;
		this.isFavorite = isFavorite;
		FillSkillLevelByUnitID(m_UnitID);
	}

	public NKMUnitData(NKMDummyUnitData dummyUnit)
	{
		for (int i = 0; i <= 5; i++)
		{
			m_listStatEXP.Add(0);
		}
		Init();
		m_UnitID = dummyUnit.UnitId;
		m_UnitLevel = dummyUnit.UnitLevel;
		m_LimitBreakLevel = dummyUnit.LimitBreakLevel;
		m_SkinID = dummyUnit.SkinId;
		tacticLevel = dummyUnit.TacticLevel;
	}

	public void Init()
	{
		m_UnitUID = 0L;
		m_UserUID = 0L;
		m_UnitID = 0;
		m_UnitLevel = 1;
		m_iUnitLevelEXP = 0;
		m_fInjury = 0f;
		m_LimitBreakLevel = 0;
		m_bLock = false;
		for (int i = 0; i <= 5; i++)
		{
			m_listStatEXP[i] = 0;
		}
		for (int j = 0; j < 4; j++)
		{
			m_EquipItemList[j] = 0L;
		}
		m_listGameUnitUID.Clear();
		m_listGameUnitUIDChange.Clear();
		m_listNearTargetRange.Clear();
		m_DungeonRespawnUnitTempletUID = 0L;
		m_DungeonRespawnUnitTemplet = null;
		m_fLastRespawnTime = -1f;
		m_fLastDieTime = -1f;
		m_bSummonUnit = false;
		for (int k = 0; k < 5; k++)
		{
			m_aUnitSkillLevel[k] = 0;
		}
		m_regDate = ServiceTime.UtcNow;
		ShipCommandModule = new List<NKMShipCmdModule>();
	}

	public void Rearm(int newUnitId, NKMUnitTempletBase fromUnitTemplet, NKMUnitTempletBase toUnitTemplet)
	{
		m_UnitID = newUnitId;
		m_bLock = true;
		for (int i = fromUnitTemplet.GetSkillCount(); i < toUnitTemplet.GetSkillCount(); i++)
		{
			m_aUnitSkillLevel[i] = 1;
		}
	}

	public void SetDungeonRespawnUnitTemplet(NKMDungeonRespawnUnitTemplet cNKMDungeonRespawnUnitTemplet)
	{
		m_DungeonRespawnUnitTemplet = cNKMDungeonRespawnUnitTemplet;
		if (m_DungeonRespawnUnitTemplet != null)
		{
			m_DungeonRespawnUnitTempletUID = m_DungeonRespawnUnitTemplet.m_TempletUID;
		}
		else
		{
			m_DungeonRespawnUnitTempletUID = 0L;
		}
	}

	public void SetDungeonRespawnUnitTemplet()
	{
		if (m_DungeonRespawnUnitTempletUID != 0L)
		{
			m_DungeonRespawnUnitTemplet = NKMDungeonManager.GetNKMDungeonRespawnUnitTemplet(m_DungeonRespawnUnitTempletUID);
		}
		else
		{
			m_DungeonRespawnUnitTemplet = null;
		}
	}

	public NKMUnitExpTemplet GetExpTemplet()
	{
		return NKMUnitExpTemplet.FindByUnitId(m_UnitID, m_UnitLevel);
	}

	public void FillSkillLevelByUnitID(int unitID)
	{
		if (unitID <= 0)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		if (unitTempletBase == null)
		{
			Log.Error($"Can not found Unit ID. {unitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitData.cs", 265);
		}
		else
		{
			if (unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL)
			{
				return;
			}
			int skillCount = NKMUnitManager.GetUnitTempletBase(unitID).GetSkillCount();
			for (int i = 0; i < 5; i++)
			{
				if (i < skillCount)
				{
					m_aUnitSkillLevel[i] = 1;
				}
				else
				{
					m_aUnitSkillLevel[i] = 0;
				}
			}
		}
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_UnitUID);
		stream.PutOrGet(ref m_UserUID);
		stream.PutOrGet(ref m_UnitID);
		stream.PutOrGet(ref m_UnitLevel);
		stream.PutOrGet(ref m_iUnitLevelEXP);
		stream.PutOrGet(ref m_SkinID);
		stream.PutOrGet(ref m_fInjury);
		stream.PutOrGet(ref m_LimitBreakLevel);
		stream.PutOrGet(ref m_bLock);
		stream.PutOrGet(ref m_bSummonUnit);
		stream.PutOrGet(ref m_listStatEXP);
		stream.PutOrGet(ref m_listGameUnitUID);
		stream.PutOrGet(ref m_listGameUnitUIDChange);
		stream.PutOrGet(ref m_listNearTargetRange);
		stream.PutOrGet(ref m_aUnitSkillLevel);
		stream.PutOrGet(ref m_EquipItemList);
		stream.PutOrGet(ref loyalty);
		stream.PutOrGet(ref isPermanentContract);
		stream.PutOrGet(ref isSeized);
		stream.PutOrGet(ref fromContract);
		stream.PutOrGet(ref officeRoomId);
		stream.PutOrGet(ref m_regDate);
		stream.PutOrGetEnum(ref officeGrade);
		stream.PutOrGet(ref officeGaugeStartTime);
		stream.PutOrGet(ref m_DungeonRespawnUnitTempletUID);
		stream.PutOrGet(ref isFavorite);
		stream.PutOrGet(ref ShipCommandModule);
		stream.PutOrGet(ref tacticLevel);
		stream.PutOrGet(ref reactorLevel);
	}

	public NKMUnitTemplet GetUnitTemplet()
	{
		return NKMUnitManager.GetUnitTemplet(m_UnitID);
	}

	public NKMUnitTempletBase GetUnitTempletBase()
	{
		return NKMUnitManager.GetUnitTempletBase(m_UnitID);
	}

	public int GetShipGroupId()
	{
		return NKMUnitManager.GetUnitTempletBase(m_UnitID)?.m_ShipGroupID ?? (-1);
	}

	public void SetOfficeRoomId(int roomId, OfficeGrade officeGrade, DateTime startTime)
	{
		officeRoomId = roomId;
		this.officeGrade = officeGrade;
		officeGaugeStartTime = startTime;
	}

	public float GetOfficeRoomHeartGauge()
	{
		if (officeRoomId <= 0)
		{
			return 0f;
		}
		NKMOfficeGradeTemplet nKMOfficeGradeTemplet = NKMOfficeGradeTemplet.Find(OfficeGrade);
		return Math.Min(1f, (float)((ServiceTime.Recent - officeGaugeStartTime).TotalSeconds / nKMOfficeGradeTemplet.ChargingTime.TotalSeconds));
	}

	public bool CheckOfficeRoomHeartFull()
	{
		if (officeRoomId <= 0)
		{
			return false;
		}
		if (IsPermanentContract)
		{
			return false;
		}
		if (loyalty >= 10000)
		{
			return false;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
		if (unitTempletBase != null && unitTempletBase.IsTrophy)
		{
			return false;
		}
		NKMOfficeGradeTemplet nKMOfficeGradeTemplet = NKMOfficeGradeTemplet.Find(OfficeGrade);
		return ServiceTime.Recent - officeGaugeStartTime >= nKMOfficeGradeTemplet.ChargingTime;
	}

	public int GetShipSkillIndex(int skillID)
	{
		return NKMUnitManager.GetUnitTempletBase(m_UnitID)?.GetShipSkillIndex(skillID) ?? (-1);
	}

	public int GetShipSkillLevel(int skillID)
	{
		int shipSkillIndex = GetShipSkillIndex(skillID);
		if (shipSkillIndex < 0)
		{
			return 0;
		}
		return GetSkillLevelByIndex(shipSkillIndex);
	}

	public NKMShipSkillTemplet GetShipSkillTempletByIndex(int index)
	{
		return NKMShipSkillManager.GetShipSkillTempletByStrID(NKMUnitManager.GetUnitTempletBase(m_UnitID).GetSkillStrID(index));
	}

	public int GetUnitSkillIndex(int skillID)
	{
		return NKMUnitManager.GetUnitTempletBase(m_UnitID)?.GetSkillIndex(skillID) ?? (-1);
	}

	public int GetUnitSkillLevel(int skillID)
	{
		int unitSkillIndex = GetUnitSkillIndex(skillID);
		if (unitSkillIndex < 0)
		{
			return 0;
		}
		return GetSkillLevelByIndex(unitSkillIndex);
	}

	public NKMUnitSkillTemplet GetUnitSkillTempletByIndex(int index)
	{
		return NKMUnitSkillManager.GetSkillTemplet(NKMUnitManager.GetUnitTempletBase(m_UnitID).GetSkillStrID(index), GetSkillLevelByIndex(index));
	}

	public NKMUnitSkillTemplet GetUnitSkillTempletByType(NKM_SKILL_TYPE eSkillType)
	{
		if (eSkillType == NKM_SKILL_TYPE.NST_INVALID)
		{
			return null;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
		if (unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL)
		{
			return null;
		}
		for (int i = 0; i < unitTempletBase.GetSkillCount(); i++)
		{
			string skillStrID = unitTempletBase.GetSkillStrID(i);
			int skillLevelByIndex = GetSkillLevelByIndex(i);
			NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(skillStrID, skillLevelByIndex);
			if (skillTemplet == null)
			{
				Log.Error($"SkillType {eSkillType} of unitID {m_UnitID} (SkillID {skillStrID}, Lv {skillLevelByIndex}) not found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitData.cs", 460);
			}
			else if (skillTemplet.m_NKM_SKILL_TYPE == eSkillType)
			{
				return skillTemplet;
			}
		}
		if (eSkillType != NKM_SKILL_TYPE.NST_ATTACK)
		{
			Log.Error($"unitID {m_UnitID} do not have skillType {eSkillType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitData.cs", 471);
		}
		return null;
	}

	public int GetSkillIndex(string skillStrID)
	{
		return NKMUnitManager.GetUnitTempletBase(m_UnitID)?.GetSkillIndex(skillStrID) ?? (-1);
	}

	public int GetSkillLevel(string skillStrID)
	{
		int skillIndex = GetSkillIndex(skillStrID);
		if (skillIndex < 0)
		{
			return 0;
		}
		return GetSkillLevelByIndex(skillIndex);
	}

	public int GetSkillLevelByIndex(int index)
	{
		if (index < 0)
		{
			return 0;
		}
		if (index < m_aUnitSkillLevel.Length)
		{
			if (m_aUnitSkillLevel[index] != 0)
			{
				return m_aUnitSkillLevel[index];
			}
			return 1;
		}
		return 0;
	}

	public bool IsUnitSkillUnlockedByType(NKM_SKILL_TYPE eSkillType)
	{
		if (eSkillType == NKM_SKILL_TYPE.NST_INVALID)
		{
			return true;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
		if (unitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL)
		{
			return true;
		}
		for (int i = 0; i < unitTempletBase.GetSkillCount(); i++)
		{
			string skillStrID = unitTempletBase.GetSkillStrID(i);
			int skillLevelByIndex = GetSkillLevelByIndex(i);
			NKMUnitSkillTemplet skillTemplet = NKMUnitSkillManager.GetSkillTemplet(skillStrID, skillLevelByIndex);
			if (skillTemplet == null)
			{
				Log.Error($"SkillType {eSkillType} of unitID {m_UnitID} (SkillID {skillStrID}, Lv {skillLevelByIndex}) not found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitData.cs", 532);
			}
			else if (skillTemplet.m_NKM_SKILL_TYPE == eSkillType)
			{
				return !NKMUnitSkillManager.IsLockedSkill(skillTemplet.m_ID, m_LimitBreakLevel);
			}
		}
		if (eSkillType != NKM_SKILL_TYPE.NST_ATTACK)
		{
			Log.Error($"unitID {m_UnitID} do not have skillType {eSkillType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitData.cs", 543);
		}
		return true;
	}

	public int GetUnitSkillCount()
	{
		return NKMUnitManager.GetUnitTempletBase(m_UnitID).GetSkillCount();
	}

	public int GetStarGrade(NKMUnitTempletBase templetBase)
	{
		NKM_UNIT_TYPE nKM_UNIT_TYPE = templetBase.m_NKM_UNIT_TYPE;
		if (nKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL || nKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			return templetBase.m_StarGradeMax - 3 + Math.Min(m_LimitBreakLevel, (short)3);
		}
		return templetBase.m_StarGradeMax;
	}

	public int GetStarGrade()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
		NKM_UNIT_TYPE nKM_UNIT_TYPE = unitTempletBase.m_NKM_UNIT_TYPE;
		if (nKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL || nKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			return unitTempletBase.m_StarGradeMax - 3 + Math.Min(m_LimitBreakLevel, (short)3);
		}
		return unitTempletBase.m_StarGradeMax;
	}

	public NKM_UNIT_GRADE GetUnitGrade()
	{
		return NKMUnitManager.GetUnitTempletBase(m_UnitID)?.m_NKM_UNIT_GRADE ?? NKM_UNIT_GRADE.NUG_N;
	}

	public int CalculateOperationPower(NKMInventoryData inventoryData, int checkLv = 0, Dictionary<int, NKMBanShipData> dicNKMBanShipData = null, NKMOperator operatorData = null)
	{
		switch (NKMUnitManager.GetUnitTempletBase(m_UnitID).m_NKM_UNIT_TYPE)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
		{
			NKMEquipmentSet equipmentSet = GetEquipmentSet(inventoryData);
			return CalculateUnitOperationPower(equipmentSet);
		}
		case NKM_UNIT_TYPE.NUT_SHIP:
			return CalculateShipOperationPower(checkLv, dicNKMBanShipData);
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			return operatorData.CalculateOperatorOperationPower();
		default:
			return 0;
		}
	}

	public void InitEquipItemFromDb(long weaponUid, long defenceUid, long accessoryUid, long accessroy2Uid)
	{
		m_EquipItemList[0] = weaponUid;
		m_EquipItemList[1] = defenceUid;
		m_EquipItemList[2] = accessoryUid;
		m_EquipItemList[3] = accessroy2Uid;
	}

	public bool EquipItem(NKMInventoryData InventoryData, long equip_item_uid, out long unequip_item_uid, ITEM_EQUIP_POSITION equipPosition)
	{
		unequip_item_uid = 0L;
		if (equipPosition == ITEM_EQUIP_POSITION.IEP_NONE)
		{
			return false;
		}
		if (equip_item_uid == 0L)
		{
			return false;
		}
		unequip_item_uid = m_EquipItemList[(int)equipPosition];
		if (unequip_item_uid != 0L && !UnEquipItem(InventoryData, unequip_item_uid, equipPosition))
		{
			return false;
		}
		NKMEquipItemData itemEquip = InventoryData.GetItemEquip(equip_item_uid);
		if (itemEquip == null)
		{
			return false;
		}
		itemEquip.m_OwnerUnitUID = m_UnitUID;
		m_EquipItemList[(int)equipPosition] = equip_item_uid;
		return true;
	}

	public bool UnEquipItem(NKMInventoryData InventoryData, long equipUid)
	{
		int num = -1;
		for (int i = 0; i < EquipItemUids.Count; i++)
		{
			if (m_EquipItemList[i] == equipUid)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return false;
		}
		NKMEquipItemData itemEquip = InventoryData.GetItemEquip(equipUid);
		if (itemEquip == null)
		{
			return false;
		}
		itemEquip.m_OwnerUnitUID = -1L;
		m_EquipItemList[num] = 0L;
		return true;
	}

	public bool UnEquipItem(NKMInventoryData InventoryData, long unequip_item_uid, ITEM_EQUIP_POSITION equipPisition)
	{
		if (equipPisition == ITEM_EQUIP_POSITION.IEP_NONE)
		{
			return false;
		}
		if (m_EquipItemList[(int)equipPisition] == 0L)
		{
			return false;
		}
		NKMEquipItemData itemEquip = InventoryData.GetItemEquip(unequip_item_uid);
		if (itemEquip == null)
		{
			return false;
		}
		itemEquip.m_OwnerUnitUID = -1L;
		m_EquipItemList[(int)equipPisition] = 0L;
		return true;
	}

	public long GetEquipItemWeaponUid()
	{
		return m_EquipItemList[0];
	}

	public long GetEquipItemDefenceUid()
	{
		return m_EquipItemList[1];
	}

	public long GetEquipItemAccessoryUid()
	{
		return m_EquipItemList[2];
	}

	public long GetEquipItemAccessory2Uid()
	{
		return m_EquipItemList[3];
	}

	public long GetEquipUid(ITEM_EQUIP_POSITION eITEM_EQUIP_POSITION)
	{
		return m_EquipItemList[(int)eITEM_EQUIP_POSITION];
	}

	public void ResetEquipment()
	{
		for (int i = 0; i < m_EquipItemList.Length; i++)
		{
			m_EquipItemList[i] = 0L;
		}
	}

	public IEnumerable<long> GetValidEquipUids()
	{
		int i = 0;
		while (i < m_EquipItemList.Length)
		{
			if (m_EquipItemList[i] > 0)
			{
				yield return m_EquipItemList[i];
			}
			int num = i + 1;
			i = num;
		}
	}

	public void FillDataFromDummy(NKMDummyUnitData cNKMDummyUnitData)
	{
		m_UnitID = cNKMDummyUnitData.UnitId;
		m_UnitLevel = cNKMDummyUnitData.UnitLevel;
		m_SkinID = cNKMDummyUnitData.SkinId;
		m_LimitBreakLevel = cNKMDummyUnitData.LimitBreakLevel;
		tacticLevel = cNKMDummyUnitData.TacticLevel;
	}

	public void FillDataFromAsyncUnitData(NKMAsyncUnitData cNKMAsyncleUnitData)
	{
		m_UnitUID = cNKMAsyncleUnitData.unitUid;
		m_UnitID = cNKMAsyncleUnitData.unitId;
		m_UnitLevel = cNKMAsyncleUnitData.unitLevel;
		m_SkinID = cNKMAsyncleUnitData.skinId;
		m_LimitBreakLevel = (short)cNKMAsyncleUnitData.limitBreakLevel;
		ShipCommandModule = cNKMAsyncleUnitData.shipModules;
		tacticLevel = cNKMAsyncleUnitData.tacticLevel;
		reactorLevel = cNKMAsyncleUnitData.reactorLevel;
		for (int i = 0; i < m_aUnitSkillLevel.Length; i++)
		{
			if (cNKMAsyncleUnitData.skillLevel.Count > i)
			{
				m_aUnitSkillLevel[i] = cNKMAsyncleUnitData.skillLevel[i];
			}
		}
		for (int j = 0; j < m_EquipItemList.Length; j++)
		{
			if (cNKMAsyncleUnitData.equipUids.Count > j)
			{
				m_EquipItemList[j] = cNKMAsyncleUnitData.equipUids[j];
			}
		}
		for (int k = 0; k < m_listStatEXP.Count; k++)
		{
			if (cNKMAsyncleUnitData.statExp.Count > k)
			{
				m_listStatEXP[k] = cNKMAsyncleUnitData.statExp[k];
			}
		}
	}

	public void FillDataFromRespawnOrigin(NKMUnitData cRespawnOriginUnitData, bool bSimple)
	{
		m_UnitLevel = cRespawnOriginUnitData.m_UnitLevel;
		m_LimitBreakLevel = cRespawnOriginUnitData.m_LimitBreakLevel;
		tacticLevel = cRespawnOriginUnitData.tacticLevel;
		if (!bSimple)
		{
			for (int i = 0; i < m_aUnitSkillLevel.Length; i++)
			{
				m_aUnitSkillLevel[i] = cRespawnOriginUnitData.m_aUnitSkillLevel[i];
			}
			for (int j = 0; j < m_listStatEXP.Count; j++)
			{
				m_listStatEXP[j] = cRespawnOriginUnitData.m_listStatEXP[j];
			}
			for (int k = 0; k < m_EquipItemList.Length; k++)
			{
				m_EquipItemList[k] = cRespawnOriginUnitData.m_EquipItemList[k];
			}
			loyalty = cRespawnOriginUnitData.loyalty;
		}
	}

	public bool IsDungeonUnit()
	{
		return m_DungeonRespawnUnitTemplet != null;
	}

	public float GetMultiplierByPermanentContract()
	{
		if (isPermanentContract || loyalty >= 10000)
		{
			return 0.02f;
		}
		return 0f;
	}

	public void SetPermanentContract()
	{
		isPermanentContract = true;
	}

	public bool IsPermanentContractEnable()
	{
		if (loyalty >= 10000)
		{
			return !isPermanentContract;
		}
		return false;
	}

	public NKMEquipmentSet GetEquipmentSet(NKMInventoryData inventoryData)
	{
		if (inventoryData == null)
		{
			return null;
		}
		NKMEquipItemData itemEquip = inventoryData.GetItemEquip(GetEquipItemWeaponUid());
		NKMEquipItemData itemEquip2 = inventoryData.GetItemEquip(GetEquipItemDefenceUid());
		NKMEquipItemData itemEquip3 = inventoryData.GetItemEquip(GetEquipItemAccessoryUid());
		NKMEquipItemData itemEquip4 = inventoryData.GetItemEquip(GetEquipItemAccessory2Uid());
		return new NKMEquipmentSet(itemEquip, itemEquip2, itemEquip3, itemEquip4);
	}

	public NKMDummyUnitData ToDummyUnitData()
	{
		return new NKMDummyUnitData
		{
			UnitId = m_UnitID,
			UnitLevel = m_UnitLevel,
			LimitBreakLevel = m_LimitBreakLevel,
			SkinId = m_SkinID,
			TacticLevel = tacticLevel
		};
	}

	public int CalculateUnitOperationPower(NKMEquipmentSet equipmentSet)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
		if (unitTempletBase == null)
		{
			Log.Error($"Can not found UnitTempletBase. UnitId:{m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitData.cs", 870);
			return 0;
		}
		float num = (float)(NKMConst.OperationPowerFactor.GetClassValue(unitTempletBase.m_NKM_UNIT_ROLE_TYPE) + NKMConst.OperationPowerFactor.GetGradeValue(unitTempletBase.m_NKM_UNIT_GRADE, unitTempletBase.m_bAwaken, unitTempletBase.IsRearmUnit)) * ((float)m_UnitLevel / 100f) * (1f + 0.1f * (float)m_LimitBreakLevel);
		int num2 = (unitTempletBase.IsRearmUnit ? 50 : 100);
		int num3 = m_aUnitSkillLevel.Sum() * num2;
		float num4 = (float)Math.Pow(tacticLevel, 1.1) * (float)NKMConst.OperationPowerFactor.GetTacticValue(unitTempletBase.m_NKM_UNIT_GRADE, unitTempletBase.m_bAwaken, unitTempletBase.IsRearmUnit);
		int num5 = reactorLevel * NKMConst.OperationPowerFactor.GetReactorValue(unitTempletBase.m_NKM_UNIT_GRADE, unitTempletBase.m_bAwaken, unitTempletBase.IsRearmUnit);
		float num6 = (num + (float)num3 + num4 + (float)num5) * 0.6f;
		int num7 = 0;
		if (equipmentSet != null)
		{
			HashSet<long> setItemActivatedMark = NKMItemManager.GetSetItemActivatedMark(equipmentSet);
			num7 = CalculateEquipmentOperationPower(equipmentSet.Weapon, setItemActivatedMark.Contains(equipmentSet.WeaponUid));
			num7 += CalculateEquipmentOperationPower(equipmentSet.Defence, setItemActivatedMark.Contains(equipmentSet.DefenceUid));
			num7 += CalculateEquipmentOperationPower(equipmentSet.Accessory, setItemActivatedMark.Contains(equipmentSet.AccessoryUid));
			num7 += CalculateEquipmentOperationPower(equipmentSet.Accessory2, setItemActivatedMark.Contains(equipmentSet.Accessory2Uid));
		}
		float num8 = (float)num7 * 0.5f;
		return (int)(num6 + num8);
	}

	public UnitLoyaltyUpdateData ToUnitLoyaltyUpdateData()
	{
		return new UnitLoyaltyUpdateData
		{
			unitUid = m_UnitUID,
			loyalty = loyalty,
			officeRoomId = OfficeRoomId,
			officeGrade = OfficeGrade,
			heartGaugeStartTime = OfficeGaugeStartTime
		};
	}

	private int CalculateEquipmentOperationPower(NKMEquipItemData equipItemData, bool bSetOptionEnabled)
	{
		if (equipItemData == null)
		{
			return 0;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipItemData.m_ItemEquipID);
		if (equipTemplet == null)
		{
			Log.Error($"Can not found EquipTemplet. EquipId:{equipItemData.m_ItemEquipID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitData.cs", 926);
			return 0;
		}
		int nKM_ITEM_TIER = equipTemplet.m_NKM_ITEM_TIER;
		float num = 100f * ((float)nKM_ITEM_TIER + 1.2f);
		float num2 = (int)(equipTemplet.m_NKM_ITEM_GRADE + 1 - 1) * (150 + 15 * (nKM_ITEM_TIER - 1));
		float num3 = (num + num2) * (1f + 0.1f * (float)equipItemData.m_EnchantLevel);
		float precisionFactor = GetPrecisionFactor(nKM_ITEM_TIER, equipItemData.m_Precision, equipTemplet.m_bRelic);
		float num4 = 0f;
		if (equipItemData.m_Precision2 > 0)
		{
			num4 = GetPrecisionFactor(nKM_ITEM_TIER, equipItemData.m_Precision2, bRelic: false);
		}
		float num5 = 0f;
		if (equipTemplet.m_bRelic && equipItemData.potentialOptions.Count > 0)
		{
			float num6 = equipItemData.CalculatePotentialPercent();
			num5 = (10f + (float)nKM_ITEM_TIER * 12.5f) * (float)equipItemData.potentialOptions[0].OpenedSocketCount * (1f + num6 - 0.5f);
		}
		float num7 = 1f;
		if (bSetOptionEnabled)
		{
			NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(equipItemData.m_SetOptionId);
			if (equipSetOptionTemplet != null)
			{
				num7 = 1f + (float)equipSetOptionTemplet.m_EquipSetPart * 0.05f;
			}
		}
		return (int)((num3 + precisionFactor + num4 + num5) * num7);
	}

	private float GetPrecisionFactor(int equipTier, int precision, bool bRelic)
	{
		int num = (bRelic ? 2 : 8);
		return (float)((4 + equipTier * 5) * num) * ((50f + (float)precision / 2f) / 100f);
	}

	private int CalculateShipOperationPower(int checkLv = 0, Dictionary<int, NKMBanShipData> dicNKMBanShipData = null)
	{
		int num = (int)(NKMUnitManager.GetUnitTempletBase(m_UnitID).m_NKM_UNIT_GRADE + 1);
		int level = ((checkLv != 0) ? checkLv : m_UnitLevel);
		int shipFactor = GetShipFactor1(num, level);
		int num2 = (NKMUnitManager.GetUnitTempletBase(m_UnitID).m_StarGradeMax - 1) * 250 * (num + 2);
		int shipFactor2 = GetShipFactor3();
		int num3 = shipFactor + num2 + shipFactor2;
		if (dicNKMBanShipData != null && dicNKMBanShipData.TryGetValue(GetShipGroupId(), out var value))
		{
			int nerfPercentByShipBanLevel = NKMUnitStatManager.GetNerfPercentByShipBanLevel(value.m_BanLevel);
			num3 = (int)((float)num3 * ((float)(100 - nerfPercentByShipBanLevel) * 0.01f));
		}
		return num3;
	}

	private int GetShipFactor1(int shipGrade, int level)
	{
		return level * (120 + shipGrade * 3);
	}

	private int GetShipFactor3()
	{
		int num = m_LimitBreakLevel * 300;
		int num2 = 0;
		if (ShipCommandModule != null)
		{
			for (int i = 0; i < ShipCommandModule.Count; i++)
			{
				if (ShipCommandModule[i] == null || ShipCommandModule[i].slots == null)
				{
					continue;
				}
				for (int j = 0; j < ShipCommandModule[i].slots.Length; j++)
				{
					NKMShipCmdSlot nKMShipCmdSlot = ShipCommandModule[i].slots[j];
					if (nKMShipCmdSlot != null && nKMShipCmdSlot.statType != NKM_STAT_TYPE.NST_RANDOM)
					{
						num2++;
						break;
					}
				}
			}
		}
		return num + num2 * 500;
	}

	public bool IsUnlockAccessory2()
	{
		return m_LimitBreakLevel >= 3;
	}

	public bool IsSameBaseUnit(int unitID)
	{
		if (unitID == m_UnitID)
		{
			return true;
		}
		return NKMUnitManager.GetUnitTempletBase(m_UnitID)?.IsSameBaseUnit(unitID) ?? false;
	}

	public void SetSeized()
	{
		isSeized = true;
	}
}
