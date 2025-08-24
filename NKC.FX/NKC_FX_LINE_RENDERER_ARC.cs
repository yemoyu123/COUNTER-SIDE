using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NKC.FX;

[ExecuteAlways]
public class NKC_FX_LINE_RENDERER_ARC : MonoBehaviour
{
	public enum ArcAxis
	{
		Top,
		Bottom,
		Right,
		Left
	}

	public Transform StartPoint;

	public Transform EndPoint;

	public Transform ArcPoint;

	[Range(1f, 6f)]
	public int Resolution = 1;

	public bool AutoAdjustWidth;

	[Range(0.01f, 2f)]
	public float WidthFactor = 0.5f;

	[Range(0.01f, 1f)]
	public float WidthPower = 0.5f;

	public bool AutoAdjustSkew;

	public float MaximumDistance = 500f;

	[Range(0f, 1f)]
	public float Skewness = 0.5f;

	public bool AutoAdjustArc;

	public bool WeightedArc;

	public ArcAxis Axis;

	[Range(0f, 100f)]
	public float Arched = 1f;

	public int ArcMultiplier = 1;

	public bool ShowGizmo;

	private int vertexCount;

	private LineRenderer LR;

	private bool init;

	private float distance;

	private float weight = 1f;

	private Vector3 FixedArcPosition;

	private List<Vector3> pointlist = new List<Vector3>();

	private void OnDestroy()
	{
		if (StartPoint != null)
		{
			StartPoint = null;
		}
		if (EndPoint != null)
		{
			EndPoint = null;
		}
		if (ArcPoint != null)
		{
			ArcPoint = null;
		}
		if (LR != null)
		{
			LR = null;
		}
		if (pointlist != null)
		{
			pointlist.Clear();
			pointlist = null;
		}
	}

	private void OnEnable()
	{
		base.transform.hasChanged = true;
		Init();
	}

	public void Init()
	{
		if (LR == null)
		{
			LR = GetComponent<LineRenderer>();
		}
		if (StartPoint == null)
		{
			StartPoint = base.transform;
		}
		if (LR != null && StartPoint != null && ArcPoint != null && EndPoint != null)
		{
			LR.receiveShadows = false;
			LR.allowOcclusionWhenDynamic = false;
			LR.shadowCastingMode = ShadowCastingMode.Off;
			LR.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			LR.loop = false;
			LR.useWorldSpace = true;
			vertexCount = (int)Mathf.Pow(2f, Resolution);
			init = true;
		}
		else
		{
			init = false;
		}
	}

	private void OnValidate()
	{
		Init();
	}

	private void Update()
	{
		SetPositions();
	}

	private void SetPositions()
	{
		if (!base.enabled || !init)
		{
			return;
		}
		if (AutoAdjustSkew || AutoAdjustArc)
		{
			distance = Vector3.Distance(StartPoint.position, EndPoint.position);
			if (AutoAdjustWidth)
			{
				LR.widthMultiplier = WidthFactor * distance * WidthPower;
			}
			if (AutoAdjustSkew)
			{
				FixedArcPosition = Vector3.Lerp(EndPoint.position, StartPoint.position, Mathf.Clamp01(MaximumDistance / distance));
			}
			else
			{
				FixedArcPosition = Vector3.Lerp(EndPoint.position, StartPoint.position, Skewness);
			}
			if (AutoAdjustArc)
			{
				if (WeightedArc)
				{
					weight = distance;
				}
				else
				{
					weight = ArcMultiplier;
				}
				switch (Axis)
				{
				case ArcAxis.Top:
					FixedArcPosition.Set(FixedArcPosition.x, FixedArcPosition.y + weight * Arched, FixedArcPosition.z);
					break;
				case ArcAxis.Bottom:
					FixedArcPosition.Set(FixedArcPosition.x, FixedArcPosition.y - weight * Arched, FixedArcPosition.z);
					break;
				case ArcAxis.Right:
					FixedArcPosition.Set(FixedArcPosition.x, FixedArcPosition.y, FixedArcPosition.z + weight * Arched);
					break;
				case ArcAxis.Left:
					FixedArcPosition.Set(FixedArcPosition.x, FixedArcPosition.y, FixedArcPosition.z - weight * Arched);
					break;
				}
				ArcPoint.position = FixedArcPosition;
			}
		}
		if (base.transform.hasChanged || StartPoint.hasChanged || EndPoint.hasChanged || ArcPoint.hasChanged)
		{
			Transform obj = base.transform;
			Transform startPoint = StartPoint;
			Transform endPoint = EndPoint;
			bool flag = (ArcPoint.hasChanged = false);
			bool flag3 = (endPoint.hasChanged = flag);
			bool hasChanged = (startPoint.hasChanged = flag3);
			obj.hasChanged = hasChanged;
			pointlist.Clear();
			for (float num = 0f; num <= 1f; num += 1f / (float)vertexCount)
			{
				Vector3 a = Vector3.Lerp(EndPoint.position, ArcPoint.position, num);
				Vector3 b = Vector3.Lerp(ArcPoint.position, StartPoint.position, num);
				Vector3 item = Vector3.Lerp(a, b, num);
				pointlist.Add(item);
			}
			LR.positionCount = pointlist.Count;
			LR.SetPositions(pointlist.ToArray());
		}
	}

	private void OnDrawGizmos()
	{
		if (base.enabled && init && ShowGizmo)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(EndPoint.position, ArcPoint.position);
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(ArcPoint.position, StartPoint.position);
			Gizmos.color = Color.red;
			for (float num = 0.5f / (float)vertexCount; num < 1f; num += 1f / (float)vertexCount)
			{
				Gizmos.DrawLine(Vector3.Lerp(EndPoint.position, ArcPoint.position, num), Vector3.Lerp(ArcPoint.position, StartPoint.position, num));
			}
		}
	}
}
