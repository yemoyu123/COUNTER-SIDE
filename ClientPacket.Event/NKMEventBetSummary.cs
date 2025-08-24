using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMEventBetSummary : ISerializable
{
	public NKMEventBetResult betResult = new NKMEventBetResult();

	public NKMEventBetPrivate betPrivate = new NKMEventBetPrivate();

	public List<NKMEventBetPrivateResult> betPrivateResult = new List<NKMEventBetPrivateResult>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref betResult);
		stream.PutOrGet(ref betPrivate);
		stream.PutOrGet(ref betPrivateResult);
	}
}
