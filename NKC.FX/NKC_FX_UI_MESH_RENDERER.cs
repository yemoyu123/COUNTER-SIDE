using UnityEngine;
using UnityEngine.UI;

namespace NKC.FX;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasRenderer))]
public class NKC_FX_UI_MESH_RENDERER : MaskableGraphic
{
	public Mesh m_Mesh;

	[SerializeField]
	private Texture texture;

	public float m_Thickness = 1f;

	private Vector3 vertMax = Vector3.zero;

	private Vector3 vertMin = Vector3.zero;

	private bool hasUV;

	private bool hasUV2;

	private bool hasNormals;

	private bool hasTangents;

	private float scaleFactor = 1f;

	public Texture Texture
	{
		get
		{
			return texture;
		}
		set
		{
			if (!(texture == value))
			{
				texture = value;
				SetMaterialDirty();
			}
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (!(texture == null))
			{
				return texture;
			}
			return Graphic.s_WhiteTexture;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void Awake()
	{
		base.Awake();
		PrepareResource();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Initialize();
	}

	public override void SetMaterialDirty()
	{
		base.SetMaterialDirty();
		SetTexture();
	}

	public void SetTexture()
	{
		if (m_Material.mainTexture != null)
		{
			Texture = m_Material.mainTexture;
		}
		else
		{
			Texture = Texture2D.whiteTexture;
		}
	}

	public void SetScaleFromPixelPerUnit()
	{
		if (base.rectTransform != null)
		{
			base.rectTransform.SetWidth(1f);
			base.rectTransform.SetHeight(1f);
			Canvas componentInParent = base.gameObject.GetComponentInParent<Canvas>(includeInactive: true);
			if (componentInParent != null)
			{
				base.rectTransform.localScale = Vector3.one * componentInParent.referencePixelsPerUnit;
			}
			else
			{
				base.rectTransform.localScale = Vector3.one * 100f;
			}
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		if (m_Mesh == null)
		{
			return;
		}
		Vector3 b = new Vector3(base.rectTransform.rect.width, base.rectTransform.rect.height, m_Thickness);
		b *= scaleFactor;
		for (int i = 0; i < m_Mesh.vertices.Length; i++)
		{
			if (hasUV2 && hasNormals && hasTangents)
			{
				vh.AddVert(Vector3.Scale(m_Mesh.vertices[i], b), color, m_Mesh.uv[i], m_Mesh.uv2[i], m_Mesh.normals[i], m_Mesh.tangents[i]);
			}
			else if (hasUV)
			{
				vh.AddVert(Vector3.Scale(m_Mesh.vertices[i], b), color, m_Mesh.uv[i]);
			}
			else
			{
				vh.AddVert(Vector3.Scale(m_Mesh.vertices[i], b), color, CalculateRectSpaceUV(m_Mesh.vertices[i]));
			}
		}
		for (int j = 0; j < m_Mesh.triangles.Length; j += 3)
		{
			vh.AddTriangle(m_Mesh.triangles[j], m_Mesh.triangles[j + 1], m_Mesh.triangles[j + 2]);
		}
	}

	private void PrepareResource()
	{
		if (m_Mesh == null)
		{
			m_Mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
		}
		if (m_Material == null)
		{
			m_Material = Graphic.defaultGraphicMaterial;
		}
	}

	private void Initialize()
	{
		if (m_Mesh != null && m_Material != null)
		{
			hasUV = m_Mesh.uv != null && m_Mesh.uv.Length != 0;
			hasUV2 = m_Mesh.uv2 != null && m_Mesh.uv2.Length != 0;
			hasNormals = m_Mesh.normals != null && m_Mesh.normals.Length != 0;
			hasTangents = m_Mesh.tangents != null && m_Mesh.tangents.Length != 0;
			SetTexture();
		}
	}

	private Vector2 CalculateRectSpaceUV(Vector3 pos)
	{
		return new Vector2(Mapping(pos.x, vertMin.x, vertMax.x, 0f, 1f), Mapping(pos.y, vertMin.y, vertMax.y, 0f, 1f));
	}

	private float Mapping(float x, float aMin, float aMax, float bMin, float bMax)
	{
		return (x - aMin) * (bMax - bMin) / (aMax - aMin) + bMin;
	}
}
