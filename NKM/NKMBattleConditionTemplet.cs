using System.Collections.Generic;
using Cs.Logging;
using NKC;
using NKM.Game;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public sealed class NKMBattleConditionTemplet : INKMTemplet
{
	public enum USE_CONTENT_TYPE
	{
		UCT_BATTLE_CONDITION,
		UCT_DIVE_ARTIFACT,
		UCT_GUILD_DUNGEON_ARTIFACT,
		UCT_FIERCE_PENALTY
	}

	private int m_BCondID;

	private string m_BCondStrID = "";

	private string m_BCondName = "";

	private string m_BCondDescription = "";

	private USE_CONTENT_TYPE m_UseContentsType;

	private string m_BCondInfoIcon = "";

	private string m_BCondWarfareIcon = "";

	private string m_BCondIngameIcon = "";

	private string m_BCondMapStrID = "";

	private readonly HashSet<string> m_hashAffectTeamUpID = new HashSet<string>();

	private bool m_bAffectSHIP;

	private bool m_bAffectCOUNTER;

	private bool m_bAffectSOLDIER;

	private bool m_bAffectMECHANIC;

	private bool m_bAffectCORRUPT;

	private bool m_bAffectNormal;

	private bool m_bAffectAwaken;

	private NKM_UNIT_ROLE_TYPE m_AffectUnitRole;

	private bool m_bHitLand;

	private bool m_bHitAir;

	private HashSet<int> m_hashAffectUnitID = new HashSet<int>();

	private HashSet<int> m_hashIgnoreUnitID = new HashSet<int>();

	private float m_BoostResource;

	private readonly HashSet<string> m_listAllyBCondUnitStrID = new HashSet<string>();

	private readonly HashSet<string> m_listEnemyBCondUnitStrID = new HashSet<string>();

	private readonly Dictionary<string, int> m_dicAllyBuff = new Dictionary<string, int>();

	private readonly Dictionary<string, int> m_dicEnemyBuff = new Dictionary<string, int>();

	private readonly int maxBattleConditionBuffCount = 5;

	private float m_fActivateTimeLeft;

	public bool m_bHide;

	public List<string> m_EventCondition;

	public int Key => m_BCondID;

	public string DebugName => $"[{BattleCondID}]{BattleCondStrID}";

	public int BattleCondID => m_BCondID;

	public string BattleCondStrID => m_BCondStrID;

	public string BattleCondName => m_BCondName;

	public string BattleCondDesc => m_BCondDescription;

	public USE_CONTENT_TYPE UseContentsType => m_UseContentsType;

	public string BattleCondInfoIcon => m_BCondInfoIcon;

	public string BattleCondWFIcon => m_BCondWarfareIcon;

	public string BattleCondIngameIcon => m_BCondIngameIcon;

	public string BattleCondMapStrID => m_BCondMapStrID;

	public HashSet<string> AffectTeamUpID => m_hashAffectTeamUpID;

	public bool AffectSHIP => m_bAffectSHIP;

	public bool AffectCOUNTER => m_bAffectCOUNTER;

	public bool AffectSOLDIER => m_bAffectSOLDIER;

	public bool AffectMECHANIC => m_bAffectMECHANIC;

	public bool AffectCORRUPT => m_bAffectCORRUPT;

	public bool AffectNormal => m_bAffectNormal;

	public bool AffectAwaken => m_bAffectAwaken;

	public NKM_UNIT_ROLE_TYPE AffectUnitRole => m_AffectUnitRole;

	public bool HitLand => m_bHitLand;

	public bool HitAir => m_bHitAir;

	public HashSet<int> hashAffectUnitID => m_hashAffectUnitID;

	public HashSet<int> hashIgnoreUnitID => m_hashIgnoreUnitID;

	public float BoostResource => m_BoostResource;

	public HashSet<string> AllyBCondUnitStrIDList => m_listAllyBCondUnitStrID;

	public HashSet<string> EnemyBCondUnitStrIDList => m_listEnemyBCondUnitStrID;

	public float ActiveTimeLeft => m_fActivateTimeLeft;

	public string BattleCondName_Translated => NKCStringTable.GetString(BattleCondName);

	public string BattleCondDesc_Translated => NKCStringTable.GetString(BattleCondDesc);

	public static NKMBattleConditionTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMBattleConditionTemplet nKMBattleConditionTemplet = new NKMBattleConditionTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_BCondID", ref nKMBattleConditionTemplet.m_BCondID);
		flag &= cNKMLua.GetData("m_BCondStrID", ref nKMBattleConditionTemplet.m_BCondStrID);
		flag &= cNKMLua.GetData("m_BCondName", ref nKMBattleConditionTemplet.m_BCondName);
		flag &= cNKMLua.GetData("m_BCondDescription", ref nKMBattleConditionTemplet.m_BCondDescription);
		flag &= cNKMLua.GetData("m_UseContentsType", ref nKMBattleConditionTemplet.m_UseContentsType);
		cNKMLua.GetData("m_BCondInfoIcon", ref nKMBattleConditionTemplet.m_BCondInfoIcon);
		cNKMLua.GetData("m_BCondWarfareIcon", ref nKMBattleConditionTemplet.m_BCondWarfareIcon);
		cNKMLua.GetData("m_BCondIngameIcon", ref nKMBattleConditionTemplet.m_BCondIngameIcon);
		cNKMLua.GetData("m_BCondMapStrID", ref nKMBattleConditionTemplet.m_BCondMapStrID);
		cNKMLua.GetData("m_hashAffectTeamUpID", nKMBattleConditionTemplet.m_hashAffectTeamUpID);
		flag &= cNKMLua.GetData("m_bAffectSHIP", ref nKMBattleConditionTemplet.m_bAffectSHIP);
		flag &= cNKMLua.GetData("m_bAffectCOUNTER", ref nKMBattleConditionTemplet.m_bAffectCOUNTER);
		flag &= cNKMLua.GetData("m_bAffectSOLDIER", ref nKMBattleConditionTemplet.m_bAffectSOLDIER);
		flag &= cNKMLua.GetData("m_bAffectMECHANIC", ref nKMBattleConditionTemplet.m_bAffectMECHANIC);
		flag &= cNKMLua.GetData("m_bAffectCORRUPT", ref nKMBattleConditionTemplet.m_bAffectCORRUPT);
		flag &= cNKMLua.GetData("m_bAffectNormal", ref nKMBattleConditionTemplet.m_bAffectNormal);
		flag &= cNKMLua.GetData("m_bAffectAwaken", ref nKMBattleConditionTemplet.m_bAffectAwaken);
		flag &= cNKMLua.GetData("m_AffectUnitRole", ref nKMBattleConditionTemplet.m_AffectUnitRole);
		flag &= cNKMLua.GetData("m_bHitLand", ref nKMBattleConditionTemplet.m_bHitLand);
		flag &= cNKMLua.GetData("m_bHitAir", ref nKMBattleConditionTemplet.m_bHitAir);
		cNKMLua.GetData("m_fActivateTimeLeft", ref nKMBattleConditionTemplet.m_fActivateTimeLeft);
		cNKMLua.GetData("m_bHide", ref nKMBattleConditionTemplet.m_bHide);
		if (cNKMLua.OpenTable("m_hashAffectUnitID"))
		{
			nKMBattleConditionTemplet.m_hashAffectUnitID.Clear();
			string rValue = "";
			for (int i = 1; cNKMLua.GetData(i, ref rValue); i++)
			{
				int unitID = NKMUnitManager.GetUnitID(rValue);
				if (unitID > 0 && !nKMBattleConditionTemplet.m_hashAffectUnitID.Contains(unitID))
				{
					nKMBattleConditionTemplet.m_hashAffectUnitID.Add(unitID);
				}
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_hashIgnoreUnitID"))
		{
			nKMBattleConditionTemplet.m_hashIgnoreUnitID.Clear();
			string rValue2 = "";
			for (int j = 1; cNKMLua.GetData(j, ref rValue2); j++)
			{
				int unitID2 = NKMUnitManager.GetUnitID(rValue2);
				if (unitID2 > 0 && !nKMBattleConditionTemplet.m_hashIgnoreUnitID.Contains(unitID2))
				{
					nKMBattleConditionTemplet.m_hashIgnoreUnitID.Add(unitID2);
				}
			}
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_BoostResource", ref nKMBattleConditionTemplet.m_BoostResource);
		cNKMLua.GetData("m_listAllyBCondUnitStrID", nKMBattleConditionTemplet.m_listAllyBCondUnitStrID);
		cNKMLua.GetData("m_listEnemyBCondUnitStrID", nKMBattleConditionTemplet.m_listEnemyBCondUnitStrID);
		cNKMLua.GetDataList("m_EventCondition", out nKMBattleConditionTemplet.m_EventCondition, nullIfEmpty: true);
		string text = null;
		int num = 1;
		for (int k = 0; k < nKMBattleConditionTemplet.maxBattleConditionBuffCount; k++)
		{
			text = null;
			num = 1;
			cNKMLua.GetData($"m_AllyBuffStrID{k + 1}", ref text);
			if (!string.IsNullOrEmpty(text))
			{
				cNKMLua.GetData($"m_AllyBuffLevel{k + 1}", ref num);
				nKMBattleConditionTemplet.m_dicAllyBuff.Add(text, num);
			}
		}
		for (int l = 0; l < nKMBattleConditionTemplet.maxBattleConditionBuffCount; l++)
		{
			text = null;
			num = 1;
			cNKMLua.GetData($"m_EnemyBuffStrID{l + 1}", ref text);
			if (!string.IsNullOrEmpty(text))
			{
				cNKMLua.GetData($"m_EnemyBuffLevel{l + 1}", ref num);
				nKMBattleConditionTemplet.m_dicEnemyBuff.Add(text, num);
			}
		}
		if (!flag)
		{
			Log.Error("NKMBattleConditionTemplet LoadFromLUA Fail", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBattleConditionManager.cs", 197);
			return null;
		}
		return nKMBattleConditionTemplet;
	}

	public HashSet<string> GetUnitStrIDList(NKM_TEAM_TYPE teamType)
	{
		switch (teamType)
		{
		case NKM_TEAM_TYPE.NTT_A1:
		case NKM_TEAM_TYPE.NTT_A2:
			return m_listAllyBCondUnitStrID;
		case NKM_TEAM_TYPE.NTT_B1:
		case NKM_TEAM_TYPE.NTT_B2:
			return m_listEnemyBCondUnitStrID;
		default:
			return null;
		}
	}

	public Dictionary<string, int> GetAllyBuffList()
	{
		return m_dicAllyBuff;
	}

	public Dictionary<string, int> GetEnemyBuffList()
	{
		return m_dicEnemyBuff;
	}

	public Dictionary<string, int> GetBuffList(NKM_TEAM_TYPE targetTeamType, NKM_TEAM_TYPE usingTeamType = NKM_TEAM_TYPE.NTT_A1)
	{
		if (!NKMGame.IsATeamStaticFunc(targetTeamType) && !NKMGame.IsBTeamStaticFunc(targetTeamType))
		{
			return null;
		}
		if (NKMGame.IsSameTeamStaticFunc(targetTeamType, usingTeamType))
		{
			return m_dicAllyBuff;
		}
		return m_dicEnemyBuff;
	}

	private void CheckValidation()
	{
		if ((m_dicAllyBuff.Count > 0 || m_dicEnemyBuff.Count > 0) && !m_bAffectSHIP && ((!m_bAffectCOUNTER && !m_bAffectSOLDIER && !m_bAffectMECHANIC) || (!m_bHitLand && !m_bHitAir)) && m_hashAffectUnitID.Count == 0 && m_hashIgnoreUnitID.Count == 0)
		{
			Log.ErrorAndExit($"[BattleConditionTemplet] 버프를 받을 유닛이 존재하지 않음 m_BCondID : {m_BCondID}, m_BCondStrID : {m_BCondStrID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBattleConditionManager.cs", 261);
		}
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_dicAllyBuff != null)
		{
			foreach (string key in m_dicAllyBuff.Keys)
			{
				if (NKMBuffManager.GetBuffTempletByStrID(key) == null)
				{
					Log.ErrorAndExit($"[NKMBattleConditionTemplet]  m_listAllyBuffStrID is invalid. m_BCondID [{m_BCondID}], m_BCondStrID [{m_BCondStrID}], m_listAllyBuffStrID [{key}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBattleConditionManager.cs", 277);
				}
			}
		}
		if (m_dicEnemyBuff != null)
		{
			foreach (string key2 in m_dicEnemyBuff.Keys)
			{
				if (NKMBuffManager.GetBuffTempletByStrID(key2) == null)
				{
					Log.ErrorAndExit($"[NKMBattleConditionTemplet]  m_listEnemyBuffStrID is invalid. m_BCondID [{m_BCondID}], m_BCondStrID [{m_BCondStrID}], m_listEnemyBuffStrID [{key2}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBattleConditionManager.cs", 288);
				}
			}
		}
		if (m_EventCondition == null)
		{
			return;
		}
		foreach (string item in m_EventCondition)
		{
			if (NKMEventConditionV2.GetTempletMacroCondition(item) == null)
			{
				Log.ErrorAndExit("[NKMBattleConditionTemplet] eventcondition [" + item + "] in m_EventCondition not exist!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBattleConditionManager.cs", 299);
			}
		}
	}
}
