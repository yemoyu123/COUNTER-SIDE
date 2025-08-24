using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_TRANSFORM_EXT : NKC_FXM_TRANSFORM
{
	[HideInInspector]
	public Space Space = Space.Self;

	[HideInInspector]
	public float Speed = 100f;
}
