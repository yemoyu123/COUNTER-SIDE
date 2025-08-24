using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_SPINE_ILLUST : MonoBehaviour
{
	public SkeletonGraphic TargetIllust;

	public GameObject ExceptionalVFX;

	public bool UseSpineLink;

	public SkeletonGraphic[] LinkedSpine;

	private Spine.Animation tempAnimation;

	private bool init;

	public void EnableExceptionalVfx(bool bOn)
	{
		NKCUtil.SetGameobjectActive(ExceptionalVFX, bOn);
	}

	private void OnDestroy()
	{
		if (TargetIllust != null)
		{
			RemoveEventListener();
			TargetIllust = null;
		}
		init = false;
	}

	private void RemoveEventListener()
	{
		if (TargetIllust.AnimationState != null)
		{
			TargetIllust.AnimationState.Start -= OnStart;
		}
	}

	private void Awake()
	{
		if (TargetIllust == null)
		{
			TargetIllust = GetComponent<SkeletonGraphic>();
		}
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
		if (TargetIllust.AnimationState != null)
		{
			TargetIllust.AnimationState.Start -= OnStart;
			TargetIllust.AnimationState.Start += OnStart;
			init = true;
		}
	}

	private void OnStart(TrackEntry entry)
	{
		if (!UseSpineLink)
		{
			return;
		}
		for (int i = 0; i < LinkedSpine.Length; i++)
		{
			if (LinkedSpine[i] == null)
			{
				Debug.LogWarning("Null LinkedSpine, Index : " + i, base.gameObject);
			}
			else if (LinkedSpine[i].AnimationState != null)
			{
				tempAnimation = LinkedSpine[i].SkeletonDataAsset.GetSkeletonData(quiet: false).FindAnimation(entry.Animation.Name);
				if (tempAnimation != null)
				{
					LinkedSpine[i].AnimationState.SetAnimation(entry.TrackIndex, entry.Animation.Name, entry.Loop);
				}
				else
				{
					Debug.LogWarning("No exist animation : " + entry.Animation.Name, base.gameObject);
				}
			}
		}
	}
}
