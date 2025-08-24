using UnityEngine;

namespace NKC.FX;

[ExecuteAlways]
public class NKC_FX_LINE_RENDERER : MonoBehaviour
{
	public bool UseWorldSpace;

	public bool UseLateUpdate;

	public Transform Line_Head;

	public Transform Line_Tail;

	public LineRenderer[] Lines;

	private bool init;

	private void OnDestroy()
	{
		if (Line_Head != null)
		{
			Line_Head = null;
		}
		if (Line_Tail != null)
		{
			Line_Tail = null;
		}
		if (Lines != null)
		{
			Lines = null;
		}
	}

	private void Awake()
	{
		Initialize();
	}

	private void OnValidate()
	{
		init = false;
		Initialize();
	}

	public void Initialize()
	{
		if (Line_Head != null && Line_Tail != null)
		{
			init = true;
		}
		else
		{
			Debug.LogWarning("Can not setup Line Point.", base.gameObject);
		}
	}

	private void Update()
	{
		if (!UseLateUpdate)
		{
			Execute();
		}
	}

	private void LateUpdate()
	{
		if (UseLateUpdate)
		{
			Execute();
		}
	}

	private void Execute()
	{
		if (!init)
		{
			return;
		}
		if (UseWorldSpace)
		{
			for (int i = 0; i < Lines.Length; i++)
			{
				Lines[i].SetPosition(0, Line_Head.position);
				Lines[i].SetPosition(1, Line_Tail.position);
			}
		}
		else
		{
			for (int j = 0; j < Lines.Length; j++)
			{
				Lines[j].SetPosition(0, Line_Head.localPosition);
				Lines[j].SetPosition(1, Line_Tail.localPosition);
			}
		}
	}
}
