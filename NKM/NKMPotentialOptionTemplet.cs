using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMPotentialOptionTemplet
{
	private static readonly Dictionary<int, NKMPotentialOptionTemplet> options = new Dictionary<int, NKMPotentialOptionTemplet>();

	public int groupId;

	public int optionKey;

	public NKM_STAT_TYPE StatType = NKM_STAT_TYPE.NST_RANDOM;

	public int precisionWeightId;

	public int rerollPrecisionWeightId;

	public NKMPotentialSocketTemplet[] sockets = new NKMPotentialSocketTemplet[3];

	public string DebugName => $"[{optionKey}|{StatType}]";

	public static NKMPotentialOptionTemplet LoadFromLUA(NKMLua lua)
	{
		NKMPotentialOptionTemplet nKMPotentialOptionTemplet = new NKMPotentialOptionTemplet();
		nKMPotentialOptionTemplet.groupId = lua.GetInt32("m_PotentialOptionGroupID");
		nKMPotentialOptionTemplet.optionKey = lua.GetInt32("OptionKey");
		nKMPotentialOptionTemplet.StatType = lua.GetEnum<NKM_STAT_TYPE>("Socket1_StatType");
		float rValue = 0f;
		if (lua.GetData("Socket1_MinStatRate", ref rValue))
		{
			NKM_STAT_TYPE factorStat = NKMUnitStatManager.GetFactorStat(nKMPotentialOptionTemplet.StatType);
			if (factorStat == NKM_STAT_TYPE.NST_END)
			{
				NKMTempletError.Add("[PotentialOption] non-factor stat has statRate:" + nKMPotentialOptionTemplet.DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionTemplet.cs", 121);
				return null;
			}
			nKMPotentialOptionTemplet.StatType = factorStat;
		}
		nKMPotentialOptionTemplet.precisionWeightId = lua.GetInt32("FirstPrecisionWeightId", nKMPotentialOptionTemplet.precisionWeightId);
		nKMPotentialOptionTemplet.rerollPrecisionWeightId = lua.GetInt32("PrecisionWeightId");
		for (int i = 0; i < 3; i++)
		{
			int socketNumber = i + 1;
			nKMPotentialOptionTemplet.sockets[i] = NKMPotentialSocketTemplet.Create(nKMPotentialOptionTemplet, socketNumber, lua);
		}
		if (options.ContainsKey(nKMPotentialOptionTemplet.optionKey))
		{
			NKMTempletError.Add($"[PotentialOption] duplicated optionkey:{nKMPotentialOptionTemplet.optionKey}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionTemplet.cs", 139);
			return null;
		}
		options.Add(nKMPotentialOptionTemplet.optionKey, nKMPotentialOptionTemplet);
		return nKMPotentialOptionTemplet;
	}

	public static void Drop()
	{
		options.Clear();
	}

	public static NKMPotentialOptionTemplet Find(int optionKey)
	{
		options.TryGetValue(optionKey, out var value);
		return value;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (NKMPotentialOptionGroupTemplet.EnableByTag)
		{
			if (StatType <= NKM_STAT_TYPE.NST_RANDOM || StatType >= NKM_STAT_TYPE.NST_END)
			{
				NKMTempletError.Add($"[Potential Option] Validate StatType:{StatType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionTemplet.cs", 172);
			}
			NKMPotentialSocketTemplet[] array = sockets;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Validate(optionKey);
			}
			if (rerollPrecisionWeightId == 0)
			{
				NKMTempletError.Add($"[PotentialOption :{optionKey}] weight id is zero reroll : {rerollPrecisionWeightId} first : {precisionWeightId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionTemplet.cs", 182);
			}
		}
	}

	public static bool CanOpenSocket(NKMEquipItemData equipItemData, int socketIndex)
	{
		if (socketIndex < 0 || socketIndex >= 3)
		{
			return false;
		}
		return socketIndex switch
		{
			0 => equipItemData.m_EnchantLevel >= 2, 
			1 => equipItemData.m_EnchantLevel >= 5, 
			2 => equipItemData.m_EnchantLevel >= 7, 
			_ => false, 
		};
	}

	public static bool CanChangeSocketPrecision(NKMEquipItemData equipItemData, int socketIndex)
	{
		if (socketIndex < 0)
		{
			return false;
		}
		if (equipItemData.m_EnchantLevel < 7)
		{
			return false;
		}
		if (equipItemData.potentialOptions.Count == 0)
		{
			return false;
		}
		if (equipItemData.potentialOptions[0].sockets == null)
		{
			return false;
		}
		NKMPotentialOption.SocketData[] array = equipItemData.potentialOptions[0].sockets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null)
			{
				return false;
			}
		}
		if (equipItemData.potentialOptions.Count == 2)
		{
			if (equipItemData.potentialOptions[1].sockets == null)
			{
				return false;
			}
			array = equipItemData.potentialOptions[1].sockets;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					return false;
				}
			}
		}
		return true;
	}

	public NKMPotentialOption GeneratePotentialOption(IReadOnlyList<int> precisions)
	{
		if (precisions.Count != 3)
		{
			Log.Error($"[PotentialOption] invalid precision count:{precisions.Count} optionKey:{optionKey}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionTemplet.cs", 256);
			return null;
		}
		if (precisions[0] < 0)
		{
			Log.Error($"[PotentialOption] invalid 1st precision value:{precisions[0]} optionKey:{optionKey}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionTemplet.cs", 262);
			return null;
		}
		NKMPotentialOption nKMPotentialOption = new NKMPotentialOption
		{
			optionKey = optionKey,
			statType = StatType
		};
		foreach (int item in Enumerable.Range(0, sockets.Length))
		{
			int num = precisions[item];
			if (num < 0)
			{
				break;
			}
			nKMPotentialOption.sockets[item] = new NKMPotentialOption.SocketData
			{
				precision = num,
				statValue = sockets[item].CalcStatValue(num)
			};
		}
		return nKMPotentialOption;
	}

	public NKMPotentialOption.SocketData GenerateSocket(int index, int precision)
	{
		return new NKMPotentialOption.SocketData
		{
			precision = precision,
			statValue = sockets[index].CalcStatValue(precision)
		};
	}
}
