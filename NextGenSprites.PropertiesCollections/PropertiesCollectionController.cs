using System;
using System.Collections;
using UnityEngine;

namespace NextGenSprites.PropertiesCollections;

[AddComponentMenu("NextGenSprites/Properties Collection/Controller")]
[HelpURL("http://wiki.next-gen-sprites.com/doku.php?id=scripting:propertiescollection#managersolo")]
[RequireComponent(typeof(SpriteRenderer))]
public class PropertiesCollectionController : PropertiesCollectionBase
{
	private string _lastId;

	private SpriteRenderer _spriteRenderer;

	private void Awake()
	{
		InitManager();
	}

	private void InitManager()
	{
		if (PropCollections.Length == 0)
		{
			Debug.LogError("There are no Properties Collections assigned!");
			return;
		}
		for (int i = 0; i < PropCollections.Length; i++)
		{
			if (PropCollections[i] == null)
			{
				Debug.LogError($"No Properties Collection assigned at Element {i}!");
				return;
			}
		}
		_spriteRenderer = GetComponent<SpriteRenderer>();
		InitMaterialCache(_spriteRenderer.sharedMaterial, _cachedMaterials);
	}

	public void UpdateMaterial(string CollectionName)
	{
		if (string.CompareOrdinal(CollectionName, _lastId) != 0)
		{
			if (_cachedMaterials.Count < 1)
			{
				InitManager();
			}
			if (_cachedMaterials.ContainsKey(CollectionName))
			{
				_spriteRenderer.material = _cachedMaterials[CollectionName];
			}
			_lastId = CollectionName;
		}
	}

	public void UpdateMaterialSmooth(string CollectionName, float LerpDuration = 1f)
	{
		if (string.CompareOrdinal(CollectionName, _lastId) != 0)
		{
			if (_cachedMaterials.Count < 1)
			{
				InitManager();
			}
			if (_cachedMaterials.ContainsKey(CollectionName))
			{
				Material material = _spriteRenderer.material;
				Material target = _cachedMaterials[CollectionName];
				StartCoroutine(SmoothMaterialLerp(material, target, LerpDuration));
			}
			_lastId = CollectionName;
		}
	}

	public void UpdateMaterialSmooth(string Arguments)
	{
		string[] array = Arguments.Split(new char[2] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length <= 1)
		{
			return;
		}
		float.TryParse(array[1], out var result);
		if (result == 0f)
		{
			Debug.LogWarning("Invalid Time argument. Check spelling?");
			return;
		}
		string text = array[0];
		float duration = result;
		if (string.CompareOrdinal(text, _lastId) != 0)
		{
			if (_cachedMaterials.Count < 1)
			{
				InitManager();
			}
			if (_cachedMaterials.ContainsKey(text))
			{
				Material material = _spriteRenderer.material;
				Material target = _cachedMaterials[text];
				StartCoroutine(SmoothMaterialLerp(material, target, duration));
			}
			_lastId = text;
		}
	}

	private IEnumerator SmoothMaterialLerp(Material origin, Material target, float duration)
	{
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			_spriteRenderer.material.Lerp(origin, target, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}
}
