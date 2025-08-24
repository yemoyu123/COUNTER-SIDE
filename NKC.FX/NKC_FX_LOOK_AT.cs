using UnityEngine;

namespace NKC.FX;

public class NKC_FX_LOOK_AT : MonoBehaviour
{
	public Transform Target;

	public float Roll;

	public bool IsVertical;

	public bool AtOnEnable;

	public bool AtLateUpdate;

	public bool IsOneshot;

	public bool Smooth;

	public float SmoothFactor;

	public bool TwoDimension;

	private bool executed;

	private Vector3 direction;

	private float angle;

	private Quaternion angleAxis;

	private Quaternion rotation;

	private void OnDestroy()
	{
		if (Target != null)
		{
			Target = null;
		}
	}

	private void Awake()
	{
		executed = false;
	}

	private void Update()
	{
		if (!AtOnEnable && !AtLateUpdate)
		{
			UpdateInternal();
		}
	}

	private void LateUpdate()
	{
		if (!AtOnEnable && AtLateUpdate)
		{
			UpdateInternal();
		}
	}

	private void UpdateInternal()
	{
		if (IsOneshot)
		{
			if (!executed)
			{
				Execute();
				executed = true;
			}
		}
		else
		{
			Execute();
		}
	}

	private void OnEnable()
	{
		if (AtOnEnable)
		{
			Execute();
		}
	}

	private void OnDisable()
	{
		executed = false;
	}

	public void Execute()
	{
		if (Target != null)
		{
			direction.Set(base.transform.position.x - Target.position.x, base.transform.position.y - Target.position.y, base.transform.position.z - Target.position.z);
			if (IsVertical)
			{
				angle = Mathf.Atan2(0f, direction.x) * 57.29578f;
				angleAxis = Quaternion.AngleAxis(angle - Roll, Vector3.up);
			}
			else
			{
				angle = Mathf.Atan2(direction.y, direction.x) * 57.29578f;
				angleAxis = Quaternion.AngleAxis(angle - Roll, Vector3.forward);
			}
			if (Smooth)
			{
				rotation = Quaternion.Slerp(base.transform.rotation, angleAxis, Time.deltaTime * SmoothFactor);
			}
			else
			{
				rotation = angleAxis;
			}
			base.transform.rotation = rotation;
		}
	}
}
