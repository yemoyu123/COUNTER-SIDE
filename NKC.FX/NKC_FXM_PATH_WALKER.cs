using Cs.Logging;
using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_PATH_WALKER : NKC_FXM_EVALUATER
{
	public NKC_FX_PATH Path;

	public NKC_FX_PATH[] Paths;

	public Transform Target;

	public ForwardMode ForwardSync;

	public AnimationCurve Curve;

	private Vector3 value;

	private void OnDestroy()
	{
		if (Path != null)
		{
			Path = null;
		}
		if (Paths != null)
		{
			Paths = null;
		}
		if (Target != null)
		{
			Target = null;
		}
		if (Curve != null)
		{
			Curve = null;
		}
	}

	public override void Init()
	{
		if (!init)
		{
			if (Target == null)
			{
				Target = base.transform;
			}
			if (Curve == null || Curve.length < 1)
			{
				Curve = InitCurveLinear(0f, 1f);
			}
			if ((bool)Path)
			{
				init = true;
			}
			else if (Application.isPlaying)
			{
				Log.Error("NULL Exception! (NKC_FX_PATH) -> " + base.gameObject.name, this, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/FX/Module/NKC_FXM_PATH_WALKER.cs", 47);
				init = false;
			}
		}
	}

	public override void SetRandomValue(bool _resimulate)
	{
		if (_resimulate && RandomValue && Paths != null && Paths.Length != 0)
		{
			Path = Paths[Random.Range(0, Paths.Length)];
		}
	}

	protected override void OnExecute(bool _render)
	{
		if (Path != null)
		{
			float t = Evaluate(Curve);
			value = Path.GetPoint(t);
			Target.position = value;
			switch (ForwardSync)
			{
			case ForwardMode.Forward2D:
			{
				Vector3 vector = value + Path.GetDirection(t) - Target.position;
				float angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				Target.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				break;
			}
			case ForwardMode.Forward3D:
				Target.LookAt(value + Path.GetDirection(t));
				break;
			}
		}
	}
}
