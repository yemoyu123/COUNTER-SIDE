using System;
using Cs.Protocol;

namespace NKM;

public struct NKMDeckIndex : ISerializable, IEquatable<NKMDeckIndex>
{
	public static readonly NKMDeckIndex None = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE);

	public NKM_DECK_TYPE m_eDeckType;

	public byte m_iIndex;

	public NKMDeckIndex(NKM_DECK_TYPE eDeckType)
	{
		m_eDeckType = eDeckType;
		m_iIndex = 0;
	}

	public NKMDeckIndex(NKM_DECK_TYPE eDeckType, int Index)
	{
		m_eDeckType = ((Index >= 0) ? eDeckType : NKM_DECK_TYPE.NDT_NONE);
		m_iIndex = (byte)Index;
	}

	public bool Compare(NKMDeckIndex rhs)
	{
		if (m_eDeckType == rhs.m_eDeckType)
		{
			return m_iIndex == rhs.m_iIndex;
		}
		return false;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref m_eDeckType);
		stream.PutOrGet(ref m_iIndex);
	}

	public override string ToString()
	{
		return $"DeckIndex {m_eDeckType.ToString()} {m_iIndex}";
	}

	public static bool operator ==(NKMDeckIndex lhs, NKMDeckIndex rhs)
	{
		if (lhs.m_eDeckType == rhs.m_eDeckType)
		{
			return lhs.m_iIndex == rhs.m_iIndex;
		}
		return false;
	}

	public static bool operator !=(NKMDeckIndex lhs, NKMDeckIndex rhs)
	{
		if (lhs.m_eDeckType == rhs.m_eDeckType)
		{
			return lhs.m_iIndex != rhs.m_iIndex;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is NKMDeckIndex nKMDeckIndex))
		{
			return false;
		}
		if (m_eDeckType == nKMDeckIndex.m_eDeckType)
		{
			return m_iIndex == nKMDeckIndex.m_iIndex;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (m_eDeckType, m_iIndex).GetHashCode();
	}

	public bool Equals(NKMDeckIndex other)
	{
		if (m_eDeckType == other.m_eDeckType)
		{
			return m_iIndex == other.m_iIndex;
		}
		return false;
	}
}
