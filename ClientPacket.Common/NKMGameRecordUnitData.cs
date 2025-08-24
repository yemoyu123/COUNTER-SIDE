using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMGameRecordUnitData : ISerializable
{
	public int unitId;

	public string changeUnitName;

	public int unitLevel;

	public bool isSummonee;

	public bool isAssistUnit;

	public bool isLeader;

	public NKM_TEAM_TYPE teamType;

	public float recordGiveDamage;

	public float recordTakeDamage;

	public float recordHeal;

	public int recordSummonCount;

	public int recordDieCount;

	public int recordKillCount;

	public int playtime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitId);
		stream.PutOrGet(ref changeUnitName);
		stream.PutOrGet(ref unitLevel);
		stream.PutOrGet(ref isSummonee);
		stream.PutOrGet(ref isAssistUnit);
		stream.PutOrGet(ref isLeader);
		stream.PutOrGetEnum(ref teamType);
		stream.PutOrGet(ref recordGiveDamage);
		stream.PutOrGet(ref recordTakeDamage);
		stream.PutOrGet(ref recordHeal);
		stream.PutOrGet(ref recordSummonCount);
		stream.PutOrGet(ref recordDieCount);
		stream.PutOrGet(ref recordKillCount);
		stream.PutOrGet(ref playtime);
	}
}
