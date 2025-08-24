using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public class NKMDungeonRespawnUnitTemplet
{
	public enum ShowGageOverride
	{
		Default,
		Show,
		Hide
	}

	public enum UnitEffect
	{
		NONE,
		SHADOW
	}

	public long m_TempletUID;

	public NKMDungeonEventTiming m_NKMDungeonEventTiming = new NKMDungeonEventTiming();

	public string m_EventRespawnUnitTag = "";

	public string m_UnitStrID = "";

	public int m_UnitLevel;

	public int m_SkinID;

	public short m_UnitLimitBreakLevel;

	public List<NKMStaticBuffData> m_listStaticBuffData = new List<NKMStaticBuffData>();

	public int m_UnitLevelBonusPerWave;

	public ShowGageOverride m_eShowGage;

	public NKMStatData m_AddStatData = new NKMStatData();

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_hsImmuneStatus;

	public HashSet<NKM_UNIT_STATUS_EFFECT> m_hsApplyStatus;

	public float m_fRespawnCoolTime;

	public int m_WaveID;

	public string m_ChangeUnitName;

	public bool m_bForceMonster;

	public bool m_bReactor;

	public int m_TacticUpdateLevel;

	public int m_SkillLevel;

	public UnitEffect m_UnitEffect;

	public NKMColor m_Color = new NKMColor(1f, 1f, 1f);

	public string StrKey
	{
		get
		{
			if (string.IsNullOrEmpty(m_ChangeUnitName))
			{
				return m_UnitStrID;
			}
			return $"{m_UnitStrID}@{m_ChangeUnitName}";
		}
	}

	public bool HasColorChange()
	{
		if (m_UnitEffect != UnitEffect.NONE)
		{
			return true;
		}
		if (m_Color.r == 1f && m_Color.g == 1f)
		{
			return m_Color.b != 1f;
		}
		return true;
	}

	public NKMColor GetColorChange()
	{
		if (m_Color.r != 1f || m_Color.g != 1f || m_Color.b != 1f)
		{
			return m_Color;
		}
		UnitEffect unitEffect = m_UnitEffect;
		if (unitEffect == UnitEffect.NONE || unitEffect != UnitEffect.SHADOW)
		{
			return m_Color;
		}
		return new NKMColor(0.4f, 0.4f, 0.4f);
	}

	public string GetUnitEffectName()
	{
		UnitEffect unitEffect = m_UnitEffect;
		if (unitEffect == UnitEffect.NONE || unitEffect != UnitEffect.SHADOW)
		{
			return string.Empty;
		}
		return "AB_FX_MOB_SMOKE";
	}

	public void DeecCopyFromSource(NKMDungeonRespawnUnitTemplet sourceTemplet)
	{
		m_NKMDungeonEventTiming = sourceTemplet.m_NKMDungeonEventTiming.Clone();
		m_EventRespawnUnitTag = sourceTemplet.m_EventRespawnUnitTag;
		m_UnitStrID = sourceTemplet.m_UnitStrID;
		m_UnitLevel = sourceTemplet.m_UnitLevel;
		m_SkinID = sourceTemplet.m_SkinID;
		m_UnitLimitBreakLevel = sourceTemplet.m_UnitLimitBreakLevel;
		m_UnitLevelBonusPerWave = sourceTemplet.m_UnitLevelBonusPerWave;
		m_eShowGage = sourceTemplet.m_eShowGage;
		m_AddStatData.DeepCopyFromSource(sourceTemplet.m_AddStatData);
		m_fRespawnCoolTime = sourceTemplet.m_fRespawnCoolTime;
		m_WaveID = sourceTemplet.m_WaveID;
		m_ChangeUnitName = sourceTemplet.m_ChangeUnitName;
		m_bForceMonster = sourceTemplet.m_bForceMonster;
		m_bReactor = sourceTemplet.m_bReactor;
		m_TacticUpdateLevel = sourceTemplet.m_TacticUpdateLevel;
		m_SkillLevel = sourceTemplet.m_SkillLevel;
		foreach (NKMStaticBuffData listStaticBuffDatum in sourceTemplet.m_listStaticBuffData)
		{
			NKMStaticBuffData item = listStaticBuffDatum.Clone();
			m_listStaticBuffData.Add(item);
		}
		if (sourceTemplet.m_hsImmuneStatus != null)
		{
			m_hsImmuneStatus = new HashSet<NKM_UNIT_STATUS_EFFECT>(sourceTemplet.m_hsImmuneStatus);
		}
		else
		{
			m_hsImmuneStatus = null;
		}
		if (sourceTemplet.m_hsApplyStatus != null)
		{
			m_hsApplyStatus = new HashSet<NKM_UNIT_STATUS_EFFECT>(sourceTemplet.m_hsApplyStatus);
		}
		else
		{
			m_hsApplyStatus = null;
		}
		m_Color.DeepCopyFromSource(sourceTemplet.m_Color);
		m_UnitEffect = sourceTemplet.m_UnitEffect;
	}

	public void Validate(string dungeonStrID)
	{
		if (!string.IsNullOrEmpty(m_UnitStrID))
		{
			if (NKMUnitManager.GetUnitTemplet(m_UnitStrID) == null)
			{
				Log.ErrorAndExit("[NKMDungeonRespawnUnitTemplet] 유닛 정보가 존재하지 않음 DungeonStrID [" + dungeonStrID + "], m_UnitStrID [" + m_UnitStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 663);
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitStrID);
			if (unitTempletBase == null)
			{
				Log.ErrorAndExit("[NKMDungeonRespawnUnitTemplet] 유닛 템플릿이 존재하지 않음 DungeonStrID [" + dungeonStrID + "], m_UnitStrID [" + m_UnitStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 669);
			}
			if (m_SkinID != 0)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_SkinID);
				if (skinTemplet == null)
				{
					Log.ErrorAndExit($"[NKMDungeonRespawnUnitTemplet]스킨 템플릿이 존재하지 않음 DungeonStrID [{dungeonStrID}], m_UnitStrID [{m_UnitStrID}], m_SkinID [{m_SkinID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 677);
				}
				if (!NKMSkinManager.IsSkinForCharacter(unitTempletBase.m_UnitID, skinTemplet))
				{
					Log.ErrorAndExit($"[NKMDungeonRespawnUnitTemplet]지정된 스킨이 목표 유닛을 위한 스킨이 아님 DungeonStrID [{dungeonStrID}], m_UnitStrID [{m_UnitStrID}], m_SkinID [{m_SkinID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 682);
				}
			}
		}
		if (m_listStaticBuffData == null)
		{
			return;
		}
		foreach (NKMStaticBuffData listStaticBuffDatum in m_listStaticBuffData)
		{
			if (!listStaticBuffDatum.Validate())
			{
				Log.ErrorAndExit("[NKMDungeonRespawnUnitTemplet] m_listStaticBuffData is invalid. DungeonStrID [" + dungeonStrID + "], StaticBuffStrID [" + listStaticBuffDatum.m_BuffStrID + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDungeonManager.cs", 693);
			}
		}
	}

	public bool LoadFromLUA(NKMLua cNKMLua, NKMDungeonTempletBase cNKMDungeonTempletBase)
	{
		if (cNKMLua.OpenTable("m_NKMDungeonEventTiming"))
		{
			m_NKMDungeonEventTiming.LoadFromLUA(cNKMLua);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_EventRespawnUnitTag", ref m_EventRespawnUnitTag);
		cNKMLua.GetData("m_UnitStrID", ref m_UnitStrID);
		cNKMLua.GetData("m_SkinID", ref m_SkinID);
		cNKMLua.GetData("m_UnitLevel", ref m_UnitLevel);
		cNKMLua.GetData("m_UnitLevelBonusPerWave", ref m_UnitLevelBonusPerWave);
		cNKMLua.GetData("m_UnitLimitBreakLevel", ref m_UnitLimitBreakLevel);
		if (m_UnitLevel == 0)
		{
			m_UnitLevel = cNKMDungeonTempletBase.m_DungeonLevel;
		}
		bool rbValue = false;
		if (cNKMLua.GetData("m_bShowGage", ref rbValue))
		{
			m_eShowGage = (rbValue ? ShowGageOverride.Show : ShowGageOverride.Hide);
		}
		else
		{
			m_eShowGage = ShowGageOverride.Default;
		}
		if (cNKMLua.OpenTable("m_listStaticBuffData"))
		{
			m_listStaticBuffData.Clear();
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				NKMStaticBuffData nKMStaticBuffData = new NKMStaticBuffData();
				nKMStaticBuffData.LoadFromLUA(cNKMLua);
				m_listStaticBuffData.Add(nKMStaticBuffData);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		cNKMLua.GetDataListEnum("m_hsApplyStatus", out m_hsApplyStatus, nullIfEmpty: true);
		cNKMLua.GetDataListEnum("m_hsImmuneStatus", out m_hsImmuneStatus, nullIfEmpty: true);
		if (cNKMLua.OpenTable("m_AddStatData"))
		{
			m_AddStatData.LoadFromLUA(cNKMLua, bDungeonRespawn: true);
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_fRespawnCoolTime", ref m_fRespawnCoolTime);
		cNKMLua.GetData("m_ChangeUnitName", ref m_ChangeUnitName);
		cNKMLua.GetData("m_bForceMonster", ref m_bForceMonster);
		cNKMLua.GetData("m_bReactor", ref m_bReactor);
		cNKMLua.GetData("m_TacticUpdateLevel", ref m_TacticUpdateLevel);
		cNKMLua.GetData("m_SkillLevel", ref m_SkillLevel);
		m_Color.LoadFromLua(cNKMLua, "m_Color");
		cNKMLua.GetData("m_UnitEffect", ref m_UnitEffect);
		return true;
	}

	public void RegisterDungeonrEspawnUnitTemplet(NKMDungeonTempletBase cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE dungeonRespawnUnitTempletType, int waveID, int respawnUnitCount)
	{
		m_TempletUID = NKMDungeonManager.AddNKMDungeonRespawnUnitTemplet(this, cNKMDungeonTempletBase.m_DungeonStrID, cNKMDungeonTempletBase.m_DungeonID, dungeonRespawnUnitTempletType, waveID, respawnUnitCount);
	}

	public bool LoadFromLUA(NKMLua cNKMLua, NKMDungeonTempletBase cNKMDungeonTempletBase, DUNGEON_RESPAWN_UNIT_TEMPLET_TYPE dungeonRespawnUnitTempletType, int waveID, int respawnUnitCount)
	{
		RegisterDungeonrEspawnUnitTemplet(cNKMDungeonTempletBase, dungeonRespawnUnitTempletType, waveID, respawnUnitCount);
		return LoadFromLUA(cNKMLua, cNKMDungeonTempletBase);
	}
}
