using System;
using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public class NKMCompanyBuffData : ISerializable
{
	private int companyBuffId;

	private long expireTicks;

	public int Id => companyBuffId;

	public long ExpireTicks => expireTicks;

	public DateTime ExpireDate => new DateTime(expireTicks, DateTimeKind.Utc);

	public NKMCompanyBuffData()
	{
	}

	public NKMCompanyBuffData(NKMCompanyBuffTemplet templet, DateTime current)
	{
		companyBuffId = templet.m_CompanyBuffID;
		expireTicks = current.AddMinutes(templet.m_CompanyBuffTime).Ticks;
	}

	public NKMCompanyBuffData(int companyBuffId, long expireTicks)
	{
		this.companyBuffId = companyBuffId;
		this.expireTicks = expireTicks;
	}

	public void UpdateExpireTicksAsMinutes(int minutes)
	{
		expireTicks = new DateTime(ExpireTicks, DateTimeKind.Utc).AddMinutes(minutes).Ticks;
	}

	public void SetExpireTicks(long expireTicks)
	{
		this.expireTicks = expireTicks;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref companyBuffId);
		stream.PutOrGet(ref expireTicks);
	}

	public void SetExpireTime(DateTime expireTime)
	{
		expireTicks = expireTime.Ticks;
	}
}
