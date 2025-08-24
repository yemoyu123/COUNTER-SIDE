using UnityEngine;

namespace NKC.FX;

public class NKC_FX_PTC_OFFICE_MAP_TILE : MonoBehaviour
{
	public RectTransform Reference;

	public float DefaultTileSize = 200f;

	public float DefaultShapeScale = 450f;

	private ParticleSystem ps;

	private Vector3 offsetScale;

	private ParticleSystem.ShapeModule shapeModule;

	private void OnEnable()
	{
		float width = Reference.rect.width;
		float num = Mathf.Abs(DefaultTileSize - width);
		_ = DefaultTileSize;
		ps = GetComponent<ParticleSystem>();
		shapeModule = ps.shape;
		offsetScale.Set(DefaultShapeScale + num, DefaultShapeScale + num, 1f);
		shapeModule.scale = offsetScale;
	}
}
