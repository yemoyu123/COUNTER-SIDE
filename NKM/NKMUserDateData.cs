using System;
using Cs.Protocol;

namespace NKM;

public class NKMUserDateData : ISerializable
{
	public DateTime m_RegisterTime;

	public DateTime m_LastLogInTime;

	public DateTime m_LastLogOutTime;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_RegisterTime);
		stream.PutOrGet(ref m_LastLogInTime);
		stream.PutOrGet(ref m_LastLogOutTime);
	}
}
