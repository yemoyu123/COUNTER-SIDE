using Spine.Unity;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_SPINE_SPRITE_MASK : MonoBehaviour
{
	public SkeletonAnimation SkeletonAnimation;

	private SpriteMaskInteraction MaskInteraction;

	public void SetMaskIntersection(int _state)
	{
		if (SkeletonAnimation != null)
		{
			switch (_state)
			{
			case 0:
				MaskInteraction = SpriteMaskInteraction.None;
				break;
			case 1:
				MaskInteraction = SpriteMaskInteraction.VisibleInsideMask;
				break;
			case 2:
				MaskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
				break;
			default:
				MaskInteraction = SpriteMaskInteraction.None;
				break;
			}
			SkeletonAnimation.maskInteraction = MaskInteraction;
		}
	}
}
