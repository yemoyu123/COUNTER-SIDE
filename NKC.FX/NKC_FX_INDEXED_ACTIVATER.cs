using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKC.FX;

[Serializable]
public class NKC_FX_INDEXED_ACTIVATER : MonoBehaviour
{
	public List<GameObject> Targets = new List<GameObject>();

	private bool init;

	private void OnDestroy()
	{
		if (Targets != null)
		{
			Targets.Clear();
			Targets = null;
		}
	}

	private void Awake()
	{
		Init();
		for (int i = 0; i < Targets.Count; i++)
		{
			if (Targets[i].activeInHierarchy)
			{
				Targets[i].SetActive(value: false);
			}
		}
	}

	private void Init()
	{
		if (init)
		{
			return;
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

	public void Execute(int index)
	{
		if (!init)
		{
			return;
		}
		for (int i = 0; i < Targets.Count; i++)
		{
			if (i < index)
			{
				if (!Targets[i].activeInHierarchy)
				{
					Targets[i].SetActive(value: true);
				}
			}
			else if (Targets[i].activeInHierarchy)
			{
				Targets[i].SetActive(value: false);
			}
		}
	}
}
