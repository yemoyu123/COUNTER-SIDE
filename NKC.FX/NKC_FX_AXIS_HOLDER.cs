using UnityEngine;

namespace NKC.FX;

[ExecuteAlways]
public class NKC_FX_AXIS_HOLDER : MonoBehaviour
{
	public Vector3 Rotation;

	public bool LockX;

	public bool LockY;

	public bool LockZ;

	private Vector3 axis;

	private void OnValidate()
	{
		HoldAxis();
	}

	private void Update()
	{
		if (base.transform.hasChanged)
		{
			HoldAxis();
		}
	}

	private void HoldAxis()
	{
		if (LockX)
		{
			axis.x = Rotation.x;
		}
		else
		{
			axis.x = base.transform.localRotation.x;
		}
		if (LockY)
		{
			axis.y = Rotation.y;
		}
		else
		{
			axis.y = base.transform.localRotation.y;
		}
		if (LockZ)
		{
			axis.z = Rotation.z;
		}
		else
		{
			axis.z = base.transform.localRotation.z;
		}
		base.transform.eulerAngles = axis;
		base.transform.hasChanged = false;
	}
}
