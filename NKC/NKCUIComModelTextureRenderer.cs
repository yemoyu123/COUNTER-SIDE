using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComModelTextureRenderer : MonoBehaviour
{
	public RawImage m_rawImage;

	public RectTransform m_rtImage;

	private Bounds m_RenderBound;

	private RenderTexture m_Texture;

	public Camera m_RenderCamera;

	private Vector3[] m_WorldCornerPosArray = new Vector3[4];

	private Camera RenderCamera
	{
		get
		{
			if (m_RenderCamera == null)
			{
				m_RenderCamera = NKCScenManager.GetScenManager().TextureCamera;
			}
			return m_RenderCamera;
		}
	}

	private void Awake()
	{
		m_rawImage.color = new Color(1f, 1f, 1f, 1f);
	}

	public void PrepareTexture(Material mat)
	{
		if (m_Texture == null)
		{
			BuildTexture((int)m_rtImage.GetWidth(), (int)m_rtImage.GetHeight(), mat);
			m_RenderBound = new Bounds
			{
				size = new Vector3(m_rtImage.GetWidth(), m_rtImage.GetHeight(), 1f),
				center = m_rtImage.position
			};
		}
	}

	public void PrepareTexture(Material mat, int width, int height)
	{
		if (m_Texture == null)
		{
			BuildTexture(width, height, mat);
			m_RenderBound = new Bounds
			{
				size = new Vector3(width, height, 1f),
				center = m_rtImage.position
			};
		}
	}

	private void BuildTexture(int textureResX, int textureResY, Material mat)
	{
		CleanUp();
		if (mat != null)
		{
			m_rawImage.material = mat;
		}
		m_Texture = new RenderTexture(textureResX, textureResY, 0, RenderTextureFormat.ARGB32);
		m_Texture.wrapMode = TextureWrapMode.Clamp;
		m_Texture.antiAliasing = 1;
		m_Texture.filterMode = FilterMode.Bilinear;
		m_Texture.anisoLevel = 0;
		m_Texture.Create();
		m_rawImage.texture = m_Texture;
		m_rawImage.SetAllDirty();
	}

	public void CleanUp()
	{
		if (m_rawImage != null)
		{
			m_rawImage.texture = null;
		}
		if (m_Texture != null)
		{
			m_Texture.Release();
			Object.DestroyImmediate(m_Texture);
			m_Texture = null;
		}
	}

	private void Update()
	{
		TextureCapture(m_RenderBound, ref m_Texture);
	}

	public void TextureCapture(Bounds bound, ref RenderTexture Texture)
	{
		if (!(m_Texture == null) && bound.size.x != 0f && bound.size.y != 0f)
		{
			m_rtImage.GetWorldCorners(m_WorldCornerPosArray);
			NKCCamera.FitCameraToWorldRect(RenderCamera, m_WorldCornerPosArray);
			RenderCamera.targetTexture = Texture;
			int layer = m_rtImage.gameObject.layer;
			NKCUtil.SetLayer(m_rtImage, 31);
			RenderCamera.Render();
			NKCUtil.SetLayer(m_rtImage, layer);
			RenderCamera.targetTexture = null;
		}
	}

	public void SetMaterial(Material mat)
	{
		if (m_rawImage != null)
		{
			m_rawImage.material = mat;
		}
	}

	public Material GetMaterial()
	{
		if (m_rawImage != null)
		{
			return m_rawImage.material;
		}
		return null;
	}

	public void SetColor(Color color)
	{
		if (m_rawImage != null)
		{
			m_rawImage.color = color;
		}
	}

	public Color GetColor()
	{
		if (m_rawImage != null)
		{
			return m_rawImage.color;
		}
		return Color.white;
	}

	private void ChangeLayer(Transform target, int layer)
	{
		foreach (Transform item in target)
		{
			item.gameObject.layer = layer;
			ChangeLayer(item, layer);
		}
	}
}
