using System;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace NKC.UI.Component;

public class NKCUIComCharacterEmotion : MonoBehaviour
{
	public enum Type
	{
		NONE,
		Angry,
		Annoy,
		Exclamation,
		Flower,
		Gloomy,
		Heart,
		Imagine,
		Laugh,
		Music,
		Question,
		Star,
		Stress,
		Surprise,
		Sweat,
		Talk,
		Warm
	}

	public CanvasGroup m_CanvasGroup;

	public SkeletonGraphic m_Spine;

	public float m_fAnimFadeTime = 0.5f;

	public float m_fAnimPlayTime = 2.5f;

	public void Init()
	{
		if (m_Spine != null)
		{
			m_Spine.Initialize(overwrite: true);
		}
		base.gameObject.SetActive(value: false);
	}

	public void Play(string animName, float speed = 1f)
	{
		if (string.IsNullOrEmpty(animName))
		{
			Stop();
		}
		else if (string.Equals(animName, "NONE", StringComparison.InvariantCultureIgnoreCase) || string.Equals(animName, "STOP", StringComparison.InvariantCultureIgnoreCase))
		{
			Stop();
		}
		else
		{
			SetAnimation(animName, speed);
		}
	}

	public void Play(Type animType, float speed = 1f)
	{
		if (animType == Type.NONE)
		{
			Stop();
		}
		else
		{
			SetAnimation(GetAnimname(animType), speed);
		}
	}

	public void Stop()
	{
		if (m_CanvasGroup != null)
		{
			m_CanvasGroup.DOKill();
			m_CanvasGroup.alpha = 1f;
		}
		base.gameObject.SetActive(value: false);
	}

	private string GetAnimname(Type animType)
	{
		return $"EMO_{animType.ToString().ToUpper()}";
	}

	protected bool HasAnimation(string AnimName)
	{
		if (m_Spine == null)
		{
			return false;
		}
		return m_Spine.SkeletonData.FindAnimation(AnimName) != null;
	}

	protected void SetAnimation(string animName, float speed = 1f)
	{
		if (HasAnimation(animName))
		{
			base.gameObject.SetActive(value: true);
			m_Spine.Skeleton?.SetToSetupPose();
			m_Spine.AnimationState.SetAnimation(0, animName, loop: true).TimeScale = speed;
			if (m_CanvasGroup != null)
			{
				m_CanvasGroup.DOKill();
				m_CanvasGroup.alpha = 1f;
				m_CanvasGroup.DOFade(0f, m_fAnimFadeTime).SetDelay(m_fAnimPlayTime).OnComplete(Stop);
			}
		}
		else
		{
			Debug.LogError("Has no emotion : " + animName);
			Stop();
		}
	}

	protected void AddAnimation(string AnimName, bool loop)
	{
		if (HasAnimation(AnimName))
		{
			m_Spine.AnimationState.AddAnimation(0, AnimName, loop, 0f);
		}
	}

	private void Update()
	{
		if (base.transform.lossyScale.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}
}
