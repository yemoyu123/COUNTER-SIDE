using UnityEngine;
using UnityEngine.UI;

namespace NKC;

[RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
[DisallowMultipleComponent]
public class NKCUIComRaycastTarget : Graphic
{
	public override void SetMaterialDirty()
	{
	}

	public override void SetVerticesDirty()
	{
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
