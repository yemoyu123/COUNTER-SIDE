using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public class NKMDamageTemplet
{
	public NKMDamageTempletBase m_DamageTempletBase = new NKMDamageTempletBase();

	public bool luaDataloaded;

	public NKM_REACT_TYPE m_ReActType = NKM_REACT_TYPE.NRT_NO_ACTION;

	public NKM_SUPER_ARMOR_LEVEL m_CrashSuperArmorLevel = NKM_SUPER_ARMOR_LEVEL.NSAL_NO;

	public bool m_bCanRevenge;

	public float m_BackSpeedKeepTimeX;

	public float m_BackSpeedKeepTimeZ;

	public float m_BackSpeedKeepTimeJumpY;

	public float m_BackSpeedX = -1f;

	public float m_BackSpeedZ = -1f;

	public float m_BackSpeedJumpY = -1f;

	public int m_ReAttackCount = 1;

	public float m_fReAttackGap;

	public float m_fStopReserveTimeAtk;

	public float m_fStopReserveTimeDef;

	public float m_fStopTimeAtk;

	public float m_fStopTimeDef;

	public float m_fCameraCrashGap;

	public float m_fCameraCrashTime;

	public bool m_bCrashBarrier;

	public float m_fFeedbackBarrier;

	public float m_fStunTime;

	public HashSet<NKM_UNIT_STYLE_TYPE> m_StunIgnoreStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_StunAllowStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public float m_fCoolTimeDamage;

	public HashSet<NKM_UNIT_STYLE_TYPE> m_CoolTimeDamageIgnoreStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_CoolTimeDamageAllowStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public NKM_UNIT_STATUS_EFFECT m_ApplyStatusEffect;

	public float m_fApplyStatusTime;

	public HashSet<NKM_UNIT_STYLE_TYPE> m_StatusIgnoreStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public HashSet<NKM_UNIT_STYLE_TYPE> m_StatusAllowStyleType = new HashSet<NKM_UNIT_STYLE_TYPE>();

	public float m_fInstantKillHPRate;

	public bool m_fInstantKillAwaken = true;

	public string m_HitSoundName = "";

	public float m_fLocalVol = 1f;

	public string m_HitEffect = "";

	public string m_HitEffectAnimName = "BASE";

	public float m_fHitEffectRange = 50f;

	public float m_fHitEffectOffsetZ = 50f;

	public bool m_bHitEffectLand;

	public string m_HitEffectAir = "";

	public string m_HitEffectAirAnimName = "BASE";

	public float m_fHitEffectAirRange = 50f;

	public float m_fHitEffectAirOffsetZ;

	public string m_AttackerStateChange = "";

	public string m_AttackerBuff = "";

	public byte m_AttackerBuffStatBaseLevel = 1;

	public byte m_AttackerBuffStatAddLVBySkillLV;

	public byte m_AttackerBuffTimeBaseLevel = 1;

	public byte m_AttackerBuffTimeAddLVBySkillLV;

	public string m_DefenderBuff = "";

	public byte m_DefenderBuffStatBaseLevel = 1;

	public byte m_DefenderBuffStatAddLVBySkillLV;

	public byte m_DefenderBuffTimeBaseLevel = 1;

	public byte m_DefenderBuffTimeAddLVBySkillLV;

	public byte m_DeleteBuffCount;

	public bool m_DeleteConfuseBuff;

	public bool m_bCanDispelStatus;

	public List<NKMHitBuff> m_listAttackerHitBuff = new List<NKMHitBuff>();

	public List<NKMHitBuff> m_listDefenderHitBuff = new List<NKMHitBuff>();

	public NKMKillFeedBack m_NKMKillFeedBack = new NKMKillFeedBack();

	public NKMEventMove m_EventMove;

	public string m_ExtraHitDamageTempletID = "";

	public NKMDamageTemplet m_ExtraHitDamageTemplet;

	public NKMMinMaxInt m_ExtraHitCountRange = new NKMMinMaxInt(1, 1);

	public NKMDamageAttribute m_ExtraHitAttribute;

	public string m_HitTrigger = string.Empty;

	public NKMDamageTemplet()
	{
		m_HitEffect = "AB_fx_hit_b_blue_small";
		m_HitEffectAir = "AB_fx_hit_b_blue_small";
	}

	public void Validate()
	{
		if (!string.IsNullOrEmpty(m_AttackerBuff) && NKMBuffManager.GetBuffTempletByStrID(m_AttackerBuff) == null)
		{
			Log.ErrorAndExit("[NKMDamageTemplet] m_AttackerBuff is invalid. DamageTempletStrID [" + m_DamageTempletBase.m_DamageTempletName + "], m_AttackerBuff [" + m_AttackerBuff + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 424);
		}
		if (!string.IsNullOrEmpty(m_DefenderBuff) && NKMBuffManager.GetBuffTempletByStrID(m_DefenderBuff) == null)
		{
			Log.ErrorAndExit("[NKMDamageTemplet] m_DefenderBuff is invalid. DamageTempletStrID [" + m_DamageTempletBase.m_DamageTempletName + "], m_DefenderBuff [" + m_DefenderBuff + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 432);
		}
		if (m_listAttackerHitBuff != null)
		{
			foreach (NKMHitBuff item in m_listAttackerHitBuff)
			{
				if (!item.Validate())
				{
					Log.ErrorAndExit("[NKMDamageTemplet] m_listAttackerHitBuff is invalid. DamageTempletStrID [" + m_DamageTempletBase.m_DamageTempletName + "], m_HitBuff [" + item.m_HitBuff + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 442);
				}
			}
		}
		if (m_listDefenderHitBuff != null)
		{
			foreach (NKMHitBuff item2 in m_listDefenderHitBuff)
			{
				if (!item2.Validate())
				{
					Log.ErrorAndExit("[NKMDamageTemplet] m_listDefenderHitBuff is invalid. DamageTempletStrID [" + m_DamageTempletBase.m_DamageTempletName + "], m_HitBuff [" + item2.m_HitBuff + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 452);
				}
			}
		}
		if (!string.IsNullOrEmpty(m_ExtraHitDamageTempletID) && m_ExtraHitDamageTemplet == null)
		{
			Log.ErrorAndExit("[NKMDamageTemplet] " + m_DamageTempletBase.m_DamageTempletName + " : m_ExtraHitDamageTempletID  " + m_ExtraHitDamageTempletID + " not found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageManager.cs", 462);
		}
	}

	public void JoinExtraHitDT()
	{
		if (!string.IsNullOrEmpty(m_ExtraHitDamageTempletID))
		{
			m_ExtraHitDamageTemplet = NKMDamageManager.GetTempletByStrID(m_ExtraHitDamageTempletID);
		}
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		luaDataloaded = true;
		cNKMLua.GetData("m_ReActType", ref m_ReActType);
		cNKMLua.GetData("m_CrashSuperArmorLevel", ref m_CrashSuperArmorLevel);
		cNKMLua.GetData("m_bCanRevenge", ref m_bCanRevenge);
		cNKMLua.GetData("m_BackSpeedKeepTimeX", ref m_BackSpeedKeepTimeX);
		cNKMLua.GetData("m_BackSpeedKeepTimeZ", ref m_BackSpeedKeepTimeZ);
		cNKMLua.GetData("m_BackSpeedKeepTimeJumpY", ref m_BackSpeedKeepTimeJumpY);
		cNKMLua.GetData("m_BackSpeedX", ref m_BackSpeedX);
		cNKMLua.GetData("m_BackSpeedZ", ref m_BackSpeedZ);
		cNKMLua.GetData("m_BackSpeedJumpY", ref m_BackSpeedJumpY);
		cNKMLua.GetData("m_ReAttackCount", ref m_ReAttackCount);
		cNKMLua.GetData("m_fReAttackGap", ref m_fReAttackGap);
		cNKMLua.GetData("m_fStopReserveTimeAtk", ref m_fStopReserveTimeAtk);
		cNKMLua.GetData("m_fStopReserveTimeDef", ref m_fStopReserveTimeDef);
		cNKMLua.GetData("m_fStopTimeAtk", ref m_fStopTimeAtk);
		cNKMLua.GetData("m_fStopTimeDef", ref m_fStopTimeDef);
		cNKMLua.GetData("m_fCameraCrashGap", ref m_fCameraCrashGap);
		cNKMLua.GetData("m_fCameraCrashTime", ref m_fCameraCrashTime);
		cNKMLua.GetData("m_bCrashBarrier", ref m_bCrashBarrier);
		cNKMLua.GetData("m_fFeedbackBarrier", ref m_fFeedbackBarrier);
		cNKMLua.GetData("m_fStunTime", ref m_fStunTime);
		m_StunIgnoreStyleType.Clear();
		if (cNKMLua.OpenTable("m_StunIgnoreStyleType"))
		{
			bool flag = true;
			int num = 1;
			NKM_UNIT_STYLE_TYPE result = NKM_UNIT_STYLE_TYPE.NUST_INVALID;
			while (flag)
			{
				flag = cNKMLua.GetData(num, ref result);
				if (flag)
				{
					m_StunIgnoreStyleType.Add(result);
				}
				num++;
			}
			cNKMLua.CloseTable();
		}
		m_StunAllowStyleType.Clear();
		if (cNKMLua.OpenTable("m_StunAllowStyleType"))
		{
			bool flag2 = true;
			int num2 = 1;
			NKM_UNIT_STYLE_TYPE result2 = NKM_UNIT_STYLE_TYPE.NUST_INVALID;
			while (flag2)
			{
				flag2 = cNKMLua.GetData(num2, ref result2);
				if (flag2)
				{
					m_StunAllowStyleType.Add(result2);
				}
				num2++;
			}
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_fCoolTimeDamage", ref m_fCoolTimeDamage);
		m_CoolTimeDamageIgnoreStyleType.Clear();
		if (cNKMLua.OpenTable("m_CoolTimeDamageIgnoreStyleType"))
		{
			bool flag3 = true;
			int num3 = 1;
			NKM_UNIT_STYLE_TYPE result3 = NKM_UNIT_STYLE_TYPE.NUST_INVALID;
			while (flag3)
			{
				flag3 = cNKMLua.GetData(num3, ref result3);
				if (flag3)
				{
					m_CoolTimeDamageIgnoreStyleType.Add(result3);
				}
				num3++;
			}
			cNKMLua.CloseTable();
		}
		m_CoolTimeDamageAllowStyleType.Clear();
		if (cNKMLua.OpenTable("m_CoolTimeDamageAllowStyleType"))
		{
			bool flag4 = true;
			int num4 = 1;
			NKM_UNIT_STYLE_TYPE result4 = NKM_UNIT_STYLE_TYPE.NUST_INVALID;
			while (flag4)
			{
				flag4 = cNKMLua.GetData(num4, ref result4);
				if (flag4)
				{
					m_CoolTimeDamageAllowStyleType.Add(result4);
				}
				num4++;
			}
			cNKMLua.CloseTable();
		}
		cNKMLua.GetDataEnum<NKM_UNIT_STATUS_EFFECT>("m_ApplyStatusEffect", out m_ApplyStatusEffect);
		cNKMLua.GetData("m_fApplyStatusTime", ref m_fApplyStatusTime);
		cNKMLua.GetDataListEnum("m_StatusIgnoreStyleType", m_StatusIgnoreStyleType);
		cNKMLua.GetDataListEnum("m_StatusAllowStyleType", m_StatusAllowStyleType);
		cNKMLua.GetData("m_fInstantKillHPRate", ref m_fInstantKillHPRate);
		cNKMLua.GetData("m_fInstantKillAwaken", ref m_fInstantKillAwaken);
		cNKMLua.GetData("m_HitSoundName", ref m_HitSoundName);
		cNKMLua.GetData("m_fLocalVol", ref m_fLocalVol);
		cNKMLua.GetData("m_HitEffect", ref m_HitEffect);
		cNKMLua.GetData("m_HitEffectAnimName", ref m_HitEffectAnimName);
		cNKMLua.GetData("m_fHitEffectRange", ref m_fHitEffectRange);
		cNKMLua.GetData("m_fHitEffectOffsetZ", ref m_fHitEffectOffsetZ);
		cNKMLua.GetData("m_bHitEffectLand", ref m_bHitEffectLand);
		cNKMLua.GetData("m_HitEffectAir", ref m_HitEffectAir);
		cNKMLua.GetData("m_HitEffectAirAnimName", ref m_HitEffectAirAnimName);
		cNKMLua.GetData("m_fHitEffectAirRange", ref m_fHitEffectAirRange);
		cNKMLua.GetData("m_fHitEffectAirOffsetZ", ref m_fHitEffectAirOffsetZ);
		cNKMLua.GetData("m_AttackerStateChange", ref m_AttackerStateChange);
		LowerCompability("m_AttackerBuffBaseLevel", ref m_AttackerBuffStatBaseLevel, ref m_AttackerBuffTimeBaseLevel);
		LowerCompability("m_AttackerBuffAddLVBySkillLV", ref m_AttackerBuffStatAddLVBySkillLV, ref m_AttackerBuffTimeAddLVBySkillLV);
		cNKMLua.GetData("m_AttackerBuff", ref m_AttackerBuff);
		cNKMLua.GetData("m_AttackerBuffStatBaseLevel", ref m_AttackerBuffStatBaseLevel);
		cNKMLua.GetData("m_AttackerBuffStatAddLVBySkillLV", ref m_AttackerBuffStatAddLVBySkillLV);
		cNKMLua.GetData("m_AttackerBuffTimeBaseLevel", ref m_AttackerBuffTimeBaseLevel);
		cNKMLua.GetData("m_AttackerBuffTimeAddLVBySkillLV", ref m_AttackerBuffTimeAddLVBySkillLV);
		LowerCompability("m_DefenderBuffBaseLevel", ref m_DefenderBuffStatBaseLevel, ref m_DefenderBuffTimeBaseLevel);
		LowerCompability("m_DefenderBuffAddLVBySkillLV", ref m_DefenderBuffStatAddLVBySkillLV, ref m_DefenderBuffTimeAddLVBySkillLV);
		cNKMLua.GetData("m_DefenderBuff", ref m_DefenderBuff);
		cNKMLua.GetData("m_DefenderBuffStatBaseLevel", ref m_DefenderBuffStatBaseLevel);
		cNKMLua.GetData("m_DefenderBuffStatAddLVBySkillLV", ref m_DefenderBuffStatAddLVBySkillLV);
		cNKMLua.GetData("m_DefenderBuffTimeBaseLevel", ref m_DefenderBuffTimeBaseLevel);
		cNKMLua.GetData("m_DefenderBuffTimeAddLVBySkillLV", ref m_DefenderBuffTimeAddLVBySkillLV);
		cNKMLua.GetData("m_DeleteBuffCount", ref m_DeleteBuffCount);
		cNKMLua.GetData("m_DeleteConfuseBuff", ref m_DeleteConfuseBuff);
		cNKMLua.GetData("m_bCanDispelStatus", ref m_bCanDispelStatus);
		if (cNKMLua.OpenTable("m_NKMKillFeedBack"))
		{
			m_NKMKillFeedBack.LoadFromLUA(cNKMLua);
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_NKMEventMove"))
		{
			m_EventMove = new NKMEventMove();
			m_EventMove.LoadFromLUA(cNKMLua);
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listAttackerHitBuff"))
		{
			for (int i = 1; cNKMLua.OpenTable(i); i++)
			{
				NKMHitBuff nKMHitBuff = null;
				if (m_listAttackerHitBuff.Count >= i)
				{
					nKMHitBuff = m_listAttackerHitBuff[i - 1];
				}
				else
				{
					nKMHitBuff = new NKMHitBuff();
					m_listAttackerHitBuff.Add(nKMHitBuff);
				}
				nKMHitBuff.LoadFromLUA(cNKMLua);
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listDefenderHitBuff"))
		{
			for (int j = 1; cNKMLua.OpenTable(j); j++)
			{
				NKMHitBuff nKMHitBuff2 = null;
				if (m_listDefenderHitBuff.Count >= j)
				{
					nKMHitBuff2 = m_listDefenderHitBuff[j - 1];
				}
				else
				{
					nKMHitBuff2 = new NKMHitBuff();
					m_listDefenderHitBuff.Add(nKMHitBuff2);
				}
				nKMHitBuff2.LoadFromLUA(cNKMLua);
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_ExtraHitDamageTempletID", ref m_ExtraHitDamageTempletID);
		m_ExtraHitCountRange.LoadFromLua(cNKMLua, "m_ExtraHitCountRange");
		m_ExtraHitAttribute = NKMDamageAttribute.LoadFromLua(cNKMLua, "m_ExtraHitAttribute");
		cNKMLua.GetData("m_HitTrigger", ref m_HitTrigger);
		return true;
		void LowerCompability(string oldName, ref byte statLevel, ref byte timeLevel)
		{
			byte rValue = 0;
			if (cNKMLua.GetData(oldName, ref rValue))
			{
				statLevel = rValue;
				timeLevel = rValue;
			}
		}
	}

	public bool CanApplyStun(NKMUnitTempletBase defenderUnitTempletBase)
	{
		if (m_fStunTime <= 0f)
		{
			return false;
		}
		return CanApply(defenderUnitTempletBase, m_StunAllowStyleType, m_StunIgnoreStyleType);
	}

	public bool CanApplyCooltimeDamage(NKMUnitTempletBase defenderUnitTempletBase)
	{
		if (m_fCoolTimeDamage <= 0f)
		{
			return false;
		}
		return CanApply(defenderUnitTempletBase, m_CoolTimeDamageAllowStyleType, m_CoolTimeDamageIgnoreStyleType);
	}

	public bool CanApplyStatus(NKMUnitTempletBase targetUnit)
	{
		if (m_ApplyStatusEffect == NKM_UNIT_STATUS_EFFECT.NUSE_NONE)
		{
			return false;
		}
		if (m_fApplyStatusTime <= 0f)
		{
			return false;
		}
		return CanApply(targetUnit, m_StatusAllowStyleType, m_StatusIgnoreStyleType);
	}

	private bool CanApply(NKMUnitTempletBase targetUnit, HashSet<NKM_UNIT_STYLE_TYPE> hsAllowStyle, HashSet<NKM_UNIT_STYLE_TYPE> hsIgnoreStyle)
	{
		if (targetUnit == null)
		{
			return true;
		}
		if (!targetUnit.IsAllowUnitStyleType(hsAllowStyle, hsIgnoreStyle))
		{
			return false;
		}
		return true;
	}

	public bool IsZeroDamage(bool isPvP)
	{
		if (m_DamageTempletBase == null)
		{
			return true;
		}
		return m_DamageTempletBase.IsZeroDamage(isPvP);
	}

	public bool IsZeroDamage()
	{
		if (m_DamageTempletBase == null)
		{
			return true;
		}
		return m_DamageTempletBase.IsZeroDamage();
	}
}
