using System.Collections.Generic;
using Cs.Logging;
using NKM.Unit;

namespace NKM;

public class NKMEventEffect : NKMUnitEffectStateEventOneTime, INKMUnitStateEvent, IEventConditionOwner
{
	public int m_SkinID = -1;

	public string m_EffectName = "";

	public NKM_EFFECT_PARENT_TYPE m_ParentType;

	public float m_fScaleFactor = 1f;

	public bool m_bUseTriggerTargetRange;

	public bool m_bForceRight;

	public bool m_bFixedPos;

	public float m_OffsetX;

	public float m_OffsetY;

	public float m_OffsetZ;

	public bool m_bUseOffsetZtoY;

	public float m_fAddRotate;

	public bool m_bUseZScale = true;

	public bool m_bLandConnect;

	public string m_BoneName = "";

	public bool m_bUseBoneRotate;

	public string m_AnimName = "";

	public float m_fReserveTime;

	public bool m_bStateEndStop;

	public bool m_bStateEndStopForce;

	public bool m_bHold;

	public bool m_bCutIn;

	public bool m_UseMasterAnimSpeed;

	public bool m_ApplyStopTime = true;

	public float m_fAnimSpeed = 1f;

	public Dictionary<int, string> m_dicSkinEffectName;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Client;

	public string GetEffectName(int unitSkinID)
	{
		if (m_dicSkinEffectName == null)
		{
			return m_EffectName;
		}
		if (m_dicSkinEffectName.TryGetValue(unitSkinID, out var value))
		{
			return value;
		}
		return m_EffectName;
	}

	public string GetEffectName(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return m_EffectName;
		}
		return GetEffectName(unitData.m_SkinID);
	}

	public string GetEffectName(NKMDamageEffectData DEData)
	{
		if (DEData == null)
		{
			return m_EffectName;
		}
		return GetEffectName(DEData.GetMasterSkinID());
	}

	public bool IsRightSkin(int unitSkinID)
	{
		if (m_SkinID < 0)
		{
			return true;
		}
		return m_SkinID == unitSkinID;
	}

	public void DeepCopyFromSource(NKMEventEffect source)
	{
		DeepCopy(source);
		m_SkinID = source.m_SkinID;
		m_bUseTriggerTargetRange = source.m_bUseTriggerTargetRange;
		m_EffectName = source.m_EffectName;
		m_ParentType = source.m_ParentType;
		m_fScaleFactor = source.m_fScaleFactor;
		m_bForceRight = source.m_bForceRight;
		m_bFixedPos = source.m_bFixedPos;
		m_OffsetX = source.m_OffsetX;
		m_OffsetY = source.m_OffsetY;
		m_OffsetZ = source.m_OffsetZ;
		m_bUseOffsetZtoY = source.m_bUseOffsetZtoY;
		m_fAddRotate = source.m_fAddRotate;
		m_bUseZScale = source.m_bUseZScale;
		m_bLandConnect = source.m_bLandConnect;
		m_BoneName = source.m_BoneName;
		m_bUseBoneRotate = source.m_bUseBoneRotate;
		m_AnimName = source.m_AnimName;
		m_fReserveTime = source.m_fReserveTime;
		m_bStateEndStop = source.m_bStateEndStop;
		m_bStateEndStopForce = source.m_bStateEndStopForce;
		m_bHold = source.m_bHold;
		m_bCutIn = source.m_bCutIn;
		m_UseMasterAnimSpeed = source.m_UseMasterAnimSpeed;
		m_ApplyStopTime = source.m_ApplyStopTime;
		m_fAnimSpeed = source.m_fAnimSpeed;
		if (source.m_dicSkinEffectName != null)
		{
			m_dicSkinEffectName = new Dictionary<int, string>();
			{
				foreach (KeyValuePair<int, string> item in source.m_dicSkinEffectName)
				{
					m_dicSkinEffectName.Add(item.Key, item.Value);
				}
				return;
			}
		}
		m_dicSkinEffectName = null;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_SkinID", ref m_SkinID);
		cNKMLua.GetData("m_bUseTriggerTargetRange", ref m_bUseTriggerTargetRange);
		cNKMLua.GetData("m_EffectName", ref m_EffectName);
		cNKMLua.GetData("m_ParentType", ref m_ParentType);
		cNKMLua.GetData("m_fScaleFactor", ref m_fScaleFactor);
		cNKMLua.GetData("m_bForceRight", ref m_bForceRight);
		cNKMLua.GetData("m_bFixedPos", ref m_bFixedPos);
		cNKMLua.GetData("m_OffsetX", ref m_OffsetX);
		cNKMLua.GetData("m_OffsetY", ref m_OffsetY);
		cNKMLua.GetData("m_OffsetZ", ref m_OffsetZ);
		cNKMLua.GetData("m_bUseOffsetZtoY", ref m_bUseOffsetZtoY);
		cNKMLua.GetData("m_fAddRotate", ref m_fAddRotate);
		cNKMLua.GetData("m_bUseZScale", ref m_bUseZScale);
		cNKMLua.GetData("m_bLandConnect", ref m_bLandConnect);
		cNKMLua.GetData("m_BoneName", ref m_BoneName);
		cNKMLua.GetData("m_bUseBoneRotate", ref m_bUseBoneRotate);
		cNKMLua.GetData("m_AnimName", ref m_AnimName);
		cNKMLua.GetData("m_fReserveTime", ref m_fReserveTime);
		cNKMLua.GetData("m_bStateEndStop", ref m_bStateEndStop);
		cNKMLua.GetData("m_bStateEndStopForce", ref m_bStateEndStopForce);
		cNKMLua.GetData("m_bHold", ref m_bHold);
		cNKMLua.GetData("m_bCutIn", ref m_bCutIn);
		cNKMLua.GetData("m_UseMasterAnimSpeed", ref m_UseMasterAnimSpeed);
		cNKMLua.GetData("m_ApplyStopTime", ref m_ApplyStopTime);
		cNKMLua.GetData("m_fAnimSpeed", ref m_fAnimSpeed);
		if (cNKMLua.OpenTable("m_dicSkinEffectName"))
		{
			if (m_SkinID >= 0)
			{
				Log.Warn($"m_dicSkinEffectName m_SkinID {m_SkinID}과 함께 사용되었습니다.. m_dicSkinEffectName이 무시될 가능성이 높습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 3583);
			}
			m_dicSkinEffectName = new Dictionary<int, string>();
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				if (cNKMLua.GetData(1, out var rValue, 0) && cNKMLua.GetData(2, out var rValue2, ""))
				{
					m_dicSkinEffectName.Add(rValue, rValue2);
				}
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect)
	{
		cNKMDamageEffect.ApplyEventEffect(this);
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange).ApplyEventEffect(this);
	}
}
