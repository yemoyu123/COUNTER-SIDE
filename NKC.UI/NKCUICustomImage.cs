using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

[AddComponentMenu("UI/Custom Image")]
[ExecuteInEditMode]
public class NKCUICustomImage : Image
{
	[HideInInspector]
	[Header("이미지 마스킹 영역 조정(절대 좌표)")]
	public Vector2[] vertOffset;

	[HideInInspector]
	[Header("이미지 마스킹 영역 조정(상대 좌표)")]
	public Vector2[] vertRelativeOffset;

	[Header("이미지 원본 형태 유지 설정")]
	public bool maintainImage = true;

	[Header("이미지 마스킹 좌표 타입(상대 좌표)")]
	public bool relativeOffset;

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		base.OnPopulateMesh(vh);
		if (vertOffset == null)
		{
			vertOffset = new Vector2[vh.currentVertCount];
		}
		if (vertRelativeOffset == null)
		{
			vertRelativeOffset = new Vector2[vh.currentVertCount];
		}
		int currentVertCount = vh.currentVertCount;
		for (int i = 0; i < currentVertCount; i++)
		{
			UIVertex vertex = UIVertex.simpleVert;
			vh.PopulateUIVertex(ref vertex, i);
			Vector3 position = vertex.position;
			if (relativeOffset)
			{
				if (base.rectTransform != null)
				{
					Vector3 vector = new Vector3(base.rectTransform.rect.width * vertRelativeOffset[i].x, base.rectTransform.rect.height * vertRelativeOffset[i].y, 0f);
					position += vector;
					vertOffset[i] = vector;
					if (maintainImage)
					{
						Vector2 vector2 = vertex.uv0;
						vector2.x += vertRelativeOffset[i].x;
						vector2.y += vertRelativeOffset[i].y;
						vertex.uv0 = vector2;
					}
				}
			}
			else
			{
				position += new Vector3(vertOffset[i].x, vertOffset[i].y, 0f);
				if (base.rectTransform != null && base.rectTransform.rect.width != 0f && base.rectTransform.rect.height != 0f)
				{
					vertRelativeOffset[i] = new Vector3(vertOffset[i].x / base.rectTransform.rect.width, vertOffset[i].y / base.rectTransform.rect.height, 0f);
					if (maintainImage)
					{
						Vector2 vector3 = vertex.uv0;
						vector3.x += vertRelativeOffset[i].x;
						vector3.y += vertRelativeOffset[i].y;
						vertex.uv0 = vector3;
					}
				}
			}
			vertex.position = position;
			vh.SetUIVertex(vertex, i);
		}
	}
}
