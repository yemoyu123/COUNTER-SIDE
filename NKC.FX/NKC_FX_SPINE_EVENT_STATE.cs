using System;
using Cs.Logging;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC.FX;

[Serializable]
public class NKC_FX_SPINE_EVENT_STATE : MonoBehaviour
{
	[Serializable]
	public class SpineStateEvent
	{
		public bool m_Enable;

		[SpineAnimation("", "SkeletonDataAsset", true, false)]
		public string m_AnimationName;

		public NKC_FX_EVENT m_CallbackEvent;
	}

	public GameObject TargetSpine;

	public SkeletonDataAsset SkeletonDataAsset;

	private SkeletonAnimation skeletonAnimation;

	private SkeletonGraphic skeletonGraphic;

	private bool init;

	[Header("On Start")]
	public SpineStateEvent[] StartStateEvents;

	[Header("On End")]
	public SpineStateEvent[] EndStateEvents;

	[Header("On Dispose")]
	public SpineStateEvent[] DisposeStateEvents;

	[Header("On Interrupt")]
	public SpineStateEvent[] InterruptStateEvents;

	[Header("On Complete")]
	public SpineStateEvent[] CompleteStateEvents;

	private void OnDestroy()
	{
		if (TargetSpine != null)
		{
			TargetSpine = null;
		}
		if (skeletonAnimation != null)
		{
			CleanupSkeletonAnimation();
			skeletonAnimation = null;
		}
		if (skeletonGraphic != null)
		{
			CleanupSkeletonGraphic();
			skeletonGraphic = null;
		}
		init = false;
	}

	private void Awake()
	{
		Initialize();
		InitEventListener();
	}

	private void OnValidate()
	{
		Initialize();
	}

	private void Start()
	{
		if (!init)
		{
			InitEventListener();
		}
	}

	public void InitEventListener()
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

	private void Initialize()
	{
		if (TargetSpine != null)
		{
			skeletonAnimation = TargetSpine.GetComponentInChildren<SkeletonAnimation>(includeInactive: true);
			if (skeletonAnimation != null)
			{
				SkeletonDataAsset = skeletonAnimation.SkeletonDataAsset;
				return;
			}
			skeletonGraphic = TargetSpine.GetComponentInChildren<SkeletonGraphic>(includeInactive: true);
			if (skeletonGraphic != null)
			{
				SkeletonDataAsset = skeletonGraphic.SkeletonDataAsset;
			}
		}
		else
		{
			Log.Warn("Can not found TargetSpine.", base.gameObject, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/FX/NKC_FX_SPINE_EVENT_STATE.cs", 101);
			SkeletonDataAsset = null;
		}
	}

	private void InitSkeletonAnimation()
	{
		if (skeletonAnimation.AnimationState != null)
		{
			if (StartStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.Start -= OnStart;
				skeletonAnimation.AnimationState.Start += OnStart;
			}
			if (EndStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.End -= OnEnd;
				skeletonAnimation.AnimationState.End += OnEnd;
			}
			if (DisposeStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.Dispose -= OnDispose;
				skeletonAnimation.AnimationState.Dispose += OnDispose;
			}
			if (InterruptStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.Interrupt -= OnInterrupt;
				skeletonAnimation.AnimationState.Interrupt += OnInterrupt;
			}
			if (CompleteStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.Complete -= OnComplete;
				skeletonAnimation.AnimationState.Complete += OnComplete;
			}
			init = true;
		}
	}

	private void InitSkeletonGraphic()
	{
		if (skeletonGraphic.AnimationState != null)
		{
			if (StartStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.Start -= OnStart;
				skeletonGraphic.AnimationState.Start += OnStart;
			}
			if (EndStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.End -= OnEnd;
				skeletonGraphic.AnimationState.End += OnEnd;
			}
			if (DisposeStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.Dispose -= OnDispose;
				skeletonGraphic.AnimationState.Dispose += OnDispose;
			}
			if (InterruptStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.Interrupt -= OnInterrupt;
				skeletonGraphic.AnimationState.Interrupt += OnInterrupt;
			}
			if (CompleteStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.Complete -= OnComplete;
				skeletonGraphic.AnimationState.Complete += OnComplete;
			}
			init = true;
		}
	}

	private void CleanupSkeletonAnimation()
	{
		if (skeletonAnimation.AnimationState != null)
		{
			if (StartStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.Start -= OnStart;
			}
			if (EndStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.End -= OnEnd;
			}
			if (DisposeStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.Dispose -= OnDispose;
			}
			if (InterruptStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.Interrupt -= OnInterrupt;
			}
			if (CompleteStateEvents.Length != 0)
			{
				skeletonAnimation.AnimationState.Complete -= OnComplete;
			}
			init = false;
		}
	}

	private void CleanupSkeletonGraphic()
	{
		if (skeletonGraphic.AnimationState != null)
		{
			if (StartStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.Start -= OnStart;
			}
			if (EndStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.End -= OnEnd;
			}
			if (DisposeStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.Dispose -= OnDispose;
			}
			if (InterruptStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.Interrupt -= OnInterrupt;
			}
			if (CompleteStateEvents.Length != 0)
			{
				skeletonGraphic.AnimationState.Complete -= OnComplete;
			}
			init = false;
		}
	}

	private void OnStart(TrackEntry entry)
	{
		for (int i = 0; i < StartStateEvents.Length; i++)
		{
			if (StartStateEvents[i].m_Enable && StartStateEvents[i].m_AnimationName == entry.Animation.Name)
			{
				ExecuteEvent(StartStateEvents[i].m_CallbackEvent);
				break;
			}
		}
	}

	private void OnEnd(TrackEntry entry)
	{
		for (int i = 0; i < EndStateEvents.Length; i++)
		{
			if (EndStateEvents[i].m_Enable && EndStateEvents[i].m_AnimationName == entry.Animation.Name)
			{
				ExecuteEvent(EndStateEvents[i].m_CallbackEvent);
				break;
			}
		}
	}

	private void OnDispose(TrackEntry entry)
	{
		for (int i = 0; i < DisposeStateEvents.Length; i++)
		{
			if (DisposeStateEvents[i].m_Enable && DisposeStateEvents[i].m_AnimationName == entry.Animation.Name)
			{
				ExecuteEvent(DisposeStateEvents[i].m_CallbackEvent);
				break;
			}
		}
	}

	private void OnInterrupt(TrackEntry entry)
	{
		for (int i = 0; i < InterruptStateEvents.Length; i++)
		{
			if (InterruptStateEvents[i].m_Enable && InterruptStateEvents[i].m_AnimationName == entry.Animation.Name && GetCurrent(entry.TrackIndex) != entry.Animation.Name)
			{
				ExecuteEvent(InterruptStateEvents[i].m_CallbackEvent);
				break;
			}
		}
	}

	private void OnComplete(TrackEntry entry)
	{
		for (int i = 0; i < CompleteStateEvents.Length; i++)
		{
			if (CompleteStateEvents[i].m_Enable && CompleteStateEvents[i].m_AnimationName == entry.Animation.Name)
			{
				ExecuteEvent(CompleteStateEvents[i].m_CallbackEvent);
				break;
			}
		}
	}

	private void ExecuteEvent(NKC_FX_EVENT _event)
	{
		if (_event != null)
		{
			_event.Execute();
		}
		else
		{
			Log.Warn("Can not found NKC_FX_EVENT.", base.gameObject, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/FX/NKC_FX_SPINE_EVENT_STATE.cs", 334);
		}
	}

	private string GetCurrent(int _trackIndex)
	{
		string result = "";
		if (skeletonAnimation != null)
		{
			result = skeletonAnimation.AnimationState.GetCurrent(_trackIndex).Animation.Name;
		}
		else if (skeletonGraphic != null)
		{
			result = skeletonGraphic.AnimationState.GetCurrent(_trackIndex).Animation.Name;
		}
		return result;
	}
}
