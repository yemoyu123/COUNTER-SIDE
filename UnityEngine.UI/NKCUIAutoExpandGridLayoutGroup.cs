namespace UnityEngine.UI;

[AddComponentMenu("Layout/Auto Expand Grid Layout Group", 152)]
public class NKCUIAutoExpandGridLayoutGroup : LayoutGroup
{
	public enum Corner
	{
		UpperLeft,
		UpperRight,
		LowerLeft,
		LowerRight
	}

	public enum Axis
	{
		Horizontal,
		Vertical
	}

	public enum Constraint
	{
		Flexible,
		FixedColumnCount,
		FixedRowCount
	}

	[SerializeField]
	protected Corner m_StartCorner;

	[SerializeField]
	protected Axis m_StartAxis;

	[SerializeField]
	protected Vector2 m_CellSize = new Vector2(100f, 100f);

	[SerializeField]
	protected Vector2 m_Spacing = Vector2.zero;

	[SerializeField]
	protected Constraint m_Constraint;

	[SerializeField]
	protected int m_ConstraintCount = 2;

	public Corner startCorner
	{
		get
		{
			return m_StartCorner;
		}
		set
		{
			SetProperty(ref m_StartCorner, value);
		}
	}

	public Axis startAxis
	{
		get
		{
			return m_StartAxis;
		}
		set
		{
			SetProperty(ref m_StartAxis, value);
		}
	}

	public Vector2 cellSize
	{
		get
		{
			return m_CellSize;
		}
		set
		{
			SetProperty(ref m_CellSize, value);
		}
	}

	public Vector2 spacing
	{
		get
		{
			return m_Spacing;
		}
		set
		{
			SetProperty(ref m_Spacing, value);
		}
	}

	public Constraint constraint
	{
		get
		{
			return m_Constraint;
		}
		set
		{
			SetProperty(ref m_Constraint, value);
		}
	}

	public int constraintCount
	{
		get
		{
			return m_ConstraintCount;
		}
		set
		{
			SetProperty(ref m_ConstraintCount, Mathf.Max(1, value));
		}
	}

	protected NKCUIAutoExpandGridLayoutGroup()
	{
	}

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		int num = 0;
		int num2 = 0;
		if (m_Constraint == Constraint.FixedColumnCount)
		{
			num = (num2 = m_ConstraintCount);
		}
		else if (m_Constraint == Constraint.FixedRowCount)
		{
			num = (num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)m_ConstraintCount - 0.001f));
		}
		else
		{
			num = 1;
			num2 = Mathf.CeilToInt(Mathf.Sqrt(base.rectChildren.Count));
		}
		SetLayoutInputForAxis((float)base.padding.horizontal + (cellSize.x + spacing.x) * (float)num - spacing.x, (float)base.padding.horizontal + (cellSize.x + spacing.x) * (float)num2 - spacing.x, -1f, 0);
	}

	public override void CalculateLayoutInputVertical()
	{
		int num = 0;
		if (m_Constraint == Constraint.FixedColumnCount)
		{
			num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)m_ConstraintCount - 0.001f);
		}
		else if (m_Constraint == Constraint.FixedRowCount)
		{
			num = m_ConstraintCount;
		}
		else
		{
			float x = base.rectTransform.rect.size.x;
			int num2 = Mathf.Max(1, Mathf.FloorToInt((x - (float)base.padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
			num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num2);
		}
		float num3 = (float)base.padding.vertical + (cellSize.y + spacing.y) * (float)num - spacing.y;
		SetLayoutInputForAxis(num3, num3, -1f, 1);
	}

	public override void SetLayoutHorizontal()
	{
		SetCellsAlongAxis(0);
	}

	public override void SetLayoutVertical()
	{
		SetCellsAlongAxis(1);
	}

	private void SetCellsAlongAxis(int axis)
	{
		if (axis == 0)
		{
			for (int i = 0; i < base.rectChildren.Count; i++)
			{
				RectTransform rectTransform = base.rectChildren[i];
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDelta);
				rectTransform.anchorMin = Vector2.up;
				rectTransform.anchorMax = Vector2.up;
				rectTransform.sizeDelta = cellSize;
			}
			return;
		}
		float x = base.rectTransform.rect.size.x;
		float y = base.rectTransform.rect.size.y;
		int num = 1;
		int num2 = 1;
		if (m_Constraint == Constraint.FixedColumnCount)
		{
			num = m_ConstraintCount;
			num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num - 0.001f);
		}
		else if (m_Constraint != Constraint.FixedRowCount)
		{
			num = ((!(cellSize.x + spacing.x <= 0f)) ? Mathf.Max(1, Mathf.FloorToInt((x - (float)base.padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x))) : int.MaxValue);
			num2 = ((!(cellSize.y + spacing.y <= 0f)) ? Mathf.Max(1, Mathf.FloorToInt((y - (float)base.padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y))) : int.MaxValue);
		}
		else
		{
			num2 = m_ConstraintCount;
			num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num2 - 0.001f);
		}
		int num3 = (int)startCorner % 2;
		int num4 = (int)startCorner / 2;
		int num5;
		int num6;
		int num7;
		if (startAxis == Axis.Horizontal)
		{
			num5 = num;
			num6 = Mathf.Clamp(num, 1, base.rectChildren.Count);
			num7 = Mathf.Clamp(num2, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num5));
		}
		else
		{
			num5 = num2;
			num7 = Mathf.Clamp(num2, 1, base.rectChildren.Count);
			num6 = Mathf.Clamp(num, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num5));
		}
		Vector2 vector = new Vector2((float)num6 * cellSize.x + (float)(num6 - 1) * spacing.x, (float)num7 * cellSize.y + (float)(num7 - 1) * spacing.y);
		Vector2 vector2 = new Vector2(GetStartOffset(0, vector.x), GetStartOffset(1, vector.y));
		for (int j = 0; j < base.rectChildren.Count; j++)
		{
			int num8;
			int num9;
			if (startAxis == Axis.Horizontal)
			{
				num8 = j % num5;
				num9 = j / num5;
			}
			else
			{
				num8 = j / num5;
				num9 = j % num5;
			}
			if (num3 == 1)
			{
				num8 = num6 - 1 - num8;
			}
			if (num4 == 1)
			{
				num9 = num7 - 1 - num9;
			}
			float num10 = (x - spacing[0] * (float)(num6 - 1)) / (float)num6;
			SetChildAlongAxis(base.rectChildren[j], 0, vector2.x + (num10 + spacing[0]) * (float)num8, num10);
			SetChildAlongAxis(base.rectChildren[j], 1, vector2.y + (cellSize[1] + spacing[1]) * (float)num9, cellSize[1]);
		}
	}
}
