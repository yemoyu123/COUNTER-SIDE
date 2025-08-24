using System.Collections.Generic;
using UnityEngine;

namespace NKC.FX;

[ExecuteAlways]
public class NKC_FX_GRID : MonoBehaviour
{
	public Vector2Int GridSize = new Vector2Int(8, 8);

	public Vector2 Spacing = new Vector2(1f, -1f);

	public float ScaleFactor = 1f;

	private Rect boundary;

	private Vector3 center;

	private Vector3 grid;

	private Vector3 scale;

	private List<Transform> children = new List<Transform>();

	private bool overCount;

	private void OnDestroy()
	{
		if (children != null)
		{
			children.Clear();
			children = null;
		}
	}

	private void OnValidate()
	{
		ExecuteGrid();
	}

	private void OnEnable()
	{
		ExecuteGrid();
	}

	private void ExecuteGrid()
	{
		SetGrid();
		SetTransform();
		SetPivotCenter();
	}

	public bool CheckOverDriven()
	{
		return base.transform.childCount > GridSize.x * GridSize.y;
	}

	public void SetTransform()
	{
		base.transform.localEulerAngles = Vector3.zero;
		if (ScaleFactor >= 0f)
		{
			scale.Set(ScaleFactor, ScaleFactor, 1f);
			base.transform.localScale = scale;
		}
	}

	public void SetGrid()
	{
		grid = Vector3.zero;
		int num = 1;
		int num2 = 1;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).localPosition = grid;
			if (GridSize.x * GridSize.y <= i)
			{
				base.transform.GetChild(i).name = ("G" + num2 + "_" + num + "_(OverDriven) -> " + i).ToString();
			}
			else
			{
				base.transform.GetChild(i).name = ("G" + num2 + "_" + num).ToString();
			}
			if (num < GridSize.x)
			{
				grid.x += Spacing.x;
				num++;
			}
			else if (num2 < GridSize.y)
			{
				grid.y += Spacing.y;
				grid.x = 0f;
				num = 1;
				num2++;
			}
			else if (num == GridSize.x)
			{
				num = GridSize.x;
			}
			else if (num2 == GridSize.y)
			{
				num2 = GridSize.y;
			}
		}
	}

	public void SetPivotCenter()
	{
		boundary.xMin = 0f;
		boundary.xMax = (float)GridSize.x * Spacing.x - Spacing.x;
		boundary.yMin = 0f;
		boundary.yMax = (float)GridSize.y * Spacing.y - Spacing.y;
		center.Set(boundary.center.x * -1f * base.transform.localScale.x, boundary.center.y * -1f * base.transform.localScale.y, 0f);
		base.transform.localPosition = center;
	}
}
