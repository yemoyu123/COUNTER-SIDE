using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMGameSyncDataPack : ISerializable
{
	public List<NKMGameSyncData_Base> m_listGameSyncData = new List<NKMGameSyncData_Base>();

	public virtual void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_listGameSyncData);
	}
}
