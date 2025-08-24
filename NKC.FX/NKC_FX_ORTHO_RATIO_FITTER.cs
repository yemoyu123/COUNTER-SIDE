using System;
using UnityEngine;

namespace NKC.FX;

[Serializable]
public class NKC_FX_ORTHO_RATIO_FITTER : MonoBehaviour
{
	public Vector2 ReferenceResolution = new Vector2(1920f, 1080f);

	public Vector3 ReferenceScale = new Vector3(1f, 1f, 1f);

	public float ReferenceSize = 500f;

	public bool AllowPosition;

	public bool AdjustAspect;

	public bool BypassScaleX;

	public bool BypassScaleY;

	private Camera cam;

	private float oldSize = 1f;

	private float ratio = 1f;

	private float widthFactor = 1f;

	private float heightFactor = 1f;

	private readonly float zeroNaN = 1E-12f;

	private Vector3 oldScale;

	private Vector3 fitPosition;

	private bool init;

	private void OnEnable()
	{
		base.transform.localScale = ReferenceScale;
		oldScale = base.transform.localScale;
		oldSize = ReferenceSize;
		if (Camera.main != null)
		{
			cam = Camera.main;
			init = true;
		}
		else
		{
			init = false;
		}
		AdjustScaleByAspect();
	}

	private void Update()
	{
		Execute();
	}

	public void Execute()
	{
		if (init)
		{
			if (AllowPosition && base.gameObject.activeInHierarchy)
			{
				SetPosition();
			}
			if (oldSize != cam.orthographicSize)
			{
				SetScale();
			}
		}
	}

	private void AdjustScaleByAspect()
	{
		if (!init)
		{
			return;
		}
		float width = cam.pixelRect.width;
		float height = cam.pixelRect.height;
		float num = ReferenceResolution.x / ReferenceResolution.y;
		float num2 = height * num;
		float num3 = num2 / num;
		float num4 = num3 * num;
		float num5 = num4 / num;
		if (num2 > width || num3 > height)
		{
			if (num2 > num3)
			{
				num2 = width;
				num3 = num2 / num;
			}
			else
			{
				num3 = height;
				num2 = num3 * num;
			}
		}
		float num6 = num2 / num4;
		float num7 = num3 / num5;
		if (AdjustAspect)
		{
			widthFactor = width / num2 * num6;
			heightFactor = height / num3 * num7;
		}
		else
		{
			widthFactor = 1f;
			heightFactor = 1f;
		}
		oldScale.Set(oldScale.x * widthFactor, oldScale.y * heightFactor, oldScale.z);
		base.transform.localScale = oldScale;
	}

	private void SetScale()
	{
		ratio = cam.orthographicSize / (oldSize + zeroNaN);
		oldSize = cam.orthographicSize;
		if (BypassScaleX)
		{
			oldScale.x = base.transform.localScale.x;
		}
		else
		{
			oldScale.x *= ratio;
		}
		if (BypassScaleY)
		{
			oldScale.y = base.transform.localScale.y;
		}
		else
		{
			oldScale.y *= ratio;
		}
		base.transform.localScale = oldScale;
	}

	private void SetPosition()
	{
		fitPosition.Set(cam.transform.position.x, cam.transform.position.y, base.transform.position.z);
		base.transform.position = fitPosition;
	}
}
