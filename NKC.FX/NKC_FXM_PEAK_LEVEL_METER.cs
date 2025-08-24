using System.Collections.Generic;
using UnityEngine;

namespace NKC.FX;

public class NKC_FXM_PEAK_LEVEL_METER : NKC_FXM_EVALUATER
{
	public AnimationCurve Curve;

	public List<GameObject> Targets = new List<GameObject>();

	private int level;

	private int oldLevel;

	private void OnDestroy()
	{
		if (Curve != null)
		{
			Curve = null;
		}
		if (Targets != null)
		{
			Targets.Clear();
			Targets = null;
		}
	}

	private void Start()
	{
		for (int i = 0; i < Targets.Count; i++)
		{
			if (Targets[i].activeSelf)
			{
				Targets[i].SetActive(value: false);
			}
		}
	}

	public override void Init()
	{
		if (init)
		{
			return;
		}
		if (Curve == null || Curve.length < 1)
		{
			Curve = InitCurve(0f, 0f);
		}
		if (Targets.Capacity != base.transform.childCount)
		{
			Targets.Clear();
			Targets.Capacity = base.transform.childCount;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Targets.Add(base.transform.GetChild(i).gameObject);
			}
		}
		Targets.Sort(CompareName);
		Targets.TrimExcess();
		init = true;
	}

	private int CompareName(GameObject x, GameObject y)
	{
		return x.name.CompareTo(y.name);
	}

	protected override void OnExecute(bool _render)
	{
		if (!base.enabled)
		{
			return;
		}
		if (_render)
		{
			level = (int)Mathf.Lerp(0f, Targets.Count, Evaluate(Curve));
			if (level == oldLevel)
			{
				return;
			}
			for (int i = 0; i < Targets.Count; i++)
			{
				if (i < level)
				{
					Targets[i].SetActive(value: true);
				}
				else
				{
					Targets[i].SetActive(value: false);
				}
			}
			oldLevel = level;
			return;
		}
		for (int j = 0; j < Targets.Count; j++)
		{
			if (Targets[j].activeSelf)
			{
				Targets[j].SetActive(value: false);
			}
		}
	}
}
