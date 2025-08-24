using System;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules;
using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_SPINE : NKC_FXM_EVALUATER
{
	[Serializable]
	public class SpineCueSheet
	{
		public enum SpineAnimationMethod
		{
			Set,
			Add
		}

		public bool m_Enable;

		public float m_StartTime;

		public SpineAnimationMethod m_Method;

		[SpineAnimation("", "SkeletonDataAsset", true, false)]
		public string m_AnimationName;

		public float m_TimeScale = 1f;

		public bool m_Loop;

		[HideInInspector]
		public bool activated;
	}

	public GameObject Target;

	public bool SyncRenderer;

	public SpineCueSheet[] CueSheet;

	public SkeletonDataAsset SkeletonDataAsset;

	protected TrackEntry m_TrackEntry;

	protected float currentTime;

	private SkeletonAnimation skeletonAnimation;

	private SkeletonRenderSeparator skeletonRenderSeparator;

	private bool isSeparated;

	private Renderer rend;

	private void Awake()
	{
		if (Target == null)
		{
			Target = base.gameObject;
		}
	}

	private void OnDestroy()
	{
		if (Target != null)
		{
			Target = null;
		}
		if (SkeletonDataAsset != null)
		{
			SkeletonDataAsset = null;
		}
		if (skeletonAnimation != null)
		{
			skeletonAnimation = null;
		}
		if (m_TrackEntry != null)
		{
			m_TrackEntry = null;
		}
		if (rend != null)
		{
			rend = null;
		}
	}

	public override void Init()
	{
		if (Target != null)
		{
			skeletonAnimation = Target.GetComponent<SkeletonAnimation>();
			if (!(skeletonAnimation != null))
			{
				return;
			}
			skeletonAnimation.enabled = false;
			rend = skeletonAnimation.GetComponent<Renderer>();
			if (rend != null)
			{
				init = true;
				skeletonRenderSeparator = Target.GetComponent<SkeletonRenderSeparator>();
				if (skeletonRenderSeparator != null)
				{
					isSeparated = true;
				}
				else
				{
					isSeparated = false;
				}
				SkeletonDataAsset = skeletonAnimation.SkeletonDataAsset;
				if (SkeletonDataAsset != null)
				{
					init = true;
					return;
				}
				init = false;
				Debug.LogWarning("Null SkeletonDataAsset.", base.gameObject);
			}
			else
			{
				init = false;
				Debug.LogWarning("Null Renderer.", base.gameObject);
			}
		}
		else
		{
			init = false;
			SkeletonDataAsset = null;
			Debug.LogWarning("Null Target.", base.gameObject);
		}
	}

	protected override void OnStart()
	{
		if (skeletonAnimation == null)
		{
			Debug.LogWarning("SkeletonAnimation not found -> " + base.transform.name + " :: " + base.transform.root, base.gameObject);
		}
		currentTime = 0f;
		if (CueSheet != null && CueSheet.Length != 0)
		{
			for (int i = 0; i < CueSheet.Length; i++)
			{
				CueSheet[i].activated = false;
			}
		}
		else
		{
			Debug.LogWarning("Cue Sheet is empty. -> " + base.transform.name + " :: " + base.transform.root, base.gameObject);
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (rend != null && SyncRenderer)
		{
			if (!isSeparated)
			{
				rend.enabled = _render;
			}
			else
			{
				rend.enabled = false;
				for (int i = 0; i < skeletonRenderSeparator.partsRenderers.Count; i++)
				{
					skeletonRenderSeparator.partsRenderers[i].MeshRenderer.enabled = _render;
				}
			}
		}
		if (!_render && m_TrackEntry != null)
		{
			m_TrackEntry.TrackTime = 0f;
		}
		if (CueSheet != null && CueSheet.Length != 0)
		{
			currentTime += deltaTime;
			if (currentTime >= 0f && Duration > currentTime)
			{
				for (int j = 0; j < CueSheet.Length; j++)
				{
					if (CueSheet[j].m_Enable && !CueSheet[j].activated && CueSheet[j].m_StartTime < currentTime)
					{
						CueSheet[j].activated = true;
						ExecuteSpine(CueSheet[j].m_Method, CueSheet[j].m_AnimationName, CueSheet[j].m_Loop, CueSheet[j].m_TimeScale);
					}
				}
			}
		}
		if (skeletonAnimation != null)
		{
			skeletonAnimation.Update(deltaTime);
			skeletonAnimation.LateUpdate();
		}
	}

	protected override void OnComplete()
	{
		if (ResetMode == ResetAction.SyncSelf)
		{
			playbackTime = 0f;
			OnExecute(_render: false);
		}
	}

	private void ExecuteSpine(SpineCueSheet.SpineAnimationMethod _method, string _animationName, bool _loop, float _timeScale)
	{
		if (!(skeletonAnimation != null))
		{
			return;
		}
		if (skeletonAnimation.AnimationState != null)
		{
			if (_method == SpineCueSheet.SpineAnimationMethod.Set)
			{
				skeletonAnimation.AnimationState.SetAnimation(0, _animationName, _loop).TimeScale = _timeScale;
			}
			else
			{
				skeletonAnimation.AnimationState.AddAnimation(0, _animationName, _loop, 0f).TimeScale = _timeScale;
			}
			m_TrackEntry = skeletonAnimation.AnimationState.GetCurrent(0);
		}
		else
		{
			Debug.LogWarning("AnimationState is null. -> " + base.transform.name + " :: " + base.transform.root, base.gameObject);
		}
	}
}
