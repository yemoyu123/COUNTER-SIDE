using System.Collections.Generic;
using Cs.Logging;

namespace NKM;

public class NKMDamageEffectTemplet
{
	public string m_BASE_ID = "";

	public string m_DamageEffectID = "";

	public string m_MainEffectName = "";

	public bool m_bSpine;

	public float m_fScaleFactor = 1f;

	public bool m_bUseTargetBuffScaleFactor;

	public int m_DamageCountMax;

	public bool m_bDamageCountMaxDie;

	public float m_fTargetDistDie;

	public bool m_bTargetDistDieOnlyTargetDie = true;

	public NKMFindTargetData m_FindTargetData = new NKMFindTargetData();

	public float m_fSeeTargetTime = 0.5f;

	public bool m_bSeeTarget = true;

	public bool m_bSeeTargetSpeed = true;

	public bool m_bUseTargetDir;

	public float m_fTargetDirSpeed;

	public bool m_bLookDir;

	public float m_fEffectSize;

	public bool m_bNoMove;

	public bool m_bLandStruck;

	public bool m_bLandBind;

	public bool m_bLandEdge;

	public bool m_bLandConnect;

	public bool m_bDamageSpeedDependMaster;

	public float m_fReloadAccel;

	public float m_fGAccel;

	public float m_fMaxGSpeed = -3000f;

	public NKC_TEAM_COLOR_TYPE m_NKC_TEAM_COLOR_TYPE;

	public float m_fShadowScaleX;

	public float m_fShadowScaleY;

	public float m_fShadowRotateX;

	public float m_fShadowRotateZ;

	public bool m_CanIgnoreStopTime;

	public bool m_UseMasterAnimSpeed;

	public List<NKMEventAttack> m_listNKMDieEventAttack = new List<NKMEventAttack>();

	public List<NKMEventEffect> m_listNKMDieEventEffect = new List<NKMEventEffect>();

	public List<NKMEventDamageEffect> m_listNKMDieEventDamageEffect = new List<NKMEventDamageEffect>();

	public List<NKMEventSound> m_listNKMDieEventSound = new List<NKMEventSound>();

	public Dictionary<string, NKMDamageEffectState> m_dicNKMState = new Dictionary<string, NKMDamageEffectState>();

	public Dictionary<int, string> m_dicSkinMainEffect;

	public NKMDamageEffectTemplet()
	{
		m_FindTargetData.m_bUseUnitSize = NKMCommonConst.FIND_TARGET_USE_UNITSIZE;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_BASE_ID", ref m_BASE_ID);
		cNKMLua.GetData("m_DamageEffectID", ref m_DamageEffectID);
		cNKMLua.GetData("m_MainEffectName", ref m_MainEffectName);
		cNKMLua.GetData("m_bSpine", ref m_bSpine);
		cNKMLua.GetData("m_fScaleFactor", ref m_fScaleFactor);
		cNKMLua.GetData("m_bUseTargetBuffScaleFactor", ref m_bUseTargetBuffScaleFactor);
		cNKMLua.GetData("m_DamageCountMax", ref m_DamageCountMax);
		cNKMLua.GetData("m_bDamageCountMaxDie", ref m_bDamageCountMaxDie);
		cNKMLua.GetData("m_fTargetDistDie", ref m_fTargetDistDie);
		cNKMLua.GetData("m_bTargetDistDieOnlyTargetDie", ref m_bTargetDistDieOnlyTargetDie);
		cNKMLua.GetData("m_fFindTargetTime", ref m_FindTargetData.m_fFindTargetTime);
		cNKMLua.GetData("m_fSeeRange", ref m_FindTargetData.m_fSeeRange);
		cNKMLua.GetData("m_NKM_FIND_TARGET_TYPE", ref m_FindTargetData.m_FindTargetType);
		cNKMLua.GetDataListEnum("m_hsFindTargetRolePriority", m_FindTargetData.m_hsFindTargetRolePriority);
		cNKMLua.GetDataListEnum("m_hsFindTargetStylePriority", m_FindTargetData.m_hsFindTargetStylePriority);
		cNKMLua.GetData("m_bTargetNoChange", ref m_FindTargetData.m_bTargetNoChange);
		cNKMLua.GetData("m_bNoBackTarget", ref m_FindTargetData.m_bNoBackTarget);
		cNKMLua.GetData("m_fSeeTargetTime", ref m_fSeeTargetTime);
		cNKMLua.GetData("m_bSeeTarget", ref m_bSeeTarget);
		cNKMLua.GetData("m_bSeeTargetSpeed", ref m_bSeeTargetSpeed);
		cNKMLua.GetData("m_bUseTargetDir", ref m_bUseTargetDir);
		cNKMLua.GetData("m_fTargetDirSpeed", ref m_fTargetDirSpeed);
		cNKMLua.GetData("m_bLookDir", ref m_bLookDir);
		cNKMLua.GetData("m_bNoMove", ref m_bNoMove);
		cNKMLua.GetData("m_bLandStruck", ref m_bLandStruck);
		cNKMLua.GetData("m_bLandBind", ref m_bLandBind);
		cNKMLua.GetData("m_bLandEdge", ref m_bLandEdge);
		cNKMLua.GetData("m_bLandConnect", ref m_bLandConnect);
		cNKMLua.GetData("m_fEffectSize", ref m_fEffectSize);
		cNKMLua.GetData("m_bDamageSpeedDependMaster", ref m_bDamageSpeedDependMaster);
		cNKMLua.GetData("m_fReloadAccel", ref m_fReloadAccel);
		cNKMLua.GetData("m_fGAccel", ref m_fGAccel);
		cNKMLua.GetData("m_fMaxGSpeed", ref m_fMaxGSpeed);
		cNKMLua.GetData("m_NKC_TEAM_COLOR_TYPE", ref m_NKC_TEAM_COLOR_TYPE);
		cNKMLua.GetData("m_fShadowScaleX", ref m_fShadowScaleX);
		cNKMLua.GetData("m_fShadowScaleY", ref m_fShadowScaleY);
		cNKMLua.GetData("m_fShadowRotateX", ref m_fShadowRotateX);
		cNKMLua.GetData("m_fShadowRotateZ", ref m_fShadowRotateZ);
		cNKMLua.GetData("m_CanIgnoreStopTime", ref m_CanIgnoreStopTime);
		cNKMLua.GetData("m_UseMasterAnimSpeed", ref m_UseMasterAnimSpeed);
		NKMUnitState.LoadAndMergeEventList(cNKMLua, "m_listNKMDieEventAttack", ref m_listNKMDieEventAttack);
		NKMUnitState.LoadAndMergeEventList(cNKMLua, "m_listNKMDieEventEffect", ref m_listNKMDieEventEffect);
		NKMUnitState.LoadAndMergeEventList(cNKMLua, "m_listNKMDieEventDamageEffect", ref m_listNKMDieEventDamageEffect);
		NKMUnitState.LoadAndMergeEventList(cNKMLua, "m_listNKMDieEventSound", ref m_listNKMDieEventSound);
		if (cNKMLua.OpenTable("m_dicNKMState"))
		{
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				NKMDamageEffectState nKMDamageEffectState = null;
				string rValue = "";
				cNKMLua.GetData("m_StateName", ref rValue);
				nKMDamageEffectState = ((!m_dicNKMState.ContainsKey(rValue)) ? new NKMDamageEffectState() : m_dicNKMState[rValue]);
				nKMDamageEffectState.LoadFromLUA(cNKMLua);
				if (!m_dicNKMState.ContainsKey(nKMDamageEffectState.m_StateName))
				{
					m_dicNKMState.Add(nKMDamageEffectState.m_StateName, nKMDamageEffectState);
				}
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_dicSkinEffect"))
		{
			if (m_dicSkinMainEffect == null)
			{
				m_dicSkinMainEffect = new Dictionary<int, string>();
			}
			int num2 = 1;
			while (cNKMLua.OpenTable(num2))
			{
				int rValue2 = -1;
				string rValue3 = "AB_FX_DUMMY";
				cNKMLua.GetData("m_SkinID", ref rValue2);
				cNKMLua.GetData("m_MainEffectName", ref rValue3);
				m_dicSkinMainEffect.Add(rValue2, rValue3);
				num2++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		foreach (NKMDamageEffectState value in m_dicNKMState.Values)
		{
			foreach (NKMEventMove item in value.m_listNKMEventMove)
			{
				if (item.m_EventPosData.m_MoveBase == NKMEventPosData.MoveBase.ME && (item.m_EventPosData.m_MoveOffset == NKMEventPosData.MoveOffset.ME || item.m_EventPosData.m_MoveOffset == NKMEventPosData.MoveOffset.ME_INV))
				{
					Log.Error(m_DamageEffectID + " : " + value.m_StateName + " Base and offset is same", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffectTemplet.cs", 342);
				}
				if (item.m_EventPosData.m_MoveBase == NKMEventPosData.MoveBase.TARGET_UNIT && (item.m_EventPosData.m_MoveOffset == NKMEventPosData.MoveOffset.TARGET_UNIT || item.m_EventPosData.m_MoveOffset == NKMEventPosData.MoveOffset.TARGET_UNIT_INV))
				{
					Log.Error(m_DamageEffectID + " : " + value.m_StateName + " Base and offset is same", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMDamageEffectTemplet.cs", 351);
				}
			}
		}
		return true;
	}

	public void DeepCopyFromSource(NKMDamageEffectTemplet source)
	{
		m_MainEffectName = source.m_MainEffectName;
		m_bSpine = source.m_bSpine;
		m_fScaleFactor = source.m_fScaleFactor;
		m_bUseTargetBuffScaleFactor = source.m_bUseTargetBuffScaleFactor;
		m_DamageCountMax = source.m_DamageCountMax;
		m_bDamageCountMaxDie = source.m_bDamageCountMaxDie;
		m_fTargetDistDie = source.m_fTargetDistDie;
		m_bTargetDistDieOnlyTargetDie = source.m_bTargetDistDieOnlyTargetDie;
		NKMFindTargetData.DeepCopyFrom(source.m_FindTargetData, out m_FindTargetData);
		m_fSeeTargetTime = source.m_fSeeTargetTime;
		m_bSeeTarget = source.m_bSeeTarget;
		m_bSeeTargetSpeed = source.m_bSeeTargetSpeed;
		m_bUseTargetDir = source.m_bUseTargetDir;
		m_fTargetDirSpeed = source.m_fTargetDirSpeed;
		m_bLookDir = source.m_bLookDir;
		m_fEffectSize = source.m_fEffectSize;
		m_bNoMove = source.m_bNoMove;
		m_bLandStruck = source.m_bLandStruck;
		m_bLandBind = source.m_bLandBind;
		m_bLandEdge = source.m_bLandEdge;
		m_bLandConnect = source.m_bLandConnect;
		m_bDamageSpeedDependMaster = source.m_bDamageSpeedDependMaster;
		m_fReloadAccel = source.m_fReloadAccel;
		m_fGAccel = source.m_fGAccel;
		m_fMaxGSpeed = source.m_fMaxGSpeed;
		m_NKC_TEAM_COLOR_TYPE = source.m_NKC_TEAM_COLOR_TYPE;
		m_fShadowScaleX = source.m_fShadowScaleX;
		m_fShadowScaleY = source.m_fShadowScaleY;
		m_fShadowRotateX = source.m_fShadowRotateX;
		m_fShadowRotateZ = source.m_fShadowRotateZ;
		m_CanIgnoreStopTime = source.m_CanIgnoreStopTime;
		m_UseMasterAnimSpeed = source.m_UseMasterAnimSpeed;
		NKMUnitState.DeepCopy(source.m_listNKMDieEventAttack, ref m_listNKMDieEventAttack, delegate(NKMEventAttack t, NKMEventAttack s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMDieEventEffect, ref m_listNKMDieEventEffect, delegate(NKMEventEffect t, NKMEventEffect s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMDieEventDamageEffect, ref m_listNKMDieEventDamageEffect, delegate(NKMEventDamageEffect t, NKMEventDamageEffect s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMDieEventSound, ref m_listNKMDieEventSound, delegate(NKMEventSound t, NKMEventSound s)
		{
			t.DeepCopyFromSource(s);
		});
		foreach (KeyValuePair<string, NKMDamageEffectState> item in source.m_dicNKMState)
		{
			NKMDamageEffectState nKMDamageEffectState = null;
			if (!m_dicNKMState.ContainsKey(item.Key))
			{
				nKMDamageEffectState = new NKMDamageEffectState();
				m_dicNKMState.Add(item.Key, nKMDamageEffectState);
			}
			else
			{
				nKMDamageEffectState = m_dicNKMState[item.Key];
			}
			nKMDamageEffectState.DeepCopyFromSource(item.Value);
		}
		if (source.m_dicSkinMainEffect == null)
		{
			m_dicSkinMainEffect = null;
			return;
		}
		m_dicSkinMainEffect = new Dictionary<int, string>();
		foreach (KeyValuePair<int, string> item2 in source.m_dicSkinMainEffect)
		{
			m_dicSkinMainEffect.Add(item2.Key, item2.Value);
		}
	}

	public NKMDamageEffectState GetState(string stateName)
	{
		if (m_dicNKMState.ContainsKey(stateName))
		{
			return m_dicNKMState[stateName];
		}
		return null;
	}

	public string GetMainEffectName(int skinID)
	{
		if (skinID < 0)
		{
			return m_MainEffectName;
		}
		if (m_dicSkinMainEffect == null)
		{
			return m_MainEffectName;
		}
		if (m_dicSkinMainEffect.TryGetValue(skinID, out var value))
		{
			return value;
		}
		return m_MainEffectName;
	}
}
