using System.Collections.Generic;

namespace NKM;

public class NKMDamageEffectState
{
	public string m_StateName = "";

	public string m_AnimName = "";

	public bool m_bAnimLoop;

	public float m_fAnimSpeed = 1f;

	public NKM_LIFE_TIME_TYPE m_NKM_LIFE_TIME_TYPE = NKM_LIFE_TIME_TYPE.NLTT_TIME;

	public float m_LifeTimeAnimCount;

	public float m_LifeTime;

	public bool m_bNoMove;

	public float m_StateTimeChangeStateTime = -1f;

	public string m_StateTimeChangeState = "";

	public int m_DamageCountChangeStateCount;

	public string m_DamageCountChangeState = "";

	public float m_TargetDistFarChangeStateDist;

	public string m_TargetDistFarChangeState = "";

	public float m_TargetDistNearChangeStateDist;

	public string m_TargetDistNearChangeState = "";

	public string m_AnimEndChangeState = "";

	public string m_FootOnLandChangeState = "";

	public List<NKMEventTargetDirSpeed> m_listNKMEventTargetDirSpeed = new List<NKMEventTargetDirSpeed>();

	public List<NKMEventDirSpeed> m_listNKMEventDirSpeed = new List<NKMEventDirSpeed>();

	public List<NKMEventSpeed> m_listNKMEventSpeed = new List<NKMEventSpeed>();

	public List<NKMEventSpeedX> m_listNKMEventSpeedX = new List<NKMEventSpeedX>();

	public List<NKMEventSpeedY> m_listNKMEventSpeedY = new List<NKMEventSpeedY>();

	public List<NKMEventMove> m_listNKMEventMove = new List<NKMEventMove>();

	public List<NKMEventAttack> m_listNKMEventAttack = new List<NKMEventAttack>();

	public List<NKMEventSound> m_listNKMEventSound = new List<NKMEventSound>();

	public List<NKMEventCameraCrash> m_listNKMEventCameraCrash = new List<NKMEventCameraCrash>();

	public List<NKMEventEffect> m_listNKMEventEffect = new List<NKMEventEffect>();

	public List<NKMEventDamageEffect> m_listNKMEventDamageEffect = new List<NKMEventDamageEffect>();

	public List<NKMEventDissolve> m_listNKMEventDissolve = new List<NKMEventDissolve>();

	public List<NKMEventBuff> m_listNKMEventBuff = new List<NKMEventBuff>();

	public List<NKMEventStatus> m_listNKMEventStatus = new List<NKMEventStatus>();

	public List<NKMEventHeal> m_listNKMEventHeal = new List<NKMEventHeal>();

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		int result = (int)(1u & (cNKMLua.GetData("m_StateName", ref m_StateName) ? 1u : 0u) & (cNKMLua.GetData("m_AnimName", ref m_AnimName) ? 1u : 0u) & (cNKMLua.GetData("m_bAnimLoop", ref m_bAnimLoop) ? 1u : 0u) & (cNKMLua.GetData("m_fAnimSpeed", ref m_fAnimSpeed) ? 1u : 0u) & (cNKMLua.GetData("m_NKM_LIFE_TIME_TYPE", ref m_NKM_LIFE_TIME_TYPE) ? 1u : 0u) & (cNKMLua.GetData("m_LifeTimeAnimCount", ref m_LifeTimeAnimCount) ? 1u : 0u) & (cNKMLua.GetData("m_LifeTime", ref m_LifeTime) ? 1u : 0u) & (cNKMLua.GetData("m_bNoMove", ref m_bNoMove) ? 1u : 0u) & (cNKMLua.GetData("m_StateTimeChangeStateTime", ref m_StateTimeChangeStateTime) ? 1u : 0u) & (cNKMLua.GetData("m_StateTimeChangeState", ref m_StateTimeChangeState) ? 1u : 0u) & (cNKMLua.GetData("m_DamageCountChangeStateCount", ref m_DamageCountChangeStateCount) ? 1u : 0u) & (cNKMLua.GetData("m_DamageCountChangeState", ref m_DamageCountChangeState) ? 1u : 0u) & (cNKMLua.GetData("m_TargetDistFarChangeStateDist", ref m_TargetDistFarChangeStateDist) ? 1u : 0u) & (cNKMLua.GetData("m_TargetDistFarChangeState", ref m_TargetDistFarChangeState) ? 1u : 0u) & (cNKMLua.GetData("m_TargetDistNearChangeStateDist", ref m_TargetDistNearChangeStateDist) ? 1u : 0u) & (cNKMLua.GetData("m_TargetDistNearChangeState", ref m_TargetDistNearChangeState) ? 1u : 0u) & (cNKMLua.GetData("m_AnimEndChangeState", ref m_AnimEndChangeState) ? 1u : 0u)) & (cNKMLua.GetData("m_FootOnLandChangeState", ref m_FootOnLandChangeState) ? 1 : 0);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventTargetDirSpeed", ref m_listNKMEventTargetDirSpeed);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventDirSpeed", ref m_listNKMEventDirSpeed);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventSpeed", ref m_listNKMEventSpeed);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventSpeedX", ref m_listNKMEventSpeedX);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventSpeedY", ref m_listNKMEventSpeedY);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventMove", ref m_listNKMEventMove);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventAttack", ref m_listNKMEventAttack);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventSound", ref m_listNKMEventSound);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventCameraCrash", ref m_listNKMEventCameraCrash);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventEffect", ref m_listNKMEventEffect);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventDamageEffect", ref m_listNKMEventDamageEffect);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventDissolve", ref m_listNKMEventDissolve);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventBuff", ref m_listNKMEventBuff);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventStatus", ref m_listNKMEventStatus);
		NKMUnitState.LoadEventList(cNKMLua, "m_listNKMEventHeal", ref m_listNKMEventHeal);
		return (byte)result != 0;
	}

	public void DeepCopyFromSource(NKMDamageEffectState source)
	{
		m_StateName = source.m_StateName;
		m_AnimName = source.m_AnimName;
		m_bAnimLoop = source.m_bAnimLoop;
		m_fAnimSpeed = source.m_fAnimSpeed;
		m_NKM_LIFE_TIME_TYPE = source.m_NKM_LIFE_TIME_TYPE;
		m_LifeTimeAnimCount = source.m_LifeTimeAnimCount;
		m_LifeTime = source.m_LifeTime;
		m_bNoMove = source.m_bNoMove;
		m_StateTimeChangeStateTime = source.m_StateTimeChangeStateTime;
		m_StateTimeChangeState = source.m_StateTimeChangeState;
		m_DamageCountChangeStateCount = source.m_DamageCountChangeStateCount;
		m_DamageCountChangeState = source.m_DamageCountChangeState;
		m_TargetDistFarChangeStateDist = source.m_TargetDistFarChangeStateDist;
		m_TargetDistFarChangeState = source.m_TargetDistFarChangeState;
		m_TargetDistNearChangeStateDist = source.m_TargetDistNearChangeStateDist;
		m_TargetDistNearChangeState = source.m_TargetDistNearChangeState;
		m_AnimEndChangeState = source.m_AnimEndChangeState;
		m_FootOnLandChangeState = source.m_FootOnLandChangeState;
		NKMUnitState.DeepCopy(source.m_listNKMEventTargetDirSpeed, ref m_listNKMEventTargetDirSpeed, delegate(NKMEventTargetDirSpeed t, NKMEventTargetDirSpeed s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventDirSpeed, ref m_listNKMEventDirSpeed, delegate(NKMEventDirSpeed t, NKMEventDirSpeed s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventSpeed, ref m_listNKMEventSpeed, delegate(NKMEventSpeed t, NKMEventSpeed s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventSpeedX, ref m_listNKMEventSpeedX, delegate(NKMEventSpeedX t, NKMEventSpeedX s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventSpeedY, ref m_listNKMEventSpeedY, delegate(NKMEventSpeedY t, NKMEventSpeedY s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventMove, ref m_listNKMEventMove, delegate(NKMEventMove t, NKMEventMove s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventAttack, ref m_listNKMEventAttack, delegate(NKMEventAttack t, NKMEventAttack s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventSound, ref m_listNKMEventSound, delegate(NKMEventSound t, NKMEventSound s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventCameraCrash, ref m_listNKMEventCameraCrash, delegate(NKMEventCameraCrash t, NKMEventCameraCrash s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventEffect, ref m_listNKMEventEffect, delegate(NKMEventEffect t, NKMEventEffect s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventDamageEffect, ref m_listNKMEventDamageEffect, delegate(NKMEventDamageEffect t, NKMEventDamageEffect s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventDissolve, ref m_listNKMEventDissolve, delegate(NKMEventDissolve t, NKMEventDissolve s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventBuff, ref m_listNKMEventBuff, delegate(NKMEventBuff t, NKMEventBuff s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventStatus, ref m_listNKMEventStatus, delegate(NKMEventStatus t, NKMEventStatus s)
		{
			t.DeepCopyFromSource(s);
		});
		NKMUnitState.DeepCopy(source.m_listNKMEventHeal, ref m_listNKMEventHeal, delegate(NKMEventHeal t, NKMEventHeal s)
		{
			t.DeepCopyFromSource(s);
		});
	}
}
