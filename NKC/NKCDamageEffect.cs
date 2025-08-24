using System;
using System.Collections.Generic;
using NKC.FX;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCDamageEffect : NKMDamageEffect
{
	private NKCGameClient m_NKCGameClient;

	private NKCASEffect m_NKCASEffect;

	private NKCASUnitShadow m_NKCASUnitShadow;

	private List<NKCASEffect> m_listEffect = new List<NKCASEffect>();

	private List<int> m_listSoundUID = new List<int>();

	private bool m_bDissolveEnable;

	private NKMTrackingFloat m_DissolveFactor = new NKMTrackingFloat();

	private Vector3 m_Vector3Temp = new Vector3(0f, 0f, 0f);

	private Vector3 m_Vector3Temp2 = new Vector3(0f, 0f, 0f);

	private Color m_ColorTemp;

	public NKCDamageEffect()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCDamageEffect;
		m_NKM_DAMAGE_EFFECT_CLASS_TYPE = NKM_DAMAGE_EFFECT_CLASS_TYPE.NDECT_NKC;
		InitShadow();
	}

	private void InitShadow()
	{
		m_NKCASUnitShadow = (NKCASUnitShadow)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitShadow);
		if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null))
		{
			m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_GAME_BATTLE_UNIT_SHADOW()
				.transform, worldPositionStays: false);
				NKCUtil.SetGameObjectLocalPos(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant, 0f, 0f, 0f);
				if (m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.activeSelf)
				{
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.SetActive(value: false);
				}
			}
		}

		public override void Open()
		{
			base.Open();
			m_DissolveFactor.SetNowValue(0f);
		}

		public override void Close()
		{
			base.Close();
			if (m_NKCASEffect != null)
			{
				m_NKCGameClient.GetNKCEffectManager().DeleteEffect(m_NKCASEffect.m_EffectUID);
			}
			if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant != null && m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.activeSelf)
			{
				m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.SetActive(value: false);
			}
			for (int i = 0; i < m_listEffect.Count; i++)
			{
				NKCASEffect nKCASEffect = m_listEffect[i];
				nKCASEffect.Stop();
				nKCASEffect.m_bAutoDie = true;
			}
			m_listEffect.Clear();
		}

		public override void Unload()
		{
			StopSound();
			m_listEffect.Clear();
			m_NKCASEffect = null;
			m_NKCASUnitShadow.Unload();
			m_NKCASUnitShadow = null;
			m_NKCGameClient = null;
			base.Unload();
		}

		public override bool SetDamageEffect(NKMGame cNKMGame, NKMDamageEffectManager cDEManager, NKMUnitSkillTemplet cUnitSkillTemplet, int masterUnitPhase, short deUID, string deTempletID, short masterUID, short targetUID, float fX, float fY, float fZ, bool bRight, NKMEventPosData.MoveOffset moveOffset, float fPosMapRate, float offsetX = 0f, float offsetY = 0f, float offsetZ = 0f, float fAddRotate = 0f, bool bUseZScale = true, float fSpeedFactorX = 0f, float fSpeedFactorY = 0f)
		{
			m_NKCGameClient = (NKCGameClient)cNKMGame;
			if (!base.SetDamageEffect(cNKMGame, cDEManager, cUnitSkillTemplet, masterUnitPhase, deUID, deTempletID, masterUID, targetUID, fX, fY, fZ, bRight, moveOffset, fPosMapRate, offsetX, offsetY, offsetZ, fAddRotate, bUseZScale, fSpeedFactorX, fSpeedFactorY))
			{
				return false;
			}
			NKMUnit unit = cNKMGame.GetUnit(masterUID);
			string text = ((unit == null || unit.GetUnitData() == null) ? m_DETemplet.m_MainEffectName : m_DETemplet.GetMainEffectName(unit.GetUnitData().m_SkinID));
			float num = 1f;
			if (m_DETemplet.m_bUseTargetBuffScaleFactor)
			{
				NKMUnit unit2 = cNKMGame.GetUnit(targetUID);
				if (unit2 != null)
				{
					num = unit2.GetUnitTemplet().m_fBuffEffectScaleFactor;
				}
			}
			m_NKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(masterUID, text, text, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_DEData.m_PosX, m_DEData.m_PosZ + m_DEData.m_JumpYPos, m_DEData.m_PosZ, bRight, m_DETemplet.m_fScaleFactor * num, 0f, 0f, 0f, m_bUseZtoY: false, m_DEData.m_fAddRotate, m_DEData.m_bUseZScale, "", bUseBoneRotate: false, bAutoDie: false, "", 1f, bNotStart: false, bCutIn: false, 0f, -1f, bDEEffect: true);
			if (m_NKCASEffect != null && m_DETemplet != null)
			{
				m_NKCASEffect.SetCanIgnoreStopTime(m_DETemplet.m_CanIgnoreStopTime);
				m_NKCASEffect.SetUseMasterAnimSpeed(m_DETemplet.m_UseMasterAnimSpeed);
			}
			SetShadow();
			SetSpecialProperty();
			return true;
		}

		private void SetShadow()
		{
			if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null))
			{
				if (!m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.activeSelf)
				{
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.SetActive(value: true);
				}
				m_Vector3Temp.Set(m_DETemplet.m_fShadowScaleX, m_DETemplet.m_fShadowScaleY, 1f);
				m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localScale = m_Vector3Temp;
				m_Vector3Temp.Set(m_DEData.m_PosX, m_DEData.m_PosZ, m_DEData.m_PosZ);
				m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localPosition = m_Vector3Temp;
				bool bTeamA = true;
				bool bRearm = false;
				if (m_DEData.m_MasterUnit != null)
				{
					bTeamA = !m_NKCGameClient.IsEnemy(m_NKCGameClient.m_MyTeam, m_DEData.m_MasterUnit.GetUnitDataGame().m_NKM_TEAM_TYPE_ORG);
					bRearm = m_DEData.m_MasterUnit.GetUnitTemplet().m_UnitTempletBase.IsRearmUnit;
				}
				m_NKCASUnitShadow.SetShadowType(m_DETemplet.m_NKC_TEAM_COLOR_TYPE, bTeamA, bRearm);
			}
		}

		private void SetSpecialProperty()
		{
			IFxProperty[] componentsInChildren = m_NKCASEffect.m_EffectInstant.m_Instant.GetComponentsInChildren<IFxProperty>(includeInactive: true);
			if (componentsInChildren != null)
			{
				NKCUnitClient masterUnit = GetMasterUnit() as NKCUnitClient;
				NKCUnitClient targetUnit = GetTargetUnit() as NKCUnitClient;
				IFxProperty[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetFxProperty(masterUnit, targetUnit, m_DEData);
				}
			}
		}

		protected void OnChangeTarget()
		{
			SetSpecialProperty();
		}

		protected override void StateStart()
		{
			if (m_EffectStateNow != null)
			{
				StopSound();
				base.StateStart();
				if (m_EffectStateNow.m_AnimName.Length > 1)
				{
					m_NKCASEffect?.PlayAnim(m_EffectStateNow.m_AnimName, m_EffectStateNow.m_bAnimLoop, m_EffectStateNow.m_fAnimSpeed);
				}
			}
		}

		protected void StopSound()
		{
			for (int i = 0; i < m_listSoundUID.Count; i++)
			{
				NKCSoundManager.StopSound(m_listSoundUID[i]);
			}
			m_listSoundUID.Clear();
		}

		protected override void StateUpdate()
		{
			if (m_EffectStateNow != null)
			{
				base.StateUpdate();
				UpdateEffect();
			}
		}

		protected float GetLookDir()
		{
			m_NKMVector3Temp1.x = m_DEData.m_DirVector.x;
			m_NKMVector3Temp1.y = m_DEData.m_DirVector.y;
			m_NKMVector3Temp1.z = m_DEData.m_DirVector.z;
			m_NKMVector3Temp1.Normalize();
			if (Math.Abs(m_NKMVector3Temp1.y) <= Math.Abs(m_NKMVector3Temp1.z))
			{
				m_NKMVector3Temp1.y = m_NKMVector3Temp1.z;
			}
			m_NKMVector3Temp1.z = 0f;
			m_NKMVector3Temp1.Normalize();
			return (float)Math.Atan2(m_NKMVector3Temp1.y, m_NKMVector3Temp1.x) * 180f / 3.14159f;
		}

		private void UpdateEffect()
		{
			m_NKCASEffect?.SetPos(m_DEData.m_PosX, m_DEData.m_PosZ + m_DEData.m_JumpYPos, m_DEData.m_PosZ);
			if (m_DETemplet.m_bLookDir)
			{
				m_NKCASEffect?.SetLookDir(GetLookDir());
			}
			else
			{
				m_NKCASEffect?.SetRight(m_DEData.m_bRight);
			}
			UpdateShadow();
		}

		private void UpdateShadow()
		{
			if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null))
			{
				if (m_DEData.m_bRight)
				{
					m_Vector3Temp.Set(m_DETemplet.m_fShadowRotateX, 180f, m_DETemplet.m_fShadowRotateZ);
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localEulerAngles = m_Vector3Temp;
				}
				else
				{
					m_Vector3Temp.Set(m_DETemplet.m_fShadowRotateX, 0f, m_DETemplet.m_fShadowRotateZ);
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localEulerAngles = m_Vector3Temp;
				}
				m_Vector3Temp.Set(m_DEData.m_PosX, m_DEData.m_PosZ, m_DEData.m_PosZ);
				m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localPosition = m_Vector3Temp;
				float num = 1f - 0.2f * m_DEData.m_JumpYPos * 0.01f;
				if (num < 0.3f)
				{
					num = 0.3f;
				}
				NKCUtil.SetGameObjectLocalScale(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant, m_DETemplet.m_fShadowScaleX * num, m_DETemplet.m_fShadowScaleY * num, 1f);
			}
		}

		public override void SetDie(bool bForce = false, bool bDieEvent = true)
		{
			base.SetDie(bForce, bDieEvent);
		}

		protected override void ProcessEventAttack()
		{
			if (m_EffectStateNow != null && m_DEData.m_DamageCountNow < m_DETemplet.m_DamageCountMax)
			{
				base.ProcessEventAttack();
			}
		}

		protected override void ProcessAttackHitEffect(NKMEventAttack cNKMEventAttack)
		{
			if (cNKMEventAttack != null)
			{
				m_Vector3Temp.Set(m_DEData.m_PosX, m_DEData.m_PosZ + m_DEData.m_JumpYPos, m_DEData.m_PosZ);
				NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(m_DEData.m_MasterGameUnitUID, cNKMEventAttack.m_EffectName, cNKMEventAttack.m_EffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_Vector3Temp.x, m_Vector3Temp.y, m_Vector3Temp.z, m_DEData.m_bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, "", 1f, bNotStart: false, bCutIn: false, 0f, -1f, bDEEffect: true);
				if (nKCASEffect != null && m_DETemplet != null)
				{
					nKCASEffect.SetCanIgnoreStopTime(m_DETemplet.m_CanIgnoreStopTime);
					nKCASEffect.SetUseMasterAnimSpeed(m_DETemplet.m_UseMasterAnimSpeed);
				}
			}
		}

		protected override void ProcessEventEffect(bool bStateEnd = false)
		{
			if (m_EffectStateNow == null)
			{
				return;
			}
			if (bStateEnd)
			{
				for (int i = 0; i < m_listEffect.Count; i++)
				{
					NKCASEffect nKCASEffect = m_listEffect[i];
					if (nKCASEffect.m_bStateEndStop && m_NKCGameClient.GetNKCEffectManager().IsLiveEffect(nKCASEffect.m_EffectUID))
					{
						nKCASEffect.Stop(nKCASEffect.m_bStateEndStopForce);
					}
				}
			}
			else
			{
				for (int j = 0; j < m_listEffect.Count; j++)
				{
					NKCASEffect nKCASEffect2 = m_listEffect[j];
					if (!m_NKCGameClient.GetNKCEffectManager().IsLiveEffect(nKCASEffect2.m_EffectUID))
					{
						m_listEffect.RemoveAt(j);
						j--;
					}
					else if (m_NKCASEffect != null && nKCASEffect2.m_BoneName.Length > 1)
					{
						Transform transform = m_NKCASEffect.m_EffectInstant.GetTransform(nKCASEffect2.m_BoneName);
						nKCASEffect2.SetPos(transform.position.x, transform.position.y, transform.position.z);
						if (nKCASEffect2.m_bUseBoneRotate)
						{
							nKCASEffect2.m_EffectInstant.m_Instant.transform.eulerAngles = transform.eulerAngles;
						}
					}
					else
					{
						nKCASEffect2.SetRight(m_DEData.m_bRight);
						nKCASEffect2.SetPos(m_DEData.m_PosX, m_DEData.m_PosZ + m_DEData.m_JumpYPos, m_DEData.m_PosZ);
					}
				}
			}
			for (int k = 0; k < m_EffectStateNow.m_listNKMEventEffect.Count; k++)
			{
				NKMEventEffect nKMEventEffect = m_EffectStateNow.m_listNKMEventEffect[k];
				if (nKMEventEffect != null && CheckEventCondition(nKMEventEffect.m_Condition) && nKMEventEffect.IsRightSkin(m_DEData.m_MasterUnit.GetUnitData().m_SkinID))
				{
					bool flag = false;
					if (nKMEventEffect.m_bStateEndTime && bStateEnd)
					{
						flag = true;
					}
					else if (EventTimer(nKMEventEffect.m_bAnimTime, nKMEventEffect.m_fEventTime, bOneTime: true) && !nKMEventEffect.m_bStateEndTime)
					{
						flag = true;
					}
					if (flag)
					{
						ApplyEventEffect(nKMEventEffect);
					}
				}
			}
		}

		public override void ApplyEventEffect(NKMEventEffect cNKMEventEffect)
		{
			m_Vector3Temp2.Set(cNKMEventEffect.m_OffsetX, cNKMEventEffect.m_OffsetY, cNKMEventEffect.m_OffsetZ);
			if (cNKMEventEffect.m_bFixedPos)
			{
				m_Vector3Temp.Set(cNKMEventEffect.m_OffsetX, cNKMEventEffect.m_OffsetY, cNKMEventEffect.m_OffsetZ);
				m_Vector3Temp2.Set(0f, 0f, 0f);
			}
			else if (m_NKCASEffect != null && cNKMEventEffect.m_BoneName.Length > 1)
			{
				Transform transform = m_NKCASEffect.m_EffectInstant.GetTransform(cNKMEventEffect.m_BoneName);
				m_Vector3Temp.Set(transform.position.x, transform.position.y, transform.position.z);
			}
			else
			{
				m_Vector3Temp.Set(m_DEData.m_PosX, m_DEData.m_PosZ + m_DEData.m_JumpYPos, m_DEData.m_PosZ);
				if (cNKMEventEffect.m_bLandConnect)
				{
					m_Vector3Temp.y = m_DEData.m_PosZ;
				}
			}
			bool bRight = m_DEData.m_bRight;
			if (cNKMEventEffect.m_bForceRight)
			{
				bRight = true;
			}
			string effectName = cNKMEventEffect.GetEffectName(m_DEData);
			NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(m_DEData.m_MasterGameUnitUID, effectName, effectName, cNKMEventEffect.m_ParentType, m_Vector3Temp.x, m_Vector3Temp.y, m_Vector3Temp.z, bRight, cNKMEventEffect.m_fScaleFactor, m_Vector3Temp2.x, m_Vector3Temp2.y, m_Vector3Temp2.z, cNKMEventEffect.m_bUseOffsetZtoY, cNKMEventEffect.m_fAddRotate, cNKMEventEffect.m_bUseZScale, cNKMEventEffect.m_BoneName, cNKMEventEffect.m_bUseBoneRotate, bAutoDie: true, cNKMEventEffect.m_AnimName, cNKMEventEffect.m_fAnimSpeed, bNotStart: false, cNKMEventEffect.m_bCutIn, cNKMEventEffect.m_fReserveTime, -1f, bDEEffect: true);
			if (nKCASEffect != null && cNKMEventEffect != null)
			{
				if (cNKMEventEffect.m_bHold || cNKMEventEffect.m_bStateEndStop)
				{
					nKCASEffect.m_bStateEndStop = cNKMEventEffect.m_bStateEndStop;
					nKCASEffect.m_bStateEndStopForce = cNKMEventEffect.m_bStateEndStopForce;
					m_listEffect.Add(nKCASEffect);
				}
				if (m_DETemplet != null)
				{
					nKCASEffect.SetCanIgnoreStopTime(m_DETemplet.m_CanIgnoreStopTime);
					nKCASEffect.SetUseMasterAnimSpeed(m_DETemplet.m_UseMasterAnimSpeed);
				}
				nKCASEffect.SetApplyStopTime(cNKMEventEffect.m_ApplyStopTime);
			}
		}

		protected override void ProcessEventSound(bool bStateEnd = false)
		{
			if (m_EffectStateNow == null)
			{
				return;
			}
			for (int i = 0; i < m_EffectStateNow.m_listNKMEventSound.Count; i++)
			{
				NKMEventSound nKMEventSound = m_EffectStateNow.m_listNKMEventSound[i];
				if (nKMEventSound != null && CheckEventCondition(nKMEventSound.m_Condition) && (m_DEData == null || nKMEventSound.IsRightSkin(m_DEData.GetMasterSkinID())))
				{
					bool bOneTime = true;
					if (m_EffectStateNow.m_bAnimLoop)
					{
						bOneTime = false;
					}
					bool flag = false;
					if (nKMEventSound.m_bStateEndTime && bStateEnd)
					{
						flag = true;
					}
					else if (EventTimer(nKMEventSound.m_bAnimTime, nKMEventSound.m_fEventTime, bOneTime) && !nKMEventSound.m_bStateEndTime)
					{
						flag = true;
					}
					if (flag)
					{
						ApplyEventSound(nKMEventSound);
					}
				}
			}
		}

		public override void ApplyEventSound(NKMEventSound cNKMEventSound)
		{
			if (NKMRandom.Range(0f, 1f) <= cNKMEventSound.m_PlayRate && cNKMEventSound.GetRandomSound(m_DEData, out var soundName))
			{
				int item = NKCSoundManager.PlaySound(soundName, cNKMEventSound.m_fLocalVol, m_DEData.m_PosX, cNKMEventSound.m_fFocusRange, cNKMEventSound.m_bLoop);
				if (cNKMEventSound.m_bStopSound)
				{
					m_listSoundUID.Add(item);
				}
			}
		}

		protected override void ProcessEventCameraCrash(bool bStateEnd = false)
		{
			if (m_EffectStateNow == null)
			{
				return;
			}
			for (int i = 0; i < m_EffectStateNow.m_listNKMEventCameraCrash.Count; i++)
			{
				NKMEventCameraCrash nKMEventCameraCrash = m_EffectStateNow.m_listNKMEventCameraCrash[i];
				if (nKMEventCameraCrash == null || !CheckEventCondition(nKMEventCameraCrash.m_Condition))
				{
					continue;
				}
				bool flag = false;
				if (nKMEventCameraCrash.m_bStateEndTime && bStateEnd)
				{
					flag = true;
				}
				else if (EventTimer(nKMEventCameraCrash.m_bAnimTime, nKMEventCameraCrash.m_fEventTime, bOneTime: true) && !nKMEventCameraCrash.m_bStateEndTime)
				{
					flag = true;
				}
				if (flag && (nKMEventCameraCrash.m_fCrashRadius <= 0f || NKCCamera.GetDist(this) <= nKMEventCameraCrash.m_fCrashRadius))
				{
					switch (nKMEventCameraCrash.m_CameraCrashType)
					{
					case NKM_CAMERA_CRASH_TYPE.NCCT_UP:
						NKCCamera.UpCrashCamera(nKMEventCameraCrash.m_fCameraCrashSpeed, nKMEventCameraCrash.m_fCameraCrashAccel);
						break;
					case NKM_CAMERA_CRASH_TYPE.NCCT_DOWN:
						NKCCamera.DownCrashCamera(nKMEventCameraCrash.m_fCameraCrashSpeed, nKMEventCameraCrash.m_fCameraCrashAccel);
						break;
					case NKM_CAMERA_CRASH_TYPE.NCCT_UP_DOWN:
						NKCCamera.UpDownCrashCamera(nKMEventCameraCrash.m_fCameraCrashGap, nKMEventCameraCrash.m_fCameraCrashTime);
						break;
					case NKM_CAMERA_CRASH_TYPE.NCCT_UP_DOWN_NO_RESET:
						NKCCamera.UpDownCrashCameraNoReset(nKMEventCameraCrash.m_fCameraCrashGap, nKMEventCameraCrash.m_fCameraCrashTime);
						break;
					}
				}
			}
		}

		protected override void ProcessEventDissolve(bool bStateEnd = false)
		{
			if (m_EffectStateNow == null)
			{
				return;
			}
			m_DissolveFactor.Update(m_fDeltaTime);
			if (m_NKCASEffect == null || !m_NKCASEffect.m_bSpine)
			{
				return;
			}
			if (m_DissolveFactor.IsTracking())
			{
				m_NKCASEffect.SetDissolveBlend(m_DissolveFactor.GetNowValue());
			}
			else if (m_DissolveFactor.GetNowValue() <= 0f && m_bDissolveEnable)
			{
				m_bDissolveEnable = false;
				m_NKCASEffect.SetDissolveBlend(0f);
				m_NKCASEffect.SetDissolveOn(m_bDissolveEnable);
			}
			for (int i = 0; i < m_EffectStateNow.m_listNKMEventDissolve.Count; i++)
			{
				NKMEventDissolve nKMEventDissolve = m_EffectStateNow.m_listNKMEventDissolve[i];
				if (nKMEventDissolve != null && CheckEventCondition(nKMEventDissolve.m_Condition))
				{
					bool flag = false;
					if (nKMEventDissolve.m_bStateEndTime && bStateEnd)
					{
						flag = true;
					}
					else if (EventTimer(nKMEventDissolve.m_bAnimTime, nKMEventDissolve.m_fEventTime, bOneTime: true) && !nKMEventDissolve.m_bStateEndTime)
					{
						flag = true;
					}
					if (flag)
					{
						ApplyEventDissolve(nKMEventDissolve);
					}
				}
			}
		}

		public override void ApplyEventDissolve(NKMEventDissolve cNKMEventDissolve)
		{
			if (!m_bDissolveEnable)
			{
				m_bDissolveEnable = true;
				m_NKCASEffect.SetDissolveOn(m_bDissolveEnable);
				m_ColorTemp.r = cNKMEventDissolve.m_fColorR;
				m_ColorTemp.g = cNKMEventDissolve.m_fColorG;
				m_ColorTemp.b = cNKMEventDissolve.m_fColorB;
				m_ColorTemp.a = 1f;
				m_NKCASEffect.SetDissolveColor(m_ColorTemp);
			}
			m_DissolveFactor.SetTracking(cNKMEventDissolve.m_fDissolve, cNKMEventDissolve.m_fTrackTime, TRACKING_DATA_TYPE.TDT_NORMAL);
		}

		protected override void ProcessDieEventEffect()
		{
			for (int i = 0; i < m_DETemplet.m_listNKMDieEventEffect.Count; i++)
			{
				NKMEventEffect nKMEventEffect = m_DETemplet.m_listNKMDieEventEffect[i];
				if (nKMEventEffect == null)
				{
					continue;
				}
				if (m_NKCASEffect != null && nKMEventEffect.m_BoneName.Length > 1)
				{
					Transform transform = m_NKCASEffect.m_EffectInstant.GetTransform(nKMEventEffect.m_BoneName);
					m_Vector3Temp.Set(transform.position.x, transform.position.y, transform.position.z);
				}
				else
				{
					m_Vector3Temp.Set(m_DEData.m_PosX, m_DEData.m_PosZ + m_DEData.m_JumpYPos, m_DEData.m_PosZ);
					if (nKMEventEffect.m_bLandConnect)
					{
						m_Vector3Temp.y = m_DEData.m_PosZ;
					}
				}
				string effectName = nKMEventEffect.GetEffectName(m_DEData);
				NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(m_DEData.m_MasterGameUnitUID, effectName, effectName, nKMEventEffect.m_ParentType, m_Vector3Temp.x, m_Vector3Temp.y, m_Vector3Temp.z, m_DEData.m_bRight, nKMEventEffect.m_fScaleFactor, nKMEventEffect.m_OffsetX, nKMEventEffect.m_OffsetY, nKMEventEffect.m_OffsetZ, nKMEventEffect.m_bUseOffsetZtoY, nKMEventEffect.m_fAddRotate, nKMEventEffect.m_bUseZScale, nKMEventEffect.m_BoneName, nKMEventEffect.m_bUseBoneRotate, bAutoDie: true, nKMEventEffect.m_AnimName, 1f, bNotStart: false, nKMEventEffect.m_bCutIn, nKMEventEffect.m_fReserveTime, -1f, bDEEffect: true);
				if (nKCASEffect != null && m_DETemplet != null)
				{
					nKCASEffect.SetCanIgnoreStopTime(m_DETemplet.m_CanIgnoreStopTime);
					nKCASEffect.SetUseMasterAnimSpeed(m_DETemplet.m_UseMasterAnimSpeed);
				}
			}
		}

		protected override void ProcessDieEventSound()
		{
			for (int i = 0; i < m_DETemplet.m_listNKMDieEventSound.Count; i++)
			{
				NKMEventSound nKMEventSound = m_DETemplet.m_listNKMDieEventSound[i];
				if (nKMEventSound != null && CheckEventCondition(nKMEventSound.m_Condition) && (m_DEData == null || nKMEventSound.IsRightSkin(m_DEData.GetMasterSkinID())) && NKMRandom.Range(0f, 1f) <= nKMEventSound.m_PlayRate && nKMEventSound.GetRandomSound(m_DEData, out var soundName))
				{
					int num = 0;
					num = (nKMEventSound.m_bVoice ? NKCSoundManager.PlayVoice(soundName, 0, bClearVoice: true, bIgnoreSameVoice: true, nKMEventSound.m_fLocalVol, m_DEData.m_PosX, nKMEventSound.m_fFocusRange, nKMEventSound.m_bLoop) : NKCSoundManager.PlaySound(soundName, nKMEventSound.m_fLocalVol, m_DEData.m_PosX, nKMEventSound.m_fFocusRange, nKMEventSound.m_bLoop));
					if (nKMEventSound.m_bStopSound)
					{
						m_listSoundUID.Add(num);
					}
				}
			}
		}

		protected override void ProcessEventBuff(bool bStateEnd = false)
		{
		}

		protected override void ProcessEventStatus(bool bStateEnd = false)
		{
		}

		public override float GetBasePosX(NKMEventPosData.MoveBase moveBase, NKMEventPosData.MoveBaseType moveBaseType, bool isATeam, float mapPosFactor, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraParams)
		{
			if (m_NKCGameClient.IsReversePosTeam(m_NKCGameClient.m_MyTeam))
			{
				isATeam = !isATeam;
			}
			return base.GetBasePosX(moveBase, moveBaseType, isATeam, mapPosFactor, extraParams);
		}

		public override bool GetOffsetDirRight(NKMEventPosData.MoveOffset offsetType, float basePos, bool isATeam, float mapPosFactor, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraParams)
		{
			if (m_NKCGameClient.IsReversePosTeam(m_NKCGameClient.m_MyTeam))
			{
				isATeam = !isATeam;
			}
			return base.GetOffsetDirRight(offsetType, basePos, isATeam, mapPosFactor, extraParams);
		}
	}
