using Cs.Protocol;

namespace NKM;

public class NKMShipCmdModule : ISerializable
{
	public NKMShipCmdSlot[] slots = new NKMShipCmdSlot[2];

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref slots);
	}
}
