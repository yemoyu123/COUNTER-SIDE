using System;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMPotentialSocketTemplet
{
	private readonly NKMPotentialOptionTemplet owner;

	private float minStatValue;

	private float maxStatValue;

	public int SocketNumber { get; private set; }

	public float MinStat => minStatValue;

	public float MaxStat => maxStatValue;

	public bool IsPrecentStat => NKMUnitStatManager.IsPercentStat(owner.StatType);

	public NKMPotentialSocketTemplet(NKMPotentialOptionTemplet owner, int socketNumber)
	{
		this.owner = owner;
		SocketNumber = socketNumber;
	}

	public static NKMPotentialSocketTemplet Create(NKMPotentialOptionTemplet owner, int socketNumber, NKMLua lua)
	{
		NKMPotentialSocketTemplet nKMPotentialSocketTemplet = new NKMPotentialSocketTemplet(owner, socketNumber);
		lua.GetData($"Socket{socketNumber}_MinStat", ref nKMPotentialSocketTemplet.minStatValue);
		lua.GetData($"Socket{socketNumber}_MaxStat", ref nKMPotentialSocketTemplet.maxStatValue);
		float rValue = 0f;
		float rValue2 = 0f;
		lua.GetData($"Socket{socketNumber}_MinStatRate", ref rValue);
		lua.GetData($"Socket{socketNumber}_MaxStatRate", ref rValue2);
		if (rValue != 0f || rValue2 != 0f)
		{
			if (!NKMUnitStatManager.IsMainFactorStat(owner.StatType))
			{
				Log.ErrorAndExit($"[NKMPotentialSocketTemplet] non-factor type stat {owner.StatType} has factor value!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionTemplet.cs", 42);
			}
			nKMPotentialSocketTemplet.minStatValue = rValue;
			nKMPotentialSocketTemplet.maxStatValue = rValue2;
		}
		return nKMPotentialSocketTemplet;
	}

	public float CalcStatValue(int precision)
	{
		float num = (float)precision / 100f;
		float num2 = ((!(maxStatValue < 0f) || !(minStatValue < 0f)) ? ((maxStatValue - minStatValue) * num + minStatValue) : ((minStatValue - maxStatValue) * num + maxStatValue));
		if (NKMUnitStatManager.IsPercentStat(owner.StatType))
		{
			return (float)Math.Truncate((float)Math.Round(num2 * 10000f)) / 10000f;
		}
		return (float)Math.Truncate(num2);
	}

	public float CalcStat(int precision)
	{
		return CalcStatValue(precision);
	}

	public void Validate(int optionKey)
	{
		string text = $"[PotentialSocket:{optionKey}]";
		if (minStatValue > maxStatValue)
		{
			NKMTempletError.Add($"{text} \ufffd\ufffdġ \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. socketNumber:{SocketNumber} minStat:{minStatValue} maxStat:{maxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionTemplet.cs", 84);
		}
		if (minStatValue * maxStatValue < 0f)
		{
			NKMTempletError.Add($"{text} \ufffd\ufffdġ \ufffd\ufffd\ufffd\ufffd minmax\ufffd\ufffd \ufffd\ufffdȣ\ufffd\ufffd \ufffdٸ\ufffd.. socketNumber:{SocketNumber} minStat:{minStatValue} maxStat:{maxStatValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Item/NKMPotentialOptionTemplet.cs", 89);
		}
	}
}
