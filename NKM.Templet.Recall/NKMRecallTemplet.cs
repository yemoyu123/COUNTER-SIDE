using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet.Recall;

public sealed class NKMRecallTemplet : INKMTemplet, INKMTempletEx
{
	private static Dictionary<int, List<NKMRecallTemplet>> recallTempletMap = new Dictionary<int, List<NKMRecallTemplet>>();

	private NKMIntervalTemplet intervalTemplet;

	private string exchangeDateStrId;

	public int Index { get; private set; }

	public int UnitId { get; private set; }

	public int UnitExchangeGroupId { get; private set; }

	public NKM_UNIT_TYPE UnitType { get; private set; }

	public Recall_Condition RecallItemCondition { get; private set; }

	public NKM_REWARD_TYPE RecallItemType { get; private set; }

	public int RecallItemID { get; private set; }

	public int RecallItemQuantity { get; private set; }

	public string RecallItemSlotName { get; private set; }

	public int Key => Index;

	public NKMIntervalTemplet IntervalTemplet => intervalTemplet;

	public static NKMRecallTemplet Find(int unitId, DateTime now)
	{
		if (!recallTempletMap.TryGetValue(unitId, out var value))
		{
			return null;
		}
		foreach (NKMRecallTemplet item in value)
		{
			if (item.intervalTemplet == null)
			{
				return null;
			}
			if (item.intervalTemplet.IsValidTime(now))
			{
				return item;
			}
		}
		return null;
	}

	public static NKMRecallTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 60))
		{
			return null;
		}
		NKMRecallTemplet nKMRecallTemplet = new NKMRecallTemplet
		{
			Index = lua.GetInt32("Index"),
			UnitId = lua.GetInt32("UnitID"),
			UnitExchangeGroupId = lua.GetInt32("UnitExchangeGroupID"),
			UnitType = lua.GetEnum<NKM_UNIT_TYPE>("m_NKM_UNIT_TYPE")
		};
		nKMRecallTemplet.exchangeDateStrId = lua.GetString("ExchangeDateStrID");
		string rValue = string.Empty;
		lua.GetData("RecallItemCondition", ref rValue);
		if (string.IsNullOrEmpty(rValue))
		{
			nKMRecallTemplet.RecallItemCondition = Recall_Condition.None;
		}
		else
		{
			if (!Enum.TryParse<Recall_Condition>(rValue, out var result))
			{
				NKMTempletError.Add($"[NKMRecallTemplet] RecallItemCondition is not null, but Recall_Condition parse Fail. index:{nKMRecallTemplet.Index} input:{rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 85);
				return null;
			}
			nKMRecallTemplet.RecallItemCondition = result;
			nKMRecallTemplet.RecallItemType = lua.GetEnum<NKM_REWARD_TYPE>("RecallItemType");
			nKMRecallTemplet.RecallItemID = lua.GetInt32("RecallItemID");
			nKMRecallTemplet.RecallItemQuantity = lua.GetInt32("RecallItemQuantity");
			nKMRecallTemplet.RecallItemSlotName = lua.GetString("RecallItemSlotName");
		}
		return nKMRecallTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
		if (!recallTempletMap.ContainsKey(UnitId))
		{
			recallTempletMap.Add(UnitId, new List<NKMRecallTemplet>());
		}
		recallTempletMap[UnitId].Add(this);
	}

	public void JoinIntervalTemplet()
	{
		intervalTemplet = NKMIntervalTemplet.Find(exchangeDateStrId);
		if (intervalTemplet == null)
		{
			intervalTemplet = NKMIntervalTemplet.Invalid;
			NKMTempletError.Add("[NKMRecallTemplet] interval templet is null, interval str id: " + exchangeDateStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 120);
		}
	}

	public void Validate()
	{
		NKMUnitTemplet unitTemplet = NKMUnitManager.GetUnitTemplet(UnitId);
		if (unitTemplet == null)
		{
			Log.ErrorAndExit($"[NKMRecallTemplet] unit id가 유효하지 않습니다. unitId:{UnitId}, index:{Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 129);
			return;
		}
		if (unitTemplet.m_UnitTempletBase == null)
		{
			Log.ErrorAndExit($"[NKMRecallTemplet] unit id의 baseTemplet이 없습니다. unitId:{UnitId}, index:{Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 134);
			return;
		}
		if (unitTemplet.m_UnitTempletBase.m_bMonster)
		{
			Log.ErrorAndExit($"[NKMRecallTemplet] monster unit이 지정되었습니다.. unitId:{UnitId}, index:{Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 139);
			return;
		}
		NKM_UNIT_TYPE nKM_UNIT_TYPE = unitTemplet.m_UnitTempletBase.m_NKM_UNIT_TYPE;
		if (UnitType != nKM_UNIT_TYPE)
		{
			NKMTempletError.Add($"[NKMRecallTemplet] 입력한 유닛 타입 값과 입력한 id의 유닛의 타입이 서로 다릅니다. m_NKM_UNIT_TYPE:{UnitType} unitType:{nKM_UNIT_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 146);
		}
		IReadOnlyList<NKMRecallUnitExchangeTemplet> unitGroupTemplet = NKMRecallUnitExchangeTemplet.GetUnitGroupTemplet(UnitExchangeGroupId);
		if (unitGroupTemplet == null)
		{
			NKMTempletError.Add($"[NKMRecallTemplet] UnitExchangeGrupId가 올바르지 않습니다. 해당 아이디가 NKMRecallUnitExchangeTemplet에 존재하지 않습니다. index:{Index}, unitExchangeGroupId:{UnitExchangeGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 153);
			return;
		}
		switch (nKM_UNIT_TYPE)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			if (UnitExchangeGroupId == 0)
			{
				NKMTempletError.Add($"[NKMRecallTemplet] UnitExchangeGroupId가 0일 수 없습니다. index: {Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 161);
			}
			break;
		case NKM_UNIT_TYPE.NUT_SHIP:
		{
			NKMShipBuildTemplet shipBuildTemplet = NKMShipManager.GetShipBuildTemplet(UnitId);
			if (shipBuildTemplet != null)
			{
				if (shipBuildTemplet.UpgradeMaterialList.Count > 0)
				{
					NKMTempletError.Add($"[NKMRecallTemplet] 함선인 경우 개장 전 최초 아이디만 입력 가능합니다. unitId:{UnitId}, index:{Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 171);
					return;
				}
				break;
			}
			Log.Warn($"[NKMRecallTemplet] 함선의 아이디가 ShipBuildTemplet에 존재하지 않습니다. unitId:{UnitId}, index:{Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 177);
			return;
		}
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			if (unitGroupTemplet.Any((NKMRecallUnitExchangeTemplet e) => e.UnitId == UnitId))
			{
				NKMTempletError.Add($"[NKMRecallTemplet] UnitExchangeGrup에 자신이 포함되어 있습니다. index:{Index}, unitId:{UnitId} unitExchangeGroupId:{UnitExchangeGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 186);
			}
			break;
		default:
			NKMTempletError.Add($"[NKMRecallTemplet] 리콜할 수 있는 유닛은 일반 유닛, 함선, 오퍼레이터만 가능합니다. unitId:{UnitId}, index:{Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 191);
			return;
		}
		if (unitGroupTemplet.Any((NKMRecallUnitExchangeTemplet e) => e.UnitType != UnitType))
		{
			NKMTempletError.Add($"[NKMRecallTemplet] UnitExchangeGrup에 UnitType 값이 다른 항목이 존재합니다. index:{Index}, unitId:{UnitId} unitExchangeGroupId:{UnitExchangeGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 198);
		}
		List<int> list = unitGroupTemplet.Select((NKMRecallUnitExchangeTemplet e) => e.UnitId).ToList();
		if (list.Count <= 0)
		{
			NKMTempletError.Add($"[NKMRecallTemplet] UnitExchangeGrupId에 해당하는 교환 대상 목록의 개수가 비정상입니다. index:{Index}, unitExchangeGroupId:{UnitExchangeGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 205);
			return;
		}
		foreach (int item in list)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(item);
			if (nKMUnitTempletBase == null)
			{
				NKMTempletError.Add($"[NKMRecallTemplet] 교환 대상 유닛의 정보가 없습니다. index:{Index} unitExchangeGroupId:{UnitExchangeGroupId} exchangeUnitId:{item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 215);
			}
			else if (nKMUnitTempletBase.m_NKM_UNIT_TYPE != UnitType)
			{
				NKMTempletError.Add($"[NKMRecallTemplet] 리콜 대상과 교환 대상의 type가 다릅니다. index:{Index} templetUnitType:{UnitType} unitExchangeGroupId:{UnitExchangeGroupId} exchangeUnitId:{item} unitType:{nKMUnitTempletBase.m_NKM_UNIT_TYPE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 222);
			}
			else if (nKMUnitTempletBase.m_bMonster)
			{
				NKMTempletError.Add($"[NKMRecallTemplet] 리콜의 교환 대상에 몬스터 타입이 있습니다. index:{Index} unitExchangeGroupId:{UnitExchangeGroupId} exchangeUnitId:{item}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 228);
			}
		}
		if (RecallItemCondition != Recall_Condition.None)
		{
			if (UnitType != NKM_UNIT_TYPE.NUT_NORMAL)
			{
				NKMTempletError.Add($"[NKMRecallTemplet] 보상 지정 에러. 유닛 외에 보상 관련 정보를 가질 수 없음. index:{Key} recallItemCondition:{RecallItemCondition} unitType:{UnitType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 237);
			}
			else if (RecallItemType != NKM_REWARD_TYPE.RT_MISC)
			{
				NKMTempletError.Add($"[NKMRecallTemplet] 보상 타입 정보가 올바르지 않음. index:{Key} RecallItemType:{RecallItemType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 241);
			}
			else if (!NKMRewardTemplet.IsValidReward(RecallItemType, RecallItemID))
			{
				NKMTempletError.Add($"[NKMRecallTemplet] 보상 정보가 올바르지 않음. index:{Key} RecallItemType:{RecallItemType} RecallItemID:{RecallItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 245);
			}
			if (RecallItemQuantity <= 0)
			{
				NKMTempletError.Add($"[NKMRecallTemplet] 보상 개수가 올바르지 않음. index:{Key} RecallItemType:{RecallItemType} RecallItemID:{RecallItemID} RecallItemQuantity:{RecallItemQuantity}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/Recall/NKMRecallTemplet.cs", 250);
			}
		}
	}

	public static void Drop()
	{
		recallTempletMap.Clear();
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
