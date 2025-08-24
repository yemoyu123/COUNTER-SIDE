using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/ScreenWater")]
public class ScreenWater : MonoBehaviour
{
	public Shader shaderRGB;

	private Material m_MaterialRGB;

	public Texture2D WaterFlows;

	public Texture2D WaterMask;

	public Texture2D WetScreen;

	public float Speed = 0.5f;

	public float Intens = 0.5f;

	protected Material material
	{
		get
		{
			if (m_MaterialRGB == null)
			{
				m_MaterialRGB = new Material(shaderRGB);
				m_MaterialRGB.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_MaterialRGB;
		}
	}

	protected void Start()
	{
		if (shaderRGB == null)
		{
			Debug.Log("Sat shaders are not set up! Disabling saturation effect.");
			base.enabled = false;
		}
		else if (!shaderRGB.isSupported)
		{
			base.enabled = false;
		}
	}

	protected void OnDisable()
	{
		if ((bool)m_MaterialRGB)
		{
			Object.DestroyImmediate(m_MaterialRGB);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Material material = this.material;
		material.SetTexture("_Splat", WaterFlows);
		material.SetTexture("_Flow", WaterMask);
		material.SetTexture("_Water", WetScreen);
		material.SetFloat("_Speed", Speed);
		material.SetFloat("_Intens", Intens);
		Graphics.Blit(source, destination, material);
	}
}
