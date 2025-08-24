using System;
using System.Collections.Generic;
using NKC.Office;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCAnimationInstance
{
	public struct EffectData
	{
		public DateTime m_fEffectEndTime;

		public GameObject m_objEffect;
	}

	public INKCAnimationActor m_Actor;

	private GameObject m_targetObj;

	private List<NKCAnimationEventTemplet> m_lstAniEvent = new List<NKCAnimationEventTemplet>();

	private List<bool> m_lstActivated = new List<bool>();

	private Transform m_effectParentTr;

	private Vector3 m_startPos;

	private Vector3 m_endPos;

	private float m_fSpeed;

	private float m_NormalizedPos;

	private Dictionary<int, EffectData> m_dicEffect = new Dictionary<int, EffectData>();

	private int m_iEffectKey;

	private bool m_bForceFinished;

	private bool m_bIsMoveAnimtion;

	private NKCAnimationEventTemplet m_LastAnimationTemplet;

	private float m_Time;

	private int GetEffectKey()
	{
		return ++m_iEffectKey;
	}

	public NKCAnimationInstance(INKCAnimationActor actor, Transform effectParentTr, List<NKCAnimationEventTemplet> lstEventTempletSet, Vector3 startPos, Vector3 endPos)
	{
		m_Actor = actor;
		m_targetObj = null;
		m_effectParentTr = effectParentTr;
		m_startPos = startPos;
		m_endPos = endPos;
		m_lstAniEvent = lstEventTempletSet;
		if (m_lstAniEvent == null)
		{
			m_lstAniEvent = new List<NKCAnimationEventTemplet>();
		}
		m_lstActivated = new List<bool>();
		for (int i = 0; i < m_lstAniEvent.Count; i++)
		{
			m_lstActivated.Add(item: false);
		}
		m_NormalizedPos = 0f;
		m_Time = 0f;
		m_iEffectKey = 0;
		m_bIsMoveAnimtion = IsMovingAnimation();
	}

	private bool IsMovingAnimation()
	{
		foreach (NKCAnimationEventTemplet item in m_lstAniEvent)
		{
			AnimationEventType aniEventType = item.m_AniEventType;
			if ((uint)(aniEventType - 7) <= 1u || aniEventType == AnimationEventType.SET_ABSOLUTE_MOVE_SPEED)
			{
				return true;
			}
		}
		return false;
	}

	public void Update(float deltaTime)
	{
		if (IsFinished())
		{
			return;
		}
		DrawDebugLine(Color.blue);
		m_Time += deltaTime;
		if (m_dicEffect.Count > 0)
		{
			DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
			foreach (KeyValuePair<int, EffectData> item in m_dicEffect)
			{
				if (item.Value.m_fEffectEndTime < serverUTCTime)
				{
					UnityEngine.Object.Destroy(item.Value.m_objEffect);
					m_dicEffect.Remove(item.Key);
					break;
				}
			}
		}
		for (int num = m_lstAniEvent.Count - 1; num >= 0; num--)
		{
			NKCAnimationEventTemplet nKCAnimationEventTemplet = m_lstAniEvent[num];
			if (!m_lstActivated[num] && m_Time >= nKCAnimationEventTemplet.m_StartTime)
			{
				switch (nKCAnimationEventTemplet.m_AniEventType)
				{
				case AnimationEventType.ANIMATION_SPINE:
					if (m_Actor != null)
					{
						bool bDefaultAnim = nKCAnimationEventTemplet.m_StartTime == 0f && nKCAnimationEventTemplet.m_BoolValue;
						m_Actor.PlaySpineAnimation(NKCAnimationEventManager.GetAnimationType(nKCAnimationEventTemplet.m_StrValue), nKCAnimationEventTemplet.m_BoolValue, nKCAnimationEventTemplet.m_FloatValue, bDefaultAnim);
					}
					m_LastAnimationTemplet = nKCAnimationEventTemplet;
					break;
				case AnimationEventType.ANIMATION_NAME_SPINE:
					if (m_Actor != null)
					{
						m_Actor.PlaySpineAnimation(nKCAnimationEventTemplet.m_StrValue, nKCAnimationEventTemplet.m_BoolValue, nKCAnimationEventTemplet.m_FloatValue);
					}
					m_LastAnimationTemplet = nKCAnimationEventTemplet;
					break;
				case AnimationEventType.ANIMATION_UNITY:
					if (m_Actor != null && m_Actor.Animator != null)
					{
						m_Actor.Animator.Play(nKCAnimationEventTemplet.m_StrValue);
						if (nKCAnimationEventTemplet.m_FloatValue != 0f)
						{
							m_Actor.Animator.speed = nKCAnimationEventTemplet.m_FloatValue;
						}
					}
					m_LastAnimationTemplet = nKCAnimationEventTemplet;
					break;
				case AnimationEventType.EFFECT_SPAWN:
				{
					string[] array = nKCAnimationEventTemplet.m_StrValue.Split('@');
					if (array.Length > 1)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(NKCResourceUtility.GetOrLoadAssetResource<GameObject>(array[0], array[1]), m_effectParentTr);
						if (2 < array.Length)
						{
							gameObject.transform.position = m_Actor.GetBonePosition(array[2]);
						}
						else
						{
							gameObject.transform.position = m_Actor.Transform.position;
						}
						if (nKCAnimationEventTemplet.m_FloatValue != 0f)
						{
							gameObject.transform.localScale = Vector3.one * nKCAnimationEventTemplet.m_FloatValue;
						}
						EffectData value = new EffectData
						{
							m_objEffect = gameObject
						};
						if (nKCAnimationEventTemplet.m_FloatValue2 > 0f)
						{
							value.m_fEffectEndTime = NKCSynchronizedTime.GetServerUTCTime().AddSeconds(nKCAnimationEventTemplet.m_FloatValue2);
						}
						else
						{
							value.m_fEffectEndTime = DateTime.MaxValue;
						}
						m_dicEffect.Add(GetEffectKey(), value);
					}
					break;
				}
				case AnimationEventType.EFFECT_SPAWN_FOLLOW:
				{
					string[] array2 = nKCAnimationEventTemplet.m_StrValue.Split('@');
					if (array2.Length > 1)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate(NKCResourceUtility.GetOrLoadAssetResource<GameObject>(array2[0], array2[1]), m_Actor.SDParent);
						if (2 < array2.Length)
						{
							gameObject2.transform.position = m_Actor.GetBonePosition(array2[2]);
						}
						if (nKCAnimationEventTemplet.m_FloatValue != 0f)
						{
							gameObject2.transform.localScale = Vector3.one * nKCAnimationEventTemplet.m_FloatValue;
						}
						EffectData value2 = new EffectData
						{
							m_objEffect = gameObject2
						};
						if (nKCAnimationEventTemplet.m_FloatValue2 > 0f)
						{
							value2.m_fEffectEndTime = NKCSynchronizedTime.GetServerUTCTime().AddSeconds(nKCAnimationEventTemplet.m_FloatValue2);
						}
						else
						{
							value2.m_fEffectEndTime = DateTime.MaxValue;
						}
						m_dicEffect.Add(GetEffectKey(), value2);
					}
					break;
				}
				case AnimationEventType.PLAY_EMOTION:
					if (m_Actor != null && (nKCAnimationEventTemplet.m_FloatValue2 == 0f || NKMRandom.Range(0f, 1f) <= nKCAnimationEventTemplet.m_FloatValue2))
					{
						m_Actor.PlayEmotion(nKCAnimationEventTemplet.m_StrValue, nKCAnimationEventTemplet.m_FloatValue);
					}
					break;
				case AnimationEventType.OFFICE_CHAR_SHADOW:
					if (m_Actor != null && m_Actor.Transform != null)
					{
						NKCOfficeCharacter component = m_Actor.Transform.GetComponent<NKCOfficeCharacter>();
						if (component != null)
						{
							component.SetShadow(nKCAnimationEventTemplet.m_BoolValue);
						}
					}
					break;
				case AnimationEventType.SET_MOVE_SPEED:
					m_fSpeed = nKCAnimationEventTemplet.m_FloatValue;
					break;
				case AnimationEventType.SET_ABSOLUTE_MOVE_SPEED:
				{
					float magnitude = (m_endPos - m_startPos).magnitude;
					if (magnitude == 0f)
					{
						m_fSpeed = 100f;
					}
					else
					{
						m_fSpeed = nKCAnimationEventTemplet.m_FloatValue / magnitude;
					}
					break;
				}
				case AnimationEventType.SET_POSITION:
					m_NormalizedPos = nKCAnimationEventTemplet.m_FloatValue;
					break;
				case AnimationEventType.PLAY_SOUND:
					if (nKCAnimationEventTemplet.m_FloatValue == 0f || NKMRandom.Range(0f, 1f) <= nKCAnimationEventTemplet.m_FloatValue)
					{
						NKCSoundManager.PlaySound(nKCAnimationEventTemplet.m_StrValue, 1f, 0f, 0f);
					}
					break;
				case AnimationEventType.INVERT_MODEL_X:
				{
					bool boolValue = nKCAnimationEventTemplet.m_BoolValue;
					m_Actor.Transform.localScale = new Vector3(boolValue ? (-1f) : (1f * Mathf.Abs(m_Actor.Transform.localScale.x)), m_Actor.Transform.localScale.y, m_Actor.Transform.localScale.z);
					break;
				}
				case AnimationEventType.INVERT_MODEL_X_BY_DIRECTION:
				{
					Vector3 vector = m_Actor.Transform.parent.TransformPoint(m_startPos);
					bool flag = m_Actor.Transform.parent.TransformPoint(m_endPos).x < vector.x == nKCAnimationEventTemplet.m_BoolValue;
					m_Actor.Transform.localScale = new Vector3(flag ? (-1f) : (1f * Mathf.Abs(m_Actor.Transform.localScale.x)), m_Actor.Transform.localScale.y, m_Actor.Transform.localScale.z);
					break;
				}
				case AnimationEventType.FLIP_MODEL_X:
					m_Actor.Transform.localScale = new Vector3(m_Actor.Transform.localScale.x * -1f, m_Actor.Transform.localScale.y, m_Actor.Transform.localScale.z);
					break;
				case AnimationEventType.FINISH_EVENT:
					m_bForceFinished = true;
					break;
				}
				m_lstActivated[num] = true;
			}
		}
		if (m_Actor != null && m_bIsMoveAnimtion)
		{
			m_NormalizedPos += m_fSpeed * deltaTime;
			if (m_NormalizedPos > 1f)
			{
				m_NormalizedPos = 1f;
			}
			m_Actor.Transform.localPosition = NKCUtil.Lerp(m_startPos, m_endPos, m_NormalizedPos);
		}
	}

	public bool IsFinished()
	{
		if (m_bForceFinished)
		{
			return true;
		}
		for (int i = 0; i < m_lstAniEvent.Count; i++)
		{
			if (!m_lstActivated[i])
			{
				return false;
			}
		}
		if (m_bIsMoveAnimtion)
		{
			if (m_NormalizedPos < 1f)
			{
				return false;
			}
		}
		else if (m_LastAnimationTemplet != null && !m_LastAnimationTemplet.m_BoolValue)
		{
			switch (m_LastAnimationTemplet.m_AniEventType)
			{
			case AnimationEventType.ANIMATION_NAME_SPINE:
				if (m_Actor != null && !m_Actor.IsSpineAnimationFinished(m_LastAnimationTemplet.m_StrValue))
				{
					return false;
				}
				break;
			case AnimationEventType.ANIMATION_SPINE:
				if (m_Actor != null && !m_Actor.IsSpineAnimationFinished(NKCAnimationEventManager.GetAnimationType(m_LastAnimationTemplet.m_StrValue)))
				{
					return false;
				}
				break;
			case AnimationEventType.ANIMATION_UNITY:
				if (m_Actor != null && m_Actor.Animator != null && m_Actor.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
				{
					return false;
				}
				break;
			}
		}
		return true;
	}

	public void RemoveEffect()
	{
		foreach (KeyValuePair<int, EffectData> item in m_dicEffect)
		{
			UnityEngine.Object.Destroy(item.Value.m_objEffect);
		}
		m_dicEffect.Clear();
		m_iEffectKey = 0;
	}

	public void Close()
	{
		if (m_targetObj != null)
		{
			if (m_targetObj.GetComponent<NKCASUISpineIllust>() != null)
			{
				m_targetObj.GetComponent<NKCASUISpineIllust>().Unload();
			}
			else
			{
				UnityEngine.Object.Destroy(m_targetObj);
			}
			m_targetObj = null;
		}
		RemoveEffect();
	}

	public float GetTotalEventAnimationTime()
	{
		List<NKCAnimationEventTemplet> list = m_lstAniEvent.FindAll((NKCAnimationEventTemplet x) => x.m_AniEventType == AnimationEventType.SET_MOVE_SPEED);
		list.Sort(CompByStartTime);
		float num = 0f;
		float num2 = 0f;
		float num3 = 1f;
		if (list.Count > 0)
		{
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				if (num4 == 0)
				{
					num2 += list[0].m_StartTime * m_fSpeed;
					num += list[0].m_StartTime;
					num3 = 1f - num2;
				}
				else
				{
					num2 += (list[num4].m_StartTime - list[num4 - 1].m_StartTime) * list[num4 - 1].m_FloatValue;
					num = list[num4].m_StartTime;
					num3 = 1f - num2;
				}
				if (num4 == list.Count - 1)
				{
					num += num3 / list[num4].m_FloatValue;
				}
			}
		}
		return num;
	}

	private int CompByStartTime(NKCAnimationEventTemplet left, NKCAnimationEventTemplet right)
	{
		return left.m_StartTime.CompareTo(right.m_StartTime);
	}

	public void DrawDebugLine(Color color)
	{
		if (m_Actor != null && m_Actor.Transform.parent != null)
		{
			Debug.DrawLine(m_Actor.Transform.parent.TransformPoint(m_startPos), m_Actor.Transform.parent.TransformPoint(m_endPos), color);
		}
	}
}
