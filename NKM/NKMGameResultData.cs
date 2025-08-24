using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public sealed class NKMGameResultData : ISerializable
{
	public Dictionary<short, NKMShipResultData> m_dicShipResult = new Dictionary<short, NKMShipResultData>();

	public NKM_TEAM_TYPE m_WinTeam;

	public float m_GamePlayTime;

	public float m_TotalDamage;

	public float m_BossRemainHp;

	public float m_BossInitHp;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref m_dicShipResult);
		stream.PutOrGetEnum(ref m_WinTeam);
		stream.PutOrGet(ref m_GamePlayTime);
		stream.PutOrGet(ref m_TotalDamage);
		stream.PutOrGet(ref m_BossRemainHp);
		stream.PutOrGet(ref m_BossInitHp);
	}

	public float GetShipResultHpByTeamData(NKMGameTeamData teamData)
	{
		float num = 0f;
		if (teamData.m_MainShip != null)
		{
			for (int i = 0; i < teamData.m_MainShip.m_listGameUnitUID.Count; i++)
			{
				short key = teamData.m_MainShip.m_listGameUnitUID[i];
				if (m_dicShipResult.TryGetValue(key, out var value))
				{
					num += value.m_fHP;
				}
			}
		}
		return num;
	}
}
