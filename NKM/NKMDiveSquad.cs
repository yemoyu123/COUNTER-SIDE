using System;
using Cs.Protocol;

namespace NKM;

public sealed class NKMDiveSquad : ISerializable
{
	private const int DEFAULT_SQUAD_SUPPLY = 2;

	private NKMDiveSquadState state;

	private int deckIndex;

	private float curHp;

	private float maxHp;

	private int supply;

	public NKMDiveSquadState State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	public int DeckIndex
	{
		get
		{
			return deckIndex;
		}
		set
		{
			deckIndex = value;
		}
	}

	public float CurHp
	{
		get
		{
			return curHp;
		}
		set
		{
			curHp = value;
		}
	}

	public float MaxHp
	{
		get
		{
			return maxHp;
		}
		set
		{
			maxHp = value;
		}
	}

	public int Supply
	{
		get
		{
			return supply;
		}
		set
		{
			supply = value;
		}
	}

	public NKMDiveSquad()
	{
	}

	public NKMDiveSquad(int deckIndex, float hp)
	{
		this.deckIndex = deckIndex;
		curHp = hp;
		maxHp = hp;
		supply = 2;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref state);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref curHp);
		stream.PutOrGet(ref maxHp);
		stream.PutOrGet(ref supply);
	}

	public void DeepCopyFromSource(NKMDiveSquad source)
	{
		state = source.state;
		deckIndex = source.deckIndex;
		curHp = source.curHp;
		maxHp = source.maxHp;
		supply = source.supply;
	}

	public bool isDead()
	{
		return State == NKMDiveSquadState.Dead;
	}

	public void ChangeHp(float hp, float minHp)
	{
		curHp += hp;
		curHp = Math.Max(minHp, curHp);
		curHp = Math.Min(curHp, maxHp);
		if (curHp <= 0f)
		{
			state = NKMDiveSquadState.Dead;
		}
	}

	public void ChangeHpByPercentage(float percentage)
	{
		float hp = MaxHp * (percentage / 100f);
		float minHp = MaxHp * 0.01f;
		ChangeHp(hp, minHp);
	}

	public void ChangeSupply(int supply)
	{
		this.supply += supply;
		this.supply = Math.Max(0, this.supply);
		this.supply = Math.Min(this.supply, 2);
	}

	public void Kill()
	{
		curHp = 0f;
		state = NKMDiveSquadState.Dead;
	}
}
