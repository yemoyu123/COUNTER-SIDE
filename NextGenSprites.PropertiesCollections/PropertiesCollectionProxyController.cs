using System;
using System.Collections;
using UnityEngine;

namespace NextGenSprites.PropertiesCollections;

[AddComponentMenu("NextGenSprites/Properties Collection/Remote/Controller - Receiver")]
[RequireComponent(typeof(SpriteRenderer))]
[HelpURL("http://wiki.next-gen-sprites.com/doku.php?id=scripting:propertiescollection#controller")]
public class PropertiesCollectionProxyController : MonoBehaviour
{
	public PropertiesCollectionProxyManager CollectionManager;

	public bool FindManagerByReference;

	public string ManagerReferenceId;

	private SpriteRenderer _spriteRenderer;

	private string _lastId = "";

	private void Start()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		if (!(CollectionManager == null))
		{
			return;
		}
		if (FindManagerByReference)
		{
			PropertiesCollectionProxyManager[] array = UnityEngine.Object.FindObjectsOfType<PropertiesCollectionProxyManager>();
			foreach (PropertiesCollectionProxyManager propertiesCollectionProxyManager in array)
			{
				if (string.CompareOrdinal(ManagerReferenceId, propertiesCollectionProxyManager.ReferenceId) == 0)
				{
					CollectionManager = propertiesCollectionProxyManager;
					break;
				}
			}
			if (CollectionManager == null)
			{
				Debug.LogError($"Could not find an Manager with the Id: {ManagerReferenceId}");
			}
		}
		else
		{
			Debug.LogError("There is no Manager assigned!");
		}
	}

	public void UpdateMaterial(string CollectionName)
	{
		if (string.CompareOrdinal(CollectionName, _lastId) != 0)
		{
			if (CollectionManager._cachedMaterials.Count < 1)
			{
				CollectionManager.InitManager();
			}
			if (CollectionManager._cachedMaterials.ContainsKey(CollectionName))
			{
				_spriteRenderer.material = CollectionManager._cachedMaterials[CollectionName];
			}
			_lastId = CollectionName;
		}
	}

	public void UpdateMaterialSmooth(string CollectionName, float LerpDuration = 1f)
	{
		if (string.CompareOrdinal(CollectionName, _lastId) != 0)
		{
			if (CollectionManager._cachedMaterials.Count < 1)
			{
				CollectionManager.InitManager();
			}
			if (CollectionManager._cachedMaterials.ContainsKey(CollectionName))
			{
				Material material = _spriteRenderer.material;
				Material target = CollectionManager._cachedMaterials[CollectionName];
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
			if (CollectionManager._cachedMaterials.Count < 1)
			{
				CollectionManager.InitManager();
			}
			if (CollectionManager._cachedMaterials.ContainsKey(text))
			{
				Material material = _spriteRenderer.material;
				Material target = CollectionManager._cachedMaterials[text];
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
