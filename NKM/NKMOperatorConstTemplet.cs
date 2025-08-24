using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMOperatorConstTemplet
{
	public class Negotiation
	{
		public readonly int itemId;

		public readonly int exp;

		public readonly int credit;

		public Negotiation(int itemId, int exp, int credit)
		{
			this.itemId = itemId;
			this.exp = exp;
			this.credit = credit;
		}
	}

	public class MaterialUnit
	{
		public NKM_UNIT_GRADE m_NKM_UNIT_GRADE;

		public int commandLevelUpPercent;

		public int levelUpSuccessRatePercent;

		public int transportSuccessRatePercent;

		public MaterialUnit(NKM_UNIT_GRADE grade, int commandLevelUpPercent, int levelUpSuccessRatePercent, int transportSuccessRatePercent)
		{
			m_NKM_UNIT_GRADE = grade;
			this.commandLevelUpPercent = commandLevelUpPercent;
			this.levelUpSuccessRatePercent = levelUpSuccessRatePercent;
			this.transportSuccessRatePercent = transportSuccessRatePercent;
		}

		public NKMConst.Buff.BuffType GetCompanyBuffType()
		{
			return m_NKM_UNIT_GRADE switch
			{
				NKM_UNIT_GRADE.NUG_SSR => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SSR, 
				NKM_UNIT_GRADE.NUG_SR => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SR, 
				NKM_UNIT_GRADE.NUG_R => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_R, 
				NKM_UNIT_GRADE.NUG_N => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N, 
				_ => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N, 
			};
		}
	}

	public class HostUnit
	{
		public NKM_UNIT_GRADE m_NKM_UNIT_GRADE;

		public int itemId;

		public int itemCount;

		public int extractPriceItemId;

		public int extractPrice;

		public HostUnit(NKM_UNIT_GRADE grade, int itemId, int itemCount, int extractPriceItemId, int extractPrice)
		{
			m_NKM_UNIT_GRADE = grade;
			this.itemId = itemId;
			this.itemCount = itemCount;
			this.extractPriceItemId = extractPriceItemId;
			this.extractPrice = extractPrice;
		}
	}

	public class PassiveToken
	{
		public NKM_ITEM_GRADE m_NKM_ITEM_GRADE;

		public List<int> ItemID;

		public int LevelUpSuccessRatePercent;

		public int TransportSuccessRatePercent;

		public PassiveToken(NKM_ITEM_GRADE itemGrade, List<int> itemID, int levelUpSuccessRatePercent, int transportSuccessRatePercent)
		{
			m_NKM_ITEM_GRADE = itemGrade;
			ItemID = itemID;
			LevelUpSuccessRatePercent = levelUpSuccessRatePercent;
			TransportSuccessRatePercent = transportSuccessRatePercent;
		}

		public NKMConst.Buff.BuffType GetCompanyBuffType()
		{
			return m_NKM_ITEM_GRADE switch
			{
				NKM_ITEM_GRADE.NIG_SSR => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SSR, 
				NKM_ITEM_GRADE.NIG_SR => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_SR, 
				NKM_ITEM_GRADE.NIG_R => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_R, 
				NKM_ITEM_GRADE.NIG_N => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N, 
				_ => NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_SUCCESS_RATE_BONUS_N, 
			};
		}
	}

	private const int MaxKindOfMaterialCount = 3;

	private const int MaxKindOfMaterialCount_PassiveToken = 4;

	public int unitMaximumLevel;

	public int maxMaterialUsageLimit = 200;

	public Negotiation[] list = new Negotiation[3];

	public List<MaterialUnit> materialUntis = new List<MaterialUnit>();

	public List<HostUnit> hostUnits = new List<HostUnit>();

	public PassiveToken[] listPassiveToken = new PassiveToken[4];

	public void LoadFromLua(NKMLua lua)
	{
		using (lua.OpenTable("MaterialUnit", "Operator MaterialUnit table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 20))
		{
			int num = 1;
			while (lua.OpenTable(num++))
			{
				NKM_UNIT_GRADE grade = lua.GetEnum<NKM_UNIT_GRADE>("m_NKM_UNIT_GRADE");
				int @int = lua.GetInt32("CommandLevelUpPercent");
				int int2 = lua.GetInt32("LevelUpSuccessRatePercent");
				int int3 = lua.GetInt32("TransportSuccessRatePercent");
				materialUntis.Add(new MaterialUnit(grade, @int, int2, int3));
				lua.CloseTable();
			}
		}
		using (lua.OpenTable("HostUnit", "Operator HostUnit table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 34))
		{
			int num2 = 1;
			while (lua.OpenTable(num2++))
			{
				NKM_UNIT_GRADE grade2 = lua.GetEnum<NKM_UNIT_GRADE>("m_NKM_UNIT_GRADE");
				int int4 = lua.GetInt32("ItemId");
				int int5 = lua.GetInt32("ItemCount");
				int int6 = lua.GetInt32("m_ExtractPriceItemID");
				int int7 = lua.GetInt32("m_ExtractPrice");
				hostUnits.Add(new HostUnit(grade2, int4, int5, int6, int7));
				lua.CloseTable();
			}
		}
		using (lua.OpenTable("Negotiation", "Operator Negotiation table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 49))
		{
			lua.GetData("MaxMaterialUsageLimit", ref maxMaterialUsageLimit);
			using (lua.OpenTable("Materials", "Operator Negotiation loading materials table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 53))
			{
				int rValue = 0;
				int rValue2 = 0;
				int rValue3 = 0;
				for (int i = 0; i < 3; i++)
				{
					int iIndex = i + 1;
					if (lua.OpenTable(iIndex))
					{
						lua.GetData("ItemId", ref rValue);
						lua.GetData("Exp", ref rValue2);
						lua.GetData("Credit", ref rValue3);
						list[i] = new Negotiation(rValue, rValue2, rValue3);
						lua.CloseTable();
					}
				}
			}
		}
		using (lua.OpenTable("PassiveToken", "Operator PassiveToken table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 74))
		{
			using (lua.OpenTable("Materials", "Operator PassiveToken loading materials table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 76))
			{
				for (int j = 0; j < 4; j++)
				{
					int iIndex2 = j + 1;
					if (lua.OpenTable(iIndex2))
					{
						NKM_ITEM_GRADE itemGrade = lua.GetEnum<NKM_ITEM_GRADE>("m_NKM_ITEM_GRADE");
						List<int> result = new List<int>();
						lua.GetDataList("ItemID", out result, nullIfEmpty: false);
						int int8 = lua.GetInt32("LevelUpSuccessRatePercent");
						int int9 = lua.GetInt32("TransportSuccessRatePercent");
						listPassiveToken[j] = new PassiveToken(itemGrade, result, int8, int9);
						lua.CloseTable();
					}
				}
			}
		}
		using (lua.OpenTable("Const", "Operator Const table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 96))
		{
			lua.GetData("MaximumLevel", ref unitMaximumLevel);
		}
	}

	public void Validate()
	{
		Negotiation[] array = list;
		foreach (Negotiation negotiation in array)
		{
			if (NKMItemManager.GetItemMiscTempletByID(negotiation.itemId) == null)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 오퍼레이터 레벨업 아이템 id가 올바르지 않습니다. itemId:{negotiation.itemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 108);
			}
			if (negotiation.exp < 0)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 오퍼레이터 레벨업 아이템 경험치가 올바르지 않습니다. itemId:{negotiation.itemId} exp:{negotiation.exp}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 113);
			}
			if (negotiation.credit < 0)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 오퍼레이터 레벨업 아이템 당 소비 크레딧 정보가 올바르지 않습니다. itemId:{negotiation.itemId} credit:{negotiation.credit}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 118);
			}
		}
		foreach (MaterialUnit materialUnti in materialUntis)
		{
			if (materialUnti.commandLevelUpPercent < 0)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 재료 오퍼 등급에 따른 오퍼레이터 메인 스킬 강화 성공 확률이 올바르지 않습니다. grade:{materialUnti.m_NKM_UNIT_GRADE} commandLevelUpPercent:{materialUnti.commandLevelUpPercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 126);
			}
			if (materialUnti.levelUpSuccessRatePercent < 0)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 재료 오퍼 등급에 따른 오퍼레이터 보조 스킬 강화 성공 확률이 올바르지 않습니다. grade:{materialUnti.m_NKM_UNIT_GRADE} levelUpSuccessRatePercent:{materialUnti.levelUpSuccessRatePercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 131);
			}
			if (materialUnti.transportSuccessRatePercent < 0)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 재료 오퍼 등급에 따른 오퍼레이터 보조 스킬 이식 성공 확률이 올바르지 않습니다. grade:{materialUnti.m_NKM_UNIT_GRADE} transportSuccessRatePercent:{materialUnti.transportSuccessRatePercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 136);
			}
		}
		foreach (HostUnit hostUnit in hostUnits)
		{
			if (NKMItemManager.GetItemMiscTempletByID(hostUnit.itemId) == null)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 등급별 오퍼레이터 스킬 강화에 사용되는 아이템이 올바르지 않습니다. grade:{hostUnit.m_NKM_UNIT_GRADE} itemId:{hostUnit.itemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 144);
			}
			if (hostUnit.itemCount < 0)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 등급별 오퍼레이터 스킬 강화에 필요한 재화량이 올바르지 않습니다. grade:{hostUnit.m_NKM_UNIT_GRADE} itemCount:{hostUnit.itemCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 149);
			}
			if (NKMItemManager.GetItemMiscTempletByID(hostUnit.extractPriceItemId) == null)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 등급별 오퍼레이터 추출시 필요한 재화 아이템이 올바르지 않습니다. grade:{hostUnit.m_NKM_UNIT_GRADE} extractPriceItemId:{hostUnit.extractPriceItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 154);
			}
			if (hostUnit.extractPrice < 0)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 등급별 오퍼레이터 추출에 필요한 재화량이 올바르지 않습니다. grade:{hostUnit.m_NKM_UNIT_GRADE} extractPrice:{hostUnit.extractPrice}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 159);
			}
		}
		PassiveToken[] array2 = listPassiveToken;
		foreach (PassiveToken passiveToken in array2)
		{
			if (passiveToken.ItemID.Any((int e) => NKMItemManager.GetItemMiscTempletByID(e) == null))
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 등급별 토큰 아이템 id 목록 중 올바르지 않은 항목이 존재합니다. grade:{passiveToken.m_NKM_ITEM_GRADE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 167);
			}
			if (passiveToken.LevelUpSuccessRatePercent < 0)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 토큰 아이템 등급에 따른 토큰 보조 스킬 강화 성공 확률이 올바르지 않습니다. grade:{passiveToken.m_NKM_ITEM_GRADE} LevelUpSuccessRatePercent:{passiveToken.LevelUpSuccessRatePercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 172);
			}
			if (passiveToken.TransportSuccessRatePercent < 0)
			{
				NKMTempletError.Add($"[OperatorConstTemplet] 토큰 아이템 등급에 따른 토큰 보조 스킬 변경 성공 확률이 올바르지 않습니다. grade:{passiveToken.m_NKM_ITEM_GRADE} TransportSuccessRatePercent:{passiveToken.TransportSuccessRatePercent}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorConstTemplet.cs", 176);
			}
		}
	}
}
