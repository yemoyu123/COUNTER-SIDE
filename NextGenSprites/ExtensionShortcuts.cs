using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace NextGenSprites;

public static class ExtensionShortcuts
{
	private static readonly string[] FloatProperties = new string[36]
	{
		"_CurvatureDepth", "_Specular", "_ReflectionStrength", "_ReflectionBlur", "_ReflectionScrollingX", "_ReflectionScrollingY", "_EmissionStrength", "_EmissionBlendAnimation1", "_EmissionPulseSpeed1", "_EmissionStrength2",
		"_EmissionBlendAnimation2", "_EmissionPulseSpeed2", "_EmissionStrength3", "_EmissionBlendAnimation3", "_EmissionPulseSpeed3", "_TransmissionDensity", "_DissolveBlend", "_DissolveBorderWidth", "_DissolveGlowStrength", "_RefractionStrength",
		"_FlowIntensity", "_FlowSpeed", "_Layer0ScrollingX", "_Layer0ScrollingY", "_Layer1Opacity", "_Layer1ScrollingX", "_Layer1ScrollingY", "_Layer2Opacity", "_Layer2ScrollingX", "_Layer2ScrollingY",
		"_Layer3Opacity", "_Layer3ScrollingX", "_Layer3ScrollingY", "_Layer0AutoScrollSpeed", "m_fOutlineWide", "_SpriteBlending"
	};

	private static readonly string[] Vector4Properties = new string[1] { "_HSBC" };

	private static readonly float[,] MinMaxFloatProperties = new float[35, 2]
	{
		{ -1f, 1f },
		{ 0f, 0.7f },
		{ 0f, 1f },
		{ 0f, 9f },
		{ 0f, 5f },
		{ 0f, 5f },
		{ 0f, 5f },
		{ 0f, 1f },
		{ 0f, 10f },
		{ 0f, 5f },
		{ 0f, 1f },
		{ 0f, 10f },
		{ 0f, 5f },
		{ 0f, 1f },
		{ 0f, 10f },
		{ 0f, 1f },
		{ 0f, 1f },
		{ 0f, 100f },
		{ 0f, 5f },
		{ -1f, 1f },
		{ -1f, 1f },
		{ -10f, 10f },
		{ -1f, 1f },
		{ -1f, 1f },
		{ 0f, 1f },
		{ -1f, 1f },
		{ -1f, 1f },
		{ 0f, 1f },
		{ -1f, 1f },
		{ -1f, 1f },
		{ 0f, 1f },
		{ -1f, 1f },
		{ -1f, 1f },
		{ 0f, 2f },
		{ 0f, 5f }
	};

	private static readonly string[] TextureProperties = new string[14]
	{
		"_MainTex", "_Layer1", "_Layer2", "_Layer3", "_StencilMask", "_BumpMap", "_Illum", "_ReflectionTex", "_ReflectionMask", "_TransmissionTex",
		"_DissolveTex", "_RefractionNormal", "_FlowMap", "_RenderTexture"
	};

	private static readonly string[] TintProperties = new string[11]
	{
		"_Color", "_Layer1Color", "_Layer2Color", "_Layer3Color", "_SpecColor", "_EmissionTint", "_EmissionTint2", "_EmissionTint3", "_DissolveGlowColor", "m_v4OutlineColor",
		"m_AmbientColor"
	};

	private static readonly string[] ShaderKeywordProperties = new string[14]
	{
		"SPRITE_MULTILAYER_ON", "SPRITE_SCROLLING_ON", "SPRITE_STENCIL_ON", "CURVATURE_ON", "REFLECTION_ON", "EMISSION_ON", "EMISSION_PULSE_ON", "TRANSMISSION_ON", "DISSOLVE_ON", "DOUBLESIDED_ON",
		"PIXELSNAP_ON", "AUTOSCROLL_ON", "HSB_TINT_ON", "RENDER_TEXTURE_ON"
	};

	private static readonly string[] ShaderRuntimeKeywordProperties = new string[4] { "CURVATURE_ON", "REFLECTION_ON", "EMISSION_ON", "DISSOLVE_ON" };

	public static string GetString(this ShaderFloat slot)
	{
		return FloatProperties[(int)slot];
	}

	public static string GetString(this ShaderVector4 slot)
	{
		return Vector4Properties[(int)slot];
	}

	public static string GetString(this ShaderTexture slot)
	{
		return TextureProperties[(int)slot];
	}

	public static string GetString(this ShaderColor slot)
	{
		return TintProperties[(int)slot];
	}

	public static string GetString(this ShaderFeature slot)
	{
		return ShaderKeywordProperties[(int)slot];
	}

	public static string GetString(this ShaderFeatureRuntime slot)
	{
		return ShaderRuntimeKeywordProperties[(int)slot];
	}

	public static float GetMax(this ShaderFloat slot)
	{
		return MinMaxFloatProperties[(int)slot, 1];
	}

	public static float GetMin(this ShaderFloat slot)
	{
		return MinMaxFloatProperties[(int)slot, 0];
	}

	public static void ToggleShadowCasting(this GameObject go, bool toggle)
	{
		ShadowCastingMode shadowCastingMode = (toggle ? ShadowCastingMode.On : ShadowCastingMode.Off);
		go.GetComponent<SpriteRenderer>().shadowCastingMode = shadowCastingMode;
	}

	public static void CopyToPropertyBlock(this MaterialPropertyBlock mBlock, Material mat)
	{
		string[] names = Enum.GetNames(typeof(ShaderFloat));
		for (int i = 0; i < names.Length; i++)
		{
			string name = FloatProperties[i];
			if (mat.HasProperty(name))
			{
				mBlock.SetFloat(name, mat.GetFloat(name));
			}
		}
		string[] names2 = Enum.GetNames(typeof(ShaderColor));
		for (int j = 0; j < names2.Length; j++)
		{
			string name2 = TintProperties[j];
			if (mat.HasProperty(name2))
			{
				mBlock.SetColor(name2, mat.GetColor(name2));
			}
		}
		string[] names3 = Enum.GetNames(typeof(ShaderTexture));
		for (int k = 1; k < names3.Length; k++)
		{
			string name3 = TextureProperties[k];
			if (mat.HasProperty(name3))
			{
				Texture texture = mat.GetTexture(name3);
				if ((bool)texture)
				{
					mBlock.SetTexture(name3, texture);
				}
			}
		}
	}
}
