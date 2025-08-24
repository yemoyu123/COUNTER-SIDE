using UnityEngine;

namespace NKC.FX;

public class NKC_FX_BILLBOARD : MonoBehaviour
{
	public Transform Target;

	public Transform TargetLook;

	public Renderer CastRenderer;

	public bool Horizontal;

	public bool Smooth;

	public float LookSpeed = 1f;

	private Quaternion rotLook;

	private Vector3 rotFinal;

	private bool init;

	private void Awake()
	{
		Init();
	}

	private void OnValidate()
	{
		init = false;
		Init();
	}

	private void Init()
	{
		if (init)
		{
			return;
		}
		if (Target == null)
		{
			Target = base.transform;
		}
		if (TargetLook == null)
		{
			Camera main = Camera.main;
			if (main != null)
			{
				TargetLook = main.transform;
			}
		}
		if (Target != null && TargetLook != null)
		{
			init = true;
		}
		else
		{
			init = false;
		}
		Execute();
	}

	private void Update()
	{
		if ((!(CastRenderer != null) || CastRenderer.isVisible) && (TargetLook.hasChanged || Target.hasChanged))
		{
			TargetLook.hasChanged = false;
			Target.hasChanged = false;
			Execute();
		}
	}

	private void Execute()
	{
		if (init)
		{
			rotLook = Quaternion.LookRotation(TargetLook.position - Target.position);
			if (Horizontal)
			{
				rotFinal.Set(0f, rotLook.eulerAngles.y, rotLook.eulerAngles.z);
				rotLook.eulerAngles = rotFinal;
			}
			if (!Smooth)
			{
				Target.rotation = rotLook;
			}
			else
			{
				Target.rotation = Quaternion.Slerp(Target.rotation, rotLook, Time.unscaledDeltaTime * LookSpeed * 10f);
			}
		}
	}
}
