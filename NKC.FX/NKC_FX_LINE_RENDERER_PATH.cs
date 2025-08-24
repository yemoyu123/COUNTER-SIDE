using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NKC.FX;

public class NKC_FX_LINE_RENDERER_PATH : MonoBehaviour
{
	public NKC_FX_PATH Path;

	[Range(1f, 8f)]
	public int Resolution = 4;

	private LineRenderer LR;

	private int vertexCount;

	private List<Vector3> pointlist = new List<Vector3>();

	private bool init;

	private void OnValidate()
	{
		Init();
		if (init)
		{
			vertexCount = (int)Mathf.Pow(2f, Resolution);
			if (vertexCount + 1 != LR.positionCount)
			{
				SetPositions();
			}
		}
	}

	private void Init()
	{
		if (Path == null)
		{
			Path = GetComponent<NKC_FX_PATH>();
			if (Path == null)
			{
				Debug.LogWarning("Null NKC_FX_PATH -> " + base.name, base.gameObject);
			}
		}
		if (LR == null)
		{
			LR = GetComponent<LineRenderer>();
			if (LR == null)
			{
				Debug.LogWarning("Null LineRenderer -> " + base.name, base.gameObject);
			}
			else
			{
				LR.receiveShadows = false;
				LR.allowOcclusionWhenDynamic = false;
				LR.shadowCastingMode = ShadowCastingMode.Off;
				LR.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
				LR.useWorldSpace = false;
				LR.loop = false;
			}
		}
		if (Path != null && LR != null)
		{
			init = true;
		}
		else
		{
			init = false;
		}
	}

	public void SetPositions()
	{
		if (init)
		{
			pointlist.Clear();
			for (float num = 0f; num <= 1f; num += 1f / (float)vertexCount)
			{
				pointlist.Add(Path.GetPoint(num, Space.Self));
			}
			LR.positionCount = pointlist.Count;
			LR.SetPositions(pointlist.ToArray());
		}
		else
		{
			Init();
		}
	}
}
