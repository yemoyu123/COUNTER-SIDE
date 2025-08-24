using Cs.Protocol;

namespace NKM;

public sealed class NKMNXToyData : ISerializable
{
	public long m_Npsn;

	public string m_NpToken;

	public string m_NpaCode;

	public long m_NexonSn;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_Npsn);
		stream.PutOrGet(ref m_NpToken);
		stream.PutOrGet(ref m_NpaCode);
		stream.PutOrGet(ref m_NexonSn);
	}
}
