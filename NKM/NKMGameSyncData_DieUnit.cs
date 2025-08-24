using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMGameSyncData_DieUnit : ISerializable
{
	public HashSet<short> m_DieGameUnitUID = new HashSet<short>();

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_DieGameUnitUID);
	}
}
