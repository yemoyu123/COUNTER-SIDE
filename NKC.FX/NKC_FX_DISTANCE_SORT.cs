using UnityEngine;

namespace NKC.FX;

[ExecuteAlways]
public class NKC_FX_DISTANCE_SORT : MonoBehaviour
{
	public float Distance;

	private Vector3 destination;

	private void Update()
	{
		if (!(base.transform.root == base.transform) && base.transform.hasChanged)
		{
			SetPosition();
			base.transform.hasChanged = false;
		}
	}

	private void OnValidate()
	{
		SetPosition();
	}

	private void OnDestroy()
	{
		destination.Set(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
		base.transform.localPosition = destination;
	}

	private void SetPosition()
	{
		if (base.enabled)
		{
			if (base.transform.eulerAngles.y >= 180f)
			{
				destination.Set(base.transform.localPosition.x, base.transform.localPosition.y, Distance * -1f);
			}
			else
			{
				destination.Set(base.transform.localPosition.x, base.transform.localPosition.y, Distance);
			}
			base.transform.localPosition = destination;
		}
	}

	public void SetDistance(float _distance)
	{
		Distance = _distance;
		base.transform.hasChanged = true;
	}
}
