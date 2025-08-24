using System;
using UnityEngine;

namespace NextGenSprites.PropertiesCollections;

[Serializable]
[HelpURL("http://wiki.next-gen-sprites.com/doku.php?id=scripting:propertiescollection")]
public class PropertiesCollection : ScriptableObject
{
	[Serializable]
	public class TextureTargets
	{
		public ShaderTexture Target;

		public Texture Value;
	}

	[Serializable]
	public class FloatTargets
	{
		public ShaderFloat Target;

		public float Value;
	}

	[Serializable]
	public class Vector4Targets
	{
		public ShaderVector4 Target;

		public Vector4 Value;
	}

	[Serializable]
	public class TintTargets
	{
		public ShaderColor Target;

		public Color Value;
	}

	[Serializable]
	public class FeatureTargets
	{
		public ShaderFeatureRuntime Target;

		public bool Value;
	}

	public string CollectionName;

	public TextureTargets[] Textures;

	public FloatTargets[] Floats;

	public Vector4Targets[] Vector4s;

	public TintTargets[] Tints;

	public FeatureTargets[] Features;
}
