using System;
using System.Runtime.Serialization;
using Cs.Protocol;

namespace NKM;

[DataContract]
public class NKMItemMiscData : Cs.Protocol.ISerializable
{
	public int BonusRatio;

	private int m_ItemMiscID;

	private long m_CountFree;

	private long m_CountPaid;

	private DateTime m_RegDate;

	[DataMember]
	public int ItemID
	{
		get
		{
			return m_ItemMiscID;
		}
		set
		{
			m_ItemMiscID = value;
		}
	}

	[DataMember]
	public long CountFree
	{
		get
		{
			return m_CountFree;
		}
		set
		{
			m_CountFree = value;
		}
	}

	[DataMember]
	public long CountPaid
	{
		get
		{
			return m_CountPaid;
		}
		set
		{
			m_CountPaid = value;
		}
	}

	[DataMember]
	public long TotalCount => CountFree + CountPaid;

	[DataMember]
	public DateTime RegDate
	{
		get
		{
			return m_RegDate;
		}
		set
		{
			m_RegDate = value;
		}
	}

	public NKM_ITEM_PAYMENT_TYPE PaymentType
	{
		get
		{
			if (m_CountFree <= 0 || m_CountPaid <= 0)
			{
				if (m_CountPaid <= 0)
				{
					return NKM_ITEM_PAYMENT_TYPE.NIPT_FREE;
				}
				return NKM_ITEM_PAYMENT_TYPE.NIPT_PAID;
			}
			return NKM_ITEM_PAYMENT_TYPE.NIPT_BOTH;
		}
	}

	public NKMItemMiscData()
	{
	}

	public NKMItemMiscData(int ItemID, long CountFree, long CountPaid = 0L, int bonusRatio = 0, DateTime regDate = default(DateTime))
	{
		m_ItemMiscID = ItemID;
		m_CountFree = CountFree;
		m_CountPaid = CountPaid;
		BonusRatio = bonusRatio;
		m_RegDate = regDate;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_ItemMiscID);
		stream.PutOrGet(ref m_CountFree);
		stream.PutOrGet(ref m_CountPaid);
		stream.PutOrGet(ref BonusRatio);
		stream.PutOrGet(ref m_RegDate);
	}

	public NKMItemMiscTemplet GetTemplet()
	{
		return NKMItemManager.GetItemMiscTempletByID(m_ItemMiscID);
	}
}
