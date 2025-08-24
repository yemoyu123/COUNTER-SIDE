using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public sealed class NKMDivePlayerBase : ISerializable
{
	private NKMDivePlayerState state;

	private int prevSlotSetIndex;

	private int prevSlotIndex;

	private int slotSetIndex = -1;

	private int slotIndex;

	private int distance;

	private int leaderDeckIndex;

	private int reservedDungeonID;

	private int reservedDeckIndex = -1;

	private List<int> artifacts = new List<int>();

	private List<int> reservedArtifacts = new List<int>();

	public NKMDivePlayerState State
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

	public int PrevSlotSetIndex
	{
		get
		{
			return prevSlotSetIndex;
		}
		set
		{
			prevSlotSetIndex = value;
		}
	}

	public int PrevSlotIndex
	{
		get
		{
			return prevSlotIndex;
		}
		set
		{
			prevSlotIndex = value;
		}
	}

	public int SlotSetIndex
	{
		get
		{
			return slotSetIndex;
		}
		set
		{
			slotSetIndex = value;
		}
	}

	public int SlotIndex
	{
		get
		{
			return slotIndex;
		}
		set
		{
			slotIndex = value;
		}
	}

	public int Distance
	{
		get
		{
			return distance;
		}
		set
		{
			distance = value;
		}
	}

	public int LeaderDeckIndex
	{
		get
		{
			return leaderDeckIndex;
		}
		set
		{
			leaderDeckIndex = value;
		}
	}

	public int ReservedDungeonID
	{
		get
		{
			return reservedDungeonID;
		}
		set
		{
			reservedDungeonID = value;
		}
	}

	public int ReservedDeckIndex
	{
		get
		{
			return reservedDeckIndex;
		}
		set
		{
			reservedDeckIndex = value;
		}
	}

	public List<int> Artifacts
	{
		get
		{
			return artifacts;
		}
		set
		{
			artifacts = value;
		}
	}

	public List<int> ReservedArtifacts
	{
		get
		{
			return reservedArtifacts;
		}
		set
		{
			reservedArtifacts = value;
		}
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref state);
		stream.PutOrGet(ref prevSlotSetIndex);
		stream.PutOrGet(ref prevSlotIndex);
		stream.PutOrGet(ref slotSetIndex);
		stream.PutOrGet(ref slotIndex);
		stream.PutOrGet(ref distance);
		stream.PutOrGet(ref leaderDeckIndex);
		stream.PutOrGet(ref reservedDungeonID);
		stream.PutOrGet(ref reservedDeckIndex);
		stream.PutOrGet(ref artifacts);
		stream.PutOrGet(ref reservedArtifacts);
	}

	public void DeepCopyFromSource(NKMDivePlayerBase source)
	{
		state = source.state;
		prevSlotSetIndex = source.prevSlotSetIndex;
		prevSlotIndex = source.prevSlotIndex;
		slotSetIndex = source.slotSetIndex;
		slotIndex = source.slotIndex;
		distance = source.distance;
		leaderDeckIndex = source.leaderDeckIndex;
		reservedDungeonID = source.reservedDungeonID;
		reservedDeckIndex = source.reservedDeckIndex;
		artifacts.Clear();
		artifacts.AddRange(source.artifacts);
		reservedArtifacts.Clear();
		reservedArtifacts.AddRange(source.reservedArtifacts);
	}
}
