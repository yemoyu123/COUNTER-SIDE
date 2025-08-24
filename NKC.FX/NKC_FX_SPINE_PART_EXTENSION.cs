using System;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules;
using UnityEngine;

namespace NKC.FX;

[Serializable]
public class NKC_FX_SPINE_PART_EXTENSION : MonoBehaviour
{
	public SkeletonAnimation BaseSkeletionAnimation;

	[HideInInspector]
	public SkeletonDataAsset BaseSkeletonDataAsset;

	[SpineAnimation("", "BaseSkeletonDataAsset", true, false)]
	public string BaseAnimationName;

	[Space(10f)]
	[HideInInspector]
	public SkeletonDataAsset PartSkeletonDataAsset;

	public SkeletonAnimation PartSkeletonAnimation;

	[SpineAnimation("", "PartSkeletonAnimation", true, false)]
	public string PartAnimationName;

	public float StartTime;

	private Renderer rend;

	private SkeletonRenderSeparator skeletonRenderSeparator;

	private bool isSeparated;

	private TrackEntry baseEntry;

	private TrackEntry partEntry;

	private bool validAnimation;

	private bool init;

	private void OnDestroy()
	{
		if (BaseSkeletionAnimation != null)
		{
			BaseSkeletionAnimation.UpdateComplete -= OnUpdate;
			if (BaseSkeletionAnimation.AnimationState != null)
			{
				BaseSkeletionAnimation.AnimationState.Start -= OnStart;
			}
			BaseSkeletionAnimation = null;
			BaseSkeletonDataAsset = null;
		}
		if (PartSkeletonAnimation != null)
		{
			PartSkeletonAnimation = null;
			PartSkeletonDataAsset = null;
		}
		rend = null;
		skeletonRenderSeparator = null;
		baseEntry = null;
		partEntry = null;
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
		if (BaseSkeletionAnimation != null)
		{
			InitSkeletonAnimation();
		}
	}

	private void Initialize()
	{
		if (BaseSkeletionAnimation != null)
		{
			BaseSkeletonDataAsset = BaseSkeletionAnimation.SkeletonDataAsset;
		}
		if (PartSkeletonAnimation != null)
		{
			PartSkeletonDataAsset = PartSkeletonAnimation.SkeletonDataAsset;
			PartSkeletonAnimation.enabled = false;
			rend = PartSkeletonAnimation.GetComponent<Renderer>();
		}
		skeletonRenderSeparator = base.gameObject.GetComponent<SkeletonRenderSeparator>();
		if (skeletonRenderSeparator != null)
		{
			isSeparated = true;
		}
		else
		{
			isSeparated = false;
		}
	}

	private void InitSkeletonAnimation()
	{
		if (BaseSkeletionAnimation.AnimationState != null)
		{
			BaseSkeletionAnimation.AnimationState.Start -= OnStart;
			BaseSkeletionAnimation.AnimationState.Start += OnStart;
			BaseSkeletionAnimation.UpdateComplete -= OnUpdate;
			BaseSkeletionAnimation.UpdateComplete += OnUpdate;
			init = true;
		}
	}

	private void OnStart(TrackEntry entry)
	{
		if (entry != null)
		{
			if (entry.Animation.Name == BaseAnimationName)
			{
				validAnimation = true;
				baseEntry = entry;
				rend.enabled = false;
				partEntry = PartSkeletonAnimation.AnimationState.SetAnimation(0, PartAnimationName, loop: false);
			}
			else
			{
				validAnimation = false;
			}
		}
	}

	private void OnUpdate(ISkeletonAnimation s)
	{
		if (baseEntry != null && validAnimation)
		{
			if (!(StartTime <= baseEntry.AnimationTime))
			{
				return;
			}
			if (!isSeparated)
			{
				rend.enabled = true;
			}
			else
			{
				rend.enabled = false;
				for (int i = 0; i < skeletonRenderSeparator.partsRenderers.Count; i++)
				{
					skeletonRenderSeparator.partsRenderers[i].MeshRenderer.enabled = true;
				}
			}
			partEntry.TimeScale = baseEntry.TimeScale;
			partEntry.TrackTime = baseEntry.AnimationTime - StartTime;
			PartSkeletonAnimation.Update(Time.deltaTime);
			PartSkeletonAnimation.LateUpdate();
		}
		else if (!isSeparated)
		{
			rend.enabled = false;
		}
		else
		{
			rend.enabled = false;
			for (int j = 0; j < skeletonRenderSeparator.partsRenderers.Count; j++)
			{
				skeletonRenderSeparator.partsRenderers[j].MeshRenderer.enabled = false;
			}
		}
	}
}
