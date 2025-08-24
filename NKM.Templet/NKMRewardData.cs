using System.Collections.Generic;
using System.Runtime.Serialization;
using ClientPacket.Contract;
using ClientPacket.Event;
using ClientPacket.Office;
using Cs.Protocol;

namespace NKM.Templet;

[DataContract]
public sealed class NKMRewardData : Cs.Protocol.ISerializable
{
	private List<NKMUnitData> unitDataList = new List<NKMUnitData>();

	private List<NKMOperator> operatorList = new List<NKMOperator>();

	private List<NKMItemMiscData> miscItemDataList = new List<NKMItemMiscData>();

	private List<NKMEquipItemData> equipItemDataList = new List<NKMEquipItemData>();

	private HashSet<int> skinIdList = new HashSet<int>();

	private List<NKMMoldItemData> moldItemDataList = new List<NKMMoldItemData>();

	private List<NKMCompanyBuffData> companyBuffDataList = new List<NKMCompanyBuffData>();

	private List<NKMRewardUnitExpData> unitExpDataList = new List<NKMRewardUnitExpData>();

	private List<NKMBingoTile> bingoTileList = new List<NKMBingoTile>();

	private HashSet<int> emoticonList = new HashSet<int>();

	private List<NKMInteriorData> interiors = new List<NKMInteriorData>();

	private int dailyMissionPoint;

	private int weeklyMissionPoint;

	private int userExp;

	private int bonusRatioOfUserExp;

	private long achievePoint;

	public List<MiscContractResult> contractList = new List<MiscContractResult>();

	public List<NKMRewardUnitExpData> UnitExpDataList => unitExpDataList;

	[DataMember]
	public List<NKMUnitData> UnitDataList => unitDataList;

	[DataMember]
	public List<NKMOperator> OperatorList => operatorList;

	[DataMember]
	public List<NKMItemMiscData> MiscItemDataList => miscItemDataList;

	[DataMember]
	public List<NKMEquipItemData> EquipItemDataList => equipItemDataList;

	[DataMember]
	public HashSet<int> SkinIdList => skinIdList;

	[DataMember]
	public List<NKMMoldItemData> MoldItemDataList => moldItemDataList;

	[DataMember]
	public List<NKMCompanyBuffData> CompanyBuffDataList => companyBuffDataList;

	[DataMember]
	public HashSet<int> EmoticonList => emoticonList;

	[DataMember]
	public List<NKMBingoTile> BingoTileList => bingoTileList;

	[DataMember]
	public List<NKMInteriorData> Interiors => interiors;

	public List<MiscContractResult> ContractList => contractList;

	public int UserExp => userExp;

	public int BonusRatioOfUserExp => bonusRatioOfUserExp;

	public int DailyMissionPoint => dailyMissionPoint;

	public int WeeklyMissionPoint => weeklyMissionPoint;

	public long AchievePoint => achievePoint;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userExp);
		stream.PutOrGet(ref bonusRatioOfUserExp);
		stream.PutOrGet(ref unitDataList);
		stream.PutOrGet(ref miscItemDataList);
		stream.PutOrGet(ref equipItemDataList);
		stream.PutOrGet(ref unitExpDataList);
		stream.PutOrGet(ref skinIdList);
		stream.PutOrGet(ref moldItemDataList);
		stream.PutOrGet(ref companyBuffDataList);
		stream.PutOrGet(ref companyBuffDataList);
		stream.PutOrGet(ref emoticonList);
		stream.PutOrGet(ref dailyMissionPoint);
		stream.PutOrGet(ref weeklyMissionPoint);
		stream.PutOrGet(ref bingoTileList);
		stream.PutOrGet(ref achievePoint);
		stream.PutOrGet(ref operatorList);
		stream.PutOrGet(ref contractList);
		stream.PutOrGet(ref interiors);
	}

	public void SetMiscItemData(List<NKMItemMiscData> miscItemDataList)
	{
		this.miscItemDataList = miscItemDataList;
	}

	public void SetUnitData(List<NKMUnitData> unitDataList)
	{
		this.unitDataList = unitDataList;
	}

	public void SetUserExp(int userExp)
	{
		this.userExp = userExp;
	}

	public void SetBonusRatioOfUserExp(int bonusRatio)
	{
		bonusRatioOfUserExp = bonusRatio;
	}

	public void SetDailyMissionPoint(int missionPoint)
	{
		dailyMissionPoint = missionPoint;
	}

	public void SetWeeklyMissionPoint(int missionPoint)
	{
		weeklyMissionPoint = missionPoint;
	}

	public void AddAchievePoint(long achievePointCount)
	{
		achievePoint += achievePointCount;
	}

	public void SetOperatorList(List<NKMOperator> lstOper)
	{
		operatorList = lstOper;
	}

	public void Upsert(NKMItemMiscData newData)
	{
		NKMItemMiscData nKMItemMiscData = miscItemDataList.Find((NKMItemMiscData e) => e.ItemID == newData.ItemID);
		if (nKMItemMiscData == null)
		{
			miscItemDataList.Add(newData.DeepCopy());
			return;
		}
		nKMItemMiscData.CountFree += newData.CountFree;
		nKMItemMiscData.CountPaid += newData.CountPaid;
	}

	public void Upsert(in NKMInteriorData newData)
	{
		int itemId = newData.itemId;
		NKMInteriorData nKMInteriorData = interiors.Find((NKMInteriorData e) => e.itemId == itemId);
		if (nKMInteriorData == null)
		{
			interiors.Add(newData.DeepCopy());
		}
		else
		{
			nKMInteriorData.count += newData.count;
		}
	}

	public void Upsert(NKMMoldItemData newData)
	{
		NKMMoldItemData nKMMoldItemData = moldItemDataList.Find((NKMMoldItemData e) => e.m_MoldID == newData.m_MoldID);
		if (nKMMoldItemData == null)
		{
			moldItemDataList.Add(newData.DeepCopy());
		}
		else
		{
			nKMMoldItemData.m_Count += newData.m_Count;
		}
	}

	public void AddUnitExp(long unitUID, int unitExp, int unitBonusExp, int unitBonusRatio)
	{
		NKMRewardUnitExpData nKMRewardUnitExpData = unitExpDataList.Find((NKMRewardUnitExpData e) => e.m_UnitUid == unitUID);
		if (nKMRewardUnitExpData == null)
		{
			nKMRewardUnitExpData = new NKMRewardUnitExpData
			{
				m_UnitUid = unitUID,
				m_Exp = 0,
				m_BonusExp = 0
			};
			unitExpDataList.Add(nKMRewardUnitExpData);
		}
		nKMRewardUnitExpData.m_Exp += unitExp;
		nKMRewardUnitExpData.m_BonusExp += unitBonusExp;
		nKMRewardUnitExpData.m_BonusRatio = unitBonusRatio;
	}

	public int GetUnitCount()
	{
		return unitDataList.Count + operatorList.Count;
	}

	public void AddRewardDataForRepeatOperation(NKMRewardData newNKMRewardData)
	{
		if (newNKMRewardData == null)
		{
			return;
		}
		SetUserExp(UserExp + newNKMRewardData.UserExp);
		unitDataList.AddRange(newNKMRewardData.unitDataList);
		EquipItemDataList.AddRange(newNKMRewardData.equipItemDataList);
		OperatorList.AddRange(newNKMRewardData.operatorList);
		emoticonList.UnionWith(newNKMRewardData.emoticonList);
		foreach (int skinId in newNKMRewardData.skinIdList)
		{
			skinIdList.Add(skinId);
		}
		for (int i = 0; i < newNKMRewardData.miscItemDataList.Count; i++)
		{
			Upsert(newNKMRewardData.miscItemDataList[i]);
		}
		for (int j = 0; j < newNKMRewardData.moldItemDataList.Count; j++)
		{
			Upsert(newNKMRewardData.moldItemDataList[j]);
		}
	}

	public bool HasAnyReward()
	{
		if (unitDataList.Count > 0)
		{
			return true;
		}
		if (operatorList.Count > 0)
		{
			return true;
		}
		if (miscItemDataList.Count > 0)
		{
			return true;
		}
		if (equipItemDataList.Count > 0)
		{
			return true;
		}
		if (skinIdList.Count > 0)
		{
			return true;
		}
		if (moldItemDataList.Count > 0)
		{
			return true;
		}
		if (companyBuffDataList.Count > 0)
		{
			return true;
		}
		if (unitExpDataList.Count > 0)
		{
			return true;
		}
		if (bingoTileList.Count > 0)
		{
			return true;
		}
		if (emoticonList.Count > 0)
		{
			return true;
		}
		if (interiors.Count > 0)
		{
			return true;
		}
		if (dailyMissionPoint > 0)
		{
			return true;
		}
		if (weeklyMissionPoint > 0)
		{
			return true;
		}
		if (userExp > 0)
		{
			return true;
		}
		if (bonusRatioOfUserExp > 0)
		{
			return true;
		}
		if (achievePoint > 0)
		{
			return true;
		}
		if (contractList.Count > 0)
		{
			return true;
		}
		return false;
	}

	public static NKMRewardData operator +(NKMRewardData a, NKMRewardData b)
	{
		NKMRewardData nKMRewardData = new NKMRewardData();
		nKMRewardData.unitDataList = a.unitDataList;
		nKMRewardData.unitDataList.AddRange(b.unitDataList);
		nKMRewardData.operatorList = a.operatorList;
		nKMRewardData.operatorList.AddRange(b.operatorList);
		nKMRewardData.miscItemDataList = a.miscItemDataList;
		nKMRewardData.miscItemDataList.AddRange(b.miscItemDataList);
		nKMRewardData.equipItemDataList = a.equipItemDataList;
		nKMRewardData.equipItemDataList.AddRange(b.equipItemDataList);
		nKMRewardData.skinIdList = a.skinIdList;
		foreach (int skinId in b.skinIdList)
		{
			nKMRewardData.skinIdList.Add(skinId);
		}
		nKMRewardData.moldItemDataList = a.moldItemDataList;
		nKMRewardData.moldItemDataList.AddRange(b.moldItemDataList);
		nKMRewardData.companyBuffDataList = a.companyBuffDataList;
		nKMRewardData.companyBuffDataList.AddRange(b.companyBuffDataList);
		nKMRewardData.unitExpDataList = a.unitExpDataList;
		nKMRewardData.unitExpDataList.AddRange(b.unitExpDataList);
		nKMRewardData.unitDataList = a.unitDataList;
		nKMRewardData.unitDataList.AddRange(b.unitDataList);
		nKMRewardData.bingoTileList = a.bingoTileList;
		nKMRewardData.bingoTileList.AddRange(b.bingoTileList);
		nKMRewardData.emoticonList = a.emoticonList;
		foreach (int emoticon in b.emoticonList)
		{
			nKMRewardData.emoticonList.Add(emoticon);
		}
		nKMRewardData.interiors = a.interiors;
		nKMRewardData.interiors.AddRange(b.interiors);
		nKMRewardData.dailyMissionPoint = a.dailyMissionPoint + b.dailyMissionPoint;
		nKMRewardData.weeklyMissionPoint = a.weeklyMissionPoint + b.weeklyMissionPoint;
		nKMRewardData.userExp = a.userExp + b.userExp;
		nKMRewardData.bonusRatioOfUserExp = a.bonusRatioOfUserExp + b.bonusRatioOfUserExp;
		nKMRewardData.achievePoint = a.achievePoint + b.achievePoint;
		nKMRewardData.contractList = a.contractList;
		nKMRewardData.contractList.AddRange(b.contractList);
		return nKMRewardData;
	}
}
