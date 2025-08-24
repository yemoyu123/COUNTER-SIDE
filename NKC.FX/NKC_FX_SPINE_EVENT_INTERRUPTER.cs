using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.FX;

public class NKC_FX_SPINE_EVENT_INTERRUPTER : MonoBehaviour
{
	public GameObject TargetSpine;

	public List<string> IgnoreAnimationName = new List<string>();

	public UnityEvent Event;

	private SkeletonAnimation skeletonAnimation;

	private SkeletonGraphic skeletonGraphic;

	private string sameAnimationName = "";

	private bool ignored;

	private void OnDestroy()
	{
		if (skeletonAnimation != null)
		{
			CleanupSkeletonAnimation();
		}
		if (skeletonGraphic != null)
		{
			CleanupSkeletonGraphic();
		}
		if (TargetSpine != null)
		{
			TargetSpine = null;
		}
		if (Event != null)
		{
			Event = null;
		}
		if (skeletonAnimation != null)
		{
			skeletonAnimation = null;
		}
		if (skeletonGraphic != null)
		{
			skeletonGraphic = null;
		}
	}

	private void Awake()
	{
		skeletonAnimation = TargetSpine.GetComponent<SkeletonAnimation>();
		if (skeletonAnimation == null)
		{
			skeletonGraphic = TargetSpine.GetComponent<SkeletonGraphic>();
		}
		else
		{
			skeletonAnimation.Awake();
		}
	}

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (skeletonAnimation != null)
		{
			InitSkeletonAnimation();
		}
		else if (skeletonGraphic != null)
		{
			InitSkeletonGraphic();
		}
	}

	private void InitSkeletonAnimation()
	{
		skeletonAnimation.state.Interrupt += OnAnimationInterrupt;
		skeletonAnimation.state.Start += OnSkeletonAnimationStart;
	}

	private void CleanupSkeletonAnimation()
	{
		skeletonAnimation.state.Interrupt -= OnAnimationInterrupt;
		skeletonAnimation.state.Start -= OnSkeletonAnimationStart;
	}

	private void InitSkeletonGraphic()
	{
		skeletonGraphic.AnimationState.Interrupt += OnAnimationInterrupt;
		skeletonGraphic.AnimationState.Start += OnSkeletonGraphicStart;
	}

	private void CleanupSkeletonGraphic()
	{
		skeletonGraphic.AnimationState.Interrupt -= OnAnimationInterrupt;
		skeletonGraphic.AnimationState.Start -= OnSkeletonGraphicStart;
	}

	private void OnAnimationInterrupt(TrackEntry entry)
	{
		sameAnimationName = entry.Animation.Name;
	}

	private void OnSkeletonAnimationStart(TrackEntry entry)
	{
		ignored = IgnoreAnimationName.Contains(entry.Animation.Name);
		if (!ignored && sameAnimationName != entry.Animation.Name)
		{
			Event.Invoke();
		}
	}

	private void OnSkeletonGraphicStart(TrackEntry entry)
	{
		ignored = IgnoreAnimationName.Contains(entry.Animation.Name);
		if (!ignored)
		{
			Event.Invoke();
		}
	}
}
