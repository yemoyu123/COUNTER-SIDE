using UnityEngine;

namespace NextGenSprites.PropertiesCollections;

[AddComponentMenu("NextGenSprites/Properties Collection/Remote/Manager - Host")]
[HelpURL("http://wiki.next-gen-sprites.com/doku.php?id=scripting:propertiescollection#manager")]
public class PropertiesCollectionProxyManager : PropertiesCollectionBase
{
	public string ReferenceId = "GIVE ME A NAME";

	public bool TargetThis = true;

	public SpriteRenderer SourceObject;

	private void Start()
	{
		InitManager();
	}

	public void InitManager()
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
		if (TargetThis)
		{
			SourceObject = GetComponent<SpriteRenderer>();
		}
		else if (SourceObject == null)
		{
			Debug.LogError("There is no Target Object assigned!");
			return;
		}
		InitMaterialCache(GetComponent<SpriteRenderer>().sharedMaterial, _cachedMaterials);
	}
}
