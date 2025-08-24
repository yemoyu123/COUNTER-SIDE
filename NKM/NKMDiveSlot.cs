using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public sealed class NKMDiveSlot : ISerializable
{
	private NKM_DIVE_SECTOR_TYPE sectorType;

	private NKM_DIVE_EVENT_TYPE eventType;

	private int eventValue;

	private int artifactEventRate;

	private int artifactRewardGroup;

	public NKM_DIVE_SECTOR_TYPE SectorType => sectorType;

	public NKM_DIVE_EVENT_TYPE EventType => eventType;

	public int EventValue => eventValue;

	public int ArtifactEventRate => artifactEventRate;

	public int ArtifactRewardGroup => artifactRewardGroup;

	public NKMDiveSlot()
	{
	}

	public NKMDiveSlot(NKM_DIVE_SECTOR_TYPE sectorType, NKM_DIVE_EVENT_TYPE eventType, int eventValue, int artifactEventRate, int artifactRewardGroup)
	{
		this.sectorType = sectorType;
		this.eventType = eventType;
		this.eventValue = eventValue;
		this.artifactEventRate = artifactEventRate;
		this.artifactRewardGroup = artifactRewardGroup;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref sectorType);
		stream.PutOrGetEnum(ref eventType);
		stream.PutOrGet(ref eventValue);
	}

	public void UpdateEvent(NKM_DIVE_EVENT_TYPE type, int value)
	{
		eventType = type;
		eventValue = value;
	}

	public NKMDiveSlot Clone()
	{
		return new NKMDiveSlot(sectorType, eventType, eventValue, artifactEventRate, artifactRewardGroup);
	}
}
