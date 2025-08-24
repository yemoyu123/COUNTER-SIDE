using Spine.Unity;
using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_SPINE_ILLUST : NKC_FXM_SPINE
{
	private SkeletonGraphic skeletonGraphic;

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
		if (skeletonGraphic != null)
		{
			skeletonGraphic = null;
		}
		if (m_TrackEntry != null)
		{
			m_TrackEntry = null;
		}
	}

	public override void Init()
	{
		if (Target != null)
		{
			skeletonGraphic = Target.GetComponent<SkeletonGraphic>();
			if (skeletonGraphic != null)
			{
				SkeletonDataAsset = skeletonGraphic.SkeletonDataAsset;
				skeletonGraphic.freeze = base.enabled;
				init = true;
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
		if (skeletonGraphic == null)
		{
			Debug.LogWarning("SkeletonGraphic not found -> " + base.transform.name + " :: " + base.transform.root, base.gameObject);
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
		if (SyncRenderer)
		{
			if (_render)
			{
				skeletonGraphic.canvasRenderer.SetAlpha(1f);
			}
			else
			{
				skeletonGraphic.canvasRenderer.SetAlpha(0f);
			}
		}
		else
		{
			skeletonGraphic.canvasRenderer.SetAlpha(1f);
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
				for (int i = 0; i < CueSheet.Length; i++)
				{
					if (CueSheet[i].m_Enable && !CueSheet[i].activated && CueSheet[i].m_StartTime < currentTime)
					{
						CueSheet[i].activated = true;
						ExecuteSpine(CueSheet[i].m_Method, CueSheet[i].m_AnimationName, CueSheet[i].m_Loop, CueSheet[i].m_TimeScale);
					}
				}
			}
		}
		if (skeletonGraphic != null)
		{
			skeletonGraphic._Update(deltaTime);
			skeletonGraphic.UpdateMesh();
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
		if (!(skeletonGraphic != null))
		{
			return;
		}
		if (skeletonGraphic.AnimationState != null)
		{
			if (_method == SpineCueSheet.SpineAnimationMethod.Set)
			{
				skeletonGraphic.AnimationState.SetAnimation(0, _animationName, _loop).TimeScale = _timeScale;
			}
			else
			{
				skeletonGraphic.AnimationState.AddAnimation(0, _animationName, _loop, 0f).TimeScale = _timeScale;
			}
			m_TrackEntry = skeletonGraphic.AnimationState.GetCurrent(0);
		}
		else
		{
			Debug.LogWarning("AnimationState is null. -> " + base.transform.name + " :: " + base.transform.root, base.gameObject);
		}
	}
}
