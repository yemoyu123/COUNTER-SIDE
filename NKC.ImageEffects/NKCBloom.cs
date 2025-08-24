using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace NKC.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class NKCBloom : PostEffectsBase
{
	public enum LensFlareStyle
	{
		Ghosting,
		Anamorphic,
		Combined
	}

	public enum TweakMode
	{
		Basic,
		Complex
	}

	public enum HDRBloomMode
	{
		Auto,
		On,
		Off
	}

	public enum BloomScreenBlendMode
	{
		Screen,
		Add
	}

	public enum BloomQuality
	{
		Cheap,
		High
	}

	public TweakMode tweakMode;

	public BloomScreenBlendMode screenBlendMode = BloomScreenBlendMode.Add;

	public float sepBlurSpread = 2.5f;

	public BloomQuality quality = BloomQuality.High;

	public float bloomIntensity = 0.5f;

	public float bloomThreshold = 0.5f;

	public Color bloomThresholdColor = Color.white;

	public int bloomBlurIterations = 2;

	public int hollywoodFlareBlurIterations = 2;

	public float flareRotation;

	public LensFlareStyle lensflareMode = LensFlareStyle.Anamorphic;

	public float hollyStretchWidth = 2.5f;

	public float lensflareIntensity;

	public float lensflareThreshold = 0.3f;

	public float lensFlareSaturation = 0.75f;

	public Color flareColorA = new Color(0.4f, 0.4f, 0.8f, 0.75f);

	public Color flareColorB = new Color(0.4f, 0.8f, 0.8f, 0.75f);

	public Color flareColorC = new Color(0.8f, 0.4f, 0.8f, 0.75f);

	public Color flareColorD = new Color(0.8f, 0.4f, 0f, 0.75f);

	public Texture2D lensFlareVignetteMask;

	public Shader lensFlareShader;

	private Material lensFlareMaterial;

	public Shader screenBlendShader;

	private Material screenBlend;

	public Shader blurAndFlaresShader;

	private Material blurAndFlaresMaterial;

	public Shader brightPassFilterShader;

	private Material brightPassFilterMaterial;

	private Camera m_Camera;

	public override bool CheckResources()
	{
		CheckSupport(needDepth: false);
		screenBlend = CheckShaderAndCreateMaterial(screenBlendShader, screenBlend);
		lensFlareMaterial = CheckShaderAndCreateMaterial(lensFlareShader, lensFlareMaterial);
		blurAndFlaresMaterial = CheckShaderAndCreateMaterial(blurAndFlaresShader, blurAndFlaresMaterial);
		brightPassFilterMaterial = CheckShaderAndCreateMaterial(brightPassFilterShader, brightPassFilterMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		int width = source.width / 4;
		int height = source.height / 4;
		float num = 1f * (float)source.width / (1f * (float)source.height);
		float num2 = 0.001953125f;
		RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default);
		screenBlend.SetVector("_Threshhold", bloomThreshold * bloomThresholdColor);
		Graphics.Blit(source, temporary, screenBlend, 1);
		float num3 = sepBlurSpread;
		RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default);
		blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(0f, num3 * num2, 0f, 0f));
		Graphics.Blit(temporary, temporary2, blurAndFlaresMaterial, 4);
		RenderTexture.ReleaseTemporary(temporary);
		temporary = temporary2;
		temporary2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default);
		blurAndFlaresMaterial.SetVector("_Offsets", new Vector4(num3 / num * num2, 0f, 0f, 0f));
		Graphics.Blit(temporary, temporary2, blurAndFlaresMaterial, 4);
		RenderTexture.ReleaseTemporary(temporary);
		temporary = temporary2;
		screenBlend.SetFloat("_Intensity", bloomIntensity);
		screenBlend.SetTexture("_ColorBuffer", source);
		Graphics.Blit(temporary, destination, screenBlend, 0);
		RenderTexture.ReleaseTemporary(temporary);
	}
}
