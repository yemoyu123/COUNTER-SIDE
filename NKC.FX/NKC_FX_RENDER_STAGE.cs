using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NKC.FX;

[RequireComponent(typeof(Camera))]
public class NKC_FX_RENDER_STAGE : MonoBehaviour
{
	public Camera m_Camara;

	[FormerlySerializedAs("m_RendTexture")]
	public RenderTexture m_RenderTexture;

	public string m_TagName;

	[Space]
	public bool m_UseInstance;

	public bool m_IsMaskableGraphic;

	public Material m_Material;

	private RenderTexture instanceRT;

	private Material instanceMat;

	public Renderer[] m_Renderers;

	public MaskableGraphic[] m_MaskableGraphics;

	private bool init;

	private bool renderMain;

	public bool RenderMain
	{
		get
		{
			return renderMain;
		}
		set
		{
			renderMain = value;
		}
	}

	private void Awake()
	{
		if (m_Camara == null)
		{
			m_Camara = GetComponent<Camera>();
		}
		if (m_Camara == null)
		{
			Debug.LogWarning("Null Camara", base.gameObject);
		}
		if (m_RenderTexture == null)
		{
			Debug.LogWarning("Null RendTexture", base.gameObject);
		}
		if (m_Camara != null && m_RenderTexture != null)
		{
			init = true;
			if (m_UseInstance)
			{
				if (instanceRT == null)
				{
					instanceRT = Object.Instantiate(m_RenderTexture);
				}
				if (instanceRT != null)
				{
					m_Camara.targetTexture = instanceRT;
				}
				if (m_IsMaskableGraphic)
				{
					return;
				}
				if (instanceMat == null)
				{
					if (m_Material != null)
					{
						instanceMat = Object.Instantiate(m_Material);
					}
					else
					{
						Debug.LogWarning("Null Material", base.gameObject);
					}
				}
				if (instanceRT != null && instanceMat != null)
				{
					instanceMat.mainTexture = instanceRT;
				}
			}
			else
			{
				if (m_Material != null)
				{
					m_Material.mainTexture = m_RenderTexture;
				}
				m_Camara.targetTexture = m_RenderTexture;
			}
		}
		else
		{
			init = false;
			Debug.LogWarning("Not Initialized", base.gameObject);
		}
	}

	private void SetRenderTextureFormat(RenderTexture _rt, RenderTextureFormat _renderTextureFormat)
	{
		if (!_rt.IsCreated())
		{
			if (SystemInfo.SupportsRenderTextureFormat(_renderTextureFormat))
			{
				_rt.format = _renderTextureFormat;
			}
			else
			{
				_rt.format = RenderTextureFormat.Default;
			}
			Debug.Log("Set RenderTexture TextureFormat. (" + _renderTextureFormat.ToString() + ")", base.gameObject);
		}
		else
		{
			Debug.LogWarning("Can't Setting Texture Format. Already created RenderTexture.", base.gameObject);
		}
	}

	private void SetRenderTextureResolution(RenderTexture _rt, Vector2Int _resolution)
	{
		if (!_rt.IsCreated())
		{
			_rt.width = _resolution.x;
			_rt.height = _resolution.y;
			Debug.Log($"Set RenderTexture Resolution. ({_resolution.x} x {_resolution.y})", base.gameObject);
		}
		else
		{
			Debug.LogWarning("Can't Setting Resolution. Already created RenderTexture.", base.gameObject);
		}
	}

	private void OnEnable()
	{
		if (!init)
		{
			return;
		}
		if (m_UseInstance)
		{
			if (m_IsMaskableGraphic)
			{
				SetMaskableGraphic(instanceRT);
			}
			else
			{
				SetRenderer(instanceMat);
			}
			SetRenderStage(instanceRT);
		}
		else
		{
			SetRenderStage(m_RenderTexture);
		}
	}

	private void OnDisable()
	{
		if (!init)
		{
			return;
		}
		renderMain = false;
		m_Camara.enabled = false;
		m_Camara.targetTexture = null;
		if (m_UseInstance)
		{
			if (m_IsMaskableGraphic)
			{
				SetMaskableGraphic(m_RenderTexture);
			}
			else
			{
				SetRenderer(m_Material);
			}
		}
	}

	private void OnDestroy()
	{
		m_Camara.targetTexture = null;
		m_Camara = null;
		if (m_RenderTexture != null)
		{
			m_RenderTexture.Release();
			m_RenderTexture = null;
		}
		if (instanceRT != null)
		{
			instanceRT.Release();
			instanceRT = null;
		}
		m_Material = null;
		instanceMat = null;
		m_Renderers = null;
		m_MaskableGraphics = null;
	}

	public void SetRenderer(Material _mat)
	{
		for (int i = 0; i < m_Renderers.Length; i++)
		{
			if (m_Renderers[i] != null)
			{
				m_Renderers[i].sharedMaterial = _mat;
			}
		}
	}

	public void SetMaskableGraphic(Texture _tex)
	{
		for (int i = 0; i < m_MaskableGraphics.Length; i++)
		{
			if (m_MaskableGraphics[i] != null)
			{
				(m_MaskableGraphics[i] as RawImage).texture = _tex;
			}
		}
	}

	public void SetRenderStage(RenderTexture _rt)
	{
		if (_rt != null && m_Camara.targetTexture == null)
		{
			m_Camara.targetTexture = _rt;
		}
		if (!string.IsNullOrEmpty(m_TagName))
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(m_TagName);
			NKC_FX_RENDER_STAGE nKC_FX_RENDER_STAGE = this;
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				nKC_FX_RENDER_STAGE = array[i].GetComponent<NKC_FX_RENDER_STAGE>();
				if (nKC_FX_RENDER_STAGE.RenderMain)
				{
					flag = true;
					continue;
				}
				nKC_FX_RENDER_STAGE.RenderMain = false;
				nKC_FX_RENDER_STAGE.m_Camara.enabled = false;
			}
			if (flag)
			{
				return;
			}
			for (int j = 0; j < array.Length; j++)
			{
				nKC_FX_RENDER_STAGE = array[j].GetComponent<NKC_FX_RENDER_STAGE>();
				if (nKC_FX_RENDER_STAGE.isActiveAndEnabled)
				{
					nKC_FX_RENDER_STAGE.RenderMain = true;
					nKC_FX_RENDER_STAGE.m_Camara.enabled = true;
					break;
				}
			}
		}
		else
		{
			renderMain = true;
			m_Camara.enabled = true;
		}
	}
}
