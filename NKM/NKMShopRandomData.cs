using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMShopRandomData : ISerializable
{
	public Dictionary<int, NKMShopRandomListData> datas = new Dictionary<int, NKMShopRandomListData>();

	public long nextRefreshDate;

	public int refreshCount;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref datas);
		stream.PutOrGet(ref nextRefreshDate);
		stream.PutOrGet(ref refreshCount);
	}
}
