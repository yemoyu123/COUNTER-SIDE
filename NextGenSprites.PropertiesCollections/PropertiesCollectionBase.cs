using System.Collections.Generic;
using UnityEngine;

namespace NextGenSprites.PropertiesCollections;

public class PropertiesCollectionBase : MonoBehaviour
{
	public PropertiesCollection[] PropCollections;

	public Dictionary<string, Material> _cachedMaterials = new Dictionary<string, Material>();

	private void SetProperties(Dictionary<string, PropertiesCollection> PropCollection, string CollectionName, Material Target)
	{
		if (PropCollection.ContainsKey(CollectionName))
		{
			PropertiesCollection.TextureTargets[] textures = PropCollection[CollectionName].Textures;
			foreach (PropertiesCollection.TextureTargets textureTargets in textures)
			{
				Target.SetTexture(textureTargets.Target.GetString(), textureTargets.Value);
			}
			PropertiesCollection.FloatTargets[] floats = PropCollection[CollectionName].Floats;
			foreach (PropertiesCollection.FloatTargets floatTargets in floats)
			{
				Target.SetFloat(floatTargets.Target.GetString(), floatTargets.Value);
			}
			PropertiesCollection.TintTargets[] tints = PropCollection[CollectionName].Tints;
			foreach (PropertiesCollection.TintTargets tintTargets in tints)
			{
				Target.SetColor(tintTargets.Target.GetString(), tintTargets.Value);
			}
			PropertiesCollection.FeatureTargets[] features = PropCollection[CollectionName].Features;
			foreach (PropertiesCollection.FeatureTargets featureTargets in features)
			{
				if (featureTargets.Value)
				{
					Target.EnableKeyword(featureTargets.Target.GetString());
				}
				else
				{
					Target.DisableKeyword(featureTargets.Target.GetString());
				}
			}
		}
		else
		{
			Debug.LogError("There is no matching Id on this Collection");
		}
	}

	protected void InitMaterialCache(Material source, Dictionary<string, Material> target)
	{
		Dictionary<string, PropertiesCollection> dictionary = new Dictionary<string, PropertiesCollection>();
		PropertiesCollection[] propCollections = PropCollections;
		foreach (PropertiesCollection propertiesCollection in propCollections)
		{
			dictionary.Add(propertiesCollection.CollectionName, propertiesCollection);
		}
		foreach (KeyValuePair<string, PropertiesCollection> item in dictionary)
		{
			Material material = new Material(source);
			material.name = $"{material.name} - [{item.Key}]";
			SetProperties(dictionary, item.Key, material);
			target.Add(item.Key, material);
		}
	}
}
