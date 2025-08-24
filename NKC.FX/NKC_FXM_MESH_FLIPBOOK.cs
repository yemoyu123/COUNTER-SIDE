using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_MESH_FLIPBOOK : NKC_FXM_EVALUATER
{
	public MeshFilter Target;

	public MeshFilter[] Targets;

	public bool UseMultiTargets;

	public bool RandomMesh;

	public bool ShuffleStart;

	public bool ShuffleArray;

	public Mesh[] Meshs;

	private Mesh[] shuffleMeshs;

	public AnimationCurve Frame;

	private int currentIndex;

	private void OnDestroy()
	{
		if (Target != null)
		{
			Target = null;
		}
		if (Targets != null)
		{
			Targets = null;
		}
		if (Meshs != null)
		{
			Meshs = null;
		}
		if (shuffleMeshs != null)
		{
			shuffleMeshs = null;
		}
		if (Frame != null)
		{
			Frame = null;
		}
	}

	public override void Init()
	{
		if (init)
		{
			return;
		}
		if (Frame == null || Frame.length < 1)
		{
			Frame = InitCurveLinear(0f, 1f);
		}
		if (UseMultiTargets)
		{
			Target = null;
			if (Targets.Length != 0)
			{
				for (int i = 0; i < Targets.Length; i++)
				{
					init = ValidTarget(Targets[i]);
				}
			}
			else
			{
				init = false;
			}
		}
		else if (Target != null)
		{
			init = ValidTarget(Target);
		}
		else
		{
			MeshFilter component = GetComponent<MeshFilter>();
			if (component != null)
			{
				Target = component;
				init = ValidTarget(Target);
			}
			else
			{
				init = false;
			}
		}
		if (Meshs != null && Meshs.Length != 0 && ShuffleArray)
		{
			shuffleMeshs = (Mesh[])Meshs.Clone();
		}
	}

	private bool ValidTarget(MeshFilter _t)
	{
		bool flag = false;
		if (_t != null)
		{
			if (_t.sharedMesh != null)
			{
				flag = true;
			}
			else
			{
				flag = false;
				Debug.LogWarning("MeshFilter Mesh not found -> " + _t.transform.name + " :: " + _t.transform.root, _t.gameObject);
			}
		}
		else
		{
			flag = false;
			Debug.LogWarning("MeshFilter not found :: " + base.gameObject.name);
		}
		return flag;
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.enabled || Meshs == null || Meshs.Length == 0)
		{
			return;
		}
		if (RandomMesh)
		{
			if (!UseMultiTargets)
			{
				Target.sharedMesh = Meshs[currentIndex];
				return;
			}
			for (int i = 0; i < Targets.Length; i++)
			{
				Targets[i].sharedMesh = Meshs[currentIndex];
			}
			return;
		}
		int num = 0;
		num = ((!ShuffleStart) ? ((int)Evaluate(Frame)) : ((int)Mathf.Repeat(Evaluate(Frame) + (float)currentIndex, Meshs.Length)));
		num = Mathf.Clamp(num, 0, Meshs.Length - 1);
		if (!UseMultiTargets)
		{
			if (ShuffleArray)
			{
				Target.sharedMesh = shuffleMeshs[num];
			}
			else
			{
				Target.sharedMesh = Meshs[num];
			}
			return;
		}
		for (int j = 0; j < Targets.Length; j++)
		{
			if (ShuffleArray)
			{
				Targets[j].sharedMesh = shuffleMeshs[num];
			}
			else
			{
				Targets[j].sharedMesh = Meshs[num];
			}
		}
	}

	public override void SetRandomValue(bool _resimulate)
	{
		if (_resimulate && base.enabled && init && Meshs != null && Meshs.Length != 0)
		{
			currentIndex = Random.Range(0, Meshs.Length);
			if (ShuffleArray)
			{
				Shuffle(shuffleMeshs);
			}
		}
	}
}
