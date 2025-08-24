using System;
using System.Collections.Generic;
using UnityEngine;

namespace NextGenSprites;

public class DualMaterial
{
	private class FloatPropertyRange
	{
		public ShaderFloat Property { get; set; }

		public float ValueMain { get; set; }

		public float ValueSecond { get; set; }
	}

	private class ColorPropertyRange
	{
		public ShaderColor Property { get; set; }

		public Color ValueMain { get; set; }

		public Color ValueSecond { get; set; }
	}

	private float _lerpAmount;

	private Material _mainMaterial;

	private FloatPropertyRange[] FloatPropertyValues;

	private ColorPropertyRange[] ColorPropertyValues;

	public Material FusedMaterial => _mainMaterial;

	public float Lerp
	{
		get
		{
			return _lerpAmount;
		}
		set
		{
			_lerpAmount = Mathf.Clamp01(value);
			LerpMaterial();
		}
	}

	public DualMaterial(Material firstMaterial, Material secondMaterial, string materialName = "New Dual Material")
	{
		if ((bool)firstMaterial && (bool)secondMaterial)
		{
			if (string.CompareOrdinal(firstMaterial.shader.name, secondMaterial.shader.name) != 0)
			{
				Debug.LogError("Invalid Materials. Both must use the same NextGenSprite shader");
				_mainMaterial = null;
			}
			_mainMaterial = new Material(Shader.Find(firstMaterial.shader.name));
			_mainMaterial.CopyPropertiesFromMaterial(firstMaterial);
			_mainMaterial.name = materialName;
			SetKeywords(firstMaterial);
			BuildProperties(firstMaterial, secondMaterial);
		}
		else
		{
			Debug.LogError("One or both of the provided Materials are null");
			_mainMaterial = null;
		}
	}

	private void LerpMaterial()
	{
		FloatPropertyRange[] floatPropertyValues = FloatPropertyValues;
		foreach (FloatPropertyRange floatPropertyRange in floatPropertyValues)
		{
			float value = Mathf.Lerp(floatPropertyRange.ValueMain, floatPropertyRange.ValueSecond, _lerpAmount);
			_mainMaterial.SetFloat(floatPropertyRange.Property.GetString(), value);
		}
		ColorPropertyRange[] colorPropertyValues = ColorPropertyValues;
		foreach (ColorPropertyRange colorPropertyRange in colorPropertyValues)
		{
			Color value2 = Color.Lerp(colorPropertyRange.ValueMain, colorPropertyRange.ValueSecond, _lerpAmount);
			_mainMaterial.SetColor(colorPropertyRange.Property.GetString(), value2);
		}
	}

	private void BuildProperties(Material firstMat, Material secondMat)
	{
		List<FloatPropertyRange> list = new List<FloatPropertyRange>();
		List<ColorPropertyRange> list2 = new List<ColorPropertyRange>();
		string[] names = Enum.GetNames(typeof(ShaderFloat));
		foreach (string value in names)
		{
			ShaderFloat shaderFloat = (ShaderFloat)Enum.Parse(typeof(ShaderFloat), value);
			if (firstMat.HasProperty(shaderFloat.GetString()))
			{
				FloatPropertyRange floatPropertyRange = new FloatPropertyRange();
				floatPropertyRange.Property = shaderFloat;
				floatPropertyRange.ValueMain = firstMat.GetFloat(shaderFloat.GetString());
				floatPropertyRange.ValueSecond = secondMat.GetFloat(shaderFloat.GetString());
				list.Add(floatPropertyRange);
			}
		}
		names = Enum.GetNames(typeof(ShaderColor));
		foreach (string value2 in names)
		{
			ShaderColor shaderColor = (ShaderColor)Enum.Parse(typeof(ShaderColor), value2);
			if (firstMat.HasProperty(shaderColor.GetString()))
			{
				ColorPropertyRange colorPropertyRange = new ColorPropertyRange();
				colorPropertyRange.Property = shaderColor;
				colorPropertyRange.ValueMain = firstMat.GetColor(shaderColor.GetString());
				colorPropertyRange.ValueSecond = secondMat.GetColor(shaderColor.GetString());
				list2.Add(colorPropertyRange);
			}
		}
		FloatPropertyValues = list.ToArray();
		ColorPropertyValues = list2.ToArray();
	}

	private void SetKeywords(Material sourceMaterial)
	{
		string[] names = Enum.GetNames(typeof(ShaderFeature));
		foreach (string value in names)
		{
			ShaderFeature slot = (ShaderFeature)Enum.Parse(typeof(ShaderFeature), value);
			if (sourceMaterial.IsKeywordEnabled(slot.GetString()))
			{
				_mainMaterial.EnableKeyword(slot.GetString());
			}
		}
	}
}
