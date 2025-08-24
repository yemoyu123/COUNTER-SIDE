using System;
using System.Collections.Generic;
using Cs.Protocol;
using NKM.Templet;

namespace NKM;

public class NKMPostData : ISerializable
{
	public int postId;

	public long postIndex;

	public string title;

	public string contents;

	public DateTime sendDate;

	public DateTime expirationDate;

	public List<NKMRewardInfo> items = new List<NKMRewardInfo>();

	public NKMPostTemplet PostTemplet => NKMPostTemplet.Find(postId);

	public NKMPostData()
	{
	}

	public NKMPostData(int postId)
	{
		this.postId = postId;
	}

	public NKMPostData(int postId, string title, string contents, DateTime sendDate, DateTime expireDate, long postIndex)
	{
		this.postId = postId;
		this.title = title;
		this.contents = contents;
		expirationDate = expireDate;
		this.sendDate = sendDate;
		this.postIndex = postIndex;
	}

	public NKMPostData(int postId, string title, string contents, DateTime sendDate, TimeSpan lifeTime, long postIndex)
	{
		this.postId = postId;
		this.title = title;
		this.contents = contents;
		this.sendDate = sendDate;
		expirationDate = sendDate + lifeTime;
		this.postIndex = postIndex;
	}

	public void InsertItem(NKMRewardInfo item)
	{
		items.Add(item);
	}

	public void InsertItem(NKM_REWARD_TYPE itemType, int itemId, int count, NKM_ITEM_PAYMENT_TYPE paymentType)
	{
		items.Add(new NKMRewardInfo
		{
			rewardType = itemType,
			paymentType = paymentType,
			ID = itemId,
			Count = count
		});
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref postId);
		stream.PutOrGet(ref postIndex);
		stream.PutOrGet(ref title);
		stream.PutOrGet(ref contents);
		stream.PutOrGet(ref sendDate);
		stream.PutOrGet(ref items);
		stream.PutOrGet(ref expirationDate);
	}
}
