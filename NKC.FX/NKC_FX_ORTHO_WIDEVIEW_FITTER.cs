using System;
using UnityEngine;

namespace NKC.FX;

[Serializable]
public class NKC_FX_ORTHO_WIDEVIEW_FITTER : MonoBehaviour
{
	public Vector3 ReferenceOffset = new Vector3(0f, 0f, 0f);

	public Vector3 ReferenceScale = new Vector3(1f, 1f, 1f);

	public float OrthoNormalSize = 500f;

	public float OrthoWideSize = 750f;

	public bool AdjustOffsetSelf;

	public bool BypassScaleX;

	public bool BypassScaleY;

	private Camera cam;

	private bool isWideView;

	private float ratio = 1f;

	private Vector3 oldScale = new Vector3(1f, 1f, 1f);

	private Vector3 offsetPosition = new Vector3(0f, 0f, 0f);

	private readonly float zeroNaN = 1E-12f;

	private void OnEnable()
	{
		base.transform.localScale = ReferenceScale;
		oldScale = base.transform.localScale;
		if (Camera.main != null)
		{
			cam = Camera.main;
			if (cam.orthographicSize > OrthoNormalSize)
			{
				isWideView = true;
			}
			else
			{
				isWideView = false;
			}
		}
		SetScale();
	}

	public void SetReferenceOffset()
	{
		ReferenceOffset = base.transform.localPosition;
	}

	public void SetReferenceScale()
	{
		ReferenceScale = base.transform.localScale;
	}

	private void SetScale()
	{
		if (isWideView)
		{
			ratio = OrthoWideSize / (OrthoNormalSize + zeroNaN);
			if (BypassScaleX)
			{
				oldScale.x = ReferenceScale.x;
				offsetPosition.x = base.transform.localPosition.x * 1f;
			}
			else
			{
				oldScale.x *= ratio;
				offsetPosition.x = base.transform.localPosition.x * ratio;
			}
			if (BypassScaleY)
			{
				oldScale.y = ReferenceScale.y;
				offsetPosition.y = base.transform.localPosition.y * 1f;
			}
			else
			{
				oldScale.y *= ratio;
				offsetPosition.y = base.transform.localPosition.y * ratio;
			}
			if (AdjustOffsetSelf)
			{
				base.transform.localPosition = offsetPosition;
			}
		}
		else
		{
			if (AdjustOffsetSelf)
			{
				base.transform.localPosition = ReferenceOffset;
			}
			oldScale.x = ReferenceScale.x;
			oldScale.y = ReferenceScale.y;
		}
		base.transform.localScale = oldScale;
	}
}
